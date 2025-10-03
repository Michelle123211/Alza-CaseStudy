using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using EshopAPI.DTOs;
using EshopAPI.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text;


namespace EshopAPI.Endpoints.v1;

/// <summary>
/// A static class handling all endpoints starting with "/api/v1/products".
/// </summary>
public static class ProductsEndpointsV1 {

	/// <summary>
	/// Extension method registering a group of "/products" endpoints to the given <c>IEndpointRouteBuilder</c>.
	/// </summary>
	/// <param name="routeBuilder">The route's prefix to which the endpoints are to be added.</param>
	public static void RegisterProductsEndpointsV1(this IEndpointRouteBuilder routeBuilder) {
		var productsGroup = routeBuilder.MapGroup("/products")
			.WithOpenApi()
			.WithTags("Products");

		productsGroup.MapGet("/", GetProducts)
			.MapToApiVersion(1)
			.WithName(nameof(GetProducts))
			.WithSummary("Lists all available products.")
			.WithDescription("Gets all available products. Optional query parameter can specify to get only products currently in stock.");

		productsGroup.MapGet("/{id:int}", GetProductById)
			.WithName(nameof(GetProductById))
			.WithSummary("Gets a single product by ID.")
			.WithDescription("Gets details of the product with the given ID.");

		productsGroup.MapPost("/", CreateProduct)
			.WithName(nameof(CreateProduct))
			.WithSummary("Creates a new product.")
			.WithDescription("Creates a new product with the details given in the request body.");

		productsGroup.MapPatch("/{id:int}", UpdateProduct)
			.WithName(nameof(UpdateProduct))
			.WithSummary("Updates a single product by ID.")
			.WithDescription("(Partially) updates the product of the given ID with the details given in the request body.");

		productsGroup.MapPut("/{id:int}/quantity", UpdateProductQuantity)
			.MapToApiVersion(1)
			.WithName(nameof(UpdateProductQuantity))
			.WithSummary("Updates the product stock.")
			.WithDescription("Updates quantity of the product with the given ID by the given delta.");
	}


	/// <summary>
	/// Gets a list of all products (optionally only those in stock).
	/// </summary>
	/// <param name="db">Products repository.</param>
	/// <param name="inStock">Optional parameter for filtering only products currently in stock (if <c>true</c>).</param>
	/// <returns><c>Ok</c> code with the list of products in the response body.</returns>
	internal static async Task<Ok<List<ProductDto>>> GetProducts(
			[FromServices] IProductsRepository db,
			[FromQuery(Name = "in_stock")] bool inStock = false) {

		// Get all (available) products from the database, map to ProductDto
		List<Product> products;
		if (!inStock) products = await db.GetProductsAsync();
		else products = await db.GetProductsInStockAsync();
		List<ProductDto> productDtos = new();
		foreach (var product in products) productDtos.Add(product.ToProductDto());

		return TypedResults.Ok(productDtos);
	}

	/// <summary>
	/// Gets a product based on its ID.
	/// </summary>
	/// <param name="db">Products repository.</param>
	/// <param name="id">Unique identifier of the product to be found.</param>
	/// <returns><c>Ok</c> code with the product in the response body, or <c>NotFound</c> code with explanation in the response body.</returns>
	internal static async Task<Results<Ok<ProductDto>, NotFound<string>>> GetProductById(
			[FromServices] IProductsRepository db,
			[FromRoute] int id) {

		// Get a product with the given ID, map to ProductDto
		return await db.GetProductByIdAsync(id)
			is Product product
				? TypedResults.Ok(product.ToProductDto())
				: TypedResults.NotFound(ErrorMsg.NOT_FOUND);
	}

	/// <summary>
	/// Creates a new product with the given details.
	/// </summary>
	/// <param name="db">Products repository.</param>
	/// <param name="product">Details of the product to be created (name and image URL are required).</param>
	/// <returns><c>Created</c> code, the newly created product's URL in the Location header and the product itself in the response body.</returns>
	internal static async Task<Results<CreatedAtRoute<ProductDto>, BadRequest<string>>> CreateProduct(
			[FromServices] IProductsRepository db,
			[FromBody] ProductCreateDto product) {

		// Validate provided parameters (required (not null, not empty), non-negative)
		StringBuilder errors = new StringBuilder()
			.Append(Validate.Name(product.Name))
			.Append(Validate.MainImageUrl(product.MainImageUrl))
			.Append(Validate.Price(product.Price))
			.Append(Validate.Quantity(product.Quantity));
		if (errors.Length > 0) return TypedResults.BadRequest(errors.ToString());
		
		// Create a Product instance (map to Product) and add it to the database
		Product newProduct = await db.CreateProductAsync(product.ToProduct());
		await db.SaveChangesAsync();
		
		// Return result mapped to ProductDto
		ProductDto newProductDto = newProduct.ToProductDto();
		return TypedResults.CreatedAtRoute(newProductDto, nameof(GetProductById), new { id = newProductDto.Id });
	}

	/// <summary>
	/// Updates details of a product with the given ID.
	/// </summary>
	/// <param name="db">Products repository.</param>
	/// <param name="id">Unique identifier of the product to be updated.</param>
	/// <param name="productUpdate">Product details which should be overwritten.</param>
	/// <returns><c>Ok</c> code with the updated product in the response body, or <c>NotFound</c>/<c>BadRequest</c> code with explanation in the response body.</returns>
	internal static async Task<Results<Ok<ProductDto>, NotFound<string>, BadRequest<string>>> UpdateProduct(
			[FromServices] IProductsRepository db,
			[FromRoute] int id,
			[FromBody] ProductUpdateDto productUpdate) {

		// Get a product with the given ID
		Product? origProduct = await db.GetProductByIdAsync(id);
		if (origProduct is null) return TypedResults.NotFound(ErrorMsg.NOT_FOUND);

		// Validate provided parameters (non-negative, required (either null or something, not empty))
		StringBuilder errors = new StringBuilder()
			.Append(Validate.Name(productUpdate.Name, true))
			.Append(Validate.MainImageUrl(productUpdate.MainImageUrl, true))
			.Append(Validate.Price(productUpdate.Price, true))
			.Append(Validate.Quantity(productUpdate.Quantity, true));
		if (errors.Length > 0) return TypedResults.BadRequest(errors.ToString());

		// Update information based on provided parameters (map ProductUpdateDto to Product, only non-nulls considered)
		origProduct.CombineWith(productUpdate);
		Product newProduct = await db.UpdateProductAsync(origProduct);
		await db.SaveChangesAsync();

		// Return result mapped to ProductDto
		return TypedResults.Ok(newProduct.ToProductDto());
	}

	/// <summary>
	/// Updates a quantity of a product with the given ID.
	/// </summary>
	/// <param name="db">Products repository.</param>
	/// <param name="id">Unique identifier of the product to be updated.</param>
	/// <param name="quantityDelta">The change in the quantity value (positive to increase stock, negative to decrease).</param>
	/// <returns><c>Ok</c> code with the updated product in the response body, or <c>NotFound</c>/<c>BadRequest</c> code with explanation in the response body.</returns>
	internal static async Task<Results<Ok<ProductDto>, NotFound<string>, BadRequest<string>>> UpdateProductQuantity(
			[FromServices] IProductsRepository db,
			[FromRoute] int id,
			[FromQuery(Name = "quantity_delta")] int quantityDelta) {

		// Get a product with the given ID
		Product? origProduct = await db.GetProductByIdAsync(id);
		if (origProduct is null) return TypedResults.NotFound(ErrorMsg.NOT_FOUND);

		// Validate provided parameter
		StringBuilder errors = new StringBuilder()
			.Append(Validate.Quantity(origProduct.Quantity, quantityDelta));
		if (errors.Length > 0) return TypedResults.BadRequest(errors.ToString());

		// Update quantity with the given delta
		Product? product = await db.UpdateProductQuantityAsync(id, quantityDelta);
		await db.SaveChangesAsync();

		// Return result mapped to ProductDto
		return product is null
			? TypedResults.NotFound(ErrorMsg.NOT_FOUND)
			: TypedResults.Ok(product.ToProductDto());
	}

}

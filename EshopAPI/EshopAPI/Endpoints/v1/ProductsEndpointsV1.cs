using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using EshopAPI.DTOs;
using EshopAPI.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;


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

	// Gets a list of all products (optionally only those in stock).
	internal static async Task<Ok<List<ProductDto>>> GetProducts(
			[FromServices] IProductsRepository db,
			[FromQuery(Name = "in_stock")] bool inStock = false) {

		// TODO: Get all (available) products from the database, map to ProductDto
		return TypedResults.Ok(new List<ProductDto>());
	}

	// Gets a product based on its ID.
	internal static async Task<Results<Ok<ProductDto>, NotFound<string>>> GetProductById(
			[FromServices] IProductsRepository db,
			[FromRoute] int id) {

		// TODO: Get a product with the given ID, map to ProductDto
		return TypedResults.NotFound("Product with the given ID doesn't exist.");
	}

	// Creates a new product with the given details.
	internal static async Task<Results<CreatedAtRoute<ProductDto>, BadRequest<string>>> CreateProduct(
			[FromServices] IProductsRepository db,
			[FromBody] ProductCreateDto product) {

		// TODO: Validate provided parameters (required (not null, not empty), non-negative)
		// TODO: Create a Product instance (map to Product)
		// TODO: Add the new product to the database
		// TODO: Return result mapped to ProductDto
		return TypedResults.BadRequest("The provided properties are invalid.");
	}

	// Updates details of a product with the given ID.
	internal static async Task<Results<Ok<ProductDto>, NotFound<string>, BadRequest<string>>> UpdateProduct(
			[FromServices] IProductsRepository db,
			[FromRoute] int id,
			[FromBody] ProductUpdateDto productUpdate) {

		// TODO: Validate provided parameters (non-negative, required (either null or something, not empty))
		// TODO: Get a product with the given ID
		// TODO: Update information based on provided parameters (map ProductUpdateDto to Product, only non-nulls considered)
		// TODO: Return result mapped to ProductDto
		return TypedResults.NotFound("Product with the given ID doesn't exist.");
	}

	// Updates a quantity of a product with the given ID.
	internal static async Task<Results<Ok<ProductDto>, NotFound<string>, BadRequest<string>>> UpdateProductQuantity(
			[FromServices] IProductsRepository db,
			[FromRoute] int id,
			[FromQuery(Name = "quantity_delta")] int quantityDelta) {

		// TODO: Get a product with the given ID
		// TODO: Validate provided parameter
		// TODO: Update quantity with the given delta
		// TODO: Return result mapped to ProductDto
		return TypedResults.NotFound("Product with the given ID doesn't exist.");
	}

}

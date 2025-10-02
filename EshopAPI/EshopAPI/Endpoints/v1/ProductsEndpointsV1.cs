using Microsoft.AspNetCore.Mvc;
using EshopAPI.DTOs;
using EshopAPI.Entities;


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
	internal static async Task<IResult> GetProducts(
			[FromServices] IProductsRepository db,
			[FromQuery(Name = "in_stock")] bool inStock = false) {
		return TypedResults.Forbid();
	}

	// Gets a product based on its ID.
	internal static async Task<IResult> GetProductById(
			[FromServices] IProductsRepository db,
			[FromRoute] int id) {
		return TypedResults.Forbid();
	}

	// Creates a new product with the given details.
	internal static async Task<IResult> CreateProduct(
			[FromServices] IProductsRepository db,
			[FromBody] ProductCreateDto product) {
		return TypedResults.Forbid();
	}

	// Updates details of a product with the given ID.
	internal static async Task<IResult> UpdateProduct(
			[FromServices] IProductsRepository db,
			[FromRoute] int id,
			[FromBody] ProductUpdateDto productUpdate) {
		return TypedResults.Forbid();
	}

	// Updates a quantity of a product with the given ID.
	internal static async Task<IResult> UpdateProductQuantity(
			[FromServices] IProductsRepository db,
			[FromRoute] int id,
			[FromQuery(Name = "quantity_delta")] int quantityDelta) {
		return TypedResults.Forbid();
	}

}

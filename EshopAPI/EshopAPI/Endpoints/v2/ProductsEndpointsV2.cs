using EshopAPI.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EshopAPI.Endpoints.v2;

/// <summary>
/// A static class handling all endpoints starting with "/api/v2/products".
/// </summary>
public static class ProductsEndpointsV2 {

	/// <summary>
	/// Extension method registering a group of "/products" endpoints to the given <c>IEndpointRouteBuilder</c>.
	/// </summary>
	/// <param name="routeBuilder">The route's prefix to which the endpoints are to be added.</param>
	public static void RegisterProductsEndpointsV2(this IEndpointRouteBuilder routeBuilder) {
		var productsGroup = routeBuilder.MapGroup("/products")
			.WithOpenApi()
			.WithTags("Products");

		productsGroup.MapGet("/", GetProductsPaged)
			.MapToApiVersion(2)
			.WithName(nameof(GetProductsPaged))
			.WithSummary("Gets a page of available products.")
			.WithDescription("Gets a single page from a list of all available products. Optional query parameter can specify to get only products currently in stock.");

		productsGroup.MapPatch("/{id:int}", EnqueueProductUpdate)
			.MapToApiVersion(2)
			.WithName(nameof(EnqueueProductUpdate))
			.WithSummary("Updates a single product by ID (with an asynchronous queue).")
			.WithDescription("(Partially) updates the product of the given ID with the details given in the request body (by enqueuing the request into an asynchronous queue).");

		productsGroup.MapPut("/{id:int}/quantity", EnqueueProductQuantityUpdate)
			.MapToApiVersion(2)
			.WithName(nameof(EnqueueProductQuantityUpdate))
			.WithSummary("Updates the product stock (with an asynchronous queue).")
			.WithDescription("Updates quantity of the product with the given ID by the given delta (by enqueuing the request into an asynchronous queue).");
	}

	// Gets a single page listing available products (optionally only those in stock).
	internal static async Task<IResult> GetProductsPaged(
			[FromServices] IProductsRepository db,
			[FromQuery(Name = "in_stock")] bool inStock = false,
			[FromQuery(Name = "page")] int page = 0,
			[FromQuery(Name = "page_size")] int pageSize = 10) {
		return TypedResults.Forbid();
	}

	// Updates details of a product with the given ID (by enqueuing the request into an asynchronous queue).
	internal static async Task<IResult> EnqueueProductUpdate(
			[FromServices] IProductsRepository db,
			[FromRoute] int id) { // TODO: ProductUpdateDto
		return TypedResults.Forbid();
	}

	// Updates a quantity of a product with the given ID (by enqueuing the request into an asynchronous queue).
	internal static async Task<IResult> EnqueueProductQuantityUpdate(
			[FromServices] IProductsRepository db,
			[FromRoute] int id,
			[FromQuery(Name = "quantity_delta")] int quantityDelta) {
		return TypedResults.Forbid();
	}

}

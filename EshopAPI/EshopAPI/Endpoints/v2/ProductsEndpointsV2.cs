using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using EshopAPI.Data;
using EshopAPI.DTOs;
using EshopAPI.Entities;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Http.Extensions;

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

		productsGroup.MapGet("/", GetProductsPage)
			.MapToApiVersion(2)
			.WithName(nameof(GetProductsPage))
			.WithSummary("Gets a page of available products.")
			.WithDescription("Gets a single page from a list of all available products. Optional query parameter can specify to get only products currently in stock. Page parameter is 0-indexed.");

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
	internal static async Task<Results<Ok<PagedResponseDto<ProductDto>>, BadRequest<string>>> GetProductsPage(
			HttpContext httpContext,
			LinkGenerator links,
			[FromServices] IProductsRepository db,
			[FromQuery(Name = "in_stock")] bool inStock = false,
			[FromQuery(Name = "page")] int page = 0,
			[FromQuery(Name = "page_size")] int pageSize = 10) {

		// Validate provided parameters (positive/non-negative)
		StringBuilder errors = new StringBuilder()
			.Append(Validate.Page(page))
			.Append(Validate.PageSize(pageSize));
		if (errors.Length > 0) return TypedResults.BadRequest(errors.ToString());

		// Get corresponding (available) products from the database, map to ProductDto and PagedResponseDto
		PagedResponse<Product> products;
		if (!inStock) products = await db.GetProductsPageAsync(page, pageSize);
		else products = await db.GetProductsInStockPageAsync(page, pageSize);
		PagedResponseDto<ProductDto> productDtos = products.ToPagedResposeProductDto();

		// Set custom headers
		httpContext.Response.Headers[Header.PAGE] = productDtos.PageNumber.ToString();
		httpContext.Response.Headers[Header.PAGE_SIZE] = productDtos.PageSize.ToString();
		httpContext.Response.Headers[Header.TOTAL_ITEMS] = productDtos.TotalItems.ToString();
		httpContext.Response.Headers[Header.TOTAL_PAGES] = productDtos.TotalPages.ToString();

		// Set Link header
		string GetLink(int page, string name) =>
			$"<{links.GetUriByName(httpContext, nameof(GetProductsPage), new { page, pageSize })}>; rel=\"{name}\"";
		List<string> linkParts = new();
		linkParts.Add(GetLink(page, "self"));
		linkParts.Add(GetLink(0, "first"));
		if (page > 0) linkParts.Add(GetLink(page - 1, "prev"));
		if (page < productDtos.TotalPages - 1) linkParts.Add(GetLink(page + 1, "next"));
		linkParts.Add(GetLink(productDtos.TotalPages - 1, "last"));
		httpContext.Response.Headers.Link = string.Join(", ", linkParts);


		return TypedResults.Ok<PagedResponseDto<ProductDto>>(productDtos);
	}

	// Updates details of a product with the given ID (by enqueuing the request into an asynchronous queue).
	internal static async Task<IResult> EnqueueProductUpdate(
			[FromServices] IProductsRepository db,
			[FromBody] ProductUpdateDto productUpdate) {

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

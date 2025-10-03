using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Text;
using EshopAPI.Data;
using EshopAPI.DTOs;
using EshopAPI.Entities;
using EshopAPI.Infrastructure;

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

		productsGroup.MapGet("/{id}/status", GetStatus)
			.MapToApiVersion(2)
			.WithName(nameof(GetStatus))
			.WithSummary("Get status of an update request.")
			.WithDescription("Gets status of the update request with the given ID, which was added to an asynchronous queue earlier.");
	}

	/// <summary>
	/// Gets a single page listing available products (optionally only those in stock).
	/// </summary>
	/// <param name="httpContext">HTTP context allowing to set response headers.</param>
	/// <param name="links">Link generator used to create links for additional pages.</param>
	/// <param name="db">Products repository.</param>
	/// <param name="inStock">Optional parameter for filtering only products currently in stock (if <c>true</c>).</param>
	/// <param name="page">Requested page (0 for the first one).</param>
	/// <param name="pageSize">Maximum number of items on a single page.</param>
	/// <returns><c>Ok</c> code with the page of products in the response body, or <c>BadRequest</c> code with explanation in the response body.</returns>
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

	/// <summary>
	/// Updates details of a product with the given ID (by enqueuing the request into an asynchronous queue).
	/// </summary>
	/// <param name="productUpdateQueue">Asynchronous queue for update requests processing.</param>
	/// <param name="statusDict">Dictionary containing status for recently enqueued requests.</param>
	/// <param name="db">Products repository.</param>
	/// <param name="id">Unique identifier of the product to be updated.</param>
	/// <param name="productUpdate">Product details which should be overwritten.</param>
	/// <returns><c>Accepted</c> code and URL for tracking the request's status in the Location header.</returns>
	internal static async Task<AcceptedAtRoute> EnqueueProductUpdate(
			[FromServices] IProductUpdateQueue productUpdateQueue,
			[FromServices] ConcurrentDictionary<string, QueuedUpdateResult> statusDict,
			[FromServices] IProductsRepository db,
			[FromRoute] int id,
			[FromBody] ProductUpdateDto productUpdate) {

		// Enqueue the product update
		var updateId = Guid.NewGuid().ToString();
		var update = new QueuedProductUpdate(updateId, id, productUpdate);
		await productUpdateQueue.EnqueueUpdateAsync(update);

		// Initialize status
		statusDict[updateId] = new QueuedUpdateResult(QueuedUpdateStatus.Queued, string.Empty);

		return TypedResults.AcceptedAtRoute(nameof(GetStatus), new { id = updateId });
	}

	/// <summary>
	/// Updates a quantity of a product with the given ID (by enqueuing the request into an asynchronous queue).
	/// </summary>
	/// <param name="productUpdateQueue">Asynchronous queue for update requests processing.</param>
	/// <param name="statusDict">Dictionary containing status for recently enqueued requests.</param>
	/// <param name="db">Products repository.</param>
	/// <param name="id">Unique identifier of the product to be updated.</param>
	/// <param name="quantityDelta">The change in the quantity value (positive to increase stock, negative to decrease).</param>
	/// <returns><c>Accepted</c> code and URL for tracking the request's status in the Location header.</returns>
	internal static async Task<AcceptedAtRoute> EnqueueProductQuantityUpdate(
			[FromServices] IProductUpdateQueue productUpdateQueue,
			[FromServices] ConcurrentDictionary<string, QueuedUpdateResult> statusDict,
			[FromServices] IProductsRepository db,
			[FromRoute] int id,
			[FromQuery(Name = "quantity_delta")] int quantityDelta) {

		// Enqueue the product update
		var updateId = Guid.NewGuid().ToString();
		var update = new QueuedQuantityUpdate(updateId, id, quantityDelta);
		await productUpdateQueue.EnqueueUpdateAsync(update);

		// Initialize status
		statusDict[updateId] = new QueuedUpdateResult(QueuedUpdateStatus.Queued, string.Empty);

		return TypedResults.AcceptedAtRoute(nameof(GetStatus), new { id = updateId });
	}

	/// <summary>
	/// Gets status of an update request with the given ID (update requests are enqueued in an asynchronous queue for processing).
	/// </summary>
	/// <param name="statusDict">Dictionary containing status for recently enqueued requests.</param>
	/// <param name="id">Unique identifier of the update request.</param>
	/// <returns><c>Ok</c> code with the current status (eventually with error messages).</returns>
	internal static async Task<Results<Ok<UpdateStatusDto>, NotFound<string>>> GetStatus(
			[FromServices] ConcurrentDictionary<string, QueuedUpdateResult> statusDict,
			[FromRoute] string id) {

		// Get a status with the given ID
		if (!statusDict.TryGetValue(id, out var result))
			return TypedResults.NotFound<string>(ErrorMsg.STATUS_NOT_FOUND);

		// Make sure to delete completed statuses (once they were polled)
		if (result.Status == QueuedUpdateStatus.Completed || result.Status == QueuedUpdateStatus.Failed)
			statusDict.Remove(id, out var _);

		return TypedResults.Ok(new UpdateStatusDto(result));
	}

}

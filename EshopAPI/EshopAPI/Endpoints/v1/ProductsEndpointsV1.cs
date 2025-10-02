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

		productsGroup.MapGet("/", () => "").MapToApiVersion(1);
	}

}

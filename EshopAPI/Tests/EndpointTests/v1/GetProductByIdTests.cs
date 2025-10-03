using Microsoft.AspNetCore.Http.HttpResults;
using EshopAPI.DTOs;
using EshopAPI.Endpoints.v1;
using Tests.Mocks;

namespace Tests.EndpointTests.v1;


/// <summary>
/// A set of unit tests targeting "GET /api/v1/products/{id}" endpoint.
/// </summary>
public class GetProductByIdTests {

	private readonly ProductsRepositoryMock productsMock = new();

	[Fact]
	public async Task When_ProductDoesNotExist_Then_ReturnNotFound() {
		// Arrange
		// Act
		var act = await ProductsEndpointsV1.GetProductById(productsMock, -5);
		// Assert
		// ... type of result
		Assert.IsType<NotFound<string>>(act.Result);
		// ... status code
		var result = act.Result as NotFound<string>;
		Assert.NotNull(result);
		Assert.Equal(404, result.StatusCode);
	}

	[Fact]
	public async Task When_ProductExists_Then_ReturnOk() {
		// Arrange
		// Act
		var act = await ProductsEndpointsV1.GetProductById(productsMock, 1);
		// Assert
		// ... type of result
		Assert.IsType<Ok<ProductDto>>(act.Result);
		// ... status code
		var result = act.Result as Ok<ProductDto>;
		Assert.NotNull(result);
		Assert.Equal(200, result.StatusCode);
	}

	[Fact]
	public async Task When_ProductExists_Then_ReturnProduct() {
		// Arrange
		int id = 1;
		// Act
		var act = await ProductsEndpointsV1.GetProductById(productsMock, 1);
		// Assert
		// ... output data
		var result = act.Result as Ok<ProductDto>;
		Assert.NotNull(result);
		Assert.NotNull(result.Value);
		Assert.IsType<ProductDto>(result.Value);
		Assert.Equal(id, result.Value.Id);
	}

	[Fact]
	public async Task When_ProductExists_Then_GetProduct() {
		// Arrange
		int getCount = productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.GetProductByIdAsync), 0);
		// Act
		var act = await ProductsEndpointsV1.GetProductById(productsMock, 1);
		// Assert
		// ... method calls
		Assert.Equal(getCount + 1, productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.GetProductByIdAsync), 0));
	}
}

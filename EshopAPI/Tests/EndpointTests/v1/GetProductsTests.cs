using Microsoft.AspNetCore.Http.HttpResults;
using EshopAPI.DTOs;
using EshopAPI.Endpoints.v1;
using Tests.Mocks;

namespace Tests.EndpointTests.v1;


/// <summary>
/// A set of unit tests targeting "GET /api/v1/products" endpoint.
/// </summary>
public class GetProductsTests {

	private readonly ProductsRepositoryMock productsMock = new();

	[Fact]
	public async Task When_ValidRequestAll_Then_ReturnOk() {
		// Arrange
		// Act
		var act = await ProductsEndpointsV1.GetProducts(productsMock);
		// Assert
		// ... type of result
		Assert.IsType<Ok<List<ProductDto>>>(act);
		// ... status code
		Assert.NotNull(act);
		Assert.Equal(200, act.StatusCode);
	}

	[Fact]
	public async Task When_ValidRequestAll_Then_ReturnList() {
		// Arrange
		// Act
		var act = await ProductsEndpointsV1.GetProducts(productsMock);
		// Assert
		// ... output data
		Assert.NotNull(act);
		Assert.NotNull(act.Value);
		Assert.IsType<List<ProductDto>>(act.Value);
	}

	[Fact]
	public async Task When_ValidRequestAll_Then_GetProducts() {
		// Arrange
		int getCount = productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.GetProductsAsync), 0);
		// Act
		var act = await ProductsEndpointsV1.GetProducts(productsMock);
		// Assert
		// ... method calls
		Assert.Equal(getCount + 1, productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.GetProductsAsync), 0));
	}

	[Fact]
	public async Task When_ValidRequestInStock_Then_ReturnOk() {
		// Arrange
		// Act
		var act = await ProductsEndpointsV1.GetProducts(productsMock, true);
		// Assert
		// ... type of result
		Assert.IsType<Ok<List<ProductDto>>>(act);
		// ... status code
		Assert.NotNull(act);
		Assert.Equal(200, act.StatusCode);
	}

	[Fact]
	public async Task When_ValidRequestInStock_Then_ReturnList() {
		// Arrange
		// Act
		var act = await ProductsEndpointsV1.GetProducts(productsMock, true);
		// Assert
		// ... output data
		Assert.NotNull(act);
		Assert.NotNull(act.Value);
		Assert.IsType<List<ProductDto>>(act.Value);
	}

	[Fact]
	public async Task When_ValidRequestInStock_Then_GetProductsInStock() {
		// Arrange
		int getCount = productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.GetProductsInStockAsync), 0);
		// Act
		var act = await ProductsEndpointsV1.GetProducts(productsMock, true);
		// Assert
		// ... method calls
		Assert.Equal(getCount + 1, productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.GetProductsInStockAsync), 0));
	}

}

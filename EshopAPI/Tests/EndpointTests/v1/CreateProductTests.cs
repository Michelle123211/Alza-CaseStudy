using Microsoft.AspNetCore.Http.HttpResults;
using EshopAPI.DTOs;
using EshopAPI.Endpoints.v1;
using Tests.Mocks;

namespace Tests.EndpointTests.v1;


/// <summary>
/// A set of unit tests targeting "POST /api/v1/products" endpoint.
/// </summary>
public class CreateProductTests {

	private readonly ProductsRepositoryMock productsMock = new();

	#region Valid request
	[Fact]
	public async Task When_RequestIsValid_Then_ReturnCreated() {
		// Arrange
		ProductCreateDto productCreateDto = new() { Name = "Test name", MainImageUrl = "Test image" };
		// Act
		var act = await ProductsEndpointsV1.CreateProduct(productsMock, productCreateDto);
		// Assert
		// ... type of result
		Assert.IsType<CreatedAtRoute<ProductDto>>(act.Result);
		// ... status code
		var result = act.Result as CreatedAtRoute<ProductDto>;
		Assert.NotNull(result);
		Assert.Equal(201, result.StatusCode);
	}

	[Fact]
	public async Task When_RequestIsValid_Then_ReturnProduct() {
		// Arrange
		ProductCreateDto productCreateDto = new() { Name = "Test name", MainImageUrl = "Test image" };
		// Act
		var act = await ProductsEndpointsV1.CreateProduct(productsMock, productCreateDto);
		// Assert
		// ... output data
		var result = act.Result as CreatedAtRoute<ProductDto>;
		Assert.NotNull(result);
		Assert.NotNull(result.Value);
		Assert.IsType<ProductDto>(result.Value);
		Assert.Equal(productCreateDto.Name, result.Value.Name);
		Assert.Equal(productCreateDto.MainImageUrl, result.Value.MainImageUrl);
	}

	[Fact]
	public async Task When_RequestIsValid_Then_ProductCreatedAndSaved() {
		// Arrange
		int createCount = productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.CreateProductAsync), 0);
		int saveCount = productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.SaveChangesAsync), 0);
		ProductCreateDto productCreateDto = new() { Name = "Test name", MainImageUrl = "Test image" };
		// Act
		var act = await ProductsEndpointsV1.CreateProduct(productsMock, productCreateDto);
		// Assert
		// ... method calls
		Assert.Equal(createCount + 1, productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.CreateProductAsync), 0));
		Assert.True(productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.SaveChangesAsync), 0) > saveCount);
	}
	#endregion

	#region Missing required properties
	[Fact]
	public async Task When_NameIsNull_Then_ReturnBadRequest() {
		// Arrange
		ProductCreateDto productCreateDto = new() { Name = null, MainImageUrl = "Test image" };
		// Act
		var act = await ProductsEndpointsV1.CreateProduct(productsMock, productCreateDto);
		// Assert
		// ... type of result
		Assert.IsType<BadRequest<string>>(act.Result);
		// ... status code
		var result = act.Result as BadRequest<string>;
		Assert.NotNull(result);
		Assert.Equal(400, result.StatusCode);
	}

	[Fact]
	public async Task When_NameIsEmpty_Then_ReturnBadRequest() {
		// Arrange
		ProductCreateDto productCreateDto = new() { Name = "", MainImageUrl = "Test image" };
		// Act
		var act = await ProductsEndpointsV1.CreateProduct(productsMock, productCreateDto);
		// Assert
		// ... type of result
		Assert.IsType<BadRequest<string>>(act.Result);
		// ... status code
		var result = act.Result as BadRequest<string>;
		Assert.NotNull(result);
		Assert.Equal(400, result.StatusCode);
	}

	[Fact]
	public async Task When_NameIsEmpty_Then_ProductNotCreated() {
		// Arrange
		int createCount = productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.CreateProductAsync), 0);
		ProductCreateDto productCreateDto = new() { Name = "", MainImageUrl = "Test image" };
		// Act
		var act = await ProductsEndpointsV1.CreateProduct(productsMock, productCreateDto);
		// Assert
		// ... method calls
		Assert.Equal(createCount, productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.CreateProductAsync), 0));
	}

	[Fact]
	public async Task When_ImageUrlIsNull_Then_ReturnBadRequest() {
		// Arrange
		ProductCreateDto productCreateDto = new() { Name = "Test name", MainImageUrl = null };
		// Act
		var act = await ProductsEndpointsV1.CreateProduct(productsMock, productCreateDto);
		// Assert
		// ... type of result
		Assert.IsType<BadRequest<string>>(act.Result);
		// ... status code
		var result = act.Result as BadRequest<string>;
		Assert.NotNull(result);
		Assert.Equal(400, result.StatusCode);
	}

	[Fact]
	public async Task When_ImageUrlIsEmpty_Then_ReturnBadRequest() {
		// Arrange
		ProductCreateDto productCreateDto = new() { Name = "Test name", MainImageUrl = "" };
		// Act
		var act = await ProductsEndpointsV1.CreateProduct(productsMock, productCreateDto);
		// Assert
		// ... type of result
		Assert.IsType<BadRequest<string>>(act.Result);
		// ... status code
		var result = act.Result as BadRequest<string>;
		Assert.NotNull(result);
		Assert.Equal(400, result.StatusCode);
	}

	[Fact]
	public async Task When_ImageUrlIsEmpty_Then_ProductNotCreated() {
		// Arrange
		int createCount = productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.CreateProductAsync), 0);
		ProductCreateDto productCreateDto = new() { Name = "Test name", MainImageUrl = "" };
		// Act
		var act = await ProductsEndpointsV1.CreateProduct(productsMock, productCreateDto);
		// Assert
		// ... method calls
		Assert.Equal(createCount, productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.CreateProductAsync), 0));
	}
	#endregion

	#region Invalid values
	[Fact]
	public async Task When_PriceIsNegative_Then_ReturnBadRequest() {
		// Arrange
		ProductCreateDto productCreateDto = new() { Name = "Test name", MainImageUrl = "Test image", Price = -100.50M };
		// Act
		var act = await ProductsEndpointsV1.CreateProduct(productsMock, productCreateDto);
		// Assert
		// ... type of result
		Assert.IsType<BadRequest<string>>(act.Result);
		// ... status code
		var result = act.Result as BadRequest<string>;
		Assert.NotNull(result);
		Assert.Equal(400, result.StatusCode);
	}

	[Fact]
	public async Task When_PriceIsNegative_Then_ProductNotCreated() {
		// Arrange
		int createCount = productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.CreateProductAsync), 0);
		ProductCreateDto productCreateDto = new() { Name = "Test name", MainImageUrl = "Test image", Price = -100.50M };
		// Act
		var act = await ProductsEndpointsV1.CreateProduct(productsMock, productCreateDto);
		// Assert
		// ... method calls
		Assert.Equal(createCount, productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.CreateProductAsync), 0));
	}

	[Fact]
	public async Task When_QuantityIsNegative_Then_ReturnBadRequest() {
		// Arrange
		ProductCreateDto productCreateDto = new() { Name = "Test name", MainImageUrl = "Test image", Quantity = -5 };
		// Act
		var act = await ProductsEndpointsV1.CreateProduct(productsMock, productCreateDto);
		// Assert
		// ... type of result
		Assert.IsType<BadRequest<string>>(act.Result);
		// ... status code
		var result = act.Result as BadRequest<string>;
		Assert.NotNull(result);
		Assert.Equal(400, result.StatusCode);
	}

	[Fact]
	public async Task When_QuantityIsNegative_Then_ProductNotCreated() {
		// Arrange
		int createCount = productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.CreateProductAsync), 0);
		ProductCreateDto productCreateDto = new() { Name = "Test name", MainImageUrl = "Test image", Quantity = -5 };
		// Act
		var act = await ProductsEndpointsV1.CreateProduct(productsMock, productCreateDto);
		// Assert
		// ... method calls
		Assert.Equal(createCount, productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.CreateProductAsync), 0));
	}
	#endregion

}

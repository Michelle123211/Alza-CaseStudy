using Microsoft.AspNetCore.Http.HttpResults;
using EshopAPI.DTOs;
using EshopAPI.Endpoints.v1;
using EshopAPI.Entities;
using Tests.Mocks;

namespace Tests.EndpointTests.v1;


/// <summary>
/// A set of unit tests targeting "PATCH /api/v1/products/{id}" endpoint.
/// </summary>
public class UpdateProductTests {

	private readonly ProductsRepositoryMock productsMock = new();

	#region Valid request
	[Fact]
	public async Task When_RequestIsValid_Then_ReturnOk() {
		// Arrange
		int id = -123;
		productsMock.Reset();
		productsMock.Products.Add(new Product() { Id = id, Name = "Some name", MainImageUrl = "Some image" });
		ProductUpdateDto productUpdateDto = new ProductUpdateDto() { Name = "Really cool name" };
		// Act
		var act = await ProductsEndpointsV1.UpdateProduct(productsMock, id, productUpdateDto);
		// Assert
		// ... type of result
		Assert.IsType<Ok<ProductDto>>(act.Result);
		// ... status code
		var result = act.Result as Ok<ProductDto>;
		Assert.NotNull(result);
		Assert.Equal(200, result.StatusCode);
	}

	[Fact]
	public async Task When_RequestIsValid_Then_ReturnProduct() {
		// Arrange
		int id = -123;
		string origName = "To-be-updated name", newName = "Updated name";
		string origImage = "Boring image", origDescription = "Just some description";
		decimal origPrice = 500.90M;
		int origQuantity = 20, newQuantity = 123456789;
		Product product = new Product() { Id = id, Name = origName, MainImageUrl = origImage,
			Price = origPrice, Description = origDescription, Quantity = origQuantity
		};
		productsMock.Reset();
		productsMock.Products.Add(product);
		ProductUpdateDto productUpdateDto = new ProductUpdateDto() { Name = newName, Quantity = newQuantity };
		// Act
		var act = await ProductsEndpointsV1.UpdateProduct(productsMock, id, productUpdateDto);
		// Assert
		// ... output data
		var result = act.Result as Ok<ProductDto>;
		Assert.NotNull(result);
		Assert.NotNull(result.Value);
		Assert.IsType<ProductDto>(result.Value);
		Assert.Equal(id, result.Value.Id);
		Assert.Equal(newName, result.Value.Name);
		Assert.Equal(origImage, result.Value.MainImageUrl);
		Assert.Equal(origPrice, result.Value.Price);
		Assert.Equal(origDescription, result.Value.Description);
		Assert.Equal(newQuantity, result.Value.Quantity);
	}

	[Fact]
	public async Task When_RequestIsValid_Then_ProductUpdatedAndSaved() {
		// Arrange
		int id = -123;
		productsMock.Reset();
		productsMock.Products.Add(new Product() { Id = id, Name = "Some name", MainImageUrl = "Some image" });
		ProductUpdateDto productUpdateDto = new ProductUpdateDto() { Name = "Really cool name" };
		int updateCount = productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.UpdateProductAsync), 0);
		int saveCount = productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.SaveChangesAsync), 0);
		// Act
		var act = await ProductsEndpointsV1.UpdateProduct(productsMock, id, productUpdateDto);
		// Assert
		// ... method calls
		Assert.Equal(updateCount + 1, productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.UpdateProductAsync), 0));
		Assert.True(productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.SaveChangesAsync), 0) > saveCount);
	}
	#endregion

	#region Non-existing ID
	[Fact]
	public async Task When_ProductDoesNotExist_Then_ReturnNotFound() {
		// Arrange
		productsMock.Reset();
		int id = -123;
		ProductUpdateDto productUpdateDto = new ProductUpdateDto() { Name = "Really cool name" };
		// Act
		var act = await ProductsEndpointsV1.UpdateProduct(productsMock, id, productUpdateDto);
		// Assert
		// ... type of result
		Assert.IsType<NotFound<string>>(act.Result);
		// ... status code
		var result = act.Result as NotFound<string>;
		Assert.NotNull(result);
		Assert.Equal(404, result.StatusCode);
	}

	[Fact]
	public async Task When_ProductDoesNotExist_Then_ProductNotUpdated() {
		// Arrange
		productsMock.Reset();
		int id = -123;
		ProductUpdateDto productUpdateDto = new ProductUpdateDto() { Name = "Really cool name" };
		int updateCount = productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.UpdateProductAsync), 0);
		// Act
		var act = await ProductsEndpointsV1.UpdateProduct(productsMock, id, productUpdateDto);
		// Assert
		// ... method calls
		Assert.Equal(updateCount, productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.UpdateProductAsync), 0));
	}
	#endregion

	#region Missing required properties
	[Fact]
	public async Task When_NameIsEmpty_Then_ReturnBadRequest() {
		// Arrange
		int id = -123;
		productsMock.Reset();
		productsMock.Products.Add(new Product() { Id = id, Name = "Some name", MainImageUrl = "Some image" });
		ProductUpdateDto productUpdateDto = new ProductUpdateDto() { Name = "" };
		// Act
		var act = await ProductsEndpointsV1.UpdateProduct(productsMock, id, productUpdateDto);
		// Assert
		// ... type of result
		Assert.IsType<BadRequest<string>>(act.Result);
		// ... status code
		var result = act.Result as BadRequest<string>;
		Assert.NotNull(result);
		Assert.Equal(400, result.StatusCode);
	}

	[Fact]
	public async Task When_NameIsEmpty_Then_ProductNotUpdated() {
		// Arrange
		int id = -123;
		productsMock.Reset();
		productsMock.Products.Add(new Product() { Id = id, Name = "Some name", MainImageUrl = "Some image" });
		ProductUpdateDto productUpdateDto = new ProductUpdateDto() { Name = "" };
		int updateCount = productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.UpdateProductAsync), 0);
		// Act
		var act = await ProductsEndpointsV1.UpdateProduct(productsMock, id, productUpdateDto);
		// Assert
		// ... method calls
		Assert.Equal(updateCount, productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.UpdateProductAsync), 0));
	}

	[Fact]
	public async Task When_ImageUrlIsEmpty_Then_ReturnBadRequest() {
		// Arrange
		int id = -123;
		productsMock.Reset();
		productsMock.Products.Add(new Product() { Id = id, Name = "Some name", MainImageUrl = "Some image" });
		ProductUpdateDto productUpdateDto = new ProductUpdateDto() { MainImageUrl = "" };
		// Act
		var act = await ProductsEndpointsV1.UpdateProduct(productsMock, id, productUpdateDto);
		// Assert
		// ... type of result
		Assert.IsType<BadRequest<string>>(act.Result);
		// ... status code
		var result = act.Result as BadRequest<string>;
		Assert.NotNull(result);
		Assert.Equal(400, result.StatusCode);
	}

	[Fact]
	public async Task When_ImageUrlIsEmpty_Then_ProductNotUpdated() {
		// Arrange
		int id = -123;
		productsMock.Reset();
		productsMock.Products.Add(new Product() { Id = id, Name = "Some name", MainImageUrl = "Some image" });
		ProductUpdateDto productUpdateDto = new ProductUpdateDto() { MainImageUrl = "" };
		int updateCount = productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.UpdateProductAsync), 0);
		// Act
		var act = await ProductsEndpointsV1.UpdateProduct(productsMock, id, productUpdateDto);
		// Assert
		// ... method calls
		Assert.Equal(updateCount, productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.UpdateProductAsync), 0));
	}
	#endregion

	#region Invalid values
	[Fact]
	public async Task When_PriceIsNegative_Then_ReturnBadRequest() {
		// Arrange
		int id = -123;
		productsMock.Reset();
		productsMock.Products.Add(new Product() { Id = id, Name = "Some name", MainImageUrl = "Some image", Price = 255 });
		ProductUpdateDto productUpdateDto = new ProductUpdateDto() { Price = -50.5M };
		// Act
		var act = await ProductsEndpointsV1.UpdateProduct(productsMock, id, productUpdateDto);
		// Assert
		// ... type of result
		Assert.IsType<BadRequest<string>>(act.Result);
		// ... status code
		var result = act.Result as BadRequest<string>;
		Assert.NotNull(result);
		Assert.Equal(400, result.StatusCode);
	}

	[Fact]
	public async Task When_PriceIsNegative_Then_ProductNotUpdated() {
		// Arrange
		int id = -123;
		productsMock.Reset();
		productsMock.Products.Add(new Product() { Id = id, Name = "Some name", MainImageUrl = "Some image", Price = 255 });
		ProductUpdateDto productUpdateDto = new ProductUpdateDto() { Price = -50.5M };
		int updateCount = productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.UpdateProductAsync), 0);
		// Act
		var act = await ProductsEndpointsV1.UpdateProduct(productsMock, id, productUpdateDto);
		// Assert
		// ... method calls
		Assert.Equal(updateCount, productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.UpdateProductAsync), 0));
	}

	[Fact]
	public async Task When_QuantityIsNegative_Then_ReturnBadRequest() {
		// Arrange
		int id = -123;
		productsMock.Reset();
		productsMock.Products.Add(new Product() { Id = id, Name = "Some name", MainImageUrl = "Some image", Quantity = 5 });
		ProductUpdateDto productUpdateDto = new ProductUpdateDto() { Quantity = -10 };
		// Act
		var act = await ProductsEndpointsV1.UpdateProduct(productsMock, id, productUpdateDto);
		// Assert
		// ... type of result
		Assert.IsType<BadRequest<string>>(act.Result);
		// ... status code
		var result = act.Result as BadRequest<string>;
		Assert.NotNull(result);
		Assert.Equal(400, result.StatusCode);
	}

	[Fact]
	public async Task When_QuantityIsNegative_Then_ProductNotUpdated() {
		// Arrange
		int id = -123;
		productsMock.Reset();
		productsMock.Products.Add(new Product() { Id = id, Name = "Some name", MainImageUrl = "Some image", Quantity = 5 });
		ProductUpdateDto productUpdateDto = new ProductUpdateDto() { Quantity = -10 };
		int updateCount = productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.UpdateProductAsync), 0);
		// Act
		var act = await ProductsEndpointsV1.UpdateProduct(productsMock, id, productUpdateDto);
		// Assert
		// ... method calls
		Assert.Equal(updateCount, productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.UpdateProductAsync), 0));
	}
	#endregion
}

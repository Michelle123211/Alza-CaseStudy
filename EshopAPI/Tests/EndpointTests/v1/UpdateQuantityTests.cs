using Microsoft.AspNetCore.Http.HttpResults;
using EshopAPI.DTOs;
using EshopAPI.Endpoints.v1;
using EshopAPI.Entities;
using Tests.Mocks;

namespace Tests.EndpointTests.v1;


/// <summary>
/// A set of unit tests targeting "PUT /api/v1/products/{id}/quantity" endpoint.
/// </summary>
public class UpdateQuantityTests {

	private readonly ProductsRepositoryMock productsMock = new();

	#region Non-existing ID
	[Fact]
	public async Task When_ProductDoesNotExist_Then_ReturnNotFound() {
		// Arrange
		productsMock.Reset();
		// Act
		var act = await ProductsEndpointsV1.UpdateProductQuantity(productsMock, -123, 0);
		// Assert
		// ... type of result
		Assert.IsType<NotFound<string>>(act.Result);
		// ... status code
		var result = act.Result as NotFound<string>;
		Assert.NotNull(result);
		Assert.Equal(404, result.StatusCode);
	}

	[Fact]
	public async Task When_ProductDoesNotExist_Then_ProductQuantityNotUpdated() {
		// Arrange
		productsMock.Reset();
		int updateCount = productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.UpdateProductQuantityAsync), 0);
		// Act
		var act = await ProductsEndpointsV1.UpdateProductQuantity(productsMock, -123, 0);
		// Assert
		// ... method calls
		Assert.Equal(updateCount, productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.UpdateProductQuantityAsync), 0));
	}
	#endregion

	#region Invalid values
	[Fact]
	public async Task When_DecreaseQuantityBelowZero_Then_ReturnBadRequest() {
		// Arrange
		int id = -123;
		productsMock.Reset();
		productsMock.Products.Add(new Product() { Id = id, Name = "Some name", MainImageUrl = "Some image", Quantity = 10 });
		// Act
		var act = await ProductsEndpointsV1.UpdateProductQuantity(productsMock, -123, -20);
		// Assert
		// ... type of result
		Assert.IsType<BadRequest<string>>(act.Result);
		// ... status code
		var result = act.Result as BadRequest<string>;
		Assert.NotNull(result);
		Assert.Equal(400, result.StatusCode);
	}

	[Fact]
	public async Task When_DecreaseQuantityBelowZero_Then_ProductQuantityNotUpdated() {
		// Arrange
		int id = -123;
		productsMock.Reset();
		productsMock.Products.Add(new Product() { Id = id, Name = "Some name", MainImageUrl = "Some image", Quantity = 10 });
		int updateCount = productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.UpdateProductQuantityAsync), 0);
		// Act
		var act = await ProductsEndpointsV1.UpdateProductQuantity(productsMock, id, -20);
		// Assert
		// ... method calls
		Assert.Equal(updateCount, productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.UpdateProductQuantityAsync), 0));
	}
	#endregion

	#region Valid request
	[Fact]
	public async Task When_DecreaseQuantityNonNegative_Then_ReturnOk() {
		// Arrange
		int id = -123;
		productsMock.Reset();
		productsMock.Products.Add(new Product() { Id = id, Name = "Some name", MainImageUrl = "Some image", Quantity = 50 });
		// Act
		var act = await ProductsEndpointsV1.UpdateProductQuantity(productsMock, id, -20);
		// Assert
		// ... type of result
		Assert.IsType<Ok<ProductDto>>(act.Result);
		// ... status code
		var result = act.Result as Ok<ProductDto>;
		Assert.NotNull(result);
		Assert.Equal(200, result.StatusCode);
	}

	[Fact]
	public async Task When_DecreaseQuantityNonNegative_Then_ReturnProduct() {
		// Arrange
		int id = -123;
		string name = "Some name", image = "Some image", description = "Some description";
		decimal price = 100;
		int origQuantity = 50, delta = -20;
		productsMock.Reset();
		productsMock.Products.Add(new Product() { Id = id, Name = name, MainImageUrl = image, Description = description, Price = price, Quantity = origQuantity });
		// Act
		var act = await ProductsEndpointsV1.UpdateProductQuantity(productsMock, id, delta);
		// Assert
		// ... output data
		var result = act.Result as Ok<ProductDto>;
		Assert.NotNull(result);
		Assert.NotNull(result.Value);
		Assert.IsType<ProductDto>(result.Value);
		Assert.Equal(id, result.Value.Id);
		Assert.Equal(name, result.Value.Name);
		Assert.Equal(image, result.Value.MainImageUrl);
		Assert.Equal(price, result.Value.Price);
		Assert.Equal(description, result.Value.Description);
		Assert.Equal(origQuantity + delta, result.Value.Quantity);
	}

	[Fact]
	public async Task When_DecreaseQuantityNonNegative_Then_ProductUpdatedAndSaved() {
		// Arrange
		int id = -123;
		productsMock.Reset();
		productsMock.Products.Add(new Product() { Id = id, Name = "Some name", MainImageUrl = "Some image", Quantity = 50 });
		int updateCount = productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.UpdateProductQuantityAsync), 0);
		int saveCount = productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.SaveChangesAsync), 0);
		// Act
		var act = await ProductsEndpointsV1.UpdateProductQuantity(productsMock, id, -20);
		// Assert
		// ... method calls
		Assert.Equal(updateCount + 1, productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.UpdateProductQuantityAsync), 0));
		Assert.True(productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.SaveChangesAsync), 0) > saveCount);
	}

	[Fact]
	public async Task When_IncreaseQuantity_Then_ReturnOk() {
		// Arrange
		int id = -123;
		productsMock.Reset();
		productsMock.Products.Add(new Product() { Id = id, Name = "Some name", MainImageUrl = "Some image", Quantity = 50 });
		// Act
		var act = await ProductsEndpointsV1.UpdateProductQuantity(productsMock, id, 15);
		// Assert
		// ... type of result
		Assert.IsType<Ok<ProductDto>>(act.Result);
		// ... status code
		var result = act.Result as Ok<ProductDto>;
		Assert.NotNull(result);
		Assert.Equal(200, result.StatusCode);
	}

	[Fact]
	public async Task When_IncreaseQuantity_Then_ReturnProduct() {
		// Arrange
		int id = -123;
		string name = "Some name", image = "Some image", description = "Some description";
		decimal price = 100;
		int origQuantity = 50, delta = 15;
		productsMock.Reset();
		productsMock.Products.Add(new Product() { Id = id, Name = name, MainImageUrl = image, Description = description, Price = price, Quantity = origQuantity });
		// Act
		var act = await ProductsEndpointsV1.UpdateProductQuantity(productsMock, id, delta);
		// Assert
		// ... output data
		var result = act.Result as Ok<ProductDto>;
		Assert.NotNull(result);
		Assert.NotNull(result.Value);
		Assert.IsType<ProductDto>(result.Value);
		Assert.Equal(id, result.Value.Id);
		Assert.Equal(name, result.Value.Name);
		Assert.Equal(image, result.Value.MainImageUrl);
		Assert.Equal(price, result.Value.Price);
		Assert.Equal(description, result.Value.Description);
		Assert.Equal(origQuantity + delta, result.Value.Quantity);
	}

	[Fact]
	public async Task When_IncreaseQuantity_Then_ProductUpdatedAndSaved() {
		// Arrange
		int id = -123;
		productsMock.Reset();
		productsMock.Products.Add(new Product() { Id = id, Name = "Some name", MainImageUrl = "Some image", Quantity = 50 });
		int updateCount = productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.UpdateProductQuantityAsync), 0);
		int saveCount = productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.SaveChangesAsync), 0);
		// Act
		var act = await ProductsEndpointsV1.UpdateProductQuantity(productsMock, id, 15);
		// Assert
		// ... method calls
		Assert.Equal(updateCount + 1, productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.UpdateProductQuantityAsync), 0));
		Assert.True(productsMock.MethodCalls.GetValueOrDefault(nameof(productsMock.SaveChangesAsync), 0) > saveCount);
	}
	#endregion

}

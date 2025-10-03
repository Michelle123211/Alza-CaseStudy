using Microsoft.AspNetCore.Http.HttpResults;
using System.Collections.Concurrent;
using EshopAPI.DTOs;
using EshopAPI.Endpoints.v2;
using EshopAPI.Infrastructure;
using Tests.Mocks;

namespace Tests.EndpointTests.v2;

/// <summary>
/// A set of unit tests targeting "PUT /api/v2/products/{id}/quantity", 
/// "PATCH /api/v2/products/{id}" and "GET /api/v2/products/{id}/status" endpoints.
/// </summary>
public class UpdateQueueTests {

	private readonly ProductUpdateQueueMock queueMock = new();
	private readonly ConcurrentDictionary<string, QueuedUpdateResult> statusDict = new();
	private readonly ProductsRepositoryMock productsMock = new();

	#region Whole product update
	[Fact]
	public async Task When_ProductUpdateRequested_Then_ReturnAccepted() {
		// Arrange
		queueMock.Reset();
		int id = -123;
		ProductUpdateDto productUpdateDto = new ProductUpdateDto() { Description = "Short description" };
		// Act
		var act = await ProductsEndpointsV2.EnqueueProductUpdate(queueMock, statusDict, productsMock, id, productUpdateDto);
		// Assert
		// ... type of result
		Assert.NotNull(act);
		Assert.IsType<AcceptedAtRoute>(act);
		// ... status code
		Assert.Equal(202, act.StatusCode);
	}

	[Fact]
	public async Task When_ProductUpdateRequested_Then_Enqueue() {
		// Arrange
		queueMock.Reset();
		int id = -123;
		string newName = "Really cool name";
		ProductUpdateDto productUpdateDto = new ProductUpdateDto() { Name = newName };
		int enqueueCount = queueMock.MethodCalls.GetValueOrDefault(nameof(queueMock.EnqueueUpdateAsync), 0);
		// Act
		var act = await ProductsEndpointsV2.EnqueueProductUpdate(queueMock, statusDict, productsMock, id, productUpdateDto);
		// Assert
		Assert.Equal(1, queueMock.Count);
		Assert.Equal(enqueueCount + 1, queueMock.MethodCalls.GetValueOrDefault(nameof(queueMock.EnqueueUpdateAsync), 0));
		QueuedUpdate update = await queueMock.DequeueUpdateAsync(CancellationToken.None);
		Assert.IsType<QueuedProductUpdate>(update);
		QueuedProductUpdate productUpdate = (QueuedProductUpdate)update;
		Assert.Equal(id, productUpdate.id);
		Assert.Equivalent(productUpdateDto, productUpdate.productUpdate);

	}
	#endregion

	#region Product quantity update
	[Fact]
	public async Task When_QuantityUpdateRequested_Then_ReturnAccepted() {
		// Arrange
		queueMock.Reset();
		int id = -123;
		int delta = 20;
		// Act
		var act = await ProductsEndpointsV2.EnqueueProductQuantityUpdate(queueMock, statusDict, productsMock, id, delta);
		// Assert
		// ... type of result
		Assert.NotNull(act);
		Assert.IsType<AcceptedAtRoute>(act);
		// ... status code
		Assert.Equal(202, act.StatusCode);
	}

	[Fact]
	public async Task When_QuantityUpdateRequested_Then_Enqueue() {
		// Arrange
		queueMock.Reset();
		int id = -123;
		int delta = 20;
		int enqueueCount = queueMock.MethodCalls.GetValueOrDefault(nameof(queueMock.EnqueueUpdateAsync), 0);
		// Act
		var act = await ProductsEndpointsV2.EnqueueProductQuantityUpdate(queueMock, statusDict, productsMock, id, delta);
		// Assert
		Assert.Equal(1, queueMock.Count);
		Assert.Equal(enqueueCount + 1, queueMock.MethodCalls.GetValueOrDefault(nameof(queueMock.EnqueueUpdateAsync), 0));
		QueuedUpdate update = await queueMock.DequeueUpdateAsync(CancellationToken.None);
		Assert.IsType<QueuedQuantityUpdate>(update);
		QueuedQuantityUpdate quantityUpdate = (QueuedQuantityUpdate)update;
		Assert.Equal(id, quantityUpdate.id);
		Assert.Equal(delta, quantityUpdate.quantityDelta);
	}
	#endregion

	#region Status
	[Fact]
	public async Task When_IdDoesNotExist_Then_ReturnNotFound() {
		// Arrange
		Guid id = Guid.NewGuid();
		// Act
		var act = await ProductsEndpointsV2.GetStatus(statusDict, id.ToString());
		// Assert
		// ... type of result
		Assert.IsType<NotFound<string>>(act.Result);
		// ... status code
		var result = act.Result as NotFound<string>;
		Assert.NotNull(result);
		Assert.Equal(404, result.StatusCode);
	}
	#endregion

}

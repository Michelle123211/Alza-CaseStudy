using EshopAPI.Infrastructure;

namespace Tests.Mocks;


/// <summary>
/// A class implementing <c>IProductUpdateQueue</c> which can be used to fake an actual queue pro update processing.
/// Uses a simple queue and also manages method call counters which can be used to verify an interaction occurred.
/// </summary>
internal class ProductUpdateQueueMock : IProductUpdateQueue {

	/// <summary>For each method, stores number of times it was called (to be able to verify whether an interaction with repository occurred).</summary>
	public Dictionary<string, int> MethodCalls = new();

	public int Count => queue.Count;

	private Queue<QueuedUpdate> queue = new();

	public void Reset() {
		queue.Clear();
		MethodCalls.Clear();
	}

	public Task EnqueueUpdateAsync(QueuedUpdate update) {
		MethodCalls[nameof(EnqueueUpdateAsync)] = 1 + MethodCalls.GetValueOrDefault(nameof(EnqueueUpdateAsync), 0);
		queue.Enqueue(update);
		return Task.CompletedTask;
	}

	public Task<QueuedUpdate> DequeueUpdateAsync(CancellationToken stoppingToken) {
		MethodCalls[nameof(DequeueUpdateAsync)] = 1 + MethodCalls.GetValueOrDefault(nameof(DequeueUpdateAsync), 0);
		return Task.FromResult(queue.Dequeue());
	}
}

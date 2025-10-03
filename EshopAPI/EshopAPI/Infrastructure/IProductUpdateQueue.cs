using System.Collections.Concurrent;
using System.Threading.Channels;
using EshopAPI.Entities;

namespace EshopAPI.Infrastructure;


/// <summary>
/// An interface representing an asynchronous queue for product updates processing;
/// </summary>
public interface IProductUpdateQueue {

	public int Count { get; }

	Task EnqueueUpdateAsync(QueuedUpdate update);
	Task<QueuedUpdate> DequeueUpdateAsync(CancellationToken stoppingToken);

}

/// <summary>
/// A specific implementation of an asynchronous queue for product updates processing.
/// Uses Channel&lt;QueuedUpdate&gt;.
/// </summary>
public class ProductUpdateQueue : IProductUpdateQueue {

	public int Count => channel.Reader.Count;

	private Channel<QueuedUpdate> channel = Channel.CreateBounded<QueuedUpdate>(new BoundedChannelOptions(100) {
			FullMode = BoundedChannelFullMode.Wait
	});

	public async Task EnqueueUpdateAsync(QueuedUpdate update) {
		await channel.Writer.WriteAsync(update);
	}

	public async Task<QueuedUpdate> DequeueUpdateAsync(CancellationToken stoppingToken) {
		return await channel.Reader.ReadAsync(stoppingToken);
	}
}

/// <summary>
/// A product update service running in the background and processing updates from asynchronous queue.
/// </summary>
public class ProductUpdateService : BackgroundService {

	private IServiceScopeFactory serviceScopeFactory;
	private IProductUpdateQueue queue;
	private ConcurrentDictionary<string, QueuedUpdateResult> statusDict;

	public ProductUpdateService(IServiceScopeFactory serviceScopeFactory, 
			IProductUpdateQueue queue, ConcurrentDictionary<string, QueuedUpdateResult> statusDict) {
		this.serviceScopeFactory = serviceScopeFactory;
		this.queue = queue;
		this.statusDict = statusDict;
	}

	/// <summary>
	/// Processes all update requests from the queue, until it is requested to stop.
	/// </summary>
	protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
		try {
			while (true) {
				QueuedUpdate update = await queue.DequeueUpdateAsync(stoppingToken);
				await ProcessUpdateAsync(update);
			}
		} catch (OperationCanceledException ex) {
			// Try to finish remaining items
			while (queue.Count > 0) {
				QueuedUpdate update = await queue.DequeueUpdateAsync(CancellationToken.None);
				await ProcessUpdateAsync(update);
			}
		}
	}

	private async Task ProcessUpdateAsync(QueuedUpdate update) {
		statusDict[update.jobId].Status = QueuedUpdateStatus.Processing;
		using (IServiceScope scope = serviceScopeFactory.CreateScope()) {
			var repository = scope.ServiceProvider.GetRequiredService<IProductsRepository>();
			string msg = await update.ExecuteUpdate(repository);
			statusDict[update.jobId].Message = msg;
			statusDict[update.jobId].Status = msg.Length > 0 ? QueuedUpdateStatus.Failed : QueuedUpdateStatus.Completed;
		}
	}
}

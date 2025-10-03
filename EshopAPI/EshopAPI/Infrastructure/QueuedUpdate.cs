using EshopAPI.DTOs;
using EshopAPI.Entities;
using System.Text;

namespace EshopAPI.Infrastructure;

/// <summary>
/// An abstract class representing an update which could be added to an asynchronous queue.
/// </summary>
public abstract class QueuedUpdate {
	/// <summary>Unique identifier of the request. Allows polling for status.</summary>
	public string jobId;

	/// <summary>Unique identifier of the product to be updated.</summary>
	public int id;

	public QueuedUpdate(string jobId, int productId) {
		this.jobId = jobId;
		this.id = productId;
	}

	/// <summary>
	/// Executes the update on the given products repository.
	/// </summary>
	/// <param name="productsRepository">Products repository to be modified.</param>
	public abstract Task<string> ExecuteUpdate(IProductsRepository productsRepository);
}


/// <summary>
/// A class representing status of an update which is queued for processing.
/// </summary>
public class QueuedUpdateResult {
	/// <summary>Current status of the process.</summary>
	public QueuedUpdateStatus Status { get; set; }
	/// <summary>Error messages in case the process failed.</summary>
	public string? Message { get; set; }

	public QueuedUpdateResult(QueuedUpdateStatus status, string? message) {
		this.Status = status;
		this.Message = message;
	}
}

/// <summary>
/// Status of the product update processed by asynchronous queue.
/// </summary>
public enum QueuedUpdateStatus { 
	Queued,
	Processing,
	Completed,
	Failed
}

/// <summary>
/// A representation of a (partial) product update which can be processed by an asynchronous queue.
/// </summary>
public class QueuedProductUpdate : QueuedUpdate {
	/// <summary>Product details which should be overwritten.</summary>
	public ProductUpdateDto productUpdate;

	public QueuedProductUpdate(string jobId, int productId, ProductUpdateDto productUpdate) : base(jobId, productId) {
		this.productUpdate = productUpdate;
	}

	/// <inheritdoc/>
	public override async Task<string> ExecuteUpdate(IProductsRepository productsRepository) {
		// Get a product with the given ID
		Product? origProduct = await productsRepository.GetProductByIdAsync(id);
		if (origProduct is null) return ErrorMsg.PRODUCT_NOT_FOUND;

		// Validate provided parameters (non-negative, required (either null or something, not empty))
		StringBuilder errors = new StringBuilder()
			.Append(Validate.Name(productUpdate.Name, true))
			.Append(Validate.MainImageUrl(productUpdate.MainImageUrl, true))
			.Append(Validate.Price(productUpdate.Price, true))
			.Append(Validate.Quantity(productUpdate.Quantity, true));
		if (errors.Length > 0) return errors.ToString();

		// Update information based on provided parameters (map ProductUpdateDto to Product, only non-nulls considered)
		origProduct.CombineWith(productUpdate);
		Product newProduct = await productsRepository.UpdateProductAsync(origProduct);
		await productsRepository.SaveChangesAsync();

		return string.Empty;
	}
}

/// <summary>
/// A representation of a product's quantity update which can be processed by an asynchronous queue.
/// </summary>
public class QueuedQuantityUpdate : QueuedUpdate {
	public int quantityDelta;

	public QueuedQuantityUpdate(string jobId, int productId, int quantityDelta) : base(jobId, productId) {
		this.quantityDelta = quantityDelta;
	}

	/// <inheritdoc/>
	public override async Task<string> ExecuteUpdate(IProductsRepository productRepository) {
		// Get a product with the given ID
		Product? origProduct = await productRepository.GetProductByIdAsync(id);
		if (origProduct is null) return ErrorMsg.PRODUCT_NOT_FOUND;

		// Validate provided parameter
		StringBuilder errors = new StringBuilder()
			.Append(Validate.Quantity(origProduct.Quantity, quantityDelta));
		if (errors.Length > 0) return errors.ToString();

		// Update quantity with the given delta
		Product? product = await productRepository.UpdateProductQuantityAsync(id, quantityDelta);
		await productRepository.SaveChangesAsync();

		return string.Empty;
	}
}
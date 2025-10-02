using EshopAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace EshopAPI.Data;

/// <summary>
/// A representation of products repository and basic interactions with it.
/// Serves as a mediator between the application code and EF Core (in accordance with the Repository pattern).
/// </summary>
public class ProductsRepository : IProductsRepository {

	private readonly ProductsDb db;

	public ProductsRepository(ProductsDb db) {
		this.db = db;
	}

	/// <inheritdoc/>
	public async Task SaveChangesAsync() {
		await db.SaveChangesAsync();
	}

	/// <inheritdoc/>
	public async Task<List<Product>> GetProductsAsync() {
		return await db.Products.ToListAsync();
	}

	/// <inheritdoc/>
	public async Task<List<Product>> GetProductsInStockAsync() {
		return await db.Products.Where(p => p.Quantity > 0).ToListAsync();
	}

	/// <inheritdoc/>
	public async Task<Product?> GetProductByIdAsync(int id) {
		return await db.Products.FindAsync(id);
	}

	/// <inheritdoc/>
	public Task<Product> CreateProductAsync(Product product) {
		Product addedProduct = db.Products.Add(product).Entity;
		return Task.FromResult(addedProduct);
	}

	/// <inheritdoc/>
	public async Task<Product?> UpdateProductAsync(Product product) {
		if (await db.Products.FindAsync(product.Id) is null) return null;
		Product updatedProduct = db.Products.Update(product).Entity;
		return updatedProduct;
	}

	/// <inheritdoc/>
	public async Task<Product?> UpdateProductQuantityAsync(int id, int quantityDelta) {
		Product? product = await db.Products.FindAsync(id);
		if (product is not null) product.Quantity += quantityDelta;
		return product;
	}
}

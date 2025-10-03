using EshopAPI.DTOs;
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
		return await db.Products.AsNoTracking().ToListAsync();
	}

	/// <inheritdoc/>
	public async Task<PagedResponse<Product>> GetProductsPageAsync(int page, int pageSize) {
		int totalItems = await db.Products.AsNoTracking().CountAsync();
		List<Product> products = await db.Products.AsNoTracking().OrderBy(p => p.Id)
			.Skip(page * pageSize)
			.Take(pageSize).ToListAsync();

		return new PagedResponse<Product>(products, page, pageSize, totalItems);
	}

	/// <inheritdoc/>
	public async Task<List<Product>> GetProductsInStockAsync() {
		return await db.Products.AsNoTracking().Where(p => p.Quantity > 0).ToListAsync();
	}

	/// <inheritdoc/>
	public async Task<PagedResponse<Product>> GetProductsInStockPageAsync(int page, int pageSize) {
		int totalItems = await db.Products.Where(p => p.Quantity > 0).AsNoTracking().CountAsync();
		List<Product> products = await db.Products.AsNoTracking().Where(p => p.Quantity > 0).OrderBy(p => p.Id)
			.Skip(page * pageSize)
			.Take(pageSize).ToListAsync();

		return new PagedResponse<Product>(products, page, pageSize, totalItems);
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
	public Task<Product> UpdateProductAsync(Product product) {
		Product updatedProduct = db.Products.Update(product).Entity;
		return Task.FromResult(updatedProduct);
	}

	/// <inheritdoc/>
	public async Task<Product?> UpdateProductQuantityAsync(int id, int quantityDelta) {
		Product? product = await db.Products.FindAsync(id);
		if (product is not null) product.Quantity += quantityDelta;
		return product;
	}
}

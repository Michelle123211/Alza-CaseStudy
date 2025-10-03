using EshopAPI.Entities;

namespace Tests.Mocks;

/// <summary>
/// A class implementing <c>IProductsRepository</c> which can be used to fake an actual products repository with a database behind the scenes.
/// Uses a pre-defined list of items, which can be further modified.
/// Manages method call counters for each method which can be used to verify an interaction occurred.
/// </summary>
internal class ProductsRepositoryMock : IProductsRepository {

	/// <summary>A pre-defined list of products which can be used for testing.</summary>
	public List<Product> Products = new();
	/// <summary>For each method, stores number of times it was called (to be able to verify whether an interaction with repository occurred).</summary>
	public Dictionary<string, int> MethodCalls = new();

	private int nextId = 1;

	public ProductsRepositoryMock() {
		Reset();
	}

	/// <summary>
	/// Initializes the mock repository with a pre-defined list of products and resets its inner state.
	/// </summary>
	public void Reset() { 
		Products.Clear();
		MethodCalls.Clear();
		for (int i = 1; i <= 5; i++) {
			Products.Add(new Product { 
				Id = i, 
				Name = $"Test Name {i}", 
				MainImageUrl = $"Test Image {i}", 
				Description = $"Test Description {i}", 
				Price = i * 110 + i * 0.1M
			});
		}
		nextId = Products.Count + 1;
	}

	/// <summary>Increments counter of <c>SaveChangesAsync()</c> method calls.</summary>
	public Task SaveChangesAsync() {
		MethodCalls[nameof(SaveChangesAsync)] = 1 + MethodCalls.GetValueOrDefault(nameof(SaveChangesAsync), 0);
		return Task.CompletedTask;
	}

	/// <summary>Increments counter of <c>GetProductsAsync()</c> method calls and returns a list of all products.</summary>
	public Task<List<Product>> GetProductsAsync() {
		MethodCalls[nameof(GetProductsAsync)] = 1 + MethodCalls.GetValueOrDefault(nameof(GetProductsAsync), 0);
		return Task.FromResult(Products);
	}

	/// <summary>Increments counter of <c>GetProductsInStockAsync()</c> method calls and returns a list of all products in stock.</summary>
	public Task<List<Product>> GetProductsInStockAsync() {
		MethodCalls[nameof(GetProductsInStockAsync)] = 1 + MethodCalls.GetValueOrDefault(nameof(GetProductsInStockAsync), 0);
		return Task.FromResult(Products.Where(p => p.Quantity > 0).ToList());
	}

	/// <summary>
	/// Increments counter of <c>GetProductByIdAsync()</c> method calls and tries to obtain a product with the given ID from a list.
	/// </summary>
	/// <param name="id">Unique identifier of the product to be found.</param>
	/// <returns>A product with the given ID (or <c>null</c> if not found).</returns>
	public Task<Product?> GetProductByIdAsync(int id) {
		MethodCalls[nameof(GetProductByIdAsync)] = 1 + MethodCalls.GetValueOrDefault(nameof(GetProductByIdAsync), 0);
		return Task.FromResult(Products.FirstOrDefault(p => p!.Id == id, null));
	}

	/// <summary>
	/// Increments counter of <c>CreateProductAsync()</c> method calls and adds the given product to the list.
	/// </summary>
	/// <param name="product">Details of the product to be created.</param>
	/// <returns>The newly created product.</returns>
	public Task<Product> CreateProductAsync(Product product) {
		MethodCalls[nameof(CreateProductAsync)] = 1 + MethodCalls.GetValueOrDefault(nameof(CreateProductAsync), 0);
		product.Id = nextId++;
		Products.Add(product);
		return Task.FromResult(product);
	}

	/// <summary>
	/// Increments counter of <c>UpdateProductAsync()</c> method calls and updates product with the corresponding identifier.
	/// </summary>
	/// <param name="product">Details of the product to be updated.</param>
	/// <returns>The updated product (or <c>null</c> if not found).</returns>
	public Task<Product> UpdateProductAsync(Product product) {
		MethodCalls[nameof(UpdateProductAsync)] = 1 + MethodCalls.GetValueOrDefault(nameof(UpdateProductAsync), 0);
		int index = Products.FindIndex(p => p.Id == product.Id);
		if (index >= 0) Products[index] = product;
		return Task.FromResult(product);
	}

	/// <summary>
	/// Increments counter of <c>UpdateProductQuantityAsync()</c> method calls and updates quantity of the product with the given identifier by the given delta.
	/// </summary>
	/// <param name="id">Unique identifier of the product to be updated.</param>
	/// <param name="quantityDelta">Relative chane in quantity (positive to increase, negative to decrease).</param>
	/// <returns>The updated product (or <c>null</c> if not found).</returns>
	public Task<Product?> UpdateProductQuantityAsync(int id, int quantityDelta) {
		MethodCalls[nameof(UpdateProductQuantityAsync)] = 1 + MethodCalls.GetValueOrDefault(nameof(UpdateProductQuantityAsync), 0);
		Product? product = Products.Find(p => p.Id == id);
		if (product is not null) product.Quantity += quantityDelta;
		return Task.FromResult(product);
	}
}

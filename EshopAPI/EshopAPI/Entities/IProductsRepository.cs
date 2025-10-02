namespace EshopAPI.Entities;

/// <summary>
/// An interface representing products repository and basic interactions with it.
/// Serves as a mediator between the application code and EF Core (in accordance with the Repository pattern).
/// </summary>
public interface IProductsRepository {

	/// <summary>
	/// Saves all unsaved changes.
	/// </summary>
	Task SaveChangesAsync();

	/// <summary>
	/// Gets a list of all products from the products repository.
	/// </summary>
	/// <returns>All products in the repository.</returns>
	Task<List<Product>> GetProductsAsync();

	/// <summary>
	/// Gets a list of all products from the products repository which are in stock.
	/// </summary>
	/// <returns>All products in stock.</returns>
	Task<List<Product>> GetProductsInStockAsync();

	/// <summary>
	/// Gets a single product based on its identifier.
	/// </summary>
	/// <param name="id">Unique identifier of the product to ne found.</param>
	/// <returns>A product with the given ID (or <c>null</c> if not found).</returns>
	Task<Product?> GetProductByIdAsync(int id);

	/// <summary>
	/// Creates a new product from the provided information and adds it to the repository.
	/// </summary>
	/// <param name="product">Details of the product to be created.</param>
	/// <returns>The newly created product.</returns>
	Task<Product> CreateProductAsync(Product product);

	/// <summary>
	/// Updates a product with the provided details based on the same identifier.
	/// </summary>
	/// <param name="product">Details of the product to be updated.</param>
	/// <returns>The updated product (or <c>null</c> if not found).</returns>
	Task<Product?> UpdateProductAsync(Product product);

	/// <summary>
	/// Updates quantity of the product with the given identifier by the given delta.
	/// </summary>
	/// <param name="id">Unique identifier of the product to be updated.</param>
	/// <param name="quantityDelta">Relative chane in quantity (positive to increase, negative to decrease).</param>
	/// <returns>The updated product (or <c>null</c> if not found).</returns>
	Task<Product?> UpdateProductQuantityAsync(int id, int quantityDelta);
}
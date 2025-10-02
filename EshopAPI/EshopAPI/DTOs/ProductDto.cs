namespace EshopAPI.DTOs;

/// <summary>
/// A representation of a product which is used for communication with a client when getting products.
/// Contains all product details, including its unique identifier.
/// </summary>
public class ProductDto {
	/// <summary>Unique identifier used as a primary key in database.</summary>
	public required int Id {  get; set; }

	/// <summary>Product name.</summary>
	public required string Name { get; set; }

	/// <summary>URL of the main product image.</summary>
	public required string MainImageUrl { get; set; }

	/// <summary>Product price.</summary>
	public required decimal Price { get; set; }

	/// <summary>Product description.</summary>
	public required string Description { get; set; }

	/// <summary>Quantity of the product in stock (0 means out of stock).</summary>
	public required int Quantity { get; set; }
}

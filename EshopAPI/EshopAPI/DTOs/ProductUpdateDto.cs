namespace EshopAPI.DTOs;

/// <summary>
/// A representation of a product which is used for communication with a client when updating a product.
/// Contains all product details, except for its unique identifier which is generated on the server.
/// All properties are optional, so only a subset could be selected for overwrite (<c>null</c> values will be ignored).
/// </summary>
public class ProductUpdateDto {
	/// <summary>Product name.</summary>
	public string? Name { get; set; }

	/// <summary>URL of the main product image.</summary>
	public string? MainImageUrl { get; set; }

	/// <summary>Product price.</summary>
	public decimal? Price { get; set; }

	/// <summary>Product description.</summary>
	public string? Description { get; set; }

	/// <summary>Quantity of the product in stock (0 means out of stock).</summary>
	public int? Quantity { get; set; }
}

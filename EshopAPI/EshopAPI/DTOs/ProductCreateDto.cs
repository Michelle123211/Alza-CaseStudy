using System.ComponentModel.DataAnnotations;

namespace EshopAPI.DTOs;

/// <summary>
/// A representation of a product which is used for communication with a client when creating a new product.
/// Contains all product details, except for its unique identifier which is generated on the server.
/// <c>Name</c> and <c>MainImageUrl</c> are required.
/// </summary>
public class ProductCreateDto {
	/// <summary>Unique identifier used as a primary key in database.</summary>
	[Required]

	/// <summary>Product name.</summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>URL of the main product image.</summary>
	[Required]
	public string MainImageUrl { get; set; } = string.Empty;

	/// <summary>Product price.</summary>
	public decimal Price { get; set; } = 0;

	/// <summary>Product description.</summary>
	public string Description { get; set; } = string.Empty;

	/// <summary>Quantity of the product in stock (0 means out of stock).</summary>
	public int Quantity { get; set; } = 0;
}

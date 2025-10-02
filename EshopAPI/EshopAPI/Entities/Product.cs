using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EshopAPI.Entities;


/// <summary>
/// An internal representation of a product with all of its details, including unique identifier.
/// <c>Name</c> and <c>MainImageUrl</c> are required.
/// </summary>
public class Product {
	/// <summary>Unique identifier used as a primary key in database.</summary>
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }

	/// <summary>Product name.</summary>
	public required string Name { get; set; }

	/// <summary>URL of the main product image.</summary>
	public required string MainImageUrl { get; set; }

	/// <summary>Product price.</summary>
	public decimal Price { get; set; } = 0;

	/// <summary>Product description.</summary>
	public string Description { get; set; } = string.Empty;

	/// <summary>Quantity of the product in stock (0 means out of stock).</summary>
	public int Quantity { get; set; } = 0;
}

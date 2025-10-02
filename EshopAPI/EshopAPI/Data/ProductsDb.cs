using Microsoft.EntityFrameworkCore;
using EshopAPI.Entities;

namespace EshopAPI.Data;

/// <summary>
/// A class derived from <c>DbContext</c> which is used to access all products in the model.
/// Available to the endpoints via dependency injection.
/// </summary>
public class ProductsDb : DbContext {

	public ProductsDb(DbContextOptions<ProductsDb> options)
		: base(options) { }

	/// <summary>Database table storing products with all of their details.</summary>
	public DbSet<Product> Products { get; set; }

	/// <summary>
	/// Fills products table with some arbitrary data for testing purposes.
	/// </summary>
	public void Seed() {
		Products.Add(new Product {
			Name = "Rubber duck - Classic",
			MainImageUrl = "https://www.shipducky.com/cdn/shop/products/Instagrampost-1a.jpg",
			Price = 250,
			Description = "This rubber duck is of great importance not only when bathing but also when debugging one's code.",
			Quantity = 42
		});
		Products.Add(new Product {
			Name = "Ortega R55",
			MainImageUrl = "https://image.alza.cz/products/OTG008w1e/OTG008w1e.jpg",
			Price = 5_149,
			Description = "Full size guitar with solid Engelmann spruce top, Catalpa back and side, and satin finish.",
			Quantity = 0
		});
		Products.Add(new Product {
			Name = "Rubber duck - Cupcake",
			MainImageUrl = "https://www.shipducky.com/cdn/shop/products/cupcake-canard-rubber-duck.jpg",
			Price = 500,
			Description = "This improved rubber duck will make sure you'll be craving cupcakes at all times.",
			Quantity = 19
		});
		Products.Add(new Product {
			Name = "Qiushi dog bed grey and white XXL",
			MainImageUrl = "https://image.alza.cz/products/CHPpe1072/CHPpe1072.jpg",
			Price = 1_390,
			Description = "Do you want to surprise your dog and buy it a comfortable bed? Look no further! We can recommend this Qiushi dog bed grey and white XXL 105 × 80 × 23 cm. This one is ideal especially for all breeds.",
			Quantity = 13
		});
		Products.Add(new Product {
			Name = "LEGO® Lord of the Rings™ 10316 The Lord of the Rings: Rivendell",
			MainImageUrl = "https://image.alza.cz/products/LO10316/LO10316.jpg",
			Price = 12_890,
			Description = "The model will delight you with its magical places and details, such as the Elven forge, Elrond's untidy study, the fragments of Narsil and paintings and sculptures from the history of Middle-earth.",
			Quantity = 23
		});
		Products.Add(new Product {
			Name = "BOSCH BGL41SIL1H GL41",
			MainImageUrl = "https://image.alza.cz/products/BSHsvys140/BSHsvys140.jpg",
			Price = 5_119,
			Description = "This model is suitable for most surfaces. Air is filtered through the hepa filter. Dust and dirt are collected in a single-use dust bag. When it gets full, you just throw it in the bin and put a new one in.",
			Quantity = 0
		});
		Products.Add(new Product {
			Name = "Rubik's Cube 2×2 Speed Cube",
			MainImageUrl = "https://image.alza.cz/products/HRAls12449/HRAls12449.jpg",
			Price = 289,
			Description = "The Rubik's Cube 2×2 Speed Cube puzzle is exactly for those who would like to build a classic Rubik's Cube but find it difficult. The improved design of the Speed Cube 2×2 Rubik's Cube uses magnets to provide better stability and act as a positioning system to help align the cube for further flipping and rotation.",
			Quantity = 46
		});
		SaveChanges();
	}

}

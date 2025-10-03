using EshopAPI.Entities;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EshopAPI;

/// <summary>
/// A static class containing frequently used error messages as constants.
/// </summary>
public static class ErrorMsg {
	public const string NOT_FOUND = "Product with the given ID doesn't exist. ";
	public const string EMPTY_NAME = "Product's name cannot be empty. ";
	public const string EMPTY_IMAGE_URL = "Product's image URL cannot be empty. ";
	public const string NEGATIVE_PRICE = "Product's price must be greater than or equal to zero. ";
	public const string NEGATIVE_QUANTITY = "Product's quantity must be greater than or equal to zero. ";
	public const string NEGATIVE_PAGE = "Page must be greater than or equal to zero. ";
	public const string NON_POSITIVE_PAGE_SIZE = "Page size must be greater than zero. ";
	public const string SUCCESS = "";
}

/// <summary>
/// A static class containing custom headers as constants.
/// </summary>
public static class Header {
	public const string PAGE = "X-Page";
	public const string PAGE_SIZE = "X-Page-Size";
	public const string TOTAL_PAGES = "X-Total-Pages";
	public const string TOTAL_ITEMS = "X-Total-Items";
}

/// <summary>
/// A static class containing helper methods for parameter validation.
/// </summary>
public static class Validate {

	/// <summary>
	/// Checks whether the provided name is valid, i.e. not <c>null</c> and not empty.
	/// </summary>
	/// <param name="name">Name to be checked.</param>
	/// <param name="allowNull">Whether <c>null</c> is allowed value (e.g. when the name is only optional parameter).</param>
	/// <returns>Error message if the name is not valid, empty string otherwise.</returns>
	public static string Name(string? name, bool allowNull = false) {
		if (name is null) return allowNull ? ErrorMsg.SUCCESS : ErrorMsg.EMPTY_NAME;
		if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
			return ErrorMsg.EMPTY_NAME;
		else return ErrorMsg.SUCCESS;
	}

	/// <summary>
	/// Checks whether the provided image URL is valid, i.e. not <c>null</c> and not empty.
	/// </summary>
	/// <param name="imageUrl">Image URL to be checked.</param>
	/// <param name="allowNull">Whether <c>null</c> is allowed value (e.g. when the image URL is only optional parameter).</param>
	/// <returns>Error message if the image URL is not valid, empty string otherwise.</returns>
	public static string MainImageUrl(string? imageUrl, bool allowNull = false) {
		if (imageUrl is null) return allowNull ? ErrorMsg.SUCCESS : ErrorMsg.EMPTY_NAME;
		if (string.IsNullOrEmpty(imageUrl) || string.IsNullOrWhiteSpace(imageUrl))
			return ErrorMsg.EMPTY_IMAGE_URL;
		else return ErrorMsg.SUCCESS;
	}

	/// <summary>
	/// Checks whether the provided price is valid, i.e. not <c>null</c> and non-negative.
	/// </summary>
	/// <param name="price">Price to be checked.</param>
	/// <param name="allowNull">Whether <c>null</c> is allowed value (e.g. when the price is only optional parameter).</param>
	/// <returns>Error message if the price is not valid, empty string otherwise.</returns>
	public static string Price(decimal? price, bool allowNull = false) {
		if ((!price.HasValue && !allowNull) || (price.HasValue && price.Value < 0))
			return ErrorMsg.NEGATIVE_PRICE;
		else return ErrorMsg.SUCCESS;
	}

	/// <summary>
	/// Checks whether the provided quantity is valid, i.e. not <c>null</c> and non-negative.
	/// </summary>
	/// <param name="quantity">Quantity to be checked.</param>
	/// <param name="allowNull">Whether <c>null</c> is allowed value (e.g. when the quantity is only optional parameter).</param>
	/// <returns>Error message if the quantity is not valid, empty string otherwise.</returns>
	public static string Quantity(int? quantity, bool allowNull = false) {
		if ((!quantity.HasValue && !allowNull) || (quantity.HasValue && quantity.Value < 0))
			return ErrorMsg.NEGATIVE_QUANTITY;
		else return ErrorMsg.SUCCESS;
	}

	/// <summary>
	/// Checks whether the provided quantity will be still valid (i.e. non-negative) after applying the given delta.
	/// </summary>
	/// <param name="quantity">Quantity to be checked.</param>
	/// <param name="delta">Delta to be applied to the given quantity.</param>
	/// <returns>Error message if the quantity would become invalid, empty string otherwise.</returns>
	public static string Quantity(int quantity, int delta) { 
		if (quantity + delta < 0) return ErrorMsg.NEGATIVE_QUANTITY;
		else return ErrorMsg.SUCCESS;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="page"></param>
	/// <returns></returns>
	public static string Page(int page) {
		if (page < 0) return ErrorMsg.NEGATIVE_PAGE;
		else return ErrorMsg.SUCCESS;
	}

	public static string PageSize(int pageSize) {
		if (pageSize <= 0) return ErrorMsg.NON_POSITIVE_PAGE_SIZE;
		else return ErrorMsg.SUCCESS;
	}

}
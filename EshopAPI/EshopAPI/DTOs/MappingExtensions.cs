using EshopAPI.Data;
using EshopAPI.Entities;

namespace EshopAPI.DTOs;

/// <summary>
/// A static class containing methods for mapping between entities and DTOs.
/// </summary>
public static class MappingExtensions {

	/// <summary>
	/// Maps internal product representation to the one used for external communication.
	/// </summary>
	/// <param name="product"><c>Product</c> to be mapped.</param>
	/// <returns><c>ProductDto</c> representation of the given product (with properties copied over).</returns>
	public static ProductDto ToProductDto(this Product product) {
		return new ProductDto() {
			Id = product.Id,
			Name = product.Name,
			MainImageUrl = product.MainImageUrl,
			Description = product.Description,
			Price = product.Price,
			Quantity = product.Quantity
		};
	}

	/// <summary>
	/// Maps data from product creation request to internal product representation.
	/// </summary>
	/// <param name="productCreateDto"><c>ProductCreateDto</c> to be mapped.</param>
	/// <returns><c>Product</c> representation of the given data (with properties copied over).</returns>
	public static Product ToProduct(this ProductCreateDto productCreateDto) {
		return new Product() {
			Name = productCreateDto.Name,
			MainImageUrl = productCreateDto.MainImageUrl,
			Price = productCreateDto.Price,
			Description = productCreateDto.Description,
			Quantity = productCreateDto.Quantity
		};
	}

	/// <summary>
	/// Updates <c>Product</c> with all non-<c>null</c> properties from the given product update request.
	/// </summary>
	/// <param name="product"><c>Product</c> which is to be updated.</param>
	/// <param name="productUpdate"><c>ProductUpdateDto</c> with changes to be applied.</param>
	public static void CombineWith(this Product product, ProductUpdateDto productUpdate) {
		product.Name = productUpdate.Name ?? product.Name;
		product.MainImageUrl = productUpdate.MainImageUrl ?? product.MainImageUrl;
		product.Price = productUpdate.Price ?? product.Price;
		product.Description = productUpdate.Description ?? product.Description;
		product.Quantity = productUpdate.Quantity ?? product.Quantity;
	}

	/// <summary>
	/// Maps data from internal representation of paged response to the one used for external communication.
	/// </summary>
	/// <typeparam name="T">Type of items on the page.</typeparam>
	/// <param name="pagedResponse"><c>PagedResponse</c> to be mapped.</param>
	/// <returns><c>PagedResponseDto</c> representation of the given product (with properties copied over).</returns>
	public static PagedResponseDto<T> ToPagedResponseDto<T>(this PagedResponse<T> pagedResponse) {
		return new PagedResponseDto<T>() {
			Items = pagedResponse.Items,
			PageNumber = pagedResponse.PageNumber,
			PageSize = pagedResponse.PageSize,
			TotalItems = pagedResponse.TotalItems,
			TotalPages = pagedResponse.TotalPages
		};
	}

}

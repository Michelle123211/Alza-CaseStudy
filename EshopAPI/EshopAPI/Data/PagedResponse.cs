namespace EshopAPI.Data;

/// <summary>
/// A class for internal representation of a single page of data with all relevant properties (e.g. page number, items count).
/// </summary>
/// <typeparam name="T">Type of items on the page.</typeparam>
public class PagedResponse<T> {

	/// <summary>Number of the represented page.</summary>
	public int PageNumber { get; set; }

	/// <summary>Maximum possible number of items on the page.</summary>
	public int PageSize { get; set; }

	/// <summary>Total number of items in database.</summary>
	public int TotalItems { get; set; }

	/// <summary>Total number of pages.</summary>
	public int TotalPages { get; set; }

	/// <summary>Data associated with this page.</summary>
	public List<T> Items {  get; set; }


	public PagedResponse(List<T> data, int page, int pageSize, int totalItems) {
		this.Items = data;
		this.PageNumber = page;
		this.PageSize = pageSize;
		this.TotalItems = totalItems;
		this.TotalPages = (int)Math.Ceiling((double)totalItems / PageSize);
	}

}

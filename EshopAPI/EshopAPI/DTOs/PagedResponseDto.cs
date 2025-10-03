namespace EshopAPI.DTOs;

/// <summary>
/// A class representing a single page of data with all relevant properties (e.g. page number, items count)
/// which is returned to the client in corresponding requests.
/// </summary>
/// <typeparam name="T">Type of items on the page.</typeparam>
public class PagedResponseDto<T> {

	/// <summary>Number of the represented page.</summary>
	public int PageNumber { get; init; }

	/// <summary>Maximum possible number of items on the page.</summary>
	public int PageSize { get; init; }

	/// <summary>Total number of items in database.</summary>
	public int TotalItems { get; init; }

	/// <summary>Total number of pages.</summary>
	public int TotalPages { get; init; }

	/// <summary>Data associated with this page.</summary>
	public List<T>? Items { get; init; }

}

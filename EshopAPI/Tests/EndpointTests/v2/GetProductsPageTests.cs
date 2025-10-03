using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using EshopAPI;
using EshopAPI.DTOs;
using EshopAPI.Endpoints.v2;
using Tests.Mocks;
using Microsoft.AspNetCore.Routing;

namespace Tests.EndpointTests.v2;


/// <summary>
/// A set of unit tests targeting "GET /api/v2/products" endpoint.
/// </summary>
public class GetProductsPageTests {

	private readonly ProductsRepositoryMock productsMock = new();
	private readonly LinkGeneratorMock links = new();

	private const int DEFAULT_PAGE = 0;
	private const int DEFAULT_PAGE_SIZE = 10;

	private int TotalItemsCount => productsMock.Products.Count;

	private int GetPageCount(int pageSize) => (int)Math.Ceiling((double)TotalItemsCount / pageSize);
	private int GetItemsOnPage(int pageSize, int page) => Math.Min(pageSize, TotalItemsCount - (page * pageSize));

	public GetProductsPageTests() {
		productsMock.Reset(54);
	}

	#region Default page and page size
	[Fact]
	public async Task When_DefaultPageAndSize_Then_ReturnOk() {
		// Arrange
		var httpContext = PrepareHttpContext(null, null);
		// Act
		var act = await ProductsEndpointsV2.GetProductsPage(httpContext, links, productsMock);
		// Assert
		// ... type of result
		Assert.IsType<Ok<PagedResponseDto<ProductDto>>>(act.Result);
		// ... status code
		var result = act.Result as Ok<PagedResponseDto<ProductDto>>;
		Assert.NotNull(result);
		Assert.Equal(200, result.StatusCode);
	}
	[Fact]
	public async Task When_DefaultPageAndSize_Then_SetCorrectHeader() {
		// Arrange
		var httpContext = PrepareHttpContext(null, null);
		// Act
		var act = await ProductsEndpointsV2.GetProductsPage(httpContext, links, productsMock);
		// Assert
		// ... Link header
		string linkHeader = httpContext.Response.Headers.Link.ToString();
		Assert.Contains("self", linkHeader);
		Assert.Contains("first", linkHeader);
		Assert.Contains("last", linkHeader);
		// ... custom headers
		Assert.Equal($"{DEFAULT_PAGE}", httpContext.Response.Headers[Header.PAGE]);
		Assert.Equal($"{DEFAULT_PAGE_SIZE}", httpContext.Response.Headers[Header.PAGE_SIZE]);
		Assert.Equal($"{GetPageCount(DEFAULT_PAGE_SIZE)}", httpContext.Response.Headers[Header.TOTAL_PAGES]);
		Assert.Equal($"{TotalItemsCount}", httpContext.Response.Headers[Header.TOTAL_ITEMS]);
	}
	[Fact]
	public async Task When_DefaultPageAndSize_Then_ReturnPage() {
		// Arrange
		var httpContext = PrepareHttpContext(null, null);
		// Act
		var act = await ProductsEndpointsV2.GetProductsPage(httpContext, links, productsMock);
		// Assert
		// ... output data
		var result = act.Result as Ok<PagedResponseDto<ProductDto>>;
		Assert.NotNull(result);
		Assert.NotNull(result.Value);
		Assert.Equal(DEFAULT_PAGE, result.Value.PageNumber);
		Assert.Equal(DEFAULT_PAGE_SIZE, result.Value.PageSize);
		Assert.Equal(TotalItemsCount, result.Value.TotalItems);
		Assert.Equal(GetPageCount(DEFAULT_PAGE_SIZE), result.Value.TotalPages);
		Assert.NotNull(result.Value.Items);
		Assert.Equal(GetItemsOnPage(DEFAULT_PAGE_SIZE, DEFAULT_PAGE), result.Value.Items.Count);
	}
	#endregion

	#region Default page, specified page size
	[Theory]
	[InlineData(4)]
	[InlineData(7)]
	[InlineData(12)]
	[InlineData(100)] // only one page
	public async Task When_DefaultPageAndSpecifiedSize_Then_ReturnOk(int pageSize) {
		// Arrange
		var httpContext = PrepareHttpContext(null, pageSize);
		// Act
		var act = await ProductsEndpointsV2.GetProductsPage(httpContext, links, productsMock, pageSize: pageSize);
		// Assert
		// ... type of result
		Assert.IsType<Ok<PagedResponseDto<ProductDto>>>(act.Result);
		// ... status code
		var result = act.Result as Ok<PagedResponseDto<ProductDto>>;
		Assert.NotNull(result);
		Assert.Equal(200, result.StatusCode);
	}
	[Theory]
	[InlineData(4)]
	[InlineData(7)]
	[InlineData(12)]
	public async Task When_DefaultPageAndSpecifiedSize_Then_SetCorrectHeader(int pageSize) {
		// Arrange
		var httpContext = PrepareHttpContext(null, pageSize);
		// Act
		var act = await ProductsEndpointsV2.GetProductsPage(httpContext, links, productsMock, pageSize: pageSize);
		// Assert
		// ... Link header
		string linkHeader = httpContext.Response.Headers.Link.ToString();
		Assert.Contains("self", linkHeader);
		Assert.Contains("first", linkHeader);
		Assert.Contains("last", linkHeader);
		// ... custom headers
		Assert.Equal($"{DEFAULT_PAGE}", httpContext.Response.Headers[Header.PAGE]);
		Assert.Equal($"{pageSize}", httpContext.Response.Headers[Header.PAGE_SIZE]);
		Assert.Equal($"{GetPageCount(pageSize)}", httpContext.Response.Headers[Header.TOTAL_PAGES]);
		Assert.Equal($"{TotalItemsCount}", httpContext.Response.Headers[Header.TOTAL_ITEMS]);
	}
	[Theory]
	[InlineData(4)]
	[InlineData(7)]
	[InlineData(12)]
	public async Task When_DefaultPageAndSpecifiedSize_Then_ReturnPage(int pageSize) {
		// Arrange
		var httpContext = PrepareHttpContext(null, pageSize);
		// Act
		var act = await ProductsEndpointsV2.GetProductsPage(httpContext, links, productsMock, pageSize: pageSize);
		// Assert
		// ... output data
		var result = act.Result as Ok<PagedResponseDto<ProductDto>>;
		Assert.NotNull(result);
		Assert.NotNull(result.Value);
		Assert.Equal(DEFAULT_PAGE, result.Value.PageNumber);
		Assert.Equal(pageSize, result.Value.PageSize);
		Assert.Equal(TotalItemsCount, result.Value.TotalItems);
		Assert.Equal(GetPageCount(pageSize), result.Value.TotalPages);
		Assert.NotNull(result.Value.Items);
		Assert.Equal(Math.Max(0, GetItemsOnPage(pageSize, DEFAULT_PAGE)), result.Value.Items.Count);
	}
	#endregion

	#region Specified page, default page size
	[Theory]
	[InlineData(0)]
	[InlineData(3)]
	[InlineData(5)] // last page
	[InlineData(100)] // out of bounds
	public async Task When_SpecifiedPageAndDefaultSize_Then_ReturnOk(int page) {
		// Arrange
		var httpContext = PrepareHttpContext(page, null);
		// Act
		var act = await ProductsEndpointsV2.GetProductsPage(httpContext, links, productsMock, page: page);
		// Assert
		// ... type of result
		Assert.IsType<Ok<PagedResponseDto<ProductDto>>>(act.Result);
		// ... status code
		var result = act.Result as Ok<PagedResponseDto<ProductDto>>;
		Assert.NotNull(result);
		Assert.Equal(200, result.StatusCode);
	}
	[Theory]
	[InlineData(0)]
	[InlineData(3)]
	[InlineData(5)] // last page
	[InlineData(100)] // out of bounds
	public async Task When_SpecifiedPageAndDefaultSize_Then_SetCorrectHeader(int page) {
		// Arrange
		var httpContext = PrepareHttpContext(page, null);
		// Act
		var act = await ProductsEndpointsV2.GetProductsPage(httpContext, links, productsMock, page: page);
		// Assert
		// ... Link header
		string linkHeader = httpContext.Response.Headers.Link.ToString();
		Assert.Contains("self", linkHeader);
		Assert.Contains("first", linkHeader);
		Assert.Contains("last", linkHeader);
		// ... custom headers
		Assert.Equal($"{page}", httpContext.Response.Headers[Header.PAGE]);
		Assert.Equal($"{DEFAULT_PAGE_SIZE}", httpContext.Response.Headers[Header.PAGE_SIZE]);
		Assert.Equal($"{GetPageCount(DEFAULT_PAGE_SIZE)}", httpContext.Response.Headers[Header.TOTAL_PAGES]);
		Assert.Equal($"{TotalItemsCount}", httpContext.Response.Headers[Header.TOTAL_ITEMS]);
	}
	[Theory]
	[InlineData(0)]
	[InlineData(3)]
	[InlineData(5)] // last page
	[InlineData(100)] // out of bounds
	public async Task When_SpecifiedPageAndDefaultSize_Then_ReturnPage(int page) {
		// Arrange
		var httpContext = PrepareHttpContext(page, null);
		// Act
		var act = await ProductsEndpointsV2.GetProductsPage(httpContext, links, productsMock, page: page);
		// Assert
		// ... output data
		var result = act.Result as Ok<PagedResponseDto<ProductDto>>;
		Assert.NotNull(result);
		Assert.NotNull(result.Value);
		Assert.Equal(page, result.Value.PageNumber);
		Assert.Equal(DEFAULT_PAGE_SIZE, result.Value.PageSize);
		Assert.Equal(TotalItemsCount, result.Value.TotalItems);
		Assert.Equal(GetPageCount(DEFAULT_PAGE_SIZE), result.Value.TotalPages);
		Assert.NotNull(result.Value.Items);
		Assert.Equal(Math.Max(0, GetItemsOnPage(DEFAULT_PAGE_SIZE, page)), result.Value.Items.Count);
	}
	#endregion

	#region Specified page and page size
	[Theory]
	[InlineData(0, 3)]
	[InlineData(4, 3)]
	[InlineData(7, 7)] // last page
	[InlineData(0, 100)] // only one page
	[InlineData(100, 10)] // out of bounds
	public async Task When_SpecifiedPageAndSize_Then_ReturnOk(int page, int pageSize) {
		// Arrange
		var httpContext = PrepareHttpContext(page, pageSize);
		// Act
		var act = await ProductsEndpointsV2.GetProductsPage(httpContext, links, productsMock, page: page, pageSize: pageSize);
		// Assert
		// ... type of result
		Assert.IsType<Ok<PagedResponseDto<ProductDto>>>(act.Result);
		// ... status code
		var result = act.Result as Ok<PagedResponseDto<ProductDto>>;
		Assert.NotNull(result);
		Assert.Equal(200, result.StatusCode);
	}
	[Theory]
	[InlineData(0, 3)]
	[InlineData(4, 3)]
	[InlineData(7, 7)] // last page
	[InlineData(0, 100)] // only one page
	[InlineData(100, 10)] // out of bounds
	public async Task When_SpecifiedPageAndSize_Then_SetCorrectHeader(int page, int pageSize) {
		// Arrange
		var httpContext = PrepareHttpContext(page, pageSize);
		// Act
		var act = await ProductsEndpointsV2.GetProductsPage(httpContext, links, productsMock, page: page, pageSize: pageSize);
		// Assert
		// ... Link header
		string linkHeader = httpContext.Response.Headers.Link.ToString();
		Assert.Contains("self", linkHeader);
		Assert.Contains("first", linkHeader);
		Assert.Contains("last", linkHeader);
		// ... custom headers
		Assert.Equal($"{page}", httpContext.Response.Headers[Header.PAGE]);
		Assert.Equal($"{pageSize}", httpContext.Response.Headers[Header.PAGE_SIZE]);
		Assert.Equal($"{GetPageCount(pageSize)}", httpContext.Response.Headers[Header.TOTAL_PAGES]);
		Assert.Equal($"{TotalItemsCount}", httpContext.Response.Headers[Header.TOTAL_ITEMS]);
	}
	[Theory]
	[InlineData(0, 3)]
	[InlineData(4, 3)]
	[InlineData(7, 7)] // last page
	[InlineData(0, 100)] // only one page
	[InlineData(100, 10)] // out of bounds
	public async Task When_SpecifiedPageAndSize_Then_ReturnPage(int page, int pageSize) {
		// Arrange
		var httpContext = PrepareHttpContext(page, pageSize);
		// Act
		var act = await ProductsEndpointsV2.GetProductsPage(httpContext, links, productsMock, page: page, pageSize: pageSize);
		// Assert
		// ... output data
		var result = act.Result as Ok<PagedResponseDto<ProductDto>>;
		Assert.NotNull(result);
		Assert.NotNull(result.Value);
		Assert.Equal(page, result.Value.PageNumber);
		Assert.Equal(pageSize, result.Value.PageSize);
		Assert.Equal(TotalItemsCount, result.Value.TotalItems);
		Assert.Equal(GetPageCount(pageSize), result.Value.TotalPages);
		Assert.NotNull(result.Value.Items);
		Assert.Equal(Math.Max(0, GetItemsOnPage(pageSize, page)), result.Value.Items.Count);
	}
	#endregion

	#region Link header
	[Fact]
	public async Task When_DefaultPage_Then_ReturnNoPrevLink() {
		// Arrange
		var httpContext = PrepareHttpContext(null, null);
		// Act
		var act = await ProductsEndpointsV2.GetProductsPage(httpContext, links, productsMock);
		// Assert
		// ... Link header
		string linkHeader = httpContext.Response.Headers.Link.ToString();
		Assert.DoesNotContain("prev", linkHeader);
		Assert.Contains("next", linkHeader);
	}
	[Fact]
	public async Task When_FirstPage_Then_ReturnNoPrevLink() {
		// Arrange
		var httpContext = PrepareHttpContext(0, null);
		// Act
		var act = await ProductsEndpointsV2.GetProductsPage(httpContext, links, productsMock, page: 0);
		// Assert
		// ... Link header
		string linkHeader = httpContext.Response.Headers.Link.ToString();
		Assert.DoesNotContain("prev", linkHeader);
		Assert.Contains("next", linkHeader);
	}
	[Fact]
	public async Task When_MiddlePage_Then_ReturnBothLinks() {
		// Arrange
		int page = 2;
		var httpContext = PrepareHttpContext(page, null);
		// Act
		var act = await ProductsEndpointsV2.GetProductsPage(httpContext, links, productsMock, page: page);
		// Assert
		// ... Link header
		string linkHeader = httpContext.Response.Headers.Link.ToString();
		Assert.Contains("prev", linkHeader);
		Assert.Contains("next", linkHeader);
	}
	[Fact]
	public async Task When_LastPage_Then_ReturnNoNextLink() {
		// Arrange
		int page = GetPageCount(DEFAULT_PAGE_SIZE) - 1;
		var httpContext = PrepareHttpContext(page, null);
		// Act
		var act = await ProductsEndpointsV2.GetProductsPage(httpContext, links, productsMock, page: page);
		// Assert
		// ... Link header
		string linkHeader = httpContext.Response.Headers.Link.ToString();
		Assert.Contains("prev", linkHeader);
		Assert.DoesNotContain("next", linkHeader);
	}
	#endregion

	#region Invalid values
	[Fact]
	public async Task When_PageNumberIsNegative_Then_ReturnBadRequest() {
		// Arrange
		int page = -5;
		var httpContext = PrepareHttpContext(page, null);
		// Act
		var act = await ProductsEndpointsV2.GetProductsPage(httpContext, links, productsMock, page: page);
		// Assert
		// ... type of result
		Assert.IsType<BadRequest<string>>(act.Result);
		// ... status code
		var result = act.Result as BadRequest<string>;
		Assert.NotNull(result);
		Assert.Equal(400, result.StatusCode);
	}

	[Theory]
	[InlineData(-5)]
	[InlineData(0)]
	public async Task When_PageSizeIsNotPositive_Then_ReturnBadRequest(int pageSize) {
		// Arrange
		var httpContext = PrepareHttpContext(null, pageSize);
		// Act
		var act = await ProductsEndpointsV2.GetProductsPage(httpContext, links, productsMock, pageSize: pageSize);
		// Assert
		// ... type of result
		Assert.IsType<BadRequest<string>>(act.Result);
		// ... status code
		var result = act.Result as BadRequest<string>;
		Assert.NotNull(result);
		Assert.Equal(400, result.StatusCode);
	}
	#endregion


	private HttpContext PrepareHttpContext(int? page, int? pageSize, string method = "GET") {
		var httpContext = new DefaultHttpContext();
		httpContext.Request.Scheme = "https";
		httpContext.Request.Host = new HostString("localhost", 7066);
		httpContext.Request.Path = "/api/v2/products";
		httpContext.Request.Method = "GET";
		string pageQuery = (page.HasValue ? $"page={page}" : "");
		string pageSizeQuery = (pageSize.HasValue ? $"page_size={pageSize}" : "");
		httpContext.Request.QueryString = new QueryString(
			$"{(page.HasValue || pageSize.HasValue ? "?" : "")}{pageQuery}{(page.HasValue && pageSize.HasValue ? "&" : "")}{pageSizeQuery}"
		);
		return httpContext;
	}
}

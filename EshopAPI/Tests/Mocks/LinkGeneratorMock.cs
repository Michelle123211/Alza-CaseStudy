using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Tests.Mocks;

/// <summary>
/// A class derived from <c>LinkGenerator</c> to be used during unit tests.
/// All abstract methods currently return empty string because the result is not checked anywhere.
/// </summary>
internal class LinkGeneratorMock : LinkGenerator {
	public override string? GetPathByAddress<TAddress>(HttpContext httpContext, TAddress address, RouteValueDictionary values, RouteValueDictionary? ambientValues = null, PathString? pathBase = null, FragmentString fragment = default, LinkOptions? options = null) {
		return string.Empty;
	}

	public override string? GetPathByAddress<TAddress>(TAddress address, RouteValueDictionary values, PathString pathBase = default, FragmentString fragment = default, LinkOptions? options = null) {
		return string.Empty;
	}

	public override string? GetUriByAddress<TAddress>(HttpContext httpContext, TAddress address, RouteValueDictionary values, RouteValueDictionary? ambientValues = null, string? scheme = null, HostString? host = null, PathString? pathBase = null, FragmentString fragment = default, LinkOptions? options = null) {
		return string.Empty;
	}

	public override string? GetUriByAddress<TAddress>(TAddress address, RouteValueDictionary values, string scheme, HostString host, PathString pathBase = default, FragmentString fragment = default, LinkOptions? options = null) {
		return string.Empty;
	}

}

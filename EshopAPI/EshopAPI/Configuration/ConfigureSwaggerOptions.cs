using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EshopAPI.Configuration;

/// <summary>
/// A class handling configuration of Swagger documentation.
/// Allows displaying different versions of API.
/// </summary>
public class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions> {

	private readonly IApiVersionDescriptionProvider provider;

	public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) {
		this.provider = provider;
	}

	public void Configure(string? name, SwaggerGenOptions options) {
		foreach (var description in provider.ApiVersionDescriptions) {
			var openApiInfo = new OpenApiInfo() {
				Title = $"E-shop API",
				Version = $"v{description.ApiVersion}"
			};
			options.SwaggerDoc(description.GroupName, openApiInfo);
		}
	}

	public void Configure(SwaggerGenOptions options) {
		Configure(options);
	}
}

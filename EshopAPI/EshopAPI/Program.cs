using Asp.Versioning;
using EshopAPI.Configuration;
using EshopAPI.Endpoints.v1;
using EshopAPI.Endpoints.v2;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure versioning
builder.Services.AddApiVersioning(opts => {
	opts.DefaultApiVersion = new ApiVersion(1);
	opts.ReportApiVersions = true;
	opts.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddApiExplorer(opts => {
	opts.GroupNameFormat = "'v'V";
	opts.SubstituteApiVersionInUrl = true;
});
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();


var app = builder.Build();


// Distinguish between different API versions
var apiVersionSet = app.NewApiVersionSet()
	.HasApiVersion(new ApiVersion(1))
	.HasApiVersion(new ApiVersion(2))
	.ReportApiVersions()
	.Build();
var versionedGroup = app.MapGroup("/api/v{version:apiVersion}")
	.WithApiVersionSet(apiVersionSet);


// Register endpoints
versionedGroup.RegisterProductsEndpointsV1();
versionedGroup.RegisterProductsEndpointsV2();


if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI(options => {
		var descriptions = app.DescribeApiVersions();
		foreach (var desc in descriptions) {
			options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", desc.GroupName.ToUpperInvariant());
		}
	});
}

app.UseHttpsRedirection();

app.Run();
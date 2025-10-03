using Asp.Versioning;
using EshopAPI.Configuration;
using EshopAPI.Data;
using EshopAPI.DTOs;
using EshopAPI.Endpoints.v1;
using EshopAPI.Endpoints.v2;
using EshopAPI.Entities;
using EshopAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Asynchronous queue
builder.Services.AddSingleton<IProductUpdateQueue>(opts => new ProductUpdateQueue());
builder.Services.AddSingleton<ConcurrentDictionary<string, QueuedUpdateResult>>(); // storing status of update requests
builder.Services.AddHostedService<ProductUpdateService>();


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


// Configure EF Core (with SQLite database)
builder.Services.AddDbContext<ProductsDb>(opts => {
	opts.UseSqlite(builder.Configuration.GetConnectionString("DatabaseConnection"));
});
builder.Services.AddScoped<IProductsRepository, ProductsRepository>();
if (builder.Environment.IsDevelopment())
	builder.Services.AddDatabaseDeveloperPageExceptionFilter(); // enable displaying database-related exceptions


var app = builder.Build();


// Make sure DB exists
using (var scope = app.Services.CreateScope()) {
	var db = scope.ServiceProvider.GetRequiredService<ProductsDb>();
	db.Database.EnsureCreated();
	if (!db.Products.Any())
		db.Seed();
}


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
			options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", $"API {desc.GroupName.ToUpperInvariant()}");
		}
	});
}

app.UseHttpsRedirection();

app.Run();
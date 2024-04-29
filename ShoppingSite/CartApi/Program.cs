using CartApi.ProductsApiClient;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<CartApi.Models.CartsContext>(
	options =>
	{
		options.UseSqlServer(builder.Configuration.GetConnectionString("CartsConnection"));
	});

builder.Services.AddControllers().AddJsonOptions(options =>
{
	options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
	options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

});

builder.Services.AddHttpClient("ProductsApiClient", httpClient =>
{
	httpClient.DefaultRequestHeaders
			  .Accept
			  .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
	httpClient.BaseAddress = new Uri("https://localhost:7154");
});

builder.Services.AddScoped<IProductClient, ProductClient>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

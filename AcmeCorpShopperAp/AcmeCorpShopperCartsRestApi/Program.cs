using AcmeCorpShopperCartsRestApi.ApiClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AcmeCorpShopperCartsRestApi.Models.CartsAcmeContext>(
	options =>
	{
		options.UseSqlServer(builder.Configuration.GetConnectionString("CartsConnection"));
	}
	);


builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("AcmeCorpProductApiClient", httpClient =>
{
	httpClient.DefaultRequestHeaders
			  .Accept
			  .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
	httpClient.BaseAddress = new Uri("https://localhost:7269");
});

builder.Services.AddScoped<IProductClient, ProductClient>();

builder.Services.AddControllers();
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

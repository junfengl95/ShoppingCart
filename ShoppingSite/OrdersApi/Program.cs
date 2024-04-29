using Microsoft.EntityFrameworkCore;
using OrdersApi.ApiClient;

var builder = WebApplication.CreateBuilder(args);

//Add services to the container.
builder.Services.AddDbContext<OrdersApi.Models.OrdersContext>(
	options =>
	{
		options.UseSqlServer(builder.Configuration.GetConnectionString("OrderConnection"));
	});

builder.Services.AddHttpClient("CartApi",
	httpClient =>
	{
		httpClient.DefaultRequestHeaders
				  .Accept
				  .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
		httpClient.BaseAddress = new Uri("https://localhost:7227");
	});

builder.Services.AddScoped<ICartClient, CartClient>();

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

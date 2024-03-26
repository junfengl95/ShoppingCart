using AcmeCorpShopperUIWebApp.ApiClient;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromSeconds(30);
	options.Cookie.Name = "CartIdCookie";
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

builder.Services.AddHttpClient("AcmeCorpProductApiClient", // Name of client 
	httpClient =>
	{
		httpClient.DefaultRequestHeaders
				  .Accept
				  .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
		httpClient.BaseAddress = new Uri("https://localhost:7269");
	});

builder.Services.AddHttpClient("AcmeCorpCartApiClient", // Name of client 
	httpClient =>
	{
		httpClient.DefaultRequestHeaders
				  .Accept
				  .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
		httpClient.BaseAddress = new Uri("https://localhost:7146");
	});

builder.Services.AddHttpClient("AcmeCorpOrderApiClient", // Name of client 
    httpClient =>
    {
        httpClient.DefaultRequestHeaders
                  .Accept
                  .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.BaseAddress = new Uri("https://localhost:7167");
    });

builder.Services.AddScoped<IAcmeCorpClient, AcmeCorpClient>();

builder.Services.AddCors(c =>
{
	c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors(options => options.AllowAnyOrigin());

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=AcmeCorpShopper}/{action=Index}/{id?}");

app.Run();

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopperUI.ApiClient;
using ShopperUI.Areas.Identity.Data;
using ShopperUI.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ShopperUIContextConnection") ?? throw new InvalidOperationException("Connection string 'ShopperUIContextConnection' not found.");

builder.Services.AddDbContext<ShopperUIContext>(options => options.UseSqlServer(connectionString));

// RequiredConfirmedAccount if true will send a verification to the provided email.
builder.Services.AddDefaultIdentity<ShopperUIUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<ShopperUIContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireUppercase = false;
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient("ProductApi",
    httpClient =>
    {
        httpClient.DefaultRequestHeaders
                  .Accept
                  .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.BaseAddress = new Uri("https://localhost:7154");
    });

builder.Services.AddHttpClient("CartApi",
    httpClient =>
    {
        httpClient.DefaultRequestHeaders
                  .Accept
                  .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.BaseAddress = new Uri("https://localhost:7227");
    });

builder.Services.AddHttpClient("OrdersApi",
	httpClient =>
	{
		httpClient.DefaultRequestHeaders
				  .Accept
				  .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
		httpClient.BaseAddress = new Uri("https://localhost:7016");
	});

builder.Services.AddScoped<IShopperUIClient, ShopperUIClient>();

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

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

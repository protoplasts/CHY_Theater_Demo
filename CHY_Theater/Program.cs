using CHY_Theater.Areas.Admin.Service;
using CHY_Theater.Areas.Identity.Authorize;
using CHY_Theater.Areas.Identity.Services;
using CHY_Theater.Service.IService;
using CHY_Theater.Service;
using CHY_Theater.Views.Shared.Filters;
using CHY_Theater_DataAcess.Data;
using CHY_Theater_Models.Models;
using CHY_Theater_Utitly;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
//Add API Service Called
builder.Services.AddHttpClient<MovieService>();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<Theater_ProjectDbContext>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<Theater_ProjectDbContext>().AddDefaultTokenProviders();
builder.Services.AddTransient<IEmailSender, EmailSender>();
//// Add JwtToken service configuration section
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<BarcodeService>();
builder.Services.AddScoped<IRewardPointService, RewardPointService>();
builder.Services.AddScoped<IUserCouponService, UserCouponService>();
//客製化的一些密碼限制
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.SignIn.RequireConfirmedEmail = false;

});

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("Admin", policy => policy.RequireRole(SD.Admin));
    opt.AddPolicy("AdminAndUser", policy => policy.RequireRole(SD.Admin).RequireRole(SD.User));
    opt.AddPolicy("AdminRole_CreateClaim", policy => policy.RequireRole(SD.Admin).RequireClaim("Create", "True"));
    opt.AddPolicy("AdminRole_CreateEditDeleteClaim", policy => policy
                    .RequireRole(SD.Admin)
                    .RequireClaim("Create", "True")
                    .RequireClaim("Edit", "True")
                    .RequireClaim("Delete", "True")
                 );

});
builder.Services.ConfigureApplicationCookie(opt =>
{
    opt.AccessDeniedPath = new PathString("/Account/NoAccess");
    opt.LoginPath = "/Identity/Account/Login";
    opt.LogoutPath = "/Account/Logout";
});
// Add Microsoft authentication

builder.Services.AddAuthentication().AddMicrosoftAccount(opt =>
{
	opt.ClientId = "95de3a24-950e-40af-b746-55671c7b109a";
	opt.ClientSecret = "vub8Q~mZPp0ksucpN2oqpCQ5Lk7rDXrifKjjFaIt";
});
// Add Google authentication
builder.Services.AddAuthentication()
	.AddGoogle(options =>
	{
		IConfigurationSection googleAuthNSection =
			builder.Configuration.GetSection("Authentication:Google");

		options.ClientId = googleAuthNSection["ClientId"];
		options.ClientSecret = googleAuthNSection["ClientSecret"];
	});
// Add Facebook authentication
builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
    options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
});
// Session configuration
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
//Register the Exception Filter
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new ApiExceptionFilter());
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
app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();

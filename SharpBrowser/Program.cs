using Microsoft.AspNetCore.Authentication.Cookies;
using SharpBrowser;


#region set passwords
Console.Write("Read Secret Code: ");
Auth.ReadSecret = Console.ReadLine() ?? "";
Console.Write("Write Secret Code: ");
Auth.WriteSecret = Console.ReadLine() ?? "";
#endregion


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options => {
		options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
		options.SlidingExpiration = true;
		options.LoginPath = "/Auth/Login";
		options.LogoutPath = "/Auth/Logout";
		options.AccessDeniedPath = "/Auth/Forbidden";
	});
builder.Services.AddAuthorization(options => {
	options.AddPolicy(
		Policy.ReadAccess,
		policy => policy.RequireClaim(Auth.AccessLevel, Auth.Read, Auth.Write));
	options.AddPolicy(
		Policy.WriteAccess,
		policy => policy.RequireClaim(Auth.AccessLevel, Auth.Write));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment()) {
	app.UseExceptionHandler("/Home/Error");
	// app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Browser}/{action=Index}"
);

app.Run();

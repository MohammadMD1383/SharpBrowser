using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SharpBrowser.Models;

namespace SharpBrowser.Controllers;

public class AuthController : Controller {
	private readonly LoginViewModel _loginViewModel = new() {
		Readable = Auth.ReadSecret != "",
		Writable = Auth.WriteSecret != ""
	};

	[HttpGet]
	public IActionResult Login() {
		return View(_loginViewModel);
	}

	[HttpPost]
	public async Task<IActionResult> Login([FromForm] string secret, [FromQuery] string? returnUrl = null) {
		if (Auth.ReadSecret == "") return View(_loginViewModel);
		
		var claims = new List<Claim>();
		
		if (Auth.WriteSecret != "" && secret == Auth.WriteSecret)
			claims.Add(new Claim(Auth.AccessLevel, Auth.Write));
		else if (secret == Auth.ReadSecret)
			claims.Add(new Claim(Auth.AccessLevel, Auth.Read));
		else return View(_loginViewModel);

		var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
		var principal = new ClaimsPrincipal(identity);
		var properties = new AuthenticationProperties { IsPersistent = true };

		await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);

		return returnUrl != null ? Redirect(returnUrl) : RedirectToAction("Index", "Browser");
	}

	public async Task<IActionResult> Logout() {
		await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		return RedirectToAction("Login");
	}

	public IActionResult Forbidden() {
		return View();
	}
}

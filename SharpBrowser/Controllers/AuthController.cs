using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SharpBrowser.Models;

namespace SharpBrowser.Controllers;

public class AuthController : Controller {
	private readonly string?        _readSecret;
	private readonly string?        _writeSecret;
	private readonly LoginViewModel _loginViewModel;

	public AuthController(IConfiguration configuration) {
		_readSecret = configuration["Access:Read"];
		_writeSecret = configuration["Access:Write"];
		
		_loginViewModel = new LoginViewModel {
			Readable = _readSecret != null,
			Writable = _writeSecret != null
		};
	}

	[HttpGet]
	public IActionResult Login() {
		return View(_loginViewModel);
	}

	[HttpPost]
	public async Task<IActionResult> Login([FromForm] string secret, [FromQuery] string? returnUrl = null) {
		if (_readSecret == null) return View(_loginViewModel);
		
		var claims = new List<Claim>();
		
		if (secret == _readSecret)
			claims.Add(new Claim(Auth.AccessLevel, Auth.Read));
		else if (_writeSecret != null && secret == _writeSecret)
			claims.Add(new Claim(Auth.AccessLevel, Auth.Write));
		else return View(_loginViewModel);

		var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
		var principal = new ClaimsPrincipal(identity);
		var properties = new AuthenticationProperties { IsPersistent = true };

		await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);

		return returnUrl != null ? Redirect(returnUrl) : RedirectToAction("Index", "Browser");
	}

	public async Task<IActionResult> Logout() {
		await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		return RedirectToAction("Index", "Browser");
	}
}

﻿using System.IO.Compression;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpBrowser.Models;
using SharpBrowser.PlatformSpecific;

namespace SharpBrowser.Controllers;

[Authorize(policy: Policy.ReadAccess)]
public class BrowserController : Controller {
	public IActionResult Index([FromQuery] string path = "/") {
		BrowserViewModel.Entry[] directories;
		BrowserViewModel.Entry[] files;

		if (Platform.IsWindows && path == "/") {
			directories = DriveInfo.GetDrives().Where(drive => drive.IsReady).Select(
				drive => new BrowserViewModel.Entry { FullPath = drive.RootDirectory.Name }
			).ToArray();

			files = Array.Empty<BrowserViewModel.Entry>();
		} else {
			directories = Directory.GetDirectories(path).Select(
				dir => new BrowserViewModel.Entry { FullPath = dir }
			).ToArray();

			files = Directory.GetFiles(path).Select(
				file => new BrowserViewModel.Entry {
					FullPath = file,
					Size = new FileInfo(file).Length
				}
			).ToArray();
		}

		var model = new BrowserViewModel {
			Root = Path.GetPathRoot(path)!,
			FullPath = path,
			Path = path.Trim('/').Split('/').Where(p => p != "").ToArray(),
			Directories = directories,
			Files = files,
			HasWritePermission = HttpContext.User.HasClaim(Auth.AccessLevel, Auth.Write)
		};

		return View("Index", model);
	}

	[Route("view")]
	public IActionResult ViewFile([FromQuery] string path) {
		return File(new StreamReader(path).BaseStream, MimeTypes.GetMimeType(path));
	}

	public IActionResult Download([FromQuery] string path) {
		var fileName = Path.GetFileName(path) + ".zip";
		StreamReader reader;

		if (System.IO.File.GetAttributes(path).HasFlag(FileAttributes.Directory)) {
			var dummyFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			ZipFile.CreateFromDirectory(path, dummyFile);
			reader = new StreamReader(dummyFile);
		} else {
			reader = new StreamReader(path);
		}

		return File(reader.BaseStream, MimeTypes.GetMimeType(fileName), fileName);
	}

	[Authorize(policy: Policy.WriteAccess)]
	public IActionResult Upload([FromQuery] string destination) {
		return View(new UploadViewModel { Destination = destination });
	}

	[HttpPost]
	[Authorize(policy: Policy.WriteAccess)]
	public async Task<IActionResult> Upload([FromForm] string destination, [FromForm] IFormFile file) {
		await using var stream = System.IO.File.Create(Path.Combine(destination, file.FileName));
		await file.CopyToAsync(stream);
		return RedirectToAction("Index", new { path = destination });
	}
}

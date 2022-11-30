using System.Text.RegularExpressions;

namespace SharpBrowser.Models;

public partial class BrowserViewModel {
	public required string   Root               { get; init; }
	public required string   FullPath           { get; set; }
	public required string[] Path               { get; init; }
	public required Entry[]  Directories        { get; init; }
	public required Entry[]  Files              { get; init; }
	public required bool     HasWritePermission { get; init; }

	public partial class Entry {
		private static readonly string[] Suffixes = { "B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB" };

		public required string FullPath { get; init; }
		public          long?  Size     { get; init; }

		public string Path() {
			if (Platform.IsWindows && DriveNamePattern().IsMatch(FullPath))
				return FullPath.Remove(FullPath.Length - 1);
			return System.IO.Path.GetFileName(FullPath);
		}

		public string? HumanReadableSize() {
			switch (Size) {
				case null: return null;
				case 0: return $"0 {Suffixes[0]}";
			}

			var place = Convert.ToInt32(Math.Floor(Math.Log((long)Size, 1024)));
			var num = Math.Round((long)Size / Math.Pow(1024, place), 2);
			return $"{num} {Suffixes[place]}";
		}

		[GeneratedRegex(@"^[A-Z]:\\$")]
		private static partial Regex DriveNamePattern();
	}
}

namespace SharpBrowser.Models;

public class BrowserViewModel {
	public string   Root        { get; set; }
	public string[] Path        { get; set; }
	public Entry[]  Directories { get; set; }
	public Entry[]  Files       { get; set; }

	public class Entry {
		private static readonly string[] Suffixes = { "B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB" };

		public required string FullPath { get; init; }
		public          string Path     => System.IO.Path.GetFileName(FullPath);
		public          long?  Size     { get; init; }

		public string? HumanReadableSize() {
			switch (Size) {
				case null: return null;
				case 0: return "0" + Suffixes[0];
			}

			var place = Convert.ToInt32(Math.Floor(Math.Log((long)Size, 1024)));
			var num = Math.Round((long)Size / Math.Pow(1024, place), 2);
			return num + Suffixes[place];
		}
	}
}

namespace SharpBrowser;

public static class Auth {
	public static string ReadSecret  { get; set; } = null!;
	public static string WriteSecret { get; set; } = null!;

	public const string AccessLevel = "AccessLevel";
	public const string Read        = "Read";
	public const string Write       = "Write";
}

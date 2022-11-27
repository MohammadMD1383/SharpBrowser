using System.Runtime.InteropServices;

namespace SharpBrowser.PlatformSpecific;

public static class Platform {
	public static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
}

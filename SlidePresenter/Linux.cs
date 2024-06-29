#if OS_LINUX
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ControllerSlidePresenter;

public static class Linux {
	public static bool CanRun() {
		if (getuid() != 0) {
			Console.WriteLine("On Linux you need tu run as 'sudo'");
			return false;
		}
		if (!IsXdoToolInstalled())
			return false;

		return true;
	}

	private static bool IsXdoToolInstalled() {
		string result = RunShellCommand("which xdotool");
		if (!string.IsNullOrEmpty(result))
			return true;

		Console.WriteLine("xdotool is not installed.");
		return false;
	}

	private static string RunShellCommand(string command) {
		try {
			ProcessStartInfo procStartInfo = new("bash", "-c \"" + command + "\"") {
				RedirectStandardOutput = true,
				UseShellExecute = false,
				CreateNoWindow = true,
			};

			using Process process = new();
			process.StartInfo = procStartInfo;
			process.Start();

			string result = process.StandardOutput.ReadToEnd();
			process.WaitForExit();

			return result.Trim();
		}
		catch (Exception ex) {
			Console.WriteLine("Error: " + ex.Message);
			return string.Empty;
		}
	}


	[DllImport("libc")]
	private static extern uint getuid();
}
#endif

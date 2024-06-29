#if OS_LINUX
using System.Diagnostics;

namespace ControllerSlidePresenter.InputSender;

public class LinuxInputSender : IInputSender {
	private const string PageUp = "0xff55";
	private const string PageDown = "0xff56";

	public void NextSlide() {
		SendKeys(PageDown);
	}

	public void PreviousSlide() {
		SendKeys(PageUp);
	}

	private static void SendKeys(string keycode) {
		Process proc = new() {
			StartInfo = {
				FileName = "xdotool",
				Arguments = $"key {keycode}",
				UseShellExecute = false,
				RedirectStandardError = false,
				RedirectStandardInput = false,
				RedirectStandardOutput = false
			}
		};
		proc.Start();
	}
}
#endif
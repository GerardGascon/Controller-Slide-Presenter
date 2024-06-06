namespace SwitchSlidePresenter;

public class SlideSwitcher : IDisposable {
	private readonly GamepadReader _reader;

	public SlideSwitcher(GamepadReader reader) {
		_reader = reader;
		_reader.NextSlide += NextSlide;
		_reader.PrevSlide += PreviousSlide;
	}

	public void Dispose() {
		_reader.NextSlide -= NextSlide;
		_reader.PrevSlide -= PreviousSlide;
	}

	private void NextSlide() {
		IntPtr handle = Win32Api.GetForegroundWindow();
		Win32Api.PostMessage(handle, Win32Api.WM_KEYDOWN, Win32Api.VK_NEXT, 0);
	}

	private void PreviousSlide() {
		IntPtr handle = Win32Api.GetForegroundWindow();
		Win32Api.PostMessage(handle, Win32Api.WM_KEYDOWN, Win32Api.VK_PRIOR, 0);
	}
}
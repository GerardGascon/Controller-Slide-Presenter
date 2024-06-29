using ControllerSlidePresenter.GamepadReader;
using ControllerSlidePresenter.InputSender;

namespace ControllerSlidePresenter;

public class SlideSwitcher : IDisposable {
	private readonly IGamepadReader? _reader;

#if OS_WINDOWS
	private readonly IInputSender _inputSender = new WindowsInputSender();
#elif OS_MAC
	private readonly IInputSender _inputSender = new MacInputSender();
#elif OS_LINUX
	private readonly IInputSender _inputSender = new LinuxInputSender();
#endif

	public SlideSwitcher(IGamepadReader? reader) {
		_reader = reader;
		if (_reader == null) return;

		_reader.NextSlide += NextSlide;
		_reader.PrevSlide += PreviousSlide;
	}

	public void Dispose() {
		if (_reader == null) return;
		_reader.NextSlide -= NextSlide;
		_reader.PrevSlide -= PreviousSlide;
	}

	private void NextSlide() => _inputSender.NextSlide();
	private void PreviousSlide() => _inputSender.PreviousSlide();
}
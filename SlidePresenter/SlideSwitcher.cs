using SwitchSlidePresenter.InputSender;

namespace SwitchSlidePresenter;

public class SlideSwitcher : IDisposable {
	private readonly IGamepadReader? _reader;
	private readonly IInputSender _inputSender = new WindowsInputSender();

	public SlideSwitcher(IGamepadReader? reader) {
		_reader = reader;
		_reader.NextSlide += NextSlide;
		_reader.PrevSlide += PreviousSlide;
	}

	public void Dispose() {
		_reader.NextSlide -= NextSlide;
		_reader.PrevSlide -= PreviousSlide;
	}

	private void NextSlide() => _inputSender.NextSlide();
	private void PreviousSlide() => _inputSender.PreviousSlide();
}
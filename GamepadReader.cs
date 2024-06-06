namespace SwitchSlidePresenter;

public abstract class GamepadReader {
	public abstract event Action NextSlide;
	public abstract event Action PrevSlide;

	public abstract Task Read();
}
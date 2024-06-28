using ControllerSlidePresenter.GamepadReader;

namespace ControllerSlidePresenter {
	internal abstract class Program {
		private static async Task Main() {
			IGamepadReader? reader = ControllerSelector.GetReader();
			if (reader == null) {
				Console.WriteLine("Invalid Controller Selected.");
				return;
			}
			SlideSwitcher switcher = new(reader);
			await reader.Read();
			switcher.Dispose();
		}
	}
}
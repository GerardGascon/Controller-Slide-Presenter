namespace SwitchSlidePresenter {
	class Program {
		private static async Task Main() {
			IGamepadReader reader = new JoyConRead();
			SlideSwitcher switcher = new(reader);
			await reader.Read();
			switcher.Dispose();
		}
	}
}
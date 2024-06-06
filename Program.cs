namespace SwitchSlidePresenter {
	class Program {
		private static async Task Main() {
			GamepadReader reader = new JoyConRead();
			await reader.Read();
		}
	}
}
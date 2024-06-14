namespace SwitchSlidePresenter;

public static class ControllerSelector {
	public static IGamepadReader? GetReader() {
		Console.WriteLine("Write a number to select controller type:");
		Console.WriteLine("[1] - JoyCon");
		Console.WriteLine("[2] - Wiimote");

		string? line = Console.ReadLine();
		if (line == null) {
			Console.WriteLine("Invalid input.");
			return null;
		}
		if (!int.TryParse(line, out int id)) {
			Console.WriteLine("Invalid number.");
			return null;
		}

		return GetReader(id);
	}

	private static IGamepadReader? GetReader(int id) => id switch {
		1 => new JoyConRead(),
		2 => new WiimoteRead(),
		_ => null
	};
}
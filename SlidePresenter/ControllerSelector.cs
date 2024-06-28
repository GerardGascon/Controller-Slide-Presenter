using ControllerSlidePresenter.GamepadReader;

namespace ControllerSlidePresenter;

public static class ControllerSelector {
	private static readonly List<(string name, IGamepadReader reader)> Readers = [
#if JoyCon
		("JoyCon", new JoyConRead()),
#endif
#if Wiimote
		("Wiimote", new WiimoteRead())
#endif
	];

	public static IGamepadReader? GetReader() {
		if (Readers.Count == 1)
			return Readers[0].reader;

		Console.WriteLine("Write a number to select controller type:");
		for (int i = 0; i < Readers.Count; i++)
			Console.WriteLine($"[{i+1}] - {Readers[i].name}");

		int? id = GetReaderIndex();
		if (id == null)
			return null;

		return Readers[id.Value].reader;
	}

	private static int? GetReaderIndex() {
		string? line = Console.ReadLine();
		if (line == null) {
			Console.WriteLine("Invalid input.");
			return null;
		}
		if (!int.TryParse(line, out int id)) {
			Console.WriteLine("Invalid number.");
			return null;
		}
		if (id <= 0 || id >= Readers.Count) {
			Console.WriteLine("Invalid number");
			return null;
		}

		return id - 1;
	}
}
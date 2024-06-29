#if Wiimote
using WiimoteLib.NetCore;

namespace ControllerSlidePresenter.GamepadReader;

public class WiimoteRead : IGamepadReader {
	public event Action? NextSlide;
	public event Action? PrevSlide;

	public async Task Read() {
		Wiimote wiimote = new();
		wiimote.Connect();

		if (string.IsNullOrEmpty(wiimote.HIDDevicePath)) {
			Console.WriteLine("No controller. Please connect Wiimote via Bluetooth.");
			Console.WriteLine("Press any key to exit program.");
			Console.ReadKey();
			return;
		}

		wiimote.WiimoteChanged += WiimoteChanged;

		Console.WriteLine("Wiimote ready for presenting.");
		Console.WriteLine("Press Enter to exit program.");
		while (Console.ReadKey().Key != ConsoleKey.Enter) {
			await Task.Yield();
		}
		wiimote.Disconnect();

		Console.WriteLine();
		Console.WriteLine("Stopped.");
		await Task.CompletedTask;
	}

	private void WiimoteChanged(object? sender, WiimoteChangedEventArgs e) {
		if (PreviousPressed(e.WiimoteState.ButtonState)) {
			PrevSlide?.Invoke();
		}
		if (NextPressed(e.WiimoteState.ButtonState)) {
			NextSlide?.Invoke();
		}
	}

	private static bool PreviousPressed(ButtonState input) {
		return input.B || input.Left;
	}
	private static bool NextPressed(ButtonState input) {
		return input.A || input.Right;
	}
}
#endif

using WiimoteLib.NetCore;

namespace SwitchSlidePresenter;

public class WiimoteRead : IGamepadReader {
	public event Action NextSlide;
	public event Action PrevSlide;

	private const int RetryDelay = 1000;

	public async Task Read() {
		Wiimote wiimote = new();
		while (string.IsNullOrEmpty(wiimote.HIDDevicePath)) {
			wiimote.Connect();
			if (string.IsNullOrEmpty(wiimote.HIDDevicePath)) {
				Console.WriteLine("Wiimote connection failed, trying again...");
				await Task.Delay(RetryDelay);
			} else {
				Console.WriteLine("Wiimote ready for presenting!");
			}
		}

		ButtonState previousState = wiimote.WiimoteState.ButtonState;
		while (true) {
			if (PreviousPressed(wiimote.WiimoteState.ButtonState, previousState)) {
				PrevSlide?.Invoke();
			}
			if (NextPressed(wiimote.WiimoteState.ButtonState, previousState)) {
				NextSlide?.Invoke();
			}
			previousState = wiimote.WiimoteState.ButtonState;

			await Task.Yield();

			if (!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Enter)
				continue;
			wiimote.Disconnect();

			Console.WriteLine();
			Console.WriteLine("Stopped.");
			break;
		}
	}

	private static bool PreviousPressed(ButtonState input) {
		return input.B || input.Left;
	}
	private static bool NextPressed(ButtonState input) {
		return input.A || input.Right;
	}

	private static bool PreviousPressed(ButtonState input, ButtonState previousState) {
		return PreviousPressed(input) && !PreviousPressed(previousState);
	}
	private static bool NextPressed(ButtonState input, ButtonState previousState) {
		return NextPressed(input) && !NextPressed(previousState);
	}
}
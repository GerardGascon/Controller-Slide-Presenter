using System.Runtime.InteropServices;
using Win32Api;

namespace SwitchSlidePresenter;

public class SlideSwitcher : IDisposable {
	private readonly IGamepadReader? _reader;

	private const uint INPUT_KEYBOARD = 1;
	private const ushort VK_NEXT = 0x22;
	private const ushort VK_PRIOR = 0x21;
	private const uint KEYEVENTF_KEYDOWN = 0x0000;
	private const uint KEYEVENTF_KEYUP = 0x0002;

	public SlideSwitcher(IGamepadReader? reader) {
		_reader = reader;
		_reader.NextSlide += NextSlide;
		_reader.PrevSlide += PreviousSlide;
	}

	public void Dispose() {
		_reader.NextSlide -= NextSlide;
		_reader.PrevSlide -= PreviousSlide;
	}

	private static void NextSlide() {
		SimulateKeyPress(VK_NEXT);
	}

	private static void PreviousSlide() {
		SimulateKeyPress(VK_PRIOR);
	}

	private static void SimulateKeyPress(ushort keyCode) {
		Input[] inputs = new Input[2];

		inputs[0] = new Input {
			type = INPUT_KEYBOARD,
			u = new InputUnion {
				ki = new KeyboardInput {
					wVk = keyCode,
					wScan = 0,
					dwFlags = KEYEVENTF_KEYDOWN,
					time = 0,
					dwExtraInfo = IntPtr.Zero
				}
			}
		};

		inputs[1] = new Input {
			type = INPUT_KEYBOARD,
			u = new InputUnion {
				ki = new KeyboardInput {
					wVk = keyCode,
					wScan = 0,
					dwFlags = KEYEVENTF_KEYUP,
					time = 0,
					dwExtraInfo = IntPtr.Zero
				}
			}
		};

		Win32Api.Win32Api.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
	}
}
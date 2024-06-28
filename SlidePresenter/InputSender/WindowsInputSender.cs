using System.Runtime.InteropServices;
using Win32Api;

namespace SwitchSlidePresenter.InputSender;

public class WindowsInputSender : IInputSender {
	private const uint INPUT_KEYBOARD = 1;
	private const ushort VK_NEXT = 0x22;
	private const ushort VK_PRIOR = 0x21;
	private const uint KEYEVENTF_KEYDOWN = 0x0000;
	private const uint KEYEVENTF_KEYUP = 0x0002;

	public void NextSlide() => SimulateKeyPress(VK_NEXT);
	public void PreviousSlide() => SimulateKeyPress(VK_PRIOR);

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
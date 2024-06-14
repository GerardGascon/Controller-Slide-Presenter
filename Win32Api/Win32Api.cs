using System.Runtime.InteropServices;

namespace Win32Api;

public static class Win32Api {
	[DllImport("user32.dll", SetLastError = true)]
	public static extern uint SendInput(uint nInputs, Input[] pInputs, int cbSize);
}
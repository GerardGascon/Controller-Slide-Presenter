using System.Runtime.InteropServices;

namespace SwitchSlidePresenter;

public static class Win32Api {
	public const UInt32 WM_KEYDOWN = 0x0100;
	public const int VK_NEXT = 0x22;
	public const int VK_PRIOR = 0x21;

	[DllImport("user32.dll")]
	public static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);
	[DllImport("user32.dll")]
	public static extern IntPtr GetForegroundWindow();
}
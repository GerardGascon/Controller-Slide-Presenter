using System.Runtime.InteropServices;

namespace Win32Api;

[StructLayout(LayoutKind.Sequential)]
public struct MouseInput {
	public int dx;
	public int dy;
	public uint mouseData;
	public uint dwFlags;
	public uint time;
	public IntPtr dwExtraInfo;
}
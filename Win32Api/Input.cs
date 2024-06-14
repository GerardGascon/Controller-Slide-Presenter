using System.Runtime.InteropServices;

namespace Win32Api;

[StructLayout(LayoutKind.Sequential)]
public struct Input {
	public uint type;
	public InputUnion u;
}
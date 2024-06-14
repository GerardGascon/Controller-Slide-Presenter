using System.Runtime.InteropServices;

namespace Win32Api;

[StructLayout(LayoutKind.Sequential)]
public struct HardwareInput {
	public uint uMsg;
	public ushort wParamL;
	public ushort wParamH;
}
#if JoyCon
using System.Text;
using HidSharp;
using HidSharp.Reports;
using wtf.cluster.JoyCon;
using wtf.cluster.JoyCon.ExtraData;
using wtf.cluster.JoyCon.InputData;
using wtf.cluster.JoyCon.InputReports;

namespace ControllerSlidePresenter.GamepadReader;

public class JoyConRead : IGamepadReader {
	public event Action NextSlide;
	public event Action PrevSlide;

	public async Task Read() {
		Console.OutputEncoding = Encoding.UTF8;

		HidDevice? device = GetHidDevice();
		if (device == null) {
			Console.WriteLine("No controller. Please connect Joy-Con via Bluetooth.");
			Console.WriteLine("Press any key to exit program.");
			Console.ReadKey();
			return;
		}
		JoyCon joycon = new(device);
		joycon.Start();
		await joycon.SetInputReportModeAsync(JoyCon.InputReportType.Simple);

		await LogDeviceInfo(joycon);

		joycon.ReportReceived += OnJoyConOnReportReceived;

		joycon.StoppedOnError += (_, ex) => {
			Console.WriteLine();
			Console.WriteLine($"Critical error: {ex.Message}");
			Console.WriteLine("Controller polling stopped.");
			Environment.Exit(1);
			return Task.CompletedTask;
		};

		Console.WriteLine("JoyCon ready for presenting.");
		Console.WriteLine("Press Enter to exit program.");
		while (Console.ReadKey().Key != ConsoleKey.Enter) {
			await Task.Yield();
		}
		joycon.Stop();

		Console.WriteLine();
		Console.WriteLine("Stopped.");
		await Task.CompletedTask;
	}

	private static async Task LogDeviceInfo(JoyCon joycon) {
		DeviceInfo deviceInfo = await joycon.GetDeviceInfoAsync();
		Console.WriteLine(
			$"Type: {deviceInfo.ControllerType}, Firmware: {deviceInfo.FirmwareVersionMajor}.{deviceInfo.FirmwareVersionMinor}");
		string? serial = await joycon.GetSerialNumberAsync();
		Console.WriteLine($"Serial number: {serial ?? "<none>"}");
		ControllerColors? colors = await joycon.GetColorsAsync();
		Console.WriteLine(colors != null
			? $"Body color: {colors.BodyColor}, buttons color: {colors.ButtonsColor}"
			: "Colors not specified, seems like the controller is grey.");
	}

	private static HidDevice? GetHidDevice() {
		return OperatingSystem.IsWindows()
			? GetWindowsHidDevice()
			: GetNonWindowsHidDevice();
	}

	private static HidDevice? GetWindowsHidDevice() {
		DeviceList list = DeviceList.Local;
		IEnumerable<HidDevice>? nintendos = list.GetHidDevices(0x057e);
		HidDevice? device = nintendos.FirstOrDefault();
		return device;
	}

	private static HidDevice? GetNonWindowsHidDevice() {
		HidDevice? device = null;
		DeviceList list = DeviceList.Local;

		IEnumerable<HidDevice>? hidDevices = list.GetHidDevices();
		foreach (HidDevice d in hidDevices)
		{
			ReportDescriptor? rd = d.GetReportDescriptor();
			if (rd == null) continue;
			if (rd.OutputReports.Count() != 4
			    || rd.OutputReports.Count(r => r.ReportID == 0x01) != 1
			    || rd.OutputReports.Count(r => r.ReportID == 0x10) != 1
			    || rd.OutputReports.Count(r => r.ReportID == 0x11) != 1
			    || rd.OutputReports.Count(r => r.ReportID == 0x12) != 1
			    || rd.InputReports.Count() != 6
			    || rd.InputReports.Count(r => r.ReportID == 0x21) != 1
			    || rd.InputReports.Count(r => r.ReportID == 0x30) != 1
			    || rd.InputReports.Count(r => r.ReportID == 0x31) != 1
			    || rd.InputReports.Count(r => r.ReportID == 0x32) != 1
			    || rd.InputReports.Count(r => r.ReportID == 0x33) != 1
			    || rd.InputReports.Count(r => r.ReportID == 0x3F) != 1) continue;

			device = d;
			break;
		}
		return device;
	}

	private Task OnJoyConOnReportReceived(JoyCon _, IJoyConReport input) {
		if (input is not InputSimple j) {
			Console.WriteLine($"Invalid input report type: {input.GetType()}");
			return Task.CompletedTask;
		}

		bool prev = PreviousPressed(j.Buttons);
		bool next = NextPressed(j.Buttons);
		if (prev)
			PrevSlide?.Invoke();
		if (next)
			NextSlide?.Invoke();
		return Task.CompletedTask;
	}

	private static bool PreviousPressed(ButtonsSimple input) {
		return input.LorR || input.Left;
	}
	private static bool NextPressed(ButtonsSimple input) {
		return input.ZLorZR || input.Down;
	}
}
#endif

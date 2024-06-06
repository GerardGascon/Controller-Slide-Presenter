using System.Text;
using HidSharp;
using wtf.cluster.JoyCon;
using wtf.cluster.JoyCon.Calibration;
using wtf.cluster.JoyCon.ExtraData;
using wtf.cluster.JoyCon.HomeLed;
using wtf.cluster.JoyCon.InputData;
using wtf.cluster.JoyCon.InputReports;
using wtf.cluster.JoyCon.Rumble;

namespace SwitchSlidePresenter;

public class JoyConRead : GamepadReader {
	public override event Action NextSlide;
	public override event Action PrevSlide;

	public override async Task Read() {
		Console.OutputEncoding = Encoding.UTF8;

		HidDevice? device = GetHidDevice();
		if (device == null) {
			Console.WriteLine("No controller. Please connect Joy-Con or Pro controller via Bluetooth.");
			return;
		}
		JoyCon joycon = new(device);
		joycon.Start();
		await joycon.SetInputReportModeAsync(JoyCon.InputReportType.Simple);

		await LogDeviceInfo(joycon);

		joycon.ReportReceived += OnJoyConOnReportReceived;
		Console.WriteLine($"JoyCon ready for presenting.");

		joycon.StoppedOnError += (_, ex) => {
			Console.WriteLine();
			Console.WriteLine($"Critical error: {ex.Message}");
			Console.WriteLine("Controller polling stopped.");
			Environment.Exit(1);
			return Task.CompletedTask;
		};

		Console.ReadKey();
		joycon.Stop();

		Console.WriteLine();
		Console.WriteLine("Stopped.");
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
		DeviceList list = DeviceList.Local;
		HidDevice? device = null;

		if (OperatingSystem.IsWindows()) {
			var nintendos = list.GetHidDevices(0x057e);
			device = nintendos.FirstOrDefault();
		} else {
			var hidDevices = list.GetHidDevices();
			foreach (var d in hidDevices) {
				var rd = d.GetReportDescriptor();
				if (rd != null) {
					if (
						rd.OutputReports.Count() == 4
						&& rd.OutputReports.Count(r => r.ReportID == 0x01) == 1
						&& rd.OutputReports.Count(r => r.ReportID == 0x10) == 1
						&& rd.OutputReports.Count(r => r.ReportID == 0x11) == 1
						&& rd.OutputReports.Count(r => r.ReportID == 0x12) == 1
						&& rd.InputReports.Count() == 6
						&& rd.InputReports.Count(r => r.ReportID == 0x21) == 1
						&& rd.InputReports.Count(r => r.ReportID == 0x30) == 1
						&& rd.InputReports.Count(r => r.ReportID == 0x31) == 1
						&& rd.InputReports.Count(r => r.ReportID == 0x32) == 1
						&& rd.InputReports.Count(r => r.ReportID == 0x33) == 1
						&& rd.InputReports.Count(r => r.ReportID == 0x3F) == 1
					) {
						device = d;
						break;
					}
				}
			}
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
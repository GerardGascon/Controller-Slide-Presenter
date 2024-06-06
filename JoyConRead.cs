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

public class JoyConRead {
	public static async Task Read() {
		Console.OutputEncoding = Encoding.UTF8;

		HidDevice? device = null;
		DeviceList list = DeviceList.Local;
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
		if (device == null) {
			Console.WriteLine("No controller. Please connect Joy-Con or Pro controller via Bluetooth.");
			return;
		}
		JoyCon joycon = new(device);
		joycon.Start();
		await joycon.SetInputReportModeAsync(JoyCon.InputReportType.Simple);

		DeviceInfo deviceInfo = await joycon.GetDeviceInfoAsync();
		Console.WriteLine(
			$"Type: {deviceInfo.ControllerType}, Firmware: {deviceInfo.FirmwareVersionMajor}.{deviceInfo.FirmwareVersionMinor}");
		var serial = await joycon.GetSerialNumberAsync();
		Console.WriteLine($"Serial number: {serial ?? "<none>"}");
		ControllerColors? colors = await joycon.GetColorsAsync();
		if (colors != null) {
			Console.WriteLine($"Body color: {colors.BodyColor}, buttons color: {colors.ButtonsColor}");
		} else {
			Console.WriteLine("Colors not specified, seems like the controller is grey.");
		}

		joycon.ReportReceived += OnJoyconOnReportReceived;

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

	private static Task OnJoyconOnReportReceived(JoyCon _, IJoyConReport input) {
		if (input is InputSimple j) {
			bool prev = PreviousPressed(j.Buttons);
			bool next = NextPressed(j.Buttons);
			if(prev)
				Console.WriteLine("Previous Slide");
			if(next)
				Console.WriteLine("Next Slide");
		} else {
			Console.WriteLine($"Invalid input report type: {input.GetType()}");
		}
		return Task.CompletedTask;
	}

	private static bool PreviousPressed(ButtonsSimple input) {
		return input.LorR || input.Left;
	}
	private static bool NextPressed(ButtonsSimple input) {
		return input.ZLorZR || input.Down;
	}
}
using System.Text;
using HidSharp;
using wtf.cluster.JoyCon;
using wtf.cluster.JoyCon.Calibration;
using wtf.cluster.JoyCon.ExtraData;
using wtf.cluster.JoyCon.HomeLed;
using wtf.cluster.JoyCon.InputReports;
using wtf.cluster.JoyCon.Rumble;

namespace SwitchSlidePresenter;

public class JoyConRead {
	public static async Task Read() {
		Console.OutputEncoding = Encoding.UTF8;

		HidDevice? device = null;
		// Get a list of all HID devices
		DeviceList list = DeviceList.Local;
		if (OperatingSystem.IsWindows()) {
			// Get all devices developed by Nintendo by vendor ID
			var nintendos = list.GetHidDevices(0x057e);
			// Get the first Nintendo controller
			device = nintendos.FirstOrDefault();
			// It works fine for Windows, but...
		} else {
			// Linux has more complicated HID device management, we can't get device's vendor ID,
			// so let's filter devices by their report descriptor
			// It should work in Windows too, so it's more multiplatform solution
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
		// Create a new JoyCon instance based on the HID device
		JoyCon joycon = new(device);
		// Start controller polling
		joycon.Start();

		// First of all, we need to set format of the input reports,
		// most of the time you will need to use InputReportMode.Full mode
		await joycon.SetInputReportModeAsync(JoyCon.InputReportType.Simple);

		// Get some information about the controller
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

		// Save the current cursor position
		(var cX, var cY) = (Console.CursorLeft, Console.CursorTop);

		// Subscribe to the input reports
		joycon.ReportReceived += (sender, input) => {
			Console.SetCursorPosition(cX, cY);
			// Check the type of the input report, most likely it will be InputWithImu
			if (input is InputSimple j) {
				// Some base data from the controller
				Console.WriteLine(
					$"Buttons: ({j.Buttons})          ");
			} else {
				Console.WriteLine($"Invalid input report type: {input.GetType()}");
			}
			return Task.CompletedTask;
		};

		// Error handling
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
}
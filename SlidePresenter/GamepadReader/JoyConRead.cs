#if JoyCon
using System.Text;
using HidSharp;
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
		DeviceList list = DeviceList.Local;
		IEnumerable<HidDevice>? nintendos = list.GetHidDevices(0x057e);

		return nintendos.FirstOrDefault();
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

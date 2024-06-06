using HidSharp;
using System.Text;
using wtf.cluster.JoyCon;
using wtf.cluster.JoyCon.Calibration;
using wtf.cluster.JoyCon.ExtraData;
using wtf.cluster.JoyCon.HomeLed;
using wtf.cluster.JoyCon.InputReports;
using wtf.cluster.JoyCon.Rumble;

namespace SwitchSlidePresenter {
	class Program {
		static async Task Main(string[] args) {
			await Tutorial.Execute();
		}
	}
}
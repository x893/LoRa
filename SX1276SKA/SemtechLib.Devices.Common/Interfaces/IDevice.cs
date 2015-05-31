using SemtechLib.Devices.Common.Events;
using SemtechLib.General;
using System;
using System.ComponentModel;
using System.IO;

namespace SemtechLib.Devices.Common.Interfaces
{
	public interface IDevice : INotifyPropertyChanged, IDisposable
	{
		string DeviceName { get; }

		bool IsOpen { get; set; }

		RegisterCollection Registers { get; set; }

		bool Test { get; set; }

		Version Version { get; set; }

		Version FwVersion { get; set; }

		bool Monitor { get; set; }

		bool IsPacketHandlerStarted { get; }

		bool IsDebugOn { get; set; }

		event EventHandler Connected;

		event EventHandler Disconected;

		event SemtechLib.General.Events.ErrorEventHandler Error;

		event EventHandler PacketHandlerStarted;

		event EventHandler PacketHandlerStoped;

		event PacketStatusEventHandler PacketHandlerTransmitted;

		event PacketStatusEventHandler PacketHandlerReceived;

		bool Open();

		bool Close();

		void Reset();

		void OpenConfig(ref FileStream stream);

		void SaveConfig(ref FileStream stream);

		void ReadRegisters();

		void ProcessWinMessage(int msg, IntPtr wParam, IntPtr lParam);

		void SetNotificationWindowHandle(IntPtr handle, bool isWpfApplication);
	}
}

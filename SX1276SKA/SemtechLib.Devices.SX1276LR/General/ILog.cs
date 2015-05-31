using System;
using System.ComponentModel;

namespace SemtechLib.Devices.SX1276LR.General
{
	public interface ILog : INotifyPropertyChanged
	{
		bool Enabled { get; set; }

		bool IsAppend { get; set; }

		string Path { get; set; }

		string FileName { get; set; }

		ulong MaxSamples { get; set; }

		event ProgressChangedEventHandler ProgressChanged;

		event EventHandler Stoped;

		void Start();

		void Stop();
	}
}
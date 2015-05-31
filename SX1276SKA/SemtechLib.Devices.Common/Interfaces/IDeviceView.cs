using SemtechLib.General.Events;
using SemtechLib.General.Interfaces;
using System;
using System.Windows.Forms;

namespace SemtechLib.Devices.Common.Interfaces
{
	public interface IDeviceView : IDisposable, INotifyDocumentationChanged
	{
		IDevice Device { get; set; }

		DockStyle Dock { get; set; }

		bool Enabled { get; set; }

		bool Visible { get; set; }

		string Name { get; set; }

		int TabIndex { get; set; }

		event ErrorEventHandler Error;
	}
}

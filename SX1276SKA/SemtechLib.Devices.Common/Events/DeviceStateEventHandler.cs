using System;
using System.Runtime.InteropServices;

namespace SemtechLib.Devices.Common.Events
{
	[ComVisible(true)]
	[Serializable]
	public delegate void DeviceStateEventHandler(object sender, DeviceStateEventArg e);
}

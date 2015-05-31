using System;
using System.Runtime.InteropServices;

namespace SemtechLib.Devices.Common.Events
{
	[ComVisible(true)]
	[Serializable]
	public delegate void PacketStatusEventHandler(object sender, PacketStatusEventArg e);
}

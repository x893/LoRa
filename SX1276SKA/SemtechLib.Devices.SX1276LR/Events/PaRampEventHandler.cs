using System;
using System.Runtime.InteropServices;

namespace SemtechLib.Devices.SX1276LR.Events
{
	[ComVisible(true)]
	[Serializable]
	public delegate void PaRampEventHandler(object sender, PaRampEventArg e);
}

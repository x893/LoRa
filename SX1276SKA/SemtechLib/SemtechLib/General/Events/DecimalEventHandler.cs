using System;
using System.Runtime.InteropServices;

namespace SemtechLib.General.Events
{
	[ComVisible(true)]
	[Serializable]
	public delegate void DecimalEventHandler(object sender, DecimalEventArg e);
}

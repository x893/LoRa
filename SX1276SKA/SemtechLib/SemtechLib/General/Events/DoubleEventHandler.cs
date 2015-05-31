using System;
using System.Runtime.InteropServices;

namespace SemtechLib.General.Events
{
	[ComVisible(true)]
	[Serializable]
	public delegate void DoubleEventHandler(object sender, DoubleEventArg e);
}

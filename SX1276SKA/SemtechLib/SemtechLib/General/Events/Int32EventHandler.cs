using System;
using System.Runtime.InteropServices;

namespace SemtechLib.General.Events
{
	[ComVisible(true)]
	[Serializable]
	public delegate void Int32EventHandler(object sender, Int32EventArg e);
}

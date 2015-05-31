using System;
using System.Runtime.InteropServices;

namespace SemtechLib.General.Events
{
	[ComVisible(true)]
	[Serializable]
	public delegate void Int16EventHandler(object sender, Int16EventArg e);
}

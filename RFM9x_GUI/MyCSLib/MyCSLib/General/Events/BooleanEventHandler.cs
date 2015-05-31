using System;
using System.Runtime.InteropServices;

namespace MyCSLib.General.Events
{
	[ComVisible(true)]
	[Serializable]
	public delegate void BooleanEventHandler(object sender, BooleanEventArg e);
}

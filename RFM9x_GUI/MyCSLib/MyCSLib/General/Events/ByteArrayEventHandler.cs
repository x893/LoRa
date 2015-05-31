using System;
using System.Runtime.InteropServices;

namespace MyCSLib.General.Events
{
	[ComVisible(true)]
	[Serializable]
	public delegate void ByteArrayEventHandler(object sender, ByteArrayEventArg e);
}

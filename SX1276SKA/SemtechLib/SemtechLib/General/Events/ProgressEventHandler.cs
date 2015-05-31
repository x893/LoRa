using System;
using System.Runtime.InteropServices;

namespace SemtechLib.General.Events
{
	[ComVisible(true)]
	[Serializable]
	public delegate void ProgressEventHandler(object sender, ProgressEventArg e);
}
﻿using System;
using System.Runtime.InteropServices;

namespace SemtechLib.Devices.SX1276LR.Events
{
	[ComVisible(true)]
	[Serializable]
	public delegate void ClockOutEventHandler(object sender, ClockOutEventArg e);
}

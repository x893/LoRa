using SemtechLib.Devices.SX1276LR.Enumerations;
using System;

namespace SemtechLib.Devices.SX1276LR.Events
{
	public class ClockOutEventArg : EventArgs
	{
		private ClockOutEnum value;

		public ClockOutEnum Value
		{
			get
			{
				return value;
			}
		}

		public ClockOutEventArg(ClockOutEnum value)
		{
			this.value = value;
		}
	}
}

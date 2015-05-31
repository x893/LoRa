using SemtechLib.Devices.SX1276.Enumerations;
using System;

namespace SemtechLib.Devices.SX1276.Events
{
	public class FeiRangeEventArg : EventArgs
	{
		private FeiRangeEnum value;

		public FeiRangeEnum Value
		{
			get
			{
				return this.value;
			}
		}

		public FeiRangeEventArg(FeiRangeEnum value)
		{
			this.value = value;
		}
	}
}

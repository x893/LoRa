using SemtechLib.Devices.SX1276LR.Enumerations;
using System;

namespace SemtechLib.Devices.SX1276LR.Events
{
	public class BandEventArg : EventArgs
	{
		private BandEnum value;

		public BandEnum Value
		{
			get
			{
				return value;
			}
		}

		public BandEventArg(BandEnum value)
		{
			this.value = value;
		}
	}
}

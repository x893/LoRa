using SemtechLib.Devices.SX1276.Enumerations;
using System;

namespace SemtechLib.Devices.SX1276.Events
{
	public class BandEventArg : EventArgs
	{
		private BandEnum value;

		public BandEnum Value
		{
			get
			{
				return this.value;
			}
		}

		public BandEventArg(BandEnum value)
		{
			this.value = value;
		}
	}
}

using SemtechLib.Devices.SX1276.Enumerations;
using System;

namespace SemtechLib.Devices.SX1276.Events
{
	public class FromTransmitEventArg : EventArgs
	{
		private FromTransmit value;

		public FromTransmit Value
		{
			get
			{
				return this.value;
			}
		}

		public FromTransmitEventArg(FromTransmit value)
		{
			this.value = value;
		}
	}
}

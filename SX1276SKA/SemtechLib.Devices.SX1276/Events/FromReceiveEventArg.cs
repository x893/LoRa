using SemtechLib.Devices.SX1276.Enumerations;
using System;

namespace SemtechLib.Devices.SX1276.Events
{
	public class FromReceiveEventArg : EventArgs
	{
		private FromReceive value;

		public FromReceive Value
		{
			get
			{
				return this.value;
			}
		}

		public FromReceiveEventArg(FromReceive value)
		{
			this.value = value;
		}
	}
}

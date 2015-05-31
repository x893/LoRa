using SemtechLib.Devices.SX1276.Enumerations;
using System;

namespace SemtechLib.Devices.SX1276.Events
{
	public class FromRxTimeoutEventArg : EventArgs
	{
		private FromRxTimeout value;

		public FromRxTimeout Value
		{
			get
			{
				return this.value;
			}
		}

		public FromRxTimeoutEventArg(FromRxTimeout value)
		{
			this.value = value;
		}
	}
}

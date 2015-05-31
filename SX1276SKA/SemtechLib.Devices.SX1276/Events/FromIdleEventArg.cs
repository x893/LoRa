using SemtechLib.Devices.SX1276.Enumerations;
using System;

namespace SemtechLib.Devices.SX1276.Events
{
	public class FromIdleEventArg : EventArgs
	{
		private FromIdle value;

		public FromIdle Value
		{
			get
			{
				return this.value;
			}
		}

		public FromIdleEventArg(FromIdle value)
		{
			this.value = value;
		}
	}
}

using SemtechLib.Devices.SX1276.Enumerations;
using System;

namespace SemtechLib.Devices.SX1276.Events
{
	public class IdleModeEventArg : EventArgs
	{
		private IdleMode value;

		public IdleMode Value
		{
			get
			{
				return this.value;
			}
		}

		public IdleModeEventArg(IdleMode value)
		{
			this.value = value;
		}
	}
}

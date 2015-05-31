using SemtechLib.Devices.SX1276.Enumerations;
using System;

namespace SemtechLib.Devices.SX1276.Events
{
	public class AutoRestartRxEventArg : EventArgs
	{
		private AutoRestartRxEnum value;

		public AutoRestartRxEnum Value
		{
			get
			{
				return this.value;
			}
		}

		public AutoRestartRxEventArg(AutoRestartRxEnum value)
		{
			this.value = value;
		}
	}
}

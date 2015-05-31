using SemtechLib.Devices.SX1276LR.Enumerations;
using System;

namespace SemtechLib.Devices.SX1276LR.Events
{
	public class PaModeEventArg : EventArgs
	{
		private PaSelectEnum value;

		public PaSelectEnum Value
		{
			get
			{
				return value;
			}
		}

		public PaModeEventArg(PaSelectEnum value)
		{
			this.value = value;
		}
	}
}

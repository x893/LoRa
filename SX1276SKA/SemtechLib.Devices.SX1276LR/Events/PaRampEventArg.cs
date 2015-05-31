using SemtechLib.Devices.SX1276LR.Enumerations;
using System;

namespace SemtechLib.Devices.SX1276LR.Events
{
	public class PaRampEventArg : EventArgs
	{
		private PaRampEnum value;

		public PaRampEnum Value
		{
			get
			{
				return value;
			}
		}

		public PaRampEventArg(PaRampEnum value)
		{
			this.value = value;
		}
	}
}

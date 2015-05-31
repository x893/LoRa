using SemtechLib.Devices.SX1276.Enumerations;
using System;

namespace SemtechLib.Devices.SX1276.Events
{
	public class FromStartEventArg : EventArgs
	{
		private FromStart value;

		public FromStart Value
		{
			get
			{
				return this.value;
			}
		}

		public FromStartEventArg(FromStart value)
		{
			this.value = value;
		}
	}
}

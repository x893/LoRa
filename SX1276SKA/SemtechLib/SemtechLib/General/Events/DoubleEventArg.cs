using System;

namespace SemtechLib.General.Events
{
	public class DoubleEventArg : EventArgs
	{
		private double value;

		public double Value
		{
			get
			{
				return this.value;
			}
		}

		public DoubleEventArg(double value)
		{
			this.value = value;
		}
	}
}

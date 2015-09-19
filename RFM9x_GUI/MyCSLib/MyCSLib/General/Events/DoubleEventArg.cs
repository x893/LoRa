using System;
using System.Runtime.InteropServices;

namespace MyCSLib.General.Events
{
	[ComVisible(true)]
	[Serializable]
	public delegate void DoubleEventHandler(object sender, DoubleEventArg e);

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

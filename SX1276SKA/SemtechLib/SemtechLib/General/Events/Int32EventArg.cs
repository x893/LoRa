using System;

namespace SemtechLib.General.Events
{
	public class Int32EventArg : EventArgs
	{
		private int value;

		public int Value
		{
			get
			{
				return this.value;
			}
		}

		public Int32EventArg(int value)
		{
			this.value = value;
		}
	}
}
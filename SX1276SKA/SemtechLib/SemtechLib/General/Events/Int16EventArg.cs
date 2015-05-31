using System;

namespace SemtechLib.General.Events
{
	public class Int16EventArg : EventArgs
	{
		private short value;

		public short Value
		{
			get
			{
				return this.value;
			}
		}

		public Int16EventArg(short value)
		{
			this.value = value;
		}
	}
}

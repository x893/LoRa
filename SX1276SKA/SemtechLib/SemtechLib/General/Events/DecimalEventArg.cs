using System;

namespace SemtechLib.General.Events
{
	public class DecimalEventArg : EventArgs
	{
		private Decimal value;

		public Decimal Value
		{
			get
			{
				return this.value;
			}
		}

		public DecimalEventArg(Decimal value)
		{
			this.value = value;
		}
	}
}

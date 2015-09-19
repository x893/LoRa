using System;
using System.Runtime.InteropServices;

namespace MyCSLib.General.Events
{
	[ComVisible(true)]
	[Serializable]
	public delegate void DecimalEventHandler(object sender, DecimalEventArg e);

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

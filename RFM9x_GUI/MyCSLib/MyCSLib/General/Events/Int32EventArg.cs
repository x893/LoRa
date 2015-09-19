using System;
using System.Runtime.InteropServices;

namespace MyCSLib.General.Events
{
	[ComVisible(true)]
	[Serializable]
	public delegate void Int32EventHandler(object sender, Int32EventArg e);

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

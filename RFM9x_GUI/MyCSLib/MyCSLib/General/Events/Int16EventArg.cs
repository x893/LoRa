using System;
using System.Runtime.InteropServices;

namespace MyCSLib.General.Events
{
	[ComVisible(true)]
	[Serializable]
	public delegate void Int16EventHandler(object sender, Int16EventArg e);

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

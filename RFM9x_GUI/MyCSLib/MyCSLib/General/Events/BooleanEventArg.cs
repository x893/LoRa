using System;
using System.Runtime.InteropServices;

namespace MyCSLib.General.Events
{
	[ComVisible(true)]
	[Serializable]
	public delegate void BooleanEventHandler(object sender, BooleanEventArg e);

	public class BooleanEventArg : EventArgs
	{
		private bool value;

		public bool Value
		{
			get { return this.value; }
		}

		public BooleanEventArg(bool value)
		{
			this.value = value;
		}
	}
}

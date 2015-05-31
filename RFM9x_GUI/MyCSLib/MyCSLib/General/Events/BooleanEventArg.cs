using System;

namespace MyCSLib.General.Events
{
	public class BooleanEventArg : EventArgs
	{
		private bool value;

		public bool Value
		{
			get
			{
				return this.value;
			}
		}

		public BooleanEventArg(bool value)
		{
			this.value = value;
		}
	}
}

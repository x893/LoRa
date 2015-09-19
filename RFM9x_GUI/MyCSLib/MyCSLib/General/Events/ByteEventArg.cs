using System;
using System.Runtime.InteropServices;

namespace MyCSLib.General.Events
{
	[ComVisible(true)]
	[Serializable]
	public delegate void ByteEventHandler(object sender, ByteEventArg e);

	public class ByteEventArg : EventArgs
	{
		private byte value;

		public byte Value
		{
			get { return this.value; }
		}

		public ByteEventArg(byte value)
		{
			this.value = value;
		}
	}
}
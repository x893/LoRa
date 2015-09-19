using System;
using System.Runtime.InteropServices;

namespace MyCSLib.General.Events
{
	[ComVisible(true)]
	[Serializable]
	public delegate void ByteArrayEventHandler(object sender, ByteArrayEventArg e);

	public class ByteArrayEventArg : EventArgs
	{
		private byte[] value;

		public byte[] Value
		{
			get { return this.value; }
		}

		public ByteArrayEventArg(byte[] value)
		{
			this.value = value;
		}
	}
}

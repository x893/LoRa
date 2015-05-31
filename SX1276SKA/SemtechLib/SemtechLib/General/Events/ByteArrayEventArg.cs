using System;

namespace SemtechLib.General.Events
{
	public class ByteArrayEventArg : EventArgs
	{
		private byte[] value;

		public byte[] Value
		{
			get
			{
				return this.value;
			}
		}

		public ByteArrayEventArg(byte[] value)
		{
			this.value = value;
		}
	}
}

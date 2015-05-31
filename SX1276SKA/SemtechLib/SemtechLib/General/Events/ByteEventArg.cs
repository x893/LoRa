using System;

namespace SemtechLib.General.Events
{
	public class ByteEventArg : EventArgs
	{
		private byte value;

		public byte Value
		{
			get
			{
				return this.value;
			}
		}

		public ByteEventArg(byte value)
		{
			this.value = value;
		}
	}
}

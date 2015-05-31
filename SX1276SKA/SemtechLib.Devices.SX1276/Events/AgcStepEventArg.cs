using System;

namespace SemtechLib.Devices.SX1276.Events
{
	public class AgcStepEventArg : EventArgs
	{
		private byte id;
		private byte value;

		public byte Id
		{
			get
			{
				return this.id;
			}
		}

		public byte Value
		{
			get
			{
				return this.value;
			}
		}

		public AgcStepEventArg(byte id, byte value)
		{
			this.id = id;
			this.value = value;
		}
	}
}

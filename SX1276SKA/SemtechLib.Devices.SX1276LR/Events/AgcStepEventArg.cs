using System;

namespace SemtechLib.Devices.SX1276LR.Events
{
	public class AgcStepEventArg : EventArgs
	{
		private byte id;
		private byte value;

		public byte Id
		{
			get { return id; }
		}

		public byte Value
		{
			get { return value; }
		}

		public AgcStepEventArg(byte id, byte value)
		{
			this.id = id;
			this.value = value;
		}
	}
}

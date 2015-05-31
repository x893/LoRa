using SemtechLib.Devices.SX1276.Enumerations;
using System;

namespace SemtechLib.Devices.SX1276.Events
{
	public class FromPacketReceivedEventArg : EventArgs
	{
		private FromPacketReceived value;

		public FromPacketReceived Value
		{
			get
			{
				return this.value;
			}
		}

		public FromPacketReceivedEventArg(FromPacketReceived value)
		{
			this.value = value;
		}
	}
}

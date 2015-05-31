using System;

namespace SemtechLib.Devices.Common.Events
{
	public class PacketStatusEventArg : EventArgs
	{
		private int number;
		private int max;

		public int Number
		{
			get
			{
				return number;
			}
		}

		public int Max
		{
			get
			{
				return max;
			}
		}

		public PacketStatusEventArg(int number, int max)
		{
			this.number = number;
			this.max = max;
		}
	}
}

using System;

namespace SemtechLib.Devices.Common.Events
{
	public class DeviceStateEventArg : EventArgs
	{
		private string name;
		private DeviceState state;

		public string Name
		{
			get
			{
				return name;
			}
		}

		public DeviceState State
		{
			get
			{
				return state;
			}
		}

		public DeviceStateEventArg(string name, DeviceState state)
		{
			this.name = name;
			this.state = state;
		}
	}
}

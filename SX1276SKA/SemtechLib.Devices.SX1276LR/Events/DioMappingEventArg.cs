using SemtechLib.Devices.SX1276LR.Enumerations;
using System;

namespace SemtechLib.Devices.SX1276LR.Events
{
	public class DioMappingEventArg : EventArgs
	{
		private byte id;
		private DioMappingEnum value;

		public byte Id
		{
			get
			{
				return id;
			}
		}

		public DioMappingEnum Value
		{
			get
			{
				return value;
			}
		}

		public DioMappingEventArg(byte id, DioMappingEnum value)
		{
			this.id = id;
			this.value = value;
		}
	}
}

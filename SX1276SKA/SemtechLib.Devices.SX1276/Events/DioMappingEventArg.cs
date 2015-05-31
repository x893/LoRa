using SemtechLib.Devices.SX1276.Enumerations;
using System;

namespace SemtechLib.Devices.SX1276.Events
{
	public class DioMappingEventArg : EventArgs
	{
		private byte id;
		private DioMappingEnum value;

		public byte Id
		{
			get
			{
				return this.id;
			}
		}

		public DioMappingEnum Value
		{
			get
			{
				return this.value;
			}
		}

		public DioMappingEventArg(byte id, DioMappingEnum value)
		{
			this.id = id;
			this.value = value;
		}
	}
}

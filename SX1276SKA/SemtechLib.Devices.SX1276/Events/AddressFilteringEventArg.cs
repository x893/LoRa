using SemtechLib.Devices.SX1276.Enumerations;
using System;

namespace SemtechLib.Devices.SX1276.Events
{
	public class AddressFilteringEventArg : EventArgs
	{
		private AddressFilteringEnum value;

		public AddressFilteringEnum Value
		{
			get
			{
				return this.value;
			}
		}

		public AddressFilteringEventArg(AddressFilteringEnum value)
		{
			this.value = value;
		}
	}
}

using System;

namespace SemtechLib.General
{
	[AttributeUsage(AttributeTargets.Property)]
	public class PropertyOrderAttribute : Attribute
	{
		private int order;

		public int Order
		{
			get
			{
				return this.order;
			}
		}

		public PropertyOrderAttribute()
		{
		}

		public PropertyOrderAttribute(int order)
		{
			this.order = order;
		}
	}
}

using System;

namespace MyCSLib.General
{
	[AttributeUsage(AttributeTargets.Property)]
	public class PropertyOrderAttribute : Attribute
	{
		private int m_order = 0;

		public int Order
		{
			get { return m_order; }
		}

		public PropertyOrderAttribute(int order)
		{
			m_order = order;
		}

		public PropertyOrderAttribute() { }
	}
}

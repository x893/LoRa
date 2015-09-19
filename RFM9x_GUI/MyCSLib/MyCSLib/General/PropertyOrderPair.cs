using System;

namespace MyCSLib.General
{
	public class PropertyOrderPair : IComparable
	{
		private int m_order = 0;
		private string m_name = string.Empty;

		public string Name
		{
			get { return this.m_name; }
		}

		public PropertyOrderPair() { }
		public PropertyOrderPair(string name, int order)
		{
			m_order = order;
			m_name = name;
		}

		public int CompareTo(object obj)
		{
			int num = ((PropertyOrderPair)obj).m_order;
			if (num == m_order)
				return string.Compare(m_name, ((PropertyOrderPair)obj).m_name);
			return num > m_order ? -1 : 1;
		}
	}
}

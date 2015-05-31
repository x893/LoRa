using System;

namespace SemtechLib.General
{
	public class PropertyOrderPair : IComparable
	{
		private string name = string.Empty;
		private int order;

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public PropertyOrderPair()
		{
		}

		public PropertyOrderPair(string name, int order)
		{
			this.order = order;
			this.name = name;
		}

		public int CompareTo(object obj)
		{
			int num = ((PropertyOrderPair)obj).order;
			if (num == this.order)
				return string.Compare(this.name, ((PropertyOrderPair)obj).name);
			return num > this.order ? -1 : 1;
		}
	}
}

using System;

namespace MyCSLib.General
{
	public class BindingRegisterList : BindingCollectionBase
	{
		public BindingRegister this[int Index]
		{
			get { return List[Index] as BindingRegister; }
			set { List[Index] = (object)value; }
		}

		protected override Type ElementType
		{
			get { return typeof(BindingRegister); }
		}

		public int Add(BindingRegister Item)
		{
			return List.Add((object)Item);
		}
	}
}

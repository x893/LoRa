using System;

namespace MyCSLib.General
{
	public class BindingRegisterList : BindingCollectionBase
	{
		public BindingRegister this[int Index]
		{
			get
			{
				return this.List[Index] as BindingRegister;
			}
			set
			{
				this.List[Index] = (object)value;
			}
		}

		protected override Type ElementType
		{
			get
			{
				return typeof(BindingRegister);
			}
		}

		public int Add(BindingRegister Item)
		{
			return this.List.Add((object)Item);
		}
	}
}

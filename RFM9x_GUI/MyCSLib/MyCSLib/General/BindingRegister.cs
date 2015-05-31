namespace MyCSLib.General
{
	public class BindingRegister : EditableObject
	{
		private uint address = 0U;
		private uint value = 0U;
		private bool readOnly = false;
		private string name;

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public uint Address
		{
			get
			{
				return this.address;
			}
			set
			{
				this.address = value;
			}
		}

		public uint Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

		public bool ReadOnly
		{
			get
			{
				return this.readOnly;
			}
			set
			{
				this.readOnly = value;
			}
		}

		public BindingRegister()
		{
			this.name = "";
			this.address = 0U;
			this.value = 0U;
		}

		public BindingRegister(string name, uint address, uint value)
		{
			this.name = name;
			this.address = address;
			this.value = value;
			this.readOnly = false;
		}

		public BindingRegister(string name, uint address, uint value, bool readOnly)
		{
			this.name = name;
			this.address = address;
			this.value = value;
			this.readOnly = readOnly;
		}
	}
}

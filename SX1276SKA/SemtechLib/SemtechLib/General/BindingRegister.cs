namespace SemtechLib.General
{
	public class BindingRegister : EditableObject
	{
		private string name;
		private uint address;
		private uint value;
		private bool readOnly;

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
			this.address = 0;
			this.value = 0;
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

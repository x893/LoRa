namespace MyCSLib.General
{
	public class BindingRegister : EditableObject
	{
		private uint m_address = 0U;
		private uint m_value = 0U;
		private bool m_readOnly = false;
		private string m_name;

		public string Name
		{
			get { return m_name; }
			set { m_name = value; }
		}

		public uint Address
		{
			get { return m_address; }
			set { m_address = value; }
		}

		public uint Value
		{
			get { return m_value; }
			set { m_value = value; }
		}

		public bool ReadOnly
		{
			get { return m_readOnly; }
			set { m_readOnly = value; }
		}

		public BindingRegister()
		{
			m_name = "";
			m_address = 0U;
			m_value = 0U;
		}

		public BindingRegister(string name, uint address, uint value)
		{
			m_name = name;
			m_address = address;
			m_value = value;
			m_readOnly = false;
		}

		public BindingRegister(string name, uint address, uint value, bool readOnly)
		{
			m_name = name;
			m_address = address;
			m_value = value;
			m_readOnly = readOnly;
		}
	}
}

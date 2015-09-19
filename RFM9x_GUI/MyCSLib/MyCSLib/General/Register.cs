using System.ComponentModel;

namespace MyCSLib.General
{
	public class Register : INotifyPropertyChanged
	{
		private uint m_oldValue = 0U;
		private uint m_address = 0U;
		private uint m_value = 0U;
		private bool m_readOnly = false;
		private bool m_visible = false;
		private string m_name;

		public string Name
		{
			get { return m_name; }
			set
			{
				m_name = value;
				OnPropertyChanged("Name");
			}
		}

		public uint Address
		{
			get { return m_address; }
			set
			{
				m_address = value;
				OnPropertyChanged("Address");
			}
		}

		public uint Value
		{
			get { return m_value; }
			set
			{
				m_value = value;
				OnPropertyChanged("Value");
			}
		}

		public bool ReadOnly
		{
			get { return m_readOnly; }
			set
			{
				m_readOnly = value;
				OnPropertyChanged("ReadOnly");
			}
		}

		public bool Visible
		{
			get { return m_visible; }
			set
			{
				m_visible = value;
				OnPropertyChanged("Visible");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public Register()
		{
			m_name = "";
			m_address = 0U;
			m_value = 0U;
			m_oldValue = 0U;
		}

		public Register(string name, uint address, uint value)
		{
			m_name = name;
			m_address = address;
			m_value = value;
			m_oldValue = value;
			m_readOnly = false;
		}

		public Register(string name, uint address, uint value, bool readOnly, bool visible)
		{
			m_name = name;
			m_address = address;
			m_value = value;
			m_oldValue = value;
			m_readOnly = readOnly;
			m_visible = visible;
		}

		public bool IsValueChanged()
		{
			return (int)m_oldValue != (int)m_value;
		}

		public void ApplyValue()
		{
			m_oldValue = m_value;
		}

		private void OnPropertyChanged(string propName)
		{
			if (PropertyChanged == null)
				return;
			PropertyChanged((object)this, new PropertyChangedEventArgs(propName));
		}
	}
}

using System.ComponentModel;

namespace LoRaModem
{
	public class RegType : INotifyPropertyChanged
	{
		private byte oldValue;
		private string name;
		private byte address;
		private byte value;
		private bool readOnly;
		private bool visible;

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
				OnPropertyChanged("Name");
			}
		}

		public byte Address
		{
			get
			{
				return address;
			}
			set
			{
				address = value;
				OnPropertyChanged("Address");
			}
		}

		public byte Value
		{
			get
			{
				return value;
			}
			set
			{
				this.value = value;
				OnPropertyChanged("Value");
			}
		}

		public bool ReadOnly
		{
			get
			{
				return readOnly;
			}
			set
			{
				readOnly = value;
				OnPropertyChanged("ReadOnly");
			}
		}

		public bool Visible
		{
			get
			{
				return visible;
			}
			set
			{
				visible = value;
				OnPropertyChanged("Visible");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public RegType()
		{
			this.name = "";
			this.address = (byte)0;
			this.value = (byte)0;
			this.oldValue = (byte)0;
		}

		public RegType(string name, byte address, byte value)
		{
			this.name = name;
			this.address = address;
			this.value = value;
			this.oldValue = value;
			this.readOnly = false;
		}

		public RegType(string name, byte address, byte value, bool readOnly, bool visible)
		{
			this.name = name;
			this.address = address;
			this.value = value;
			this.oldValue = value;
			this.readOnly = readOnly;
			this.visible = visible;
		}

		public bool IsValueChanged()
		{
			return (int)oldValue != (int)value;
		}

		public void ApplyValue()
		{
			oldValue = value;
		}

		private void OnPropertyChanged(string propName)
		{
			if (PropertyChanged != null)
				PropertyChanged((object)this, new PropertyChangedEventArgs(propName));
		}
	}
}

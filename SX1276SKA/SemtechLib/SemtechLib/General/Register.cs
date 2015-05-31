using System.ComponentModel;

namespace SemtechLib.General
{
	public class Register : INotifyPropertyChanged
	{
		private uint oldValue;
		private string name;
		private uint address;
		private uint value;
		private bool readOnly;
		private bool visible;

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
				this.OnPropertyChanged("Name");
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
				this.OnPropertyChanged("Address");
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
				this.OnPropertyChanged("Value");
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
				this.OnPropertyChanged("ReadOnly");
			}
		}

		public bool Visible
		{
			get
			{
				return this.visible;
			}
			set
			{
				this.visible = value;
				this.OnPropertyChanged("Visible");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public Register()
		{
			this.name = "";
			this.address = 0U;
			this.value = 0U;
			this.oldValue = 0U;
		}

		public Register(string name, uint address, uint value)
		{
			this.name = name;
			this.address = address;
			this.value = value;
			this.oldValue = value;
			this.readOnly = false;
		}

		public Register(string name, uint address, uint value, bool readOnly, bool visible)
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
			return (int)this.oldValue != (int)this.value;
		}

		public void ApplyValue()
		{
			this.oldValue = this.value;
		}

		private void OnPropertyChanged(string propName)
		{
			if (this.PropertyChanged == null)
				return;
			this.PropertyChanged((object)this, new PropertyChangedEventArgs(propName));
		}
	}
}
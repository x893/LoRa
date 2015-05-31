// Decompiled with JetBrains decompiler
// Type: MyCSLib.General.Register
// Assembly: MyCSLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 70B056C4-DB31-49BD-9F27-15818C65F327
// Assembly location: C:\Tools\HopeRF\RFM9x_GUI_1_1_0_9\MyCSLib.dll

using System.ComponentModel;

namespace MyCSLib.General
{
  public class Register : INotifyPropertyChanged
  {
    private uint oldValue = 0U;
    private uint address = 0U;
    private uint value = 0U;
    private bool readOnly = false;
    private bool visible = false;
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
      return (int) this.oldValue != (int) this.value;
    }

    public void ApplyValue()
    {
      this.oldValue = this.value;
    }

    private void OnPropertyChanged(string propName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propName));
    }
  }
}

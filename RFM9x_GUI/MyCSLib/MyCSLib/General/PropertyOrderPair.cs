// Decompiled with JetBrains decompiler
// Type: MyCSLib.General.PropertyOrderPair
// Assembly: MyCSLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 70B056C4-DB31-49BD-9F27-15818C65F327
// Assembly location: C:\Tools\HopeRF\RFM9x_GUI_1_1_0_9\MyCSLib.dll

using System;

namespace MyCSLib.General
{
  public class PropertyOrderPair : IComparable
  {
    private int order = 0;
    private string name = string.Empty;

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
      int num = ((PropertyOrderPair) obj).order;
      if (num == this.order)
        return string.Compare(this.name, ((PropertyOrderPair) obj).name);
      return num > this.order ? -1 : 1;
    }
  }
}

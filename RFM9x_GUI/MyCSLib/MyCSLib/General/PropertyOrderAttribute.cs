// Decompiled with JetBrains decompiler
// Type: MyCSLib.General.PropertyOrderAttribute
// Assembly: MyCSLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 70B056C4-DB31-49BD-9F27-15818C65F327
// Assembly location: C:\Tools\HopeRF\RFM9x_GUI_1_1_0_9\MyCSLib.dll

using System;

namespace MyCSLib.General
{
  [AttributeUsage(AttributeTargets.Property)]
  public class PropertyOrderAttribute : Attribute
  {
    private int order = 0;

    public int Order
    {
      get
      {
        return this.order;
      }
    }

    public PropertyOrderAttribute()
    {
    }

    public PropertyOrderAttribute(int order)
    {
      this.order = order;
    }
  }
}

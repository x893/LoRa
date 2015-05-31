// Decompiled with JetBrains decompiler
// Type: MyCSLib.General.Events.DoubleEventArg
// Assembly: MyCSLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 70B056C4-DB31-49BD-9F27-15818C65F327
// Assembly location: C:\Tools\HopeRF\RFM9x_GUI_1_1_0_9\MyCSLib.dll

using System;

namespace MyCSLib.General.Events
{
  public class DoubleEventArg : EventArgs
  {
    private double value;

    public double Value
    {
      get
      {
        return this.value;
      }
    }

    public DoubleEventArg(double value)
    {
      this.value = value;
    }
  }
}

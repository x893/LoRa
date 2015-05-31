// Decompiled with JetBrains decompiler
// Type: MyCSLib.General.Events.ErrorEventArgs
// Assembly: MyCSLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 70B056C4-DB31-49BD-9F27-15818C65F327
// Assembly location: C:\Tools\HopeRF\RFM9x_GUI_1_1_0_9\MyCSLib.dll

using System;

namespace MyCSLib.General.Events
{
  public class ErrorEventArgs : EventArgs
  {
    private byte status;
    private string message;

    public byte Status
    {
      get
      {
        return this.status;
      }
    }

    public string Message
    {
      get
      {
        return this.message;
      }
    }

    public ErrorEventArgs(byte status, string message)
    {
      this.status = status;
      this.message = message;
    }
  }
}

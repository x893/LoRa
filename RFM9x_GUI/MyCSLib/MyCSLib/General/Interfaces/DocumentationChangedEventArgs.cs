// Decompiled with JetBrains decompiler
// Type: MyCSLib.General.Interfaces.DocumentationChangedEventArgs
// Assembly: MyCSLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 70B056C4-DB31-49BD-9F27-15818C65F327
// Assembly location: C:\Tools\HopeRF\RFM9x_GUI_1_1_0_9\MyCSLib.dll

using System;

namespace MyCSLib.General.Interfaces
{
  public class DocumentationChangedEventArgs : EventArgs
  {
    private string docFolder;
    private string docName;

    public string DocFolder
    {
      get
      {
        return this.docFolder;
      }
    }

    public string DocName
    {
      get
      {
        return this.docName;
      }
    }

    public DocumentationChangedEventArgs(string docFolder, string docName)
    {
      this.docFolder = docFolder;
      this.docName = docName;
    }
  }
}

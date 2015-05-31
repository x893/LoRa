// Decompiled with JetBrains decompiler
// Type: MyCSLib.General.TypeConverters.ByteHexConverter
// Assembly: MyCSLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 70B056C4-DB31-49BD-9F27-15818C65F327
// Assembly location: C:\Tools\HopeRF\RFM9x_GUI_1_1_0_9\MyCSLib.dll

using System;
using System.ComponentModel;
using System.Globalization;

namespace MyCSLib.General.TypeConverters
{
  public class ByteHexConverter : ByteConverter
  {
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type t)
    {
      if (t == typeof (string))
        return true;
      return base.CanConvertFrom(context, t);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo info, object value)
    {
      if (value is string)
      {
        try
        {
          string str = (string) value;
          if (str.StartsWith("0x", true, info) && str.Length <= 4 || str.Length <= 2)
            return (object) Convert.ToByte(str, 16);
        }
        catch
        {
        }
        throw new ArgumentException("Can not convert '" + (string) value + "' to type ByteHexConverter");
      }
      return base.ConvertFrom(context, info, value);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
    {
      if (destType == typeof (string) && value is byte)
        return (object) ("0x" + ((byte) value).ToString("X02"));
      return base.ConvertTo(context, culture, value, destType);
    }
  }
}

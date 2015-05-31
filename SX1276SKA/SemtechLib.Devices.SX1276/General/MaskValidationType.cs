// Decompiled with JetBrains decompiler
// Type: SemtechLib.Devices.SX1276.General.MaskValidationType
// Assembly: SemtechLib.Devices.SX1276, Version=2.0.0.0, Culture=neutral, PublicKeyToken=a3cee8af388a4f11
// MVID: 64B2382D-7AA3-4D8B-BE9D-2E742AB27E64
// Assembly location: C:\Tools\Semtech\SX1276SKA\SemtechLib.Devices.SX1276.dll

using System;
using System.Globalization;

namespace SemtechLib.Devices.SX1276.General
{
  public class MaskValidationType
  {
    private static int length = 1;
    private byte[] arrayValue;

    public static int Length
    {
      get
      {
        return MaskValidationType.length;
      }
    }

    public static MaskValidationType InvalidMask
    {
      get
      {
        return new MaskValidationType(new byte[MaskValidationType.Length]);
      }
    }

    public string StringValue
    {
      get
      {
        return this.ToString();
      }
      set
      {
        try
        {
          string[] strArray = value.Split('-');
          this.arrayValue = new byte[strArray.Length];
          for (int index = 0; index < strArray.Length; ++index)
            this.arrayValue[index] = Convert.ToByte(strArray[index], 16);
        }
        catch
        {
        }
        finally
        {
          MaskValidationType.length = this.arrayValue.Length;
        }
      }
    }

    public byte[] ArrayValue
    {
      get
      {
        return this.arrayValue;
      }
      set
      {
        if (this.arrayValue == null)
          this.arrayValue = new byte[1];
        if (value == null)
          throw new ArgumentNullException("The array cannot be null.");
        if (value.Length < 1 && value.Length > 8)
          throw new ArgumentException("Array should have as size comprized between 1 and 8.");
        if (this.arrayValue.Length != value.Length)
          Array.Resize<byte>(ref this.arrayValue, value.Length);
        Array.Copy((Array) value, (Array) this.arrayValue, value.Length);
        MaskValidationType.length = value.Length;
      }
    }

    public MaskValidationType()
    {
      this.arrayValue = new byte[1];
      MaskValidationType.length = 1;
    }

    public MaskValidationType(string stringValue)
    {
      this.StringValue = stringValue;
    }

    public MaskValidationType(byte[] array)
    {
      this.ArrayValue = array;
    }

    private static void doParsing(string s, out byte[] bytes)
    {
      s = s.Replace(" ", "");
      string[] strArray = s.Split('-');
      bytes = new byte[strArray.Length];
      try
      {
        int index = 0;
        foreach (string str in strArray)
        {
          bytes[index] = Convert.ToByte(str, 16);
          ++index;
        }
      }
      catch
      {
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The provided string {0} is not valid", new object[1]
        {
          (object) s
        }));
      }
    }

    public static MaskValidationType Parse(string s)
    {
      byte[] bytes;
      MaskValidationType.doParsing(s, out bytes);
      return new MaskValidationType(bytes);
    }

    public override string ToString()
    {
      string str = "";
      int index;
      for (index = 0; index < this.arrayValue.Length - 1; ++index)
        str = str + this.arrayValue[index].ToString("X02", (IFormatProvider) CultureInfo.CurrentCulture) + "-";
      return str + this.arrayValue[index].ToString("X02", (IFormatProvider) CultureInfo.CurrentCulture);
    }
  }
}

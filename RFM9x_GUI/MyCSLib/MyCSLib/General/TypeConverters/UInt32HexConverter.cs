using System;
using System.ComponentModel;
using System.Globalization;

namespace MyCSLib.General.TypeConverters
{
	public class UInt32HexConverter : UInt32Converter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type t)
		{
			if (t == typeof(string))
				return true;
			return base.CanConvertFrom(context, t);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo info, object value)
		{
			if (value is string)
			{
				try
				{
					string str = (string)value;
					if (str.StartsWith("0x", true, info) && str.Length <= 10 || str.Length <= 8)
						return (object)Convert.ToUInt32(str, 16);
				}
				catch { }
				throw new ArgumentException("Can not convert '" + (string)value + "' to type UInt32HexConverter");
			}
			return base.ConvertFrom(context, info, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
		{
			if (destType == typeof(string) && value is uint)
				return (object)("0x" + ((uint)value).ToString("X08"));
			return base.ConvertTo(context, culture, value, destType);
		}
	}
}

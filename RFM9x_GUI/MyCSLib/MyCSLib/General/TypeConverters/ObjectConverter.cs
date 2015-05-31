// Decompiled with JetBrains decompiler
// Type: MyCSLib.General.TypeConverters.ObjectConverter
// Assembly: MyCSLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 70B056C4-DB31-49BD-9F27-15818C65F327
// Assembly location: C:\Tools\HopeRF\RFM9x_GUI_1_1_0_9\MyCSLib.dll

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace MyCSLib.General.TypeConverters
{
  public class ObjectConverter : TypeConverter
  {
    private Type objectType;

    protected ObjectConverter(Type ObjectType)
    {
      this.objectType = ObjectType;
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      if (destinationType == typeof (InstanceDescriptor))
        return true;
      return base.CanConvertTo(context, destinationType);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      if (destinationType == typeof (InstanceDescriptor) && value.GetType() == this.objectType)
      {
        ConstructorInfo constructor = this.objectType.GetConstructor(new Type[0]);
        if (constructor != (ConstructorInfo) null)
          return (object) new InstanceDescriptor((MemberInfo) constructor, (ICollection) new object[0], false);
      }
      return base.ConvertTo(context, culture, value, destinationType);
    }
  }
}

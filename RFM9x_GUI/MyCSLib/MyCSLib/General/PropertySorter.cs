// Decompiled with JetBrains decompiler
// Type: MyCSLib.General.PropertySorter
// Assembly: MyCSLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 70B056C4-DB31-49BD-9F27-15818C65F327
// Assembly location: C:\Tools\HopeRF\RFM9x_GUI_1_1_0_9\MyCSLib.dll

using System;
using System.Collections;
using System.ComponentModel;

namespace MyCSLib.General
{
  public class PropertySorter : ExpandableObjectConverter
  {
    public override bool GetPropertiesSupported(ITypeDescriptorContext context)
    {
      return true;
    }

    public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
    {
      PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value, attributes);
      ArrayList arrayList1 = new ArrayList();
      foreach (PropertyDescriptor propertyDescriptor in properties)
      {
        Attribute attribute = propertyDescriptor.Attributes[typeof (PropertyOrderAttribute)];
        if (attribute != null)
        {
          PropertyOrderAttribute propertyOrderAttribute = (PropertyOrderAttribute) attribute;
          arrayList1.Add((object) new PropertyOrderPair(propertyDescriptor.Name, propertyOrderAttribute.Order));
        }
        else
          arrayList1.Add((object) new PropertyOrderPair(propertyDescriptor.Name, 0));
      }
      arrayList1.Sort();
      ArrayList arrayList2 = new ArrayList();
      foreach (PropertyOrderPair propertyOrderPair in arrayList1)
        arrayList2.Add((object) propertyOrderPair.Name);
      return properties.Sort((string[]) arrayList2.ToArray(typeof (string)));
    }
  }
}

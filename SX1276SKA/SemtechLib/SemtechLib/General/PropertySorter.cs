using System;
using System.Collections;
using System.ComponentModel;

namespace SemtechLib.General
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
				Attribute attribute = propertyDescriptor.Attributes[typeof(PropertyOrderAttribute)];
				if (attribute != null)
				{
					PropertyOrderAttribute propertyOrderAttribute = (PropertyOrderAttribute)attribute;
					arrayList1.Add((object)new PropertyOrderPair(propertyDescriptor.Name, propertyOrderAttribute.Order));
				}
				else
					arrayList1.Add((object)new PropertyOrderPair(propertyDescriptor.Name, 0));
			}
			arrayList1.Sort();
			ArrayList arrayList2 = new ArrayList();
			foreach (PropertyOrderPair propertyOrderPair in arrayList1)
				arrayList2.Add((object)propertyOrderPair.Name);
			return properties.Sort((string[])arrayList2.ToArray(typeof(string)));
		}
	}
}

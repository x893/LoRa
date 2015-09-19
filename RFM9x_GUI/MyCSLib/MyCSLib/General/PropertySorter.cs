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
			ArrayList props = new ArrayList();
			foreach (PropertyDescriptor propertyDescriptor in properties)
			{
				Attribute attribute = propertyDescriptor.Attributes[typeof(PropertyOrderAttribute)];
				if (attribute != null)
				{
					PropertyOrderAttribute propertyOrderAttribute = (PropertyOrderAttribute)attribute;
					props.Add(new PropertyOrderPair(propertyDescriptor.Name, propertyOrderAttribute.Order));
				}
				else
					props.Add(new PropertyOrderPair(propertyDescriptor.Name, 0));
			}
			props.Sort();
			ArrayList sorted = new ArrayList();
			foreach (PropertyOrderPair propertyOrderPair in props)
				sorted.Add(propertyOrderPair.Name);
			return properties.Sort((string[])sorted.ToArray(typeof(string)));
		}
	}
}

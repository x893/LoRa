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
		private Type m_type;

		protected ObjectConverter(Type ObjectType)
		{
			m_type = ObjectType;
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))
				return true;
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor) && value.GetType() == this.m_type)
			{
				ConstructorInfo constructor = this.m_type.GetConstructor(new Type[0]);
				if (constructor != (ConstructorInfo)null)
					return (object)new InstanceDescriptor((MemberInfo)constructor, (ICollection)new object[0], false);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}

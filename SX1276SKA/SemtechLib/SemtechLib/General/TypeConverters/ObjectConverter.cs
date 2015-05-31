using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace SemtechLib.General.TypeConverters
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
			if (destinationType == typeof(InstanceDescriptor))
				return true;
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor) && value.GetType() == this.objectType)
			{
				ConstructorInfo constructor = this.objectType.GetConstructor(new Type[0]);
				if (constructor != (ConstructorInfo)null)
					return (object)new InstanceDescriptor((MemberInfo)constructor, (ICollection)new object[0], false);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}

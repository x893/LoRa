using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;

namespace MyCSLib.General
{
	public class EditableObject : IEditableObject
	{
		private BindingCollectionBase collection = (BindingCollectionBase)null;
		private object[] originalValues = (object[])null;

		protected BindingCollectionBase Collection
		{
			get
			{
				return this.collection;
			}
		}

		private bool PendingInsert
		{
			get
			{
				return this.collection.pendingInsert == this;
			}
		}

		private bool IsEdit
		{
			get
			{
				return this.originalValues != null;
			}
		}

		internal void SetCollection(BindingCollectionBase Collection)
		{
			this.collection = Collection;
		}

		void IEditableObject.BeginEdit()
		{
			Trace.WriteLine("BeginEdit");
			if (this.IsEdit)
				return;
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties((object)this, (Attribute[])null);
			object[] objArray = new object[properties.Count];
			for (int index = 0; index < properties.Count; ++index)
			{
				objArray[index] = (object)EditableObject.NotCopied.Value;
				PropertyDescriptor propertyDescriptor = properties[index];
				if (propertyDescriptor.PropertyType.IsSubclassOf(typeof(ValueType)))
				{
					objArray[index] = propertyDescriptor.GetValue((object)this);
				}
				else
				{
					object obj = propertyDescriptor.GetValue((object)this);
					if (obj == null)
						objArray[index] = (object)null;
					else if (!(obj is IList) && obj is ICloneable)
						objArray[index] = ((ICloneable)obj).Clone();
				}
			}
			this.originalValues = objArray;
		}

		void IEditableObject.CancelEdit()
		{
			Trace.WriteLine("CancelEdit");
			if (!this.IsEdit)
				return;
			if (this.PendingInsert)
				((IList)this.collection).Remove((object)this);
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties((object)this, (Attribute[])null);
			for (int index = 0; index < properties.Count; ++index)
			{
				if (!(this.originalValues[index] is EditableObject.NotCopied))
					properties[index].SetValue((object)this, this.originalValues[index]);
			}
			this.originalValues = (object[])null;
		}

		void IEditableObject.EndEdit()
		{
			Trace.WriteLine("EndEdit");
			if (!this.IsEdit)
				return;
			if (this.PendingInsert)
				this.collection.pendingInsert = (object)null;
			this.originalValues = (object[])null;
		}

		private class NotCopied
		{
			private static EditableObject.NotCopied value = new EditableObject.NotCopied();

			public static EditableObject.NotCopied Value
			{
				get
				{
					return EditableObject.NotCopied.value;
				}
			}
		}
	}
}

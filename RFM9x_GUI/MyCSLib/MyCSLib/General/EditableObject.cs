using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;

namespace MyCSLib.General
{
	public class EditableObject : IEditableObject
	{
		private BindingCollectionBase m_collection = (BindingCollectionBase)null;
		private object[] m_originalValues = (object[])null;

		protected BindingCollectionBase Collection
		{
			get { return m_collection; }
		}

		private bool PendingInsert
		{
			get { return m_collection.pendingInsert == this; }
		}

		private bool IsEdit
		{
			get { return m_originalValues != null; }
		}

		internal void SetCollection(BindingCollectionBase Collection)
		{
			m_collection = Collection;
		}

		void IEditableObject.BeginEdit()
		{
			Trace.WriteLine("BeginEdit");
			if (IsEdit)
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
			m_originalValues = objArray;
		}

		void IEditableObject.CancelEdit()
		{
			Trace.WriteLine("CancelEdit");
			if (!IsEdit)
				return;
			if (PendingInsert)
				((IList)m_collection).Remove((object)this);
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties((object)this, (Attribute[])null);
			for (int index = 0; index < properties.Count; ++index)
				if (!(m_originalValues[index] is EditableObject.NotCopied))
					properties[index].SetValue((object)this, m_originalValues[index]);
			m_originalValues = (object[])null;
		}

		void IEditableObject.EndEdit()
		{
			Trace.WriteLine("EndEdit");
			if (!IsEdit)
				return;
			if (PendingInsert)
				m_collection.pendingInsert = (object)null;
			m_originalValues = (object[])null;
		}

		private class NotCopied
		{
			private static EditableObject.NotCopied value = new EditableObject.NotCopied();

			public static EditableObject.NotCopied Value
			{
				get { return EditableObject.NotCopied.value; }
			}
		}
	}
}

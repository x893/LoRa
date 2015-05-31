using System;
using System.Collections;
using System.ComponentModel;

namespace SemtechLib.General
{
	public abstract class BindingCollectionBase : IBindingList, IList, ICollection, IEnumerable
	{
		private ArrayList list;
		internal object pendingInsert;
		private ListChangedEventHandler listChanged;

		public int Count
		{
			get
			{
				return list.Count;
			}
		}

		protected IList List
		{
			get
			{
				return (IList)this;
			}
		}

		protected virtual Type ElementType
		{
			get
			{
				return typeof(object);
			}
		}

		bool IBindingList.AllowEdit
		{
			get
			{
				return true;
			}
		}

		bool IBindingList.AllowNew
		{
			get
			{
				return true;
			}
		}

		bool IBindingList.AllowRemove
		{
			get
			{
				return true;
			}
		}

		bool IBindingList.SupportsChangeNotification
		{
			get
			{
				return true;
			}
		}

		bool IBindingList.SupportsSearching
		{
			get
			{
				return false;
			}
		}

		bool IBindingList.SupportsSorting
		{
			get
			{
				return false;
			}
		}

		bool IBindingList.IsSorted
		{
			get
			{
				return false;
			}
		}

		ListSortDirection IBindingList.SortDirection
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		PropertyDescriptor IBindingList.SortProperty
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return list.IsSynchronized;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return list.SyncRoot;
			}
		}

		object IList.this[int index]
		{
			get
			{
				if (index < 0 || index >= list.Count)
					throw new ArgumentOutOfRangeException();
				return list[index];
			}
			set
			{
				if (index < 0 || index >= list.Count)
					throw new ArgumentOutOfRangeException();
				OnValidate(value);
				object oldValue = list[index];
				OnSet(index, oldValue, value);
				list[index] = value;
				try
				{
					OnSetComplete(index, oldValue, value);
				}
				catch
				{
					list[index] = oldValue;
					throw;
				}
				if (listChanged == null)
					return;
				listChanged((object)this, new ListChangedEventArgs(ListChangedType.ItemChanged, index));
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return list.IsFixedSize;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return list.IsReadOnly;
			}
		}

		event ListChangedEventHandler IBindingList.ListChanged
		{
			add
			{
				listChanged += value;
			}
			remove
			{
				listChanged -= value;
			}
		}

		protected BindingCollectionBase()
		{
			list = new ArrayList();
			pendingInsert = (object)null;
		}

		public void Clear()
		{
			OnClear();
			for (int index = 0; index < list.Count; ++index)
				((EditableObject)list[index]).SetCollection((BindingCollectionBase)null);
			list.Clear();
			pendingInsert = (object)null;
			OnClearComplete();
			if (listChanged == null)
				return;
			listChanged((object)this, new ListChangedEventArgs(ListChangedType.Reset, 0));
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= list.Count)
				throw new ArgumentOutOfRangeException();
			object obj = list[index];
			OnValidate(obj);
			OnRemove(index, obj);
			((EditableObject)list[index]).SetCollection((BindingCollectionBase)null);
			if (pendingInsert == obj)
				pendingInsert = (object)null;
			list.RemoveAt(index);
			OnRemoveComplete(index, obj);
			if (listChanged == null)
				return;
			listChanged((object)this, new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
		}

		public IEnumerator GetEnumerator()
		{
			return list.GetEnumerator();
		}

		protected virtual object CreateInstance()
		{
			return Activator.CreateInstance(ElementType);
		}

		object IBindingList.AddNew()
		{
			if (pendingInsert != null)
				((IEditableObject)pendingInsert).CancelEdit();
			object instance = CreateInstance();
			((IList)this).Add(instance);
			pendingInsert = instance;
			return instance;
		}

		void IBindingList.AddIndex(PropertyDescriptor property)
		{
			throw new NotSupportedException();
		}

		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
		{
			throw new NotSupportedException();
		}

		int IBindingList.Find(PropertyDescriptor property, object key)
		{
			throw new NotSupportedException();
		}

		void IBindingList.RemoveIndex(PropertyDescriptor property)
		{
			throw new NotSupportedException();
		}

		void IBindingList.RemoveSort()
		{
			throw new NotSupportedException();
		}

		void ICollection.CopyTo(Array array, int index)
		{
			list.CopyTo(array, index);
		}

		int IList.Add(object value)
		{
			OnValidate(value);
			OnInsert(list.Count, value);
			int num = list.Add(value);
			try
			{
				OnInsertComplete(num, value);
			}
			catch
			{
				list.RemoveAt(num);
				throw;
			}
			((EditableObject)value).SetCollection(this);
			if (listChanged != null)
				listChanged((object)this, new ListChangedEventArgs(ListChangedType.ItemAdded, num));
			return num;
		}

		bool IList.Contains(object value)
		{
			return list.Contains(value);
		}

		int IList.IndexOf(object value)
		{
			return list.IndexOf(value);
		}

		void IList.Insert(int index, object value)
		{
			if (index < 0 || index > list.Count)
				throw new ArgumentOutOfRangeException();
			OnValidate(value);
			OnInsert(index, value);
			list.Insert(index, value);
			try
			{
				OnInsertComplete(index, value);
			}
			catch
			{
				list.RemoveAt(index);
				throw;
			}
			((EditableObject)value).SetCollection(this);
			if (listChanged == null)
				return;
			listChanged((object)this, new ListChangedEventArgs(ListChangedType.ItemAdded, index));
		}

		void IList.Remove(object value)
		{
			OnValidate(value);
			int num = list.IndexOf(value);
			if (num < 0)
				throw new ArgumentException();
			OnRemove(num, value);
			list.RemoveAt(num);
			OnRemoveComplete(num, value);
			if (listChanged == null)
				return;
			listChanged((object)this, new ListChangedEventArgs(ListChangedType.ItemDeleted, num));
		}

		protected virtual void OnClear()
		{
		}

		protected virtual void OnClearComplete()
		{
		}

		protected virtual void OnInsert(int index, object value)
		{
		}

		protected virtual void OnInsertComplete(int index, object value)
		{
		}

		protected virtual void OnRemove(int index, object value)
		{
		}

		protected virtual void OnRemoveComplete(int index, object value)
		{
		}

		protected virtual void OnSet(int index, object oldValue, object newValue)
		{
		}

		protected virtual void OnSetComplete(int index, object oldValue, object newValue)
		{
		}

		protected virtual void OnValidate(object value)
		{
			if (value == null)
				throw new ArgumentNullException("value");
		}
	}
}
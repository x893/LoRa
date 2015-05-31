using System;
using System.Collections;
using System.ComponentModel;

namespace MyCSLib.General
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
				return this.list.Count;
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
				return this.list.IsSynchronized;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this.list.SyncRoot;
			}
		}

		object IList.this[int index]
		{
			get
			{
				if (index < 0 || index >= this.list.Count)
					throw new ArgumentOutOfRangeException();
				return this.list[index];
			}
			set
			{
				if (index < 0 || index >= this.list.Count)
					throw new ArgumentOutOfRangeException();
				this.OnValidate(value);
				object oldValue = this.list[index];
				this.OnSet(index, oldValue, value);
				this.list[index] = value;
				try
				{
					this.OnSetComplete(index, oldValue, value);
				}
				catch
				{
					this.list[index] = oldValue;
					throw;
				}
				if (this.listChanged == null)
					return;
				this.listChanged((object)this, new ListChangedEventArgs(ListChangedType.ItemChanged, index));
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return this.list.IsFixedSize;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return this.list.IsReadOnly;
			}
		}

		event ListChangedEventHandler IBindingList.ListChanged
		{
			add
			{
				this.listChanged += value;
			}
			remove
			{
				this.listChanged -= value;
			}
		}

		protected BindingCollectionBase()
		{
			this.list = new ArrayList();
			this.pendingInsert = (object)null;
		}

		public void Clear()
		{
			this.OnClear();
			for (int index = 0; index < this.list.Count; ++index)
				((EditableObject)this.list[index]).SetCollection((BindingCollectionBase)null);
			this.list.Clear();
			this.pendingInsert = (object)null;
			this.OnClearComplete();
			if (this.listChanged == null)
				return;
			this.listChanged((object)this, new ListChangedEventArgs(ListChangedType.Reset, 0));
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= this.list.Count)
				throw new ArgumentOutOfRangeException();
			object obj = this.list[index];
			this.OnValidate(obj);
			this.OnRemove(index, obj);
			((EditableObject)this.list[index]).SetCollection((BindingCollectionBase)null);
			if (this.pendingInsert == obj)
				this.pendingInsert = (object)null;
			this.list.RemoveAt(index);
			this.OnRemoveComplete(index, obj);
			if (this.listChanged == null)
				return;
			this.listChanged((object)this, new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
		}

		public IEnumerator GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		protected virtual object CreateInstance()
		{
			return Activator.CreateInstance(this.ElementType);
		}

		object IBindingList.AddNew()
		{
			if (this.pendingInsert != null)
				((IEditableObject)this.pendingInsert).CancelEdit();
			object instance = this.CreateInstance();
			((IList)this).Add(instance);
			this.pendingInsert = instance;
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
			this.list.CopyTo(array, index);
		}

		int IList.Add(object value)
		{
			this.OnValidate(value);
			this.OnInsert(this.list.Count, value);
			int num = this.list.Add(value);
			try
			{
				this.OnInsertComplete(num, value);
			}
			catch
			{
				this.list.RemoveAt(num);
				throw;
			}
			((EditableObject)value).SetCollection(this);
			if (this.listChanged != null)
				this.listChanged((object)this, new ListChangedEventArgs(ListChangedType.ItemAdded, num));
			return num;
		}

		bool IList.Contains(object value)
		{
			return this.list.Contains(value);
		}

		int IList.IndexOf(object value)
		{
			return this.list.IndexOf(value);
		}

		void IList.Insert(int index, object value)
		{
			if (index < 0 || index > this.list.Count)
				throw new ArgumentOutOfRangeException();
			this.OnValidate(value);
			this.OnInsert(index, value);
			this.list.Insert(index, value);
			try
			{
				this.OnInsertComplete(index, value);
			}
			catch
			{
				this.list.RemoveAt(index);
				throw;
			}
			((EditableObject)value).SetCollection(this);
			if (this.listChanged == null)
				return;
			this.listChanged((object)this, new ListChangedEventArgs(ListChangedType.ItemAdded, index));
		}

		void IList.Remove(object value)
		{
			this.OnValidate(value);
			int num = this.list.IndexOf(value);
			if (num < 0)
				throw new ArgumentException();
			this.OnRemove(num, value);
			this.list.RemoveAt(num);
			this.OnRemoveComplete(num, value);
			if (this.listChanged == null)
				return;
			this.listChanged((object)this, new ListChangedEventArgs(ListChangedType.ItemDeleted, num));
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

using System;
using System.Collections;
using System.ComponentModel;

namespace MyCSLib.General
{
	public abstract class BindingCollectionBase : IBindingList, IList, ICollection, IEnumerable
	{
		private ArrayList m_list;
		private ListChangedEventHandler m_listChanged;
		internal object pendingInsert;

		public int Count
		{
			get { return m_list.Count; }
		}

		protected IList List
		{
			get { return (IList)this; }
		}

		protected virtual Type ElementType
		{
			get { return typeof(object); }
		}

		bool IBindingList.AllowEdit
		{
			get { return true; }
		}

		bool IBindingList.AllowNew
		{
			get { return true; }
		}

		bool IBindingList.AllowRemove
		{
			get { return true; }
		}

		bool IBindingList.SupportsChangeNotification
		{
			get { return true; }
		}

		bool IBindingList.SupportsSearching
		{
			get { return false; }
		}

		bool IBindingList.SupportsSorting
		{
			get { return false; }
		}

		bool IBindingList.IsSorted
		{
			get { return false; }
		}

		ListSortDirection IBindingList.SortDirection
		{
			get { throw new NotSupportedException(); }
		}

		PropertyDescriptor IBindingList.SortProperty
		{
			get { throw new NotSupportedException(); }
		}

		bool ICollection.IsSynchronized
		{
			get { return m_list.IsSynchronized; }
		}

		object ICollection.SyncRoot
		{
			get { return m_list.SyncRoot; }
		}

		object IList.this[int index]
		{
			get
			{
				if (index < 0 || index >= m_list.Count)
					throw new ArgumentOutOfRangeException();
				return m_list[index];
			}
			set
			{
				if (index < 0 || index >= m_list.Count)
					throw new ArgumentOutOfRangeException();
				OnValidate(value);
				object oldValue = m_list[index];
				OnSet(index, oldValue, value);
				m_list[index] = value;
				try
				{
					OnSetComplete(index, oldValue, value);
				}
				catch
				{
					m_list[index] = oldValue;
					throw;
				}
				if (m_listChanged == null)
					return;
				m_listChanged((object)this, new ListChangedEventArgs(ListChangedType.ItemChanged, index));
			}
		}

		bool IList.IsFixedSize
		{
			get { return m_list.IsFixedSize; }
		}

		bool IList.IsReadOnly
		{
			get { return m_list.IsReadOnly; }
		}

		event ListChangedEventHandler IBindingList.ListChanged
		{
			add { m_listChanged += value; }
			remove { m_listChanged -= value; }
		}

		protected BindingCollectionBase()
		{
			m_list = new ArrayList();
			pendingInsert = null;
		}

		public void Clear()
		{
			OnClear();
			for (int index = 0; index < m_list.Count; ++index)
				((EditableObject)m_list[index]).SetCollection((BindingCollectionBase)null);
			m_list.Clear();
			pendingInsert = null;
			OnClearComplete();
			if (m_listChanged == null)
				return;
			m_listChanged(this, new ListChangedEventArgs(ListChangedType.Reset, 0));
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= m_list.Count)
				throw new ArgumentOutOfRangeException();
			object obj = m_list[index];
			OnValidate(obj);
			OnRemove(index, obj);
			((EditableObject)m_list[index]).SetCollection((BindingCollectionBase)null);
			if (pendingInsert == obj)
				pendingInsert = (object)null;
			m_list.RemoveAt(index);
			OnRemoveComplete(index, obj);
			if (m_listChanged == null)
				return;
			m_listChanged((object)this, new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
		}

		public IEnumerator GetEnumerator()
		{
			return m_list.GetEnumerator();
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
			m_list.CopyTo(array, index);
		}

		int IList.Add(object value)
		{
			OnValidate(value);
			OnInsert(m_list.Count, value);
			int num = m_list.Add(value);
			try
			{
				OnInsertComplete(num, value);
			}
			catch
			{
				m_list.RemoveAt(num);
				throw;
			}
			((EditableObject)value).SetCollection(this);
			if (m_listChanged != null)
				m_listChanged((object)this, new ListChangedEventArgs(ListChangedType.ItemAdded, num));
			return num;
		}

		bool IList.Contains(object value)
		{
			return m_list.Contains(value);
		}

		int IList.IndexOf(object value)
		{
			return m_list.IndexOf(value);
		}

		void IList.Insert(int index, object value)
		{
			if (index < 0 || index > m_list.Count)
				throw new ArgumentOutOfRangeException();
			OnValidate(value);
			OnInsert(index, value);
			m_list.Insert(index, value);
			try
			{
				OnInsertComplete(index, value);
			}
			catch
			{
				m_list.RemoveAt(index);
				throw;
			}
			((EditableObject)value).SetCollection(this);
			if (m_listChanged == null)
				return;
			m_listChanged((object)this, new ListChangedEventArgs(ListChangedType.ItemAdded, index));
		}

		void IList.Remove(object value)
		{
			OnValidate(value);
			int num = m_list.IndexOf(value);
			if (num < 0)
				throw new ArgumentException();
			OnRemove(num, value);
			m_list.RemoveAt(num);
			OnRemoveComplete(num, value);
			if (m_listChanged == null)
				return;
			m_listChanged((object)this, new ListChangedEventArgs(ListChangedType.ItemDeleted, num));
		}

		protected virtual void OnClear(){ }
		protected virtual void OnClearComplete() { }
		protected virtual void OnInsert(int index, object value) { }
		protected virtual void OnInsertComplete(int index, object value) { }
		protected virtual void OnRemove(int index, object value) { }
		protected virtual void OnRemoveComplete(int index, object value) { }
		protected virtual void OnSet(int index, object oldValue, object newValue) { }
		protected virtual void OnSetComplete(int index, object oldValue, object newValue) { }

		protected virtual void OnValidate(object value)
		{
			if (value == null)
				throw new ArgumentNullException("value");
		}
	}
}

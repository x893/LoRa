using System;
using System.Collections;

namespace MyCSLib.General
{
	public class RegisterCollection : CollectionBase
	{
		public event EventHandler DataInserted;

		public Register this[int index]
		{
			get { return (Register)List[index]; }
			set { List[index] = (object)value; }
		}

		public Register this[string name]
		{
			get
			{
				foreach (Register register in (IEnumerable)List)
					if (register.Name == name)
						return register;
				return (Register)null;
			}
			set
			{
				foreach (Register register in (IEnumerable)List)
					if (register.Name == name)
						List[(int)register.Address] = (object)value;
			}
		}

		public RegisterCollection()
		{
		}

		public RegisterCollection(RegisterCollection value)
		{
			AddRange(value);
		}

		public RegisterCollection(Register[] value)
		{
			AddRange(value);
		}

		public int Add(Register value)
		{
			return List.Add((object)value);
		}

		public void AddRange(Register[] value)
		{
			for (int index = 0; index < value.Length; ++index)
				Add(value[index]);
		}

		public void AddRange(RegisterCollection value)
		{
			for (int index = 0; index < value.Count; ++index)
				Add(value[index]);
		}

		public bool Contains(Register value)
		{
			return List.Contains((object)value);
		}

		public void CopyTo(Register[] array, int index)
		{
			List.CopyTo((Array)array, index);
		}

		public int IndexOf(Register value)
		{
			return List.IndexOf((object)value);
		}

		public void Insert(int index, Register value)
		{
			List.Insert(index, (object)value);
		}

		public new RegisterCollection.RegisterEnumerator GetEnumerator()
		{
			return new RegisterCollection.RegisterEnumerator(this);
		}

		public void Remove(Register value)
		{
			--Capacity;
			List.Remove((object)value);
		}

		protected override void OnInsert(int index, object value)
		{
			base.OnInsert(index, value);
			if (DataInserted == null)
				return;
			DataInserted((object)this, EventArgs.Empty);
		}

		public class RegisterEnumerator : IEnumerator
		{
			private IEnumerator m_baseEnumerator;
			private IEnumerable m_temp;

			public Register Current
			{
				get { return (Register)m_baseEnumerator.Current; }
			}

			object IEnumerator.Current
			{
				get { return m_baseEnumerator.Current; }
			}

			public RegisterEnumerator(RegisterCollection mappings)
			{
				m_temp = (IEnumerable)mappings;
				m_baseEnumerator = m_temp.GetEnumerator();
			}

			public bool MoveNext()
			{
				return m_baseEnumerator.MoveNext();
			}

			bool IEnumerator.MoveNext()
			{
				return m_baseEnumerator.MoveNext();
			}

			public void Reset()
			{
				m_baseEnumerator.Reset();
			}

			void IEnumerator.Reset()
			{
				m_baseEnumerator.Reset();
			}
		}
	}
}

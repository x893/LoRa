using System;
using System.Collections;

namespace SemtechLib.General
{
	public class RegisterCollection : CollectionBase
	{
		public Register this[int index]
		{
			get { return (Register)List[index]; }
			set { List[index] = value; }
		}

		public Register this[string name]
		{
			get
			{
				foreach (Register register in (IEnumerable)List)
				{
					if (register.Name == name)
						return register;
				}
				return (Register)null;
			}
			set
			{
				foreach (Register register in (IEnumerable)List)
				{
					if (register.Name == name)
						List[(int)register.Address] = value;
				}
			}
		}

		public event EventHandler DataInserted;

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
			try
			{
				List.Remove((object)value);
				--Capacity;
			}
			catch { }
		}

		public void RemoveRange(Register[] value)
		{
			for (int index = 0; index < value.Length; ++index)
				Remove(value[index]);
		}

		public void RemoveRange(RegisterCollection value)
		{
			for (int index = 0; index < value.Count; ++index)
				Remove(value[index]);
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
			private IEnumerator baseEnumerator;
			private IEnumerable temp;

			public Register Current
			{
				get
				{
					return (Register)baseEnumerator.Current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return baseEnumerator.Current;
				}
			}

			public RegisterEnumerator(RegisterCollection mappings)
			{
				temp = (IEnumerable)mappings;
				baseEnumerator = temp.GetEnumerator();
			}

			public bool MoveNext()
			{
				return baseEnumerator.MoveNext();
			}

			bool IEnumerator.MoveNext()
			{
				return baseEnumerator.MoveNext();
			}

			public void Reset()
			{
				baseEnumerator.Reset();
			}

			void IEnumerator.Reset()
			{
				baseEnumerator.Reset();
			}
		}
	}
}
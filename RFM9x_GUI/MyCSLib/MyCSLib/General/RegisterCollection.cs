using System;
using System.Collections;

namespace MyCSLib.General
{
	public class RegisterCollection : CollectionBase
	{
		public Register this[int index]
		{
			get
			{
				return (Register)this.List[index];
			}
			set
			{
				this.List[index] = (object)value;
			}
		}

		public Register this[string name]
		{
			get
			{
				foreach (Register register in (IEnumerable)this.List)
				{
					if (register.Name == name)
						return register;
				}
				return (Register)null;
			}
			set
			{
				foreach (Register register in (IEnumerable)this.List)
				{
					if (register.Name == name)
						this.List[(int)register.Address] = (object)value;
				}
			}
		}

		public event EventHandler DataInserted;

		public RegisterCollection()
		{
		}

		public RegisterCollection(RegisterCollection value)
		{
			this.AddRange(value);
		}

		public RegisterCollection(Register[] value)
		{
			this.AddRange(value);
		}

		public int Add(Register value)
		{
			return this.List.Add((object)value);
		}

		public void AddRange(Register[] value)
		{
			for (int index = 0; index < value.Length; ++index)
				this.Add(value[index]);
		}

		public void AddRange(RegisterCollection value)
		{
			for (int index = 0; index < value.Count; ++index)
				this.Add(value[index]);
		}

		public bool Contains(Register value)
		{
			return this.List.Contains((object)value);
		}

		public void CopyTo(Register[] array, int index)
		{
			this.List.CopyTo((Array)array, index);
		}

		public int IndexOf(Register value)
		{
			return this.List.IndexOf((object)value);
		}

		public void Insert(int index, Register value)
		{
			this.List.Insert(index, (object)value);
		}

		public new RegisterCollection.RegisterEnumerator GetEnumerator()
		{
			return new RegisterCollection.RegisterEnumerator(this);
		}

		public void Remove(Register value)
		{
			--this.Capacity;
			this.List.Remove((object)value);
		}

		protected override void OnInsert(int index, object value)
		{
			base.OnInsert(index, value);
			if (this.DataInserted == null)
				return;
			this.DataInserted((object)this, EventArgs.Empty);
		}

		public class RegisterEnumerator : IEnumerator
		{
			private IEnumerator baseEnumerator;
			private IEnumerable temp;

			public Register Current
			{
				get
				{
					return (Register)this.baseEnumerator.Current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.baseEnumerator.Current;
				}
			}

			public RegisterEnumerator(RegisterCollection mappings)
			{
				this.temp = (IEnumerable)mappings;
				this.baseEnumerator = this.temp.GetEnumerator();
			}

			public bool MoveNext()
			{
				return this.baseEnumerator.MoveNext();
			}

			bool IEnumerator.MoveNext()
			{
				return this.baseEnumerator.MoveNext();
			}

			public void Reset()
			{
				this.baseEnumerator.Reset();
			}

			void IEnumerator.Reset()
			{
				this.baseEnumerator.Reset();
			}
		}
	}
}

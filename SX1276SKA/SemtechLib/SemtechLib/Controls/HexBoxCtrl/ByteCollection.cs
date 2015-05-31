using System;
using System.Collections;

namespace SemtechLib.Controls.HexBoxCtrl
{
	public class ByteCollection : CollectionBase
	{
		public byte this[int index]
		{
			get
			{
				return (byte)List[index];
			}
			set
			{
				List[index] = (object)value;
			}
		}

		public ByteCollection()
		{
		}

		public ByteCollection(byte[] bs)
		{
			AddRange(bs);
		}

		public void Add(byte b)
		{
			List.Add((object)b);
		}

		public void AddRange(byte[] bs)
		{
			InnerList.AddRange((ICollection)bs);
		}

		public void Remove(byte b)
		{
			List.Remove((object)b);
		}

		public void RemoveRange(int index, int count)
		{
			InnerList.RemoveRange(index, count);
		}

		public void InsertRange(int index, byte[] bs)
		{
			InnerList.InsertRange(index, (ICollection)bs);
		}

		public byte[] GetBytes()
		{
			byte[] numArray = new byte[Count];
			InnerList.CopyTo(0, (Array)numArray, 0, numArray.Length);
			return numArray;
		}

		public void Insert(int index, byte b)
		{
			InnerList.Insert(index, (object)b);
		}

		public int IndexOf(byte b)
		{
			return InnerList.IndexOf((object)b);
		}

		public bool Contains(byte b)
		{
			return InnerList.Contains((object)b);
		}

		public void CopyTo(byte[] bs, int index)
		{
			InnerList.CopyTo((Array)bs, index);
		}

		public byte[] ToArray()
		{
			byte[] bs = new byte[Count];
			CopyTo(bs, 0);
			return bs;
		}
	}
}

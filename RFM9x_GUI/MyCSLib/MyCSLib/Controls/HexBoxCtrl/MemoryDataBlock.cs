using System;

namespace MyCSLib.Controls.HexBoxCtrl
{
	internal sealed class MemoryDataBlock : DataBlock
	{
		private byte[] _data;

		public override long Length
		{
			get
			{
				return this._data.LongLength;
			}
		}

		public byte[] Data
		{
			get
			{
				return this._data;
			}
		}

		public MemoryDataBlock(byte data)
		{
			this._data = new byte[1]
      {
        data
      };
		}

		public MemoryDataBlock(byte[] data)
		{
			if (data == null)
				throw new ArgumentNullException("data");
			this._data = (byte[])data.Clone();
		}

		public void AddByteToEnd(byte value)
		{
			byte[] numArray = new byte[this._data.LongLength + 1L];
			this._data.CopyTo((Array)numArray, 0);
			numArray[numArray.LongLength - 1L] = value;
			this._data = numArray;
		}

		public void AddByteToStart(byte value)
		{
			byte[] numArray = new byte[this._data.LongLength + 1L];
			numArray[0] = value;
			this._data.CopyTo((Array)numArray, 1);
			this._data = numArray;
		}

		public void InsertBytes(long position, byte[] data)
		{
			byte[] numArray = new byte[this._data.LongLength + data.LongLength];
			if (position > 0L)
				Array.Copy((Array)this._data, 0L, (Array)numArray, 0L, position);
			Array.Copy((Array)data, 0L, (Array)numArray, position, data.LongLength);
			if (position < this._data.LongLength)
				Array.Copy((Array)this._data, position, (Array)numArray, position + data.LongLength, this._data.LongLength - position);
			this._data = numArray;
		}

		public override void RemoveBytes(long position, long count)
		{
			byte[] numArray = new byte[this._data.LongLength - count];
			if (position > 0L)
				Array.Copy((Array)this._data, 0L, (Array)numArray, 0L, position);
			if (position + count < this._data.LongLength)
				Array.Copy((Array)this._data, position + count, (Array)numArray, position, numArray.LongLength - position);
			this._data = numArray;
		}
	}
}

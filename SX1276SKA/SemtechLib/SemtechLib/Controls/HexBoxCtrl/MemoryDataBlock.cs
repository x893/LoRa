using System;

namespace SemtechLib.Controls.HexBoxCtrl
{
	internal sealed class MemoryDataBlock : DataBlock
	{
		private byte[] _data;

		public override long Length
		{
			get
			{
				return _data.LongLength;
			}
		}

		public byte[] Data
		{
			get
			{
				return _data;
			}
		}

		public MemoryDataBlock(byte data)
		{
			_data = new byte[1]
      {
        data
      };
		}

		public MemoryDataBlock(byte[] data)
		{
			if (data == null)
				throw new ArgumentNullException("data");
			_data = (byte[])data.Clone();
		}

		public void AddByteToEnd(byte value)
		{
			byte[] numArray = new byte[_data.LongLength + 1L];
			_data.CopyTo((Array)numArray, 0);
			numArray[numArray.LongLength - 1L] = value;
			_data = numArray;
		}

		public void AddByteToStart(byte value)
		{
			byte[] numArray = new byte[_data.LongLength + 1L];
			numArray[0] = value;
			_data.CopyTo((Array)numArray, 1);
			_data = numArray;
		}

		public void InsertBytes(long position, byte[] data)
		{
			byte[] numArray = new byte[_data.LongLength + data.LongLength];
			if (position > 0L)
				Array.Copy((Array)_data, 0L, (Array)numArray, 0L, position);
			Array.Copy((Array)data, 0L, (Array)numArray, position, data.LongLength);
			if (position < _data.LongLength)
				Array.Copy((Array)_data, position, (Array)numArray, position + data.LongLength, _data.LongLength - position);
			_data = numArray;
		}

		public override void RemoveBytes(long position, long count)
		{
			byte[] numArray = new byte[_data.LongLength - count];
			if (position > 0L)
				Array.Copy((Array)_data, 0L, (Array)numArray, 0L, position);
			if (position + count < _data.LongLength)
				Array.Copy((Array)_data, position + count, (Array)numArray, position, numArray.LongLength - position);
			_data = numArray;
		}
	}
}
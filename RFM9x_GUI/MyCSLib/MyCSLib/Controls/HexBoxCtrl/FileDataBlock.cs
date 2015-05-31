using System;

namespace MyCSLib.Controls.HexBoxCtrl
{
	internal sealed class FileDataBlock : DataBlock
	{
		private long _length;
		private long _fileOffset;

		public long FileOffset
		{
			get
			{
				return this._fileOffset;
			}
		}

		public override long Length
		{
			get
			{
				return this._length;
			}
		}

		public FileDataBlock(long fileOffset, long length)
		{
			this._fileOffset = fileOffset;
			this._length = length;
		}

		public void SetFileOffset(long value)
		{
			this._fileOffset = value;
		}

		public void RemoveBytesFromEnd(long count)
		{
			if (count > this._length)
				throw new ArgumentOutOfRangeException("count");
			this._length -= count;
		}

		public void RemoveBytesFromStart(long count)
		{
			if (count > this._length)
				throw new ArgumentOutOfRangeException("count");
			this._fileOffset += count;
			this._length -= count;
		}

		public override void RemoveBytes(long position, long count)
		{
			if (position > this._length)
				throw new ArgumentOutOfRangeException("position");
			if (position + count > this._length)
				throw new ArgumentOutOfRangeException("count");
			long num1 = position;
			long num2 = this._fileOffset;
			long length = this._length - count - num1;
			long fileOffset = this._fileOffset + position + count;
			if (num1 > 0L && length > 0L)
			{
				this._fileOffset = num2;
				this._length = num1;
				this._map.AddAfter((DataBlock)this, (DataBlock)new FileDataBlock(fileOffset, length));
			}
			else if (num1 > 0L)
			{
				this._fileOffset = num2;
				this._length = num1;
			}
			else
			{
				this._fileOffset = fileOffset;
				this._length = length;
			}
		}
	}
}
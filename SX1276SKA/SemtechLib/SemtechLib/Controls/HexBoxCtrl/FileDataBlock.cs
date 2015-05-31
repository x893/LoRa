using System;

namespace SemtechLib.Controls.HexBoxCtrl
{
	internal sealed class FileDataBlock : DataBlock
	{
		private long _length;
		private long _fileOffset;

		public long FileOffset
		{
			get
			{
				return _fileOffset;
			}
		}

		public override long Length
		{
			get
			{
				return _length;
			}
		}

		public FileDataBlock(long fileOffset, long length)
		{
			_fileOffset = fileOffset;
			_length = length;
		}

		public void SetFileOffset(long value)
		{
			_fileOffset = value;
		}

		public void RemoveBytesFromEnd(long count)
		{
			if (count > _length)
				throw new ArgumentOutOfRangeException("count");
			_length -= count;
		}

		public void RemoveBytesFromStart(long count)
		{
			if (count > _length)
				throw new ArgumentOutOfRangeException("count");
			_fileOffset += count;
			_length -= count;
		}

		public override void RemoveBytes(long position, long count)
		{
			if (position > _length)
				throw new ArgumentOutOfRangeException("position");
			if (position + count > _length)
				throw new ArgumentOutOfRangeException("count");
			long num1 = position;
			long num2 = _fileOffset;
			long length = _length - count - num1;
			long fileOffset = _fileOffset + position + count;
			if (num1 > 0L && length > 0L)
			{
				_fileOffset = num2;
				_length = num1;
				_map.AddAfter((DataBlock)this, (DataBlock)new FileDataBlock(fileOffset, length));
			}
			else if (num1 > 0L)
			{
				_fileOffset = num2;
				_length = num1;
			}
			else
			{
				_fileOffset = fileOffset;
				_length = length;
			}
		}
	}
}

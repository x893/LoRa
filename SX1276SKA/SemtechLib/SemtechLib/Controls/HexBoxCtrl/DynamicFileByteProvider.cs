using System;
using System.IO;

namespace SemtechLib.Controls.HexBoxCtrl
{
	public sealed class DynamicFileByteProvider : IByteProvider, IDisposable
	{
		private const int COPY_BLOCK_SIZE = 4096;
		private string _fileName;
		private Stream _stream;
		private DataMap _dataMap;
		private long _totalLength;
		private bool _readOnly;

		public long Length
		{
			get
			{
				return _totalLength;
			}
		}

		public bool ReadOnly
		{
			get
			{
				return _readOnly;
			}
		}

		public event EventHandler LengthChanged;

		public event EventHandler Changed;

		public DynamicFileByteProvider(string fileName)
			: this(fileName, false)
		{
		}

		public DynamicFileByteProvider(string fileName, bool readOnly)
		{
			_fileName = fileName;
			_stream = readOnly ? (Stream)File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite) : (Stream)File.Open(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
			_readOnly = readOnly;
			ReInitialize();
		}

		public DynamicFileByteProvider(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (!stream.CanSeek)
				throw new ArgumentException("stream must supported seek operations(CanSeek)");
			_stream = stream;
			_readOnly = !stream.CanWrite;
			ReInitialize();
		}

		~DynamicFileByteProvider()
		{
			Dispose();
		}

		public byte ReadByte(long index)
		{
			long blockOffset;
			DataBlock dataBlock = GetDataBlock(index, out blockOffset);
			FileDataBlock fileDataBlock = dataBlock as FileDataBlock;
			if (fileDataBlock != null)
				return ReadByteFromFile(fileDataBlock.FileOffset + index - blockOffset);
			return ((MemoryDataBlock)dataBlock).Data[index - blockOffset];
		}

		public void WriteByte(long index, byte value)
		{
			try
			{
				long blockOffset;
				DataBlock dataBlock = GetDataBlock(index, out blockOffset);
				MemoryDataBlock memoryDataBlock1 = dataBlock as MemoryDataBlock;
				if (memoryDataBlock1 != null)
				{
					memoryDataBlock1.Data[index - blockOffset] = value;
				}
				else
				{
					FileDataBlock fileDataBlock1 = (FileDataBlock)dataBlock;
					if (blockOffset == index && dataBlock.PreviousBlock != null)
					{
						MemoryDataBlock memoryDataBlock2 = dataBlock.PreviousBlock as MemoryDataBlock;
						if (memoryDataBlock2 != null)
						{
							memoryDataBlock2.AddByteToEnd(value);
							fileDataBlock1.RemoveBytesFromStart(1L);
							if (fileDataBlock1.Length != 0L)
								return;
							_dataMap.Remove((DataBlock)fileDataBlock1);
							return;
						}
					}
					if (blockOffset + fileDataBlock1.Length - 1L == index && dataBlock.NextBlock != null)
					{
						MemoryDataBlock memoryDataBlock2 = dataBlock.NextBlock as MemoryDataBlock;
						if (memoryDataBlock2 != null)
						{
							memoryDataBlock2.AddByteToStart(value);
							fileDataBlock1.RemoveBytesFromEnd(1L);
							if (fileDataBlock1.Length != 0L)
								return;
							_dataMap.Remove((DataBlock)fileDataBlock1);
							return;
						}
					}
					FileDataBlock fileDataBlock2 = (FileDataBlock)null;
					if (index > blockOffset)
						fileDataBlock2 = new FileDataBlock(fileDataBlock1.FileOffset, index - blockOffset);
					FileDataBlock fileDataBlock3 = (FileDataBlock)null;
					if (index < blockOffset + fileDataBlock1.Length - 1L)
						fileDataBlock3 = new FileDataBlock(fileDataBlock1.FileOffset + index - blockOffset + 1L, fileDataBlock1.Length - (index - blockOffset + 1L));
					DataBlock block = _dataMap.Replace(dataBlock, (DataBlock)new MemoryDataBlock(value));
					if (fileDataBlock2 != null)
						_dataMap.AddBefore(block, (DataBlock)fileDataBlock2);
					if (fileDataBlock3 == null)
						return;
					_dataMap.AddAfter(block, (DataBlock)fileDataBlock3);
				}
			}
			finally
			{
				OnChanged(EventArgs.Empty);
			}
		}

		public void InsertBytes(long index, byte[] bs)
		{
			try
			{
				long blockOffset;
				DataBlock dataBlock = GetDataBlock(index, out blockOffset);
				MemoryDataBlock memoryDataBlock1 = dataBlock as MemoryDataBlock;
				if (memoryDataBlock1 != null)
				{
					memoryDataBlock1.InsertBytes(index - blockOffset, bs);
				}
				else
				{
					FileDataBlock fileDataBlock1 = (FileDataBlock)dataBlock;
					if (blockOffset == index && dataBlock.PreviousBlock != null)
					{
						MemoryDataBlock memoryDataBlock2 = dataBlock.PreviousBlock as MemoryDataBlock;
						if (memoryDataBlock2 != null)
						{
							memoryDataBlock2.InsertBytes(memoryDataBlock2.Length, bs);
							return;
						}
					}
					FileDataBlock fileDataBlock2 = (FileDataBlock)null;
					if (index > blockOffset)
						fileDataBlock2 = new FileDataBlock(fileDataBlock1.FileOffset, index - blockOffset);
					FileDataBlock fileDataBlock3 = (FileDataBlock)null;
					if (index < blockOffset + fileDataBlock1.Length)
						fileDataBlock3 = new FileDataBlock(fileDataBlock1.FileOffset + index - blockOffset, fileDataBlock1.Length - (index - blockOffset));
					DataBlock block = _dataMap.Replace(dataBlock, (DataBlock)new MemoryDataBlock(bs));
					if (fileDataBlock2 != null)
						_dataMap.AddBefore(block, (DataBlock)fileDataBlock2);
					if (fileDataBlock3 == null)
						return;
					_dataMap.AddAfter(block, (DataBlock)fileDataBlock3);
				}
			}
			finally
			{
				_totalLength += (long)bs.Length;
				OnLengthChanged(EventArgs.Empty);
				OnChanged(EventArgs.Empty);
			}
		}

		public void DeleteBytes(long index, long length)
		{
			try
			{
				long num2;
				DataBlock nextBlock;
				long num = length;
				for (DataBlock block = GetDataBlock(index, out num2); (num > 0L) && (block != null); block = (num > 0L) ? nextBlock : null)
				{
					long num3 = block.Length;
					nextBlock = block.NextBlock;
					long count = Math.Min(num, num3 - (index - num2));
					block.RemoveBytes(index - num2, count);
					if (block.Length == 0L)
					{
						_dataMap.Remove(block);
						if (_dataMap.FirstBlock == null)
						{
							_dataMap.AddFirst(new MemoryDataBlock(new byte[0]));
						}
					}
					num -= count;
					num2 += block.Length;
				}
			}
			finally
			{
				_totalLength -= length;
				OnLengthChanged(EventArgs.Empty);
				OnChanged(EventArgs.Empty);
			}
		}

		public bool HasChanges()
		{
			if (_readOnly)
				return false;
			if (_totalLength != _stream.Length)
				return true;
			long num = 0L;
			for (DataBlock dataBlock = _dataMap.FirstBlock; dataBlock != null; dataBlock = dataBlock.NextBlock)
			{
				FileDataBlock fileDataBlock = dataBlock as FileDataBlock;
				if (fileDataBlock == null || fileDataBlock.FileOffset != num)
					return true;
				num += fileDataBlock.Length;
			}
			return num != _stream.Length;
		}

		public void ApplyChanges()
		{
			if (_readOnly)
				throw new OperationCanceledException("File is in read-only mode");
			if (_totalLength > _stream.Length)
				_stream.SetLength(_totalLength);
			long dataOffset = 0L;
			for (DataBlock dataBlock = _dataMap.FirstBlock; dataBlock != null; dataBlock = dataBlock.NextBlock)
			{
				FileDataBlock fileBlock = dataBlock as FileDataBlock;
				if (fileBlock != null && fileBlock.FileOffset != dataOffset)
					MoveFileBlock(fileBlock, dataOffset);
				dataOffset += dataBlock.Length;
			}
			long num = 0L;
			for (DataBlock dataBlock = _dataMap.FirstBlock; dataBlock != null; dataBlock = dataBlock.NextBlock)
			{
				MemoryDataBlock memoryDataBlock = dataBlock as MemoryDataBlock;
				if (memoryDataBlock != null)
				{
					_stream.Position = num;
					int offset = 0;
					while ((long)offset < memoryDataBlock.Length)
					{
						_stream.Write(memoryDataBlock.Data, offset, (int)Math.Min(4096L, memoryDataBlock.Length - (long)offset));
						offset += 4096;
					}
				}
				num += dataBlock.Length;
			}
			_stream.SetLength(_totalLength);
			ReInitialize();
		}

		public bool SupportsWriteByte()
		{
			return !_readOnly;
		}

		public bool SupportsInsertBytes()
		{
			return !_readOnly;
		}

		public bool SupportsDeleteBytes()
		{
			return !_readOnly;
		}

		public void Dispose()
		{
			if (_stream != null)
			{
				_stream.Close();
				_stream = (Stream)null;
			}
			_fileName = (string)null;
			_dataMap = (DataMap)null;
			GC.SuppressFinalize((object)this);
		}

		private void OnLengthChanged(EventArgs e)
		{
			if (LengthChanged == null)
				return;
			LengthChanged((object)this, e);
		}

		private void OnChanged(EventArgs e)
		{
			if (Changed == null)
				return;
			Changed((object)this, e);
		}

		private DataBlock GetDataBlock(long findOffset, out long blockOffset)
		{
			if (findOffset < 0L || findOffset > _totalLength)
				throw new ArgumentOutOfRangeException("findOffset");
			blockOffset = 0L;
			for (DataBlock dataBlock = _dataMap.FirstBlock; dataBlock != null; dataBlock = dataBlock.NextBlock)
			{
				if (blockOffset <= findOffset && blockOffset + dataBlock.Length > findOffset || dataBlock.NextBlock == null)
					return dataBlock;
				blockOffset += dataBlock.Length;
			}
			return (DataBlock)null;
		}

		private FileDataBlock GetNextFileDataBlock(DataBlock block, long dataOffset, out long nextDataOffset)
		{
			nextDataOffset = dataOffset + block.Length;
			for (block = block.NextBlock; block != null; block = block.NextBlock)
			{
				FileDataBlock fileDataBlock = block as FileDataBlock;
				if (fileDataBlock != null)
					return fileDataBlock;
				nextDataOffset += block.Length;
			}
			return (FileDataBlock)null;
		}

		private byte ReadByteFromFile(long fileOffset)
		{
			if (_stream.Position != fileOffset)
				_stream.Position = fileOffset;
			return (byte)_stream.ReadByte();
		}

		private void MoveFileBlock(FileDataBlock fileBlock, long dataOffset)
		{
			long nextDataOffset;
			FileDataBlock nextFileDataBlock = GetNextFileDataBlock((DataBlock)fileBlock, dataOffset, out nextDataOffset);
			if (nextFileDataBlock != null && dataOffset + fileBlock.Length > nextFileDataBlock.FileOffset)
				MoveFileBlock(nextFileDataBlock, nextDataOffset);
			if (fileBlock.FileOffset > dataOffset)
			{
				byte[] buffer = new byte[4096];
				long num1 = 0L;
				while (num1 < fileBlock.Length)
				{
					long num2 = fileBlock.FileOffset + num1;
					int count = (int)Math.Min((long)buffer.Length, fileBlock.Length - num1);
					_stream.Position = num2;
					_stream.Read(buffer, 0, count);
					_stream.Position = dataOffset + num1;
					_stream.Write(buffer, 0, count);
					num1 += (long)buffer.Length;
				}
			}
			else
			{
				byte[] buffer = new byte[4096];
				long num = 0L;
				while (num < fileBlock.Length)
				{
					int count = (int)Math.Min((long)buffer.Length, fileBlock.Length - num);
					_stream.Position = fileBlock.FileOffset + fileBlock.Length - num - (long)count;
					_stream.Read(buffer, 0, count);
					_stream.Position = dataOffset + fileBlock.Length - num - (long)count;
					_stream.Write(buffer, 0, count);
					num += (long)buffer.Length;
				}
			}
			fileBlock.SetFileOffset(dataOffset);
		}

		private void ReInitialize()
		{
			_dataMap = new DataMap();
			_dataMap.AddFirst((DataBlock)new FileDataBlock(0L, _stream.Length));
			_totalLength = _stream.Length;
		}
	}
}

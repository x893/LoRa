using System;
using System.IO;

namespace MyCSLib.Controls.HexBoxCtrl
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
				return this._totalLength;
			}
		}

		public bool ReadOnly
		{
			get
			{
				return this._readOnly;
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
			this._fileName = fileName;
			this._stream = readOnly ? (Stream)File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite) : (Stream)File.Open(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
			this._readOnly = readOnly;
			this.ReInitialize();
		}

		public DynamicFileByteProvider(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (!stream.CanSeek)
				throw new ArgumentException("stream must supported seek operations(CanSeek)");
			this._stream = stream;
			this._readOnly = !stream.CanWrite;
			this.ReInitialize();
		}

		~DynamicFileByteProvider()
		{
			this.Dispose();
		}

		public byte ReadByte(long index)
		{
			long blockOffset;
			DataBlock dataBlock = this.GetDataBlock(index, out blockOffset);
			FileDataBlock fileDataBlock = dataBlock as FileDataBlock;
			if (fileDataBlock != null)
				return this.ReadByteFromFile(fileDataBlock.FileOffset + index - blockOffset);
			return ((MemoryDataBlock)dataBlock).Data[index - blockOffset];
		}

		public void WriteByte(long index, byte value)
		{
			try
			{
				long blockOffset;
				DataBlock dataBlock = this.GetDataBlock(index, out blockOffset);
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
							this._dataMap.Remove((DataBlock)fileDataBlock1);
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
							this._dataMap.Remove((DataBlock)fileDataBlock1);
							return;
						}
					}
					FileDataBlock fileDataBlock2 = (FileDataBlock)null;
					if (index > blockOffset)
						fileDataBlock2 = new FileDataBlock(fileDataBlock1.FileOffset, index - blockOffset);
					FileDataBlock fileDataBlock3 = (FileDataBlock)null;
					if (index < blockOffset + fileDataBlock1.Length - 1L)
						fileDataBlock3 = new FileDataBlock(fileDataBlock1.FileOffset + index - blockOffset + 1L, fileDataBlock1.Length - (index - blockOffset + 1L));
					DataBlock block = this._dataMap.Replace(dataBlock, (DataBlock)new MemoryDataBlock(value));
					if (fileDataBlock2 != null)
						this._dataMap.AddBefore(block, (DataBlock)fileDataBlock2);
					if (fileDataBlock3 != null)
						this._dataMap.AddAfter(block, (DataBlock)fileDataBlock3);
				}
			}
			finally
			{
				this.OnChanged(EventArgs.Empty);
			}
		}

		public void InsertBytes(long index, byte[] bs)
		{
			try
			{
				long blockOffset;
				DataBlock dataBlock = this.GetDataBlock(index, out blockOffset);
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
					DataBlock block = this._dataMap.Replace(dataBlock, (DataBlock)new MemoryDataBlock(bs));
					if (fileDataBlock2 != null)
						this._dataMap.AddBefore(block, (DataBlock)fileDataBlock2);
					if (fileDataBlock3 != null)
						this._dataMap.AddAfter(block, (DataBlock)fileDataBlock3);
				}
			}
			finally
			{
				this._totalLength += (long)bs.Length;
				this.OnLengthChanged(EventArgs.Empty);
				this.OnChanged(EventArgs.Empty);
			}
		}

		public void DeleteBytes(long index, long length)
		{
			try
			{
				long num2;
				DataBlock nextBlock;
				long num = length;
				for (DataBlock block = this.GetDataBlock(index, out num2); (num > 0L) && (block != null); block = (num > 0L) ? nextBlock : null)
				{
					long num3 = block.Length;
					nextBlock = block.NextBlock;
					long count = Math.Min(num, num3 - (index - num2));
					block.RemoveBytes(index - num2, count);
					if (block.Length == 0L)
					{
						this._dataMap.Remove(block);
						if (this._dataMap.FirstBlock == null)
						{
							this._dataMap.AddFirst(new MemoryDataBlock(new byte[0]));
						}
					}
					num -= count;
					num2 += block.Length;
				}
			}
			finally
			{
				this._totalLength -= length;
				this.OnLengthChanged(EventArgs.Empty);
				this.OnChanged(EventArgs.Empty);
			}
		}


		public bool HasChanges()
		{
			if (this._readOnly)
				return false;
			if (this._totalLength != this._stream.Length)
				return true;
			long num = 0L;
			for (DataBlock dataBlock = this._dataMap.FirstBlock; dataBlock != null; dataBlock = dataBlock.NextBlock)
			{
				FileDataBlock fileDataBlock = dataBlock as FileDataBlock;
				if (fileDataBlock == null || fileDataBlock.FileOffset != num)
					return true;
				num += fileDataBlock.Length;
			}
			return num != this._stream.Length;
		}

		public void ApplyChanges()
		{
			if (this._readOnly)
				throw new OperationCanceledException("File is in read-only mode");
			if (this._totalLength > this._stream.Length)
				this._stream.SetLength(this._totalLength);
			long dataOffset = 0L;
			for (DataBlock dataBlock = this._dataMap.FirstBlock; dataBlock != null; dataBlock = dataBlock.NextBlock)
			{
				FileDataBlock fileBlock = dataBlock as FileDataBlock;
				if (fileBlock != null && fileBlock.FileOffset != dataOffset)
					this.MoveFileBlock(fileBlock, dataOffset);
				dataOffset += dataBlock.Length;
			}
			long num = 0L;
			for (DataBlock dataBlock = this._dataMap.FirstBlock; dataBlock != null; dataBlock = dataBlock.NextBlock)
			{
				MemoryDataBlock memoryDataBlock = dataBlock as MemoryDataBlock;
				if (memoryDataBlock != null)
				{
					this._stream.Position = num;
					int offset = 0;
					while ((long)offset < memoryDataBlock.Length)
					{
						this._stream.Write(memoryDataBlock.Data, offset, (int)Math.Min(4096L, memoryDataBlock.Length - (long)offset));
						offset += 4096;
					}
				}
				num += dataBlock.Length;
			}
			this._stream.SetLength(this._totalLength);
			this.ReInitialize();
		}

		public bool SupportsWriteByte()
		{
			return !this._readOnly;
		}

		public bool SupportsInsertBytes()
		{
			return !this._readOnly;
		}

		public bool SupportsDeleteBytes()
		{
			return !this._readOnly;
		}

		public void Dispose()
		{
			if (this._stream != null)
			{
				this._stream.Close();
				this._stream = (Stream)null;
			}
			this._fileName = (string)null;
			this._dataMap = (DataMap)null;
			GC.SuppressFinalize((object)this);
		}

		private void OnLengthChanged(EventArgs e)
		{
			if (this.LengthChanged == null)
				return;
			this.LengthChanged((object)this, e);
		}

		private void OnChanged(EventArgs e)
		{
			if (this.Changed == null)
				return;
			this.Changed((object)this, e);
		}

		private DataBlock GetDataBlock(long findOffset, out long blockOffset)
		{
			if (findOffset < 0L || findOffset > this._totalLength)
				throw new ArgumentOutOfRangeException("findOffset");
			blockOffset = 0L;
			for (DataBlock dataBlock = this._dataMap.FirstBlock; dataBlock != null; dataBlock = dataBlock.NextBlock)
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
			if (this._stream.Position != fileOffset)
				this._stream.Position = fileOffset;
			return (byte)this._stream.ReadByte();
		}

		private void MoveFileBlock(FileDataBlock fileBlock, long dataOffset)
		{
			long nextDataOffset;
			FileDataBlock nextFileDataBlock = this.GetNextFileDataBlock((DataBlock)fileBlock, dataOffset, out nextDataOffset);
			if (nextFileDataBlock != null && dataOffset + fileBlock.Length > nextFileDataBlock.FileOffset)
				this.MoveFileBlock(nextFileDataBlock, nextDataOffset);
			if (fileBlock.FileOffset > dataOffset)
			{
				byte[] buffer = new byte[4096];
				long num1 = 0L;
				while (num1 < fileBlock.Length)
				{
					long num2 = fileBlock.FileOffset + num1;
					int count = (int)Math.Min((long)buffer.Length, fileBlock.Length - num1);
					this._stream.Position = num2;
					this._stream.Read(buffer, 0, count);
					this._stream.Position = dataOffset + num1;
					this._stream.Write(buffer, 0, count);
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
					this._stream.Position = fileBlock.FileOffset + fileBlock.Length - num - (long)count;
					this._stream.Read(buffer, 0, count);
					this._stream.Position = dataOffset + fileBlock.Length - num - (long)count;
					this._stream.Write(buffer, 0, count);
					num += (long)buffer.Length;
				}
			}
			fileBlock.SetFileOffset(dataOffset);
		}

		private void ReInitialize()
		{
			this._dataMap = new DataMap();
			this._dataMap.AddFirst((DataBlock)new FileDataBlock(0L, this._stream.Length));
			this._totalLength = this._stream.Length;
		}
	}
}

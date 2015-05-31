using System;
using System.Collections;
using System.IO;

namespace SemtechLib.Controls.HexBoxCtrl
{
	public class FileByteProvider : IByteProvider, IDisposable
	{
		private FileByteProvider.WriteCollection _writes = new FileByteProvider.WriteCollection();
		private string _fileName;
		private FileStream _fileStream;
		private bool _readOnly;

		public string FileName
		{
			get
			{
				return _fileName;
			}
		}

		public long Length
		{
			get
			{
				return _fileStream.Length;
			}
		}

		public event EventHandler Changed;

		public event EventHandler LengthChanged;

		public FileByteProvider(string fileName)
		{
			_fileName = fileName;
			try
			{
				_fileStream = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
			}
			catch
			{
				try
				{
					_fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
					_readOnly = true;
				}
				catch
				{
					throw;
				}
			}
		}

		~FileByteProvider()
		{
			Dispose();
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

		public bool HasChanges()
		{
			return _writes.Count > 0;
		}

		public void ApplyChanges()
		{
			if (_readOnly)
				throw new Exception("File is in read-only mode.");
			if (!HasChanges())
				return;
			IDictionaryEnumerator enumerator = _writes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				long num1 = (long)enumerator.Key;
				byte num2 = (byte)enumerator.Value;
				if (_fileStream.Position != num1)
					_fileStream.Position = num1;
				_fileStream.Write(new byte[1]
        {
          num2
        }, 0, 1);
			}
			_writes.Clear();
		}

		public void RejectChanges()
		{
			_writes.Clear();
		}

		public byte ReadByte(long index)
		{
			if (_writes.Contains(index))
				return _writes[index];
			if (_fileStream.Position != index)
				_fileStream.Position = index;
			return (byte)_fileStream.ReadByte();
		}

		public void WriteByte(long index, byte value)
		{
			if (_writes.Contains(index))
				_writes[index] = value;
			else
				_writes.Add(index, value);
			OnChanged(EventArgs.Empty);
		}

		public void DeleteBytes(long index, long length)
		{
			throw new NotSupportedException("FileByteProvider.DeleteBytes");
		}

		public void InsertBytes(long index, byte[] bs)
		{
			throw new NotSupportedException("FileByteProvider.InsertBytes");
		}

		public bool SupportsWriteByte()
		{
			return !_readOnly;
		}

		public bool SupportsInsertBytes()
		{
			return false;
		}

		public bool SupportsDeleteBytes()
		{
			return false;
		}

		public void Dispose()
		{
			if (_fileStream != null)
			{
				_fileName = (string)null;
				_fileStream.Close();
				_fileStream = (FileStream)null;
			}
			GC.SuppressFinalize((object)this);
		}

		private class WriteCollection : DictionaryBase
		{
			public byte this[long index]
			{
				get
				{
					return (byte)Dictionary[(object)index];
				}
				set
				{
					Dictionary[(object)index] = (object)value;
				}
			}

			public void Add(long index, byte value)
			{
				Dictionary.Add((object)index, (object)value);
			}

			public bool Contains(long index)
			{
				return Dictionary.Contains((object)index);
			}
		}
	}
}

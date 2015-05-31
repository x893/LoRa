using System;
using System.Collections;
using System.IO;

namespace MyCSLib.Controls.HexBoxCtrl
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
				return this._fileName;
			}
		}

		public long Length
		{
			get
			{
				return this._fileStream.Length;
			}
		}

		public event EventHandler Changed;

		public event EventHandler LengthChanged;

		public FileByteProvider(string fileName)
		{
			this._fileName = fileName;
			try
			{
				this._fileStream = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
			}
			catch
			{
				try
				{
					this._fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
					this._readOnly = true;
				}
				catch
				{
					throw;
				}
			}
		}

		~FileByteProvider()
		{
			this.Dispose();
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

		public bool HasChanges()
		{
			return this._writes.Count > 0;
		}

		public void ApplyChanges()
		{
			if (this._readOnly)
				throw new Exception("File is in read-only mode.");
			if (!this.HasChanges())
				return;
			IDictionaryEnumerator enumerator = this._writes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				long num1 = (long)enumerator.Key;
				byte num2 = (byte)enumerator.Value;
				if (this._fileStream.Position != num1)
					this._fileStream.Position = num1;
				this._fileStream.Write(new byte[1]
        {
          num2
        }, 0, 1);
			}
			this._writes.Clear();
		}

		public void RejectChanges()
		{
			this._writes.Clear();
		}

		public byte ReadByte(long index)
		{
			if (this._writes.Contains(index))
				return this._writes[index];
			if (this._fileStream.Position != index)
				this._fileStream.Position = index;
			return (byte)this._fileStream.ReadByte();
		}

		public void WriteByte(long index, byte value)
		{
			if (this._writes.Contains(index))
				this._writes[index] = value;
			else
				this._writes.Add(index, value);
			this.OnChanged(EventArgs.Empty);
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
			return !this._readOnly;
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
			if (this._fileStream != null)
			{
				this._fileName = (string)null;
				this._fileStream.Close();
				this._fileStream = (FileStream)null;
			}
			GC.SuppressFinalize((object)this);
		}

		private class WriteCollection : DictionaryBase
		{
			public byte this[long index]
			{
				get
				{
					return (byte)this.Dictionary[(object)index];
				}
				set
				{
					this.Dictionary[(object)index] = (object)value;
				}
			}

			public void Add(long index, byte value)
			{
				this.Dictionary.Add((object)index, (object)value);
			}

			public bool Contains(long index)
			{
				return this.Dictionary.Contains((object)index);
			}
		}
	}
}
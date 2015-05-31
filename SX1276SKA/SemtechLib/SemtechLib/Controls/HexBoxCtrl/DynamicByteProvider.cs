using System;

namespace SemtechLib.Controls.HexBoxCtrl
{
	public class DynamicByteProvider : IByteProvider
	{
		private bool _hasChanges;
		private ByteCollection _bytes;

		public ByteCollection Bytes
		{
			get
			{
				return this._bytes;
			}
		}

		public long Length
		{
			get
			{
				return (long)this._bytes.Count;
			}
		}

		public event EventHandler Changed;

		public event EventHandler LengthChanged;

		public DynamicByteProvider(byte[] data)
			: this(new ByteCollection(data))
		{
		}

		public DynamicByteProvider(ByteCollection bytes)
		{
			this._bytes = bytes;
		}

		private void OnChanged(EventArgs e)
		{
			this._hasChanges = true;
			if (this.Changed == null)
				return;
			this.Changed((object)this, e);
		}

		private void OnLengthChanged(EventArgs e)
		{
			if (this.LengthChanged == null)
				return;
			this.LengthChanged((object)this, e);
		}

		public bool HasChanges()
		{
			return this._hasChanges;
		}

		public void ApplyChanges()
		{
			this._hasChanges = false;
		}

		public byte ReadByte(long index)
		{
			return this._bytes[(int)index];
		}

		public void WriteByte(long index, byte value)
		{
			this._bytes[(int)index] = value;
			this.OnChanged(EventArgs.Empty);
		}

		public void DeleteBytes(long index, long length)
		{
			this._bytes.RemoveRange((int)Math.Max(0L, index), (int)Math.Min((long)(int)this.Length, length));
			this.OnLengthChanged(EventArgs.Empty);
			this.OnChanged(EventArgs.Empty);
		}

		public void InsertBytes(long index, byte[] bs)
		{
			this._bytes.InsertRange((int)index, bs);
			this.OnLengthChanged(EventArgs.Empty);
			this.OnChanged(EventArgs.Empty);
		}

		public bool SupportsWriteByte()
		{
			return true;
		}

		public bool SupportsInsertBytes()
		{
			return true;
		}

		public bool SupportsDeleteBytes()
		{
			return true;
		}
	}
}
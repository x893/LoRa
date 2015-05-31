namespace MyCSLib.Controls
{
	public class Command
	{
		private bool _IsHex = true;
		private byte[] _DataBytes = (byte[])null;

		public bool IsHex
		{
			get
			{
				return this._IsHex;
			}
			set
			{
				this._IsHex = value;
			}
		}

		public byte[] DataBytes
		{
			get
			{
				return this._DataBytes;
			}
			set
			{
				this._DataBytes = value;
			}
		}
	}
}

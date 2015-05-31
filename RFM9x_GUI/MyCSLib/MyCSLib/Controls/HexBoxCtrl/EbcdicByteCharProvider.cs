using System.Text;

namespace MyCSLib.Controls.HexBoxCtrl
{
	public class EbcdicByteCharProvider : IByteCharConverter
	{
		private Encoding _ebcdicEncoding = Encoding.GetEncoding(500);

		public char ToChar(byte b)
		{
			string @string = this._ebcdicEncoding.GetString(new byte[1] { b });
			return @string.Length > 0 ? @string[0] : '.';
		}

		public byte ToByte(char c)
		{
			byte[] bytes = this._ebcdicEncoding.GetBytes(new char[1]
      {
        c
      });
			return bytes.Length > 0 ? bytes[0] : (byte)0;
		}

		public override string ToString()
		{
			return "EBCDIC (Code Page 500)";
		}
	}
}

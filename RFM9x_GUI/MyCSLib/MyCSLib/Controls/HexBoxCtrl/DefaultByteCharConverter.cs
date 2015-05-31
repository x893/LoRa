namespace MyCSLib.Controls.HexBoxCtrl
{
	public class DefaultByteCharConverter : IByteCharConverter
	{
		public char ToChar(byte b)
		{
			return (int)b <= 31 || (int)b > 126 && (int)b < 160 ? '.' : (char)b;
		}

		public byte ToByte(char c)
		{
			return (byte)c;
		}

		public override string ToString()
		{
			return "Default";
		}
	}
}
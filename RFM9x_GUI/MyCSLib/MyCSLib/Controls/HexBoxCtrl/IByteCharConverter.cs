namespace MyCSLib.Controls.HexBoxCtrl
{
	public interface IByteCharConverter
	{
		char ToChar(byte b);

		byte ToByte(char c);
	}
}
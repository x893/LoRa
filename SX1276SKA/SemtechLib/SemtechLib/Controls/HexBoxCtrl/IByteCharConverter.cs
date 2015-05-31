namespace SemtechLib.Controls.HexBoxCtrl
{
	public interface IByteCharConverter
	{
		char ToChar(byte b);

		byte ToByte(char c);
	}
}

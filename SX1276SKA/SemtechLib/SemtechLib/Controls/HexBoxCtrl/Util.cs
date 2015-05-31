using System.Diagnostics;

namespace SemtechLib.Controls.HexBoxCtrl
{
	internal static class Util
	{
		private static bool _designMode = Process.GetCurrentProcess().ProcessName.ToLower() == "devenv";

		public static bool DesignMode
		{
			get
			{
				return Util._designMode;
			}
		}
	}
}

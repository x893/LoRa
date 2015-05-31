using System;
using System.Runtime.InteropServices;

namespace MyCSLib.Controls.HexBoxCtrl
{
	internal static class NativeMethods
	{
		public const int WM_KEYDOWN = 256;
		public const int WM_KEYUP = 257;
		public const int WM_CHAR = 258;

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool ShowCaret(IntPtr hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool DestroyCaret();

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool SetCaretPos(int X, int Y);
	}
}

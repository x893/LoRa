using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Fusionbird.FusionToolkit
{
	internal class NativeMethods
	{
		public const int NM_CUSTOMDRAW = -12;
		public const int NM_FIRST = 0;
		public const int S_OK = 0;
		public const int TMT_COLOR = 204;

		private NativeMethods()
		{
		}

		[DllImport("UxTheme.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int CloseThemeData(IntPtr hTheme);

		[DllImport("Comctl32.dll", EntryPoint = "DllGetVersion", CallingConvention = CallingConvention.Cdecl)]
		public static extern int CommonControlsGetVersion(ref NativeMethods.DLLVERSIONINFO pdvi);

		[DllImport("UxTheme.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int DrawThemeBackground(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, ref NativeMethods.RECT pRect, ref NativeMethods.RECT pClipRect);

		[DllImport("UxTheme.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int GetThemeColor(IntPtr hTheme, int iPartId, int iStateId, int iPropId, ref int pColor);

		[DllImport("UxTheme.dll", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsAppThemed();

		[DllImport("UxTheme.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr OpenThemeData(IntPtr hwnd, string pszClassList);

		public enum CustomDrawDrawStage
		{
			CDDS_PREPAINT = 1,
			CDDS_POSTPAINT = 2,
			CDDS_PREERASE = 3,
			CDDS_POSTERASE = 4,
			CDDS_ITEM = 65536,
			CDDS_ITEMPREPAINT = 65537,
			CDDS_ITEMPOSTPAINT = 65538,
			CDDS_ITEMPREERASE = 65539,
			CDDS_ITEMPOSTERASE = 65540,
			CDDS_SUBITEM = 131072,
		}

		public enum CustomDrawItemState
		{
			CDIS_SELECTED = 1,
			CDIS_GRAYED = 2,
			CDIS_DISABLED = 4,
			CDIS_CHECKED = 8,
			CDIS_FOCUS = 16,
			CDIS_DEFAULT = 32,
			CDIS_HOT = 64,
			CDIS_MARKED = 128,
			CDIS_INDETERMINATE = 256,
			CDIS_SHOWKEYBOARDCUES = 512,
		}

		public enum CustomDrawReturnFlags
		{
			CDRF_DODEFAULT = 0,
			CDRF_NEWFONT = 2,
			CDRF_SKIPDEFAULT = 4,
			CDRF_NOTIFYPOSTPAINT = 16,
			CDRF_NOTIFYITEMDRAW = 32,
			CDRF_NOTIFYSUBITEMDRAW = 32,
			CDRF_NOTIFYPOSTERASE = 64,
		}

		public enum TrackBarCustomDrawPart
		{
			TBCD_TICS = 1,
			TBCD_THUMB = 2,
			TBCD_CHANNEL = 3,
		}

		public enum TrackBarParts
		{
			TKP_TRACK = 1,
			TKP_TRACKVERT = 2,
			TKP_THUMB = 3,
			TKP_THUMBBOTTOM = 4,
			TKP_THUMBTOP = 5,
			TKP_THUMBVERT = 6,
			TKP_THUMBLEFT = 7,
			TKP_THUMBRIGHT = 8,
			TKP_TICS = 9,
			TKP_TICSVERT = 10,
		}

		public struct DLLVERSIONINFO
		{
#pragma warning disable 0649
			public int cbSize;
			public int dwMajorVersion;
			public int dwMinorVersion;
			public int dwBuildNumber;
			public int dwPlatformID;
#pragma warning restore 0649
		}

		public struct NMCUSTOMDRAW
		{
#pragma warning disable 0649
			public NativeMethods.NMHDR hdr;
			public NativeMethods.CustomDrawDrawStage dwDrawStage;
			public IntPtr hdc;
			public NativeMethods.RECT rc;
			public IntPtr dwItemSpec;
			public NativeMethods.CustomDrawItemState uItemState;
			public IntPtr lItemlParam;
#pragma warning restore 0649
		}

		public struct NMHDR
		{
#pragma warning disable 0649
			public IntPtr HWND;
			public int idFrom;
			public int code;
#pragma warning restore 0649

			public override string ToString()
			{
				return string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Hwnd: {0}, ControlID: {1}, Code: {2}", (object)this.HWND, (object)this.idFrom, (object)this.code);
			}
		}

		public struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;

			public RECT(Rectangle rect)
			{
				this = new NativeMethods.RECT();
				this.Left = rect.Left;
				this.Top = rect.Top;
				this.Right = rect.Right;
				this.Bottom = rect.Bottom;
			}

			public override string ToString()
			{
				return string.Format((IFormatProvider)CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}", (object)this.Left, (object)this.Top, (object)this.Right, (object)this.Bottom);
			}

			public Rectangle ToRectangle()
			{
				return Rectangle.FromLTRB(this.Left, this.Top, this.Right, this.Bottom);
			}
		}
	}
}

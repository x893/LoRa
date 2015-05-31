using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MyCSLib.General
{
	public class MouseGlobalHook : IDisposable
	{
		private static int hHook = 0;
		private const int WH_MOUSE_LL = 14;
		private const int WH_KEYBOARD_LL = 13;
		private const int WH_MOUSE = 7;
		private const int WH_KEYBOARD = 2;
		private const int WM_MOUSEMOVE = 512;
		private const int WM_LBUTTONDOWN = 513;
		private const int WM_RBUTTONDOWN = 516;
		private const int WM_MBUTTONDOWN = 519;
		private const int WM_LBUTTONUP = 514;
		private const int WM_RBUTTONUP = 517;
		private const int WM_MBUTTONUP = 520;
		private const int WM_LBUTTONDBLCLK = 515;
		private const int WM_RBUTTONDBLCLK = 518;
		private const int WM_MBUTTONDBLCLK = 521;
		private const int WM_MOUSEWHEEL = 522;
		private static int OldX;
		private static int OldY;
		private static MouseGlobalHook.HookProc MouseHookProcedure;

		public event MouseEventHandler MouseMove;

		public MouseGlobalHook()
		{
			if (MouseGlobalHook.hHook == 0)
			{
				MouseGlobalHook.MouseHookProcedure = new MouseGlobalHook.HookProc(this.MouseHookProc);
				MouseGlobalHook.hHook = MouseGlobalHook.SetWindowsHookEx(14, MouseGlobalHook.MouseHookProcedure, Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
				if (MouseGlobalHook.hHook == 0)
				{
					MessageBox.Show("SetWindowsHookEx Failed");
				}
			}
		}

		~MouseGlobalHook()
		{
			this.Dispose();
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern int SetWindowsHookEx(int idHook, MouseGlobalHook.HookProc lpfn, IntPtr hInstance, int threadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern bool UnhookWindowsHookEx(int idHook);

		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

		private int MouseHookProc(int nCode, int wParam, IntPtr lParam)
		{
			if (nCode >= 0)
			{
				MouseGlobalHook.MouseLLHookStruct mouseLlHookStruct = (MouseGlobalHook.MouseLLHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseGlobalHook.MouseLLHookStruct));
				MouseButtons button = MouseButtons.None;
				short num = (short)0;
				int clicks = 0;
				MouseEventArgs e = new MouseEventArgs(button, clicks, mouseLlHookStruct.Point.X, mouseLlHookStruct.Point.Y, (int)num);
				if (this.MouseMove != null && (MouseGlobalHook.OldX != mouseLlHookStruct.Point.X || MouseGlobalHook.OldY != mouseLlHookStruct.Point.Y))
				{
					MouseGlobalHook.OldX = mouseLlHookStruct.Point.X;
					MouseGlobalHook.OldY = mouseLlHookStruct.Point.Y;
					if (this.MouseMove != null)
						this.MouseMove((object)null, e);
				}
			}
			return MouseGlobalHook.CallNextHookEx(MouseGlobalHook.hHook, nCode, wParam, lParam);
		}

		public void Dispose()
		{
			if (MouseGlobalHook.hHook == 0)
				return;
			MouseGlobalHook.UnhookWindowsHookEx(MouseGlobalHook.hHook);
			MouseGlobalHook.hHook = 0;
		}

		public delegate int HookProc(int nCode, int wParam, IntPtr lParam);

		private struct Point
		{
#pragma warning disable 0649
			public int X;
			public int Y;
#pragma warning restore 0649
		}

		[StructLayout(LayoutKind.Sequential)]
		private class MouseLLHookStruct
		{
			public MouseGlobalHook.Point Point;
			public int MouseData;
			public int Flags;
			public int Time;
			public int ExtraInfo;
		}
	}
}

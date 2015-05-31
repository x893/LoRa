using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SemtechLib.Controls
{
	public class MenuStripEx : MenuStrip
	{
		private bool suppressHighlighting = true;
		private bool clickThrough;

		[Category("Extended")]
		[DefaultValue("false")]
		public bool ClickThrough
		{
			get
			{
				return clickThrough;
			}
			set
			{
				clickThrough = value;
			}
		}

		[DefaultValue("true")]
		[Category("Extended")]
		public bool SuppressHighlighting
		{
			get
			{
				return suppressHighlighting;
			}
			set
			{
				suppressHighlighting = value;
			}
		}

		protected override void WndProc(ref Message m)
		{
			if ((long)m.Msg == 512L && suppressHighlighting && !TopLevelControl.ContainsFocus)
				return;
			base.WndProc(ref m);
			if ((long)m.Msg != 33L || !clickThrough || !(m.Result == (IntPtr)2L))
				return;
			m.Result = (IntPtr)1L;
		}
	}
}
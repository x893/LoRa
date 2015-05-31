using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace MyCSLib.Controls
{
	public class ToolStripEx : ToolStrip
	{
		private bool clickThrough = false;
		private bool suppressHighlighting = true;

		[DefaultValue("false")]
		[Category("Extended")]
		public bool ClickThrough
		{
			get
			{
				return this.clickThrough;
			}
			set
			{
				this.clickThrough = value;
			}
		}

		[DefaultValue("true")]
		[Category("Extended")]
		public bool SuppressHighlighting
		{
			get
			{
				return this.suppressHighlighting;
			}
			set
			{
				this.suppressHighlighting = value;
			}
		}

		protected override void WndProc(ref Message m)
		{
			if ((long)m.Msg == 512L && this.suppressHighlighting && !this.TopLevelControl.ContainsFocus)
				return;
			base.WndProc(ref m);
			if ((long)m.Msg != 33L || !this.clickThrough || !(m.Result == (IntPtr)2L))
				return;
			m.Result = (IntPtr)1L;
		}
	}
}

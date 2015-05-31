using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SemtechLib.Controls
{
	[DesignerCategory("code")]
	public class GroupBoxEx : GroupBox
	{
		private bool mouseOver;

		[EditorBrowsable(EditorBrowsableState.Always)]
		[Category("Mouse")]
		[Browsable(true)]
		public new event EventHandler MouseEnter;

		[Browsable(true)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[Category("Mouse")]
		public new event EventHandler MouseLeave;

		public GroupBoxEx()
		{
			MouseEnter += new EventHandler(this.MouseEnterLeave);
			MouseLeave += new EventHandler(this.MouseEnterLeave);
		}

		private void MouseEnterLeave(object sender, EventArgs e)
		{
			bool flag = this.RectangleToScreen(this.ClientRectangle).Contains(Control.MousePosition);
			if (!(this.mouseOver ^ flag))
				return;
			this.mouseOver = flag;
			if (this.mouseOver)
			{
				if (this.MouseEnter == null)
					return;
				this.MouseEnter((object)this, EventArgs.Empty);
			}
			else
			{
				if (this.MouseLeave == null)
					return;
				this.MouseLeave((object)this, EventArgs.Empty);
			}
		}
	}
}

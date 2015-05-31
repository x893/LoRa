using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace SemtechLib.Controls
{
	[DesignerCategory("code")]
	public class NumericUpDownEx : NumericUpDown
	{
		private TextBox tBox;
		private Control udBtn;
		private bool mouseOver;

		[Browsable(true)]
		[Category("Mouse")]
		[EditorBrowsable(EditorBrowsableState.Always)]
		public new event EventHandler MouseEnter;

		[EditorBrowsable(EditorBrowsableState.Always)]
		[Browsable(true)]
		[Category("Mouse")]
		public new event EventHandler MouseLeave;

		public NumericUpDownEx()
		{
			tBox = (TextBox)GetPrivateField("upDownEdit");
			if (tBox == null)
				throw new ArgumentNullException(GetType().FullName + ": Can't find internal TextBox field.");
			udBtn = GetPrivateField("upDownButtons");
			if (udBtn == null)
				throw new ArgumentNullException(GetType().FullName + ": Can't find internal UpDown buttons field.");
			tBox.MouseEnter += new EventHandler(MouseEnterLeave);
			tBox.MouseLeave += new EventHandler(MouseEnterLeave);
			udBtn.MouseEnter += new EventHandler(MouseEnterLeave);
			udBtn.MouseLeave += new EventHandler(MouseEnterLeave);
			base.MouseEnter += new EventHandler(MouseEnterLeave);
			base.MouseLeave += new EventHandler(MouseEnterLeave);
		}

		protected Control GetPrivateField(string name)
		{
			return (Control)GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).GetValue((object)this);
		}

		private void MouseEnterLeave(object sender, EventArgs e)
		{
			bool flag = RectangleToScreen(ClientRectangle).Contains(Control.MousePosition);
			if (!(mouseOver ^ flag))
				return;
			mouseOver = flag;
			if (mouseOver)
			{
				if (MouseEnter == null)
					return;
				MouseEnter((object)this, EventArgs.Empty);
			}
			else
			{
				if (MouseLeave == null)
					return;
				MouseLeave((object)this, EventArgs.Empty);
			}
		}
	}
}

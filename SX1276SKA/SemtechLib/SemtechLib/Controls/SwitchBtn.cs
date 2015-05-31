using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SemtechLib.Controls
{
	public class SwitchBtn : Control
	{
		private ContentAlignment controlAlign = ContentAlignment.MiddleCenter;
		private Size itemSize = new Size();
		private bool _checked;

		[Description("Indicates whether the component is in the checked state")]
		[Category("Appearance")]
		[DefaultValue(false)]
		public bool Checked
		{
			get
			{
				return _checked;
			}
			set
			{
				_checked = value;
				Invalidate();
			}
		}

		[Category("Appearance")]
		[Description("Indicates how the LED should be aligned")]
		[DefaultValue(ContentAlignment.MiddleCenter)]
		public ContentAlignment ControlAlign
		{
			get
			{
				return controlAlign;
			}
			set
			{
				controlAlign = value;
				Invalidate();
			}
		}

		private Point PosFromAlignment
		{
			get
			{
				Point point = new Point();
				switch (controlAlign)
				{
					case ContentAlignment.BottomCenter:
						point.X = (int)((double)Width / 2.0 - (double)itemSize.Width / 2.0);
						point.Y = Height - itemSize.Height;
						return point;
					case ContentAlignment.BottomRight:
						point.X = Width - itemSize.Width;
						point.Y = Height - itemSize.Height;
						return point;
					case ContentAlignment.MiddleRight:
						point.X = Width - itemSize.Width;
						point.Y = (int)((double)Height / 2.0 - (double)itemSize.Height / 2.0);
						return point;
					case ContentAlignment.BottomLeft:
						point.X = 0;
						point.Y = Height - itemSize.Height;
						return point;
					case ContentAlignment.TopLeft:
						point.X = 0;
						point.Y = 0;
						return point;
					case ContentAlignment.TopCenter:
						point.X = (int)((double)Width / 2.0 - (double)itemSize.Width / 2.0);
						point.Y = 0;
						return point;
					case ContentAlignment.TopRight:
						point.X = Width - itemSize.Width;
						point.Y = 0;
						return point;
					case ContentAlignment.MiddleLeft:
						point.X = 0;
						point.Y = (int)((double)Height / 2.0 - (double)itemSize.Height / 2.0);
						return point;
					case ContentAlignment.MiddleCenter:
						point.X = (int)((double)Width / 2.0 - (double)itemSize.Width / 2.0);
						point.Y = (int)((double)Height / 2.0 - (double)itemSize.Height / 2.0);
						return point;
					default:
						point.X = 0;
						point.Y = 0;
						return point;
				}
			}
		}

		protected Size ItemSize
		{
			get
			{
				return itemSize;
			}
			set
			{
				itemSize = value;
				Invalidate();
			}
		}

		public new event PaintEventHandler Paint;

		public SwitchBtn()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			BackColor = Color.Transparent;
			Width = 15;
			Height = 25;
			itemSize.Width = 10;
			itemSize.Height = 23;
			MouseDown += new MouseEventHandler(mouseDown);
			MouseUp += new MouseEventHandler(mouseUp);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (Paint != null)
			{
				Paint((object)this, e);
			}
			else
			{
				base.OnPaint(e);
				if (Enabled)
				{
					e.Graphics.FillRectangle((Brush)new SolidBrush(Color.FromArgb((int)byte.MaxValue, 0, 0)), PosFromAlignment.X, PosFromAlignment.Y, itemSize.Width, itemSize.Height);
					e.Graphics.FillRectangle((Brush)new SolidBrush(Color.FromArgb(150, 150, 150)), PosFromAlignment.X + 2, PosFromAlignment.Y + 5, itemSize.Width - 4, itemSize.Height - 10);
					if (Checked)
						e.Graphics.FillRectangle((Brush)new SolidBrush(Color.FromArgb(0, 0, 0)), PosFromAlignment.X + 3, PosFromAlignment.Y + 6, itemSize.Width - 6, itemSize.Height - 16);
					else
						e.Graphics.FillRectangle((Brush)new SolidBrush(Color.FromArgb(0, 0, 0)), PosFromAlignment.X + 3, PosFromAlignment.Y + 10, itemSize.Width - 6, itemSize.Height - 16);
				}
				else
				{
					e.Graphics.FillRectangle((Brush)new SolidBrush(Color.FromArgb(200, 120, 120)), PosFromAlignment.X, PosFromAlignment.Y, itemSize.Width, itemSize.Height);
					e.Graphics.FillRectangle((Brush)new SolidBrush(Color.FromArgb(150, 150, 150)), PosFromAlignment.X + 2, PosFromAlignment.Y + 5, itemSize.Width - 4, itemSize.Height - 10);
					if (Checked)
						e.Graphics.FillRectangle((Brush)new SolidBrush(Color.FromArgb(100, 100, 100)), PosFromAlignment.X + 3, PosFromAlignment.Y + 6, itemSize.Width - 6, itemSize.Height - 16);
					else
						e.Graphics.FillRectangle((Brush)new SolidBrush(Color.FromArgb(100, 100, 100)), PosFromAlignment.X + 3, PosFromAlignment.Y + 10, itemSize.Width - 6, itemSize.Height - 16);
				}
			}
		}

		protected void mouseDown(object sender, MouseEventArgs e)
		{
			buttonDown();
		}

		protected void mouseUp(object sender, MouseEventArgs e)
		{
			buttonUp();
		}

		protected void buttonDown()
		{
			Invalidate();
		}

		protected void buttonUp()
		{
			Checked = !Checked;
			Invalidate();
		}
	}
}

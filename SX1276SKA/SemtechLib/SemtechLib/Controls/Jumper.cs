using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SemtechLib.Controls
{
	public class Jumper : Control
	{
		private ContentAlignment jumperAlign = ContentAlignment.MiddleCenter;
		private Size itemSize = new Size();
		private bool _checked;

		[DefaultValue(false)]
		[Category("Appearance")]
		[Description("Indicates whether the component is in the checked state")]
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

		[Description("Indicates how the Jumper should be aligned")]
		[Category("Appearance")]
		[DefaultValue(ContentAlignment.MiddleCenter)]
		public ContentAlignment JumperAlign
		{
			get
			{
				return jumperAlign;
			}
			set
			{
				jumperAlign = value;
				Invalidate();
			}
		}

		private Point PosFromAlignment
		{
			get
			{
				Point point = new Point();
				switch (jumperAlign)
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

		public new event PaintEventHandler Paint;

		public Jumper()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			BackColor = Color.Transparent;
			Size = new Size(19, 35);
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
				itemSize.Width = Size.Width * 66 / 100;
				itemSize.Height = Size.Height * 92 / 100;
				if (Enabled)
				{
					Size size = new Size(itemSize.Width * 40 / 100, itemSize.Width * 40 / 100);
					e.Graphics.FillRectangle((Brush)new SolidBrush(Color.FromArgb(150, 150, 150)), PosFromAlignment.X, PosFromAlignment.Y, itemSize.Width, itemSize.Height);
					e.Graphics.FillRectangle((Brush)new SolidBrush(Color.FromArgb(0, 0, 0)), PosFromAlignment.X + itemSize.Width / 2 - size.Width / 2, PosFromAlignment.Y + itemSize.Height / 4 - size.Height / 2, size.Width, size.Height);
					e.Graphics.FillRectangle((Brush)new SolidBrush(Color.FromArgb(0, 0, 0)), PosFromAlignment.X + itemSize.Width / 2 - size.Width / 2, PosFromAlignment.Y + itemSize.Height / 2 - size.Height / 2, size.Width, size.Height);
					e.Graphics.FillRectangle((Brush)new SolidBrush(Color.FromArgb(0, 0, 0)), PosFromAlignment.X + itemSize.Width / 2 - size.Width / 2, PosFromAlignment.Y + 3 * (itemSize.Height / 4) - size.Height / 2, size.Width, size.Height);
					if (Checked)
						e.Graphics.FillRectangle((Brush)new SolidBrush(ForeColor), PosFromAlignment.X, PosFromAlignment.Y, itemSize.Width, 3 * (itemSize.Height / 5));
					else
						e.Graphics.FillRectangle((Brush)new SolidBrush(ForeColor), PosFromAlignment.X, PosFromAlignment.Y + 2 * (itemSize.Height / 5), itemSize.Width, 3 * (itemSize.Height / 5));
				}
				else
				{
					Size size = new Size(itemSize.Width * 40 / 100, itemSize.Width * 40 / 100);
					e.Graphics.FillRectangle((Brush)new SolidBrush(SystemColors.InactiveCaption), PosFromAlignment.X, PosFromAlignment.Y, itemSize.Width, itemSize.Height);
					e.Graphics.FillRectangle((Brush)new SolidBrush(SystemColors.InactiveBorder), PosFromAlignment.X + itemSize.Width / 2 - size.Width / 2, PosFromAlignment.Y + itemSize.Height / 4 - size.Height / 2, size.Width, size.Height);
					e.Graphics.FillRectangle((Brush)new SolidBrush(SystemColors.InactiveBorder), PosFromAlignment.X + itemSize.Width / 2 - size.Width / 2, PosFromAlignment.Y + itemSize.Height / 2 - size.Height / 2, size.Width, size.Height);
					e.Graphics.FillRectangle((Brush)new SolidBrush(SystemColors.InactiveBorder), PosFromAlignment.X + itemSize.Width / 2 - size.Width / 2, PosFromAlignment.Y + 3 * (itemSize.Height / 4) - size.Height / 2, size.Width, size.Height);
				}
			}
		}
	}
}
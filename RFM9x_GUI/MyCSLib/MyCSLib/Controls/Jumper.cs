using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MyCSLib.Controls
{
	public class Jumper : Control
	{
		private bool _checked = false;
		private ContentAlignment jumperAlign = ContentAlignment.MiddleCenter;
		private Size itemSize = new Size();

		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("Indicates whether the component is in the checked state")]
		public bool Checked
		{
			get
			{
				return this._checked;
			}
			set
			{
				this._checked = value;
				this.Invalidate();
			}
		}

		[Category("Appearance")]
		[Description("Indicates how the Jumper should be aligned")]
		[DefaultValue(ContentAlignment.MiddleCenter)]
		public ContentAlignment JumperAlign
		{
			get
			{
				return this.jumperAlign;
			}
			set
			{
				this.jumperAlign = value;
				this.Invalidate();
			}
		}

		private Point PosFromAlignment
		{
			get
			{
				Point point = new Point();
				switch (this.jumperAlign)
				{
					case ContentAlignment.BottomCenter:
						point.X = (int)((double)this.Width / 2.0 - (double)this.itemSize.Width / 2.0);
						point.Y = this.Height - this.itemSize.Height;
						return point;
					case ContentAlignment.BottomRight:
						point.X = this.Width - this.itemSize.Width;
						point.Y = this.Height - this.itemSize.Height;
						return point;
					case ContentAlignment.MiddleRight:
						point.X = this.Width - this.itemSize.Width;
						point.Y = (int)((double)this.Height / 2.0 - (double)this.itemSize.Height / 2.0);
						return point;
					case ContentAlignment.BottomLeft:
						point.X = 0;
						point.Y = this.Height - this.itemSize.Height;
						return point;
					case ContentAlignment.TopLeft:
						point.X = 0;
						point.Y = 0;
						return point;
					case ContentAlignment.TopCenter:
						point.X = (int)((double)this.Width / 2.0 - (double)this.itemSize.Width / 2.0);
						point.Y = 0;
						return point;
					case ContentAlignment.TopRight:
						point.X = this.Width - this.itemSize.Width;
						point.Y = 0;
						return point;
					case ContentAlignment.MiddleLeft:
						point.X = 0;
						point.Y = (int)((double)this.Height / 2.0 - (double)this.itemSize.Height / 2.0);
						return point;
					case ContentAlignment.MiddleCenter:
						point.X = (int)((double)this.Width / 2.0 - (double)this.itemSize.Width / 2.0);
						point.Y = (int)((double)this.Height / 2.0 - (double)this.itemSize.Height / 2.0);
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
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.BackColor = Color.Transparent;
			this.Size = new Size(19, 35);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (this.Paint != null)
			{
				this.Paint((object)this, e);
			}
			else
			{
				base.OnPaint(e);
				this.itemSize.Width = this.Size.Width * 66 / 100;
				this.itemSize.Height = this.Size.Height * 92 / 100;
				if (this.Enabled)
				{
					Size size = new Size(this.itemSize.Width * 40 / 100, this.itemSize.Width * 40 / 100);
					Graphics graphics1 = e.Graphics;
					SolidBrush solidBrush1 = new SolidBrush(Color.FromArgb(150, 150, 150));
					Point posFromAlignment = this.PosFromAlignment;
					int x1 = posFromAlignment.X;
					posFromAlignment = this.PosFromAlignment;
					int y1 = posFromAlignment.Y;
					int width1 = this.itemSize.Width;
					int height1 = this.itemSize.Height;
					graphics1.FillRectangle((Brush)solidBrush1, x1, y1, width1, height1);
					Graphics graphics2 = e.Graphics;
					SolidBrush solidBrush2 = new SolidBrush(Color.FromArgb(0, 0, 0));
					posFromAlignment = this.PosFromAlignment;
					int x2 = posFromAlignment.X + this.itemSize.Width / 2 - size.Width / 2;
					posFromAlignment = this.PosFromAlignment;
					int y2 = posFromAlignment.Y + this.itemSize.Height / 4 - size.Height / 2;
					int width2 = size.Width;
					int height2 = size.Height;
					graphics2.FillRectangle((Brush)solidBrush2, x2, y2, width2, height2);
					Graphics graphics3 = e.Graphics;
					SolidBrush solidBrush3 = new SolidBrush(Color.FromArgb(0, 0, 0));
					posFromAlignment = this.PosFromAlignment;
					int x3 = posFromAlignment.X + this.itemSize.Width / 2 - size.Width / 2;
					posFromAlignment = this.PosFromAlignment;
					int y3 = posFromAlignment.Y + this.itemSize.Height / 2 - size.Height / 2;
					int width3 = size.Width;
					int height3 = size.Height;
					graphics3.FillRectangle((Brush)solidBrush3, x3, y3, width3, height3);
					Graphics graphics4 = e.Graphics;
					SolidBrush solidBrush4 = new SolidBrush(Color.FromArgb(0, 0, 0));
					posFromAlignment = this.PosFromAlignment;
					int x4 = posFromAlignment.X + this.itemSize.Width / 2 - size.Width / 2;
					posFromAlignment = this.PosFromAlignment;
					int y4 = posFromAlignment.Y + 3 * (this.itemSize.Height / 4) - size.Height / 2;
					int width4 = size.Width;
					int height4 = size.Height;
					graphics4.FillRectangle((Brush)solidBrush4, x4, y4, width4, height4);
					if (this.Checked)
					{
						Graphics graphics5 = e.Graphics;
						SolidBrush solidBrush5 = new SolidBrush(this.ForeColor);
						posFromAlignment = this.PosFromAlignment;
						int x5 = posFromAlignment.X;
						posFromAlignment = this.PosFromAlignment;
						int y5 = posFromAlignment.Y;
						int width5 = this.itemSize.Width;
						int height5 = 3 * (this.itemSize.Height / 5);
						graphics5.FillRectangle((Brush)solidBrush5, x5, y5, width5, height5);
					}
					else
					{
						Graphics graphics5 = e.Graphics;
						SolidBrush solidBrush5 = new SolidBrush(this.ForeColor);
						posFromAlignment = this.PosFromAlignment;
						int x5 = posFromAlignment.X;
						posFromAlignment = this.PosFromAlignment;
						int y5 = posFromAlignment.Y + 2 * (this.itemSize.Height / 5);
						int width5 = this.itemSize.Width;
						int height5 = 3 * (this.itemSize.Height / 5);
						graphics5.FillRectangle((Brush)solidBrush5, x5, y5, width5, height5);
					}
				}
				else
				{
					Size size = new Size(this.itemSize.Width * 40 / 100, this.itemSize.Width * 40 / 100);
					e.Graphics.FillRectangle((Brush)new SolidBrush(SystemColors.InactiveCaption), this.PosFromAlignment.X, this.PosFromAlignment.Y, this.itemSize.Width, this.itemSize.Height);
					Graphics graphics1 = e.Graphics;
					SolidBrush solidBrush1 = new SolidBrush(SystemColors.InactiveBorder);
					Point posFromAlignment = this.PosFromAlignment;
					int x1 = posFromAlignment.X + this.itemSize.Width / 2 - size.Width / 2;
					posFromAlignment = this.PosFromAlignment;
					int y1 = posFromAlignment.Y + this.itemSize.Height / 4 - size.Height / 2;
					int width1 = size.Width;
					int height1 = size.Height;
					graphics1.FillRectangle((Brush)solidBrush1, x1, y1, width1, height1);
					Graphics graphics2 = e.Graphics;
					SolidBrush solidBrush2 = new SolidBrush(SystemColors.InactiveBorder);
					posFromAlignment = this.PosFromAlignment;
					int x2 = posFromAlignment.X + this.itemSize.Width / 2 - size.Width / 2;
					posFromAlignment = this.PosFromAlignment;
					int y2 = posFromAlignment.Y + this.itemSize.Height / 2 - size.Height / 2;
					int width2 = size.Width;
					int height2 = size.Height;
					graphics2.FillRectangle((Brush)solidBrush2, x2, y2, width2, height2);
					Graphics graphics3 = e.Graphics;
					SolidBrush solidBrush3 = new SolidBrush(SystemColors.InactiveBorder);
					posFromAlignment = this.PosFromAlignment;
					int x3 = posFromAlignment.X + this.itemSize.Width / 2 - size.Width / 2;
					posFromAlignment = this.PosFromAlignment;
					int y3 = posFromAlignment.Y + 3 * (this.itemSize.Height / 4) - size.Height / 2;
					int width3 = size.Width;
					int height3 = size.Height;
					graphics3.FillRectangle((Brush)solidBrush3, x3, y3, width3, height3);
				}
			}
		}
	}
}
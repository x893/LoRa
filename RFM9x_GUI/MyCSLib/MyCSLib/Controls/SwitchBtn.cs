using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MyCSLib.Controls
{
	public class SwitchBtn : Control
	{
		private bool _checked = false;
		private ContentAlignment controlAlign = ContentAlignment.MiddleCenter;
		private Size itemSize = new Size();

		[Category("Appearance")]
		[Description("Indicates whether the component is in the checked state")]
		[DefaultValue(false)]
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
		[DefaultValue(ContentAlignment.MiddleCenter)]
		[Description("Indicates how the LED should be aligned")]
		public ContentAlignment ControlAlign
		{
			get
			{
				return this.controlAlign;
			}
			set
			{
				this.controlAlign = value;
				this.Invalidate();
			}
		}

		private Point PosFromAlignment
		{
			get
			{
				Point point = new Point();
				switch (this.controlAlign)
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

		protected Size ItemSize
		{
			get
			{
				return this.itemSize;
			}
			set
			{
				this.itemSize = value;
				this.Invalidate();
			}
		}

		public new event PaintEventHandler Paint;

		public SwitchBtn()
		{
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.BackColor = Color.Transparent;
			this.Width = 15;
			this.Height = 25;
			this.itemSize.Width = 10;
			this.itemSize.Height = 23;
			this.MouseDown += new MouseEventHandler(this.mouseDown);
			this.MouseUp += new MouseEventHandler(this.mouseUp);
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
				if (this.Enabled)
				{
					Graphics graphics1 = e.Graphics;
					SolidBrush solidBrush1 = new SolidBrush(Color.FromArgb((int)byte.MaxValue, 0, 0));
					int x1 = this.PosFromAlignment.X;
					Point posFromAlignment = this.PosFromAlignment;
					int y1 = posFromAlignment.Y;
					int width1 = this.itemSize.Width;
					int height1 = this.itemSize.Height;
					graphics1.FillRectangle((Brush)solidBrush1, x1, y1, width1, height1);
					Graphics graphics2 = e.Graphics;
					SolidBrush solidBrush2 = new SolidBrush(Color.FromArgb(150, 150, 150));
					posFromAlignment = this.PosFromAlignment;
					int x2 = posFromAlignment.X + 2;
					posFromAlignment = this.PosFromAlignment;
					int y2 = posFromAlignment.Y + 5;
					int width2 = this.itemSize.Width - 4;
					int height2 = this.itemSize.Height - 10;
					graphics2.FillRectangle((Brush)solidBrush2, x2, y2, width2, height2);
					if (this.Checked)
					{
						Graphics graphics3 = e.Graphics;
						SolidBrush solidBrush3 = new SolidBrush(Color.FromArgb(0, 0, 0));
						posFromAlignment = this.PosFromAlignment;
						int x3 = posFromAlignment.X + 3;
						posFromAlignment = this.PosFromAlignment;
						int y3 = posFromAlignment.Y + 6;
						int width3 = this.itemSize.Width - 6;
						int height3 = this.itemSize.Height - 16;
						graphics3.FillRectangle((Brush)solidBrush3, x3, y3, width3, height3);
					}
					else
					{
						Graphics graphics3 = e.Graphics;
						SolidBrush solidBrush3 = new SolidBrush(Color.FromArgb(0, 0, 0));
						posFromAlignment = this.PosFromAlignment;
						int x3 = posFromAlignment.X + 3;
						posFromAlignment = this.PosFromAlignment;
						int y3 = posFromAlignment.Y + 10;
						int width3 = this.itemSize.Width - 6;
						int height3 = this.itemSize.Height - 16;
						graphics3.FillRectangle((Brush)solidBrush3, x3, y3, width3, height3);
					}
				}
				else
				{
					Graphics graphics1 = e.Graphics;
					SolidBrush solidBrush1 = new SolidBrush(Color.FromArgb(200, 120, 120));
					int x1 = this.PosFromAlignment.X;
					Point posFromAlignment = this.PosFromAlignment;
					int y1 = posFromAlignment.Y;
					int width1 = this.itemSize.Width;
					int height1 = this.itemSize.Height;
					graphics1.FillRectangle((Brush)solidBrush1, x1, y1, width1, height1);
					Graphics graphics2 = e.Graphics;
					SolidBrush solidBrush2 = new SolidBrush(Color.FromArgb(150, 150, 150));
					posFromAlignment = this.PosFromAlignment;
					int x2 = posFromAlignment.X + 2;
					posFromAlignment = this.PosFromAlignment;
					int y2 = posFromAlignment.Y + 5;
					int width2 = this.itemSize.Width - 4;
					int height2 = this.itemSize.Height - 10;
					graphics2.FillRectangle((Brush)solidBrush2, x2, y2, width2, height2);
					if (this.Checked)
					{
						Graphics graphics3 = e.Graphics;
						SolidBrush solidBrush3 = new SolidBrush(Color.FromArgb(100, 100, 100));
						posFromAlignment = this.PosFromAlignment;
						int x3 = posFromAlignment.X + 3;
						posFromAlignment = this.PosFromAlignment;
						int y3 = posFromAlignment.Y + 6;
						int width3 = this.itemSize.Width - 6;
						int height3 = this.itemSize.Height - 16;
						graphics3.FillRectangle((Brush)solidBrush3, x3, y3, width3, height3);
					}
					else
					{
						Graphics graphics3 = e.Graphics;
						SolidBrush solidBrush3 = new SolidBrush(Color.FromArgb(100, 100, 100));
						posFromAlignment = this.PosFromAlignment;
						int x3 = posFromAlignment.X + 3;
						posFromAlignment = this.PosFromAlignment;
						int y3 = posFromAlignment.Y + 10;
						int width3 = this.itemSize.Width - 6;
						int height3 = this.itemSize.Height - 16;
						graphics3.FillRectangle((Brush)solidBrush3, x3, y3, width3, height3);
					}
				}
			}
		}

		protected void mouseDown(object sender, MouseEventArgs e)
		{
			this.buttonDown();
		}

		protected void mouseUp(object sender, MouseEventArgs e)
		{
			this.buttonUp();
		}

		protected void buttonDown()
		{
			this.Invalidate();
		}

		protected void buttonUp()
		{
			this.Checked = !this.Checked;
			this.Invalidate();
		}
	}
}

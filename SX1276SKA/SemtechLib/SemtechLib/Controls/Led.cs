using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SemtechLib.Controls
{
	public class Led : Control
	{
		private Color ledColor = Color.Green;
		private ContentAlignment ledAlign = ContentAlignment.MiddleCenter;
		private Size itemSize = new Size(11, 11);
		private bool _checked;

		[Category("Appearance")]
		[DefaultValue(false)]
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

		[Category("Appearance")]
		[Description("Indicates the color of the LED")]
		public Color LedColor
		{
			get
			{
				return ledColor;
			}
			set
			{
				ledColor = value;
				Invalidate();
			}
		}

		[DefaultValue(ContentAlignment.MiddleCenter)]
		[Category("Appearance")]
		[Description("Indicates how the LED should be aligned")]
		public ContentAlignment LedAlign
		{
			get
			{
				return ledAlign;
			}
			set
			{
				ledAlign = value;
				Invalidate();
			}
		}

		[Category("Layout")]
		[Description("Sets the size of the led")]
		public Size LedSize
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

		private Point PosFromAlignment
		{
			get
			{
				Point point = new Point();
				switch (ledAlign)
				{
					case ContentAlignment.BottomCenter:
						point.X = (int)((double)Width / 2.0 - (double)itemSize.Width / 2.0);
						point.Y = Height - itemSize.Height - 1;
						return point;
					case ContentAlignment.BottomRight:
						point.X = Width - itemSize.Width - 1;
						point.Y = Height - itemSize.Height - 1;
						return point;
					case ContentAlignment.MiddleRight:
						point.X = Width - itemSize.Width - 1;
						point.Y = (int)((double)Height / 2.0 - (double)itemSize.Height / 2.0);
						return point;
					case ContentAlignment.BottomLeft:
						point.X = 0;
						point.Y = Height - itemSize.Height - 1;
						return point;
					case ContentAlignment.TopLeft:
						point.X = 1;
						point.Y = 1;
						return point;
					case ContentAlignment.TopCenter:
						point.X = (int)((double)Width / 2.0 - (double)itemSize.Width / 2.0);
						point.Y = 1;
						return point;
					case ContentAlignment.TopRight:
						point.X = Width - itemSize.Width - 1;
						point.Y = 1;
						return point;
					case ContentAlignment.MiddleLeft:
						point.X = 1;
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

		public Led()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			BackColor = Color.Transparent;
			Size = new Size(15, 15);
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
				float angle = (float)(50.0 - 15.0 * (1.0 - (double)Width / (double)Height));
				Rectangle rect1 = new Rectangle(PosFromAlignment.X, PosFromAlignment.Y, itemSize.Width, itemSize.Height);
				LinearGradientBrush linearGradientBrush1 = new LinearGradientBrush(rect1, ControlPaint.Dark(Parent.BackColor), ControlPaint.LightLight(Parent.BackColor), angle);
				linearGradientBrush1.Blend = new Blend()
				{
					Positions = new float[6]
          {
            0.0f,
            0.2f,
            0.4f,
            0.6f,
            0.8f,
            1f
          },
					Factors = new float[6]
          {
            0.2f,
            0.2f,
            0.4f,
            0.4f,
            1f,
            1f
          }
				};
				Rectangle rect2 = rect1;
				rect2.Inflate(1, 1);
				e.Graphics.FillEllipse((Brush)linearGradientBrush1, rect2);
				if (Enabled)
				{
					if (Checked)
						e.Graphics.FillEllipse((Brush)new SolidBrush(ControlPaint.Light(ledColor)), PosFromAlignment.X, PosFromAlignment.Y, itemSize.Width, itemSize.Height);
					else
						e.Graphics.FillEllipse((Brush)new SolidBrush(ControlPaint.Dark(ledColor)), PosFromAlignment.X, PosFromAlignment.Y, itemSize.Width, itemSize.Height);
				}
				LinearGradientBrush linearGradientBrush2 = new LinearGradientBrush(rect1, Color.FromArgb(150, (int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue), Color.Transparent, angle);
				LinearGradientBrush linearGradientBrush3 = new LinearGradientBrush(rect1, Color.FromArgb(100, (int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue), Color.FromArgb(100, (int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue), angle);
				Blend blend = new Blend();
				blend.Positions = new float[6]
        {
          0.0f,
          0.2f,
          0.4f,
          0.6f,
          0.8f,
          1f
        };
				blend.Factors = new float[6]
        {
          0.2f,
          0.2f,
          0.4f,
          0.4f,
          1f,
          1f
        };
				linearGradientBrush2.Blend = blend;
				linearGradientBrush3.Blend = blend;
				e.Graphics.FillEllipse((Brush)linearGradientBrush3, PosFromAlignment.X + itemSize.Width * 13 / 100, PosFromAlignment.Y + itemSize.Height * 13 / 100, itemSize.Width * 40 / 100, itemSize.Height * 40 / 100);
				e.Graphics.FillEllipse((Brush)linearGradientBrush2, new Rectangle(PosFromAlignment, itemSize));
			}
		}
	}
}
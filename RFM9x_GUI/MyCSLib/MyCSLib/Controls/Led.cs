using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MyCSLib.Controls
{
	public class Led : Control
	{
		private bool _checked = false;
		private Color ledColor = Color.Green;
		private ContentAlignment ledAlign = ContentAlignment.MiddleCenter;
		private Size itemSize = new Size(11, 11);

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

		[Description("Indicates the color of the LED")]
		[Category("Appearance")]
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
				Paint(this, e);
			}
			else
			{
				base.OnPaint(e);
				float num = 1f - (((float)base.Width) / ((float)base.Height));
				float angle = 50f - (15f * num);
				float num3 = 230f - (15f * num);
				Rectangle rect = new Rectangle(PosFromAlignment.X, PosFromAlignment.Y, itemSize.Width, itemSize.Height);
				LinearGradientBrush brush = new LinearGradientBrush(rect, ControlPaint.Dark(base.Parent.BackColor), ControlPaint.LightLight(base.Parent.BackColor), angle);
				Blend blend = new Blend
				{
					Positions = new float[] { 0f, 0.2f, 0.4f, 0.6f, 0.8f, 1f },
					Factors = new float[] { 0.2f, 0.2f, 0.4f, 0.4f, 1f, 1f }
				};
				brush.Blend = blend;
				Rectangle rectangle2 = rect;
				rectangle2.Inflate(1, 1);
				e.Graphics.FillEllipse(brush, rectangle2);
				if (base.Enabled)
				{
					if (Checked)
					{
						e.Graphics.FillEllipse(new SolidBrush(ControlPaint.Light(ledColor)), PosFromAlignment.X, PosFromAlignment.Y, itemSize.Width, itemSize.Height);
					}
					else
					{
						e.Graphics.FillEllipse(new SolidBrush(ControlPaint.Dark(ledColor)), PosFromAlignment.X, PosFromAlignment.Y, itemSize.Width, itemSize.Height);
					}
				}
				LinearGradientBrush brush2 = new LinearGradientBrush(rect, Color.FromArgb(150, 0xff, 0xff, 0xff), Color.Transparent, angle);
				LinearGradientBrush brush3 = new LinearGradientBrush(rect, Color.FromArgb(100, 0xff, 0xff, 0xff), Color.FromArgb(100, 0xff, 0xff, 0xff), angle);
				Blend blend2 = new Blend
				{
					Positions = new float[] { 0f, 0.2f, 0.4f, 0.6f, 0.8f, 1f },
					Factors = new float[] { 0.2f, 0.2f, 0.4f, 0.4f, 1f, 1f }
				};
				brush2.Blend = blend2;
				brush3.Blend = blend2;
				e.Graphics.FillEllipse(brush3, (int)(PosFromAlignment.X + ((itemSize.Width * 13) / 100)), (int)(PosFromAlignment.Y + ((itemSize.Height * 13) / 100)), (int)((itemSize.Width * 40) / 100), (int)((itemSize.Height * 40) / 100));
				e.Graphics.FillEllipse(brush2, new Rectangle(PosFromAlignment, itemSize));
			}
		}
	}
}

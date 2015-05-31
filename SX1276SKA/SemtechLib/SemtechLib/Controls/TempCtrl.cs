using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SemtechLib.Controls
{
	public class TempCtrl : Control
	{
		private TempCtrl.Ranges range = new TempCtrl.Ranges();
		private double value = 25.0;
		private bool drawTics = true;
		private int smallTicFreq = 5;
		private int largeTicFreq = 10;
		private Font fntText = new Font("Arial", 10f, FontStyle.Bold);
		private StringFormat strfmtText = new StringFormat();
		private bool enableTransparentBackground;
		private bool requiresRedraw;
		private Image backgroundImg;
		private RectangleF rectBackgroundImg;
		private Color colorFore;
		private Color colorBack;
		private Color colorScale;
		private Color colorScaleText;
		private Color colorOutline;
		private Color colorBackground;
		private Pen forePen;
		private Pen scalePen;
		private Pen outlinePen;
		private SolidBrush blackBrush;
		private SolidBrush fillBrush;
		private LinearGradientBrush bulbBrush;
		private RectangleF rectCylinder;
		private RectangleF rectBulb;
		private PointF pointCenter;
		private float fTmpWidth;
		private float fRange;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TempCtrl.Ranges Range
		{
			get
			{
				return range;
			}
			set
			{
				range = value;
				requiresRedraw = true;
				Invalidate();
			}
		}

		public double Value
		{
			get
			{
				return value;
			}
			set
			{
				if (value > range.Max)
					value = range.Max;
				if (value < range.Min)
					value = range.Min;
				this.value = value;
				Invalidate();
			}
		}

		public bool DrawTics
		{
			get
			{
				return drawTics;
			}
			set
			{
				drawTics = value;
				requiresRedraw = true;
				Invalidate();
			}
		}

		public int SmallTicFreq
		{
			get
			{
				return smallTicFreq;
			}
			set
			{
				smallTicFreq = value;
				requiresRedraw = true;
				Invalidate();
			}
		}

		public int LargeTicFreq
		{
			get
			{
				return largeTicFreq;
			}
			set
			{
				largeTicFreq = value;
				requiresRedraw = true;
				Invalidate();
			}
		}

		[Description("Enables or Disables Transparent Background color. Note: Enabling this will reduce the performance and may make the control flicker.")]
		[DefaultValue(false)]
		public bool EnableTransparentBackground
		{
			get
			{
				return enableTransparentBackground;
			}
			set
			{
				enableTransparentBackground = value;
				SetStyle(ControlStyles.OptimizedDoubleBuffer, !enableTransparentBackground);
				requiresRedraw = true;
				Refresh();
			}
		}

		public new event PaintEventHandler Paint;

		public TempCtrl()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			Size = new Size(75, 253);
			ForeColor = Color.Red;
			colorFore = ForeColor;
			colorBack = SystemColors.Control;
			colorScale = Color.FromArgb(0, 0, 0);
			colorScaleText = Color.FromArgb(0, 0, 0);
			colorOutline = Color.FromArgb(64, 0, 0);
			colorBackground = Color.FromKnownColor(KnownColor.Transparent);
			EnabledChanged += new EventHandler(TempCtrl_EnabledChanged);
			SizeChanged += new EventHandler(TempCtrl_SizeChanged);
		}

		private void TempCtrl_EnabledChanged(object sender, EventArgs e)
		{
			requiresRedraw = true;
			Refresh();
		}

		private void TempCtrl_SizeChanged(object sender, EventArgs e)
		{
			requiresRedraw = true;
			Refresh();
		}

		protected Color OffsetColor(Color color, short offset)
		{
			short val1 = offset;
			short val2_1 = offset;
			short val2_2 = offset;
			if ((int)offset < -255 || (int)offset > (int)byte.MaxValue)
				return color;
			byte r = color.R;
			byte g = color.G;
			byte b = color.B;
			if ((int)offset > 0)
			{
				if ((int)r + (int)offset > (int)byte.MaxValue)
					val1 = (short)((int)byte.MaxValue - (int)r);
				if ((int)g + (int)offset > (int)byte.MaxValue)
					val2_1 = (short)((int)byte.MaxValue - (int)g);
				if ((int)b + (int)offset > (int)byte.MaxValue)
					val2_2 = (short)((int)byte.MaxValue - (int)b);
				offset = Math.Min(Math.Min(val1, val2_1), val2_2);
			}
			else
			{
				if ((int)r + (int)offset < 0)
					val1 = (short)-r;
				if ((int)g + (int)offset < 0)
					val2_1 = (short)-g;
				if ((int)b + (int)offset < 0)
					val2_2 = (short)-b;
				offset = Math.Max(Math.Max(val1, val2_1), val2_2);
			}
			return Color.FromArgb((int)color.A, (int)r + (int)offset, (int)g + (int)offset, (int)b + (int)offset);
		}

		protected void FillCylinder(Graphics g, RectangleF ctrl, Brush fillBrush, Color outlineColor)
		{
			RectangleF rect1 = new RectangleF(ctrl.X, ctrl.Y - 5f, ctrl.Width, 5f);
			RectangleF rect2 = new RectangleF(ctrl.X, ctrl.Bottom - 5f, ctrl.Width, 5f);
			Pen pen = new Pen(outlineColor);
			GraphicsPath path = new GraphicsPath();
			path.AddArc(rect1, 0.0f, 180f);
			path.AddArc(rect2, 180f, -180f);
			path.CloseFigure();
			g.FillPath(fillBrush, path);
			g.DrawPath(pen, path);
			path.Reset();
			path.AddEllipse(rect1);
			g.FillPath(fillBrush, path);
			g.DrawPath(pen, path);
		}

		private double Fahrenheit2Celsius(double fahrenheit)
		{
			return (fahrenheit - 32.0) / 1.8;
		}

		private double Celsius2Fahrenheit(double celsius)
		{
			return celsius * 1.8 + 32.0;
		}

		private void DrawBulb(Graphics g, RectangleF rect, bool enabled)
		{
			g.FillEllipse((Brush)bulbBrush, rectBulb);
			g.DrawEllipse(outlinePen, rectBulb);
		}

		private void DrawCylinder(Graphics g, RectangleF rect, bool enabled)
		{
			FillCylinder(g, rectCylinder, (Brush)fillBrush, colorOutline);
		}

		private void DrawValue(Graphics g, RectangleF rect, bool enabled)
		{
			if (!enabled)
				return;
			fRange = (float)(Range.Max - Range.Min);
			float num = (float)value;
			if (Range.Min < 0.0)
				num += (float)Math.Abs((int)Range.Min);
			if ((double)num > 0.0)
			{
				float height = rectCylinder.Height / 100f * (float)((double)num / (double)fRange * 100.0);
				RectangleF ctrl = new RectangleF(rectCylinder.Left, rectCylinder.Bottom - height, rectCylinder.Width, height);
				FillCylinder(g, ctrl, (Brush)bulbBrush, colorOutline);
			}
			RectangleF layoutRectangle = new RectangleF(pointCenter.X + 10f, rectBulb.Bottom + 5f, 70f, 20f);
			g.DrawString(value.ToString("0 [°C]"), fntText, (Brush)blackBrush, layoutRectangle, strfmtText);
			layoutRectangle = new RectangleF(pointCenter.X - 80f, rectBulb.Bottom + 5f, 70f, 20f);
			g.DrawString(Celsius2Fahrenheit(value).ToString("0 [°F]"), fntText, (Brush)blackBrush, layoutRectangle, strfmtText);
		}

		private void DrawTicks(Graphics g, RectangleF rect, bool enabled)
		{
			if (!drawTics)
				return;
			fRange = (float)(Range.Max - Range.Min);
			Font font = new Font("Arial", 7f);
			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Far;
			format.LineAlignment = StringAlignment.Center;
			float num1 = rectCylinder.Height / fRange;
			float num2 = num1 * (float)largeTicFreq;
			long num3 = (long)Range.Max;
			float top1 = rectCylinder.Top;
			Point pt1;
			Point pt2;
			PointF point;
			while ((double)top1 <= (double)rectCylinder.Bottom)
			{
				pt1 = new Point((int)rectCylinder.Right + 3, (int)top1);
				pt2 = new Point((int)rectCylinder.Right + 10, (int)top1);
				g.DrawLine(scalePen, pt1, pt2);
				point = new PointF(rectCylinder.Right + 30f, top1);
				g.DrawString(num3.ToString(), font, (Brush)blackBrush, point, format);
				num3 -= (long)largeTicFreq;
				top1 += num2;
			}
			float num4 = num1 * (float)smallTicFreq;
			float top2 = rectCylinder.Top;
			while ((double)top2 <= (double)rectCylinder.Bottom)
			{
				pt1 = new Point((int)rectCylinder.Right + 3, (int)top2);
				pt2 = new Point((int)rectCylinder.Right + 8, (int)top2);
				g.DrawLine(scalePen, pt1, pt2);
				top2 += num4;
			}
			double num5 = Celsius2Fahrenheit(Range.Max);
			int num6 = (int)(num5 % 10.0);
			if (num6 != 0)
				num6 = 10 - num6;
			double num7 = num5 - (double)num6;
			fRange = (float)(Celsius2Fahrenheit(Range.Max) - Celsius2Fahrenheit(Range.Min));
			float num8 = rectCylinder.Height / fRange;
			float num9 = num8 * (float)largeTicFreq;
			num3 = (long)Celsius2Fahrenheit(Range.Min);
			float bottom1 = rectCylinder.Bottom;
			while ((double)bottom1 >= (double)rectCylinder.Top)
			{
				pt1 = new Point((int)rectCylinder.Left - 10, (int)bottom1);
				pt2 = new Point((int)rectCylinder.Left - 3, (int)bottom1);
				g.DrawLine(scalePen, pt1, pt2);
				point = new PointF(rectCylinder.Left - 15f, bottom1);
				g.DrawString(num3.ToString(), font, (Brush)blackBrush, point, format);
				num3 += (long)largeTicFreq;
				bottom1 -= num9;
			}
			float num10 = num8 * (float)smallTicFreq;
			float bottom2 = rectCylinder.Bottom;
			while ((double)bottom2 >= (double)rectCylinder.Top)
			{
				pt1 = new Point((int)rectCylinder.Left - 8, (int)bottom2);
				pt2 = new Point((int)rectCylinder.Left - 3, (int)bottom2);
				g.DrawLine(scalePen, pt1, pt2);
				bottom2 -= num10;
			}
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
				e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
				Image image = (Image)new Bitmap(Width, Height);
				Graphics g = Graphics.FromImage(image);
				g.SmoothingMode = SmoothingMode.HighQuality;
				RectangleF rect = new RectangleF(0.0f, 0.0f, (float)Width, (float)Height);
				DrawValue(g, rect, Enabled);
				e.Graphics.DrawImage(image, rect);
			}
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			if (!enableTransparentBackground)
				base.OnPaintBackground(e);
			e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			e.Graphics.FillRectangle((Brush)new SolidBrush(Color.Transparent), new RectangleF(0.0f, 0.0f, (float)Width, (float)Height));
			if (backgroundImg == null || requiresRedraw)
			{
				backgroundImg = (Image)new Bitmap(Width, Height);
				Graphics g = Graphics.FromImage(backgroundImg);
				g.SmoothingMode = SmoothingMode.HighQuality;
				rectBackgroundImg = new RectangleF(0.0f, 0.0f, (float)Width, (float)Height);
				pointCenter = new PointF(rectBackgroundImg.Left + rectBackgroundImg.Width / 2f, rectBackgroundImg.Top + rectBackgroundImg.Height / 2f);
				fTmpWidth = rectBackgroundImg.Width / 5f;
				rectBulb = new RectangleF(pointCenter.X - fTmpWidth, rectBackgroundImg.Bottom - (float)((double)fTmpWidth * 2.0 + 25.0), fTmpWidth * 2f, fTmpWidth * 2f);
				rectCylinder = new RectangleF(pointCenter.X - fTmpWidth / 2f, rectBackgroundImg.Top + (drawTics ? 25f : 10f), fTmpWidth, (float)((double)rectBulb.Top - (double)rectBackgroundImg.Top - (drawTics ? 20.0 : 5.0)));
				if (!Enabled)
				{
					colorFore = SystemColors.ControlDark;
					colorScale = SystemColors.GrayText;
					colorScaleText = SystemColors.GrayText;
					colorOutline = SystemColors.ControlDark;
				}
				else
				{
					colorFore = ForeColor;
					colorScale = Color.FromArgb(0, 0, 0);
					colorScaleText = Color.FromArgb(0, 0, 0);
					colorOutline = Color.FromArgb(64, 0, 0);
				}
				forePen = new Pen(colorFore);
				scalePen = new Pen(colorScale);
				outlinePen = new Pen(colorOutline);
				blackBrush = new SolidBrush(colorScaleText);
				fillBrush = new SolidBrush(colorBack);
				bulbBrush = new LinearGradientBrush(rectBulb, OffsetColor(colorFore, (short)55), OffsetColor(colorFore, (short)-55), LinearGradientMode.Horizontal);
				strfmtText.Alignment = StringAlignment.Center;
				strfmtText.LineAlignment = StringAlignment.Center;
				DrawBulb(g, rectBackgroundImg, Enabled);
				DrawCylinder(g, rectBackgroundImg, Enabled);
				RectangleF rect = new RectangleF(rectCylinder.X, rectCylinder.Y - 5f, rectCylinder.Width, 5f);
				g.DrawEllipse(outlinePen, rect);
				DrawTicks(g, rectBackgroundImg, Enabled);
				requiresRedraw = false;
			}
			e.Graphics.DrawImage(backgroundImg, rectBackgroundImg);
		}

		public class RangeTypeConverter : TypeConverter
		{
			public override bool GetPropertiesSupported(ITypeDescriptorContext context)
			{
				return true;
			}

			public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
			{
				return TypeDescriptor.GetProperties(typeof(TempCtrl.Ranges));
			}
		}

		[TypeConverter(typeof(TempCtrl.RangeTypeConverter))]
		[Category("Behavior")]
		[Description("Range.")]
		public class Ranges
		{
			public delegate void PropertyChangedEventHandler();

			private double min;
			private double max;

			[Description("Minimum value.")]
			public double Min
			{
				get
				{
					return min;
				}
				set
				{
					min = value;
					if (PropertyChanged == null)
						return;
					PropertyChanged();
				}
			}

			[Description("Maximum value.")]
			public double Max
			{
				get
				{
					return max;
				}
				set
				{
					max = value;
					if (PropertyChanged == null)
						return;
					PropertyChanged();
				}
			}

			public event TempCtrl.Ranges.PropertyChangedEventHandler PropertyChanged;

			public Ranges()
			{
				min = -40.0;
				Max = 365.0;
			}

			public Ranges(double min, double max)
			{
				this.min = min;
				this.max = max;
			}

			public override string ToString()
			{
				return (string)(object)max + (object)"; " + (string)(object)min;
			}
		}
	}
}

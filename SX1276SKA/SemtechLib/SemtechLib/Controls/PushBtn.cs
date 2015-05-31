using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SemtechLib.Controls
{
	public class PushBtn : Control
	{
		private ContentAlignment controlAlign = ContentAlignment.MiddleCenter;
		private Size itemSize = new Size();
		private int recessDepth = 1;
		private int bevelHeight = 1;
		private Color buttonColor = Color.Aqua;
		private Color backgroudColor = Color.GhostWhite;
		private Color borderColor = Color.Black;
		private int edgeWidth = 1;
		private float lightAngle = 50f;
		private Color cColor = Color.White;
		private int bevelDepth;
		private bool dome;
		private LinearGradientBrush edgeBrush;
		private Blend edgeBlend;
		private Color edgeColor1;
		private Color edgeColor2;
		private int buttonPressOffset;
		private bool gotFocus;
		private GraphicsPath bpath;
		private GraphicsPath gpath;

		[Description("Indicates how the LED should be aligned")]
		[DefaultValue(ContentAlignment.MiddleCenter)]
		[Category("Appearance")]
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
						point.Y = Size.Height - itemSize.Height;
						return point;
					case ContentAlignment.BottomRight:
						point.X = Size.Width - itemSize.Width;
						point.Y = Size.Height - itemSize.Height;
						return point;
					case ContentAlignment.MiddleRight:
						point.X = Size.Width - itemSize.Width;
						point.Y = (int)((double)Size.Height / 2.0 - (double)itemSize.Height / 2.0);
						return point;
					case ContentAlignment.BottomLeft:
						point.X = 0;
						point.Y = Size.Height - itemSize.Height;
						return point;
					case ContentAlignment.TopLeft:
						point.X = 0;
						point.Y = 0;
						return point;
					case ContentAlignment.TopCenter:
						point.X = (int)((double)Size.Width / 2.0 - (double)itemSize.Width / 2.0);
						point.Y = 0;
						return point;
					case ContentAlignment.TopRight:
						point.X = Size.Width - itemSize.Width;
						point.Y = 0;
						return point;
					case ContentAlignment.MiddleLeft:
						point.X = 0;
						point.Y = (int)((double)Size.Height / 2.0 - (double)itemSize.Height / 2.0);
						return point;
					case ContentAlignment.MiddleCenter:
						point.X = (int)((double)Size.Width / 2.0 - (double)itemSize.Width / 2.0);
						point.Y = (int)((double)Size.Height / 2.0 - (double)itemSize.Height / 2.0);
						return point;
					default:
						point.X = 0;
						point.Y = 0;
						return point;
				}
			}
		}

		private int RecessDepth
		{
			get
			{
				return recessDepth;
			}
			set
			{
				recessDepth = value >= 0 ? (value <= 15 ? value : 15) : 0;
				Invalidate();
			}
		}

		private int BevelHeight
		{
			get
			{
				return bevelHeight;
			}
			set
			{
				bevelHeight = value >= 0 ? value : 0;
				Invalidate();
			}
		}

		private int BevelDepth
		{
			get
			{
				return bevelDepth;
			}
			set
			{
				bevelDepth = value >= 0 ? value : 0;
				Invalidate();
			}
		}

		private bool Dome
		{
			get
			{
				return dome;
			}
			set
			{
				dome = value;
				Invalidate();
			}
		}

		public new event PaintEventHandler Paint;

		public PushBtn()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			BackColor = Color.Transparent;
			Size = new Size(23, 23);
			MouseDown += new MouseEventHandler(mouseDown);
			MouseUp += new MouseEventHandler(mouseUp);
			Enter += new EventHandler(weGotFocus);
			Leave += new EventHandler(weLostFocus);
			KeyDown += new KeyEventHandler(keyDown);
			KeyUp += new KeyEventHandler(keyUp);
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
				if (!Enabled)
				{
					buttonColor = ControlPaint.Light(SystemColors.InactiveCaption);
					backgroudColor = SystemColors.InactiveCaption;
					borderColor = SystemColors.InactiveBorder;
				}
				else
				{
					buttonColor = Color.Aqua;
					backgroudColor = Color.GhostWhite;
					borderColor = Color.Black;
				}
				edgeColor1 = ControlPaint.Light(buttonColor);
				edgeColor2 = ControlPaint.Dark(buttonColor);
				Graphics graphics = e.Graphics;
				graphics.SmoothingMode = SmoothingMode.AntiAlias;
				itemSize.Width = Size.Width * 80 / 100;
				itemSize.Height = Size.Height * 80 / 100;
				int num = Size.Width * 10 / 100;
				Rectangle rectangle = new Rectangle(PosFromAlignment.X + 2, PosFromAlignment.Y + 2, itemSize.Width - 4, itemSize.Height - 4);
				Rectangle rect = new Rectangle(PosFromAlignment.X - num, PosFromAlignment.Y - num, itemSize.Width + num * 2 - 1, itemSize.Height + num * 2 - 1);
				edgeWidth = GetEdgeWidth(rectangle);
				FillBackground(graphics, rect);
				graphics.DrawRectangle(new Pen((Brush)new SolidBrush(borderColor)), rect);
				if (RecessDepth > 0)
					DrawRecess(ref graphics, ref rectangle);
				DrawEdges(graphics, ref rectangle);
				ShrinkShape(ref graphics, ref rectangle, edgeWidth);
				DrawButton(graphics, rectangle);
			}
		}

		protected void FillBackground(Graphics g, Rectangle rect)
		{
			Rectangle rect1 = rect;
			rect1.Inflate(1, 1);
			SolidBrush solidBrush = new SolidBrush(Color.FromKnownColor(KnownColor.Transparent));
			solidBrush.Color = backgroudColor;
			g.FillRectangle((Brush)solidBrush, rect1);
			solidBrush.Dispose();
		}

		protected virtual void DrawRecess(ref Graphics g, ref Rectangle recessRect)
		{
			LinearGradientBrush linearGradientBrush = new LinearGradientBrush(recessRect, ControlPaint.Dark(backgroudColor), ControlPaint.LightLight(backgroudColor), GetLightAngle(50f));
			linearGradientBrush.Blend = new Blend()
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
			Rectangle rect = recessRect;
			ShrinkShape(ref g, ref rect, 1);
			FillShape(g, (object)linearGradientBrush, rect);
			ShrinkShape(ref g, ref recessRect, recessDepth);
		}

		protected virtual void DrawEdges(Graphics g, ref Rectangle edgeRect)
		{
			ShrinkShape(ref g, ref edgeRect, 1);
			Rectangle rect = edgeRect;
			rect.Inflate(1, 1);
			edgeBrush = new LinearGradientBrush(rect, edgeColor1, edgeColor2, GetLightAngle(lightAngle));
			edgeBlend = new Blend();
			edgeBlend.Positions = new float[6]
      {
        0.0f,
        0.2f,
        0.4f,
        0.6f,
        0.8f,
        1f
      };
			edgeBlend.Factors = new float[6]
      {
        0.0f,
        0.0f,
        0.2f,
        0.4f,
        1f,
        1f
      };
			edgeBrush.Blend = edgeBlend;
			FillShape(g, (object)edgeBrush, edgeRect);
		}

		protected virtual void DrawButton(Graphics g, Rectangle buttonRect)
		{
			BuildGraphicsPath(buttonRect);
			PathGradientBrush pathGradientBrush = new PathGradientBrush(bpath);
			pathGradientBrush.SurroundColors = new Color[1]
      {
        buttonColor
      };
			buttonRect.Offset(buttonPressOffset, buttonPressOffset);
			if (bevelHeight > 0)
			{
				buttonRect.Inflate(1, 1);
				pathGradientBrush.CenterPoint = new PointF((float)(buttonRect.X + buttonRect.Width / 8 + buttonPressOffset), (float)(buttonRect.Y + buttonRect.Height / 8 + buttonPressOffset));
				pathGradientBrush.CenterColor = cColor;
				FillShape(g, (object)pathGradientBrush, buttonRect);
				ShrinkShape(ref g, ref buttonRect, bevelHeight);
			}
			if (bevelDepth > 0)
			{
				DrawInnerBevel(g, buttonRect, bevelDepth, buttonColor);
				ShrinkShape(ref g, ref buttonRect, bevelDepth);
			}
			pathGradientBrush.CenterColor = buttonColor;
			if (dome)
			{
				pathGradientBrush.CenterColor = cColor;
				pathGradientBrush.CenterPoint = new PointF((float)(buttonRect.X + buttonRect.Width / 8 + buttonPressOffset), (float)(buttonRect.Y + buttonRect.Height / 8 + buttonPressOffset));
			}
			FillShape(g, (object)pathGradientBrush, buttonRect);
			if (!gotFocus)
				return;
			DrawFocus(g, buttonRect);
		}

		protected virtual void BuildGraphicsPath(Rectangle buttonRect)
		{
			bpath = new GraphicsPath();
			AddShape(bpath, new Rectangle(buttonRect.X - 1, buttonRect.Y - 1, buttonRect.Width + 2, buttonRect.Height + 2));
			AddShape(bpath, buttonRect);
		}

		protected virtual void SetClickableRegion()
		{
			gpath = new GraphicsPath();
			gpath.AddEllipse(ClientRectangle);
			Region = new Region(gpath);
		}

		protected virtual void FillShape(Graphics g, object brush, Rectangle rect)
		{
			if (brush.GetType().ToString() == "System.Drawing.Drawing2D.LinearGradientBrush")
			{
				g.FillEllipse((Brush)brush, rect);
			}
			else
			{
				if (!(brush.GetType().ToString() == "System.Drawing.Drawing2D.PathGradientBrush"))
					return;
				g.FillEllipse((Brush)brush, rect);
			}
		}

		protected virtual void AddShape(GraphicsPath gpath, Rectangle rect)
		{
			gpath.AddEllipse(rect);
		}

		protected virtual void DrawShape(Graphics g, Pen pen, Rectangle rect)
		{
			g.DrawEllipse(pen, rect);
		}

		protected virtual void ShrinkShape(ref Graphics g, ref Rectangle rect, int amount)
		{
			rect.Inflate(-amount, -amount);
		}

		protected virtual void DrawFocus(Graphics g, Rectangle rect)
		{
			rect.Inflate(-2, -2);
			DrawShape(g, new Pen(Color.Black)
			{
				DashStyle = DashStyle.Dot
			}, rect);
		}

		protected virtual void DrawInnerBevel(Graphics g, Rectangle rect, int depth, Color buttonColor)
		{
			Color color2 = ControlPaint.LightLight(buttonColor);
			Color color1 = ControlPaint.Dark(buttonColor);
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
        0.4f,
        0.6f,
        0.6f,
        1f,
        1f
      };
			Rectangle rect1 = rect;
			rect1.Inflate(1, 1);
			FillShape(g, (object)new LinearGradientBrush(rect1, color1, color2, GetLightAngle(50f))
			{
				Blend = blend
			}, rect);
		}

		protected float GetLightAngle(float angle)
		{
			float num = (float)(1.0 - (double)Width / (double)Height);
			return angle - 15f * num;
		}

		protected int GetEdgeWidth(Rectangle rect)
		{
			return rect.Width < 50 | rect.Height < 50 ? 1 : 2;
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
			lightAngle = 230f;
			buttonPressOffset = 1;
			Invalidate();
		}

		protected void buttonUp()
		{
			lightAngle = 50f;
			buttonPressOffset = 0;
			Invalidate();
		}

		protected void keyDown(object sender, KeyEventArgs e)
		{
			if (!(e.KeyCode.ToString() == "Space"))
				return;
			buttonDown();
		}

		protected void keyUp(object sender, KeyEventArgs e)
		{
			if (!(e.KeyCode.ToString() == "Space"))
				return;
			buttonUp();
		}

		protected void weGotFocus(object sender, EventArgs e)
		{
			gotFocus = true;
			Invalidate();
		}

		protected void weLostFocus(object sender, EventArgs e)
		{
			gotFocus = false;
			buttonUp();
			Invalidate();
		}
	}
}

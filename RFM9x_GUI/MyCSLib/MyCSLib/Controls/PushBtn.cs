using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MyCSLib.Controls
{
	public class PushBtn : Control
	{
		private ContentAlignment controlAlign = ContentAlignment.MiddleCenter;
		private Size itemSize = new Size();
		private int recessDepth = 1;
		private int bevelHeight = 1;
		private int bevelDepth = 0;
		private bool dome = false;
		private Color buttonColor = Color.Aqua;
		private Color backgroudColor = Color.GhostWhite;
		private Color borderColor = Color.Black;
		private int edgeWidth = 1;
		private float lightAngle = 50f;
		private Color cColor = Color.White;
		private bool gotFocus = false;
		private LinearGradientBrush edgeBrush;
		private Blend edgeBlend;
		private Color edgeColor1;
		private Color edgeColor2;
		private int buttonPressOffset;
		private GraphicsPath bpath;
		private GraphicsPath gpath;

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
						point.Y = this.Size.Height - this.itemSize.Height;
						return point;
					case ContentAlignment.BottomRight:
						point.X = this.Size.Width - this.itemSize.Width;
						point.Y = this.Size.Height - this.itemSize.Height;
						return point;
					case ContentAlignment.MiddleRight:
						point.X = this.Size.Width - this.itemSize.Width;
						point.Y = (int)((double)this.Size.Height / 2.0 - (double)this.itemSize.Height / 2.0);
						return point;
					case ContentAlignment.BottomLeft:
						point.X = 0;
						point.Y = this.Size.Height - this.itemSize.Height;
						return point;
					case ContentAlignment.TopLeft:
						point.X = 0;
						point.Y = 0;
						return point;
					case ContentAlignment.TopCenter:
						point.X = (int)((double)this.Size.Width / 2.0 - (double)this.itemSize.Width / 2.0);
						point.Y = 0;
						return point;
					case ContentAlignment.TopRight:
						point.X = this.Size.Width - this.itemSize.Width;
						point.Y = 0;
						return point;
					case ContentAlignment.MiddleLeft:
						point.X = 0;
						point.Y = (int)((double)this.Size.Height / 2.0 - (double)this.itemSize.Height / 2.0);
						return point;
					case ContentAlignment.MiddleCenter:
						point.X = (int)((double)this.Size.Width / 2.0 - (double)this.itemSize.Width / 2.0);
						point.Y = (int)((double)this.Size.Height / 2.0 - (double)this.itemSize.Height / 2.0);
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
				return this.recessDepth;
			}
			set
			{
				this.recessDepth = value >= 0 ? (value <= 15 ? value : 15) : 0;
				this.Invalidate();
			}
		}

		private int BevelHeight
		{
			get
			{
				return this.bevelHeight;
			}
			set
			{
				this.bevelHeight = value >= 0 ? value : 0;
				this.Invalidate();
			}
		}

		private int BevelDepth
		{
			get
			{
				return this.bevelDepth;
			}
			set
			{
				this.bevelDepth = value >= 0 ? value : 0;
				this.Invalidate();
			}
		}

		private bool Dome
		{
			get
			{
				return this.dome;
			}
			set
			{
				this.dome = value;
				this.Invalidate();
			}
		}

		public new event PaintEventHandler Paint;

		public PushBtn()
		{
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.BackColor = Color.Transparent;
			this.Size = new Size(23, 23);
			this.MouseDown += new MouseEventHandler(this.mouseDown);
			this.MouseUp += new MouseEventHandler(this.mouseUp);
			this.Enter += new EventHandler(this.weGotFocus);
			this.Leave += new EventHandler(this.weLostFocus);
			this.KeyDown += new KeyEventHandler(this.keyDown);
			this.KeyUp += new KeyEventHandler(this.keyUp);
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
				if (!this.Enabled)
				{
					this.buttonColor = ControlPaint.Light(SystemColors.InactiveCaption);
					this.backgroudColor = SystemColors.InactiveCaption;
					this.borderColor = SystemColors.InactiveBorder;
				}
				else
				{
					this.buttonColor = Color.Aqua;
					this.backgroudColor = Color.GhostWhite;
					this.borderColor = Color.Black;
				}
				this.edgeColor1 = ControlPaint.Light(this.buttonColor);
				this.edgeColor2 = ControlPaint.Dark(this.buttonColor);
				Graphics graphics = e.Graphics;
				graphics.SmoothingMode = SmoothingMode.AntiAlias;
				this.itemSize.Width = this.Size.Width * 80 / 100;
				this.itemSize.Height = this.Size.Height * 80 / 100;
				int num = this.Size.Width * 10 / 100;
				Rectangle rectangle = new Rectangle(this.PosFromAlignment.X + 2, this.PosFromAlignment.Y + 2, this.itemSize.Width - 4, this.itemSize.Height - 4);
				Rectangle rect = new Rectangle(this.PosFromAlignment.X - num, this.PosFromAlignment.Y - num, this.itemSize.Width + num * 2 - 1, this.itemSize.Height + num * 2 - 1);
				this.edgeWidth = this.GetEdgeWidth(rectangle);
				this.FillBackground(graphics, rect);
				graphics.DrawRectangle(new Pen((Brush)new SolidBrush(this.borderColor)), rect);
				if (this.RecessDepth > 0)
					this.DrawRecess(ref graphics, ref rectangle);
				this.DrawEdges(graphics, ref rectangle);
				this.ShrinkShape(ref graphics, ref rectangle, this.edgeWidth);
				this.DrawButton(graphics, rectangle);
			}
		}

		protected void FillBackground(Graphics g, Rectangle rect)
		{
			Rectangle rect1 = rect;
			rect1.Inflate(1, 1);
			SolidBrush solidBrush = new SolidBrush(Color.FromKnownColor(KnownColor.Transparent));
			solidBrush.Color = this.backgroudColor;
			g.FillRectangle((Brush)solidBrush, rect1);
			solidBrush.Dispose();
		}

		protected virtual void DrawRecess(ref Graphics g, ref Rectangle recessRect)
		{
			LinearGradientBrush linearGradientBrush = new LinearGradientBrush(recessRect, ControlPaint.Dark(this.backgroudColor), ControlPaint.LightLight(this.backgroudColor), this.GetLightAngle(50f));
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
			this.ShrinkShape(ref g, ref rect, 1);
			this.FillShape(g, (object)linearGradientBrush, rect);
			this.ShrinkShape(ref g, ref recessRect, this.recessDepth);
		}

		protected virtual void DrawEdges(Graphics g, ref Rectangle edgeRect)
		{
			this.ShrinkShape(ref g, ref edgeRect, 1);
			Rectangle rect = edgeRect;
			rect.Inflate(1, 1);
			this.edgeBrush = new LinearGradientBrush(rect, this.edgeColor1, this.edgeColor2, this.GetLightAngle(this.lightAngle));
			this.edgeBlend = new Blend();
			this.edgeBlend.Positions = new float[6]
      {
        0.0f,
        0.2f,
        0.4f,
        0.6f,
        0.8f,
        1f
      };
			this.edgeBlend.Factors = new float[6]
      {
        0.0f,
        0.0f,
        0.2f,
        0.4f,
        1f,
        1f
      };
			this.edgeBrush.Blend = this.edgeBlend;
			this.FillShape(g, (object)this.edgeBrush, edgeRect);
		}

		protected virtual void DrawButton(Graphics g, Rectangle buttonRect)
		{
			this.BuildGraphicsPath(buttonRect);
			PathGradientBrush pathGradientBrush = new PathGradientBrush(this.bpath);
			pathGradientBrush.SurroundColors = new Color[1]
      {
        this.buttonColor
      };
			buttonRect.Offset(this.buttonPressOffset, this.buttonPressOffset);
			if (this.bevelHeight > 0)
			{
				buttonRect.Inflate(1, 1);
				pathGradientBrush.CenterPoint = new PointF((float)(buttonRect.X + buttonRect.Width / 8 + this.buttonPressOffset), (float)(buttonRect.Y + buttonRect.Height / 8 + this.buttonPressOffset));
				pathGradientBrush.CenterColor = this.cColor;
				this.FillShape(g, (object)pathGradientBrush, buttonRect);
				this.ShrinkShape(ref g, ref buttonRect, this.bevelHeight);
			}
			if (this.bevelDepth > 0)
			{
				this.DrawInnerBevel(g, buttonRect, this.bevelDepth, this.buttonColor);
				this.ShrinkShape(ref g, ref buttonRect, this.bevelDepth);
			}
			pathGradientBrush.CenterColor = this.buttonColor;
			if (this.dome)
			{
				pathGradientBrush.CenterColor = this.cColor;
				pathGradientBrush.CenterPoint = new PointF((float)(buttonRect.X + buttonRect.Width / 8 + this.buttonPressOffset), (float)(buttonRect.Y + buttonRect.Height / 8 + this.buttonPressOffset));
			}
			this.FillShape(g, (object)pathGradientBrush, buttonRect);
			if (!this.gotFocus)
				return;
			this.DrawFocus(g, buttonRect);
		}

		protected virtual void BuildGraphicsPath(Rectangle buttonRect)
		{
			this.bpath = new GraphicsPath();
			this.AddShape(this.bpath, new Rectangle(buttonRect.X - 1, buttonRect.Y - 1, buttonRect.Width + 2, buttonRect.Height + 2));
			this.AddShape(this.bpath, buttonRect);
		}

		protected virtual void SetClickableRegion()
		{
			this.gpath = new GraphicsPath();
			this.gpath.AddEllipse(this.ClientRectangle);
			this.Region = new Region(this.gpath);
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
			this.DrawShape(g, new Pen(Color.Black)
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
			this.FillShape(g, (object)new LinearGradientBrush(rect1, color1, color2, this.GetLightAngle(50f))
			{
				Blend = blend
			}, rect);
		}

		protected float GetLightAngle(float angle)
		{
			float num = (float)(1.0 - (double)this.Width / (double)this.Height);
			return angle - 15f * num;
		}

		protected int GetEdgeWidth(Rectangle rect)
		{
			return rect.Width < 50 | rect.Height < 50 ? 1 : 2;
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
			this.lightAngle = 230f;
			this.buttonPressOffset = 1;
			this.Invalidate();
		}

		protected void buttonUp()
		{
			this.lightAngle = 50f;
			this.buttonPressOffset = 0;
			this.Invalidate();
		}

		protected void keyDown(object sender, KeyEventArgs e)
		{
			if (!(e.KeyCode.ToString() == "Space"))
				return;
			this.buttonDown();
		}

		protected void keyUp(object sender, KeyEventArgs e)
		{
			if (!(e.KeyCode.ToString() == "Space"))
				return;
			this.buttonUp();
		}

		protected void weGotFocus(object sender, EventArgs e)
		{
			this.gotFocus = true;
			this.Invalidate();
		}

		protected void weLostFocus(object sender, EventArgs e)
		{
			this.gotFocus = false;
			this.buttonUp();
			this.Invalidate();
		}
	}
}

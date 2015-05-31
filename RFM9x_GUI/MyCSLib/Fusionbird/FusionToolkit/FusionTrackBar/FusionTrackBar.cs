using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Fusionbird.FusionToolkit.FusionTrackBar
{
	public class FusionTrackBar : System.Windows.Forms.TrackBar
	{
		private Rectangle ChannelBounds;
		private TrackBarOwnerDrawParts m_OwnerDrawParts;
		private Rectangle ThumbBounds;
		private int ThumbState;

		[DefaultValue(typeof(TrackBarOwnerDrawParts), "None")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Description("Gets/sets the trackbar parts that will be OwnerDrawn.")]
		[Editor(typeof(TrackDrawModeEditor), typeof(UITypeEditor))]
		public TrackBarOwnerDrawParts OwnerDrawParts
		{
			get
			{
				return this.m_OwnerDrawParts;
			}
			set
			{
				this.m_OwnerDrawParts = value;
			}
		}

		public event EventHandler<TrackBarDrawItemEventArgs> DrawChannel;

		public event EventHandler<TrackBarDrawItemEventArgs> DrawThumb;

		public event EventHandler<TrackBarDrawItemEventArgs> DrawTicks;

		public FusionTrackBar()
		{
			this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 20)
			{
				m.Result = IntPtr.Zero;
			}
			else
			{
				base.WndProc(ref m);
				if (m.Msg == 8270)
				{
					Fusionbird.FusionToolkit.NativeMethods.NMHDR nmhdr = (Fusionbird.FusionToolkit.NativeMethods.NMHDR)Marshal.PtrToStructure(m.LParam, typeof(Fusionbird.FusionToolkit.NativeMethods.NMHDR));
					if (nmhdr.code == -12)
					{
						Marshal.StructureToPtr((object)nmhdr, m.LParam, false);
						Fusionbird.FusionToolkit.NativeMethods.NMCUSTOMDRAW nmcustomdraw = (Fusionbird.FusionToolkit.NativeMethods.NMCUSTOMDRAW)Marshal.PtrToStructure(m.LParam, typeof(Fusionbird.FusionToolkit.NativeMethods.NMCUSTOMDRAW));
						if (nmcustomdraw.dwDrawStage == Fusionbird.FusionToolkit.NativeMethods.CustomDrawDrawStage.CDDS_PREPAINT)
						{
							Graphics graphics = Graphics.FromHdc(nmcustomdraw.hdc);
							PaintEventArgs e = new PaintEventArgs(graphics, this.Bounds);
							e.Graphics.TranslateTransform((float)-this.Left, (float)-this.Top);
							this.InvokePaintBackground(this.Parent, e);
							this.InvokePaint(this.Parent, e);
							SolidBrush solidBrush = new SolidBrush(this.BackColor);
							e.Graphics.FillRectangle((Brush)solidBrush, this.Bounds);
							solidBrush.Dispose();
							e.Graphics.ResetTransform();
							e.Dispose();
							graphics.Dispose();
							IntPtr num = new IntPtr(48);
							m.Result = num;
						}
						else if (nmcustomdraw.dwDrawStage == Fusionbird.FusionToolkit.NativeMethods.CustomDrawDrawStage.CDDS_POSTPAINT)
						{
							this.OnDrawTicks(nmcustomdraw.hdc);
							this.OnDrawChannel(nmcustomdraw.hdc);
							this.OnDrawThumb(nmcustomdraw.hdc);
						}
						else if (nmcustomdraw.dwDrawStage == Fusionbird.FusionToolkit.NativeMethods.CustomDrawDrawStage.CDDS_ITEMPREPAINT)
						{
							if (nmcustomdraw.dwItemSpec.ToInt32() == 2)
							{
								this.ThumbBounds = nmcustomdraw.rc.ToRectangle();
								this.ThumbState = !this.Enabled ? 5 : (nmcustomdraw.uItemState != Fusionbird.FusionToolkit.NativeMethods.CustomDrawItemState.CDIS_SELECTED ? 1 : 3);
								this.OnDrawThumb(nmcustomdraw.hdc);
							}
							else if (nmcustomdraw.dwItemSpec.ToInt32() == 3)
							{
								this.ChannelBounds = nmcustomdraw.rc.ToRectangle();
								this.OnDrawChannel(nmcustomdraw.hdc);
							}
							else if (nmcustomdraw.dwItemSpec.ToInt32() == 1)
								this.OnDrawTicks(nmcustomdraw.hdc);
							IntPtr num = new IntPtr(4);
							m.Result = num;
						}
					}
				}
			}
		}

		private void DrawHorizontalTicks(Graphics g, Color color)
		{
			int num1 = this.Maximum / this.TickFrequency - 1;
			Pen pen = new Pen(color);
			RectangleF rectangleF1 = new RectangleF((float)(this.ChannelBounds.Left + this.ThumbBounds.Width / 2), (float)(this.ThumbBounds.Top - 5), 0.0f, 3f);
			RectangleF rectangleF2 = new RectangleF((float)(this.ChannelBounds.Right - this.ThumbBounds.Width / 2 - 1), (float)(this.ThumbBounds.Top - 5), 0.0f, 3f);
			float x = (rectangleF2.Right - rectangleF1.Left) / (float)(num1 + 1);
			RectangleF rectangleF3;
			if (this.TickStyle != TickStyle.BottomRight)
			{
				g.DrawLine(pen, rectangleF1.Left, rectangleF1.Top, rectangleF1.Right, rectangleF1.Bottom);
				g.DrawLine(pen, rectangleF2.Left, rectangleF2.Top, rectangleF2.Right, rectangleF2.Bottom);
				rectangleF3 = rectangleF1;
				--rectangleF3.Height;
				rectangleF3.Offset(x, 1f);
				int num2 = num1 - 1;
				for (int index = 0; index <= num2; ++index)
				{
					g.DrawLine(pen, rectangleF3.Left, rectangleF3.Top, rectangleF3.Left, rectangleF3.Bottom);
					rectangleF3.Offset(x, 0.0f);
				}
			}
			rectangleF1.Offset(0.0f, (float)(this.ThumbBounds.Height + 6));
			rectangleF2.Offset(0.0f, (float)(this.ThumbBounds.Height + 6));
			if (this.TickStyle != TickStyle.TopLeft)
			{
				g.DrawLine(pen, rectangleF1.Left, rectangleF1.Top, rectangleF1.Left, rectangleF1.Bottom);
				g.DrawLine(pen, rectangleF2.Left, rectangleF2.Top, rectangleF2.Left, rectangleF2.Bottom);
				rectangleF3 = rectangleF1;
				--rectangleF3.Height;
				rectangleF3.Offset(x, 0.0f);
				int num2 = num1 - 1;
				for (int index = 0; index <= num2; ++index)
				{
					g.DrawLine(pen, rectangleF3.Left, rectangleF3.Top, rectangleF3.Left, rectangleF3.Bottom);
					rectangleF3.Offset(x, 0.0f);
				}
			}
			pen.Dispose();
		}

		private void DrawVerticalTicks(Graphics g, Color color)
		{
			int num1 = this.Maximum / this.TickFrequency - 1;
			Pen pen = new Pen(color);
			RectangleF rectangleF1 = new RectangleF((float)(this.ThumbBounds.Left - 5), (float)(this.ChannelBounds.Bottom - this.ThumbBounds.Height / 2 - 1), 3f, 0.0f);
			RectangleF rectangleF2 = new RectangleF((float)(this.ThumbBounds.Left - 5), (float)(this.ChannelBounds.Top + this.ThumbBounds.Height / 2), 3f, 0.0f);
			float y = (rectangleF2.Bottom - rectangleF1.Top) / (float)(num1 + 1);
			RectangleF rectangleF3;
			if (this.TickStyle != TickStyle.BottomRight)
			{
				g.DrawLine(pen, rectangleF1.Left, rectangleF1.Top, rectangleF1.Right, rectangleF1.Bottom);
				g.DrawLine(pen, rectangleF2.Left, rectangleF2.Top, rectangleF2.Right, rectangleF2.Bottom);
				rectangleF3 = rectangleF1;
				--rectangleF3.Width;
				rectangleF3.Offset(1f, y);
				int num2 = num1 - 1;
				for (int index = 0; index <= num2; ++index)
				{
					g.DrawLine(pen, rectangleF3.Left, rectangleF3.Top, rectangleF3.Right, rectangleF3.Bottom);
					rectangleF3.Offset(0.0f, y);
				}
			}
			rectangleF1.Offset((float)(this.ThumbBounds.Width + 6), 0.0f);
			rectangleF2.Offset((float)(this.ThumbBounds.Width + 6), 0.0f);
			if (this.TickStyle != TickStyle.TopLeft)
			{
				g.DrawLine(pen, rectangleF1.Left, rectangleF1.Top, rectangleF1.Right, rectangleF1.Bottom);
				g.DrawLine(pen, rectangleF2.Left, rectangleF2.Top, rectangleF2.Right, rectangleF2.Bottom);
				rectangleF3 = rectangleF1;
				--rectangleF3.Width;
				rectangleF3.Offset(0.0f, y);
				int num2 = num1 - 1;
				for (int index = 0; index <= num2; ++index)
				{
					g.DrawLine(pen, rectangleF3.Left, rectangleF3.Top, rectangleF3.Right, rectangleF3.Bottom);
					rectangleF3.Offset(0.0f, y);
				}
			}
			pen.Dispose();
		}

		private void DrawPointerDown(Graphics g)
		{
			Point[] points1 = new Point[6]
      {
        new Point(this.ThumbBounds.Left + this.ThumbBounds.Width / 2, this.ThumbBounds.Bottom - 1),
        new Point(this.ThumbBounds.Left, this.ThumbBounds.Bottom - this.ThumbBounds.Width / 2 - 1),
        this.ThumbBounds.Location,
        new Point(this.ThumbBounds.Right - 1, this.ThumbBounds.Top),
        new Point(this.ThumbBounds.Right - 1, this.ThumbBounds.Bottom - this.ThumbBounds.Width / 2 - 1),
        new Point(this.ThumbBounds.Left + this.ThumbBounds.Width / 2, this.ThumbBounds.Bottom - 1)
      };
			GraphicsPath path = new GraphicsPath();
			path.AddLines(points1);
			Region region = new Region(path);
			g.Clip = region;
			if (this.ThumbState == 3 || !this.Enabled)
				ControlPaint.DrawButton(g, this.ThumbBounds, ButtonState.All);
			else
				g.Clear(SystemColors.Control);
			g.ResetClip();
			region.Dispose();
			path.Dispose();
			Point[] points2 = new Point[4]
      {
        points1[0],
        points1[1],
        points1[2],
        points1[3]
      };
			g.DrawLines(SystemPens.ControlLightLight, points2);
			Point[] points3 = new Point[3]
      {
        points1[3],
        points1[4],
        points1[5]
      };
			g.DrawLines(SystemPens.ControlDarkDark, points3);
			points1[0].Offset(0, -1);
			points1[1].Offset(1, 0);
			points1[2].Offset(1, 1);
			points1[3].Offset(-1, 1);
			points1[4].Offset(-1, 0);
			points1[5] = points1[0];
			Point[] points4 = new Point[4]
      {
        points1[0],
        points1[1],
        points1[2],
        points1[3]
      };
			g.DrawLines(SystemPens.ControlLight, points4);
			Point[] points5 = new Point[3]
      {
        points1[3],
        points1[4],
        points1[5]
      };
			g.DrawLines(SystemPens.ControlDark, points5);
		}

		private void DrawPointerLeft(Graphics g)
		{
			Point[] points1 = new Point[6]
      {
        new Point(this.ThumbBounds.Left, this.ThumbBounds.Top + this.ThumbBounds.Height / 2),
        new Point(this.ThumbBounds.Left + this.ThumbBounds.Height / 2, this.ThumbBounds.Top),
        new Point(this.ThumbBounds.Right - 1, this.ThumbBounds.Top),
        new Point(this.ThumbBounds.Right - 1, this.ThumbBounds.Bottom - 1),
        new Point(this.ThumbBounds.Left + this.ThumbBounds.Height / 2, this.ThumbBounds.Bottom - 1),
        new Point(this.ThumbBounds.Left, this.ThumbBounds.Top + this.ThumbBounds.Height / 2)
      };
			GraphicsPath path = new GraphicsPath();
			path.AddLines(points1);
			Region region = new Region(path);
			g.Clip = region;
			if (this.ThumbState == 3 || !this.Enabled)
				ControlPaint.DrawButton(g, this.ThumbBounds, ButtonState.All);
			else
				g.Clear(SystemColors.Control);
			g.ResetClip();
			region.Dispose();
			path.Dispose();
			Point[] points2 = new Point[3]
      {
        points1[0],
        points1[1],
        points1[2]
      };
			g.DrawLines(SystemPens.ControlLightLight, points2);
			Point[] points3 = new Point[4]
      {
        points1[2],
        points1[3],
        points1[4],
        points1[5]
      };
			g.DrawLines(SystemPens.ControlDarkDark, points3);
			points1[0].Offset(1, 0);
			points1[1].Offset(0, 1);
			points1[2].Offset(-1, 1);
			points1[3].Offset(-1, -1);
			points1[4].Offset(0, -1);
			points1[5] = points1[0];
			Point[] points4 = new Point[3]
      {
        points1[0],
        points1[1],
        points1[2]
      };
			g.DrawLines(SystemPens.ControlLight, points4);
			Point[] points5 = new Point[4]
      {
        points1[2],
        points1[3],
        points1[4],
        points1[5]
      };
			g.DrawLines(SystemPens.ControlDark, points5);
		}

		private void DrawPointerRight(Graphics g)
		{
			Point[] points1 = new Point[6]
      {
        new Point(this.ThumbBounds.Left, this.ThumbBounds.Bottom - 1),
        new Point(this.ThumbBounds.Left, this.ThumbBounds.Top),
        new Point(this.ThumbBounds.Right - this.ThumbBounds.Height / 2 - 1, this.ThumbBounds.Top),
        new Point(this.ThumbBounds.Right - 1, this.ThumbBounds.Top + this.ThumbBounds.Height / 2),
        new Point(this.ThumbBounds.Right - this.ThumbBounds.Height / 2 - 1, this.ThumbBounds.Bottom - 1),
        new Point(this.ThumbBounds.Left, this.ThumbBounds.Bottom - 1)
      };
			GraphicsPath path = new GraphicsPath();
			path.AddLines(points1);
			Region region = new Region(path);
			g.Clip = region;
			if (this.ThumbState == 3 || !this.Enabled)
				ControlPaint.DrawButton(g, this.ThumbBounds, ButtonState.All);
			else
				g.Clear(SystemColors.Control);
			g.ResetClip();
			region.Dispose();
			path.Dispose();
			Point[] points2 = new Point[4]
      {
        points1[0],
        points1[1],
        points1[2],
        points1[3]
      };
			g.DrawLines(SystemPens.ControlLightLight, points2);
			Point[] points3 = new Point[3]
      {
        points1[3],
        points1[4],
        points1[5]
      };
			g.DrawLines(SystemPens.ControlDarkDark, points3);
			points1[0].Offset(1, -1);
			points1[1].Offset(1, 1);
			points1[2].Offset(0, 1);
			points1[3].Offset(-1, 0);
			points1[4].Offset(0, -1);
			points1[5] = points1[0];
			Point[] points4 = new Point[4]
      {
        points1[0],
        points1[1],
        points1[2],
        points1[3]
      };
			g.DrawLines(SystemPens.ControlLight, points4);
			Point[] points5 = new Point[3]
      {
        points1[3],
        points1[4],
        points1[5]
      };
			g.DrawLines(SystemPens.ControlDark, points5);
		}

		private void DrawPointerUp(Graphics g)
		{
			Point[] points1 = new Point[6]
      {
        new Point(this.ThumbBounds.Left, this.ThumbBounds.Bottom - 1),
        new Point(this.ThumbBounds.Left, this.ThumbBounds.Top + this.ThumbBounds.Width / 2),
        new Point(this.ThumbBounds.Left + this.ThumbBounds.Width / 2, this.ThumbBounds.Top),
        new Point(this.ThumbBounds.Right - 1, this.ThumbBounds.Top + this.ThumbBounds.Width / 2),
        new Point(this.ThumbBounds.Right - 1, this.ThumbBounds.Bottom - 1),
        new Point(this.ThumbBounds.Left, this.ThumbBounds.Bottom - 1)
      };
			GraphicsPath path = new GraphicsPath();
			path.AddLines(points1);
			Region region = new Region(path);
			g.Clip = region;
			if (this.ThumbState == 3 || !this.Enabled)
				ControlPaint.DrawButton(g, this.ThumbBounds, ButtonState.All);
			else
				g.Clear(SystemColors.Control);
			g.ResetClip();
			region.Dispose();
			path.Dispose();
			Point[] points2 = new Point[3]
      {
        points1[0],
        points1[1],
        points1[2]
      };
			g.DrawLines(SystemPens.ControlLightLight, points2);
			Point[] points3 = new Point[4]
      {
        points1[2],
        points1[3],
        points1[4],
        points1[5]
      };
			g.DrawLines(SystemPens.ControlDarkDark, points3);
			points1[0].Offset(1, -1);
			points1[1].Offset(1, 0);
			points1[2].Offset(0, 1);
			points1[3].Offset(-1, 0);
			points1[4].Offset(-1, -1);
			points1[5] = points1[0];
			Point[] points4 = new Point[3]
      {
        points1[0],
        points1[1],
        points1[2]
      };
			g.DrawLines(SystemPens.ControlLight, points4);
			Point[] points5 = new Point[4]
      {
        points1[2],
        points1[3],
        points1[4],
        points1[5]
      };
			g.DrawLines(SystemPens.ControlDark, points5);
		}

		protected virtual void OnDrawTicks(IntPtr hdc)
		{
			Graphics graphics = Graphics.FromHdc(hdc);
			if ((this.OwnerDrawParts & TrackBarOwnerDrawParts.Ticks) == TrackBarOwnerDrawParts.Ticks && !this.DesignMode)
			{
				TrackBarDrawItemEventArgs e = new TrackBarDrawItemEventArgs(graphics, this.ClientRectangle, (TrackBarItemState)this.ThumbState);
				if (this.DrawTicks != null)
					this.DrawTicks((object)this, e);
			}
			else
			{
				if (this.TickStyle == TickStyle.None || this.ThumbBounds.Equals((object)Rectangle.Empty))
					return;
				Color color = Color.Black;
				if (VisualStyleRenderer.IsSupported)
					color = new VisualStyleRenderer("TRACKBAR", 9, this.ThumbState).GetColor(ColorProperty.TextColor);
				if (this.Orientation == Orientation.Horizontal)
					this.DrawHorizontalTicks(graphics, color);
				else
					this.DrawVerticalTicks(graphics, color);
			}
			graphics.Dispose();
		}

		protected virtual void OnDrawThumb(IntPtr hdc)
		{
			Graphics graphics = Graphics.FromHdc(hdc);
			graphics.Clip = new Region(this.ThumbBounds);
			if ((this.OwnerDrawParts & TrackBarOwnerDrawParts.Thumb) == TrackBarOwnerDrawParts.Thumb && !this.DesignMode)
			{
				TrackBarDrawItemEventArgs e = new TrackBarDrawItemEventArgs(graphics, this.ThumbBounds, (TrackBarItemState)this.ThumbState);
				if (this.DrawThumb != null)
					this.DrawThumb((object)this, e);
			}
			else
			{
				Fusionbird.FusionToolkit.NativeMethods.TrackBarParts trackBarParts = Fusionbird.FusionToolkit.NativeMethods.TrackBarParts.TKP_THUMB;
				if (this.ThumbBounds.Equals((object)Rectangle.Empty))
					return;
				switch (this.TickStyle)
				{
					case TickStyle.None:
					case TickStyle.BottomRight:
						trackBarParts = this.Orientation != Orientation.Horizontal ? Fusionbird.FusionToolkit.NativeMethods.TrackBarParts.TKP_THUMBRIGHT : Fusionbird.FusionToolkit.NativeMethods.TrackBarParts.TKP_THUMBBOTTOM;
						break;
					case TickStyle.TopLeft:
						trackBarParts = this.Orientation != Orientation.Horizontal ? Fusionbird.FusionToolkit.NativeMethods.TrackBarParts.TKP_THUMBLEFT : Fusionbird.FusionToolkit.NativeMethods.TrackBarParts.TKP_THUMBTOP;
						break;
					case TickStyle.Both:
						trackBarParts = this.Orientation != Orientation.Horizontal ? Fusionbird.FusionToolkit.NativeMethods.TrackBarParts.TKP_THUMBVERT : Fusionbird.FusionToolkit.NativeMethods.TrackBarParts.TKP_THUMB;
						break;
				}
				if (VisualStyleRenderer.IsSupported)
				{
					new VisualStyleRenderer("TRACKBAR", (int)trackBarParts, this.ThumbState).DrawBackground((IDeviceContext)graphics, this.ThumbBounds);
					graphics.ResetClip();
					graphics.Dispose();
					return;
				}
				switch (trackBarParts)
				{
					case Fusionbird.FusionToolkit.NativeMethods.TrackBarParts.TKP_THUMBBOTTOM:
						this.DrawPointerDown(graphics);
						break;
					case Fusionbird.FusionToolkit.NativeMethods.TrackBarParts.TKP_THUMBTOP:
						this.DrawPointerUp(graphics);
						break;
					case Fusionbird.FusionToolkit.NativeMethods.TrackBarParts.TKP_THUMBLEFT:
						this.DrawPointerLeft(graphics);
						break;
					case Fusionbird.FusionToolkit.NativeMethods.TrackBarParts.TKP_THUMBRIGHT:
						this.DrawPointerRight(graphics);
						break;
					default:
						if (this.ThumbState == 3 || !this.Enabled)
							ControlPaint.DrawButton(graphics, this.ThumbBounds, ButtonState.All);
						else
							graphics.FillRectangle(SystemBrushes.Control, this.ThumbBounds);
						ControlPaint.DrawBorder3D(graphics, this.ThumbBounds, Border3DStyle.Raised);
						break;
				}
			}
			graphics.ResetClip();
			graphics.Dispose();
		}

		protected virtual void OnDrawChannel(IntPtr hdc)
		{
			Graphics graphics = Graphics.FromHdc(hdc);
			if ((this.OwnerDrawParts & TrackBarOwnerDrawParts.Channel) == TrackBarOwnerDrawParts.Channel && !this.DesignMode)
			{
				TrackBarDrawItemEventArgs e = new TrackBarDrawItemEventArgs(graphics, this.ChannelBounds, (TrackBarItemState)this.ThumbState);
				if (this.DrawChannel != null)
					this.DrawChannel((object)this, e);
			}
			else
			{
				if (this.ChannelBounds.Equals((object)Rectangle.Empty))
					return;
				if (VisualStyleRenderer.IsSupported)
				{
					new VisualStyleRenderer("TRACKBAR", 1, 1).DrawBackground((IDeviceContext)graphics, this.ChannelBounds);
					graphics.ResetClip();
					graphics.Dispose();
					return;
				}
				ControlPaint.DrawBorder3D(graphics, this.ChannelBounds, Border3DStyle.Sunken);
			}
			graphics.Dispose();
		}
	}
}
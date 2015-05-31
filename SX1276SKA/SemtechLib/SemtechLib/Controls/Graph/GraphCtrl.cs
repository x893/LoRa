using SemtechLib.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SemtechLib.Controls.Graph
{
	public class GraphCtrl : UserControl
	{
		private Font fontText = new Font(FontFamily.GenericSansSerif, 8f);
		private GraphCtrl.GraphDataCollection dataCollection = new GraphCtrl.GraphDataCollection();
		private GraphCtrl.GraphFrame frameFull = new GraphCtrl.GraphFrame();
		private Rectangle workingArea = new Rectangle();
		private Point mousePos = new Point();
		private GraphCtrl.GraphFrame frameZoom = new GraphCtrl.GraphFrame();
		private GraphCtrl.GraphScale scaleX = new GraphCtrl.GraphScale();
		private GraphCtrl.GraphScale scaleY = new GraphCtrl.GraphScale();
		private GraphCtrl.GraphGrid gridX = new GraphCtrl.GraphGrid();
		private GraphCtrl.GraphGrid gridY = new GraphCtrl.GraphGrid();
		private int previousClick = SystemInformation.DoubleClickTime + 1;
		private IContainer components;
		private Timer tmrRefresh;
		private CheckBox checkBoxAutoScale;
		private CheckBox checkBoxZoomIn;
		private CheckBox checkBoxZoomOut;
		private CheckBox checkBoxHand;
		private Label labelScaleYmin;
		private Label labelScaleYmax;
		private TextBox textBoxScaleYmax;
		private TextBox textBoxScaleYmin;
		private Label labelSample;
		private ToolTip grphToolTip;
		private float zoomLeft;
		private float zoomRight;
		private float zoomTop;
		private float zoomBottom;
		private int rightOffset;
		private int leftOffset;
		private bool updateData;
		private GraphCtrl.eGraphType graphType;
		private Brush brushBackground;
		private Brush brushPoint;
		private Pen penFrame;
		private Pen penLine;
		private Color colorBackground;
		private Color colorFrame;
		private Color colorLine;
		private Brush brushText;
		private Color colorText;
		private int history;
		private bool frameFit;
		private GraphCtrl.eZoomOption zoomOption;

		[Description("Color of the Graph Text")]
		[Category("Graph")]
		public Color ColorText
		{
			get
			{
				return colorText;
			}
			set
			{
				colorText = value;
				brushText = (Brush)new SolidBrush(colorText);
				labelScaleYmax.ForeColor = colorText;
				textBoxScaleYmax.ForeColor = colorText;
				labelScaleYmin.ForeColor = colorText;
				textBoxScaleYmin.ForeColor = colorText;
				labelSample.ForeColor = colorText;
				updateData = true;
			}
		}

		[Description("Color of the Graph Background")]
		[Category("Graph")]
		public Color ColorBackground
		{
			get
			{
				return colorBackground;
			}
			set
			{
				colorBackground = value;
				brushBackground = (Brush)new SolidBrush(colorBackground);
				labelScaleYmax.BackColor = colorBackground;
				textBoxScaleYmax.BackColor = colorBackground;
				labelScaleYmin.BackColor = colorBackground;
				textBoxScaleYmin.BackColor = colorBackground;
				labelSample.BackColor = Color.Transparent;
				updateData = true;
			}
		}

		[Category("Graph")]
		[Description("Color of the Graph Frame")]
		public Color ColorFrame
		{
			get
			{
				return colorFrame;
			}
			set
			{
				colorFrame = value;
				penFrame = new Pen(colorFrame);
				updateData = true;
			}
		}

		[Description("Color of the Graph Line")]
		[Category("Graph")]
		public Color ColorLine
		{
			get
			{
				return colorLine;
			}
			set
			{
				colorLine = value;
				penLine = new Pen(colorLine);
				brushPoint = (Brush)new SolidBrush(colorLine);
				updateData = true;
			}
		}

		[DefaultValue(GraphCtrl.eGraphType.Dot)]
		[Category("Graph")]
		[Description("Graph Type")]
		public GraphCtrl.eGraphType Type
		{
			get
			{
				return graphType;
			}
			set
			{
				graphType = value;
				updateData = true;
			}
		}

		[Description("How many point to keep in hystory")]
		[DefaultValue(100)]
		[Category("Graph")]
		public int History
		{
			get
			{
				return history;
			}
			set
			{
				history = value;
				foreach (GraphCtrl.GraphData graphData in dataCollection)
				{
					if (graphData.Value.Count > history)
						graphData.Value.RemoveRange(0, graphData.Value.Count - history);
				}
			}
		}

		[Description("Let the Frame Fit in the Control Window")]
		[DefaultValue(false)]
		[Category("Graph")]
		public bool FrameFit
		{
			get
			{
				return frameFit;
			}
			set
			{
				frameFit = value;
				updateData = true;
			}
		}

		[Category("Graph")]
		[Description("Update Rate in milliseconds")]
		[DefaultValue(25)]
		public int UpdateRate
		{
			get
			{
				return tmrRefresh.Interval;
			}
			set
			{
				tmrRefresh.Interval = value;
			}
		}

		[DefaultValue(false)]
		[Category("Graph")]
		[Description("Enable the Autoscale features.")]
		public bool AutoScale
		{
			get
			{
				return zoomOption == GraphCtrl.eZoomOption.AutoScale;
			}
			set
			{
				if (value)
					zoomOption = GraphCtrl.eZoomOption.AutoScale;
				else
					zoomOption = GraphCtrl.eZoomOption.None;
			}
		}

		[DefaultValue(false)]
		[Category("Graph")]
		[Description("Sets the graph to full scale.")]
		public bool FullScale
		{
			get
			{
				return zoomOption == GraphCtrl.eZoomOption.FullScale;
			}
			set
			{
				if (value)
				{
					zoomOption = GraphCtrl.eZoomOption.FullScale;
					zoomLeft = 0.0f;
					zoomRight = 0.0f;
					zoomTop = 0.0f;
					zoomBottom = 0.0f;
				}
				else
					zoomOption = GraphCtrl.eZoomOption.None;
			}
		}

		[Category("Graph")]
		[Description("Enable the Zoom features.")]
		[DefaultValue(false)]
		public bool Zoom
		{
			get
			{
				return checkBoxAutoScale.Visible;
			}
			set
			{
				checkBoxAutoScale.Visible = value;
				checkBoxZoomIn.Visible = value;
				checkBoxZoomOut.Visible = value;
				checkBoxHand.Visible = value;
				rightOffset = !value ? 0 : checkBoxAutoScale.ClientRectangle.Width / 2;
				updateData = true;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GraphCtrl.GraphScale ScaleX
		{
			get
			{
				return scaleX;
			}
			set
			{
				scaleX = value;
				updateData = true;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GraphCtrl.GraphScale ScaleY
		{
			get
			{
				return scaleY;
			}
			set
			{
				scaleY = value;
				updateData = true;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GraphCtrl.GraphGrid GridX
		{
			get
			{
				return gridX;
			}
			set
			{
				gridX = value;
				updateData = true;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GraphCtrl.GraphGrid GridY
		{
			get
			{
				return gridY;
			}
			set
			{
				gridY = value;
				updateData = true;
			}
		}

		private List<GraphCtrl.Line> GridLinesX
		{
			get
			{
				List<GraphCtrl.Line> list = new List<GraphCtrl.Line>();
				Point pt1 = new Point(0, workingArea.Top);
				Point pt2 = new Point(0, workingArea.Bottom);
				int num;
				if (GridX.ShowMinor)
				{
					num = GridX.Minor;
				}
				else
				{
					if (!GridX.ShowMajor)
						return list;
					num = GridX.Major;
				}
				int main = GridX.Main;
				while (main > FrameZoom.Left)
					main -= num;
				while (main < FrameZoom.Left)
					main += num;
				while (main < FrameZoom.Right)
				{
					pt1.X = pt2.X = workingArea.Left + workingArea.Width * (main - FrameZoom.Left) / FrameZoom.Width;
					list.Add(new GraphCtrl.Line(pt1, pt2));
					main += num;
				}
				return list;
			}
		}

		private List<GraphCtrl.Line> GridLinesY
		{
			get
			{
				List<GraphCtrl.Line> list = new List<GraphCtrl.Line>();
				Point pt1 = new Point(workingArea.Left, 0);
				Point pt2 = new Point(workingArea.Right, 0);
				int num;
				if (GridY.ShowMinor)
				{
					num = GridY.Minor;
				}
				else
				{
					if (!GridY.ShowMajor)
						return list;
					num = GridY.Major;
				}
				int main = GridY.Main;
				while (main > FrameZoom.Bottom)
					main -= num;
				while (main < FrameZoom.Bottom)
					main += num;
				while (main < FrameZoom.Top)
				{
					pt1.Y = pt2.Y = workingArea.Bottom - workingArea.Height * (main - FrameZoom.Bottom) / FrameZoom.Height;
					list.Add(new GraphCtrl.Line(pt1, pt2));
					main += num;
				}
				return list;
			}
		}

		private Rectangle ZoomRect
		{
			get
			{
				return new Rectangle(new Point()
				{
					X = workingArea.Left + (int)((double)workingArea.Width * (double)zoomLeft),
					Y = workingArea.Top + (int)((double)workingArea.Height * (double)zoomTop)
				}, new Size()
				{
					Width = workingArea.Width * FrameZoom.Width / frameFull.Width,
					Height = workingArea.Height * FrameZoom.Height / frameFull.Height
				});
			}
		}

		private GraphCtrl.GraphFrame FrameZoom
		{
			get
			{
				return new GraphCtrl.GraphFrame()
				{
					Left = frameFull.Left + (int)((double)frameFull.Width * (double)zoomLeft),
					Right = frameFull.Right - (int)((double)frameFull.Width * (double)zoomRight),
					Top = frameFull.Top - (int)((double)frameFull.Height * (double)zoomTop),
					Bottom = frameFull.Bottom + (int)((double)frameFull.Height * (double)zoomBottom)
				};
			}
			set
			{
				zoomLeft = (float)(value.Left - frameFull.Left) / (float)frameFull.Width;
				zoomRight = (float)(frameFull.Right - value.Right) / (float)frameFull.Width;
				zoomTop = (float)(frameFull.Top - value.Top) / (float)frameFull.Height;
				zoomBottom = (float)(value.Bottom - frameFull.Bottom) / (float)frameFull.Height;
				if ((double)zoomLeft < 0.0)
					zoomLeft = 0.0f;
				if ((double)zoomRight < 0.0)
					zoomRight = 0.0f;
				if ((double)zoomTop < 0.0)
					zoomTop = 0.0f;
				if ((double)zoomBottom >= 0.0)
					return;
				zoomBottom = 0.0f;
			}
		}

		private Rectangle GraphWindow
		{
			get
			{
				Rectangle clientRectangle = ClientRectangle;
				if (!frameFit)
				{
					clientRectangle.Inflate(-5 * clientRectangle.Width / 100 - rightOffset - leftOffset, -5 * clientRectangle.Height / 100);
					Point point = new Point(clientRectangle.Location.X, clientRectangle.Location.Y);
					clientRectangle.Location = point;
				}
				return clientRectangle;
			}
		}

		public GraphCtrl()
		{
			InitializeComponent();
			gridX.PropertyChanged += new GraphCtrl.GraphGrid.PropertyChangedEventHandler(Graph_Changed);
			gridY.PropertyChanged += new GraphCtrl.GraphGrid.PropertyChangedEventHandler(Graph_Changed);
			scaleX.PropertyChanged += new GraphCtrl.GraphScale.PropertyChangedEventHandler(Graph_Changed);
			scaleY.PropertyChanged += new GraphCtrl.GraphScale.PropertyChangedEventHandler(Graph_Changed);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			updateData = false;
			ColorBackground = Color.Black;
			ColorFrame = Color.Gray;
			ColorLine = Color.White;
			ColorText = Color.Gray;
			Type = GraphCtrl.eGraphType.Dot;
			History = 100;
			FrameFit = false;
			UpdateRate = 100;
			zoomOption = GraphCtrl.eZoomOption.None;
			zoomLeft = 0.0f;
			zoomRight = 0.0f;
			zoomTop = 0.0f;
			zoomBottom = 0.0f;
			ScaleX.Min = 0;
			ScaleX.Max = 100;
			ScaleX.Show = true;
			ScaleY.Min = 0;
			ScaleY.Max = 100;
			ScaleY.Show = true;
			GridX.Main = 0;
			GridX.Major = 25;
			GridX.Minor = 5;
			GridX.ShowMajor = true;
			GridX.ShowMinor = true;
			GridY.Main = 0;
			GridY.Major = 25;
			GridY.Minor = 5;
			GridY.ShowMajor = true;
			GridY.ShowMinor = true;
			Zoom = false;
			leftOffset = 0;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = (IContainer)new Container();
			ComponentResourceManager resources = new ComponentResourceManager(typeof(GraphCtrl));
			this.tmrRefresh = new Timer(this.components);
			this.labelScaleYmin = new Label();
			this.labelScaleYmax = new Label();
			this.textBoxScaleYmax = new TextBox();
			this.textBoxScaleYmin = new TextBox();
			this.labelSample = new Label();
			this.grphToolTip = new ToolTip(this.components);
			this.checkBoxHand = new CheckBox();
			this.checkBoxZoomOut = new CheckBox();
			this.checkBoxZoomIn = new CheckBox();
			this.checkBoxAutoScale = new CheckBox();
			this.SuspendLayout();
			this.tmrRefresh.Enabled = true;
			this.tmrRefresh.Interval = 25;
			this.tmrRefresh.Tick += new EventHandler(this.tmrRefresh_Tick);
			this.labelScaleYmin.AutoSize = true;
			this.labelScaleYmin.Location = new Point(4, 42);
			this.labelScaleYmin.Name = "labelScaleYmin";
			this.labelScaleYmin.Size = new Size(23, 13);
			this.labelScaleYmin.TabIndex = 1;
			this.labelScaleYmin.Text = "min";
			this.labelScaleYmin.Click += new EventHandler(this.labelScaleY_Click);
			this.labelScaleYmax.AutoSize = true;
			this.labelScaleYmax.Location = new Point(4, 28);
			this.labelScaleYmax.Name = "labelScaleYmax";
			this.labelScaleYmax.Size = new Size(26, 13);
			this.labelScaleYmax.TabIndex = 1;
			this.labelScaleYmax.Text = "max";
			this.labelScaleYmax.Click += new EventHandler(this.labelScaleY_Click);
			this.textBoxScaleYmax.BorderStyle = BorderStyle.None;
			this.textBoxScaleYmax.Location = new Point(36, 28);
			this.textBoxScaleYmax.Name = "textBoxScaleYmax";
			this.textBoxScaleYmax.Size = new Size(47, 13);
			this.textBoxScaleYmax.TabIndex = 2;
			this.textBoxScaleYmax.Text = "max";
			this.textBoxScaleYmax.Visible = false;
			this.textBoxScaleYmax.KeyPress += new KeyPressEventHandler(this.textBoxScaleY_KeyPress);
			this.textBoxScaleYmax.Validated += new EventHandler(this.textBoxScaleY_Validated);
			this.textBoxScaleYmin.BorderStyle = BorderStyle.None;
			this.textBoxScaleYmin.Location = new Point(36, 42);
			this.textBoxScaleYmin.Name = "textBoxScaleYmin";
			this.textBoxScaleYmin.Size = new Size(47, 13);
			this.textBoxScaleYmin.TabIndex = 2;
			this.textBoxScaleYmin.Text = "min";
			this.textBoxScaleYmin.Visible = false;
			this.textBoxScaleYmin.KeyPress += new KeyPressEventHandler(this.textBoxScaleY_KeyPress);
			this.textBoxScaleYmin.Validated += new EventHandler(this.textBoxScaleY_Validated);
			this.labelSample.AutoSize = true;
			this.labelSample.BackColor = Color.Transparent;
			this.labelSample.Location = new Point(50, 128);
			this.labelSample.Name = "labelSample";
			this.labelSample.Size = new Size(42, 13);
			this.labelSample.TabIndex = 3;
			this.labelSample.Text = "Sample";
			this.checkBoxHand.Anchor = AnchorStyles.Right;
			this.checkBoxHand.Appearance = Appearance.Button;
			this.checkBoxHand.Image = (Image)Resources.Move;
			this.checkBoxHand.Location = new Point((int)sbyte.MaxValue, 106);
			this.checkBoxHand.Name = "checkBoxHand";
			this.checkBoxHand.Size = new Size(26, 26);
			this.checkBoxHand.TabIndex = 0;
			this.checkBoxHand.TextAlign = ContentAlignment.MiddleCenter;
			this.grphToolTip.SetToolTip((Control)this.checkBoxHand, "Move:\r\n\r\n-Left Mouse Button Click on the zone you want to move and move the mouse.");
			this.checkBoxHand.UseVisualStyleBackColor = true;
			this.checkBoxHand.CheckedChanged += new EventHandler(this.checkBoxZoom_CheckedChanged);
			this.checkBoxZoomOut.Anchor = AnchorStyles.Right;
			this.checkBoxZoomOut.Appearance = Appearance.Button;
			this.checkBoxZoomOut.Image = (Image)Resources.ZoomOut;
			this.checkBoxZoomOut.Location = new Point((int)sbyte.MaxValue, 80);
			this.checkBoxZoomOut.Name = "checkBoxZoomOut";
			this.checkBoxZoomOut.Size = new Size(26, 26);
			this.checkBoxZoomOut.TabIndex = 0;
			this.checkBoxZoomOut.TextAlign = ContentAlignment.MiddleCenter;
			this.grphToolTip.SetToolTip((Control)this.checkBoxZoomOut, "ZoomOut:\r\n\r\nSelect the button and then each time the graphic is clicked it will zoom out.");
			this.checkBoxZoomOut.UseVisualStyleBackColor = true;
			this.checkBoxZoomOut.CheckedChanged += new EventHandler(this.checkBoxZoom_CheckedChanged);
			this.checkBoxZoomIn.Anchor = AnchorStyles.Right;
			this.checkBoxZoomIn.Appearance = Appearance.Button;
			this.checkBoxZoomIn.Image = (Image)Resources.ZoomIn;
			this.checkBoxZoomIn.Location = new Point((int)sbyte.MaxValue, 54);
			this.checkBoxZoomIn.Name = "checkBoxZoomIn";
			this.checkBoxZoomIn.Size = new Size(26, 26);
			this.checkBoxZoomIn.TabIndex = 0;
			this.checkBoxZoomIn.TextAlign = ContentAlignment.MiddleCenter;
			this.grphToolTip.SetToolTip((Control)this.checkBoxZoomIn, "ZoomIn:\r\n\r\nDraw a rectangle with the Left Mouse button on the graphic zone to zoom");
			this.checkBoxZoomIn.UseVisualStyleBackColor = true;
			this.checkBoxZoomIn.CheckedChanged += new EventHandler(this.checkBoxZoom_CheckedChanged);
			this.checkBoxAutoScale.Anchor = AnchorStyles.Right;
			this.checkBoxAutoScale.Appearance = Appearance.Button;
			this.checkBoxAutoScale.Image = (Image)Resources.Auto;
			this.checkBoxAutoScale.Location = new Point((int)sbyte.MaxValue, 28);
			this.checkBoxAutoScale.Name = "checkBoxAutoScale";
			this.checkBoxAutoScale.Size = new Size(26, 26);
			this.checkBoxAutoScale.TabIndex = 0;
			this.checkBoxAutoScale.TextAlign = ContentAlignment.MiddleCenter;
			this.grphToolTip.SetToolTip((Control)this.checkBoxAutoScale, resources.GetString("checkBoxAutoScale.ToolTip"));
			this.checkBoxAutoScale.UseVisualStyleBackColor = true;
			this.checkBoxAutoScale.Click += new EventHandler(this.checkBoxAutoScale_Click);
			this.checkBoxAutoScale.CheckedChanged += new EventHandler(this.checkBoxZoom_CheckedChanged);
			this.AutoScaleDimensions = new SizeF(6f, 13f);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.Controls.Add((Control)this.labelSample);
			this.Controls.Add((Control)this.textBoxScaleYmin);
			this.Controls.Add((Control)this.textBoxScaleYmax);
			this.Controls.Add((Control)this.labelScaleYmax);
			this.Controls.Add((Control)this.labelScaleYmin);
			this.Controls.Add((Control)this.checkBoxHand);
			this.Controls.Add((Control)this.checkBoxZoomOut);
			this.Controls.Add((Control)this.checkBoxZoomIn);
			this.Controls.Add((Control)this.checkBoxAutoScale);
			this.Name = "GraphCtrl";
			this.Size = new Size(155, 161);
			this.MouseDown += new MouseEventHandler(this.GraphCtrl_MouseDown);
			this.MouseMove += new MouseEventHandler(this.GraphCtrl_MouseMove);
			this.Paint += new PaintEventHandler(this.GraphCtrl_Paint);
			this.MouseUp += new MouseEventHandler(this.GraphCtrl_MouseUp);
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		private List<Point> GetPointsZoom(GraphCtrl.GraphData graphData)
		{
			List<Point> list = new List<Point>();
			if (this.FrameZoom.Height != 0 && this.FrameZoom.Width != 0)
			{
				Point point = new Point();
				for (int index = 0; index < graphData.Value.Count; ++index)
				{
					int num = this.frameFull.Right - graphData.Value.Count + index;
					point.Y = this.workingArea.Bottom - this.workingArea.Height * (graphData.Value[index] - this.FrameZoom.Bottom) / this.FrameZoom.Height;
					point.X = this.workingArea.Right - this.workingArea.Width * (this.FrameZoom.Right - num) / this.FrameZoom.Width;
					list.Add(point);
				}
			}
			return list;
		}

		private List<Point> GetPoints(GraphCtrl.GraphData graphData)
		{
			List<Point> list = new List<Point>();
			if (this.frameFull.Height != 0 && this.frameFull.Width != 0)
			{
				Point point = new Point();
				for (int index = 0; index < graphData.Value.Count; ++index)
				{
					int num = this.frameFull.Right - graphData.Value.Count + index;
					point.Y = this.workingArea.Bottom - this.workingArea.Height * (graphData.Value[index] - this.frameFull.Bottom) / this.frameFull.Height;
					point.X = this.workingArea.Right - this.workingArea.Width * (this.frameFull.Right - num) / (this.frameFull.Width - 1);
					list.Add(point);
				}
			}
			return list;
		}

		private void Graph_Changed()
		{
			this.frameFull.Left = this.scaleX.Min;
			this.frameFull.Right = this.scaleX.Max;
			this.frameFull.Top = this.scaleY.Max;
			this.frameFull.Bottom = this.scaleY.Min;
			this.updateData = true;
		}

		private void GraphCtrl_Paint(object sender, PaintEventArgs e)
		{
			if (this.dataCollection.Count > 0 && this.dataCollection[0].Value.Count > 0)
				this.labelSample.Text = this.dataCollection[0].Value[this.dataCollection[0].Value.Count - 1].ToString();
			if (this.zoomOption == GraphCtrl.eZoomOption.AutoScale && this.dataCollection[0].Value.Count > 1)
			{
				int num1;
				int num2 = num1 = this.dataCollection[0].Value[0];
				for (int index = 1; index < this.dataCollection[0].Value.Count; ++index)
				{
					if (this.dataCollection[0].Value[index] < num2)
						num2 = this.dataCollection[0].Value[index];
					if (this.dataCollection[0].Value[index] > num1)
						num1 = this.dataCollection[0].Value[index];
				}
				if (num1 == num2)
				{
					++num1;
					--num2;
				}
				this.zoomTop = ((float)this.frameFull.Top - ((float)num1 + (float)(num1 - num2) / 10f)) / (float)this.frameFull.Height;
				this.zoomBottom = ((float)num2 - (float)(num1 - num2) / 10f - (float)this.frameFull.Bottom) / (float)this.frameFull.Height;
				if ((double)this.zoomTop < 0.0)
					this.zoomTop = 0.0f;
				if ((double)this.zoomBottom < 0.0)
					this.zoomBottom = 0.0f;
				this.Graph_Changed();
			}
			e.Graphics.FillRectangle(this.brushBackground, this.ClientRectangle);
			this.workingArea = this.GraphWindow;
			if (!this.frameFit)
			{
				if (this.ScaleY.Show)
				{
					this.labelScaleYmax.Text = this.FrameZoom.Top.ToString();
					this.labelScaleYmin.Text = this.FrameZoom.Bottom.ToString();
					Point point = new Point(0, this.GraphWindow.Top - this.labelScaleYmax.Size.Height / 2);
					this.labelScaleYmax.Location = point;
					this.textBoxScaleYmax.Location = point;
					point.Y = this.GraphWindow.Bottom - this.labelScaleYmin.Size.Height / 2;
					this.labelScaleYmin.Location = point;
					this.textBoxScaleYmin.Location = point;
					this.labelScaleYmax.Visible = true;
					this.labelScaleYmin.Visible = true;
					this.leftOffset = Math.Max(this.labelScaleYmax.ClientRectangle.Width / 2, this.labelScaleYmin.ClientRectangle.Width / 2);
				}
				else
				{
					this.labelScaleYmax.Visible = false;
					this.labelScaleYmin.Visible = false;
					this.leftOffset = 0;
				}
				if (!this.ScaleX.Show)
				{ }
			}
			else
			{
				this.labelScaleYmax.Visible = false;
				this.labelScaleYmin.Visible = false;
			}
			this.labelSample.Location = new Point()
			{
				X = this.workingArea.Left + this.workingArea.Width / 2 - this.labelSample.ClientRectangle.Width / 2,
				Y = this.workingArea.Bottom - this.labelSample.ClientRectangle.Height
			};
			e.Graphics.SetClip(this.workingArea);
			e.Graphics.DrawRectangle(this.penFrame, this.workingArea.X, this.workingArea.Y, this.workingArea.Width - 1, this.workingArea.Height - 1);
			this.penFrame.DashStyle = DashStyle.Dot;
			foreach (GraphCtrl.Line line in this.GridLinesX)
				e.Graphics.DrawLine(this.penFrame, line.Pt1, line.Pt2);
			foreach (GraphCtrl.Line line in this.GridLinesY)
				e.Graphics.DrawLine(this.penFrame, line.Pt1, line.Pt2);
			this.penFrame.DashStyle = DashStyle.Solid;
			int num3 = 0;
			foreach (GraphCtrl.GraphData graphData in this.dataCollection)
			{
				if (graphData.GraphType == GraphCtrl.eGraphType.Bar)
					++num3;
			}
			int num4 = 0;
			foreach (GraphCtrl.GraphData graphData in this.dataCollection)
			{
				this.penLine.Color = graphData.Color;
				this.brushPoint = (Brush)new SolidBrush(graphData.Color);
				List<Point> pointsZoom = this.GetPointsZoom(graphData);
				switch (graphData.GraphType)
				{
					case GraphCtrl.eGraphType.Dot:
						using (List<Point>.Enumerator enumerator = pointsZoom.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								Point current = enumerator.Current;
								e.Graphics.FillEllipse(this.brushPoint, current.X, current.Y, 2, 2);
							}
							break;
						}
					case GraphCtrl.eGraphType.Line:
						for (int index = 0; index < pointsZoom.Count - 1; ++index)
							e.Graphics.DrawLine(this.penLine, pointsZoom[index], pointsZoom[index + 1]);
						break;
					case GraphCtrl.eGraphType.Bar:
						if (pointsZoom.Count > 0)
						{
							int y = this.workingArea.Bottom - this.workingArea.Height * -this.FrameZoom.Bottom / this.FrameZoom.Height;
							if (pointsZoom[pointsZoom.Count - 1].Y < y)
							{
								e.Graphics.FillRectangle(this.brushPoint, new Rectangle(this.workingArea.Location.X + num4 * (this.workingArea.Width / num3), pointsZoom[pointsZoom.Count - 1].Y, this.workingArea.Width / num3, y - pointsZoom[pointsZoom.Count - 1].Y));
								break;
							}
							if (pointsZoom[pointsZoom.Count - 1].Y > y)
							{
								e.Graphics.FillRectangle(this.brushPoint, new Rectangle(this.workingArea.Location.X + num4 * (this.workingArea.Width / num3), y, this.workingArea.Width / num3, pointsZoom[pointsZoom.Count - 1].Y - y));
								break;
							}
							this.penLine.Color = Color.Blue;
							e.Graphics.DrawRectangle(this.penLine, (float)(this.workingArea.Location.X + num4 * (this.workingArea.Width / num3)), (float)y, (float)(this.workingArea.Width / num3), 0.1f);
							break;
						}
						break;
				}
				++num4;
			}
			if (this.FrameZoom.Top != this.frameFull.Top || this.FrameZoom.Bottom != this.frameFull.Bottom || (this.FrameZoom.Left != this.frameFull.Left || this.FrameZoom.Right != this.frameFull.Right))
			{
				Point point = new Point(this.workingArea.Location.X + this.workingArea.Width / 50, this.workingArea.Location.Y + this.workingArea.Height / 50);
				this.workingArea.Inflate(-2 * this.workingArea.Width / 5, -2 * this.workingArea.Height / 5);
				this.workingArea.Location = point;
				e.Graphics.SetClip(this.workingArea);
				e.Graphics.FillRectangle(this.brushBackground, this.workingArea);
				e.Graphics.DrawRectangle(this.penLine, this.workingArea.X, this.workingArea.Y, this.workingArea.Width - 1, this.workingArea.Height - 1);
				int num1 = 0;
				foreach (GraphCtrl.GraphData graphData in this.dataCollection)
				{
					this.penLine.Color = graphData.Color;
					this.brushPoint = (Brush)new SolidBrush(graphData.Color);
					List<Point> points = this.GetPoints(graphData);
					switch (graphData.GraphType)
					{
						case GraphCtrl.eGraphType.Dot:
							using (List<Point>.Enumerator enumerator = points.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									Point current = enumerator.Current;
									e.Graphics.FillEllipse(this.brushPoint, current.X, current.Y, 2, 2);
								}
								break;
							}
						case GraphCtrl.eGraphType.Line:
							for (int index = 0; index < points.Count - 1; ++index)
								e.Graphics.DrawLine(this.penLine, points[index], points[index + 1]);
							break;
						case GraphCtrl.eGraphType.Bar:
							if (points.Count > 0)
							{
								e.Graphics.FillRectangle(this.brushPoint, new Rectangle(this.workingArea.Location.X + num1 * (this.workingArea.Width / num3), points[points.Count - 1].Y, this.workingArea.Width / num3, this.workingArea.Location.Y + this.workingArea.Height));
								break;
							}
							break;
					}
					++num1;
				}
				this.penFrame.DashStyle = DashStyle.Dot;
				e.Graphics.DrawRectangle(this.penFrame, this.ZoomRect);
				this.penFrame.DashStyle = DashStyle.Solid;
			}
			if (!this.Zoom)
				return;
			int x = this.ClientRectangle.Right - (this.checkBoxAutoScale.Width + 3);
			Rectangle clientRectangle = this.ClientRectangle;
			int bottom = clientRectangle.Bottom;
			clientRectangle = this.ClientRectangle;
			int top = clientRectangle.Top;
			int y1 = (bottom - top) / 2;
			Point point1 = new Point(x, y1 - this.checkBoxAutoScale.Height * 2);
			this.checkBoxAutoScale.Location = point1;
			point1 = new Point(x, y1 - this.checkBoxAutoScale.Height);
			this.checkBoxZoomIn.Location = point1;
			point1 = new Point(x, y1);
			this.checkBoxZoomOut.Location = point1;
			point1 = new Point(x, y1 + this.checkBoxAutoScale.Height);
			this.checkBoxHand.Location = point1;
		}

		public void AddData(int series, int data, Color color, GraphCtrl.eGraphType graphType)
		{
			if (series >= this.dataCollection.Count)
				this.dataCollection.Add(new GraphCtrl.GraphData(new List<int>(data), color, graphType));
			this.dataCollection[series].Color = color;
			this.dataCollection[series].Value.Add(data);
			if (this.dataCollection[series].Value.Count > this.history)
				this.dataCollection[series].Value.RemoveRange(0, this.dataCollection[series].Value.Count - this.history);
			this.updateData = true;
		}

		public void ClearData(int series)
		{
			this.dataCollection[series].Value.Clear();
		}

		private void tmrRefresh_Tick(object sender, EventArgs e)
		{
			if (this.updateData)
				this.Invalidate();
			this.updateData = false;
		}

		private void GraphCtrl_MouseDown(object sender, MouseEventArgs e)
		{
			switch (this.zoomOption)
			{
				case GraphCtrl.eZoomOption.ZoomIn:
					if (e.Button != MouseButtons.Left)
						break;
					this.mousePos = e.Location;
					break;
				case GraphCtrl.eZoomOption.ZoomOut:
					if (e.Button != MouseButtons.Left)
						break;
					this.FrameZoom = this.FrameZoom * 1.5f;
					this.updateData = true;
					break;
				case GraphCtrl.eZoomOption.Hand:
					if (e.Button != MouseButtons.Left)
						break;
					this.mousePos = e.Location;
					this.frameZoom = this.FrameZoom;
					break;
			}
		}

		private void GraphCtrl_MouseMove(object sender, MouseEventArgs e)
		{
			switch (this.zoomOption)
			{
				case GraphCtrl.eZoomOption.ZoomIn:
					if (e.Button != MouseButtons.Left)
						break;
					this.Refresh();
					Graphics graphics = this.CreateGraphics();
					Point point1 = new Point();
					Point point2 = new Point();
					point1.X = this.mousePos.X < e.X ? this.mousePos.X : e.X;
					point2.X = this.mousePos.X >= e.X ? this.mousePos.X : e.X;
					point1.Y = this.mousePos.Y < e.Y ? this.mousePos.Y : e.Y;
					point2.Y = this.mousePos.Y >= e.Y ? this.mousePos.Y : e.Y;
					Rectangle rect1 = new Rectangle(point1.X, point1.Y, point2.X - point1.X, point2.Y - point1.Y);
					Rectangle rect2 = new Rectangle(point1.X, point1.Y, point2.X - point1.X + 1, point2.Y - point1.Y + 1);
					graphics.SetClip(rect2);
					graphics.DrawRectangle(new Pen(Color.Gray)
					{
						DashStyle = DashStyle.Dot
					}, rect1);
					break;
				case GraphCtrl.eZoomOption.Hand:
					if (e.Button != MouseButtons.Left)
						break;
					GraphCtrl.GraphFrame graphFrame = new GraphCtrl.GraphFrame();
					this.workingArea = this.GraphWindow;
					graphFrame.Left = this.frameZoom.Left - this.frameZoom.Width * (e.X - this.mousePos.X) / this.workingArea.Width;
					graphFrame.Right = this.frameZoom.Right - this.frameZoom.Width * (e.X - this.mousePos.X) / this.workingArea.Width;
					graphFrame.Top = this.frameZoom.Top - this.frameZoom.Height * (this.mousePos.Y - e.Y) / this.workingArea.Height;
					graphFrame.Bottom = this.frameZoom.Bottom - this.frameZoom.Height * (this.mousePos.Y - e.Y) / this.workingArea.Height;
					if (graphFrame.Left < this.frameFull.Left)
					{
						graphFrame.Left = this.frameFull.Left;
						graphFrame.Right = this.frameFull.Left + this.frameZoom.Width;
					}
					if (graphFrame.Right > this.frameFull.Right)
					{
						graphFrame.Right = this.frameFull.Right;
						graphFrame.Left = this.frameFull.Right - this.frameZoom.Width;
					}
					if (graphFrame.Top > this.frameFull.Top)
					{
						graphFrame.Top = this.frameFull.Top;
						graphFrame.Bottom = this.frameFull.Top - this.frameZoom.Height;
					}
					if (graphFrame.Bottom < this.frameFull.Bottom)
					{
						graphFrame.Bottom = this.frameFull.Bottom;
						graphFrame.Top = this.frameFull.Bottom + this.frameZoom.Height;
					}
					this.FrameZoom = graphFrame;
					this.updateData = true;
					break;
			}
		}

		private void GraphCtrl_MouseUp(object sender, MouseEventArgs e)
		{
			switch (this.zoomOption)
			{
				case GraphCtrl.eZoomOption.ZoomIn:
					if (e.Button != MouseButtons.Left)
						break;
					int num1 = this.mousePos.X < e.X ? this.mousePos.X : e.X;
					int num2 = this.mousePos.X >= e.X ? this.mousePos.X : e.X;
					int num3 = this.mousePos.Y < e.Y ? this.mousePos.Y : e.Y;
					int num4 = this.mousePos.Y >= e.Y ? this.mousePos.Y : e.Y;
					if (num1 == num2 || num3 == num4)
					{
						this.FrameZoom *= 0.5f;
					}
					else
					{
						GraphCtrl.GraphFrame graphFrame1 = new GraphCtrl.GraphFrame();
						GraphCtrl.GraphFrame graphFrame2 = new GraphCtrl.GraphFrame();
						this.workingArea = this.GraphWindow;
						GraphCtrl.GraphFrame frameZoom = this.FrameZoom;
						graphFrame2.Left = frameZoom.Left + frameZoom.Width * (num1 - this.workingArea.Left) / this.workingArea.Width;
						graphFrame2.Right = frameZoom.Right - frameZoom.Width * (this.workingArea.Right - num2) / this.workingArea.Width;
						graphFrame2.Top = frameZoom.Top - frameZoom.Height * (num3 - this.workingArea.Top) / this.workingArea.Height;
						graphFrame2.Bottom = frameZoom.Bottom + frameZoom.Height * (this.workingArea.Bottom - num4) / this.workingArea.Height;
						this.FrameZoom = graphFrame2;
					}
					this.updateData = true;
					break;
			}
		}

		private void checkBoxZoom_CheckedChanged(object sender, EventArgs e)
		{
			if (sender == this.checkBoxAutoScale)
			{
				if (this.checkBoxAutoScale.Checked)
				{
					this.checkBoxAutoScale.Image = (Image)Resources.AutoSelected;
					this.checkBoxZoomIn.Image = (Image)Resources.ZoomIn;
					this.checkBoxZoomOut.Image = (Image)Resources.ZoomOut;
					this.checkBoxHand.Image = (Image)Resources.Move;
					this.checkBoxZoomIn.Checked = false;
					this.checkBoxZoomOut.Checked = false;
					this.checkBoxHand.Checked = false;
					this.zoomOption = GraphCtrl.eZoomOption.AutoScale;
				}
				else
				{
					this.checkBoxAutoScale.Image = (Image)Resources.Auto;
					this.zoomOption = GraphCtrl.eZoomOption.None;
				}
			}
			else if (sender == this.checkBoxZoomIn)
			{
				if (this.checkBoxZoomIn.Checked)
				{
					this.checkBoxAutoScale.Image = (Image)Resources.Auto;
					this.checkBoxZoomIn.Image = (Image)Resources.ZoomInSelected;
					this.checkBoxZoomOut.Image = (Image)Resources.ZoomOut;
					this.checkBoxHand.Image = (Image)Resources.Move;
					this.checkBoxAutoScale.Checked = false;
					this.checkBoxZoomOut.Checked = false;
					this.checkBoxHand.Checked = false;
					this.zoomOption = GraphCtrl.eZoomOption.ZoomIn;
				}
				else
				{
					this.checkBoxZoomIn.Image = (Image)Resources.ZoomIn;
					this.zoomOption = GraphCtrl.eZoomOption.None;
				}
			}
			else if (sender == this.checkBoxZoomOut)
			{
				if (this.checkBoxZoomOut.Checked)
				{
					this.checkBoxAutoScale.Image = (Image)Resources.Auto;
					this.checkBoxZoomIn.Image = (Image)Resources.ZoomIn;
					this.checkBoxZoomOut.Image = (Image)Resources.ZoomOutSelected;
					this.checkBoxHand.Image = (Image)Resources.Move;
					this.checkBoxAutoScale.Checked = false;
					this.checkBoxZoomIn.Checked = false;
					this.checkBoxHand.Checked = false;
					this.zoomOption = GraphCtrl.eZoomOption.ZoomOut;
				}
				else
				{
					this.checkBoxZoomOut.Image = (Image)Resources.ZoomOut;
					this.zoomOption = GraphCtrl.eZoomOption.None;
				}
			}
			else if (sender == this.checkBoxHand)
			{
				if (this.checkBoxHand.Checked)
				{
					this.checkBoxAutoScale.Image = (Image)Resources.Auto;
					this.checkBoxZoomIn.Image = (Image)Resources.ZoomIn;
					this.checkBoxZoomOut.Image = (Image)Resources.ZoomOut;
					this.checkBoxHand.Image = (Image)Resources.MoveSelected;
					this.checkBoxAutoScale.Checked = false;
					this.checkBoxZoomIn.Checked = false;
					this.checkBoxZoomOut.Checked = false;
					this.zoomOption = GraphCtrl.eZoomOption.Hand;
				}
				else
				{
					this.checkBoxHand.Image = (Image)Resources.Move;
					this.zoomOption = GraphCtrl.eZoomOption.None;
				}
			}
			this.Refresh();
		}

		private void checkBoxAutoScale_Click(object sender, EventArgs e)
		{
			int tickCount = Environment.TickCount;
			if (tickCount - this.previousClick <= SystemInformation.DoubleClickTime)
			{
				this.checkBoxAutoScale.Image = (Image)Resources.AutoSelected;
				this.checkBoxZoomIn.Image = (Image)Resources.ZoomIn;
				this.checkBoxZoomOut.Image = (Image)Resources.ZoomOut;
				this.checkBoxHand.Image = (Image)Resources.Move;
				this.checkBoxAutoScale.Checked = true;
				this.zoomOption = GraphCtrl.eZoomOption.AutoScale;
			}
			else if (this.checkBoxAutoScale.Checked)
			{
				this.checkBoxAutoScale.Checked = false;
				this.checkBoxAutoScale.Image = (Image)Resources.Auto;
				this.zoomOption = GraphCtrl.eZoomOption.None;
			}
			this.previousClick = tickCount;
		}

		private void labelScaleY_Click(object sender, EventArgs e)
		{
			Label label = (Label)sender;
			TextBox textBox = !label.Equals((object)this.labelScaleYmax) ? this.textBoxScaleYmin : this.textBoxScaleYmax;
			textBox.Text = label.Text;
			textBox.Visible = true;
			textBox.Focus();
			textBox.SelectAll();
		}

		private void textBoxScaleY_Validated(object sender, EventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			textBox.Visible = false;
			GraphCtrl.GraphFrame frameZoom = this.FrameZoom;
			if (textBox == this.textBoxScaleYmax)
				frameZoom.Top = Convert.ToInt32(textBox.Text);
			else if (textBox == this.textBoxScaleYmin)
				frameZoom.Bottom = Convert.ToInt32(textBox.Text);
			this.FrameZoom = frameZoom;
			this.updateData = true;
		}

		private void textBoxScaleY_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (char.IsDigit(e.KeyChar) || (int)e.KeyChar == 8 || (int)e.KeyChar == 45)
				return;
			e.Handled = true;
			if ((int)e.KeyChar != 13)
				return;
			this.textBoxScaleY_Validated(sender, new EventArgs());
		}

		public enum eZoomOption
		{
			None,
			ZoomIn,
			ZoomOut,
			AutoScale,
			Hand,
			FullScale,
		}

		public enum eGraphType
		{
			Dot,
			Line,
			Bar,
		}

		private class GraphGridTypeConverter : TypeConverter
		{
			public override bool GetPropertiesSupported(ITypeDescriptorContext context)
			{
				return true;
			}

			public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
			{
				return TypeDescriptor.GetProperties(typeof(GraphCtrl.GraphGrid));
			}
		}

		[Description("Graph Grid.")]
		[TypeConverter(typeof(GraphCtrl.GraphGridTypeConverter))]
		[Category("Graph")]
		public class GraphGrid
		{
			public delegate void PropertyChangedEventHandler();

			private int main;
			private int major;
			private int minor;
			private bool showMajor;
			private bool showMinor;

			[Description("Main axe value from which the grid will depends.")]
			public int Main
			{
				get
				{
					return main;
				}
				set
				{
					main = value;
					PropertyChanged();
				}
			}

			[Description("Major Grid Unit.")]
			public int Major
			{
				get
				{
					return major;
				}
				set
				{
					major = value;
					PropertyChanged();
				}
			}

			[Description("Minor Grid Unit.")]
			public int Minor
			{
				get
				{
					return minor;
				}
				set
				{
					minor = value;
					PropertyChanged();
				}
			}

			[Description("Show Major Grid.")]
			public bool ShowMajor
			{
				get
				{
					return showMajor;
				}
				set
				{
					showMajor = value;
					PropertyChanged();
				}
			}

			[Description("Show Minor Grid.")]
			public bool ShowMinor
			{
				get
				{
					return showMinor;
				}
				set
				{
					showMinor = value;
					PropertyChanged();
				}
			}

			public event GraphCtrl.GraphGrid.PropertyChangedEventHandler PropertyChanged;

			public override string ToString()
			{
				return main.ToString()
					+ "; " + major.ToString()
					+ "; " + minor.ToString()
					+ "; " + (showMajor ? true : false).ToString()
					+ "; " + (showMinor ? true : false).ToString()
					;
			}
		}

		public class GraphScaleTypeConverter : TypeConverter
		{
			public override bool GetPropertiesSupported(ITypeDescriptorContext context)
			{
				return true;
			}

			public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
			{
				return TypeDescriptor.GetProperties(typeof(GraphCtrl.GraphScale));
			}
		}

		[Category("Graph")]
		[TypeConverter(typeof(GraphCtrl.GraphScaleTypeConverter))]
		[Description("Graph Scale.")]
		public class GraphScale
		{
			public event GraphCtrl.GraphScale.PropertyChangedEventHandler PropertyChanged;

			private int min;
			private int max;
			private bool show;

			[Description("Minimum Scale value.")]
			public int Min
			{
				get
				{
					return min;
				}
				set
				{
					min = value;
					PropertyChanged();
				}
			}

			[Description("Maximum Scale value.")]
			public int Max
			{
				get
				{
					return max;
				}
				set
				{
					max = value;
					PropertyChanged();
				}
			}

			[Description("If true, the scale will be show on the Graph.")]
			public bool Show
			{
				get
				{
					return show;
				}
				set
				{
					show = value;
					PropertyChanged();
				}
			}

			public override string ToString()
			{
				return max
					+ "; " + min
					+ "; " + (show ? true : false).ToString();
			}

			public delegate void PropertyChangedEventHandler();
		}

		private class GraphFrame
		{
			private int left;
			private int right;
			private int top;
			private int bottom;

			public int Left
			{
				get
				{
					return left;
				}
				set
				{
					left = value;
				}
			}

			public int Right
			{
				get
				{
					return right;
				}
				set
				{
					right = value;
				}
			}

			public int Top
			{
				get
				{
					return top;
				}
				set
				{
					top = value;
				}
			}

			public int Bottom
			{
				get
				{
					return bottom;
				}
				set
				{
					bottom = value;
				}
			}

			public int Width
			{
				get
				{
					return Right - Left;
				}
			}

			public int Height
			{
				get
				{
					return Top - Bottom;
				}
			}

			public static GraphCtrl.GraphFrame operator *(GraphCtrl.GraphFrame gf, float times)
			{
				return new GraphCtrl.GraphFrame()
				{
					Right = (int)((double)(gf.Right - gf.Width / 2) + (double)times * (double)gf.Width / 2.0),
					Top = (int)((double)(gf.Top - gf.Height / 2) + (double)times * (double)gf.Height / 2.0),
					Left = (int)((double)gf.Left + (double)gf.Width * (1.0 - (double)times) / 2.0),
					Bottom = (int)((double)gf.Bottom + (double)gf.Height * (1.0 - (double)times) / 2.0)
				};
			}
		}

		private class Line
		{
			private Point pt1 = new Point();
			private Point pt2 = new Point();

			public Point Pt1
			{
				get
				{
					return pt1;
				}
				set
				{
					pt1 = value;
				}
			}

			public Point Pt2
			{
				get
				{
					return pt2;
				}
				set
				{
					pt2 = value;
				}
			}

			public Line(Point pt1, Point pt2)
			{
				Pt1 = pt1;
				Pt2 = pt2;
			}
		}

		public class GraphData
		{
			protected List<int> _value = new List<int>();
			protected int _maxWidth = 15;
			protected GraphCtrl.eGraphType _graphType = GraphCtrl.eGraphType.Line;
			protected Color _color;
			protected Color _colorGradient;
			protected string _text;

			public Color Color
			{
				get
				{
					return _color;
				}
				set
				{
					_color = value;
				}
			}

			public Color ColorGradient
			{
				get
				{
					if (_colorGradient == Color.Empty)
						return _color;
					return _colorGradient;
				}
				set
				{
					_colorGradient = value;
				}
			}

			public string Text
			{
				get
				{
					return _text;
				}
				set
				{
					_text = value;
				}
			}

			public List<int> Value
			{
				get
				{
					return _value;
				}
				set
				{
					_value = value;
				}
			}

			public int MaxWidth
			{
				get
				{
					return _maxWidth;
				}
				set
				{
					_maxWidth = value;
				}
			}

			public GraphCtrl.eGraphType GraphType
			{
				get
				{
					return _graphType;
				}
				set
				{
					_graphType = value;
				}
			}

			public GraphData()
			{
			}

			public GraphData(List<int> value, Color color, GraphCtrl.eGraphType graphType)
			{
				_value = value;
				_color = color;
				_graphType = graphType;
			}

			public GraphData(List<int> value, Color color, Color colorGradient, GraphCtrl.eGraphType graphType)
			{
				_value = value;
				_color = color;
				_colorGradient = colorGradient;
				_graphType = graphType;
			}

			public GraphData(List<int> value, Color color, string text, GraphCtrl.eGraphType graphType)
			{
				_value = value;
				_color = color;
				_text = text;
				_graphType = graphType;
			}

			public GraphData(List<int> value, Color color, Color colorGradient, string text, GraphCtrl.eGraphType graphType)
			{
				_value = value;
				_color = color;
				_text = text;
				_colorGradient = colorGradient;
				_graphType = graphType;
			}
		}

		[Serializable]
		public class GraphDataCollection : CollectionBase
		{
			public GraphCtrl.GraphData this[int index]
			{
				get
				{
					return (GraphCtrl.GraphData)List[index];
				}
				set
				{
					List[index] = (object)value;
				}
			}

			public GraphDataCollection()
			{
			}

			public GraphDataCollection(GraphCtrl.GraphDataCollection value)
			{
				AddRange(value);
			}

			public GraphDataCollection(GraphCtrl.GraphData[] value)
			{
				AddRange(value);
			}

			public int Add(GraphCtrl.GraphData value)
			{
				return List.Add((object)value);
			}

			public void AddRange(GraphCtrl.GraphData[] value)
			{
				for (int index = 0; index < value.Length; ++index)
					Add(value[index]);
			}

			public void AddRange(GraphCtrl.GraphDataCollection value)
			{
				for (int index = 0; index < value.Count; ++index)
					Add(value[index]);
			}

			public bool Contains(GraphCtrl.GraphData value)
			{
				return List.Contains((object)value);
			}

			public void CopyTo(GraphCtrl.GraphData[] array, int index)
			{
				List.CopyTo((Array)array, index);
			}

			public int IndexOf(GraphCtrl.GraphData value)
			{
				return List.IndexOf((object)value);
			}

			public void Insert(int index, GraphCtrl.GraphData value)
			{
				List.Insert(index, (object)value);
			}

			public new GraphCtrl.GraphDataCollection.GraphDataEnumerator GetEnumerator()
			{
				return new GraphCtrl.GraphDataCollection.GraphDataEnumerator(this);
			}

			public void Remove(GraphCtrl.GraphData value)
			{
				--Capacity;
				List.Remove((object)value);
			}

			public class GraphDataEnumerator : IEnumerator
			{
				private IEnumerator baseEnumerator;
				private IEnumerable temp;

				public GraphCtrl.GraphData Current
				{
					get
					{
						return (GraphCtrl.GraphData)baseEnumerator.Current;
					}
				}

				object IEnumerator.Current
				{
					get
					{
						return baseEnumerator.Current;
					}
				}

				public GraphDataEnumerator(GraphCtrl.GraphDataCollection mappings)
				{
					temp = (IEnumerable)mappings;
					baseEnumerator = temp.GetEnumerator();
				}

				public bool MoveNext()
				{
					return baseEnumerator.MoveNext();
				}

				bool IEnumerator.MoveNext()
				{
					return baseEnumerator.MoveNext();
				}

				public void Reset()
				{
					baseEnumerator.Reset();
				}

				void IEnumerator.Reset()
				{
					baseEnumerator.Reset();
				}
			}
		}
	}
}

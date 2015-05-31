using MyCSLib.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MyCSLib.Controls.Graph
{
	public class GraphCtrl : UserControl
	{
		private IContainer components = (IContainer)null;
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
				return this.colorText;
			}
			set
			{
				this.colorText = value;
				this.brushText = (Brush)new SolidBrush(this.colorText);
				this.labelScaleYmax.ForeColor = this.colorText;
				this.textBoxScaleYmax.ForeColor = this.colorText;
				this.labelScaleYmin.ForeColor = this.colorText;
				this.textBoxScaleYmin.ForeColor = this.colorText;
				this.labelSample.ForeColor = this.colorText;
				this.updateData = true;
			}
		}

		[Description("Color of the Graph Background")]
		[Category("Graph")]
		public Color ColorBackground
		{
			get
			{
				return this.colorBackground;
			}
			set
			{
				this.colorBackground = value;
				this.brushBackground = (Brush)new SolidBrush(this.colorBackground);
				this.labelScaleYmax.BackColor = this.colorBackground;
				this.textBoxScaleYmax.BackColor = this.colorBackground;
				this.labelScaleYmin.BackColor = this.colorBackground;
				this.textBoxScaleYmin.BackColor = this.colorBackground;
				this.labelSample.BackColor = Color.Transparent;
				this.updateData = true;
			}
		}

		[Category("Graph")]
		[Description("Color of the Graph Frame")]
		public Color ColorFrame
		{
			get
			{
				return this.colorFrame;
			}
			set
			{
				this.colorFrame = value;
				this.penFrame = new Pen(this.colorFrame);
				this.updateData = true;
			}
		}

		[Category("Graph")]
		[Description("Color of the Graph Line")]
		public Color ColorLine
		{
			get
			{
				return this.colorLine;
			}
			set
			{
				this.colorLine = value;
				this.penLine = new Pen(this.colorLine);
				this.brushPoint = (Brush)new SolidBrush(this.colorLine);
				this.updateData = true;
			}
		}

		[Category("Graph")]
		[DefaultValue(GraphCtrl.eGraphType.Dot)]
		[Description("Graph Type")]
		public GraphCtrl.eGraphType Type
		{
			get
			{
				return this.graphType;
			}
			set
			{
				this.graphType = value;
				this.updateData = true;
			}
		}

		[DefaultValue(100)]
		[Description("How many point to keep in hystory")]
		[Category("Graph")]
		public int History
		{
			get
			{
				return this.history;
			}
			set
			{
				this.history = value;
				foreach (GraphCtrl.GraphData graphData in this.dataCollection)
				{
					if (graphData.Value.Count > this.history)
						graphData.Value.RemoveRange(0, graphData.Value.Count - this.history);
				}
			}
		}

		[DefaultValue(false)]
		[Description("Let the Frame Fit in the Control Window")]
		[Category("Graph")]
		public bool FrameFit
		{
			get
			{
				return this.frameFit;
			}
			set
			{
				this.frameFit = value;
				this.updateData = true;
			}
		}

		[DefaultValue(25)]
		[Description("Update Rate in milliseconds")]
		[Category("Graph")]
		public int UpdateRate
		{
			get
			{
				return this.tmrRefresh.Interval;
			}
			set
			{
				this.tmrRefresh.Interval = value;
			}
		}

		[DefaultValue(false)]
		[Description("Enable the Autoscale features.")]
		[Category("Graph")]
		public bool AutoScale
		{
			get
			{
				return this.zoomOption == GraphCtrl.eZoomOption.AutoScale;
			}
			set
			{
				if (value)
					this.zoomOption = GraphCtrl.eZoomOption.AutoScale;
				else
					this.zoomOption = GraphCtrl.eZoomOption.None;
			}
		}

		[DefaultValue(false)]
		[Description("Sets the graph to full scale.")]
		[Category("Graph")]
		public bool FullScale
		{
			get
			{
				return this.zoomOption == GraphCtrl.eZoomOption.FullScale;
			}
			set
			{
				if (value)
				{
					this.zoomOption = GraphCtrl.eZoomOption.FullScale;
					this.zoomLeft = 0.0f;
					this.zoomRight = 0.0f;
					this.zoomTop = 0.0f;
					this.zoomBottom = 0.0f;
				}
				else
					this.zoomOption = GraphCtrl.eZoomOption.None;
			}
		}

		[Description("Enable the Zoom features.")]
		[DefaultValue(false)]
		[Category("Graph")]
		public bool Zoom
		{
			get
			{
				return this.checkBoxAutoScale.Visible;
			}
			set
			{
				this.checkBoxAutoScale.Visible = value;
				this.checkBoxZoomIn.Visible = value;
				this.checkBoxZoomOut.Visible = value;
				this.checkBoxHand.Visible = value;
				this.rightOffset = !value ? 0 : this.checkBoxAutoScale.ClientRectangle.Width / 2;
				this.updateData = true;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GraphCtrl.GraphScale ScaleX
		{
			get
			{
				return this.scaleX;
			}
			set
			{
				this.scaleX = value;
				this.updateData = true;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GraphCtrl.GraphScale ScaleY
		{
			get
			{
				return this.scaleY;
			}
			set
			{
				this.scaleY = value;
				this.updateData = true;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GraphCtrl.GraphGrid GridX
		{
			get
			{
				return this.gridX;
			}
			set
			{
				this.gridX = value;
				this.updateData = true;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GraphCtrl.GraphGrid GridY
		{
			get
			{
				return this.gridY;
			}
			set
			{
				this.gridY = value;
				this.updateData = true;
			}
		}

		private List<GraphCtrl.Line> GridLinesX
		{
			get
			{
				List<GraphCtrl.Line> list = new List<GraphCtrl.Line>();
				Point pt1 = new Point(0, this.workingArea.Top);
				Point pt2 = new Point(0, this.workingArea.Bottom);
				int num;
				if (this.GridX.ShowMinor)
				{
					num = this.GridX.Minor;
				}
				else
				{
					if (!this.GridX.ShowMajor)
						return list;
					num = this.GridX.Major;
				}
				int main = this.GridX.Main;
				while (main > this.FrameZoom.Left)
					main -= num;
				while (main < this.FrameZoom.Left)
					main += num;
				while (main < this.FrameZoom.Right)
				{
					pt1.X = pt2.X = this.workingArea.Left + this.workingArea.Width * (main - this.FrameZoom.Left) / this.FrameZoom.Width;
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
				Point pt1 = new Point(this.workingArea.Left, 0);
				Point pt2 = new Point(this.workingArea.Right, 0);
				int num;
				if (this.GridY.ShowMinor)
				{
					num = this.GridY.Minor;
				}
				else
				{
					if (!this.GridY.ShowMajor)
						return list;
					num = this.GridY.Major;
				}
				int main = this.GridY.Main;
				while (main > this.FrameZoom.Bottom)
					main -= num;
				while (main < this.FrameZoom.Bottom)
					main += num;
				while (main < this.FrameZoom.Top)
				{
					pt1.Y = pt2.Y = this.workingArea.Bottom - this.workingArea.Height * (main - this.FrameZoom.Bottom) / this.FrameZoom.Height;
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
					X = this.workingArea.Left + (int)((double)this.workingArea.Width * (double)this.zoomLeft),
					Y = this.workingArea.Top + (int)((double)this.workingArea.Height * (double)this.zoomTop)
				}, new Size()
				{
					Width = this.workingArea.Width * this.FrameZoom.Width / this.frameFull.Width,
					Height = this.workingArea.Height * this.FrameZoom.Height / this.frameFull.Height
				});
			}
		}

		private GraphCtrl.GraphFrame FrameZoom
		{
			get
			{
				return new GraphCtrl.GraphFrame()
				{
					Left = this.frameFull.Left + (int)((double)this.frameFull.Width * (double)this.zoomLeft),
					Right = this.frameFull.Right - (int)((double)this.frameFull.Width * (double)this.zoomRight),
					Top = this.frameFull.Top - (int)((double)this.frameFull.Height * (double)this.zoomTop),
					Bottom = this.frameFull.Bottom + (int)((double)this.frameFull.Height * (double)this.zoomBottom)
				};
			}
			set
			{
				this.zoomLeft = (float)(value.Left - this.frameFull.Left) / (float)this.frameFull.Width;
				this.zoomRight = (float)(this.frameFull.Right - value.Right) / (float)this.frameFull.Width;
				this.zoomTop = (float)(this.frameFull.Top - value.Top) / (float)this.frameFull.Height;
				this.zoomBottom = (float)(value.Bottom - this.frameFull.Bottom) / (float)this.frameFull.Height;
				if ((double)this.zoomLeft < 0.0)
					this.zoomLeft = 0.0f;
				if ((double)this.zoomRight < 0.0)
					this.zoomRight = 0.0f;
				if ((double)this.zoomTop < 0.0)
					this.zoomTop = 0.0f;
				if ((double)this.zoomBottom >= 0.0)
					return;
				this.zoomBottom = 0.0f;
			}
		}

		private Rectangle GraphWindow
		{
			get
			{
				Rectangle clientRectangle = this.ClientRectangle;
				if (!this.frameFit)
				{
					clientRectangle.Inflate(-5 * clientRectangle.Width / 100 - this.rightOffset - this.leftOffset, -5 * clientRectangle.Height / 100);
					Point point = new Point(clientRectangle.Location.X, clientRectangle.Location.Y);
					clientRectangle.Location = point;
				}
				return clientRectangle;
			}
		}

		public GraphCtrl()
		{
			this.InitializeComponent();
			this.gridX.PropertyChanged += new GraphCtrl.GraphGrid.PropertyChangedEventHandler(this.Graph_Changed);
			this.gridY.PropertyChanged += new GraphCtrl.GraphGrid.PropertyChangedEventHandler(this.Graph_Changed);
			this.scaleX.PropertyChanged += new GraphCtrl.GraphScale.PropertyChangedEventHandler(this.Graph_Changed);
			this.scaleY.PropertyChanged += new GraphCtrl.GraphScale.PropertyChangedEventHandler(this.Graph_Changed);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.updateData = false;
			this.ColorBackground = Color.Black;
			this.ColorFrame = Color.Gray;
			this.ColorLine = Color.White;
			this.ColorText = Color.Gray;
			this.Type = GraphCtrl.eGraphType.Dot;
			this.History = 100;
			this.FrameFit = false;
			this.UpdateRate = 100;
			this.zoomOption = GraphCtrl.eZoomOption.None;
			this.zoomLeft = 0.0f;
			this.zoomRight = 0.0f;
			this.zoomTop = 0.0f;
			this.zoomBottom = 0.0f;
			this.ScaleX.Min = 0;
			this.ScaleX.Max = 100;
			this.ScaleX.Show = true;
			this.ScaleY.Min = 0;
			this.ScaleY.Max = 100;
			this.ScaleY.Show = true;
			this.GridX.Main = 0;
			this.GridX.Major = 25;
			this.GridX.Minor = 5;
			this.GridX.ShowMajor = true;
			this.GridX.ShowMinor = true;
			this.GridY.Main = 0;
			this.GridY.Major = 25;
			this.GridY.Minor = 5;
			this.GridY.ShowMajor = true;
			this.GridY.ShowMinor = true;
			this.Zoom = false;
			this.leftOffset = 0;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
				this.components.Dispose();
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = (IContainer)new Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(GraphCtrl));
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
			this.grphToolTip.SetToolTip((Control)this.checkBoxAutoScale, componentResourceManager.GetString("checkBoxAutoScale.ToolTip"));
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
			int num3;
			Point current;
			List<Point>.Enumerator enumerator3;
			Point point3;
			if ((this.dataCollection.Count > 0) && (this.dataCollection[0].Value.Count > 0))
			{
				this.labelSample.Text = this.dataCollection[0].Value[this.dataCollection[0].Value.Count - 1].ToString();
			}
			if ((this.zoomOption == eZoomOption.AutoScale) && (this.dataCollection[0].Value.Count > 1))
			{
				int num2;
				int num = num2 = this.dataCollection[0].Value[0];
				num3 = 1;
				while (num3 < this.dataCollection[0].Value.Count)
				{
					if (this.dataCollection[0].Value[num3] < num)
					{
						num = this.dataCollection[0].Value[num3];
					}
					if (this.dataCollection[0].Value[num3] > num2)
					{
						num2 = this.dataCollection[0].Value[num3];
					}
					num3++;
				}
				if (num2 == num)
				{
					num2++;
					num--;
				}
				this.zoomTop = (this.frameFull.Top - (num2 + (((float)(num2 - num)) / 10f))) / ((float)this.frameFull.Height);
				this.zoomBottom = ((num - (((float)(num2 - num)) / 10f)) - this.frameFull.Bottom) / ((float)this.frameFull.Height);
				if (this.zoomTop < 0f)
				{
					this.zoomTop = 0f;
				}
				if (this.zoomBottom < 0f)
				{
					this.zoomBottom = 0f;
				}
				this.Graph_Changed();
			}
			e.Graphics.FillRectangle(this.brushBackground, base.ClientRectangle);
			this.workingArea = this.GraphWindow;
			if (!this.frameFit)
			{
				if (this.ScaleY.Show)
				{
					this.labelScaleYmax.Text = this.FrameZoom.Top.ToString();
					this.labelScaleYmin.Text = this.FrameZoom.Bottom.ToString();
					current = new Point(0, this.GraphWindow.Top - (this.labelScaleYmax.Size.Height / 2));
					this.labelScaleYmax.Location = current;
					this.textBoxScaleYmax.Location = current;
					current.Y = this.GraphWindow.Bottom - (this.labelScaleYmin.Size.Height / 2);
					this.labelScaleYmin.Location = current;
					this.textBoxScaleYmin.Location = current;
					this.labelScaleYmax.Visible = true;
					this.labelScaleYmin.Visible = true;
					this.leftOffset = Math.Max((int)(this.labelScaleYmax.ClientRectangle.Width / 2), (int)(this.labelScaleYmin.ClientRectangle.Width / 2));
				}
				else
				{
					this.labelScaleYmax.Visible = false;
					this.labelScaleYmin.Visible = false;
					this.leftOffset = 0;
				}
				if (this.ScaleX.Show)
				{
				}
			}
			else
			{
				this.labelScaleYmax.Visible = false;
				this.labelScaleYmin.Visible = false;
			}
			current = new Point
			{
				X = (this.workingArea.Left + (this.workingArea.Width / 2)) - (this.labelSample.ClientRectangle.Width / 2),
				Y = this.workingArea.Bottom - this.labelSample.ClientRectangle.Height
			};
			this.labelSample.Location = current;
			e.Graphics.SetClip(this.workingArea);
			e.Graphics.DrawRectangle(this.penFrame, this.workingArea.X, this.workingArea.Y, this.workingArea.Width - 1, this.workingArea.Height - 1);
			this.penFrame.DashStyle = DashStyle.Dot;
			List<Line> gridLinesX = this.GridLinesX;
			foreach (Line line in gridLinesX)
			{
				e.Graphics.DrawLine(this.penFrame, line.Pt1, line.Pt2);
			}
			gridLinesX = this.GridLinesY;
			foreach (Line line in gridLinesX)
			{
				e.Graphics.DrawLine(this.penFrame, line.Pt1, line.Pt2);
			}
			this.penFrame.DashStyle = DashStyle.Solid;
			int num4 = 0;
			foreach (GraphData data in this.dataCollection)
			{
				if (data.GraphType == eGraphType.Bar)
				{
					num4++;
				}
			}
			int num5 = 0;
			foreach (GraphData data in this.dataCollection)
			{
				int num6;
				this.penLine.Color = data.Color;
				this.brushPoint = new SolidBrush(data.Color);
				List<Point> pointsZoom = this.GetPointsZoom(data);
				switch (data.GraphType)
				{
					case eGraphType.Dot:
						using (enumerator3 = pointsZoom.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								current = enumerator3.Current;
								e.Graphics.FillEllipse(this.brushPoint, current.X, current.Y, 2, 2);
							}
						}
						goto Label_090B;

					case eGraphType.Line:
						num3 = 0;
						goto Label_0702;

					case eGraphType.Bar:
						if (pointsZoom.Count > 0)
						{
							num6 = this.workingArea.Bottom - ((this.workingArea.Height * -this.FrameZoom.Bottom) / this.FrameZoom.Height);
							point3 = pointsZoom[pointsZoom.Count - 1];
							if (point3.Y >= num6)
							{
								goto Label_0812;
							}
							point3 = pointsZoom[pointsZoom.Count - 1];
							point3 = pointsZoom[pointsZoom.Count - 1];
							e.Graphics.FillRectangle(this.brushPoint, new Rectangle(this.workingArea.Location.X + (num5 * (this.workingArea.Width / num4)), point3.Y, this.workingArea.Width / num4, num6 - point3.Y));
						}
						goto Label_090B;

					default:
						goto Label_090B;
				}
			Label_06D8:
				e.Graphics.DrawLine(this.penLine, pointsZoom[num3], pointsZoom[num3 + 1]);
				num3++;
			Label_0702:
				if (num3 < (pointsZoom.Count - 1))
				{
					goto Label_06D8;
				}
				goto Label_090B;
			Label_0812:
				point3 = pointsZoom[pointsZoom.Count - 1];
				if (point3.Y > num6)
				{
					point3 = pointsZoom[pointsZoom.Count - 1];
					e.Graphics.FillRectangle(this.brushPoint, new Rectangle(this.workingArea.Location.X + (num5 * (this.workingArea.Width / num4)), num6, this.workingArea.Width / num4, point3.Y - num6));
				}
				else
				{
					this.penLine.Color = Color.Blue;
					e.Graphics.DrawRectangle(this.penLine, (float)(this.workingArea.Location.X + (num5 * (this.workingArea.Width / num4))), (float)num6, (float)(this.workingArea.Width / num4), 0.1f);
				}
			Label_090B:
				num5++;
			}
			if ((((this.FrameZoom.Top != this.frameFull.Top) || (this.FrameZoom.Bottom != this.frameFull.Bottom)) || (this.FrameZoom.Left != this.frameFull.Left)) || (this.FrameZoom.Right != this.frameFull.Right))
			{
				Point point2 = new Point(this.workingArea.Location.X + (this.workingArea.Width / 50), this.workingArea.Location.Y + (this.workingArea.Height / 50));
				this.workingArea.Inflate((-2 * this.workingArea.Width) / 5, (-2 * this.workingArea.Height) / 5);
				this.workingArea.Location = point2;
				e.Graphics.SetClip(this.workingArea);
				e.Graphics.FillRectangle(this.brushBackground, this.workingArea);
				e.Graphics.DrawRectangle(this.penLine, this.workingArea.X, this.workingArea.Y, this.workingArea.Width - 1, this.workingArea.Height - 1);
				num5 = 0;
				foreach (GraphData data in this.dataCollection)
				{
					this.penLine.Color = data.Color;
					this.brushPoint = new SolidBrush(data.Color);
					List<Point> points = this.GetPoints(data);
					switch (data.GraphType)
					{
						case eGraphType.Dot:
							using (enumerator3 = points.GetEnumerator())
							{
								while (enumerator3.MoveNext())
								{
									current = enumerator3.Current;
									e.Graphics.FillEllipse(this.brushPoint, current.X, current.Y, 2, 2);
								}
							}
							goto Label_0C52;

						case eGraphType.Line:
							num3 = 0;
							goto Label_0B9D;

						case eGraphType.Bar:
							if (points.Count > 0)
							{
								point3 = points[points.Count - 1];
								e.Graphics.FillRectangle(this.brushPoint, new Rectangle(this.workingArea.Location.X + (num5 * (this.workingArea.Width / num4)), point3.Y, this.workingArea.Width / num4, this.workingArea.Location.Y + this.workingArea.Height));
							}
							goto Label_0C52;

						default:
							goto Label_0C52;
					}
				Label_0B73:
					e.Graphics.DrawLine(this.penLine, points[num3], points[num3 + 1]);
					num3++;
				Label_0B9D:
					if (num3 < (points.Count - 1))
					{
						goto Label_0B73;
					}
				Label_0C52:
					num5++;
				}
				this.penFrame.DashStyle = DashStyle.Dot;
				e.Graphics.DrawRectangle(this.penFrame, this.ZoomRect);
				this.penFrame.DashStyle = DashStyle.Solid;
			}
			if (this.Zoom)
			{
				int x = base.ClientRectangle.Right - (this.checkBoxAutoScale.Width + 3);
				int y = (base.ClientRectangle.Bottom - base.ClientRectangle.Top) / 2;
				current = new Point(x, y - (this.checkBoxAutoScale.Height * 2));
				this.checkBoxAutoScale.Location = current;
				current = new Point(x, y - this.checkBoxAutoScale.Height);
				this.checkBoxZoomIn.Location = current;
				current = new Point(x, y);
				this.checkBoxZoomOut.Location = current;
				current = new Point(x, y + this.checkBoxAutoScale.Height);
				this.checkBoxHand.Location = current;
			}
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
			if ((int)e.KeyChar == 13)
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
		[Category("Graph")]
		[TypeConverter(typeof(GraphCtrl.GraphGridTypeConverter))]
		public class GraphGrid
		{
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
					return this.main;
				}
				set
				{
					this.main = value;
					this.PropertyChanged();
				}
			}

			[Description("Major Grid Unit.")]
			public int Major
			{
				get
				{
					return this.major;
				}
				set
				{
					this.major = value;
					this.PropertyChanged();
				}
			}

			[Description("Minor Grid Unit.")]
			public int Minor
			{
				get
				{
					return this.minor;
				}
				set
				{
					this.minor = value;
					this.PropertyChanged();
				}
			}

			[Description("Show Major Grid.")]
			public bool ShowMajor
			{
				get
				{
					return this.showMajor;
				}
				set
				{
					this.showMajor = value;
					this.PropertyChanged();
				}
			}

			[Description("Show Minor Grid.")]
			public bool ShowMinor
			{
				get
				{
					return this.showMinor;
				}
				set
				{
					this.showMinor = value;
					this.PropertyChanged();
				}
			}

			public event GraphCtrl.GraphGrid.PropertyChangedEventHandler PropertyChanged;

			public override string ToString()
			{
				return main.ToString()
					+ "; " + major.ToString()
					+ "; " + minor.ToString()
					+ "; " + (showMajor ? true : false).ToString()
					+ "; " + (showMinor ? true : false).ToString();
			}

			public delegate void PropertyChangedEventHandler();
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
		[Description("Graph Scale.")]
		[TypeConverter(typeof(GraphCtrl.GraphScaleTypeConverter))]
		public class GraphScale
		{
			private int min;
			private int max;
			private bool show;

			[Description("Minimum Scale value.")]
			public int Min
			{
				get
				{
					return this.min;
				}
				set
				{
					this.min = value;
					this.PropertyChanged();
				}
			}

			[Description("Maximum Scale value.")]
			public int Max
			{
				get
				{
					return this.max;
				}
				set
				{
					this.max = value;
					this.PropertyChanged();
				}
			}

			[Description("If true, the scale will be show on the Graph.")]
			public bool Show
			{
				get
				{
					return this.show;
				}
				set
				{
					this.show = value;
					this.PropertyChanged();
				}
			}

			public event GraphCtrl.GraphScale.PropertyChangedEventHandler PropertyChanged;

			public override string ToString()
			{
				return max.ToString()
					+ "; " + min.ToString()
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
					return this.left;
				}
				set
				{
					this.left = value;
				}
			}

			public int Right
			{
				get
				{
					return this.right;
				}
				set
				{
					this.right = value;
				}
			}

			public int Top
			{
				get
				{
					return this.top;
				}
				set
				{
					this.top = value;
				}
			}

			public int Bottom
			{
				get
				{
					return this.bottom;
				}
				set
				{
					this.bottom = value;
				}
			}

			public int Width
			{
				get
				{
					return this.Right - this.Left;
				}
			}

			public int Height
			{
				get
				{
					return this.Top - this.Bottom;
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
					return this.pt1;
				}
				set
				{
					this.pt1 = value;
				}
			}

			public Point Pt2
			{
				get
				{
					return this.pt2;
				}
				set
				{
					this.pt2 = value;
				}
			}

			public Line(Point pt1, Point pt2)
			{
				this.pt1 = pt1;
				this.pt2 = pt2;
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
					return this._color;
				}
				set
				{
					this._color = value;
				}
			}

			public Color ColorGradient
			{
				get
				{
					if (this._colorGradient == Color.Empty)
						return this._color;
					return this._colorGradient;
				}
				set
				{
					this._colorGradient = value;
				}
			}

			public string Text
			{
				get
				{
					return this._text;
				}
				set
				{
					this._text = value;
				}
			}

			public List<int> Value
			{
				get
				{
					return this._value;
				}
				set
				{
					this._value = value;
				}
			}

			public int MaxWidth
			{
				get
				{
					return this._maxWidth;
				}
				set
				{
					this._maxWidth = value;
				}
			}

			public GraphCtrl.eGraphType GraphType
			{
				get
				{
					return this._graphType;
				}
				set
				{
					this._graphType = value;
				}
			}

			public GraphData()
			{
			}

			public GraphData(List<int> value, Color color, GraphCtrl.eGraphType graphType)
			{
				this._value = value;
				this._color = color;
				this._graphType = graphType;
			}

			public GraphData(List<int> value, Color color, Color colorGradient, GraphCtrl.eGraphType graphType)
			{
				this._value = value;
				this._color = color;
				this._colorGradient = colorGradient;
				this._graphType = graphType;
			}

			public GraphData(List<int> value, Color color, string text, GraphCtrl.eGraphType graphType)
			{
				this._value = value;
				this._color = color;
				this._text = text;
				this._graphType = graphType;
			}

			public GraphData(List<int> value, Color color, Color colorGradient, string text, GraphCtrl.eGraphType graphType)
			{
				this._value = value;
				this._color = color;
				this._text = text;
				this._colorGradient = colorGradient;
				this._graphType = graphType;
			}
		}

		[Serializable]
		public class GraphDataCollection : CollectionBase
		{
			public GraphCtrl.GraphData this[int index]
			{
				get
				{
					return (GraphCtrl.GraphData)this.List[index];
				}
				set
				{
					this.List[index] = (object)value;
				}
			}

			public GraphDataCollection()
			{
			}

			public GraphDataCollection(GraphCtrl.GraphDataCollection value)
			{
				this.AddRange(value);
			}

			public GraphDataCollection(GraphCtrl.GraphData[] value)
			{
				this.AddRange(value);
			}

			public int Add(GraphCtrl.GraphData value)
			{
				return this.List.Add((object)value);
			}

			public void AddRange(GraphCtrl.GraphData[] value)
			{
				for (int index = 0; index < value.Length; ++index)
					this.Add(value[index]);
			}

			public void AddRange(GraphCtrl.GraphDataCollection value)
			{
				for (int index = 0; index < value.Count; ++index)
					this.Add(value[index]);
			}

			public bool Contains(GraphCtrl.GraphData value)
			{
				return this.List.Contains((object)value);
			}

			public void CopyTo(GraphCtrl.GraphData[] array, int index)
			{
				this.List.CopyTo((Array)array, index);
			}

			public int IndexOf(GraphCtrl.GraphData value)
			{
				return this.List.IndexOf((object)value);
			}

			public void Insert(int index, GraphCtrl.GraphData value)
			{
				this.List.Insert(index, (object)value);
			}

			public new GraphCtrl.GraphDataCollection.GraphDataEnumerator GetEnumerator()
			{
				return new GraphCtrl.GraphDataCollection.GraphDataEnumerator(this);
			}

			public void Remove(GraphCtrl.GraphData value)
			{
				--this.Capacity;
				this.List.Remove((object)value);
			}

			public class GraphDataEnumerator : IEnumerator
			{
				private IEnumerator baseEnumerator;
				private IEnumerable temp;

				public GraphCtrl.GraphData Current
				{
					get
					{
						return (GraphCtrl.GraphData)this.baseEnumerator.Current;
					}
				}

				object IEnumerator.Current
				{
					get
					{
						return this.baseEnumerator.Current;
					}
				}

				public GraphDataEnumerator(GraphCtrl.GraphDataCollection mappings)
				{
					this.temp = (IEnumerable)mappings;
					this.baseEnumerator = this.temp.GetEnumerator();
				}

				public bool MoveNext()
				{
					return this.baseEnumerator.MoveNext();
				}

				bool IEnumerator.MoveNext()
				{
					return this.baseEnumerator.MoveNext();
				}

				public void Reset()
				{
					this.baseEnumerator.Reset();
				}

				void IEnumerator.Reset()
				{
					this.baseEnumerator.Reset();
				}
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using ZedGraph;

namespace SemtechLib.Devices.SX1276.UI.Controls
{
	public class RssiGraphControl : UserControl
	{
		private Color[] graphDataColors = new Color[13]
		{
			Color.Red,
			Color.Aqua,
			Color.Yellow,
			Color.Blue,
			Color.Orange,
			Color.Cyan,
			Color.Brown,
			Color.Lavender,
			Color.Ivory,
			Color.Indigo,
			Color.HotPink,
			Color.LightPink,
			Color.Green
		};
		private RollingPointPairList[] graphCurveListPoints = new RollingPointPairList[13];
		private const float TitleSize = 14f;
		private const float LegendSize = 12f;
		private const float GraphTitleSize = 14f;
		private const float AxisTitleSize = 6f;
		private const float AxisScaleSize = 6f;
		private const float BarGraphLabelSize = 5f;
		private const float MasterPaneBaseDimension = 8f;
		private const float GraphPaneBaseDimension = 4f;
		private MasterPane masterPane;
		private IContainer components;
		private ZedGraphControl graph;

		public PaneList PaneList
		{
			get
			{
				return masterPane.PaneList;
			}
		}

		public RssiGraphControl()
		{
			InitializeComponent();

			graph.ContextMenuBuilder += new ZedGraphControl.ContextMenuBuilderEventHandler(graph_ContextMenuBuilder);
			graph.IsShowPointValues = true;
			for (int index = 0; index < graphCurveListPoints.Length; ++index)
				graphCurveListPoints[index] = new RollingPointPairList(1200);
			MasterPaneInit();
			MasterPaneAddGraph(CreateLineGraph(3, "", "Samples", "Power [dBm]", true, true, false, true, false, false, true, false));
			MasterPaneLayout();
		}

		public void MasterPaneInit()
		{
			masterPane = graph.MasterPane;
			masterPane.PaneList.Clear();
			masterPane.Title.Text = "RSSI";
			masterPane.Title.IsVisible = false;
			masterPane.Title.FontSpec.Size = 14f;
			masterPane.Title.FontSpec.FontColor = Color.Gray;
			masterPane.Title.FontSpec.IsBold = true;
			masterPane.Fill = new Fill(Color.Black);
			masterPane.Margin.All = 0.0f;
			masterPane.InnerPaneGap = 0.0f;
			masterPane.Legend.IsVisible = false;
			masterPane.Legend.Position = LegendPos.TopCenter;
			masterPane.Legend.Fill = new Fill(Color.Black);
			masterPane.Legend.FontSpec.FontColor = Color.Gray;
			masterPane.Legend.FontSpec.Size = 12f;
			masterPane.Legend.Border.Color = Color.Gray;
			masterPane.BaseDimension = 10f;
		}

		public void MasterPaneAddGraph(GraphPane graphPane)
		{
			masterPane.Add(graphPane);
		}

		public void MasterPaneLayout()
		{
			using (Graphics graphics = graph.CreateGraphics())
			{
				masterPane.SetLayout(graphics, PaneLayout.SquareColPreferred);
				masterPane.AxisChange(graphics);
			}
		}

		public GraphPane CreateLineGraph(int nSeries, string title, string xAxisTitle, string yAxisTitle, bool xAxisTitleVisible, bool yAxisTitleVisible, bool xScaleVisible, bool yScaleVisible, bool xMajorGridVisible, bool xIsBetweenLabels, bool yMajorGridVisible, bool yIsBetweenLabels)
		{
			GraphPane graphPane = new GraphPane();
			SetupGraphPane(graphPane, title, xAxisTitle, yAxisTitle, xAxisTitleVisible, yAxisTitleVisible, xScaleVisible, yScaleVisible, xMajorGridVisible, xIsBetweenLabels, yMajorGridVisible, yIsBetweenLabels);
			RollingPointPairList[] rollingPointPairListArray = new RollingPointPairList[nSeries];
			for (int index = 0; index < rollingPointPairListArray.Length; ++index)
			{
				rollingPointPairListArray[index] = new RollingPointPairList(1200);
				graphPane.AddCurve("", (IPointList)rollingPointPairListArray[index], graphDataColors[index], SymbolType.None);
			}
			return graphPane;
		}

		private void SetupGraphPane(GraphPane graphPane, string title, string xAxisTitle, string yAxisTitle, bool xAxisTitleVisible, bool yAxisTitleVisible, bool xScaleVisible, bool yScaleVisible, bool xMajorGridVisible, bool xIsBetweenLabels, bool yMajorGridVisible, bool yIsBetweenLabels)
		{
			graphPane.Title.Text = title;
			graphPane.Title.FontSpec.Size = 14f;
			graphPane.Title.FontSpec.FontColor = Color.Gray;
			graphPane.Title.FontSpec.IsBold = true;
			graphPane.Fill = new Fill(Color.Black);
			graphPane.Chart.Fill = new Fill(Color.Black);
			graphPane.Chart.Border.Color = Color.Gray;
			graphPane.Legend.IsVisible = false;
			graphPane.XAxis.Title.IsVisible = xAxisTitleVisible;
			graphPane.XAxis.Title.Text = xAxisTitle;
			graphPane.XAxis.Title.FontSpec.Size = 6f;
			graphPane.XAxis.Title.FontSpec.FontColor = Color.Gray;
			graphPane.XAxis.Color = Color.Gray;
			graphPane.XAxis.Scale.IsVisible = xScaleVisible;
			graphPane.XAxis.Scale.FontSpec.Size = 6f;
			graphPane.XAxis.Scale.FontSpec.FontColor = Color.Gray;
			graphPane.XAxis.MajorGrid.IsVisible = xMajorGridVisible;
			graphPane.XAxis.MajorGrid.Color = Color.LightGray;
			graphPane.XAxis.MinorGrid.Color = Color.LightGray;
			graphPane.XAxis.MajorTic.IsOpposite = false;
			graphPane.XAxis.MajorTic.IsBetweenLabels = xIsBetweenLabels;
			graphPane.XAxis.MajorTic.Color = Color.LightGray;
			graphPane.XAxis.MinorTic.IsOpposite = false;
			graphPane.XAxis.MinorTic.Color = Color.LightGray;
			graphPane.YAxis.Title.IsVisible = yAxisTitleVisible;
			graphPane.YAxis.Title.Text = yAxisTitle;
			graphPane.YAxis.Title.FontSpec.Size = 6f;
			graphPane.YAxis.Title.FontSpec.FontColor = Color.Gray;
			graphPane.YAxis.Color = Color.Gray;
			graphPane.YAxis.Scale.IsVisible = yScaleVisible;
			graphPane.YAxis.Scale.FontSpec.Size = 6f;
			graphPane.YAxis.Scale.FontSpec.FontColor = Color.Gray;
			graphPane.YAxis.Scale.Min = -150.0;
			graphPane.YAxis.Scale.Max = 0.0;
			graphPane.YAxis.MajorGrid.IsVisible = yMajorGridVisible;
			graphPane.YAxis.MinorGrid.IsVisible = yMajorGridVisible;
			graphPane.YAxis.MajorTic.IsAllTics = false;
			graphPane.YAxis.MajorGrid.Color = Color.LightGray;
			graphPane.YAxis.MinorGrid.Color = Color.LightGray;
			graphPane.YAxis.MajorTic.IsOpposite = false;
			graphPane.YAxis.MajorTic.IsBetweenLabels = yIsBetweenLabels;
			graphPane.YAxis.MajorTic.Color = Color.LightGray;
			graphPane.YAxis.MinorTic.IsOpposite = false;
			graphPane.YAxis.MinorTic.Color = Color.LightGray;
			graphPane.BaseDimension = 3f;
		}

		public void ClearAllGraphData()
		{
			try
			{
				for (int index = 0; index < masterPane.PaneList.Count; ++index)
				{
					if (masterPane.PaneList[index].CurveList.Count <= 0)
						return;
					foreach (CurveItem curveItem in (List<CurveItem>)masterPane.PaneList[index].CurveList)
					{
						if (curveItem is LineItem)
							(curveItem.Points as IPointListEdit).Clear();
						if (curveItem is BarItem)
							curveItem.Points[0].Y = 0.0;
					}
					masterPane.PaneList[index].AxisChange();
				}
				graph.Invalidate();
			}
			catch
			{
				throw new Exception("While Clearing data");
			}
		}

		public void AddLineGraphPoint(int serie, double time, double value)
		{
			graphCurveListPoints[serie].Add(time, value);
		}

		public void UpdateLineGraph(double time, double value)
		{
			UpdateLineGraph(0, time, value);
		}

		public void UpdateLineGraph(int serie, double time, double value)
		{
			try
			{
				GraphPane graphPane = masterPane.PaneList[0];
				if (graphPane.CurveList.Count <= 0)
					return;
				LineItem lineItem = graphPane.CurveList[serie] as LineItem;
				if (lineItem == null)
					return;
				IPointListEdit pointListEdit = lineItem.Points as IPointListEdit;
				if (pointListEdit == null)
					return;
				pointListEdit.Add(time, value);
				if (time > graphPane.XAxis.Scale.Max || graphPane.XAxis.Scale.Max > 10.0)
				{
					graphPane.XAxis.Scale.Max = time;
					graphPane.XAxis.Scale.Min = graphPane.XAxis.Scale.Max - 10.0;
				}
				graphPane.AxisChange();
				graph.Invalidate();
			}
			catch
			{
				throw new Exception("While updating data graph");
			}
		}

		private void graph_ContextMenuBuilder(ZedGraphControl control, ContextMenuStrip menuStrip, Point mousePt, ZedGraphControl.ContextMenuObjectState objState)
		{
			foreach (ToolStripMenuItem toolStripMenuItem in (ArrangedElementCollection)menuStrip.Items)
			{
				if ((string)toolStripMenuItem.Tag == "set_default")
				{
					menuStrip.Items.Remove((ToolStripItem)toolStripMenuItem);
					break;
				}
			}
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
			this.graph = new ZedGraphControl();
			this.SuspendLayout();
			this.graph.Dock = DockStyle.Fill;
			this.graph.Location = new Point(0, 0);
			this.graph.Margin = new Padding(0);
			this.graph.Name = "graph";
			this.graph.ScrollGrace = 0.0;
			this.graph.ScrollMaxX = 0.0;
			this.graph.ScrollMaxY = 0.0;
			this.graph.ScrollMaxY2 = 0.0;
			this.graph.ScrollMinX = 0.0;
			this.graph.ScrollMinY = 0.0;
			this.graph.ScrollMinY2 = 0.0;
			this.graph.Size = new Size(205, 133);
			this.graph.TabIndex = 3;
			this.AutoScaleDimensions = new SizeF(6f, 13f);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add((Control)this.graph);
			this.Name = "GraphDisplay";
			this.Size = new Size(205, 133);
			this.ResumeLayout(false);
		}
	}
}
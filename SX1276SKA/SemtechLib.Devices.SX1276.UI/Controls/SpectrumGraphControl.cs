// Decompiled with JetBrains decompiler
// Type: SemtechLib.Devices.SX1276.UI.Controls.SpectrumGraphControl
// Assembly: SemtechLib.Devices.SX1276.UI, Version=2.0.0.0, Culture=neutral, PublicKeyToken=a3cee8af388a4f11
// MVID: 2B98C92B-3345-4D34-A253-90690D8C71AF
// Assembly location: C:\Tools\Semtech\SX1276SKA\SemtechLib.Devices.SX1276.UI.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using ZedGraph;

namespace SemtechLib.Devices.SX1276.UI.Controls
{
  public class SpectrumGraphControl : UserControl
  {
    private RollingPointPairList[] graphCurveListPoints = new RollingPointPairList[13];
    private const float TitleSize = 14f;
    private const float LegendSize = 12f;
    private const float GraphTitleSize = 14f;
    private const float AxisTitleSize = 6f;
    private const float AxisScaleSize = 6f;
    private const float BarGraphLabelSize = 5f;
    private const float MasterPaneBaseDimension = 8f;
    private const float GraphPaneBaseDimension = 4f;
    private IContainer components;
    private ZedGraphControl graph;
    private MasterPane masterPane;

    public PaneList PaneList
    {
      get
      {
        return this.masterPane.PaneList;
      }
    }

    public SpectrumGraphControl()
    {
      this.InitializeComponent();
      this.graph.ContextMenuBuilder += new ZedGraphControl.ContextMenuBuilderEventHandler(this.graph_ContextMenuBuilder);
      this.graph.IsShowPointValues = true;
      for (int index = 0; index < this.graphCurveListPoints.Length; ++index)
        this.graphCurveListPoints[index] = new RollingPointPairList(1200);
      this.MasterPaneInit();
      this.MasterPaneAddGraph(this.CreateLineGraph(1, "", "Frequency [MHz]", "Power [dBm]", true, true, true, true, false, false, true, false));
      this.MasterPaneLayout();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new Container();
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
      this.AutoScaleMode = AutoScaleMode.Font;
      this.Controls.Add((Control) this.graph);
      this.Name = "GraphDisplay";
      this.Size = new Size(205, 133);
      this.ResumeLayout(false);
    }

    public void MasterPaneInit()
    {
      this.masterPane = this.graph.MasterPane;
      this.masterPane.PaneList.Clear();
      this.masterPane.Title.Text = "Spectrum Analyser";
      this.masterPane.Title.IsVisible = false;
      this.masterPane.Title.FontSpec.Size = 14f;
      this.masterPane.Title.FontSpec.FontColor = Color.Gray;
      this.masterPane.Title.FontSpec.IsBold = true;
      this.masterPane.Fill = new Fill(Color.Black);
      this.masterPane.Margin.All = 0.0f;
      this.masterPane.InnerPaneGap = 0.0f;
      this.masterPane.Legend.IsVisible = false;
      this.masterPane.Legend.Position = LegendPos.TopCenter;
      this.masterPane.Legend.Fill = new Fill(Color.Black);
      this.masterPane.Legend.FontSpec.FontColor = Color.Gray;
      this.masterPane.Legend.FontSpec.Size = 12f;
      this.masterPane.Legend.Border.Color = Color.Gray;
      this.masterPane.BaseDimension = 10f;
    }

    public void MasterPaneAddGraph(GraphPane graphPane)
    {
      this.masterPane.Add(graphPane);
    }

    public void MasterPaneLayout()
    {
      using (Graphics graphics = this.graph.CreateGraphics())
      {
        this.masterPane.SetLayout(graphics, PaneLayout.SquareColPreferred);
        this.masterPane.AxisChange(graphics);
      }
    }

    public GraphPane CreateLineGraph(int nSeries, string title, string xAxisTitle, string yAxisTitle, bool xAxisTitleVisible, bool yAxisTitleVisible, bool xScaleVisible, bool yScaleVisible, bool xMajorGridVisible, bool xIsBetweenLabels, bool yMajorGridVisible, bool yIsBetweenLabels)
    {
      GraphPane graphPane = new GraphPane();
      this.SetupGraphPane(graphPane, title, xAxisTitle, yAxisTitle, xAxisTitleVisible, yAxisTitleVisible, xScaleVisible, yScaleVisible, xMajorGridVisible, xIsBetweenLabels, yMajorGridVisible, yIsBetweenLabels);
      PointPairList pointPairList = new PointPairList();
      graphPane.AddCurve("", (IPointList) pointPairList, Color.Yellow, SymbolType.None);
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
      graphPane.XAxis.Title.IsOmitMag = true;
      graphPane.XAxis.Color = Color.Gray;
      graphPane.XAxis.Scale.IsVisible = xScaleVisible;
      graphPane.XAxis.Scale.FontSpec.Size = 6f;
      graphPane.XAxis.Scale.FontSpec.FontColor = Color.Gray;
      graphPane.XAxis.Scale.Min = 914000000.0;
      graphPane.XAxis.Scale.Max = 915000000.0;
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
      graphPane.YAxis.Title.IsOmitMag = true;
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
        for (int index = 0; index < this.masterPane.PaneList.Count; ++index)
        {
          if (this.masterPane.PaneList[index].CurveList.Count <= 0)
            return;
          foreach (CurveItem curveItem in (List<CurveItem>) this.masterPane.PaneList[index].CurveList)
          {
            if (curveItem is LineItem)
              (curveItem.Points as IPointListEdit).Clear();
            if (curveItem is BarItem)
              curveItem.Points[0].Y = 0.0;
          }
          this.masterPane.PaneList[index].AxisChange();
        }
        this.graph.Invalidate();
      }
      catch
      {
        throw new Exception("While Clearing data");
      }
    }

    public void AddLineGraphPoint(int serie, double freq, double value)
    {
      this.graphCurveListPoints[serie].Add(freq, value);
    }

    public void UpdateLineGraph(double xValue, double yValue)
    {
      try
      {
        GraphPane graphPane = this.masterPane.PaneList[0];
        if (graphPane.CurveList.Count <= 0)
          return;
        LineItem lineItem = graphPane.CurveList[0] as LineItem;
        if (lineItem == null)
          return;
        IPointListEdit pointListEdit = lineItem.Points as IPointListEdit;
        if (pointListEdit == null)
          return;
        pointListEdit.Add(xValue, yValue);
        graphPane.AxisChange();
        this.graph.Invalidate();
      }
      catch
      {
        throw new Exception("While updating data graph");
      }
    }

    public void UpdateLineGraph(int id, double yValue)
    {
      try
      {
        GraphPane graphPane = this.masterPane.PaneList[0];
        if (graphPane.CurveList.Count <= 0)
          return;
        LineItem lineItem = graphPane.CurveList[0] as LineItem;
        if (lineItem == null)
          return;
        IPointListEdit pointListEdit = lineItem.Points as IPointListEdit;
        if (pointListEdit == null)
          return;
        pointListEdit[id].Y = yValue;
        graphPane.AxisChange();
        this.graph.Invalidate();
      }
      catch
      {
        throw new Exception("While updating data graph");
      }
    }

    private void graph_ContextMenuBuilder(ZedGraphControl control, ContextMenuStrip menuStrip, Point mousePt, ZedGraphControl.ContextMenuObjectState objState)
    {
      foreach (ToolStripMenuItem toolStripMenuItem in (ArrangedElementCollection) menuStrip.Items)
      {
        if ((string) toolStripMenuItem.Tag == "set_default")
        {
          menuStrip.Items.Remove((ToolStripItem) toolStripMenuItem);
          break;
        }
      }
    }
  }
}

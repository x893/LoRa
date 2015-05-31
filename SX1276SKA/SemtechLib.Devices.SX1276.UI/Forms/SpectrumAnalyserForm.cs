using SemtechLib.Controls;
using SemtechLib.Devices.Common.Interfaces;
using SemtechLib.Devices.SX1276;
using SemtechLib.Devices.SX1276.Enumerations;
using SemtechLib.Devices.SX1276.UI.Controls;
using SemtechLib.General;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

namespace SemtechLib.Devices.SX1276.UI.Forms
{
	public class SpectrumAnalyserForm : Form
	{
		private Decimal rxBw = new Decimal(10417);
		private IContainer components;
		private Panel panel1;
		private Panel panel2;
		private SpectrumGraphControl graph;
		private NumericUpDownEx nudFreqCenter;
		private NumericUpDownEx nudFreqSpan;
		private NumericUpDownEx nudChanBw;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private ComboBox cBoxLanGain;
		private System.Windows.Forms.Label label7;
		private PointPairList points;
		private ApplicationSettings appSettings;
		private SX1276 device;

		private Decimal FrequencyRf
		{
			get
			{
				return nudFreqCenter.Value;
			}
			set
			{
				try
				{
					nudFreqCenter.ValueChanged -= new EventHandler(nudFreqCenter_ValueChanged);
					nudFreqCenter.Value = (Decimal)(uint)Math.Round(value / device.FrequencyStep, MidpointRounding.AwayFromZero) * device.FrequencyStep;
				}
				catch (Exception)
				{
					nudFreqCenter.BackColor = ControlPaint.LightLight(Color.Red);
				}
				finally
				{
					nudFreqCenter.ValueChanged += new EventHandler(nudFreqCenter_ValueChanged);
				}
			}
		}

		private Decimal FrequencySpan
		{
			get
			{
				return nudFreqSpan.Value;
			}
			set
			{
				try
				{
					nudFreqSpan.ValueChanged -= new EventHandler(nudFreqSpan_ValueChanged);
					nudFreqSpan.Value = (Decimal)(uint)Math.Round(value / device.FrequencyStep, MidpointRounding.AwayFromZero) * device.FrequencyStep;
				}
				catch (Exception)
				{
					nudFreqSpan.BackColor = ControlPaint.LightLight(Color.Red);
				}
				finally
				{
					nudFreqSpan.ValueChanged += new EventHandler(nudFreqSpan_ValueChanged);
				}
			}
		}

		private Decimal RxBw
		{
			get
			{
				return rxBw;
			}
			set
			{
				try
				{
					nudChanBw.ValueChanged -= new EventHandler(nudChanBw_ValueChanged);
					int mant = 0;
					int exp = 0;
					SX1276.ComputeRxBwMantExp(device.FrequencyXo, device.ModulationType, value, ref mant, ref exp);
					rxBw = SX1276.ComputeRxBw(device.FrequencyXo, device.ModulationType, mant, exp);
					nudChanBw.Value = rxBw;
				}
				catch (Exception)
				{
				}
				finally
				{
					nudChanBw.ValueChanged += new EventHandler(nudChanBw_ValueChanged);
				}
			}
		}

		private LnaGainEnum LnaGain
		{
			get
			{
				return (LnaGainEnum)(cBoxLanGain.SelectedIndex + 1);
			}
			set
			{
				cBoxLanGain.SelectedIndexChanged -= new EventHandler(cBoxLanGain_SelectedIndexChanged);
				cBoxLanGain.SelectedIndex = (int)(value - 1);
				cBoxLanGain.SelectedIndexChanged += new EventHandler(cBoxLanGain_SelectedIndexChanged);
			}
		}

		public ApplicationSettings AppSettings
		{
			get
			{
				return appSettings;
			}
			set
			{
				appSettings = value;
			}
		}

		public IDevice Device
		{
			set
			{
				if (device == value)
					return;
				device = (SX1276)value;
				device.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(device_PropertyChanged);
				FrequencyRf = device.FrequencyRf;
				RxBw = device.RxBw;
				LnaGain = device.LnaGain;
				UpdatePointsList();
			}
		}

		public SpectrumAnalyserForm()
		{
			InitializeComponent();
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
			ComponentResourceManager resources = new ComponentResourceManager(typeof(SpectrumAnalyserForm));
			this.panel1 = new Panel();
			this.cBoxLanGain = new ComboBox();
			this.nudFreqCenter = new NumericUpDownEx();
			this.nudFreqSpan = new NumericUpDownEx();
			this.nudChanBw = new NumericUpDownEx();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.panel2 = new Panel();
			this.graph = new SpectrumGraphControl();
			this.panel1.SuspendLayout();
			this.nudFreqCenter.BeginInit();
			this.nudFreqSpan.BeginInit();
			this.nudChanBw.BeginInit();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			this.panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
			this.panel1.BackColor = Color.Black;
			this.panel1.BorderStyle = BorderStyle.FixedSingle;
			this.panel1.Controls.Add((Control)this.cBoxLanGain);
			this.panel1.Controls.Add((Control)this.nudFreqCenter);
			this.panel1.Controls.Add((Control)this.nudFreqSpan);
			this.panel1.Controls.Add((Control)this.nudChanBw);
			this.panel1.Controls.Add((Control)this.label2);
			this.panel1.Controls.Add((Control)this.label1);
			this.panel1.Controls.Add((Control)this.label6);
			this.panel1.Controls.Add((Control)this.label7);
			this.panel1.Controls.Add((Control)this.label3);
			this.panel1.Controls.Add((Control)this.label4);
			this.panel1.Controls.Add((Control)this.label5);
			this.panel1.Location = new Point(557, 0);
			this.panel1.Margin = new Padding(0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new Size(223, 370);
			this.panel1.TabIndex = 0;
			this.cBoxLanGain.FormattingEnabled = true;
			this.cBoxLanGain.Items.AddRange(new object[6]
      {
        (object) "G1",
        (object) "G2",
        (object) "G3",
        (object) "G4",
        (object) "G5",
        (object) "G6"
      });
			this.cBoxLanGain.Location = new Point(99, 181);
			this.cBoxLanGain.Name = "cBoxLanGain";
			this.cBoxLanGain.Size = new Size(98, 21);
			this.cBoxLanGain.TabIndex = 10;
			this.cBoxLanGain.SelectedIndexChanged += new EventHandler(this.cBoxLanGain_SelectedIndexChanged);
			this.nudFreqCenter.Anchor = AnchorStyles.None;
			NumericUpDownEx numericUpDownEx1 = this.nudFreqCenter;
			int[] bits1 = new int[4];
			bits1[0] = 61;
			Decimal num1 = new Decimal(bits1);
			numericUpDownEx1.Increment = num1;
			this.nudFreqCenter.Location = new Point(99, 103);
			NumericUpDownEx numericUpDownEx2 = this.nudFreqCenter;
			int[] bits2 = new int[4];
			bits2[0] = 1020000000;
			Decimal num2 = new Decimal(bits2);
			numericUpDownEx2.Maximum = num2;
			NumericUpDownEx numericUpDownEx3 = this.nudFreqCenter;
			int[] bits3 = new int[4];
			bits3[0] = 290000000;
			Decimal num3 = new Decimal(bits3);
			numericUpDownEx3.Minimum = num3;
			this.nudFreqCenter.Name = "nudFreqCenter";
			this.nudFreqCenter.Size = new Size(98, 20);
			this.nudFreqCenter.TabIndex = 1;
			this.nudFreqCenter.ThousandsSeparator = true;
			NumericUpDownEx numericUpDownEx4 = this.nudFreqCenter;
			int[] bits4 = new int[4];
			bits4[0] = 915000000;
			Decimal num4 = new Decimal(bits4);
			numericUpDownEx4.Value = num4;
			this.nudFreqCenter.ValueChanged += new EventHandler(this.nudFreqCenter_ValueChanged);
			this.nudFreqSpan.Anchor = AnchorStyles.None;
			NumericUpDownEx numericUpDownEx5 = this.nudFreqSpan;
			int[] bits5 = new int[4];
			bits5[0] = 61;
			Decimal num5 = new Decimal(bits5);
			numericUpDownEx5.Increment = num5;
			this.nudFreqSpan.Location = new Point(99, 129);
			NumericUpDownEx numericUpDownEx6 = this.nudFreqSpan;
			int[] bits6 = new int[4];
			bits6[0] = 100000000;
			Decimal num6 = new Decimal(bits6);
			numericUpDownEx6.Maximum = num6;
			this.nudFreqSpan.Name = "nudFreqSpan";
			this.nudFreqSpan.Size = new Size(98, 20);
			this.nudFreqSpan.TabIndex = 4;
			this.nudFreqSpan.ThousandsSeparator = true;
			NumericUpDownEx numericUpDownEx7 = this.nudFreqSpan;
			int[] bits7 = new int[4];
			bits7[0] = 1000000;
			Decimal num7 = new Decimal(bits7);
			numericUpDownEx7.Value = num7;
			this.nudFreqSpan.ValueChanged += new EventHandler(this.nudFreqSpan_ValueChanged);
			this.nudChanBw.Anchor = AnchorStyles.None;
			this.nudChanBw.Location = new Point(99, 155);
			NumericUpDownEx numericUpDownEx8 = this.nudChanBw;
			int[] bits8 = new int[4];
			bits8[0] = 500000;
			Decimal num8 = new Decimal(bits8);
			numericUpDownEx8.Maximum = num8;
			NumericUpDownEx numericUpDownEx9 = this.nudChanBw;
			int[] bits9 = new int[4];
			bits9[0] = 3906;
			Decimal num9 = new Decimal(bits9);
			numericUpDownEx9.Minimum = num9;
			this.nudChanBw.Name = "nudChanBw";
			this.nudChanBw.Size = new Size(98, 20);
			this.nudChanBw.TabIndex = 7;
			this.nudChanBw.ThousandsSeparator = true;
			NumericUpDownEx numericUpDownEx10 = this.nudChanBw;
			int[] bits10 = new int[4];
			bits10[0] = 10417;
			Decimal num10 = new Decimal(bits10);
			numericUpDownEx10.Value = num10;
			this.nudChanBw.ValueChanged += new EventHandler(this.nudChanBw_ValueChanged);
			this.label2.Anchor = AnchorStyles.None;
			this.label2.AutoSize = true;
			this.label2.BackColor = Color.Transparent;
			this.label2.ForeColor = Color.Gray;
			this.label2.Location = new Point(-2, 133);
			this.label2.Name = "label2";
			this.label2.Size = new Size(35, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Span:";
			this.label1.Anchor = AnchorStyles.None;
			this.label1.AutoSize = true;
			this.label1.BackColor = Color.Transparent;
			this.label1.ForeColor = Color.Gray;
			this.label1.Location = new Point(-2, 107);
			this.label1.Name = "label1";
			this.label1.Size = new Size(91, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Center frequency:";
			this.label6.Anchor = AnchorStyles.None;
			this.label6.AutoSize = true;
			this.label6.ForeColor = Color.Gray;
			this.label6.Location = new Point(203, 159);
			this.label6.Name = "label6";
			this.label6.Size = new Size(20, 13);
			this.label6.TabIndex = 8;
			this.label6.Text = "Hz";
			this.label7.Anchor = AnchorStyles.None;
			this.label7.AutoSize = true;
			this.label7.BackColor = Color.Transparent;
			this.label7.ForeColor = Color.Gray;
			this.label7.Location = new Point(-2, 185);
			this.label7.Name = "label7";
			this.label7.Size = new Size(54, 13);
			this.label7.TabIndex = 9;
			this.label7.Text = "LNA gain:";
			this.label3.Anchor = AnchorStyles.None;
			this.label3.AutoSize = true;
			this.label3.BackColor = Color.Transparent;
			this.label3.ForeColor = Color.Gray;
			this.label3.Location = new Point(-2, 159);
			this.label3.Name = "label3";
			this.label3.Size = new Size(101, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "Channel bandwidth:";
			this.label4.Anchor = AnchorStyles.None;
			this.label4.AutoSize = true;
			this.label4.ForeColor = Color.Gray;
			this.label4.Location = new Point(203, 107);
			this.label4.Name = "label4";
			this.label4.Size = new Size(20, 13);
			this.label4.TabIndex = 2;
			this.label4.Text = "Hz";
			this.label5.Anchor = AnchorStyles.None;
			this.label5.AutoSize = true;
			this.label5.ForeColor = Color.Gray;
			this.label5.Location = new Point(203, 133);
			this.label5.Name = "label5";
			this.label5.Size = new Size(20, 13);
			this.label5.TabIndex = 5;
			this.label5.Text = "Hz";
			this.panel2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.panel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel2.BorderStyle = BorderStyle.FixedSingle;
			this.panel2.Controls.Add((Control)this.graph);
			this.panel2.Location = new Point(0, 0);
			this.panel2.Margin = new Padding(0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new Size(557, 370);
			this.panel2.TabIndex = 2;
			this.graph.Dock = DockStyle.Fill;
			this.graph.Location = new Point(0, 0);
			this.graph.Name = "graph";
			this.graph.Size = new Size(555, 368);
			this.graph.TabIndex = 0;
			this.AutoScaleDimensions = new SizeF(6f, 13f);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.ClientSize = new Size(780, 370);
			this.Controls.Add((Control)this.panel2);
			this.Controls.Add((Control)this.panel1);
			this.Icon = (Icon)resources.GetObject("$this.Icon");
			this.Name = "SpectrumAnalyserForm";
			this.Text = "Spectrum analyser";
			this.Load += new EventHandler(this.SpectrumAnalyserForm_Load);
			this.FormClosed += new FormClosedEventHandler(this.SpectrumAnalyserForm_FormClosed);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.nudFreqCenter.EndInit();
			this.nudFreqSpan.EndInit();
			this.nudChanBw.EndInit();
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);
		}

		private bool IsFormLocatedInScreen(Form frm, Screen[] screens)
		{
			int upperBound = screens.GetUpperBound(0);
			bool flag = false;
			for (int index = 0; index <= upperBound; ++index)
			{
				if (frm.Left < screens[index].WorkingArea.Left || frm.Top < screens[index].WorkingArea.Top || (frm.Left > screens[index].WorkingArea.Right || frm.Top > screens[index].WorkingArea.Bottom))
				{
					flag = false;
				}
				else
				{
					flag = true;
					break;
				}
			}
			return flag;
		}

		private void UpdatePointsList()
		{
			GraphPane graphPane = this.graph.PaneList[0];
			graphPane.XAxis.Scale.Max = (double)this.device.SpectrumFrequencyMax;
			graphPane.XAxis.Scale.Min = (double)this.device.SpectrumFrequencyMin;
			this.device.SpectrumFrequencyId = 0;
			this.points = new PointPairList();
			for (int index = 0; index < this.device.SpectrumNbFrequenciesMax; ++index)
				this.points.Add(new PointPair((double)(this.device.SpectrumFrequencyMin + this.device.SpectrumFrequencyStep * (Decimal)index), -127.5));
			graphPane.CurveList[0] = (CurveItem)new LineItem("", (IPointList)this.points, Color.Yellow, SymbolType.None);
			graphPane.AxisChange();
			this.graph.Invalidate();
			this.graph.Refresh();
		}

		private void OnDevicePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			try
			{
				switch (e.PropertyName)
				{
					case "FrequencyRf":
						this.FrequencyRf = this.device.FrequencyRf;
						this.UpdatePointsList();
						break;
					case "RxBw":
						this.RxBw = this.device.RxBw;
						this.UpdatePointsList();
						break;
					case "RxBwMin":
						this.nudChanBw.Minimum = this.device.RxBwMin;
						break;
					case "RxBwMax":
						this.nudChanBw.Maximum = this.device.RxBwMax;
						break;
					case "SpectrumFreqSpan":
						this.FrequencySpan = this.device.SpectrumFrequencySpan;
						this.UpdatePointsList();
						break;
					case "LnaGain":
						this.LnaGain = this.device.LnaGain;
						break;
					case "SpectrumData":
						this.graph.UpdateLineGraph(this.device.SpectrumFrequencyId, (double)this.device.SpectrumRssiValue);
						break;
				}
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
		}

		private void OnError(byte status, string message)
		{
			this.Refresh();
		}

		private void device_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (this.InvokeRequired)
				this.BeginInvoke((Delegate)new SpectrumAnalyserForm.DeviceDataChangedDelegate(this.OnDevicePropertyChanged), sender, (object)e);
			else
				this.OnDevicePropertyChanged(sender, e);
		}

		private void SpectrumAnalyserForm_Load(object sender, EventArgs e)
		{
			string s1 = this.appSettings.GetValue("SpectrumAnalyserTop");
			if (s1 != null)
			{
				try
				{
					this.Top = int.Parse(s1);
				}
				catch
				{
					int num = (int)MessageBox.Show((IWin32Window)this, "Error getting Top value.");
				}
			}
			string s2 = this.appSettings.GetValue("SpectrumAnalyserLeft");
			if (s2 != null)
			{
				try
				{
					this.Left = int.Parse(s2);
				}
				catch
				{
					int num = (int)MessageBox.Show((IWin32Window)this, "Error getting Left value.");
				}
			}
			Screen[] allScreens = Screen.AllScreens;
			if (!this.IsFormLocatedInScreen((Form)this, allScreens))
			{
				this.Top = allScreens[0].WorkingArea.Top;
				this.Left = allScreens[0].WorkingArea.Left;
			}
			this.device.SpectrumOn = true;
		}

		private void SpectrumAnalyserForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			try
			{
				this.appSettings.SetValue("SpectrumAnalyserTop", this.Top.ToString());
				this.appSettings.SetValue("SpectrumAnalyserLeft", this.Left.ToString());
				this.device.SpectrumOn = false;
			}
			catch (Exception)
			{
			}
		}

		private void nudFreqCenter_ValueChanged(object sender, EventArgs e)
		{
			this.FrequencyRf = this.nudFreqCenter.Value;
			this.device.SetFrequencyRf(this.FrequencyRf);
		}

		private void nudFreqSpan_ValueChanged(object sender, EventArgs e)
		{
			this.FrequencySpan = this.nudFreqSpan.Value;
			this.device.SpectrumFrequencySpan = this.FrequencySpan;
		}

		private void nudChanBw_ValueChanged(object sender, EventArgs e)
		{
			Decimal[] rxBwFreqTable = SX1276.ComputeRxBwFreqTable(this.device.FrequencyXo, this.device.ModulationType);
			int num1 = (int)(this.nudChanBw.Value - this.RxBw);
			int index;
			if (num1 >= -1 && num1 <= 1)
			{
				index = Array.IndexOf<Decimal>(rxBwFreqTable, this.RxBw) - num1;
			}
			else
			{
				int mant = 0;
				int exp = 0;
				Decimal num2 = new Decimal(0);
				SX1276.ComputeRxBwMantExp(this.device.FrequencyXo, this.device.ModulationType, this.nudChanBw.Value, ref mant, ref exp);
				Decimal rxBw = SX1276.ComputeRxBw(this.device.FrequencyXo, this.device.ModulationType, mant, exp);
				index = Array.IndexOf<Decimal>(rxBwFreqTable, rxBw);
			}
			this.nudChanBw.ValueChanged -= new EventHandler(this.nudChanBw_ValueChanged);
			this.nudChanBw.Value = rxBwFreqTable[index];
			this.nudChanBw.ValueChanged += new EventHandler(this.nudChanBw_ValueChanged);
			this.RxBw = this.nudChanBw.Value;
			this.device.SetRxBw(this.RxBw);
		}

		private void cBoxLanGain_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.device.SetLnaGain(this.LnaGain);
		}

		private delegate void DeviceDataChangedDelegate(object sender, PropertyChangedEventArgs e);
	}
}

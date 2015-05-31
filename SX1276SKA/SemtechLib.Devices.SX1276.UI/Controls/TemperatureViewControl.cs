using SemtechLib.Controls;
using SemtechLib.Devices.SX1276.Enumerations;
using SemtechLib.Devices.SX1276.Events;
using SemtechLib.Devices.SX1276.UI.Forms;
using SemtechLib.General.Events;
using SemtechLib.General.Interfaces;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SemtechLib.Devices.SX1276.UI.Controls
{
	public class TemperatureViewControl : UserControl, INotifyDocumentationChanged
	{
		private OperatingModeEnum mode = OperatingModeEnum.Stdby;
		private Decimal tempValueRoom = new Decimal(250, 0, 0, false, (byte)1);
		private Decimal tempDelta = new Decimal(0, 0, 0, false, (byte)1);
		private bool tempCalDone;
		private IContainer components;
		private Button btnCalibrate;
		private TempCtrl thermometerCtrl;
		private Led ledTempMeasRunning;
		private Label lblMeasuring;
		private GroupBoxEx gBoxIQCalibration;
		private Label lblSensitivityBoost;
		private Panel pnlSensitivityBoost;
		private RadioButton rBtnRxCalAutoOff;
		private RadioButton rBtnRxCalAutoOn;
		private Label label1;
		private Button btnRxCalibration;
		private Label label2;
		private Led ledRxCalOnGoing;
		private GroupBoxEx gBoxTemperature;
		private Led ledTempStatHigher;
		private Label label3;
		private Label label4;
		private Label label5;
		private Panel panel2;
		private RadioButton rBtnTempMeasOff;
		private RadioButton rBtnTempMeasOn;
		private Label label16;
		private ComboBox cBoxTempThreshold;
		private Panel panel1;
		private Label label6;
		private Label label54;
		private Label lblTempDelta;

		public OperatingModeEnum Mode
		{
			get
			{
				return mode;
			}
			set
			{
				mode = value;
				switch (mode)
				{
					case OperatingModeEnum.Sleep:
					case OperatingModeEnum.Stdby:
						btnCalibrate.Enabled = false;
						panel1.Enabled = false;
						thermometerCtrl.Enabled = false;
						break;
					case OperatingModeEnum.FsTx:
					case OperatingModeEnum.Tx:
					case OperatingModeEnum.FsRx:
					case OperatingModeEnum.Rx:
						btnCalibrate.Enabled = true;
						panel1.Enabled = true;
						if (!TempCalDone)
							break;
						thermometerCtrl.Enabled = true;
						break;
				}
			}
		}

		public bool AutoImageCalOn
		{
			get
			{
				return rBtnRxCalAutoOn.Checked;
			}
			set
			{
				rBtnRxCalAutoOn.CheckedChanged -= new EventHandler(rBtnRxCalAutoOn_CheckedChanged);
				rBtnRxCalAutoOff.CheckedChanged -= new EventHandler(rBtnRxCalAutoOn_CheckedChanged);
				if (value)
				{
					rBtnRxCalAutoOn.Checked = true;
					rBtnRxCalAutoOff.Checked = false;
				}
				else
				{
					rBtnRxCalAutoOn.Checked = false;
					rBtnRxCalAutoOff.Checked = true;
				}
				rBtnRxCalAutoOn.CheckedChanged += new EventHandler(rBtnRxCalAutoOn_CheckedChanged);
				rBtnRxCalAutoOff.CheckedChanged += new EventHandler(rBtnRxCalAutoOn_CheckedChanged);
			}
		}

		public bool ImageCalRunning
		{
			get
			{
				return ledRxCalOnGoing.Checked;
			}
			set
			{
				ledRxCalOnGoing.Checked = value;
			}
		}

		public bool TempChange
		{
			get
			{
				return ledTempStatHigher.Checked;
			}
			set
			{
				ledTempStatHigher.Checked = value;
			}
		}

		public TempThresholdEnum TempThreshold
		{
			get
			{
				return (TempThresholdEnum)cBoxTempThreshold.SelectedIndex;
			}
			set
			{
				cBoxTempThreshold.SelectedIndexChanged -= new EventHandler(cBoxTempThreshold_SelectedIndexChanged);
				cBoxTempThreshold.SelectedIndex = (int)value;
				cBoxTempThreshold.SelectedIndexChanged += new EventHandler(cBoxTempThreshold_SelectedIndexChanged);
			}
		}

		public bool TempMonitorOff
		{
			get
			{
				return rBtnTempMeasOff.Checked;
			}
			set
			{
				rBtnTempMeasOn.CheckedChanged -= new EventHandler(rBtnTempMeasOn_CheckedChanged);
				rBtnTempMeasOff.CheckedChanged -= new EventHandler(rBtnTempMeasOn_CheckedChanged);
				if (value)
				{
					rBtnTempMeasOn.Checked = false;
					rBtnTempMeasOff.Checked = true;
				}
				else
				{
					rBtnTempMeasOn.Checked = true;
					rBtnTempMeasOff.Checked = false;
				}
				rBtnTempMeasOn.CheckedChanged += new EventHandler(rBtnTempMeasOn_CheckedChanged);
				rBtnTempMeasOff.CheckedChanged += new EventHandler(rBtnTempMeasOn_CheckedChanged);
			}
		}

		public bool TempMeasRunning
		{
			get
			{
				return ledTempMeasRunning.Checked;
			}
			set
			{
				ledTempMeasRunning.Checked = value;
			}
		}

		public Decimal TempValue
		{
			get
			{
				return (Decimal)thermometerCtrl.Value;
			}
			set
			{
				thermometerCtrl.Value = (double)value;
			}
		}

		public bool TempCalDone
		{
			get
			{
				return tempCalDone;
			}
			set
			{
				tempCalDone = value;
				thermometerCtrl.Enabled = value;
			}
		}

		public Decimal TempValueRoom
		{
			get
			{
				return tempValueRoom;
			}
			set
			{
				tempValueRoom = value;
			}
		}

		public Decimal TempDelta
		{
			get
			{
				return tempDelta;
			}
			set
			{
				tempDelta = value;
				lblTempDelta.Text = tempDelta.ToString("N0");
			}
		}

		public event BooleanEventHandler RxCalAutoOnChanged;

		public event EventHandler RxCalibrationChanged;

		public event TempThresholdEventHandler TempThresholdChanged;

		public event DecimalEventHandler TempCalibrateChanged;

		public event BooleanEventHandler TempMeasOnChanged;

		public event DocumentationChangedEventHandler DocumentationChanged;

		public TemperatureViewControl()
		{
			InitializeComponent();
		}

		private void OnRxCalAutoOnChanged(bool value)
		{
			if (RxCalAutoOnChanged == null)
				return;
			RxCalAutoOnChanged((object)this, new BooleanEventArg(value));
		}

		private void OnRxCalibrationChanged()
		{
			if (RxCalibrationChanged == null)
				return;
			RxCalibrationChanged((object)this, EventArgs.Empty);
		}

		private void OnTempThresholdChanged(TempThresholdEnum value)
		{
			if (TempThresholdChanged == null)
				return;
			TempThresholdChanged((object)this, new TempThresholdEventArg(value));
		}

		private void OnTempCalibrateChanged(Decimal value)
		{
			if (TempCalibrateChanged == null)
				return;
			TempCalibrateChanged((object)this, new DecimalEventArg(value));
		}

		private void OnTempMeasOnChanged(bool value)
		{
			if (TempMeasOnChanged == null)
				return;
			TempMeasOnChanged((object)this, new BooleanEventArg(value));
		}

		private void rBtnRxCalAutoOn_CheckedChanged(object sender, EventArgs e)
		{
			AutoImageCalOn = rBtnRxCalAutoOn.Checked;
			OnRxCalAutoOnChanged(AutoImageCalOn);
		}

		private void btnRxCalibration_Click(object sender, EventArgs e)
		{
			OnRxCalibrationChanged();
		}

		private void cBoxTempThreshold_SelectedIndexChanged(object sender, EventArgs e)
		{
			TempThreshold = (TempThresholdEnum)cBoxTempThreshold.SelectedIndex;
			OnTempThresholdChanged(TempThreshold);
		}

		private void rBtnTempMeasOn_CheckedChanged(object sender, EventArgs e)
		{
			TempMonitorOff = rBtnTempMeasOff.Checked;
			OnTempMeasOnChanged(TempMonitorOff);
		}

		private void btnTempCalibrate_Click(object sender, EventArgs e)
		{
			TemperatureCalibrationForm temperatureCalibrationForm = new TemperatureCalibrationForm();
			temperatureCalibrationForm.TempValueRoom = TempValueRoom;
			if (temperatureCalibrationForm.ShowDialog() != DialogResult.OK)
				return;
			try
			{
				Cursor = Cursors.WaitCursor;
				TempValueRoom = temperatureCalibrationForm.TempValueRoom;
				OnTempCalibrateChanged(TempValueRoom);
			}
			catch
			{
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void control_MouseEnter(object sender, EventArgs e)
		{
			if (sender == thermometerCtrl)
				OnDocumentationChanged(new DocumentationChangedEventArgs("Temperature", "Thermometer"));
			else if (sender == btnCalibrate)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Temperature", "Calibrate"));
			}
			else
			{
				if (sender != ledTempMeasRunning && sender != lblMeasuring)
					return;
				OnDocumentationChanged(new DocumentationChangedEventArgs("Temperature", "Measure running"));
			}
		}

		private void control_MouseLeave(object sender, EventArgs e)
		{
			OnDocumentationChanged(new DocumentationChangedEventArgs(".", "Overview"));
		}

		private void OnDocumentationChanged(DocumentationChangedEventArgs e)
		{
			if (DocumentationChanged == null)
				return;
			DocumentationChanged((object)this, e);
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
			this.panel1 = new Panel();
			this.thermometerCtrl = new TempCtrl();
			this.gBoxTemperature = new GroupBoxEx();
			this.lblMeasuring = new Label();
			this.ledTempMeasRunning = new Led();
			this.btnCalibrate = new Button();
			this.label16 = new Label();
			this.cBoxTempThreshold = new ComboBox();
			this.ledTempStatHigher = new Led();
			this.label3 = new Label();
			this.label4 = new Label();
			this.label5 = new Label();
			this.panel2 = new Panel();
			this.rBtnTempMeasOff = new RadioButton();
			this.rBtnTempMeasOn = new RadioButton();
			this.gBoxIQCalibration = new GroupBoxEx();
			this.label6 = new Label();
			this.ledRxCalOnGoing = new Led();
			this.btnRxCalibration = new Button();
			this.label54 = new Label();
			this.label2 = new Label();
			this.label1 = new Label();
			this.lblTempDelta = new Label();
			this.lblSensitivityBoost = new Label();
			this.pnlSensitivityBoost = new Panel();
			this.rBtnRxCalAutoOff = new RadioButton();
			this.rBtnRxCalAutoOn = new RadioButton();
			this.panel1.SuspendLayout();
			this.gBoxTemperature.SuspendLayout();
			this.panel2.SuspendLayout();
			this.gBoxIQCalibration.SuspendLayout();
			this.pnlSensitivityBoost.SuspendLayout();
			this.SuspendLayout();
			this.panel1.Controls.Add((Control)this.thermometerCtrl);
			this.panel1.Location = new Point(364, 3);
			this.panel1.Name = "panel1";
			this.panel1.Size = new Size(432, 487);
			this.panel1.TabIndex = 7;
			this.thermometerCtrl.BackColor = Color.Transparent;
			this.thermometerCtrl.DrawTics = true;
			this.thermometerCtrl.Enabled = false;
			this.thermometerCtrl.ForeColor = Color.Red;
			this.thermometerCtrl.LargeTicFreq = 10;
			this.thermometerCtrl.Location = new Point(142, 26);
			this.thermometerCtrl.Name = "thermometerCtrl";
			this.thermometerCtrl.Range.Max = 90.0;
			this.thermometerCtrl.Range.Min = -40.0;
			this.thermometerCtrl.Size = new Size(148, 434);
			this.thermometerCtrl.SmallTicFreq = 5;
			this.thermometerCtrl.TabIndex = 0;
			this.thermometerCtrl.Text = "Thermometer";
			this.thermometerCtrl.Value = 25.0;
			this.thermometerCtrl.MouseEnter += new EventHandler(this.control_MouseEnter);
			this.thermometerCtrl.MouseLeave += new EventHandler(this.control_MouseLeave);
			this.gBoxTemperature.Controls.Add((Control)this.lblMeasuring);
			this.gBoxTemperature.Controls.Add((Control)this.ledTempMeasRunning);
			this.gBoxTemperature.Controls.Add((Control)this.btnCalibrate);
			this.gBoxTemperature.Controls.Add((Control)this.label16);
			this.gBoxTemperature.Controls.Add((Control)this.cBoxTempThreshold);
			this.gBoxTemperature.Controls.Add((Control)this.ledTempStatHigher);
			this.gBoxTemperature.Controls.Add((Control)this.label3);
			this.gBoxTemperature.Controls.Add((Control)this.label4);
			this.gBoxTemperature.Controls.Add((Control)this.label5);
			this.gBoxTemperature.Controls.Add((Control)this.panel2);
			this.gBoxTemperature.Location = new Point(3, 244);
			this.gBoxTemperature.Name = "gBoxTemperature";
			this.gBoxTemperature.Size = new Size(355, 147);
			this.gBoxTemperature.TabIndex = 6;
			this.gBoxTemperature.TabStop = false;
			this.gBoxTemperature.Text = "Temperature";
			this.lblMeasuring.AutoSize = true;
			this.lblMeasuring.Location = new Point(6, 72);
			this.lblMeasuring.Name = "lblMeasuring";
			this.lblMeasuring.Size = new Size(59, 13);
			this.lblMeasuring.TabIndex = 1;
			this.lblMeasuring.Text = "Measuring:";
			this.lblMeasuring.TextAlign = ContentAlignment.MiddleLeft;
			this.lblMeasuring.MouseEnter += new EventHandler(this.control_MouseEnter);
			this.lblMeasuring.MouseLeave += new EventHandler(this.control_MouseLeave);
			this.ledTempMeasRunning.BackColor = Color.Transparent;
			this.ledTempMeasRunning.LedColor = Color.Green;
			this.ledTempMeasRunning.LedSize = new Size(11, 11);
			this.ledTempMeasRunning.Location = new Point(164, 71);
			this.ledTempMeasRunning.Name = "ledTempMeasRunning";
			this.ledTempMeasRunning.Size = new Size(15, 15);
			this.ledTempMeasRunning.TabIndex = 2;
			this.ledTempMeasRunning.Text = "Measuring";
			this.ledTempMeasRunning.MouseEnter += new EventHandler(this.control_MouseEnter);
			this.ledTempMeasRunning.MouseLeave += new EventHandler(this.control_MouseLeave);
			this.btnCalibrate.Location = new Point(164, 42);
			this.btnCalibrate.Name = "btnCalibrate";
			this.btnCalibrate.Size = new Size(75, 23);
			this.btnCalibrate.TabIndex = 0;
			this.btnCalibrate.Text = "Calibrate";
			this.btnCalibrate.UseVisualStyleBackColor = true;
			this.btnCalibrate.Click += new EventHandler(this.btnTempCalibrate_Click);
			this.btnCalibrate.MouseEnter += new EventHandler(this.control_MouseEnter);
			this.btnCalibrate.MouseLeave += new EventHandler(this.control_MouseLeave);
			this.label16.AutoSize = true;
			this.label16.Location = new Point(294, 96);
			this.label16.Name = "label16";
			this.label16.Size = new Size(18, 13);
			this.label16.TabIndex = 9;
			this.label16.Text = "°C";
			this.cBoxTempThreshold.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxTempThreshold.FormattingEnabled = true;
			this.cBoxTempThreshold.Items.AddRange(new object[4]
      {
        (object) "5",
        (object) "10",
        (object) "15",
        (object) "20"
      });
			this.cBoxTempThreshold.Location = new Point(164, 92);
			this.cBoxTempThreshold.Name = "cBoxTempThreshold";
			this.cBoxTempThreshold.Size = new Size(124, 21);
			this.cBoxTempThreshold.TabIndex = 8;
			this.cBoxTempThreshold.SelectedIndexChanged += new EventHandler(this.cBoxTempThreshold_SelectedIndexChanged);
			this.ledTempStatHigher.BackColor = Color.Transparent;
			this.ledTempStatHigher.LedColor = Color.Green;
			this.ledTempStatHigher.LedSize = new Size(11, 11);
			this.ledTempStatHigher.Location = new Point(164, 121);
			this.ledTempStatHigher.Name = "ledTempStatHigher";
			this.ledTempStatHigher.Size = new Size(15, 15);
			this.ledTempStatHigher.TabIndex = 7;
			this.ledTempStatHigher.Text = "led1";
			this.label3.AutoSize = true;
			this.label3.Location = new Point(6, 123);
			this.label3.Name = "label3";
			this.label3.Size = new Size(149, 13);
			this.label3.TabIndex = 3;
			this.label3.Text = "Change higher than threshold:";
			this.label4.AutoSize = true;
			this.label4.Location = new Point(6, 97);
			this.label4.Name = "label4";
			this.label4.Size = new Size(57, 13);
			this.label4.TabIndex = 3;
			this.label4.Text = "Threshold:";
			this.label5.AutoSize = true;
			this.label5.Location = new Point(6, 21);
			this.label5.Name = "label5";
			this.label5.Size = new Size(45, 13);
			this.label5.TabIndex = 3;
			this.label5.Text = "Monitor:";
			this.panel2.AutoSize = true;
			this.panel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel2.Controls.Add((Control)this.rBtnTempMeasOff);
			this.panel2.Controls.Add((Control)this.rBtnTempMeasOn);
			this.panel2.Location = new Point(164, 19);
			this.panel2.Name = "panel2";
			this.panel2.Size = new Size(93, 17);
			this.panel2.TabIndex = 4;
			this.rBtnTempMeasOff.AutoSize = true;
			this.rBtnTempMeasOff.Location = new Point(45, 0);
			this.rBtnTempMeasOff.Margin = new Padding(3, 0, 3, 0);
			this.rBtnTempMeasOff.Name = "rBtnTempMeasOff";
			this.rBtnTempMeasOff.Size = new Size(45, 17);
			this.rBtnTempMeasOff.TabIndex = 1;
			this.rBtnTempMeasOff.Text = "OFF";
			this.rBtnTempMeasOff.UseVisualStyleBackColor = true;
			this.rBtnTempMeasOff.CheckedChanged += new EventHandler(this.rBtnTempMeasOn_CheckedChanged);
			this.rBtnTempMeasOn.AutoSize = true;
			this.rBtnTempMeasOn.Checked = true;
			this.rBtnTempMeasOn.Location = new Point(3, 0);
			this.rBtnTempMeasOn.Margin = new Padding(3, 0, 3, 0);
			this.rBtnTempMeasOn.Name = "rBtnTempMeasOn";
			this.rBtnTempMeasOn.Size = new Size(41, 17);
			this.rBtnTempMeasOn.TabIndex = 0;
			this.rBtnTempMeasOn.TabStop = true;
			this.rBtnTempMeasOn.Text = "ON";
			this.rBtnTempMeasOn.UseVisualStyleBackColor = true;
			this.rBtnTempMeasOn.CheckedChanged += new EventHandler(this.rBtnTempMeasOn_CheckedChanged);
			this.gBoxIQCalibration.Controls.Add((Control)this.label6);
			this.gBoxIQCalibration.Controls.Add((Control)this.ledRxCalOnGoing);
			this.gBoxIQCalibration.Controls.Add((Control)this.btnRxCalibration);
			this.gBoxIQCalibration.Controls.Add((Control)this.label54);
			this.gBoxIQCalibration.Controls.Add((Control)this.label2);
			this.gBoxIQCalibration.Controls.Add((Control)this.label1);
			this.gBoxIQCalibration.Controls.Add((Control)this.lblTempDelta);
			this.gBoxIQCalibration.Controls.Add((Control)this.lblSensitivityBoost);
			this.gBoxIQCalibration.Controls.Add((Control)this.pnlSensitivityBoost);
			this.gBoxIQCalibration.Location = new Point(3, 102);
			this.gBoxIQCalibration.Name = "gBoxIQCalibration";
			this.gBoxIQCalibration.Size = new Size(355, 136);
			this.gBoxIQCalibration.TabIndex = 6;
			this.gBoxIQCalibration.TabStop = false;
			this.gBoxIQCalibration.Text = "IQ calibration";
			this.label6.AutoSize = true;
			this.label6.Location = new Point(6, 96);
			this.label6.Name = "label6";
			this.label6.Size = new Size(93, 26);
			this.label6.TabIndex = 20;
			this.label6.Text = "Temperature delta\r\n( Actual - Former ):";
			this.ledRxCalOnGoing.BackColor = Color.Transparent;
			this.ledRxCalOnGoing.LedColor = Color.Green;
			this.ledRxCalOnGoing.LedSize = new Size(11, 11);
			this.ledRxCalOnGoing.Location = new Point(164, 71);
			this.ledRxCalOnGoing.Name = "ledRxCalOnGoing";
			this.ledRxCalOnGoing.Size = new Size(15, 15);
			this.ledRxCalOnGoing.TabIndex = 7;
			this.ledRxCalOnGoing.Text = "led1";
			this.btnRxCalibration.Location = new Point(164, 42);
			this.btnRxCalibration.Name = "btnRxCalibration";
			this.btnRxCalibration.Size = new Size(75, 23);
			this.btnRxCalibration.TabIndex = 5;
			this.btnRxCalibration.Text = "Calibrate";
			this.btnRxCalibration.UseVisualStyleBackColor = true;
			this.btnRxCalibration.Click += new EventHandler(this.btnRxCalibration_Click);
			this.label54.AutoSize = true;
			this.label54.BackColor = Color.Transparent;
			this.label54.Location = new Point(294, 103);
			this.label54.Name = "label54";
			this.label54.Size = new Size(18, 13);
			this.label54.TabIndex = 19;
			this.label54.Text = "°C";
			this.label54.TextAlign = ContentAlignment.MiddleCenter;
			this.label2.AutoSize = true;
			this.label2.Location = new Point(6, 73);
			this.label2.Name = "label2";
			this.label2.Size = new Size(90, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Calibration status:";
			this.label1.AutoSize = true;
			this.label1.Location = new Point(6, 47);
			this.label1.Name = "label1";
			this.label1.Size = new Size(59, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Calibration:";
			this.lblTempDelta.BackColor = Color.Transparent;
			this.lblTempDelta.BorderStyle = BorderStyle.Fixed3D;
			this.lblTempDelta.Location = new Point(164, 99);
			this.lblTempDelta.Margin = new Padding(3);
			this.lblTempDelta.Name = "lblTempDelta";
			this.lblTempDelta.Size = new Size(124, 20);
			this.lblTempDelta.TabIndex = 18;
			this.lblTempDelta.Text = "0";
			this.lblTempDelta.TextAlign = ContentAlignment.MiddleCenter;
			this.lblSensitivityBoost.AutoSize = true;
			this.lblSensitivityBoost.Location = new Point(6, 21);
			this.lblSensitivityBoost.Name = "lblSensitivityBoost";
			this.lblSensitivityBoost.Size = new Size(32, 13);
			this.lblSensitivityBoost.TabIndex = 3;
			this.lblSensitivityBoost.Text = "Auto:";
			this.pnlSensitivityBoost.AutoSize = true;
			this.pnlSensitivityBoost.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.pnlSensitivityBoost.Controls.Add((Control)this.rBtnRxCalAutoOff);
			this.pnlSensitivityBoost.Controls.Add((Control)this.rBtnRxCalAutoOn);
			this.pnlSensitivityBoost.Enabled = true;
			this.pnlSensitivityBoost.Location = new Point(164, 19);
			this.pnlSensitivityBoost.Name = "pnlSensitivityBoost";
			this.pnlSensitivityBoost.Size = new Size(93, 17);
			this.pnlSensitivityBoost.TabIndex = 4;
			this.rBtnRxCalAutoOff.AutoSize = true;
			this.rBtnRxCalAutoOff.Location = new Point(45, 0);
			this.rBtnRxCalAutoOff.Margin = new Padding(3, 0, 3, 0);
			this.rBtnRxCalAutoOff.Name = "rBtnRxCalAutoOff";
			this.rBtnRxCalAutoOff.Size = new Size(45, 17);
			this.rBtnRxCalAutoOff.TabIndex = 1;
			this.rBtnRxCalAutoOff.Text = "OFF";
			this.rBtnRxCalAutoOff.UseVisualStyleBackColor = true;
			this.rBtnRxCalAutoOff.CheckedChanged += new EventHandler(this.rBtnRxCalAutoOn_CheckedChanged);
			this.rBtnRxCalAutoOn.AutoSize = true;
			this.rBtnRxCalAutoOn.Checked = true;
			this.rBtnRxCalAutoOn.Location = new Point(3, 0);
			this.rBtnRxCalAutoOn.Margin = new Padding(3, 0, 3, 0);
			this.rBtnRxCalAutoOn.Name = "rBtnRxCalAutoOn";
			this.rBtnRxCalAutoOn.Size = new Size(41, 17);
			this.rBtnRxCalAutoOn.TabIndex = 0;
			this.rBtnRxCalAutoOn.TabStop = true;
			this.rBtnRxCalAutoOn.Text = "ON";
			this.rBtnRxCalAutoOn.UseVisualStyleBackColor = true;
			this.rBtnRxCalAutoOn.CheckedChanged += new EventHandler(this.rBtnRxCalAutoOn_CheckedChanged);
			this.AutoScaleDimensions = new SizeF(6f, 13f);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.Controls.Add((Control)this.panel1);
			this.Controls.Add((Control)this.gBoxTemperature);
			this.Controls.Add((Control)this.gBoxIQCalibration);
			this.Name = "TemperatureViewControl";
			this.Size = new Size(799, 493);
			this.panel1.ResumeLayout(false);
			this.gBoxTemperature.ResumeLayout(false);
			this.gBoxTemperature.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.gBoxIQCalibration.ResumeLayout(false);
			this.gBoxIQCalibration.PerformLayout();
			this.pnlSensitivityBoost.ResumeLayout(false);
			this.pnlSensitivityBoost.PerformLayout();
			this.ResumeLayout(false);
		}
	}
}

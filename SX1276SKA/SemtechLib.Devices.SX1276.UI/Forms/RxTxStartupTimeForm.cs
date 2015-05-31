using SemtechLib.Controls;
using SemtechLib.Devices.Common.Interfaces;
using SemtechLib.Devices.SX1276;
using SemtechLib.Devices.SX1276.Enumerations;
using SemtechLib.General;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SemtechLib.Devices.SX1276.UI.Forms
{
	public class RxTxStartupTimeForm : Form
	{
		private string unit = "";
		private const double Tana = 2E-05;
		private double txTsTr;
		private double rxTsRe;
		private double rxTsRssi;
		private double rxTsReTsRssi;
		private ApplicationSettings appSettings;
		private SX1276 device;
		private double Tcf;
		private IContainer components;
		private GroupBoxEx gBoxTxStartupTime;
		private Label lblTsTrUnit;
		private Label lblTsTrValue;
		private Label lblTsTr;
		private GroupBoxEx gBoxRxStartupTime;
		private Label label4;
		private Label label3;
		private Label lblAgcOnValue;
		private Label lblAfcOnValue;
		private Label lblAfcOn;
		private Label lblAgcOn;
		private Label lblTsReUnit;
		private Label lblTsReValue;
		private Label lblTsRe;
		private Label label9;
		private Label label6;
		private Label lblTsReTsRssiUnit;
		private Label lblTsRssiUnit;
		private Label label8;
		private Label label5;
		private Label lblTsReTsRssiValue;
		private Label lblTsRssiValue;
		private Label label7;
		private Label label2;
		private Label label10;
		private Label label1;
		private Panel pnlHorizontalSeparator;

		private double TxTsTr
		{
			get
			{
				return txTsTr;
			}
			set
			{
				txTsTr = value;
				unit = "s";
				lblTsTrValue.Text = EngineeringNotation.ToString(txTsTr, ref unit);
				lblTsTrUnit.Text = unit;
			}
		}

		private double RxTsRe
		{
			get
			{
				return rxTsRe;
			}
			set
			{
				rxTsRe = value;
				unit = "s";
				lblTsReValue.Text = EngineeringNotation.ToString(rxTsRe, ref unit);
				lblTsReUnit.Text = unit;
			}
		}

		private double RxTsRssi
		{
			get
			{
				return rxTsRssi;
			}
			set
			{
				rxTsRssi = value;
				unit = "s";
				lblTsRssiValue.Text = EngineeringNotation.ToString(rxTsRssi, ref unit);
				lblTsRssiUnit.Text = unit;
			}
		}

		private double RxTsReTsRssi
		{
			get
			{
				return rxTsReTsRssi;
			}
			set
			{
				rxTsReTsRssi = value;
				unit = "s";
				lblTsReTsRssiValue.Text = EngineeringNotation.ToString(rxTsReTsRssi, ref unit);
				lblTsReTsRssiUnit.Text = unit;
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
				ComputeStartupTimmings();
			}
		}

		public RxTxStartupTimeForm()
		{
			InitializeComponent();
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

		private void ComputeStartupTimmings()
		{
			if (device.ModulationType == ModulationTypeEnum.OOK)
			{
				Tcf = 34.0 / (4.0 * (double)device.RxBw);
				TxTsTr = 5E-06 + 0.5 * (double)device.Tbit;
			}
			else
			{
				Tcf = 21.0 / (4.0 * (double)device.RxBw);
				double num = 4E-05;
				switch ((byte)device.PaRamp)
				{
					case (byte)0:
						num = 0.0034;
						break;
					case (byte)1:
						num = 0.002;
						break;
					case (byte)2:
						num = 0.001;
						break;
					case (byte)3:
						num = 0.0005;
						break;
					case (byte)4:
						num = 0.00025;
						break;
					case (byte)5:
						num = 0.000125;
						break;
					case (byte)6:
						num = 0.0001;
						break;
					case (byte)7:
						num = 6.2E-05;
						break;
					case (byte)8:
						num = 5E-05;
						break;
					case (byte)9:
						num = 4E-05;
						break;
					case (byte)10:
						num = 3.1E-05;
						break;
					case (byte)11:
						num = 2.5E-05;
						break;
					case (byte)12:
						num = 2E-05;
						break;
					case (byte)13:
						num = 1.5E-05;
						break;
					case (byte)14:
						num = 1.2E-05;
						break;
					case (byte)15:
						num = 1E-05;
						break;
				}
				TxTsTr = 5E-06 + 1.25 * num + 0.5 * (double)device.Tbit;
			}
			lblAgcOnValue.Text = device.AgcAutoOn ? "ON" : "OFF";
			lblAfcOnValue.Text = device.AfcAutoOn ? "ON" : "OFF";
			if (!device.AfcAutoOn)
			{
				int mant = 0;
				int exp = 0;
				SX1276.ComputeRxBwMantExp(device.FrequencyXo, device.ModulationType, device.RxBw, ref mant, ref exp);
				double num1 = 1.0 / (2.0 * (double)device.FrequencyXo / (double)mant);
				double num2 = 1.0 / (4.0 * (double)device.RxBw);
				RxTsRe = !(device.RxBw <= new Decimal(160000)) ? 2E-05 + 76.0 * num1 + 24.0 * num2 : 2E-05 + 12.0 * num1 + 24.0 * num2;
				RxTsRssi = (double)device.RssiSmoothing * num2;
				RxTsReTsRssi = RxTsRe + RxTsRssi;
			}
			else
			{
				int mant = 0;
				int exp = 0;
				SX1276.ComputeRxBwMantExp(device.FrequencyXo, device.ModulationType, device.AfcRxBw, ref mant, ref exp);
				double num1 = 1.0 / (2.0 * (double)device.FrequencyXo / (double)mant);
				double num2 = 1.0 / (4.0 * (double)device.AfcRxBw);
				RxTsRe = !(device.AfcRxBw <= new Decimal(160000)) ? 2E-05 + 76.0 * num1 + 24.0 * num2 : 2E-05 + 12.0 * num1 + 24.0 * num2;
				RxTsRssi = (double)device.RssiSmoothing * num2;
				RxTsReTsRssi = RxTsRe + RxTsRssi;
			}
		}

		private void OnDevicePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "FrequencyXo":
				case "Bitrate":
				case "PaRamp":
				case "RxBw":
				case "AfcRxBw":
				case "DccFastInitOn":
				case "DccForceOn":
				case "AgcAutoOn":
				case "AfcAutoOn":
					ComputeStartupTimmings();
					break;
			}
		}

		private void OnError(byte status, string message)
		{
			Refresh();
		}

		private void device_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (InvokeRequired)
				BeginInvoke((Delegate)new RxTxStartupTimeForm.DeviceDataChangedDelegate(OnDevicePropertyChanged), sender, (object)e);
			else
				OnDevicePropertyChanged(sender, e);
		}

		private void RxTxStartupTimeForm_Load(object sender, EventArgs e)
		{
			try
			{
				string s1 = appSettings.GetValue("RxTxStartupTimeTop");
				if (s1 != null)
				{
					try
					{
						Top = int.Parse(s1);
					}
					catch
					{
						int num = (int)MessageBox.Show((IWin32Window)this, "Error getting Top value.");
					}
				}
				string s2 = appSettings.GetValue("RxTxStartupTimeLeft");
				if (s2 != null)
				{
					try
					{
						Left = int.Parse(s2);
					}
					catch
					{
						int num = (int)MessageBox.Show((IWin32Window)this, "Error getting Left value.");
					}
				}
				Screen[] allScreens = Screen.AllScreens;
				if (IsFormLocatedInScreen((Form)this, allScreens))
					return;
				Top = allScreens[0].WorkingArea.Top;
				Left = allScreens[0].WorkingArea.Left;
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		private void RxTxStartupTimeForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			try
			{
				appSettings.SetValue("RxTxStartupTimeTop", Top.ToString());
				appSettings.SetValue("RxTxStartupTimeLeft", Left.ToString());
			}
			catch (Exception)
			{
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
			ComponentResourceManager resources = new ComponentResourceManager(typeof(RxTxStartupTimeForm));
			this.label4 = new Label();
			this.label3 = new Label();
			this.gBoxRxStartupTime = new GroupBoxEx();
			this.lblTsReUnit = new Label();
			this.lblTsReValue = new Label();
			this.lblTsRe = new Label();
			this.lblAgcOnValue = new Label();
			this.lblAfcOnValue = new Label();
			this.lblAfcOn = new Label();
			this.lblAgcOn = new Label();
			this.gBoxTxStartupTime = new GroupBoxEx();
			this.lblTsTrUnit = new Label();
			this.lblTsTrValue = new Label();
			this.lblTsTr = new Label();
			this.label1 = new Label();
			this.lblTsRssiValue = new Label();
			this.lblTsRssiUnit = new Label();
			this.label2 = new Label();
			this.label5 = new Label();
			this.label6 = new Label();
			this.label7 = new Label();
			this.label8 = new Label();
			this.label9 = new Label();
			this.label10 = new Label();
			this.lblTsReTsRssiValue = new Label();
			this.lblTsReTsRssiUnit = new Label();
			this.pnlHorizontalSeparator = new Panel();
			this.gBoxRxStartupTime.SuspendLayout();
			this.gBoxTxStartupTime.SuspendLayout();
			this.SuspendLayout();
			this.label4.AutoSize = true;
			this.label4.Location = new Point(60, 264);
			this.label4.Name = "label4";
			this.label4.Size = new Size(191, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "See drawings section 4.2 of datasheet.";
			this.label4.Visible = false;
			this.label3.AutoSize = true;
			this.label3.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
			this.label3.Location = new Point(16, 264);
			this.label3.Name = "label3";
			this.label3.Size = new Size(38, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Note:";
			this.label3.Visible = false;
			this.gBoxRxStartupTime.Controls.Add((Control)this.pnlHorizontalSeparator);
			this.gBoxRxStartupTime.Controls.Add((Control)this.label9);
			this.gBoxRxStartupTime.Controls.Add((Control)this.label6);
			this.gBoxRxStartupTime.Controls.Add((Control)this.lblTsReTsRssiUnit);
			this.gBoxRxStartupTime.Controls.Add((Control)this.lblTsRssiUnit);
			this.gBoxRxStartupTime.Controls.Add((Control)this.lblTsReUnit);
			this.gBoxRxStartupTime.Controls.Add((Control)this.label8);
			this.gBoxRxStartupTime.Controls.Add((Control)this.label5);
			this.gBoxRxStartupTime.Controls.Add((Control)this.lblTsReTsRssiValue);
			this.gBoxRxStartupTime.Controls.Add((Control)this.lblTsRssiValue);
			this.gBoxRxStartupTime.Controls.Add((Control)this.lblTsReValue);
			this.gBoxRxStartupTime.Controls.Add((Control)this.label7);
			this.gBoxRxStartupTime.Controls.Add((Control)this.label2);
			this.gBoxRxStartupTime.Controls.Add((Control)this.label10);
			this.gBoxRxStartupTime.Controls.Add((Control)this.label1);
			this.gBoxRxStartupTime.Controls.Add((Control)this.lblTsRe);
			this.gBoxRxStartupTime.Controls.Add((Control)this.lblAgcOnValue);
			this.gBoxRxStartupTime.Controls.Add((Control)this.lblAfcOnValue);
			this.gBoxRxStartupTime.Controls.Add((Control)this.lblAfcOn);
			this.gBoxRxStartupTime.Controls.Add((Control)this.lblAgcOn);
			this.gBoxRxStartupTime.Location = new Point(12, 81);
			this.gBoxRxStartupTime.Name = "gBoxRxStartupTime";
			this.gBoxRxStartupTime.Size = new Size(324, 180);
			this.gBoxRxStartupTime.TabIndex = 4;
			this.gBoxRxStartupTime.TabStop = false;
			this.gBoxRxStartupTime.Text = "Rx startup time";
			this.lblTsReUnit.AutoSize = true;
			this.lblTsReUnit.Location = new Point(275, 68);
			this.lblTsReUnit.Margin = new Padding(1);
			this.lblTsReUnit.MinimumSize = new Size(40, 20);
			this.lblTsReUnit.Name = "lblTsReUnit";
			this.lblTsReUnit.Size = new Size(40, 20);
			this.lblTsReUnit.TabIndex = 29;
			this.lblTsReUnit.Text = "μs";
			this.lblTsReUnit.TextAlign = ContentAlignment.MiddleLeft;
			this.lblTsReValue.AutoSize = true;
			this.lblTsReValue.Location = new Point(193, 68);
			this.lblTsReValue.Margin = new Padding(1);
			this.lblTsReValue.MinimumSize = new Size(80, 20);
			this.lblTsReValue.Name = "lblTsReValue";
			this.lblTsReValue.Size = new Size(80, 20);
			this.lblTsReValue.TabIndex = 28;
			this.lblTsReValue.Text = "0.000";
			this.lblTsReValue.TextAlign = ContentAlignment.MiddleRight;
			this.lblTsRe.AutoSize = true;
			this.lblTsRe.Location = new Point(4, 68);
			this.lblTsRe.Margin = new Padding(1);
			this.lblTsRe.MinimumSize = new Size(0, 20);
			this.lblTsRe.Name = "lblTsRe";
			this.lblTsRe.Size = new Size(45, 20);
			this.lblTsRe.TabIndex = 27;
			this.lblTsRe.Text = "TS_RE:";
			this.lblTsRe.TextAlign = ContentAlignment.MiddleLeft;
			this.lblAgcOnValue.AutoSize = true;
			this.lblAgcOnValue.Location = new Point(193, 17);
			this.lblAgcOnValue.Margin = new Padding(1);
			this.lblAgcOnValue.MinimumSize = new Size(80, 20);
			this.lblAgcOnValue.Name = "lblAgcOnValue";
			this.lblAgcOnValue.Size = new Size(80, 20);
			this.lblAgcOnValue.TabIndex = 25;
			this.lblAgcOnValue.Text = "OFF";
			this.lblAgcOnValue.TextAlign = ContentAlignment.MiddleRight;
			this.lblAfcOnValue.AutoSize = true;
			this.lblAfcOnValue.Location = new Point(193, 39);
			this.lblAfcOnValue.Margin = new Padding(1);
			this.lblAfcOnValue.MinimumSize = new Size(80, 20);
			this.lblAfcOnValue.Name = "lblAfcOnValue";
			this.lblAfcOnValue.Size = new Size(80, 20);
			this.lblAfcOnValue.TabIndex = 25;
			this.lblAfcOnValue.Text = "OFF";
			this.lblAfcOnValue.TextAlign = ContentAlignment.MiddleRight;
			this.lblAfcOn.AutoSize = true;
			this.lblAfcOn.Location = new Point(4, 39);
			this.lblAfcOn.Margin = new Padding(1);
			this.lblAfcOn.MinimumSize = new Size(0, 20);
			this.lblAfcOn.Name = "lblAfcOn";
			this.lblAfcOn.Size = new Size(30, 20);
			this.lblAfcOn.TabIndex = 24;
			this.lblAfcOn.Text = "AFC:";
			this.lblAfcOn.TextAlign = ContentAlignment.MiddleLeft;
			this.lblAgcOn.AutoSize = true;
			this.lblAgcOn.Location = new Point(4, 17);
			this.lblAgcOn.Margin = new Padding(1);
			this.lblAgcOn.MinimumSize = new Size(0, 20);
			this.lblAgcOn.Name = "lblAgcOn";
			this.lblAgcOn.Size = new Size(32, 20);
			this.lblAgcOn.TabIndex = 24;
			this.lblAgcOn.Text = "AGC:";
			this.lblAgcOn.TextAlign = ContentAlignment.MiddleLeft;
			this.gBoxTxStartupTime.Controls.Add((Control)this.lblTsTrUnit);
			this.gBoxTxStartupTime.Controls.Add((Control)this.lblTsTrValue);
			this.gBoxTxStartupTime.Controls.Add((Control)this.lblTsTr);
			this.gBoxTxStartupTime.Location = new Point(12, 12);
			this.gBoxTxStartupTime.Name = "gBoxTxStartupTime";
			this.gBoxTxStartupTime.Size = new Size(324, 63);
			this.gBoxTxStartupTime.TabIndex = 4;
			this.gBoxTxStartupTime.TabStop = false;
			this.gBoxTxStartupTime.Text = "Tx startup time";
			this.lblTsTrUnit.AutoSize = true;
			this.lblTsTrUnit.Location = new Point(275, 27);
			this.lblTsTrUnit.Margin = new Padding(1);
			this.lblTsTrUnit.MinimumSize = new Size(40, 20);
			this.lblTsTrUnit.Name = "lblTsTrUnit";
			this.lblTsTrUnit.Size = new Size(40, 20);
			this.lblTsTrUnit.TabIndex = 26;
			this.lblTsTrUnit.Text = "μs";
			this.lblTsTrUnit.TextAlign = ContentAlignment.MiddleLeft;
			this.lblTsTrValue.AutoSize = true;
			this.lblTsTrValue.Location = new Point(193, 27);
			this.lblTsTrValue.Margin = new Padding(1);
			this.lblTsTrValue.MinimumSize = new Size(80, 20);
			this.lblTsTrValue.Name = "lblTsTrValue";
			this.lblTsTrValue.Size = new Size(80, 20);
			this.lblTsTrValue.TabIndex = 25;
			this.lblTsTrValue.Text = "0.000";
			this.lblTsTrValue.TextAlign = ContentAlignment.MiddleRight;
			this.lblTsTr.AutoSize = true;
			this.lblTsTr.Location = new Point(4, 27);
			this.lblTsTr.Margin = new Padding(1);
			this.lblTsTr.MinimumSize = new Size(0, 20);
			this.lblTsTr.Name = "lblTsTr";
			this.lblTsTr.Size = new Size(45, 20);
			this.lblTsTr.TabIndex = 24;
			this.lblTsTr.Text = "TS_TR:";
			this.lblTsTr.TextAlign = ContentAlignment.MiddleLeft;
			this.label1.AutoSize = true;
			this.label1.Location = new Point(4, 90);
			this.label1.Margin = new Padding(1);
			this.label1.MinimumSize = new Size(0, 20);
			this.label1.Name = "label1";
			this.label1.Size = new Size(55, 20);
			this.label1.TabIndex = 27;
			this.label1.Text = "TS_RSSI:";
			this.label1.TextAlign = ContentAlignment.MiddleLeft;
			this.lblTsRssiValue.AutoSize = true;
			this.lblTsRssiValue.Location = new Point(193, 90);
			this.lblTsRssiValue.Margin = new Padding(1);
			this.lblTsRssiValue.MinimumSize = new Size(80, 20);
			this.lblTsRssiValue.Name = "lblTsRssiValue";
			this.lblTsRssiValue.Size = new Size(80, 20);
			this.lblTsRssiValue.TabIndex = 28;
			this.lblTsRssiValue.Text = "0.000";
			this.lblTsRssiValue.TextAlign = ContentAlignment.MiddleRight;
			this.lblTsRssiUnit.AutoSize = true;
			this.lblTsRssiUnit.Location = new Point(275, 90);
			this.lblTsRssiUnit.Margin = new Padding(1);
			this.lblTsRssiUnit.MinimumSize = new Size(40, 20);
			this.lblTsRssiUnit.Name = "lblTsRssiUnit";
			this.lblTsRssiUnit.Size = new Size(40, 20);
			this.lblTsRssiUnit.TabIndex = 29;
			this.lblTsRssiUnit.Text = "μs";
			this.lblTsRssiUnit.TextAlign = ContentAlignment.MiddleLeft;
			this.label2.AutoSize = true;
			this.label2.Location = new Point(4, 134);
			this.label2.Margin = new Padding(1);
			this.label2.MinimumSize = new Size(0, 20);
			this.label2.Name = "label2";
			this.label2.Size = new Size(43, 20);
			this.label2.TabIndex = 27;
			this.label2.Text = "TS_FS:";
			this.label2.TextAlign = ContentAlignment.MiddleLeft;
			this.label5.AutoSize = true;
			this.label5.Location = new Point(193, 134);
			this.label5.Margin = new Padding(1);
			this.label5.MinimumSize = new Size(80, 20);
			this.label5.Name = "label5";
			this.label5.Size = new Size(80, 20);
			this.label5.TabIndex = 28;
			this.label5.Text = "60.000";
			this.label5.TextAlign = ContentAlignment.MiddleRight;
			this.label6.AutoSize = true;
			this.label6.Location = new Point(275, 134);
			this.label6.Margin = new Padding(1);
			this.label6.MinimumSize = new Size(40, 20);
			this.label6.Name = "label6";
			this.label6.Size = new Size(40, 20);
			this.label6.TabIndex = 29;
			this.label6.Text = "μs";
			this.label6.TextAlign = ContentAlignment.MiddleLeft;
			this.label7.AutoSize = true;
			this.label7.Location = new Point(4, 156);
			this.label7.Margin = new Padding(1);
			this.label7.MinimumSize = new Size(0, 20);
			this.label7.Name = "label7";
			this.label7.Size = new Size(150, 20);
			this.label7.TabIndex = 27;
			this.label7.Text = "TS_OSC (depends on crystal):";
			this.label7.TextAlign = ContentAlignment.MiddleLeft;
			this.label8.AutoSize = true;
			this.label8.Location = new Point(193, 156);
			this.label8.Margin = new Padding(1);
			this.label8.MinimumSize = new Size(80, 20);
			this.label8.Name = "label8";
			this.label8.Size = new Size(80, 20);
			this.label8.TabIndex = 28;
			this.label8.Text = "250.000";
			this.label8.TextAlign = ContentAlignment.MiddleRight;
			this.label9.AutoSize = true;
			this.label9.Location = new Point(275, 156);
			this.label9.Margin = new Padding(1);
			this.label9.MinimumSize = new Size(40, 20);
			this.label9.Name = "label9";
			this.label9.Size = new Size(40, 20);
			this.label9.TabIndex = 29;
			this.label9.Text = "μs";
			this.label9.TextAlign = ContentAlignment.MiddleLeft;
			this.label10.AutoSize = true;
			this.label10.Location = new Point(4, 112);
			this.label10.Margin = new Padding(1);
			this.label10.MinimumSize = new Size(0, 20);
			this.label10.Name = "label10";
			this.label10.Size = new Size(139, 20);
			this.label10.TabIndex = 27;
			this.label10.Text = "TS_RE + TS_RSSI (+/-5%):";
			this.label10.TextAlign = ContentAlignment.MiddleLeft;
			this.lblTsReTsRssiValue.AutoSize = true;
			this.lblTsReTsRssiValue.Location = new Point(193, 112);
			this.lblTsReTsRssiValue.Margin = new Padding(1);
			this.lblTsReTsRssiValue.MinimumSize = new Size(80, 20);
			this.lblTsReTsRssiValue.Name = "lblTsReTsRssiValue";
			this.lblTsReTsRssiValue.Size = new Size(80, 20);
			this.lblTsReTsRssiValue.TabIndex = 28;
			this.lblTsReTsRssiValue.Text = "0.000";
			this.lblTsReTsRssiValue.TextAlign = ContentAlignment.MiddleRight;
			this.lblTsReTsRssiUnit.AutoSize = true;
			this.lblTsReTsRssiUnit.Location = new Point(275, 112);
			this.lblTsReTsRssiUnit.Margin = new Padding(1);
			this.lblTsReTsRssiUnit.MinimumSize = new Size(40, 20);
			this.lblTsReTsRssiUnit.Name = "lblTsReTsRssiUnit";
			this.lblTsReTsRssiUnit.Size = new Size(40, 20);
			this.lblTsReTsRssiUnit.TabIndex = 29;
			this.lblTsReTsRssiUnit.Text = "μs";
			this.lblTsReTsRssiUnit.TextAlign = ContentAlignment.MiddleLeft;
			this.pnlHorizontalSeparator.BorderStyle = BorderStyle.FixedSingle;
			this.pnlHorizontalSeparator.Location = new Point(7, 63);
			this.pnlHorizontalSeparator.Name = "pnlHorizontalSeparator";
			this.pnlHorizontalSeparator.Size = new Size(311, 1);
			this.pnlHorizontalSeparator.TabIndex = 30;
			this.AutoScaleDimensions = new SizeF(6f, 13f);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.ClientSize = new Size(347, 286);
			this.Controls.Add((Control)this.label4);
			this.Controls.Add((Control)this.label3);
			this.Controls.Add((Control)this.gBoxRxStartupTime);
			this.Controls.Add((Control)this.gBoxTxStartupTime);
			this.DoubleBuffered = true;
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.Icon = (Icon)resources.GetObject("$this.Icon");
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.Name = "RxTxStartupTimeForm";
			this.Text = "RxTx Startup Time";
			this.FormClosed += new FormClosedEventHandler(this.RxTxStartupTimeForm_FormClosed);
			this.Load += new EventHandler(this.RxTxStartupTimeForm_Load);
			this.gBoxRxStartupTime.ResumeLayout(false);
			this.gBoxRxStartupTime.PerformLayout();
			this.gBoxTxStartupTime.ResumeLayout(false);
			this.gBoxTxStartupTime.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		private delegate void DeviceDataChangedDelegate(object sender, PropertyChangedEventArgs e);
	}
}

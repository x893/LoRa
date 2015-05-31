using SemtechLib.Controls;
using SemtechLib.Devices.SX1276.Enumerations;
using SemtechLib.Devices.SX1276.Events;
using SemtechLib.General.Events;
using SemtechLib.General.Interfaces;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SemtechLib.Devices.SX1276.UI.Controls
{
	public class TransmitterViewControl : UserControl, INotifyDocumentationChanged
	{
		private Decimal maxOutputPower = new Decimal(132, 0, 0, false, (byte)1);
		private Decimal outputPower = new Decimal(132, 0, 0, false, (byte)1);
		private Decimal ocpTrim = new Decimal(100);
		private Decimal pllBandwidth;
		private IContainer components;
		private NumericUpDownEx nudOutputPower;
		private NumericUpDownEx nudOcpTrim;
		private ComboBox cBoxPaRamp;
		private Panel panel4;
		private RadioButton rBtnOcpOff;
		private RadioButton rBtnOcpOn;
		private Panel pnlPaSelect;
		private RadioButton rBtnRfPa;
		private RadioButton rBtnRfo;
		private Label suffixOutputPower;
		private Label suffixPAramp;
		private Label suffixOCPtrim;
		private Label label3;
		private Label label5;
		private ErrorProvider errorProvider;
		private GroupBoxEx gBoxPowerAmplifier;
		private GroupBoxEx gBoxOverloadCurrentProtection;
		private GroupBoxEx gBoxOutputPower;
		private GroupBoxEx groupBoxEx1;
		private NumericUpDownEx nudPllBandwidth;
		private Label label4;
		private Label label2;
		private NumericUpDownEx nudMaxOutputPower;
		private Label label7;
		private Label label6;
		private Label label1;
		private Panel pnlPa20dBm;
		private RadioButton rBtnPa20dBmOff;
		private RadioButton rBtnPa20dBmOn;
		private Label lblPa20dBm;

		public PaSelectEnum PaSelect
		{
			get
			{
				return rBtnRfo.Checked || !rBtnRfPa.Checked ? PaSelectEnum.RFO : PaSelectEnum.PA_BOOST;
			}
			set
			{
				rBtnRfo.CheckedChanged -= new EventHandler(rBtnPaControl_CheckedChanged);
				rBtnRfPa.CheckedChanged -= new EventHandler(rBtnPaControl_CheckedChanged);
				switch (value)
				{
					case PaSelectEnum.RFO:
						rBtnRfo.Checked = true;
						rBtnRfPa.Checked = false;
						nudMaxOutputPower.Enabled = true;
						break;
					case PaSelectEnum.PA_BOOST:
						rBtnRfo.Checked = false;
						rBtnRfPa.Checked = true;
						nudMaxOutputPower.Enabled = false;
						break;
				}
				rBtnRfo.CheckedChanged += new EventHandler(rBtnPaControl_CheckedChanged);
				rBtnRfPa.CheckedChanged += new EventHandler(rBtnPaControl_CheckedChanged);
			}
		}

		public Decimal MaxOutputPower
		{
			get
			{
				return maxOutputPower;
			}
			set
			{
				try
				{
					nudMaxOutputPower.ValueChanged -= new EventHandler(nudMaxOutputPower_ValueChanged);
					nudMaxOutputPower.BackColor = SystemColors.Window;
					if (PaSelect == PaSelectEnum.RFO)
					{
						nudMaxOutputPower.Maximum = new Decimal(150, 0, 0, false, (byte)1);
						nudMaxOutputPower.Minimum = new Decimal(108, 0, 0, false, (byte)1);
						maxOutputPower = new Decimal(108, 0, 0, false, (byte)1) + new Decimal(6, 0, 0, false, (byte)1) * (Decimal)((ushort)((int)((value - new Decimal(108, 0, 0, false, (byte)1)) / new Decimal(6, 0, 0, false, (byte)1)) & 7));
						nudMaxOutputPower.Value = maxOutputPower;
					}
					else if (!Pa20dBm)
					{
						maxOutputPower = new Decimal(170, 0, 0, false, (byte)1);
						nudMaxOutputPower.Maximum = maxOutputPower;
						nudMaxOutputPower.Minimum = maxOutputPower;
						nudMaxOutputPower.Value = maxOutputPower;
					}
					else
					{
						maxOutputPower = new Decimal(200, 0, 0, false, (byte)1);
						nudMaxOutputPower.Maximum = maxOutputPower;
						nudMaxOutputPower.Minimum = maxOutputPower;
						nudMaxOutputPower.Value = maxOutputPower;
					}
				}
				catch (Exception)
				{
					nudMaxOutputPower.BackColor = ControlPaint.LightLight(Color.Red);
				}
				finally
				{
					nudMaxOutputPower.ValueChanged += new EventHandler(nudMaxOutputPower_ValueChanged);
				}
			}
		}

		public Decimal OutputPower
		{
			get
			{
				return nudOutputPower.Value;
			}
			set
			{
				try
				{
					nudOutputPower.ValueChanged -= new EventHandler(nudOutputPower_ValueChanged);
					nudOutputPower.BackColor = SystemColors.Window;
					nudOutputPower.Maximum = MaxOutputPower;
					nudOutputPower.Minimum = MaxOutputPower - new Decimal(150, 0, 0, false, (byte)1);
					if (PaSelect == PaSelectEnum.RFO)
					{
						outputPower = MaxOutputPower - new Decimal(150, 0, 0, false, (byte)1) - (Decimal)((ushort)((int)(value - MaxOutputPower + new Decimal(150, 0, 0, false, (byte)1)) & 15));
						nudOutputPower.Value = outputPower;
					}
					else if (!Pa20dBm)
					{
						outputPower = new Decimal(17) - new Decimal(150, 0, 0, false, (byte)1) - (Decimal)((ushort)((int)(value - new Decimal(17) + new Decimal(150, 0, 0, false, (byte)1)) & 15));
						nudOutputPower.Value = outputPower;
					}
					else
					{
						outputPower = new Decimal(20) - new Decimal(150, 0, 0, false, (byte)1) - (Decimal)((ushort)((int)(value - new Decimal(20) + new Decimal(150, 0, 0, false, (byte)1)) & 15));
						nudOutputPower.Value = outputPower;
					}
				}
				catch (Exception)
				{
					nudOutputPower.BackColor = ControlPaint.LightLight(Color.Red);
				}
				finally
				{
					nudOutputPower.ValueChanged += new EventHandler(nudOutputPower_ValueChanged);
				}
			}
		}

		public PaRampEnum PaRamp
		{
			get
			{
				return (PaRampEnum)cBoxPaRamp.SelectedIndex;
			}
			set
			{
				cBoxPaRamp.SelectedIndexChanged -= new EventHandler(cBoxPaRamp_SelectedIndexChanged);
				cBoxPaRamp.SelectedIndex = (int)value;
				cBoxPaRamp.SelectedIndexChanged += new EventHandler(cBoxPaRamp_SelectedIndexChanged);
			}
		}

		public bool OcpOn
		{
			get
			{
				return rBtnOcpOn.Checked;
			}
			set
			{
				rBtnOcpOn.CheckedChanged -= new EventHandler(rBtnOcpOn_CheckedChanged);
				rBtnOcpOff.CheckedChanged -= new EventHandler(rBtnOcpOn_CheckedChanged);
				if (value)
				{
					rBtnOcpOn.Checked = true;
					rBtnOcpOff.Checked = false;
				}
				else
				{
					rBtnOcpOn.Checked = false;
					rBtnOcpOff.Checked = true;
				}
				rBtnOcpOn.CheckedChanged += new EventHandler(rBtnOcpOn_CheckedChanged);
				rBtnOcpOff.CheckedChanged += new EventHandler(rBtnOcpOn_CheckedChanged);
			}
		}

		public Decimal OcpTrim
		{
			get
			{
				return ocpTrim;
			}
			set
			{
				try
				{
					nudOcpTrim.ValueChanged -= new EventHandler(nudOcpTrim_ValueChanged);
					ocpTrim = !(value <= new Decimal(1200, 0, 0, false, (byte)1)) ? (!(value > new Decimal(120)) || !(value <= new Decimal(2400, 0, 0, false, (byte)1)) ? new Decimal(300, 0, 0, true, (byte)1) + new Decimal(100, 0, 0, false, (byte)1) * (Decimal)(ushort)27 : new Decimal(300, 0, 0, true, (byte)1) + new Decimal(100, 0, 0, false, (byte)1) * (Decimal)(ushort)Math.Round((value + new Decimal(300, 0, 0, false, (byte)1)) / new Decimal(100, 0, 0, false, (byte)1), MidpointRounding.AwayFromZero)) : new Decimal(450, 0, 0, false, (byte)1) + new Decimal(50, 0, 0, false, (byte)1) * (Decimal)(ushort)Math.Round((value - new Decimal(450, 0, 0, false, (byte)1)) / new Decimal(50, 0, 0, false, (byte)1), MidpointRounding.AwayFromZero);
					nudOcpTrim.Value = ocpTrim;
					nudOcpTrim.ValueChanged += new EventHandler(nudOcpTrim_ValueChanged);
				}
				catch (Exception)
				{
					nudOcpTrim.BackColor = ControlPaint.LightLight(Color.Red);
					nudOcpTrim.ValueChanged += new EventHandler(nudOcpTrim_ValueChanged);
				}
			}
		}

		public bool Pa20dBm
		{
			get
			{
				return rBtnPa20dBmOn.Checked;
			}
			set
			{
				rBtnPa20dBmOn.CheckedChanged -= new EventHandler(rBtnPa20dBm_CheckedChanged);
				rBtnPa20dBmOff.CheckedChanged -= new EventHandler(rBtnPa20dBm_CheckedChanged);
				if (value)
				{
					rBtnPa20dBmOn.Checked = true;
					rBtnPa20dBmOff.Checked = false;
					pnlPaSelect.Enabled = false;
				}
				else
				{
					rBtnPa20dBmOn.Checked = false;
					rBtnPa20dBmOff.Checked = true;
					pnlPaSelect.Enabled = true;
				}
				rBtnPa20dBmOn.CheckedChanged += new EventHandler(rBtnPa20dBm_CheckedChanged);
				rBtnPa20dBmOff.CheckedChanged += new EventHandler(rBtnPa20dBm_CheckedChanged);
			}
		}

		public Decimal PllBandwidth
		{
			get
			{
				return pllBandwidth;
			}
			set
			{
				try
				{
					nudPllBandwidth.ValueChanged -= new EventHandler(nudPllBandwidth_ValueChanged);
					nudPllBandwidth.Value = pllBandwidth = (Decimal)(((int)(ushort)Decimal.Subtract(value / new Decimal(75000), 1) + 1) * 75000);
				}
				finally
				{
					nudPllBandwidth.ValueChanged += new EventHandler(nudPllBandwidth_ValueChanged);
				}
			}
		}

		public event PaModeEventHandler PaModeChanged;

		public event DecimalEventHandler OutputPowerChanged;

		public event DecimalEventHandler MaxOutputPowerChanged;

		public event PaRampEventHandler PaRampChanged;

		public event BooleanEventHandler OcpOnChanged;

		public event DecimalEventHandler OcpTrimChanged;

		public event BooleanEventHandler Pa20dBmChanged;

		public event DecimalEventHandler PllBandwidthChanged;

		public event DocumentationChangedEventHandler DocumentationChanged;

		public TransmitterViewControl()
		{
			InitializeComponent();
		}

		private void OnPaModeChanged(PaSelectEnum value)
		{
			if (PaModeChanged == null)
				return;
			PaModeChanged((object)this, new PaModeEventArg(value));
		}

		private void OnMaxOutputPowerChanged(Decimal value)
		{
			if (MaxOutputPowerChanged == null)
				return;
			MaxOutputPowerChanged((object)this, new DecimalEventArg(value));
		}

		private void OnOutputPowerChanged(Decimal value)
		{
			if (OutputPowerChanged == null)
				return;
			OutputPowerChanged((object)this, new DecimalEventArg(value));
		}

		private void OnPaRampChanged(PaRampEnum value)
		{
			if (PaRampChanged == null)
				return;
			PaRampChanged((object)this, new PaRampEventArg(value));
		}

		private void OnOcpOnChanged(bool value)
		{
			if (OcpOnChanged == null)
				return;
			OcpOnChanged((object)this, new BooleanEventArg(value));
		}

		private void OnOcpTrimChanged(Decimal value)
		{
			if (OcpTrimChanged == null)
				return;
			OcpTrimChanged((object)this, new DecimalEventArg(value));
		}

		private void OnPa20dBmChanged(bool value)
		{
			if (Pa20dBmChanged == null)
				return;
			Pa20dBmChanged((object)this, new BooleanEventArg(value));
		}

		private void OnPllBandwidthChanged(Decimal value)
		{
			if (PllBandwidthChanged == null)
				return;
			PllBandwidthChanged((object)this, new DecimalEventArg(value));
		}

		public void UpdateOcpTrimLimits(LimitCheckStatusEnum status, string message)
		{
			switch (status)
			{
				case LimitCheckStatusEnum.OK:
					nudOcpTrim.BackColor = SystemColors.Window;
					break;
				case LimitCheckStatusEnum.OUT_OF_RANGE:
					nudOcpTrim.BackColor = ControlPaint.LightLight(Color.Orange);
					break;
				case LimitCheckStatusEnum.ERROR:
					nudOcpTrim.BackColor = ControlPaint.LightLight(Color.Red);
					break;
			}
			errorProvider.SetError((Control)nudOcpTrim, message);
		}

		private void rBtnPaControl_CheckedChanged(object sender, EventArgs e)
		{
			if (rBtnRfo.Checked)
				PaSelect = PaSelectEnum.RFO;
			else if (rBtnRfPa.Checked)
				PaSelect = PaSelectEnum.PA_BOOST;
			OnPaModeChanged(PaSelect);
		}

		private void nudMaxOutputPower_ValueChanged(object sender, EventArgs e)
		{
			MaxOutputPower = nudMaxOutputPower.Value;
			OnMaxOutputPowerChanged(MaxOutputPower);
		}

		private void nudOutputPower_ValueChanged(object sender, EventArgs e)
		{
			OutputPower = nudOutputPower.Value;
			OnOutputPowerChanged(OutputPower);
		}

		private void rBtnPa20dBm_CheckedChanged(object sender, EventArgs e)
		{
			Pa20dBm = rBtnPa20dBmOn.Checked;
			OnPa20dBmChanged(Pa20dBm);
		}

		private void cBoxPaRamp_SelectedIndexChanged(object sender, EventArgs e)
		{
			PaRamp = (PaRampEnum)cBoxPaRamp.SelectedIndex;
			OnPaRampChanged(PaRamp);
		}

		private void rBtnOcpOn_CheckedChanged(object sender, EventArgs e)
		{
			OcpOn = rBtnOcpOn.Checked;
			OnOcpOnChanged(OcpOn);
		}

		private void nudOcpTrim_ValueChanged(object sender, EventArgs e)
		{
			int num1;
			int num2;
			int num3;
			if (nudOcpTrim.Value <= new Decimal(1200, 0, 0, false, (byte)1))
			{
				num1 = (int)Math.Round((OcpTrim - new Decimal(450, 0, 0, false, (byte)1)) / new Decimal(50, 0, 0, false, (byte)1), MidpointRounding.AwayFromZero);
				num2 = (int)Math.Round((nudOcpTrim.Value - new Decimal(450, 0, 0, false, (byte)1)) / new Decimal(50, 0, 0, false, (byte)1), MidpointRounding.AwayFromZero);
				num3 = (int)(nudOcpTrim.Value - OcpTrim);
			}
			else if (nudOcpTrim.Value > new Decimal(120) && nudOcpTrim.Value <= new Decimal(2400, 0, 0, false, (byte)1))
			{
				num1 = (int)Math.Round((OcpTrim + new Decimal(300, 0, 0, false, (byte)1)) / new Decimal(100, 0, 0, false, (byte)1), MidpointRounding.AwayFromZero);
				num2 = (int)Math.Round((nudOcpTrim.Value + new Decimal(300, 0, 0, false, (byte)1)) / new Decimal(100, 0, 0, false, (byte)1), MidpointRounding.AwayFromZero);
				num3 = (int)(nudOcpTrim.Value - OcpTrim);
			}
			else
			{
				num1 = (int)Math.Round(new Decimal(27), MidpointRounding.AwayFromZero);
				num2 = (int)Math.Round((nudOcpTrim.Value + new Decimal(300, 0, 0, false, (byte)1)) / new Decimal(100, 0, 0, false, (byte)1), MidpointRounding.AwayFromZero);
				num3 = (int)(nudOcpTrim.Value - new Decimal(2400, 0, 0, false, (byte)1));
			}
			if (num3 >= -1 && num3 <= 1 && num1 == num2)
			{
				nudOcpTrim.ValueChanged -= new EventHandler(nudOcpTrim_ValueChanged);
				if (nudOcpTrim.Value <= new Decimal(1200, 0, 0, false, (byte)1))
					nudOcpTrim.Value = new Decimal(450, 0, 0, false, (byte)1) + new Decimal(50, 0, 0, false, (byte)1) * (Decimal)(num2 + num3);
				else if (nudOcpTrim.Value > new Decimal(120) && nudOcpTrim.Value <= new Decimal(2400, 0, 0, false, (byte)1))
					nudOcpTrim.Value = new Decimal(300, 0, 0, true, (byte)1) + new Decimal(100, 0, 0, false, (byte)1) * (Decimal)(num2 + num3);
				else
					nudOcpTrim.Value = new Decimal(2400, 0, 0, false, (byte)1);
				nudOcpTrim.ValueChanged += new EventHandler(nudOcpTrim_ValueChanged);
			}
			OcpTrim = nudOcpTrim.Value;
			OnOcpTrimChanged(OcpTrim);
		}

		private void nudPllBandwidth_ValueChanged(object sender, EventArgs e)
		{
			int num1 = (int)Decimal.Subtract(PllBandwidth / new Decimal(75000), 1);
			int num2 = (int)Decimal.Subtract(nudPllBandwidth.Value / new Decimal(75000), 1);
			int num3 = (int)(nudPllBandwidth.Value - PllBandwidth);
			nudPllBandwidth.ValueChanged -= new EventHandler(nudPllBandwidth_ValueChanged);
			if (num1 == 0)
				num3 = 0;
			if (num3 >= 0 && num3 <= 1)
				nudPllBandwidth.Value = (Decimal)((num2 + num3 + 1) * 75000);
			else
				nudPllBandwidth.Value = (Decimal)((num2 + 1) * 75000);
			nudPllBandwidth.ValueChanged += new EventHandler(nudPllBandwidth_ValueChanged);
			PllBandwidth = nudPllBandwidth.Value;
			OnPllBandwidthChanged(PllBandwidth);
		}

		private void control_MouseEnter(object sender, EventArgs e)
		{
			if (sender == gBoxPowerAmplifier)
				OnDocumentationChanged(new DocumentationChangedEventArgs("Transmitter", "Power amplifier"));
			else if (sender == gBoxOutputPower)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Transmitter", "Output power"));
			}
			else
			{
				if (sender != gBoxOverloadCurrentProtection)
					return;
				OnDocumentationChanged(new DocumentationChangedEventArgs("Transmitter", "Overload current protection"));
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
			this.errorProvider = new ErrorProvider(components);
			this.nudOcpTrim = new NumericUpDownEx();
			this.gBoxOverloadCurrentProtection = new GroupBoxEx();
			this.panel4 = new Panel();
			this.rBtnOcpOff = new RadioButton();
			this.rBtnOcpOn = new RadioButton();
			this.label5 = new Label();
			this.suffixOCPtrim = new Label();
			this.gBoxOutputPower = new GroupBoxEx();
			this.pnlPa20dBm = new Panel();
			this.rBtnPa20dBmOff = new RadioButton();
			this.rBtnPa20dBmOn = new RadioButton();
			this.lblPa20dBm = new Label();
			this.nudMaxOutputPower = new NumericUpDownEx();
			this.label7 = new Label();
			this.nudOutputPower = new NumericUpDownEx();
			this.label6 = new Label();
			this.label1 = new Label();
			this.suffixOutputPower = new Label();
			this.gBoxPowerAmplifier = new GroupBoxEx();
			this.cBoxPaRamp = new ComboBox();
			this.pnlPaSelect = new Panel();
			this.rBtnRfPa = new RadioButton();
			this.rBtnRfo = new RadioButton();
			this.suffixPAramp = new Label();
			this.label3 = new Label();
			this.groupBoxEx1 = new GroupBoxEx();
			this.nudPllBandwidth = new NumericUpDownEx();
			this.label4 = new Label();
			this.label2 = new Label();
			((ISupportInitialize)this.errorProvider).BeginInit();
			this.nudOcpTrim.BeginInit();
			this.gBoxOverloadCurrentProtection.SuspendLayout();
			this.panel4.SuspendLayout();
			this.gBoxOutputPower.SuspendLayout();
			this.pnlPa20dBm.SuspendLayout();
			this.nudMaxOutputPower.BeginInit();
			this.nudOutputPower.BeginInit();
			this.gBoxPowerAmplifier.SuspendLayout();
			this.pnlPaSelect.SuspendLayout();
			this.groupBoxEx1.SuspendLayout();
			this.nudPllBandwidth.BeginInit();
			this.SuspendLayout();
			this.errorProvider.ContainerControl = (ContainerControl)this;
			this.errorProvider.SetIconPadding((Control)this.nudOcpTrim, 30);
			this.nudOcpTrim.Location = new Point(192, 45);
			NumericUpDownEx numericUpDownEx1 = this.nudOcpTrim;
			int[] bits1 = new int[4];
			bits1[0] = 240;
			Decimal num1 = new Decimal(bits1);
			numericUpDownEx1.Maximum = num1;
			NumericUpDownEx numericUpDownEx2 = this.nudOcpTrim;
			int[] bits2 = new int[4];
			bits2[0] = 45;
			Decimal num2 = new Decimal(bits2);
			numericUpDownEx2.Minimum = num2;
			this.nudOcpTrim.Name = "nudOcpTrim";
			this.nudOcpTrim.Size = new Size(124, 20);
			this.nudOcpTrim.TabIndex = 2;
			this.nudOcpTrim.ThousandsSeparator = true;
			NumericUpDownEx numericUpDownEx3 = this.nudOcpTrim;
			int[] bits3 = new int[4];
			bits3[0] = 100;
			Decimal num3 = new Decimal(bits3);
			numericUpDownEx3.Value = num3;
			this.nudOcpTrim.ValueChanged += new EventHandler(this.nudOcpTrim_ValueChanged);
			this.gBoxOverloadCurrentProtection.Controls.Add((Control)this.panel4);
			this.gBoxOverloadCurrentProtection.Controls.Add((Control)this.label5);
			this.gBoxOverloadCurrentProtection.Controls.Add((Control)this.nudOcpTrim);
			this.gBoxOverloadCurrentProtection.Controls.Add((Control)this.suffixOCPtrim);
			this.gBoxOverloadCurrentProtection.Location = new Point(217, 301);
			this.gBoxOverloadCurrentProtection.Name = "gBoxOverloadCurrentProtection";
			this.gBoxOverloadCurrentProtection.Size = new Size(364, 69);
			this.gBoxOverloadCurrentProtection.TabIndex = 2;
			this.gBoxOverloadCurrentProtection.TabStop = false;
			this.gBoxOverloadCurrentProtection.Text = "Overload current protection";
			this.gBoxOverloadCurrentProtection.MouseEnter += new EventHandler(this.control_MouseEnter);
			this.gBoxOverloadCurrentProtection.MouseLeave += new EventHandler(this.control_MouseLeave);
			this.panel4.AutoSize = true;
			this.panel4.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel4.Controls.Add((Control)this.rBtnOcpOff);
			this.panel4.Controls.Add((Control)this.rBtnOcpOn);
			this.panel4.Location = new Point(192, 19);
			this.panel4.Name = "panel4";
			this.panel4.Size = new Size(102, 20);
			this.panel4.TabIndex = 0;
			this.rBtnOcpOff.AutoSize = true;
			this.rBtnOcpOff.Location = new Point(54, 3);
			this.rBtnOcpOff.Margin = new Padding(3, 0, 3, 0);
			this.rBtnOcpOff.Name = "rBtnOcpOff";
			this.rBtnOcpOff.Size = new Size(45, 17);
			this.rBtnOcpOff.TabIndex = 1;
			this.rBtnOcpOff.Text = "OFF";
			this.rBtnOcpOff.UseVisualStyleBackColor = true;
			this.rBtnOcpOff.CheckedChanged += new EventHandler(this.rBtnOcpOn_CheckedChanged);
			this.rBtnOcpOn.AutoSize = true;
			this.rBtnOcpOn.Checked = true;
			this.rBtnOcpOn.Location = new Point(3, 3);
			this.rBtnOcpOn.Margin = new Padding(3, 0, 3, 0);
			this.rBtnOcpOn.Name = "rBtnOcpOn";
			this.rBtnOcpOn.Size = new Size(41, 17);
			this.rBtnOcpOn.TabIndex = 0;
			this.rBtnOcpOn.TabStop = true;
			this.rBtnOcpOn.Text = "ON";
			this.rBtnOcpOn.UseVisualStyleBackColor = true;
			this.rBtnOcpOn.CheckedChanged += new EventHandler(this.rBtnOcpOn_CheckedChanged);
			this.label5.AutoSize = true;
			this.label5.Location = new Point(6, 49);
			this.label5.Name = "label5";
			this.label5.Size = new Size(52, 13);
			this.label5.TabIndex = 1;
			this.label5.Text = "Trimming:";
			this.suffixOCPtrim.AutoSize = true;
			this.suffixOCPtrim.Location = new Point(322, 49);
			this.suffixOCPtrim.Name = "suffixOCPtrim";
			this.suffixOCPtrim.Size = new Size(22, 13);
			this.suffixOCPtrim.TabIndex = 3;
			this.suffixOCPtrim.Text = "mA";
			this.gBoxOutputPower.Controls.Add((Control)this.pnlPa20dBm);
			this.gBoxOutputPower.Controls.Add((Control)this.lblPa20dBm);
			this.gBoxOutputPower.Controls.Add((Control)this.nudMaxOutputPower);
			this.gBoxOutputPower.Controls.Add((Control)this.label7);
			this.gBoxOutputPower.Controls.Add((Control)this.nudOutputPower);
			this.gBoxOutputPower.Controls.Add((Control)this.label6);
			this.gBoxOutputPower.Controls.Add((Control)this.label1);
			this.gBoxOutputPower.Controls.Add((Control)this.suffixOutputPower);
			this.gBoxOutputPower.Location = new Point(217, 194);
			this.gBoxOutputPower.Name = "gBoxOutputPower";
			this.gBoxOutputPower.Size = new Size(364, 101);
			this.gBoxOutputPower.TabIndex = 1;
			this.gBoxOutputPower.TabStop = false;
			this.gBoxOutputPower.Text = "Output power";
			this.gBoxOutputPower.MouseEnter += new EventHandler(this.control_MouseEnter);
			this.gBoxOutputPower.MouseLeave += new EventHandler(this.control_MouseLeave);
			this.pnlPa20dBm.AutoSize = true;
			this.pnlPa20dBm.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.pnlPa20dBm.Controls.Add((Control)this.rBtnPa20dBmOff);
			this.pnlPa20dBm.Controls.Add((Control)this.rBtnPa20dBmOn);
			this.pnlPa20dBm.Location = new Point(192, 71);
			this.pnlPa20dBm.Name = "pnlPa20dBm";
			this.pnlPa20dBm.Size = new Size(102, 20);
			this.pnlPa20dBm.TabIndex = 4;
			this.rBtnPa20dBmOff.AutoSize = true;
			this.rBtnPa20dBmOff.Checked = true;
			this.rBtnPa20dBmOff.Location = new Point(54, 3);
			this.rBtnPa20dBmOff.Margin = new Padding(3, 0, 3, 0);
			this.rBtnPa20dBmOff.Name = "rBtnPa20dBmOff";
			this.rBtnPa20dBmOff.Size = new Size(45, 17);
			this.rBtnPa20dBmOff.TabIndex = 1;
			this.rBtnPa20dBmOff.TabStop = true;
			this.rBtnPa20dBmOff.Text = "OFF";
			this.rBtnPa20dBmOff.UseVisualStyleBackColor = true;
			this.rBtnPa20dBmOff.CheckedChanged += new EventHandler(this.rBtnPa20dBm_CheckedChanged);
			this.rBtnPa20dBmOn.AutoSize = true;
			this.rBtnPa20dBmOn.Location = new Point(3, 3);
			this.rBtnPa20dBmOn.Margin = new Padding(3, 0, 3, 0);
			this.rBtnPa20dBmOn.Name = "rBtnPa20dBmOn";
			this.rBtnPa20dBmOn.Size = new Size(41, 17);
			this.rBtnPa20dBmOn.TabIndex = 0;
			this.rBtnPa20dBmOn.Text = "ON";
			this.rBtnPa20dBmOn.UseVisualStyleBackColor = true;
			this.rBtnPa20dBmOn.CheckedChanged += new EventHandler(this.rBtnPa20dBm_CheckedChanged);
			this.lblPa20dBm.AutoSize = true;
			this.lblPa20dBm.Location = new Point(6, 75);
			this.lblPa20dBm.Name = "lblPa20dBm";
			this.lblPa20dBm.Size = new Size(144, 13);
			this.lblPa20dBm.TabIndex = 5;
			this.lblPa20dBm.Text = "+20 dBm on pin PA_BOOST:";
			this.nudMaxOutputPower.DecimalPlaces = 1;
			this.nudMaxOutputPower.Increment = new Decimal(new int[4]
      {
        6,
        0,
        0,
        65536
      });
			this.nudMaxOutputPower.Location = new Point(192, 19);
			NumericUpDownEx numericUpDownEx4 = this.nudMaxOutputPower;
			int[] bits4 = new int[4];
			bits4[0] = 15;
			Decimal num4 = new Decimal(bits4);
			numericUpDownEx4.Maximum = num4;
			this.nudMaxOutputPower.Minimum = new Decimal(new int[4]
      {
        108,
        0,
        0,
        65536
      });
			this.nudMaxOutputPower.Name = "nudMaxOutputPower";
			this.nudMaxOutputPower.Size = new Size(124, 20);
			this.nudMaxOutputPower.TabIndex = 0;
			this.nudMaxOutputPower.ThousandsSeparator = true;
			this.nudMaxOutputPower.Value = new Decimal(new int[4]
      {
        132,
        0,
        0,
        65536
      });
			this.nudMaxOutputPower.ValueChanged += new EventHandler(this.nudMaxOutputPower_ValueChanged);
			this.label7.AutoSize = true;
			this.label7.Location = new Point(6, 23);
			this.label7.Name = "label7";
			this.label7.Size = new Size(119, 13);
			this.label7.TabIndex = 1;
			this.label7.Text = "Maximum output power:";
			this.nudOutputPower.DecimalPlaces = 1;
			this.nudOutputPower.Location = new Point(192, 45);
			this.nudOutputPower.Maximum = new Decimal(new int[4]
      {
        132,
        0,
        0,
        65536
      });
			this.nudOutputPower.Minimum = new Decimal(new int[4]
      {
        18,
        0,
        0,
        -2147418112
      });
			this.nudOutputPower.Name = "nudOutputPower";
			this.nudOutputPower.Size = new Size(124, 20);
			this.nudOutputPower.TabIndex = 0;
			this.nudOutputPower.ThousandsSeparator = true;
			this.nudOutputPower.Value = new Decimal(new int[4]
      {
        132,
        0,
        0,
        65536
      });
			this.nudOutputPower.ValueChanged += new EventHandler(this.nudOutputPower_ValueChanged);
			this.label6.AutoSize = true;
			this.label6.Location = new Point(322, 23);
			this.label6.Name = "label6";
			this.label6.Size = new Size(28, 13);
			this.label6.TabIndex = 1;
			this.label6.Text = "dBm";
			this.label1.AutoSize = true;
			this.label1.Location = new Point(6, 49);
			this.label1.Name = "label1";
			this.label1.Size = new Size(74, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Output power:";
			this.suffixOutputPower.AutoSize = true;
			this.suffixOutputPower.Location = new Point(322, 49);
			this.suffixOutputPower.Name = "suffixOutputPower";
			this.suffixOutputPower.Size = new Size(28, 13);
			this.suffixOutputPower.TabIndex = 1;
			this.suffixOutputPower.Text = "dBm";
			this.gBoxPowerAmplifier.Controls.Add((Control)this.cBoxPaRamp);
			this.gBoxPowerAmplifier.Controls.Add((Control)this.pnlPaSelect);
			this.gBoxPowerAmplifier.Controls.Add((Control)this.suffixPAramp);
			this.gBoxPowerAmplifier.Controls.Add((Control)this.label3);
			this.gBoxPowerAmplifier.Location = new Point(217, 69);
			this.gBoxPowerAmplifier.Name = "gBoxPowerAmplifier";
			this.gBoxPowerAmplifier.Size = new Size(364, 119);
			this.gBoxPowerAmplifier.TabIndex = 0;
			this.gBoxPowerAmplifier.TabStop = false;
			this.gBoxPowerAmplifier.Text = "Power Amplifier";
			this.gBoxPowerAmplifier.MouseEnter += new EventHandler(this.control_MouseEnter);
			this.gBoxPowerAmplifier.MouseLeave += new EventHandler(this.control_MouseLeave);
			this.cBoxPaRamp.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxPaRamp.Items.AddRange(new object[16]
      {
        (object) "3400",
        (object) "2000",
        (object) "1000",
        (object) "500",
        (object) "250",
        (object) "125",
        (object) "100",
        (object) "62",
        (object) "50",
        (object) "40",
        (object) "31",
        (object) "25",
        (object) "20",
        (object) "15",
        (object) "12",
        (object) "10"
      });
			this.cBoxPaRamp.Location = new Point(192, 94);
			this.cBoxPaRamp.Name = "cBoxPaRamp";
			this.cBoxPaRamp.Size = new Size(124, 21);
			this.cBoxPaRamp.TabIndex = 2;
			this.cBoxPaRamp.SelectedIndexChanged += new EventHandler(this.cBoxPaRamp_SelectedIndexChanged);
			this.pnlPaSelect.AutoSize = true;
			this.pnlPaSelect.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.pnlPaSelect.Controls.Add((Control)this.rBtnRfPa);
			this.pnlPaSelect.Controls.Add((Control)this.rBtnRfo);
			this.pnlPaSelect.Location = new Point(65, 19);
			this.pnlPaSelect.Name = "pnlPaSelect";
			this.pnlPaSelect.Size = new Size(203, 46);
			this.pnlPaSelect.TabIndex = 0;
			this.rBtnRfPa.AutoSize = true;
			this.rBtnRfPa.Location = new Point(3, 26);
			this.rBtnRfPa.Name = "rBtnRfPa";
			this.rBtnRfPa.Size = new Size(197, 17);
			this.rBtnRfPa.TabIndex = 1;
			this.rBtnRfPa.Text = "PA1 -> Transmits on pin PA_BOOST";
			this.rBtnRfPa.UseVisualStyleBackColor = true;
			this.rBtnRfPa.CheckedChanged += new EventHandler(this.rBtnPaControl_CheckedChanged);
			this.rBtnRfo.AutoSize = true;
			this.rBtnRfo.Checked = true;
			this.rBtnRfo.Location = new Point(3, 3);
			this.rBtnRfo.Name = "rBtnRfo";
			this.rBtnRfo.Size = new Size(162, 17);
			this.rBtnRfo.TabIndex = 0;
			this.rBtnRfo.TabStop = true;
			this.rBtnRfo.Text = "PA0 -> Transmits on pin RFO";
			this.rBtnRfo.UseVisualStyleBackColor = true;
			this.rBtnRfo.CheckedChanged += new EventHandler(this.rBtnPaControl_CheckedChanged);
			this.suffixPAramp.AutoSize = true;
			this.suffixPAramp.Location = new Point(322, 98);
			this.suffixPAramp.Name = "suffixPAramp";
			this.suffixPAramp.Size = new Size(18, 13);
			this.suffixPAramp.TabIndex = 3;
			this.suffixPAramp.Text = "µs";
			this.label3.AutoSize = true;
			this.label3.Location = new Point(6, 98);
			this.label3.Name = "label3";
			this.label3.Size = new Size(50, 13);
			this.label3.TabIndex = 1;
			this.label3.Text = "PA ramp:";
			this.groupBoxEx1.Controls.Add((Control)this.nudPllBandwidth);
			this.groupBoxEx1.Controls.Add((Control)this.label4);
			this.groupBoxEx1.Controls.Add((Control)this.label2);
			this.groupBoxEx1.Location = new Point(217, 376);
			this.groupBoxEx1.Name = "groupBoxEx1";
			this.groupBoxEx1.Size = new Size(364, 48);
			this.groupBoxEx1.TabIndex = 3;
			this.groupBoxEx1.TabStop = false;
			this.groupBoxEx1.Text = "PLL bandwidth";
			NumericUpDownEx numericUpDownEx5 = this.nudPllBandwidth;
			int[] bits5 = new int[4];
			bits5[0] = 75000;
			Decimal num5 = new Decimal(bits5);
			numericUpDownEx5.Increment = num5;
			this.nudPllBandwidth.Location = new Point(192, 19);
			NumericUpDownEx numericUpDownEx6 = this.nudPllBandwidth;
			int[] bits6 = new int[4];
			bits6[0] = 300000;
			Decimal num6 = new Decimal(bits6);
			numericUpDownEx6.Maximum = num6;
			NumericUpDownEx numericUpDownEx7 = this.nudPllBandwidth;
			int[] bits7 = new int[4];
			bits7[0] = 75000;
			Decimal num7 = new Decimal(bits7);
			numericUpDownEx7.Minimum = num7;
			this.nudPllBandwidth.Name = "nudPllBandwidth";
			this.nudPllBandwidth.Size = new Size(124, 20);
			this.nudPllBandwidth.TabIndex = 2;
			this.nudPllBandwidth.ThousandsSeparator = true;
			NumericUpDownEx numericUpDownEx8 = this.nudPllBandwidth;
			int[] bits8 = new int[4];
			bits8[0] = 300000;
			Decimal num8 = new Decimal(bits8);
			numericUpDownEx8.Value = num8;
			this.nudPllBandwidth.ValueChanged += new EventHandler(this.nudPllBandwidth_ValueChanged);
			this.label4.AutoSize = true;
			this.label4.Location = new Point(6, 23);
			this.label4.Name = "label4";
			this.label4.Size = new Size(29, 13);
			this.label4.TabIndex = 1;
			this.label4.Text = "PLL:";
			this.label2.AutoSize = true;
			this.label2.Location = new Point(322, 23);
			this.label2.Name = "label2";
			this.label2.Size = new Size(20, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Hz";
			this.AutoScaleDimensions = new SizeF(6f, 13f);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.Controls.Add((Control)this.groupBoxEx1);
			this.Controls.Add((Control)this.gBoxOverloadCurrentProtection);
			this.Controls.Add((Control)this.gBoxOutputPower);
			this.Controls.Add((Control)this.gBoxPowerAmplifier);
			this.Name = "TransmitterViewControl";
			this.Size = new Size(799, 493);
			((ISupportInitialize)this.errorProvider).EndInit();
			this.nudOcpTrim.EndInit();
			this.gBoxOverloadCurrentProtection.ResumeLayout(false);
			this.gBoxOverloadCurrentProtection.PerformLayout();
			this.panel4.ResumeLayout(false);
			this.panel4.PerformLayout();
			this.gBoxOutputPower.ResumeLayout(false);
			this.gBoxOutputPower.PerformLayout();
			this.pnlPa20dBm.ResumeLayout(false);
			this.pnlPa20dBm.PerformLayout();
			this.nudMaxOutputPower.EndInit();
			this.nudOutputPower.EndInit();
			this.gBoxPowerAmplifier.ResumeLayout(false);
			this.gBoxPowerAmplifier.PerformLayout();
			this.pnlPaSelect.ResumeLayout(false);
			this.pnlPaSelect.PerformLayout();
			this.groupBoxEx1.ResumeLayout(false);
			this.groupBoxEx1.PerformLayout();
			this.nudPllBandwidth.EndInit();
			this.ResumeLayout(false);
		}
	}
}
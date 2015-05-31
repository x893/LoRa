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
	public class CommonViewControl : UserControl, INotifyDocumentationChanged
	{
		private Decimal bitRate = new Decimal(4800);
		private IContainer components;
		private Button btnRcCalibration;
		private NumericUpDownEx nudFrequencyXo;
		private ComboBox cBoxLowBatTrim;
		private NumericUpDownEx nudBitrate;
		private NumericUpDownEx nudFrequencyRf;
		private NumericUpDownEx nudFdev;
		private Panel panel4;
		private RadioButton rBtnLowBatOff;
		private RadioButton rBtnLowBatOn;
		private Label label1;
		private Panel panel2;
		private RadioButton rBtnModulationTypeOok;
		private RadioButton rBtnModulationTypeFsk;
		private Panel panel3;
		private RadioButton rBtnModulationShaping11;
		private RadioButton rBtnModulationShaping10;
		private RadioButton rBtnModulationShaping01;
		private RadioButton rBtnModulationShapingOff;
		private Label label5;
		private Label label7;
		private Label label6;
		private Label label10;
		private Label label9;
		private Label label14;
		private Label label16;
		private Label lblRcOscillatorCalStat;
		private Label lblRcOscillatorCal;
		private Label label15;
		private Label label13;
		private Label label17;
		private Label label11;
		private Label label8;
		private Label label12;
		private Label label20;
		private Panel panel6;
		private Label label21;
		private Label label22;
		private Panel panel7;
		private Label label23;
		private Label label24;
		private Label label25;
		private Label label26;
		private Label label27;
		private Label label28;
		private ErrorProvider errorProvider;
		private Label label30;
		private Label lblListenResolRx;
		private GroupBoxEx gBoxOscillators;
		private GroupBoxEx gBoxModulation;
		private GroupBoxEx gBoxGeneral;
		private GroupBoxEx gBoxBatteryManagement;
		private NumericUpDownEx nudBitrateFrac;
		private Label label2;
		private Panel panel1;
		private RadioButton rBtnTcxoInputOff;
		private RadioButton rBtnTcxoInputOn;
		private Label label3;
		private Panel panel5;
		private RadioButton rBtnFastHopOff;
		private RadioButton rBtnFastHopOn;
		private Panel panel8;
		private RadioButton rBtnLowFrequencyModeOff;
		private RadioButton rBtnLowFrequencyModeOn;
		private ComboBox cBoxBand;
		private Label label19;
		private Label label18;
		private Label label4;
		private GroupBoxEx groupBoxEx1;
		private Panel panel10;
		private RadioButton rBtnForceRxBandLowFrequencyOff;
		private RadioButton rBtnForceRxBandLowFrequencyOn;
		private Label label31;
		private Panel panel9;
		private RadioButton rBtnForceTxBandLowFrequencyOff;
		private RadioButton rBtnForceTxBandLowFrequencyOn;
		private Label label29;
		private GroupBoxEx groupBoxEx2;

		public Decimal FrequencyXo
		{
			get
			{
				return nudFrequencyXo.Value;
			}
			set
			{
				nudFrequencyXo.ValueChanged -= new EventHandler(nudFrequencyXo_ValueChanged);
				nudFrequencyXo.Value = value;
				nudFrequencyXo.ValueChanged += new EventHandler(nudFrequencyXo_ValueChanged);
			}
		}

		public Decimal FrequencyStep
		{
			get
			{
				return nudFrequencyRf.Increment;
			}
			set
			{
				nudFrequencyRf.Increment = value;
				nudFdev.Increment = value;
			}
		}

		public ModulationTypeEnum ModulationType
		{
			get
			{
				return rBtnModulationTypeFsk.Checked || !rBtnModulationTypeOok.Checked ? ModulationTypeEnum.FSK : ModulationTypeEnum.OOK;
			}
			set
			{
				rBtnModulationTypeFsk.CheckedChanged -= new EventHandler(rBtnModulationType_CheckedChanged);
				rBtnModulationTypeOok.CheckedChanged -= new EventHandler(rBtnModulationType_CheckedChanged);
				switch (value)
				{
					case ModulationTypeEnum.FSK:
						rBtnModulationTypeFsk.Checked = true;
						rBtnModulationTypeOok.Checked = false;
						rBtnModulationShapingOff.Text = "OFF";
						rBtnModulationShaping01.Text = "Gaussian filter, BT = 1.0";
						rBtnModulationShaping10.Text = "Gaussian filter, BT = 0.5";
						rBtnModulationShaping11.Text = "Gaussian filter, BT = 0.3";
						rBtnModulationShaping11.Visible = true;
						nudBitrateFrac.Enabled = true;
						break;
					case ModulationTypeEnum.OOK:
						rBtnModulationTypeFsk.Checked = false;
						rBtnModulationTypeOok.Checked = true;
						rBtnModulationShapingOff.Text = "OFF";
						rBtnModulationShaping01.Text = "Filtering with fCutOff = BR";
						rBtnModulationShaping10.Text = "Filtering with fCutOff = 2 * BR";
						rBtnModulationShaping11.Text = "Unused";
						rBtnModulationShaping11.Visible = false;
						nudBitrateFrac.Enabled = false;
						break;
				}
				rBtnModulationTypeFsk.CheckedChanged += new EventHandler(rBtnModulationType_CheckedChanged);
				rBtnModulationTypeOok.CheckedChanged += new EventHandler(rBtnModulationType_CheckedChanged);
			}
		}

		public byte ModulationShaping
		{
			get
			{
				if (rBtnModulationShapingOff.Checked)
					return (byte)0;
				if (rBtnModulationShaping01.Checked)
					return (byte)1;
				return rBtnModulationShaping10.Checked ? (byte)2 : (byte)3;
			}
			set
			{
				rBtnModulationShapingOff.CheckedChanged -= new EventHandler(rBtnModulationShaping_CheckedChanged);
				rBtnModulationShaping01.CheckedChanged -= new EventHandler(rBtnModulationShaping_CheckedChanged);
				rBtnModulationShaping10.CheckedChanged -= new EventHandler(rBtnModulationShaping_CheckedChanged);
				rBtnModulationShaping11.CheckedChanged -= new EventHandler(rBtnModulationShaping_CheckedChanged);
				switch (value)
				{
					case (byte)0:
						rBtnModulationShapingOff.Checked = true;
						rBtnModulationShaping01.Checked = false;
						rBtnModulationShaping10.Checked = false;
						rBtnModulationShaping11.Checked = false;
						break;
					case (byte)1:
						rBtnModulationShapingOff.Checked = false;
						rBtnModulationShaping01.Checked = true;
						rBtnModulationShaping10.Checked = false;
						rBtnModulationShaping11.Checked = false;
						break;
					case (byte)2:
						rBtnModulationShapingOff.Checked = false;
						rBtnModulationShaping01.Checked = false;
						rBtnModulationShaping10.Checked = true;
						rBtnModulationShaping11.Checked = false;
						break;
					case (byte)3:
						rBtnModulationShapingOff.Checked = false;
						rBtnModulationShaping01.Checked = false;
						rBtnModulationShaping10.Checked = false;
						rBtnModulationShaping11.Checked = true;
						break;
				}
				rBtnModulationShapingOff.CheckedChanged += new EventHandler(rBtnModulationShaping_CheckedChanged);
				rBtnModulationShaping01.CheckedChanged += new EventHandler(rBtnModulationShaping_CheckedChanged);
				rBtnModulationShaping10.CheckedChanged += new EventHandler(rBtnModulationShaping_CheckedChanged);
				rBtnModulationShaping11.CheckedChanged += new EventHandler(rBtnModulationShaping_CheckedChanged);
			}
		}

		public Decimal Bitrate
		{
			get
			{
				return bitRate;
			}
			set
			{
				try
				{
					nudBitrate.ValueChanged -= new EventHandler(nudBitrate_ValueChanged);
					bitRate = Math.Round(FrequencyXo / ((Decimal)(ushort)Math.Round(FrequencyXo / value - BitrateFrac / new Decimal(16), MidpointRounding.AwayFromZero) + BitrateFrac / new Decimal(16)), MidpointRounding.AwayFromZero);
					nudBitrate.Value = bitRate;
				}
				catch (Exception)
				{
					nudBitrate.BackColor = ControlPaint.LightLight(Color.Red);
				}
				finally
				{
					nudBitrate.ValueChanged += new EventHandler(nudBitrate_ValueChanged);
				}
			}
		}

		public Decimal BitrateFrac
		{
			get
			{
				return nudBitrateFrac.Value;
			}
			set
			{
				try
				{
					nudBitrateFrac.ValueChanged -= new EventHandler(nudBitrateFrac_ValueChanged);
					nudBitrateFrac.Value = value;
				}
				catch (Exception)
				{
					nudBitrateFrac.BackColor = ControlPaint.LightLight(Color.Red);
				}
				finally
				{
					nudBitrateFrac.ValueChanged += new EventHandler(nudBitrateFrac_ValueChanged);
				}
			}
		}

		public Decimal Fdev
		{
			get
			{
				return nudFdev.Value;
			}
			set
			{
				try
				{
					nudFdev.ValueChanged -= new EventHandler(nudFdev_ValueChanged);
					nudFdev.Value = (Decimal)(ushort)Math.Round(value / FrequencyStep, MidpointRounding.AwayFromZero) * FrequencyStep;
				}
				catch (Exception)
				{
					nudFdev.BackColor = ControlPaint.LightLight(Color.Red);
				}
				finally
				{
					nudFdev.ValueChanged += new EventHandler(nudFdev_ValueChanged);
				}
			}
		}

		public BandEnum Band
		{
			get
			{
				return (BandEnum)cBoxBand.SelectedIndex;
			}
			set
			{
				cBoxBand.SelectedIndexChanged -= new EventHandler(cBoxBand_SelectedIndexChanged);
				cBoxBand.SelectedIndex = (int)value;
				cBoxBand.SelectedIndexChanged += new EventHandler(cBoxBand_SelectedIndexChanged);
			}
		}

		public bool ForceTxBandLowFrequencyOn
		{
			get
			{
				return rBtnForceTxBandLowFrequencyOn.Checked;
			}
			set
			{
				rBtnForceTxBandLowFrequencyOn.CheckedChanged -= new EventHandler(rBtnForceTxBandLowFrequency_CheckedChanged);
				rBtnForceTxBandLowFrequencyOff.CheckedChanged -= new EventHandler(rBtnForceTxBandLowFrequency_CheckedChanged);
				if (value)
				{
					rBtnForceTxBandLowFrequencyOn.Checked = true;
					rBtnForceTxBandLowFrequencyOff.Checked = false;
				}
				else
				{
					rBtnForceTxBandLowFrequencyOn.Checked = false;
					rBtnForceTxBandLowFrequencyOff.Checked = true;
				}
				rBtnForceTxBandLowFrequencyOn.CheckedChanged += new EventHandler(rBtnForceTxBandLowFrequency_CheckedChanged);
				rBtnForceTxBandLowFrequencyOff.CheckedChanged += new EventHandler(rBtnForceTxBandLowFrequency_CheckedChanged);
			}
		}

		public bool ForceRxBandLowFrequencyOn
		{
			get
			{
				return rBtnForceRxBandLowFrequencyOn.Checked;
			}
			set
			{
				rBtnForceRxBandLowFrequencyOn.CheckedChanged -= new EventHandler(rBtnForceRxBandLowFrequency_CheckedChanged);
				rBtnForceRxBandLowFrequencyOff.CheckedChanged -= new EventHandler(rBtnForceRxBandLowFrequency_CheckedChanged);
				if (value)
				{
					rBtnForceRxBandLowFrequencyOn.Checked = true;
					rBtnForceRxBandLowFrequencyOff.Checked = false;
				}
				else
				{
					rBtnForceRxBandLowFrequencyOn.Checked = false;
					rBtnForceRxBandLowFrequencyOff.Checked = true;
				}
				rBtnForceRxBandLowFrequencyOn.CheckedChanged += new EventHandler(rBtnForceRxBandLowFrequency_CheckedChanged);
				rBtnForceRxBandLowFrequencyOff.CheckedChanged += new EventHandler(rBtnForceRxBandLowFrequency_CheckedChanged);
			}
		}

		public bool LowFrequencyModeOn
		{
			get
			{
				return rBtnLowFrequencyModeOn.Checked;
			}
			set
			{
				rBtnLowFrequencyModeOn.CheckedChanged -= new EventHandler(rBtnLowFrequencyMode_CheckedChanged);
				rBtnLowFrequencyModeOff.CheckedChanged -= new EventHandler(rBtnLowFrequencyMode_CheckedChanged);
				if (value)
				{
					rBtnLowFrequencyModeOn.Checked = true;
					rBtnLowFrequencyModeOff.Checked = false;
				}
				else
				{
					rBtnLowFrequencyModeOn.Checked = false;
					rBtnLowFrequencyModeOff.Checked = true;
				}
				rBtnLowFrequencyModeOn.CheckedChanged += new EventHandler(rBtnLowFrequencyMode_CheckedChanged);
				rBtnLowFrequencyModeOff.CheckedChanged += new EventHandler(rBtnLowFrequencyMode_CheckedChanged);
			}
		}

		public Decimal FrequencyRf
		{
			get
			{
				return nudFrequencyRf.Value;
			}
			set
			{
				try
				{
					nudFrequencyRf.ValueChanged -= new EventHandler(nudFrequencyRf_ValueChanged);
					nudFrequencyRf.Value = (Decimal)(uint)Math.Round(value / FrequencyStep, MidpointRounding.AwayFromZero) * FrequencyStep;
				}
				catch (Exception)
				{
					nudFrequencyRf.BackColor = ControlPaint.LightLight(Color.Red);
				}
				finally
				{
					nudFrequencyRf.ValueChanged += new EventHandler(nudFrequencyRf_ValueChanged);
				}
			}
		}

		public bool FastHopOn
		{
			get
			{
				return rBtnFastHopOn.Checked;
			}
			set
			{
				rBtnFastHopOn.CheckedChanged -= new EventHandler(rBtnFastHop_CheckedChanged);
				rBtnFastHopOff.CheckedChanged -= new EventHandler(rBtnFastHop_CheckedChanged);
				if (value)
				{
					rBtnFastHopOn.Checked = true;
					rBtnFastHopOff.Checked = false;
				}
				else
				{
					rBtnFastHopOn.Checked = false;
					rBtnFastHopOff.Checked = true;
				}
				rBtnFastHopOn.CheckedChanged += new EventHandler(rBtnFastHop_CheckedChanged);
				rBtnFastHopOff.CheckedChanged += new EventHandler(rBtnFastHop_CheckedChanged);
			}
		}

		public bool TcxoInputOn
		{
			get
			{
				return rBtnTcxoInputOn.Checked;
			}
			set
			{
				rBtnTcxoInputOn.CheckedChanged -= new EventHandler(rBtnTcxoInput_CheckedChanged);
				rBtnTcxoInputOff.CheckedChanged -= new EventHandler(rBtnTcxoInput_CheckedChanged);
				rBtnTcxoInputOn.Checked = value;
				rBtnTcxoInputOff.Checked = !value;
				rBtnTcxoInputOn.CheckedChanged += new EventHandler(rBtnTcxoInput_CheckedChanged);
				rBtnTcxoInputOff.CheckedChanged += new EventHandler(rBtnTcxoInput_CheckedChanged);
			}
		}

		public bool LowBatOn
		{
			get
			{
				return rBtnLowBatOn.Checked;
			}
			set
			{
				rBtnLowBatOn.CheckedChanged -= new EventHandler(rBtnLowBatOn_CheckedChanged);
				rBtnLowBatOff.CheckedChanged -= new EventHandler(rBtnLowBatOn_CheckedChanged);
				if (value)
				{
					rBtnLowBatOn.Checked = true;
					rBtnLowBatOff.Checked = false;
				}
				else
				{
					rBtnLowBatOn.Checked = false;
					rBtnLowBatOff.Checked = true;
				}
				rBtnLowBatOn.CheckedChanged += new EventHandler(rBtnLowBatOn_CheckedChanged);
				rBtnLowBatOff.CheckedChanged += new EventHandler(rBtnLowBatOn_CheckedChanged);
			}
		}

		public LowBatTrimEnum LowBatTrim
		{
			get
			{
				return (LowBatTrimEnum)cBoxLowBatTrim.SelectedIndex;
			}
			set
			{
				cBoxLowBatTrim.SelectedIndexChanged -= new EventHandler(cBoxLowBatTrim_SelectedIndexChanged);
				cBoxLowBatTrim.SelectedIndex = (int)value;
				cBoxLowBatTrim.SelectedIndexChanged += new EventHandler(cBoxLowBatTrim_SelectedIndexChanged);
			}
		}

		public event DecimalEventHandler FrequencyXoChanged;

		public event ModulationTypeEventHandler ModulationTypeChanged;

		public event ByteEventHandler ModulationShapingChanged;

		public event DecimalEventHandler BitrateChanged;

		public event DecimalEventHandler BitrateFracChanged;

		public event DecimalEventHandler FdevChanged;

		public event BandEventHandler BandChanged;

		public event BooleanEventHandler ForceTxBandLowFrequencyOnChanged;

		public event BooleanEventHandler ForceRxBandLowFrequencyOnChanged;

		public event BooleanEventHandler LowFrequencyModeOnChanged;

		public event DecimalEventHandler FrequencyRfChanged;

		public event BooleanEventHandler FastHopOnChanged;

		public event BooleanEventHandler TcxoInputChanged;

		public event EventHandler RcCalibrationChanged;

		public event BooleanEventHandler LowBatOnChanged;

		public event LowBatTrimEventHandler LowBatTrimChanged;

		public event DocumentationChangedEventHandler DocumentationChanged;

		public CommonViewControl()
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
			this.label20 = new Label();
			this.panel6 = new Panel();
			this.label21 = new Label();
			this.label22 = new Label();
			this.panel7 = new Panel();
			this.label23 = new Label();
			this.label24 = new Label();
			this.label25 = new Label();
			this.label26 = new Label();
			this.label27 = new Label();
			this.label28 = new Label();
			this.errorProvider = new ErrorProvider(this.components);
			this.nudBitrate = new NumericUpDownEx();
			this.nudFdev = new NumericUpDownEx();
			this.nudFrequencyRf = new NumericUpDownEx();
			this.lblListenResolRx = new Label();
			this.label30 = new Label();
			this.gBoxBatteryManagement = new GroupBoxEx();
			this.panel4 = new Panel();
			this.rBtnLowBatOff = new RadioButton();
			this.rBtnLowBatOn = new RadioButton();
			this.label17 = new Label();
			this.label15 = new Label();
			this.label16 = new Label();
			this.cBoxLowBatTrim = new ComboBox();
			this.gBoxOscillators = new GroupBoxEx();
			this.panel1 = new Panel();
			this.rBtnTcxoInputOff = new RadioButton();
			this.rBtnTcxoInputOn = new RadioButton();
			this.nudFrequencyXo = new NumericUpDownEx();
			this.label9 = new Label();
			this.btnRcCalibration = new Button();
			this.label1 = new Label();
			this.lblRcOscillatorCalStat = new Label();
			this.lblRcOscillatorCal = new Label();
			this.gBoxModulation = new GroupBoxEx();
			this.panel2 = new Panel();
			this.rBtnModulationTypeOok = new RadioButton();
			this.rBtnModulationTypeFsk = new RadioButton();
			this.label6 = new Label();
			this.label5 = new Label();
			this.panel3 = new Panel();
			this.rBtnModulationShaping11 = new RadioButton();
			this.rBtnModulationShaping10 = new RadioButton();
			this.rBtnModulationShaping01 = new RadioButton();
			this.rBtnModulationShapingOff = new RadioButton();
			this.gBoxGeneral = new GroupBoxEx();
			this.label3 = new Label();
			this.panel5 = new Panel();
			this.rBtnFastHopOff = new RadioButton();
			this.rBtnFastHopOn = new RadioButton();
			this.nudBitrateFrac = new NumericUpDownEx();
			this.label12 = new Label();
			this.label8 = new Label();
			this.label11 = new Label();
			this.label13 = new Label();
			this.label14 = new Label();
			this.label2 = new Label();
			this.label10 = new Label();
			this.label7 = new Label();
			this.panel8 = new Panel();
			this.rBtnLowFrequencyModeOff = new RadioButton();
			this.rBtnLowFrequencyModeOn = new RadioButton();
			this.cBoxBand = new ComboBox();
			this.label19 = new Label();
			this.label18 = new Label();
			this.label4 = new Label();
			this.groupBoxEx1 = new GroupBoxEx();
			this.panel10 = new Panel();
			this.rBtnForceRxBandLowFrequencyOff = new RadioButton();
			this.rBtnForceRxBandLowFrequencyOn = new RadioButton();
			this.label31 = new Label();
			this.panel9 = new Panel();
			this.rBtnForceTxBandLowFrequencyOff = new RadioButton();
			this.rBtnForceTxBandLowFrequencyOn = new RadioButton();
			this.label29 = new Label();
			this.groupBoxEx2 = new GroupBoxEx();
			((ISupportInitialize)this.errorProvider).BeginInit();
			this.nudBitrate.BeginInit();
			this.nudFdev.BeginInit();
			this.nudFrequencyRf.BeginInit();
			this.gBoxBatteryManagement.SuspendLayout();
			this.panel4.SuspendLayout();
			this.gBoxOscillators.SuspendLayout();
			this.panel1.SuspendLayout();
			this.nudFrequencyXo.BeginInit();
			this.gBoxModulation.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel3.SuspendLayout();
			this.gBoxGeneral.SuspendLayout();
			this.panel5.SuspendLayout();
			this.nudBitrateFrac.BeginInit();
			this.panel8.SuspendLayout();
			this.groupBoxEx1.SuspendLayout();
			this.panel10.SuspendLayout();
			this.panel9.SuspendLayout();
			this.groupBoxEx2.SuspendLayout();
			this.SuspendLayout();
			this.label20.Location = new Point(0, 0);
			this.label20.Name = "label20";
			this.label20.Size = new Size(100, 23);
			this.label20.TabIndex = 0;
			this.panel6.AutoSize = true;
			this.panel6.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel6.Location = new Point(165, 85);
			this.panel6.Name = "panel6";
			this.panel6.Size = new Size(98, 17);
			this.panel6.TabIndex = 1;
			this.label21.AutoSize = true;
			this.label21.Location = new Point(8, 114);
			this.label21.Name = "label21";
			this.label21.Size = new Size(105, 13);
			this.label21.TabIndex = 3;
			this.label21.Text = "Listen resolution idle:";
			this.label22.AutoSize = true;
			this.label22.Location = new Point(295, 112);
			this.label22.Name = "label22";
			this.label22.Size = new Size(18, 13);
			this.label22.TabIndex = 5;
			this.label22.Text = "µs";
			this.panel7.Location = new Point(0, 0);
			this.panel7.Name = "panel7";
			this.panel7.Size = new Size(200, 100);
			this.panel7.TabIndex = 0;
			this.label23.AutoSize = true;
			this.label23.Location = new Point(8, 167);
			this.label23.Name = "label23";
			this.label23.Size = new Size(72, 13);
			this.label23.TabIndex = 9;
			this.label23.Text = "Listen criteria:";
			this.label24.AutoSize = true;
			this.label24.Location = new Point(8, 217);
			this.label24.Name = "label24";
			this.label24.Size = new Size(59, 13);
			this.label24.TabIndex = 11;
			this.label24.Text = "Listen end:";
			this.label25.AutoSize = true;
			this.label25.Location = new Point(295, 245);
			this.label25.Name = "label25";
			this.label25.Size = new Size(20, 13);
			this.label25.TabIndex = 15;
			this.label25.Text = "ms";
			this.label26.AutoSize = true;
			this.label26.Location = new Point(295, 271);
			this.label26.Name = "label26";
			this.label26.Size = new Size(20, 13);
			this.label26.TabIndex = 18;
			this.label26.Text = "ms";
			this.label27.AutoSize = true;
			this.label27.Location = new Point(8, 245);
			this.label27.Name = "label27";
			this.label27.Size = new Size(79, 13);
			this.label27.TabIndex = 13;
			this.label27.Text = "Listen idle time:";
			this.label28.AutoSize = true;
			this.label28.Location = new Point(8, 270);
			this.label28.Name = "label28";
			this.label28.Size = new Size(76, 13);
			this.label28.TabIndex = 16;
			this.label28.Text = "Listen Rx time:";
			this.errorProvider.ContainerControl = (ContainerControl)this;
			this.errorProvider.SetIconPadding((Control)this.nudBitrate, 30);
			this.nudBitrate.Location = new Point(164, 68);
			NumericUpDownEx numericUpDownEx1 = this.nudBitrate;
			int[] bits1 = new int[4];
			bits1[0] = 603774;
			Decimal num1 = new Decimal(bits1);
			numericUpDownEx1.Maximum = num1;
			NumericUpDownEx numericUpDownEx2 = this.nudBitrate;
			int[] bits2 = new int[4];
			bits2[0] = 600;
			Decimal num2 = new Decimal(bits2);
			numericUpDownEx2.Minimum = num2;
			this.nudBitrate.Name = "nudBitrate";
			this.nudBitrate.Size = new Size(124, 20);
			this.nudBitrate.TabIndex = 4;
			this.nudBitrate.ThousandsSeparator = true;
			NumericUpDownEx numericUpDownEx3 = this.nudBitrate;
			int[] bits3 = new int[4];
			bits3[0] = 4800;
			Decimal num3 = new Decimal(bits3);
			numericUpDownEx3.Value = num3;
			this.nudBitrate.ValueChanged += new EventHandler(this.nudBitrate_ValueChanged);
			this.errorProvider.SetIconPadding((Control)this.nudFdev, 30);
			NumericUpDownEx numericUpDownEx4 = this.nudFdev;
			int[] bits4 = new int[4];
			bits4[0] = 61;
			Decimal num4 = new Decimal(bits4);
			numericUpDownEx4.Increment = num4;
			this.nudFdev.Location = new Point(164, 119);
			NumericUpDownEx numericUpDownEx5 = this.nudFdev;
			int[] bits5 = new int[4];
			bits5[0] = 300000;
			Decimal num5 = new Decimal(bits5);
			numericUpDownEx5.Maximum = num5;
			this.nudFdev.Name = "nudFdev";
			this.nudFdev.Size = new Size(124, 20);
			this.nudFdev.TabIndex = 8;
			this.nudFdev.ThousandsSeparator = true;
			NumericUpDownEx numericUpDownEx6 = this.nudFdev;
			int[] bits6 = new int[4];
			bits6[0] = 5005;
			Decimal num6 = new Decimal(bits6);
			numericUpDownEx6.Value = num6;
			this.nudFdev.ValueChanged += new EventHandler(this.nudFdev_ValueChanged);
			this.errorProvider.SetIconPadding((Control)this.nudFrequencyRf, 30);
			NumericUpDownEx numericUpDownEx7 = this.nudFrequencyRf;
			int[] bits7 = new int[4];
			bits7[0] = 61;
			Decimal num7 = new Decimal(bits7);
			numericUpDownEx7.Increment = num7;
			this.nudFrequencyRf.Location = new Point(164, 19);
			NumericUpDownEx numericUpDownEx8 = this.nudFrequencyRf;
			int[] bits8 = new int[4];
			bits8[0] = 2040000000;
			Decimal num8 = new Decimal(bits8);
			numericUpDownEx8.Maximum = num8;
			NumericUpDownEx numericUpDownEx9 = this.nudFrequencyRf;
			int[] bits9 = new int[4];
			bits9[0] = 100000000;
			Decimal num9 = new Decimal(bits9);
			numericUpDownEx9.Minimum = num9;
			this.nudFrequencyRf.Name = "nudFrequencyRf";
			this.nudFrequencyRf.Size = new Size(124, 20);
			this.nudFrequencyRf.TabIndex = 1;
			this.nudFrequencyRf.ThousandsSeparator = true;
			NumericUpDownEx numericUpDownEx10 = this.nudFrequencyRf;
			int[] bits10 = new int[4];
			bits10[0] = 915000000;
			Decimal num10 = new Decimal(bits10);
			numericUpDownEx10.Value = num10;
			this.nudFrequencyRf.ValueChanged += new EventHandler(this.nudFrequencyRf_ValueChanged);
			this.lblListenResolRx.AutoSize = true;
			this.lblListenResolRx.Location = new Point(8, 141);
			this.lblListenResolRx.Name = "lblListenResolRx";
			this.lblListenResolRx.Size = new Size(102, 13);
			this.lblListenResolRx.TabIndex = 6;
			this.lblListenResolRx.Text = "Listen resolution Rx:";
			this.label30.AutoSize = true;
			this.label30.Location = new Point(295, 139);
			this.label30.Name = "label30";
			this.label30.Size = new Size(18, 13);
			this.label30.TabIndex = 8;
			this.label30.Text = "µs";
			this.gBoxBatteryManagement.Controls.Add((Control)this.panel4);
			this.gBoxBatteryManagement.Controls.Add((Control)this.label17);
			this.gBoxBatteryManagement.Controls.Add((Control)this.label15);
			this.gBoxBatteryManagement.Controls.Add((Control)this.label16);
			this.gBoxBatteryManagement.Controls.Add((Control)this.cBoxLowBatTrim);
			this.gBoxBatteryManagement.Location = new Point(222, 410);
			this.gBoxBatteryManagement.Name = "gBoxBatteryManagement";
			this.gBoxBatteryManagement.Size = new Size(355, 80);
			this.gBoxBatteryManagement.TabIndex = 5;
			this.gBoxBatteryManagement.TabStop = false;
			this.gBoxBatteryManagement.Text = "Battery management";
			this.gBoxBatteryManagement.MouseEnter += new EventHandler(this.control_MouseEnter);
			this.gBoxBatteryManagement.MouseLeave += new EventHandler(this.control_MouseLeave);
			this.panel4.AutoSize = true;
			this.panel4.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel4.Controls.Add((Control)this.rBtnLowBatOff);
			this.panel4.Controls.Add((Control)this.rBtnLowBatOn);
			this.panel4.Location = new Point(164, 19);
			this.panel4.Name = "panel4";
			this.panel4.Size = new Size(102, 20);
			this.panel4.TabIndex = 1;
			this.rBtnLowBatOff.AutoSize = true;
			this.rBtnLowBatOff.Location = new Point(54, 3);
			this.rBtnLowBatOff.Margin = new Padding(3, 0, 3, 0);
			this.rBtnLowBatOff.Name = "rBtnLowBatOff";
			this.rBtnLowBatOff.Size = new Size(45, 17);
			this.rBtnLowBatOff.TabIndex = 1;
			this.rBtnLowBatOff.Text = "OFF";
			this.rBtnLowBatOff.UseVisualStyleBackColor = true;
			this.rBtnLowBatOn.AutoSize = true;
			this.rBtnLowBatOn.Checked = true;
			this.rBtnLowBatOn.Location = new Point(3, 3);
			this.rBtnLowBatOn.Margin = new Padding(3, 0, 3, 0);
			this.rBtnLowBatOn.Name = "rBtnLowBatOn";
			this.rBtnLowBatOn.Size = new Size(41, 17);
			this.rBtnLowBatOn.TabIndex = 0;
			this.rBtnLowBatOn.TabStop = true;
			this.rBtnLowBatOn.Text = "ON";
			this.rBtnLowBatOn.UseVisualStyleBackColor = true;
			this.label17.AutoSize = true;
			this.label17.Location = new Point(6, 48);
			this.label17.Name = "label17";
			this.label17.Size = new Size(130, 13);
			this.label17.TabIndex = 2;
			this.label17.Text = "Low battery threshold trim:";
			this.label15.AutoSize = true;
			this.label15.Location = new Point(6, 24);
			this.label15.Name = "label15";
			this.label15.Size = new Size(107, 13);
			this.label15.TabIndex = 0;
			this.label15.Text = "Low battery detector:";
			this.label16.AutoSize = true;
			this.label16.Location = new Point(294, 49);
			this.label16.Name = "label16";
			this.label16.Size = new Size(14, 13);
			this.label16.TabIndex = 4;
			this.label16.Text = "V";
			this.cBoxLowBatTrim.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxLowBatTrim.FormattingEnabled = true;
			this.cBoxLowBatTrim.Items.AddRange(new object[8]
      {
        (object) "1.695",
        (object) "1.764",
        (object) "1.835",
        (object) "1.905",
        (object) "1.976",
        (object) "2.045",
        (object) "2.116",
        (object) "2.185"
      });
			this.cBoxLowBatTrim.Location = new Point(164, 45);
			this.cBoxLowBatTrim.Name = "cBoxLowBatTrim";
			this.cBoxLowBatTrim.Size = new Size(124, 21);
			this.cBoxLowBatTrim.TabIndex = 3;
			this.cBoxLowBatTrim.SelectedIndexChanged += new EventHandler(this.cBoxLowBatTrim_SelectedIndexChanged);
			this.gBoxOscillators.Controls.Add((Control)this.panel1);
			this.gBoxOscillators.Controls.Add((Control)this.nudFrequencyXo);
			this.gBoxOscillators.Controls.Add((Control)this.label9);
			this.gBoxOscillators.Controls.Add((Control)this.btnRcCalibration);
			this.gBoxOscillators.Controls.Add((Control)this.label1);
			this.gBoxOscillators.Controls.Add((Control)this.lblRcOscillatorCalStat);
			this.gBoxOscillators.Controls.Add((Control)this.lblRcOscillatorCal);
			this.gBoxOscillators.Location = new Point(222, 304);
			this.gBoxOscillators.Name = "gBoxOscillators";
			this.gBoxOscillators.Size = new Size(355, 100);
			this.gBoxOscillators.TabIndex = 3;
			this.gBoxOscillators.TabStop = false;
			this.gBoxOscillators.Text = "Oscillators";
			this.gBoxOscillators.MouseEnter += new EventHandler(this.control_MouseEnter);
			this.gBoxOscillators.MouseLeave += new EventHandler(this.control_MouseLeave);
			this.panel1.AutoSize = true;
			this.panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel1.Controls.Add((Control)this.rBtnTcxoInputOff);
			this.panel1.Controls.Add((Control)this.rBtnTcxoInputOn);
			this.panel1.Location = new Point(164, 45);
			this.panel1.Name = "panel1";
			this.panel1.Size = new Size(122, 23);
			this.panel1.TabIndex = 1;
			this.rBtnTcxoInputOff.AutoSize = true;
			this.rBtnTcxoInputOff.Location = new Point(63, 3);
			this.rBtnTcxoInputOff.Name = "rBtnTcxoInputOff";
			this.rBtnTcxoInputOff.Size = new Size(56, 17);
			this.rBtnTcxoInputOff.TabIndex = 1;
			this.rBtnTcxoInputOff.Text = "Crystal";
			this.rBtnTcxoInputOff.UseVisualStyleBackColor = true;
			this.rBtnTcxoInputOff.CheckedChanged += new EventHandler(this.rBtnTcxoInput_CheckedChanged);
			this.rBtnTcxoInputOn.AutoSize = true;
			this.rBtnTcxoInputOn.Checked = true;
			this.rBtnTcxoInputOn.Location = new Point(3, 3);
			this.rBtnTcxoInputOn.Name = "rBtnTcxoInputOn";
			this.rBtnTcxoInputOn.Size = new Size(54, 17);
			this.rBtnTcxoInputOn.TabIndex = 0;
			this.rBtnTcxoInputOn.TabStop = true;
			this.rBtnTcxoInputOn.Text = "TCXO";
			this.rBtnTcxoInputOn.UseVisualStyleBackColor = true;
			this.rBtnTcxoInputOn.CheckedChanged += new EventHandler(this.rBtnTcxoInput_CheckedChanged);
			this.nudFrequencyXo.Location = new Point(164, 19);
			NumericUpDownEx numericUpDownEx11 = this.nudFrequencyXo;
			int[] bits11 = new int[4];
			bits11[0] = 32000000;
			Decimal num11 = new Decimal(bits11);
			numericUpDownEx11.Maximum = num11;
			NumericUpDownEx numericUpDownEx12 = this.nudFrequencyXo;
			int[] bits12 = new int[4];
			bits12[0] = 26000000;
			Decimal num12 = new Decimal(bits12);
			numericUpDownEx12.Minimum = num12;
			this.nudFrequencyXo.Name = "nudFrequencyXo";
			this.nudFrequencyXo.Size = new Size(124, 20);
			this.nudFrequencyXo.TabIndex = 1;
			this.nudFrequencyXo.ThousandsSeparator = true;
			NumericUpDownEx numericUpDownEx13 = this.nudFrequencyXo;
			int[] bits13 = new int[4];
			bits13[0] = 32000000;
			Decimal num13 = new Decimal(bits13);
			numericUpDownEx13.Value = num13;
			this.nudFrequencyXo.ValueChanged += new EventHandler(this.nudFrequencyXo_ValueChanged);
			this.label9.AutoSize = true;
			this.label9.Location = new Point(294, 23);
			this.label9.Name = "label9";
			this.label9.Size = new Size(20, 13);
			this.label9.TabIndex = 2;
			this.label9.Text = "Hz";
			this.btnRcCalibration.Location = new Point(164, 71);
			this.btnRcCalibration.Name = "btnRcCalibration";
			this.btnRcCalibration.Size = new Size(75, 23);
			this.btnRcCalibration.TabIndex = 4;
			this.btnRcCalibration.Text = "Calibrate";
			this.btnRcCalibration.UseVisualStyleBackColor = true;
			this.btnRcCalibration.Click += new EventHandler(this.btnRcCalibration_Click);
			this.label1.AutoSize = true;
			this.label1.Location = new Point(6, 23);
			this.label1.Name = "label1";
			this.label1.Size = new Size(78, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "XO Frequency:";
			this.lblRcOscillatorCalStat.AutoSize = true;
			this.lblRcOscillatorCalStat.Location = new Point(6, 47);
			this.lblRcOscillatorCalStat.Name = "lblRcOscillatorCalStat";
			this.lblRcOscillatorCalStat.Size = new Size(96, 13);
			this.lblRcOscillatorCalStat.TabIndex = 5;
			this.lblRcOscillatorCalStat.Text = "XO input selection:";
			this.lblRcOscillatorCal.AutoSize = true;
			this.lblRcOscillatorCal.Location = new Point(6, 76);
			this.lblRcOscillatorCal.Name = "lblRcOscillatorCal";
			this.lblRcOscillatorCal.Size = new Size(120, 13);
			this.lblRcOscillatorCal.TabIndex = 3;
			this.lblRcOscillatorCal.Text = "RC oscillator calibration:";
			this.gBoxModulation.Controls.Add((Control)this.panel2);
			this.gBoxModulation.Controls.Add((Control)this.label6);
			this.gBoxModulation.Controls.Add((Control)this.label5);
			this.gBoxModulation.Controls.Add((Control)this.panel3);
			this.gBoxModulation.Location = new Point(222, 154);
			this.gBoxModulation.Name = "gBoxModulation";
			this.gBoxModulation.Size = new Size(355, 144);
			this.gBoxModulation.TabIndex = 2;
			this.gBoxModulation.TabStop = false;
			this.gBoxModulation.Text = "Modulation";
			this.gBoxModulation.MouseEnter += new EventHandler(this.control_MouseEnter);
			this.gBoxModulation.MouseLeave += new EventHandler(this.control_MouseLeave);
			this.panel2.AutoSize = true;
			this.panel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel2.Controls.Add((Control)this.rBtnModulationTypeOok);
			this.panel2.Controls.Add((Control)this.rBtnModulationTypeFsk);
			this.panel2.Location = new Point(164, 19);
			this.panel2.Name = "panel2";
			this.panel2.Size = new Size(105, 23);
			this.panel2.TabIndex = 1;
			this.rBtnModulationTypeOok.AutoSize = true;
			this.rBtnModulationTypeOok.Location = new Point(54, 3);
			this.rBtnModulationTypeOok.Name = "rBtnModulationTypeOok";
			this.rBtnModulationTypeOok.Size = new Size(48, 17);
			this.rBtnModulationTypeOok.TabIndex = 1;
			this.rBtnModulationTypeOok.Text = "OOK";
			this.rBtnModulationTypeOok.UseVisualStyleBackColor = true;
			this.rBtnModulationTypeOok.CheckedChanged += new EventHandler(this.rBtnModulationType_CheckedChanged);
			this.rBtnModulationTypeFsk.AutoSize = true;
			this.rBtnModulationTypeFsk.Checked = true;
			this.rBtnModulationTypeFsk.Location = new Point(3, 3);
			this.rBtnModulationTypeFsk.Name = "rBtnModulationTypeFsk";
			this.rBtnModulationTypeFsk.Size = new Size(45, 17);
			this.rBtnModulationTypeFsk.TabIndex = 0;
			this.rBtnModulationTypeFsk.TabStop = true;
			this.rBtnModulationTypeFsk.Text = "FSK";
			this.rBtnModulationTypeFsk.UseVisualStyleBackColor = true;
			this.rBtnModulationTypeFsk.CheckedChanged += new EventHandler(this.rBtnModulationType_CheckedChanged);
			this.label6.AutoSize = true;
			this.label6.Location = new Point(6, 53);
			this.label6.Name = "label6";
			this.label6.Size = new Size(102, 13);
			this.label6.TabIndex = 2;
			this.label6.Text = "Modulation shaping:";
			this.label5.AutoSize = true;
			this.label5.Location = new Point(6, 24);
			this.label5.Name = "label5";
			this.label5.Size = new Size(62, 13);
			this.label5.TabIndex = 0;
			this.label5.Text = "Modulation:";
			this.panel3.AutoSize = true;
			this.panel3.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel3.Controls.Add((Control)this.rBtnModulationShaping11);
			this.panel3.Controls.Add((Control)this.rBtnModulationShaping10);
			this.panel3.Controls.Add((Control)this.rBtnModulationShaping01);
			this.panel3.Controls.Add((Control)this.rBtnModulationShapingOff);
			this.panel3.Location = new Point(164, 48);
			this.panel3.Name = "panel3";
			this.panel3.Size = new Size(144, 92);
			this.panel3.TabIndex = 3;
			this.rBtnModulationShaping11.AutoSize = true;
			this.rBtnModulationShaping11.Location = new Point(3, 72);
			this.rBtnModulationShaping11.Name = "rBtnModulationShaping11";
			this.rBtnModulationShaping11.Size = new Size(138, 17);
			this.rBtnModulationShaping11.TabIndex = 3;
			this.rBtnModulationShaping11.Text = "Gaussian filter, BT = 0.3";
			this.rBtnModulationShaping11.UseVisualStyleBackColor = true;
			this.rBtnModulationShaping11.CheckedChanged += new EventHandler(this.rBtnModulationShaping_CheckedChanged);
			this.rBtnModulationShaping10.AutoSize = true;
			this.rBtnModulationShaping10.Location = new Point(3, 49);
			this.rBtnModulationShaping10.Name = "rBtnModulationShaping10";
			this.rBtnModulationShaping10.Size = new Size(138, 17);
			this.rBtnModulationShaping10.TabIndex = 2;
			this.rBtnModulationShaping10.Text = "Gaussian filter, BT = 0.5";
			this.rBtnModulationShaping10.UseVisualStyleBackColor = true;
			this.rBtnModulationShaping10.CheckedChanged += new EventHandler(this.rBtnModulationShaping_CheckedChanged);
			this.rBtnModulationShaping01.AutoSize = true;
			this.rBtnModulationShaping01.Location = new Point(3, 26);
			this.rBtnModulationShaping01.Name = "rBtnModulationShaping01";
			this.rBtnModulationShaping01.Size = new Size(138, 17);
			this.rBtnModulationShaping01.TabIndex = 1;
			this.rBtnModulationShaping01.Text = "Gaussian filter, BT = 1.0";
			this.rBtnModulationShaping01.UseVisualStyleBackColor = true;
			this.rBtnModulationShaping01.CheckedChanged += new EventHandler(this.rBtnModulationShaping_CheckedChanged);
			this.rBtnModulationShapingOff.AutoSize = true;
			this.rBtnModulationShapingOff.Checked = true;
			this.rBtnModulationShapingOff.Location = new Point(3, 3);
			this.rBtnModulationShapingOff.Name = "rBtnModulationShapingOff";
			this.rBtnModulationShapingOff.Size = new Size(45, 17);
			this.rBtnModulationShapingOff.TabIndex = 0;
			this.rBtnModulationShapingOff.TabStop = true;
			this.rBtnModulationShapingOff.Text = "OFF";
			this.rBtnModulationShapingOff.UseVisualStyleBackColor = true;
			this.rBtnModulationShapingOff.CheckedChanged += new EventHandler(this.rBtnModulationShaping_CheckedChanged);
			this.gBoxGeneral.Controls.Add((Control)this.label3);
			this.gBoxGeneral.Controls.Add((Control)this.panel5);
			this.gBoxGeneral.Controls.Add((Control)this.nudBitrateFrac);
			this.gBoxGeneral.Controls.Add((Control)this.nudBitrate);
			this.gBoxGeneral.Controls.Add((Control)this.label12);
			this.gBoxGeneral.Controls.Add((Control)this.label8);
			this.gBoxGeneral.Controls.Add((Control)this.label11);
			this.gBoxGeneral.Controls.Add((Control)this.label13);
			this.gBoxGeneral.Controls.Add((Control)this.label14);
			this.gBoxGeneral.Controls.Add((Control)this.label2);
			this.gBoxGeneral.Controls.Add((Control)this.label10);
			this.gBoxGeneral.Controls.Add((Control)this.label7);
			this.gBoxGeneral.Controls.Add((Control)this.nudFdev);
			this.gBoxGeneral.Controls.Add((Control)this.nudFrequencyRf);
			this.gBoxGeneral.Location = new Point(222, 3);
			this.gBoxGeneral.Name = "gBoxGeneral";
			this.gBoxGeneral.Size = new Size(355, 145);
			this.gBoxGeneral.TabIndex = 0;
			this.gBoxGeneral.TabStop = false;
			this.gBoxGeneral.Text = "General";
			this.gBoxGeneral.MouseEnter += new EventHandler(this.control_MouseEnter);
			this.gBoxGeneral.MouseLeave += new EventHandler(this.control_MouseLeave);
			this.label3.AutoSize = true;
			this.label3.Location = new Point(6, 47);
			this.label3.Name = "label3";
			this.label3.Size = new Size(71, 13);
			this.label3.TabIndex = 22;
			this.label3.Text = "Fast hopping:";
			this.panel5.AutoSize = true;
			this.panel5.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel5.Controls.Add((Control)this.rBtnFastHopOff);
			this.panel5.Controls.Add((Control)this.rBtnFastHopOn);
			this.panel5.Location = new Point(164, 45);
			this.panel5.Name = "panel5";
			this.panel5.Size = new Size(98, 17);
			this.panel5.TabIndex = 23;
			this.rBtnFastHopOff.AutoSize = true;
			this.rBtnFastHopOff.Location = new Point(50, 0);
			this.rBtnFastHopOff.Margin = new Padding(3, 0, 3, 0);
			this.rBtnFastHopOff.Name = "rBtnFastHopOff";
			this.rBtnFastHopOff.Size = new Size(45, 17);
			this.rBtnFastHopOff.TabIndex = 1;
			this.rBtnFastHopOff.Text = "OFF";
			this.rBtnFastHopOff.UseVisualStyleBackColor = true;
			this.rBtnFastHopOff.CheckedChanged += new EventHandler(this.rBtnFastHop_CheckedChanged);
			this.rBtnFastHopOn.AutoSize = true;
			this.rBtnFastHopOn.Checked = true;
			this.rBtnFastHopOn.Location = new Point(3, 0);
			this.rBtnFastHopOn.Margin = new Padding(3, 0, 3, 0);
			this.rBtnFastHopOn.Name = "rBtnFastHopOn";
			this.rBtnFastHopOn.Size = new Size(41, 17);
			this.rBtnFastHopOn.TabIndex = 0;
			this.rBtnFastHopOn.TabStop = true;
			this.rBtnFastHopOn.Text = "ON";
			this.rBtnFastHopOn.UseVisualStyleBackColor = true;
			this.rBtnFastHopOn.CheckedChanged += new EventHandler(this.rBtnFastHop_CheckedChanged);
			this.nudBitrateFrac.Location = new Point(164, 94);
			NumericUpDownEx numericUpDownEx14 = this.nudBitrateFrac;
			int[] bits14 = new int[4];
			bits14[0] = 15;
			Decimal num14 = new Decimal(bits14);
			numericUpDownEx14.Maximum = num14;
			this.nudBitrateFrac.Name = "nudBitrateFrac";
			this.nudBitrateFrac.Size = new Size(124, 20);
			this.nudBitrateFrac.TabIndex = 4;
			this.nudBitrateFrac.ThousandsSeparator = true;
			this.nudBitrateFrac.ValueChanged += new EventHandler(this.nudBitrateFrac_ValueChanged);
			this.label12.AutoSize = true;
			this.label12.Location = new Point(137, 123);
			this.label12.Name = "label12";
			this.label12.Size = new Size(21, 13);
			this.label12.TabIndex = 7;
			this.label12.Text = "+/-";
			this.label8.AutoSize = true;
			this.label8.Location = new Point(294, 72);
			this.label8.Name = "label8";
			this.label8.Size = new Size(24, 13);
			this.label8.TabIndex = 5;
			this.label8.Text = "bps";
			this.label11.AutoSize = true;
			this.label11.Location = new Point(294, 123);
			this.label11.Name = "label11";
			this.label11.Size = new Size(20, 13);
			this.label11.TabIndex = 9;
			this.label11.Text = "Hz";
			this.label13.AutoSize = true;
			this.label13.Location = new Point(294, 23);
			this.label13.Name = "label13";
			this.label13.Size = new Size(20, 13);
			this.label13.TabIndex = 2;
			this.label13.Text = "Hz";
			this.label14.AutoSize = true;
			this.label14.Location = new Point(6, 23);
			this.label14.Name = "label14";
			this.label14.Size = new Size(74, 13);
			this.label14.TabIndex = 0;
			this.label14.Text = "RF frequency:";
			this.label2.AutoSize = true;
			this.label2.Location = new Point(6, 98);
			this.label2.Name = "label2";
			this.label2.Size = new Size(92, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Bitrate fine tuning:";
			this.label10.AutoSize = true;
			this.label10.Location = new Point(6, 123);
			this.label10.Name = "label10";
			this.label10.Size = new Size(34, 13);
			this.label10.TabIndex = 6;
			this.label10.Text = "Fdev:";
			this.label7.AutoSize = true;
			this.label7.Location = new Point(6, 72);
			this.label7.Name = "label7";
			this.label7.Size = new Size(40, 13);
			this.label7.TabIndex = 3;
			this.label7.Text = "Bitrate:";
			this.panel8.AutoSize = true;
			this.panel8.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel8.Controls.Add((Control)this.rBtnLowFrequencyModeOff);
			this.panel8.Controls.Add((Control)this.rBtnLowFrequencyModeOn);
			this.panel8.Location = new Point(164, 46);
			this.panel8.Name = "panel8";
			this.panel8.Size = new Size(102, 23);
			this.panel8.TabIndex = 1;
			this.rBtnLowFrequencyModeOff.AutoSize = true;
			this.rBtnLowFrequencyModeOff.Location = new Point(54, 3);
			this.rBtnLowFrequencyModeOff.Name = "rBtnLowFrequencyModeOff";
			this.rBtnLowFrequencyModeOff.Size = new Size(45, 17);
			this.rBtnLowFrequencyModeOff.TabIndex = 1;
			this.rBtnLowFrequencyModeOff.Text = "OFF";
			this.rBtnLowFrequencyModeOff.UseVisualStyleBackColor = true;
			this.rBtnLowFrequencyModeOff.CheckedChanged += new EventHandler(this.rBtnLowFrequencyMode_CheckedChanged);
			this.rBtnLowFrequencyModeOn.AutoSize = true;
			this.rBtnLowFrequencyModeOn.Checked = true;
			this.rBtnLowFrequencyModeOn.Location = new Point(3, 3);
			this.rBtnLowFrequencyModeOn.Name = "rBtnLowFrequencyModeOn";
			this.rBtnLowFrequencyModeOn.Size = new Size(41, 17);
			this.rBtnLowFrequencyModeOn.TabIndex = 0;
			this.rBtnLowFrequencyModeOn.TabStop = true;
			this.rBtnLowFrequencyModeOn.Text = "ON";
			this.rBtnLowFrequencyModeOn.UseVisualStyleBackColor = true;
			this.rBtnLowFrequencyModeOn.CheckedChanged += new EventHandler(this.rBtnLowFrequencyMode_CheckedChanged);
			this.cBoxBand.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxBand.FormattingEnabled = true;
			this.cBoxBand.Items.AddRange(new object[4]
      {
        (object) "Auto",
        (object) "820-1024",
        (object) "410-525",
        (object) "137-175"
      });
			this.cBoxBand.Location = new Point(164, 19);
			this.cBoxBand.Name = "cBoxBand";
			this.cBoxBand.Size = new Size(124, 21);
			this.cBoxBand.TabIndex = 3;
			this.cBoxBand.SelectedIndexChanged += new EventHandler(this.cBoxBand_SelectedIndexChanged);
			this.label19.AutoSize = true;
			this.label19.Location = new Point(6, 51);
			this.label19.Name = "label19";
			this.label19.Size = new Size(109, 13);
			this.label19.TabIndex = 0;
			this.label19.Text = "Low frequency mode:";
			this.label18.AutoSize = true;
			this.label18.Location = new Point(6, 22);
			this.label18.Name = "label18";
			this.label18.Size = new Size(35, 13);
			this.label18.TabIndex = 2;
			this.label18.Text = "Band:";
			this.label4.AutoSize = true;
			this.label4.Location = new Point(294, 23);
			this.label4.Name = "label4";
			this.label4.Size = new Size(29, 13);
			this.label4.TabIndex = 4;
			this.label4.Text = "MHz";
			this.groupBoxEx1.Controls.Add((Control)this.panel10);
			this.groupBoxEx1.Controls.Add((Control)this.label31);
			this.groupBoxEx1.Controls.Add((Control)this.panel9);
			this.groupBoxEx1.Controls.Add((Control)this.label29);
			this.groupBoxEx1.Location = new Point(583, 154);
			this.groupBoxEx1.Name = "groupBoxEx1";
			this.groupBoxEx1.Size = new Size(355, 78);
			this.groupBoxEx1.TabIndex = 6;
			this.groupBoxEx1.TabStop = false;
			this.groupBoxEx1.Text = "Low frequency band";
			this.groupBoxEx1.Visible = false;
			this.panel10.AutoSize = true;
			this.panel10.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel10.Controls.Add((Control)this.rBtnForceRxBandLowFrequencyOff);
			this.panel10.Controls.Add((Control)this.rBtnForceRxBandLowFrequencyOn);
			this.panel10.Location = new Point(164, 48);
			this.panel10.Name = "panel10";
			this.panel10.Size = new Size(102, 23);
			this.panel10.TabIndex = 1;
			this.rBtnForceRxBandLowFrequencyOff.AutoSize = true;
			this.rBtnForceRxBandLowFrequencyOff.Location = new Point(54, 3);
			this.rBtnForceRxBandLowFrequencyOff.Name = "rBtnForceRxBandLowFrequencyOff";
			this.rBtnForceRxBandLowFrequencyOff.Size = new Size(45, 17);
			this.rBtnForceRxBandLowFrequencyOff.TabIndex = 1;
			this.rBtnForceRxBandLowFrequencyOff.Text = "OFF";
			this.rBtnForceRxBandLowFrequencyOff.UseVisualStyleBackColor = true;
			this.rBtnForceRxBandLowFrequencyOff.CheckedChanged += new EventHandler(this.rBtnForceRxBandLowFrequency_CheckedChanged);
			this.rBtnForceRxBandLowFrequencyOn.AutoSize = true;
			this.rBtnForceRxBandLowFrequencyOn.Checked = true;
			this.rBtnForceRxBandLowFrequencyOn.Location = new Point(3, 3);
			this.rBtnForceRxBandLowFrequencyOn.Name = "rBtnForceRxBandLowFrequencyOn";
			this.rBtnForceRxBandLowFrequencyOn.Size = new Size(41, 17);
			this.rBtnForceRxBandLowFrequencyOn.TabIndex = 0;
			this.rBtnForceRxBandLowFrequencyOn.TabStop = true;
			this.rBtnForceRxBandLowFrequencyOn.Text = "ON";
			this.rBtnForceRxBandLowFrequencyOn.UseVisualStyleBackColor = true;
			this.rBtnForceRxBandLowFrequencyOn.CheckedChanged += new EventHandler(this.rBtnForceRxBandLowFrequency_CheckedChanged);
			this.label31.AutoSize = true;
			this.label31.Location = new Point(6, 53);
			this.label31.Name = "label31";
			this.label31.Size = new Size(53, 13);
			this.label31.TabIndex = 0;
			this.label31.Text = "Force Rx:";
			this.panel9.AutoSize = true;
			this.panel9.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel9.Controls.Add((Control)this.rBtnForceTxBandLowFrequencyOff);
			this.panel9.Controls.Add((Control)this.rBtnForceTxBandLowFrequencyOn);
			this.panel9.Location = new Point(164, 19);
			this.panel9.Name = "panel9";
			this.panel9.Size = new Size(102, 23);
			this.panel9.TabIndex = 1;
			this.rBtnForceTxBandLowFrequencyOff.AutoSize = true;
			this.rBtnForceTxBandLowFrequencyOff.Location = new Point(54, 3);
			this.rBtnForceTxBandLowFrequencyOff.Name = "rBtnForceTxBandLowFrequencyOff";
			this.rBtnForceTxBandLowFrequencyOff.Size = new Size(45, 17);
			this.rBtnForceTxBandLowFrequencyOff.TabIndex = 1;
			this.rBtnForceTxBandLowFrequencyOff.Text = "OFF";
			this.rBtnForceTxBandLowFrequencyOff.UseVisualStyleBackColor = true;
			this.rBtnForceTxBandLowFrequencyOff.CheckedChanged += new EventHandler(this.rBtnForceTxBandLowFrequency_CheckedChanged);
			this.rBtnForceTxBandLowFrequencyOn.AutoSize = true;
			this.rBtnForceTxBandLowFrequencyOn.Checked = true;
			this.rBtnForceTxBandLowFrequencyOn.Location = new Point(3, 3);
			this.rBtnForceTxBandLowFrequencyOn.Name = "rBtnForceTxBandLowFrequencyOn";
			this.rBtnForceTxBandLowFrequencyOn.Size = new Size(41, 17);
			this.rBtnForceTxBandLowFrequencyOn.TabIndex = 0;
			this.rBtnForceTxBandLowFrequencyOn.TabStop = true;
			this.rBtnForceTxBandLowFrequencyOn.Text = "ON";
			this.rBtnForceTxBandLowFrequencyOn.UseVisualStyleBackColor = true;
			this.rBtnForceTxBandLowFrequencyOn.CheckedChanged += new EventHandler(this.rBtnForceTxBandLowFrequency_CheckedChanged);
			this.label29.AutoSize = true;
			this.label29.Location = new Point(6, 24);
			this.label29.Name = "label29";
			this.label29.Size = new Size(52, 13);
			this.label29.TabIndex = 0;
			this.label29.Text = "Force Tx:";
			this.groupBoxEx2.Controls.Add((Control)this.cBoxBand);
			this.groupBoxEx2.Controls.Add((Control)this.panel8);
			this.groupBoxEx2.Controls.Add((Control)this.label4);
			this.groupBoxEx2.Controls.Add((Control)this.label18);
			this.groupBoxEx2.Controls.Add((Control)this.label19);
			this.groupBoxEx2.Location = new Point(583, 238);
			this.groupBoxEx2.Name = "groupBoxEx2";
			this.groupBoxEx2.Size = new Size(355, 83);
			this.groupBoxEx2.TabIndex = 7;
			this.groupBoxEx2.TabStop = false;
			this.groupBoxEx2.Text = "Band";
			this.groupBoxEx2.Visible = false;
			this.AutoScaleDimensions = new SizeF(6f, 13f);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.Controls.Add((Control)this.groupBoxEx2);
			this.Controls.Add((Control)this.groupBoxEx1);
			this.Controls.Add((Control)this.gBoxBatteryManagement);
			this.Controls.Add((Control)this.gBoxOscillators);
			this.Controls.Add((Control)this.gBoxModulation);
			this.Controls.Add((Control)this.gBoxGeneral);
			this.Name = "CommonViewControl";
			this.Size = new Size(799, 493);
			((ISupportInitialize)this.errorProvider).EndInit();
			this.nudBitrate.EndInit();
			this.nudFdev.EndInit();
			this.nudFrequencyRf.EndInit();
			this.gBoxBatteryManagement.ResumeLayout(false);
			this.gBoxBatteryManagement.PerformLayout();
			this.panel4.ResumeLayout(false);
			this.panel4.PerformLayout();
			this.gBoxOscillators.ResumeLayout(false);
			this.gBoxOscillators.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.nudFrequencyXo.EndInit();
			this.gBoxModulation.ResumeLayout(false);
			this.gBoxModulation.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			this.gBoxGeneral.ResumeLayout(false);
			this.gBoxGeneral.PerformLayout();
			this.panel5.ResumeLayout(false);
			this.panel5.PerformLayout();
			this.nudBitrateFrac.EndInit();
			this.panel8.ResumeLayout(false);
			this.panel8.PerformLayout();
			this.groupBoxEx1.ResumeLayout(false);
			this.groupBoxEx1.PerformLayout();
			this.panel10.ResumeLayout(false);
			this.panel10.PerformLayout();
			this.panel9.ResumeLayout(false);
			this.panel9.PerformLayout();
			this.groupBoxEx2.ResumeLayout(false);
			this.groupBoxEx2.PerformLayout();
			this.ResumeLayout(false);
		}

		private void OnFrequencyXoChanged(Decimal value)
		{
			if (FrequencyXoChanged == null)
				return;
			FrequencyXoChanged((object)this, new DecimalEventArg(value));
		}

		private void OnModulationTypeChanged(ModulationTypeEnum value)
		{
			if (ModulationTypeChanged == null)
				return;
			ModulationTypeChanged((object)this, new ModulationTypeEventArg(value));
		}

		private void OnModulationShapingChanged(byte value)
		{
			if (ModulationShapingChanged == null)
				return;
			ModulationShapingChanged((object)this, new ByteEventArg(value));
		}

		private void OnBitrateChanged(Decimal value)
		{
			if (BitrateChanged == null)
				return;
			BitrateChanged((object)this, new DecimalEventArg(value));
		}

		private void OnBitrateFracChanged(Decimal value)
		{
			if (BitrateFracChanged == null)
				return;
			BitrateFracChanged((object)this, new DecimalEventArg(value));
		}

		private void OnFdevChanged(Decimal value)
		{
			if (FdevChanged == null)
				return;
			FdevChanged((object)this, new DecimalEventArg(value));
		}

		private void OnBandChanged(BandEnum value)
		{
			if (BandChanged == null)
				return;
			BandChanged((object)this, new BandEventArg(value));
		}

		private void OnForceTxBandLowFrequencyOnChanged(bool value)
		{
			if (ForceTxBandLowFrequencyOnChanged == null)
				return;
			ForceTxBandLowFrequencyOnChanged((object)this, new BooleanEventArg(value));
		}

		private void OnForceRxBandLowFrequencyOnChanged(bool value)
		{
			if (ForceRxBandLowFrequencyOnChanged == null)
				return;
			ForceRxBandLowFrequencyOnChanged((object)this, new BooleanEventArg(value));
		}

		private void OnLowFrequencyModeOnChanged(bool value)
		{
			if (LowFrequencyModeOnChanged == null)
				return;
			LowFrequencyModeOnChanged((object)this, new BooleanEventArg(value));
		}

		private void OnFrequencyRfChanged(Decimal value)
		{
			if (FrequencyRfChanged == null)
				return;
			FrequencyRfChanged((object)this, new DecimalEventArg(value));
		}

		private void OnFastHopOnChanged(bool value)
		{
			if (FastHopOnChanged == null)
				return;
			FastHopOnChanged((object)this, new BooleanEventArg(value));
		}

		private void OnTcxoInputChanged(bool value)
		{
			if (TcxoInputChanged == null)
				return;
			TcxoInputChanged((object)this, new BooleanEventArg(value));
		}

		private void OnRcCalibrationChanged()
		{
			if (RcCalibrationChanged == null)
				return;
			RcCalibrationChanged((object)this, EventArgs.Empty);
		}

		private void OnLowBatOnChanged(bool value)
		{
			if (LowBatOnChanged == null)
				return;
			LowBatOnChanged((object)this, new BooleanEventArg(value));
		}

		private void OnLowBatTrimChanged(LowBatTrimEnum value)
		{
			if (LowBatTrimChanged == null)
				return;
			LowBatTrimChanged((object)this, new LowBatTrimEventArg(value));
		}

		public void UpdateBitrateLimits(LimitCheckStatusEnum status, string message)
		{
			switch (status)
			{
				case LimitCheckStatusEnum.OK:
					nudBitrate.BackColor = SystemColors.Window;
					break;
				case LimitCheckStatusEnum.OUT_OF_RANGE:
					nudBitrate.BackColor = ControlPaint.LightLight(Color.Orange);
					break;
				case LimitCheckStatusEnum.ERROR:
					nudBitrate.BackColor = ControlPaint.LightLight(Color.Red);
					break;
			}
			errorProvider.SetError((Control)nudBitrate, message);
		}

		public void UpdateFdevLimits(LimitCheckStatusEnum status, string message)
		{
			switch (status)
			{
				case LimitCheckStatusEnum.OK:
					nudFdev.BackColor = SystemColors.Window;
					break;
				case LimitCheckStatusEnum.OUT_OF_RANGE:
					nudFdev.BackColor = ControlPaint.LightLight(Color.Orange);
					break;
				case LimitCheckStatusEnum.ERROR:
					nudFdev.BackColor = ControlPaint.LightLight(Color.Red);
					break;
			}
			errorProvider.SetError((Control)nudFdev, message);
		}

		public void UpdateFrequencyRfLimits(LimitCheckStatusEnum status, string message)
		{
			switch (status)
			{
				case LimitCheckStatusEnum.OK:
					nudFrequencyRf.BackColor = SystemColors.Window;
					break;
				case LimitCheckStatusEnum.OUT_OF_RANGE:
					nudFrequencyRf.BackColor = ControlPaint.LightLight(Color.Orange);
					break;
				case LimitCheckStatusEnum.ERROR:
					nudFrequencyRf.BackColor = ControlPaint.LightLight(Color.Red);
					break;
			}
			errorProvider.SetError((Control)nudFrequencyRf, message);
		}

		private void nudFrequencyXo_ValueChanged(object sender, EventArgs e)
		{
			FrequencyXo = nudFrequencyXo.Value;
			OnFrequencyXoChanged(FrequencyXo);
		}

		private void rBtnModulationType_CheckedChanged(object sender, EventArgs e)
		{
			ModulationType = !rBtnModulationTypeFsk.Checked ? (!rBtnModulationTypeOok.Checked ? ModulationTypeEnum.Reserved2 : ModulationTypeEnum.OOK) : ModulationTypeEnum.FSK;
			OnModulationTypeChanged(ModulationType);
		}

		private void rBtnModulationShaping_CheckedChanged(object sender, EventArgs e)
		{
			if (rBtnModulationShapingOff.Checked)
				ModulationShaping = (byte)0;
			else if (rBtnModulationShaping01.Checked)
				ModulationShaping = (byte)1;
			else if (rBtnModulationShaping10.Checked)
				ModulationShaping = (byte)2;
			else if (rBtnModulationShaping11.Checked)
				ModulationShaping = (byte)3;
			OnModulationShapingChanged(ModulationShaping);
		}

		private void nudBitrate_ValueChanged(object sender, EventArgs e)
		{
			int num1 = (int)Math.Round(FrequencyXo / Bitrate - BitrateFrac / new Decimal(16), MidpointRounding.AwayFromZero);
			int num2 = (int)Math.Round(FrequencyXo / nudBitrate.Value - BitrateFrac / new Decimal(16), MidpointRounding.AwayFromZero);
			int num3 = (int)(nudBitrate.Value - Bitrate);
			nudBitrate.ValueChanged -= new EventHandler(nudBitrate_ValueChanged);
			if (num3 >= -1 && num3 <= 1)
				nudBitrate.Value = Math.Round(FrequencyXo / ((Decimal)(num2 - num3) + BitrateFrac / new Decimal(16)), MidpointRounding.AwayFromZero);
			else
				nudBitrate.Value = Math.Round(FrequencyXo / ((Decimal)num2 + BitrateFrac / new Decimal(16)), MidpointRounding.AwayFromZero);
			nudBitrate.ValueChanged += new EventHandler(nudBitrate_ValueChanged);
			Bitrate = nudBitrate.Value;
			OnBitrateChanged(Bitrate);
		}

		private void nudBitrateFrac_ValueChanged(object sender, EventArgs e)
		{
			BitrateFrac = nudBitrateFrac.Value;
			OnBitrateFracChanged(BitrateFrac);
		}

		private void nudFdev_ValueChanged(object sender, EventArgs e)
		{
			Fdev = nudFdev.Value;
			OnFdevChanged(Fdev);
		}

		private void cBoxBand_SelectedIndexChanged(object sender, EventArgs e)
		{
			Band = (BandEnum)cBoxBand.SelectedIndex;
			OnBandChanged(Band);
		}

		private void rBtnForceTxBandLowFrequency_CheckedChanged(object sender, EventArgs e)
		{
			ForceTxBandLowFrequencyOn = rBtnForceTxBandLowFrequencyOn.Checked;
			OnForceTxBandLowFrequencyOnChanged(ForceTxBandLowFrequencyOn);
		}

		private void rBtnForceRxBandLowFrequency_CheckedChanged(object sender, EventArgs e)
		{
			ForceRxBandLowFrequencyOn = rBtnForceRxBandLowFrequencyOn.Checked;
			OnForceRxBandLowFrequencyOnChanged(ForceRxBandLowFrequencyOn);
		}

		private void rBtnLowFrequencyMode_CheckedChanged(object sender, EventArgs e)
		{
			LowFrequencyModeOn = rBtnLowFrequencyModeOn.Checked;
			OnLowFrequencyModeOnChanged(LowFrequencyModeOn);
		}

		private void nudFrequencyRf_ValueChanged(object sender, EventArgs e)
		{
			FrequencyRf = nudFrequencyRf.Value;
			OnFrequencyRfChanged(FrequencyRf);
		}

		private void rBtnFastHop_CheckedChanged(object sender, EventArgs e)
		{
			FastHopOn = rBtnFastHopOn.Checked;
			OnFastHopOnChanged(FastHopOn);
		}

		private void rBtnTcxoInput_CheckedChanged(object sender, EventArgs e)
		{
			OnTcxoInputChanged(TcxoInputOn);
		}

		private void btnRcCalibration_Click(object sender, EventArgs e)
		{
			OnRcCalibrationChanged();
		}

		private void rBtnLowBatOn_CheckedChanged(object sender, EventArgs e)
		{
			LowBatOn = rBtnLowBatOn.Checked;
			OnLowBatOnChanged(LowBatOn);
		}

		private void cBoxLowBatTrim_SelectedIndexChanged(object sender, EventArgs e)
		{
			LowBatTrim = (LowBatTrimEnum)cBoxLowBatTrim.SelectedIndex;
			OnLowBatTrimChanged(LowBatTrim);
		}

		private void control_MouseEnter(object sender, EventArgs e)
		{
			if (sender == gBoxGeneral)
				OnDocumentationChanged(new DocumentationChangedEventArgs("Common", "General"));
			else if (sender == gBoxModulation)
				OnDocumentationChanged(new DocumentationChangedEventArgs("Common", "Modulation"));
			else if (sender == gBoxOscillators)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Common", "Oscillators"));
			}
			else
			{
				if (sender != gBoxBatteryManagement)
					return;
				OnDocumentationChanged(new DocumentationChangedEventArgs("Common", "Battery management"));
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
	}
}

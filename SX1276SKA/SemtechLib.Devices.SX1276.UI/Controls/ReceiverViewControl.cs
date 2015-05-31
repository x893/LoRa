using SemtechLib.Controls;
using SemtechLib.Devices.SX1276;
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
	public class ReceiverViewControl : UserControl, INotifyDocumentationChanged
	{
		public event Int32EventHandler AgcReferenceLevelChanged;
		public event AgcStepEventHandler AgcStepChanged;
		public event LnaGainEventHandler LnaGainChanged;
		public event BooleanEventHandler LnaBoostChanged;
		public event BooleanEventHandler RestartRxOnCollisionOnChanged;
		public event EventHandler RestartRxWithoutPllLockChanged;
		public event EventHandler RestartRxWithPllLockChanged;
		public event BooleanEventHandler AfcAutoOnChanged;
		public event BooleanEventHandler AgcAutoOnChanged;
		public event RxTriggerEventHandler RxTriggerChanged;
		public event DecimalEventHandler RssiOffsetChanged;
		public event DecimalEventHandler RssiSmoothingChanged;
		public event DecimalEventHandler RssiCollisionThresholdChanged;
		public event DecimalEventHandler RssiThreshChanged;
		public event DecimalEventHandler RxBwChanged;
		public event DecimalEventHandler AfcRxBwChanged;
		public event BooleanEventHandler BitSyncOnChanged;
		public event OokThreshTypeEventHandler OokThreshTypeChanged;
		public event DecimalEventHandler OokPeakThreshStepChanged;
		public event OokPeakThreshDecEventHandler OokPeakThreshDecChanged;
		public event OokAverageThreshFiltEventHandler OokAverageThreshFiltChanged;
		public event DecimalEventHandler OokAverageBiasChanged;
		public event ByteEventHandler OokFixedThreshChanged;
		public event EventHandler AgcStartChanged;
		public event EventHandler FeiReadChanged;
		public event BooleanEventHandler AfcAutoClearOnChanged;
		public event EventHandler AfcClearChanged;
		public event BooleanEventHandler PreambleDetectorOnChanged;
		public event ByteEventHandler PreambleDetectorSizeChanged;
		public event ByteEventHandler PreambleDetectorTolChanged;
		public event DecimalEventHandler TimeoutRssiChanged;
		public event DecimalEventHandler TimeoutPreambleChanged;
		public event DecimalEventHandler TimeoutSyncWordChanged;
		public event DecimalEventHandler AutoRxRestartDelayChanged;
		public event DocumentationChangedEventHandler DocumentationChanged;

		#region Privates
		private Decimal frequencyXo = new Decimal(32000000);
		private DataModeEnum dataMode = DataModeEnum.Packet;
		private Decimal bitRate = new Decimal(4800);
		private bool lowFrequencyModeOn = true;
		private int agcReference = 19;
		private Decimal rssiSmoothing = new Decimal(16);
		private Decimal rssiValue = new Decimal(1275, 0, 0, true, (byte)1);
		private Decimal rxBw = new Decimal(10417);
		private Decimal afcRxBw = new Decimal(50000);
		private Decimal ookPeakThreshStep = new Decimal(5, 0, 0, false, (byte)1);
		private Decimal afcValue = new Decimal(0, 0, 0, false, (byte)1);
		private Decimal feiValue = new Decimal(0, 0, 0, false, (byte)1);
		private IContainer components;
		private ErrorProvider errorProvider;
		private Label suffixOOKfixed;
		private Label suffixOOKstep;
		private Label suffixAFCRxBw;
		private Label suffixRxBw;
		private Label lblOokFixed;
		private Label lblOokCutoff;
		private Label lblOokDec;
		private Label lblOokStep;
		private Label lblOokType;
		private Label lblAfcRxBw;
		private Label lblRxBw;
		private NumericUpDownEx nudRxFilterBw;
		private NumericUpDownEx nudRxFilterBwAfc;
		private Panel panel5;
		private RadioButton rBtnAgcAutoOff;
		private RadioButton rBtnAgcAutoOn;
		private Label lblAgcThresh5;
		private Label lblAgcThresh4;
		private Label lblAgcThresh3;
		private Label lblAgcThresh2;
		private Label lblAgcThresh1;
		private Label lblLnaGain6;
		private Label lblLnaGain5;
		private Label lblLnaGain4;
		private Label lblLnaGain3;
		private Label lblLnaGain2;
		private Label lblLnaGain1;
		private Label lblAgcReference;
		private Panel panel6;
		private RadioButton rBtnLnaGain1;
		private RadioButton rBtnLnaGain2;
		private RadioButton rBtnLnaGain3;
		private RadioButton rBtnLnaGain4;
		private RadioButton rBtnLnaGain5;
		private RadioButton rBtnLnaGain6;
		private Label label47;
		private Label label53;
		private Label label52;
		private Label label51;
		private Label label50;
		private Label label49;
		private Label label48;
		private Label label54;
		private Label label55;
		private Label lblRssiValue;
		private Label label56;
		private Label label13;
		private NumericUpDownEx nudRssiThresh;
		private ComboBox cBoxOokThreshType;
		private NumericUpDownEx nudOokPeakThreshStep;
		private ComboBox cBoxOokAverageThreshFilt;
		private ComboBox cBoxOokPeakThreshDec;
		private NumericUpDownEx nudOokFixedThresh;
		private Label label10;
		private Label lblFeiValue;
		private Button btnAfcClear;
		private Label lblAfcValue;
		private Panel panel9;
		private RadioButton rBtnAfcAutoOff;
		private RadioButton rBtnAfcAutoOn;
		private Panel panel8;
		private RadioButton rBtnAfcAutoClearOff;
		private RadioButton rBtnAfcAutoClearOn;
		private Label label20;
		private Label label19;
		private Button btnFeiRead;
		private Label label22;
		private Label label12;
		private NumericUpDownEx nudTimeoutPreamble;
		private NumericUpDownEx nudAutoRxRestartDelay;
		private Label label15;
		private Label label11;
		private Label label14;
		private Label label9;
		private GroupBoxEx gBoxRssi;
		private GroupBoxEx gBoxAfc;
		private GroupBoxEx gBoxDemodulator;
		private GroupBoxEx gBoxRxBw;
		private GroupBoxEx gBoxLnaSettings;
		private Label label17;
		private Label label18;
		private Button btnRestartRxWithoutPllLock;
		private Label label2;
		private Label label5;
		private Panel panel4;
		private RadioButton rBtnRestartRxOnCollisionOff;
		private RadioButton rBtnRestartRxOnCollisionOn;
		private NumericUpDownEx nudRssiSmoothing;
		private Label label25;
		private NumericUpDownEx nudRssiOffset;
		private Label label24;
		private NumericUpDownEx nudRssiCollisionThreshold;
		private Label label26;
		private Label label28;
		private Panel panel13;
		private RadioButton rBtnBitSyncOff;
		private RadioButton rBtnBitSyncOn;
		private NumericUpDownEx nudOokAverageOffset;
		private Label label30;
		private GroupBoxEx gBoxTimeout;
		private NumericUpDownEx nudTimeoutSyncWord;
		private Label label35;
		private Label label27;
		private NumericUpDownEx nudTimeoutRssi;
		private Label label37;
		private Label label36;
		private Button btnRestartRxWithPllLock;
		private GroupBoxEx gBoxRxConfig;
		private Label label16;
		private Label label23;
		private Label label38;
		private Label label39;
		private Button btnAgcStart;
		private Label label40;
		private GroupBoxEx gBoxPreamble;
		private NumericUpDownEx nudPreambleDetectorTol;
		private Label label41;
		private NumericUpDownEx nudPreambleDetectorSize;
		private Label label44;
		private Panel panel1;
		private RadioButton rBtnPreambleDetectorOff;
		private RadioButton rBtnPreambleDetectorOn;
		private Label label57;
		private Label label58;
		private Label label42;
		private Panel pnlHorizontalSeparator;
		private Label label1;
		private Panel panel2;
		private GroupBoxEx gBoxAgc;
		private Label label4;
		private Label label6;
		private Label label29;
		private Label label31;
		private Label label32;
		private Label label33;
		private Label label34;
		private Label label46;
		private Label label59;
		private Label label60;
		private Label label61;
		private Label label62;
		private NumericUpDown nudAgcStep5;
		private NumericUpDown nudAgcStep4;
		private NumericUpDownEx nudAgcReferenceLevel;
		private NumericUpDown nudAgcStep3;
		private NumericUpDown nudAgcStep1;
		private NumericUpDown nudAgcStep2;
		private Panel panel3;
		private RadioButton rBtnLnaBoostOff;
		private RadioButton rBtnLnaBoostOn;
		private ComboBox cBoxRxTrigger;
		private Label label3;
		private ModulationTypeEnum modulationType;
		private int agcThresh1;
		private int agcThresh2;
		private int agcThresh3;
		private int agcThresh4;
		private int agcThresh5;
		#endregion

		public ReceiverViewControl()
		{
			InitializeComponent();
		}

		public Decimal FrequencyXo
		{
			get { return frequencyXo; }
			set { frequencyXo = value; }
		}

		public DataModeEnum DataMode
		{
			get { return dataMode; }
			set { dataMode = value; }
		}

		public ModulationTypeEnum ModulationType
		{
			get { return modulationType; }
			set { modulationType = value; }
		}

		public Decimal Bitrate
		{
			get
			{
				return bitRate;
			}
			set
			{
				if (!(bitRate != value))
					return;
				int num1 = (int)OokAverageThreshFilt;
				cBoxOokAverageThreshFilt.Items.Clear();
				int num2 = 32;
				while (num2 >= 2)
				{
					if (num2 != 16)
						cBoxOokAverageThreshFilt.Items.Add((object)Math.Round(value / (Decimal)((double)num2 * Math.PI)).ToString());
					num2 /= 2;
				}
				OokAverageThreshFilt = (OokAverageThreshFiltEnum)num1;
				try
				{
					nudTimeoutRssi.ValueChanged -= new EventHandler(nudTimeoutRssi_ValueChanged);
					Decimal num3 = (Decimal)(uint)Math.Round(nudTimeoutRssi.Value / new Decimal(1000) / new Decimal(16) / bitRate, MidpointRounding.AwayFromZero);
					nudTimeoutRssi.Maximum = new Decimal((int)byte.MaxValue) * new Decimal(16) / value * new Decimal(1000);
					nudTimeoutRssi.Increment = nudTimeoutRssi.Maximum / new Decimal((int)byte.MaxValue);
					nudTimeoutRssi.Value = num3 * new Decimal(16) / value * new Decimal(1000);
				}
				catch
				{
				}
				finally
				{
					nudTimeoutRssi.ValueChanged += new EventHandler(nudTimeoutRssi_ValueChanged);
				}
				try
				{
					nudTimeoutPreamble.ValueChanged -= new EventHandler(nudTimeoutPreamble_ValueChanged);
					Decimal num3 = (Decimal)(uint)Math.Round(nudTimeoutPreamble.Value / new Decimal(1000) / new Decimal(16) / bitRate, MidpointRounding.AwayFromZero);
					nudTimeoutPreamble.Maximum = new Decimal((int)byte.MaxValue) * new Decimal(16) / value * new Decimal(1000);
					nudTimeoutPreamble.Increment = nudTimeoutPreamble.Maximum / new Decimal((int)byte.MaxValue);
					nudTimeoutPreamble.Value = num3 * new Decimal(16) / value * new Decimal(1000);
				}
				catch { }
				finally
				{
					nudTimeoutPreamble.ValueChanged += new EventHandler(nudTimeoutPreamble_ValueChanged);
				}
				try
				{
					nudTimeoutSyncWord.ValueChanged -= new EventHandler(nudTimeoutSyncWord_ValueChanged);
					Decimal num3 = (Decimal)(uint)Math.Round(nudTimeoutSyncWord.Value / new Decimal(1000) / new Decimal(16) / bitRate, MidpointRounding.AwayFromZero);
					nudTimeoutSyncWord.Maximum = new Decimal((int)byte.MaxValue) * new Decimal(16) / value * new Decimal(1000);
					nudTimeoutSyncWord.Increment = nudTimeoutSyncWord.Maximum / new Decimal((int)byte.MaxValue);
					nudTimeoutSyncWord.Value = num3 * new Decimal(16) / value * new Decimal(1000);
				}
				catch { }
				finally
				{
					nudTimeoutSyncWord.ValueChanged += new EventHandler(nudTimeoutSyncWord_ValueChanged);
				}
				try
				{
					nudAutoRxRestartDelay.ValueChanged -= new EventHandler(nudAutoRxRestartDelay_ValueChanged);
					Decimal num3 = (Decimal)(uint)Math.Round(nudAutoRxRestartDelay.Value / new Decimal(1000) / new Decimal(4) / bitRate, MidpointRounding.AwayFromZero);
					nudAutoRxRestartDelay.Maximum = new Decimal((int)byte.MaxValue) * new Decimal(4) / value * new Decimal(1000);
					nudAutoRxRestartDelay.Increment = nudAutoRxRestartDelay.Maximum / new Decimal((int)byte.MaxValue);
					nudAutoRxRestartDelay.Value = num3 * new Decimal(4) / value * new Decimal(1000);
				}
				catch { }
				finally
				{
					nudAutoRxRestartDelay.ValueChanged += new EventHandler(nudAutoRxRestartDelay_ValueChanged);
				}
				bitRate = value;
			}
		}

		public bool LowFrequencyModeOn
		{
			get
			{
				return lowFrequencyModeOn;
			}
			set
			{
				if (value)
				{
					rBtnLnaBoostOn.Enabled = false;
					rBtnLnaBoostOff.Enabled = false;
				}
				else
				{
					rBtnLnaBoostOn.Enabled = true;
					rBtnLnaBoostOff.Enabled = true;
				}
			}
		}

		public int AgcReference
		{
			get
			{
				return agcReference;
			}
			set
			{
				agcReference = value;
				lblAgcReference.Text = value.ToString();
			}
		}

		public int AgcThresh1
		{
			get
			{
				return agcThresh1;
			}
			set
			{
				agcThresh1 = value;
				lblAgcThresh1.Text = value.ToString();
			}
		}

		public int AgcThresh2
		{
			get
			{
				return agcThresh2;
			}
			set
			{
				agcThresh2 = value;
				lblAgcThresh2.Text = value.ToString();
			}
		}

		public int AgcThresh3
		{
			get
			{
				return agcThresh3;
			}
			set
			{
				agcThresh3 = value;
				lblAgcThresh3.Text = value.ToString();
			}
		}

		public int AgcThresh4
		{
			get
			{
				return agcThresh4;
			}
			set
			{
				agcThresh4 = value;
				lblAgcThresh4.Text = value.ToString();
			}
		}

		public int AgcThresh5
		{
			get
			{
				return agcThresh5;
			}
			set
			{
				agcThresh5 = value;
				lblAgcThresh5.Text = value.ToString();
			}
		}

		public Decimal RxBwMin
		{
			get { return nudRxFilterBw.Minimum; }
			set { nudRxFilterBw.Minimum = value; }
		}

		public Decimal RxBwMax
		{
			get { return nudRxFilterBw.Maximum; }
			set { nudRxFilterBw.Maximum = value; }
		}

		public Decimal AfcRxBwMin
		{
			get { return nudRxFilterBwAfc.Minimum; }
			set { nudRxFilterBwAfc.Minimum = value; }
		}

		public Decimal AfcRxBwMax
		{
			get { return nudRxFilterBwAfc.Maximum; }
			set { nudRxFilterBwAfc.Maximum = value; }
		}

		public int AgcReferenceLevel
		{
			get
			{
				return (int)nudAgcReferenceLevel.Value;
			}
			set
			{
				try
				{
					nudAgcReferenceLevel.ValueChanged -= new EventHandler(nudAgcReferenceLevel_ValueChanged);
					nudAgcReferenceLevel.Value = (Decimal)value;
					nudAgcReferenceLevel.ValueChanged += new EventHandler(nudAgcReferenceLevel_ValueChanged);
				}
				catch (Exception)
				{
					nudAgcReferenceLevel.ValueChanged += new EventHandler(nudAgcReferenceLevel_ValueChanged);
				}
			}
		}

		public byte AgcStep1
		{
			get
			{
				return (byte)nudAgcStep1.Value;
			}
			set
			{
				try
				{
					nudAgcStep1.ValueChanged -= new EventHandler(nudAgcStep_ValueChanged);
					nudAgcStep1.Value = (Decimal)value;
					nudAgcStep1.ValueChanged += new EventHandler(nudAgcStep_ValueChanged);
				}
				catch (Exception)
				{
					nudAgcStep1.ValueChanged += new EventHandler(nudAgcStep_ValueChanged);
				}
			}
		}

		public byte AgcStep2
		{
			get
			{
				return (byte)nudAgcStep2.Value;
			}
			set
			{
				try
				{
					nudAgcStep2.ValueChanged -= new EventHandler(nudAgcStep_ValueChanged);
					nudAgcStep2.Value = (Decimal)value;
					nudAgcStep2.ValueChanged += new EventHandler(nudAgcStep_ValueChanged);
				}
				catch (Exception)
				{
					nudAgcStep2.ValueChanged += new EventHandler(nudAgcStep_ValueChanged);
				}
			}
		}

		public byte AgcStep3
		{
			get
			{
				return (byte)nudAgcStep3.Value;
			}
			set
			{
				try
				{
					nudAgcStep3.ValueChanged -= new EventHandler(nudAgcStep_ValueChanged);
					nudAgcStep3.Value = (Decimal)value;
					nudAgcStep3.ValueChanged += new EventHandler(nudAgcStep_ValueChanged);
				}
				catch (Exception)
				{
					nudAgcStep3.ValueChanged += new EventHandler(nudAgcStep_ValueChanged);
				}
			}
		}

		public byte AgcStep4
		{
			get
			{
				return (byte)nudAgcStep4.Value;
			}
			set
			{
				try
				{
					nudAgcStep4.ValueChanged -= new EventHandler(nudAgcStep_ValueChanged);
					nudAgcStep4.Value = (Decimal)value;
					nudAgcStep4.ValueChanged += new EventHandler(nudAgcStep_ValueChanged);
				}
				catch (Exception)
				{
					nudAgcStep4.ValueChanged += new EventHandler(nudAgcStep_ValueChanged);
				}
			}
		}

		public byte AgcStep5
		{
			get
			{
				return (byte)nudAgcStep5.Value;
			}
			set
			{
				try
				{
					nudAgcStep5.ValueChanged -= new EventHandler(nudAgcStep_ValueChanged);
					nudAgcStep5.Value = (Decimal)value;
					nudAgcStep5.ValueChanged += new EventHandler(nudAgcStep_ValueChanged);
				}
				catch (Exception)
				{
					nudAgcStep5.ValueChanged += new EventHandler(nudAgcStep_ValueChanged);
				}
			}
		}

		public LnaGainEnum LnaGain
		{
			private get
			{
				if (rBtnLnaGain1.Checked)
					return LnaGainEnum.G1;
				if (rBtnLnaGain2.Checked)
					return LnaGainEnum.G2;
				if (rBtnLnaGain3.Checked)
					return LnaGainEnum.G3;
				if (rBtnLnaGain4.Checked)
					return LnaGainEnum.G4;
				if (rBtnLnaGain5.Checked)
					return LnaGainEnum.G5;
				return rBtnLnaGain6.Checked ? LnaGainEnum.G6 : LnaGainEnum.G1;
			}
			set
			{
				rBtnLnaGain1.CheckedChanged -= new EventHandler(rBtnLnaGain_CheckedChanged);
				rBtnLnaGain2.CheckedChanged -= new EventHandler(rBtnLnaGain_CheckedChanged);
				rBtnLnaGain3.CheckedChanged -= new EventHandler(rBtnLnaGain_CheckedChanged);
				rBtnLnaGain4.CheckedChanged -= new EventHandler(rBtnLnaGain_CheckedChanged);
				rBtnLnaGain5.CheckedChanged -= new EventHandler(rBtnLnaGain_CheckedChanged);
				rBtnLnaGain6.CheckedChanged -= new EventHandler(rBtnLnaGain_CheckedChanged);
				switch (value)
				{
					case LnaGainEnum.G1:
						rBtnLnaGain1.Checked = true;
						rBtnLnaGain2.Checked = false;
						rBtnLnaGain3.Checked = false;
						rBtnLnaGain4.Checked = false;
						rBtnLnaGain5.Checked = false;
						rBtnLnaGain6.Checked = false;
						lblLnaGain1.BackColor = Color.LightSteelBlue;
						lblLnaGain2.BackColor = Color.Transparent;
						lblLnaGain3.BackColor = Color.Transparent;
						lblLnaGain4.BackColor = Color.Transparent;
						lblLnaGain5.BackColor = Color.Transparent;
						lblLnaGain6.BackColor = Color.Transparent;
						break;
					case LnaGainEnum.G2:
						rBtnLnaGain1.Checked = false;
						rBtnLnaGain2.Checked = true;
						rBtnLnaGain3.Checked = false;
						rBtnLnaGain4.Checked = false;
						rBtnLnaGain5.Checked = false;
						rBtnLnaGain6.Checked = false;
						lblLnaGain1.BackColor = Color.Transparent;
						lblLnaGain2.BackColor = Color.LightSteelBlue;
						lblLnaGain3.BackColor = Color.Transparent;
						lblLnaGain4.BackColor = Color.Transparent;
						lblLnaGain5.BackColor = Color.Transparent;
						lblLnaGain6.BackColor = Color.Transparent;
						break;
					case LnaGainEnum.G3:
						rBtnLnaGain1.Checked = false;
						rBtnLnaGain2.Checked = false;
						rBtnLnaGain3.Checked = true;
						rBtnLnaGain4.Checked = false;
						rBtnLnaGain5.Checked = false;
						rBtnLnaGain6.Checked = false;
						lblLnaGain1.BackColor = Color.Transparent;
						lblLnaGain2.BackColor = Color.Transparent;
						lblLnaGain3.BackColor = Color.LightSteelBlue;
						lblLnaGain4.BackColor = Color.Transparent;
						lblLnaGain5.BackColor = Color.Transparent;
						lblLnaGain6.BackColor = Color.Transparent;
						break;
					case LnaGainEnum.G4:
						rBtnLnaGain1.Checked = false;
						rBtnLnaGain2.Checked = false;
						rBtnLnaGain3.Checked = false;
						rBtnLnaGain4.Checked = true;
						rBtnLnaGain5.Checked = false;
						rBtnLnaGain6.Checked = false;
						lblLnaGain1.BackColor = Color.Transparent;
						lblLnaGain2.BackColor = Color.Transparent;
						lblLnaGain3.BackColor = Color.Transparent;
						lblLnaGain4.BackColor = Color.LightSteelBlue;
						lblLnaGain5.BackColor = Color.Transparent;
						lblLnaGain6.BackColor = Color.Transparent;
						break;
					case LnaGainEnum.G5:
						rBtnLnaGain1.Checked = false;
						rBtnLnaGain2.Checked = false;
						rBtnLnaGain3.Checked = false;
						rBtnLnaGain4.Checked = false;
						rBtnLnaGain5.Checked = true;
						rBtnLnaGain6.Checked = false;
						lblLnaGain1.BackColor = Color.Transparent;
						lblLnaGain2.BackColor = Color.Transparent;
						lblLnaGain3.BackColor = Color.Transparent;
						lblLnaGain4.BackColor = Color.Transparent;
						lblLnaGain5.BackColor = Color.LightSteelBlue;
						lblLnaGain6.BackColor = Color.Transparent;
						break;
					case LnaGainEnum.G6:
						rBtnLnaGain1.Checked = false;
						rBtnLnaGain2.Checked = false;
						rBtnLnaGain3.Checked = false;
						rBtnLnaGain4.Checked = false;
						rBtnLnaGain5.Checked = false;
						rBtnLnaGain6.Checked = true;
						lblLnaGain1.BackColor = Color.Transparent;
						lblLnaGain2.BackColor = Color.Transparent;
						lblLnaGain3.BackColor = Color.Transparent;
						lblLnaGain4.BackColor = Color.Transparent;
						lblLnaGain5.BackColor = Color.Transparent;
						lblLnaGain6.BackColor = Color.LightSteelBlue;
						break;
				}
				rBtnLnaGain1.CheckedChanged += new EventHandler(rBtnLnaGain_CheckedChanged);
				rBtnLnaGain2.CheckedChanged += new EventHandler(rBtnLnaGain_CheckedChanged);
				rBtnLnaGain3.CheckedChanged += new EventHandler(rBtnLnaGain_CheckedChanged);
				rBtnLnaGain4.CheckedChanged += new EventHandler(rBtnLnaGain_CheckedChanged);
				rBtnLnaGain5.CheckedChanged += new EventHandler(rBtnLnaGain_CheckedChanged);
				rBtnLnaGain6.CheckedChanged += new EventHandler(rBtnLnaGain_CheckedChanged);
			}
		}

		public bool LnaBoost
		{
			get
			{
				return rBtnLnaBoostOn.Checked;
			}
			set
			{
				rBtnLnaBoostOn.CheckedChanged -= new EventHandler(rBtnLnaBoost_CheckedChanged);
				rBtnLnaBoostOff.CheckedChanged -= new EventHandler(rBtnLnaBoost_CheckedChanged);
				if (value)
				{
					rBtnLnaBoostOn.Checked = true;
					rBtnLnaBoostOff.Checked = false;
				}
				else
				{
					rBtnLnaBoostOn.Checked = false;
					rBtnLnaBoostOff.Checked = true;
				}
				rBtnLnaBoostOn.CheckedChanged += new EventHandler(rBtnLnaBoost_CheckedChanged);
				rBtnLnaBoostOff.CheckedChanged += new EventHandler(rBtnLnaBoost_CheckedChanged);
			}
		}

		public bool RestartRxOnCollision
		{
			get
			{
				return rBtnRestartRxOnCollisionOn.Checked;
			}
			set
			{
				rBtnRestartRxOnCollisionOn.CheckedChanged -= new EventHandler(rBtnRestartRxOnCollisionOn_CheckedChanged);
				rBtnRestartRxOnCollisionOff.CheckedChanged -= new EventHandler(rBtnRestartRxOnCollisionOn_CheckedChanged);
				if (value)
				{
					rBtnRestartRxOnCollisionOn.Checked = true;
					rBtnRestartRxOnCollisionOff.Checked = false;
				}
				else
				{
					rBtnRestartRxOnCollisionOn.Checked = false;
					rBtnRestartRxOnCollisionOff.Checked = true;
				}
				rBtnRestartRxOnCollisionOn.CheckedChanged += new EventHandler(rBtnRestartRxOnCollisionOn_CheckedChanged);
				rBtnRestartRxOnCollisionOff.CheckedChanged += new EventHandler(rBtnRestartRxOnCollisionOn_CheckedChanged);
			}
		}

		public bool AfcAutoOn
		{
			get
			{
				return rBtnAfcAutoOn.Checked;
			}
			set
			{
				rBtnAfcAutoOn.CheckedChanged -= new EventHandler(rBtnAfcAutoOn_CheckedChanged);
				rBtnAfcAutoOff.CheckedChanged -= new EventHandler(rBtnAfcAutoOn_CheckedChanged);
				if (value)
				{
					rBtnAfcAutoOn.Checked = true;
					rBtnAfcAutoOff.Checked = false;
				}
				else
				{
					rBtnAfcAutoOn.Checked = false;
					rBtnAfcAutoOff.Checked = true;
				}
				rBtnAfcAutoOn.CheckedChanged += new EventHandler(rBtnAfcAutoOn_CheckedChanged);
				rBtnAfcAutoOff.CheckedChanged += new EventHandler(rBtnAfcAutoOn_CheckedChanged);
			}
		}

		public bool AgcAutoOn
		{
			get
			{
				return rBtnAgcAutoOn.Checked;
			}
			set
			{
				rBtnAgcAutoOn.CheckedChanged -= new EventHandler(rBtnAgcAutoOn_CheckedChanged);
				rBtnAgcAutoOff.CheckedChanged -= new EventHandler(rBtnAgcAutoOn_CheckedChanged);
				if (value)
				{
					rBtnAgcAutoOn.Checked = true;
					rBtnAgcAutoOff.Checked = false;
					btnAgcStart.Enabled = true;
					rBtnLnaGain1.Enabled = false;
					rBtnLnaGain2.Enabled = false;
					rBtnLnaGain3.Enabled = false;
					rBtnLnaGain4.Enabled = false;
					rBtnLnaGain5.Enabled = false;
					rBtnLnaGain6.Enabled = false;
				}
				else
				{
					rBtnAgcAutoOn.Checked = false;
					rBtnAgcAutoOff.Checked = true;
					btnAgcStart.Enabled = false;
					rBtnLnaGain1.Enabled = true;
					rBtnLnaGain2.Enabled = true;
					rBtnLnaGain3.Enabled = true;
					rBtnLnaGain4.Enabled = true;
					rBtnLnaGain5.Enabled = true;
					rBtnLnaGain6.Enabled = true;
				}
				rBtnAgcAutoOn.CheckedChanged += new EventHandler(rBtnAgcAutoOn_CheckedChanged);
				rBtnAgcAutoOff.CheckedChanged += new EventHandler(rBtnAgcAutoOn_CheckedChanged);
			}
		}

		public RxTriggerEnum RxTrigger
		{
			get
			{
				return (RxTriggerEnum)cBoxRxTrigger.SelectedIndex;
			}
			set
			{
				cBoxRxTrigger.SelectedIndexChanged -= new EventHandler(cBoxRxTrigger_SelectedIndexChanged);
				cBoxRxTrigger.SelectedIndex = (int)value;
				cBoxRxTrigger.SelectedIndexChanged += new EventHandler(cBoxRxTrigger_SelectedIndexChanged);
			}
		}

		public Decimal RssiOffset
		{
			get
			{
				return nudRssiOffset.Value;
			}
			set
			{
				try
				{
					nudRssiOffset.ValueChanged -= new EventHandler(nudRssiOffset_ValueChanged);
					nudRssiOffset.Value = value;
				}
				finally
				{
					nudRssiOffset.ValueChanged += new EventHandler(nudRssiOffset_ValueChanged);
				}
			}
		}

		public Decimal RssiSmoothing
		{
			get
			{
				return rssiSmoothing;
			}
			set
			{
				try
				{
					nudRssiSmoothing.ValueChanged -= new EventHandler(nudRssiSmoothing_ValueChanged);
					rssiSmoothing = (Decimal)Math.Pow(2.0, (double)(ushort)Math.Log((double)value, 2.0));
					nudRssiSmoothing.Value = rssiSmoothing;
				}
				finally
				{
					nudRssiSmoothing.ValueChanged += new EventHandler(nudRssiSmoothing_ValueChanged);
				}
			}
		}

		public Decimal RssiCollisionThreshold
		{
			get
			{
				return nudRssiCollisionThreshold.Value;
			}
			set
			{
				try
				{
					nudRssiCollisionThreshold.ValueChanged -= new EventHandler(nudRssiCollisionThreshold_ValueChanged);
					nudRssiCollisionThreshold.Value = value;
				}
				finally
				{
					nudRssiCollisionThreshold.ValueChanged += new EventHandler(nudRssiCollisionThreshold_ValueChanged);
				}
			}
		}

		public Decimal RssiThreshold
		{
			get
			{
				return nudRssiThresh.Value;
			}
			set
			{
				try
				{
					nudRssiThresh.ValueChanged -= new EventHandler(nudRssiThresh_ValueChanged);
					nudRssiThresh.Value = value;
					nudRssiThresh.ValueChanged += new EventHandler(nudRssiThresh_ValueChanged);
				}
				catch (Exception)
				{
					nudRssiThresh.ValueChanged += new EventHandler(nudRssiThresh_ValueChanged);
				}
			}
		}

		public Decimal RssiValue
		{
			get
			{
				return rssiValue;
			}
			set
			{
				rssiValue = value;
				lblRssiValue.Text = value.ToString("###.0");
			}
		}

		public Decimal RxBw
		{
			get
			{
				return rxBw;
			}
			set
			{
				try
				{
					nudRxFilterBw.ValueChanged -= new EventHandler(nudRxFilterBw_ValueChanged);
					int mant = 0;
					int exp = 0;
					SX1276.ComputeRxBwMantExp(frequencyXo, ModulationType, value, ref mant, ref exp);
					rxBw = SX1276.ComputeRxBw(frequencyXo, ModulationType, mant, exp);
					nudRxFilterBw.Value = rxBw;
					nudRxFilterBw.ValueChanged += new EventHandler(nudRxFilterBw_ValueChanged);
				}
				catch (Exception)
				{
					nudRxFilterBw.ValueChanged += new EventHandler(nudRxFilterBw_ValueChanged);
				}
			}
		}

		public Decimal AfcRxBw
		{
			get
			{
				return afcRxBw;
			}
			set
			{
				try
				{
					nudRxFilterBwAfc.ValueChanged -= new EventHandler(nudRxFilterBwAfc_ValueChanged);
					int mant = 0;
					int exp = 0;
					SX1276.ComputeRxBwMantExp(frequencyXo, ModulationType, value, ref mant, ref exp);
					afcRxBw = SX1276.ComputeRxBw(frequencyXo, ModulationType, mant, exp);
					nudRxFilterBwAfc.Value = afcRxBw;
					nudRxFilterBwAfc.ValueChanged += new EventHandler(nudRxFilterBwAfc_ValueChanged);
				}
				catch (Exception)
				{
					nudRxFilterBwAfc.ValueChanged += new EventHandler(nudRxFilterBwAfc_ValueChanged);
				}
			}
		}

		public bool BitSyncOn
		{
			get
			{
				return rBtnBitSyncOn.Checked;
			}
			set
			{
				rBtnBitSyncOn.CheckedChanged -= new EventHandler(rBtnBitSyncOn_CheckedChanged);
				rBtnBitSyncOff.CheckedChanged -= new EventHandler(rBtnBitSyncOn_CheckedChanged);
				if (value)
				{
					rBtnBitSyncOn.Checked = true;
					rBtnBitSyncOff.Checked = false;
				}
				else
				{
					rBtnBitSyncOn.Checked = false;
					rBtnBitSyncOff.Checked = true;
				}
				rBtnBitSyncOn.CheckedChanged += new EventHandler(rBtnBitSyncOn_CheckedChanged);
				rBtnBitSyncOff.CheckedChanged += new EventHandler(rBtnBitSyncOn_CheckedChanged);
			}
		}

		public OokThreshTypeEnum OokThreshType
		{
			get
			{
				return (OokThreshTypeEnum)cBoxOokThreshType.SelectedIndex;
			}
			set
			{
				cBoxOokThreshType.SelectedIndexChanged -= new EventHandler(cBoxOokThreshType_SelectedIndexChanged);
				switch (value)
				{
					case OokThreshTypeEnum.Fixed:
						cBoxOokThreshType.SelectedIndex = 0;
						nudOokPeakThreshStep.Enabled = false;
						cBoxOokPeakThreshDec.Enabled = false;
						cBoxOokAverageThreshFilt.Enabled = false;
						nudOokFixedThresh.Enabled = true;
						break;
					case OokThreshTypeEnum.Peak:
						cBoxOokThreshType.SelectedIndex = 1;
						nudOokPeakThreshStep.Enabled = true;
						cBoxOokPeakThreshDec.Enabled = true;
						cBoxOokAverageThreshFilt.Enabled = false;
						nudOokFixedThresh.Enabled = true;
						break;
					case OokThreshTypeEnum.Average:
						cBoxOokThreshType.SelectedIndex = 2;
						nudOokPeakThreshStep.Enabled = false;
						cBoxOokPeakThreshDec.Enabled = false;
						cBoxOokAverageThreshFilt.Enabled = true;
						nudOokFixedThresh.Enabled = false;
						break;
					default:
						cBoxOokThreshType.SelectedIndex = -1;
						break;
				}
				cBoxOokThreshType.SelectedIndexChanged += new EventHandler(cBoxOokThreshType_SelectedIndexChanged);
			}
		}

		public Decimal OokPeakThreshStep
		{
			get
			{
				return ookPeakThreshStep;
			}
			set
			{
				try
				{
					nudOokPeakThreshStep.ValueChanged -= new EventHandler(nudOokPeakThreshStep_ValueChanged);
					Decimal[] array = new Decimal[8]
          {
            new Decimal(5, 0, 0, false, (byte) 1),
            new Decimal(10, 0, 0, false, (byte) 1),
            new Decimal(15, 0, 0, false, (byte) 1),
            new Decimal(20, 0, 0, false, (byte) 1),
            new Decimal(30, 0, 0, false, (byte) 1),
            new Decimal(40, 0, 0, false, (byte) 1),
            new Decimal(50, 0, 0, false, (byte) 1),
            new Decimal(60, 0, 0, false, (byte) 1)
          };
					int index = Array.IndexOf<Decimal>(array, value);
					ookPeakThreshStep = array[index];
					nudOokPeakThreshStep.Value = ookPeakThreshStep;
					nudOokPeakThreshStep.ValueChanged += new EventHandler(nudOokPeakThreshStep_ValueChanged);
				}
				catch (Exception)
				{
					nudOokPeakThreshStep.ValueChanged += new EventHandler(nudOokPeakThreshStep_ValueChanged);
				}
			}
		}

		public OokPeakThreshDecEnum OokPeakThreshDec
		{
			get
			{
				return (OokPeakThreshDecEnum)cBoxOokPeakThreshDec.SelectedIndex;
			}
			set
			{
				cBoxOokPeakThreshDec.SelectedIndexChanged -= new EventHandler(cBoxOokPeakThreshDec_SelectedIndexChanged);
				cBoxOokPeakThreshDec.SelectedIndex = (int)value;
				cBoxOokPeakThreshDec.SelectedIndexChanged += new EventHandler(cBoxOokPeakThreshDec_SelectedIndexChanged);
			}
		}

		public Decimal OokAverageOffset
		{
			get
			{
				return nudOokAverageOffset.Value;
			}
			set
			{
				try
				{
					nudOokAverageOffset.ValueChanged -= new EventHandler(nudOokAverageOffset_ValueChanged);
					nudOokAverageOffset.Value = value;
				}
				finally
				{
					nudOokAverageOffset.ValueChanged += new EventHandler(nudOokAverageOffset_ValueChanged);
				}
			}
		}

		public OokAverageThreshFiltEnum OokAverageThreshFilt
		{
			get
			{
				return (OokAverageThreshFiltEnum)cBoxOokAverageThreshFilt.SelectedIndex;
			}
			set
			{
				cBoxOokAverageThreshFilt.SelectedIndexChanged -= new EventHandler(cBoxOokAverageThreshFilt_SelectedIndexChanged);
				cBoxOokAverageThreshFilt.SelectedIndex = (int)value;
				cBoxOokAverageThreshFilt.SelectedIndexChanged += new EventHandler(cBoxOokAverageThreshFilt_SelectedIndexChanged);
			}
		}

		public byte OokFixedThreshold
		{
			get
			{
				return (byte)nudOokFixedThresh.Value;
			}
			set
			{
				try
				{
					nudOokFixedThresh.ValueChanged -= new EventHandler(nudOokFixedThresh_ValueChanged);
					nudOokFixedThresh.Value = (Decimal)value;
					nudOokFixedThresh.ValueChanged += new EventHandler(nudOokFixedThresh_ValueChanged);
				}
				catch (Exception)
				{
					nudOokFixedThresh.ValueChanged += new EventHandler(nudOokFixedThresh_ValueChanged);
				}
			}
		}

		public bool AfcAutoClearOn
		{
			get
			{
				return rBtnAfcAutoClearOn.Checked;
			}
			set
			{
				rBtnAfcAutoClearOn.CheckedChanged -= new EventHandler(rBtnAfcAutoClearOn_CheckedChanged);
				rBtnAfcAutoClearOff.CheckedChanged -= new EventHandler(rBtnAfcAutoClearOn_CheckedChanged);
				if (value)
				{
					rBtnAfcAutoClearOn.Checked = true;
					rBtnAfcAutoClearOff.Checked = false;
				}
				else
				{
					rBtnAfcAutoClearOn.Checked = false;
					rBtnAfcAutoClearOff.Checked = true;
				}
				rBtnAfcAutoClearOn.CheckedChanged += new EventHandler(rBtnAfcAutoClearOn_CheckedChanged);
				rBtnAfcAutoClearOff.CheckedChanged += new EventHandler(rBtnAfcAutoClearOn_CheckedChanged);
			}
		}

		public Decimal AfcValue
		{
			get
			{
				return afcValue;
			}
			set
			{
				afcValue = value;
				lblAfcValue.Text = afcValue.ToString("N0");
			}
		}

		public Decimal FeiValue
		{
			get
			{
				return feiValue;
			}
			set
			{
				feiValue = value;
				lblFeiValue.Text = feiValue.ToString("N0");
			}
		}

		public bool PreambleDetectorOn
		{
			get
			{
				return rBtnPreambleDetectorOn.Checked;
			}
			set
			{
				rBtnPreambleDetectorOn.CheckedChanged -= new EventHandler(rBtnPreambleDetectorOn_CheckedChanged);
				rBtnPreambleDetectorOff.CheckedChanged -= new EventHandler(rBtnPreambleDetectorOn_CheckedChanged);
				if (value)
				{
					rBtnPreambleDetectorOn.Checked = true;
					rBtnPreambleDetectorOff.Checked = false;
				}
				else
				{
					rBtnPreambleDetectorOn.Checked = false;
					rBtnPreambleDetectorOff.Checked = true;
				}
				rBtnPreambleDetectorOn.CheckedChanged += new EventHandler(rBtnPreambleDetectorOn_CheckedChanged);
				rBtnPreambleDetectorOff.CheckedChanged += new EventHandler(rBtnPreambleDetectorOn_CheckedChanged);
			}
		}

		public byte PreambleDetectorSize
		{
			get
			{
				return (byte)nudPreambleDetectorSize.Value;
			}
			set
			{
				try
				{
					nudPreambleDetectorSize.ValueChanged -= new EventHandler(nudPreambleDetectorSize_ValueChanged);
					nudPreambleDetectorSize.Value = (Decimal)value;
				}
				finally
				{
					nudPreambleDetectorSize.ValueChanged += new EventHandler(nudPreambleDetectorSize_ValueChanged);
				}
			}
		}

		public byte PreambleDetectorTol
		{
			get
			{
				return (byte)nudPreambleDetectorTol.Value;
			}
			set
			{
				try
				{
					nudPreambleDetectorTol.ValueChanged -= new EventHandler(nudPreambleDetectorTol_ValueChanged);
					nudPreambleDetectorTol.Value = (Decimal)value;
				}
				finally
				{
					nudPreambleDetectorTol.ValueChanged += new EventHandler(nudPreambleDetectorTol_ValueChanged);
				}
			}
		}

		public Decimal TimeoutRxRssi
		{
			get
			{
				return nudTimeoutRssi.Value;
			}
			set
			{
				try
				{
					nudTimeoutRssi.ValueChanged -= new EventHandler(nudTimeoutRssi_ValueChanged);
					nudTimeoutRssi.Value = (Decimal)(uint)Math.Round(value / new Decimal(1000) / new Decimal(16) / Bitrate, MidpointRounding.AwayFromZero) * new Decimal(16) / Bitrate * new Decimal(1000);
					nudTimeoutRssi.ValueChanged += new EventHandler(nudTimeoutRssi_ValueChanged);
				}
				catch (Exception)
				{
					nudTimeoutRssi.ValueChanged += new EventHandler(nudTimeoutRssi_ValueChanged);
				}
			}
		}

		public Decimal TimeoutRxPreamble
		{
			get
			{
				return nudTimeoutPreamble.Value;
			}
			set
			{
				try
				{
					nudTimeoutPreamble.ValueChanged -= new EventHandler(nudTimeoutPreamble_ValueChanged);
					nudTimeoutPreamble.Value = (Decimal)(uint)Math.Round(value / new Decimal(1000) / new Decimal(16) / Bitrate, MidpointRounding.AwayFromZero) * new Decimal(16) / Bitrate * new Decimal(1000);
					nudTimeoutPreamble.ValueChanged += new EventHandler(nudTimeoutPreamble_ValueChanged);
				}
				catch (Exception)
				{
					nudTimeoutPreamble.ValueChanged += new EventHandler(nudTimeoutPreamble_ValueChanged);
				}
			}
		}

		public Decimal TimeoutSignalSync
		{
			get
			{
				return nudTimeoutSyncWord.Value;
			}
			set
			{
				try
				{
					nudTimeoutSyncWord.ValueChanged -= new EventHandler(nudTimeoutSyncWord_ValueChanged);
					nudTimeoutSyncWord.Value = (Decimal)(uint)Math.Round(value / new Decimal(1000) / new Decimal(16) / Bitrate, MidpointRounding.AwayFromZero) * new Decimal(16) / Bitrate * new Decimal(1000);
					nudTimeoutSyncWord.ValueChanged += new EventHandler(nudTimeoutSyncWord_ValueChanged);
				}
				catch (Exception)
				{
					nudTimeoutSyncWord.ValueChanged += new EventHandler(nudTimeoutSyncWord_ValueChanged);
				}
			}
		}

		public Decimal InterPacketRxDelay
		{
			get
			{
				return nudAutoRxRestartDelay.Value;
			}
			set
			{
				try
				{
					nudAutoRxRestartDelay.ValueChanged -= new EventHandler(nudAutoRxRestartDelay_ValueChanged);
					nudAutoRxRestartDelay.Value = (Decimal)(uint)Math.Round(value / new Decimal(1000) / new Decimal(4) / Bitrate, MidpointRounding.AwayFromZero) * new Decimal(4) / Bitrate * new Decimal(1000);
					nudAutoRxRestartDelay.ValueChanged += new EventHandler(nudAutoRxRestartDelay_ValueChanged);
				}
				catch (Exception)
				{
					nudAutoRxRestartDelay.ValueChanged += new EventHandler(nudAutoRxRestartDelay_ValueChanged);
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
			this.errorProvider = new ErrorProvider(components);
			this.nudPreambleDetectorTol = new NumericUpDownEx();
			this.nudPreambleDetectorSize = new NumericUpDownEx();
			this.cBoxRxTrigger = new ComboBox();
			this.nudTimeoutRssi = new NumericUpDownEx();
			this.nudAutoRxRestartDelay = new NumericUpDownEx();
			this.nudTimeoutSyncWord = new NumericUpDownEx();
			this.nudTimeoutPreamble = new NumericUpDownEx();
			this.nudRxFilterBwAfc = new NumericUpDownEx();
			this.nudRxFilterBw = new NumericUpDownEx();
			this.nudOokAverageOffset = new NumericUpDownEx();
			this.cBoxOokThreshType = new ComboBox();
			this.nudOokPeakThreshStep = new NumericUpDownEx();
			this.nudOokFixedThresh = new NumericUpDownEx();
			this.cBoxOokPeakThreshDec = new ComboBox();
			this.cBoxOokAverageThreshFilt = new ComboBox();
			this.nudRssiOffset = new NumericUpDownEx();
			this.nudRssiSmoothing = new NumericUpDownEx();
			this.nudRssiCollisionThreshold = new NumericUpDownEx();
			this.lblRssiValue = new Label();
			this.nudRssiThresh = new NumericUpDownEx();
			this.panel2 = new Panel();
			this.gBoxAgc = new GroupBoxEx();
			this.label4 = new Label();
			this.label39 = new Label();
			this.label6 = new Label();
			this.label29 = new Label();
			this.label31 = new Label();
			this.label32 = new Label();
			this.label16 = new Label();
			this.label33 = new Label();
			this.label34 = new Label();
			this.label46 = new Label();
			this.btnAgcStart = new Button();
			this.panel5 = new Panel();
			this.rBtnAgcAutoOff = new RadioButton();
			this.rBtnAgcAutoOn = new RadioButton();
			this.label59 = new Label();
			this.label60 = new Label();
			this.label61 = new Label();
			this.label62 = new Label();
			this.nudAgcStep5 = new NumericUpDown();
			this.nudAgcStep4 = new NumericUpDown();
			this.nudAgcReferenceLevel = new NumericUpDownEx();
			this.nudAgcStep3 = new NumericUpDown();
			this.nudAgcStep1 = new NumericUpDown();
			this.nudAgcStep2 = new NumericUpDown();
			this.gBoxTimeout = new GroupBoxEx();
			this.label35 = new Label();
			this.label15 = new Label();
			this.label37 = new Label();
			this.label27 = new Label();
			this.label11 = new Label();
			this.label36 = new Label();
			this.label14 = new Label();
			this.label9 = new Label();
			this.gBoxPreamble = new GroupBoxEx();
			this.label41 = new Label();
			this.label42 = new Label();
			this.label44 = new Label();
			this.panel1 = new Panel();
			this.rBtnPreambleDetectorOff = new RadioButton();
			this.rBtnPreambleDetectorOn = new RadioButton();
			this.label57 = new Label();
			this.label58 = new Label();
			this.gBoxRxBw = new GroupBoxEx();
			this.lblAfcRxBw = new Label();
			this.lblRxBw = new Label();
			this.suffixAFCRxBw = new Label();
			this.suffixRxBw = new Label();
			this.gBoxRxConfig = new GroupBoxEx();
			this.label38 = new Label();
			this.label5 = new Label();
			this.btnRestartRxWithPllLock = new Button();
			this.label3 = new Label();
			this.btnRestartRxWithoutPllLock = new Button();
			this.panel4 = new Panel();
			this.rBtnRestartRxOnCollisionOff = new RadioButton();
			this.rBtnRestartRxOnCollisionOn = new RadioButton();
			this.label26 = new Label();
			this.gBoxDemodulator = new GroupBoxEx();
			this.pnlHorizontalSeparator = new Panel();
			this.label30 = new Label();
			this.label1 = new Label();
			this.lblOokType = new Label();
			this.lblOokStep = new Label();
			this.label28 = new Label();
			this.lblOokDec = new Label();
			this.lblOokCutoff = new Label();
			this.lblOokFixed = new Label();
			this.suffixOOKstep = new Label();
			this.label40 = new Label();
			this.suffixOOKfixed = new Label();
			this.panel13 = new Panel();
			this.rBtnBitSyncOff = new RadioButton();
			this.rBtnBitSyncOn = new RadioButton();
			this.gBoxRssi = new GroupBoxEx();
			this.label23 = new Label();
			this.label17 = new Label();
			this.label54 = new Label();
			this.label24 = new Label();
			this.label55 = new Label();
			this.label25 = new Label();
			this.label56 = new Label();
			this.gBoxAfc = new GroupBoxEx();
			this.label19 = new Label();
			this.btnFeiRead = new Button();
			this.panel8 = new Panel();
			this.rBtnAfcAutoClearOff = new RadioButton();
			this.rBtnAfcAutoClearOn = new RadioButton();
			this.lblFeiValue = new Label();
			this.label12 = new Label();
			this.label20 = new Label();
			this.label18 = new Label();
			this.label10 = new Label();
			this.btnAfcClear = new Button();
			this.lblAfcValue = new Label();
			this.label22 = new Label();
			this.panel9 = new Panel();
			this.rBtnAfcAutoOff = new RadioButton();
			this.rBtnAfcAutoOn = new RadioButton();
			this.gBoxLnaSettings = new GroupBoxEx();
			this.panel3 = new Panel();
			this.rBtnLnaBoostOff = new RadioButton();
			this.rBtnLnaBoostOn = new RadioButton();
			this.label2 = new Label();
			this.label13 = new Label();
			this.lblAgcReference = new Label();
			this.label48 = new Label();
			this.label49 = new Label();
			this.label50 = new Label();
			this.label51 = new Label();
			this.label52 = new Label();
			this.lblLnaGain1 = new Label();
			this.label53 = new Label();
			this.panel6 = new Panel();
			this.rBtnLnaGain1 = new RadioButton();
			this.rBtnLnaGain2 = new RadioButton();
			this.rBtnLnaGain3 = new RadioButton();
			this.rBtnLnaGain4 = new RadioButton();
			this.rBtnLnaGain5 = new RadioButton();
			this.rBtnLnaGain6 = new RadioButton();
			this.lblLnaGain2 = new Label();
			this.lblLnaGain3 = new Label();
			this.lblLnaGain4 = new Label();
			this.lblLnaGain5 = new Label();
			this.lblLnaGain6 = new Label();
			this.lblAgcThresh1 = new Label();
			this.lblAgcThresh2 = new Label();
			this.lblAgcThresh3 = new Label();
			this.lblAgcThresh4 = new Label();
			this.lblAgcThresh5 = new Label();
			this.label47 = new Label();
			((ISupportInitialize)this.errorProvider).BeginInit();
			this.nudPreambleDetectorTol.BeginInit();
			this.nudPreambleDetectorSize.BeginInit();
			this.nudTimeoutRssi.BeginInit();
			this.nudAutoRxRestartDelay.BeginInit();
			this.nudTimeoutSyncWord.BeginInit();
			this.nudTimeoutPreamble.BeginInit();
			this.nudRxFilterBwAfc.BeginInit();
			this.nudRxFilterBw.BeginInit();
			this.nudOokAverageOffset.BeginInit();
			this.nudOokPeakThreshStep.BeginInit();
			this.nudOokFixedThresh.BeginInit();
			this.nudRssiOffset.BeginInit();
			this.nudRssiSmoothing.BeginInit();
			this.nudRssiCollisionThreshold.BeginInit();
			this.nudRssiThresh.BeginInit();
			this.panel2.SuspendLayout();
			this.gBoxAgc.SuspendLayout();
			this.panel5.SuspendLayout();
			this.nudAgcStep5.BeginInit();
			this.nudAgcStep4.BeginInit();
			this.nudAgcReferenceLevel.BeginInit();
			this.nudAgcStep3.BeginInit();
			this.nudAgcStep1.BeginInit();
			this.nudAgcStep2.BeginInit();
			this.gBoxTimeout.SuspendLayout();
			this.gBoxPreamble.SuspendLayout();
			this.panel1.SuspendLayout();
			this.gBoxRxBw.SuspendLayout();
			this.gBoxRxConfig.SuspendLayout();
			this.panel4.SuspendLayout();
			this.gBoxDemodulator.SuspendLayout();
			this.panel13.SuspendLayout();
			this.gBoxRssi.SuspendLayout();
			this.gBoxAfc.SuspendLayout();
			this.panel8.SuspendLayout();
			this.panel9.SuspendLayout();
			this.gBoxLnaSettings.SuspendLayout();
			this.panel3.SuspendLayout();
			this.panel6.SuspendLayout();
			this.SuspendLayout();
			this.errorProvider.ContainerControl = (ContainerControl)this;
			this.errorProvider.SetIconPadding((Control)this.nudPreambleDetectorTol, 30);
			this.nudPreambleDetectorTol.Location = new Point(120, 65);
			NumericUpDownEx numericUpDownEx1 = this.nudPreambleDetectorTol;
			int[] bits1 = new int[4];
			bits1[0] = 31;
			Decimal num1 = new Decimal(bits1);
			numericUpDownEx1.Maximum = num1;
			this.nudPreambleDetectorTol.Name = "nudPreambleDetectorTol";
			this.nudPreambleDetectorTol.Size = new Size(98, 20);
			this.nudPreambleDetectorTol.TabIndex = 6;
			this.nudPreambleDetectorTol.ThousandsSeparator = true;
			this.nudPreambleDetectorTol.ValueChanged += new EventHandler(this.nudPreambleDetectorTol_ValueChanged);
			this.errorProvider.SetIconPadding((Control)this.nudPreambleDetectorSize, 30);
			this.nudPreambleDetectorSize.Location = new Point(120, 40);
			NumericUpDownEx numericUpDownEx2 = this.nudPreambleDetectorSize;
			int[] bits2 = new int[4];
			bits2[0] = 4;
			Decimal num2 = new Decimal(bits2);
			numericUpDownEx2.Maximum = num2;
			NumericUpDownEx numericUpDownEx3 = this.nudPreambleDetectorSize;
			int[] bits3 = new int[4];
			bits3[0] = 1;
			Decimal num3 = new Decimal(bits3);
			numericUpDownEx3.Minimum = num3;
			this.nudPreambleDetectorSize.Name = "nudPreambleDetectorSize";
			this.nudPreambleDetectorSize.Size = new Size(98, 20);
			this.nudPreambleDetectorSize.TabIndex = 6;
			this.nudPreambleDetectorSize.ThousandsSeparator = true;
			NumericUpDownEx numericUpDownEx4 = this.nudPreambleDetectorSize;
			int[] bits4 = new int[4];
			bits4[0] = 1;
			Decimal num4 = new Decimal(bits4);
			numericUpDownEx4.Value = num4;
			this.nudPreambleDetectorSize.ValueChanged += new EventHandler(this.nudPreambleDetectorSize_ValueChanged);
			this.cBoxRxTrigger.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxRxTrigger.FormattingEnabled = true;
			this.errorProvider.SetIconPadding((Control)this.cBoxRxTrigger, 30);
			this.cBoxRxTrigger.Items.AddRange(new object[4]
      {
        (object) "None",
        (object) "RSSI",
        (object) "Preamble",
        (object) "RSSI & Preamble"
      });
			this.cBoxRxTrigger.Location = new Point(125, 97);
			this.cBoxRxTrigger.Name = "cBoxRxTrigger";
			this.cBoxRxTrigger.Size = new Size(102, 21);
			this.cBoxRxTrigger.TabIndex = 22;
			this.cBoxRxTrigger.SelectedIndexChanged += new EventHandler(this.cBoxRxTrigger_SelectedIndexChanged);
			this.nudTimeoutRssi.DecimalPlaces = 3;
			this.errorProvider.SetIconPadding((Control)this.nudTimeoutRssi, 30);
			this.nudTimeoutRssi.Location = new Point(125, 19);
			NumericUpDownEx numericUpDownEx5 = this.nudTimeoutRssi;
			int[] bits5 = new int[4];
			bits5[0] = 850;
			Decimal num5 = new Decimal(bits5);
			numericUpDownEx5.Maximum = num5;
			this.nudTimeoutRssi.Name = "nudTimeoutRssi";
			this.nudTimeoutRssi.Size = new Size(98, 20);
			this.nudTimeoutRssi.TabIndex = 3;
			this.nudTimeoutRssi.ThousandsSeparator = true;
			this.nudTimeoutRssi.ValueChanged += new EventHandler(this.nudTimeoutRssi_ValueChanged);
			this.nudAutoRxRestartDelay.DecimalPlaces = 3;
			this.errorProvider.SetIconPadding((Control)this.nudAutoRxRestartDelay, 30);
			this.nudAutoRxRestartDelay.Location = new Point(125, 97);
			NumericUpDownEx numericUpDownEx6 = this.nudAutoRxRestartDelay;
			int[] bits6 = new int[4];
			bits6[0] = 850;
			Decimal num6 = new Decimal(bits6);
			numericUpDownEx6.Maximum = num6;
			this.nudAutoRxRestartDelay.Name = "nudAutoRxRestartDelay";
			this.nudAutoRxRestartDelay.Size = new Size(98, 20);
			this.nudAutoRxRestartDelay.TabIndex = 3;
			this.nudAutoRxRestartDelay.ThousandsSeparator = true;
			this.nudAutoRxRestartDelay.ValueChanged += new EventHandler(this.nudAutoRxRestartDelay_ValueChanged);
			this.nudTimeoutSyncWord.DecimalPlaces = 3;
			this.errorProvider.SetIconPadding((Control)this.nudTimeoutSyncWord, 30);
			this.nudTimeoutSyncWord.Location = new Point(125, 71);
			NumericUpDownEx numericUpDownEx7 = this.nudTimeoutSyncWord;
			int[] bits7 = new int[4];
			bits7[0] = 850;
			Decimal num7 = new Decimal(bits7);
			numericUpDownEx7.Maximum = num7;
			this.nudTimeoutSyncWord.Name = "nudTimeoutSyncWord";
			this.nudTimeoutSyncWord.Size = new Size(98, 20);
			this.nudTimeoutSyncWord.TabIndex = 6;
			this.nudTimeoutSyncWord.ThousandsSeparator = true;
			this.nudTimeoutSyncWord.ValueChanged += new EventHandler(this.nudTimeoutSyncWord_ValueChanged);
			this.nudTimeoutPreamble.DecimalPlaces = 3;
			this.errorProvider.SetIconPadding((Control)this.nudTimeoutPreamble, 30);
			this.nudTimeoutPreamble.Location = new Point(125, 45);
			NumericUpDownEx numericUpDownEx8 = this.nudTimeoutPreamble;
			int[] bits8 = new int[4];
			bits8[0] = 850;
			Decimal num8 = new Decimal(bits8);
			numericUpDownEx8.Maximum = num8;
			this.nudTimeoutPreamble.Name = "nudTimeoutPreamble";
			this.nudTimeoutPreamble.Size = new Size(98, 20);
			this.nudTimeoutPreamble.TabIndex = 6;
			this.nudTimeoutPreamble.ThousandsSeparator = true;
			this.nudTimeoutPreamble.ValueChanged += new EventHandler(this.nudTimeoutPreamble_ValueChanged);
			this.errorProvider.SetIconPadding((Control)this.nudRxFilterBwAfc, 30);
			this.nudRxFilterBwAfc.Location = new Point(120, 42);
			NumericUpDownEx numericUpDownEx9 = this.nudRxFilterBwAfc;
			int[] bits9 = new int[4];
			bits9[0] = 400000;
			Decimal num9 = new Decimal(bits9);
			numericUpDownEx9.Maximum = num9;
			NumericUpDownEx numericUpDownEx10 = this.nudRxFilterBwAfc;
			int[] bits10 = new int[4];
			bits10[0] = 3125;
			Decimal num10 = new Decimal(bits10);
			numericUpDownEx10.Minimum = num10;
			this.nudRxFilterBwAfc.Name = "nudRxFilterBwAfc";
			this.nudRxFilterBwAfc.Size = new Size(98, 20);
			this.nudRxFilterBwAfc.TabIndex = 4;
			this.nudRxFilterBwAfc.ThousandsSeparator = true;
			NumericUpDownEx numericUpDownEx11 = this.nudRxFilterBwAfc;
			int[] bits11 = new int[4];
			bits11[0] = 50000;
			Decimal num11 = new Decimal(bits11);
			numericUpDownEx11.Value = num11;
			this.nudRxFilterBwAfc.ValueChanged += new EventHandler(this.nudRxFilterBwAfc_ValueChanged);
			this.errorProvider.SetIconPadding((Control)this.nudRxFilterBw, 25);
			this.nudRxFilterBw.Location = new Point(120, 17);
			NumericUpDownEx numericUpDownEx12 = this.nudRxFilterBw;
			int[] bits12 = new int[4];
			bits12[0] = 500000;
			Decimal num12 = new Decimal(bits12);
			numericUpDownEx12.Maximum = num12;
			NumericUpDownEx numericUpDownEx13 = this.nudRxFilterBw;
			int[] bits13 = new int[4];
			bits13[0] = 3906;
			Decimal num13 = new Decimal(bits13);
			numericUpDownEx13.Minimum = num13;
			this.nudRxFilterBw.Name = "nudRxFilterBw";
			this.nudRxFilterBw.Size = new Size(98, 20);
			this.nudRxFilterBw.TabIndex = 4;
			this.nudRxFilterBw.ThousandsSeparator = true;
			NumericUpDownEx numericUpDownEx14 = this.nudRxFilterBw;
			int[] bits14 = new int[4];
			bits14[0] = 10417;
			Decimal num14 = new Decimal(bits14);
			numericUpDownEx14.Value = num14;
			this.nudRxFilterBw.ValueChanged += new EventHandler(this.nudRxFilterBw_ValueChanged);
			this.errorProvider.SetIconPadding((Control)this.nudOokAverageOffset, 30);
			NumericUpDownEx numericUpDownEx15 = this.nudOokAverageOffset;
			int[] bits15 = new int[4];
			bits15[0] = 2;
			Decimal num15 = new Decimal(bits15);
			numericUpDownEx15.Increment = num15;
			this.nudOokAverageOffset.Location = new Point(125, 188);
			NumericUpDownEx numericUpDownEx16 = this.nudOokAverageOffset;
			int[] bits16 = new int[4];
			bits16[0] = 6;
			Decimal num16 = new Decimal(bits16);
			numericUpDownEx16.Maximum = num16;
			this.nudOokAverageOffset.Name = "nudOokAverageOffset";
			this.nudOokAverageOffset.Size = new Size(98, 20);
			this.nudOokAverageOffset.TabIndex = 3;
			this.nudOokAverageOffset.ThousandsSeparator = true;
			this.nudOokAverageOffset.ValueChanged += new EventHandler(this.nudOokAverageOffset_ValueChanged);
			this.cBoxOokThreshType.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxOokThreshType.FormattingEnabled = true;
			this.errorProvider.SetIconPadding((Control)this.cBoxOokThreshType, 30);
			this.cBoxOokThreshType.Items.AddRange(new object[3]
      {
        (object) "Fixed",
        (object) "Peak",
        (object) "Average"
      });
			this.cBoxOokThreshType.Location = new Point(125, 57);
			this.cBoxOokThreshType.Name = "cBoxOokThreshType";
			this.cBoxOokThreshType.Size = new Size(102, 21);
			this.cBoxOokThreshType.TabIndex = 1;
			this.cBoxOokThreshType.SelectedIndexChanged += new EventHandler(this.cBoxOokThreshType_SelectedIndexChanged);
			this.nudOokPeakThreshStep.DecimalPlaces = 1;
			this.errorProvider.SetIconPadding((Control)this.nudOokPeakThreshStep, 30);
			this.nudOokPeakThreshStep.Increment = new Decimal(new int[4]
      {
        5,
        0,
        0,
        65536
      });
			this.nudOokPeakThreshStep.Location = new Point(125, 83);
			this.nudOokPeakThreshStep.Maximum = new Decimal(new int[4]
      {
        60,
        0,
        0,
        65536
      });
			this.nudOokPeakThreshStep.Minimum = new Decimal(new int[4]
      {
        5,
        0,
        0,
        65536
      });
			this.nudOokPeakThreshStep.Name = "nudOokPeakThreshStep";
			this.nudOokPeakThreshStep.Size = new Size(98, 20);
			this.nudOokPeakThreshStep.TabIndex = 3;
			this.nudOokPeakThreshStep.ThousandsSeparator = true;
			this.nudOokPeakThreshStep.Value = new Decimal(new int[4]
      {
        5,
        0,
        0,
        65536
      });
			this.nudOokPeakThreshStep.ValueChanged += new EventHandler(this.nudOokPeakThreshStep_ValueChanged);
			this.nudOokPeakThreshStep.Validating += new CancelEventHandler(this.nudOokPeakThreshStep_Validating);
			this.errorProvider.SetIconPadding((Control)this.nudOokFixedThresh, 30);
			this.nudOokFixedThresh.Location = new Point(125, 109);
			NumericUpDownEx numericUpDownEx17 = this.nudOokFixedThresh;
			int[] bits17 = new int[4];
			bits17[0] = (int)byte.MaxValue;
			Decimal num17 = new Decimal(bits17);
			numericUpDownEx17.Maximum = num17;
			this.nudOokFixedThresh.Name = "nudOokFixedThresh";
			this.nudOokFixedThresh.Size = new Size(98, 20);
			this.nudOokFixedThresh.TabIndex = 10;
			this.nudOokFixedThresh.ThousandsSeparator = true;
			NumericUpDownEx numericUpDownEx18 = this.nudOokFixedThresh;
			int[] bits18 = new int[4];
			bits18[0] = 6;
			Decimal num18 = new Decimal(bits18);
			numericUpDownEx18.Value = num18;
			this.nudOokFixedThresh.ValueChanged += new EventHandler(this.nudOokFixedThresh_ValueChanged);
			this.cBoxOokPeakThreshDec.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxOokPeakThreshDec.FormattingEnabled = true;
			this.errorProvider.SetIconPadding((Control)this.cBoxOokPeakThreshDec, 30);
			this.cBoxOokPeakThreshDec.Items.AddRange(new object[8]
      {
        (object) "1x per chip",
        (object) "1x every 2 chips",
        (object) "1x every 4 chips",
        (object) "1x every 8 chips",
        (object) "2x per chip",
        (object) "4x per chip",
        (object) "8x per chip",
        (object) "16x per chip"
      });
			this.cBoxOokPeakThreshDec.Location = new Point(125, 135);
			this.cBoxOokPeakThreshDec.Name = "cBoxOokPeakThreshDec";
			this.cBoxOokPeakThreshDec.Size = new Size(102, 21);
			this.cBoxOokPeakThreshDec.TabIndex = 6;
			this.cBoxOokPeakThreshDec.SelectedIndexChanged += new EventHandler(this.cBoxOokPeakThreshDec_SelectedIndexChanged);
			this.cBoxOokAverageThreshFilt.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxOokAverageThreshFilt.FormattingEnabled = true;
			this.errorProvider.SetIconPadding((Control)this.cBoxOokAverageThreshFilt, 30);
			this.cBoxOokAverageThreshFilt.Items.AddRange(new object[4]
      {
        (object) "Bitrate / 32π",
        (object) "Bitrate / 8π",
        (object) "Bitrate / 4π",
        (object) "Bitrate / 2π"
      });
			this.cBoxOokAverageThreshFilt.Location = new Point(125, 161);
			this.cBoxOokAverageThreshFilt.Name = "cBoxOokAverageThreshFilt";
			this.cBoxOokAverageThreshFilt.Size = new Size(102, 21);
			this.cBoxOokAverageThreshFilt.TabIndex = 8;
			this.cBoxOokAverageThreshFilt.SelectedIndexChanged += new EventHandler(this.cBoxOokAverageThreshFilt_SelectedIndexChanged);
			this.errorProvider.SetIconPadding((Control)this.nudRssiOffset, 30);
			this.nudRssiOffset.Location = new Point(125, 19);
			NumericUpDownEx numericUpDownEx19 = this.nudRssiOffset;
			int[] bits19 = new int[4];
			bits19[0] = 15;
			Decimal num19 = new Decimal(bits19);
			numericUpDownEx19.Maximum = num19;
			this.nudRssiOffset.Minimum = new Decimal(new int[4]
      {
        16,
        0,
        0,
        int.MinValue
      });
			this.nudRssiOffset.Name = "nudRssiOffset";
			this.nudRssiOffset.Size = new Size(98, 20);
			this.nudRssiOffset.TabIndex = 3;
			this.nudRssiOffset.ThousandsSeparator = true;
			this.nudRssiOffset.ValueChanged += new EventHandler(this.nudRssiOffset_ValueChanged);
			this.errorProvider.SetIconPadding((Control)this.nudRssiSmoothing, 30);
			this.nudRssiSmoothing.Location = new Point(125, 45);
			NumericUpDownEx numericUpDownEx20 = this.nudRssiSmoothing;
			int[] bits20 = new int[4];
			bits20[0] = 256;
			Decimal num20 = new Decimal(bits20);
			numericUpDownEx20.Maximum = num20;
			NumericUpDownEx numericUpDownEx21 = this.nudRssiSmoothing;
			int[] bits21 = new int[4];
			bits21[0] = 2;
			Decimal num21 = new Decimal(bits21);
			numericUpDownEx21.Minimum = num21;
			this.nudRssiSmoothing.Name = "nudRssiSmoothing";
			this.nudRssiSmoothing.Size = new Size(98, 20);
			this.nudRssiSmoothing.TabIndex = 3;
			this.nudRssiSmoothing.ThousandsSeparator = true;
			NumericUpDownEx numericUpDownEx22 = this.nudRssiSmoothing;
			int[] bits22 = new int[4];
			bits22[0] = 2;
			Decimal num22 = new Decimal(bits22);
			numericUpDownEx22.Value = num22;
			this.nudRssiSmoothing.ValueChanged += new EventHandler(this.nudRssiSmoothing_ValueChanged);
			this.errorProvider.SetIconPadding((Control)this.nudRssiCollisionThreshold, 30);
			this.nudRssiCollisionThreshold.Location = new Point(125, 42);
			NumericUpDownEx numericUpDownEx23 = this.nudRssiCollisionThreshold;
			int[] bits23 = new int[4];
			bits23[0] = (int)byte.MaxValue;
			Decimal num23 = new Decimal(bits23);
			numericUpDownEx23.Maximum = num23;
			this.nudRssiCollisionThreshold.Name = "nudRssiCollisionThreshold";
			this.nudRssiCollisionThreshold.Size = new Size(98, 20);
			this.nudRssiCollisionThreshold.TabIndex = 3;
			this.nudRssiCollisionThreshold.ThousandsSeparator = true;
			this.nudRssiCollisionThreshold.ValueChanged += new EventHandler(this.nudRssiCollisionThreshold_ValueChanged);
			this.lblRssiValue.BackColor = Color.Transparent;
			this.lblRssiValue.BorderStyle = BorderStyle.Fixed3D;
			this.errorProvider.SetIconPadding((Control)this.lblRssiValue, 30);
			this.lblRssiValue.Location = new Point(125, 97);
			this.lblRssiValue.Margin = new Padding(3);
			this.lblRssiValue.Name = "lblRssiValue";
			this.lblRssiValue.Size = new Size(98, 20);
			this.lblRssiValue.TabIndex = 15;
			this.lblRssiValue.Text = "0";
			this.lblRssiValue.TextAlign = ContentAlignment.MiddleCenter;
			this.nudRssiThresh.DecimalPlaces = 1;
			this.errorProvider.SetIconPadding((Control)this.nudRssiThresh, 30);
			this.nudRssiThresh.Increment = new Decimal(new int[4]
      {
        5,
        0,
        0,
        65536
      });
			this.nudRssiThresh.Location = new Point(125, 71);
			this.nudRssiThresh.Maximum = new Decimal(new int[4]);
			this.nudRssiThresh.Minimum = new Decimal(new int[4]
      {
        1275,
        0,
        0,
        -2147418112
      });
			this.nudRssiThresh.Name = "nudRssiThresh";
			this.nudRssiThresh.Size = new Size(98, 20);
			this.nudRssiThresh.TabIndex = 11;
			this.nudRssiThresh.ThousandsSeparator = true;
			this.nudRssiThresh.Value = new Decimal(new int[4]
      {
        80,
        0,
        0,
        int.MinValue
      });
			this.nudRssiThresh.ValueChanged += new EventHandler(this.nudRssiThresh_ValueChanged);
			this.panel2.Controls.Add((Control)this.gBoxAgc);
			this.panel2.Controls.Add((Control)this.gBoxTimeout);
			this.panel2.Controls.Add((Control)this.gBoxPreamble);
			this.panel2.Controls.Add((Control)this.gBoxRxBw);
			this.panel2.Controls.Add((Control)this.gBoxRxConfig);
			this.panel2.Controls.Add((Control)this.gBoxDemodulator);
			this.panel2.Controls.Add((Control)this.gBoxRssi);
			this.panel2.Controls.Add((Control)this.gBoxAfc);
			this.panel2.Location = new Point(0, 0);
			this.panel2.Margin = new Padding(0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new Size(799, 384);
			this.panel2.TabIndex = 8;
			this.gBoxAgc.Controls.Add((Control)this.label4);
			this.gBoxAgc.Controls.Add((Control)this.label39);
			this.gBoxAgc.Controls.Add((Control)this.label6);
			this.gBoxAgc.Controls.Add((Control)this.label29);
			this.gBoxAgc.Controls.Add((Control)this.label31);
			this.gBoxAgc.Controls.Add((Control)this.label32);
			this.gBoxAgc.Controls.Add((Control)this.label16);
			this.gBoxAgc.Controls.Add((Control)this.label33);
			this.gBoxAgc.Controls.Add((Control)this.label34);
			this.gBoxAgc.Controls.Add((Control)this.label46);
			this.gBoxAgc.Controls.Add((Control)this.btnAgcStart);
			this.gBoxAgc.Controls.Add((Control)this.panel5);
			this.gBoxAgc.Controls.Add((Control)this.label59);
			this.gBoxAgc.Controls.Add((Control)this.label60);
			this.gBoxAgc.Controls.Add((Control)this.label61);
			this.gBoxAgc.Controls.Add((Control)this.label62);
			this.gBoxAgc.Controls.Add((Control)this.nudAgcStep5);
			this.gBoxAgc.Controls.Add((Control)this.nudAgcStep4);
			this.gBoxAgc.Controls.Add((Control)this.nudAgcReferenceLevel);
			this.gBoxAgc.Controls.Add((Control)this.nudAgcStep3);
			this.gBoxAgc.Controls.Add((Control)this.nudAgcStep1);
			this.gBoxAgc.Controls.Add((Control)this.nudAgcStep2);
			this.gBoxAgc.Location = new Point(3, 74);
			this.gBoxAgc.Name = "gBoxAgc";
			this.gBoxAgc.Size = new Size(260, 217);
			this.gBoxAgc.TabIndex = 7;
			this.gBoxAgc.TabStop = false;
			this.gBoxAgc.Text = "AGC";
			this.label4.AutoSize = true;
			this.label4.BackColor = Color.Transparent;
			this.label4.Location = new Point(3, 69);
			this.label4.Name = "label4";
			this.label4.Size = new Size(89, 13);
			this.label4.TabIndex = 2;
			this.label4.Text = "Reference Level:";
			this.label39.AutoSize = true;
			this.label39.BackColor = Color.Transparent;
			this.label39.Location = new Point(3, 18);
			this.label39.Name = "label39";
			this.label39.Size = new Size(32, 13);
			this.label39.TabIndex = 0;
			this.label39.Text = "AGC:";
			this.label6.AutoSize = true;
			this.label6.BackColor = Color.Transparent;
			this.label6.Location = new Point(3, 94);
			this.label6.Name = "label6";
			this.label6.Size = new Size(89, 13);
			this.label6.TabIndex = 8;
			this.label6.Text = "Threshold step 1:";
			this.label29.AutoSize = true;
			this.label29.BackColor = Color.Transparent;
			this.label29.Location = new Point(3, 120);
			this.label29.Name = "label29";
			this.label29.Size = new Size(89, 13);
			this.label29.TabIndex = 11;
			this.label29.Text = "Threshold step 2:";
			this.label31.AutoSize = true;
			this.label31.BackColor = Color.Transparent;
			this.label31.Location = new Point(3, 146);
			this.label31.Name = "label31";
			this.label31.Size = new Size(89, 13);
			this.label31.TabIndex = 14;
			this.label31.Text = "Threshold step 3:";
			this.label32.AutoSize = true;
			this.label32.BackColor = Color.Transparent;
			this.label32.Location = new Point(3, 172);
			this.label32.Name = "label32";
			this.label32.Size = new Size(89, 13);
			this.label32.TabIndex = 17;
			this.label32.Text = "Threshold step 4:";
			this.label16.AutoSize = true;
			this.label16.Location = new Point(3, 44);
			this.label16.Name = "label16";
			this.label16.Size = new Size(56, 13);
			this.label16.TabIndex = 8;
			this.label16.Text = "AGC auto:";
			this.label33.AutoSize = true;
			this.label33.BackColor = Color.Transparent;
			this.label33.Location = new Point(3, 198);
			this.label33.Name = "label33";
			this.label33.Size = new Size(89, 13);
			this.label33.TabIndex = 20;
			this.label33.Text = "Threshold step 5:";
			this.label34.AutoSize = true;
			this.label34.BackColor = Color.Transparent;
			this.label34.Location = new Point(224, 69);
			this.label34.Name = "label34";
			this.label34.Size = new Size(20, 13);
			this.label34.TabIndex = 4;
			this.label34.Text = "dB";
			this.label46.AutoSize = true;
			this.label46.BackColor = Color.Transparent;
			this.label46.Location = new Point(224, 94);
			this.label46.Name = "label46";
			this.label46.Size = new Size(20, 13);
			this.label46.TabIndex = 10;
			this.label46.Text = "dB";
			this.btnAgcStart.Location = new Point(120, 13);
			this.btnAgcStart.Name = "btnAgcStart";
			this.btnAgcStart.Size = new Size(98, 23);
			this.btnAgcStart.TabIndex = 19;
			this.btnAgcStart.Text = "Start";
			this.btnAgcStart.UseVisualStyleBackColor = true;
			this.btnAgcStart.Click += new EventHandler(this.btnAgcStart_Click);
			this.panel5.AutoSize = true;
			this.panel5.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel5.Controls.Add((Control)this.rBtnAgcAutoOff);
			this.panel5.Controls.Add((Control)this.rBtnAgcAutoOn);
			this.panel5.Location = new Point(120, 42);
			this.panel5.Name = "panel5";
			this.panel5.Size = new Size(98, 17);
			this.panel5.TabIndex = 21;
			this.rBtnAgcAutoOff.AutoSize = true;
			this.rBtnAgcAutoOff.Location = new Point(50, 0);
			this.rBtnAgcAutoOff.Margin = new Padding(3, 0, 3, 0);
			this.rBtnAgcAutoOff.Name = "rBtnAgcAutoOff";
			this.rBtnAgcAutoOff.Size = new Size(45, 17);
			this.rBtnAgcAutoOff.TabIndex = 1;
			this.rBtnAgcAutoOff.Text = "OFF";
			this.rBtnAgcAutoOff.UseVisualStyleBackColor = true;
			this.rBtnAgcAutoOff.CheckedChanged += new EventHandler(this.rBtnAgcAutoOn_CheckedChanged);
			this.rBtnAgcAutoOn.AutoSize = true;
			this.rBtnAgcAutoOn.Checked = true;
			this.rBtnAgcAutoOn.Location = new Point(3, 0);
			this.rBtnAgcAutoOn.Margin = new Padding(3, 0, 3, 0);
			this.rBtnAgcAutoOn.Name = "rBtnAgcAutoOn";
			this.rBtnAgcAutoOn.Size = new Size(41, 17);
			this.rBtnAgcAutoOn.TabIndex = 0;
			this.rBtnAgcAutoOn.TabStop = true;
			this.rBtnAgcAutoOn.Text = "ON";
			this.rBtnAgcAutoOn.UseVisualStyleBackColor = true;
			this.rBtnAgcAutoOn.CheckedChanged += new EventHandler(this.rBtnAgcAutoOn_CheckedChanged);
			this.label59.AutoSize = true;
			this.label59.BackColor = Color.Transparent;
			this.label59.Location = new Point(224, 120);
			this.label59.Name = "label59";
			this.label59.Size = new Size(20, 13);
			this.label59.TabIndex = 13;
			this.label59.Text = "dB";
			this.label60.AutoSize = true;
			this.label60.BackColor = Color.Transparent;
			this.label60.Location = new Point(224, 146);
			this.label60.Name = "label60";
			this.label60.Size = new Size(20, 13);
			this.label60.TabIndex = 16;
			this.label60.Text = "dB";
			this.label61.AutoSize = true;
			this.label61.BackColor = Color.Transparent;
			this.label61.Location = new Point(224, 172);
			this.label61.Name = "label61";
			this.label61.Size = new Size(20, 13);
			this.label61.TabIndex = 19;
			this.label61.Text = "dB";
			this.label62.AutoSize = true;
			this.label62.BackColor = Color.Transparent;
			this.label62.Location = new Point(224, 198);
			this.label62.Name = "label62";
			this.label62.Size = new Size(20, 13);
			this.label62.TabIndex = 22;
			this.label62.Text = "dB";
			this.nudAgcStep5.Location = new Point(120, 194);
			NumericUpDown numericUpDown1 = this.nudAgcStep5;
			int[] bits24 = new int[4];
			bits24[0] = 15;
			Decimal num24 = new Decimal(bits24);
			numericUpDown1.Maximum = num24;
			this.nudAgcStep5.Name = "nudAgcStep5";
			this.nudAgcStep5.Size = new Size(98, 20);
			this.nudAgcStep5.TabIndex = 21;
			NumericUpDown numericUpDown2 = this.nudAgcStep5;
			int[] bits25 = new int[4];
			bits25[0] = 11;
			Decimal num25 = new Decimal(bits25);
			numericUpDown2.Value = num25;
			this.nudAgcStep5.ValueChanged += new EventHandler(this.nudAgcStep_ValueChanged);
			this.nudAgcStep4.Location = new Point(120, 168);
			NumericUpDown numericUpDown3 = this.nudAgcStep4;
			int[] bits26 = new int[4];
			bits26[0] = 15;
			Decimal num26 = new Decimal(bits26);
			numericUpDown3.Maximum = num26;
			this.nudAgcStep4.Name = "nudAgcStep4";
			this.nudAgcStep4.Size = new Size(98, 20);
			this.nudAgcStep4.TabIndex = 18;
			NumericUpDown numericUpDown4 = this.nudAgcStep4;
			int[] bits27 = new int[4];
			bits27[0] = 9;
			Decimal num27 = new Decimal(bits27);
			numericUpDown4.Value = num27;
			this.nudAgcStep4.ValueChanged += new EventHandler(this.nudAgcStep_ValueChanged);
			this.nudAgcReferenceLevel.Location = new Point(120, 65);
			NumericUpDownEx numericUpDownEx24 = this.nudAgcReferenceLevel;
			int[] bits28 = new int[4];
			bits28[0] = 63;
			Decimal num28 = new Decimal(bits28);
			numericUpDownEx24.Maximum = num28;
			this.nudAgcReferenceLevel.Name = "nudAgcReferenceLevel";
			this.nudAgcReferenceLevel.Size = new Size(98, 20);
			this.nudAgcReferenceLevel.TabIndex = 3;
			this.nudAgcReferenceLevel.ThousandsSeparator = true;
			NumericUpDownEx numericUpDownEx25 = this.nudAgcReferenceLevel;
			int[] bits29 = new int[4];
			bits29[0] = 19;
			Decimal num29 = new Decimal(bits29);
			numericUpDownEx25.Value = num29;
			this.nudAgcReferenceLevel.ValueChanged += new EventHandler(this.nudAgcReferenceLevel_ValueChanged);
			this.nudAgcStep3.Location = new Point(120, 142);
			NumericUpDown numericUpDown5 = this.nudAgcStep3;
			int[] bits30 = new int[4];
			bits30[0] = 15;
			Decimal num30 = new Decimal(bits30);
			numericUpDown5.Maximum = num30;
			this.nudAgcStep3.Name = "nudAgcStep3";
			this.nudAgcStep3.Size = new Size(98, 20);
			this.nudAgcStep3.TabIndex = 15;
			NumericUpDown numericUpDown6 = this.nudAgcStep3;
			int[] bits31 = new int[4];
			bits31[0] = 11;
			Decimal num31 = new Decimal(bits31);
			numericUpDown6.Value = num31;
			this.nudAgcStep3.ValueChanged += new EventHandler(this.nudAgcStep_ValueChanged);
			this.nudAgcStep1.Location = new Point(120, 90);
			NumericUpDown numericUpDown7 = this.nudAgcStep1;
			int[] bits32 = new int[4];
			bits32[0] = 31;
			Decimal num32 = new Decimal(bits32);
			numericUpDown7.Maximum = num32;
			this.nudAgcStep1.Name = "nudAgcStep1";
			this.nudAgcStep1.Size = new Size(98, 20);
			this.nudAgcStep1.TabIndex = 9;
			NumericUpDown numericUpDown8 = this.nudAgcStep1;
			int[] bits33 = new int[4];
			bits33[0] = 16;
			Decimal num33 = new Decimal(bits33);
			numericUpDown8.Value = num33;
			this.nudAgcStep1.ValueChanged += new EventHandler(this.nudAgcStep_ValueChanged);
			this.nudAgcStep2.Location = new Point(120, 116);
			NumericUpDown numericUpDown9 = this.nudAgcStep2;
			int[] bits34 = new int[4];
			bits34[0] = 15;
			Decimal num34 = new Decimal(bits34);
			numericUpDown9.Maximum = num34;
			this.nudAgcStep2.Name = "nudAgcStep2";
			this.nudAgcStep2.Size = new Size(98, 20);
			this.nudAgcStep2.TabIndex = 12;
			NumericUpDown numericUpDown10 = this.nudAgcStep2;
			int[] bits35 = new int[4];
			bits35[0] = 7;
			Decimal num35 = new Decimal(bits35);
			numericUpDown10.Value = num35;
			this.nudAgcStep2.ValueChanged += new EventHandler(this.nudAgcStep_ValueChanged);
			this.gBoxTimeout.Controls.Add((Control)this.nudTimeoutRssi);
			this.gBoxTimeout.Controls.Add((Control)this.nudAutoRxRestartDelay);
			this.gBoxTimeout.Controls.Add((Control)this.nudTimeoutSyncWord);
			this.gBoxTimeout.Controls.Add((Control)this.label35);
			this.gBoxTimeout.Controls.Add((Control)this.nudTimeoutPreamble);
			this.gBoxTimeout.Controls.Add((Control)this.label15);
			this.gBoxTimeout.Controls.Add((Control)this.label37);
			this.gBoxTimeout.Controls.Add((Control)this.label27);
			this.gBoxTimeout.Controls.Add((Control)this.label11);
			this.gBoxTimeout.Controls.Add((Control)this.label36);
			this.gBoxTimeout.Controls.Add((Control)this.label14);
			this.gBoxTimeout.Controls.Add((Control)this.label9);
			this.gBoxTimeout.Location = new Point(535, 244);
			this.gBoxTimeout.Name = "gBoxTimeout";
			this.gBoxTimeout.Size = new Size(261, 121);
			this.gBoxTimeout.TabIndex = 4;
			this.gBoxTimeout.TabStop = false;
			this.gBoxTimeout.Text = "Timeout";
			this.gBoxTimeout.MouseEnter += new EventHandler(this.control_MouseEnter);
			this.gBoxTimeout.MouseLeave += new EventHandler(this.control_MouseLeave);
			this.label35.AutoSize = true;
			this.label35.BackColor = Color.Transparent;
			this.label35.Location = new Point(229, 75);
			this.label35.Name = "label35";
			this.label35.Size = new Size(20, 13);
			this.label35.TabIndex = 7;
			this.label35.Text = "ms";
			this.label15.AutoSize = true;
			this.label15.BackColor = Color.Transparent;
			this.label15.Location = new Point(229, 49);
			this.label15.Name = "label15";
			this.label15.Size = new Size(20, 13);
			this.label15.TabIndex = 7;
			this.label15.Text = "ms";
			this.label37.AutoSize = true;
			this.label37.BackColor = Color.Transparent;
			this.label37.Location = new Point(229, 101);
			this.label37.Name = "label37";
			this.label37.Size = new Size(20, 13);
			this.label37.TabIndex = 4;
			this.label37.Text = "ms";
			this.label27.AutoSize = true;
			this.label27.Location = new Point(7, 75);
			this.label27.Name = "label27";
			this.label27.Size = new Size(64, 13);
			this.label27.TabIndex = 5;
			this.label27.Text = "Signal sync:";
			this.label11.AutoSize = true;
			this.label11.BackColor = Color.Transparent;
			this.label11.Location = new Point(229, 23);
			this.label11.Name = "label11";
			this.label11.Size = new Size(20, 13);
			this.label11.TabIndex = 4;
			this.label11.Text = "ms";
			this.label36.AutoSize = true;
			this.label36.Location = new Point(7, 101);
			this.label36.Name = "label36";
			this.label36.Size = new Size(111, 13);
			this.label36.TabIndex = 2;
			this.label36.Text = "Inter packet Rx delay:";
			this.label14.AutoSize = true;
			this.label14.Location = new Point(7, 49);
			this.label14.Name = "label14";
			this.label14.Size = new Size(54, 13);
			this.label14.TabIndex = 5;
			this.label14.Text = "Preamble:";
			this.label9.AutoSize = true;
			this.label9.Location = new Point(7, 23);
			this.label9.Name = "label9";
			this.label9.Size = new Size(35, 13);
			this.label9.TabIndex = 2;
			this.label9.Text = "RSSI:";
			this.gBoxPreamble.Controls.Add((Control)this.nudPreambleDetectorTol);
			this.gBoxPreamble.Controls.Add((Control)this.label41);
			this.gBoxPreamble.Controls.Add((Control)this.nudPreambleDetectorSize);
			this.gBoxPreamble.Controls.Add((Control)this.label42);
			this.gBoxPreamble.Controls.Add((Control)this.label44);
			this.gBoxPreamble.Controls.Add((Control)this.panel1);
			this.gBoxPreamble.Controls.Add((Control)this.label57);
			this.gBoxPreamble.Controls.Add((Control)this.label58);
			this.gBoxPreamble.Location = new Point(3, 294);
			this.gBoxPreamble.Name = "gBoxPreamble";
			this.gBoxPreamble.Size = new Size(260, 88);
			this.gBoxPreamble.TabIndex = 4;
			this.gBoxPreamble.TabStop = false;
			this.gBoxPreamble.Text = "Preamble detetction";
			this.gBoxPreamble.MouseEnter += new EventHandler(this.control_MouseEnter);
			this.gBoxPreamble.MouseLeave += new EventHandler(this.control_MouseLeave);
			this.label41.AutoSize = true;
			this.label41.BackColor = Color.Transparent;
			this.label41.Location = new Point(224, 69);
			this.label41.Name = "label41";
			this.label41.Size = new Size(27, 13);
			this.label41.TabIndex = 7;
			this.label41.Text = "chip";
			this.label42.AutoSize = true;
			this.label42.BackColor = Color.Transparent;
			this.label42.Location = new Point(224, 44);
			this.label42.Name = "label42";
			this.label42.Size = new Size(27, 13);
			this.label42.TabIndex = 7;
			this.label42.Text = "byte";
			this.label44.AutoSize = true;
			this.label44.Location = new Point(7, 69);
			this.label44.Name = "label44";
			this.label44.Size = new Size(79, 13);
			this.label44.TabIndex = 5;
			this.label44.Text = "Error tolerance:";
			this.panel1.AutoSize = true;
			this.panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel1.Controls.Add((Control)this.rBtnPreambleDetectorOff);
			this.panel1.Controls.Add((Control)this.rBtnPreambleDetectorOn);
			this.panel1.Location = new Point(120, 18);
			this.panel1.Name = "panel1";
			this.panel1.Size = new Size(98, 17);
			this.panel1.TabIndex = 6;
			this.rBtnPreambleDetectorOff.AutoSize = true;
			this.rBtnPreambleDetectorOff.Location = new Point(50, 0);
			this.rBtnPreambleDetectorOff.Margin = new Padding(3, 0, 3, 0);
			this.rBtnPreambleDetectorOff.Name = "rBtnPreambleDetectorOff";
			this.rBtnPreambleDetectorOff.Size = new Size(45, 17);
			this.rBtnPreambleDetectorOff.TabIndex = 1;
			this.rBtnPreambleDetectorOff.Text = "OFF";
			this.rBtnPreambleDetectorOff.UseVisualStyleBackColor = true;
			this.rBtnPreambleDetectorOff.CheckedChanged += new EventHandler(this.rBtnPreambleDetectorOn_CheckedChanged);
			this.rBtnPreambleDetectorOn.AutoSize = true;
			this.rBtnPreambleDetectorOn.Checked = true;
			this.rBtnPreambleDetectorOn.Location = new Point(3, 0);
			this.rBtnPreambleDetectorOn.Margin = new Padding(3, 0, 3, 0);
			this.rBtnPreambleDetectorOn.Name = "rBtnPreambleDetectorOn";
			this.rBtnPreambleDetectorOn.Size = new Size(41, 17);
			this.rBtnPreambleDetectorOn.TabIndex = 0;
			this.rBtnPreambleDetectorOn.TabStop = true;
			this.rBtnPreambleDetectorOn.Text = "ON";
			this.rBtnPreambleDetectorOn.UseVisualStyleBackColor = true;
			this.rBtnPreambleDetectorOn.CheckedChanged += new EventHandler(this.rBtnPreambleDetectorOn_CheckedChanged);
			this.label57.AutoSize = true;
			this.label57.Location = new Point(7, 44);
			this.label57.Name = "label57";
			this.label57.Size = new Size(30, 13);
			this.label57.TabIndex = 5;
			this.label57.Text = "Size:";
			this.label58.AutoSize = true;
			this.label58.Location = new Point(7, 20);
			this.label58.Name = "label58";
			this.label58.Size = new Size(56, 13);
			this.label58.TabIndex = 2;
			this.label58.Text = "Detection:";
			this.gBoxRxBw.Controls.Add((Control)this.lblAfcRxBw);
			this.gBoxRxBw.Controls.Add((Control)this.lblRxBw);
			this.gBoxRxBw.Controls.Add((Control)this.suffixAFCRxBw);
			this.gBoxRxBw.Controls.Add((Control)this.nudRxFilterBwAfc);
			this.gBoxRxBw.Controls.Add((Control)this.suffixRxBw);
			this.gBoxRxBw.Controls.Add((Control)this.nudRxFilterBw);
			this.gBoxRxBw.Location = new Point(3, 2);
			this.gBoxRxBw.Name = "gBoxRxBw";
			this.gBoxRxBw.Size = new Size(260, 66);
			this.gBoxRxBw.TabIndex = 0;
			this.gBoxRxBw.TabStop = false;
			this.gBoxRxBw.Text = "Bandwidth";
			this.gBoxRxBw.MouseEnter += new EventHandler(this.control_MouseEnter);
			this.gBoxRxBw.MouseLeave += new EventHandler(this.control_MouseLeave);
			this.lblAfcRxBw.AutoSize = true;
			this.lblAfcRxBw.Location = new Point(3, 44);
			this.lblAfcRxBw.Name = "lblAfcRxBw";
			this.lblAfcRxBw.Size = new Size(104, 13);
			this.lblAfcRxBw.TabIndex = 3;
			this.lblAfcRxBw.Text = "AFC filter bandwidth:";
			this.lblRxBw.AutoSize = true;
			this.lblRxBw.Location = new Point(3, 18);
			this.lblRxBw.Name = "lblRxBw";
			this.lblRxBw.Size = new Size(97, 13);
			this.lblRxBw.TabIndex = 3;
			this.lblRxBw.Text = "Rx filter bandwidth:";
			this.suffixAFCRxBw.AutoSize = true;
			this.suffixAFCRxBw.Location = new Point(224, 46);
			this.suffixAFCRxBw.Name = "suffixAFCRxBw";
			this.suffixAFCRxBw.Size = new Size(20, 13);
			this.suffixAFCRxBw.TabIndex = 5;
			this.suffixAFCRxBw.Text = "Hz";
			this.suffixRxBw.AutoSize = true;
			this.suffixRxBw.Location = new Point(224, 21);
			this.suffixRxBw.Name = "suffixRxBw";
			this.suffixRxBw.Size = new Size(20, 13);
			this.suffixRxBw.TabIndex = 5;
			this.suffixRxBw.Text = "Hz";
			this.gBoxRxConfig.Controls.Add((Control)this.cBoxRxTrigger);
			this.gBoxRxConfig.Controls.Add((Control)this.label38);
			this.gBoxRxConfig.Controls.Add((Control)this.label5);
			this.gBoxRxConfig.Controls.Add((Control)this.btnRestartRxWithPllLock);
			this.gBoxRxConfig.Controls.Add((Control)this.label3);
			this.gBoxRxConfig.Controls.Add((Control)this.btnRestartRxWithoutPllLock);
			this.gBoxRxConfig.Controls.Add((Control)this.panel4);
			this.gBoxRxConfig.Controls.Add((Control)this.nudRssiCollisionThreshold);
			this.gBoxRxConfig.Controls.Add((Control)this.label26);
			this.gBoxRxConfig.Location = new Point(269, 258);
			this.gBoxRxConfig.Name = "gBoxRxConfig";
			this.gBoxRxConfig.Size = new Size(261, 124);
			this.gBoxRxConfig.TabIndex = 4;
			this.gBoxRxConfig.TabStop = false;
			this.gBoxRxConfig.Text = "Rx startup control";
			this.gBoxRxConfig.MouseEnter += new EventHandler(this.control_MouseEnter);
			this.gBoxRxConfig.MouseLeave += new EventHandler(this.control_MouseLeave);
			this.label38.AutoSize = true;
			this.label38.BackColor = Color.Transparent;
			this.label38.Location = new Point(229, 46);
			this.label38.Name = "label38";
			this.label38.Size = new Size(20, 13);
			this.label38.TabIndex = 12;
			this.label38.Text = "dB";
			this.label38.TextAlign = ContentAlignment.MiddleCenter;
			this.label5.AutoSize = true;
			this.label5.Location = new Point(3, 21);
			this.label5.Name = "label5";
			this.label5.Size = new Size(115, 13);
			this.label5.TabIndex = 5;
			this.label5.Text = "Restart Rx on collision:";
			this.btnRestartRxWithPllLock.Location = new Point(133, 68);
			this.btnRestartRxWithPllLock.Name = "btnRestartRxWithPllLock";
			this.btnRestartRxWithPllLock.Size = new Size(89, 23);
			this.btnRestartRxWithPllLock.TabIndex = 19;
			this.btnRestartRxWithPllLock.Text = "Rx Restart PLL";
			this.btnRestartRxWithPllLock.UseVisualStyleBackColor = true;
			this.btnRestartRxWithPllLock.Click += new EventHandler(this.btnRestartRxWithPllLock_Click);
			this.label3.AutoSize = true;
			this.label3.Location = new Point(3, 101);
			this.label3.Name = "label3";
			this.label3.Size = new Size(55, 13);
			this.label3.TabIndex = 8;
			this.label3.Text = "Rx trigger:";
			this.btnRestartRxWithoutPllLock.Location = new Point(38, 68);
			this.btnRestartRxWithoutPllLock.Name = "btnRestartRxWithoutPllLock";
			this.btnRestartRxWithoutPllLock.Size = new Size(89, 23);
			this.btnRestartRxWithoutPllLock.TabIndex = 19;
			this.btnRestartRxWithoutPllLock.Text = " Rx Restart";
			this.btnRestartRxWithoutPllLock.UseVisualStyleBackColor = true;
			this.btnRestartRxWithoutPllLock.Click += new EventHandler(this.btnRestartRxWithoutPllLock_Click);
			this.panel4.AutoSize = true;
			this.panel4.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel4.Controls.Add((Control)this.rBtnRestartRxOnCollisionOff);
			this.panel4.Controls.Add((Control)this.rBtnRestartRxOnCollisionOn);
			this.panel4.Location = new Point(125, 19);
			this.panel4.Name = "panel4";
			this.panel4.Size = new Size(98, 17);
			this.panel4.TabIndex = 6;
			this.rBtnRestartRxOnCollisionOff.AutoSize = true;
			this.rBtnRestartRxOnCollisionOff.Location = new Point(50, 0);
			this.rBtnRestartRxOnCollisionOff.Margin = new Padding(3, 0, 3, 0);
			this.rBtnRestartRxOnCollisionOff.Name = "rBtnRestartRxOnCollisionOff";
			this.rBtnRestartRxOnCollisionOff.Size = new Size(45, 17);
			this.rBtnRestartRxOnCollisionOff.TabIndex = 1;
			this.rBtnRestartRxOnCollisionOff.Text = "OFF";
			this.rBtnRestartRxOnCollisionOff.UseVisualStyleBackColor = true;
			this.rBtnRestartRxOnCollisionOff.CheckedChanged += new EventHandler(this.rBtnRestartRxOnCollisionOn_CheckedChanged);
			this.rBtnRestartRxOnCollisionOn.AutoSize = true;
			this.rBtnRestartRxOnCollisionOn.Checked = true;
			this.rBtnRestartRxOnCollisionOn.Location = new Point(3, 0);
			this.rBtnRestartRxOnCollisionOn.Margin = new Padding(3, 0, 3, 0);
			this.rBtnRestartRxOnCollisionOn.Name = "rBtnRestartRxOnCollisionOn";
			this.rBtnRestartRxOnCollisionOn.Size = new Size(41, 17);
			this.rBtnRestartRxOnCollisionOn.TabIndex = 0;
			this.rBtnRestartRxOnCollisionOn.TabStop = true;
			this.rBtnRestartRxOnCollisionOn.Text = "ON";
			this.rBtnRestartRxOnCollisionOn.UseVisualStyleBackColor = true;
			this.rBtnRestartRxOnCollisionOn.CheckedChanged += new EventHandler(this.rBtnRestartRxOnCollisionOn_CheckedChanged);
			this.label26.AutoSize = true;
			this.label26.Location = new Point(3, 46);
			this.label26.Name = "label26";
			this.label26.Size = new Size(94, 13);
			this.label26.TabIndex = 2;
			this.label26.Text = "Collision threshold:";
			this.gBoxDemodulator.Controls.Add((Control)this.pnlHorizontalSeparator);
			this.gBoxDemodulator.Controls.Add((Control)this.nudOokAverageOffset);
			this.gBoxDemodulator.Controls.Add((Control)this.label30);
			this.gBoxDemodulator.Controls.Add((Control)this.cBoxOokThreshType);
			this.gBoxDemodulator.Controls.Add((Control)this.label1);
			this.gBoxDemodulator.Controls.Add((Control)this.lblOokType);
			this.gBoxDemodulator.Controls.Add((Control)this.lblOokStep);
			this.gBoxDemodulator.Controls.Add((Control)this.label28);
			this.gBoxDemodulator.Controls.Add((Control)this.lblOokDec);
			this.gBoxDemodulator.Controls.Add((Control)this.lblOokCutoff);
			this.gBoxDemodulator.Controls.Add((Control)this.lblOokFixed);
			this.gBoxDemodulator.Controls.Add((Control)this.suffixOOKstep);
			this.gBoxDemodulator.Controls.Add((Control)this.label40);
			this.gBoxDemodulator.Controls.Add((Control)this.suffixOOKfixed);
			this.gBoxDemodulator.Controls.Add((Control)this.nudOokPeakThreshStep);
			this.gBoxDemodulator.Controls.Add((Control)this.nudOokFixedThresh);
			this.gBoxDemodulator.Controls.Add((Control)this.panel13);
			this.gBoxDemodulator.Controls.Add((Control)this.cBoxOokPeakThreshDec);
			this.gBoxDemodulator.Controls.Add((Control)this.cBoxOokAverageThreshFilt);
			this.gBoxDemodulator.Location = new Point(535, 20);
			this.gBoxDemodulator.Name = "gBoxDemodulator";
			this.gBoxDemodulator.Size = new Size(261, 218);
			this.gBoxDemodulator.TabIndex = 2;
			this.gBoxDemodulator.TabStop = false;
			this.gBoxDemodulator.Text = "Demodulator";
			this.gBoxDemodulator.MouseEnter += new EventHandler(this.control_MouseEnter);
			this.gBoxDemodulator.MouseLeave += new EventHandler(this.control_MouseLeave);
			this.pnlHorizontalSeparator.BorderStyle = BorderStyle.FixedSingle;
			this.pnlHorizontalSeparator.Location = new Point(49, 47);
			this.pnlHorizontalSeparator.Name = "pnlHorizontalSeparator";
			this.pnlHorizontalSeparator.Size = new Size(206, 1);
			this.pnlHorizontalSeparator.TabIndex = 12;
			this.label30.AutoSize = true;
			this.label30.Location = new Point(6, 192);
			this.label30.Name = "label30";
			this.label30.Size = new Size(58, 13);
			this.label30.TabIndex = 2;
			this.label30.Text = "Avg offset:";
			this.label1.AutoSize = true;
			this.label1.Location = new Point(6, 41);
			this.label1.Name = "label1";
			this.label1.Size = new Size(30, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "OOK";
			this.lblOokType.AutoSize = true;
			this.lblOokType.Location = new Point(6, 61);
			this.lblOokType.Name = "lblOokType";
			this.lblOokType.Size = new Size(80, 13);
			this.lblOokType.TabIndex = 0;
			this.lblOokType.Text = "Threshold type:";
			this.lblOokStep.AutoSize = true;
			this.lblOokStep.Location = new Point(6, 87);
			this.lblOokStep.Name = "lblOokStep";
			this.lblOokStep.Size = new Size(104, 13);
			this.lblOokStep.TabIndex = 2;
			this.lblOokStep.Text = "Peak threshold step:";
			this.label28.AutoSize = true;
			this.label28.Location = new Point(6, 21);
			this.label28.Name = "label28";
			this.label28.Size = new Size(84, 13);
			this.label28.TabIndex = 8;
			this.label28.Text = "Bit synchronizer:";
			this.lblOokDec.AutoSize = true;
			this.lblOokDec.Location = new Point(6, 139);
			this.lblOokDec.Name = "lblOokDec";
			this.lblOokDec.Size = new Size(108, 13);
			this.lblOokDec.TabIndex = 5;
			this.lblOokDec.Text = "Peak threshold decr.:";
			this.lblOokCutoff.AutoSize = true;
			this.lblOokCutoff.Location = new Point(6, 165);
			this.lblOokCutoff.Name = "lblOokCutoff";
			this.lblOokCutoff.Size = new Size(105, 13);
			this.lblOokCutoff.TabIndex = 7;
			this.lblOokCutoff.Text = "Avg threshold cutoff:";
			this.lblOokFixed.AutoSize = true;
			this.lblOokFixed.Location = new Point(6, 113);
			this.lblOokFixed.Name = "lblOokFixed";
			this.lblOokFixed.Size = new Size(81, 13);
			this.lblOokFixed.TabIndex = 9;
			this.lblOokFixed.Text = "Fixed threshold:";
			this.suffixOOKstep.AutoSize = true;
			this.suffixOOKstep.BackColor = Color.Transparent;
			this.suffixOOKstep.Location = new Point(229, 87);
			this.suffixOOKstep.Name = "suffixOOKstep";
			this.suffixOOKstep.Size = new Size(20, 13);
			this.suffixOOKstep.TabIndex = 4;
			this.suffixOOKstep.Text = "dB";
			this.label40.AutoSize = true;
			this.label40.BackColor = Color.Transparent;
			this.label40.Location = new Point(229, 192);
			this.label40.Name = "label40";
			this.label40.Size = new Size(20, 13);
			this.label40.TabIndex = 11;
			this.label40.Text = "dB";
			this.suffixOOKfixed.AutoSize = true;
			this.suffixOOKfixed.BackColor = Color.Transparent;
			this.suffixOOKfixed.Location = new Point(229, 122);
			this.suffixOOKfixed.Name = "suffixOOKfixed";
			this.suffixOOKfixed.Size = new Size(20, 13);
			this.suffixOOKfixed.TabIndex = 11;
			this.suffixOOKfixed.Text = "dB";
			this.panel13.AutoSize = true;
			this.panel13.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel13.Controls.Add((Control)this.rBtnBitSyncOff);
			this.panel13.Controls.Add((Control)this.rBtnBitSyncOn);
			this.panel13.Location = new Point(125, 19);
			this.panel13.Name = "panel13";
			this.panel13.Size = new Size(98, 17);
			this.panel13.TabIndex = 7;
			this.rBtnBitSyncOff.AutoSize = true;
			this.rBtnBitSyncOff.Location = new Point(50, 0);
			this.rBtnBitSyncOff.Margin = new Padding(3, 0, 3, 0);
			this.rBtnBitSyncOff.Name = "rBtnBitSyncOff";
			this.rBtnBitSyncOff.Size = new Size(45, 17);
			this.rBtnBitSyncOff.TabIndex = 1;
			this.rBtnBitSyncOff.Text = "OFF";
			this.rBtnBitSyncOff.UseVisualStyleBackColor = true;
			this.rBtnBitSyncOff.CheckedChanged += new EventHandler(this.rBtnBitSyncOn_CheckedChanged);
			this.rBtnBitSyncOn.AutoSize = true;
			this.rBtnBitSyncOn.Checked = true;
			this.rBtnBitSyncOn.Location = new Point(3, 0);
			this.rBtnBitSyncOn.Margin = new Padding(3, 0, 3, 0);
			this.rBtnBitSyncOn.Name = "rBtnBitSyncOn";
			this.rBtnBitSyncOn.Size = new Size(41, 17);
			this.rBtnBitSyncOn.TabIndex = 0;
			this.rBtnBitSyncOn.TabStop = true;
			this.rBtnBitSyncOn.Text = "ON";
			this.rBtnBitSyncOn.UseVisualStyleBackColor = true;
			this.rBtnBitSyncOn.CheckedChanged += new EventHandler(this.rBtnBitSyncOn_CheckedChanged);
			this.gBoxRssi.Controls.Add((Control)this.label23);
			this.gBoxRssi.Controls.Add((Control)this.label17);
			this.gBoxRssi.Controls.Add((Control)this.label54);
			this.gBoxRssi.Controls.Add((Control)this.nudRssiOffset);
			this.gBoxRssi.Controls.Add((Control)this.label24);
			this.gBoxRssi.Controls.Add((Control)this.label55);
			this.gBoxRssi.Controls.Add((Control)this.label25);
			this.gBoxRssi.Controls.Add((Control)this.nudRssiSmoothing);
			this.gBoxRssi.Controls.Add((Control)this.label56);
			this.gBoxRssi.Controls.Add((Control)this.lblRssiValue);
			this.gBoxRssi.Controls.Add((Control)this.nudRssiThresh);
			this.gBoxRssi.Location = new Point(269, (int)sbyte.MaxValue);
			this.gBoxRssi.Name = "gBoxRssi";
			this.gBoxRssi.Size = new Size(261, 125);
			this.gBoxRssi.TabIndex = 4;
			this.gBoxRssi.TabStop = false;
			this.gBoxRssi.Text = "RSSI";
			this.gBoxRssi.MouseEnter += new EventHandler(this.control_MouseEnter);
			this.gBoxRssi.MouseLeave += new EventHandler(this.control_MouseLeave);
			this.label23.AutoSize = true;
			this.label23.BackColor = Color.Transparent;
			this.label23.Location = new Point(229, 23);
			this.label23.Name = "label23";
			this.label23.Size = new Size(20, 13);
			this.label23.TabIndex = 12;
			this.label23.Text = "dB";
			this.label23.TextAlign = ContentAlignment.MiddleCenter;
			this.label17.AutoSize = true;
			this.label17.BackColor = Color.Transparent;
			this.label17.Location = new Point(229, 75);
			this.label17.Name = "label17";
			this.label17.Size = new Size(28, 13);
			this.label17.TabIndex = 12;
			this.label17.Text = "dBm";
			this.label17.TextAlign = ContentAlignment.MiddleCenter;
			this.label54.AutoSize = true;
			this.label54.BackColor = Color.Transparent;
			this.label54.Location = new Point(229, 101);
			this.label54.Name = "label54";
			this.label54.Size = new Size(28, 13);
			this.label54.TabIndex = 17;
			this.label54.Text = "dBm";
			this.label54.TextAlign = ContentAlignment.MiddleCenter;
			this.label24.AutoSize = true;
			this.label24.Location = new Point(3, 23);
			this.label24.Name = "label24";
			this.label24.Size = new Size(38, 13);
			this.label24.TabIndex = 2;
			this.label24.Text = "Offset:";
			this.label55.AutoSize = true;
			this.label55.BackColor = Color.Transparent;
			this.label55.Location = new Point(3, 75);
			this.label55.Margin = new Padding(0);
			this.label55.Name = "label55";
			this.label55.Size = new Size(57, 13);
			this.label55.TabIndex = 10;
			this.label55.Text = "Threshold:";
			this.label55.TextAlign = ContentAlignment.MiddleCenter;
			this.label25.AutoSize = true;
			this.label25.Location = new Point(3, 49);
			this.label25.Name = "label25";
			this.label25.Size = new Size(60, 13);
			this.label25.TabIndex = 2;
			this.label25.Text = "Smoothing:";
			this.label56.AutoSize = true;
			this.label56.BackColor = Color.Transparent;
			this.label56.Location = new Point(3, 101);
			this.label56.Margin = new Padding(0);
			this.label56.Name = "label56";
			this.label56.Size = new Size(37, 13);
			this.label56.TabIndex = 13;
			this.label56.Text = "Value:";
			this.label56.TextAlign = ContentAlignment.MiddleCenter;
			this.gBoxAfc.Controls.Add((Control)this.label19);
			this.gBoxAfc.Controls.Add((Control)this.btnFeiRead);
			this.gBoxAfc.Controls.Add((Control)this.panel8);
			this.gBoxAfc.Controls.Add((Control)this.lblFeiValue);
			this.gBoxAfc.Controls.Add((Control)this.label12);
			this.gBoxAfc.Controls.Add((Control)this.label20);
			this.gBoxAfc.Controls.Add((Control)this.label18);
			this.gBoxAfc.Controls.Add((Control)this.label10);
			this.gBoxAfc.Controls.Add((Control)this.btnAfcClear);
			this.gBoxAfc.Controls.Add((Control)this.lblAfcValue);
			this.gBoxAfc.Controls.Add((Control)this.label22);
			this.gBoxAfc.Controls.Add((Control)this.panel9);
			this.gBoxAfc.Location = new Point(269, 3);
			this.gBoxAfc.Name = "gBoxAfc";
			this.gBoxAfc.Size = new Size(261, 118);
			this.gBoxAfc.TabIndex = 3;
			this.gBoxAfc.TabStop = false;
			this.gBoxAfc.Text = "AFC";
			this.gBoxAfc.MouseEnter += new EventHandler(this.control_MouseEnter);
			this.gBoxAfc.MouseLeave += new EventHandler(this.control_MouseLeave);
			this.label19.AutoSize = true;
			this.label19.BackColor = Color.Transparent;
			this.label19.Location = new Point(3, 44);
			this.label19.Name = "label19";
			this.label19.Size = new Size(80, 13);
			this.label19.TabIndex = 5;
			this.label19.Text = "AFC auto clear:";
			this.btnFeiRead.Location = new Point(78, 90);
			this.btnFeiRead.Name = "btnFeiRead";
			this.btnFeiRead.Size = new Size(41, 23);
			this.btnFeiRead.TabIndex = 16;
			this.btnFeiRead.Text = "Read";
			this.btnFeiRead.UseVisualStyleBackColor = true;
			this.btnFeiRead.Click += new EventHandler(this.btnFeiMeasure_Click);
			this.panel8.AutoSize = true;
			this.panel8.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel8.Controls.Add((Control)this.rBtnAfcAutoClearOff);
			this.panel8.Controls.Add((Control)this.rBtnAfcAutoClearOn);
			this.panel8.Location = new Point(125, 42);
			this.panel8.Name = "panel8";
			this.panel8.Size = new Size(98, 17);
			this.panel8.TabIndex = 6;
			this.rBtnAfcAutoClearOff.AutoSize = true;
			this.rBtnAfcAutoClearOff.Location = new Point(50, 0);
			this.rBtnAfcAutoClearOff.Margin = new Padding(3, 0, 3, 0);
			this.rBtnAfcAutoClearOff.Name = "rBtnAfcAutoClearOff";
			this.rBtnAfcAutoClearOff.Size = new Size(45, 17);
			this.rBtnAfcAutoClearOff.TabIndex = 1;
			this.rBtnAfcAutoClearOff.Text = "OFF";
			this.rBtnAfcAutoClearOff.UseVisualStyleBackColor = true;
			this.rBtnAfcAutoClearOff.CheckedChanged += new EventHandler(this.rBtnAfcAutoClearOn_CheckedChanged);
			this.rBtnAfcAutoClearOn.AutoSize = true;
			this.rBtnAfcAutoClearOn.Checked = true;
			this.rBtnAfcAutoClearOn.Location = new Point(3, 0);
			this.rBtnAfcAutoClearOn.Margin = new Padding(3, 0, 3, 0);
			this.rBtnAfcAutoClearOn.Name = "rBtnAfcAutoClearOn";
			this.rBtnAfcAutoClearOn.Size = new Size(41, 17);
			this.rBtnAfcAutoClearOn.TabIndex = 0;
			this.rBtnAfcAutoClearOn.TabStop = true;
			this.rBtnAfcAutoClearOn.Text = "ON";
			this.rBtnAfcAutoClearOn.UseVisualStyleBackColor = true;
			this.rBtnAfcAutoClearOn.CheckedChanged += new EventHandler(this.rBtnAfcAutoClearOn_CheckedChanged);
			this.lblFeiValue.BackColor = Color.Transparent;
			this.lblFeiValue.BorderStyle = BorderStyle.Fixed3D;
			this.lblFeiValue.Location = new Point(125, 91);
			this.lblFeiValue.Margin = new Padding(3);
			this.lblFeiValue.Name = "lblFeiValue";
			this.lblFeiValue.Size = new Size(98, 20);
			this.lblFeiValue.TabIndex = 17;
			this.lblFeiValue.Text = "0";
			this.lblFeiValue.TextAlign = ContentAlignment.MiddleLeft;
			this.label12.AutoSize = true;
			this.label12.BackColor = Color.Transparent;
			this.label12.Location = new Point(3, 95);
			this.label12.Name = "label12";
			this.label12.Size = new Size(26, 13);
			this.label12.TabIndex = 15;
			this.label12.Text = "FEI:";
			this.label12.TextAlign = ContentAlignment.MiddleCenter;
			this.label20.AutoSize = true;
			this.label20.Location = new Point(3, 21);
			this.label20.Name = "label20";
			this.label20.Size = new Size(54, 13);
			this.label20.TabIndex = 8;
			this.label20.Text = "AFC auto:";
			this.label18.AutoSize = true;
			this.label18.BackColor = Color.Transparent;
			this.label18.Location = new Point(229, 69);
			this.label18.Name = "label18";
			this.label18.Size = new Size(20, 13);
			this.label18.TabIndex = 14;
			this.label18.Text = "Hz";
			this.label10.AutoSize = true;
			this.label10.BackColor = Color.Transparent;
			this.label10.Location = new Point(229, 95);
			this.label10.Name = "label10";
			this.label10.Size = new Size(20, 13);
			this.label10.TabIndex = 19;
			this.label10.Text = "Hz";
			this.btnAfcClear.Location = new Point(78, 64);
			this.btnAfcClear.Name = "btnAfcClear";
			this.btnAfcClear.Size = new Size(41, 23);
			this.btnAfcClear.TabIndex = 11;
			this.btnAfcClear.Text = "Clear";
			this.btnAfcClear.UseVisualStyleBackColor = true;
			this.btnAfcClear.Click += new EventHandler(this.btnAfcClear_Click);
			this.lblAfcValue.BackColor = Color.Transparent;
			this.lblAfcValue.BorderStyle = BorderStyle.Fixed3D;
			this.lblAfcValue.Location = new Point(125, 65);
			this.lblAfcValue.Margin = new Padding(3);
			this.lblAfcValue.Name = "lblAfcValue";
			this.lblAfcValue.Size = new Size(98, 20);
			this.lblAfcValue.TabIndex = 12;
			this.lblAfcValue.Text = "0";
			this.lblAfcValue.TextAlign = ContentAlignment.MiddleLeft;
			this.label22.AutoSize = true;
			this.label22.BackColor = Color.Transparent;
			this.label22.Location = new Point(3, 69);
			this.label22.Name = "label22";
			this.label22.Size = new Size(30, 13);
			this.label22.TabIndex = 9;
			this.label22.Text = "AFC:";
			this.label22.TextAlign = ContentAlignment.MiddleCenter;
			this.panel9.AutoSize = true;
			this.panel9.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel9.Controls.Add((Control)this.rBtnAfcAutoOff);
			this.panel9.Controls.Add((Control)this.rBtnAfcAutoOn);
			this.panel9.Location = new Point(125, 19);
			this.panel9.Name = "panel9";
			this.panel9.Size = new Size(98, 17);
			this.panel9.TabIndex = 7;
			this.rBtnAfcAutoOff.AutoSize = true;
			this.rBtnAfcAutoOff.Location = new Point(50, 0);
			this.rBtnAfcAutoOff.Margin = new Padding(3, 0, 3, 0);
			this.rBtnAfcAutoOff.Name = "rBtnAfcAutoOff";
			this.rBtnAfcAutoOff.Size = new Size(45, 17);
			this.rBtnAfcAutoOff.TabIndex = 1;
			this.rBtnAfcAutoOff.Text = "OFF";
			this.rBtnAfcAutoOff.UseVisualStyleBackColor = true;
			this.rBtnAfcAutoOff.CheckedChanged += new EventHandler(this.rBtnAfcAutoOn_CheckedChanged);
			this.rBtnAfcAutoOn.AutoSize = true;
			this.rBtnAfcAutoOn.Checked = true;
			this.rBtnAfcAutoOn.Location = new Point(3, 0);
			this.rBtnAfcAutoOn.Margin = new Padding(3, 0, 3, 0);
			this.rBtnAfcAutoOn.Name = "rBtnAfcAutoOn";
			this.rBtnAfcAutoOn.Size = new Size(41, 17);
			this.rBtnAfcAutoOn.TabIndex = 0;
			this.rBtnAfcAutoOn.TabStop = true;
			this.rBtnAfcAutoOn.Text = "ON";
			this.rBtnAfcAutoOn.UseVisualStyleBackColor = true;
			this.rBtnAfcAutoOn.CheckedChanged += new EventHandler(this.rBtnAfcAutoOn_CheckedChanged);
			this.gBoxLnaSettings.Controls.Add((Control)this.panel3);
			this.gBoxLnaSettings.Controls.Add((Control)this.label2);
			this.gBoxLnaSettings.Controls.Add((Control)this.label13);
			this.gBoxLnaSettings.Controls.Add((Control)this.lblAgcReference);
			this.gBoxLnaSettings.Controls.Add((Control)this.label48);
			this.gBoxLnaSettings.Controls.Add((Control)this.label49);
			this.gBoxLnaSettings.Controls.Add((Control)this.label50);
			this.gBoxLnaSettings.Controls.Add((Control)this.label51);
			this.gBoxLnaSettings.Controls.Add((Control)this.label52);
			this.gBoxLnaSettings.Controls.Add((Control)this.lblLnaGain1);
			this.gBoxLnaSettings.Controls.Add((Control)this.label53);
			this.gBoxLnaSettings.Controls.Add((Control)this.panel6);
			this.gBoxLnaSettings.Controls.Add((Control)this.lblLnaGain2);
			this.gBoxLnaSettings.Controls.Add((Control)this.lblLnaGain3);
			this.gBoxLnaSettings.Controls.Add((Control)this.lblLnaGain4);
			this.gBoxLnaSettings.Controls.Add((Control)this.lblLnaGain5);
			this.gBoxLnaSettings.Controls.Add((Control)this.lblLnaGain6);
			this.gBoxLnaSettings.Controls.Add((Control)this.lblAgcThresh1);
			this.gBoxLnaSettings.Controls.Add((Control)this.lblAgcThresh2);
			this.gBoxLnaSettings.Controls.Add((Control)this.lblAgcThresh3);
			this.gBoxLnaSettings.Controls.Add((Control)this.lblAgcThresh4);
			this.gBoxLnaSettings.Controls.Add((Control)this.lblAgcThresh5);
			this.gBoxLnaSettings.Controls.Add((Control)this.label47);
			this.gBoxLnaSettings.Location = new Point(3, 387);
			this.gBoxLnaSettings.Name = "gBoxLnaSettings";
			this.gBoxLnaSettings.Size = new Size(793, 103);
			this.gBoxLnaSettings.TabIndex = 7;
			this.gBoxLnaSettings.TabStop = false;
			this.gBoxLnaSettings.Text = "Lna settings";
			this.gBoxLnaSettings.MouseEnter += new EventHandler(this.control_MouseEnter);
			this.gBoxLnaSettings.MouseLeave += new EventHandler(this.control_MouseLeave);
			this.panel3.AutoSize = true;
			this.panel3.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel3.Controls.Add((Control)this.rBtnLnaBoostOff);
			this.panel3.Controls.Add((Control)this.rBtnLnaBoostOn);
			this.panel3.Location = new Point(71, 50);
			this.panel3.Name = "panel3";
			this.panel3.Size = new Size(98, 17);
			this.panel3.TabIndex = 23;
			this.rBtnLnaBoostOff.AutoSize = true;
			this.rBtnLnaBoostOff.Location = new Point(50, 0);
			this.rBtnLnaBoostOff.Margin = new Padding(3, 0, 3, 0);
			this.rBtnLnaBoostOff.Name = "rBtnLnaBoostOff";
			this.rBtnLnaBoostOff.Size = new Size(45, 17);
			this.rBtnLnaBoostOff.TabIndex = 1;
			this.rBtnLnaBoostOff.Text = "OFF";
			this.rBtnLnaBoostOff.UseVisualStyleBackColor = true;
			this.rBtnLnaBoostOff.CheckedChanged += new EventHandler(this.rBtnLnaBoost_CheckedChanged);
			this.rBtnLnaBoostOn.AutoSize = true;
			this.rBtnLnaBoostOn.Checked = true;
			this.rBtnLnaBoostOn.Location = new Point(3, 0);
			this.rBtnLnaBoostOn.Margin = new Padding(3, 0, 3, 0);
			this.rBtnLnaBoostOn.Name = "rBtnLnaBoostOn";
			this.rBtnLnaBoostOn.Size = new Size(41, 17);
			this.rBtnLnaBoostOn.TabIndex = 0;
			this.rBtnLnaBoostOn.TabStop = true;
			this.rBtnLnaBoostOn.Text = "ON";
			this.rBtnLnaBoostOn.UseVisualStyleBackColor = true;
			this.rBtnLnaBoostOn.CheckedChanged += new EventHandler(this.rBtnLnaBoost_CheckedChanged);
			this.label2.AutoSize = true;
			this.label2.Location = new Point(6, 52);
			this.label2.Name = "label2";
			this.label2.Size = new Size(60, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "LNA boost:";
			this.label13.BackColor = Color.Transparent;
			this.label13.Location = new Point(167, 75);
			this.label13.Name = "label13";
			this.label13.Size = new Size(42, 13);
			this.label13.TabIndex = 6;
			this.label13.Text = "Gain";
			this.label13.TextAlign = ContentAlignment.MiddleCenter;
			this.lblAgcReference.BackColor = Color.Transparent;
			this.lblAgcReference.Location = new Point(124, 32);
			this.lblAgcReference.Margin = new Padding(0, 0, 0, 3);
			this.lblAgcReference.Name = "lblAgcReference";
			this.lblAgcReference.Size = new Size(100, 13);
			this.lblAgcReference.TabIndex = 7;
			this.lblAgcReference.Text = "-80";
			this.lblAgcReference.TextAlign = ContentAlignment.MiddleCenter;
			this.label48.BackColor = Color.Transparent;
			this.label48.Location = new Point(124, 16);
			this.label48.Margin = new Padding(0, 0, 0, 3);
			this.label48.Name = "label48";
			this.label48.Size = new Size(100, 13);
			this.label48.TabIndex = 0;
			this.label48.Text = "Reference";
			this.label48.TextAlign = ContentAlignment.MiddleCenter;
			this.label49.BackColor = Color.Transparent;
			this.label49.Location = new Point(224, 16);
			this.label49.Margin = new Padding(0, 0, 0, 3);
			this.label49.Name = "label49";
			this.label49.Size = new Size(100, 13);
			this.label49.TabIndex = 1;
			this.label49.Text = "Threshold 1";
			this.label49.TextAlign = ContentAlignment.MiddleCenter;
			this.label50.BackColor = Color.Transparent;
			this.label50.Location = new Point(324, 16);
			this.label50.Margin = new Padding(0, 0, 0, 3);
			this.label50.Name = "label50";
			this.label50.Size = new Size(100, 13);
			this.label50.TabIndex = 2;
			this.label50.Text = "Threshold 2";
			this.label50.TextAlign = ContentAlignment.MiddleCenter;
			this.label51.BackColor = Color.Transparent;
			this.label51.Location = new Point(424, 16);
			this.label51.Margin = new Padding(0, 0, 0, 3);
			this.label51.Name = "label51";
			this.label51.Size = new Size(100, 13);
			this.label51.TabIndex = 3;
			this.label51.Text = "Threshold 3";
			this.label51.TextAlign = ContentAlignment.MiddleCenter;
			this.label52.BackColor = Color.Transparent;
			this.label52.Location = new Point(524, 16);
			this.label52.Margin = new Padding(0, 0, 0, 3);
			this.label52.Name = "label52";
			this.label52.Size = new Size(100, 13);
			this.label52.TabIndex = 4;
			this.label52.Text = "Threshold 4";
			this.label52.TextAlign = ContentAlignment.MiddleCenter;
			this.lblLnaGain1.BackColor = Color.LightSteelBlue;
			this.lblLnaGain1.BorderStyle = BorderStyle.Fixed3D;
			this.lblLnaGain1.Location = new Point(174, 48);
			this.lblLnaGain1.Margin = new Padding(0, 0, 0, 3);
			this.lblLnaGain1.Name = "lblLnaGain1";
			this.lblLnaGain1.Size = new Size(100, 20);
			this.lblLnaGain1.TabIndex = 14;
			this.lblLnaGain1.Text = "G1";
			this.lblLnaGain1.TextAlign = ContentAlignment.MiddleCenter;
			this.label53.BackColor = Color.Transparent;
			this.label53.Location = new Point(624, 16);
			this.label53.Margin = new Padding(0, 0, 0, 3);
			this.label53.Name = "label53";
			this.label53.Size = new Size(100, 13);
			this.label53.TabIndex = 5;
			this.label53.Text = "Threshold 5";
			this.label53.TextAlign = ContentAlignment.MiddleCenter;
			this.panel6.AutoSize = true;
			this.panel6.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel6.Controls.Add((Control)this.rBtnLnaGain1);
			this.panel6.Controls.Add((Control)this.rBtnLnaGain2);
			this.panel6.Controls.Add((Control)this.rBtnLnaGain3);
			this.panel6.Controls.Add((Control)this.rBtnLnaGain4);
			this.panel6.Controls.Add((Control)this.rBtnLnaGain5);
			this.panel6.Controls.Add((Control)this.rBtnLnaGain6);
			this.panel6.Location = new Point(215, 75);
			this.panel6.Name = "panel6";
			this.panel6.Size = new Size(521, 13);
			this.panel6.TabIndex = 22;
			this.rBtnLnaGain1.AutoSize = true;
			this.rBtnLnaGain1.Checked = true;
			this.rBtnLnaGain1.Location = new Point(3, 0);
			this.rBtnLnaGain1.Margin = new Padding(3, 0, 3, 0);
			this.rBtnLnaGain1.Name = "rBtnLnaGain1";
			this.rBtnLnaGain1.Size = new Size(14, 13);
			this.rBtnLnaGain1.TabIndex = 0;
			this.rBtnLnaGain1.TabStop = true;
			this.rBtnLnaGain1.UseVisualStyleBackColor = true;
			this.rBtnLnaGain1.CheckedChanged += new EventHandler(this.rBtnLnaGain_CheckedChanged);
			this.rBtnLnaGain2.AutoSize = true;
			this.rBtnLnaGain2.Location = new Point(102, 0);
			this.rBtnLnaGain2.Margin = new Padding(3, 0, 3, 0);
			this.rBtnLnaGain2.Name = "rBtnLnaGain2";
			this.rBtnLnaGain2.Size = new Size(14, 13);
			this.rBtnLnaGain2.TabIndex = 1;
			this.rBtnLnaGain2.UseVisualStyleBackColor = true;
			this.rBtnLnaGain2.CheckedChanged += new EventHandler(this.rBtnLnaGain_CheckedChanged);
			this.rBtnLnaGain3.AutoSize = true;
			this.rBtnLnaGain3.Location = new Point(203, 0);
			this.rBtnLnaGain3.Margin = new Padding(3, 0, 3, 0);
			this.rBtnLnaGain3.Name = "rBtnLnaGain3";
			this.rBtnLnaGain3.Size = new Size(14, 13);
			this.rBtnLnaGain3.TabIndex = 2;
			this.rBtnLnaGain3.UseVisualStyleBackColor = true;
			this.rBtnLnaGain3.CheckedChanged += new EventHandler(this.rBtnLnaGain_CheckedChanged);
			this.rBtnLnaGain4.AutoSize = true;
			this.rBtnLnaGain4.Location = new Point(303, 0);
			this.rBtnLnaGain4.Margin = new Padding(3, 0, 3, 0);
			this.rBtnLnaGain4.Name = "rBtnLnaGain4";
			this.rBtnLnaGain4.Size = new Size(14, 13);
			this.rBtnLnaGain4.TabIndex = 3;
			this.rBtnLnaGain4.UseVisualStyleBackColor = true;
			this.rBtnLnaGain4.CheckedChanged += new EventHandler(this.rBtnLnaGain_CheckedChanged);
			this.rBtnLnaGain5.AutoSize = true;
			this.rBtnLnaGain5.Location = new Point(404, 0);
			this.rBtnLnaGain5.Margin = new Padding(3, 0, 3, 0);
			this.rBtnLnaGain5.Name = "rBtnLnaGain5";
			this.rBtnLnaGain5.Size = new Size(14, 13);
			this.rBtnLnaGain5.TabIndex = 4;
			this.rBtnLnaGain5.UseVisualStyleBackColor = true;
			this.rBtnLnaGain5.CheckedChanged += new EventHandler(this.rBtnLnaGain_CheckedChanged);
			this.rBtnLnaGain6.AutoSize = true;
			this.rBtnLnaGain6.Location = new Point(504, 0);
			this.rBtnLnaGain6.Margin = new Padding(3, 0, 3, 0);
			this.rBtnLnaGain6.Name = "rBtnLnaGain6";
			this.rBtnLnaGain6.Size = new Size(14, 13);
			this.rBtnLnaGain6.TabIndex = 5;
			this.rBtnLnaGain6.UseVisualStyleBackColor = true;
			this.rBtnLnaGain6.CheckedChanged += new EventHandler(this.rBtnLnaGain_CheckedChanged);
			this.lblLnaGain2.BackColor = Color.Transparent;
			this.lblLnaGain2.BorderStyle = BorderStyle.Fixed3D;
			this.lblLnaGain2.Location = new Point(274, 48);
			this.lblLnaGain2.Margin = new Padding(0, 0, 0, 3);
			this.lblLnaGain2.Name = "lblLnaGain2";
			this.lblLnaGain2.Size = new Size(100, 20);
			this.lblLnaGain2.TabIndex = 15;
			this.lblLnaGain2.Text = "G2";
			this.lblLnaGain2.TextAlign = ContentAlignment.MiddleCenter;
			this.lblLnaGain3.BackColor = Color.Transparent;
			this.lblLnaGain3.BorderStyle = BorderStyle.Fixed3D;
			this.lblLnaGain3.Location = new Point(374, 48);
			this.lblLnaGain3.Margin = new Padding(0, 0, 0, 3);
			this.lblLnaGain3.Name = "lblLnaGain3";
			this.lblLnaGain3.Size = new Size(100, 20);
			this.lblLnaGain3.TabIndex = 16;
			this.lblLnaGain3.Text = "G3";
			this.lblLnaGain3.TextAlign = ContentAlignment.MiddleCenter;
			this.lblLnaGain4.BackColor = Color.Transparent;
			this.lblLnaGain4.BorderStyle = BorderStyle.Fixed3D;
			this.lblLnaGain4.Location = new Point(474, 48);
			this.lblLnaGain4.Margin = new Padding(0, 0, 0, 3);
			this.lblLnaGain4.Name = "lblLnaGain4";
			this.lblLnaGain4.Size = new Size(100, 20);
			this.lblLnaGain4.TabIndex = 17;
			this.lblLnaGain4.Text = "G4";
			this.lblLnaGain4.TextAlign = ContentAlignment.MiddleCenter;
			this.lblLnaGain5.BackColor = Color.Transparent;
			this.lblLnaGain5.BorderStyle = BorderStyle.Fixed3D;
			this.lblLnaGain5.Location = new Point(574, 48);
			this.lblLnaGain5.Margin = new Padding(0, 0, 0, 3);
			this.lblLnaGain5.Name = "lblLnaGain5";
			this.lblLnaGain5.Size = new Size(100, 20);
			this.lblLnaGain5.TabIndex = 18;
			this.lblLnaGain5.Text = "G5";
			this.lblLnaGain5.TextAlign = ContentAlignment.MiddleCenter;
			this.lblLnaGain6.BackColor = Color.Transparent;
			this.lblLnaGain6.BorderStyle = BorderStyle.Fixed3D;
			this.lblLnaGain6.Location = new Point(674, 48);
			this.lblLnaGain6.Margin = new Padding(0, 0, 0, 3);
			this.lblLnaGain6.Name = "lblLnaGain6";
			this.lblLnaGain6.Size = new Size(100, 20);
			this.lblLnaGain6.TabIndex = 19;
			this.lblLnaGain6.Text = "G6";
			this.lblLnaGain6.TextAlign = ContentAlignment.MiddleCenter;
			this.lblAgcThresh1.BackColor = Color.Transparent;
			this.lblAgcThresh1.Location = new Point(224, 32);
			this.lblAgcThresh1.Margin = new Padding(0, 0, 0, 3);
			this.lblAgcThresh1.Name = "lblAgcThresh1";
			this.lblAgcThresh1.Size = new Size(100, 13);
			this.lblAgcThresh1.TabIndex = 8;
			this.lblAgcThresh1.Text = "0";
			this.lblAgcThresh1.TextAlign = ContentAlignment.MiddleCenter;
			this.lblAgcThresh2.BackColor = Color.Transparent;
			this.lblAgcThresh2.Location = new Point(324, 32);
			this.lblAgcThresh2.Margin = new Padding(0, 0, 0, 3);
			this.lblAgcThresh2.Name = "lblAgcThresh2";
			this.lblAgcThresh2.Size = new Size(100, 13);
			this.lblAgcThresh2.TabIndex = 9;
			this.lblAgcThresh2.Text = "0";
			this.lblAgcThresh2.TextAlign = ContentAlignment.MiddleCenter;
			this.lblAgcThresh3.BackColor = Color.Transparent;
			this.lblAgcThresh3.Location = new Point(424, 32);
			this.lblAgcThresh3.Margin = new Padding(0, 0, 0, 3);
			this.lblAgcThresh3.Name = "lblAgcThresh3";
			this.lblAgcThresh3.Size = new Size(100, 13);
			this.lblAgcThresh3.TabIndex = 10;
			this.lblAgcThresh3.Text = "0";
			this.lblAgcThresh3.TextAlign = ContentAlignment.MiddleCenter;
			this.lblAgcThresh4.BackColor = Color.Transparent;
			this.lblAgcThresh4.Location = new Point(524, 32);
			this.lblAgcThresh4.Margin = new Padding(0, 0, 0, 3);
			this.lblAgcThresh4.Name = "lblAgcThresh4";
			this.lblAgcThresh4.Size = new Size(100, 13);
			this.lblAgcThresh4.TabIndex = 11;
			this.lblAgcThresh4.Text = "0";
			this.lblAgcThresh4.TextAlign = ContentAlignment.MiddleCenter;
			this.lblAgcThresh5.BackColor = Color.Transparent;
			this.lblAgcThresh5.Location = new Point(624, 32);
			this.lblAgcThresh5.Margin = new Padding(0, 0, 0, 3);
			this.lblAgcThresh5.Name = "lblAgcThresh5";
			this.lblAgcThresh5.Size = new Size(100, 13);
			this.lblAgcThresh5.TabIndex = 12;
			this.lblAgcThresh5.Text = "0";
			this.lblAgcThresh5.TextAlign = ContentAlignment.MiddleCenter;
			this.label47.AutoSize = true;
			this.label47.BackColor = Color.Transparent;
			this.label47.Location = new Point(723, 32);
			this.label47.Margin = new Padding(0);
			this.label47.Name = "label47";
			this.label47.Size = new Size(64, 13);
			this.label47.TabIndex = 13;
			this.label47.Text = "-> Pin [dBm]";
			this.label47.TextAlign = ContentAlignment.MiddleLeft;
			this.AutoScaleDimensions = new SizeF(6f, 13f);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.Controls.Add((Control)this.gBoxLnaSettings);
			this.Controls.Add((Control)this.panel2);
			this.Name = "ReceiverViewControl";
			this.Size = new Size(799, 493);
			((ISupportInitialize)this.errorProvider).EndInit();
			this.nudPreambleDetectorTol.EndInit();
			this.nudPreambleDetectorSize.EndInit();
			this.nudTimeoutRssi.EndInit();
			this.nudAutoRxRestartDelay.EndInit();
			this.nudTimeoutSyncWord.EndInit();
			this.nudTimeoutPreamble.EndInit();
			this.nudRxFilterBwAfc.EndInit();
			this.nudRxFilterBw.EndInit();
			this.nudOokAverageOffset.EndInit();
			this.nudOokPeakThreshStep.EndInit();
			this.nudOokFixedThresh.EndInit();
			this.nudRssiOffset.EndInit();
			this.nudRssiSmoothing.EndInit();
			this.nudRssiCollisionThreshold.EndInit();
			this.nudRssiThresh.EndInit();
			this.panel2.ResumeLayout(false);
			this.gBoxAgc.ResumeLayout(false);
			this.gBoxAgc.PerformLayout();
			this.panel5.ResumeLayout(false);
			this.panel5.PerformLayout();
			this.nudAgcStep5.EndInit();
			this.nudAgcStep4.EndInit();
			this.nudAgcReferenceLevel.EndInit();
			this.nudAgcStep3.EndInit();
			this.nudAgcStep1.EndInit();
			this.nudAgcStep2.EndInit();
			this.gBoxTimeout.ResumeLayout(false);
			this.gBoxTimeout.PerformLayout();
			this.gBoxPreamble.ResumeLayout(false);
			this.gBoxPreamble.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.gBoxRxBw.ResumeLayout(false);
			this.gBoxRxBw.PerformLayout();
			this.gBoxRxConfig.ResumeLayout(false);
			this.gBoxRxConfig.PerformLayout();
			this.panel4.ResumeLayout(false);
			this.panel4.PerformLayout();
			this.gBoxDemodulator.ResumeLayout(false);
			this.gBoxDemodulator.PerformLayout();
			this.panel13.ResumeLayout(false);
			this.panel13.PerformLayout();
			this.gBoxRssi.ResumeLayout(false);
			this.gBoxRssi.PerformLayout();
			this.gBoxAfc.ResumeLayout(false);
			this.gBoxAfc.PerformLayout();
			this.panel8.ResumeLayout(false);
			this.panel8.PerformLayout();
			this.panel9.ResumeLayout(false);
			this.panel9.PerformLayout();
			this.gBoxLnaSettings.ResumeLayout(false);
			this.gBoxLnaSettings.PerformLayout();
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			this.panel6.ResumeLayout(false);
			this.panel6.PerformLayout();
			this.ResumeLayout(false);
		}

		private void OnAgcReferenceLevelChanged(int value)
		{
			if (this.AgcReferenceLevelChanged == null)
				return;
			this.AgcReferenceLevelChanged((object)this, new Int32EventArg(value));
		}

		private void OnAgcStepChanged(byte id, byte value)
		{
			if (this.AgcStepChanged == null)
				return;
			this.AgcStepChanged((object)this, new AgcStepEventArg(id, value));
		}

		private void OnLnaGainChanged(LnaGainEnum value)
		{
			if (this.LnaGainChanged == null)
				return;
			this.LnaGainChanged((object)this, new LnaGainEventArg(value));
		}

		private void OnLnaBoostChanged(bool value)
		{
			if (this.LnaBoostChanged == null)
				return;
			this.LnaBoostChanged((object)this, new BooleanEventArg(value));
		}

		private void OnRestartRxOnCollisionOnChanged(bool value)
		{
			if (this.RestartRxOnCollisionOnChanged == null)
				return;
			this.RestartRxOnCollisionOnChanged((object)this, new BooleanEventArg(value));
		}

		private void OnRestartRxWithoutPllLockChanged()
		{
			if (this.RestartRxWithoutPllLockChanged == null)
				return;
			this.RestartRxWithoutPllLockChanged((object)this, EventArgs.Empty);
		}

		private void OnRestartRxWithPllLockChanged()
		{
			if (this.RestartRxWithPllLockChanged == null)
				return;
			this.RestartRxWithPllLockChanged((object)this, EventArgs.Empty);
		}

		private void OnAfcAutoOnChanged(bool value)
		{
			if (this.AfcAutoOnChanged == null)
				return;
			this.AfcAutoOnChanged((object)this, new BooleanEventArg(value));
		}

		private void OnAgcAutoOnChanged(bool value)
		{
			if (this.AgcAutoOnChanged == null)
				return;
			this.AgcAutoOnChanged((object)this, new BooleanEventArg(value));
		}

		private void OnRxTriggerChanged(RxTriggerEnum value)
		{
			if (this.RxTriggerChanged == null)
				return;
			this.RxTriggerChanged((object)this, new RxTriggerEventArg(value));
		}

		private void OnRssiOffsetChanged(Decimal value)
		{
			if (this.RssiOffsetChanged == null)
				return;
			this.RssiOffsetChanged((object)this, new DecimalEventArg(value));
		}

		private void OnRssiSmoothingChanged(Decimal value)
		{
			if (this.RssiSmoothingChanged == null)
				return;
			this.RssiSmoothingChanged((object)this, new DecimalEventArg(value));
		}

		private void OnRssiCollisionThresholdChanged(Decimal value)
		{
			if (this.RssiCollisionThresholdChanged == null)
				return;
			this.RssiCollisionThresholdChanged((object)this, new DecimalEventArg(value));
		}

		private void OnRssiThreshChanged(Decimal value)
		{
			if (this.RssiThreshChanged == null)
				return;
			this.RssiThreshChanged((object)this, new DecimalEventArg(value));
		}

		private void OnRxBwChanged(Decimal value)
		{
			if (this.RxBwChanged == null)
				return;
			this.RxBwChanged((object)this, new DecimalEventArg(value));
		}

		private void OnAfcRxBwChanged(Decimal value)
		{
			if (this.AfcRxBwChanged == null)
				return;
			this.AfcRxBwChanged((object)this, new DecimalEventArg(value));
		}

		private void OnBitSyncOnChanged(bool value)
		{
			if (this.BitSyncOnChanged == null)
				return;
			this.BitSyncOnChanged((object)this, new BooleanEventArg(value));
		}

		private void OnOokThreshTypeChanged(OokThreshTypeEnum value)
		{
			if (this.OokThreshTypeChanged == null)
				return;
			this.OokThreshTypeChanged((object)this, new OokThreshTypeEventArg(value));
		}

		private void OnOokPeakThreshStepChanged(Decimal value)
		{
			if (this.OokPeakThreshStepChanged == null)
				return;
			this.OokPeakThreshStepChanged((object)this, new DecimalEventArg(value));
		}

		private void OnOokPeakThreshDecChanged(OokPeakThreshDecEnum value)
		{
			if (this.OokPeakThreshDecChanged == null)
				return;
			this.OokPeakThreshDecChanged((object)this, new OokPeakThreshDecEventArg(value));
		}

		private void OnOokAverageBiasChanged(Decimal value)
		{
			if (this.OokAverageBiasChanged == null)
				return;
			this.OokAverageBiasChanged((object)this, new DecimalEventArg(value));
		}

		private void OnOokAverageThreshFiltChanged(OokAverageThreshFiltEnum value)
		{
			if (this.OokAverageThreshFiltChanged == null)
				return;
			this.OokAverageThreshFiltChanged((object)this, new OokAverageThreshFiltEventArg(value));
		}

		private void OnOokFixedThreshChanged(byte value)
		{
			if (this.OokFixedThreshChanged == null)
				return;
			this.OokFixedThreshChanged((object)this, new ByteEventArg(value));
		}

		private void OnAgcStartChanged()
		{
			if (this.AgcStartChanged == null)
				return;
			this.AgcStartChanged((object)this, EventArgs.Empty);
		}

		private void OnFeiReadChanged()
		{
			if (this.FeiReadChanged == null)
				return;
			this.FeiReadChanged((object)this, EventArgs.Empty);
		}

		private void OnAfcAutoClearOnChanged(bool value)
		{
			if (this.AfcAutoClearOnChanged == null)
				return;
			this.AfcAutoClearOnChanged((object)this, new BooleanEventArg(value));
		}

		private void OnAfcClearChanged()
		{
			if (this.AfcClearChanged == null)
				return;
			this.AfcClearChanged((object)this, EventArgs.Empty);
		}

		private void OnPreambleDetectorOnChanged(bool value)
		{
			if (this.PreambleDetectorOnChanged == null)
				return;
			this.PreambleDetectorOnChanged((object)this, new BooleanEventArg(value));
		}

		private void OnPreambleDetectorSizeChanged(byte value)
		{
			if (this.PreambleDetectorSizeChanged == null)
				return;
			this.PreambleDetectorSizeChanged((object)this, new ByteEventArg(value));
		}

		private void OnPreambleDetectorTolChanged(byte value)
		{
			if (this.PreambleDetectorTolChanged == null)
				return;
			this.PreambleDetectorTolChanged((object)this, new ByteEventArg(value));
		}

		private void OnTimeoutRssiChanged(Decimal value)
		{
			if (this.TimeoutRssiChanged == null)
				return;
			this.TimeoutRssiChanged((object)this, new DecimalEventArg(value));
		}

		private void OnTimeoutPreambleChanged(Decimal value)
		{
			if (this.TimeoutPreambleChanged == null)
				return;
			this.TimeoutPreambleChanged((object)this, new DecimalEventArg(value));
		}

		private void OnTimeoutSyncWordChanged(Decimal value)
		{
			if (this.TimeoutSyncWordChanged == null)
				return;
			this.TimeoutSyncWordChanged((object)this, new DecimalEventArg(value));
		}

		private void OnAutoRxRestartDelayChanged(Decimal value)
		{
			if (this.AutoRxRestartDelayChanged == null)
				return;
			this.AutoRxRestartDelayChanged((object)this, new DecimalEventArg(value));
		}

		public void UpdateRxBwLimits(LimitCheckStatusEnum status, string message)
		{
			switch (status)
			{
				case LimitCheckStatusEnum.OK:
					this.nudRxFilterBw.BackColor = SystemColors.Window;
					break;
				case LimitCheckStatusEnum.OUT_OF_RANGE:
					this.nudRxFilterBw.BackColor = ControlPaint.LightLight(Color.Orange);
					break;
				case LimitCheckStatusEnum.ERROR:
					this.nudRxFilterBw.BackColor = ControlPaint.LightLight(Color.Red);
					break;
			}
			this.errorProvider.SetError((Control)this.nudRxFilterBw, message);
		}

		private void nudAgcReferenceLevel_ValueChanged(object sender, EventArgs e)
		{
			this.AgcReferenceLevel = (int)this.nudAgcReferenceLevel.Value;
			this.OnAgcReferenceLevelChanged(this.AgcReferenceLevel);
		}

		private void nudAgcStep_ValueChanged(object sender, EventArgs e)
		{
			byte num = (byte)0;
			byte id = (byte)0;
			if (sender == this.nudAgcStep1)
			{
				num = this.AgcStep1 = (byte)this.nudAgcStep1.Value;
				id = (byte)1;
			}
			else if (sender == this.nudAgcStep2)
			{
				num = this.AgcStep2 = (byte)this.nudAgcStep2.Value;
				id = (byte)2;
			}
			else if (sender == this.nudAgcStep3)
			{
				num = this.AgcStep3 = (byte)this.nudAgcStep3.Value;
				id = (byte)3;
			}
			else if (sender == this.nudAgcStep4)
			{
				num = this.AgcStep4 = (byte)this.nudAgcStep4.Value;
				id = (byte)4;
			}
			else if (sender == this.nudAgcStep5)
			{
				num = this.AgcStep5 = (byte)this.nudAgcStep5.Value;
				id = (byte)5;
			}
			this.OnAgcStepChanged(id, num);
		}

		private void rBtnLnaGain_CheckedChanged(object sender, EventArgs e)
		{
			this.LnaGain = !this.rBtnLnaGain1.Checked ? (!this.rBtnLnaGain2.Checked ? (!this.rBtnLnaGain3.Checked ? (!this.rBtnLnaGain4.Checked ? (!this.rBtnLnaGain5.Checked ? (!this.rBtnLnaGain6.Checked ? LnaGainEnum.G1 : LnaGainEnum.G6) : LnaGainEnum.G5) : LnaGainEnum.G4) : LnaGainEnum.G3) : LnaGainEnum.G2) : LnaGainEnum.G1;
			this.OnLnaGainChanged(this.LnaGain);
		}

		private void rBtnLnaBoost_CheckedChanged(object sender, EventArgs e)
		{
			this.LnaBoost = this.rBtnLnaBoostOn.Checked;
			this.OnLnaBoostChanged(this.LnaBoost);
		}

		private void rBtnRestartRxOnCollisionOn_CheckedChanged(object sender, EventArgs e)
		{
			this.RestartRxOnCollision = this.rBtnRestartRxOnCollisionOn.Checked;
			this.OnRestartRxOnCollisionOnChanged(this.RestartRxOnCollision);
		}

		private void btnRestartRxWithoutPllLock_Click(object sender, EventArgs e)
		{
			this.OnRestartRxWithoutPllLockChanged();
		}

		private void btnRestartRxWithPllLock_Click(object sender, EventArgs e)
		{
			this.OnRestartRxWithPllLockChanged();
		}

		private void rBtnAfcAutoOn_CheckedChanged(object sender, EventArgs e)
		{
			this.AfcAutoOn = this.rBtnAfcAutoOn.Checked;
			this.OnAfcAutoOnChanged(this.AfcAutoOn);
		}

		private void rBtnAgcAutoOn_CheckedChanged(object sender, EventArgs e)
		{
			this.AgcAutoOn = this.rBtnAgcAutoOn.Checked;
			this.OnAgcAutoOnChanged(this.AgcAutoOn);
		}

		private void cBoxRxTrigger_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.RxTrigger = (RxTriggerEnum)this.cBoxRxTrigger.SelectedIndex;
			this.OnRxTriggerChanged(this.RxTrigger);
		}

		private void nudRssiOffset_ValueChanged(object sender, EventArgs e)
		{
			this.RssiOffset = this.nudRssiOffset.Value;
			this.OnRssiOffsetChanged(this.RssiOffset);
		}

		private void nudRssiSmoothing_ValueChanged(object sender, EventArgs e)
		{
			int num1 = (int)Math.Log((double)this.RssiSmoothing, 2.0);
			int num2 = (int)Math.Log((double)this.nudRssiSmoothing.Value, 2.0);
			int num3 = (int)(this.nudRssiSmoothing.Value - this.RssiSmoothing);
			this.nudRssiSmoothing.ValueChanged -= new EventHandler(this.nudRssiSmoothing_ValueChanged);
			if (num1 == 0)
				num3 = 0;
			if (num3 >= 0 && num3 <= 1)
				this.nudRssiSmoothing.Value = (Decimal)Math.Pow(2.0, (double)(num2 + num3));
			else
				this.nudRssiSmoothing.Value = (Decimal)Math.Pow(2.0, (double)num2);
			this.nudRssiSmoothing.ValueChanged += new EventHandler(this.nudRssiSmoothing_ValueChanged);
			this.RssiSmoothing = this.nudRssiSmoothing.Value;
			this.OnRssiSmoothingChanged(this.RssiSmoothing);
		}

		private void nudRssiCollisionThreshold_ValueChanged(object sender, EventArgs e)
		{
			this.RssiCollisionThreshold = this.nudRssiCollisionThreshold.Value;
			this.OnRssiCollisionThresholdChanged(this.RssiCollisionThreshold);
		}

		private void nudRssiThresh_ValueChanged(object sender, EventArgs e)
		{
			this.RssiThreshold = this.nudRssiThresh.Value;
			this.OnRssiThreshChanged(this.RssiThreshold);
		}

		private void nudRxFilterBw_ValueChanged(object sender, EventArgs e)
		{
			Decimal[] rxBwFreqTable = SX1276.ComputeRxBwFreqTable(this.frequencyXo, this.modulationType);
			int num1 = (int)(this.nudRxFilterBw.Value - this.RxBw);
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
				SX1276.ComputeRxBwMantExp(this.frequencyXo, this.ModulationType, this.nudRxFilterBw.Value, ref mant, ref exp);
				Decimal rxBw = SX1276.ComputeRxBw(this.frequencyXo, this.ModulationType, mant, exp);
				index = Array.IndexOf<Decimal>(rxBwFreqTable, rxBw);
			}
			this.nudRxFilterBw.ValueChanged -= new EventHandler(this.nudRxFilterBw_ValueChanged);
			this.nudRxFilterBw.Value = rxBwFreqTable[index];
			this.nudRxFilterBw.ValueChanged += new EventHandler(this.nudRxFilterBw_ValueChanged);
			this.RxBw = this.nudRxFilterBw.Value;
			this.OnRxBwChanged(this.RxBw);
		}

		private void nudRxFilterBwAfc_ValueChanged(object sender, EventArgs e)
		{
			Decimal[] rxBwFreqTable = SX1276.ComputeRxBwFreqTable(this.frequencyXo, this.modulationType);
			int num1 = (int)(this.nudRxFilterBwAfc.Value - this.AfcRxBw);
			int index;
			if (num1 >= -1 && num1 <= 1)
			{
				index = Array.IndexOf<Decimal>(rxBwFreqTable, this.AfcRxBw) - num1;
			}
			else
			{
				int mant = 0;
				int exp = 0;
				Decimal num2 = new Decimal(0);
				SX1276.ComputeRxBwMantExp(this.frequencyXo, this.ModulationType, this.nudRxFilterBwAfc.Value, ref mant, ref exp);
				Decimal rxBw = SX1276.ComputeRxBw(this.frequencyXo, this.ModulationType, mant, exp);
				index = Array.IndexOf<Decimal>(rxBwFreqTable, rxBw);
			}
			this.nudRxFilterBwAfc.ValueChanged -= new EventHandler(this.nudRxFilterBwAfc_ValueChanged);
			this.nudRxFilterBwAfc.Value = rxBwFreqTable[index];
			this.nudRxFilterBwAfc.ValueChanged += new EventHandler(this.nudRxFilterBwAfc_ValueChanged);
			this.AfcRxBw = this.nudRxFilterBwAfc.Value;
			this.OnAfcRxBwChanged(this.AfcRxBw);
		}

		private void rBtnBitSyncOn_CheckedChanged(object sender, EventArgs e)
		{
			this.BitSyncOn = this.rBtnBitSyncOn.Checked;
			this.OnBitSyncOnChanged(this.BitSyncOn);
		}

		private void cBoxOokThreshType_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.OokThreshType = (OokThreshTypeEnum)this.cBoxOokThreshType.SelectedIndex;
			this.OnOokThreshTypeChanged(this.OokThreshType);
		}

		private void nudOokPeakThreshStep_Validating(object sender, CancelEventArgs e)
		{
			int num = this.nudOokPeakThreshStep.Value < new Decimal(2) ? 1 : 0;
		}

		private void nudOokPeakThreshStep_ValueChanged(object sender, EventArgs e)
		{
			try
			{
				this.nudOokPeakThreshStep.ValueChanged -= new EventHandler(this.nudOokPeakThreshStep_ValueChanged);
				Decimal[] array = new Decimal[8]
        {
          new Decimal(5, 0, 0, false, (byte) 1),
          new Decimal(10, 0, 0, false, (byte) 1),
          new Decimal(15, 0, 0, false, (byte) 1),
          new Decimal(20, 0, 0, false, (byte) 1),
          new Decimal(30, 0, 0, false, (byte) 1),
          new Decimal(40, 0, 0, false, (byte) 1),
          new Decimal(50, 0, 0, false, (byte) 1),
          new Decimal(60, 0, 0, false, (byte) 1)
        };
				int index1 = 0;
				Decimal num1 = this.nudOokPeakThreshStep.Value - this.OokPeakThreshStep;
				Decimal num2 = new Decimal(10000000);
				for (int index2 = 0; index2 < 8; ++index2)
				{
					if (Math.Abs(this.nudOokPeakThreshStep.Value - array[index2]) < num2)
					{
						num2 = Math.Abs(this.nudOokPeakThreshStep.Value - array[index2]);
						index1 = index2;
					}
				}
				if (num2 / Math.Abs(num1) == new Decimal(1) && num1 >= new Decimal(5, 0, 0, false, (byte)1))
				{
					if (num1 > new Decimal(0))
					{
						NumericUpDownEx numericUpDownEx = this.nudOokPeakThreshStep;
						Decimal num3 = numericUpDownEx.Value + this.nudOokPeakThreshStep.Increment;
						numericUpDownEx.Value = num3;
					}
					else
					{
						NumericUpDownEx numericUpDownEx = this.nudOokPeakThreshStep;
						Decimal num3 = numericUpDownEx.Value - this.nudOokPeakThreshStep.Increment;
						numericUpDownEx.Value = num3;
					}
					index1 = Array.IndexOf<Decimal>(array, this.nudOokPeakThreshStep.Value);
				}
				this.nudOokPeakThreshStep.Value = array[index1];
				this.nudOokPeakThreshStep.ValueChanged += new EventHandler(this.nudOokPeakThreshStep_ValueChanged);
				this.OokPeakThreshStep = this.nudOokPeakThreshStep.Value;
				this.OnOokPeakThreshStepChanged(this.OokPeakThreshStep);
			}
			catch
			{
				this.nudOokPeakThreshStep.ValueChanged += new EventHandler(this.nudOokPeakThreshStep_ValueChanged);
			}
		}

		private void cBoxOokPeakThreshDec_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.OokPeakThreshDec = (OokPeakThreshDecEnum)this.cBoxOokPeakThreshDec.SelectedIndex;
			this.OnOokPeakThreshDecChanged(this.OokPeakThreshDec);
		}

		private void cBoxOokAverageThreshFilt_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.OokAverageThreshFilt = (OokAverageThreshFiltEnum)this.cBoxOokAverageThreshFilt.SelectedIndex;
			this.OnOokAverageThreshFiltChanged(this.OokAverageThreshFilt);
		}

		private void nudOokAverageOffset_ValueChanged(object sender, EventArgs e)
		{
			this.OokAverageOffset = this.nudOokAverageOffset.Value;
			this.OnOokAverageBiasChanged(this.OokAverageOffset);
		}

		private void nudOokFixedThresh_ValueChanged(object sender, EventArgs e)
		{
			this.OokFixedThreshold = (byte)this.nudOokFixedThresh.Value;
			this.OnOokFixedThreshChanged(this.OokFixedThreshold);
		}

		private void btnAgcStart_Click(object sender, EventArgs e)
		{
			this.OnAgcStartChanged();
		}

		private void btnFeiMeasure_Click(object sender, EventArgs e)
		{
			this.OnFeiReadChanged();
		}

		private void rBtnAfcAutoClearOn_CheckedChanged(object sender, EventArgs e)
		{
			this.AfcAutoClearOn = this.rBtnAfcAutoClearOn.Checked;
			this.OnAfcAutoClearOnChanged(this.AfcAutoClearOn);
		}

		private void btnAfcClear_Click(object sender, EventArgs e)
		{
			this.OnAfcClearChanged();
		}

		private void rBtnPreambleDetectorOn_CheckedChanged(object sender, EventArgs e)
		{
			this.PreambleDetectorOn = this.rBtnPreambleDetectorOn.Checked;
			this.OnPreambleDetectorOnChanged(this.PreambleDetectorOn);
		}

		private void nudPreambleDetectorSize_ValueChanged(object sender, EventArgs e)
		{
			this.PreambleDetectorSize = (byte)this.nudPreambleDetectorSize.Value;
			this.OnPreambleDetectorSizeChanged(this.PreambleDetectorSize);
		}

		private void nudPreambleDetectorTol_ValueChanged(object sender, EventArgs e)
		{
			this.PreambleDetectorTol = (byte)this.nudPreambleDetectorTol.Value;
			this.OnPreambleDetectorTolChanged(this.PreambleDetectorTol);
		}

		private void nudTimeoutRssi_ValueChanged(object sender, EventArgs e)
		{
			this.TimeoutRxRssi = this.nudTimeoutRssi.Value;
			this.OnTimeoutRssiChanged(this.TimeoutRxRssi);
		}

		private void nudTimeoutPreamble_ValueChanged(object sender, EventArgs e)
		{
			this.TimeoutRxPreamble = this.nudTimeoutPreamble.Value;
			this.OnTimeoutPreambleChanged(this.TimeoutRxPreamble);
		}

		private void nudTimeoutSyncWord_ValueChanged(object sender, EventArgs e)
		{
			this.TimeoutSignalSync = this.nudTimeoutSyncWord.Value;
			this.OnTimeoutSyncWordChanged(this.TimeoutSignalSync);
		}

		private void nudAutoRxRestartDelay_ValueChanged(object sender, EventArgs e)
		{
			this.InterPacketRxDelay = this.nudAutoRxRestartDelay.Value;
			this.OnAutoRxRestartDelayChanged(this.InterPacketRxDelay);
		}

		private void control_MouseEnter(object sender, EventArgs e)
		{
			if (sender == this.gBoxRxBw)
				this.OnDocumentationChanged(new DocumentationChangedEventArgs("Receiver", "Bandwidth"));
			else if (sender == this.gBoxDemodulator)
				this.OnDocumentationChanged(new DocumentationChangedEventArgs("Receiver", "Demodulator"));
			else if (sender == this.gBoxAfc)
				this.OnDocumentationChanged(new DocumentationChangedEventArgs("Receiver", "Afc"));
			else if (sender == this.gBoxRssi)
			{
				this.OnDocumentationChanged(new DocumentationChangedEventArgs("Receiver", "Rssi"));
			}
			else
			{
				if (sender != this.gBoxLnaSettings)
					return;
				this.OnDocumentationChanged(new DocumentationChangedEventArgs("Receiver", "Lna"));
			}
		}

		private void control_MouseLeave(object sender, EventArgs e)
		{
			this.OnDocumentationChanged(new DocumentationChangedEventArgs(".", "Overview"));
		}

		private void OnDocumentationChanged(DocumentationChangedEventArgs e)
		{
			if (this.DocumentationChanged == null)
				return;
			this.DocumentationChanged((object)this, e);
		}
	}
}

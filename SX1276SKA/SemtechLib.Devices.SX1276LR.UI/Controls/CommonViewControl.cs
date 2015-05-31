using SemtechLib.Controls;
using SemtechLib.Devices.SX1276LR.Enumerations;
using SemtechLib.Devices.SX1276LR.Events;
using SemtechLib.General.Events;
using SemtechLib.General.Interfaces;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SemtechLib.Devices.SX1276LR.UI.Controls
{
	public class CommonViewControl : UserControl, INotifyDocumentationChanged
	{
		private Decimal maxOutputPower = new Decimal(132, 0, 0, false, (byte)1);
		private Decimal outputPower = new Decimal(132, 0, 0, false, (byte)1);
		private Decimal ocpTrim = new Decimal(100);
		private int agcReference = 19;
		private Decimal pllBandwidth;
		private int agcThresh1;
		private int agcThresh2;
		private int agcThresh3;
		private int agcThresh4;
		private int agcThresh5;
		private IContainer components;
		private NumericUpDownEx nudFrequencyXo;
		private NumericUpDownEx nudFrequencyRf;
		private Label label1;
		private Label label9;
		private Label label14;
		private Label lblRcOscillatorCalStat;
		private Label label13;
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
		private GroupBoxEx gBoxGeneral;
		private Panel panel1;
		private RadioButton rBtnTcxoInputOff;
		private RadioButton rBtnTcxoInputOn;
		private Label label3;
		private Panel panel5;
		private RadioButton rBtnFastHopOff;
		private RadioButton rBtnFastHopOn;
		private GroupBoxEx gBoxRxSettings;
		private Panel panel3;
		private RadioButton rBtnLnaBoostOff;
		private RadioButton rBtnLnaBoostOn;
		private Label label2;
		private Label label4;
		private Label lblAgcReference;
		private Label label48;
		private Label label49;
		private Label label50;
		private Label label51;
		private Label label52;
		private Label lblLnaGain1;
		private Label label53;
		private Panel panel2;
		private RadioButton rBtnLnaGain1;
		private RadioButton rBtnLnaGain2;
		private RadioButton rBtnLnaGain3;
		private RadioButton rBtnLnaGain4;
		private RadioButton rBtnLnaGain5;
		private RadioButton rBtnLnaGain6;
		private Label lblLnaGain2;
		private Label lblLnaGain3;
		private Label lblLnaGain4;
		private Label lblLnaGain5;
		private Label lblLnaGain6;
		private Label lblAgcThresh1;
		private Label lblAgcThresh2;
		private Label lblAgcThresh3;
		private Label lblAgcThresh4;
		private Label lblAgcThresh5;
		private Label label47;
		private NumericUpDownEx nudPllBandwidth;
		private Label label5;
		private Label label8;
		private Panel panel4;
		private RadioButton rBtnOcpOff;
		private RadioButton rBtnOcpOn;
		private Label label10;
		private NumericUpDownEx nudOcpTrim;
		private Label suffixOCPtrim;
		private NumericUpDownEx nudOutputPower;
		private Label suffixOutputPower;
		private GroupBoxEx gBoxTxSettings;
		private ComboBox cBoxPaRamp;
		private Panel pnlPaSelect;
		private RadioButton rBtnRfPa;
		private RadioButton rBtnRfo;
		private Label suffixPAramp;
		private Label label12;
		private Label label17;
		private Panel panel9;
		private RadioButton rBtnAgcAutoOff;
		private RadioButton rBtnAgcAutoOn;
		private GroupBoxEx gBoxAgc;
		private Label label15;
		private Label label16;
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
		private Label label19;
		private Label label18;
		private GroupBox groupBox1;
		private TableLayoutPanel tableLayoutPanel1;
		private ComboBox cBoxDio4Mapping;
		private ComboBox cBoxDio3Mapping;
		private ComboBox cBoxDio2Mapping;
		private ComboBox cBoxDio1Mapping;
		private ComboBox cBoxDio0Mapping;
		private Label label54;
		private Label label55;
		private Label label56;
		private Label label57;
		private Label label58;
		private Label label35;
		private ComboBox cBoxDio5Mapping;
		private NumericUpDownEx nudMaxOutputPower;
		private Label label7;
		private Label label6;
		private ComboBox cBoxBand;
		private Label label11;
		private Label label36;
		private Panel panel8;
		private RadioButton rBtnLowFrequencyModeOff;
		private RadioButton rBtnLowFrequencyModeOn;
		private Label label37;
		private Panel panel10;
		private RadioButton rBtnForceRxBandLowFrequencyOff;
		private RadioButton rBtnForceRxBandLowFrequencyOn;
		private Label label38;
		private Panel panel11;
		private RadioButton rBtnForceTxBandLowFrequencyOff;
		private RadioButton rBtnForceTxBandLowFrequencyOn;
		private Label label39;
		private Panel panel12;
		private GroupBoxEx gBoxOptioanl;
		private Panel pnlPa20dBm;
		private RadioButton rBtnPa20dBmOff;
		private RadioButton rBtnPa20dBmOn;
		private Label lblPa20dBm;

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
					rBtnLnaBoostOn.Enabled = false;
					rBtnLnaBoostOff.Enabled = false;
				}
				else
				{
					rBtnLowFrequencyModeOn.Checked = false;
					rBtnLowFrequencyModeOff.Checked = true;
					rBtnLnaBoostOn.Enabled = true;
					rBtnLnaBoostOff.Enabled = true;
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
					nudFrequencyRf.BackColor = SystemColors.Window;
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
				catch
				{
				}
				finally
				{
					nudPllBandwidth.ValueChanged += new EventHandler(nudPllBandwidth_ValueChanged);
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

		public DioMappingEnum Dio0Mapping
		{
			get
			{
				return (DioMappingEnum)cBoxDio0Mapping.SelectedIndex;
			}
			set
			{
				try
				{
					cBoxDio0Mapping.SelectedIndexChanged -= new EventHandler(cBoxDio0Mapping_SelectedIndexChanged);
					cBoxDio0Mapping.SelectedIndex = (int)value;
					cBoxDio0Mapping.SelectedIndexChanged += new EventHandler(cBoxDio0Mapping_SelectedIndexChanged);
				}
				catch
				{
					cBoxDio0Mapping.SelectedIndexChanged += new EventHandler(cBoxDio0Mapping_SelectedIndexChanged);
				}
			}
		}

		public DioMappingEnum Dio1Mapping
		{
			get
			{
				return (DioMappingEnum)cBoxDio1Mapping.SelectedIndex;
			}
			set
			{
				try
				{
					cBoxDio1Mapping.SelectedIndexChanged -= new EventHandler(cBoxDio1Mapping_SelectedIndexChanged);
					cBoxDio1Mapping.SelectedIndex = (int)value;
					cBoxDio1Mapping.SelectedIndexChanged += new EventHandler(cBoxDio1Mapping_SelectedIndexChanged);
				}
				catch
				{
					cBoxDio1Mapping.SelectedIndexChanged += new EventHandler(cBoxDio1Mapping_SelectedIndexChanged);
				}
			}
		}

		public DioMappingEnum Dio2Mapping
		{
			get
			{
				return (DioMappingEnum)cBoxDio2Mapping.SelectedIndex;
			}
			set
			{
				try
				{
					cBoxDio2Mapping.SelectedIndexChanged -= new EventHandler(cBoxDio2Mapping_SelectedIndexChanged);
					cBoxDio2Mapping.SelectedIndex = (int)value;
					cBoxDio2Mapping.SelectedIndexChanged += new EventHandler(cBoxDio2Mapping_SelectedIndexChanged);
				}
				catch
				{
					cBoxDio2Mapping.SelectedIndexChanged += new EventHandler(cBoxDio2Mapping_SelectedIndexChanged);
				}
			}
		}

		public DioMappingEnum Dio3Mapping
		{
			get
			{
				return (DioMappingEnum)cBoxDio3Mapping.SelectedIndex;
			}
			set
			{
				try
				{
					cBoxDio3Mapping.SelectedIndexChanged -= new EventHandler(cBoxDio3Mapping_SelectedIndexChanged);
					cBoxDio3Mapping.SelectedIndex = (int)value;
					cBoxDio3Mapping.SelectedIndexChanged += new EventHandler(cBoxDio3Mapping_SelectedIndexChanged);
				}
				catch
				{
					cBoxDio3Mapping.SelectedIndexChanged += new EventHandler(cBoxDio3Mapping_SelectedIndexChanged);
				}
			}
		}

		public DioMappingEnum Dio4Mapping
		{
			get
			{
				return (DioMappingEnum)cBoxDio4Mapping.SelectedIndex;
			}
			set
			{
				try
				{
					cBoxDio4Mapping.SelectedIndexChanged -= new EventHandler(cBoxDio4Mapping_SelectedIndexChanged);
					cBoxDio4Mapping.SelectedIndex = (int)value;
					cBoxDio4Mapping.SelectedIndexChanged += new EventHandler(cBoxDio4Mapping_SelectedIndexChanged);
				}
				catch
				{
					cBoxDio4Mapping.SelectedIndexChanged += new EventHandler(cBoxDio4Mapping_SelectedIndexChanged);
				}
			}
		}

		public DioMappingEnum Dio5Mapping
		{
			get
			{
				return (DioMappingEnum)cBoxDio5Mapping.SelectedIndex;
			}
			set
			{
				try
				{
					cBoxDio5Mapping.SelectedIndexChanged -= new EventHandler(cBoxDio5Mapping_SelectedIndexChanged);
					cBoxDio5Mapping.SelectedIndex = (int)value;
					cBoxDio5Mapping.SelectedIndexChanged += new EventHandler(cBoxDio5Mapping_SelectedIndexChanged);
				}
				catch
				{
					cBoxDio5Mapping.SelectedIndexChanged += new EventHandler(cBoxDio5Mapping_SelectedIndexChanged);
				}
			}
		}

		public event DecimalEventHandler FrequencyXoChanged;

		public event BandEventHandler BandChanged;

		public event BooleanEventHandler ForceTxBandLowFrequencyOnChanged;

		public event BooleanEventHandler ForceRxBandLowFrequencyOnChanged;

		public event BooleanEventHandler LowFrequencyModeOnChanged;

		public event DecimalEventHandler FrequencyRfChanged;

		public event BooleanEventHandler FastHopOnChanged;

		public event PaModeEventHandler PaModeChanged;

		public event DecimalEventHandler OutputPowerChanged;

		public event DecimalEventHandler MaxOutputPowerChanged;

		public event PaRampEventHandler PaRampChanged;

		public event BooleanEventHandler OcpOnChanged;

		public event DecimalEventHandler OcpTrimChanged;

		public event BooleanEventHandler Pa20dBmChanged;

		public event DecimalEventHandler PllBandwidthChanged;

		public event Int32EventHandler AgcReferenceLevelChanged;

		public event AgcStepEventHandler AgcStepChanged;

		public event LnaGainEventHandler LnaGainChanged;

		public event BooleanEventHandler LnaBoostChanged;

		public event BooleanEventHandler AgcAutoOnChanged;

		public event BooleanEventHandler TcxoInputChanged;

		public event DioMappingEventHandler DioMappingChanged;

		public event DocumentationChangedEventHandler DocumentationChanged;

		public CommonViewControl()
		{
			InitializeComponent();
		}

		private void OnFrequencyXoChanged(Decimal value)
		{
			if (FrequencyXoChanged == null)
				return;
			FrequencyXoChanged((object)this, new DecimalEventArg(value));
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

		private void OnAgcReferenceLevelChanged(int value)
		{
			if (AgcReferenceLevelChanged == null)
				return;
			AgcReferenceLevelChanged((object)this, new Int32EventArg(value));
		}

		private void OnAgcStepChanged(byte id, byte value)
		{
			if (AgcStepChanged == null)
				return;
			AgcStepChanged((object)this, new AgcStepEventArg(id, value));
		}

		private void OnLnaGainChanged(LnaGainEnum value)
		{
			if (LnaGainChanged == null)
				return;
			LnaGainChanged((object)this, new LnaGainEventArg(value));
		}

		private void OnLnaBoostChanged(bool value)
		{
			if (LnaBoostChanged == null)
				return;
			LnaBoostChanged((object)this, new BooleanEventArg(value));
		}

		private void OnAgcAutoOnChanged(bool value)
		{
			if (AgcAutoOnChanged == null)
				return;
			AgcAutoOnChanged((object)this, new BooleanEventArg(value));
		}

		private void OnTcxoInputChanged(bool value)
		{
			if (TcxoInputChanged == null)
				return;
			TcxoInputChanged((object)this, new BooleanEventArg(value));
		}

		private void OnDioMappingChanged(byte id, DioMappingEnum value)
		{
			if (DioMappingChanged == null)
				return;
			DioMappingChanged((object)this, new DioMappingEventArg(id, value));
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

		private void nudFrequencyXo_ValueChanged(object sender, EventArgs e)
		{
			FrequencyXo = nudFrequencyXo.Value;
			OnFrequencyXoChanged(FrequencyXo);
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

		private void nudAgcReferenceLevel_ValueChanged(object sender, EventArgs e)
		{
			AgcReferenceLevel = (int)nudAgcReferenceLevel.Value;
			OnAgcReferenceLevelChanged(AgcReferenceLevel);
		}

		private void nudAgcStep_ValueChanged(object sender, EventArgs e)
		{
		}

		private void rBtnLnaGain_CheckedChanged(object sender, EventArgs e)
		{
			LnaGain = !rBtnLnaGain1.Checked ? (!rBtnLnaGain2.Checked ? (!rBtnLnaGain3.Checked ? (!rBtnLnaGain4.Checked ? (!rBtnLnaGain5.Checked ? (!rBtnLnaGain6.Checked ? LnaGainEnum.G1 : LnaGainEnum.G6) : LnaGainEnum.G5) : LnaGainEnum.G4) : LnaGainEnum.G3) : LnaGainEnum.G2) : LnaGainEnum.G1;
			OnLnaGainChanged(LnaGain);
		}

		private void rBtnLnaBoost_CheckedChanged(object sender, EventArgs e)
		{
			LnaBoost = rBtnLnaBoostOn.Checked;
			OnLnaBoostChanged(LnaBoost);
		}

		private void rBtnAgcAutoOn_CheckedChanged(object sender, EventArgs e)
		{
			AgcAutoOn = rBtnAgcAutoOn.Checked;
			OnAgcAutoOnChanged(AgcAutoOn);
		}

		private void rBtnTcxoInput_CheckedChanged(object sender, EventArgs e)
		{
			OnTcxoInputChanged(TcxoInputOn);
		}

		private void cBoxDio0Mapping_SelectedIndexChanged(object sender, EventArgs e)
		{
			OnDioMappingChanged((byte)0, (DioMappingEnum)cBoxDio0Mapping.SelectedIndex);
		}

		private void cBoxDio1Mapping_SelectedIndexChanged(object sender, EventArgs e)
		{
			OnDioMappingChanged((byte)1, (DioMappingEnum)cBoxDio1Mapping.SelectedIndex);
		}

		private void cBoxDio2Mapping_SelectedIndexChanged(object sender, EventArgs e)
		{
			OnDioMappingChanged((byte)2, (DioMappingEnum)cBoxDio2Mapping.SelectedIndex);
		}

		private void cBoxDio3Mapping_SelectedIndexChanged(object sender, EventArgs e)
		{
			OnDioMappingChanged((byte)3, (DioMappingEnum)cBoxDio3Mapping.SelectedIndex);
		}

		private void cBoxDio4Mapping_SelectedIndexChanged(object sender, EventArgs e)
		{
			OnDioMappingChanged((byte)4, (DioMappingEnum)cBoxDio4Mapping.SelectedIndex);
		}

		private void cBoxDio5Mapping_SelectedIndexChanged(object sender, EventArgs e)
		{
			OnDioMappingChanged((byte)5, (DioMappingEnum)cBoxDio5Mapping.SelectedIndex);
		}

		private void control_MouseEnter(object sender, EventArgs e)
		{
			if (sender == gBoxGeneral)
				OnDocumentationChanged(new DocumentationChangedEventArgs("Common", "General"));
			if (sender == gBoxTxSettings)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Transmitter", "Tx Settings"));
			}
			else
			{
				if (sender != gBoxRxSettings)
					return;
				OnDocumentationChanged(new DocumentationChangedEventArgs("Receiver", "Rx Settings"));
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
			this.lblListenResolRx = new Label();
			this.label30 = new Label();
			this.groupBox1 = new GroupBox();
			this.tableLayoutPanel1 = new TableLayoutPanel();
			this.cBoxDio4Mapping = new ComboBox();
			this.cBoxDio3Mapping = new ComboBox();
			this.cBoxDio2Mapping = new ComboBox();
			this.cBoxDio1Mapping = new ComboBox();
			this.cBoxDio0Mapping = new ComboBox();
			this.label54 = new Label();
			this.label55 = new Label();
			this.label56 = new Label();
			this.label57 = new Label();
			this.label58 = new Label();
			this.label35 = new Label();
			this.cBoxDio5Mapping = new ComboBox();
			this.gBoxRxSettings = new GroupBoxEx();
			this.panel12 = new Panel();
			this.panel9 = new Panel();
			this.rBtnAgcAutoOff = new RadioButton();
			this.rBtnAgcAutoOn = new RadioButton();
			this.label17 = new Label();
			this.label2 = new Label();
			this.panel3 = new Panel();
			this.rBtnLnaBoostOff = new RadioButton();
			this.rBtnLnaBoostOn = new RadioButton();
			this.label4 = new Label();
			this.lblAgcReference = new Label();
			this.label48 = new Label();
			this.label49 = new Label();
			this.label50 = new Label();
			this.label51 = new Label();
			this.label52 = new Label();
			this.lblLnaGain1 = new Label();
			this.label53 = new Label();
			this.panel2 = new Panel();
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
			this.gBoxAgc = new GroupBoxEx();
			this.label15 = new Label();
			this.label16 = new Label();
			this.label29 = new Label();
			this.label31 = new Label();
			this.label32 = new Label();
			this.label33 = new Label();
			this.label34 = new Label();
			this.label46 = new Label();
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
			this.gBoxTxSettings = new GroupBoxEx();
			this.pnlPa20dBm = new Panel();
			this.rBtnPa20dBmOff = new RadioButton();
			this.rBtnPa20dBmOn = new RadioButton();
			this.lblPa20dBm = new Label();
			this.nudMaxOutputPower = new NumericUpDownEx();
			this.label7 = new Label();
			this.label6 = new Label();
			this.nudPllBandwidth = new NumericUpDownEx();
			this.panel4 = new Panel();
			this.rBtnOcpOff = new RadioButton();
			this.rBtnOcpOn = new RadioButton();
			this.label19 = new Label();
			this.label10 = new Label();
			this.label5 = new Label();
			this.label8 = new Label();
			this.nudOcpTrim = new NumericUpDownEx();
			this.suffixOCPtrim = new Label();
			this.label18 = new Label();
			this.cBoxPaRamp = new ComboBox();
			this.pnlPaSelect = new Panel();
			this.rBtnRfPa = new RadioButton();
			this.rBtnRfo = new RadioButton();
			this.nudOutputPower = new NumericUpDownEx();
			this.suffixOutputPower = new Label();
			this.suffixPAramp = new Label();
			this.label12 = new Label();
			this.gBoxGeneral = new GroupBoxEx();
			this.gBoxOptioanl = new GroupBoxEx();
			this.panel10 = new Panel();
			this.rBtnForceRxBandLowFrequencyOff = new RadioButton();
			this.rBtnForceRxBandLowFrequencyOn = new RadioButton();
			this.label38 = new Label();
			this.cBoxBand = new ComboBox();
			this.panel8 = new Panel();
			this.rBtnLowFrequencyModeOff = new RadioButton();
			this.rBtnLowFrequencyModeOn = new RadioButton();
			this.label36 = new Label();
			this.label37 = new Label();
			this.label11 = new Label();
			this.panel11 = new Panel();
			this.rBtnForceTxBandLowFrequencyOff = new RadioButton();
			this.rBtnForceTxBandLowFrequencyOn = new RadioButton();
			this.panel5 = new Panel();
			this.rBtnFastHopOff = new RadioButton();
			this.rBtnFastHopOn = new RadioButton();
			this.label39 = new Label();
			this.label3 = new Label();
			this.panel1 = new Panel();
			this.rBtnTcxoInputOff = new RadioButton();
			this.rBtnTcxoInputOn = new RadioButton();
			this.nudFrequencyXo = new NumericUpDownEx();
			this.label9 = new Label();
			this.label1 = new Label();
			this.label13 = new Label();
			this.lblRcOscillatorCalStat = new Label();
			this.label14 = new Label();
			this.nudFrequencyRf = new NumericUpDownEx();
			((ISupportInitialize)this.errorProvider).BeginInit();
			this.groupBox1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.gBoxRxSettings.SuspendLayout();
			this.panel12.SuspendLayout();
			this.panel9.SuspendLayout();
			this.panel3.SuspendLayout();
			this.panel2.SuspendLayout();
			this.gBoxAgc.SuspendLayout();
			this.nudAgcStep5.BeginInit();
			this.nudAgcStep4.BeginInit();
			this.nudAgcReferenceLevel.BeginInit();
			this.nudAgcStep3.BeginInit();
			this.nudAgcStep1.BeginInit();
			this.nudAgcStep2.BeginInit();
			this.gBoxTxSettings.SuspendLayout();
			this.pnlPa20dBm.SuspendLayout();
			this.nudMaxOutputPower.BeginInit();
			this.nudPllBandwidth.BeginInit();
			this.panel4.SuspendLayout();
			this.nudOcpTrim.BeginInit();
			this.pnlPaSelect.SuspendLayout();
			this.nudOutputPower.BeginInit();
			this.gBoxGeneral.SuspendLayout();
			this.gBoxOptioanl.SuspendLayout();
			this.panel10.SuspendLayout();
			this.panel8.SuspendLayout();
			this.panel11.SuspendLayout();
			this.panel5.SuspendLayout();
			this.panel1.SuspendLayout();
			this.nudFrequencyXo.BeginInit();
			this.nudFrequencyRf.BeginInit();
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
			this.groupBox1.Controls.Add((Control)this.tableLayoutPanel1);
			this.groupBox1.Location = new Point(3, 402);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new Size(793, 81);
			this.groupBox1.TabIndex = 14;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "DIO mapping";
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
			this.tableLayoutPanel1.ColumnCount = 6;
			this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667f));
			this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667f));
			this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667f));
			this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667f));
			this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667f));
			this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667f));
			this.tableLayoutPanel1.Controls.Add((Control)this.cBoxDio4Mapping, 1, 1);
			this.tableLayoutPanel1.Controls.Add((Control)this.cBoxDio3Mapping, 2, 1);
			this.tableLayoutPanel1.Controls.Add((Control)this.cBoxDio2Mapping, 3, 1);
			this.tableLayoutPanel1.Controls.Add((Control)this.cBoxDio1Mapping, 4, 1);
			this.tableLayoutPanel1.Controls.Add((Control)this.cBoxDio0Mapping, 5, 1);
			this.tableLayoutPanel1.Controls.Add((Control)this.label54, 5, 0);
			this.tableLayoutPanel1.Controls.Add((Control)this.label55, 4, 0);
			this.tableLayoutPanel1.Controls.Add((Control)this.label56, 3, 0);
			this.tableLayoutPanel1.Controls.Add((Control)this.label57, 2, 0);
			this.tableLayoutPanel1.Controls.Add((Control)this.label58, 1, 0);
			this.tableLayoutPanel1.Controls.Add((Control)this.label35, 0, 0);
			this.tableLayoutPanel1.Controls.Add((Control)this.cBoxDio5Mapping, 0, 1);
			this.tableLayoutPanel1.Location = new Point(6, 19);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel1.Size = new Size(781, 49);
			this.tableLayoutPanel1.TabIndex = 29;
			this.cBoxDio4Mapping.Anchor = AnchorStyles.None;
			this.cBoxDio4Mapping.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxDio4Mapping.FormattingEnabled = true;
			this.cBoxDio4Mapping.Items.AddRange(new object[4]
      {
        (object) "CadDetected",
        (object) "PllLock",
        (object) "PllLock",
        (object) "-"
      });
			this.cBoxDio4Mapping.Location = new Point(133, 24);
			this.cBoxDio4Mapping.Name = "cBoxDio4Mapping";
			this.cBoxDio4Mapping.Size = new Size(122, 21);
			this.cBoxDio4Mapping.TabIndex = 0;
			this.cBoxDio4Mapping.SelectedIndexChanged += new EventHandler(this.cBoxDio4Mapping_SelectedIndexChanged);
			this.cBoxDio3Mapping.Anchor = AnchorStyles.None;
			this.cBoxDio3Mapping.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxDio3Mapping.FormattingEnabled = true;
			this.cBoxDio3Mapping.Items.AddRange(new object[4]
      {
        (object) "CadDone",
        (object) "ValidHeader",
        (object) "PayloadCrcError",
        (object) "-"
      });
			this.cBoxDio3Mapping.Location = new Point(262, 24);
			this.cBoxDio3Mapping.Name = "cBoxDio3Mapping";
			this.cBoxDio3Mapping.Size = new Size(122, 21);
			this.cBoxDio3Mapping.TabIndex = 0;
			this.cBoxDio3Mapping.SelectedIndexChanged += new EventHandler(this.cBoxDio3Mapping_SelectedIndexChanged);
			this.cBoxDio2Mapping.Anchor = AnchorStyles.None;
			this.cBoxDio2Mapping.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxDio2Mapping.FormattingEnabled = true;
			this.cBoxDio2Mapping.Items.AddRange(new object[4]
      {
        (object) "FhssChangeChannel",
        (object) "FhssChangeChannel",
        (object) "FhssChangeChannel",
        (object) "-"
      });
			this.cBoxDio2Mapping.Location = new Point(391, 24);
			this.cBoxDio2Mapping.Name = "cBoxDio2Mapping";
			this.cBoxDio2Mapping.Size = new Size(122, 21);
			this.cBoxDio2Mapping.TabIndex = 0;
			this.cBoxDio2Mapping.SelectedIndexChanged += new EventHandler(this.cBoxDio2Mapping_SelectedIndexChanged);
			this.cBoxDio1Mapping.Anchor = AnchorStyles.None;
			this.cBoxDio1Mapping.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxDio1Mapping.FormattingEnabled = true;
			this.cBoxDio1Mapping.Items.AddRange(new object[4]
      {
        (object) "RxTimeout",
        (object) "FhssChangeChannel",
        (object) "CadDetected",
        (object) "-"
      });
			this.cBoxDio1Mapping.Location = new Point(520, 24);
			this.cBoxDio1Mapping.Name = "cBoxDio1Mapping";
			this.cBoxDio1Mapping.Size = new Size(122, 21);
			this.cBoxDio1Mapping.TabIndex = 0;
			this.cBoxDio1Mapping.SelectedIndexChanged += new EventHandler(this.cBoxDio1Mapping_SelectedIndexChanged);
			this.cBoxDio0Mapping.Anchor = AnchorStyles.None;
			this.cBoxDio0Mapping.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxDio0Mapping.FormattingEnabled = true;
			this.cBoxDio0Mapping.Items.AddRange(new object[4]
      {
        (object) "RxDone",
        (object) "TxDone",
        (object) "CadDone",
        (object) "-"
      });
			this.cBoxDio0Mapping.Location = new Point(651, 24);
			this.cBoxDio0Mapping.Name = "cBoxDio0Mapping";
			this.cBoxDio0Mapping.Size = new Size(123, 21);
			this.cBoxDio0Mapping.TabIndex = 0;
			this.cBoxDio0Mapping.SelectedIndexChanged += new EventHandler(this.cBoxDio0Mapping_SelectedIndexChanged);
			this.label54.Anchor = AnchorStyles.None;
			this.label54.AutoSize = true;
			this.label54.Location = new Point(697, 4);
			this.label54.Margin = new Padding(3);
			this.label54.Name = "label54";
			this.label54.Size = new Size(32, 13);
			this.label54.TabIndex = 1;
			this.label54.Text = "DIO0";
			this.label54.TextAlign = ContentAlignment.MiddleCenter;
			this.label55.Anchor = AnchorStyles.None;
			this.label55.AutoSize = true;
			this.label55.Location = new Point(565, 4);
			this.label55.Margin = new Padding(3);
			this.label55.Name = "label55";
			this.label55.Size = new Size(32, 13);
			this.label55.TabIndex = 1;
			this.label55.Text = "DIO1";
			this.label55.TextAlign = ContentAlignment.MiddleCenter;
			this.label56.Anchor = AnchorStyles.None;
			this.label56.AutoSize = true;
			this.label56.Location = new Point(436, 4);
			this.label56.Margin = new Padding(3);
			this.label56.Name = "label56";
			this.label56.Size = new Size(32, 13);
			this.label56.TabIndex = 1;
			this.label56.Text = "DIO2";
			this.label56.TextAlign = ContentAlignment.MiddleCenter;
			this.label57.Anchor = AnchorStyles.None;
			this.label57.AutoSize = true;
			this.label57.Location = new Point(307, 4);
			this.label57.Margin = new Padding(3);
			this.label57.Name = "label57";
			this.label57.Size = new Size(32, 13);
			this.label57.TabIndex = 1;
			this.label57.Text = "DIO3";
			this.label57.TextAlign = ContentAlignment.MiddleCenter;
			this.label58.Anchor = AnchorStyles.None;
			this.label58.AutoSize = true;
			this.label58.Location = new Point(178, 4);
			this.label58.Margin = new Padding(3);
			this.label58.Name = "label58";
			this.label58.Size = new Size(32, 13);
			this.label58.TabIndex = 1;
			this.label58.Text = "DIO4";
			this.label58.TextAlign = ContentAlignment.MiddleCenter;
			this.label35.Anchor = AnchorStyles.None;
			this.label35.AutoSize = true;
			this.label35.Location = new Point(49, 4);
			this.label35.Margin = new Padding(3);
			this.label35.Name = "label35";
			this.label35.Size = new Size(32, 13);
			this.label35.TabIndex = 1;
			this.label35.Text = "DIO5";
			this.label35.TextAlign = ContentAlignment.MiddleCenter;
			this.cBoxDio5Mapping.Anchor = AnchorStyles.None;
			this.cBoxDio5Mapping.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxDio5Mapping.FormattingEnabled = true;
			this.cBoxDio5Mapping.Items.AddRange(new object[4]
      {
        (object) "ModeReady",
        (object) "ClkOut",
        (object) "ClkOut",
        (object) "-"
      });
			this.cBoxDio5Mapping.Location = new Point(4, 24);
			this.cBoxDio5Mapping.Name = "cBoxDio5Mapping";
			this.cBoxDio5Mapping.Size = new Size(122, 21);
			this.cBoxDio5Mapping.TabIndex = 0;
			this.cBoxDio5Mapping.SelectedIndexChanged += new EventHandler(this.cBoxDio5Mapping_SelectedIndexChanged);
			this.gBoxRxSettings.Controls.Add((Control)this.panel12);
			this.gBoxRxSettings.Controls.Add((Control)this.label4);
			this.gBoxRxSettings.Controls.Add((Control)this.lblAgcReference);
			this.gBoxRxSettings.Controls.Add((Control)this.label48);
			this.gBoxRxSettings.Controls.Add((Control)this.label49);
			this.gBoxRxSettings.Controls.Add((Control)this.label50);
			this.gBoxRxSettings.Controls.Add((Control)this.label51);
			this.gBoxRxSettings.Controls.Add((Control)this.label52);
			this.gBoxRxSettings.Controls.Add((Control)this.lblLnaGain1);
			this.gBoxRxSettings.Controls.Add((Control)this.label53);
			this.gBoxRxSettings.Controls.Add((Control)this.panel2);
			this.gBoxRxSettings.Controls.Add((Control)this.lblLnaGain2);
			this.gBoxRxSettings.Controls.Add((Control)this.lblLnaGain3);
			this.gBoxRxSettings.Controls.Add((Control)this.lblLnaGain4);
			this.gBoxRxSettings.Controls.Add((Control)this.lblLnaGain5);
			this.gBoxRxSettings.Controls.Add((Control)this.lblLnaGain6);
			this.gBoxRxSettings.Controls.Add((Control)this.lblAgcThresh1);
			this.gBoxRxSettings.Controls.Add((Control)this.lblAgcThresh2);
			this.gBoxRxSettings.Controls.Add((Control)this.lblAgcThresh3);
			this.gBoxRxSettings.Controls.Add((Control)this.lblAgcThresh4);
			this.gBoxRxSettings.Controls.Add((Control)this.lblAgcThresh5);
			this.gBoxRxSettings.Controls.Add((Control)this.label47);
			this.gBoxRxSettings.Location = new Point(3, 293);
			this.gBoxRxSettings.Name = "gBoxRxSettings";
			this.gBoxRxSettings.Size = new Size(793, 103);
			this.gBoxRxSettings.TabIndex = 8;
			this.gBoxRxSettings.TabStop = false;
			this.gBoxRxSettings.Text = "Rx settings";
			this.panel12.AutoSize = true;
			this.panel12.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel12.Controls.Add((Control)this.panel9);
			this.panel12.Controls.Add((Control)this.label17);
			this.panel12.Controls.Add((Control)this.label2);
			this.panel12.Controls.Add((Control)this.panel3);
			this.panel12.Location = new Point(264, 28);
			this.panel12.Name = "panel12";
			this.panel12.Size = new Size(264, 46);
			this.panel12.TabIndex = 7;
			this.panel9.AutoSize = true;
			this.panel9.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel9.Controls.Add((Control)this.rBtnAgcAutoOff);
			this.panel9.Controls.Add((Control)this.rBtnAgcAutoOn);
			this.panel9.Location = new Point(163, 3);
			this.panel9.Name = "panel9";
			this.panel9.Size = new Size(98, 17);
			this.panel9.TabIndex = 23;
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
			this.label17.AutoSize = true;
			this.label17.Location = new Point(3, 5);
			this.label17.Name = "label17";
			this.label17.Size = new Size(56, 13);
			this.label17.TabIndex = 22;
			this.label17.Text = "AGC auto:";
			this.label2.AutoSize = true;
			this.label2.Location = new Point(3, 28);
			this.label2.Name = "label2";
			this.label2.Size = new Size(60, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "LNA boost:";
			this.panel3.AutoSize = true;
			this.panel3.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel3.Controls.Add((Control)this.rBtnLnaBoostOff);
			this.panel3.Controls.Add((Control)this.rBtnLnaBoostOn);
			this.panel3.Location = new Point(163, 26);
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
			this.label4.BackColor = Color.Transparent;
			this.label4.Location = new Point(167, 75);
			this.label4.Name = "label4";
			this.label4.Size = new Size(42, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "Gain";
			this.label4.TextAlign = ContentAlignment.MiddleCenter;
			this.label4.Visible = false;
			this.lblAgcReference.BackColor = Color.Transparent;
			this.lblAgcReference.Location = new Point(124, 32);
			this.lblAgcReference.Margin = new Padding(0, 0, 0, 3);
			this.lblAgcReference.Name = "lblAgcReference";
			this.lblAgcReference.Size = new Size(100, 13);
			this.lblAgcReference.TabIndex = 7;
			this.lblAgcReference.Text = "-80";
			this.lblAgcReference.TextAlign = ContentAlignment.MiddleCenter;
			this.lblAgcReference.Visible = false;
			this.label48.BackColor = Color.Transparent;
			this.label48.Location = new Point(124, 16);
			this.label48.Margin = new Padding(0, 0, 0, 3);
			this.label48.Name = "label48";
			this.label48.Size = new Size(100, 13);
			this.label48.TabIndex = 0;
			this.label48.Text = "Reference";
			this.label48.TextAlign = ContentAlignment.MiddleCenter;
			this.label48.Visible = false;
			this.label49.BackColor = Color.Transparent;
			this.label49.Location = new Point(224, 16);
			this.label49.Margin = new Padding(0, 0, 0, 3);
			this.label49.Name = "label49";
			this.label49.Size = new Size(100, 13);
			this.label49.TabIndex = 1;
			this.label49.Text = "Threshold 1";
			this.label49.TextAlign = ContentAlignment.MiddleCenter;
			this.label49.Visible = false;
			this.label50.BackColor = Color.Transparent;
			this.label50.Location = new Point(324, 16);
			this.label50.Margin = new Padding(0, 0, 0, 3);
			this.label50.Name = "label50";
			this.label50.Size = new Size(100, 13);
			this.label50.TabIndex = 2;
			this.label50.Text = "Threshold 2";
			this.label50.TextAlign = ContentAlignment.MiddleCenter;
			this.label50.Visible = false;
			this.label51.BackColor = Color.Transparent;
			this.label51.Location = new Point(424, 16);
			this.label51.Margin = new Padding(0, 0, 0, 3);
			this.label51.Name = "label51";
			this.label51.Size = new Size(100, 13);
			this.label51.TabIndex = 3;
			this.label51.Text = "Threshold 3";
			this.label51.TextAlign = ContentAlignment.MiddleCenter;
			this.label51.Visible = false;
			this.label52.BackColor = Color.Transparent;
			this.label52.Location = new Point(524, 16);
			this.label52.Margin = new Padding(0, 0, 0, 3);
			this.label52.Name = "label52";
			this.label52.Size = new Size(100, 13);
			this.label52.TabIndex = 4;
			this.label52.Text = "Threshold 4";
			this.label52.TextAlign = ContentAlignment.MiddleCenter;
			this.label52.Visible = false;
			this.lblLnaGain1.BackColor = Color.LightSteelBlue;
			this.lblLnaGain1.BorderStyle = BorderStyle.Fixed3D;
			this.lblLnaGain1.Location = new Point(174, 48);
			this.lblLnaGain1.Margin = new Padding(0, 0, 0, 3);
			this.lblLnaGain1.Name = "lblLnaGain1";
			this.lblLnaGain1.Size = new Size(100, 20);
			this.lblLnaGain1.TabIndex = 14;
			this.lblLnaGain1.Text = "G1";
			this.lblLnaGain1.TextAlign = ContentAlignment.MiddleCenter;
			this.lblLnaGain1.Visible = false;
			this.label53.BackColor = Color.Transparent;
			this.label53.Location = new Point(624, 16);
			this.label53.Margin = new Padding(0, 0, 0, 3);
			this.label53.Name = "label53";
			this.label53.Size = new Size(100, 13);
			this.label53.TabIndex = 5;
			this.label53.Text = "Threshold 5";
			this.label53.TextAlign = ContentAlignment.MiddleCenter;
			this.label53.Visible = false;
			this.panel2.AutoSize = true;
			this.panel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel2.Controls.Add((Control)this.rBtnLnaGain1);
			this.panel2.Controls.Add((Control)this.rBtnLnaGain2);
			this.panel2.Controls.Add((Control)this.rBtnLnaGain3);
			this.panel2.Controls.Add((Control)this.rBtnLnaGain4);
			this.panel2.Controls.Add((Control)this.rBtnLnaGain5);
			this.panel2.Controls.Add((Control)this.rBtnLnaGain6);
			this.panel2.Location = new Point(215, 75);
			this.panel2.Name = "panel2";
			this.panel2.Size = new Size(521, 13);
			this.panel2.TabIndex = 22;
			this.panel2.Visible = false;
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
			this.lblLnaGain2.Visible = false;
			this.lblLnaGain3.BackColor = Color.Transparent;
			this.lblLnaGain3.BorderStyle = BorderStyle.Fixed3D;
			this.lblLnaGain3.Location = new Point(374, 48);
			this.lblLnaGain3.Margin = new Padding(0, 0, 0, 3);
			this.lblLnaGain3.Name = "lblLnaGain3";
			this.lblLnaGain3.Size = new Size(100, 20);
			this.lblLnaGain3.TabIndex = 16;
			this.lblLnaGain3.Text = "G3";
			this.lblLnaGain3.TextAlign = ContentAlignment.MiddleCenter;
			this.lblLnaGain3.Visible = false;
			this.lblLnaGain4.BackColor = Color.Transparent;
			this.lblLnaGain4.BorderStyle = BorderStyle.Fixed3D;
			this.lblLnaGain4.Location = new Point(474, 48);
			this.lblLnaGain4.Margin = new Padding(0, 0, 0, 3);
			this.lblLnaGain4.Name = "lblLnaGain4";
			this.lblLnaGain4.Size = new Size(100, 20);
			this.lblLnaGain4.TabIndex = 17;
			this.lblLnaGain4.Text = "G4";
			this.lblLnaGain4.TextAlign = ContentAlignment.MiddleCenter;
			this.lblLnaGain4.Visible = false;
			this.lblLnaGain5.BackColor = Color.Transparent;
			this.lblLnaGain5.BorderStyle = BorderStyle.Fixed3D;
			this.lblLnaGain5.Location = new Point(574, 48);
			this.lblLnaGain5.Margin = new Padding(0, 0, 0, 3);
			this.lblLnaGain5.Name = "lblLnaGain5";
			this.lblLnaGain5.Size = new Size(100, 20);
			this.lblLnaGain5.TabIndex = 18;
			this.lblLnaGain5.Text = "G5";
			this.lblLnaGain5.TextAlign = ContentAlignment.MiddleCenter;
			this.lblLnaGain5.Visible = false;
			this.lblLnaGain6.BackColor = Color.Transparent;
			this.lblLnaGain6.BorderStyle = BorderStyle.Fixed3D;
			this.lblLnaGain6.Location = new Point(674, 48);
			this.lblLnaGain6.Margin = new Padding(0, 0, 0, 3);
			this.lblLnaGain6.Name = "lblLnaGain6";
			this.lblLnaGain6.Size = new Size(100, 20);
			this.lblLnaGain6.TabIndex = 19;
			this.lblLnaGain6.Text = "G6";
			this.lblLnaGain6.TextAlign = ContentAlignment.MiddleCenter;
			this.lblLnaGain6.Visible = false;
			this.lblAgcThresh1.BackColor = Color.Transparent;
			this.lblAgcThresh1.Location = new Point(224, 32);
			this.lblAgcThresh1.Margin = new Padding(0, 0, 0, 3);
			this.lblAgcThresh1.Name = "lblAgcThresh1";
			this.lblAgcThresh1.Size = new Size(100, 13);
			this.lblAgcThresh1.TabIndex = 8;
			this.lblAgcThresh1.Text = "0";
			this.lblAgcThresh1.TextAlign = ContentAlignment.MiddleCenter;
			this.lblAgcThresh1.Visible = false;
			this.lblAgcThresh2.BackColor = Color.Transparent;
			this.lblAgcThresh2.Location = new Point(324, 32);
			this.lblAgcThresh2.Margin = new Padding(0, 0, 0, 3);
			this.lblAgcThresh2.Name = "lblAgcThresh2";
			this.lblAgcThresh2.Size = new Size(100, 13);
			this.lblAgcThresh2.TabIndex = 9;
			this.lblAgcThresh2.Text = "0";
			this.lblAgcThresh2.TextAlign = ContentAlignment.MiddleCenter;
			this.lblAgcThresh2.Visible = false;
			this.lblAgcThresh3.BackColor = Color.Transparent;
			this.lblAgcThresh3.Location = new Point(424, 32);
			this.lblAgcThresh3.Margin = new Padding(0, 0, 0, 3);
			this.lblAgcThresh3.Name = "lblAgcThresh3";
			this.lblAgcThresh3.Size = new Size(100, 13);
			this.lblAgcThresh3.TabIndex = 10;
			this.lblAgcThresh3.Text = "0";
			this.lblAgcThresh3.TextAlign = ContentAlignment.MiddleCenter;
			this.lblAgcThresh3.Visible = false;
			this.lblAgcThresh4.BackColor = Color.Transparent;
			this.lblAgcThresh4.Location = new Point(524, 32);
			this.lblAgcThresh4.Margin = new Padding(0, 0, 0, 3);
			this.lblAgcThresh4.Name = "lblAgcThresh4";
			this.lblAgcThresh4.Size = new Size(100, 13);
			this.lblAgcThresh4.TabIndex = 11;
			this.lblAgcThresh4.Text = "0";
			this.lblAgcThresh4.TextAlign = ContentAlignment.MiddleCenter;
			this.lblAgcThresh4.Visible = false;
			this.lblAgcThresh5.BackColor = Color.Transparent;
			this.lblAgcThresh5.Location = new Point(624, 32);
			this.lblAgcThresh5.Margin = new Padding(0, 0, 0, 3);
			this.lblAgcThresh5.Name = "lblAgcThresh5";
			this.lblAgcThresh5.Size = new Size(100, 13);
			this.lblAgcThresh5.TabIndex = 12;
			this.lblAgcThresh5.Text = "0";
			this.lblAgcThresh5.TextAlign = ContentAlignment.MiddleCenter;
			this.lblAgcThresh5.Visible = false;
			this.label47.AutoSize = true;
			this.label47.BackColor = Color.Transparent;
			this.label47.Location = new Point(723, 32);
			this.label47.Margin = new Padding(0);
			this.label47.Name = "label47";
			this.label47.Size = new Size(64, 13);
			this.label47.TabIndex = 13;
			this.label47.Text = "-> Pin [dBm]";
			this.label47.TextAlign = ContentAlignment.MiddleLeft;
			this.label47.Visible = false;
			this.gBoxAgc.Controls.Add((Control)this.label15);
			this.gBoxAgc.Controls.Add((Control)this.label16);
			this.gBoxAgc.Controls.Add((Control)this.label29);
			this.gBoxAgc.Controls.Add((Control)this.label31);
			this.gBoxAgc.Controls.Add((Control)this.label32);
			this.gBoxAgc.Controls.Add((Control)this.label33);
			this.gBoxAgc.Controls.Add((Control)this.label34);
			this.gBoxAgc.Controls.Add((Control)this.label46);
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
			this.gBoxAgc.Location = new Point(3, 489);
			this.gBoxAgc.Name = "gBoxAgc";
			this.gBoxAgc.Size = new Size(515, 98);
			this.gBoxAgc.TabIndex = 13;
			this.gBoxAgc.TabStop = false;
			this.gBoxAgc.Text = "AGC";
			this.gBoxAgc.Visible = false;
			this.label15.AutoSize = true;
			this.label15.BackColor = Color.Transparent;
			this.label15.Location = new Point(2, 23);
			this.label15.Name = "label15";
			this.label15.Size = new Size(89, 13);
			this.label15.TabIndex = 2;
			this.label15.Text = "Reference Level:";
			this.label16.AutoSize = true;
			this.label16.BackColor = Color.Transparent;
			this.label16.Location = new Point(244, 23);
			this.label16.Name = "label16";
			this.label16.Size = new Size(89, 13);
			this.label16.TabIndex = 8;
			this.label16.Text = "Threshold step 1:";
			this.label29.AutoSize = true;
			this.label29.BackColor = Color.Transparent;
			this.label29.Location = new Point(2, 49);
			this.label29.Name = "label29";
			this.label29.Size = new Size(89, 13);
			this.label29.TabIndex = 11;
			this.label29.Text = "Threshold step 2:";
			this.label31.AutoSize = true;
			this.label31.BackColor = Color.Transparent;
			this.label31.Location = new Point(244, 49);
			this.label31.Name = "label31";
			this.label31.Size = new Size(89, 13);
			this.label31.TabIndex = 14;
			this.label31.Text = "Threshold step 3:";
			this.label32.AutoSize = true;
			this.label32.BackColor = Color.Transparent;
			this.label32.Location = new Point(2, 75);
			this.label32.Name = "label32";
			this.label32.Size = new Size(89, 13);
			this.label32.TabIndex = 17;
			this.label32.Text = "Threshold step 4:";
			this.label33.AutoSize = true;
			this.label33.BackColor = Color.Transparent;
			this.label33.Location = new Point(244, 75);
			this.label33.Name = "label33";
			this.label33.Size = new Size(89, 13);
			this.label33.TabIndex = 20;
			this.label33.Text = "Threshold step 5:";
			this.label34.AutoSize = true;
			this.label34.BackColor = Color.Transparent;
			this.label34.Location = new Point(223, 23);
			this.label34.Name = "label34";
			this.label34.Size = new Size(20, 13);
			this.label34.TabIndex = 4;
			this.label34.Text = "dB";
			this.label46.AutoSize = true;
			this.label46.BackColor = Color.Transparent;
			this.label46.Location = new Point(465, 23);
			this.label46.Name = "label46";
			this.label46.Size = new Size(20, 13);
			this.label46.TabIndex = 10;
			this.label46.Text = "dB";
			this.label59.AutoSize = true;
			this.label59.BackColor = Color.Transparent;
			this.label59.Location = new Point(223, 49);
			this.label59.Name = "label59";
			this.label59.Size = new Size(20, 13);
			this.label59.TabIndex = 13;
			this.label59.Text = "dB";
			this.label60.AutoSize = true;
			this.label60.BackColor = Color.Transparent;
			this.label60.Location = new Point(465, 49);
			this.label60.Name = "label60";
			this.label60.Size = new Size(20, 13);
			this.label60.TabIndex = 16;
			this.label60.Text = "dB";
			this.label61.AutoSize = true;
			this.label61.BackColor = Color.Transparent;
			this.label61.Location = new Point(223, 75);
			this.label61.Name = "label61";
			this.label61.Size = new Size(20, 13);
			this.label61.TabIndex = 19;
			this.label61.Text = "dB";
			this.label62.AutoSize = true;
			this.label62.BackColor = Color.Transparent;
			this.label62.Location = new Point(465, 75);
			this.label62.Name = "label62";
			this.label62.Size = new Size(20, 13);
			this.label62.TabIndex = 22;
			this.label62.Text = "dB";
			this.nudAgcStep5.Location = new Point(361, 71);
			NumericUpDown numericUpDown1 = this.nudAgcStep5;
			int[] bits1 = new int[4];
			bits1[0] = 15;
			Decimal num1 = new Decimal(bits1);
			numericUpDown1.Maximum = num1;
			this.nudAgcStep5.Name = "nudAgcStep5";
			this.nudAgcStep5.Size = new Size(98, 20);
			this.nudAgcStep5.TabIndex = 21;
			NumericUpDown numericUpDown2 = this.nudAgcStep5;
			int[] bits2 = new int[4];
			bits2[0] = 11;
			Decimal num2 = new Decimal(bits2);
			numericUpDown2.Value = num2;
			this.nudAgcStep5.ValueChanged += new EventHandler(this.nudAgcStep_ValueChanged);
			this.nudAgcStep4.Location = new Point(119, 71);
			NumericUpDown numericUpDown3 = this.nudAgcStep4;
			int[] bits3 = new int[4];
			bits3[0] = 15;
			Decimal num3 = new Decimal(bits3);
			numericUpDown3.Maximum = num3;
			this.nudAgcStep4.Name = "nudAgcStep4";
			this.nudAgcStep4.Size = new Size(98, 20);
			this.nudAgcStep4.TabIndex = 18;
			NumericUpDown numericUpDown4 = this.nudAgcStep4;
			int[] bits4 = new int[4];
			bits4[0] = 9;
			Decimal num4 = new Decimal(bits4);
			numericUpDown4.Value = num4;
			this.nudAgcStep4.ValueChanged += new EventHandler(this.nudAgcStep_ValueChanged);
			this.nudAgcReferenceLevel.Location = new Point(119, 19);
			NumericUpDownEx numericUpDownEx1 = this.nudAgcReferenceLevel;
			int[] bits5 = new int[4];
			bits5[0] = 63;
			Decimal num5 = new Decimal(bits5);
			numericUpDownEx1.Maximum = num5;
			this.nudAgcReferenceLevel.Name = "nudAgcReferenceLevel";
			this.nudAgcReferenceLevel.Size = new Size(98, 20);
			this.nudAgcReferenceLevel.TabIndex = 3;
			this.nudAgcReferenceLevel.ThousandsSeparator = true;
			NumericUpDownEx numericUpDownEx2 = this.nudAgcReferenceLevel;
			int[] bits6 = new int[4];
			bits6[0] = 19;
			Decimal num6 = new Decimal(bits6);
			numericUpDownEx2.Value = num6;
			this.nudAgcReferenceLevel.ValueChanged += new EventHandler(this.nudAgcReferenceLevel_ValueChanged);
			this.nudAgcStep3.Location = new Point(361, 45);
			NumericUpDown numericUpDown5 = this.nudAgcStep3;
			int[] bits7 = new int[4];
			bits7[0] = 15;
			Decimal num7 = new Decimal(bits7);
			numericUpDown5.Maximum = num7;
			this.nudAgcStep3.Name = "nudAgcStep3";
			this.nudAgcStep3.Size = new Size(98, 20);
			this.nudAgcStep3.TabIndex = 15;
			NumericUpDown numericUpDown6 = this.nudAgcStep3;
			int[] bits8 = new int[4];
			bits8[0] = 11;
			Decimal num8 = new Decimal(bits8);
			numericUpDown6.Value = num8;
			this.nudAgcStep3.ValueChanged += new EventHandler(this.nudAgcStep_ValueChanged);
			this.nudAgcStep1.Location = new Point(361, 19);
			NumericUpDown numericUpDown7 = this.nudAgcStep1;
			int[] bits9 = new int[4];
			bits9[0] = 31;
			Decimal num9 = new Decimal(bits9);
			numericUpDown7.Maximum = num9;
			this.nudAgcStep1.Name = "nudAgcStep1";
			this.nudAgcStep1.Size = new Size(98, 20);
			this.nudAgcStep1.TabIndex = 9;
			NumericUpDown numericUpDown8 = this.nudAgcStep1;
			int[] bits10 = new int[4];
			bits10[0] = 16;
			Decimal num10 = new Decimal(bits10);
			numericUpDown8.Value = num10;
			this.nudAgcStep1.ValueChanged += new EventHandler(this.nudAgcStep_ValueChanged);
			this.nudAgcStep2.Location = new Point(119, 45);
			NumericUpDown numericUpDown9 = this.nudAgcStep2;
			int[] bits11 = new int[4];
			bits11[0] = 15;
			Decimal num11 = new Decimal(bits11);
			numericUpDown9.Maximum = num11;
			this.nudAgcStep2.Name = "nudAgcStep2";
			this.nudAgcStep2.Size = new Size(98, 20);
			this.nudAgcStep2.TabIndex = 12;
			NumericUpDown numericUpDown10 = this.nudAgcStep2;
			int[] bits12 = new int[4];
			bits12[0] = 7;
			Decimal num12 = new Decimal(bits12);
			numericUpDown10.Value = num12;
			this.nudAgcStep2.ValueChanged += new EventHandler(this.nudAgcStep_ValueChanged);
			this.gBoxTxSettings.Controls.Add((Control)this.pnlPa20dBm);
			this.gBoxTxSettings.Controls.Add((Control)this.lblPa20dBm);
			this.gBoxTxSettings.Controls.Add((Control)this.nudMaxOutputPower);
			this.gBoxTxSettings.Controls.Add((Control)this.label7);
			this.gBoxTxSettings.Controls.Add((Control)this.label6);
			this.gBoxTxSettings.Controls.Add((Control)this.nudPllBandwidth);
			this.gBoxTxSettings.Controls.Add((Control)this.panel4);
			this.gBoxTxSettings.Controls.Add((Control)this.label19);
			this.gBoxTxSettings.Controls.Add((Control)this.label10);
			this.gBoxTxSettings.Controls.Add((Control)this.label5);
			this.gBoxTxSettings.Controls.Add((Control)this.label8);
			this.gBoxTxSettings.Controls.Add((Control)this.nudOcpTrim);
			this.gBoxTxSettings.Controls.Add((Control)this.suffixOCPtrim);
			this.gBoxTxSettings.Controls.Add((Control)this.label18);
			this.gBoxTxSettings.Controls.Add((Control)this.cBoxPaRamp);
			this.gBoxTxSettings.Controls.Add((Control)this.pnlPaSelect);
			this.gBoxTxSettings.Controls.Add((Control)this.nudOutputPower);
			this.gBoxTxSettings.Controls.Add((Control)this.suffixOutputPower);
			this.gBoxTxSettings.Controls.Add((Control)this.suffixPAramp);
			this.gBoxTxSettings.Controls.Add((Control)this.label12);
			this.gBoxTxSettings.Location = new Point(3, 111);
			this.gBoxTxSettings.Name = "gBoxTxSettings";
			this.gBoxTxSettings.Size = new Size(793, 176);
			this.gBoxTxSettings.TabIndex = 9;
			this.gBoxTxSettings.TabStop = false;
			this.gBoxTxSettings.Text = "Tx settings";
			this.pnlPa20dBm.AutoSize = true;
			this.pnlPa20dBm.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.pnlPa20dBm.Controls.Add((Control)this.rBtnPa20dBmOff);
			this.pnlPa20dBm.Controls.Add((Control)this.rBtnPa20dBmOn);
			this.pnlPa20dBm.Location = new Point(164, 150);
			this.pnlPa20dBm.Name = "pnlPa20dBm";
			this.pnlPa20dBm.Size = new Size(102, 20);
			this.pnlPa20dBm.TabIndex = 7;
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
			this.lblPa20dBm.Location = new Point(6, 154);
			this.lblPa20dBm.Name = "lblPa20dBm";
			this.lblPa20dBm.Size = new Size(144, 13);
			this.lblPa20dBm.TabIndex = 8;
			this.lblPa20dBm.Text = "+20 dBm on pin PA_BOOST:";
			this.nudMaxOutputPower.DecimalPlaces = 1;
			this.nudMaxOutputPower.Increment = new Decimal(new int[4]
      {
        6,
        0,
        0,
        65536
      });
			this.nudMaxOutputPower.Location = new Point(164, 98);
			NumericUpDownEx numericUpDownEx3 = this.nudMaxOutputPower;
			int[] bits13 = new int[4];
			bits13[0] = 15;
			Decimal num13 = new Decimal(bits13);
			numericUpDownEx3.Maximum = num13;
			this.nudMaxOutputPower.Minimum = new Decimal(new int[4]
      {
        108,
        0,
        0,
        65536
      });
			this.nudMaxOutputPower.Name = "nudMaxOutputPower";
			this.nudMaxOutputPower.Size = new Size(124, 20);
			this.nudMaxOutputPower.TabIndex = 4;
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
			this.label7.Location = new Point(6, 102);
			this.label7.Name = "label7";
			this.label7.Size = new Size(119, 13);
			this.label7.TabIndex = 6;
			this.label7.Text = "Maximum output power:";
			this.label6.AutoSize = true;
			this.label6.Location = new Point(294, 102);
			this.label6.Name = "label6";
			this.label6.Size = new Size(28, 13);
			this.label6.TabIndex = 5;
			this.label6.Text = "dBm";
			NumericUpDownEx numericUpDownEx4 = this.nudPllBandwidth;
			int[] bits14 = new int[4];
			bits14[0] = 75000;
			Decimal num14 = new Decimal(bits14);
			numericUpDownEx4.Increment = num14;
			this.nudPllBandwidth.Location = new Point(542, 72);
			NumericUpDownEx numericUpDownEx5 = this.nudPllBandwidth;
			int[] bits15 = new int[4];
			bits15[0] = 300000;
			Decimal num15 = new Decimal(bits15);
			numericUpDownEx5.Maximum = num15;
			NumericUpDownEx numericUpDownEx6 = this.nudPllBandwidth;
			int[] bits16 = new int[4];
			bits16[0] = 75000;
			Decimal num16 = new Decimal(bits16);
			numericUpDownEx6.Minimum = num16;
			this.nudPllBandwidth.Name = "nudPllBandwidth";
			this.nudPllBandwidth.Size = new Size(124, 20);
			this.nudPllBandwidth.TabIndex = 2;
			this.nudPllBandwidth.ThousandsSeparator = true;
			NumericUpDownEx numericUpDownEx7 = this.nudPllBandwidth;
			int[] bits17 = new int[4];
			bits17[0] = 300000;
			Decimal num17 = new Decimal(bits17);
			numericUpDownEx7.Value = num17;
			this.nudPllBandwidth.ValueChanged += new EventHandler(this.nudPllBandwidth_ValueChanged);
			this.panel4.AutoSize = true;
			this.panel4.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel4.Controls.Add((Control)this.rBtnOcpOff);
			this.panel4.Controls.Add((Control)this.rBtnOcpOn);
			this.panel4.Location = new Point(542, 98);
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
			this.label19.AutoSize = true;
			this.label19.Location = new Point(376, 102);
			this.label19.Name = "label19";
			this.label19.Size = new Size(139, 13);
			this.label19.TabIndex = 1;
			this.label19.Text = "Overload current protection:";
			this.label10.AutoSize = true;
			this.label10.Location = new Point(376, 129);
			this.label10.Name = "label10";
			this.label10.Size = new Size(130, 13);
			this.label10.TabIndex = 1;
			this.label10.Text = "Overload current trimming:";
			this.label5.AutoSize = true;
			this.label5.Location = new Point(376, 76);
			this.label5.Name = "label5";
			this.label5.Size = new Size(81, 13);
			this.label5.TabIndex = 1;
			this.label5.Text = "PLL bandwidth:";
			this.label8.AutoSize = true;
			this.label8.Location = new Point(672, 76);
			this.label8.Name = "label8";
			this.label8.Size = new Size(20, 13);
			this.label8.TabIndex = 3;
			this.label8.Text = "Hz";
			this.errorProvider.SetIconPadding((Control)this.nudOcpTrim, 30);
			this.nudOcpTrim.Location = new Point(542, 125);
			NumericUpDownEx numericUpDownEx8 = this.nudOcpTrim;
			int[] bits18 = new int[4];
			bits18[0] = 240;
			Decimal num18 = new Decimal(bits18);
			numericUpDownEx8.Maximum = num18;
			NumericUpDownEx numericUpDownEx9 = this.nudOcpTrim;
			int[] bits19 = new int[4];
			bits19[0] = 45;
			Decimal num19 = new Decimal(bits19);
			numericUpDownEx9.Minimum = num19;
			this.nudOcpTrim.Name = "nudOcpTrim";
			this.nudOcpTrim.Size = new Size(124, 20);
			this.nudOcpTrim.TabIndex = 2;
			this.nudOcpTrim.ThousandsSeparator = true;
			NumericUpDownEx numericUpDownEx10 = this.nudOcpTrim;
			int[] bits20 = new int[4];
			bits20[0] = 100;
			Decimal num20 = new Decimal(bits20);
			numericUpDownEx10.Value = num20;
			this.nudOcpTrim.ValueChanged += new EventHandler(this.nudOcpTrim_ValueChanged);
			this.suffixOCPtrim.AutoSize = true;
			this.suffixOCPtrim.Location = new Point(672, 129);
			this.suffixOCPtrim.Name = "suffixOCPtrim";
			this.suffixOCPtrim.Size = new Size(22, 13);
			this.suffixOCPtrim.TabIndex = 3;
			this.suffixOCPtrim.Text = "mA";
			this.label18.AutoSize = true;
			this.label18.Location = new Point(6, 128);
			this.label18.Name = "label18";
			this.label18.Size = new Size(74, 13);
			this.label18.TabIndex = 3;
			this.label18.Text = "Output power:";
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
			this.cBoxPaRamp.Location = new Point(164, 71);
			this.cBoxPaRamp.Name = "cBoxPaRamp";
			this.cBoxPaRamp.Size = new Size(124, 21);
			this.cBoxPaRamp.TabIndex = 2;
			this.cBoxPaRamp.SelectedIndexChanged += new EventHandler(this.cBoxPaRamp_SelectedIndexChanged);
			this.pnlPaSelect.AutoSize = true;
			this.pnlPaSelect.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.pnlPaSelect.Controls.Add((Control)this.rBtnRfPa);
			this.pnlPaSelect.Controls.Add((Control)this.rBtnRfo);
			this.pnlPaSelect.Location = new Point(266, 19);
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
			this.nudOutputPower.DecimalPlaces = 1;
			this.nudOutputPower.Location = new Point(164, 124);
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
			this.suffixOutputPower.AutoSize = true;
			this.suffixOutputPower.Location = new Point(294, 128);
			this.suffixOutputPower.Name = "suffixOutputPower";
			this.suffixOutputPower.Size = new Size(28, 13);
			this.suffixOutputPower.TabIndex = 1;
			this.suffixOutputPower.Text = "dBm";
			this.suffixPAramp.AutoSize = true;
			this.suffixPAramp.Location = new Point(294, 75);
			this.suffixPAramp.Name = "suffixPAramp";
			this.suffixPAramp.Size = new Size(18, 13);
			this.suffixPAramp.TabIndex = 3;
			this.suffixPAramp.Text = "µs";
			this.label12.AutoSize = true;
			this.label12.Location = new Point(6, 75);
			this.label12.Name = "label12";
			this.label12.Size = new Size(50, 13);
			this.label12.TabIndex = 1;
			this.label12.Text = "PA ramp:";
			this.gBoxGeneral.Controls.Add((Control)this.gBoxOptioanl);
			this.gBoxGeneral.Controls.Add((Control)this.panel1);
			this.gBoxGeneral.Controls.Add((Control)this.nudFrequencyXo);
			this.gBoxGeneral.Controls.Add((Control)this.label9);
			this.gBoxGeneral.Controls.Add((Control)this.label1);
			this.gBoxGeneral.Controls.Add((Control)this.label13);
			this.gBoxGeneral.Controls.Add((Control)this.lblRcOscillatorCalStat);
			this.gBoxGeneral.Controls.Add((Control)this.label14);
			this.gBoxGeneral.Controls.Add((Control)this.nudFrequencyRf);
			this.gBoxGeneral.Location = new Point(3, 3);
			this.gBoxGeneral.Name = "gBoxGeneral";
			this.gBoxGeneral.Size = new Size(793, 102);
			this.gBoxGeneral.TabIndex = 0;
			this.gBoxGeneral.TabStop = false;
			this.gBoxGeneral.Text = "General";
			this.gBoxGeneral.MouseEnter += new EventHandler(this.control_MouseEnter);
			this.gBoxGeneral.MouseLeave += new EventHandler(this.control_MouseLeave);
			this.gBoxOptioanl.Controls.Add((Control)this.panel10);
			this.gBoxOptioanl.Controls.Add((Control)this.label38);
			this.gBoxOptioanl.Controls.Add((Control)this.cBoxBand);
			this.gBoxOptioanl.Controls.Add((Control)this.panel8);
			this.gBoxOptioanl.Controls.Add((Control)this.label36);
			this.gBoxOptioanl.Controls.Add((Control)this.label37);
			this.gBoxOptioanl.Controls.Add((Control)this.label11);
			this.gBoxOptioanl.Controls.Add((Control)this.panel11);
			this.gBoxOptioanl.Controls.Add((Control)this.panel5);
			this.gBoxOptioanl.Controls.Add((Control)this.label39);
			this.gBoxOptioanl.Controls.Add((Control)this.label3);
			this.gBoxOptioanl.Location = new Point(474, 76);
			this.gBoxOptioanl.Name = "gBoxOptioanl";
			this.gBoxOptioanl.Size = new Size(338, 154);
			this.gBoxOptioanl.TabIndex = 7;
			this.gBoxOptioanl.TabStop = false;
			this.gBoxOptioanl.Text = "Optional";
			this.gBoxOptioanl.Visible = false;
			this.panel10.AutoSize = true;
			this.panel10.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel10.Controls.Add((Control)this.rBtnForceRxBandLowFrequencyOff);
			this.panel10.Controls.Add((Control)this.rBtnForceRxBandLowFrequencyOn);
			this.panel10.Location = new Point(173, 99);
			this.panel10.Name = "panel10";
			this.panel10.Size = new Size(102, 23);
			this.panel10.TabIndex = 9;
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
			this.label38.AutoSize = true;
			this.label38.Location = new Point(7, 104);
			this.label38.Name = "label38";
			this.label38.Size = new Size(149, 13);
			this.label38.TabIndex = 8;
			this.label38.Text = "Force Rx band low frequency:";
			this.cBoxBand.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxBand.FormattingEnabled = true;
			this.cBoxBand.Items.AddRange(new object[4]
      {
        (object) "Auto",
        (object) "820-1024",
        (object) "410-525",
        (object) "137-175"
      });
			this.cBoxBand.Location = new Point(173, 22);
			this.cBoxBand.Name = "cBoxBand";
			this.cBoxBand.Size = new Size(124, 21);
			this.cBoxBand.TabIndex = 25;
			this.cBoxBand.SelectedIndexChanged += new EventHandler(this.cBoxBand_SelectedIndexChanged);
			this.panel8.AutoSize = true;
			this.panel8.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel8.Controls.Add((Control)this.rBtnLowFrequencyModeOff);
			this.panel8.Controls.Add((Control)this.rBtnLowFrequencyModeOn);
			this.panel8.Location = new Point(173, 123);
			this.panel8.Name = "panel8";
			this.panel8.Size = new Size(102, 23);
			this.panel8.TabIndex = 28;
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
			this.label36.AutoSize = true;
			this.label36.Location = new Point(303, 26);
			this.label36.Name = "label36";
			this.label36.Size = new Size(29, 13);
			this.label36.TabIndex = 26;
			this.label36.Text = "MHz";
			this.label37.AutoSize = true;
			this.label37.Location = new Point(7, 128);
			this.label37.Name = "label37";
			this.label37.Size = new Size(109, 13);
			this.label37.TabIndex = 27;
			this.label37.Text = "Low frequency mode:";
			this.label11.AutoSize = true;
			this.label11.Location = new Point(15, 25);
			this.label11.Name = "label11";
			this.label11.Size = new Size(35, 13);
			this.label11.TabIndex = 24;
			this.label11.Text = "Band:";
			this.panel11.AutoSize = true;
			this.panel11.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel11.Controls.Add((Control)this.rBtnForceTxBandLowFrequencyOff);
			this.panel11.Controls.Add((Control)this.rBtnForceTxBandLowFrequencyOn);
			this.panel11.Location = new Point(173, 49);
			this.panel11.Name = "panel11";
			this.panel11.Size = new Size(102, 23);
			this.panel11.TabIndex = 10;
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
			this.panel5.AutoSize = true;
			this.panel5.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel5.Controls.Add((Control)this.rBtnFastHopOff);
			this.panel5.Controls.Add((Control)this.rBtnFastHopOn);
			this.panel5.Location = new Point(173, 76);
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
			this.label39.AutoSize = true;
			this.label39.Location = new Point(15, 54);
			this.label39.Name = "label39";
			this.label39.Size = new Size(148, 13);
			this.label39.TabIndex = 7;
			this.label39.Text = "Force Tx band low frequency:";
			this.label3.AutoSize = true;
			this.label3.Location = new Point(15, 78);
			this.label3.Name = "label3";
			this.label3.Size = new Size(71, 13);
			this.label3.TabIndex = 22;
			this.label3.Text = "Fast hopping:";
			this.panel1.AutoSize = true;
			this.panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel1.Controls.Add((Control)this.rBtnTcxoInputOff);
			this.panel1.Controls.Add((Control)this.rBtnTcxoInputOn);
			this.panel1.Location = new Point(545, 54);
			this.panel1.Name = "panel1";
			this.panel1.Size = new Size(122, 20);
			this.panel1.TabIndex = 1;
			this.rBtnTcxoInputOff.AutoSize = true;
			this.rBtnTcxoInputOff.Location = new Point(63, 3);
			this.rBtnTcxoInputOff.Margin = new Padding(3, 0, 3, 0);
			this.rBtnTcxoInputOff.Name = "rBtnTcxoInputOff";
			this.rBtnTcxoInputOff.Size = new Size(56, 17);
			this.rBtnTcxoInputOff.TabIndex = 1;
			this.rBtnTcxoInputOff.Text = "Crystal";
			this.rBtnTcxoInputOff.UseVisualStyleBackColor = true;
			this.rBtnTcxoInputOff.CheckedChanged += new EventHandler(this.rBtnTcxoInput_CheckedChanged);
			this.rBtnTcxoInputOn.AutoSize = true;
			this.rBtnTcxoInputOn.Checked = true;
			this.rBtnTcxoInputOn.Location = new Point(3, 3);
			this.rBtnTcxoInputOn.Margin = new Padding(3, 0, 3, 0);
			this.rBtnTcxoInputOn.Name = "rBtnTcxoInputOn";
			this.rBtnTcxoInputOn.Size = new Size(54, 17);
			this.rBtnTcxoInputOn.TabIndex = 0;
			this.rBtnTcxoInputOn.TabStop = true;
			this.rBtnTcxoInputOn.Text = "TCXO";
			this.rBtnTcxoInputOn.UseVisualStyleBackColor = true;
			this.rBtnTcxoInputOn.CheckedChanged += new EventHandler(this.rBtnTcxoInput_CheckedChanged);
			this.nudFrequencyXo.Location = new Point(545, 28);
			NumericUpDownEx numericUpDownEx11 = this.nudFrequencyXo;
			int[] bits21 = new int[4];
			bits21[0] = 32000000;
			Decimal num21 = new Decimal(bits21);
			numericUpDownEx11.Maximum = num21;
			NumericUpDownEx numericUpDownEx12 = this.nudFrequencyXo;
			int[] bits22 = new int[4];
			bits22[0] = 26000000;
			Decimal num22 = new Decimal(bits22);
			numericUpDownEx12.Minimum = num22;
			this.nudFrequencyXo.Name = "nudFrequencyXo";
			this.nudFrequencyXo.Size = new Size(124, 20);
			this.nudFrequencyXo.TabIndex = 1;
			this.nudFrequencyXo.ThousandsSeparator = true;
			NumericUpDownEx numericUpDownEx13 = this.nudFrequencyXo;
			int[] bits23 = new int[4];
			bits23[0] = 32000000;
			Decimal num23 = new Decimal(bits23);
			numericUpDownEx13.Value = num23;
			this.nudFrequencyXo.ValueChanged += new EventHandler(this.nudFrequencyXo_ValueChanged);
			this.label9.AutoSize = true;
			this.label9.Location = new Point(675, 32);
			this.label9.Name = "label9";
			this.label9.Size = new Size(20, 13);
			this.label9.TabIndex = 2;
			this.label9.Text = "Hz";
			this.label1.AutoSize = true;
			this.label1.Location = new Point(379, 32);
			this.label1.Name = "label1";
			this.label1.Size = new Size(78, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "XO Frequency:";
			this.label13.AutoSize = true;
			this.label13.Location = new Point(297, 45);
			this.label13.Name = "label13";
			this.label13.Size = new Size(20, 13);
			this.label13.TabIndex = 2;
			this.label13.Text = "Hz";
			this.lblRcOscillatorCalStat.AutoSize = true;
			this.lblRcOscillatorCalStat.Location = new Point(379, 58);
			this.lblRcOscillatorCalStat.Name = "lblRcOscillatorCalStat";
			this.lblRcOscillatorCalStat.Size = new Size(96, 13);
			this.lblRcOscillatorCalStat.TabIndex = 5;
			this.lblRcOscillatorCalStat.Text = "XO input selection:";
			this.label14.AutoSize = true;
			this.label14.Location = new Point(9, 45);
			this.label14.Name = "label14";
			this.label14.Size = new Size(74, 13);
			this.label14.TabIndex = 0;
			this.label14.Text = "RF frequency:";
			this.errorProvider.SetIconPadding((Control)this.nudFrequencyRf, 30);
			NumericUpDownEx numericUpDownEx14 = this.nudFrequencyRf;
			int[] bits24 = new int[4];
			bits24[0] = 61;
			Decimal num24 = new Decimal(bits24);
			numericUpDownEx14.Increment = num24;
			this.nudFrequencyRf.Location = new Point(167, 41);
			NumericUpDownEx numericUpDownEx15 = this.nudFrequencyRf;
			int[] bits25 = new int[4];
			bits25[0] = 2040000000;
			Decimal num25 = new Decimal(bits25);
			numericUpDownEx15.Maximum = num25;
			NumericUpDownEx numericUpDownEx16 = this.nudFrequencyRf;
			int[] bits26 = new int[4];
			bits26[0] = 100000000;
			Decimal num26 = new Decimal(bits26);
			numericUpDownEx16.Minimum = num26;
			this.nudFrequencyRf.Name = "nudFrequencyRf";
			this.nudFrequencyRf.Size = new Size(124, 20);
			this.nudFrequencyRf.TabIndex = 1;
			this.nudFrequencyRf.ThousandsSeparator = true;
			NumericUpDownEx numericUpDownEx17 = this.nudFrequencyRf;
			int[] bits27 = new int[4];
			bits27[0] = 915000000;
			Decimal num27 = new Decimal(bits27);
			numericUpDownEx17.Value = num27;
			this.nudFrequencyRf.ValueChanged += new EventHandler(this.nudFrequencyRf_ValueChanged);
			this.AutoScaleDimensions = new SizeF(6f, 13f);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.Controls.Add((Control)this.groupBox1);
			this.Controls.Add((Control)this.gBoxRxSettings);
			this.Controls.Add((Control)this.gBoxAgc);
			this.Controls.Add((Control)this.gBoxTxSettings);
			this.Controls.Add((Control)this.gBoxGeneral);
			this.Name = "CommonViewControl";
			this.Size = new Size(799, 493);
			((ISupportInitialize)this.errorProvider).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.gBoxRxSettings.ResumeLayout(false);
			this.gBoxRxSettings.PerformLayout();
			this.panel12.ResumeLayout(false);
			this.panel12.PerformLayout();
			this.panel9.ResumeLayout(false);
			this.panel9.PerformLayout();
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.gBoxAgc.ResumeLayout(false);
			this.gBoxAgc.PerformLayout();
			this.nudAgcStep5.EndInit();
			this.nudAgcStep4.EndInit();
			this.nudAgcReferenceLevel.EndInit();
			this.nudAgcStep3.EndInit();
			this.nudAgcStep1.EndInit();
			this.nudAgcStep2.EndInit();
			this.gBoxTxSettings.ResumeLayout(false);
			this.gBoxTxSettings.PerformLayout();
			this.pnlPa20dBm.ResumeLayout(false);
			this.pnlPa20dBm.PerformLayout();
			this.nudMaxOutputPower.EndInit();
			this.nudPllBandwidth.EndInit();
			this.panel4.ResumeLayout(false);
			this.panel4.PerformLayout();
			this.nudOcpTrim.EndInit();
			this.pnlPaSelect.ResumeLayout(false);
			this.pnlPaSelect.PerformLayout();
			this.nudOutputPower.EndInit();
			this.gBoxGeneral.ResumeLayout(false);
			this.gBoxGeneral.PerformLayout();
			this.gBoxOptioanl.ResumeLayout(false);
			this.gBoxOptioanl.PerformLayout();
			this.panel10.ResumeLayout(false);
			this.panel10.PerformLayout();
			this.panel8.ResumeLayout(false);
			this.panel8.PerformLayout();
			this.panel11.ResumeLayout(false);
			this.panel11.PerformLayout();
			this.panel5.ResumeLayout(false);
			this.panel5.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.nudFrequencyXo.EndInit();
			this.nudFrequencyRf.EndInit();
			this.ResumeLayout(false);
		}
	}
}
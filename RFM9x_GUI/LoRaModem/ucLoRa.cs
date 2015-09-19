using MyCSLib.Controls;
using MyCSLib.General.Events;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace LoRaModem
{
	public class ucLoRa : UserControl
	{
		public event DecimalEventHandler FrequencyRfChanged;
		public FTDI ftdi = new FTDI();

		public byte[] TxBuf = new byte[255];
		public RFM96 rfm96 = new RFM96();
		public RFM92 rfm92 = new RFM92();
		public GroupBox gbOpMode;
		public System.Windows.Forms.Timer RxChkTimer;
		public System.Windows.Forms.Timer TxInterTimer;
		public BytesBox bbPayload;
		public uint PktTxCnt;
		public uint PktRxCnt;
		public ucLoRa.RFStatus rfstatus;
		public ucLoRa.ChipSet ChipVer;

		protected object syncThread = new object();

		private const int FR_BAND_1_MAX = 175000000;
		private const int FR_BAND_1_MIN = 137000000;
		private const int FR_BAND_2_MAX = 525000000;
		private const int FR_BAND_2_MIN = 410000000;
		private const int FR_BAND_3_MAX = 1020000000;
		private const int FR_BAND_3_MIN = 862000000;

		#region Private
		private IContainer components;
		private GroupBox gbCrystal;
		private NumericUpDown nudRadioFreq;
		private Label label1;
		private RadioButton rbXTAL;
		private RadioButton rbTCXO;
		private Label label3;
		private GroupBox gbTxSetting;
		private ComboBox cbPaOutput;
		private Label label4;
		private Label label6;
		private ComboBox cbPaRamp;
		private Label label5;
		private Label label7;
		private Label label10;
		private ComboBox cbOutputPower;
		private Label label9;
		private Label label8;
		private Label label11;
		private Label label15;
		private Label label14;
		private Label label13;
		private RadioButton rbOCPOff;
		private RadioButton rbOCPOn;
		private Label label12;
		private GroupBox gbRxSetting;
		private RadioButton rbLNAOff;
		private RadioButton rbLNAOn;
		private Label label17;
		private RadioButton rbAGCOff;
		private RadioButton rbAGCOn;
		private Label label16;
		private Panel pAGC;
		private Panel pLNA;
		private GroupBox gbLoraSetting;
		private Panel panel2;
		private RadioButton rbPayloadCRCOff;
		private RadioButton rbPayloadCRCOn;
		private Label label26;
		private NumericUpDown nudPreambleLength;
		private Label label25;
		private Panel pLROptimize;
		private RadioButton rbLROptimizeOff;
		private RadioButton rbLROptimizeOn;
		private Label label24;
		private NumericUpDown nudRxTimeOut;
		private Label label22;
		private Label label21;
		private ComboBox cbBW;
		private Label label20;
		private ComboBox cbCR;
		private Label label19;
		private ComboBox cbSF;
		private Label label18;
		private GroupBox gbPHInfo;
		private TextBox tbRxPacketCnt;
		private TextBox tbRxNbBytes;
		private TextBox tbRxHeaderCnt;
		private Label label39;
		private Label label37;
		private Label label28;
		private TextBox tbRssiValue;
		private TextBox tbPktRssiValue;
		private TextBox tbPktSnrValue;
		private Label label42;
		private Label label41;
		private Label label40;
		private GroupBox gbMessage;
		private RadioButton rbGoTx;
		private RadioButton rbGoRx;
		private Label label45;
		private TextBox tbTxPktCnt;
		private Label label44;
		private Label label43;
		private Button bSwitch;
		private NumericUpDown nudTxDelay;
		private ComboBox cbMaxOutputPower;
		private ComboBox cbOcpTrimming;
		private ComboBox cbPLLBW;
		private Label lTs;
		private GroupBox gbIRQMask;
		private CheckBox cbCADDetect;
		private Label label35;
		private Label label34;
		private CheckBox cbFHSSChannel;
		private Label label33;
		private Label label32;
		private CheckBox cbCADDone;
		private Label label31;
		private Label label36;
		private CheckBox cbTxDone;
		private CheckBox cbRxTimeOut;
		private Label label29;
		private CheckBox cbValidHeader;
		private CheckBox cbCRCError;
		private CheckBox cbRxDone;
		private Label label30;
		private Label label23;
		private Label label27;
		private Label label2;
		private ComboBox cbLnaGain;
		private Led CADDetectLed;
		private Led FHSSChannelLed;
		private Led CADDoneLed;
		private Led TxDoneLed;
		private Led ValidHeaderLed;
		private Led CRCErrorLed;
		private Led RxDoneLed;
		private Led RxTimeOutLed;
		private RadioButton rbGoSleep;
		private RadioButton rbGoCAD;
		private Button rbClear;
		private CheckBox cbHex;
		private Label label47;
		private NumericUpDown nudImplicitRxLength;
		private CheckBox cbImplicit;
		private Label label46;
		private Label label53;
		private Label label52;
		private Label label51;
		private Label label50;
		private Led CRC_LED;
		private Label label38;
		private TextBox tbRxCR;
		private Led PllLockLed;
		private Label label49;
		private System.Windows.Forms.Timer RxRssitimer;
		private RadioButton rbGoTxTest;
		private RadioButton rbGoRxTest;
		private CheckBox cbNewLine;
		private Led PayCRCLed;

		private ushort Cnt1;
		private ushort Cnt2;
		private ushort Cnt3;
		private ushort Cnt4;
		private ushort Cnt5;
		private ushort Cnt6;
		private ushort Cnt7;
		private ushort Cnt8;
		#endregion

		public ucLoRa()
		{
			InitializeComponent();
			InitAllValue();
		}

		public Decimal FrequencyStep
		{
			get { return nudRadioFreq.Increment; }
			set { nudRadioFreq.Increment = value; }
		}

		public Decimal FrequencyRf
		{
			get { return nudRadioFreq.Value; }
			set
			{
				try
				{
					nudRadioFreq.ValueChanged -= new EventHandler(nudRadioFreq_ValueChanged);
					nudRadioFreq.Value = (Decimal)(uint)Math.Round(value / FrequencyStep, MidpointRounding.AwayFromZero) * FrequencyStep;
				}
				catch (Exception)
				{
					nudRadioFreq.BackColor = ControlPaint.LightLight(Color.Red);
				}
				finally
				{
					nudRadioFreq.ValueChanged += new EventHandler(nudRadioFreq_ValueChanged);
				}
			}
		}

		private void nudRadioFreq_ValueChanged(object sender, EventArgs e)
		{
			double num1 = 15625.0 / 256.0;
			FrequencyRf = nudRadioFreq.Value;
			if (FrequencyRf < new Decimal(137000000) || FrequencyRf > new Decimal(175000000) && FrequencyRf < new Decimal(410000000) || (FrequencyRf > new Decimal(525000000) && FrequencyRf < new Decimal(862000000) || FrequencyRf > new Decimal(1020000000)))
				nudRadioFreq.BackColor = ControlPaint.LightLight(Color.Red);
			else
				nudRadioFreq.BackColor = ControlPaint.LightLight(Color.White);
			OnFrequencyRfChanged(FrequencyRf);
			if (FrequencyRf < new Decimal(525000000))
				pLNA.Enabled = false;
			else
				pLNA.Enabled = true;
			uint num2 = (uint)((double)nudRadioFreq.Value / num1);
			if (ChipVer == ucLoRa.ChipSet.RF92)
			{
				rfm92.RegFrLsb.Value = (byte)num2;
				rfm92.RegFrMid.Value = (byte)(num2 >> 8);
				rfm92.RegFrMsb.Value = (byte)(num2 >> 16);
			}
			else
			{
				rfm96.RegFrLsb.Value = (byte)num2;
				rfm96.RegFrMid.Value = (byte)(num2 >> 8);
				rfm96.RegFrMsb.Value = (byte)(num2 >> 16);
			}
		}

		public void SetcbBW()
		{
			if (rbTCXO.Checked)
			{
				if (ChipVer == ucLoRa.ChipSet.RF92)
				{
					cbBW.Items.Clear();
					cbBW.Items.Add("125");
					cbBW.Items.Add("250");
					cbBW.Items.Add("500");
				}
				else
				{
					cbBW.Items.Clear();
					cbBW.Items.Add("7.8");
					cbBW.Items.Add("10.4");
					cbBW.Items.Add("15.6");
					cbBW.Items.Add("20.8");
					cbBW.Items.Add("31.25");
					cbBW.Items.Add("41.7");
					cbBW.Items.Add("62.5");
					cbBW.Items.Add("125");
					cbBW.Items.Add("250");
					cbBW.Items.Add("500");
				}
			}
			else if (ChipVer == ucLoRa.ChipSet.RF92)
			{
				cbBW.Items.Clear();
				cbBW.Items.Add("125");
				cbBW.Items.Add("250");
				cbBW.Items.Add("500");
			}
			else
			{
				cbBW.Items.Clear();
				cbBW.Items.Add("62.5");
				cbBW.Items.Add("125");
				cbBW.Items.Add("250");
				cbBW.Items.Add("500");
			}
		}

		private void UpdatecbBW()
		{
			if (rbTCXO.Checked)
			{
				if (ChipVer == ucLoRa.ChipSet.RF92)
				{
					rfm92.RegTcxo.Value |= 16;
					cbBW.Items.Clear();
					cbBW.Items.Add("125");
					cbBW.Items.Add("250");
					cbBW.Items.Add("500");
					cbBW.SelectedIndex = 0;
					cbBW.Text = "125";
				}
				else
				{
					rfm96.RegTcxo.Value |= 16;
					cbBW.Items.Clear();
					cbBW.Items.Add("7.8");
					cbBW.Items.Add("10.4");
					cbBW.Items.Add("15.6");
					cbBW.Items.Add("20.8");
					cbBW.Items.Add("31.25");
					cbBW.Items.Add("41.7");
					cbBW.Items.Add("62.5");
					cbBW.Items.Add("125");
					cbBW.Items.Add("250");
					cbBW.Items.Add("500");
					cbBW.SelectedIndex = 7;
					cbBW.Text = "125";
				}
			}
			else if (ChipVer == ucLoRa.ChipSet.RF92)
			{
				rfm92.RegTcxo.Value &= 239;
				cbBW.Items.Clear();
				cbBW.Items.Add("125");
				cbBW.Items.Add("250");
				cbBW.Items.Add("500");
				cbBW.SelectedIndex = 0;
				cbBW.Text = "125";
			}
			else
			{
				rfm96.RegTcxo.Value &= 239;
				cbBW.Items.Clear();
				cbBW.Items.Add("62.5");
				cbBW.Items.Add("125");
				cbBW.Items.Add("250");
				cbBW.Items.Add("500");
				cbBW.SelectedIndex = 1;
				cbBW.Text = "125";
			}
		}

		private void Crystal_CheckedChanged(object sender, EventArgs e)
		{
			UpdatecbBW();
		}

		private void cbPaOutput_SelectedValueChanged(object sender, EventArgs e)
		{
			switch (cbPaOutput.Text)
			{
				case "PA_BOOST":
					cbMaxOutputPower.Items.Clear();
					cbMaxOutputPower.Text = "20";
					cbMaxOutputPower.Enabled = false;
					cbOutputPower.Items.Clear();
					cbOutputPower.Items.Add(2);
					cbOutputPower.Items.Add(3);
					cbOutputPower.Items.Add(4);
					cbOutputPower.Items.Add(5);
					cbOutputPower.Items.Add(6);
					cbOutputPower.Items.Add(7);
					cbOutputPower.Items.Add(8);
					cbOutputPower.Items.Add(9);
					cbOutputPower.Items.Add(10);
					cbOutputPower.Items.Add(11);
					cbOutputPower.Items.Add(12);
					cbOutputPower.Items.Add(13);
					cbOutputPower.Items.Add(14);
					cbOutputPower.Items.Add(15);
					cbOutputPower.Items.Add(16);
					cbOutputPower.Items.Add(17);
					cbOutputPower.Items.Add(18);
					cbOutputPower.Items.Add(19);
					cbOutputPower.Items.Add(20);
					break;
				default:
					if (ChipVer == ucLoRa.ChipSet.RF92)
					{
						cbMaxOutputPower.Items.Clear();
						cbMaxOutputPower.Text = "20";
						cbMaxOutputPower.Enabled = false;
						cbOutputPower.Items.Clear();
						cbOutputPower.Items.Add(-1);
						cbOutputPower.Items.Add(0);
						cbOutputPower.Items.Add(1);
						cbOutputPower.Items.Add(2);
						cbOutputPower.Items.Add(3);
						cbOutputPower.Items.Add(4);
						cbOutputPower.Items.Add(5);
						cbOutputPower.Items.Add(6);
						cbOutputPower.Items.Add(7);
						cbOutputPower.Items.Add(8);
						cbOutputPower.Items.Add(9);
						cbOutputPower.Items.Add(10);
						cbOutputPower.Items.Add(11);
						cbOutputPower.Items.Add(12);
						cbOutputPower.Items.Add(13);
						cbOutputPower.Items.Add(14);
						break;
					}
					cbMaxOutputPower.Enabled = true;
					cbMaxOutputPower.Items.Clear();
					cbMaxOutputPower.Items.Add(10.8);
					cbMaxOutputPower.Items.Add(11.4);
					cbMaxOutputPower.Items.Add(12.0);
					cbMaxOutputPower.Items.Add(12.6);
					cbMaxOutputPower.Items.Add(13.2);
					cbMaxOutputPower.Items.Add(13.8);
					cbMaxOutputPower.Items.Add(14.4);
					cbMaxOutputPower.Items.Add(15.0);
					cbOutputPower.Items.Clear();
					cbOutputPower.Items.Add(0);
					cbOutputPower.Items.Add(1);
					cbOutputPower.Items.Add(2);
					cbOutputPower.Items.Add(3);
					cbOutputPower.Items.Add(4);
					cbOutputPower.Items.Add(5);
					cbOutputPower.Items.Add(6);
					cbOutputPower.Items.Add(7);
					cbOutputPower.Items.Add(8);
					cbOutputPower.Items.Add(9);
					cbOutputPower.Items.Add(10);
					cbOutputPower.Items.Add(11);
					cbOutputPower.Items.Add(12);
					cbOutputPower.Items.Add(13);
					cbOutputPower.Items.Add(14);
					cbOutputPower.Items.Add(15);
					break;
			}
		}

		private void cbPaRamp_SelectedValueChanged(object sender, EventArgs e)
		{
			byte num = (byte)((ChipVer != ucLoRa.ChipSet.RF92 ? (uint)rfm96.RegPaRamp.Value : (uint)rfm92.RegPaRamp.Value) & 240U);
			switch (cbPaRamp.Text)
			{
				case "2000":
					num |= 1;
					break;
				case "1000":
					num |= 2;
					break;
				case "500":
					num |= 3;
					break;
				case "250":
					num |= 4;
					break;
				case "125":
					num |= 5;
					break;
				case "100":
					num |= 6;
					break;
				case "62":
					num |= 7;
					break;
				case "50":
					num |= 8;
					break;
				case "40":
					num |= 9;
					break;
				case "31":
					num |= 10;
					break;
				case "25":
					num |= 11;
					break;
				case "20":
					num |= 12;
					break;
				case "15":
					num |= 13;
					break;
				case "12":
					num |= 14;
					break;
				case "10":
					num |= 15;
					break;
			}
			if (ChipVer == ucLoRa.ChipSet.RF92)
				rfm92.RegPaRamp.Value = num;
			else
				rfm96.RegPaRamp.Value = num;
		}

		private void cbMaxOutputPower_SelectedValueChanged(object sender, EventArgs e)
		{
			byte num = (byte)((uint)rfm96.RegPaConfig.Value & 143U);
			if (cbPaOutput.Text == "RFO")
			{
				switch (cbMaxOutputPower.SelectedIndex)
				{
					case 1:
						num |= 16;
						break;
					case 2:
						num |= 32;
						break;
					case 3:
						num |= 48;
						break;
					case 4:
						num |= 64;
						break;
					case 5:
						num |= 80;
						break;
					case 6:
						num |= 96;
						break;
					case 7:
						num |= 112;
						break;
				}
			}
			else
				num |= 112;
			rfm96.RegPaConfig.Value = num;
		}

		private void cbOutputPower_SelectedValueChanged(object sender, EventArgs e)
		{
			byte num1 = (byte)((ChipVer != ucLoRa.ChipSet.RF92 ? (uint)rfm96.RegPaConfig.Value : (uint)rfm92.RegPaConfig.Value) & 240U);
			if (cbPaOutput.Text == "RFO")
			{
				byte num2 = (byte)cbOutputPower.SelectedIndex;
				byte num3 = (byte)(num1 | num2);
				if (ChipVer == ucLoRa.ChipSet.RF92)
				{
					rfm92.RegPaConfig.Value = num3;
					rfm92.RegPaDac.Value = 132;
				}
				else
				{
					rfm96.RegPaConfig.Value = num3;
					rfm96.RegPaDac.Value = 132;
				}
			}
			else if (cbPaOutput.SelectedIndex > 15)
			{
				if (ChipVer == ucLoRa.ChipSet.RF92)
				{
					rfm92.RegPaDac.Value = 135;
					rfm92.RegPaConfig.Value |= (byte)(cbPaOutput.SelectedIndex - 3);
				}
				else
				{
					rfm96.RegPaDac.Value = 135;
					rfm96.RegPaConfig.Value |= (byte)(cbPaOutput.SelectedIndex - 3);
				}
			}
			else if (ChipVer == ucLoRa.ChipSet.RF92)
			{
				rfm92.RegPaDac.Value = 132;
				rfm92.RegPaConfig.Value |= (byte)cbPaOutput.SelectedIndex;
			}
			else
			{
				rfm96.RegPaDac.Value = 132;
				rfm96.RegPaConfig.Value |= (byte)cbPaOutput.SelectedIndex;
			}
		}

		private void cbPLLBW_SelectedValueChanged(object sender, EventArgs e)
		{
			byte num1 = ChipVer != ucLoRa.ChipSet.RF92 ? rfm96.RegPllHf.Value : rfm92.RegPllHf.Value;
			byte num2;
			switch (cbPLLBW.SelectedIndex)
			{
				case 0:
					num2 = (byte)(num1 | 16);
					break;
				case 1:
					num2 = (byte)(num1 | 80);
					break;
				case 2:
					num2 = (byte)(num1 | 144);
					break;
				default:
					num2 = (byte)(num1 | 208);
					break;
			}
			if (ChipVer == ucLoRa.ChipSet.RF92)
				rfm92.RegPllHf.Value = num2;
			else
				rfm96.RegPllHf.Value = num2;
		}

		private void rbOCP_CheckedChanged(object sender, EventArgs e)
		{
			if (rbOCPOn.Checked)
			{
				cbOcpTrimming.Enabled = true;
				byte num;
				switch (cbOcpTrimming.SelectedIndex)
				{
					case 0:
						num = 35;
						break;
					case 1:
						num = 39;
						break;
					case 2:
						num = 43;
						break;
					case 3:
						num = 47;
						break;
					case 4:
						num = 49;
						break;
					case 5:
						num = 51;
						break;
					case 6:
						num = 53;
						break;
					case 7:
						num = 55;
						break;
					case 8:
						num = 57;
						break;
					default:
						num = 59;
						break;
				}
				if (ChipVer == ucLoRa.ChipSet.RF92)
					rfm92.RegOcp.Value = num;
				else
					rfm96.RegOcp.Value = num;
			}
			else
			{
				cbOcpTrimming.Enabled = false;
				if (ChipVer == ucLoRa.ChipSet.RF92)
					rfm92.RegOcp.Value = 31;
				else
					rfm96.RegOcp.Value = 31;
			}
		}

		private void cbOcpTrimming_SelectedValueChanged(object sender, EventArgs e)
		{
			byte num;
			switch (cbOcpTrimming.SelectedIndex)
			{
				case 0:
					num = 35;
					break;
				case 1:
					num = 39;
					break;
				case 2:
					num = 43;
					break;
				case 3:
					num = 47;
					break;
				case 4:
					num = 49;
					break;
				case 5:
					num = 51;
					break;
				case 6:
					num = 53;
					break;
				case 7:
					num = 55;
					break;
				case 8:
					num = 57;
					break;
				default:
					num = 59;
					break;
			}
			if (ChipVer == ucLoRa.ChipSet.RF92)
				rfm92.RegOcp.Value = num;
			else
				rfm96.RegOcp.Value = num;
		}

		private void rbAGC_CheckedChanged(object sender, EventArgs e)
		{
			if (rbAGCOn.Checked)
			{
				if (ChipVer == ucLoRa.ChipSet.RF92)
					rfm92.RegModemConfig2.Value |= 4;
				else
					rfm96.RegModemConfig3.Value |= 4;
				cbLnaGain.Enabled = false;
			}
			else
			{
				if (ChipVer == ucLoRa.ChipSet.RF92)
					rfm92.RegModemConfig2.Value &= 251;
				else
					rfm96.RegModemConfig3.Value &= 251;
				cbLnaGain.Enabled = true;
			}
		}

		private void rbLNA_CheckedChanged(object sender, EventArgs e)
		{
			if (rbLNAOn.Checked)
			{
				if (ChipVer == ucLoRa.ChipSet.RF92)
					rfm92.RegLna.Value |= 3;
				else
					rfm96.RegLna.Value |= 3;
			}
			else if (ChipVer == ucLoRa.ChipSet.RF92)
				rfm92.RegLna.Value &= 252;
			else
				rfm96.RegLna.Value &= 252;
		}

		private void cbSF_SelectedValueChanged(object sender, EventArgs e)
		{
			byte num1 = (byte)((ChipVer != ucLoRa.ChipSet.RF92 ? (uint)rfm96.RegModemConfig2.Value : (uint)rfm92.RegModemConfig2.Value) & 15U);
			cbImplicit.Enabled = true;
			byte num2;
			byte num3;
			switch (cbSF.Text)
			{
				case "SF6":
					num2 = (byte)6;
					num3 = (byte)((uint)num1 | 96U);
					cbImplicit.Checked = true;
					cbImplicit.Enabled = false;
					break;
				case "SF7":
					num2 = (byte)7;
					num3 = (byte)((uint)num1 | 112U);
					break;
				case "SF8":
					num2 = 8;
					num3 = (byte)((uint)num1 | 128U);
					break;
				case "SF9":
					num2 = (byte)9;
					num3 = (byte)((uint)num1 | 144U);
					break;
				case "SF10":
					num2 = (byte)10;
					num3 = (byte)((uint)num1 | 160U);
					break;
				case "SF11":
					num2 = (byte)11;
					num3 = (byte)((uint)num1 | 176U);
					break;
				default:
					num2 = (byte)12;
					num3 = (byte)((uint)num1 | 192U);
					break;
			}
			if (ChipVer == ucLoRa.ChipSet.RF92)
				rfm92.RegModemConfig2.Value = num3;
			else
				rfm96.RegModemConfig2.Value = num3;
			double num4;
			if (ChipVer == ucLoRa.ChipSet.RF92)
			{
				switch ((byte)((uint)rfm92.RegModemConfig1.Value & 63U))
				{
					case (byte)64:
						num4 = 250.0;
						break;
					case 0x80:
						num4 = 500.0;
						break;
					default:
						num4 = 125.0;
						break;
				}
			}
			else
			{
				byte num5 = (byte)((uint)rfm96.RegModemConfig1.Value & 15U);
				if ((uint)num5 <= 64U)
				{
					if ((uint)num5 <= 32U)
					{
						if ((int)num5 != 16)
						{
							if ((int)num5 == 32)
							{
								num4 = 10.4;
								goto label_37;
							}
						}
						else
						{
							num4 = 7.8;
							goto label_37;
						}
					}
					else if ((int)num5 != 48)
					{
						if ((int)num5 == 64)
						{
							num4 = 31.25;
							goto label_37;
						}
					}
					else
					{
						num4 = 20.8;
						goto label_37;
					}
				}
				else if ((uint)num5 <= 96U)
				{
					if ((int)num5 != 80)
					{
						if ((int)num5 == 96)
						{
							num4 = 62.5;
							goto label_37;
						}
					}
					else
					{
						num4 = 41.7;
						goto label_37;
					}
				}
				else if ((int)num5 != 112)
				{
					if ((int)num5 != 128)
					{
						if ((int)num5 == 144)
						{
							num4 = 500.0;
							goto label_37;
						}
					}
					else
					{
						num4 = 250.0;
						goto label_37;
					}
				}
				num4 = 125.0;
			}
		label_37:
			lTs.Text = (Math.Pow(2.0, (double)num2) / num4).ToString("#0.00");
		}

		private void cbCR_SelectedValueChanged(object sender, EventArgs e)
		{
			if (ChipVer == ucLoRa.ChipSet.RF92)
			{
				byte num1 = (byte)((uint)rfm92.RegModemConfig1.Value & 199U);
				byte num2;
				switch (cbCR.Text)
				{
					case "4/6":
						num2 = (byte)((uint)num1 | 16U);
						break;
					case "4/7":
						num2 = (byte)((uint)num1 | 24U);
						break;
					case "4/8":
						num2 = (byte)((uint)num1 | 32U);
						break;
					default:
						num2 = (byte)((uint)num1 | 8U);
						break;
				}
				rfm92.RegModemConfig1.Value = num2;
			}
			else
			{
				byte num1 = (byte)((uint)rfm96.RegModemConfig1.Value & 241U);
				byte num2;
				switch (cbCR.Text)
				{
					case "4/6":
						num2 = (byte)((uint)num1 | 4U);
						break;
					case "4/7":
						num2 = (byte)((uint)num1 | 6U);
						break;
					case "4/8":
						num2 = (byte)((uint)num1 | 8U);
						break;
					default:
						num2 = (byte)((uint)num1 | 2U);
						break;
				}
				rfm96.RegModemConfig1.Value = num2;
			}
		}

		private void cbBW_SelectedValueChanged(object sender, EventArgs e)
		{
			double num1;
			if (ChipVer == ucLoRa.ChipSet.RF92)
			{
				byte num2 = (byte)((uint)rfm92.RegModemConfig1.Value & 63U);
				switch (cbBW.Text)
				{
					case "250":
						num1 = 250.0;
						num2 |= (byte)64;
						break;
					case "500":
						num1 = 500.0;
						num2 |= 0x80;
						break;
					default:
						num1 = 125.0;
						break;
				}
				rfm92.RegModemConfig1.Value = num2;
			}
			else
			{
				byte num2 = (byte)((uint)rfm96.RegModemConfig1.Value & 15U);
				switch (cbBW.Text)
				{
					case "7.8":
						num1 = 7.8;
						break;
					case "10.4":
						num1 = 10.4;
						num2 |= (byte)16;
						break;
					case "15.6":
						num1 = 15.6;
						num2 |= 32;
						break;
					case "20.8":
						num1 = 20.8;
						num2 |= (byte)48;
						break;
					case "31.25":
						num1 = 31.25;
						num2 |= (byte)64;
						break;
					case "41.7":
						num1 = 41.7;
						num2 |= 80;
						break;
					case "62.5":
						num1 = 62.5;
						num2 |= (byte)96;
						break;
					case "250":
						num1 = 250.0;
						num2 |= 0x80;
						break;
					case "500":
						num1 = 500.0;
						num2 |= (byte)144;
						break;
					default:
						num1 = 125.0;
						num2 |= (byte)112;
						break;
				}
				rfm96.RegModemConfig1.Value = num2;
			}
			byte num3;
			switch (cbSF.Text)
			{
				case "SF6":
					num3 = (byte)6;
					break;
				case "SF7":
					num3 = (byte)7;
					break;
				case "SF8":
					num3 = 8;
					break;
				case "SF9":
					num3 = (byte)9;
					break;
				case "SF10":
					num3 = (byte)10;
					break;
				case "SF11":
					num3 = (byte)11;
					break;
				default:
					num3 = (byte)12;
					break;
			}
			lTs.Text = (Math.Pow(2.0, (double)num3) / num1).ToString("#0.00");
		}

		private void nudRxTimeOut_ValueChanged(object sender, EventArgs e)
		{
			ushort num = (ushort)nudRxTimeOut.Value;
			if (ChipVer == ucLoRa.ChipSet.RF92)
			{
				rfm92.RegSymbTimeoutLsb.Value = (byte)num;
				rfm92.RegModemConfig2.Value &= (byte)252;
				rfm92.RegModemConfig2.Value |= (byte)((int)num >> 8 & 3);
			}
			else
			{
				rfm96.RegSymbTimeoutLsb.Value = (byte)num;
				rfm96.RegModemConfig2.Value &= (byte)252;
				rfm96.RegModemConfig2.Value |= (byte)((int)num >> 8 & 3);
			}
		}

		private void rbLROptimize_CheckedChanged(object sender, EventArgs e)
		{
			if (rbLROptimizeOn.Checked)
			{
				if (ChipVer == ucLoRa.ChipSet.RF92)
					rfm92.RegModemConfig1.Value |= 0x01;
				else
					rfm96.RegModemConfig3.Value |= 0x08;
			}
			else if (ChipVer == ucLoRa.ChipSet.RF92)
				rfm92.RegModemConfig1.Value &= (byte)254;
			else
				rfm96.RegModemConfig3.Value &= 247;
		}

		private void nudPreambleLength_ValueChanged(object sender, EventArgs e)
		{
			if (ChipVer == ucLoRa.ChipSet.RF92)
			{
				rfm92.RegPreambleLsb.Value = (byte)nudPreambleLength.Value;
				rfm92.RegPreambleMsb.Value = (byte)((uint)(ushort)nudPreambleLength.Value >> 8);
			}
			else
			{
				rfm96.RegPreambleLsb.Value = (byte)nudPreambleLength.Value;
				rfm96.RegPreambleMsb.Value = (byte)((uint)(ushort)nudPreambleLength.Value >> 8);
			}
		}

		private void rbPayloadCRC_CheckedChanged(object sender, EventArgs e)
		{
			if (rbPayloadCRCOn.Checked)
			{
				if (ChipVer == ucLoRa.ChipSet.RF92)
					rfm92.RegModemConfig1.Value |= 0x02;
				else
					rfm96.RegModemConfig2.Value |= 0x04;
			}
			else if (ChipVer == ucLoRa.ChipSet.RF92)
				rfm92.RegModemConfig1.Value &= 253;
			else
				rfm96.RegModemConfig2.Value &= 251;
		}

		private void SetRegIrq(CheckBox cb, Led led, byte mask92, byte mask96)
		{
			if (cb.Checked)
			{
				led.LedColor = Color.Red;
				if (ChipVer == ucLoRa.ChipSet.RF92)
					rfm92.RegIrqFlagsMask.Value |= mask92;
				else
					rfm96.RegIrqFlagsMask.Value |= mask96;

			}
			else
			{
				led.LedColor = Color.Lime;
				if (ChipVer == ucLoRa.ChipSet.RF92)
					rfm92.RegIrqFlagsMask.Value &= (byte)(~mask92);
				else
					rfm96.RegIrqFlagsMask.Value &= (byte)(~mask96);
			}
		}
		private void cbRxTimeOut_CheckedChanged(object sender, EventArgs e)
		{
			SetRegIrq(cbRxTimeOut, RxTimeOutLed, 0x80, 0x80);
		}

		private void cbRxDone_CheckedChanged(object sender, EventArgs e)
		{
			SetRegIrq(cbRxDone, RxDoneLed, 0x40, 0x40);
		}

		private void cbCRCError_CheckedChanged(object sender, EventArgs e)
		{
			SetRegIrq(cbCRCError, CRCErrorLed, 0x20, 0x20);
		}

		private void cbValidHeader_CheckedChanged(object sender, EventArgs e)
		{
			SetRegIrq(cbValidHeader, ValidHeaderLed, 0x10, 0x10);
		}

		private void cbTxDone_CheckedChanged(object sender, EventArgs e)
		{
			SetRegIrq(cbTxDone, TxDoneLed, 0x08, 0x08);
		}

		private void cbCADDone_CheckedChanged(object sender, EventArgs e)
		{
			SetRegIrq(cbCADDone, CADDoneLed, 0x04, 0x04);
		}

		private void cbFHSSChannel_CheckedChanged(object sender, EventArgs e)
		{
			SetRegIrq(cbFHSSChannel, FHSSChannelLed, 0x02, 0x02);
		}

		private void cbCADDetect_CheckedChanged(object sender, EventArgs e)
		{
			SetRegIrq(cbCADDetect, CADDetectLed, 0x01, 0x01);
		}

		private void rbStatus_CheckedChanged(object sender, EventArgs e)
		{
			if (rbGoRx.Checked || rbGoSleep.Checked || (rbGoCAD.Checked || rbGoRxTest.Checked) || rbGoTxTest.Checked)
				bbPayload.ReadOnly = true;
			else
				bbPayload.ReadOnly = false;
		}

		private void cbImplicit_CheckedChanged(object sender, EventArgs e)
		{
			if (ChipVer == ucLoRa.ChipSet.RF92)
			{
				if (cbImplicit.Checked)
					rfm92.RegModemConfig1.Value |= 4;
				else
					rfm92.RegModemConfig1.Value &= 251;
			}
			else if (cbImplicit.Checked)
				rfm96.RegModemConfig1.Value |= 1;
			else
				rfm96.RegModemConfig1.Value &= 254;
		}

		private void nudImplicitRxLength_ValueChanged(object sender, EventArgs e)
		{
			if (ChipVer == ucLoRa.ChipSet.RF92)
				rfm92.RegPayloadLength.Value = (byte)nudImplicitRxLength.Value;
			else
				rfm96.RegPayloadLength.Value = (byte)nudImplicitRxLength.Value;
		}

		private void bSwitch_Click(object sender, EventArgs e)
		{
			if (bSwitch.Text == "Start")
			{
				bSwitch.Text = "Stop";
				rbGoTx.Enabled = false;
				rbGoRx.Enabled = false;
				rbGoSleep.Enabled = false;
				rbGoCAD.Enabled = false;
				rbGoRxTest.Enabled = false;
				rbGoTxTest.Enabled = false;
				PktTxCnt = 0U;
				PktRxCnt = 0U;
				tbTxPktCnt.Text = "0";

				if (rbGoTx.Checked)
				{
					ClearLedStatus();
					if (EntryTx())
					{
						ChkPllLock();
						rfstatus = ucLoRa.RFStatus.Transmitter;
						RxChkTimer.Enabled = true;
						ftdi.TxLedOn();
					}
					else
					{
						bSwitch.Text = "Start";
						rfstatus = ucLoRa.RFStatus.Standby;
						rbGoTx.Enabled = true;
						rbGoRx.Enabled = true;
						RxChkTimer.Enabled = false;
						TxInterTimer.Enabled = false;
						EntryStandby();
						ftdi.LedOff();
					}
				}
				else if (rbGoRx.Checked)
				{
					bbPayload.Clear();
					rfstatus = ucLoRa.RFStatus.Receiver;
					ClearLedStatus();
					EntryRx();
					ChkPllLock();
					RxChkTimer.Enabled = true;
					ftdi.RxLedOn();
				}
				else if (rbGoCAD.Checked)
				{
					bbPayload.Clear();
					rfstatus = ucLoRa.RFStatus.CADdetect;
					ClearLedStatus();
					EntryCAD();
					RxChkTimer.Enabled = true;
					ftdi.RxLedOn();
				}
				else if (rbGoRxTest.Checked)
				{
					bbPayload.Clear();
					ClearLedStatus();
					EntryRxTest();
					RxChkTimer.Enabled = false;
					ftdi.RxLedOn();
				}
				else if (rbGoTxTest.Checked)
				{
					bbPayload.Clear();
					ClearLedStatus();
					EntryTxTest();
					RxChkTimer.Enabled = false;
					ftdi.TxLedOn();
				}
				else
				{
					bbPayload.Clear();
					RxChkTimer.Enabled = false;
					rfstatus = ucLoRa.RFStatus.Sleep;
					EntrySleep();
					ClearLedStatus();
				}
			}
			else
			{
				bSwitch.Text = "Start";
				rfstatus = ucLoRa.RFStatus.Standby;
				rbGoTx.Enabled = true;
				rbGoRx.Enabled = true;
				rbGoSleep.Enabled = true;
				rbGoCAD.Enabled = true;
				rbGoRxTest.Enabled = true;
				rbGoTxTest.Enabled = true;
				RxChkTimer.Enabled = false;
				TxInterTimer.Enabled = false;
				EntryStandby();
				ftdi.LedOff();
				ClearLedStatus();
			}
		}

		private ushort UpdataLedStatusEx(ushort cnt, byte status, byte mask, Led led)
		{
			if (led.LedColor != Color.Red)
			{
				if ((status & mask) != 0)
				{
					led.LedColor = Color.Blue;
					cnt = 0;
				}
				else
				{
					++cnt;
					if (cnt > 3)
						led.LedColor = Color.Lime;
				}
			}
			return cnt;
		}

		private void UpdataLedStatus(byte tmp)
		{
			Cnt1 = UpdataLedStatusEx(Cnt1, tmp, 0x80, RxTimeOutLed);
			Cnt2 = UpdataLedStatusEx(Cnt2, tmp, 0x40, RxDoneLed);
			Cnt3 = UpdataLedStatusEx(Cnt3, tmp, 0x20, CRCErrorLed);
			Cnt4 = UpdataLedStatusEx(Cnt4, tmp, 0x10, ValidHeaderLed);
			Cnt5 = UpdataLedStatusEx(Cnt5, tmp, 0x08, TxDoneLed);
			Cnt6 = UpdataLedStatusEx(Cnt6, tmp, 0x04, CADDoneLed);
			Cnt7 = UpdataLedStatusEx(Cnt7, tmp, 0x02, FHSSChannelLed);
			Cnt8 = UpdataLedStatusEx(Cnt8, tmp, 0x01, CADDetectLed);
		}

		private void ClearLedStatus()
		{
			if (RxTimeOutLed.LedColor != Color.Red)
				RxTimeOutLed.LedColor = Color.Lime;
			if (RxDoneLed.LedColor != Color.Red)
				RxDoneLed.LedColor = Color.Lime;
			if (CRCErrorLed.LedColor != Color.Red)
				CRCErrorLed.LedColor = Color.Lime;
			if (ValidHeaderLed.LedColor != Color.Red)
				ValidHeaderLed.LedColor = Color.Lime;
			if (TxDoneLed.LedColor != Color.Red)
				TxDoneLed.LedColor = Color.Lime;
			if (CADDoneLed.LedColor != Color.Red)
				CADDoneLed.LedColor = Color.Lime;
			if (FHSSChannelLed.LedColor != Color.Red)
				FHSSChannelLed.LedColor = Color.Lime;
			if (CADDetectLed.LedColor != Color.Red)
				CADDetectLed.LedColor = Color.Lime;
		}

		private void ChkPllLock()
		{
			byte data = 0x80;
			if (ChipVer == ucLoRa.ChipSet.RF92)
				ftdi.ReadByte(rfm92.RegHopChannel.Address, ref data);
			else
				ftdi.ReadByte(rfm92.RegHopChannel.Address, ref data);
			if ((data & 0x80) == 0x80)
				PllLockLed.LedColor = Color.Red;
			else
				PllLockLed.LedColor = Color.Lime;
		}

		public void SetAllValue()
		{
			rfstatus = ucLoRa.RFStatus.Standby;
			if (ChipVer == ucLoRa.ChipSet.RF92)
				rfm92.ApplyValue();
			else
				rfm96.ApplyValue();

			try
			{
				double num1 = (double)
					(
						(ChipVer != ucLoRa.ChipSet.RF92
						? (((uint)rfm96.RegFrMsb.Value << 16) + ((uint)rfm96.RegFrMid.Value << 8)) + (uint)rfm96.RegFrLsb.Value
						: (((uint)rfm92.RegFrMsb.Value << 16) + ((uint)rfm92.RegFrMid.Value << 8)) + (uint)rfm92.RegFrLsb.Value
						)
						+ 1
					) * (15625.0 / 256.0);
				nudRadioFreq.Value = 434000000M;	// !!! (Decimal)(num1);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
			if (((ChipVer != ucLoRa.ChipSet.RF92 ? rfm96.RegTcxo.Value : rfm92.RegTcxo.Value) & 16) == 16)
			{
				rbTCXO.Checked = true;
				rbXTAL.Checked = false;
			}
			else
			{
				rbTCXO.Checked = false;
				rbXTAL.Checked = true;
			}
			if (((ChipVer != ucLoRa.ChipSet.RF92 ? rfm96.RegPaConfig.Value : rfm92.RegPaConfig.Value) & 128) == 128)
				cbPaOutput.SelectedIndex = 1;
			else
				cbPaOutput.SelectedIndex = 0;

			if (ChipVer == ucLoRa.ChipSet.RF92)
			{
				cbPaRamp.SelectedIndex = (int)rfm92.RegPaRamp.Value & 15;
				if (cbPaOutput.SelectedIndex == 1)
				{
					if ((int)rfm92.RegPaDac.Value == 135)
						cbOutputPower.SelectedIndex = ((int)rfm92.RegPaConfig.Value & 15) + 3;
					else
						cbOutputPower.SelectedIndex = (int)rfm92.RegPaConfig.Value & 15;
				}
				else
					cbOutputPower.SelectedIndex = (int)rfm92.RegPaConfig.Value & 15;
				cbPLLBW.SelectedIndex = ((int)rfm92.RegPllHf.Value & 192) >> 6;
			}
			else
			{
				cbPaRamp.SelectedIndex = (int)rfm96.RegPaRamp.Value & 15;
				if (cbPaOutput.SelectedIndex == 1)
				{
					if (rfm96.RegPaDac.Value == 135)
						cbOutputPower.SelectedIndex = ((int)rfm96.RegPaConfig.Value & 15) + 3;
					else
						cbOutputPower.SelectedIndex = (int)rfm96.RegPaConfig.Value & 15;
				}
				else
				{
					cbMaxOutputPower.SelectedIndex = ((int)rfm96.RegPaConfig.Value & 112) >> 4;
					cbOutputPower.SelectedIndex = (int)rfm96.RegPaConfig.Value & 15;
				}
				cbPLLBW.SelectedIndex = ((int)rfm96.RegPllHf.Value & 192) >> 6;
			}
			byte num3 = ChipVer != ucLoRa.ChipSet.RF92 ? rfm96.RegOcp.Value : rfm92.RegOcp.Value;
			if ((num3 & 32) == 32)
			{
				rbOCPOn.Checked = true;
				rbOCPOff.Checked = false;
			}
			else
			{
				rbOCPOn.Checked = false;
				rbOCPOff.Checked = true;
			}
			switch (num3 & 0x1F)
			{
				case 0:
				case 1:
				case 2:
				case 3:
					cbOcpTrimming.SelectedIndex = 0;
					break;
				case 4:
				case 5:
				case 6:
				case 7:
					cbOcpTrimming.SelectedIndex = 1;
					break;
				case 8:
				case 9:
				case 10:
				case 11:
					cbOcpTrimming.SelectedIndex = 2;
					break;
				case 12:
				case 13:
				case 14:
				case 15:
					cbOcpTrimming.SelectedIndex = 3;
					break;
				case 16:
				case 17:
					cbOcpTrimming.SelectedIndex = 4;
					break;
				case 18:
				case 19:
					cbOcpTrimming.SelectedIndex = 5;
					break;
				case 20:
				case 21:
					cbOcpTrimming.SelectedIndex = 6;
					break;
				case 22:
				case 23:
					cbOcpTrimming.SelectedIndex = 7;
					break;
				case 24:
				case 25:
					cbOcpTrimming.SelectedIndex = 8;
					break;
				case 26:
				case 27:
				case 28:
				case 29:
				case 30:
				case 31:
					cbOcpTrimming.SelectedIndex = 9;
					break;
			}
			if (((ChipVer != ucLoRa.ChipSet.RF92 ? (int)rfm96.RegModemConfig3.Value : (int)rfm92.RegModemConfig2.Value) & 4) == 4)
			{
				rbAGCOn.Checked = true;
				rbAGCOff.Checked = false;
				cbLnaGain.Enabled = false;
			}
			else
			{
				rbAGCOn.Checked = false;
				rbAGCOff.Checked = true;
				cbLnaGain.Enabled = true;
			}
			cbLnaGain.SelectedIndex = ((ChipVer != ucLoRa.ChipSet.RF92 ? (int)rfm96.RegLna.Value : (int)rfm92.RegLna.Value) & 224) >> 5;
			byte num4 = ChipVer != ucLoRa.ChipSet.RF92 ? rfm96.RegModemConfig2.Value : rfm92.RegModemConfig2.Value;
			cbImplicit.Enabled = true;
			switch (num4 & 0xF0)
			{
				case 176:
					cbSF.SelectedIndex = 5;
					break;
				case 144:
					cbSF.SelectedIndex = 3;
					break;
				case 160:
					cbSF.SelectedIndex = 4;
					break;
				case 96:
					cbSF.SelectedIndex = 0;
					cbImplicit.Enabled = false;
					break;
				case 112:
					cbSF.SelectedIndex = 1;
					break;
				case 128:
					cbSF.SelectedIndex = 2;
					break;
				default:
					cbSF.SelectedIndex = 6;
					break;
			}
			switch ((ChipVer != ucLoRa.ChipSet.RF92 ? (int)(byte)((uint)rfm96.RegModemConfig1.Value >> 1) : (int)(byte)((uint)rfm92.RegModemConfig1.Value >> 3)) & 7)
			{
				case 2:
					cbCR.SelectedIndex = 1;
					break;
				case 3:
					cbCR.SelectedIndex = 2;
					break;
				case 4:
					cbCR.SelectedIndex = 3;
					break;
				default:
					cbCR.SelectedIndex = 0;
					break;
			}
			if (ChipVer == ucLoRa.ChipSet.RF92)
			{
				switch (rfm92.RegModemConfig1.Value & 0xC0)
				{
					case 64:
						cbBW.SelectedIndex = 1;
						break;
					case 128:
						cbBW.SelectedIndex = 2;
						break;
					default:
						cbBW.SelectedIndex = 0;
						break;
				}
			}
			else if (rbTCXO.Checked)
			{
				switch (rfm96.RegModemConfig1.Value & 0xF0)
				{
					case 128:
						cbBW.SelectedIndex = 8;
						break;
					case 144:
						cbBW.SelectedIndex = 9;
						break;
					case 80:
						cbBW.SelectedIndex = 5;
						break;
					case 96:
						cbBW.SelectedIndex = 6;
						break;
					case 32:
						cbBW.SelectedIndex = 2;
						break;
					case 48:
						cbBW.SelectedIndex = 3;
						break;
					case 64:
						cbBW.SelectedIndex = 4;
						break;
					case 0:
						cbBW.SelectedIndex = 0;
						break;
					case 16:
						cbBW.SelectedIndex = 1;
						break;
					default:
						cbBW.SelectedIndex = 7;
						break;
				}
			}
			else
			{
				switch (rfm96.RegModemConfig1.Value & 0xF0)
				{
					case 128:
						cbBW.SelectedIndex = 2;
						break;
					case 144:
						cbBW.SelectedIndex = 3;
						break;
					case 96:
						cbBW.SelectedIndex = 0;
						break;
					default:
						cbBW.SelectedIndex = 1;
						break;
				}
			}
			nudRxTimeOut.Value = ChipVer != ucLoRa.ChipSet.RF92 ? (Decimal)(((int)rfm96.RegModemConfig2.Value & 3) * 256 + (int)rfm96.RegSymbTimeoutLsb.Value) : (Decimal)(((int)rfm92.RegModemConfig2.Value & 3) * 256 + (int)rfm92.RegSymbTimeoutLsb.Value);
			if (ChipVer == ucLoRa.ChipSet.RF92)
			{
				if ((rfm92.RegModemConfig1.Value & 4) == 4)
				{
					cbImplicit.Checked = true;
					nudImplicitRxLength.Value = (Decimal)rfm92.RegPayloadLength.Value;
				}
				else
				{
					cbImplicit.Checked = false;
					nudImplicitRxLength.Value = (Decimal)rfm96.RegPayloadLength.Value;
				}
			}
			else
				cbImplicit.Checked = ((int)rfm96.RegModemConfig1.Value & 1) == 1;
			if (ChipVer == ucLoRa.ChipSet.RF92)
			{
				if ((rfm92.RegModemConfig1.Value & 1) == 1)
				{
					rbLROptimizeOn.Checked = true;
					rbLROptimizeOff.Checked = false;
				}
				else
				{
					rbLROptimizeOn.Checked = false;
					rbLROptimizeOff.Checked = true;
				}
			}
			else if ((rfm96.RegModemConfig3.Value & 8) == 8)
			{
				rbLROptimizeOn.Checked = true;
				rbLROptimizeOff.Checked = false;
			}
			else
			{
				rbLROptimizeOn.Checked = false;
				rbLROptimizeOff.Checked = true;
			}
			nudPreambleLength.Value = ChipVer != ucLoRa.ChipSet.RF92 ? (Decimal)((int)rfm96.RegPreambleMsb.Value * 256 + (int)rfm96.RegPreambleLsb.Value) : (Decimal)((int)rfm92.RegPreambleMsb.Value * 256 + (int)rfm92.RegPreambleLsb.Value);
			if (ChipVer == ucLoRa.ChipSet.RF92)
			{
				if ((rfm92.RegModemConfig1.Value & 2) == 2)
				{
					rbPayloadCRCOn.Checked = true;
					rbPayloadCRCOff.Checked = false;
				}
				else
				{
					rbPayloadCRCOn.Checked = false;
					rbPayloadCRCOff.Checked = true;
				}
			}
			else if ((rfm96.RegModemConfig2.Value & 4) == 4)
			{
				rbPayloadCRCOn.Checked = true;
				rbPayloadCRCOff.Checked = false;
			}
			else
			{
				rbPayloadCRCOn.Checked = false;
				rbPayloadCRCOff.Checked = true;
			}
			byte num5 = ChipVer != ucLoRa.ChipSet.RF92 ? rfm96.RegIrqFlagsMask.Value : rfm92.RegIrqFlagsMask.Value;
			if ((num5 & 0x80) == 0x80)
			{
				cbRxTimeOut.Checked = true;
				RxTimeOutLed.LedColor = Color.Red;
			}
			else
			{
				cbRxTimeOut.Checked = false;
				RxTimeOutLed.LedColor = Color.Lime;
			}
			if ((num5 & 0x40) == 0x40)
			{
				cbRxDone.Checked = true;
				RxDoneLed.LedColor = Color.Red;
			}
			else
			{
				cbRxDone.Checked = false;
				RxDoneLed.LedColor = Color.Lime;
			}
			if ((num5 & 0x20) == 0x20)
			{
				cbCRCError.Checked = true;
				CRCErrorLed.LedColor = Color.Red;
			}
			else
			{
				cbCRCError.Checked = false;
				CRCErrorLed.LedColor = Color.Lime;
			}
			if ((num5 & 0x10) == 0x10)
			{
				cbValidHeader.Checked = true;
				ValidHeaderLed.LedColor = Color.Red;
			}
			else
			{
				cbValidHeader.Checked = false;
				ValidHeaderLed.LedColor = Color.Lime;
			}
			if ((num5 & 0x08) == 0x08)
			{
				cbTxDone.Checked = true;
				TxDoneLed.LedColor = Color.Red;
			}
			else
			{
				cbTxDone.Checked = false;
				TxDoneLed.LedColor = Color.Lime;
			}
			if ((num5 & 0x04) == 0x04)
			{
				cbCADDone.Checked = true;
				CADDoneLed.LedColor = Color.Red;
			}
			else
			{
				cbCADDone.Checked = false;
				CADDoneLed.LedColor = Color.Lime;
			}
			if ((num5 & 0x02) == 0x02)
			{
				cbFHSSChannel.Checked = true;
				FHSSChannelLed.LedColor = Color.Red;
			}
			else
			{
				cbFHSSChannel.Checked = false;
				FHSSChannelLed.LedColor = Color.Lime;
			}
			if ((num5 & 0x01) == 0x01)
			{
				cbCADDetect.Checked = true;
				CADDetectLed.LedColor = Color.Red;
			}
			else
			{
				cbCADDetect.Checked = false;
				CADDetectLed.LedColor = Color.Lime;
			}
		}

		public void GetAllValue()
		{
			byte data = 0;
			if (ChipVer == ucLoRa.ChipSet.RF92)
			{
				ftdi.ReadByte(rfm92.RegFrMsb.Address, ref data);
				rfm92.RegFrMsb.Value = data;
				ftdi.ReadByte(rfm92.RegFrMid.Address, ref data);
				rfm92.RegFrMid.Value = data;
				ftdi.ReadByte(rfm92.RegFrLsb.Address, ref data);
				rfm92.RegFrLsb.Value = data;
				ftdi.ReadByte(rfm92.RegPaConfig.Address, ref data);
				rfm92.RegPaConfig.Value = data;
				ftdi.ReadByte(rfm92.RegPaRamp.Address, ref data);
				rfm92.RegPaRamp.Value = data;
				ftdi.ReadByte(rfm92.RegOcp.Address, ref data);
				rfm92.RegOcp.Value = data;
				ftdi.ReadByte(rfm92.RegLna.Address, ref data);
				rfm92.RegLna.Value = data;
				ftdi.ReadByte(rfm92.RegFifoAddrPtr.Address, ref data);
				rfm92.RegFifoAddrPtr.Value = data;
				ftdi.ReadByte(rfm92.RegFifoTxBaseAddr.Address, ref data);
				rfm92.RegFifoTxBaseAddr.Value = data;
				ftdi.ReadByte(rfm92.RegFifoRxBaseAddr.Address, ref data);
				rfm92.RegFifoRxBaseAddr.Value = data;
				ftdi.ReadByte(rfm92.RegFifoRxCurrentAddr.Address, ref data);
				rfm92.RegFifoRxCurrentAddr.Value = data;
				ftdi.ReadByte(rfm92.RegIrqFlagsMask.Address, ref data);
				rfm92.RegIrqFlagsMask.Value = data;
				ftdi.ReadByte(rfm92.RegIrqFlags.Address, ref data);
				rfm92.RegIrqFlags.Value = data;
				ftdi.ReadByte(rfm92.RegRxNbBytes.Address, ref data);
				rfm92.RegRxNbBytes.Value = data;
				ftdi.ReadByte(rfm92.RegRxHeaderCntValueMsb.Address, ref data);
				rfm92.RegRxHeaderCntValueMsb.Value = data;
				ftdi.ReadByte(rfm92.RegRxHeaderCntValueLsb.Address, ref data);
				rfm92.RegRxHeaderCntValueLsb.Value = data;
				ftdi.ReadByte(rfm92.RegRxPacketCntValueMsb.Address, ref data);
				rfm92.RegRxPacketCntValueMsb.Value = data;
				ftdi.ReadByte(rfm92.RegRxPacketCntValueLsb.Address, ref data);
				rfm92.RegRxPacketCntValueLsb.Value = data;
				ftdi.ReadByte(rfm92.RegModemStat.Address, ref data);
				rfm92.RegModemStat.Value = data;
				ftdi.ReadByte(rfm92.RegPktSnrValue.Address, ref data);
				rfm92.RegPktSnrValue.Value = data;
				ftdi.ReadByte(rfm92.RegPktRssiValue.Address, ref data);
				rfm92.RegPktRssiValue.Value = data;
				ftdi.ReadByte(rfm92.RegRssiValue.Address, ref data);
				rfm92.RegRssiValue.Value = data;
				ftdi.ReadByte(rfm92.RegHopChannel.Address, ref data);
				rfm92.RegHopChannel.Value = data;
				ftdi.ReadByte(rfm92.RegModemConfig1.Address, ref data);
				rfm92.RegModemConfig1.Value = data;
				ftdi.ReadByte(rfm92.RegModemConfig2.Address, ref data);
				rfm92.RegModemConfig2.Value = data;
				ftdi.ReadByte(rfm92.RegSymbTimeoutLsb.Address, ref data);
				rfm92.RegSymbTimeoutLsb.Value = data;
				ftdi.ReadByte(rfm92.RegPreambleMsb.Address, ref data);
				rfm92.RegPreambleMsb.Value = data;
				ftdi.ReadByte(rfm92.RegPreambleLsb.Address, ref data);
				rfm92.RegPreambleLsb.Value = data;
				ftdi.ReadByte(rfm92.RegPayloadLength.Address, ref data);
				rfm92.RegPayloadLength.Value = data;
				ftdi.ReadByte(rfm92.RegMaxPayloadLength.Address, ref data);
				rfm92.RegMaxPayloadLength.Value = data;
				ftdi.ReadByte(rfm92.RegHopPeriod.Address, ref data);
				rfm92.RegHopPeriod.Value = data;
				ftdi.ReadByte(rfm92.RegFifoRxByteAddr.Address, ref data);
				rfm92.RegFifoRxByteAddr.Value = data;
				ftdi.ReadByte(rfm92.RegVersion.Address, ref data);
				rfm92.RegVersion.Value = data;
				ftdi.ReadByte(rfm92.RegTcxo.Address, ref data);
				rfm92.RegTcxo.Value = data;
				ftdi.ReadByte(rfm92.RegPaDac.Address, ref data);
				rfm92.RegPaDac.Value = data;
				ftdi.ReadByte(rfm92.RegPllHf.Address, ref data);
				rfm92.RegPllHf.Value = data;
				rfm92.ApplyValue();
			}
			else
			{
				ftdi.ReadByte(rfm96.RegFrMsb.Address, ref data);
				rfm96.RegFrMsb.Value = data;
				ftdi.ReadByte(rfm96.RegFrMid.Address, ref data);
				rfm96.RegFrMid.Value = data;
				ftdi.ReadByte(rfm96.RegFrLsb.Address, ref data);
				rfm96.RegFrLsb.Value = data;
				ftdi.ReadByte(rfm96.RegPaConfig.Address, ref data);
				rfm96.RegPaConfig.Value = data;
				ftdi.ReadByte(rfm96.RegPaRamp.Address, ref data);
				rfm96.RegPaRamp.Value = data;
				ftdi.ReadByte(rfm96.RegOcp.Address, ref data);
				rfm96.RegOcp.Value = data;
				ftdi.ReadByte(rfm96.RegLna.Address, ref data);
				rfm96.RegLna.Value = data;
				ftdi.ReadByte(rfm96.RegFifoAddrPtr.Address, ref data);
				rfm96.RegFifoAddrPtr.Value = data;
				ftdi.ReadByte(rfm96.RegFifoTxBaseAddr.Address, ref data);
				rfm96.RegFifoTxBaseAddr.Value = data;
				ftdi.ReadByte(rfm96.RegFifoRxBaseAddr.Address, ref data);
				rfm96.RegFifoRxBaseAddr.Value = data;
				ftdi.ReadByte(rfm96.RegFifoRxCurrentAddr.Address, ref data);
				rfm96.RegFifoRxCurrentAddr.Value = data;
				ftdi.ReadByte(rfm96.RegIrqFlagsMask.Address, ref data);
				rfm96.RegIrqFlagsMask.Value = data;
				ftdi.ReadByte(rfm96.RegIrqFlags.Address, ref data);
				rfm96.RegIrqFlags.Value = data;
				ftdi.ReadByte(rfm96.RegRxNbBytes.Address, ref data);
				rfm96.RegRxNbBytes.Value = data;
				ftdi.ReadByte(rfm96.RegRxHeaderCntValueMsb.Address, ref data);
				rfm96.RegRxHeaderCntValueMsb.Value = data;
				ftdi.ReadByte(rfm96.RegRxHeaderCntValueLsb.Address, ref data);
				rfm96.RegRxHeaderCntValueLsb.Value = data;
				ftdi.ReadByte(rfm96.RegRxPacketCntValueMsb.Address, ref data);
				rfm96.RegRxPacketCntValueMsb.Value = data;
				ftdi.ReadByte(rfm96.RegRxPacketCntValueLsb.Address, ref data);
				rfm96.RegRxPacketCntValueLsb.Value = data;
				ftdi.ReadByte(rfm96.RegModemStat.Address, ref data);
				rfm96.RegModemStat.Value = data;
				ftdi.ReadByte(rfm96.RegPktSnrValue.Address, ref data);
				rfm96.RegPktSnrValue.Value = data;
				ftdi.ReadByte(rfm96.RegPktRssiValue.Address, ref data);
				rfm96.RegPktRssiValue.Value = data;
				ftdi.ReadByte(rfm96.RegRssiValue.Address, ref data);
				rfm96.RegRssiValue.Value = data;
				ftdi.ReadByte(rfm96.RegHopChannel.Address, ref data);
				rfm96.RegHopChannel.Value = data;
				ftdi.ReadByte(rfm96.RegModemConfig1.Address, ref data);
				rfm96.RegModemConfig1.Value = data;
				ftdi.ReadByte(rfm96.RegModemConfig2.Address, ref data);
				rfm96.RegModemConfig2.Value = data;
				ftdi.ReadByte(rfm96.RegSymbTimeoutLsb.Address, ref data);
				rfm96.RegSymbTimeoutLsb.Value = data;
				ftdi.ReadByte(rfm96.RegPreambleMsb.Address, ref data);
				rfm96.RegPreambleMsb.Value = data;
				ftdi.ReadByte(rfm96.RegPreambleLsb.Address, ref data);
				rfm96.RegPreambleLsb.Value = data;
				ftdi.ReadByte(rfm96.RegPayloadLength.Address, ref data);
				rfm96.RegPayloadLength.Value = data;
				ftdi.ReadByte(rfm96.RegMaxPayloadLength.Address, ref data);
				rfm96.RegMaxPayloadLength.Value = data;
				ftdi.ReadByte(rfm96.RegHopPeriod.Address, ref data);
				rfm96.RegHopPeriod.Value = data;
				ftdi.ReadByte(rfm96.RegFifoRxByteAddr.Address, ref data);
				rfm96.RegFifoRxByteAddr.Value = data;
				ftdi.ReadByte(rfm96.RegVersion.Address, ref data);
				rfm96.RegVersion.Value = data;
				ftdi.ReadByte(rfm96.RegTcxo.Address, ref data);
				rfm96.RegTcxo.Value = data;
				ftdi.ReadByte(rfm96.RegPaDac.Address, ref data);
				rfm96.RegPaDac.Value = data;
				ftdi.ReadByte(rfm96.RegPllHf.Address, ref data);
				rfm96.RegPllHf.Value = data;
				rfm96.ApplyValue();
			}
			UpdatecbBW();
		}

		public void InitAllValue()
		{
			bbPayload.ContextMenuStrip.Enabled = false;
			if (ChipVer != ucLoRa.ChipSet.RF92 && ChipVer != ucLoRa.ChipSet.RF96)
				ChipVer = ucLoRa.ChipSet.RF96;

			if (ChipVer == ucLoRa.ChipSet.RF92)
				rfm92.RegOpMode.Value = 129;
			else if (nudRadioFreq.Value < new Decimal(525000000))
				rfm96.RegOpMode.Value = 137;
			else
				rfm96.RegOpMode.Value = 129;
		}

		public void UpdataAllValue()
		{
			uint num1 = (uint)((double)nudRadioFreq.Value / (15625.0 / 256.0));
			if (ChipVer == ucLoRa.ChipSet.RF92)
			{
				rfm92.RegFrLsb.Value = (byte)num1;
				rfm92.RegFrMid.Value = (byte)(num1 >> 8);
				rfm92.RegFrMsb.Value = (byte)(num1 >> 16);
				if (rbTCXO.Checked)
					rfm92.RegTcxo.Value |= 16;
				else
					rfm92.RegTcxo.Value &= 239;
				rfm92.RegPaConfig.Value = 0;
				switch (cbPaOutput.SelectedIndex)
				{
					case 1:
						rfm92.RegPaConfig.Value |= 0x80;
						if (cbOutputPower.SelectedIndex > 15)
						{
							rfm92.RegPaConfig.Value |= (byte)(cbOutputPower.SelectedIndex - 3);
							rfm92.RegPaDac.Value = 135;
							break;
						}
						rfm92.RegPaConfig.Value |= (byte)cbOutputPower.SelectedIndex;
						rfm92.RegPaDac.Value = 132;
						break;
					default:
						rfm92.RegPaConfig.Value &= 127;
						rfm92.RegPaConfig.Value |= (byte)cbOutputPower.SelectedIndex;
						rfm92.RegPaDac.Value = 132;
						break;
				}
				rfm92.RegPaRamp.Value &= 240;
				rfm92.RegPaRamp.Value |= (byte)cbPaRamp.SelectedIndex;
				rfm92.RegPllHf.Value &= 63;
				rfm92.RegPllHf.Value |= (byte)(cbPLLBW.SelectedIndex << 6);
				if (rbOCPOn.Checked)
				{
					rfm92.RegOcp.Value &= 192;
					rfm92.RegOcp.Value |= 32;
					switch (cbOcpTrimming.SelectedIndex)
					{
						case 0:
							rfm92.RegOcp.Value |= 3;
							break;
						case 1:
							rfm92.RegOcp.Value |= 7;
							break;
						case 2:
							rfm92.RegOcp.Value |= 11;
							break;
						case 3:
							rfm92.RegOcp.Value |= 15;
							break;
						case 4:
							rfm92.RegOcp.Value |= 17;
							break;
						case 5:
							rfm92.RegOcp.Value |= 19;
							break;
						case 6:
							rfm92.RegOcp.Value |= 21;
							break;
						case 7:
							rfm92.RegOcp.Value |= 23;
							break;
						case 8:
							rfm92.RegOcp.Value |= 25;
							break;
						default:
							rfm92.RegOcp.Value |= 27;
							break;
					}
				}
				else
					rfm92.RegOcp.Value &= 223;
				if (rbAGCOn.Checked)
					rfm92.RegModemConfig2.Value |= 4;
				else
					rfm92.RegModemConfig2.Value &= 251;
				rfm92.RegLna.Value &= 31;
				rfm92.RegLna.Value |= (byte)(cbLnaGain.SelectedIndex << 5);
				rfm92.RegLna.Value &= 252;
				if (rbLNAOn.Checked)
					rfm92.RegLna.Value |= 3;
				rfm92.RegModemConfig2.Value &= 15;
				rfm92.RegModemConfig2.Value |= (byte)(cbSF.SelectedIndex + 6 << 4);
				rfm92.RegModemConfig1.Value &= 7;
				switch (cbCR.SelectedIndex)
				{
					case 0:
						rfm92.RegModemConfig1.Value |= 8;
						break;
					case 1:
						rfm92.RegModemConfig1.Value |= 16;
						break;
					case 2:
						rfm92.RegModemConfig1.Value |= 24;
						break;
					case 3:
						rfm92.RegModemConfig1.Value |= 32;
						break;
				}
				switch (cbBW.SelectedIndex)
				{
					case 1:
						rfm92.RegModemConfig1.Value |= 64;
						break;
					case 2:
						rfm92.RegModemConfig1.Value |= 0x80;
						break;
				}
				if (rbLROptimizeOn.Checked)
					rfm92.RegModemConfig1.Value |= 1;
				else
					rfm92.RegModemConfig1.Value &= 254;
				if (rbPayloadCRCOn.Checked)
					rfm92.RegModemConfig1.Value |= 2;
				else
					rfm92.RegModemConfig1.Value &= 253;
				if (cbImplicit.Checked)
					rfm92.RegModemConfig1.Value |= 4;
				else
					rfm92.RegModemConfig1.Value &= 251;
				rfm92.RegSymbTimeoutLsb.Value = (byte)nudRxTimeOut.Value;
				rfm92.RegModemConfig2.Value &= 252;
				rfm92.RegModemConfig2.Value |= (byte)((uint)nudRxTimeOut.Value >> 8 & 3U);
				rfm92.RegPreambleLsb.Value = (byte)nudPreambleLength.Value;
				rfm92.RegPreambleMsb.Value = (byte)((uint)nudPreambleLength.Value >> 8);
			}
			else
			{
				rfm96.RegFrLsb.Value = (byte)num1;
				rfm96.RegFrMid.Value = (byte)(num1 >> 8);
				rfm96.RegFrMsb.Value = (byte)(num1 >> 16);
				if (rbTCXO.Checked)
					rfm96.RegTcxo.Value |= 16;
				else
					rfm96.RegTcxo.Value &= 239;
				rfm96.RegPaConfig.Value = 0;
				switch (cbPaOutput.SelectedIndex)
				{
					case 1:
						rfm96.RegPaConfig.Value |= 240;
						if (cbOutputPower.SelectedIndex > 15)
						{
							rfm96.RegPaConfig.Value |= (byte)(cbOutputPower.SelectedIndex - 3);
							rfm96.RegPaDac.Value = 135;
							break;
						}
						rfm96.RegPaConfig.Value |= (byte)cbOutputPower.SelectedIndex;
						rfm96.RegPaDac.Value = 132;
						break;
					default:
						rfm96.RegPaConfig.Value &= 127;
						rfm96.RegPaConfig.Value |= (byte)(cbMaxOutputPower.SelectedIndex << 4);
						rfm96.RegPaConfig.Value |= (byte)cbOutputPower.SelectedIndex;
						rfm96.RegPaDac.Value = 132;
						break;
				}
				rfm96.RegPaRamp.Value |= (byte)cbPaRamp.SelectedIndex;
				rfm96.RegPllHf.Value &= 63;
				rfm96.RegPllHf.Value |= (byte)(cbPLLBW.SelectedIndex << 6);
				if (rbOCPOn.Checked)
				{
					rfm96.RegOcp.Value &= 192;
					rfm96.RegOcp.Value |= 32;
					switch (cbOcpTrimming.SelectedIndex)
					{
						case 0:
							rfm96.RegOcp.Value |= 3;
							break;
						case 1:
							rfm96.RegOcp.Value |= 7;
							break;
						case 2:
							rfm96.RegOcp.Value |= 11;
							break;
						case 3:
							rfm96.RegOcp.Value |= 15;
							break;
						case 4:
							rfm96.RegOcp.Value |= 17;
							break;
						case 5:
							rfm96.RegOcp.Value |= 19;
							break;
						case 6:
							rfm96.RegOcp.Value |= 21;
							break;
						case 7:
							rfm96.RegOcp.Value |= 23;
							break;
						case 8:
							rfm96.RegOcp.Value |= 25;
							break;
						default:
							rfm96.RegOcp.Value |= 27;
							break;
					}
				}
				else
					rfm96.RegOcp.Value &= 223;
				if (rbAGCOn.Checked)
					rfm96.RegModemConfig3.Value |= 4;
				else
					rfm96.RegModemConfig3.Value &= 251;
				rfm96.RegLna.Value &= 31;
				rfm96.RegLna.Value |= (byte)(cbLnaGain.SelectedIndex << 5);
				rfm96.RegLna.Value &= 252;
				if (rbLNAOn.Checked)
					rfm96.RegLna.Value |= 3;
				rfm96.RegModemConfig2.Value &= 15;
				rfm96.RegModemConfig2.Value |= (byte)(cbSF.SelectedIndex + 6 << 4);
				rfm96.RegModemConfig1.Value &= 1;
				switch (cbCR.SelectedIndex)
				{
					case 0:
						rfm96.RegModemConfig1.Value |= 2;
						break;
					case 1:
						rfm96.RegModemConfig1.Value |= 4;
						break;
					case 2:
						rfm96.RegModemConfig1.Value |= 6;
						break;
					case 3:
						rfm96.RegModemConfig1.Value |= 8;
						break;
				}
				if (rbTCXO.Checked)
				{
					rfm96.RegModemConfig1.Value |= (byte)(cbBW.SelectedIndex << 4);
				}
				else
				{
					switch (cbBW.SelectedIndex)
					{
						case 0:
							rfm96.RegModemConfig1.Value |= 96;
							break;
						case 2:
							rfm96.RegModemConfig1.Value |= 0x80;
							break;
						case 3:
							rfm96.RegModemConfig1.Value |= 144;
							break;
						default:
							rfm96.RegModemConfig1.Value |= 112;
							break;
					}
				}
				if (cbImplicit.Checked)
					rfm96.RegModemConfig1.Value |= 1;
				else
					rfm96.RegModemConfig1.Value &= 254;
				if (rbLROptimizeOn.Checked)
					rfm96.RegModemConfig3.Value |= 8;
				else
					rfm96.RegModemConfig3.Value &= 247;
				if (rbPayloadCRCOn.Checked)
					rfm96.RegModemConfig2.Value |= 4;
				else
					rfm96.RegModemConfig2.Value &= 251;
				try
				{
					rfm96.RegSymbTimeoutLsb.Value = (byte)nudRxTimeOut.Value;
				}
				catch
				{
				}
				rfm96.RegModemConfig2.Value &= 252;
				rfm96.RegModemConfig2.Value |= (byte)((uint)nudRxTimeOut.Value >> 8 & 3U);
				rfm96.RegPreambleLsb.Value = (byte)nudPreambleLength.Value;
				rfm96.RegPreambleMsb.Value = (byte)((uint)nudPreambleLength.Value >> 8);
			}
			byte num2 = 0;
			if (cbRxTimeOut.Checked)
				num2 |= 0x80;
			if (cbRxDone.Checked)
				num2 |= 64;
			if (cbCRCError.Checked)
				num2 |= 32;
			if (cbValidHeader.Checked)
				num2 |= 16;
			if (cbTxDone.Checked)
				num2 |= 8;
			if (cbCADDone.Checked)
				num2 |= 4;
			if (cbFHSSChannel.Checked)
				num2 |= 2;
			if (cbCADDetect.Checked)
				num2 |= 1;
			if (ChipVer == ucLoRa.ChipSet.RF92)
				rfm92.RegIrqFlagsMask.Value = num2;
			else
				rfm96.RegIrqFlagsMask.Value = num2;
			if (ChipVer == ucLoRa.ChipSet.RF92)
				rfm92.ApplyValue();
			else
				rfm96.ApplyValue();
		}

		private void OnFrequencyRfChanged(Decimal value)
		{
			if (FrequencyRfChanged == null)
				return;
			FrequencyRfChanged(this, new DecimalEventArg(value));
		}

		public bool TryConnect()
		{
			if (!ftdi.Open() || !GetVersion())
				return false;
			if (ChipVer == ucLoRa.ChipSet.RF92)
			{
				if (!ftdi.HReset())
					return false;
			}
			else if (!ftdi.LReset())
				return false;

			EntrySleep();
			EntryLoRa();
			GetAllValue();
			SetAllValue();
			ftdi.LedOff();
			ftdi.BeepOn();
			ftdi.LCDFull();
			Thread.Sleep(50);
			ftdi.BeepOff();
			Thread.Sleep(150);
			ftdi.LCDClear();
			Thread.Sleep(50);
			LCDInitShow();
			ftdi.LCDDisplay();
			Thread.Sleep(50);
			return true;
		}

		public bool GetVersion()
		{
			byte data = 0;
			if (!ftdi.ReadByte(rfm92.RegVersion.Address, ref data))
				return false;
			ChipVer = (ucLoRa.ChipSet)data;
			return true;
		}

		public void EntryLoRa()
		{
			rfstatus = ucLoRa.RFStatus.Sleep;
			if (ChipVer == ucLoRa.ChipSet.RF92)
			{
				rfm92.RegOpMode.Value = (byte)(0x80 | (int)rfstatus);
				ftdi.SendByte(rfm92.RegOpMode.Address, rfm92.RegOpMode.Value);
			}
			else
			{
				rfm96.RegOpMode.Value |= (byte)(0x80 | (int)rfstatus);
				if (nudRadioFreq.Value < new Decimal(525000000))
					rfm96.RegOpMode.Value |= 8;
				else
					rfm96.RegOpMode.Value &= 0xF7;
				ftdi.SendByte(rfm96.RegOpMode.Address, rfm96.RegOpMode.Value);
			}
		}

		public void EntryFSK()
		{
			rfstatus = ucLoRa.RFStatus.Sleep;
			if (ChipVer == ucLoRa.ChipSet.RF92)
			{
				rfm92.RegOpMode.Value = (byte)rfstatus;
				ftdi.SendByte(rfm92.RegOpMode.Address, rfm92.RegOpMode.Value);
			}
			else
			{
				rfm96.RegOpMode.Value &= 127;
				rfm96.RegOpMode.Value |= (byte)rfstatus;
				if (nudRadioFreq.Value < new Decimal(525000000))
					rfm96.RegOpMode.Value |= 8;
				else
					rfm96.RegOpMode.Value &= 247;
				ftdi.SendByte(rfm96.RegOpMode.Address, rfm96.RegOpMode.Value);
			}
		}

		public void EntrySleep()
		{
			byte data = 0;
			if (ChipVer == ucLoRa.ChipSet.RF92)
			{
				ftdi.ReadByte(rfm92.RegOpMode.Address, ref data);
				data &= 248;
				rfstatus = ucLoRa.RFStatus.Sleep;
				rfm92.RegOpMode.Value = (byte)((uint)data | (uint)rfstatus);
				ftdi.SendByte(rfm92.RegOpMode.Address, rfm92.RegOpMode.Value);
			}
			else
			{
				ftdi.ReadByte(rfm96.RegOpMode.Address, ref data);
				data &= 248;
				rfstatus = ucLoRa.RFStatus.Sleep;
				rfm96.RegOpMode.Value = (byte)((uint)data | (uint)rfstatus);
				if (nudRadioFreq.Value < new Decimal(525000000))
					rfm96.RegOpMode.Value |= 8;
				else
					rfm96.RegOpMode.Value &= 247;
				ftdi.SendByte(rfm96.RegOpMode.Address, rfm96.RegOpMode.Value);
			}
		}

		public void EntryStandby()
		{
			byte data = 0;
			rfstatus = ucLoRa.RFStatus.Standby;
			if (ChipVer == ucLoRa.ChipSet.RF92)
			{
				ftdi.ReadByte(rfm92.RegOpMode.Address, ref data);
				rfm92.RegOpMode.Value = (byte)((data & 248) | (byte)rfstatus);
				ftdi.SendByte(rfm92.RegOpMode.Address, rfm92.RegOpMode.Value);
			}
			else
			{
				ftdi.ReadByte(rfm96.RegOpMode.Address, ref data);
				rfm96.RegOpMode.Value = (byte)((data & 248) | (byte)rfstatus);
				if (nudRadioFreq.Value < new Decimal(525000000))
					rfm96.RegOpMode.Value |= 8;
				else
					rfm96.RegOpMode.Value &= 247;
				ftdi.SendByte(rfm96.RegOpMode.Address, rfm96.RegOpMode.Value);
			}
		}

		public bool EntryRx()
		{
			byte data = 0;
			ConfigRF();
			ClearIrq();
			rfstatus = ucLoRa.RFStatus.Receiver;
			if (ChipVer == ucLoRa.ChipSet.RF92)
			{
				ftdi.SendByte(rfm92.RegIrqFlagsMask.Address, rfm92.RegIrqFlagsMask.Value);
				ftdi.ReadByte(rfm92.RegFifoRxBaseAddr.Address, ref data);
				rfm92.RegFifoAddrPtr.Value = data;
				ftdi.SendByte(rfm92.RegFifoAddrPtr.Address, rfm92.RegFifoAddrPtr.Value);
				if (cbImplicit.Checked)
				{
					rfm92.RegPayloadLength.Value = (byte)nudImplicitRxLength.Value;
					ftdi.SendByte(rfm92.RegPayloadLength.Address, rfm92.RegPayloadLength.Value);
				}
				if (cbSF.SelectedIndex == 0)
				{
					ftdi.ReadByte(49, ref data);
					ftdi.SendByte(49, (byte)((data & 248) | 5));
					ftdi.SendByte(55, 12);
				}
				ftdi.ReadByte(rfm92.RegOpMode.Address, ref data);
				rfm92.RegOpMode.Value = (byte)((data & 248) | (byte)rfstatus);
				ftdi.SendByte(rfm92.RegOpMode.Address, rfm92.RegOpMode.Value);
				Thread.Sleep(5);
				ftdi.ReadByte(rfm92.RegModemStat.Address, ref data);
				return ((data & 4) == 4);
			}
			ftdi.SendByte(rfm96.RegIrqFlagsMask.Address, rfm96.RegIrqFlagsMask.Value);
			ftdi.ReadByte(rfm96.RegFifoRxBaseAddr.Address, ref data);
			rfm96.RegFifoAddrPtr.Value = data;
			ftdi.SendByte(rfm96.RegFifoAddrPtr.Address, rfm96.RegFifoAddrPtr.Value);
			if (cbImplicit.Checked)
			{
				rfm96.RegPayloadLength.Value = (byte)nudImplicitRxLength.Value;
				ftdi.SendByte(rfm96.RegPayloadLength.Address, rfm96.RegPayloadLength.Value);
			}
			if (cbSF.SelectedIndex == 0)
			{
				byte data3 = 0;
				ftdi.ReadByte(49, ref data3);
				data3 &= 248;
				data3 |= 5;
				ftdi.SendByte(49, data3);
				ftdi.SendByte(55, 12);
			}
			ftdi.ReadByte(rfm96.RegOpMode.Address, ref data);
			rfm96.RegOpMode.Value = (byte)((data & 248) | (byte)rfstatus);
			if (nudRadioFreq.Value < new Decimal(525000000))
				rfm96.RegOpMode.Value |= 8;
			else
				rfm96.RegOpMode.Value &= 247;
			ftdi.SendByte(rfm96.RegOpMode.Address, rfm96.RegOpMode.Value);
			Thread.Sleep(5);
			ftdi.ReadByte(rfm96.RegModemStat.Address, ref data);
			return (data & 4) == 4;
		}

		public void EntryCAD()
		{
			byte data = 0;
			ConfigRF();
			ClearIrq();
			rfstatus = ucLoRa.RFStatus.CADdetect;
			if (ChipVer == ucLoRa.ChipSet.RF92)
			{
				ftdi.ReadByte(rfm92.RegOpMode.Address, ref data);
				rfm92.RegOpMode.Value = (byte)((data & 248) | (byte)rfstatus);
				ftdi.SendByte(rfm92.RegOpMode.Address, rfm92.RegOpMode.Value);
			}
			else
			{
				ftdi.ReadByte(rfm96.RegOpMode.Address, ref data);
				rfm96.RegOpMode.Value = (byte)((data & 248) | (byte)rfstatus);
				if (nudRadioFreq.Value < new Decimal(525000000))
					rfm96.RegOpMode.Value |= 8;
				else
					rfm96.RegOpMode.Value &= 247;
				ftdi.SendByte(rfm96.RegOpMode.Address, rfm96.RegOpMode.Value);
			}
		}

		public void RemainCAD()
		{
			if (ChipVer == ucLoRa.ChipSet.RF92)
				ftdi.SendByte(rfm92.RegOpMode.Address, rfm92.RegOpMode.Value);
			else
				ftdi.SendByte(rfm96.RegOpMode.Address, rfm96.RegOpMode.Value);
		}

		public bool EntryTx()
		{
			byte data = 0;

			ConfigRF();
			ClearIrq();
			byte txData = GetTxData(bbPayload.Text);
			if (txData != 0)
			{
				rfstatus = ucLoRa.RFStatus.Transmitter;
				if (ChipVer == ucLoRa.ChipSet.RF92)
				{
					ftdi.SendByte(rfm92.RegIrqFlagsMask.Address, rfm92.RegIrqFlagsMask.Value);
					rfm92.RegPayloadLength.Value = txData;
					ftdi.SendByte(rfm92.RegPayloadLength.Address, rfm92.RegPayloadLength.Value);
					ftdi.ReadByte(rfm92.RegFifoTxBaseAddr.Address, ref data);
					rfm92.RegFifoAddrPtr.Value = data;
					ftdi.SendByte(rfm92.RegFifoAddrPtr.Address, rfm92.RegFifoAddrPtr.Value);
					if (ftdi.SendBytes(rfm92.RegFifo.Address, TxBuf, txData))
					{
						rfm92.RegOpMode.Value &= 248;
						rfm92.RegOpMode.Value |= (byte)rfstatus;
						ftdi.SendByte(rfm92.RegOpMode.Address, rfm92.RegOpMode.Value);
					}
					else
					{
						rfstatus = ucLoRa.RFStatus.Standby;
						rfm92.RegOpMode.Value &= 248;
						rfm92.RegOpMode.Value |= (byte)rfstatus;
						ftdi.LedOff();
						return false;
					}
				}
				else
				{
					ftdi.SendByte(rfm96.RegIrqFlagsMask.Address, rfm96.RegIrqFlagsMask.Value);
					rfm96.RegPayloadLength.Value = txData;
					ftdi.SendByte(rfm96.RegPayloadLength.Address, rfm96.RegPayloadLength.Value);
					ftdi.ReadByte(rfm96.RegFifoTxBaseAddr.Address, ref data);
					rfm96.RegFifoAddrPtr.Value = data;
					ftdi.SendByte(rfm96.RegFifoAddrPtr.Address, rfm96.RegFifoAddrPtr.Value);
					ftdi.ReadByte(rfm96.RegOpMode.Address, ref data);
					rfm96.RegOpMode.Value = (byte)((data & 248) | (byte)rfstatus);
					if (nudRadioFreq.Value < new Decimal(525000000))
						rfm96.RegOpMode.Value |= 8;
					else
						rfm96.RegOpMode.Value &= 247;
					if (ftdi.SendBytes(rfm96.RegFifo.Address, TxBuf, txData))
					{
						ftdi.SendByte(rfm96.RegOpMode.Address, rfm96.RegOpMode.Value);
					}
					else
					{
						rfstatus = ucLoRa.RFStatus.Standby;
						rfm96.RegOpMode.Value &= 248;
						rfm96.RegOpMode.Value |= (byte)rfstatus;
						ftdi.LedOff();
						return false;
					}
				}
				return true;
			}
			MessageBox.Show("No data!");
			return false;
		}

		public void TxAgain()
		{
			byte data = 0;

			byte txData = GetTxData(bbPayload.Text);
			if ((int)txData != 0)
			{
				rfstatus = ucLoRa.RFStatus.Transmitter;
				if (ChipVer == ucLoRa.ChipSet.RF92)
				{
					ftdi.SendByte(rfm92.RegIrqFlagsMask.Address, rfm92.RegIrqFlagsMask.Value);
					rfm92.RegPayloadLength.Value = txData;
					ftdi.SendByte(rfm92.RegPayloadLength.Address, rfm92.RegPayloadLength.Value);
					ftdi.ReadByte(rfm92.RegFifoTxBaseAddr.Address, ref data);
					rfm92.RegFifoAddrPtr.Value = data;
					ftdi.SendByte(rfm92.RegFifoAddrPtr.Address, rfm92.RegFifoAddrPtr.Value);
					if (ftdi.SendBytes(rfm92.RegFifo.Address, TxBuf, txData))
					{
						rfm92.RegOpMode.Value &= 248;
						rfm92.RegOpMode.Value |= (byte)rfstatus;
						ftdi.SendByte(rfm92.RegOpMode.Address, rfm92.RegOpMode.Value);
					}
					else
					{
						rfstatus = ucLoRa.RFStatus.Standby;
						rfm92.RegOpMode.Value &= 248;
						rfm92.RegOpMode.Value |= (byte)rfstatus;
						ftdi.LedOff();
					}
				}
				else
				{
					ftdi.SendByte(rfm96.RegIrqFlagsMask.Address, rfm96.RegIrqFlagsMask.Value);
					rfm96.RegPayloadLength.Value = txData;
					ftdi.SendByte(rfm96.RegPayloadLength.Address, rfm96.RegPayloadLength.Value);
					ftdi.ReadByte(rfm96.RegFifoTxBaseAddr.Address, ref data);
					rfm96.RegFifoAddrPtr.Value = data;
					ftdi.SendByte(rfm96.RegFifoAddrPtr.Address, rfm96.RegFifoAddrPtr.Value);
					ftdi.ReadByte(rfm96.RegOpMode.Address, ref data);
					rfm96.RegOpMode.Value = (byte)((data & 248) | (byte)rfstatus);
					if (nudRadioFreq.Value < new Decimal(525000000))
						rfm96.RegOpMode.Value |= 8;
					else
						rfm96.RegOpMode.Value &= 247;
					if (ftdi.SendBytes(rfm96.RegFifo.Address, TxBuf, txData))
					{
						ftdi.SendByte(rfm96.RegOpMode.Address, rfm96.RegOpMode.Value);
					}
					else
					{
						rfstatus = ucLoRa.RFStatus.Standby;
						rfm96.RegOpMode.Value &= 248;
						rfm96.RegOpMode.Value |= (byte)rfstatus;
						ftdi.LedOff();
					}
				}
			}
			else
			{
				MessageBox.Show("No data!");
			}
		}

		public void ConfigRF()
		{
			UpdataAllValue();
			if (ChipVer == ucLoRa.ChipSet.RF92)
			{
				ftdi.HReset();
				EntrySleep();
				EntryLoRa();
				EntryStandby();
				ftdi.SendByte(rfm92.RegFrMsb.Address, rfm92.RegFrMsb.Value);
				ftdi.SendByte(rfm92.RegFrMid.Address, rfm92.RegFrMid.Value);
				ftdi.SendByte(rfm92.RegFrLsb.Address, rfm92.RegFrLsb.Value);
				ftdi.SendByte(rfm92.RegPaConfig.Address, rfm92.RegPaConfig.Value);
				ftdi.SendByte(rfm92.RegPaRamp.Address, rfm92.RegPaRamp.Value);
				ftdi.SendByte(rfm92.RegOcp.Address, rfm92.RegOcp.Value);
				ftdi.SendByte(rfm92.RegLna.Address, rfm92.RegLna.Value);
				ftdi.SendByte(rfm92.RegIrqFlagsMask.Address, rfm92.RegIrqFlagsMask.Value);
				ftdi.SendByte(rfm92.RegModemConfig1.Address, rfm92.RegModemConfig1.Value);
				ftdi.SendByte(rfm92.RegModemConfig2.Address, rfm92.RegModemConfig2.Value);
				ftdi.SendByte(rfm92.RegSymbTimeoutLsb.Address, rfm92.RegSymbTimeoutLsb.Value);
				ftdi.SendByte(rfm92.RegPreambleMsb.Address, rfm92.RegPreambleMsb.Value);
				ftdi.SendByte(rfm92.RegPreambleLsb.Address, rfm92.RegPreambleLsb.Value);
				ftdi.SendByte(rfm92.RegTcxo.Address, rfm92.RegTcxo.Value);
				ftdi.SendByte(rfm92.RegPaDac.Address, rfm92.RegPaDac.Value);
				ftdi.SendByte(rfm92.RegPllHf.Address, rfm92.RegPllHf.Value);
				EntryStandby();
			}
			else
			{
				ftdi.LReset();
				EntrySleep();
				EntryLoRa();
				EntryStandby();
				ftdi.SendByte(rfm96.RegFrMsb.Address, rfm96.RegFrMsb.Value);
				ftdi.SendByte(rfm96.RegFrMid.Address, rfm96.RegFrMid.Value);
				ftdi.SendByte(rfm96.RegFrLsb.Address, rfm96.RegFrLsb.Value);
				ftdi.SendByte(rfm96.RegPaConfig.Address, rfm96.RegPaConfig.Value);
				ftdi.SendByte(rfm96.RegPaRamp.Address, rfm96.RegPaRamp.Value);
				ftdi.SendByte(rfm96.RegOcp.Address, rfm96.RegOcp.Value);
				ftdi.SendByte(rfm96.RegLna.Address, rfm96.RegLna.Value);
				ftdi.SendByte(rfm96.RegIrqFlagsMask.Address, rfm96.RegIrqFlagsMask.Value);
				ftdi.SendByte(rfm96.RegModemConfig1.Address, rfm96.RegModemConfig1.Value);
				ftdi.SendByte(rfm96.RegModemConfig2.Address, rfm96.RegModemConfig2.Value);
				ftdi.SendByte(rfm96.RegModemConfig3.Address, rfm96.RegModemConfig3.Value);
				ftdi.SendByte(rfm96.RegSymbTimeoutLsb.Address, rfm96.RegSymbTimeoutLsb.Value);
				ftdi.SendByte(rfm96.RegPreambleMsb.Address, rfm96.RegPreambleMsb.Value);
				ftdi.SendByte(rfm96.RegPreambleLsb.Address, rfm96.RegPreambleLsb.Value);
				ftdi.SendByte(rfm96.RegTcxo.Address, rfm96.RegTcxo.Value);
				ftdi.SendByte(rfm96.RegPaDac.Address, rfm96.RegPaDac.Value);
				ftdi.SendByte(rfm96.RegPllHf.Address, rfm96.RegPllHf.Value);
				EntryStandby();
			}
			UpdataLCD();
		}

		public void ClearIrq()
		{
			byte data = 255;
			if (ChipVer == ucLoRa.ChipSet.RF92)
				ftdi.SendByte(rfm92.RegIrqFlags.Address, data);
			else
				ftdi.SendByte(rfm96.RegIrqFlags.Address, data);
		}

		public void TestConfigRF()
		{
			UpdataAllValue();
			if (ChipVer == ucLoRa.ChipSet.RF92)
			{
				ftdi.HReset();
				EntrySleep();
				EntryFSK();
				EntryStandby();
				ftdi.SendByte(rfm92.RegBitrateMsb.Address, 208);
				ftdi.SendByte(rfm92.RegBitrateLsb.Address, 85);
				ftdi.SendByte(rfm92.RegFdevMsb.Address, 1);
				ftdi.SendByte(rfm92.RegFdevLsb.Address, 39);
				ftdi.SendByte(rfm92.RegFrMsb.Address, rfm92.RegFrMsb.Value);
				ftdi.SendByte(rfm92.RegFrMid.Address, rfm92.RegFrMid.Value);
				ftdi.SendByte(rfm92.RegFrLsb.Address, rfm92.RegFrLsb.Value);
				ftdi.SendByte(rfm92.RegOcp.Address, rfm92.RegOcp.Value);
				ftdi.SendByte(18, 18);
				ftdi.SendByte(31, 160);
				ftdi.SendByte(37, 0);
				ftdi.SendByte(38, 0);
				ftdi.SendByte(49, 0);
				ftdi.SendByte(39, 2);
				ftdi.SendByte(rfm92.RegPaConfig.Address, rfm92.RegPaConfig.Value);
				ftdi.SendByte(rfm92.RegPaRamp.Address, rfm92.RegPaRamp.Value);
				ftdi.SendByte(64, 0);
				ftdi.SendByte(65, 0);
				ftdi.SendByte(rfm92.RegPllHf.Address, rfm92.RegPllHf.Value);
				ftdi.SendByte(rfm92.RegPaDac.Address, rfm92.RegPaDac.Value);
				ftdi.SendByte(rfm92.RegTcxo.Address, rfm92.RegTcxo.Value);
				EntryStandby();
			}
			else
			{
				ftdi.LReset();
				EntrySleep();
				EntryFSK();
				EntryStandby();
				ftdi.SendByte(rfm96.RegBitrateMsb.Address, 208);
				ftdi.SendByte(rfm96.RegBitrateLsb.Address, 85);
				ftdi.SendByte(rfm96.RegFdevMsb.Address, 1);
				ftdi.SendByte(rfm96.RegFdevLsb.Address, 39);
				ftdi.SendByte(rfm96.RegFrMsb.Address, rfm96.RegFrMsb.Value);
				ftdi.SendByte(rfm96.RegFrMid.Address, rfm96.RegFrMid.Value);
				ftdi.SendByte(rfm96.RegFrLsb.Address, rfm96.RegFrLsb.Value);
				ftdi.SendByte(rfm96.RegOcp.Address, rfm96.RegOcp.Value);
				ftdi.SendByte(rfm96.RegLna.Address, rfm96.RegLna.Value);
				ftdi.SendByte(18, 18);
				ftdi.SendByte(31, 160);
				ftdi.SendByte(37, 0);
				ftdi.SendByte(38, 0);
				ftdi.SendByte(49, 0);
				ftdi.SendByte(39, 2);
				ftdi.SendByte(rfm96.RegPaConfig.Address, rfm96.RegPaConfig.Value);
				ftdi.SendByte(rfm96.RegPaRamp.Address, rfm96.RegPaRamp.Value);
				ftdi.SendByte(64, 0);
				ftdi.SendByte(65, 0);
				ftdi.SendByte(rfm96.RegPllHf.Address, rfm96.RegPllHf.Value);
				ftdi.SendByte(rfm96.RegPaDac.Address, rfm96.RegPaDac.Value);
				ftdi.SendByte(rfm96.RegTcxo.Address, rfm96.RegTcxo.Value);
				EntryStandby();
			}
			UpdataTestLCD();
		}

		public bool EntryRxTest()
		{
			byte data = 0;
			TestConfigRF();
			rfstatus = ucLoRa.RFStatus.RxTest;
			if (ChipVer == ucLoRa.ChipSet.RF92)
			{
				ftdi.ReadByte(rfm92.RegOpMode.Address, ref data);
				rfm92.RegOpMode.Value = (byte)((data & 152) | (byte)rfstatus);
				ftdi.SendByte(rfm92.RegOpMode.Address, rfm92.RegOpMode.Value);
				ftdi.RfDataIn();
				Thread.Sleep(10);
				data = 0;
				ftdi.ReadByte(62, ref data);
				return ((data & 192) == 192);
			}

			ftdi.ReadByte(rfm96.RegOpMode.Address, ref data);
			rfm96.RegOpMode.Value = (byte)((data & 152) | (byte)rfstatus);
			if (nudRadioFreq.Value < new Decimal(525000000))
				rfm96.RegOpMode.Value |= 8;
			else
				rfm96.RegOpMode.Value &= 247;
			ftdi.SendByte(rfm96.RegOpMode.Address, rfm96.RegOpMode.Value);
			ftdi.RfDataIn();
			Thread.Sleep(10);
			data = 0;
			ftdi.ReadByte(62, ref data);
			return ((data & 192) == 192);
		}

		public bool EntryTxTest()
		{
			byte data = 0;
			TestConfigRF();
			rfstatus = ucLoRa.RFStatus.TxTest;

			if (ChipVer == ucLoRa.ChipSet.RF92)
			{
				ftdi.ReadByte(rfm92.RegOpMode.Address, ref data);
				rfm92.RegOpMode.Value = (byte)((data & 152) | 32 | (byte)rfstatus);
				ftdi.SendByte(rfm92.RegOpMode.Address, rfm92.RegOpMode.Value);
				ftdi.RfDataOut();
				ftdi.RfDataHigh();
				Thread.Sleep(10);
				data = 0;
				ftdi.ReadByte(62, ref data);
				return ((data & 160) == 160);
			}

			ftdi.ReadByte(rfm96.RegOpMode.Address, ref data);
			rfm96.RegOpMode.Value = (byte)((data & 152) | 32 | (byte)rfstatus);
			if (nudRadioFreq.Value < new Decimal(525000000))
				rfm96.RegOpMode.Value |= 8;
			else
				rfm96.RegOpMode.Value &= 247;

			ftdi.SendByte(rfm96.RegOpMode.Address, rfm96.RegOpMode.Value);
			ftdi.RfDataOut();
			ftdi.RfDataHigh();

			Thread.Sleep(10);
			data = 0;
			ftdi.ReadByte(62, ref data);
			return ((data & 160) == 160);
		}

		private void RxChkTimer_Tick(object sender, EventArgs e)
		{
			byte data = 0;
			if (rfstatus == ucLoRa.RFStatus.Receiver || rfstatus == ucLoRa.RFStatus.Transmitter || rfstatus == ucLoRa.RFStatus.CADdetect)
			{
				lock (syncThread)
				{
					RxChkTimer.Enabled = false;
					if (ChipVer == ucLoRa.ChipSet.RF92)
						ftdi.ReadByte(rfm92.RegIrqFlags.Address, ref data);
					else
						ftdi.ReadByte(rfm96.RegIrqFlags.Address, ref data);

					UpdataLedStatus(data);

					if (ChipVer == ucLoRa.ChipSet.RF92)
					{
						if ((data & 0x40) == 0x40)
						{
							++PktRxCnt;
							RFM92_RxDone();
							UpdataCnt();
							if (rbGoCAD.Checked)
								EntryCAD();
						}
						if ((data & 0x08) == 0x08)
						{
							ClearIrq();
							EntryStandby();
							++PktTxCnt;
							tbTxPktCnt.Text = PktTxCnt.ToString();
							TxInterTimer.Interval = (int)nudTxDelay.Value;
							TxInterTimer.Enabled = true;
							UpdataCnt();
						}
						if ((data & 0x01) == 0x01)
						{
							rfstatus = ucLoRa.RFStatus.Receiver;
							TxInterTimer.Enabled = false;
							data &= 251;
							ClearIrq();
							EntryRx();
							ChkPllLock();
							ftdi.RxLedOn();
						}
						if ((data & 0x04) == 0x04)
						{
							ClearIrq();
							EntrySleep();
							ftdi.LedOff();
							TxInterTimer.Interval = (int)nudTxDelay.Value;
							TxInterTimer.Enabled = true;
						}
					}
					else
					{
						if ((data & 0x40) == 0x40)
						{
							++PktRxCnt;
							RFM96_RxDone();
							UpdataCnt();
							if (rbGoCAD.Checked)
								EntryCAD();
						}
						if ((data & 0x08) == 0x08)
						{
							ClearIrq();
							EntryStandby();
							++PktTxCnt;
							tbTxPktCnt.Text = PktTxCnt.ToString();
							TxInterTimer.Interval = (int)nudTxDelay.Value;
							TxInterTimer.Enabled = true;
							UpdataCnt();
						}
						if ((data & 0x01) == 0x01)
						{
							rfstatus = ucLoRa.RFStatus.Receiver;
							TxInterTimer.Enabled = false;
							data &= 251;
							ClearIrq();
							EntryRx();
							ChkPllLock();
							ftdi.RxLedOn();
						}
						if ((data & 0x04) == 0x04)
						{
							ClearIrq();
							EntrySleep();
							ftdi.LedOff();
							TxInterTimer.Interval = (int)nudTxDelay.Value;
							TxInterTimer.Enabled = true;
						}
					}
					RxChkTimer.Enabled = true;
				}
			}
			else
				UpdataLedStatus(0);
		}

		private void TxInterTimer_Tick(object sender, EventArgs e)
		{
			RxChkTimer.Enabled = false;
			TxInterTimer.Enabled = false;
			if (rbGoTx.Checked)
			{
				EntryTx();
				ChkPllLock();
			}
			else if (rbGoCAD.Checked)
			{
				EntryCAD();
				ftdi.RxLedOn();
			}
			RxChkTimer.Enabled = true;
		}

		private void RxRssitimer_Tick(object sender, EventArgs e)
		{
			byte data = 0;
			if (rfstatus != ucLoRa.RFStatus.Receiver)
				return;
			byte num;
			if (ChipVer == ucLoRa.ChipSet.RF92)
			{
				ftdi.ReadByte(rfm92.RegRssiValue.Address, ref data);
				switch (cbBW.Text)
				{
					case "250":
						num = (byte)(133 - data);
						break;
					case "500":
						num = (byte)(255 - data);
						break;
					default:
						num = (byte)(137 - data);
						break;
				}
			}
			else
			{
				ftdi.ReadByte(rfm96.RegRssiValue.Address, ref data);
				num = !(nudRadioFreq.Value >= new Decimal(862000000)) ? (byte)(155 - data) : (byte)(150 - data);
			}
			tbRssiValue.Text = "-" + num.ToString();
		}

		private byte GetTxData(string tx_str)
		{
			byte tx_length = 0;
			if (bbPayload.IsHex)
			{
				if (tx_str.Length > 767)
				{
					MessageBox.Show("Longer than 256 bytes!");
				}
				else if (tx_str.Length > 0)
				{
					int idxIn = 0;
					int idxOut = 0;
					for (; idxIn < tx_str.Length; ++idxIn)
					{
						try
						{
							while (tx_str[idxIn] != 32)
							{
								byte data = (byte)(
									tx_str[idxIn] < 97 || tx_str[idxIn] > 122
									? (tx_str[idxIn] < 65 || tx_str[idxIn] > 90
										? (tx_str[idxIn] - 48)
										: (tx_str[idxIn] - 65 + 10)
										)
									: (tx_str[idxIn] - 97 + 10));
								++idxIn;
								data <<= 4;
								data |= (byte)(
									tx_str[idxIn] < 97 || (int)tx_str[idxIn] > 122
									? (tx_str[idxIn] < 65 || tx_str[idxIn] > 90
										? (tx_str[idxIn] - 48)
										: (tx_str[idxIn] - 65 + 10))
									: (tx_str[idxIn] - 97 + 10));
								++idxIn;
								TxBuf[idxOut] = data;
								++idxOut;
							}
						}
						catch { }
					}
					tx_length = (byte)(idxOut + 1);
				}
			}
			else if (tx_str.Length > 255)
			{
				MessageBox.Show("Longer than 256 bytes!");
			}
			else
			{
				int idx;
				for (idx = 0; idx < tx_str.Length; ++idx)
					TxBuf[idx] = (byte)tx_str[idx];
				tx_length = (byte)tx_str.Length;
				if (cbNewLine.Checked)
				{
					TxBuf[idx + 0] = 13;
					TxBuf[idx + 1] = 10;
					tx_length += 2;
				}
			}
			return tx_length;
		}

		private void rbClear_Click(object sender, EventArgs e)
		{
			bbPayload.Clear();
		}

		private void PayloadText_CheckedChange(object sender, EventArgs e)
		{
			if (cbHex.Checked)
			{
				bbPayload.IsHex = true;
				cbNewLine.Checked = false;
				cbNewLine.Enabled = false;
			}
			else
			{
				bbPayload.IsHex = false;
				cbNewLine.Enabled = true;
			}
			bbPayload.ChangeText();
		}

		private void RFM92_RxDone()
		{
			byte data = 0;
			byte[] data4 = new byte[255];

			ftdi.BeepOn();
			ftdi.ReadByte(rfm92.RegHopChannel.Address, ref data);
			PllLockLed.LedColor = (data & 0x80) != 0x80 ? Color.Lime : Color.Red;
			CRC_LED.LedColor = (data & 0x40) != 0x40 ? Color.Red : Color.Lime;

			ftdi.ReadByte(rfm92.RegModemStat.Address, ref data);
			switch (data & 224)
			{
				case 96:
					tbRxCR.Text = "4/7";
					break;
				case 128:
					tbRxCR.Text = "4/8";
					break;
				case 64:
					tbRxCR.Text = "4/6";
					break;
				default:
					tbRxCR.Text = "4/5";
					break;
			}
			ftdi.ReadByte(rfm92.RegPktSnrValue.Address, ref data);
			byte data5;
			if ((data & 0x80) == 0x80)
			{
				double[] numArray = new double[3]
				{
					50.9691001300806,
					53.9794000867204,
					56.9897000433602
				};
				data5 = (byte)(((uint)~data + 1U) >> 2);
				byte num1 = data5;
				tbPktSnrValue.Text = "-";
				tbPktSnrValue.Text += data5.ToString();
				double num2;
				switch (cbBW.Text)
				{
					case "250":
						num2 = 174.0 + (double)num1 - 6.0 - numArray[1];
						break;
					case "500":
						num2 = 174.0 + (double)num1 - 6.0 - numArray[2];
						break;
					default:
						num2 = 174.0 + (double)num1 - 6.0 - numArray[0];
						break;
				}
				tbPktRssiValue.Text = "-";
				tbPktRssiValue.Text += string.Format("{0:N2}", num2);
			}
			else
			{
				byte data6 = (byte)(data >> 2);
				tbPktSnrValue.Text = "+";
				tbPktSnrValue.Text += data6.ToString();
				ftdi.ReadByte(rfm92.RegPktRssiValue.Address, ref data6);
				switch (cbBW.Text)
				{
					case "250":
						data5 = (byte)(133 - data6);
						break;
					case "500":
						data5 = (byte)(255 - data6);
						break;
					default:
						data5 = (byte)(137 - data6);
						break;
				}
				tbPktRssiValue.Text = "-" + data5.ToString();
			}
			ftdi.ReadByte(rfm92.RegRssiValue.Address, ref data5);
			switch (cbBW.Text)
			{
				case "250":
					data5 = (byte)(133 - data5);
					break;
				case "500":
					data5 = (byte)(255 - data5);
					break;
				default:
					data5 = (byte)(137 - data5);
					break;
			}
			tbRssiValue.Text = "-" + data5.ToString();
			ftdi.ReadByte(rfm92.RegRxHeaderCntValueLsb.Address, ref data5);
			ushort num3 = (ushort)data5;
			ftdi.ReadByte(rfm92.RegRxHeaderCntValueMsb.Address, ref data5);
			tbRxHeaderCnt.Text = ((ushort)((uint)num3 | (uint)(ushort)((uint)data5 << 8))).ToString();
			if (rbPayloadCRCOn.Checked)
			{
				PayCRCLed.LedColor = Color.Lime;
				ftdi.ReadByte(rfm92.RegRxPacketCntValueLsb.Address, ref data5);
				ushort num1 = (ushort)data5;
				ftdi.ReadByte(rfm92.RegRxPacketCntValueMsb.Address, ref data5);
				tbRxPacketCnt.Text = ((ushort)((uint)num1 | (uint)(ushort)((uint)data5 << 8))).ToString();
			}
			else
			{
				PayCRCLed.LedColor = Color.Red;
				tbRxPacketCnt.Text = PktRxCnt.ToString();
			}
			ftdi.ReadByte(rfm92.RegFifoRxCurrentAddr.Address, ref data);
			rfm92.RegFifoAddrPtr.Value = data;
			ftdi.SendByte(rfm92.RegFifoAddrPtr.Address, rfm92.RegFifoAddrPtr.Value);
			ftdi.ReadByte(rfm92.RegRxNbBytes.Address, ref data);
			tbRxNbBytes.Text = data.ToString();
			ftdi.ReadBytes(rfm92.RegFifo.Address, ref data4, data);
			ClearIrq();
			if (!bbPayload.IsHex)
			{
				string text = bbPayload.Text;
				for (byte index = 0; index < data; ++index)
				{
					char ch = (char)data4[(int)index];
					text += ch.ToString();
				}
				bbPayload.Text = text;
			}
			else if (bbPayload.Text.Length == 0)
			{
				string str = "";
				for (byte index = 0; index < data; ++index)
				{
					char ch = (char)data4[(int)index];
					str += ch.ToString();
				}
				bbPayload.IsHex = false;
				bbPayload.Text = str;
				bbPayload.IsHex = true;
				bbPayload.ChangeText();
			}
			else
			{
				bbPayload.IsHex = false;
				bbPayload.ChangeText();
				string text = bbPayload.Text;
				for (byte index = 0; index < data; ++index)
				{
					char ch = (char)data4[(int)index];
					text += ch.ToString();
				}
				bbPayload.Text = text;
				bbPayload.IsHex = true;
				bbPayload.ChangeText();
			}
			bbPayload.ScrollToCaret();
			bbPayload.Show();
			ftdi.BeepOff();
		}

		private void RFM96_RxDone()
		{
			byte data = 0;
			byte data2 = 0;
			byte data3 = 0;
			byte[] data4 = new byte[255];

			ftdi.BeepOn();
			ftdi.ReadByte(rfm96.RegHopChannel.Address, ref data);
			PllLockLed.LedColor = (data & 0x80) != 0x80 ? Color.Lime : Color.Red;
			CRC_LED.LedColor = (data & 0x40) != 0x40 ? Color.Red : Color.Lime;

			ftdi.ReadByte(rfm96.RegModemStat.Address, ref data);
			switch (data & 224)
			{
				case 96:
					tbRxCR.Text = "4/7";
					break;
				case 128:
					tbRxCR.Text = "4/8";
					break;
				case 64:
					tbRxCR.Text = "4/6";
					break;
				default:
					tbRxCR.Text = "4/5";
					break;
			}
			ftdi.ReadByte(rfm96.RegPktSnrValue.Address, ref data);
			byte data5;
			if ((data & 0x80) == 0x80)
			{
				double[] numArray = new double[10]
				{
					38.9279003035213,
					40.1773015670055,
					41.9382002601611,
					43.1875866931373,
					44.9485002168009,
					46.1978910572384,
					47.9588001734408,
					50.9691001300806,
					53.9794000867204,
					56.9897000433602
				};
				data5 = (byte)(((uint)~data + 1U) >> 2);
				byte num1 = data5;
				tbPktSnrValue.Text = "-";
				tbPktSnrValue.Text += data5.ToString();
				double num2;
				if (nudRadioFreq.Value >= new Decimal(862000000))
				{
					switch (cbBW.Text)
					{
						case "7.8":
							num2 = 174.0 + (double)num1 - 6.0 - numArray[0];
							break;
						case "10.4":
							num2 = 174.0 + (double)num1 - 6.0 - numArray[1];
							break;
						case "15.6":
							num2 = 174.0 + (double)num1 - 6.0 - numArray[2];
							break;
						case "20.8":
							num2 = 174.0 + (double)num1 - 6.0 - numArray[3];
							break;
						case "31.25":
							num2 = 174.0 + (double)num1 - 6.0 - numArray[4];
							break;
						case "41.7":
							num2 = 174.0 + (double)num1 - 6.0 - numArray[5];
							break;
						case "62.5":
							num2 = 174.0 + (double)num1 - 6.0 - numArray[6];
							break;
						case "125":
							num2 = 174.0 + (double)num1 - 6.0 - numArray[7];
							break;
						case "250":
							num2 = 174.0 + (double)num1 - 6.0 - numArray[9];
							break;
						case "500":
							num2 = 174.0 + (double)num1 - 6.0 - numArray[10];
							break;
						default:
							num2 = 174.0 + (double)num1 - 6.0 - numArray[8];
							break;
					}
				}
				else
				{
					switch (cbBW.Text)
					{
						case "7.8":
							num2 = 174.0 + (double)num1 - 4.0 - numArray[0];
							break;
						case "10.4":
							num2 = 174.0 + (double)num1 - 4.0 - numArray[1];
							break;
						case "15.6":
							num2 = 174.0 + (double)num1 - 4.0 - numArray[2];
							break;
						case "20.8":
							num2 = 174.0 + (double)num1 - 4.0 - numArray[3];
							break;
						case "31.25":
							num2 = 174.0 + (double)num1 - 4.0 - numArray[4];
							break;
						case "41.7":
							num2 = 174.0 + (double)num1 - 4.0 - numArray[5];
							break;
						case "62.5":
							num2 = 174.0 + (double)num1 - 4.0 - numArray[6];
							break;
						case "125":
							num2 = 174.0 + (double)num1 - 4.0 - numArray[7];
							break;
						case "250":
							num2 = 174.0 + (double)num1 - 4.0 - numArray[9];
							break;
						case "500":
							num2 = 174.0 + (double)num1 - 4.0 - numArray[10];
							break;
						default:
							num2 = 174.0 + (double)num1 - 4.0 - numArray[8];
							break;
					}
				}
				tbPktRssiValue.Text = "-";
				tbPktRssiValue.Text += string.Format("{0:N2}", num2);
			}
			else
			{
				byte data6 = (byte)(data >> 2);
				tbPktSnrValue.Text = "+";
				tbPktSnrValue.Text += data6.ToString();
				ftdi.ReadByte(rfm96.RegPktRssiValue.Address, ref data6);
				data5 = (nudRadioFreq.Value >= new Decimal(862000000))
					? (byte)(150 - data6)
					: (byte)(155 - data6);
				tbPktRssiValue.Text = "-" + data5.ToString();
			}
			ftdi.ReadByte(rfm96.RegRssiValue.Address, ref data5);
			data5 = (nudRadioFreq.Value >= new Decimal(862000000))
				? (byte)(150 - data5)
				: (byte)(155 - data5);
			tbRssiValue.Text = "-" + data5.ToString();
			ftdi.ReadByte(rfm96.RegRxHeaderCntValueLsb.Address, ref data5);
			ushort num3 = (ushort)data5;
			ftdi.ReadByte(rfm96.RegRxHeaderCntValueMsb.Address, ref data5);
			tbRxHeaderCnt.Text = ((ushort)((uint)num3 | (uint)(ushort)((uint)data5 << 8))).ToString();
			if (rbPayloadCRCOn.Checked)
			{
				PayCRCLed.LedColor = Color.Lime;
				ftdi.ReadByte(rfm96.RegRxPacketCntValueLsb.Address, ref data5);
				ushort num1 = (ushort)data5;
				ftdi.ReadByte(rfm96.RegRxPacketCntValueMsb.Address, ref data5);
				tbRxPacketCnt.Text = ((ushort)((uint)num1 | (uint)(ushort)((uint)data5 << 8))).ToString();
			}
			else
			{
				PayCRCLed.LedColor = Color.Red;
				tbRxPacketCnt.Text = PktRxCnt.ToString();
			}
			ftdi.ReadByte(rfm96.RegFifoRxCurrentAddr.Address, ref data2);
			rfm96.RegFifoAddrPtr.Value = data2;
			ftdi.SendByte(rfm96.RegFifoAddrPtr.Address, rfm96.RegFifoAddrPtr.Value);
			ftdi.ReadByte(rfm96.RegRxNbBytes.Address, ref data3);
			tbRxNbBytes.Text = data3.ToString();
			ftdi.ReadBytes(rfm96.RegFifo.Address, ref data4, data3);
			ClearIrq();
			if (!bbPayload.IsHex)
			{
				string text = bbPayload.Text;
				for (byte index = 0; (int)index < (int)data3; ++index)
				{
					char ch = (char)data4[(int)index];
					text += ch.ToString();
				}
				bbPayload.Text = text;
			}
			else if (bbPayload.Text.Length == 0)
			{
				string str = "";
				for (byte index = 0; (int)index < (int)data3; ++index)
				{
					char ch = (char)data4[(int)index];
					str += ch.ToString();
				}
				bbPayload.IsHex = false;
				bbPayload.Text = str;
				bbPayload.IsHex = true;
				bbPayload.ChangeText();
			}
			else
			{
				bbPayload.IsHex = false;
				bbPayload.ChangeText();
				string text = bbPayload.Text;
				for (byte index = 0; (int)index < (int)data3; ++index)
				{
					char ch = (char)data4[(int)index];
					text += ch.ToString();
				}
				bbPayload.Text = text;
				bbPayload.IsHex = true;
				bbPayload.ChangeText();
			}
			bbPayload.Show();
			ftdi.BeepOff();
		}

		private void DotSet(ucLoRa.DOT_INF dot)
		{
			byte address = (byte)((uint)dot >> 3);
			byte num1 = (byte)((uint)dot & 7);
			byte num2 = 1;
			for (; num1 != 0; --num1)
				num2 <<= 1;
			ftdi.LCDOrValue(address, num2);
		}

		private void DotClear(ucLoRa.DOT_INF dot)
		{
			byte address = (byte)((uint)dot >> 3);
			byte num1 = (byte)((uint)dot & 7);
			uint num2 = 254;
			for (; num1 != 0; --num1)
				num2 = (byte)((num2 << 1) | 1);
			ftdi.LCDAndValue(address, (byte)num2);
		}

		private static byte[] NumberSetArray = new byte[38]
		{
			119,
			36,
			93,
			109,
			46,
			107,
			123,
			37,
			127,
			111,
			63,
			122,
			83,
			124,
			91,
			27,
			111,
			62,
			36,
			100,
			26,
			82,
			55,
			56,
			120,
			31,
			47,
			24,
			107,
			90,
			112,
			118,
			0,
			0,
			0,
			0,
			0,
			8
		};
		private void NumberSet(byte addr, char label)
		{
			ftdi.LCDAndValue(addr, 0x80);
			byte num1 = (byte)(
				(label < 65 || label > 90)
				? ((label < 48 || label > 57)
					? (label != 45 ? 36 : 37)
					: (label - 48))
				: (label - 65 + 10)
				);
			byte num2 = NumberSetArray[num1];
			ftdi.LCDOrValue(addr, num2);
		}

		private void NumberAllClear()
		{
			for (byte address = 0; address < 30; ++address)
			{
				if (address != 19)
					ftdi.LCDAndValue(address, 0x80);
			}
		}

		private void FreqNumberClear()
		{
			for (byte address = 20; address <= 25; ++address)
				ftdi.LCDAndValue(address, 0x80);
		}

		private void TxNumberClear()
		{
			for (byte address = 26; address <= 29; ++address)
				ftdi.LCDAndValue(address, 0x80);
		}

		private void RxNumberClear()
		{
			for (byte address = 0; address <= 3; ++address)
				ftdi.LCDAndValue(address, 0x80);
		}

		private void FdevNumberClear()
		{
			ftdi.LCDAndValue(5, 0x80);
			ftdi.LCDAndValue(6, 0x80);
			ftdi.LCDAndValue(7, 0x80);
		}

		private void BWNumberClear()
		{
			ftdi.LCDAndValue(8, 0x80);
			ftdi.LCDAndValue(9, 0x80);
			ftdi.LCDAndValue(10, 0x80);
		}

		private void PWRNumberClear()
		{
			ftdi.LCDAndValue(13, 0x80);
			ftdi.LCDAndValue(14, 0x80);
		}

		private void BRNumberClear()
		{
			ftdi.LCDAndValue(4, 0x80);
			ftdi.LCDAndValue(11, 0x80);
			ftdi.LCDAndValue(12, 0x80);
		}

		private void ModuleNumberClear()
		{
			ftdi.LCDAndValue(15, 0x80);
			ftdi.LCDAndValue(16, 0x80);
			ftdi.LCDAndValue(17, 0x80);
			ftdi.LCDAndValue(18, 0x80);
		}

		private void LCDConnect()
		{
			NumberSet(25, '-');
			NumberSet(24, '-');
			NumberSet(23, 'P');
			NumberSet(22, 'C');
			NumberSet(21, '-');
			NumberSet(20, '-');
		}

		private void LCDLoRa()
		{
			NumberSet(18, 'L');
			NumberSet(17, 'O');
			NumberSet(16, 'R');
			NumberSet(15, 'A');
		}

		private void LCDInitShow()
		{
			DotSet(ucLoRa.DOT_INF.HopeRF);
			DotSet(ucLoRa.DOT_INF.ANT);
			DotSet(ucLoRa.DOT_INF.TX);
			DotSet(ucLoRa.DOT_INF.RX);
			DotSet(ucLoRa.DOT_INF.BW);
			DotSet(ucLoRa.DOT_INF.PWR);
			LCDConnect();
			LCDLoRa();
		}

		private void UpdataPwrLCD(string state)
		{
			NumberSet(14, state[0]);
			NumberSet(13, state[1]);
		}

		private void UpdataSfLCD(string str)
		{
			DotClear(ucLoRa.DOT_INF.DEV);
			DotClear(ucLoRa.DOT_INF.DevDot);
			if (str.Length > 3)
			{
				NumberSet(5, str[1]);
				NumberSet(6, str[2]);
				NumberSet(7, str[3]);
			}
			else
			{
				NumberSet(5, str[0]);
				NumberSet(6, str[1]);
				NumberSet(7, str[2]);
			}
		}

		private void UpdataFdevLCD(string str)
		{
			DotSet(ucLoRa.DOT_INF.DEV);
			DotClear(ucLoRa.DOT_INF.DevDot);
			if (str.Length > 2)
			{
				NumberSet(5, str[0]);
				NumberSet(6, str[1]);
				NumberSet(7, str[2]);
			}
			else
			{
				NumberSet(6, str[0]);
				NumberSet(7, str[1]);
			}
		}

		private void UpdataBwLCD(string str)
		{
			DotClear(ucLoRa.DOT_INF.BwDot);
			switch (str)
			{
				case "7.8":
					NumberSet(9, str[0]);
					DotSet(ucLoRa.DOT_INF.BwDot);
					NumberSet(10, str[2]);
					break;
				case "10.4":
				case "15.6":
				case "20.8":
				case "31.25":
				case "41.7":
				case "62.5":
					NumberSet(8, str[0]);
					NumberSet(9, str[1]);
					DotSet(ucLoRa.DOT_INF.BwDot);
					NumberSet(10, str[3]);
					break;
				case "83":
					NumberSet(9, str[0]);
					NumberSet(10, str[1]);
					break;
				default:
					NumberSet(8, str[0]);
					NumberSet(9, str[1]);
					NumberSet(10, str[2]);
					break;
			}
		}

		private void UpdataFreqLCD(string str)
		{
			DotClear(ucLoRa.DOT_INF.F315);
			DotClear(ucLoRa.DOT_INF.F434);
			DotClear(ucLoRa.DOT_INF.F868);
			DotClear(ucLoRa.DOT_INF.F915);
			NumberSet(25, ' ');
			NumberSet(24, str[0]);
			NumberSet(23, str[1]);
			NumberSet(22, str[2]);
			NumberSet(21, str[3]);
			NumberSet(20, str[4]);
			DotSet(ucLoRa.DOT_INF.FreqDot);
			switch (str[0])
			{
				case '4':
					DotSet(ucLoRa.DOT_INF.F434);
					break;
				case '8':
					DotSet(ucLoRa.DOT_INF.F868);
					break;
				case '9':
					DotSet(ucLoRa.DOT_INF.F915);
					break;
			}
		}

		private void UpdataCrLCD(string str)
		{
			DotClear(ucLoRa.DOT_INF.BR);
			DotClear(ucLoRa.DOT_INF.BrDot);
			NumberSet(12, str[0]);
			NumberSet(11, '-');
			NumberSet(4, str[2]);
		}

		private void UpdataBrLCD()
		{
			DotSet(ucLoRa.DOT_INF.BR);
			NumberSet(11, '0');
			DotSet(ucLoRa.DOT_INF.BrDot);
			NumberSet(4, '6');
		}

		private void UpdataLCD()
		{
			UpdataFreqLCD(nudRadioFreq.Value.ToString());
			UpdataSfLCD(cbSF.Text);
			UpdataBwLCD(cbBW.Text);
			UpdataCrLCD(cbCR.Text);
			UpdataPwrLCD(cbOutputPower.Text);
			ftdi.LCDDisplay();
		}

		private void UpdataCnt()
		{
			RxNumberClear();
			switch (tbRxPacketCnt.Text.Length)
			{
				case 0:
					TxNumberClear();
					switch (tbTxPktCnt.Text.Length)
					{
						case 0:
							ftdi.LCDDisplay();
							return;
						case 1:
							NumberSet(26, tbTxPktCnt.Text[0]);
							goto case 0;
						case 2:
							NumberSet(27, tbTxPktCnt.Text[0]);
							NumberSet(26, tbTxPktCnt.Text[1]);
							goto case 0;
						case 3:
							NumberSet(28, tbTxPktCnt.Text[0]);
							NumberSet(27, tbTxPktCnt.Text[1]);
							NumberSet(26, tbTxPktCnt.Text[2]);
							goto case 0;
						default:
							NumberSet(29, tbTxPktCnt.Text[tbTxPktCnt.Text.Length - 4]);
							NumberSet(28, tbTxPktCnt.Text[tbTxPktCnt.Text.Length - 3]);
							NumberSet(27, tbTxPktCnt.Text[tbTxPktCnt.Text.Length - 2]);
							NumberSet(26, tbTxPktCnt.Text[tbTxPktCnt.Text.Length - 1]);
							goto case 0;
					}
				case 1:
					NumberSet(3, tbRxPacketCnt.Text[0]);
					goto case 0;
				case 2:
					NumberSet(2, tbRxPacketCnt.Text[0]);
					NumberSet(3, tbRxPacketCnt.Text[1]);
					goto case 0;
				case 3:
					NumberSet(1, tbRxPacketCnt.Text[0]);
					NumberSet(2, tbRxPacketCnt.Text[1]);
					NumberSet(3, tbRxPacketCnt.Text[2]);
					goto case 0;
				default:
					NumberSet(0, tbRxPacketCnt.Text[tbRxPacketCnt.Text.Length - 4]);
					NumberSet(1, tbRxPacketCnt.Text[tbRxPacketCnt.Text.Length - 3]);
					NumberSet(2, tbRxPacketCnt.Text[tbRxPacketCnt.Text.Length - 2]);
					NumberSet(3, tbRxPacketCnt.Text[tbRxPacketCnt.Text.Length - 1]);
					goto case 0;
			}
		}

		private void UpdataTestLCD()
		{
			UpdataFreqLCD(nudRadioFreq.Value.ToString());
			UpdataPwrLCD(cbOutputPower.Text);
			UpdataBwLCD("83");
			UpdataBrLCD();
			UpdataFdevLCD("18");
			ftdi.LCDDisplay();
		}

		public enum RFStatus
		{
			Sleep = 0,
			Standby = 1,
			Transmitter = 3,
			Receiver = 5,
			CADdetect = 7,
			TxTest = 131,
			RxTest = 133,
		}

		public enum ChipSet
		{
			RF96 = 18,
			RF92 = 34,
		}

		private enum DOT_INF
		{
			ANT = 7,
			RX = 15,
			RxDot = 23,
			Alt = 31,
			BR = 39,
			RH = 47,
			DevDot = 55,
			DEV = 63,
			BwDot = 79,
			BW = 87,
			BrDot = 95,
			TX = 103,
			PWR = 111,
			mV = 127,
			HopeRF = 135,
			Temp = 143,
			TxDot = 151,
			RFM = 152,
			MHz = 153,
			HP = 154,
			hPa = 155,
			Psi = 156,
			Bar = 157,
			KPa = 158,
			F915 = 159,
			F868 = 167,
			FreqDot = 175,
			SecDot = 183,
			MinDot = 191,
			F434 = 199,
			F315 = 207,
			Rssi4 = 215,
			Rssi3 = 223,
			Rssi2 = 231,
			Rssi1 = 239,
		}

		#region InitializeComponent
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.gbCrystal = new System.Windows.Forms.GroupBox();
			this.rbXTAL = new System.Windows.Forms.RadioButton();
			this.rbTCXO = new System.Windows.Forms.RadioButton();
			this.label3 = new System.Windows.Forms.Label();
			this.nudRadioFreq = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.gbTxSetting = new System.Windows.Forms.GroupBox();
			this.cbPLLBW = new System.Windows.Forms.ComboBox();
			this.cbOcpTrimming = new System.Windows.Forms.ComboBox();
			this.label15 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.rbOCPOff = new System.Windows.Forms.RadioButton();
			this.rbOCPOn = new System.Windows.Forms.RadioButton();
			this.label12 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.cbOutputPower = new System.Windows.Forms.ComboBox();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.cbMaxOutputPower = new System.Windows.Forms.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.cbPaRamp = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.cbPaOutput = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.gbRxSetting = new System.Windows.Forms.GroupBox();
			this.pAGC = new System.Windows.Forms.Panel();
			this.label16 = new System.Windows.Forms.Label();
			this.rbAGCOn = new System.Windows.Forms.RadioButton();
			this.rbAGCOff = new System.Windows.Forms.RadioButton();
			this.cbLnaGain = new System.Windows.Forms.ComboBox();
			this.pLNA = new System.Windows.Forms.Panel();
			this.label17 = new System.Windows.Forms.Label();
			this.rbLNAOff = new System.Windows.Forms.RadioButton();
			this.rbLNAOn = new System.Windows.Forms.RadioButton();
			this.label2 = new System.Windows.Forms.Label();
			this.gbLoraSetting = new System.Windows.Forms.GroupBox();
			this.label47 = new System.Windows.Forms.Label();
			this.nudImplicitRxLength = new System.Windows.Forms.NumericUpDown();
			this.cbImplicit = new System.Windows.Forms.CheckBox();
			this.label46 = new System.Windows.Forms.Label();
			this.label23 = new System.Windows.Forms.Label();
			this.label27 = new System.Windows.Forms.Label();
			this.lTs = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.rbPayloadCRCOff = new System.Windows.Forms.RadioButton();
			this.rbPayloadCRCOn = new System.Windows.Forms.RadioButton();
			this.label26 = new System.Windows.Forms.Label();
			this.nudPreambleLength = new System.Windows.Forms.NumericUpDown();
			this.label25 = new System.Windows.Forms.Label();
			this.pLROptimize = new System.Windows.Forms.Panel();
			this.rbLROptimizeOff = new System.Windows.Forms.RadioButton();
			this.rbLROptimizeOn = new System.Windows.Forms.RadioButton();
			this.label24 = new System.Windows.Forms.Label();
			this.nudRxTimeOut = new System.Windows.Forms.NumericUpDown();
			this.label22 = new System.Windows.Forms.Label();
			this.label21 = new System.Windows.Forms.Label();
			this.cbBW = new System.Windows.Forms.ComboBox();
			this.label20 = new System.Windows.Forms.Label();
			this.cbCR = new System.Windows.Forms.ComboBox();
			this.label19 = new System.Windows.Forms.Label();
			this.cbSF = new System.Windows.Forms.ComboBox();
			this.label18 = new System.Windows.Forms.Label();
			this.gbPHInfo = new System.Windows.Forms.GroupBox();
			this.PayCRCLed = new MyCSLib.Controls.Led();
			this.label53 = new System.Windows.Forms.Label();
			this.label52 = new System.Windows.Forms.Label();
			this.label51 = new System.Windows.Forms.Label();
			this.label50 = new System.Windows.Forms.Label();
			this.CRC_LED = new MyCSLib.Controls.Led();
			this.label38 = new System.Windows.Forms.Label();
			this.tbRssiValue = new System.Windows.Forms.TextBox();
			this.tbPktRssiValue = new System.Windows.Forms.TextBox();
			this.tbPktSnrValue = new System.Windows.Forms.TextBox();
			this.tbRxPacketCnt = new System.Windows.Forms.TextBox();
			this.tbRxCR = new System.Windows.Forms.TextBox();
			this.tbRxNbBytes = new System.Windows.Forms.TextBox();
			this.tbRxHeaderCnt = new System.Windows.Forms.TextBox();
			this.label42 = new System.Windows.Forms.Label();
			this.label41 = new System.Windows.Forms.Label();
			this.label40 = new System.Windows.Forms.Label();
			this.label39 = new System.Windows.Forms.Label();
			this.label37 = new System.Windows.Forms.Label();
			this.label28 = new System.Windows.Forms.Label();
			this.gbMessage = new System.Windows.Forms.GroupBox();
			this.cbNewLine = new System.Windows.Forms.CheckBox();
			this.PllLockLed = new MyCSLib.Controls.Led();
			this.label49 = new System.Windows.Forms.Label();
			this.cbHex = new System.Windows.Forms.CheckBox();
			this.rbClear = new System.Windows.Forms.Button();
			this.bbPayload = new MyCSLib.Controls.BytesBox();
			this.gbOpMode = new System.Windows.Forms.GroupBox();
			this.rbGoTxTest = new System.Windows.Forms.RadioButton();
			this.rbGoRxTest = new System.Windows.Forms.RadioButton();
			this.rbGoSleep = new System.Windows.Forms.RadioButton();
			this.rbGoCAD = new System.Windows.Forms.RadioButton();
			this.nudTxDelay = new System.Windows.Forms.NumericUpDown();
			this.rbGoTx = new System.Windows.Forms.RadioButton();
			this.rbGoRx = new System.Windows.Forms.RadioButton();
			this.label45 = new System.Windows.Forms.Label();
			this.tbTxPktCnt = new System.Windows.Forms.TextBox();
			this.label44 = new System.Windows.Forms.Label();
			this.label43 = new System.Windows.Forms.Label();
			this.bSwitch = new System.Windows.Forms.Button();
			this.cbRxDone = new System.Windows.Forms.CheckBox();
			this.cbCRCError = new System.Windows.Forms.CheckBox();
			this.cbValidHeader = new System.Windows.Forms.CheckBox();
			this.label29 = new System.Windows.Forms.Label();
			this.cbRxTimeOut = new System.Windows.Forms.CheckBox();
			this.cbTxDone = new System.Windows.Forms.CheckBox();
			this.label36 = new System.Windows.Forms.Label();
			this.label31 = new System.Windows.Forms.Label();
			this.cbCADDone = new System.Windows.Forms.CheckBox();
			this.label32 = new System.Windows.Forms.Label();
			this.label33 = new System.Windows.Forms.Label();
			this.cbFHSSChannel = new System.Windows.Forms.CheckBox();
			this.label34 = new System.Windows.Forms.Label();
			this.label35 = new System.Windows.Forms.Label();
			this.cbCADDetect = new System.Windows.Forms.CheckBox();
			this.gbIRQMask = new System.Windows.Forms.GroupBox();
			this.CADDetectLed = new MyCSLib.Controls.Led();
			this.FHSSChannelLed = new MyCSLib.Controls.Led();
			this.CADDoneLed = new MyCSLib.Controls.Led();
			this.TxDoneLed = new MyCSLib.Controls.Led();
			this.ValidHeaderLed = new MyCSLib.Controls.Led();
			this.CRCErrorLed = new MyCSLib.Controls.Led();
			this.RxDoneLed = new MyCSLib.Controls.Led();
			this.RxTimeOutLed = new MyCSLib.Controls.Led();
			this.label30 = new System.Windows.Forms.Label();
			this.RxChkTimer = new System.Windows.Forms.Timer(this.components);
			this.TxInterTimer = new System.Windows.Forms.Timer(this.components);
			this.RxRssitimer = new System.Windows.Forms.Timer(this.components);
			this.gbCrystal.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudRadioFreq)).BeginInit();
			this.gbTxSetting.SuspendLayout();
			this.gbRxSetting.SuspendLayout();
			this.pAGC.SuspendLayout();
			this.pLNA.SuspendLayout();
			this.gbLoraSetting.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudImplicitRxLength)).BeginInit();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudPreambleLength)).BeginInit();
			this.pLROptimize.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudRxTimeOut)).BeginInit();
			this.gbPHInfo.SuspendLayout();
			this.gbMessage.SuspendLayout();
			this.gbOpMode.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudTxDelay)).BeginInit();
			this.gbIRQMask.SuspendLayout();
			this.SuspendLayout();
			// 
			// gbCrystal
			// 
			this.gbCrystal.Controls.Add(this.rbXTAL);
			this.gbCrystal.Controls.Add(this.rbTCXO);
			this.gbCrystal.Controls.Add(this.label3);
			this.gbCrystal.Controls.Add(this.nudRadioFreq);
			this.gbCrystal.Controls.Add(this.label1);
			this.gbCrystal.Location = new System.Drawing.Point(3, 3);
			this.gbCrystal.Name = "gbCrystal";
			this.gbCrystal.Size = new System.Drawing.Size(236, 66);
			this.gbCrystal.TabIndex = 0;
			this.gbCrystal.TabStop = false;
			this.gbCrystal.Text = "Crystal";
			// 
			// rbXTAL
			// 
			this.rbXTAL.AutoSize = true;
			this.rbXTAL.Checked = true;
			this.rbXTAL.Location = new System.Drawing.Point(183, 46);
			this.rbXTAL.Name = "rbXTAL";
			this.rbXTAL.Size = new System.Drawing.Size(52, 17);
			this.rbXTAL.TabIndex = 5;
			this.rbXTAL.TabStop = true;
			this.rbXTAL.Text = "XTAL";
			this.rbXTAL.UseVisualStyleBackColor = true;
			this.rbXTAL.CheckedChanged += new System.EventHandler(this.Crystal_CheckedChanged);
			// 
			// rbTCXO
			// 
			this.rbTCXO.AutoSize = true;
			this.rbTCXO.Location = new System.Drawing.Point(107, 44);
			this.rbTCXO.Name = "rbTCXO";
			this.rbTCXO.Size = new System.Drawing.Size(54, 17);
			this.rbTCXO.TabIndex = 4;
			this.rbTCXO.Text = "TCXO";
			this.rbTCXO.UseVisualStyleBackColor = true;
			this.rbTCXO.CheckedChanged += new System.EventHandler(this.Crystal_CheckedChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(7, 44);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(65, 13);
			this.label3.TabIndex = 3;
			this.label3.Text = "Crystal Type";
			// 
			// nudRadioFreq
			// 
			this.nudRadioFreq.BackColor = System.Drawing.Color.White;
			this.nudRadioFreq.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.nudRadioFreq.Increment = new decimal(new int[] {
            61,
            0,
            0,
            0});
			this.nudRadioFreq.Location = new System.Drawing.Point(107, 14);
			this.nudRadioFreq.Maximum = new decimal(new int[] {
            1020000000,
            0,
            0,
            0});
			this.nudRadioFreq.Minimum = new decimal(new int[] {
            137000000,
            0,
            0,
            0});
			this.nudRadioFreq.Name = "nudRadioFreq";
			this.nudRadioFreq.Size = new System.Drawing.Size(123, 20);
			this.nudRadioFreq.TabIndex = 1;
			this.nudRadioFreq.ThousandsSeparator = true;
			this.nudRadioFreq.Value = new decimal(new int[] {
            434000000,
            0,
            0,
            0});
			this.nudRadioFreq.ValueChanged += new System.EventHandler(this.nudRadioFreq_ValueChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(88, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Radio Frequency";
			// 
			// gbTxSetting
			// 
			this.gbTxSetting.Controls.Add(this.cbPLLBW);
			this.gbTxSetting.Controls.Add(this.cbOcpTrimming);
			this.gbTxSetting.Controls.Add(this.label15);
			this.gbTxSetting.Controls.Add(this.label14);
			this.gbTxSetting.Controls.Add(this.label13);
			this.gbTxSetting.Controls.Add(this.rbOCPOff);
			this.gbTxSetting.Controls.Add(this.rbOCPOn);
			this.gbTxSetting.Controls.Add(this.label12);
			this.gbTxSetting.Controls.Add(this.label11);
			this.gbTxSetting.Controls.Add(this.label10);
			this.gbTxSetting.Controls.Add(this.cbOutputPower);
			this.gbTxSetting.Controls.Add(this.label9);
			this.gbTxSetting.Controls.Add(this.label8);
			this.gbTxSetting.Controls.Add(this.cbMaxOutputPower);
			this.gbTxSetting.Controls.Add(this.label7);
			this.gbTxSetting.Controls.Add(this.label6);
			this.gbTxSetting.Controls.Add(this.cbPaRamp);
			this.gbTxSetting.Controls.Add(this.label5);
			this.gbTxSetting.Controls.Add(this.cbPaOutput);
			this.gbTxSetting.Controls.Add(this.label4);
			this.gbTxSetting.Location = new System.Drawing.Point(4, 73);
			this.gbTxSetting.Name = "gbTxSetting";
			this.gbTxSetting.Size = new System.Drawing.Size(235, 207);
			this.gbTxSetting.TabIndex = 1;
			this.gbTxSetting.TabStop = false;
			this.gbTxSetting.Text = "TX Setting";
			// 
			// cbPLLBW
			// 
			this.cbPLLBW.FormattingEnabled = true;
			this.cbPLLBW.Items.AddRange(new object[] {
            "75",
            "150",
            "225",
            "300"});
			this.cbPLLBW.Location = new System.Drawing.Point(106, 127);
			this.cbPLLBW.Name = "cbPLLBW";
			this.cbPLLBW.Size = new System.Drawing.Size(96, 21);
			this.cbPLLBW.TabIndex = 22;
			this.cbPLLBW.Text = "300";
			this.cbPLLBW.SelectedValueChanged += new System.EventHandler(this.cbPLLBW_SelectedValueChanged);
			// 
			// cbOcpTrimming
			// 
			this.cbOcpTrimming.FormattingEnabled = true;
			this.cbOcpTrimming.Items.AddRange(new object[] {
            "60",
            "80",
            "100",
            "120",
            "140",
            "160",
            "180",
            "200",
            "220",
            "240"});
			this.cbOcpTrimming.Location = new System.Drawing.Point(106, 178);
			this.cbOcpTrimming.Name = "cbOcpTrimming";
			this.cbOcpTrimming.Size = new System.Drawing.Size(96, 21);
			this.cbOcpTrimming.TabIndex = 21;
			this.cbOcpTrimming.Text = "100";
			this.cbOcpTrimming.SelectedValueChanged += new System.EventHandler(this.cbOcpTrimming_SelectedValueChanged);
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(206, 181);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(22, 13);
			this.label15.TabIndex = 20;
			this.label15.Text = "mA";
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(206, 131);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(27, 13);
			this.label14.TabIndex = 19;
			this.label14.Text = "KHz";
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(7, 181);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(67, 13);
			this.label13.TabIndex = 17;
			this.label13.Text = "OC Trimming";
			// 
			// rbOCPOff
			// 
			this.rbOCPOff.AutoSize = true;
			this.rbOCPOff.Location = new System.Drawing.Point(155, 155);
			this.rbOCPOff.Name = "rbOCPOff";
			this.rbOCPOff.Size = new System.Drawing.Size(45, 17);
			this.rbOCPOff.TabIndex = 16;
			this.rbOCPOff.Text = "OFF";
			this.rbOCPOff.UseVisualStyleBackColor = true;
			this.rbOCPOff.CheckedChanged += new System.EventHandler(this.rbOCP_CheckedChanged);
			// 
			// rbOCPOn
			// 
			this.rbOCPOn.AutoSize = true;
			this.rbOCPOn.Checked = true;
			this.rbOCPOn.Location = new System.Drawing.Point(106, 155);
			this.rbOCPOn.Name = "rbOCPOn";
			this.rbOCPOn.Size = new System.Drawing.Size(41, 17);
			this.rbOCPOn.TabIndex = 15;
			this.rbOCPOn.TabStop = true;
			this.rbOCPOn.Text = "ON";
			this.rbOCPOn.UseVisualStyleBackColor = true;
			this.rbOCPOn.CheckedChanged += new System.EventHandler(this.rbOCP_CheckedChanged);
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(6, 157);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(65, 13);
			this.label12.TabIndex = 14;
			this.label12.Text = "OCP Enable";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(6, 131);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(78, 13);
			this.label11.TabIndex = 12;
			this.label11.Text = "PLL bandwidth";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(205, 102);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(28, 13);
			this.label10.TabIndex = 11;
			this.label10.Text = "dBm";
			// 
			// cbOutputPower
			// 
			this.cbOutputPower.FormattingEnabled = true;
			this.cbOutputPower.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15"});
			this.cbOutputPower.Location = new System.Drawing.Point(106, 98);
			this.cbOutputPower.Name = "cbOutputPower";
			this.cbOutputPower.Size = new System.Drawing.Size(96, 21);
			this.cbOutputPower.TabIndex = 10;
			this.cbOutputPower.Text = "15";
			this.cbOutputPower.SelectedValueChanged += new System.EventHandler(this.cbOutputPower_SelectedValueChanged);
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(6, 101);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(72, 13);
			this.label9.TabIndex = 9;
			this.label9.Text = "Output Power";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(205, 74);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(28, 13);
			this.label8.TabIndex = 8;
			this.label8.Text = "dBm";
			// 
			// cbMaxOutputPower
			// 
			this.cbMaxOutputPower.FormattingEnabled = true;
			this.cbMaxOutputPower.Items.AddRange(new object[] {
            "10.8",
            "11.4",
            "12.0",
            "12.6",
            "13.2",
            "13.8",
            "14.4",
            "15.0"});
			this.cbMaxOutputPower.Location = new System.Drawing.Point(106, 69);
			this.cbMaxOutputPower.Name = "cbMaxOutputPower";
			this.cbMaxOutputPower.Size = new System.Drawing.Size(96, 21);
			this.cbMaxOutputPower.TabIndex = 7;
			this.cbMaxOutputPower.Text = "13.2";
			this.cbMaxOutputPower.SelectedValueChanged += new System.EventHandler(this.cbMaxOutputPower_SelectedValueChanged);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(6, 73);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(89, 13);
			this.label7.TabIndex = 6;
			this.label7.Text = "MaxOutputPower";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(206, 48);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(20, 13);
			this.label6.TabIndex = 5;
			this.label6.Text = "uS";
			// 
			// cbPaRamp
			// 
			this.cbPaRamp.FormattingEnabled = true;
			this.cbPaRamp.Items.AddRange(new object[] {
            "3400",
            "2000",
            "1000",
            "500",
            "250",
            "125",
            "100",
            "62",
            "50",
            "40",
            "31",
            "25",
            "20",
            "15",
            "12",
            "10"});
			this.cbPaRamp.Location = new System.Drawing.Point(106, 42);
			this.cbPaRamp.Name = "cbPaRamp";
			this.cbPaRamp.Size = new System.Drawing.Size(96, 21);
			this.cbPaRamp.TabIndex = 4;
			this.cbPaRamp.Text = "40";
			this.cbPaRamp.SelectedValueChanged += new System.EventHandler(this.cbPaRamp_SelectedValueChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(6, 46);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(52, 13);
			this.label5.TabIndex = 3;
			this.label5.Text = "PA Ramp";
			// 
			// cbPaOutput
			// 
			this.cbPaOutput.DisplayMember = "2";
			this.cbPaOutput.FormattingEnabled = true;
			this.cbPaOutput.Items.AddRange(new object[] {
            "RFO",
            "PA_BOOST"});
			this.cbPaOutput.Location = new System.Drawing.Point(106, 14);
			this.cbPaOutput.Name = "cbPaOutput";
			this.cbPaOutput.Size = new System.Drawing.Size(96, 21);
			this.cbPaOutput.TabIndex = 2;
			this.cbPaOutput.Text = "RFO";
			this.cbPaOutput.SelectedValueChanged += new System.EventHandler(this.cbPaOutput_SelectedValueChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(6, 18);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(56, 13);
			this.label4.TabIndex = 0;
			this.label4.Text = "PA Output";
			// 
			// gbRxSetting
			// 
			this.gbRxSetting.Controls.Add(this.pAGC);
			this.gbRxSetting.Controls.Add(this.cbLnaGain);
			this.gbRxSetting.Controls.Add(this.pLNA);
			this.gbRxSetting.Controls.Add(this.label2);
			this.gbRxSetting.Location = new System.Drawing.Point(4, 286);
			this.gbRxSetting.Name = "gbRxSetting";
			this.gbRxSetting.Size = new System.Drawing.Size(235, 111);
			this.gbRxSetting.TabIndex = 2;
			this.gbRxSetting.TabStop = false;
			this.gbRxSetting.Text = "RX Setting";
			// 
			// pAGC
			// 
			this.pAGC.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.pAGC.Controls.Add(this.label16);
			this.pAGC.Controls.Add(this.rbAGCOn);
			this.pAGC.Controls.Add(this.rbAGCOff);
			this.pAGC.Location = new System.Drawing.Point(9, 20);
			this.pAGC.Name = "pAGC";
			this.pAGC.Size = new System.Drawing.Size(220, 25);
			this.pAGC.TabIndex = 25;
			// 
			// label16
			// 
			this.label16.AutoSize = true;
			this.label16.Location = new System.Drawing.Point(6, 7);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(54, 13);
			this.label16.TabIndex = 18;
			this.label16.Text = "AGC Auto";
			// 
			// rbAGCOn
			// 
			this.rbAGCOn.AutoSize = true;
			this.rbAGCOn.Location = new System.Drawing.Point(100, 4);
			this.rbAGCOn.Name = "rbAGCOn";
			this.rbAGCOn.Size = new System.Drawing.Size(41, 17);
			this.rbAGCOn.TabIndex = 19;
			this.rbAGCOn.Text = "ON";
			this.rbAGCOn.UseVisualStyleBackColor = true;
			this.rbAGCOn.CheckedChanged += new System.EventHandler(this.rbAGC_CheckedChanged);
			// 
			// rbAGCOff
			// 
			this.rbAGCOff.AutoSize = true;
			this.rbAGCOff.Checked = true;
			this.rbAGCOff.Location = new System.Drawing.Point(157, 4);
			this.rbAGCOff.Name = "rbAGCOff";
			this.rbAGCOff.Size = new System.Drawing.Size(45, 17);
			this.rbAGCOff.TabIndex = 20;
			this.rbAGCOff.TabStop = true;
			this.rbAGCOff.Text = "OFF";
			this.rbAGCOff.UseVisualStyleBackColor = true;
			this.rbAGCOff.CheckedChanged += new System.EventHandler(this.rbAGC_CheckedChanged);
			// 
			// cbLnaGain
			// 
			this.cbLnaGain.FormattingEnabled = true;
			this.cbLnaGain.Items.AddRange(new object[] {
            "not used",
            "G1(Max)",
            "G2",
            "G3",
            "G4",
            "G5",
            "G6(Min)",
            "not used"});
			this.cbLnaGain.Location = new System.Drawing.Point(109, 49);
			this.cbLnaGain.Name = "cbLnaGain";
			this.cbLnaGain.Size = new System.Drawing.Size(121, 21);
			this.cbLnaGain.TabIndex = 21;
			this.cbLnaGain.Text = "G1(Max)";
			this.cbLnaGain.SelectedValueChanged += new System.EventHandler(this.cbOcpTrimming_SelectedValueChanged);
			// 
			// pLNA
			// 
			this.pLNA.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.pLNA.Controls.Add(this.label17);
			this.pLNA.Controls.Add(this.rbLNAOff);
			this.pLNA.Controls.Add(this.rbLNAOn);
			this.pLNA.Location = new System.Drawing.Point(9, 77);
			this.pLNA.Name = "pLNA";
			this.pLNA.Size = new System.Drawing.Size(221, 26);
			this.pLNA.TabIndex = 24;
			// 
			// label17
			// 
			this.label17.AutoSize = true;
			this.label17.Location = new System.Drawing.Point(5, 5);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(58, 13);
			this.label17.TabIndex = 21;
			this.label17.Text = "LNA Boost";
			// 
			// rbLNAOff
			// 
			this.rbLNAOff.AutoSize = true;
			this.rbLNAOff.Checked = true;
			this.rbLNAOff.Location = new System.Drawing.Point(157, 3);
			this.rbLNAOff.Name = "rbLNAOff";
			this.rbLNAOff.Size = new System.Drawing.Size(45, 17);
			this.rbLNAOff.TabIndex = 23;
			this.rbLNAOff.TabStop = true;
			this.rbLNAOff.Text = "OFF";
			this.rbLNAOff.UseVisualStyleBackColor = true;
			this.rbLNAOff.CheckedChanged += new System.EventHandler(this.rbLNA_CheckedChanged);
			// 
			// rbLNAOn
			// 
			this.rbLNAOn.AutoSize = true;
			this.rbLNAOn.Location = new System.Drawing.Point(100, 4);
			this.rbLNAOn.Name = "rbLNAOn";
			this.rbLNAOn.Size = new System.Drawing.Size(41, 17);
			this.rbLNAOn.TabIndex = 22;
			this.rbLNAOn.Text = "ON";
			this.rbLNAOn.UseVisualStyleBackColor = true;
			this.rbLNAOn.CheckedChanged += new System.EventHandler(this.rbLNA_CheckedChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 54);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(53, 13);
			this.label2.TabIndex = 17;
			this.label2.Text = "LNA Gain";
			// 
			// gbLoraSetting
			// 
			this.gbLoraSetting.Controls.Add(this.label47);
			this.gbLoraSetting.Controls.Add(this.nudImplicitRxLength);
			this.gbLoraSetting.Controls.Add(this.cbImplicit);
			this.gbLoraSetting.Controls.Add(this.label46);
			this.gbLoraSetting.Controls.Add(this.label23);
			this.gbLoraSetting.Controls.Add(this.label27);
			this.gbLoraSetting.Controls.Add(this.lTs);
			this.gbLoraSetting.Controls.Add(this.panel2);
			this.gbLoraSetting.Controls.Add(this.nudPreambleLength);
			this.gbLoraSetting.Controls.Add(this.label25);
			this.gbLoraSetting.Controls.Add(this.pLROptimize);
			this.gbLoraSetting.Controls.Add(this.nudRxTimeOut);
			this.gbLoraSetting.Controls.Add(this.label22);
			this.gbLoraSetting.Controls.Add(this.label21);
			this.gbLoraSetting.Controls.Add(this.cbBW);
			this.gbLoraSetting.Controls.Add(this.label20);
			this.gbLoraSetting.Controls.Add(this.cbCR);
			this.gbLoraSetting.Controls.Add(this.label19);
			this.gbLoraSetting.Controls.Add(this.cbSF);
			this.gbLoraSetting.Controls.Add(this.label18);
			this.gbLoraSetting.Location = new System.Drawing.Point(245, 3);
			this.gbLoraSetting.Name = "gbLoraSetting";
			this.gbLoraSetting.Size = new System.Drawing.Size(241, 244);
			this.gbLoraSetting.TabIndex = 3;
			this.gbLoraSetting.TabStop = false;
			this.gbLoraSetting.Text = "LoRa Setting";
			// 
			// label47
			// 
			this.label47.AutoSize = true;
			this.label47.Location = new System.Drawing.Point(204, 217);
			this.label47.Name = "label47";
			this.label47.Size = new System.Drawing.Size(28, 13);
			this.label47.TabIndex = 25;
			this.label47.Text = "Byte";
			// 
			// nudImplicitRxLength
			// 
			this.nudImplicitRxLength.Location = new System.Drawing.Point(137, 212);
			this.nudImplicitRxLength.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.nudImplicitRxLength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudImplicitRxLength.Name = "nudImplicitRxLength";
			this.nudImplicitRxLength.Size = new System.Drawing.Size(61, 20);
			this.nudImplicitRxLength.TabIndex = 24;
			this.nudImplicitRxLength.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudImplicitRxLength.ValueChanged += new System.EventHandler(this.nudImplicitRxLength_ValueChanged);
			// 
			// cbImplicit
			// 
			this.cbImplicit.AutoSize = true;
			this.cbImplicit.Location = new System.Drawing.Point(107, 217);
			this.cbImplicit.Name = "cbImplicit";
			this.cbImplicit.Size = new System.Drawing.Size(15, 14);
			this.cbImplicit.TabIndex = 23;
			this.cbImplicit.UseVisualStyleBackColor = true;
			this.cbImplicit.CheckedChanged += new System.EventHandler(this.cbImplicit_CheckedChanged);
			// 
			// label46
			// 
			this.label46.AutoSize = true;
			this.label46.Location = new System.Drawing.Point(9, 219);
			this.label46.Name = "label46";
			this.label46.Size = new System.Drawing.Size(74, 13);
			this.label46.TabIndex = 22;
			this.label46.Text = "ImplicitHeader";
			// 
			// label23
			// 
			this.label23.AutoSize = true;
			this.label23.Location = new System.Drawing.Point(169, 105);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(13, 13);
			this.label23.TabIndex = 21;
			this.label23.Text = "×";
			// 
			// label27
			// 
			this.label27.AutoSize = true;
			this.label27.Location = new System.Drawing.Point(219, 104);
			this.label27.Name = "label27";
			this.label27.Size = new System.Drawing.Size(20, 13);
			this.label27.TabIndex = 21;
			this.label27.Text = "ms";
			// 
			// lTs
			// 
			this.lTs.AutoSize = true;
			this.lTs.Location = new System.Drawing.Point(185, 104);
			this.lTs.Name = "lTs";
			this.lTs.Size = new System.Drawing.Size(34, 13);
			this.lTs.TabIndex = 21;
			this.lTs.Text = "32.77";
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.rbPayloadCRCOff);
			this.panel2.Controls.Add(this.rbPayloadCRCOn);
			this.panel2.Controls.Add(this.label26);
			this.panel2.Location = new System.Drawing.Point(6, 185);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(212, 22);
			this.panel2.TabIndex = 20;
			// 
			// rbPayloadCRCOff
			// 
			this.rbPayloadCRCOff.AutoSize = true;
			this.rbPayloadCRCOff.Checked = true;
			this.rbPayloadCRCOff.Location = new System.Drawing.Point(151, 2);
			this.rbPayloadCRCOff.Name = "rbPayloadCRCOff";
			this.rbPayloadCRCOff.Size = new System.Drawing.Size(45, 17);
			this.rbPayloadCRCOff.TabIndex = 20;
			this.rbPayloadCRCOff.TabStop = true;
			this.rbPayloadCRCOff.Text = "OFF";
			this.rbPayloadCRCOff.UseVisualStyleBackColor = true;
			this.rbPayloadCRCOff.CheckedChanged += new System.EventHandler(this.rbPayloadCRC_CheckedChanged);
			// 
			// rbPayloadCRCOn
			// 
			this.rbPayloadCRCOn.AutoSize = true;
			this.rbPayloadCRCOn.Location = new System.Drawing.Point(102, 2);
			this.rbPayloadCRCOn.Name = "rbPayloadCRCOn";
			this.rbPayloadCRCOn.Size = new System.Drawing.Size(41, 17);
			this.rbPayloadCRCOn.TabIndex = 20;
			this.rbPayloadCRCOn.Text = "ON";
			this.rbPayloadCRCOn.UseVisualStyleBackColor = true;
			this.rbPayloadCRCOn.CheckedChanged += new System.EventHandler(this.rbPayloadCRC_CheckedChanged);
			// 
			// label26
			// 
			this.label26.AutoSize = true;
			this.label26.Location = new System.Drawing.Point(3, 4);
			this.label26.Name = "label26";
			this.label26.Size = new System.Drawing.Size(70, 13);
			this.label26.TabIndex = 11;
			this.label26.Text = "Payload CRC";
			// 
			// nudPreambleLength
			// 
			this.nudPreambleLength.Location = new System.Drawing.Point(108, 155);
			this.nudPreambleLength.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
			this.nudPreambleLength.Name = "nudPreambleLength";
			this.nudPreambleLength.Size = new System.Drawing.Size(88, 20);
			this.nudPreambleLength.TabIndex = 18;
			this.nudPreambleLength.ThousandsSeparator = true;
			this.nudPreambleLength.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
			this.nudPreambleLength.ValueChanged += new System.EventHandler(this.nudPreambleLength_ValueChanged);
			// 
			// label25
			// 
			this.label25.AutoSize = true;
			this.label25.Location = new System.Drawing.Point(7, 159);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(87, 13);
			this.label25.TabIndex = 17;
			this.label25.Text = "Preamble Length";
			// 
			// pLROptimize
			// 
			this.pLROptimize.Controls.Add(this.rbLROptimizeOff);
			this.pLROptimize.Controls.Add(this.rbLROptimizeOn);
			this.pLROptimize.Controls.Add(this.label24);
			this.pLROptimize.Location = new System.Drawing.Point(6, 128);
			this.pLROptimize.Name = "pLROptimize";
			this.pLROptimize.Size = new System.Drawing.Size(213, 22);
			this.pLROptimize.TabIndex = 16;
			// 
			// rbLROptimizeOff
			// 
			this.rbLROptimizeOff.AutoSize = true;
			this.rbLROptimizeOff.Checked = true;
			this.rbLROptimizeOff.Location = new System.Drawing.Point(151, 2);
			this.rbLROptimizeOff.Name = "rbLROptimizeOff";
			this.rbLROptimizeOff.Size = new System.Drawing.Size(45, 17);
			this.rbLROptimizeOff.TabIndex = 20;
			this.rbLROptimizeOff.TabStop = true;
			this.rbLROptimizeOff.Text = "OFF";
			this.rbLROptimizeOff.UseVisualStyleBackColor = true;
			this.rbLROptimizeOff.CheckedChanged += new System.EventHandler(this.rbLROptimize_CheckedChanged);
			// 
			// rbLROptimizeOn
			// 
			this.rbLROptimizeOn.AutoSize = true;
			this.rbLROptimizeOn.Location = new System.Drawing.Point(102, 2);
			this.rbLROptimizeOn.Name = "rbLROptimizeOn";
			this.rbLROptimizeOn.Size = new System.Drawing.Size(41, 17);
			this.rbLROptimizeOn.TabIndex = 20;
			this.rbLROptimizeOn.Text = "ON";
			this.rbLROptimizeOn.UseVisualStyleBackColor = true;
			this.rbLROptimizeOn.CheckedChanged += new System.EventHandler(this.rbLROptimize_CheckedChanged);
			// 
			// label24
			// 
			this.label24.AutoSize = true;
			this.label24.Location = new System.Drawing.Point(1, 4);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(90, 13);
			this.label24.TabIndex = 11;
			this.label24.Text = "LowRateOptimize";
			// 
			// nudRxTimeOut
			// 
			this.nudRxTimeOut.Location = new System.Drawing.Point(107, 100);
			this.nudRxTimeOut.Maximum = new decimal(new int[] {
            1023,
            0,
            0,
            0});
			this.nudRxTimeOut.Name = "nudRxTimeOut";
			this.nudRxTimeOut.Size = new System.Drawing.Size(59, 20);
			this.nudRxTimeOut.TabIndex = 14;
			this.nudRxTimeOut.ThousandsSeparator = true;
			this.nudRxTimeOut.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.nudRxTimeOut.ValueChanged += new System.EventHandler(this.nudRxTimeOut_ValueChanged);
			// 
			// label22
			// 
			this.label22.AutoSize = true;
			this.label22.Location = new System.Drawing.Point(6, 104);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(87, 13);
			this.label22.TabIndex = 10;
			this.label22.Text = "ReceiveTimeOut";
			// 
			// label21
			// 
			this.label21.AutoSize = true;
			this.label21.Location = new System.Drawing.Point(195, 75);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(27, 13);
			this.label21.TabIndex = 9;
			this.label21.Text = "KHz";
			// 
			// cbBW
			// 
			this.cbBW.FormattingEnabled = true;
			this.cbBW.Items.AddRange(new object[] {
            "62.5",
            "125",
            "250",
            "500"});
			this.cbBW.Location = new System.Drawing.Point(107, 73);
			this.cbBW.Name = "cbBW";
			this.cbBW.Size = new System.Drawing.Size(88, 21);
			this.cbBW.TabIndex = 6;
			this.cbBW.Text = "125";
			this.cbBW.SelectedValueChanged += new System.EventHandler(this.cbBW_SelectedValueChanged);
			// 
			// label20
			// 
			this.label20.AutoSize = true;
			this.label20.Location = new System.Drawing.Point(6, 76);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(63, 13);
			this.label20.TabIndex = 5;
			this.label20.Text = "Band Width";
			// 
			// cbCR
			// 
			this.cbCR.FormattingEnabled = true;
			this.cbCR.Items.AddRange(new object[] {
            "4/5",
            "4/6",
            "4/7",
            "4/8"});
			this.cbCR.Location = new System.Drawing.Point(107, 46);
			this.cbCR.Name = "cbCR";
			this.cbCR.Size = new System.Drawing.Size(88, 21);
			this.cbCR.TabIndex = 4;
			this.cbCR.Text = "4/5";
			this.cbCR.SelectedValueChanged += new System.EventHandler(this.cbCR_SelectedValueChanged);
			// 
			// label19
			// 
			this.label19.AutoSize = true;
			this.label19.Location = new System.Drawing.Point(6, 49);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(66, 13);
			this.label19.TabIndex = 3;
			this.label19.Text = "Coding Rate";
			// 
			// cbSF
			// 
			this.cbSF.FormattingEnabled = true;
			this.cbSF.Items.AddRange(new object[] {
            "SF6",
            "SF7",
            "SF8",
            "SF9",
            "SF10",
            "SF11",
            "SF12"});
			this.cbSF.Location = new System.Drawing.Point(107, 18);
			this.cbSF.Name = "cbSF";
			this.cbSF.Size = new System.Drawing.Size(88, 21);
			this.cbSF.TabIndex = 2;
			this.cbSF.Text = "SF12";
			this.cbSF.SelectedValueChanged += new System.EventHandler(this.cbSF_SelectedValueChanged);
			// 
			// label18
			// 
			this.label18.AutoSize = true;
			this.label18.Location = new System.Drawing.Point(6, 22);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(85, 13);
			this.label18.TabIndex = 1;
			this.label18.Text = "SpreadingFactor";
			// 
			// gbPHInfo
			// 
			this.gbPHInfo.Controls.Add(this.PayCRCLed);
			this.gbPHInfo.Controls.Add(this.label53);
			this.gbPHInfo.Controls.Add(this.label52);
			this.gbPHInfo.Controls.Add(this.label51);
			this.gbPHInfo.Controls.Add(this.label50);
			this.gbPHInfo.Controls.Add(this.CRC_LED);
			this.gbPHInfo.Controls.Add(this.label38);
			this.gbPHInfo.Controls.Add(this.tbRssiValue);
			this.gbPHInfo.Controls.Add(this.tbPktRssiValue);
			this.gbPHInfo.Controls.Add(this.tbPktSnrValue);
			this.gbPHInfo.Controls.Add(this.tbRxPacketCnt);
			this.gbPHInfo.Controls.Add(this.tbRxCR);
			this.gbPHInfo.Controls.Add(this.tbRxNbBytes);
			this.gbPHInfo.Controls.Add(this.tbRxHeaderCnt);
			this.gbPHInfo.Controls.Add(this.label42);
			this.gbPHInfo.Controls.Add(this.label41);
			this.gbPHInfo.Controls.Add(this.label40);
			this.gbPHInfo.Controls.Add(this.label39);
			this.gbPHInfo.Controls.Add(this.label37);
			this.gbPHInfo.Controls.Add(this.label28);
			this.gbPHInfo.Location = new System.Drawing.Point(245, 254);
			this.gbPHInfo.Name = "gbPHInfo";
			this.gbPHInfo.Size = new System.Drawing.Size(296, 143);
			this.gbPHInfo.TabIndex = 6;
			this.gbPHInfo.TabStop = false;
			this.gbPHInfo.Text = "Packet Info";
			// 
			// PayCRCLed
			// 
			this.PayCRCLed.BackColor = System.Drawing.Color.Transparent;
			this.PayCRCLed.LedColor = System.Drawing.Color.Red;
			this.PayCRCLed.LedSize = new System.Drawing.Size(11, 11);
			this.PayCRCLed.Location = new System.Drawing.Point(121, 53);
			this.PayCRCLed.Name = "PayCRCLed";
			this.PayCRCLed.Size = new System.Drawing.Size(15, 16);
			this.PayCRCLed.TabIndex = 8;
			this.PayCRCLed.Text = "led1";
			// 
			// label53
			// 
			this.label53.AutoSize = true;
			this.label53.Location = new System.Drawing.Point(6, 116);
			this.label53.Name = "label53";
			this.label53.Size = new System.Drawing.Size(63, 13);
			this.label53.TabIndex = 7;
			this.label53.Text = "CodingRate";
			// 
			// label52
			// 
			this.label52.AutoSize = true;
			this.label52.Location = new System.Drawing.Point(268, 114);
			this.label52.Name = "label52";
			this.label52.Size = new System.Drawing.Size(28, 13);
			this.label52.TabIndex = 6;
			this.label52.Text = "dBm";
			// 
			// label51
			// 
			this.label51.AutoSize = true;
			this.label51.Location = new System.Drawing.Point(268, 85);
			this.label51.Name = "label51";
			this.label51.Size = new System.Drawing.Size(28, 13);
			this.label51.TabIndex = 6;
			this.label51.Text = "dBm";
			// 
			// label50
			// 
			this.label50.AutoSize = true;
			this.label50.Location = new System.Drawing.Point(268, 54);
			this.label50.Name = "label50";
			this.label50.Size = new System.Drawing.Size(20, 13);
			this.label50.TabIndex = 6;
			this.label50.Text = "dB";
			// 
			// CRC_LED
			// 
			this.CRC_LED.BackColor = System.Drawing.Color.Transparent;
			this.CRC_LED.LedColor = System.Drawing.Color.Red;
			this.CRC_LED.LedSize = new System.Drawing.Size(11, 11);
			this.CRC_LED.Location = new System.Drawing.Point(214, 16);
			this.CRC_LED.Name = "CRC_LED";
			this.CRC_LED.Size = new System.Drawing.Size(20, 25);
			this.CRC_LED.TabIndex = 5;
			this.CRC_LED.Text = "led1";
			// 
			// label38
			// 
			this.label38.AutoSize = true;
			this.label38.Location = new System.Drawing.Point(166, 23);
			this.label38.Name = "label38";
			this.label38.Size = new System.Drawing.Size(48, 13);
			this.label38.TabIndex = 4;
			this.label38.Text = "CRC ON";
			// 
			// tbRssiValue
			// 
			this.tbRssiValue.Location = new System.Drawing.Point(214, 109);
			this.tbRssiValue.Name = "tbRssiValue";
			this.tbRssiValue.ReadOnly = true;
			this.tbRssiValue.Size = new System.Drawing.Size(50, 20);
			this.tbRssiValue.TabIndex = 1;
			// 
			// tbPktRssiValue
			// 
			this.tbPktRssiValue.Location = new System.Drawing.Point(214, 80);
			this.tbPktRssiValue.Name = "tbPktRssiValue";
			this.tbPktRssiValue.ReadOnly = true;
			this.tbPktRssiValue.Size = new System.Drawing.Size(50, 20);
			this.tbPktRssiValue.TabIndex = 1;
			// 
			// tbPktSnrValue
			// 
			this.tbPktSnrValue.Location = new System.Drawing.Point(214, 50);
			this.tbPktSnrValue.Name = "tbPktSnrValue";
			this.tbPktSnrValue.ReadOnly = true;
			this.tbPktSnrValue.Size = new System.Drawing.Size(50, 20);
			this.tbPktSnrValue.TabIndex = 1;
			// 
			// tbRxPacketCnt
			// 
			this.tbRxPacketCnt.Location = new System.Drawing.Point(71, 50);
			this.tbRxPacketCnt.Name = "tbRxPacketCnt";
			this.tbRxPacketCnt.ReadOnly = true;
			this.tbRxPacketCnt.Size = new System.Drawing.Size(49, 20);
			this.tbRxPacketCnt.TabIndex = 1;
			// 
			// tbRxCR
			// 
			this.tbRxCR.Location = new System.Drawing.Point(71, 112);
			this.tbRxCR.Name = "tbRxCR";
			this.tbRxCR.ReadOnly = true;
			this.tbRxCR.Size = new System.Drawing.Size(49, 20);
			this.tbRxCR.TabIndex = 1;
			// 
			// tbRxNbBytes
			// 
			this.tbRxNbBytes.Location = new System.Drawing.Point(71, 81);
			this.tbRxNbBytes.Name = "tbRxNbBytes";
			this.tbRxNbBytes.ReadOnly = true;
			this.tbRxNbBytes.Size = new System.Drawing.Size(49, 20);
			this.tbRxNbBytes.TabIndex = 1;
			// 
			// tbRxHeaderCnt
			// 
			this.tbRxHeaderCnt.Location = new System.Drawing.Point(71, 20);
			this.tbRxHeaderCnt.Name = "tbRxHeaderCnt";
			this.tbRxHeaderCnt.ReadOnly = true;
			this.tbRxHeaderCnt.Size = new System.Drawing.Size(49, 20);
			this.tbRxHeaderCnt.TabIndex = 1;
			// 
			// label42
			// 
			this.label42.AutoSize = true;
			this.label42.Location = new System.Drawing.Point(140, 114);
			this.label42.Name = "label42";
			this.label42.Size = new System.Drawing.Size(66, 13);
			this.label42.TabIndex = 0;
			this.label42.Text = "CurrentRSSI";
			// 
			// label41
			// 
			this.label41.AutoSize = true;
			this.label41.Location = new System.Drawing.Point(146, 86);
			this.label41.Name = "label41";
			this.label41.Size = new System.Drawing.Size(66, 13);
			this.label41.TabIndex = 0;
			this.label41.Text = "PacketRSSI";
			// 
			// label40
			// 
			this.label40.AutoSize = true;
			this.label40.Location = new System.Drawing.Point(151, 54);
			this.label40.Name = "label40";
			this.label40.Size = new System.Drawing.Size(64, 13);
			this.label40.TabIndex = 0;
			this.label40.Text = "PacketSNR";
			// 
			// label39
			// 
			this.label39.AutoSize = true;
			this.label39.Location = new System.Drawing.Point(6, 54);
			this.label39.Name = "label39";
			this.label39.Size = new System.Drawing.Size(57, 13);
			this.label39.TabIndex = 0;
			this.label39.Text = "PacketCnt";
			// 
			// label37
			// 
			this.label37.AutoSize = true;
			this.label37.Location = new System.Drawing.Point(6, 86);
			this.label37.Name = "label37";
			this.label37.Size = new System.Drawing.Size(62, 13);
			this.label37.TabIndex = 0;
			this.label37.Text = "RxBytesCnt";
			// 
			// label28
			// 
			this.label28.AutoSize = true;
			this.label28.Location = new System.Drawing.Point(6, 23);
			this.label28.Name = "label28";
			this.label28.Size = new System.Drawing.Size(58, 13);
			this.label28.TabIndex = 0;
			this.label28.Text = "HeaderCnt";
			// 
			// gbMessage
			// 
			this.gbMessage.Controls.Add(this.cbNewLine);
			this.gbMessage.Controls.Add(this.PllLockLed);
			this.gbMessage.Controls.Add(this.label49);
			this.gbMessage.Controls.Add(this.cbHex);
			this.gbMessage.Controls.Add(this.rbClear);
			this.gbMessage.Controls.Add(this.bbPayload);
			this.gbMessage.Location = new System.Drawing.Point(4, 403);
			this.gbMessage.Name = "gbMessage";
			this.gbMessage.Size = new System.Drawing.Size(537, 155);
			this.gbMessage.TabIndex = 7;
			this.gbMessage.TabStop = false;
			this.gbMessage.Text = "Message";
			// 
			// cbNewLine
			// 
			this.cbNewLine.AutoSize = true;
			this.cbNewLine.Location = new System.Drawing.Point(457, 73);
			this.cbNewLine.Name = "cbNewLine";
			this.cbNewLine.Size = new System.Drawing.Size(68, 17);
			this.cbNewLine.TabIndex = 8;
			this.cbNewLine.Text = "NewLine";
			this.cbNewLine.UseVisualStyleBackColor = true;
			// 
			// PllLockLed
			// 
			this.PllLockLed.BackColor = System.Drawing.Color.Transparent;
			this.PllLockLed.LedColor = System.Drawing.Color.Red;
			this.PllLockLed.LedSize = new System.Drawing.Size(11, 11);
			this.PllLockLed.Location = new System.Drawing.Point(515, 20);
			this.PllLockLed.Name = "PllLockLed";
			this.PllLockLed.Size = new System.Drawing.Size(15, 16);
			this.PllLockLed.TabIndex = 7;
			this.PllLockLed.Text = "led2";
			// 
			// label49
			// 
			this.label49.AutoSize = true;
			this.label49.Location = new System.Drawing.Point(456, 23);
			this.label49.Name = "label49";
			this.label49.Size = new System.Drawing.Size(53, 13);
			this.label49.TabIndex = 6;
			this.label49.Text = "PLL Lock";
			// 
			// cbHex
			// 
			this.cbHex.AutoSize = true;
			this.cbHex.Location = new System.Drawing.Point(457, 49);
			this.cbHex.Name = "cbHex";
			this.cbHex.Size = new System.Drawing.Size(48, 17);
			this.cbHex.TabIndex = 6;
			this.cbHex.Text = "HEX";
			this.cbHex.UseVisualStyleBackColor = true;
			this.cbHex.CheckedChanged += new System.EventHandler(this.PayloadText_CheckedChange);
			// 
			// rbClear
			// 
			this.rbClear.Location = new System.Drawing.Point(457, 96);
			this.rbClear.Name = "rbClear";
			this.rbClear.Size = new System.Drawing.Size(72, 34);
			this.rbClear.TabIndex = 5;
			this.rbClear.Text = "Clear";
			this.rbClear.UseVisualStyleBackColor = true;
			this.rbClear.Click += new System.EventHandler(this.rbClear_Click);
			// 
			// bbPayload
			// 
			this.bbPayload.IsHex = false;
			this.bbPayload.Location = new System.Drawing.Point(6, 17);
			this.bbPayload.Multiline = true;
			this.bbPayload.Name = "bbPayload";
			this.bbPayload.ReadOnly = true;
			this.bbPayload.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.bbPayload.Size = new System.Drawing.Size(445, 131);
			this.bbPayload.TabIndex = 3;
			// 
			// gbOpMode
			// 
			this.gbOpMode.Controls.Add(this.rbGoTxTest);
			this.gbOpMode.Controls.Add(this.rbGoRxTest);
			this.gbOpMode.Controls.Add(this.rbGoSleep);
			this.gbOpMode.Controls.Add(this.rbGoCAD);
			this.gbOpMode.Controls.Add(this.nudTxDelay);
			this.gbOpMode.Controls.Add(this.rbGoTx);
			this.gbOpMode.Controls.Add(this.rbGoRx);
			this.gbOpMode.Controls.Add(this.label45);
			this.gbOpMode.Controls.Add(this.tbTxPktCnt);
			this.gbOpMode.Controls.Add(this.label44);
			this.gbOpMode.Controls.Add(this.label43);
			this.gbOpMode.Controls.Add(this.bSwitch);
			this.gbOpMode.Enabled = false;
			this.gbOpMode.Location = new System.Drawing.Point(547, 254);
			this.gbOpMode.Name = "gbOpMode";
			this.gbOpMode.Size = new System.Drawing.Size(124, 304);
			this.gbOpMode.TabIndex = 8;
			this.gbOpMode.TabStop = false;
			this.gbOpMode.Text = "Operator";
			// 
			// rbGoTxTest
			// 
			this.rbGoTxTest.AutoSize = true;
			this.rbGoTxTest.Location = new System.Drawing.Point(29, 182);
			this.rbGoTxTest.Name = "rbGoTxTest";
			this.rbGoTxTest.Size = new System.Drawing.Size(63, 17);
			this.rbGoTxTest.TabIndex = 9;
			this.rbGoTxTest.TabStop = true;
			this.rbGoTxTest.Text = "TX Test";
			this.rbGoTxTest.UseVisualStyleBackColor = true;
			// 
			// rbGoRxTest
			// 
			this.rbGoRxTest.AutoSize = true;
			this.rbGoRxTest.Location = new System.Drawing.Point(29, 151);
			this.rbGoRxTest.Name = "rbGoRxTest";
			this.rbGoRxTest.Size = new System.Drawing.Size(64, 17);
			this.rbGoRxTest.TabIndex = 8;
			this.rbGoRxTest.TabStop = true;
			this.rbGoRxTest.Text = "RX Test";
			this.rbGoRxTest.UseVisualStyleBackColor = true;
			// 
			// rbGoSleep
			// 
			this.rbGoSleep.AutoSize = true;
			this.rbGoSleep.Location = new System.Drawing.Point(29, 119);
			this.rbGoSleep.Name = "rbGoSleep";
			this.rbGoSleep.Size = new System.Drawing.Size(52, 17);
			this.rbGoSleep.TabIndex = 7;
			this.rbGoSleep.TabStop = true;
			this.rbGoSleep.Text = "Sleep";
			this.rbGoSleep.UseVisualStyleBackColor = true;
			this.rbGoSleep.CheckedChanged += new System.EventHandler(this.rbStatus_CheckedChanged);
			// 
			// rbGoCAD
			// 
			this.rbGoCAD.AutoSize = true;
			this.rbGoCAD.Location = new System.Drawing.Point(29, 88);
			this.rbGoCAD.Name = "rbGoCAD";
			this.rbGoCAD.Size = new System.Drawing.Size(76, 17);
			this.rbGoCAD.TabIndex = 6;
			this.rbGoCAD.TabStop = true;
			this.rbGoCAD.Text = "CAD LoRa";
			this.rbGoCAD.UseVisualStyleBackColor = true;
			this.rbGoCAD.CheckedChanged += new System.EventHandler(this.rbStatus_CheckedChanged);
			// 
			// nudTxDelay
			// 
			this.nudTxDelay.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.nudTxDelay.Location = new System.Drawing.Point(46, 246);
			this.nudTxDelay.Maximum = new decimal(new int[] {
            4000,
            0,
            0,
            0});
			this.nudTxDelay.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.nudTxDelay.Name = "nudTxDelay";
			this.nudTxDelay.Size = new System.Drawing.Size(51, 20);
			this.nudTxDelay.TabIndex = 3;
			this.nudTxDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.nudTxDelay.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
			// 
			// rbGoTx
			// 
			this.rbGoTx.AutoSize = true;
			this.rbGoTx.Location = new System.Drawing.Point(29, 56);
			this.rbGoTx.Name = "rbGoTx";
			this.rbGoTx.Size = new System.Drawing.Size(68, 17);
			this.rbGoTx.TabIndex = 5;
			this.rbGoTx.Text = "TX LoRa";
			this.rbGoTx.UseVisualStyleBackColor = true;
			this.rbGoTx.CheckedChanged += new System.EventHandler(this.rbStatus_CheckedChanged);
			// 
			// rbGoRx
			// 
			this.rbGoRx.AutoSize = true;
			this.rbGoRx.Checked = true;
			this.rbGoRx.Location = new System.Drawing.Point(29, 25);
			this.rbGoRx.Name = "rbGoRx";
			this.rbGoRx.Size = new System.Drawing.Size(69, 17);
			this.rbGoRx.TabIndex = 5;
			this.rbGoRx.TabStop = true;
			this.rbGoRx.Text = "RX LoRa";
			this.rbGoRx.UseVisualStyleBackColor = true;
			this.rbGoRx.CheckedChanged += new System.EventHandler(this.rbStatus_CheckedChanged);
			// 
			// label45
			// 
			this.label45.AutoSize = true;
			this.label45.Location = new System.Drawing.Point(101, 249);
			this.label45.Name = "label45";
			this.label45.Size = new System.Drawing.Size(20, 13);
			this.label45.TabIndex = 3;
			this.label45.Text = "ms";
			// 
			// tbTxPktCnt
			// 
			this.tbTxPktCnt.Location = new System.Drawing.Point(46, 275);
			this.tbTxPktCnt.Name = "tbTxPktCnt";
			this.tbTxPktCnt.ReadOnly = true;
			this.tbTxPktCnt.Size = new System.Drawing.Size(72, 20);
			this.tbTxPktCnt.TabIndex = 2;
			// 
			// label44
			// 
			this.label44.AutoSize = true;
			this.label44.Location = new System.Drawing.Point(6, 250);
			this.label44.Name = "label44";
			this.label44.Size = new System.Drawing.Size(25, 13);
			this.label44.TabIndex = 1;
			this.label44.Text = "INT";
			// 
			// label43
			// 
			this.label43.AutoSize = true;
			this.label43.Location = new System.Drawing.Point(6, 280);
			this.label43.Name = "label43";
			this.label43.Size = new System.Drawing.Size(35, 13);
			this.label43.TabIndex = 1;
			this.label43.Text = "TxCnt";
			// 
			// bSwitch
			// 
			this.bSwitch.Location = new System.Drawing.Point(7, 215);
			this.bSwitch.Name = "bSwitch";
			this.bSwitch.Size = new System.Drawing.Size(110, 25);
			this.bSwitch.TabIndex = 0;
			this.bSwitch.Text = "Start";
			this.bSwitch.UseVisualStyleBackColor = true;
			this.bSwitch.Click += new System.EventHandler(this.bSwitch_Click);
			// 
			// cbRxDone
			// 
			this.cbRxDone.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.cbRxDone.AutoSize = true;
			this.cbRxDone.Location = new System.Drawing.Point(112, 49);
			this.cbRxDone.Name = "cbRxDone";
			this.cbRxDone.Size = new System.Drawing.Size(15, 14);
			this.cbRxDone.TabIndex = 1;
			this.cbRxDone.UseVisualStyleBackColor = true;
			this.cbRxDone.CheckedChanged += new System.EventHandler(this.cbRxDone_CheckedChanged);
			// 
			// cbCRCError
			// 
			this.cbCRCError.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.cbCRCError.AutoSize = true;
			this.cbCRCError.Location = new System.Drawing.Point(112, 75);
			this.cbCRCError.Name = "cbCRCError";
			this.cbCRCError.Size = new System.Drawing.Size(15, 14);
			this.cbCRCError.TabIndex = 1;
			this.cbCRCError.UseVisualStyleBackColor = true;
			this.cbCRCError.CheckedChanged += new System.EventHandler(this.cbCRCError_CheckedChanged);
			// 
			// cbValidHeader
			// 
			this.cbValidHeader.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.cbValidHeader.AutoSize = true;
			this.cbValidHeader.Location = new System.Drawing.Point(112, 102);
			this.cbValidHeader.Name = "cbValidHeader";
			this.cbValidHeader.Size = new System.Drawing.Size(15, 14);
			this.cbValidHeader.TabIndex = 1;
			this.cbValidHeader.UseVisualStyleBackColor = true;
			this.cbValidHeader.CheckedChanged += new System.EventHandler(this.cbValidHeader_CheckedChanged);
			// 
			// label29
			// 
			this.label29.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label29.AutoSize = true;
			this.label29.Location = new System.Drawing.Point(15, 50);
			this.label29.Name = "label29";
			this.label29.Size = new System.Drawing.Size(46, 13);
			this.label29.TabIndex = 0;
			this.label29.Text = "RxDone";
			// 
			// cbRxTimeOut
			// 
			this.cbRxTimeOut.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.cbRxTimeOut.AutoSize = true;
			this.cbRxTimeOut.Location = new System.Drawing.Point(112, 21);
			this.cbRxTimeOut.Name = "cbRxTimeOut";
			this.cbRxTimeOut.Size = new System.Drawing.Size(15, 14);
			this.cbRxTimeOut.TabIndex = 1;
			this.cbRxTimeOut.UseVisualStyleBackColor = true;
			this.cbRxTimeOut.CheckedChanged += new System.EventHandler(this.cbRxTimeOut_CheckedChanged);
			// 
			// cbTxDone
			// 
			this.cbTxDone.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.cbTxDone.AutoSize = true;
			this.cbTxDone.Location = new System.Drawing.Point(112, 129);
			this.cbTxDone.Name = "cbTxDone";
			this.cbTxDone.Size = new System.Drawing.Size(15, 14);
			this.cbTxDone.TabIndex = 1;
			this.cbTxDone.UseVisualStyleBackColor = true;
			this.cbTxDone.CheckedChanged += new System.EventHandler(this.cbTxDone_CheckedChanged);
			// 
			// label36
			// 
			this.label36.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label36.AutoSize = true;
			this.label36.Location = new System.Drawing.Point(15, 23);
			this.label36.Name = "label36";
			this.label36.Size = new System.Drawing.Size(60, 13);
			this.label36.TabIndex = 0;
			this.label36.Text = "RxTimeOut";
			// 
			// label31
			// 
			this.label31.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label31.AutoSize = true;
			this.label31.Location = new System.Drawing.Point(15, 104);
			this.label31.Name = "label31";
			this.label31.Size = new System.Drawing.Size(65, 13);
			this.label31.TabIndex = 0;
			this.label31.Text = "ValidHeader";
			// 
			// cbCADDone
			// 
			this.cbCADDone.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.cbCADDone.AutoSize = true;
			this.cbCADDone.Location = new System.Drawing.Point(112, 156);
			this.cbCADDone.Name = "cbCADDone";
			this.cbCADDone.Size = new System.Drawing.Size(15, 14);
			this.cbCADDone.TabIndex = 1;
			this.cbCADDone.UseVisualStyleBackColor = true;
			this.cbCADDone.CheckedChanged += new System.EventHandler(this.cbCADDone_CheckedChanged);
			// 
			// label32
			// 
			this.label32.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label32.AutoSize = true;
			this.label32.Location = new System.Drawing.Point(15, 131);
			this.label32.Name = "label32";
			this.label32.Size = new System.Drawing.Size(45, 13);
			this.label32.TabIndex = 0;
			this.label32.Text = "TxDone";
			// 
			// label33
			// 
			this.label33.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label33.AutoSize = true;
			this.label33.Location = new System.Drawing.Point(15, 158);
			this.label33.Name = "label33";
			this.label33.Size = new System.Drawing.Size(58, 13);
			this.label33.TabIndex = 0;
			this.label33.Text = "CAD Done";
			// 
			// cbFHSSChannel
			// 
			this.cbFHSSChannel.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.cbFHSSChannel.AutoSize = true;
			this.cbFHSSChannel.Location = new System.Drawing.Point(112, 183);
			this.cbFHSSChannel.Name = "cbFHSSChannel";
			this.cbFHSSChannel.Size = new System.Drawing.Size(15, 14);
			this.cbFHSSChannel.TabIndex = 1;
			this.cbFHSSChannel.UseVisualStyleBackColor = true;
			this.cbFHSSChannel.CheckedChanged += new System.EventHandler(this.cbFHSSChannel_CheckedChanged);
			// 
			// label34
			// 
			this.label34.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label34.AutoSize = true;
			this.label34.Location = new System.Drawing.Point(15, 185);
			this.label34.Name = "label34";
			this.label34.Size = new System.Drawing.Size(77, 13);
			this.label34.TabIndex = 0;
			this.label34.Text = "FHSS Channel";
			// 
			// label35
			// 
			this.label35.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label35.AutoSize = true;
			this.label35.Location = new System.Drawing.Point(15, 212);
			this.label35.Name = "label35";
			this.label35.Size = new System.Drawing.Size(64, 13);
			this.label35.TabIndex = 0;
			this.label35.Text = "CAD Detect";
			// 
			// cbCADDetect
			// 
			this.cbCADDetect.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.cbCADDetect.AutoSize = true;
			this.cbCADDetect.Location = new System.Drawing.Point(112, 209);
			this.cbCADDetect.Name = "cbCADDetect";
			this.cbCADDetect.Size = new System.Drawing.Size(15, 14);
			this.cbCADDetect.TabIndex = 1;
			this.cbCADDetect.UseVisualStyleBackColor = true;
			this.cbCADDetect.CheckedChanged += new System.EventHandler(this.cbCADDetect_CheckedChanged);
			// 
			// gbIRQMask
			// 
			this.gbIRQMask.Controls.Add(this.CADDetectLed);
			this.gbIRQMask.Controls.Add(this.FHSSChannelLed);
			this.gbIRQMask.Controls.Add(this.CADDoneLed);
			this.gbIRQMask.Controls.Add(this.TxDoneLed);
			this.gbIRQMask.Controls.Add(this.ValidHeaderLed);
			this.gbIRQMask.Controls.Add(this.CRCErrorLed);
			this.gbIRQMask.Controls.Add(this.RxDoneLed);
			this.gbIRQMask.Controls.Add(this.RxTimeOutLed);
			this.gbIRQMask.Controls.Add(this.cbCADDetect);
			this.gbIRQMask.Controls.Add(this.label35);
			this.gbIRQMask.Controls.Add(this.label34);
			this.gbIRQMask.Controls.Add(this.cbFHSSChannel);
			this.gbIRQMask.Controls.Add(this.label33);
			this.gbIRQMask.Controls.Add(this.label32);
			this.gbIRQMask.Controls.Add(this.cbCADDone);
			this.gbIRQMask.Controls.Add(this.label31);
			this.gbIRQMask.Controls.Add(this.label36);
			this.gbIRQMask.Controls.Add(this.cbTxDone);
			this.gbIRQMask.Controls.Add(this.cbRxTimeOut);
			this.gbIRQMask.Controls.Add(this.label29);
			this.gbIRQMask.Controls.Add(this.cbValidHeader);
			this.gbIRQMask.Controls.Add(this.cbCRCError);
			this.gbIRQMask.Controls.Add(this.cbRxDone);
			this.gbIRQMask.Controls.Add(this.label30);
			this.gbIRQMask.Location = new System.Drawing.Point(492, 3);
			this.gbIRQMask.Name = "gbIRQMask";
			this.gbIRQMask.Size = new System.Drawing.Size(179, 244);
			this.gbIRQMask.TabIndex = 5;
			this.gbIRQMask.TabStop = false;
			this.gbIRQMask.Text = "IRQ Mask";
			// 
			// CADDetectLed
			// 
			this.CADDetectLed.BackColor = System.Drawing.Color.Transparent;
			this.CADDetectLed.LedColor = System.Drawing.Color.Lime;
			this.CADDetectLed.LedSize = new System.Drawing.Size(11, 11);
			this.CADDetectLed.Location = new System.Drawing.Point(143, 208);
			this.CADDetectLed.Name = "CADDetectLed";
			this.CADDetectLed.Size = new System.Drawing.Size(15, 18);
			this.CADDetectLed.TabIndex = 2;
			this.CADDetectLed.Text = "led1";
			// 
			// FHSSChannelLed
			// 
			this.FHSSChannelLed.BackColor = System.Drawing.Color.Transparent;
			this.FHSSChannelLed.LedColor = System.Drawing.Color.Lime;
			this.FHSSChannelLed.LedSize = new System.Drawing.Size(11, 11);
			this.FHSSChannelLed.Location = new System.Drawing.Point(143, 183);
			this.FHSSChannelLed.Name = "FHSSChannelLed";
			this.FHSSChannelLed.Size = new System.Drawing.Size(15, 18);
			this.FHSSChannelLed.TabIndex = 2;
			this.FHSSChannelLed.Text = "led1";
			// 
			// CADDoneLed
			// 
			this.CADDoneLed.BackColor = System.Drawing.Color.Transparent;
			this.CADDoneLed.LedColor = System.Drawing.Color.Lime;
			this.CADDoneLed.LedSize = new System.Drawing.Size(11, 11);
			this.CADDoneLed.Location = new System.Drawing.Point(143, 156);
			this.CADDoneLed.Name = "CADDoneLed";
			this.CADDoneLed.Size = new System.Drawing.Size(15, 18);
			this.CADDoneLed.TabIndex = 2;
			this.CADDoneLed.Text = "led1";
			// 
			// TxDoneLed
			// 
			this.TxDoneLed.BackColor = System.Drawing.Color.Transparent;
			this.TxDoneLed.LedColor = System.Drawing.Color.Lime;
			this.TxDoneLed.LedSize = new System.Drawing.Size(11, 11);
			this.TxDoneLed.Location = new System.Drawing.Point(143, 126);
			this.TxDoneLed.Name = "TxDoneLed";
			this.TxDoneLed.Size = new System.Drawing.Size(15, 18);
			this.TxDoneLed.TabIndex = 2;
			this.TxDoneLed.Text = "led1";
			// 
			// ValidHeaderLed
			// 
			this.ValidHeaderLed.BackColor = System.Drawing.Color.Transparent;
			this.ValidHeaderLed.LedColor = System.Drawing.Color.Lime;
			this.ValidHeaderLed.LedSize = new System.Drawing.Size(11, 11);
			this.ValidHeaderLed.Location = new System.Drawing.Point(143, 101);
			this.ValidHeaderLed.Name = "ValidHeaderLed";
			this.ValidHeaderLed.Size = new System.Drawing.Size(15, 16);
			this.ValidHeaderLed.TabIndex = 2;
			this.ValidHeaderLed.Text = "led1";
			// 
			// CRCErrorLed
			// 
			this.CRCErrorLed.BackColor = System.Drawing.Color.Transparent;
			this.CRCErrorLed.LedColor = System.Drawing.Color.Lime;
			this.CRCErrorLed.LedSize = new System.Drawing.Size(11, 11);
			this.CRCErrorLed.Location = new System.Drawing.Point(143, 75);
			this.CRCErrorLed.Name = "CRCErrorLed";
			this.CRCErrorLed.Size = new System.Drawing.Size(15, 16);
			this.CRCErrorLed.TabIndex = 2;
			this.CRCErrorLed.Text = "led1";
			// 
			// RxDoneLed
			// 
			this.RxDoneLed.BackColor = System.Drawing.Color.Transparent;
			this.RxDoneLed.LedColor = System.Drawing.Color.Lime;
			this.RxDoneLed.LedSize = new System.Drawing.Size(11, 11);
			this.RxDoneLed.Location = new System.Drawing.Point(143, 47);
			this.RxDoneLed.Name = "RxDoneLed";
			this.RxDoneLed.Size = new System.Drawing.Size(15, 16);
			this.RxDoneLed.TabIndex = 2;
			this.RxDoneLed.Text = "led1";
			// 
			// RxTimeOutLed
			// 
			this.RxTimeOutLed.BackColor = System.Drawing.Color.Transparent;
			this.RxTimeOutLed.LedColor = System.Drawing.Color.Lime;
			this.RxTimeOutLed.LedSize = new System.Drawing.Size(11, 11);
			this.RxTimeOutLed.Location = new System.Drawing.Point(143, 18);
			this.RxTimeOutLed.Name = "RxTimeOutLed";
			this.RxTimeOutLed.Size = new System.Drawing.Size(15, 16);
			this.RxTimeOutLed.TabIndex = 2;
			this.RxTimeOutLed.Text = "led1";
			// 
			// label30
			// 
			this.label30.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label30.AutoSize = true;
			this.label30.Location = new System.Drawing.Point(15, 77);
			this.label30.Name = "label30";
			this.label30.Size = new System.Drawing.Size(54, 13);
			this.label30.TabIndex = 0;
			this.label30.Text = "CRC Error";
			// 
			// RxChkTimer
			// 
			this.RxChkTimer.Interval = 10;
			this.RxChkTimer.Tick += new System.EventHandler(this.RxChkTimer_Tick);
			// 
			// TxInterTimer
			// 
			this.TxInterTimer.Tick += new System.EventHandler(this.TxInterTimer_Tick);
			// 
			// RxRssitimer
			// 
			this.RxRssitimer.Enabled = true;
			this.RxRssitimer.Tick += new System.EventHandler(this.RxRssitimer_Tick);
			// 
			// ucLoRa
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.gbOpMode);
			this.Controls.Add(this.gbMessage);
			this.Controls.Add(this.gbIRQMask);
			this.Controls.Add(this.gbPHInfo);
			this.Controls.Add(this.gbLoraSetting);
			this.Controls.Add(this.gbRxSetting);
			this.Controls.Add(this.gbTxSetting);
			this.Controls.Add(this.gbCrystal);
			this.Name = "ucLoRa";
			this.Size = new System.Drawing.Size(677, 561);
			this.gbCrystal.ResumeLayout(false);
			this.gbCrystal.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudRadioFreq)).EndInit();
			this.gbTxSetting.ResumeLayout(false);
			this.gbTxSetting.PerformLayout();
			this.gbRxSetting.ResumeLayout(false);
			this.gbRxSetting.PerformLayout();
			this.pAGC.ResumeLayout(false);
			this.pAGC.PerformLayout();
			this.pLNA.ResumeLayout(false);
			this.pLNA.PerformLayout();
			this.gbLoraSetting.ResumeLayout(false);
			this.gbLoraSetting.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudImplicitRxLength)).EndInit();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudPreambleLength)).EndInit();
			this.pLROptimize.ResumeLayout(false);
			this.pLROptimize.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudRxTimeOut)).EndInit();
			this.gbPHInfo.ResumeLayout(false);
			this.gbPHInfo.PerformLayout();
			this.gbMessage.ResumeLayout(false);
			this.gbMessage.PerformLayout();
			this.gbOpMode.ResumeLayout(false);
			this.gbOpMode.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudTxDelay)).EndInit();
			this.gbIRQMask.ResumeLayout(false);
			this.gbIRQMask.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion
	}
}
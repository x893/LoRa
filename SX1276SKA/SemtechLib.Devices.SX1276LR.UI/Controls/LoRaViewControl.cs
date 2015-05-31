using SemtechLib.Controls;
using SemtechLib.Controls.HexBoxCtrl;
using SemtechLib.Devices.SX1276LR.Enumerations;
using SemtechLib.General.Events;
using SemtechLib.General.Interfaces;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SemtechLib.Devices.SX1276LR.UI.Controls
{
	public class LoRaViewControl : UserControl, INotifyDocumentationChanged
	{
		private Decimal frequencyXo = new Decimal(32000000);
		private OperatingModeEnum mode = OperatingModeEnum.Stdby;
		private bool inHexPayloadDataChanged;
		private bool isDebugOn;
		private Decimal symbolTime;
		private byte[] payload;
		private IContainer components;
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
		private GroupBoxEx gBoxIrqMask;
		private Panel panel10;
		private RadioButton rBtnCadDetectedMaskOff;
		private RadioButton rBtnCadDetectedMaskOn;
		private Label label7;
		private Panel panel9;
		private RadioButton rBtnFhssChangeChannelMaskOff;
		private RadioButton rBtnFhssChangeChannelMaskOn;
		private Label label6;
		private Panel panel8;
		private RadioButton rBtnCadDoneMaskOff;
		private RadioButton rBtnCadDoneMaskOn;
		private Label label5;
		private Panel panel5;
		private RadioButton rBtnTxDoneMaskOff;
		private RadioButton rBtnTxDoneMaskOn;
		private Label label4;
		private Panel panel3;
		private RadioButton rBtnValidHeaderMaskOff;
		private RadioButton rBtnValidHeaderMaskOn;
		private Label label3;
		private Panel panel2;
		private RadioButton rBtnPayloadCrcErrorMaskOff;
		private RadioButton rBtnPayloadCrcErrorMaskOn;
		private Label label2;
		private Panel panel1;
		private RadioButton rBtnRxDoneMaskOff;
		private RadioButton rBtnRxDoneMaskOn;
		private Label label1;
		private Panel panel4;
		private RadioButton rBtnRxTimeoutMaskOff;
		private RadioButton rBtnRxTimeoutMaskOn;
		private Label label10;
		private Panel panel11;
		private RadioButton rBtnImplicitHeaderOff;
		private RadioButton rBtnImplicitHeaderOn;
		private Label label8;
		private Label label11;
		private Label label9;
		private NumericUpDownEx nudSymbTimeout;
		private Panel panel12;
		private RadioButton rBtnPayloadCrcOff;
		private RadioButton rBtnPayloadCrcOn;
		private Label label12;
		private ComboBox cBoxCodingRate;
		private Label label15;
		private Label label16;
		private Label label17;
		private NumericUpDownEx nudPayloadLength;
		private Label label19;
		private NumericUpDownEx nudPreambleLength;
		private Label label29;
		private ComboBox cBoxBandwidth;
		private Label label32;
		private Label label31;
		private ComboBox cBoxSpreadingFactor;
		private Label label33;
		private Label label35;
		private NumericUpDownEx nudFreqHoppingPeriod;
		private Label label34;
		private Led ledRxPayloadCrcOn;
		private Label label39;
		private Label lblRxPayloadCodingRate;
		private Label label37;
		private Label lblRxNbBytes;
		private Label label38;
		private Label lblRxValidHeaderCnt;
		private Label label18;
		private Label lblRxPacketCnt;
		private Label label40;
		private GroupBoxEx groupBoxEx1;
		private Label label42;
		private Led ledSignalDetected;
		private Label label45;
		private Led ledSignalSynchronized;
		private Label label43;
		private Led ledRxOnGoing;
		private Label label41;
		private Led ledHeaderInfoValid;
		private Label label44;
		private Led ledModemClear;
		private Label lblPacketSnr;
		private Label label46;
		private Label lblPacketRssi;
		private Label label47;
		private Label lblRssiValue;
		private Label label48;
		private Label lblHopChannel;
		private Label label49;
		private Label lblFifoRxCurrentAddr;
		private Label label50;
		private GroupBoxEx gBoxControl;
		private TextBox tBoxPacketsNb;
		private Button btnLog;
		private CheckBox cBtnPacketHandlerStartStop;
		private Label lblPacketsNb;
		private TextBox tBoxPacketsRepeatValue;
		private Label lblPacketsRepeatValue;
		private GroupBoxEx gBoxMessage;
		private TableLayoutPanel tblPayloadMessage;
		private HexBox hexBoxPayload;
		private Label label51;
		private Label label52;
		private Label label53;
		private TableLayoutPanel pnlPacketStatus;
		private TableLayoutPanel pnlHeaderInfo;
		private Panel pnlPacketStatusHeaderName;
		private Label lblPacketStatusHeaderName;
		private Panel pnlRxHeaderInfoHeader;
		private Label lblRxHeaderInfoHeaderName;
		private Label lblPllTimeout;
		private Led ledPllTimeout;
		private GroupBox gBoxSettings;
		private Panel pnlPacketMode;
		private RadioButton rBtnPacketModeRx;
		private RadioButton rBtnPacketModeTx;
		private Led ledLogEnabled;
		private Label label13;
		private Panel panel13;
		private RadioButton rBtnLowDatarateOptimizeOff;
		private RadioButton rBtnLowDatarateOptimizeOn;

		public bool IsDebugOn
		{
			get
			{
				return this.isDebugOn;
			}
			set
			{
				this.isDebugOn = value;
			}
		}

		public Decimal FrequencyXo
		{
			get
			{
				return this.frequencyXo;
			}
			set
			{
				this.frequencyXo = value;
			}
		}

		public Decimal SymbolTime
		{
			get
			{
				return this.symbolTime;
			}
			set
			{
				this.symbolTime = value;
				this.nudSymbTimeout.Increment = this.symbolTime;
				this.nudSymbTimeout.Maximum = this.symbolTime * new Decimal(1023);
			}
		}

		public OperatingModeEnum Mode
		{
			get
			{
				return this.mode;
			}
			set
			{
				this.mode = value;
				this.UpdateControls();
			}
		}

		public bool RxTimeoutMask
		{
			get
			{
				return this.rBtnRxTimeoutMaskOn.Checked;
			}
			set
			{
				this.rBtnRxTimeoutMaskOn.CheckedChanged -= new EventHandler(this.rBtnRxTimeoutMask_CheckedChanged);
				this.rBtnRxTimeoutMaskOff.CheckedChanged -= new EventHandler(this.rBtnRxTimeoutMask_CheckedChanged);
				if (value)
				{
					this.rBtnRxTimeoutMaskOn.Checked = true;
					this.rBtnRxTimeoutMaskOff.Checked = false;
				}
				else
				{
					this.rBtnRxTimeoutMaskOn.Checked = false;
					this.rBtnRxTimeoutMaskOff.Checked = true;
				}
				this.rBtnRxTimeoutMaskOn.CheckedChanged += new EventHandler(this.rBtnRxTimeoutMask_CheckedChanged);
				this.rBtnRxTimeoutMaskOff.CheckedChanged += new EventHandler(this.rBtnRxTimeoutMask_CheckedChanged);
			}
		}

		public bool RxDoneMask
		{
			get
			{
				return this.rBtnRxDoneMaskOn.Checked;
			}
			set
			{
				this.rBtnRxDoneMaskOn.CheckedChanged -= new EventHandler(this.rBtnRxDoneMask_CheckedChanged);
				this.rBtnRxDoneMaskOff.CheckedChanged -= new EventHandler(this.rBtnRxDoneMask_CheckedChanged);
				if (value)
				{
					this.rBtnRxDoneMaskOn.Checked = true;
					this.rBtnRxDoneMaskOff.Checked = false;
				}
				else
				{
					this.rBtnRxDoneMaskOn.Checked = false;
					this.rBtnRxDoneMaskOff.Checked = true;
				}
				this.rBtnRxDoneMaskOn.CheckedChanged += new EventHandler(this.rBtnRxDoneMask_CheckedChanged);
				this.rBtnRxDoneMaskOff.CheckedChanged += new EventHandler(this.rBtnRxDoneMask_CheckedChanged);
			}
		}

		public bool PayloadCrcErrorMask
		{
			get
			{
				return this.rBtnPayloadCrcErrorMaskOn.Checked;
			}
			set
			{
				this.rBtnPayloadCrcErrorMaskOn.CheckedChanged -= new EventHandler(this.rBtnPayloadCrcErrorMask_CheckedChanged);
				this.rBtnPayloadCrcErrorMaskOff.CheckedChanged -= new EventHandler(this.rBtnPayloadCrcErrorMask_CheckedChanged);
				if (value)
				{
					this.rBtnPayloadCrcErrorMaskOn.Checked = true;
					this.rBtnPayloadCrcErrorMaskOff.Checked = false;
				}
				else
				{
					this.rBtnPayloadCrcErrorMaskOn.Checked = false;
					this.rBtnPayloadCrcErrorMaskOff.Checked = true;
				}
				this.rBtnPayloadCrcErrorMaskOn.CheckedChanged += new EventHandler(this.rBtnPayloadCrcErrorMask_CheckedChanged);
				this.rBtnPayloadCrcErrorMaskOff.CheckedChanged += new EventHandler(this.rBtnPayloadCrcErrorMask_CheckedChanged);
			}
		}

		public bool ValidHeaderMask
		{
			get
			{
				return this.rBtnValidHeaderMaskOn.Checked;
			}
			set
			{
				this.rBtnValidHeaderMaskOn.CheckedChanged -= new EventHandler(this.rBtnValidHeaderMask_CheckedChanged);
				this.rBtnValidHeaderMaskOff.CheckedChanged -= new EventHandler(this.rBtnValidHeaderMask_CheckedChanged);
				if (value)
				{
					this.rBtnValidHeaderMaskOn.Checked = true;
					this.rBtnValidHeaderMaskOff.Checked = false;
				}
				else
				{
					this.rBtnValidHeaderMaskOn.Checked = false;
					this.rBtnValidHeaderMaskOff.Checked = true;
				}
				this.rBtnValidHeaderMaskOn.CheckedChanged += new EventHandler(this.rBtnValidHeaderMask_CheckedChanged);
				this.rBtnValidHeaderMaskOff.CheckedChanged += new EventHandler(this.rBtnValidHeaderMask_CheckedChanged);
			}
		}

		public bool TxDoneMask
		{
			get
			{
				return this.rBtnTxDoneMaskOn.Checked;
			}
			set
			{
				this.rBtnTxDoneMaskOn.CheckedChanged -= new EventHandler(this.rBtnTxDoneMask_CheckedChanged);
				this.rBtnTxDoneMaskOff.CheckedChanged -= new EventHandler(this.rBtnTxDoneMask_CheckedChanged);
				if (value)
				{
					this.rBtnTxDoneMaskOn.Checked = true;
					this.rBtnTxDoneMaskOff.Checked = false;
				}
				else
				{
					this.rBtnTxDoneMaskOn.Checked = false;
					this.rBtnTxDoneMaskOff.Checked = true;
				}
				this.rBtnTxDoneMaskOn.CheckedChanged += new EventHandler(this.rBtnTxDoneMask_CheckedChanged);
				this.rBtnTxDoneMaskOff.CheckedChanged += new EventHandler(this.rBtnTxDoneMask_CheckedChanged);
			}
		}

		public bool CadDoneMask
		{
			get
			{
				return this.rBtnCadDoneMaskOn.Checked;
			}
			set
			{
				this.rBtnCadDoneMaskOn.CheckedChanged -= new EventHandler(this.rBtnCadDoneMask_CheckedChanged);
				this.rBtnCadDoneMaskOff.CheckedChanged -= new EventHandler(this.rBtnCadDoneMask_CheckedChanged);
				if (value)
				{
					this.rBtnCadDoneMaskOn.Checked = true;
					this.rBtnCadDoneMaskOff.Checked = false;
				}
				else
				{
					this.rBtnCadDoneMaskOn.Checked = false;
					this.rBtnCadDoneMaskOff.Checked = true;
				}
				this.rBtnCadDoneMaskOn.CheckedChanged += new EventHandler(this.rBtnCadDoneMask_CheckedChanged);
				this.rBtnCadDoneMaskOff.CheckedChanged += new EventHandler(this.rBtnCadDoneMask_CheckedChanged);
			}
		}

		public bool FhssChangeChannelMask
		{
			get
			{
				return this.rBtnFhssChangeChannelMaskOn.Checked;
			}
			set
			{
				this.rBtnFhssChangeChannelMaskOn.CheckedChanged -= new EventHandler(this.rBtnFhssChangeChannelMask_CheckedChanged);
				this.rBtnFhssChangeChannelMaskOff.CheckedChanged -= new EventHandler(this.rBtnFhssChangeChannelMask_CheckedChanged);
				if (value)
				{
					this.rBtnFhssChangeChannelMaskOn.Checked = true;
					this.rBtnFhssChangeChannelMaskOff.Checked = false;
				}
				else
				{
					this.rBtnFhssChangeChannelMaskOn.Checked = false;
					this.rBtnFhssChangeChannelMaskOff.Checked = true;
				}
				this.rBtnFhssChangeChannelMaskOn.CheckedChanged += new EventHandler(this.rBtnFhssChangeChannelMask_CheckedChanged);
				this.rBtnFhssChangeChannelMaskOff.CheckedChanged += new EventHandler(this.rBtnFhssChangeChannelMask_CheckedChanged);
			}
		}

		public bool CadDetectedMask
		{
			get
			{
				return this.rBtnCadDetectedMaskOn.Checked;
			}
			set
			{
				this.rBtnCadDetectedMaskOn.CheckedChanged -= new EventHandler(this.rBtnCadDetectedMask_CheckedChanged);
				this.rBtnCadDetectedMaskOff.CheckedChanged -= new EventHandler(this.rBtnCadDetectedMask_CheckedChanged);
				if (value)
				{
					this.rBtnCadDetectedMaskOn.Checked = true;
					this.rBtnCadDetectedMaskOff.Checked = false;
				}
				else
				{
					this.rBtnCadDetectedMaskOn.Checked = false;
					this.rBtnCadDetectedMaskOff.Checked = true;
				}
				this.rBtnCadDetectedMaskOn.CheckedChanged += new EventHandler(this.rBtnCadDetectedMask_CheckedChanged);
				this.rBtnCadDetectedMaskOff.CheckedChanged += new EventHandler(this.rBtnCadDetectedMask_CheckedChanged);
			}
		}

		public bool ImplicitHeaderModeOn
		{
			get
			{
				return this.rBtnImplicitHeaderOn.Checked;
			}
			set
			{
				this.rBtnImplicitHeaderOn.CheckedChanged -= new EventHandler(this.rBtnImplicitHeader_CheckedChanged);
				this.rBtnImplicitHeaderOff.CheckedChanged -= new EventHandler(this.rBtnImplicitHeader_CheckedChanged);
				if (value)
				{
					this.rBtnImplicitHeaderOn.Checked = true;
					this.rBtnImplicitHeaderOff.Checked = false;
					this.lblRxHeaderInfoHeaderName.Visible = false;
					this.pnlRxHeaderInfoHeader.Visible = false;
					this.pnlHeaderInfo.Visible = false;
				}
				else
				{
					this.rBtnImplicitHeaderOn.Checked = false;
					this.rBtnImplicitHeaderOff.Checked = true;
					this.lblRxHeaderInfoHeaderName.Visible = true;
					this.pnlRxHeaderInfoHeader.Visible = true;
					this.pnlHeaderInfo.Visible = true;
				}
				this.rBtnImplicitHeaderOn.CheckedChanged += new EventHandler(this.rBtnImplicitHeader_CheckedChanged);
				this.rBtnImplicitHeaderOff.CheckedChanged += new EventHandler(this.rBtnImplicitHeader_CheckedChanged);
			}
		}

		public Decimal SymbTimeout
		{
			get
			{
				return this.nudSymbTimeout.Value;
			}
			set
			{
				try
				{
					this.nudSymbTimeout.ValueChanged -= new EventHandler(this.nudSymbTimeout_ValueChanged);
					this.nudSymbTimeout.BackColor = SystemColors.Window;
					this.nudSymbTimeout.Value = (Decimal)(uint)Math.Round(value / this.SymbolTime, MidpointRounding.AwayFromZero) * this.SymbolTime;
				}
				catch (Exception)
				{
					this.nudSymbTimeout.BackColor = ControlPaint.LightLight(Color.Red);
				}
				finally
				{
					this.nudSymbTimeout.ValueChanged += new EventHandler(this.nudSymbTimeout_ValueChanged);
				}
			}
		}

		public bool PayloadCrcOn
		{
			get
			{
				return this.rBtnPayloadCrcOn.Checked;
			}
			set
			{
				this.rBtnPayloadCrcOn.CheckedChanged -= new EventHandler(this.rBtnPayloadCrc_CheckedChanged);
				this.rBtnPayloadCrcOff.CheckedChanged -= new EventHandler(this.rBtnPayloadCrc_CheckedChanged);
				if (value)
				{
					this.rBtnPayloadCrcOn.Checked = true;
					this.rBtnPayloadCrcOff.Checked = false;
				}
				else
				{
					this.rBtnPayloadCrcOn.Checked = false;
					this.rBtnPayloadCrcOff.Checked = true;
				}
				this.rBtnPayloadCrcOn.CheckedChanged += new EventHandler(this.rBtnPayloadCrc_CheckedChanged);
				this.rBtnPayloadCrcOff.CheckedChanged += new EventHandler(this.rBtnPayloadCrc_CheckedChanged);
			}
		}

		public byte CodingRate
		{
			get
			{
				return (byte)(this.cBoxCodingRate.SelectedIndex + 1);
			}
			set
			{
				this.cBoxCodingRate.SelectedIndexChanged -= new EventHandler(this.cBoxCodingRate_SelectedIndexChanged);
				this.cBoxCodingRate.SelectedIndex = (int)value - 1;
				this.cBoxCodingRate.SelectedIndexChanged += new EventHandler(this.cBoxCodingRate_SelectedIndexChanged);
			}
		}

		public byte PayloadLength
		{
			get
			{
				return (byte)this.nudPayloadLength.Value;
			}
			set
			{
				try
				{
					this.nudPayloadLength.ValueChanged -= new EventHandler(this.nudPayloadLength_ValueChanged);
					this.nudPayloadLength.BackColor = SystemColors.Window;
					this.nudPayloadLength.Value = (Decimal)value;
				}
				catch (Exception)
				{
					this.nudPayloadLength.BackColor = ControlPaint.LightLight(Color.Red);
				}
				finally
				{
					this.nudPayloadLength.ValueChanged += new EventHandler(this.nudPayloadLength_ValueChanged);
				}
			}
		}

		public int PreambleLength
		{
			get
			{
				return (int)this.nudPreambleLength.Value;
			}
			set
			{
				try
				{
					this.nudPreambleLength.ValueChanged -= new EventHandler(this.nudPreambleLength_ValueChanged);
					this.nudPreambleLength.BackColor = SystemColors.Window;
					this.nudPreambleLength.Value = (Decimal)value;
				}
				catch (Exception)
				{
					this.nudPreambleLength.BackColor = ControlPaint.LightLight(Color.Red);
				}
				finally
				{
					this.nudPreambleLength.ValueChanged += new EventHandler(this.nudPreambleLength_ValueChanged);
				}
			}
		}

		public byte Bandwidth
		{
			get
			{
				return (byte)this.cBoxBandwidth.SelectedIndex;
			}
			set
			{
				this.cBoxBandwidth.SelectedIndexChanged -= new EventHandler(this.cBoxBandwidth_SelectedIndexChanged);
				this.cBoxBandwidth.SelectedIndex = (int)value;
				this.cBoxBandwidth.SelectedIndexChanged += new EventHandler(this.cBoxBandwidth_SelectedIndexChanged);
			}
		}

		public byte SpreadingFactor
		{
			get
			{
				return (byte)(this.cBoxSpreadingFactor.SelectedIndex + 7);
			}
			set
			{
				try
				{
					this.cBoxSpreadingFactor.SelectedIndexChanged -= new EventHandler(this.cBoxSpreadingFactor_SelectedIndexChanged);
					this.cBoxSpreadingFactor.SelectedIndex = (int)value - 7;
				}
				catch (Exception)
				{
				}
				finally
				{
					this.cBoxSpreadingFactor.SelectedIndexChanged += new EventHandler(this.cBoxSpreadingFactor_SelectedIndexChanged);
				}
			}
		}

		public byte FreqHoppingPeriod
		{
			get
			{
				return (byte)this.nudFreqHoppingPeriod.Value;
			}
			set
			{
				try
				{
					this.nudFreqHoppingPeriod.ValueChanged -= new EventHandler(this.nudFreqHoppingPeriod_ValueChanged);
					this.nudFreqHoppingPeriod.BackColor = SystemColors.Window;
					this.nudFreqHoppingPeriod.Value = (Decimal)value;
				}
				catch (Exception)
				{
					this.nudFreqHoppingPeriod.BackColor = ControlPaint.LightLight(Color.Red);
				}
				finally
				{
					this.nudFreqHoppingPeriod.ValueChanged += new EventHandler(this.nudFreqHoppingPeriod_ValueChanged);
				}
			}
		}

		public byte RxNbBytes
		{
			set
			{
				this.lblRxNbBytes.Text = value.ToString();
			}
		}

		public bool PllTimeout
		{
			set
			{
				this.ledPllTimeout.Checked = value;
			}
		}

		public bool RxPayloadCrcOn
		{
			set
			{
				this.ledRxPayloadCrcOn.Checked = value;
			}
		}

		public byte RxPayloadCodingRate
		{
			set
			{
				string str = "-";
				switch (value)
				{
					case (byte)1:
						str = "4/5";
						break;
					case (byte)2:
						str = "4/6";
						break;
					case (byte)3:
						str = "4/7";
						break;
					case (byte)4:
						str = "4/8";
						break;
				}
				this.lblRxPayloadCodingRate.Text = str;
			}
		}

		public ushort ValidHeaderCnt
		{
			set
			{
				this.lblRxValidHeaderCnt.Text = value.ToString();
			}
		}

		public ushort ValidPacketCnt
		{
			set
			{
				this.lblRxPacketCnt.Text = value.ToString();
			}
		}

		public bool ModemClear
		{
			set
			{
				this.ledModemClear.Checked = value;
			}
		}

		public bool HeaderInfoValid
		{
			set
			{
				this.ledHeaderInfoValid.Checked = value;
			}
		}

		public bool RxOnGoing
		{
			set
			{
				this.ledRxOnGoing.Checked = value;
			}
		}

		public bool SignalSynchronized
		{
			set
			{
				this.ledSignalDetected.Checked = value;
			}
		}

		public bool SignalDetected
		{
			set
			{
				this.ledSignalDetected.Checked = value;
			}
		}

		public sbyte PktSnrValue
		{
			set
			{
				if ((int)value > 0 && !this.isDebugOn)
					this.lblPacketSnr.Text = "> 0";
				else
					this.lblPacketSnr.Text = value.ToString();
			}
		}

		public Decimal RssiValue
		{
			set
			{
				this.lblRssiValue.Text = value.ToString("###.0");
			}
		}

		public Decimal PktRssiValue
		{
			set
			{
				this.lblPacketRssi.Text = value.ToString("###.0");
			}
		}

		public byte HopChannel
		{
			set
			{
				this.lblHopChannel.Text = value.ToString();
			}
		}

		public byte FifoRxCurrentAddr
		{
			set
			{
				this.lblFifoRxCurrentAddr.Text = value.ToString();
			}
		}

		public byte[] Payload
		{
			get
			{
				return this.payload;
			}
			set
			{
				this.payload = value;
				DynamicByteProvider dynamicByteProvider = this.hexBoxPayload.ByteProvider as DynamicByteProvider;
				dynamicByteProvider.Bytes.Clear();
				dynamicByteProvider.Bytes.AddRange(value);
				this.hexBoxPayload.ByteProvider.ApplyChanges();
				this.hexBoxPayload.UpdateScrollSize();
				this.hexBoxPayload.Invalidate();
			}
		}

		public bool LogEnabled
		{
			get
			{
				return this.ledLogEnabled.Checked;
			}
			set
			{
				this.ledLogEnabled.Checked = value;
			}
		}

		public bool StartStop
		{
			get
			{
				return this.cBtnPacketHandlerStartStop.Checked;
			}
			set
			{
				this.cBtnPacketHandlerStartStop.Checked = value;
			}
		}

		public int PacketNumber
		{
			get
			{
				return Convert.ToInt32(this.tBoxPacketsNb.Text);
			}
			set
			{
				this.tBoxPacketsNb.Text = value.ToString();
			}
		}

		public int MaxPacketNumber
		{
			get
			{
				return Convert.ToInt32(this.tBoxPacketsRepeatValue.Text);
			}
			set
			{
				this.tBoxPacketsRepeatValue.Text = value.ToString();
			}
		}

		public bool PacketModeTx
		{
			get
			{
				return this.rBtnPacketModeTx.Checked;
			}
			set
			{
				this.rBtnPacketModeTx.CheckedChanged -= new EventHandler(this.rBtnPacketMode_CheckedChanged);
				this.rBtnPacketModeRx.CheckedChanged -= new EventHandler(this.rBtnPacketMode_CheckedChanged);
				if (value)
				{
					this.rBtnPacketModeTx.Checked = true;
					this.rBtnPacketModeRx.Checked = false;
				}
				else
				{
					this.rBtnPacketModeTx.Checked = false;
					this.rBtnPacketModeRx.Checked = true;
				}
				this.UpdateControls();
				this.rBtnPacketModeTx.CheckedChanged += new EventHandler(this.rBtnPacketMode_CheckedChanged);
				this.rBtnPacketModeRx.CheckedChanged += new EventHandler(this.rBtnPacketMode_CheckedChanged);
			}
		}

		public bool LowDatarateOptimize
		{
			get
			{
				return this.rBtnLowDatarateOptimizeOn.Checked;
			}
			set
			{
				this.rBtnLowDatarateOptimizeOn.CheckedChanged -= new EventHandler(this.rBtnLowDatarateOptimize_CheckedChanged);
				this.rBtnLowDatarateOptimizeOff.CheckedChanged -= new EventHandler(this.rBtnLowDatarateOptimize_CheckedChanged);
				if (value)
				{
					this.rBtnLowDatarateOptimizeOn.Checked = true;
					this.rBtnLowDatarateOptimizeOff.Checked = false;
				}
				else
				{
					this.rBtnLowDatarateOptimizeOn.Checked = false;
					this.rBtnLowDatarateOptimizeOff.Checked = true;
				}
				this.rBtnLowDatarateOptimizeOn.CheckedChanged += new EventHandler(this.rBtnLowDatarateOptimize_CheckedChanged);
				this.rBtnLowDatarateOptimizeOff.CheckedChanged += new EventHandler(this.rBtnLowDatarateOptimize_CheckedChanged);
			}
		}

		public event ErrorEventHandler Error;

		public event BooleanEventHandler RxTimeoutMaskChanged;

		public event BooleanEventHandler RxDoneMaskChanged;

		public event BooleanEventHandler PayloadCrcErrorMaskChanged;

		public event BooleanEventHandler ValidHeaderMaskChanged;

		public event BooleanEventHandler TxDoneMaskChanged;

		public event BooleanEventHandler CadDoneMaskChanged;

		public event BooleanEventHandler FhssChangeChannelMaskChanged;

		public event BooleanEventHandler CadDetectedMaskChanged;

		public event BooleanEventHandler ImplicitHeaderModeOnChanged;

		public event DecimalEventHandler SymbTimeoutChanged;

		public event BooleanEventHandler PayloadCrcOnChanged;

		public event ByteEventHandler CodingRateChanged;

		public event ByteEventHandler PayloadLengthChanged;

		public event Int32EventHandler PreambleLengthChanged;

		public event ByteEventHandler BandwidthChanged;

		public event ByteEventHandler SpreadingFactorChanged;

		public event ByteEventHandler FreqHoppingPeriodChanged;

		public event ByteArrayEventHandler MessageChanged;

		public event BooleanEventHandler StartStopChanged;

		public event Int32EventHandler MaxPacketNumberChanged;

		public event EventHandler PacketHandlerLog;

		public event BooleanEventHandler PacketModeTxChanged;

		public event BooleanEventHandler LowDatarateOptimizeChanged;

		public event DocumentationChangedEventHandler DocumentationChanged;

		public LoRaViewControl()
		{
			this.InitializeComponent();
			this.payload = new byte[0];
			this.hexBoxPayload.ByteProvider = (IByteProvider)new DynamicByteProvider(new byte[this.Payload.Length]);
			this.hexBoxPayload.ByteProvider.Changed += new EventHandler(this.hexBoxPayload_DataChanged);
			this.hexBoxPayload.ByteProvider.ApplyChanges();
		}

		private void UpdateControls()
		{
			if (this.mode == OperatingModeEnum.Sleep || this.mode == OperatingModeEnum.Stdby)
			{
				this.cBtnPacketHandlerStartStop.Enabled = true;
				this.tBoxPacketsNb.Enabled = true;
				if (!this.cBtnPacketHandlerStartStop.Checked)
				{
					this.tBoxPacketsRepeatValue.Enabled = true;
					this.pnlPacketMode.Enabled = true;
				}
			}
			this.gBoxControl.Enabled = false;
			switch (this.Mode)
			{
				case OperatingModeEnum.Sleep:
				case OperatingModeEnum.Stdby:
					this.gBoxControl.Enabled = true;
					if (this.rBtnPacketModeRx.Checked)
					{
						this.nudPayloadLength.Enabled = true;
						this.lblPacketsNb.Text = "Rx packets:";
					}
					else
					{
						this.nudPayloadLength.Enabled = false;
						this.lblPacketsNb.Text = "Tx Packets:";
					}
					this.lblPacketsNb.Visible = true;
					this.tBoxPacketsNb.Visible = true;
					if (this.rBtnPacketModeRx.Checked)
					{
						this.lblPacketsRepeatValue.Visible = false;
						this.tBoxPacketsRepeatValue.Visible = false;
						break;
					}
					this.lblPacketsRepeatValue.Visible = true;
					this.tBoxPacketsRepeatValue.Visible = true;
					break;
				case OperatingModeEnum.FsTx:
					this.nudPayloadLength.Enabled = false;
					this.lblPacketsNb.Visible = false;
					this.tBoxPacketsNb.Visible = false;
					this.lblPacketsRepeatValue.Visible = false;
					this.tBoxPacketsRepeatValue.Visible = false;
					break;
				case OperatingModeEnum.Tx:
				case OperatingModeEnum.TxContinuous:
					this.nudPayloadLength.Enabled = false;
					this.lblPacketsNb.Visible = false;
					this.tBoxPacketsNb.Visible = false;
					this.lblPacketsRepeatValue.Visible = false;
					this.tBoxPacketsRepeatValue.Visible = false;
					break;
				case OperatingModeEnum.FsRx:
					this.nudPayloadLength.Enabled = true;
					this.lblPacketsNb.Visible = false;
					this.tBoxPacketsNb.Visible = false;
					this.lblPacketsRepeatValue.Visible = false;
					this.tBoxPacketsRepeatValue.Visible = false;
					break;
				case OperatingModeEnum.Rx:
				case OperatingModeEnum.RxSingle:
				case OperatingModeEnum.Cad:
					this.nudPayloadLength.Enabled = true;
					this.lblPacketsNb.Visible = false;
					this.tBoxPacketsNb.Visible = false;
					this.lblPacketsRepeatValue.Visible = false;
					this.tBoxPacketsRepeatValue.Visible = false;
					break;
			}
		}

		private void OnError(byte status, string message)
		{
			if (this.Error == null)
				return;
			this.Error((object)this, new ErrorEventArgs(status, message));
		}

		private void OnRxTimeoutMaskChanged(bool value)
		{
			if (this.RxTimeoutMaskChanged == null)
				return;
			this.RxTimeoutMaskChanged((object)this, new BooleanEventArg(value));
		}

		private void OnRxDoneMaskChanged(bool value)
		{
			if (this.RxDoneMaskChanged == null)
				return;
			this.RxDoneMaskChanged((object)this, new BooleanEventArg(value));
		}

		private void OnPayloadCrcErrorMaskChanged(bool value)
		{
			if (this.PayloadCrcErrorMaskChanged == null)
				return;
			this.PayloadCrcErrorMaskChanged((object)this, new BooleanEventArg(value));
		}

		private void OnValidHeaderMaskChanged(bool value)
		{
			if (this.ValidHeaderMaskChanged == null)
				return;
			this.ValidHeaderMaskChanged((object)this, new BooleanEventArg(value));
		}

		private void OnTxDoneMaskChanged(bool value)
		{
			if (this.TxDoneMaskChanged == null)
				return;
			this.TxDoneMaskChanged((object)this, new BooleanEventArg(value));
		}

		private void OnCadDoneMaskChanged(bool value)
		{
			if (this.CadDoneMaskChanged == null)
				return;
			this.CadDoneMaskChanged((object)this, new BooleanEventArg(value));
		}

		private void OnFhssChangeChannelMaskChanged(bool value)
		{
			if (this.FhssChangeChannelMaskChanged == null)
				return;
			this.FhssChangeChannelMaskChanged((object)this, new BooleanEventArg(value));
		}

		private void OnCadDetectedMaskChanged(bool value)
		{
			if (this.CadDetectedMaskChanged == null)
				return;
			this.CadDetectedMaskChanged((object)this, new BooleanEventArg(value));
		}

		private void OnImplicitHeaderChanged(bool value)
		{
			if (this.ImplicitHeaderModeOnChanged == null)
				return;
			this.ImplicitHeaderModeOnChanged((object)this, new BooleanEventArg(value));
		}

		private void OnSymbTimeoutChanged(Decimal value)
		{
			if (this.SymbTimeoutChanged == null)
				return;
			this.SymbTimeoutChanged((object)this, new DecimalEventArg(value));
		}

		private void OnPayloadCrcOnChanged(bool value)
		{
			if (this.PayloadCrcOnChanged == null)
				return;
			this.PayloadCrcOnChanged((object)this, new BooleanEventArg(value));
		}

		private void OnCodingRateChanged(byte value)
		{
			if (this.CodingRateChanged == null)
				return;
			this.CodingRateChanged((object)this, new ByteEventArg(value));
		}

		private void OnPayloadLengthChanged(byte value)
		{
			if (this.PayloadLengthChanged == null)
				return;
			this.PayloadLengthChanged((object)this, new ByteEventArg(value));
		}

		private void OnPreambleLengthChanged(int value)
		{
			if (this.PreambleLengthChanged == null)
				return;
			this.PreambleLengthChanged((object)this, new Int32EventArg(value));
		}

		private void OnBandwidthChanged(byte value)
		{
			if (this.BandwidthChanged == null)
				return;
			this.BandwidthChanged((object)this, new ByteEventArg(value));
		}

		private void OnSpreadingFactorChanged(byte value)
		{
			if (this.SpreadingFactorChanged == null)
				return;
			this.SpreadingFactorChanged((object)this, new ByteEventArg(value));
		}

		private void OnFreqHoppingPeriodChanged(byte value)
		{
			if (this.FreqHoppingPeriodChanged == null)
				return;
			this.FreqHoppingPeriodChanged((object)this, new ByteEventArg(value));
		}

		private void OnMessageChanged(byte[] value)
		{
			if (this.MessageChanged == null)
				return;
			this.MessageChanged((object)this, new ByteArrayEventArg(value));
		}

		private void OnStartStopChanged(bool value)
		{
			if (this.StartStopChanged == null)
				return;
			this.StartStopChanged((object)this, new BooleanEventArg(value));
		}

		private void OnMaxPacketNumberChanged(int value)
		{
			if (this.MaxPacketNumberChanged == null)
				return;
			this.MaxPacketNumberChanged((object)this, new Int32EventArg(value));
		}

		private void OnPacketHandlerLog()
		{
			if (this.PacketHandlerLog == null)
				return;
			this.PacketHandlerLog((object)this, EventArgs.Empty);
		}

		private void OnPacketModeTxChanged(bool value)
		{
			if (this.PacketModeTxChanged == null)
				return;
			this.PacketModeTxChanged((object)this, new BooleanEventArg(value));
		}

		private void OnLowDatarateOptimizeChanged(bool value)
		{
			if (this.LowDatarateOptimizeChanged == null)
				return;
			this.LowDatarateOptimizeChanged((object)this, new BooleanEventArg(value));
		}

		public void UpdateBandwidthLimits(LimitCheckStatusEnum status, string message)
		{
			this.errorProvider.SetError((Control)this.cBoxBandwidth, message);
		}

		private void rBtnRxTimeoutMask_CheckedChanged(object sender, EventArgs e)
		{
			this.RxTimeoutMask = this.rBtnRxTimeoutMaskOn.Checked;
			this.OnRxTimeoutMaskChanged(this.RxTimeoutMask);
		}

		private void rBtnRxDoneMask_CheckedChanged(object sender, EventArgs e)
		{
			this.RxDoneMask = this.rBtnRxDoneMaskOn.Checked;
			this.OnRxDoneMaskChanged(this.RxDoneMask);
		}

		private void rBtnPayloadCrcErrorMask_CheckedChanged(object sender, EventArgs e)
		{
			this.PayloadCrcErrorMask = this.rBtnPayloadCrcErrorMaskOn.Checked;
			this.OnPayloadCrcErrorMaskChanged(this.PayloadCrcErrorMask);
		}

		private void rBtnValidHeaderMask_CheckedChanged(object sender, EventArgs e)
		{
			this.ValidHeaderMask = this.rBtnValidHeaderMaskOn.Checked;
			this.OnValidHeaderMaskChanged(this.ValidHeaderMask);
		}

		private void rBtnTxDoneMask_CheckedChanged(object sender, EventArgs e)
		{
			this.TxDoneMask = this.rBtnTxDoneMaskOn.Checked;
			this.OnTxDoneMaskChanged(this.TxDoneMask);
		}

		private void rBtnCadDoneMask_CheckedChanged(object sender, EventArgs e)
		{
			this.CadDoneMask = this.rBtnCadDoneMaskOn.Checked;
			this.OnCadDoneMaskChanged(this.CadDoneMask);
		}

		private void rBtnFhssChangeChannelMask_CheckedChanged(object sender, EventArgs e)
		{
			this.FhssChangeChannelMask = this.rBtnFhssChangeChannelMaskOn.Checked;
			this.OnFhssChangeChannelMaskChanged(this.FhssChangeChannelMask);
		}

		private void rBtnCadDetectedMask_CheckedChanged(object sender, EventArgs e)
		{
			this.CadDetectedMask = this.rBtnCadDetectedMaskOn.Checked;
			this.OnCadDetectedMaskChanged(this.CadDetectedMask);
		}

		private void rBtnImplicitHeader_CheckedChanged(object sender, EventArgs e)
		{
			this.ImplicitHeaderModeOn = this.rBtnImplicitHeaderOn.Checked;
			this.OnImplicitHeaderChanged(this.ImplicitHeaderModeOn);
		}

		private void nudSymbTimeout_ValueChanged(object sender, EventArgs e)
		{
			this.SymbTimeout = this.nudSymbTimeout.Value;
			this.OnSymbTimeoutChanged(this.SymbTimeout);
		}

		private void rBtnPayloadCrc_CheckedChanged(object sender, EventArgs e)
		{
			this.PayloadCrcOn = this.rBtnPayloadCrcOn.Checked;
			this.OnPayloadCrcOnChanged(this.PayloadCrcOn);
		}

		private void cBoxCodingRate_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.CodingRate = (byte)(this.cBoxCodingRate.SelectedIndex + 1);
			this.OnCodingRateChanged(this.CodingRate);
		}

		private void nudPayloadLength_ValueChanged(object sender, EventArgs e)
		{
			this.PayloadLength = (byte)this.nudPayloadLength.Value;
			this.OnPayloadLengthChanged(this.PayloadLength);
		}

		private void nudPreambleLength_ValueChanged(object sender, EventArgs e)
		{
			this.PreambleLength = (int)this.nudPreambleLength.Value;
			this.OnPreambleLengthChanged(this.PreambleLength);
		}

		private void cBoxBandwidth_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.Bandwidth = (byte)this.cBoxBandwidth.SelectedIndex;
			this.OnBandwidthChanged(this.Bandwidth);
		}

		private void cBoxSpreadingFactor_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.SpreadingFactor = (byte)(this.cBoxSpreadingFactor.SelectedIndex + 7);
			this.OnSpreadingFactorChanged(this.SpreadingFactor);
		}

		private void nudFreqHoppingPeriod_ValueChanged(object sender, EventArgs e)
		{
			this.FreqHoppingPeriod = (byte)this.nudFreqHoppingPeriod.Value;
			this.OnFreqHoppingPeriodChanged(this.FreqHoppingPeriod);
		}

		private void hexBoxPayload_DataChanged(object sender, EventArgs e)
		{
			if (this.inHexPayloadDataChanged)
				return;
			this.inHexPayloadDataChanged = true;
			if (this.hexBoxPayload.ByteProvider.Length > (long)byte.MaxValue)
			{
				this.hexBoxPayload.ByteProvider.DeleteBytes((long)byte.MaxValue, this.hexBoxPayload.ByteProvider.Length - (long)byte.MaxValue);
				this.hexBoxPayload.SelectionStart = (long)byte.MaxValue;
				this.hexBoxPayload.SelectionLength = 0L;
			}
			Array.Resize<byte>(ref this.payload, (int)this.hexBoxPayload.ByteProvider.Length);
			for (int index = 0; index < this.payload.Length; ++index)
				this.payload[index] = this.hexBoxPayload.ByteProvider.ReadByte((long)index);
			this.OnMessageChanged(this.Payload);
			this.inHexPayloadDataChanged = false;
		}

		private void cBtnPacketHandlerStartStop_CheckedChanged(object sender, EventArgs e)
		{
			if (this.cBtnPacketHandlerStartStop.Checked)
				this.cBtnPacketHandlerStartStop.Text = "Stop";
			else
				this.cBtnPacketHandlerStartStop.Text = "Start";
			this.gBoxSettings.Enabled = !this.cBtnPacketHandlerStartStop.Checked;
			this.gBoxIrqMask.Enabled = !this.cBtnPacketHandlerStartStop.Checked;
			this.hexBoxPayload.ReadOnly = this.cBtnPacketHandlerStartStop.Checked;
			this.tBoxPacketsRepeatValue.Enabled = !this.cBtnPacketHandlerStartStop.Checked;
			this.pnlPacketMode.Enabled = !this.cBtnPacketHandlerStartStop.Checked;
			this.btnLog.Enabled = !this.cBtnPacketHandlerStartStop.Checked;
			try
			{
				this.MaxPacketNumber = Convert.ToInt32(this.tBoxPacketsRepeatValue.Text);
			}
			catch
			{
				this.MaxPacketNumber = 0;
				this.tBoxPacketsRepeatValue.Text = "0";
				this.OnError((byte)1, "Wrong max packet value! Value has been reseted.");
			}
			this.OnMaxPacketNumberChanged(this.MaxPacketNumber);
			this.OnStartStopChanged(this.cBtnPacketHandlerStartStop.Checked);
		}

		private void btnLog_Click(object sender, EventArgs e)
		{
			this.OnPacketHandlerLog();
		}

		private void rBtnPacketMode_CheckedChanged(object sender, EventArgs e)
		{
			this.PacketModeTx = this.rBtnPacketModeTx.Checked;
			this.OnPacketModeTxChanged(this.PacketModeTx);
		}

		private void rBtnLowDatarateOptimize_CheckedChanged(object sender, EventArgs e)
		{
			this.LowDatarateOptimize = this.rBtnLowDatarateOptimizeOn.Checked;
			this.OnLowDatarateOptimizeChanged(this.LowDatarateOptimize);
		}

		private void control_MouseEnter(object sender, EventArgs e)
		{
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

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
				this.components.Dispose();
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
			this.nudPreambleLength = new NumericUpDownEx();
			this.cBoxBandwidth = new ComboBox();
			this.lblListenResolRx = new Label();
			this.label30 = new Label();
			this.panel11 = new Panel();
			this.rBtnImplicitHeaderOff = new RadioButton();
			this.rBtnImplicitHeaderOn = new RadioButton();
			this.label8 = new Label();
			this.label9 = new Label();
			this.label11 = new Label();
			this.label12 = new Label();
			this.panel12 = new Panel();
			this.rBtnPayloadCrcOff = new RadioButton();
			this.rBtnPayloadCrcOn = new RadioButton();
			this.cBoxCodingRate = new ComboBox();
			this.label15 = new Label();
			this.label16 = new Label();
			this.label17 = new Label();
			this.label19 = new Label();
			this.label29 = new Label();
			this.label31 = new Label();
			this.label32 = new Label();
			this.label33 = new Label();
			this.cBoxSpreadingFactor = new ComboBox();
			this.label34 = new Label();
			this.label35 = new Label();
			this.lblRxNbBytes = new Label();
			this.label38 = new Label();
			this.label18 = new Label();
			this.lblRxValidHeaderCnt = new Label();
			this.label40 = new Label();
			this.lblRxPacketCnt = new Label();
			this.label46 = new Label();
			this.lblPacketSnr = new Label();
			this.label47 = new Label();
			this.lblPacketRssi = new Label();
			this.label48 = new Label();
			this.lblRssiValue = new Label();
			this.label49 = new Label();
			this.lblHopChannel = new Label();
			this.label50 = new Label();
			this.lblFifoRxCurrentAddr = new Label();
			this.pnlPacketStatus = new TableLayoutPanel();
			this.lblRxPayloadCodingRate = new Label();
			this.label37 = new Label();
			this.label39 = new Label();
			this.pnlHeaderInfo = new TableLayoutPanel();
			this.lblPllTimeout = new Label();
			this.ledRxPayloadCrcOn = new Led();
			this.ledPllTimeout = new Led();
			this.pnlRxHeaderInfoHeader = new Panel();
			this.lblRxHeaderInfoHeaderName = new Label();
			this.pnlPacketStatusHeaderName = new Panel();
			this.lblPacketStatusHeaderName = new Label();
			this.gBoxSettings = new GroupBox();
			this.nudSymbTimeout = new NumericUpDownEx();
			this.label13 = new Label();
			this.panel13 = new Panel();
			this.rBtnLowDatarateOptimizeOff = new RadioButton();
			this.rBtnLowDatarateOptimizeOn = new RadioButton();
			this.nudFreqHoppingPeriod = new NumericUpDownEx();
			this.nudPayloadLength = new NumericUpDownEx();
			this.gBoxMessage = new GroupBoxEx();
			this.tblPayloadMessage = new TableLayoutPanel();
			this.hexBoxPayload = new HexBox();
			this.label51 = new Label();
			this.label52 = new Label();
			this.gBoxControl = new GroupBoxEx();
			this.ledLogEnabled = new Led();
			this.pnlPacketMode = new Panel();
			this.rBtnPacketModeRx = new RadioButton();
			this.rBtnPacketModeTx = new RadioButton();
			this.tBoxPacketsNb = new TextBox();
			this.btnLog = new Button();
			this.cBtnPacketHandlerStartStop = new CheckBox();
			this.lblPacketsNb = new Label();
			this.tBoxPacketsRepeatValue = new TextBox();
			this.lblPacketsRepeatValue = new Label();
			this.groupBoxEx1 = new GroupBoxEx();
			this.label53 = new Label();
			this.label42 = new Label();
			this.ledSignalDetected = new Led();
			this.label45 = new Label();
			this.ledSignalSynchronized = new Led();
			this.label43 = new Label();
			this.ledRxOnGoing = new Led();
			this.label41 = new Label();
			this.ledHeaderInfoValid = new Led();
			this.label44 = new Label();
			this.ledModemClear = new Led();
			this.gBoxIrqMask = new GroupBoxEx();
			this.panel10 = new Panel();
			this.rBtnCadDetectedMaskOff = new RadioButton();
			this.rBtnCadDetectedMaskOn = new RadioButton();
			this.label7 = new Label();
			this.panel9 = new Panel();
			this.rBtnFhssChangeChannelMaskOff = new RadioButton();
			this.rBtnFhssChangeChannelMaskOn = new RadioButton();
			this.label6 = new Label();
			this.panel8 = new Panel();
			this.rBtnCadDoneMaskOff = new RadioButton();
			this.rBtnCadDoneMaskOn = new RadioButton();
			this.label5 = new Label();
			this.panel5 = new Panel();
			this.rBtnTxDoneMaskOff = new RadioButton();
			this.rBtnTxDoneMaskOn = new RadioButton();
			this.label4 = new Label();
			this.panel3 = new Panel();
			this.rBtnValidHeaderMaskOff = new RadioButton();
			this.rBtnValidHeaderMaskOn = new RadioButton();
			this.label3 = new Label();
			this.panel2 = new Panel();
			this.rBtnPayloadCrcErrorMaskOff = new RadioButton();
			this.rBtnPayloadCrcErrorMaskOn = new RadioButton();
			this.label2 = new Label();
			this.panel1 = new Panel();
			this.rBtnRxDoneMaskOff = new RadioButton();
			this.rBtnRxDoneMaskOn = new RadioButton();
			this.label1 = new Label();
			this.panel4 = new Panel();
			this.rBtnRxTimeoutMaskOff = new RadioButton();
			this.rBtnRxTimeoutMaskOn = new RadioButton();
			this.label10 = new Label();
			((ISupportInitialize)this.errorProvider).BeginInit();
			this.nudPreambleLength.BeginInit();
			this.panel11.SuspendLayout();
			this.panel12.SuspendLayout();
			this.pnlPacketStatus.SuspendLayout();
			this.pnlHeaderInfo.SuspendLayout();
			this.gBoxSettings.SuspendLayout();
			this.nudSymbTimeout.BeginInit();
			this.panel13.SuspendLayout();
			this.nudFreqHoppingPeriod.BeginInit();
			this.nudPayloadLength.BeginInit();
			this.gBoxMessage.SuspendLayout();
			this.tblPayloadMessage.SuspendLayout();
			this.gBoxControl.SuspendLayout();
			this.pnlPacketMode.SuspendLayout();
			this.groupBoxEx1.SuspendLayout();
			this.gBoxIrqMask.SuspendLayout();
			this.panel10.SuspendLayout();
			this.panel9.SuspendLayout();
			this.panel8.SuspendLayout();
			this.panel5.SuspendLayout();
			this.panel3.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel1.SuspendLayout();
			this.panel4.SuspendLayout();
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
			this.errorProvider.SetIconPadding((Control)this.nudPreambleLength, 6);
			this.nudPreambleLength.Location = new Point(368, 23);
			NumericUpDownEx numericUpDownEx1 = this.nudPreambleLength;
			int[] bits1 = new int[4];
			bits1[0] = 65539;
			Decimal num1 = new Decimal(bits1);
			numericUpDownEx1.Maximum = num1;
			NumericUpDownEx numericUpDownEx2 = this.nudPreambleLength;
			int[] bits2 = new int[4];
			bits2[0] = 4;
			Decimal num2 = new Decimal(bits2);
			numericUpDownEx2.Minimum = num2;
			this.nudPreambleLength.Name = "nudPreambleLength";
			this.nudPreambleLength.Size = new Size(124, 20);
			this.nudPreambleLength.TabIndex = 14;
			NumericUpDownEx numericUpDownEx3 = this.nudPreambleLength;
			int[] bits3 = new int[4];
			bits3[0] = 12;
			Decimal num3 = new Decimal(bits3);
			numericUpDownEx3.Value = num3;
			this.nudPreambleLength.ValueChanged += new EventHandler(this.nudPreambleLength_ValueChanged);
			this.cBoxBandwidth.DropDownStyle = ComboBoxStyle.DropDownList;
			this.errorProvider.SetIconPadding((Control)this.cBoxBandwidth, 30);
			this.cBoxBandwidth.Items.AddRange(new object[10]
      {
        (object) "7.8125",
        (object) "10.4167",
        (object) "15.625",
        (object) "20.8333",
        (object) "31.25",
        (object) "41.6667",
        (object) "62.5",
        (object) "125",
        (object) "250",
        (object) "500"
      });
			this.cBoxBandwidth.Location = new Point(99, 77);
			this.cBoxBandwidth.Name = "cBoxBandwidth";
			this.cBoxBandwidth.Size = new Size(124, 21);
			this.cBoxBandwidth.TabIndex = 5;
			this.cBoxBandwidth.SelectedIndexChanged += new EventHandler(this.cBoxBandwidth_SelectedIndexChanged);
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
			this.panel11.AutoSize = true;
			this.panel11.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel11.Controls.Add((Control)this.rBtnImplicitHeaderOff);
			this.panel11.Controls.Add((Control)this.rBtnImplicitHeaderOn);
			this.panel11.Location = new Point(368, 50);
			this.panel11.Name = "panel11";
			this.panel11.Size = new Size(102, 20);
			this.panel11.TabIndex = 17;
			this.rBtnImplicitHeaderOff.AutoSize = true;
			this.rBtnImplicitHeaderOff.Location = new Point(54, 3);
			this.rBtnImplicitHeaderOff.Margin = new Padding(3, 0, 3, 0);
			this.rBtnImplicitHeaderOff.Name = "rBtnImplicitHeaderOff";
			this.rBtnImplicitHeaderOff.Size = new Size(45, 17);
			this.rBtnImplicitHeaderOff.TabIndex = 1;
			this.rBtnImplicitHeaderOff.Text = "OFF";
			this.rBtnImplicitHeaderOff.UseVisualStyleBackColor = true;
			this.rBtnImplicitHeaderOff.CheckedChanged += new EventHandler(this.rBtnImplicitHeader_CheckedChanged);
			this.rBtnImplicitHeaderOn.AutoSize = true;
			this.rBtnImplicitHeaderOn.Checked = true;
			this.rBtnImplicitHeaderOn.Location = new Point(3, 3);
			this.rBtnImplicitHeaderOn.Margin = new Padding(3, 0, 3, 0);
			this.rBtnImplicitHeaderOn.Name = "rBtnImplicitHeaderOn";
			this.rBtnImplicitHeaderOn.Size = new Size(41, 17);
			this.rBtnImplicitHeaderOn.TabIndex = 0;
			this.rBtnImplicitHeaderOn.TabStop = true;
			this.rBtnImplicitHeaderOn.Text = "ON";
			this.rBtnImplicitHeaderOn.UseVisualStyleBackColor = true;
			this.rBtnImplicitHeaderOn.CheckedChanged += new EventHandler(this.rBtnImplicitHeader_CheckedChanged);
			this.label8.AutoSize = true;
			this.label8.Location = new Point(276, 54);
			this.label8.Name = "label8";
			this.label8.Size = new Size(78, 13);
			this.label8.TabIndex = 16;
			this.label8.Text = "Implicit header:";
			this.label9.AutoSize = true;
			this.label9.Location = new Point(5, 108);
			this.label9.Name = "label9";
			this.label9.Size = new Size(60, 13);
			this.label9.TabIndex = 7;
			this.label9.Text = "Rx timeout:";
			this.label11.AutoSize = true;
			this.label11.Location = new Point(229, 108);
			this.label11.Name = "label11";
			this.label11.Size = new Size(12, 13);
			this.label11.TabIndex = 9;
			this.label11.Text = "s";
			this.label12.AutoSize = true;
			this.label12.Location = new Point(276, 108);
			this.label12.Name = "label12";
			this.label12.Size = new Size(73, 13);
			this.label12.TabIndex = 21;
			this.label12.Text = "Payload CRC:";
			this.panel12.AutoSize = true;
			this.panel12.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel12.Controls.Add((Control)this.rBtnPayloadCrcOff);
			this.panel12.Controls.Add((Control)this.rBtnPayloadCrcOn);
			this.panel12.Location = new Point(368, 104);
			this.panel12.Name = "panel12";
			this.panel12.Size = new Size(102, 20);
			this.panel12.TabIndex = 22;
			this.rBtnPayloadCrcOff.AutoSize = true;
			this.rBtnPayloadCrcOff.Location = new Point(54, 3);
			this.rBtnPayloadCrcOff.Margin = new Padding(3, 0, 3, 0);
			this.rBtnPayloadCrcOff.Name = "rBtnPayloadCrcOff";
			this.rBtnPayloadCrcOff.Size = new Size(45, 17);
			this.rBtnPayloadCrcOff.TabIndex = 1;
			this.rBtnPayloadCrcOff.Text = "OFF";
			this.rBtnPayloadCrcOff.UseVisualStyleBackColor = true;
			this.rBtnPayloadCrcOff.CheckedChanged += new EventHandler(this.rBtnPayloadCrc_CheckedChanged);
			this.rBtnPayloadCrcOn.AutoSize = true;
			this.rBtnPayloadCrcOn.Checked = true;
			this.rBtnPayloadCrcOn.Location = new Point(3, 3);
			this.rBtnPayloadCrcOn.Margin = new Padding(3, 0, 3, 0);
			this.rBtnPayloadCrcOn.Name = "rBtnPayloadCrcOn";
			this.rBtnPayloadCrcOn.Size = new Size(41, 17);
			this.rBtnPayloadCrcOn.TabIndex = 0;
			this.rBtnPayloadCrcOn.TabStop = true;
			this.rBtnPayloadCrcOn.Text = "ON";
			this.rBtnPayloadCrcOn.UseVisualStyleBackColor = true;
			this.rBtnPayloadCrcOn.CheckedChanged += new EventHandler(this.rBtnPayloadCrc_CheckedChanged);
			this.cBoxCodingRate.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxCodingRate.Items.AddRange(new object[4]
      {
        (object) "4/5",
        (object) "4/6",
        (object) "4/7",
        (object) "4/8"
      });
			this.cBoxCodingRate.Location = new Point(99, 50);
			this.cBoxCodingRate.Name = "cBoxCodingRate";
			this.cBoxCodingRate.Size = new Size(124, 21);
			this.cBoxCodingRate.TabIndex = 3;
			this.cBoxCodingRate.SelectedIndexChanged += new EventHandler(this.cBoxCodingRate_SelectedIndexChanged);
			this.label15.AutoSize = true;
			this.label15.Location = new Point(5, 54);
			this.label15.Name = "label15";
			this.label15.Size = new Size(64, 13);
			this.label15.TabIndex = 2;
			this.label15.Text = "Coding rate:";
			this.label16.AutoSize = true;
			this.label16.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
			this.label16.Location = new Point(276, 81);
			this.label16.Name = "label16";
			this.label16.Size = new Size(80, 13);
			this.label16.TabIndex = 18;
			this.label16.Text = "Payload length:";
			this.label16.TextAlign = ContentAlignment.MiddleLeft;
			this.label17.AutoSize = true;
			this.label17.Location = new Point(498, 81);
			this.label17.Name = "label17";
			this.label17.Size = new Size(32, 13);
			this.label17.TabIndex = 20;
			this.label17.Text = "bytes";
			this.label17.TextAlign = ContentAlignment.MiddleLeft;
			this.label19.AutoSize = true;
			this.label19.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
			this.label19.Location = new Point(276, 27);
			this.label19.Name = "label19";
			this.label19.Size = new Size(86, 13);
			this.label19.TabIndex = 13;
			this.label19.Text = "Preamble length:";
			this.label19.TextAlign = ContentAlignment.MiddleLeft;
			this.label29.AutoSize = true;
			this.label29.Location = new Point(498, 27);
			this.label29.Name = "label29";
			this.label29.Size = new Size(44, 13);
			this.label29.TabIndex = 15;
			this.label29.Text = "symbols";
			this.label29.TextAlign = ContentAlignment.MiddleLeft;
			this.label31.AutoSize = true;
			this.label31.Location = new Point(5, 81);
			this.label31.Name = "label31";
			this.label31.Size = new Size(60, 13);
			this.label31.TabIndex = 4;
			this.label31.Text = "Bandwidth:";
			this.label32.AutoSize = true;
			this.label32.Location = new Point(229, 81);
			this.label32.Name = "label32";
			this.label32.Size = new Size(26, 13);
			this.label32.TabIndex = 6;
			this.label32.Text = "kHz";
			this.label33.AutoSize = true;
			this.label33.Location = new Point(5, 27);
			this.label33.Name = "label33";
			this.label33.Size = new Size(88, 13);
			this.label33.TabIndex = 0;
			this.label33.Text = "Spreading factor:";
			this.cBoxSpreadingFactor.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxSpreadingFactor.Items.AddRange(new object[6]
      {
        (object) "SF7",
        (object) "SF8",
        (object) "SF9",
        (object) "SF10",
        (object) "SF11",
        (object) "SF12"
      });
			this.cBoxSpreadingFactor.Location = new Point(99, 23);
			this.cBoxSpreadingFactor.Name = "cBoxSpreadingFactor";
			this.cBoxSpreadingFactor.Size = new Size(124, 21);
			this.cBoxSpreadingFactor.TabIndex = 1;
			this.cBoxSpreadingFactor.SelectedIndexChanged += new EventHandler(this.cBoxSpreadingFactor_SelectedIndexChanged);
			this.label34.AutoSize = true;
			this.label34.Location = new Point(229, 170);
			this.label34.Name = "label34";
			this.label34.Size = new Size(76, 13);
			this.label34.TabIndex = 12;
			this.label34.Text = "symbol periods";
			this.label34.TextAlign = ContentAlignment.MiddleLeft;
			this.label34.Visible = false;
			this.label35.AutoSize = true;
			this.label35.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
			this.label35.Location = new Point(5, 163);
			this.label35.MaximumSize = new Size(94, 0);
			this.label35.Name = "label35";
			this.label35.Size = new Size(80, 26);
			this.label35.TabIndex = 10;
			this.label35.Text = "Frequency hopping period:";
			this.label35.TextAlign = ContentAlignment.MiddleLeft;
			this.label35.Visible = false;
			this.lblRxNbBytes.Anchor = AnchorStyles.None;
			this.lblRxNbBytes.BorderStyle = BorderStyle.Fixed3D;
			this.lblRxNbBytes.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
			this.lblRxNbBytes.Location = new Point(407, 37);
			this.lblRxNbBytes.Margin = new Padding(3);
			this.lblRxNbBytes.Name = "lblRxNbBytes";
			this.lblRxNbBytes.Size = new Size(59, 20);
			this.lblRxNbBytes.TabIndex = 9;
			this.lblRxNbBytes.Text = "-";
			this.lblRxNbBytes.TextAlign = ContentAlignment.MiddleCenter;
			this.label38.Anchor = AnchorStyles.None;
			this.label38.AutoSize = true;
			this.label38.Location = new Point(392, 4);
			this.label38.Margin = new Padding(3);
			this.label38.MaximumSize = new Size(90, 0);
			this.label38.MinimumSize = new Size(90, 0);
			this.label38.Name = "label38";
			this.label38.Size = new Size(90, 26);
			this.label38.TabIndex = 4;
			this.label38.Text = "Number of bytes received";
			this.label38.TextAlign = ContentAlignment.MiddleCenter;
			this.label18.Anchor = AnchorStyles.None;
			this.label18.AutoSize = true;
			this.label18.Location = new Point(4, 4);
			this.label18.Margin = new Padding(3);
			this.label18.MaximumSize = new Size(90, 0);
			this.label18.MinimumSize = new Size(90, 0);
			this.label18.Name = "label18";
			this.label18.Size = new Size(90, 26);
			this.label18.TabIndex = 0;
			this.label18.Text = "Received valid header count";
			this.label18.TextAlign = ContentAlignment.MiddleCenter;
			this.lblRxValidHeaderCnt.Anchor = AnchorStyles.None;
			this.lblRxValidHeaderCnt.BorderStyle = BorderStyle.Fixed3D;
			this.lblRxValidHeaderCnt.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
			this.lblRxValidHeaderCnt.Location = new Point(19, 37);
			this.lblRxValidHeaderCnt.Margin = new Padding(3);
			this.lblRxValidHeaderCnt.Name = "lblRxValidHeaderCnt";
			this.lblRxValidHeaderCnt.Size = new Size(59, 20);
			this.lblRxValidHeaderCnt.TabIndex = 5;
			this.lblRxValidHeaderCnt.Text = "-";
			this.lblRxValidHeaderCnt.TextAlign = ContentAlignment.MiddleCenter;
			this.label40.Anchor = AnchorStyles.None;
			this.label40.AutoSize = true;
			this.label40.Location = new Point(100, 4);
			this.label40.Margin = new Padding(3);
			this.label40.MaximumSize = new Size(90, 0);
			this.label40.MinimumSize = new Size(90, 0);
			this.label40.Name = "label40";
			this.label40.Size = new Size(90, 26);
			this.label40.TabIndex = 1;
			this.label40.Text = "Received valid packet count";
			this.label40.TextAlign = ContentAlignment.MiddleCenter;
			this.lblRxPacketCnt.Anchor = AnchorStyles.None;
			this.lblRxPacketCnt.BorderStyle = BorderStyle.Fixed3D;
			this.lblRxPacketCnt.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
			this.lblRxPacketCnt.Location = new Point(115, 37);
			this.lblRxPacketCnt.Margin = new Padding(3);
			this.lblRxPacketCnt.Name = "lblRxPacketCnt";
			this.lblRxPacketCnt.Size = new Size(59, 20);
			this.lblRxPacketCnt.TabIndex = 7;
			this.lblRxPacketCnt.Text = "-";
			this.lblRxPacketCnt.TextAlign = ContentAlignment.MiddleCenter;
			this.label46.Anchor = AnchorStyles.None;
			this.label46.AutoSize = true;
			this.label46.Location = new Point(292, 4);
			this.label46.Margin = new Padding(3);
			this.label46.MaximumSize = new Size(90, 0);
			this.label46.MinimumSize = new Size(90, 0);
			this.label46.Name = "label46";
			this.label46.Size = new Size(90, 26);
			this.label46.TabIndex = 3;
			this.label46.Text = "Received packet SNR [dB]";
			this.label46.TextAlign = ContentAlignment.MiddleCenter;
			this.lblPacketSnr.Anchor = AnchorStyles.None;
			this.lblPacketSnr.BorderStyle = BorderStyle.Fixed3D;
			this.lblPacketSnr.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
			this.lblPacketSnr.Location = new Point(307, 37);
			this.lblPacketSnr.Margin = new Padding(3);
			this.lblPacketSnr.Name = "lblPacketSnr";
			this.lblPacketSnr.Size = new Size(59, 20);
			this.lblPacketSnr.TabIndex = 9;
			this.lblPacketSnr.Text = "-";
			this.lblPacketSnr.TextAlign = ContentAlignment.MiddleCenter;
			this.label47.Anchor = AnchorStyles.None;
			this.label47.AutoSize = true;
			this.label47.Location = new Point(388, 4);
			this.label47.Margin = new Padding(3);
			this.label47.MaximumSize = new Size(90, 0);
			this.label47.MinimumSize = new Size(90, 0);
			this.label47.Name = "label47";
			this.label47.Size = new Size(90, 26);
			this.label47.TabIndex = 4;
			this.label47.Text = "Received packet RSSI [dBm]";
			this.label47.TextAlign = ContentAlignment.MiddleCenter;
			this.lblPacketRssi.Anchor = AnchorStyles.None;
			this.lblPacketRssi.BorderStyle = BorderStyle.Fixed3D;
			this.lblPacketRssi.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
			this.lblPacketRssi.Location = new Point(403, 37);
			this.lblPacketRssi.Margin = new Padding(3);
			this.lblPacketRssi.Name = "lblPacketRssi";
			this.lblPacketRssi.Size = new Size(59, 20);
			this.lblPacketRssi.TabIndex = 10;
			this.lblPacketRssi.Text = "-";
			this.lblPacketRssi.TextAlign = ContentAlignment.MiddleCenter;
			this.label48.Anchor = AnchorStyles.None;
			this.label48.AutoSize = true;
			this.label48.Location = new Point(486, 4);
			this.label48.Margin = new Padding(3);
			this.label48.MaximumSize = new Size(90, 0);
			this.label48.MinimumSize = new Size(90, 0);
			this.label48.Name = "label48";
			this.label48.Size = new Size(90, 26);
			this.label48.TabIndex = 5;
			this.label48.Text = "Current RSSI value [dBm]";
			this.label48.TextAlign = ContentAlignment.MiddleCenter;
			this.lblRssiValue.Anchor = AnchorStyles.None;
			this.lblRssiValue.BorderStyle = BorderStyle.Fixed3D;
			this.lblRssiValue.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
			this.lblRssiValue.Location = new Point(502, 37);
			this.lblRssiValue.Margin = new Padding(3);
			this.lblRssiValue.Name = "lblRssiValue";
			this.lblRssiValue.Size = new Size(59, 20);
			this.lblRssiValue.TabIndex = 11;
			this.lblRssiValue.Text = "-";
			this.lblRssiValue.TextAlign = ContentAlignment.MiddleCenter;
			this.label49.Anchor = AnchorStyles.None;
			this.label49.AutoSize = true;
			this.label49.Location = new Point(4, 4);
			this.label49.Margin = new Padding(3);
			this.label49.MaximumSize = new Size(90, 0);
			this.label49.MinimumSize = new Size(90, 0);
			this.label49.Name = "label49";
			this.label49.Size = new Size(90, 26);
			this.label49.TabIndex = 0;
			this.label49.Text = "Current hopping channel";
			this.label49.TextAlign = ContentAlignment.MiddleCenter;
			this.lblHopChannel.Anchor = AnchorStyles.None;
			this.lblHopChannel.BorderStyle = BorderStyle.Fixed3D;
			this.lblHopChannel.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
			this.lblHopChannel.Location = new Point(19, 37);
			this.lblHopChannel.Margin = new Padding(3);
			this.lblHopChannel.Name = "lblHopChannel";
			this.lblHopChannel.Size = new Size(59, 20);
			this.lblHopChannel.TabIndex = 6;
			this.lblHopChannel.Text = "-";
			this.lblHopChannel.TextAlign = ContentAlignment.MiddleCenter;
			this.label50.Anchor = AnchorStyles.None;
			this.label50.AutoSize = true;
			this.label50.Location = new Point(196, 4);
			this.label50.Margin = new Padding(3);
			this.label50.MaximumSize = new Size(90, 0);
			this.label50.MinimumSize = new Size(90, 0);
			this.label50.Name = "label50";
			this.label50.Size = new Size(90, 26);
			this.label50.TabIndex = 2;
			this.label50.Text = "Rx databuffer address";
			this.label50.TextAlign = ContentAlignment.MiddleCenter;
			this.lblFifoRxCurrentAddr.Anchor = AnchorStyles.None;
			this.lblFifoRxCurrentAddr.BorderStyle = BorderStyle.Fixed3D;
			this.lblFifoRxCurrentAddr.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
			this.lblFifoRxCurrentAddr.Location = new Point(211, 37);
			this.lblFifoRxCurrentAddr.Margin = new Padding(3);
			this.lblFifoRxCurrentAddr.Name = "lblFifoRxCurrentAddr";
			this.lblFifoRxCurrentAddr.Size = new Size(59, 20);
			this.lblFifoRxCurrentAddr.TabIndex = 8;
			this.lblFifoRxCurrentAddr.Text = "-";
			this.lblFifoRxCurrentAddr.TextAlign = ContentAlignment.MiddleCenter;
			this.pnlPacketStatus.AutoSize = true;
			this.pnlPacketStatus.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.pnlPacketStatus.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
			this.pnlPacketStatus.ColumnCount = 6;
			this.pnlPacketStatus.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667f));
			this.pnlPacketStatus.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667f));
			this.pnlPacketStatus.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667f));
			this.pnlPacketStatus.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667f));
			this.pnlPacketStatus.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667f));
			this.pnlPacketStatus.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667f));
			this.pnlPacketStatus.Controls.Add((Control)this.label49, 0, 0);
			this.pnlPacketStatus.Controls.Add((Control)this.lblHopChannel, 0, 1);
			this.pnlPacketStatus.Controls.Add((Control)this.lblRssiValue, 5, 1);
			this.pnlPacketStatus.Controls.Add((Control)this.lblPacketRssi, 4, 1);
			this.pnlPacketStatus.Controls.Add((Control)this.label48, 5, 0);
			this.pnlPacketStatus.Controls.Add((Control)this.label47, 4, 0);
			this.pnlPacketStatus.Controls.Add((Control)this.lblPacketSnr, 3, 1);
			this.pnlPacketStatus.Controls.Add((Control)this.label46, 3, 0);
			this.pnlPacketStatus.Controls.Add((Control)this.lblRxPacketCnt, 1, 1);
			this.pnlPacketStatus.Controls.Add((Control)this.label50, 2, 0);
			this.pnlPacketStatus.Controls.Add((Control)this.label40, 1, 0);
			this.pnlPacketStatus.Controls.Add((Control)this.lblFifoRxCurrentAddr, 2, 1);
			this.pnlPacketStatus.Location = new Point(108, 326);
			this.pnlPacketStatus.Name = "pnlPacketStatus";
			this.pnlPacketStatus.RowCount = 2;
			this.pnlPacketStatus.RowStyles.Add(new RowStyle());
			this.pnlPacketStatus.RowStyles.Add(new RowStyle());
			this.pnlPacketStatus.Size = new Size(583, 61);
			this.pnlPacketStatus.TabIndex = 7;
			this.lblRxPayloadCodingRate.Anchor = AnchorStyles.None;
			this.lblRxPayloadCodingRate.BorderStyle = BorderStyle.Fixed3D;
			this.lblRxPayloadCodingRate.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
			this.lblRxPayloadCodingRate.Location = new Point(310, 37);
			this.lblRxPayloadCodingRate.Margin = new Padding(3);
			this.lblRxPayloadCodingRate.Name = "lblRxPayloadCodingRate";
			this.lblRxPayloadCodingRate.Size = new Size(59, 20);
			this.lblRxPayloadCodingRate.TabIndex = 8;
			this.lblRxPayloadCodingRate.Text = "-";
			this.lblRxPayloadCodingRate.TextAlign = ContentAlignment.MiddleCenter;
			this.label37.Anchor = AnchorStyles.None;
			this.label37.AutoSize = true;
			this.label37.Location = new Point(295, 4);
			this.label37.Margin = new Padding(3);
			this.label37.MaximumSize = new Size(90, 0);
			this.label37.MinimumSize = new Size(90, 0);
			this.label37.Name = "label37";
			this.label37.Size = new Size(90, 26);
			this.label37.TabIndex = 3;
			this.label37.Text = "Rx payload coding rate";
			this.label37.TextAlign = ContentAlignment.MiddleCenter;
			this.label39.Anchor = AnchorStyles.None;
			this.label39.AutoSize = true;
			this.label39.Location = new Point(198, 10);
			this.label39.Margin = new Padding(3);
			this.label39.MaximumSize = new Size(90, 0);
			this.label39.MinimumSize = new Size(90, 0);
			this.label39.Name = "label39";
			this.label39.Size = new Size(90, 13);
			this.label39.TabIndex = 2;
			this.label39.Text = "Rx payload CRC";
			this.label39.TextAlign = ContentAlignment.MiddleCenter;
			this.pnlHeaderInfo.AutoSize = true;
			this.pnlHeaderInfo.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.pnlHeaderInfo.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
			this.pnlHeaderInfo.ColumnCount = 5;
			this.pnlHeaderInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20f));
			this.pnlHeaderInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20f));
			this.pnlHeaderInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20f));
			this.pnlHeaderInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20f));
			this.pnlHeaderInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20f));
			this.pnlHeaderInfo.Controls.Add((Control)this.label37, 3, 0);
			this.pnlHeaderInfo.Controls.Add((Control)this.lblRxPayloadCodingRate, 3, 1);
			this.pnlHeaderInfo.Controls.Add((Control)this.lblPllTimeout, 1, 0);
			this.pnlHeaderInfo.Controls.Add((Control)this.ledRxPayloadCrcOn, 2, 1);
			this.pnlHeaderInfo.Controls.Add((Control)this.ledPllTimeout, 1, 1);
			this.pnlHeaderInfo.Controls.Add((Control)this.label39, 2, 0);
			this.pnlHeaderInfo.Controls.Add((Control)this.label18, 0, 0);
			this.pnlHeaderInfo.Controls.Add((Control)this.lblRxValidHeaderCnt, 0, 1);
			this.pnlHeaderInfo.Controls.Add((Control)this.label38, 4, 0);
			this.pnlHeaderInfo.Controls.Add((Control)this.lblRxNbBytes, 4, 1);
			this.pnlHeaderInfo.Location = new Point(156, 245);
			this.pnlHeaderInfo.Name = "pnlHeaderInfo";
			this.pnlHeaderInfo.RowCount = 2;
			this.pnlHeaderInfo.RowStyles.Add(new RowStyle());
			this.pnlHeaderInfo.RowStyles.Add(new RowStyle());
			this.pnlHeaderInfo.Size = new Size(486, 61);
			this.pnlHeaderInfo.TabIndex = 4;
			this.lblPllTimeout.Anchor = AnchorStyles.None;
			this.lblPllTimeout.AutoSize = true;
			this.lblPllTimeout.Location = new Point(101, 10);
			this.lblPllTimeout.Margin = new Padding(3);
			this.lblPllTimeout.MaximumSize = new Size(90, 0);
			this.lblPllTimeout.MinimumSize = new Size(90, 0);
			this.lblPllTimeout.Name = "lblPllTimeout";
			this.lblPllTimeout.Size = new Size(90, 13);
			this.lblPllTimeout.TabIndex = 1;
			this.lblPllTimeout.Text = "PLL timeout";
			this.lblPllTimeout.TextAlign = ContentAlignment.MiddleCenter;
			this.ledRxPayloadCrcOn.Anchor = AnchorStyles.None;
			this.ledRxPayloadCrcOn.BackColor = Color.Transparent;
			this.ledRxPayloadCrcOn.LedColor = Color.Green;
			this.ledRxPayloadCrcOn.LedSize = new Size(11, 11);
			this.ledRxPayloadCrcOn.Location = new Point(235, 39);
			this.ledRxPayloadCrcOn.Name = "ledRxPayloadCrcOn";
			this.ledRxPayloadCrcOn.Size = new Size(15, 15);
			this.ledRxPayloadCrcOn.TabIndex = 7;
			this.ledRxPayloadCrcOn.Text = "Rx payload CRC on";
			this.ledPllTimeout.Anchor = AnchorStyles.None;
			this.ledPllTimeout.BackColor = Color.Transparent;
			this.ledPllTimeout.LedColor = Color.Green;
			this.ledPllTimeout.LedSize = new Size(11, 11);
			this.ledPllTimeout.Location = new Point(138, 39);
			this.ledPllTimeout.Name = "ledPllTimeout";
			this.ledPllTimeout.Size = new Size(15, 15);
			this.ledPllTimeout.TabIndex = 6;
			this.ledPllTimeout.Text = "PLL timeout";
			this.pnlRxHeaderInfoHeader.BorderStyle = BorderStyle.FixedSingle;
			this.pnlRxHeaderInfoHeader.Location = new Point(85, 234);
			this.pnlRxHeaderInfoHeader.Name = "pnlRxHeaderInfoHeader";
			this.pnlRxHeaderInfoHeader.Size = new Size(710, 1);
			this.pnlRxHeaderInfoHeader.TabIndex = 3;
			this.lblRxHeaderInfoHeaderName.AutoSize = true;
			this.lblRxHeaderInfoHeaderName.Location = new Point(3, 228);
			this.lblRxHeaderInfoHeaderName.Name = "lblRxHeaderInfoHeaderName";
			this.lblRxHeaderInfoHeaderName.Size = new Size(76, 13);
			this.lblRxHeaderInfoHeaderName.TabIndex = 2;
			this.lblRxHeaderInfoHeaderName.Text = "Rx header info";
			this.pnlPacketStatusHeaderName.BorderStyle = BorderStyle.FixedSingle;
			this.pnlPacketStatusHeaderName.Location = new Point(85, 315);
			this.pnlPacketStatusHeaderName.Name = "pnlPacketStatusHeaderName";
			this.pnlPacketStatusHeaderName.Size = new Size(710, 1);
			this.pnlPacketStatusHeaderName.TabIndex = 6;
			this.lblPacketStatusHeaderName.AutoSize = true;
			this.lblPacketStatusHeaderName.Location = new Point(3, 309);
			this.lblPacketStatusHeaderName.Name = "lblPacketStatusHeaderName";
			this.lblPacketStatusHeaderName.Size = new Size(72, 13);
			this.lblPacketStatusHeaderName.TabIndex = 5;
			this.lblPacketStatusHeaderName.Text = "Packet status";
			this.gBoxSettings.Controls.Add((Control)this.label33);
			this.gBoxSettings.Controls.Add((Control)this.nudSymbTimeout);
			this.gBoxSettings.Controls.Add((Control)this.label9);
			this.gBoxSettings.Controls.Add((Control)this.label11);
			this.gBoxSettings.Controls.Add((Control)this.label8);
			this.gBoxSettings.Controls.Add((Control)this.panel11);
			this.gBoxSettings.Controls.Add((Control)this.label35);
			this.gBoxSettings.Controls.Add((Control)this.label13);
			this.gBoxSettings.Controls.Add((Control)this.label12);
			this.gBoxSettings.Controls.Add((Control)this.label19);
			this.gBoxSettings.Controls.Add((Control)this.panel13);
			this.gBoxSettings.Controls.Add((Control)this.panel12);
			this.gBoxSettings.Controls.Add((Control)this.nudFreqHoppingPeriod);
			this.gBoxSettings.Controls.Add((Control)this.label34);
			this.gBoxSettings.Controls.Add((Control)this.nudPreambleLength);
			this.gBoxSettings.Controls.Add((Control)this.label15);
			this.gBoxSettings.Controls.Add((Control)this.label29);
			this.gBoxSettings.Controls.Add((Control)this.label31);
			this.gBoxSettings.Controls.Add((Control)this.nudPayloadLength);
			this.gBoxSettings.Controls.Add((Control)this.cBoxCodingRate);
			this.gBoxSettings.Controls.Add((Control)this.label16);
			this.gBoxSettings.Controls.Add((Control)this.label32);
			this.gBoxSettings.Controls.Add((Control)this.label17);
			this.gBoxSettings.Controls.Add((Control)this.cBoxBandwidth);
			this.gBoxSettings.Controls.Add((Control)this.cBoxSpreadingFactor);
			this.gBoxSettings.Location = new Point(3, 3);
			this.gBoxSettings.Name = "gBoxSettings";
			this.gBoxSettings.Size = new Size(548, 225);
			this.gBoxSettings.TabIndex = 0;
			this.gBoxSettings.TabStop = false;
			this.gBoxSettings.Text = "Settings";
			this.nudSymbTimeout.DecimalPlaces = 6;
			this.nudSymbTimeout.Increment = new Decimal(new int[4]
      {
        1024,
        0,
        0,
        393216
      });
			this.nudSymbTimeout.Location = new Point(99, 104);
			this.nudSymbTimeout.Maximum = new Decimal(new int[4]
      {
        1047552,
        0,
        0,
        393216
      });
			this.nudSymbTimeout.Name = "nudSymbTimeout";
			this.nudSymbTimeout.Size = new Size(124, 20);
			this.nudSymbTimeout.TabIndex = 8;
			this.nudSymbTimeout.ThousandsSeparator = true;
			this.nudSymbTimeout.Value = new Decimal(new int[4]
      {
        1024,
        0,
        0,
        262144
      });
			this.nudSymbTimeout.ValueChanged += new EventHandler(this.nudSymbTimeout_ValueChanged);
			this.label13.AutoSize = true;
			this.label13.Location = new Point(5, 131);
			this.label13.MaximumSize = new Size(94, 0);
			this.label13.Name = "label13";
			this.label13.Size = new Size(72, 26);
			this.label13.TabIndex = 21;
			this.label13.Text = "Low datarate optimize:";
			this.panel13.AutoSize = true;
			this.panel13.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel13.Controls.Add((Control)this.rBtnLowDatarateOptimizeOff);
			this.panel13.Controls.Add((Control)this.rBtnLowDatarateOptimizeOn);
			this.panel13.Location = new Point(99, 134);
			this.panel13.Name = "panel13";
			this.panel13.Size = new Size(102, 20);
			this.panel13.TabIndex = 22;
			this.rBtnLowDatarateOptimizeOff.AutoSize = true;
			this.rBtnLowDatarateOptimizeOff.Location = new Point(54, 3);
			this.rBtnLowDatarateOptimizeOff.Margin = new Padding(3, 0, 3, 0);
			this.rBtnLowDatarateOptimizeOff.Name = "rBtnLowDatarateOptimizeOff";
			this.rBtnLowDatarateOptimizeOff.Size = new Size(45, 17);
			this.rBtnLowDatarateOptimizeOff.TabIndex = 1;
			this.rBtnLowDatarateOptimizeOff.Text = "OFF";
			this.rBtnLowDatarateOptimizeOff.UseVisualStyleBackColor = true;
			this.rBtnLowDatarateOptimizeOff.CheckedChanged += new EventHandler(this.rBtnLowDatarateOptimize_CheckedChanged);
			this.rBtnLowDatarateOptimizeOn.AutoSize = true;
			this.rBtnLowDatarateOptimizeOn.Checked = true;
			this.rBtnLowDatarateOptimizeOn.Location = new Point(3, 3);
			this.rBtnLowDatarateOptimizeOn.Margin = new Padding(3, 0, 3, 0);
			this.rBtnLowDatarateOptimizeOn.Name = "rBtnLowDatarateOptimizeOn";
			this.rBtnLowDatarateOptimizeOn.Size = new Size(41, 17);
			this.rBtnLowDatarateOptimizeOn.TabIndex = 0;
			this.rBtnLowDatarateOptimizeOn.TabStop = true;
			this.rBtnLowDatarateOptimizeOn.Text = "ON";
			this.rBtnLowDatarateOptimizeOn.UseVisualStyleBackColor = true;
			this.rBtnLowDatarateOptimizeOn.CheckedChanged += new EventHandler(this.rBtnLowDatarateOptimize_CheckedChanged);
			this.nudFreqHoppingPeriod.Location = new Point(99, 166);
			NumericUpDownEx numericUpDownEx4 = this.nudFreqHoppingPeriod;
			int[] bits4 = new int[4];
			bits4[0] = (int)byte.MaxValue;
			Decimal num4 = new Decimal(bits4);
			numericUpDownEx4.Maximum = num4;
			this.nudFreqHoppingPeriod.Name = "nudFreqHoppingPeriod";
			this.nudFreqHoppingPeriod.Size = new Size(124, 20);
			this.nudFreqHoppingPeriod.TabIndex = 11;
			this.nudFreqHoppingPeriod.Visible = false;
			this.nudFreqHoppingPeriod.ValueChanged += new EventHandler(this.nudFreqHoppingPeriod_ValueChanged);
			this.nudPayloadLength.Location = new Point(368, 77);
			NumericUpDownEx numericUpDownEx5 = this.nudPayloadLength;
			int[] bits5 = new int[4];
			bits5[0] = 256;
			Decimal num5 = new Decimal(bits5);
			numericUpDownEx5.Maximum = num5;
			this.nudPayloadLength.Name = "nudPayloadLength";
			this.nudPayloadLength.Size = new Size(124, 20);
			this.nudPayloadLength.TabIndex = 19;
			NumericUpDownEx numericUpDownEx6 = this.nudPayloadLength;
			int[] bits6 = new int[4];
			bits6[0] = 14;
			Decimal num6 = new Decimal(bits6);
			numericUpDownEx6.Value = num6;
			this.nudPayloadLength.ValueChanged += new EventHandler(this.nudPayloadLength_ValueChanged);
			this.gBoxMessage.Controls.Add((Control)this.tblPayloadMessage);
			this.gBoxMessage.Location = new Point(3, 393);
			this.gBoxMessage.Name = "gBoxMessage";
			this.gBoxMessage.Size = new Size(528, 97);
			this.gBoxMessage.TabIndex = 8;
			this.gBoxMessage.TabStop = false;
			this.gBoxMessage.Text = "Message";
			this.tblPayloadMessage.AutoSize = true;
			this.tblPayloadMessage.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tblPayloadMessage.ColumnCount = 2;
			this.tblPayloadMessage.ColumnStyles.Add(new ColumnStyle());
			this.tblPayloadMessage.ColumnStyles.Add(new ColumnStyle());
			this.tblPayloadMessage.Controls.Add((Control)this.hexBoxPayload, 0, 1);
			this.tblPayloadMessage.Controls.Add((Control)this.label51, 1, 0);
			this.tblPayloadMessage.Controls.Add((Control)this.label52, 0, 0);
			this.tblPayloadMessage.Location = new Point(11, 16);
			this.tblPayloadMessage.Name = "tblPayloadMessage";
			this.tblPayloadMessage.RowCount = 2;
			this.tblPayloadMessage.RowStyles.Add(new RowStyle());
			this.tblPayloadMessage.RowStyles.Add(new RowStyle());
			this.tblPayloadMessage.Size = new Size(507, 79);
			this.tblPayloadMessage.TabIndex = 0;
			this.hexBoxPayload.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.tblPayloadMessage.SetColumnSpan((Control)this.hexBoxPayload, 2);
			this.hexBoxPayload.Font = new Font("Courier New", 8.25f);
			this.hexBoxPayload.LineInfoDigits = (byte)2;
			this.hexBoxPayload.LineInfoForeColor = Color.Empty;
			this.hexBoxPayload.Location = new Point(3, 16);
			this.hexBoxPayload.Name = "hexBoxPayload";
			this.hexBoxPayload.ShadowSelectionColor = Color.FromArgb(100, 60, 188, (int)byte.MaxValue);
			this.hexBoxPayload.Size = new Size(501, 60);
			this.hexBoxPayload.StringViewVisible = true;
			this.hexBoxPayload.TabIndex = 2;
			this.hexBoxPayload.UseFixedBytesPerLine = true;
			this.hexBoxPayload.VScrollBarVisible = true;
			this.label51.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.label51.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
			this.label51.Location = new Point(329, 0);
			this.label51.Name = "label51";
			this.label51.Size = new Size(175, 13);
			this.label51.TabIndex = 1;
			this.label51.Text = "ASCII";
			this.label51.TextAlign = ContentAlignment.MiddleCenter;
			this.label52.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.label52.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
			this.label52.Location = new Point(3, 0);
			this.label52.Name = "label52";
			this.label52.Size = new Size(320, 13);
			this.label52.TabIndex = 0;
			this.label52.Text = "HEXADECIMAL";
			this.label52.TextAlign = ContentAlignment.MiddleCenter;
			this.gBoxControl.Controls.Add((Control)this.ledLogEnabled);
			this.gBoxControl.Controls.Add((Control)this.pnlPacketMode);
			this.gBoxControl.Controls.Add((Control)this.tBoxPacketsNb);
			this.gBoxControl.Controls.Add((Control)this.btnLog);
			this.gBoxControl.Controls.Add((Control)this.cBtnPacketHandlerStartStop);
			this.gBoxControl.Controls.Add((Control)this.lblPacketsNb);
			this.gBoxControl.Controls.Add((Control)this.tBoxPacketsRepeatValue);
			this.gBoxControl.Controls.Add((Control)this.lblPacketsRepeatValue);
			this.gBoxControl.Location = new Point(537, 393);
			this.gBoxControl.Name = "gBoxControl";
			this.gBoxControl.Size = new Size(259, 97);
			this.gBoxControl.TabIndex = 9;
			this.gBoxControl.TabStop = false;
			this.gBoxControl.Text = "Packet Control";
			this.ledLogEnabled.BackColor = Color.Transparent;
			this.ledLogEnabled.LedColor = Color.Green;
			this.ledLogEnabled.LedSize = new Size(11, 11);
			this.ledLogEnabled.Location = new Point(143, 23);
			this.ledLogEnabled.Name = "ledLogEnabled";
			this.ledLogEnabled.Size = new Size(15, 15);
			this.ledLogEnabled.TabIndex = 2;
			this.ledLogEnabled.Text = "Log status";
			this.pnlPacketMode.AutoSize = true;
			this.pnlPacketMode.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.pnlPacketMode.Controls.Add((Control)this.rBtnPacketModeRx);
			this.pnlPacketMode.Controls.Add((Control)this.rBtnPacketModeTx);
			this.pnlPacketMode.Location = new Point(168, 19);
			this.pnlPacketMode.Name = "pnlPacketMode";
			this.pnlPacketMode.Size = new Size(87, 20);
			this.pnlPacketMode.TabIndex = 3;
			this.rBtnPacketModeRx.AutoSize = true;
			this.rBtnPacketModeRx.Checked = true;
			this.rBtnPacketModeRx.Location = new Point(46, 3);
			this.rBtnPacketModeRx.Margin = new Padding(3, 0, 3, 0);
			this.rBtnPacketModeRx.Name = "rBtnPacketModeRx";
			this.rBtnPacketModeRx.Size = new Size(38, 17);
			this.rBtnPacketModeRx.TabIndex = 1;
			this.rBtnPacketModeRx.TabStop = true;
			this.rBtnPacketModeRx.Text = "Rx";
			this.rBtnPacketModeRx.UseVisualStyleBackColor = true;
			this.rBtnPacketModeRx.CheckedChanged += new EventHandler(this.rBtnPacketMode_CheckedChanged);
			this.rBtnPacketModeTx.AutoSize = true;
			this.rBtnPacketModeTx.Location = new Point(3, 3);
			this.rBtnPacketModeTx.Margin = new Padding(3, 0, 3, 0);
			this.rBtnPacketModeTx.Name = "rBtnPacketModeTx";
			this.rBtnPacketModeTx.Size = new Size(37, 17);
			this.rBtnPacketModeTx.TabIndex = 0;
			this.rBtnPacketModeTx.Text = "Tx";
			this.rBtnPacketModeTx.UseVisualStyleBackColor = true;
			this.rBtnPacketModeTx.CheckedChanged += new EventHandler(this.rBtnPacketMode_CheckedChanged);
			this.tBoxPacketsNb.Location = new Point(149, 48);
			this.tBoxPacketsNb.Name = "tBoxPacketsNb";
			this.tBoxPacketsNb.ReadOnly = true;
			this.tBoxPacketsNb.Size = new Size(79, 20);
			this.tBoxPacketsNb.TabIndex = 5;
			this.tBoxPacketsNb.Text = "0";
			this.tBoxPacketsNb.TextAlign = HorizontalAlignment.Right;
			this.btnLog.Location = new Point(87, 19);
			this.btnLog.Name = "btnLog";
			this.btnLog.Size = new Size(75, 23);
			this.btnLog.TabIndex = 1;
			this.btnLog.Text = "Log";
			this.btnLog.Click += new EventHandler(this.btnLog_Click);
			this.cBtnPacketHandlerStartStop.Appearance = Appearance.Button;
			this.cBtnPacketHandlerStartStop.Location = new Point(6, 19);
			this.cBtnPacketHandlerStartStop.Name = "cBtnPacketHandlerStartStop";
			this.cBtnPacketHandlerStartStop.Size = new Size(75, 23);
			this.cBtnPacketHandlerStartStop.TabIndex = 0;
			this.cBtnPacketHandlerStartStop.Text = "Start";
			this.cBtnPacketHandlerStartStop.TextAlign = ContentAlignment.MiddleCenter;
			this.cBtnPacketHandlerStartStop.UseVisualStyleBackColor = true;
			this.cBtnPacketHandlerStartStop.CheckedChanged += new EventHandler(this.cBtnPacketHandlerStartStop_CheckedChanged);
			this.lblPacketsNb.AutoSize = true;
			this.lblPacketsNb.Location = new Point(3, 51);
			this.lblPacketsNb.Name = "lblPacketsNb";
			this.lblPacketsNb.Size = new Size(64, 13);
			this.lblPacketsNb.TabIndex = 4;
			this.lblPacketsNb.Text = "Tx Packets:";
			this.lblPacketsNb.TextAlign = ContentAlignment.MiddleLeft;
			this.tBoxPacketsRepeatValue.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
			this.tBoxPacketsRepeatValue.Location = new Point(149, 70);
			this.tBoxPacketsRepeatValue.Name = "tBoxPacketsRepeatValue";
			this.tBoxPacketsRepeatValue.Size = new Size(79, 20);
			this.tBoxPacketsRepeatValue.TabIndex = 7;
			this.tBoxPacketsRepeatValue.Text = "0";
			this.tBoxPacketsRepeatValue.TextAlign = HorizontalAlignment.Right;
			this.lblPacketsRepeatValue.AutoSize = true;
			this.lblPacketsRepeatValue.Location = new Point(3, 73);
			this.lblPacketsRepeatValue.Name = "lblPacketsRepeatValue";
			this.lblPacketsRepeatValue.Size = new Size(74, 13);
			this.lblPacketsRepeatValue.TabIndex = 6;
			this.lblPacketsRepeatValue.Text = "Repeat value:";
			this.lblPacketsRepeatValue.TextAlign = ContentAlignment.MiddleLeft;
			this.groupBoxEx1.Controls.Add((Control)this.label53);
			this.groupBoxEx1.Controls.Add((Control)this.label42);
			this.groupBoxEx1.Controls.Add((Control)this.ledSignalDetected);
			this.groupBoxEx1.Controls.Add((Control)this.label45);
			this.groupBoxEx1.Controls.Add((Control)this.ledSignalSynchronized);
			this.groupBoxEx1.Controls.Add((Control)this.label43);
			this.groupBoxEx1.Controls.Add((Control)this.ledRxOnGoing);
			this.groupBoxEx1.Controls.Add((Control)this.label41);
			this.groupBoxEx1.Controls.Add((Control)this.ledHeaderInfoValid);
			this.groupBoxEx1.Controls.Add((Control)this.label44);
			this.groupBoxEx1.Controls.Add((Control)this.ledModemClear);
			this.groupBoxEx1.Location = new Point(863, 241);
			this.groupBoxEx1.Name = "groupBoxEx1";
			this.groupBoxEx1.Size = new Size(135, 139);
			this.groupBoxEx1.TabIndex = 28;
			this.groupBoxEx1.TabStop = false;
			this.groupBoxEx1.Text = "Modem status";
			this.groupBoxEx1.Visible = false;
			this.label53.AutoSize = true;
			this.label53.Location = new Point(30, 16);
			this.label53.Name = "label53";
			this.label53.Size = new Size(102, 13);
			this.label53.TabIndex = 1;
			this.label53.Text = "Searching Preamble";
			this.label42.AutoSize = true;
			this.label42.Location = new Point(30, 34);
			this.label42.Name = "label42";
			this.label42.Size = new Size(68, 13);
			this.label42.TabIndex = 1;
			this.label42.Text = "Modem clear";
			this.ledSignalDetected.BackColor = Color.Transparent;
			this.ledSignalDetected.LedColor = Color.Green;
			this.ledSignalDetected.LedSize = new Size(11, 11);
			this.ledSignalDetected.Location = new Point(6, 117);
			this.ledSignalDetected.Name = "ledSignalDetected";
			this.ledSignalDetected.Size = new Size(15, 15);
			this.ledSignalDetected.TabIndex = 2;
			this.ledSignalDetected.Text = "Signal detected";
			this.label45.AutoSize = true;
			this.label45.Location = new Point(30, 118);
			this.label45.Name = "label45";
			this.label45.Size = new Size(81, 13);
			this.label45.TabIndex = 3;
			this.label45.Text = "Signal detected";
			this.ledSignalSynchronized.BackColor = Color.Transparent;
			this.ledSignalSynchronized.LedColor = Color.Green;
			this.ledSignalSynchronized.LedSize = new Size(11, 11);
			this.ledSignalSynchronized.Location = new Point(6, 96);
			this.ledSignalSynchronized.Name = "ledSignalSynchronized";
			this.ledSignalSynchronized.Size = new Size(15, 15);
			this.ledSignalSynchronized.TabIndex = 2;
			this.ledSignalSynchronized.Text = "Signal synchronized";
			this.label43.AutoSize = true;
			this.label43.Location = new Point(30, 97);
			this.label43.Name = "label43";
			this.label43.Size = new Size(101, 13);
			this.label43.TabIndex = 3;
			this.label43.Text = "Signal synchronized";
			this.ledRxOnGoing.BackColor = Color.Transparent;
			this.ledRxOnGoing.LedColor = Color.Green;
			this.ledRxOnGoing.LedSize = new Size(11, 11);
			this.ledRxOnGoing.Location = new Point(6, 75);
			this.ledRxOnGoing.Name = "ledRxOnGoing";
			this.ledRxOnGoing.Size = new Size(15, 15);
			this.ledRxOnGoing.TabIndex = 2;
			this.ledRxOnGoing.Text = "Rx on going";
			this.label41.AutoSize = true;
			this.label41.Location = new Point(30, 76);
			this.label41.Name = "label41";
			this.label41.Size = new Size(64, 13);
			this.label41.TabIndex = 3;
			this.label41.Text = "Rx on going";
			this.ledHeaderInfoValid.BackColor = Color.Transparent;
			this.ledHeaderInfoValid.LedColor = Color.Green;
			this.ledHeaderInfoValid.LedSize = new Size(11, 11);
			this.ledHeaderInfoValid.Location = new Point(6, 54);
			this.ledHeaderInfoValid.Name = "ledHeaderInfoValid";
			this.ledHeaderInfoValid.Size = new Size(15, 15);
			this.ledHeaderInfoValid.TabIndex = 2;
			this.ledHeaderInfoValid.Text = "Header info valid";
			this.label44.AutoSize = true;
			this.label44.Location = new Point(30, 55);
			this.label44.Name = "label44";
			this.label44.Size = new Size(87, 13);
			this.label44.TabIndex = 3;
			this.label44.Text = "Header info valid";
			this.ledModemClear.BackColor = Color.Transparent;
			this.ledModemClear.LedColor = Color.Green;
			this.ledModemClear.LedSize = new Size(11, 11);
			this.ledModemClear.Location = new Point(6, 33);
			this.ledModemClear.Name = "ledModemClear";
			this.ledModemClear.Size = new Size(15, 15);
			this.ledModemClear.TabIndex = 0;
			this.ledModemClear.Text = "Modem clear";
			this.gBoxIrqMask.Controls.Add((Control)this.panel10);
			this.gBoxIrqMask.Controls.Add((Control)this.label7);
			this.gBoxIrqMask.Controls.Add((Control)this.panel9);
			this.gBoxIrqMask.Controls.Add((Control)this.label6);
			this.gBoxIrqMask.Controls.Add((Control)this.panel8);
			this.gBoxIrqMask.Controls.Add((Control)this.label5);
			this.gBoxIrqMask.Controls.Add((Control)this.panel5);
			this.gBoxIrqMask.Controls.Add((Control)this.label4);
			this.gBoxIrqMask.Controls.Add((Control)this.panel3);
			this.gBoxIrqMask.Controls.Add((Control)this.label3);
			this.gBoxIrqMask.Controls.Add((Control)this.panel2);
			this.gBoxIrqMask.Controls.Add((Control)this.label2);
			this.gBoxIrqMask.Controls.Add((Control)this.panel1);
			this.gBoxIrqMask.Controls.Add((Control)this.label1);
			this.gBoxIrqMask.Controls.Add((Control)this.panel4);
			this.gBoxIrqMask.Controls.Add((Control)this.label10);
			this.gBoxIrqMask.Location = new Point(557, 3);
			this.gBoxIrqMask.Name = "gBoxIrqMask";
			this.gBoxIrqMask.Size = new Size(239, 225);
			this.gBoxIrqMask.TabIndex = 1;
			this.gBoxIrqMask.TabStop = false;
			this.gBoxIrqMask.Text = "IRQ mask";
			this.gBoxIrqMask.MouseEnter += new EventHandler(this.control_MouseEnter);
			this.gBoxIrqMask.MouseLeave += new EventHandler(this.control_MouseLeave);
			this.panel10.AutoSize = true;
			this.panel10.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel10.Controls.Add((Control)this.rBtnCadDetectedMaskOff);
			this.panel10.Controls.Add((Control)this.rBtnCadDetectedMaskOn);
			this.panel10.Location = new Point(128, 198);
			this.panel10.Name = "panel10";
			this.panel10.Size = new Size(102, 20);
			this.panel10.TabIndex = 15;
			this.rBtnCadDetectedMaskOff.AutoSize = true;
			this.rBtnCadDetectedMaskOff.Location = new Point(54, 3);
			this.rBtnCadDetectedMaskOff.Margin = new Padding(3, 0, 3, 0);
			this.rBtnCadDetectedMaskOff.Name = "rBtnCadDetectedMaskOff";
			this.rBtnCadDetectedMaskOff.Size = new Size(45, 17);
			this.rBtnCadDetectedMaskOff.TabIndex = 1;
			this.rBtnCadDetectedMaskOff.Text = "OFF";
			this.rBtnCadDetectedMaskOff.UseVisualStyleBackColor = true;
			this.rBtnCadDetectedMaskOff.CheckedChanged += new EventHandler(this.rBtnCadDetectedMask_CheckedChanged);
			this.rBtnCadDetectedMaskOn.AutoSize = true;
			this.rBtnCadDetectedMaskOn.Checked = true;
			this.rBtnCadDetectedMaskOn.Location = new Point(3, 3);
			this.rBtnCadDetectedMaskOn.Margin = new Padding(3, 0, 3, 0);
			this.rBtnCadDetectedMaskOn.Name = "rBtnCadDetectedMaskOn";
			this.rBtnCadDetectedMaskOn.Size = new Size(41, 17);
			this.rBtnCadDetectedMaskOn.TabIndex = 0;
			this.rBtnCadDetectedMaskOn.TabStop = true;
			this.rBtnCadDetectedMaskOn.Text = "ON";
			this.rBtnCadDetectedMaskOn.UseVisualStyleBackColor = true;
			this.rBtnCadDetectedMaskOn.CheckedChanged += new EventHandler(this.rBtnCadDetectedMask_CheckedChanged);
			this.label7.AutoSize = true;
			this.label7.Location = new Point(6, 202);
			this.label7.Name = "label7";
			this.label7.Size = new Size(77, 13);
			this.label7.TabIndex = 14;
			this.label7.Text = "CAD detected:";
			this.panel9.AutoSize = true;
			this.panel9.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel9.Controls.Add((Control)this.rBtnFhssChangeChannelMaskOff);
			this.panel9.Controls.Add((Control)this.rBtnFhssChangeChannelMaskOn);
			this.panel9.Location = new Point(128, 172);
			this.panel9.Name = "panel9";
			this.panel9.Size = new Size(102, 20);
			this.panel9.TabIndex = 13;
			this.rBtnFhssChangeChannelMaskOff.AutoSize = true;
			this.rBtnFhssChangeChannelMaskOff.Location = new Point(54, 3);
			this.rBtnFhssChangeChannelMaskOff.Margin = new Padding(3, 0, 3, 0);
			this.rBtnFhssChangeChannelMaskOff.Name = "rBtnFhssChangeChannelMaskOff";
			this.rBtnFhssChangeChannelMaskOff.Size = new Size(45, 17);
			this.rBtnFhssChangeChannelMaskOff.TabIndex = 1;
			this.rBtnFhssChangeChannelMaskOff.Text = "OFF";
			this.rBtnFhssChangeChannelMaskOff.UseVisualStyleBackColor = true;
			this.rBtnFhssChangeChannelMaskOff.CheckedChanged += new EventHandler(this.rBtnFhssChangeChannelMask_CheckedChanged);
			this.rBtnFhssChangeChannelMaskOn.AutoSize = true;
			this.rBtnFhssChangeChannelMaskOn.Checked = true;
			this.rBtnFhssChangeChannelMaskOn.Location = new Point(3, 3);
			this.rBtnFhssChangeChannelMaskOn.Margin = new Padding(3, 0, 3, 0);
			this.rBtnFhssChangeChannelMaskOn.Name = "rBtnFhssChangeChannelMaskOn";
			this.rBtnFhssChangeChannelMaskOn.Size = new Size(41, 17);
			this.rBtnFhssChangeChannelMaskOn.TabIndex = 0;
			this.rBtnFhssChangeChannelMaskOn.TabStop = true;
			this.rBtnFhssChangeChannelMaskOn.Text = "ON";
			this.rBtnFhssChangeChannelMaskOn.UseVisualStyleBackColor = true;
			this.rBtnFhssChangeChannelMaskOn.CheckedChanged += new EventHandler(this.rBtnFhssChangeChannelMask_CheckedChanged);
			this.label6.AutoSize = true;
			this.label6.Location = new Point(6, 176);
			this.label6.Name = "label6";
			this.label6.Size = new Size(118, 13);
			this.label6.TabIndex = 12;
			this.label6.Text = "FHSS change channel:";
			this.panel8.AutoSize = true;
			this.panel8.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel8.Controls.Add((Control)this.rBtnCadDoneMaskOff);
			this.panel8.Controls.Add((Control)this.rBtnCadDoneMaskOn);
			this.panel8.Location = new Point(128, 146);
			this.panel8.Name = "panel8";
			this.panel8.Size = new Size(102, 20);
			this.panel8.TabIndex = 11;
			this.rBtnCadDoneMaskOff.AutoSize = true;
			this.rBtnCadDoneMaskOff.Location = new Point(54, 3);
			this.rBtnCadDoneMaskOff.Margin = new Padding(3, 0, 3, 0);
			this.rBtnCadDoneMaskOff.Name = "rBtnCadDoneMaskOff";
			this.rBtnCadDoneMaskOff.Size = new Size(45, 17);
			this.rBtnCadDoneMaskOff.TabIndex = 1;
			this.rBtnCadDoneMaskOff.Text = "OFF";
			this.rBtnCadDoneMaskOff.UseVisualStyleBackColor = true;
			this.rBtnCadDoneMaskOff.CheckedChanged += new EventHandler(this.rBtnCadDoneMask_CheckedChanged);
			this.rBtnCadDoneMaskOn.AutoSize = true;
			this.rBtnCadDoneMaskOn.Checked = true;
			this.rBtnCadDoneMaskOn.Location = new Point(3, 3);
			this.rBtnCadDoneMaskOn.Margin = new Padding(3, 0, 3, 0);
			this.rBtnCadDoneMaskOn.Name = "rBtnCadDoneMaskOn";
			this.rBtnCadDoneMaskOn.Size = new Size(41, 17);
			this.rBtnCadDoneMaskOn.TabIndex = 0;
			this.rBtnCadDoneMaskOn.TabStop = true;
			this.rBtnCadDoneMaskOn.Text = "ON";
			this.rBtnCadDoneMaskOn.UseVisualStyleBackColor = true;
			this.rBtnCadDoneMaskOn.CheckedChanged += new EventHandler(this.rBtnCadDoneMask_CheckedChanged);
			this.label5.AutoSize = true;
			this.label5.Location = new Point(6, 150);
			this.label5.Name = "label5";
			this.label5.Size = new Size(59, 13);
			this.label5.TabIndex = 10;
			this.label5.Text = "CAD done:";
			this.panel5.AutoSize = true;
			this.panel5.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel5.Controls.Add((Control)this.rBtnTxDoneMaskOff);
			this.panel5.Controls.Add((Control)this.rBtnTxDoneMaskOn);
			this.panel5.Location = new Point(128, 120);
			this.panel5.Name = "panel5";
			this.panel5.Size = new Size(102, 20);
			this.panel5.TabIndex = 9;
			this.rBtnTxDoneMaskOff.AutoSize = true;
			this.rBtnTxDoneMaskOff.Location = new Point(54, 3);
			this.rBtnTxDoneMaskOff.Margin = new Padding(3, 0, 3, 0);
			this.rBtnTxDoneMaskOff.Name = "rBtnTxDoneMaskOff";
			this.rBtnTxDoneMaskOff.Size = new Size(45, 17);
			this.rBtnTxDoneMaskOff.TabIndex = 1;
			this.rBtnTxDoneMaskOff.Text = "OFF";
			this.rBtnTxDoneMaskOff.UseVisualStyleBackColor = true;
			this.rBtnTxDoneMaskOff.CheckedChanged += new EventHandler(this.rBtnTxDoneMask_CheckedChanged);
			this.rBtnTxDoneMaskOn.AutoSize = true;
			this.rBtnTxDoneMaskOn.Checked = true;
			this.rBtnTxDoneMaskOn.Location = new Point(3, 3);
			this.rBtnTxDoneMaskOn.Margin = new Padding(3, 0, 3, 0);
			this.rBtnTxDoneMaskOn.Name = "rBtnTxDoneMaskOn";
			this.rBtnTxDoneMaskOn.Size = new Size(41, 17);
			this.rBtnTxDoneMaskOn.TabIndex = 0;
			this.rBtnTxDoneMaskOn.TabStop = true;
			this.rBtnTxDoneMaskOn.Text = "ON";
			this.rBtnTxDoneMaskOn.UseVisualStyleBackColor = true;
			this.rBtnTxDoneMaskOn.CheckedChanged += new EventHandler(this.rBtnTxDoneMask_CheckedChanged);
			this.label4.AutoSize = true;
			this.label4.Location = new Point(6, 124);
			this.label4.Name = "label4";
			this.label4.Size = new Size(49, 13);
			this.label4.TabIndex = 8;
			this.label4.Text = "Tx done:";
			this.panel3.AutoSize = true;
			this.panel3.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel3.Controls.Add((Control)this.rBtnValidHeaderMaskOff);
			this.panel3.Controls.Add((Control)this.rBtnValidHeaderMaskOn);
			this.panel3.Location = new Point(128, 94);
			this.panel3.Name = "panel3";
			this.panel3.Size = new Size(102, 20);
			this.panel3.TabIndex = 7;
			this.rBtnValidHeaderMaskOff.AutoSize = true;
			this.rBtnValidHeaderMaskOff.Location = new Point(54, 3);
			this.rBtnValidHeaderMaskOff.Margin = new Padding(3, 0, 3, 0);
			this.rBtnValidHeaderMaskOff.Name = "rBtnValidHeaderMaskOff";
			this.rBtnValidHeaderMaskOff.Size = new Size(45, 17);
			this.rBtnValidHeaderMaskOff.TabIndex = 1;
			this.rBtnValidHeaderMaskOff.Text = "OFF";
			this.rBtnValidHeaderMaskOff.UseVisualStyleBackColor = true;
			this.rBtnValidHeaderMaskOff.CheckedChanged += new EventHandler(this.rBtnValidHeaderMask_CheckedChanged);
			this.rBtnValidHeaderMaskOn.AutoSize = true;
			this.rBtnValidHeaderMaskOn.Checked = true;
			this.rBtnValidHeaderMaskOn.Location = new Point(3, 3);
			this.rBtnValidHeaderMaskOn.Margin = new Padding(3, 0, 3, 0);
			this.rBtnValidHeaderMaskOn.Name = "rBtnValidHeaderMaskOn";
			this.rBtnValidHeaderMaskOn.Size = new Size(41, 17);
			this.rBtnValidHeaderMaskOn.TabIndex = 0;
			this.rBtnValidHeaderMaskOn.TabStop = true;
			this.rBtnValidHeaderMaskOn.Text = "ON";
			this.rBtnValidHeaderMaskOn.UseVisualStyleBackColor = true;
			this.rBtnValidHeaderMaskOn.CheckedChanged += new EventHandler(this.rBtnValidHeaderMask_CheckedChanged);
			this.label3.AutoSize = true;
			this.label3.Location = new Point(6, 98);
			this.label3.Name = "label3";
			this.label3.Size = new Size(69, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "Valid header:";
			this.panel2.AutoSize = true;
			this.panel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel2.Controls.Add((Control)this.rBtnPayloadCrcErrorMaskOff);
			this.panel2.Controls.Add((Control)this.rBtnPayloadCrcErrorMaskOn);
			this.panel2.Location = new Point(128, 68);
			this.panel2.Name = "panel2";
			this.panel2.Size = new Size(102, 20);
			this.panel2.TabIndex = 5;
			this.rBtnPayloadCrcErrorMaskOff.AutoSize = true;
			this.rBtnPayloadCrcErrorMaskOff.Location = new Point(54, 3);
			this.rBtnPayloadCrcErrorMaskOff.Margin = new Padding(3, 0, 3, 0);
			this.rBtnPayloadCrcErrorMaskOff.Name = "rBtnPayloadCrcErrorMaskOff";
			this.rBtnPayloadCrcErrorMaskOff.Size = new Size(45, 17);
			this.rBtnPayloadCrcErrorMaskOff.TabIndex = 1;
			this.rBtnPayloadCrcErrorMaskOff.Text = "OFF";
			this.rBtnPayloadCrcErrorMaskOff.UseVisualStyleBackColor = true;
			this.rBtnPayloadCrcErrorMaskOff.CheckedChanged += new EventHandler(this.rBtnPayloadCrcErrorMask_CheckedChanged);
			this.rBtnPayloadCrcErrorMaskOn.AutoSize = true;
			this.rBtnPayloadCrcErrorMaskOn.Checked = true;
			this.rBtnPayloadCrcErrorMaskOn.Location = new Point(3, 3);
			this.rBtnPayloadCrcErrorMaskOn.Margin = new Padding(3, 0, 3, 0);
			this.rBtnPayloadCrcErrorMaskOn.Name = "rBtnPayloadCrcErrorMaskOn";
			this.rBtnPayloadCrcErrorMaskOn.Size = new Size(41, 17);
			this.rBtnPayloadCrcErrorMaskOn.TabIndex = 0;
			this.rBtnPayloadCrcErrorMaskOn.TabStop = true;
			this.rBtnPayloadCrcErrorMaskOn.Text = "ON";
			this.rBtnPayloadCrcErrorMaskOn.UseVisualStyleBackColor = true;
			this.rBtnPayloadCrcErrorMaskOn.CheckedChanged += new EventHandler(this.rBtnPayloadCrcErrorMask_CheckedChanged);
			this.label2.AutoSize = true;
			this.label2.Location = new Point(6, 72);
			this.label2.Name = "label2";
			this.label2.Size = new Size(97, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Payload CRC error:";
			this.panel1.AutoSize = true;
			this.panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel1.Controls.Add((Control)this.rBtnRxDoneMaskOff);
			this.panel1.Controls.Add((Control)this.rBtnRxDoneMaskOn);
			this.panel1.Location = new Point(128, 42);
			this.panel1.Name = "panel1";
			this.panel1.Size = new Size(102, 20);
			this.panel1.TabIndex = 3;
			this.rBtnRxDoneMaskOff.AutoSize = true;
			this.rBtnRxDoneMaskOff.Location = new Point(54, 3);
			this.rBtnRxDoneMaskOff.Margin = new Padding(3, 0, 3, 0);
			this.rBtnRxDoneMaskOff.Name = "rBtnRxDoneMaskOff";
			this.rBtnRxDoneMaskOff.Size = new Size(45, 17);
			this.rBtnRxDoneMaskOff.TabIndex = 1;
			this.rBtnRxDoneMaskOff.Text = "OFF";
			this.rBtnRxDoneMaskOff.UseVisualStyleBackColor = true;
			this.rBtnRxDoneMaskOff.CheckedChanged += new EventHandler(this.rBtnRxDoneMask_CheckedChanged);
			this.rBtnRxDoneMaskOn.AutoSize = true;
			this.rBtnRxDoneMaskOn.Checked = true;
			this.rBtnRxDoneMaskOn.Location = new Point(3, 3);
			this.rBtnRxDoneMaskOn.Margin = new Padding(3, 0, 3, 0);
			this.rBtnRxDoneMaskOn.Name = "rBtnRxDoneMaskOn";
			this.rBtnRxDoneMaskOn.Size = new Size(41, 17);
			this.rBtnRxDoneMaskOn.TabIndex = 0;
			this.rBtnRxDoneMaskOn.TabStop = true;
			this.rBtnRxDoneMaskOn.Text = "ON";
			this.rBtnRxDoneMaskOn.UseVisualStyleBackColor = true;
			this.rBtnRxDoneMaskOn.CheckedChanged += new EventHandler(this.rBtnRxDoneMask_CheckedChanged);
			this.label1.AutoSize = true;
			this.label1.Location = new Point(6, 46);
			this.label1.Name = "label1";
			this.label1.Size = new Size(50, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Rx done:";
			this.panel4.AutoSize = true;
			this.panel4.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel4.Controls.Add((Control)this.rBtnRxTimeoutMaskOff);
			this.panel4.Controls.Add((Control)this.rBtnRxTimeoutMaskOn);
			this.panel4.Location = new Point(128, 16);
			this.panel4.Name = "panel4";
			this.panel4.Size = new Size(102, 20);
			this.panel4.TabIndex = 1;
			this.rBtnRxTimeoutMaskOff.AutoSize = true;
			this.rBtnRxTimeoutMaskOff.Location = new Point(54, 3);
			this.rBtnRxTimeoutMaskOff.Margin = new Padding(3, 0, 3, 0);
			this.rBtnRxTimeoutMaskOff.Name = "rBtnRxTimeoutMaskOff";
			this.rBtnRxTimeoutMaskOff.Size = new Size(45, 17);
			this.rBtnRxTimeoutMaskOff.TabIndex = 1;
			this.rBtnRxTimeoutMaskOff.Text = "OFF";
			this.rBtnRxTimeoutMaskOff.UseVisualStyleBackColor = true;
			this.rBtnRxTimeoutMaskOff.CheckedChanged += new EventHandler(this.rBtnRxTimeoutMask_CheckedChanged);
			this.rBtnRxTimeoutMaskOn.AutoSize = true;
			this.rBtnRxTimeoutMaskOn.Checked = true;
			this.rBtnRxTimeoutMaskOn.Location = new Point(3, 3);
			this.rBtnRxTimeoutMaskOn.Margin = new Padding(3, 0, 3, 0);
			this.rBtnRxTimeoutMaskOn.Name = "rBtnRxTimeoutMaskOn";
			this.rBtnRxTimeoutMaskOn.Size = new Size(41, 17);
			this.rBtnRxTimeoutMaskOn.TabIndex = 0;
			this.rBtnRxTimeoutMaskOn.TabStop = true;
			this.rBtnRxTimeoutMaskOn.Text = "ON";
			this.rBtnRxTimeoutMaskOn.UseVisualStyleBackColor = true;
			this.rBtnRxTimeoutMaskOn.CheckedChanged += new EventHandler(this.rBtnRxTimeoutMask_CheckedChanged);
			this.label10.AutoSize = true;
			this.label10.Location = new Point(6, 20);
			this.label10.Name = "label10";
			this.label10.Size = new Size(60, 13);
			this.label10.TabIndex = 0;
			this.label10.Text = "Rx timeout:";
			this.AutoScaleDimensions = new SizeF(6f, 13f);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.Controls.Add((Control)this.pnlPacketStatusHeaderName);
			this.Controls.Add((Control)this.lblPacketStatusHeaderName);
			this.Controls.Add((Control)this.pnlRxHeaderInfoHeader);
			this.Controls.Add((Control)this.lblRxHeaderInfoHeaderName);
			this.Controls.Add((Control)this.pnlHeaderInfo);
			this.Controls.Add((Control)this.pnlPacketStatus);
			this.Controls.Add((Control)this.gBoxMessage);
			this.Controls.Add((Control)this.gBoxControl);
			this.Controls.Add((Control)this.groupBoxEx1);
			this.Controls.Add((Control)this.gBoxIrqMask);
			this.Controls.Add((Control)this.gBoxSettings);
			this.Name = "LoRaViewControl";
			this.Size = new Size(799, 493);
			((ISupportInitialize)this.errorProvider).EndInit();
			this.nudPreambleLength.EndInit();
			this.panel11.ResumeLayout(false);
			this.panel11.PerformLayout();
			this.panel12.ResumeLayout(false);
			this.panel12.PerformLayout();
			this.pnlPacketStatus.ResumeLayout(false);
			this.pnlPacketStatus.PerformLayout();
			this.pnlHeaderInfo.ResumeLayout(false);
			this.pnlHeaderInfo.PerformLayout();
			this.gBoxSettings.ResumeLayout(false);
			this.gBoxSettings.PerformLayout();
			this.nudSymbTimeout.EndInit();
			this.panel13.ResumeLayout(false);
			this.panel13.PerformLayout();
			this.nudFreqHoppingPeriod.EndInit();
			this.nudPayloadLength.EndInit();
			this.gBoxMessage.ResumeLayout(false);
			this.gBoxMessage.PerformLayout();
			this.tblPayloadMessage.ResumeLayout(false);
			this.gBoxControl.ResumeLayout(false);
			this.gBoxControl.PerformLayout();
			this.pnlPacketMode.ResumeLayout(false);
			this.pnlPacketMode.PerformLayout();
			this.groupBoxEx1.ResumeLayout(false);
			this.groupBoxEx1.PerformLayout();
			this.gBoxIrqMask.ResumeLayout(false);
			this.gBoxIrqMask.PerformLayout();
			this.panel10.ResumeLayout(false);
			this.panel10.PerformLayout();
			this.panel9.ResumeLayout(false);
			this.panel9.PerformLayout();
			this.panel8.ResumeLayout(false);
			this.panel8.PerformLayout();
			this.panel5.ResumeLayout(false);
			this.panel5.PerformLayout();
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel4.ResumeLayout(false);
			this.panel4.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
	}
}
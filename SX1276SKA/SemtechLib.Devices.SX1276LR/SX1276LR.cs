using SemtechLib.Devices.Common.Events;
using SemtechLib.Devices.Common.Hid;
using SemtechLib.Devices.Common.Interfaces;
using SemtechLib.Devices.SX1276LR.Enumerations;
using SemtechLib.Devices.SX1276LR.Events;
using SemtechLib.Devices.SX1276LR.General;
using SemtechLib.General;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SemtechLib.Devices.SX1276LR
{
	public class SX1276LR : IDevice, INotifyPropertyChanged, IDisposable
	{
		#region Enums
		private enum HidCommandsStatus
		{
			SX_OK,
			SX_ERROR,
			SX_BUSY,
			SX_EMPTY,
			SX_DONE,
			SX_TIMEOUT,
			SX_UNSUPPORTED,
			SX_WAIT,
			SX_CLOSE,
			SX_ACK,
			SX_NACK,
			SX_YES,
			SX_NO,
		}

		private enum HidCommands
		{
			HID_SK_RESET = 0,
			HID_SK_GET_VERSION = 1,
			HID_SK_GET_NAME = 2,
			HID_SK_GET_ID = 3,
			HID_SK_SET_ID = 4,
			HID_SK_SET_ID_RND = 5,
			HID_SK_FW_UPDATE = 6,
			HID_SK_GET_PIN = 16,
			HID_SK_SET_PIN = 17,
			HID_SK_GET_DIR = 18,
			HID_SK_SET_DIR = 19,
			HID_SK_GET_PINS = 20,
			HID_SK_SM_RESET = 32,
			HID_SK_SM_STEP = 33,
			HID_SK_SM_GET_TIME = 34,
			HID_SK_SM_SET_TIME = 35,
			HID_SK_SM_TRIG_ON_TIME = 36,
			HID_SK_SM_TRIG_ON_PIN = 37,
			HID_EEPROM_READ = 112,
			HID_EEPROM_WRITE = 113,
			HID_DEVICE_READ = 128,
			HID_DEVICE_WRITE = 129,
			HID_DEVICE_GET_COM_INTERFACE = 130,
			HID_DEVICE_SET_COM_INTERFACE = 131,
			HID_DEVICE_GET_COM_INTERFACE_SPEED = 132,
			HID_DEVICE_SET_COM_INTERFACE_SPEED = 133,
			HID_DEVICE_GET_COM_ADDR = 134,
			HID_DEVICE_SET_COM_ADDR = 135,
			HID_DEVICE_INIT = 136,
			HID_DEVICE_RESET = 137,
			HID_DEVICE_GET_BITRATE = 138,
			HID_DEVICE_SET_BITRATE = 139,
			HID_DEVICE_GET_PACKET = 140,
			HID_DEVICE_SET_PACKET = 141,
			HID_DEVICE_GET_PACKET_BUFFER = 142,
			HID_DEVICE_SET_PACKET_BUFFER = 143,
			HID_DEVICE_SEND_TX_PACKET = 144,
			HID_DEVICE_GET_BTN_PACKET = 145,
			HID_DEVICE_SET_BTN_PACKET = 146,
			HID_DEVICE_GET_PN_SEQUENCE = 160,
			HID_DEVICE_SET_PN_SEQUENCE = 161,
			HID_DEVICE_GET_PN_ENABLE = 162,
			HID_DEVICE_SET_PN_ENABLE = 163,
			HID_SK_CMD_NONE = 255,
		}
		#endregion

		public delegate void LimitCheckStatusChangedEventHandler(object sender, LimitCheckStatusEventArg e);
		public delegate void IoChangedEventHandler(object sender, SX1276LR.IoChangedEventArgs e);

		#region Privates
		private List<double> pktRssiValues = new List<double>();
		private List<int> pktLnaValues = new List<int>();
		private object syncThread = new object();
		private Version fwVersion = new Version(0, 0);
		private int spiSpeed = 2000000;
		private Decimal frequencyXo = new Decimal(32000000);
		private Decimal frequencyStep = new Decimal(32000000) / (Decimal)Math.Pow(2.0, 19.0);
		private bool monitor = true;
		private Decimal spectrumFreqSpan = new Decimal(1000000);
		private Decimal spectrumRssiValue = new Decimal(1275, 0, 0, true, (byte)1);
		private bool lowFrequencyModeOn = true;
		private OperatingModeEnum mode = OperatingModeEnum.Stdby;
		private Decimal frequencyRf = new Decimal(915000000);
		private Decimal maxOutputPower = new Decimal(132, 0, 0, false, (byte)1);
		private Decimal outputPower = new Decimal(132, 0, 0, false, (byte)1);
		private PaRampEnum paRamp = PaRampEnum.PaRamp_40;
		private bool ocpOn = true;
		private Decimal ocpTrim = new Decimal(100);
		private LnaGainEnum lnaGain = LnaGainEnum.G1;
		private byte rxPayloadCodingRate = (byte)2;
		private Decimal prevRssiValue = new Decimal(1275, 0, 0, true, (byte)1);
		private byte bandwidth = (byte)7;
		private byte codingRate = (byte)1;
		private byte spreadingFactor = (byte)7;
		private ushort preambleLength = (ushort)12;
		private byte payloadLength = (byte)14;
		private byte payloadLengthRx = (byte)32;
		private byte payloadMaxLength = (byte)14;
		private Version version = new Version(0, 0);
		private byte agcReferenceLevel = (byte)19;
		private byte agcStep1 = (byte)14;
		private byte agcStep2 = (byte)5;
		private byte agcStep3 = (byte)11;
		private byte agcStep4 = (byte)13;
		private byte agcStep5 = (byte)11;
		private Decimal pllBandwidth = new Decimal(300000);
		private Decimal rfPaRssiValue = new Decimal(1275, 0, 0, true, (byte)1);
		private Decimal rfIoRssiValue = new Decimal(1275, 0, 0, true, (byte)1);
		private byte[] payload = new byte[0];
		private bool lowFrequencyMode = true;
		private bool lnaBoostPrev = true;
		private byte[] packetBuffer = new byte[256];
		private const byte HID_CMD_INDEX = (byte)0;
		private const byte HID_CMD_OPT_INDEX = (byte)0;
		private const byte HID_CMD_DATA_SIZE_INDEX = (byte)1;
		private const byte HID_CMD_DATA_INDEX = (byte)2;
		private const byte HID_CMD_ANS_INDEX = (byte)0;
		private const byte HID_CMD_ANS_STAT_INDEX = (byte)0;
		private const byte HID_CMD_ANS_TIME_STAMP_INDEX = (byte)1;
		private const byte HID_CMD_ANS_DATA_SIZE_INDEX = (byte)9;
		private const byte HID_CMD_ANS_DATA_INDEX = (byte)10;
		private const int FR_BAND_1_MAX = 175000000;
		private const int FR_BAND_1_MIN = 137000000;
		private const int FR_BAND_2_MAX = 525000000;
		private const int FR_BAND_2_MIN = 410000000;
		private const int FR_BAND_3_MAX = 1024000000;
		private const int FR_BAND_3_MIN = 820000000;
		private const int BRF_MAX = 250000;
		private const int BRO_MAX = 32768;
		private const int BR_MIN = 1200;
		private const int FDA_MAX = 200000;
		private const int FDA_MIN = 600;
		private const int BW_SSB_MAX = 250000;
		private const int PA_20_DBM_OCP_TRIM_MIN = 150;
		private const int PA_20_DBM_OCP_TRIM_MAX = 240;
		private const int NOISE_ABSOLUTE_ZERO = -174;
		private const int RSSI_OFFSET_LF = -164;
		private const int RSSI_OFFSET_HF = -157;
		protected bool regUpdateThreadContinue;
		protected Thread regUpdateThread;
		protected int readLock;
		protected int writeLock;
		protected bool restartRx;
		protected bool bitRateFdevCheckDisbale;
		protected bool frequencyRfCheckDisable;
		private HidDevice usbDevice;
		private string deviceName;
		private bool isOpen;
		private RegisterCollection registers;
		private bool test;
		private ILog packetHandlerLog;
		private bool packetModeTx;
		private bool packetModeRxSingle;
		private bool packetUsePer;
		private bool isDebugOn;
		private bool isReceiving;
		private OperatingModeEnum prevMode;
		private LnaGainEnum prevLnaGain;
		private bool prevAgcAutoOn;
		private bool prevMonitorOn;
		private bool spectrumOn;
		private int spectrumFreqId;
		private bool accessSharedFskReg;
		private BandEnum band;
		private PaSelectEnum paSelect;
		private bool forceTxBandLowFrequencyOn;
		private bool forceRxBandLowFrequencyOn;
		private bool lnaBoost;
		private byte fifoAddrPtr;
		private byte fifoTxBaseAddr;
		private byte fifoRxBaseAddr;
		private byte fifoRxCurrentAddr;
		private bool rxTimeoutMask;
		private bool rxDoneMask;
		private bool payloadCrcErrorMask;
		private bool validHeaderMask;
		private bool txDoneMask;
		private bool cadDoneMask;
		private bool fhssChangeChannelMask;
		private bool cadDetectedMask;
		private bool rxTimeout;
		private bool rxDone;
		private bool payloadCrcError;
		private bool validHeader;
		private bool txDone;
		private bool cadDone;
		private bool fhssChangeChannel;
		private bool cadDetected;
		private byte rxNbBytes;
		private ushort validHeaderCnt;
		private ushort validPacketCnt;
		private bool modemClear;
		private bool headerInfoValid;
		private bool rxOnGoing;
		private bool signalSynchronized;
		private bool signalDetected;
		private sbyte pktSnrValue;
		private Decimal pktRssiValue;
		private Decimal rssiValue;
		private bool pllTimeout;
		private bool rxPayloadCrcOn;
		private byte hopChannel;
		private bool implicitHeaderModeOn;
		private bool txContinuousModeOn;
		private bool payloadCrcOn;
		private Decimal symbTimeout;
		private byte freqHoppingPeriod;
		private bool lowDatarateOptimize;
		private bool agcAutoOn;
		private DioMappingEnum dio0Mapping;
		private DioMappingEnum dio1Mapping;
		private DioMappingEnum dio2Mapping;
		private DioMappingEnum dio3Mapping;
		private DioMappingEnum dio4Mapping;
		private DioMappingEnum dio5Mapping;
		private bool fastHopOn;
		private bool tcxoInputOn;
		private bool pa20dBm;
		private RfPaSwitchSelEnum rfPaSwitchSel;
		private RfPaSwitchSelEnum prevRfPaSwitchSel;
		private int prevRfPaSwitchEnabled;
		private int rfPaSwitchEnabled;
		private bool logEnabled;
		private bool isPacketHandlerStarted;
		private int packetNumber;
		private int maxPacketNumber;
		private bool frameTransmitted;
		private bool frameReceived;
		private bool firstTransmit;
		#endregion

		public event EventHandler Connected;
		public event EventHandler Disconected;
		public event SemtechLib.General.Events.ErrorEventHandler Error;
		public event SX1276LR.LimitCheckStatusChangedEventHandler OcpTrimLimitStatusChanged;
		public event SX1276LR.LimitCheckStatusChangedEventHandler FrequencyRfLimitStatusChanged;
		public event SX1276LR.LimitCheckStatusChangedEventHandler BandwidthLimitStatusChanged;
		public event SX1276LR.IoChangedEventHandler Dio0Changed;
		public event SX1276LR.IoChangedEventHandler Dio1Changed;
		public event SX1276LR.IoChangedEventHandler Dio2Changed;
		public event SX1276LR.IoChangedEventHandler Dio3Changed;
		public event SX1276LR.IoChangedEventHandler Dio4Changed;
		public event SX1276LR.IoChangedEventHandler Dio5Changed;
		public event EventHandler PacketHandlerStarted;
		public event EventHandler PacketHandlerStoped;
		public event PacketStatusEventHandler PacketHandlerTransmitted;
		public event PacketStatusEventHandler PacketHandlerReceived;
		public event PropertyChangedEventHandler PropertyChanged;

		public SX1276LR()
		{
			PropertyChanged += new PropertyChangedEventHandler(device_PropertyChanged);

			deviceName = "SX12xxEiger";
			usbDevice = new HidDevice(0x047A, 0x000B, deviceName);

			usbDevice.Opened += new EventHandler(usbDevice_Opened);
			usbDevice.Closed += new EventHandler(usbDevice_Closed);

			Dio0Changed += new SX1276LR.IoChangedEventHandler(device_Dio0Changed);
			Dio1Changed += new SX1276LR.IoChangedEventHandler(device_Dio1Changed);
			Dio2Changed += new SX1276LR.IoChangedEventHandler(device_Dio2Changed);
			Dio3Changed += new SX1276LR.IoChangedEventHandler(device_Dio3Changed);
			Dio4Changed += new SX1276LR.IoChangedEventHandler(device_Dio4Changed);
			Dio5Changed += new SX1276LR.IoChangedEventHandler(device_Dio5Changed);

			PacketHandlerLog = (ILog)new PacketLog((IDevice)this);
			PacketHandlerLog.PropertyChanged += new PropertyChangedEventHandler(PacketHandlerLog_PropertyChanged);

			PopulateRegisters();
		}

		public List<double> PktRssiValues
		{
			get { return pktRssiValues; }
			set
			{
				if (pktRssiValues == value)
					return;
				pktRssiValues = value;
			}
		}

		public List<int> PktLnaValues
		{
			get { return pktLnaValues; }
			set
			{
				if (pktLnaValues == value)
					return;
				pktLnaValues = value;
			}
		}

		public object SyncThread
		{
			get { return syncThread; }
		}

		public string DeviceName
		{
			get { return deviceName; }
		}

		public Version FwVersion
		{
			get { return fwVersion; }
			set
			{
				if (fwVersion == value)
					return;
				fwVersion = value;
				OnPropertyChanged("FwVersion");
			}
		}

		public HidDevice UsbDevice
		{
			get { return usbDevice; }
		}

		public bool IsOpen
		{
			get { return isOpen; }
			set
			{
				isOpen = value;
				OnPropertyChanged("IsOpen");
			}
		}

		public int SPISpeed
		{
			get { return spiSpeed; }
			set
			{
				spiSpeed = value;
				OnPropertyChanged("SPISpeed");
			}
		}

		public RegisterCollection Registers
		{
			get { return registers; }
			set
			{
				registers = value;
				OnPropertyChanged("Registers");
			}
		}

		public bool Test
		{
			get { return test; }
			set { test = value; }
		}

		public ILog PacketHandlerLog
		{
			get { return packetHandlerLog; }
			set
			{
				packetHandlerLog = value;
				OnPropertyChanged("PacketHandlerLog");
			}
		}

		public bool PacketModeTx
		{
			get { return packetModeTx; }
			set
			{
				packetModeTx = value;
				OnPropertyChanged("PacketModeTx");
				OnPropertyChanged("PayloadLength");
			}
		}

		public bool PacketModeRxSingle
		{
			get { return packetModeRxSingle; }
			set
			{
				packetModeRxSingle = value;
				OnPropertyChanged("PacketModeRxSingle");
				OnPropertyChanged("PayloadLength");
			}
		}

		public bool PacketUsePer
		{
			get { return packetUsePer; }
			set
			{
				packetUsePer = value;
				OnPropertyChanged("PacketUsePer");
			}
		}

		public bool IsDebugOn
		{
			get { return isDebugOn; }
			set
			{
				isDebugOn = value;
				OnPropertyChanged("IsDebugOn");
			}
		}

		public Decimal FrequencyXo
		{
			get { return frequencyXo; }
			set
			{
				frequencyXo = value;
				FrequencyStep = frequencyXo / (Decimal)Math.Pow(2.0, 19.0);
				FrequencyRf = (Decimal)((uint)((int)registers["RegFrfMsb"].Value << 16 | (int)registers["RegFrfMid"].Value << 8) | registers["RegFrfLsb"].Value) * FrequencyStep;
				OnPropertyChanged("FrequencyXo");
			}
		}

		public Decimal FrequencyStep
		{
			get { return frequencyStep; }
			set
			{
				frequencyStep = value;
				OnPropertyChanged("FrequencyStep");
			}
		}

		public Decimal SymbolRate
		{
			get { return (Decimal)(BandwidthHz / Math.Pow(2.0, (double)SpreadingFactor)); }
		}

		public Decimal SymbolTime
		{
			get { return new Decimal(1) / SymbolRate; }
		}

		public bool Monitor
		{
			get
			{
				lock (syncThread)
					return monitor;
			}
			set
			{
				lock (syncThread)
				{
					monitor = value;
					OnPropertyChanged("Monitor");
				}
			}
		}

		public bool SpectrumOn
		{
			get { return spectrumOn; }
			set
			{
				spectrumOn = value;
				if (spectrumOn)
				{
					RfPaSwitchEnabled = 0;
					prevAgcAutoOn = AgcAutoOn;
					SetAgcAutoOn(false);
					prevLnaGain = LnaGain;
					SetLnaGain(LnaGainEnum.G1);
					prevMode = Mode;
					SetOperatingMode(OperatingModeEnum.Rx);
					prevMonitorOn = Monitor;
					Monitor = true;
				}
				else
				{
					SetFrequencyRf(FrequencyRf);
					RfPaSwitchEnabled = prevRfPaSwitchEnabled;
					SetLnaGain(prevLnaGain);
					SetAgcAutoOn(prevAgcAutoOn);
					SetOperatingMode(prevMode);
					Monitor = prevMonitorOn;
				}
				OnPropertyChanged("SpectrumOn");
			}
		}

		public Decimal SpectrumFrequencySpan
		{
			get { return spectrumFreqSpan; }
			set
			{
				spectrumFreqSpan = value;
				OnPropertyChanged("SpectrumFreqSpan");
			}
		}

		public Decimal SpectrumFrequencyMax
		{
			get { return FrequencyRf + SpectrumFrequencySpan / new Decimal(20, 0, 0, false, 1); }
		}

		public Decimal SpectrumFrequencyMin
		{
			get { return FrequencyRf - SpectrumFrequencySpan / new Decimal(20, 0, 0, false, 1); }
		}

		public int SpectrumNbFrequenciesMax
		{
			get { return (int)((SpectrumFrequencyMax - SpectrumFrequencyMin) / SpectrumFrequencyStep); }
		}

		public Decimal SpectrumFrequencyStep
		{
			get { return (Decimal)BandwidthHz / new Decimal(30, 0, 0, false, 1); }
		}

		public int SpectrumFrequencyId
		{
			get { return spectrumFreqId; }
			set
			{
				spectrumFreqId = value;
				OnPropertyChanged("SpectrumFreqId");
			}
		}

		public Decimal SpectrumRssiValue
		{
			get { return spectrumRssiValue; }
		}

		public bool LowFrequencyModeOn
		{
			get { return lowFrequencyModeOn; }
			set
			{
				lowFrequencyModeOn = value;
				OnPropertyChanged("LowFrequencyModeOn");
			}
		}

		public bool AccessSharedFskReg
		{
			get { return accessSharedFskReg; }
			set
			{
				accessSharedFskReg = value;
				OnPropertyChanged("AccessSharedFskReg");
			}
		}

		public OperatingModeEnum Mode
		{
			get { return mode; }
			set
			{
				mode = value;
				OnPropertyChanged("Mode");
			}
		}

		public BandEnum Band
		{
			get { return band; }
			set
			{
				band = value;
				OnPropertyChanged("Band");
			}
		}

		public Decimal FrequencyRf
		{
			get { return frequencyRf; }
			set
			{
				frequencyRf = value;
				OnPropertyChanged("FrequencyRf");
				FrequencyRfCheck(value);
			}
		}

		public PaSelectEnum PaSelect
		{
			get { return paSelect; }
			set
			{
				paSelect = value;
				OnPropertyChanged("PaSelect");
			}
		}

		public Decimal MaxOutputPower
		{
			get { return maxOutputPower; }
			set
			{
				maxOutputPower = value;
				OnPropertyChanged("MaxOutputPower");
			}
		}

		public Decimal OutputPower
		{
			get { return outputPower; }
			set
			{
				outputPower = value;
				OnPropertyChanged("OutputPower");
			}
		}

		public bool ForceTxBandLowFrequencyOn
		{
			get { return forceTxBandLowFrequencyOn; }
			set
			{
				forceTxBandLowFrequencyOn = value;
				OnPropertyChanged("ForceTxBandLowFrequencyOn");
			}
		}

		public PaRampEnum PaRamp
		{
			get { return paRamp; }
			set
			{
				paRamp = value;
				OnPropertyChanged("PaRamp");
			}
		}

		public bool OcpOn
		{
			get { return ocpOn; }
			set
			{
				ocpOn = value;
				OnPropertyChanged("OcpOn");
			}
		}

		public Decimal OcpTrim
		{
			get { return ocpTrim; }
			set
			{
				ocpTrim = value;
				OnPropertyChanged("OcpTrim");
			}
		}

		public LnaGainEnum LnaGain
		{
			get { return lnaGain; }
			set
			{
				lnaGain = value;
				OnPropertyChanged("LnaGain");
			}
		}

		public bool ForceRxBandLowFrequencyOn
		{
			get { return forceRxBandLowFrequencyOn; }
			set
			{
				forceRxBandLowFrequencyOn = value;
				OnPropertyChanged("ForceRxBandLowFrequencyOn");
			}
		}

		public bool LnaBoost
		{
			get { return lnaBoost; }
			set
			{
				lnaBoost = value;
				OnPropertyChanged("LnaBoost");
			}
		}

		public byte FifoAddrPtr
		{
			get { return fifoAddrPtr; }
			set
			{
				fifoAddrPtr = value;
				OnPropertyChanged("FifoAddrPtr");
			}
		}

		public byte FifoTxBaseAddr
		{
			get { return fifoTxBaseAddr; }
			set
			{
				fifoTxBaseAddr = value;
				OnPropertyChanged("FifoTxBaseAddr");
			}
		}

		public byte FifoRxBaseAddr
		{
			get { return fifoRxBaseAddr; }
			set
			{
				fifoRxBaseAddr = value;
				OnPropertyChanged("FifoRxBaseAddr");
			}
		}

		public byte FifoRxCurrentAddr
		{
			get { return fifoRxCurrentAddr; }
		}

		public bool RxTimeoutMask
		{
			get { return rxTimeoutMask; }
			set
			{
				rxTimeoutMask = value;
				OnPropertyChanged("RxTimeoutMask");
			}
		}

		public bool RxDoneMask
		{
			get { return rxDoneMask; }
			set
			{
				rxDoneMask = value;
				OnPropertyChanged("RxDoneMask");
			}
		}

		public bool PayloadCrcErrorMask
		{
			get { return payloadCrcErrorMask; }
			set
			{
				payloadCrcErrorMask = value;
				OnPropertyChanged("PayloadCrcErrorMask");
			}
		}

		public bool ValidHeaderMask
		{
			get { return validHeaderMask; }
			set
			{
				validHeaderMask = value;
				OnPropertyChanged("ValidHeaderMask");
			}
		}

		public bool TxDoneMask
		{
			get { return txDoneMask; }
			set
			{
				txDoneMask = value;
				OnPropertyChanged("TxDoneMask");
			}
		}

		public bool CadDoneMask
		{
			get { return cadDoneMask; }
			set
			{
				cadDoneMask = value;
				OnPropertyChanged("CadDoneMask");
			}
		}

		public bool FhssChangeChannelMask
		{
			get { return fhssChangeChannelMask; }
			set
			{
				fhssChangeChannelMask = value;
				OnPropertyChanged("FhssChangeChannelMask");
			}
		}

		public bool CadDetectedMask
		{
			get { return cadDetectedMask; }
			set
			{
				cadDetectedMask = value;
				OnPropertyChanged("CadDetectedMask");
			}
		}

		public bool RxTimeout
		{
			get { return rxTimeout; }
		}

		public bool RxDone
		{
			get { return rxDone; }
		}

		public bool PayloadCrcError
		{
			get { return payloadCrcError; }
		}

		public bool ValidHeader
		{
			get { return validHeader; }
		}

		public bool TxDone
		{
			get { return txDone; }
		}

		public bool CadDone
		{
			get { return cadDone; }
		}

		public bool FhssChangeChannel
		{
			get { return fhssChangeChannel; }
		}

		public bool CadDetected
		{
			get { return cadDetected; }
		}

		public byte RxNbBytes
		{
			get { return rxNbBytes; }
		}

		public ushort ValidHeaderCnt
		{
			get { return validHeaderCnt; }
		}

		public ushort ValidPacketCnt
		{
			get { return validPacketCnt; }
		}

		public byte RxPayloadCodingRate
		{
			get { return rxPayloadCodingRate; }
		}

		public bool ModemClear
		{
			get { return modemClear; }
		}

		public bool HeaderInfoValid
		{
			get { return headerInfoValid; }
		}

		public bool RxOnGoing
		{
			get { return rxOnGoing; }
		}

		public bool SignalSynchronized
		{
			get { return signalSynchronized; }
		}

		public bool SignalDetected
		{
			get { return signalDetected; }
		}

		public sbyte PktSnrValue
		{
			get { return pktSnrValue; }
		}

		public Decimal PktRssiValue
		{
			get { return pktRssiValue; }
		}

		public Decimal RssiValue
		{
			get { return rssiValue; }
		}

		public bool PllTimeout
		{
			get { return pllTimeout; }
		}

		public bool RxPayloadCrcOn
		{
			get { return rxPayloadCrcOn; }
		}

		public byte HopChannel
		{
			get { return hopChannel; }
		}

		public byte Bandwidth
		{
			get { return bandwidth; }
			set
			{
				bandwidth = value;
				OnPropertyChanged("Bandwidth");
				BandwidthCheck(value);
			}
		}

		public byte CodingRate
		{
			get { return codingRate; }
			set
			{
				codingRate = value;
				OnPropertyChanged("CodingRate");
			}
		}

		public bool ImplicitHeaderModeOn
		{
			get { return implicitHeaderModeOn; }
			set
			{
				implicitHeaderModeOn = value;
				OnPropertyChanged("ImplicitHeaderModeOn");
			}
		}

		public byte SpreadingFactor
		{
			get { return spreadingFactor; }
			set
			{
				spreadingFactor = value;
				OnPropertyChanged("SpreadingFactor");
			}
		}

		public bool TxContinuousModeOn
		{
			get { return txContinuousModeOn; }
			set
			{
				txContinuousModeOn = value;
				OnPropertyChanged("TxContinuousModeOn");
			}
		}

		public bool PayloadCrcOn
		{
			get { return payloadCrcOn; }
			set
			{
				payloadCrcOn = value;
				OnPropertyChanged("PayloadCrcOn");
			}
		}

		public Decimal SymbTimeout
		{
			get { return symbTimeout; }
			set
			{
				symbTimeout = value;
				OnPropertyChanged("SymbTimeout");
			}
		}

		public ushort PreambleLength
		{
			get { return preambleLength; }
			set
			{
				preambleLength = value;
				OnPropertyChanged("PreambleLength");
			}
		}

		public byte PayloadLength
		{
			get
			{
				if (Mode == OperatingModeEnum.Rx || !PacketModeTx)
					return payloadLengthRx;
				return (byte)(0U + (uint)(byte)Payload.Length);
			}
			set
			{
				if (Mode == OperatingModeEnum.Rx || !PacketModeTx)
					payloadLengthRx = value;
				else
					payloadLength = value;
				OnPropertyChanged("PayloadLength");
			}
		}

		public byte PayloadMaxLength
		{
			get
			{
				return payloadMaxLength;
			}
			set
			{
				payloadMaxLength = value;
				OnPropertyChanged("PayloadMaxLength");
			}
		}

		public byte FreqHoppingPeriod
		{
			get
			{
				return freqHoppingPeriod;
			}
			set
			{
				freqHoppingPeriod = value;
				OnPropertyChanged("FreqHoppingPeriod");
			}
		}

		public bool LowDatarateOptimize
		{
			get
			{
				return lowDatarateOptimize;
			}
			set
			{
				lowDatarateOptimize = value;
				OnPropertyChanged("LowDatarateOptimize");
			}
		}

		public bool AgcAutoOn
		{
			get
			{
				return agcAutoOn;
			}
			set
			{
				agcAutoOn = value;
				OnPropertyChanged("AgcAutoOn");
			}
		}

		public DioMappingEnum Dio0Mapping
		{
			get
			{
				return dio0Mapping;
			}
			set
			{
				dio0Mapping = value;
				OnPropertyChanged("Dio0Mapping");
			}
		}

		public DioMappingEnum Dio1Mapping
		{
			get
			{
				return dio1Mapping;
			}
			set
			{
				dio1Mapping = value;
				OnPropertyChanged("Dio1Mapping");
			}
		}

		public DioMappingEnum Dio2Mapping
		{
			get
			{
				return dio2Mapping;
			}
			set
			{
				dio2Mapping = value;
				OnPropertyChanged("Dio2Mapping");
			}
		}

		public DioMappingEnum Dio3Mapping
		{
			get
			{
				return dio3Mapping;
			}
			set
			{
				dio3Mapping = value;
				OnPropertyChanged("Dio3Mapping");
			}
		}

		public DioMappingEnum Dio4Mapping
		{
			get
			{
				return dio4Mapping;
			}
			set
			{
				dio4Mapping = value;
				OnPropertyChanged("Dio4Mapping");
			}
		}

		public DioMappingEnum Dio5Mapping
		{
			get
			{
				return dio5Mapping;
			}
			set
			{
				dio5Mapping = value;
				OnPropertyChanged("Dio5Mapping");
			}
		}

		public Version Version
		{
			get
			{
				return version;
			}
			set
			{
				if (!(version != value))
					return;
				version = value;
				OnPropertyChanged("Version");
			}
		}

		public double BandwidthHz
		{
			get
			{
				switch (Bandwidth)
				{
					case (byte)0:
						return 7812.5;
					case (byte)1:
						return 10416.7;
					case (byte)2:
						return 15625.0;
					case (byte)3:
						return 20833.3;
					case (byte)4:
						return 31250.0;
					case (byte)5:
						return 41666.7;
					case (byte)6:
						return 62500.0;
					case (byte)8:
						return 250000.0;
					case (byte)9:
						return 500000.0;
					default:
						return 125000.0;
				}
			}
		}

		public int AgcReference
		{
			get
			{
				return (int)Math.Round(10.0 * Math.Log10(2.0 * BandwidthHz) - 174.0 + (double)AgcReferenceLevel, MidpointRounding.AwayFromZero);
			}
		}

		public int AgcThresh1
		{
			get
			{
				return AgcReference + (int)agcStep1;
			}
		}

		public int AgcThresh2
		{
			get
			{
				return AgcThresh1 + (int)agcStep2;
			}
		}

		public int AgcThresh3
		{
			get
			{
				return AgcThresh2 + (int)agcStep3;
			}
		}

		public int AgcThresh4
		{
			get
			{
				return AgcThresh3 + (int)agcStep4;
			}
		}

		public int AgcThresh5
		{
			get
			{
				return AgcThresh4 + (int)agcStep5;
			}
		}

		public byte AgcReferenceLevel
		{
			get
			{
				return agcReferenceLevel;
			}
			set
			{
				agcReferenceLevel = value;
				OnPropertyChanged("AgcReferenceLevel");
				OnPropertyChanged("AgcReference");
				OnPropertyChanged("AgcThresh1");
				OnPropertyChanged("AgcThresh2");
				OnPropertyChanged("AgcThresh3");
				OnPropertyChanged("AgcThresh4");
				OnPropertyChanged("AgcThresh5");
			}
		}

		public byte AgcStep1
		{
			get
			{
				return agcStep1;
			}
			set
			{
				agcStep1 = value;
				OnPropertyChanged("AgcStep1");
				OnPropertyChanged("AgcThresh1");
				OnPropertyChanged("AgcThresh2");
				OnPropertyChanged("AgcThresh3");
				OnPropertyChanged("AgcThresh4");
				OnPropertyChanged("AgcThresh5");
			}
		}

		public byte AgcStep2
		{
			get
			{
				return agcStep2;
			}
			set
			{
				agcStep2 = value;
				OnPropertyChanged("AgcStep2");
				OnPropertyChanged("AgcThresh2");
				OnPropertyChanged("AgcThresh3");
				OnPropertyChanged("AgcThresh4");
				OnPropertyChanged("AgcThresh5");
			}
		}

		public byte AgcStep3
		{
			get
			{
				return agcStep3;
			}
			set
			{
				agcStep3 = value;
				OnPropertyChanged("AgcStep3");
				OnPropertyChanged("AgcThresh3");
				OnPropertyChanged("AgcThresh4");
				OnPropertyChanged("AgcThresh5");
			}
		}

		public byte AgcStep4
		{
			get
			{
				return agcStep4;
			}
			set
			{
				agcStep4 = value;
				OnPropertyChanged("AgcStep4");
				OnPropertyChanged("AgcThresh4");
				OnPropertyChanged("AgcThresh5");
			}
		}

		public byte AgcStep5
		{
			get
			{
				return agcStep5;
			}
			set
			{
				agcStep5 = value;
				OnPropertyChanged("AgcStep5");
				OnPropertyChanged("AgcThresh5");
			}
		}

		public bool FastHopOn
		{
			get
			{
				return fastHopOn;
			}
			set
			{
				fastHopOn = value;
				OnPropertyChanged("FastHopOn");
			}
		}

		public bool TcxoInputOn
		{
			get
			{
				return tcxoInputOn;
			}
			set
			{
				tcxoInputOn = value;
				OnPropertyChanged("TcxoInputOn");
			}
		}

		public bool Pa20dBm
		{
			get
			{
				return pa20dBm;
			}
			set
			{
				pa20dBm = value;
				OnPropertyChanged("Pa20dBm");
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
				pllBandwidth = value;
				OnPropertyChanged("PllBandwidth");
			}
		}

		public RfPaSwitchSelEnum RfPaSwitchSel
		{
			get
			{
				return rfPaSwitchSel;
			}
			set
			{
				lock (syncThread)
				{
					try
					{
						rfPaSwitchSel = value;
						switch (value)
						{
							default:
								OnPropertyChanged("RfPaSwitchSel");
								break;
						}
					}
					catch (Exception exception_0)
					{
						OnError(1, exception_0.Message);
					}
				}
			}
		}

		public int RfPaSwitchEnabled
		{
			get
			{
				return rfPaSwitchEnabled;
			}
			set
			{
				lock (syncThread)
				{
					try
					{
						prevRfPaSwitchEnabled = rfPaSwitchEnabled;
						rfPaSwitchEnabled = value;
						if (prevRfPaSwitchEnabled != rfPaSwitchEnabled)
						{
							if (rfPaSwitchEnabled == 2)
								prevRfPaSwitchSel = rfPaSwitchSel;
							else
								rfPaSwitchSel = prevRfPaSwitchSel;
						}
						int temp_14 = rfPaSwitchEnabled;
						OnPropertyChanged("RfPaSwitchEnabled");
						OnPropertyChanged("RfPaSwitchSel");
					}
					catch (Exception exception_0)
					{
						OnError(1, exception_0.Message);
					}
				}
			}
		}

		public Decimal RfPaRssiValue
		{
			get
			{
				return rfPaRssiValue;
			}
		}

		public Decimal RfIoRssiValue
		{
			get
			{
				return rfIoRssiValue;
			}
		}

		public byte[] Payload
		{
			get
			{
				return payload;
			}
			set
			{
				payload = value;
				OnPropertyChanged("Payload");
			}
		}

		public bool LogEnabled
		{
			get
			{
				return logEnabled;
			}
			set
			{
				logEnabled = value;
				OnPropertyChanged("LogEnabled");
			}
		}

		public bool IsPacketHandlerStarted
		{
			get
			{
				return isPacketHandlerStarted;
			}
		}

		private void PacketHandlerLog_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "Enabled":
					LogEnabled = PacketHandlerLog.Enabled;
					break;
			}
		}

		private void OnDio0Changed(bool state)
		{
			if (Dio0Changed != null)
				Dio0Changed(this, new SX1276LR.IoChangedEventArgs(state));
		}

		private void OnDio1Changed(bool state)
		{
			if (Dio1Changed != null)
				Dio1Changed(this, new SX1276LR.IoChangedEventArgs(state));
		}

		private void OnDio2Changed(bool state)
		{
			if (Dio2Changed != null)
				Dio2Changed(this, new SX1276LR.IoChangedEventArgs(state));
		}

		private void OnDio3Changed(bool state)
		{
			if (Dio3Changed != null)
				Dio3Changed(this, new SX1276LR.IoChangedEventArgs(state));
		}

		private void OnDio4Changed(bool state)
		{
			if (Dio4Changed != null)
				Dio4Changed(this, new SX1276LR.IoChangedEventArgs(state));
		}

		private void OnDio5Changed(bool state)
		{
			if (Dio5Changed != null)
				Dio5Changed(this, new SX1276LR.IoChangedEventArgs(state));
		}

		private void PopulateRegisters()
		{
			if (IsOpen)
			{
				SetLoraOn(true);
				byte data = 0;
				if (!Read(66, ref data))
					throw new Exception("Unable to read register RegVersion");
				if (!Read(66, ref data))
					throw new Exception("Unable to read register RegVersion");
				Version = new Version(((int)data & 0xF0) >> 4, (int)data & 0xF);
			}

			registers = new RegisterCollection();
			registers.Add(new Register("RegFifo", 0, 0, true, true));
			registers.Add(new Register("RegOpMode", 1, 137, false, true));
			for (uint index = 2; index < 4; ++index)
				registers.Add(new Register("RegRes" + index.ToString("X02"), index, 0, false, true));
			registers.Add(new Register("RegFdevMsb", 4, 0, false, true));
			registers.Add(new Register("RegRes05", 5, 82, false, true));
			registers.Add(new Register("RegFrfMsb", 6, 228, false, true));
			registers.Add(new Register("RegFrfMid", 7, 192, false, true));
			registers.Add(new Register("RegFrfLsb", 8, 9, false, true));
			registers.Add(new Register("RegPaConfig", 9, 15, false, true));
			registers.Add(new Register("RegPaRamp", 10, 25, false, true));
			registers.Add(new Register("RegOcp", 11, 43, false, true));
			registers.Add(new Register("RegLna", 12, 32, false, true));
			registers.Add(new Register("RegFifoAddrPtr", 13, 0, false, true));
			registers.Add(new Register("RegFifoTxBaseAddr", 14, 128, false, true));
			registers.Add(new Register("RegFifoRxBaseAddr", 15, 0, false, true));
			registers.Add(new Register("RegFifoRxCurrentAddr", 16, 0, true, true));
			registers.Add(new Register("RegIrqFlagsMask", 17, 0, false, true));
			registers.Add(new Register("RegIrqFlags", 18, 0, false, true));
			registers.Add(new Register("RegRxNbBytes", 19, 0, true, true));
			registers.Add(new Register("RegRxHeaderCntValueMsb", 20, 0, true, true));
			registers.Add(new Register("RegRxHeaderCntValueLsb", 21, 0, true, true));
			registers.Add(new Register("RegRxPacketCntValueMsb", 22, 0, true, true));
			registers.Add(new Register("RegRxPacketCntValueLsb", 23, 0, true, true));
			registers.Add(new Register("RegModemStat", 24, 0, true, true));
			registers.Add(new Register("RegPktSnrValue", 25, 26, true, true));
			registers.Add(new Register("RegPktRssiValue", 26, 0, true, true));
			registers.Add(new Register("RegRssiValue", 27, 0, true, true));
			registers.Add(new Register("RegHopChannel", 28, 0, true, true));
			registers.Add(new Register("RegModemConfig1", 29, 114, false, true));
			registers.Add(new Register("RegModemConfig2", 30, 112, false, true));
			registers.Add(new Register("RegSymbTimeoutLsb", 31, 100, false, true));
			registers.Add(new Register("RegPreambleMsb", 32, 0, false, true));
			registers.Add(new Register("RegPreambleLsb", 33, 8, false, true));
			registers.Add(new Register("RegPayloadLength", 34, 1, false, true));
			registers.Add(new Register("RegMaxPayloadLength", 35, 255, false, true));
			registers.Add(new Register("RegHopPeriod", 36, 0, false, true));
			registers.Add(new Register("RegFifoRxByteAddr", 37, 0, false, true));
			registers.Add(new Register("RegModemConfig3", 38, 4, false, true));
			for (uint index = 39; index < 64; ++index)
				registers.Add(new Register("RegTest" + index.ToString("X02"), index, 0, false, true));

			registers.Add(new Register("RegDioMapping1", 64, 0, false, true));
			registers.Add(new Register("RegDioMapping2", 65, 0, false, true));
			registers.Add(new Register("RegVersion", 66, 17, false, true));
			for (int index = 67; index < 68; ++index)
				registers.Add(new Register("RegTest" + index.ToString("X02"), (uint)index, 0, false, true));

			registers.Add(new Register("RegPllHop", 68, 45, false, true));
			for (uint index = 69; index < 75; ++index)
				registers.Add(new Register("RegTest" + index.ToString("X02"), index, 0, false, true));

			registers.Add(new Register("RegTcxo", 75, 9, false, true));
			registers.Add(new Register("RegTest4C", 76, 0, false, true));
			registers.Add(new Register("RegPaDac", 77, 132, false, true));
			for (uint index = 78; index < 91; ++index)
				registers.Add(new Register("RegTest" + index.ToString("X02"), index, 0, false, true));

			registers.Add(new Register("RegFormerTemp", 91, 0, false, true));
			registers.Add(new Register("RegTest5C", 92, 0, false, true));
			registers.Add(new Register("RegBitrateFrac", 93, 0, false, true));
			for (uint index = 94; index < 97; ++index)
				registers.Add(new Register("RegTest" + index.ToString("X02"), index, 0, false, true));

			registers.Add(new Register("RegAgcRef", 97, 25, false, true));
			registers.Add(new Register("RegAgcThresh1", 98, 12, false, true));
			registers.Add(new Register("RegAgcThresh2", 99, 75, false, true));
			registers.Add(new Register("RegAgcThresh3", 100, 204, false, true));
			for (uint index = 101; index < 112; ++index)
				registers.Add(new Register("RegTest" + index.ToString("X02"), index, 0, false, true));

			registers.Add(new Register("RegPll", 112, 208, false, true));
			for (uint index = 113; index < 128; ++index)
				registers.Add(new Register("RegTest" + index.ToString("X02"), index, 0, false, true));

			foreach (Register register in registers)
				register.PropertyChanged += new PropertyChangedEventHandler(registers_PropertyChanged);
		}

		private void UpdateRegisterTable()
		{
			if (lowFrequencyMode == LowFrequencyModeOn)
				return;
			lowFrequencyMode = LowFrequencyModeOn;
			ReadRegisters();
		}

		private void UpdateRegisterValue(Register r)
		{
			switch (r.Name)
			{
				case "RegOpMode":
					LowFrequencyModeOn = ((int)(r.Value >> 3) & 1) == 1;
					Console.WriteLine("RegOpMode (URV): 0x{0}", r.Value.ToString("X02"));
					Mode = (registers["RegModemConfig2"].Value & 8) != 8 || (r.Value & 7) != 3
						? (OperatingModeEnum)((int)r.Value & 7)
						: OperatingModeEnum.TxContinuous;
					if (registers["RegPayloadLength"].Value != (uint)PayloadLength)
						registers["RegPayloadLength"].Value = (uint)PayloadLength;
					lock (syncThread)
					{
						SetModeLeds(Mode);
						break;
					}
				case "RegFdevMsb":
					Band = (BandEnum)((registers["RegFdevMsb"].Value >> 6) & 3);
					break;
				case "RegFrfMsb":
				case "RegFrfMid":
				case "RegFrfLsb":
					FrequencyRf = (Decimal)(registers["RegFrfMsb"].Value << 16 | registers["RegFrfMid"].Value << 8 | registers["RegFrfLsb"].Value) * FrequencyStep;
					BandwidthCheck(Bandwidth);
					break;
				case "RegPaConfig":
					PaSelect = (r.Value & 128) == 128 ? PaSelectEnum.PA_BOOST : PaSelectEnum.RFO;
					if (PaSelect == PaSelectEnum.RFO)
					{
						maxOutputPower = new Decimal(108, 0, 0, false, 1) + new Decimal(6, 0, 0, false, 1) * (Decimal)(r.Value >> 4 & 7U);
						outputPower = MaxOutputPower - new Decimal(150, 0, 0, false, 1) - (Decimal)(r.Value & 15U);
					}
					else if (!Pa20dBm)
					{
						maxOutputPower = new Decimal(17);
						outputPower = new Decimal(17) - new Decimal(150, 0, 0, false, 1) - (Decimal)(r.Value & 15U);
					}
					else
					{
						maxOutputPower = new Decimal(20);
						outputPower = new Decimal(20) - new Decimal(150, 0, 0, false, 1) - (Decimal)(r.Value & 15U);
					}
					OnPropertyChanged("MaxOutputPower");
					OnPropertyChanged("OutputPower");
					break;
				case "RegPaRamp":
					ForceTxBandLowFrequencyOn = ((int)(r.Value >> 4) & 1) == 1;
					PaRamp = (PaRampEnum)((int)r.Value & 15);
					break;
				case "RegOcp":
					OcpOn = ((int)(r.Value >> 5) & 1) == 1;
					OcpTrim = (r.Value & 31U) > 15U ? ((r.Value & 31U) <= 15U || (r.Value & 31U) > 27U ? new Decimal(2400, 0, 0, false, 1) : (Decimal)(-30L + (long)(uint)(10 * ((int)r.Value & 31)))) : (Decimal)((uint)(45 + 5 * ((int)r.Value & 15)));
					if (!OcpOn)
						break;
					OcpTrimCheck(OcpTrim);
					break;
				case "RegLna":
					LnaGain = (LnaGainEnum)((int)(r.Value >> 5) & 7);
					ForceRxBandLowFrequencyOn = ((int)(r.Value >> 2) & 1) == 1;
					LnaBoost = ((int)r.Value & 3) == 3;
					if (!isDebugOn || !isReceiving)
						break;
					pktLnaValues.Add((int)LnaGain);
					break;
				case "RegFifoAddrPtr":
					FifoAddrPtr = (byte)r.Value;
					break;
				case "RegFifoTxBaseAddr":
					FifoTxBaseAddr = (byte)r.Value;
					break;
				case "RegFifoRxBaseAddr":
					FifoRxBaseAddr = (byte)r.Value;
					break;
				case "RegFifoRxCurrentAddr":
					fifoRxCurrentAddr = (byte)r.Value;
					OnPropertyChanged("FifoRxCurrentAddr");
					break;
				case "RegIrqFlagsMask":
					RxTimeoutMask = ((int)(r.Value >> 7) & 1) == 1;
					RxDoneMask = ((int)(r.Value >> 6) & 1) == 1;
					PayloadCrcErrorMask = ((int)(r.Value >> 5) & 1) == 1;
					ValidHeaderMask = ((int)(r.Value >> 4) & 1) == 1;
					TxDoneMask = ((int)(r.Value >> 3) & 1) == 1;
					CadDoneMask = ((int)(r.Value >> 2) & 1) == 1;
					FhssChangeChannelMask = ((int)(r.Value >> 1) & 1) == 1;
					CadDetectedMask = ((int)r.Value & 1) == 1;
					break;
				case "RegIrqFlags":
					rxTimeout = ((int)(r.Value >> 7) & 1) == 1;
					OnPropertyChanged("RxTimeout");
					rxDone = ((int)(r.Value >> 6) & 1) == 1;
					OnPropertyChanged("RxDone");
					payloadCrcError = ((int)(r.Value >> 5) & 1) == 1;
					OnPropertyChanged("PayloadCrcError");
					validHeader = ((int)(r.Value >> 4) & 1) == 1;
					OnPropertyChanged("ValidHeader");
					txDone = ((int)(r.Value >> 3) & 1) == 1;
					OnPropertyChanged("TxDone");
					cadDone = ((int)(r.Value >> 2) & 1) == 1;
					OnPropertyChanged("CadDone");
					fhssChangeChannel = ((int)(r.Value >> 1) & 1) == 1;
					OnPropertyChanged("FhssChangeChannel");
					cadDetected = ((int)r.Value & 1) == 1;
					OnPropertyChanged("CadDetected");
					if (isDebugOn && ValidHeader)
						isReceiving = true;
					if (!isDebugOn || !RxDone)
						break;
					isReceiving = false;
					break;
				case "RegRxNbBytes":
					rxNbBytes = (byte)r.Value;
					OnPropertyChanged("RxNbBytes");
					break;
				case "RegRxHeaderCntValueMsb":
				case "RegRxHeaderCntValueLsb":
					validHeaderCnt = (ushort)(registers["RegRxHeaderCntValueMsb"].Value << 8 | registers["RegRxHeaderCntValueLsb"].Value);
					OnPropertyChanged("ValidHeaderCnt");
					break;
				case "RegRxPacketCntValueMsb":
				case "RegRxPacketCntValueLsb":
					validPacketCnt = (ushort)(registers["RegRxPacketCntValueMsb"].Value << 8 | registers["RegRxPacketCntValueLsb"].Value);
					OnPropertyChanged("ValidPacketCnt");
					break;
				case "RegModemStat":
					rxPayloadCodingRate = (byte)((r.Value & 224U) >> 5);
					OnPropertyChanged("RxPayloadCodingRate");
					modemClear = ((int)(r.Value >> 4) & 1) == 1;
					OnPropertyChanged("ModemClear");
					headerInfoValid = ((int)(r.Value >> 3) & 1) == 1;
					OnPropertyChanged("HeaderInfoValid");
					rxOnGoing = ((int)(r.Value >> 2) & 1) == 1;
					OnPropertyChanged("RxOnGoing");
					signalSynchronized = ((int)(r.Value >> 1) & 1) == 1;
					OnPropertyChanged("SignalSynchronized");
					signalDetected = ((int)r.Value & 1) == 1;
					OnPropertyChanged("SignalDetected");
					break;
				case "RegPktSnrValue":
					byte num = (byte)r.Value;
					if ((num & 0x80) == 0x80)
					{
						// pktSnrValue = (sbyte)(((int)~num + 1 & (int)byte.MaxValue) >> 2);
						// pktSnrValue = -pktSnrValue;
						pktSnrValue = (sbyte)(((~num + 1) & 0xFF) >> 2);
						pktSnrValue = (sbyte)(-pktSnrValue);
					}
					else
						pktSnrValue = (sbyte)((num & 0xFF) >> 2);

					if (!isDebugOn)
					{
						pktRssiValue = (int)pktSnrValue >= 0 ? (!LowFrequencyModeOn ? (Decimal)(-157L + (long)registers["RegPktRssiValue"].Value + (long)(registers["RegPktRssiValue"].Value >> 4)) : (Decimal)(-164L + (long)registers["RegPktRssiValue"].Value + (long)(registers["RegPktRssiValue"].Value >> 4))) : (!LowFrequencyModeOn ? (Decimal)(-157L + (long)registers["RegPktRssiValue"].Value + (long)(registers["RegPktRssiValue"].Value >> 4) + (long)pktSnrValue) : (Decimal)(-164L + (long)registers["RegPktRssiValue"].Value + (long)(registers["RegPktRssiValue"].Value >> 4) + (long)pktSnrValue));
						OnPropertyChanged("PktRssiValue");
					}
					OnPropertyChanged("PktSnrValue");
					break;
				case "RegPktRssiValue":
					if (!isDebugOn)
					{
						if ((int)pktSnrValue < 0)
							break;
						pktRssiValue = !LowFrequencyModeOn ? (Decimal)(-157L + (long)r.Value + (long)(r.Value >> 4)) : (Decimal)(-164L + (long)r.Value + (long)(r.Value >> 4));
						OnPropertyChanged("PktRssiValue");
						break;
					}
					pktRssiValue = (Decimal)r.Value;
					OnPropertyChanged("PktRssiValue");
					break;
				case "RegRssiValue":
					prevRssiValue = rssiValue;
					rssiValue = isDebugOn ? (Decimal)r.Value : (!LowFrequencyModeOn ? (Decimal)(-157L + (long)r.Value) : (Decimal)(-164L + (long)r.Value));
					if (RfPaSwitchEnabled != 0)
					{
						if (RfPaSwitchSel == RfPaSwitchSelEnum.RF_IO_RFIO)
						{
							if (RfPaSwitchEnabled == 1)
								rfPaRssiValue = new Decimal(1277, 0, 0, true, 1);
							rfIoRssiValue = rssiValue;
							OnPropertyChanged("RfIoRssiValue");
						}
						else if (RfPaSwitchSel == RfPaSwitchSelEnum.RF_IO_PA_BOOST)
						{
							if (RfPaSwitchEnabled == 1)
								rfIoRssiValue = new Decimal(1277, 0, 0, true, 1);
							rfPaRssiValue = rssiValue;
							OnPropertyChanged("RfPaRssiValue");
						}
					}
					spectrumRssiValue = rssiValue;
					if (isDebugOn && isReceiving)
						pktRssiValues.Add((double)rssiValue);
					OnPropertyChanged("RssiValue");
					OnPropertyChanged("SpectrumData");
					break;
				case "RegHopChannel":
					pllTimeout = ((int)(r.Value >> 7) & 1) == 1;
					OnPropertyChanged("PllTimeout");
					rxPayloadCrcOn = ((int)(r.Value >> 6) & 1) == 1;
					OnPropertyChanged("RxPayloadCrcOn");
					hopChannel = (byte)(r.Value & 63U);
					OnPropertyChanged("HopChannel");
					break;
				case "RegModemConfig1":
					Bandwidth = (byte)(r.Value >> 4 & 15U);
					CodingRate = (byte)(r.Value >> 1 & 7U);
					ImplicitHeaderModeOn = ((int)r.Value & 1) == 1;
					OnPropertyChanged("SymbolRate");
					OnPropertyChanged("SymbolTime");
					break;
				case "RegModemConfig2":
					SpreadingFactor = (byte)(r.Value >> 4 & 15U);
					OnPropertyChanged("SymbolRate");
					OnPropertyChanged("SymbolTime");
					TxContinuousModeOn = ((int)(r.Value >> 3) & 1) == 1;
					PayloadCrcOn = ((int)(r.Value >> 2) & 1) == 1;
					SymbTimeout = (Decimal)((uint)(((int)r.Value & 3) << 8) | registers["RegSymbTimeoutLsb"].Value) * SymbolTime;
					break;
				case "RegSymbTimeoutLsb":
					SymbTimeout = (Decimal)((uint)(((int)registers["RegModemConfig2"].Value & 3) << 8) | registers["RegSymbTimeoutLsb"].Value) * SymbolTime;
					break;
				case "RegPreambleMsb":
				case "RegPreambleLsb":
					PreambleLength = (ushort)(((int)registers["RegPreambleMsb"].Value << 8 | (int)registers["RegPreambleLsb"].Value) + 4);
					break;
				case "RegPayloadLength":
					PayloadLength = (byte)r.Value;
					break;
				case "RegPayloadMaxLength":
					PayloadMaxLength = (byte)r.Value;
					break;
				case "RegHopPeriod":
					FreqHoppingPeriod = (byte)r.Value;
					break;
				case "RegModemConfig3":
					LowDatarateOptimize = ((int)(r.Value >> 3) & 1) == 1;
					AgcAutoOn = ((int)(r.Value >> 2) & 1) == 1;
					break;
				case "RegDioMapping1":
					Dio0Mapping = (DioMappingEnum)((int)(r.Value >> 6) & 3);
					Dio1Mapping = (DioMappingEnum)((int)(r.Value >> 4) & 3);
					Dio2Mapping = (DioMappingEnum)((int)(r.Value >> 2) & 3);
					Dio3Mapping = (DioMappingEnum)((int)r.Value & 3);
					break;
				case "RegDioMapping2":
					Dio4Mapping = (DioMappingEnum)((int)(r.Value >> 6) & 3);
					Dio5Mapping = (DioMappingEnum)((int)(r.Value >> 4) & 3);
					break;
				case "RegVersion":
					Version = new Version((int)((r.Value & 240U) >> 4), (int)r.Value & 15);
					break;
				case "RegPllHop":
					FastHopOn = ((int)r.Value & 128) == 128;
					break;
				case "RegTcxo":
					TcxoInputOn = ((int)r.Value & 16) == 16;
					break;
				case "RegPaDac":
					Pa20dBm = ((int)r.Value & 7) == 7;
					ReadRegister(registers["RegPaConfig"]);
					break;
				case "RegAgcRef":
					AgcReferenceLevel = (byte)(r.Value & 63U);
					OnPropertyChanged("AgcReference");
					OnPropertyChanged("AgcThresh1");
					OnPropertyChanged("AgcThresh2");
					OnPropertyChanged("AgcThresh3");
					OnPropertyChanged("AgcThresh4");
					OnPropertyChanged("AgcThresh5");
					break;
				case "RegAgcThresh1":
					AgcStep1 = (byte)(r.Value & 31U);
					OnPropertyChanged("AgcThresh1");
					OnPropertyChanged("AgcThresh2");
					OnPropertyChanged("AgcThresh3");
					OnPropertyChanged("AgcThresh4");
					OnPropertyChanged("AgcThresh5");
					break;
				case "RegAgcThresh2":
					AgcStep2 = (byte)(r.Value >> 4);
					AgcStep3 = (byte)(r.Value & 15U);
					OnPropertyChanged("AgcThresh2");
					OnPropertyChanged("AgcThresh3");
					OnPropertyChanged("AgcThresh4");
					OnPropertyChanged("AgcThresh5");
					break;
				case "RegAgcThresh3":
					AgcStep4 = (byte)(r.Value >> 4);
					AgcStep5 = (byte)(r.Value & 15U);
					OnPropertyChanged("AgcThresh4");
					OnPropertyChanged("AgcThresh5");
					break;
				case "RegPll":
					PllBandwidth = (Decimal)((uint)(((int)(r.Value >> 6) + 1) * 75000));
					break;
			}
		}

		private void OcpTrimCheck(Decimal value)
		{
			if (Pa20dBm && value < new Decimal(150))
				OnOcpTrimLimitStatusChanged(LimitCheckStatusEnum.OUT_OF_RANGE, "The Overload current protection is out of range.\nThe valid range is:\n" + new string[1]
        {
          "[" + 150.ToString() + " mA, " + 240.ToString() + " mA]"
        }[0]);
			else
				OnOcpTrimLimitStatusChanged(LimitCheckStatusEnum.OK, "");
		}

		private void FrequencyRfCheck(Decimal value)
		{
			if (frequencyRfCheckDisable)
				return;
			if (value < new Decimal(137000000) || value > new Decimal(175000000) && value < new Decimal(410000000) || (value > new Decimal(525000000) && value < new Decimal(820000000) || value > new Decimal(1024000000)))
			{
				string[] strArray = new string[3]
        {
          "[" + 137000000.ToString() + ", " + 175000000.ToString() + "]",
          "[" + 410000000.ToString() + ", " + 525000000.ToString() + "]",
          "[" + 820000000.ToString() + ", " + 1024000000.ToString() + "]"
        };
				OnFrequencyRfLimitStatusChanged(LimitCheckStatusEnum.OUT_OF_RANGE, "The RF frequency is out of range.\nThe valid ranges are:\n" + strArray[0] + "\n" + strArray[1] + "\n" + strArray[2]);
			}
			else
				OnFrequencyRfLimitStatusChanged(LimitCheckStatusEnum.OK, "");
		}

		private void BandwidthCheck(byte value)
		{
			if (FrequencyRf < new Decimal(175000000) && (int)value >= 8)
				OnBandwidthLimitStatusChanged(LimitCheckStatusEnum.OUT_OF_RANGE, "This bandwidth setting is unsupported in lowest frequency band.");
			else
				OnBandwidthLimitStatusChanged(LimitCheckStatusEnum.OK, "");
		}

		private void OnConnected()
		{
			if (Connected != null)
				Connected(this, EventArgs.Empty);
		}

		private void OnDisconnected()
		{
			if (Disconected != null)
				Disconected(this, EventArgs.Empty);
		}

		private void OnError(byte status, string message)
		{
			if (Error != null)
				Error(this, new SemtechLib.General.Events.ErrorEventArgs(status, message));
		}

		private void OnOcpTrimLimitStatusChanged(LimitCheckStatusEnum status, string message)
		{
			if (OcpTrimLimitStatusChanged != null)
				OcpTrimLimitStatusChanged(this, new LimitCheckStatusEventArg(status, message));
		}

		private void OnFrequencyRfLimitStatusChanged(LimitCheckStatusEnum status, string message)
		{
			if (FrequencyRfLimitStatusChanged != null)
				FrequencyRfLimitStatusChanged(this, new LimitCheckStatusEventArg(status, message));
		}

		private void OnBandwidthLimitStatusChanged(LimitCheckStatusEnum status, string message)
		{
			if (BandwidthLimitStatusChanged != null)
				BandwidthLimitStatusChanged(this, new LimitCheckStatusEventArg(status, message));
		}

		public bool Open()
		{
			try
			{
				Close();
				return usbDevice.Open();
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			return false;
		}

		public bool Close()
		{
			if (isOpen || (usbDevice != null && usbDevice.IsOpen))
			{
				usbDevice.Close();
				isOpen = false;
			}
			return true;
		}

		/// <summary>
		///	[Status(U8)][Timeout(U32)]
		/// </summary>
		/// <returns></returns>
		public bool SKReset()
		{
			lock (syncThread)
			{
				byte command = (byte)HidCommands.HID_DEVICE_RESET;
				SX1276LR.HidCommandsStatus status = SX1276LR.HidCommandsStatus.SX_ERROR;
				ulong timeout = ulong.MaxValue;
				byte[] inData = new byte[10];
				byte[] outData = new byte[2];
				try
				{
					if (IsOpen)
					{
						usbDevice.TxRxCommand(command, outData, ref inData);
						status = (SX1276LR.HidCommandsStatus)inData[0];
						timeout = ((ulong)inData[1] << 56 | (ulong)inData[2] << 48 | (ulong)inData[3] << 40 | (ulong)inData[4] << 32 | (ulong)inData[5] << 24 | (ulong)inData[6] << 16 | (ulong)inData[7] << 8 | (ulong)inData[8]);
						if (status == SX1276LR.HidCommandsStatus.SX_OK)
							return true;
					}
					return false;
				}
				finally
				{
					Console.WriteLine("{0} ms: {1} with status {2}",
						timeout,
						Enum.GetName(typeof(SX1276LR.HidCommands), (SX1276LR.HidCommands)command),
						Enum.GetName(typeof(SX1276LR.HidCommandsStatus), (object)status)
						);
				}
			}
		}

		/// <summary>
		///	[Status(U8)][Timeout(U32)][Length(U8)][Version(U8 * Length)]
		/// </summary>
		/// <returns></returns>
		public bool SKGetVersion()
		{
			lock (syncThread)
			{
				byte command = (byte)HidCommands.HID_SK_GET_VERSION;
				SX1276LR.HidCommandsStatus status = SX1276LR.HidCommandsStatus.SX_ERROR;
				ulong timeout = ulong.MaxValue;
				byte[] inData = new byte[17];
				byte[] outData = new byte[2];
				try
				{
					usbDevice.TxRxCommand(command, outData, ref inData);
					status = (SX1276LR.HidCommandsStatus)inData[0];
					timeout = ((ulong)inData[1] << 56 | (ulong)inData[2] << 48 | (ulong)inData[3] << 40 | (ulong)inData[4] << 32 | (ulong)inData[5] << 24 | (ulong)inData[6] << 16 | (ulong)inData[7] << 8 | (ulong)inData[8]);
					if (status != SX1276LR.HidCommandsStatus.SX_OK || inData[9] < 5)
						return false;

					Array.Copy(inData, 10, inData, 0, inData[9]);
					Array.Resize<byte>(ref inData, inData[9]);
					string version = new ASCIIEncoding().GetString(inData);
					fwVersion = version.Length <= 5 ? new Version(version) : new Version(version.Remove(4, 1));
					return true;
				}
				finally
				{
					Console.WriteLine(
						"{0} ms: {1} with status {2}",
						timeout,
						Enum.GetName(typeof(SX1276LR.HidCommands), (SX1276LR.HidCommands)command),
						Enum.GetName(typeof(SX1276LR.HidCommandsStatus), status)
						);
				}
			}
		}

		/// <summary>
		///	[Status(U8)][Timeout(U32)][Length(U8 >= 9)][Name(U8 * Length)]
		/// </summary>
		/// <returns></returns>
		public bool SKGetName()
		{
			lock (syncThread)
			{
				byte command = (byte)HidCommands.HID_SK_GET_NAME;
				SX1276LR.HidCommandsStatus status = SX1276LR.HidCommandsStatus.SX_ERROR;
				ulong timeout = ulong.MaxValue;
				byte[] inData = new byte[25];
				byte[] outData = new byte[2];
				try
				{
					usbDevice.TxRxCommand(command, outData, ref inData);
					status = (SX1276LR.HidCommandsStatus)inData[0];
					timeout = ((ulong)inData[1] << 56 | (ulong)inData[2] << 48 | (ulong)inData[3] << 40 | (ulong)inData[4] << 32 | (ulong)inData[5] << 24 | (ulong)inData[6] << 16 | (ulong)inData[7] << 8 | (ulong)inData[8]);
					if (status == SX1276LR.HidCommandsStatus.SX_OK && inData[9] >= 9)
					{
						Array.Copy(inData, 10, inData, 0, 10);
						Array.Resize<byte>(ref inData, 9);
						deviceName = new ASCIIEncoding().GetString(inData);
						return true;
					}
					deviceName = string.Empty;
					return false;
				}
				finally
				{
					Console.WriteLine("{0} ms: {1} with status {2}", (object)timeout, (object)Enum.GetName(typeof(SX1276LR.HidCommands), (object)(SX1276LR.HidCommands)command), (object)Enum.GetName(typeof(SX1276LR.HidCommandsStatus), (object)status));
				}
			}
		}

		/// <summary>
		///	[Status(U8)][xx(U32)][Length(U8 == 1)][ID(U8)]
		/// </summary>
		/// <returns></returns>
		public bool SKGetId(ref byte id)
		{
			lock (syncThread)
			{
				byte command = (byte)HidCommands.HID_SK_GET_ID;
				byte[] inData = new byte[11];
				byte[] outData = new byte[2];
				usbDevice.TxRxCommand(command, outData, ref inData);
				SX1276LR.HidCommandsStatus status = (SX1276LR.HidCommandsStatus)inData[0];
				if (status != SX1276LR.HidCommandsStatus.SX_OK || inData[9] != 1)
					return false;
				id = inData[10];
				return true;
			}
		}

		/// <summary>
		/// [Command(U8)][0][1][ID]
		///	[Status(U8)][xx(U32)][Length(U8 == 1)][ID(U8)]
		/// </summary>
		/// <returns></returns>
		public bool SKSetId(byte id)
		{
			lock (syncThread)
			{
				byte command = (byte)HidCommands.HID_SK_SET_ID;
				byte[] inData = new byte[10];
				byte[] outData = new byte[3] { 0, 1, id };
				usbDevice.TxRxCommand(command, outData, ref inData);
				SX1276LR.HidCommandsStatus status = (SX1276LR.HidCommandsStatus)inData[0];
				return (status == SX1276LR.HidCommandsStatus.SX_OK);
			}
		}

		public bool SKSetRndId(ref byte id)
		{
			lock (syncThread)
			{
				byte command = (byte)HidCommands.HID_SK_SET_ID_RND;
				byte[] inData = new byte[11];
				byte[] outData = new byte[2];
				usbDevice.TxRxCommand(command, outData, ref inData);
				SX1276LR.HidCommandsStatus status = (SX1276LR.HidCommandsStatus)inData[0];
				if (status != SX1276LR.HidCommandsStatus.SX_OK || inData[9] != 1)
					return false;
				id = inData[10];
				return true;
			}
		}

		public bool SKGetPinState(byte pinId, ref byte state)
		{
			lock (syncThread)
			{
				byte command = (byte)HidCommands.HID_SK_GET_PIN;
				byte[] inData = new byte[11];
				byte[] outData = new byte[3] { 0, 1, pinId };
				usbDevice.TxRxCommand(command, outData, ref inData);
				SX1276LR.HidCommandsStatus status = (SX1276LR.HidCommandsStatus)inData[0];
				if (status != SX1276LR.HidCommandsStatus.SX_OK || inData[9] != 1)
					return false;
				state = inData[10];
				return true;
			}
		}

		public bool SKSetPinState(byte pinId, byte state)
		{
			lock (syncThread)
			{
				byte command = (byte)HidCommands.HID_SK_SET_PIN;
				byte[] inData = new byte[10];
				byte[] outData = new byte[4] { 0, 2, pinId, state };
				usbDevice.TxRxCommand(command, outData, ref inData);
				SX1276LR.HidCommandsStatus status = (SX1276LR.HidCommandsStatus)inData[0];
				return status == SX1276LR.HidCommandsStatus.SX_OK;
			}
		}

		public bool SKGetPinDir(byte pinId, ref byte dir)
		{
			lock (syncThread)
			{
				byte command = (byte)HidCommands.HID_SK_GET_DIR;
				byte[] inData = new byte[11];
				byte[] outData = new byte[3] { 0, 1, pinId };
				usbDevice.TxRxCommand(command, outData, ref inData);
				SX1276LR.HidCommandsStatus status = (SX1276LR.HidCommandsStatus)inData[0];
				if (status != SX1276LR.HidCommandsStatus.SX_OK || inData[9] != 1)
					return false;
				dir = inData[10];
				return true;
			}
		}

		public bool SKSetPinDir(byte pinId, byte dir)
		{
			lock (syncThread)
			{
				byte local_0 = (byte)HidCommands.HID_SK_SET_DIR;
				byte[] inData = new byte[10];
				byte[] outData = new byte[4] { 0, 2, pinId, dir };
				usbDevice.TxRxCommand(local_0, outData, ref inData);
				SX1276LR.HidCommandsStatus status = (SX1276LR.HidCommandsStatus)inData[0];
				return status == SX1276LR.HidCommandsStatus.SX_OK;
			}
		}

		public bool SKGetPinsState(ref byte pinsState)
		{
			lock (syncThread)
			{
				byte command = (byte)HidCommands.HID_SK_GET_PINS;
				byte[] inData = new byte[11];
				byte[] outData = new byte[2];
				usbDevice.TxRxCommand(command, outData, ref inData);
				SX1276LR.HidCommandsStatus status = (SX1276LR.HidCommandsStatus)inData[0];
				if (status != SX1276LR.HidCommandsStatus.SX_OK || inData[9] != 1)
					return false;
				pinsState = inData[10];
				return true;
			}
		}

		public bool SKReadEeprom(byte id, byte address, ref byte[] data)
		{
			lock (syncThread)
			{
				byte command = (byte)HidCommands.HID_EEPROM_READ;
				byte[] inData = new byte[42];
				byte[] outData = new byte[5] { 0, 3, 32, id, address };
				int index = (int)address;
				while (index < data.Length)
				{
					outData[4] = (byte)index;
					usbDevice.TxRxCommand(command, outData, ref inData);
					SX1276LR.HidCommandsStatus status = (SX1276LR.HidCommandsStatus)inData[0];
					if (status == SX1276LR.HidCommandsStatus.SX_OK)
					{
						if (32 == inData[9])
						{
							Array.Copy(inData, 10, data, index, 32);
						}
						else
						{
							data = null;
							return false;
						}
					}
					index += 32;
				}
				return true;
			}
		}

		public bool SKWriteEeprom(byte id, byte address, byte[] data)
		{
			lock (syncThread)
			{
				byte command = (byte)HidCommands.HID_EEPROM_WRITE;
				byte[] inData = new byte[10];
				byte[] outData = new byte[37];
				outData[0] = 0;
				outData[1] = 35;
				outData[2] = 32;
				outData[3] = id;
				int index = (int)address;
				while (index < data.Length)
				{
					outData[4] = (byte)index;
					Array.Copy(data, index, outData, 5, 32);
					usbDevice.TxRxCommand(command, outData, ref inData);
					SX1276LR.HidCommandsStatus status = (SX1276LR.HidCommandsStatus)inData[0];
					if (status != SX1276LR.HidCommandsStatus.SX_OK)
						return false;
					index += 32;
				}
				return true;
			}
		}

		public bool SKDeviceRead(byte address, ref byte[] data)
		{
			lock (syncThread)
			{
				byte command = (byte)HidCommands.HID_DEVICE_READ;
				byte[] inData = new byte[10 + data.Length];
				byte[] outData = new byte[4] { 0, (byte)(data.Length + 2), (byte)data.Length, address };
				usbDevice.TxRxCommand(command, outData, ref inData);
				SX1276LR.HidCommandsStatus status = (SX1276LR.HidCommandsStatus)inData[0];
				if (status != SX1276LR.HidCommandsStatus.SX_OK || inData[9] != data.Length)
					return false;
				Array.Copy(inData, 10, data, 0, data.Length);
				return true;
			}
		}

		public bool SKDeviceWrite(byte address, byte[] data)
		{
			lock (syncThread)
			{
				byte command = (byte)HidCommands.HID_DEVICE_WRITE;
				byte[] inData = new byte[10];
				byte[] outData = new byte[data.Length + 4];
				outData[0] = 0;
				outData[1] = (byte)(data.Length + 2);
				outData[2] = (byte)data.Length;
				outData[3] = address;
				Array.Copy(data, 0, outData, 4, data.Length);
				usbDevice.TxRxCommand(command, outData, ref inData);
				SX1276LR.HidCommandsStatus status = (SX1276LR.HidCommandsStatus)inData[0];
				return status == SX1276LR.HidCommandsStatus.SX_OK;
			}
		}

		public bool Write(byte address, byte data)
		{
			return SKDeviceWrite(address, new byte[1] { data });
		}

		public bool Write(byte address, byte[] data)
		{
			return SKDeviceWrite(address, data);
		}

		public bool Read(byte address, ref byte data)
		{
			byte[] data1 = new byte[1];
			if (!SKDeviceRead(address, ref data1))
				return false;
			data = data1[0];
			return true;
		}

		public bool Read(byte address, ref byte[] data)
		{
			return SKDeviceRead(address, ref data);
		}

		public void ProcessWinMessage(int msg, IntPtr wParam, IntPtr lParam)
		{
			usbDevice.ProcessWinMessage(msg, wParam, lParam);
		}

		public void SetNotificationWindowHandle(IntPtr handle, bool isWpfApplication)
		{
			usbDevice.RegisterNotification(handle, isWpfApplication);
		}

		public void OpenConfig(ref FileStream stream)
		{
			OnError(0, "-");
			StreamReader streamReader = new StreamReader(stream, Encoding.ASCII);
			int lineNumber = 1;
			string strPKT = "";
			try
			{
				string line;
				while ((line = streamReader.ReadLine()) != null)
				{
					if (line[0] == '#')
					{
						++lineNumber;
						continue;
					}

					if (line[0] != 'R' && line[0] != 'P' && line[0] != 'X')
						throw new Exception("At line " + lineNumber.ToString() + ": A configuration line must start either by\n\"#\" for comments\nor a\n\"R\" for the register name.\nor a\n\"P\" for packet settings.\nor a\n\"X\" for crystal frequency.");

					string[] parts = line.Split('\t');
					if (parts.Length != 4)
					{
						if (parts.Length != 2)
							throw new Exception("At line " + lineNumber.ToString() + ": The number of columns is " + parts.Length.ToString() + " and it should be 4 or 2.");

						if (parts[0] == "PKT")
						{
							strPKT = parts[1];
						}
						else
						{
							if (parts[0] != "XTAL")
								throw new Exception("At line " + lineNumber.ToString() + ": Invalid Packet or XTAL freuqncy");

							FrequencyXo = Convert.ToDecimal(parts[1], System.Globalization.CultureInfo.InvariantCulture);
						}
					}
					else
					{
						bool flag = true;
						for (int index = 0; index < registers.Count; ++index)
						{
							if (registers[index].Name == parts[1])
							{
								flag = false;
								break;
							}
						}
						if (flag)
							throw new Exception("At line " + lineNumber.ToString() + ": Invalid register name.");
						if (parts[1] != "RegVersion")
						{
							registers[parts[1]].Value = (uint)Convert.ToByte(parts[3], 16);
						}
					}
					++lineNumber;
				}

				string[] strArray1 = strPKT.Split(';');
				if (strArray1.Length > 1)
					PacketModeTx = bool.Parse(strArray1[1]);
				string[] strArray2 = strArray1[0].Split(',');
				if (payload != null)
					Array.Resize<byte>(ref payload, strArray2.Length);
				else
					payload = new byte[strArray2.Length];
				for (int index = 0; index < strArray2.Length; ++index)
				{
					if (strArray2[index].Length != 0)
						payload[index] = Convert.ToByte(strArray2[index], 16);
				}
				OnPropertyChanged("Payload");
				OnPropertyChanged("PayloadLength");
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				streamReader.Close();
				if (!IsOpen)
					ReadRegisters();
			}
		}

		public void SaveConfig(ref FileStream stream)
		{
			OnError(0, "-");
			StreamWriter streamWriter = new StreamWriter(stream, Encoding.ASCII);
			try
			{
				streamWriter.WriteLine("#Type\tRegister Name\tAddress[Hex]\tValue[Hex]");
				for (int index = 0; index < registers.Count; ++index)
					streamWriter.WriteLine("REG\t{0}\t0x{1}\t0x{2}", registers[index].Name, registers[index].Address.ToString("X02"), registers[index].Value.ToString("X02"));

				string str1 = "";
				if (Payload != null && Payload.Length != 0)
				{
					int index;
					for (index = 0; index < Payload.Length - 1; ++index)
						str1 = str1 + Payload[index].ToString("X02") + ",";
					str1 += Payload[index].ToString("X02");
				}
				string strPKT = str1 + ";" + PacketModeTx.ToString();
				streamWriter.WriteLine("PKT\t{0}", strPKT);
				streamWriter.WriteLine("XTAL\t{0}", FrequencyXo);
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				streamWriter.Close();
			}
		}

		public Version GetVersion()
		{
			byte data = 0;
			if (!Read(0x42, ref data))
				throw new Exception("Unable to read register RegVersion");
			if (!Read(0x42, ref data))
				throw new Exception("Unable to read register RegVersion");
			return new Version((data & 0xF0) >> 4, data & 0x0F);
		}

		public void Reset()
		{
			lock (syncThread)
			{
				try
				{
					bool spectrumOn = SpectrumOn;
					if (SpectrumOn)
						SpectrumOn = false;
					PacketHandlerStop();
					if (!SKReset())
						throw new Exception("Unable to reset the SK");
					ReadRegisters();
					Decimal freq = FrequencyRf;
					SetFrequencyRf(new Decimal(915000000));
					ImageCalStart();
					SetFrequencyRf(freq);
					ImageCalStart();
					SetLoraOn(true);
					SetDefaultValues();
					ReadRegisters();
					RfPaSwitchEnabled = 0;
					RfPaSwitchSel = RfPaSwitchSelEnum.RF_IO_RFIO;
					if (spectrumOn)
						SpectrumOn = true;
				}
				catch (Exception exception_0)
				{
					OnError(1, exception_0.Message);
				}
			}
		}

		public bool WriteRegister(Register r, byte data)
		{
			lock (syncThread)
			{
				try
				{
					++writeLock;
					if (!Write((byte)r.Address, data))
						throw new Exception("Unable to read register: " + r.Name);
					return true;
				}
				catch (Exception exception_0)
				{
					OnError(1, exception_0.Message);
					return false;
				}
				finally
				{
					--writeLock;
				}
			}
		}

		public void WriteRegisters()
		{
			lock (syncThread)
			{
				try
				{
					foreach (Register register in registers)
					{
						if (register.Address != 0 && !Write((byte)register.Address, (byte)register.Value))
							throw new Exception("Writing register " + register.Name);
					}
				}
				catch (Exception exception_0)
				{
					OnError(1, exception_0.Message);
				}
			}
		}

		public bool ReadRegister(Register r)
		{
			byte data = 0;
			return ReadRegister(r, ref data);
		}

		public bool ReadRegister(Register r, ref byte data)
		{
			lock (syncThread)
			{
				try
				{
					++readLock;
					if (IsOpen)
					{
						if (!Read((byte)r.Address, ref data))
							throw new Exception("Unable to read register: " + r.Name);
						r.Value = data;
					}
					else
						UpdateRegisterValue(r);
					return true;
				}
				catch (Exception exception_0)
				{
					OnError(1, exception_0.Message);
					return false;
				}
				finally
				{
					--readLock;
				}
			}
		}

		public void ReadRegisters()
		{
			lock (syncThread)
			{
				try
				{
					++readLock;
					foreach (Register register in registers)
					{
						if (register.Address != 0 && (register.Address != 1 || !isPacketHandlerStarted))
							ReadRegister(register);
					}
				}
				catch (Exception exception_0)
				{
					OnError(1, exception_0.Message);
				}
				finally
				{
					--readLock;
				}
			}
		}

		public void ReadRegisters(RegisterCollection regs)
		{
			lock (syncThread)
			{
				try
				{
					++readLock;
					foreach (Register register in regs)
					{
						if (register.Address != 0 && (register.Address != 1 || !isPacketHandlerStarted))
							ReadRegister(register);
					}
				}
				catch (Exception exception_0)
				{
					OnError(1, exception_0.Message);
				}
				finally
				{
					--readLock;
				}
			}
		}

		public bool WriteFifo(byte[] data)
		{
			byte[] array = new byte[32];
			int length = data.Length;
			int sourceIndex = 0;
			while (length > 0)
			{
				if (length > 32)
				{
					Array.Resize<byte>(ref array, 32);
					Array.Copy(data, sourceIndex, array, 0, 32);
					if (!Write(0, array))
						return false;
					length -= 32;
					sourceIndex += 32;
				}
				else
				{
					Array.Resize<byte>(ref array, length);
					Array.Copy(data, sourceIndex, array, 0, length);
					if (!Write(0, array))
						return false;
					length -= length;
					sourceIndex += length;
				}
			}
			return true;
		}

		private bool ReadFifo(ref byte[] data)
		{
			byte[] numArray = new byte[32];
			int length = data.Length;
			int destinationIndex = 0;
			while (length > 0)
			{
				if (length > 32)
				{
					Array.Resize<byte>(ref numArray, 32);
					if (!Read(0, ref numArray))
						return false;
					Array.Copy(numArray, 0, data, destinationIndex, 32);
					length -= 32;
					destinationIndex += 32;
				}
				else
				{
					Array.Resize<byte>(ref numArray, length);
					if (!Read(0, ref numArray))
						return false;
					Array.Copy(numArray, 0, data, destinationIndex, length);
					length -= length;
					destinationIndex += length;
				}
			}
			return true;
		}

		public void ReadIrqFlags()
		{
			lock (syncThread)
			{
				ReadRegister(registers["RegIrqFlags"]);
				if (RxTimeout && PacketModeRxSingle && isPacketHandlerStarted)
				{
					Console.WriteLine("IRQ: RxTimeoutIrq [0x{0}] - ReadIrqFlags", registers["RegIrqFlags"].Value.ToString("X02"));
					ClrRxTimeoutIrq();
					PacketModeRxSingle = false;
					PacketHandlerStop();
				}
				int temp_13 = ValidHeader ? 1 : 0;
				if (!FhssChangeChannel)
					return;
				Console.WriteLine("IRQ: FhssChangeChannelIrq [0x{0}] - ReadIrqFlags", registers["RegIrqFlags"].Value.ToString("X02"));
				ClrFhssChangeChannelIrq();
				ReadRegister(registers["RegHopChannel"]);
			}
		}

		public void SetModeLeds(OperatingModeEnum mode)
		{
			if (test)
				return;
			switch (mode)
			{
				case OperatingModeEnum.Tx:
				case OperatingModeEnum.TxContinuous:
					SKSetPinState(7, 1);
					SKSetPinState(8, 0);
					break;
				case OperatingModeEnum.Rx:
				case OperatingModeEnum.RxSingle:
				case OperatingModeEnum.Cad:
					SKSetPinState(7, 0);
					SKSetPinState(8, 1);
					break;
				default:
					SKSetPinState(6, 1);
					SKSetPinState(7, 1);
					SKSetPinState(8, 1);
					break;
			}
		}

		public void SetDefaultValues()
		{
			if (IsOpen)
			{
				if (!Write((byte)registers["RegModemConfig2"].Address, new byte[2] { 115, 255 }))
					throw new Exception("Unable to write register: " + registers["RegModemConfig2"].Name);
				if (!Write((byte)registers["RegModemConfig3"].Address, 4))
					throw new Exception("Unable to write register: " + registers["RegModemConfig3"].Name);
			}
			else
			{
				registers["RegModemConfig2"].Value = 115;
				registers["RegSymbTimeoutLsb"].Value = 255;
				registers["RegModemConfig3"].Value = 4;
				ReadRegisters();
			}
		}

		public void SetLoraOn(bool enable)
		{
			if (enable)
			{
				SetOperatingMode(OperatingModeEnum.Sleep);
				byte data = 0;
				if (!Read(1, ref data))
					throw new Exception("Unable to read LoRa mode");
				if (!Write(1, (byte)((data | 128) & 248)))
					throw new Exception("Unable to write LoRa mode");
				SetOperatingMode(OperatingModeEnum.Stdby);
			}
			else
			{
				SetOperatingMode(OperatingModeEnum.Sleep);
				byte data = 0;
				if (!Read(1, ref data))
					throw new Exception("Unable to read FSK mode");
				if (!Write(1, (byte)((uint)data & 120U)))
					throw new Exception("Unable to write FSK mode");
				SetOperatingMode(OperatingModeEnum.Stdby);
			}
		}

		public void ImageCalStart()
		{
			lock (syncThread)
			{
				OperatingModeEnum local_0 = Mode;
				try
				{
					byte reg59 = 0;
					SetOperatingMode(OperatingModeEnum.Stdby);
					Read(59, ref reg59);
					Write(59, (byte)(reg59 | 64));
					DateTime startTime = DateTime.Now;
					bool timeout;
					do
					{
						reg59 = 0;
						Read(59, ref reg59);
						timeout = (DateTime.Now - startTime).TotalMilliseconds >= 1000.0;
					}
					while ((reg59 & 32) == 32 && !timeout)
						;
					if (timeout)
						throw new Exception("Image calibration timeout.");
				}
				finally
				{
					SetOperatingMode(local_0);
				}
			}
		}

		private void SetRegister(string name, int mask, int value)
		{
			registers[name].Value = (registers[name].Value & (uint)mask) | (uint)value;
		}

		public void SetAccessSharedFskReg(bool value)
		{
			try
			{
				SetRegister("RegOpMode", 191, value ? 64 : 0);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetLowFrequencyModeOn(bool value)
		{
			try
			{
				lock (syncThread)
				{
					SetRegister("RegOpMode", 247, value ? 8 : 0);
					UpdateRegisterTable();
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetOperatingMode(OperatingModeEnum value)
		{
			SetOperatingMode(value, false);
		}

		public void SetOperatingMode(OperatingModeEnum value, bool isQuiet)
		{
			try
			{
				if (value == OperatingModeEnum.Tx || value == OperatingModeEnum.TxContinuous)
				{
					SKSetPinState(11, 0);
					SKSetPinState(12, 1);
				}
				else
				{
					SKSetPinState(11, 1);
					SKSetPinState(12, 0);
				}
				if (value == OperatingModeEnum.TxContinuous)
				{
					SetTxContinuousOn(true);
					value = OperatingModeEnum.Tx;
				}
				else
					SetTxContinuousOn(false);

				byte data = (byte)((registers["RegOpMode"].Value & 248U) | (uint)value);
				if (!isQuiet)
				{
					registers["RegOpMode"].Value = data;
				}
				else
				{
					lock (syncThread)
					{
						if (!Write((byte)registers["RegOpMode"].Address, data))
							throw new Exception("Unable to write register " + registers["RegOpMode"].Name);
						if (Mode == OperatingModeEnum.Rx || Mode == OperatingModeEnum.RxSingle || Mode == OperatingModeEnum.Cad)
						{
							ReadRegister(registers["RegLna"]);
						}
					}
				}
				Console.WriteLine("RegOpMode (SOM): 0x{0}", data.ToString("X02"));
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetBand(BandEnum value)
		{
			try
			{
				lock (syncThread)
					SetRegister("RegFdevMsb", 63, (int)value << 6);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetFrequencyRf(Decimal value)
		{
			try
			{
				lock (syncThread)
				{
					// byte frfMsb = (byte)registers["RegFrfMsb"].Value;
					// byte frfMid = (byte)registers["RegFrfMid"].Value;
					// byte frfLsb = (byte)registers["RegFrfLsb"].Value;
					byte frfMsb = (byte)((long)(value / frequencyStep) >> 16);
					byte frfMid = (byte)((long)(value / frequencyStep) >> 8);
					byte frfLsb = (byte)((long)(value / frequencyStep) >> 0);
					frequencyRfCheckDisable = true;
					registers["RegFrfMsb"].Value = frfMsb;
					registers["RegFrfMid"].Value = frfMid;
					frequencyRfCheckDisable = false;
					registers["RegFrfLsb"].Value = frfLsb;
					if (FrequencyRf >= new Decimal(640000000))
					{
						SetLowFrequencyModeOn(false);
						SetLnaBoost(lnaBoostPrev);
					}
					else
					{
						SetLowFrequencyModeOn(true);
						lnaBoostPrev = LnaBoost;
						SetLnaBoost(false);
					}
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetPaMode(PaSelectEnum value)
		{
			try
			{
				lock (syncThread)
					SetRegister("RegPaConfig", 0x7F, (value == PaSelectEnum.PA_BOOST) ? 0x80 : 0);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetMaxOutputPower(Decimal value)
		{
			try
			{
				lock (syncThread)
					SetRegister("RegPaConfig", 0x8F, ((((int)((value - 10.8M) / 0.6M)) & 7) << 4));
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetOutputPower(Decimal value)
		{
			try
			{
				lock (syncThread)
				{
					byte local_0_1 = (byte)(registers["RegPaConfig"].Value & 240U);
					byte local_0_2;
					if (PaSelect == PaSelectEnum.RFO)
					{
						if (value < MaxOutputPower - new Decimal(150, 0, 0, false, 1))
							value = MaxOutputPower - new Decimal(150, 0, 0, false, 1);
						if (value > MaxOutputPower)
							value = MaxOutputPower;
						local_0_2 = (byte)((uint)local_0_1 | (uint)((int)(value - MaxOutputPower + new Decimal(150, 0, 0, false, 1)) & 15U));
					}
					else if (!Pa20dBm)
					{
						if (value < new Decimal(2))
							value = new Decimal(2);
						if (value > new Decimal(17))
							value = new Decimal(17);
						local_0_2 = (byte)((uint)local_0_1 | (uint)((int)(value - new Decimal(170, 0, 0, false, 1) + new Decimal(150, 0, 0, false, 1)) & 15));
					}
					else
					{
						if (value < new Decimal(5))
							value = new Decimal(5);
						if (value > new Decimal(20))
							value = new Decimal(20);
						local_0_2 = (byte)((uint)local_0_1 | (uint)((int)(value - new Decimal(200, 0, 0, false, 1) + new Decimal(150, 0, 0, false, 1)) & 15));
					}
					registers["RegPaConfig"].Value = (uint)local_0_2;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetForceTxBandLowFrequencyOn(bool value)
		{
			try
			{
				lock (syncThread)
					SetRegister("RegPaRamp", 0xEF, value ? 0x10 : 0);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetPaRamp(PaRampEnum value)
		{
			try
			{
				lock (syncThread)
					SetRegister("RegPaRamp", 0xF0, (int)value & 0x0F);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetOcpOn(bool value)
		{
			try
			{
				lock (syncThread)
					SetRegister("RegOcp", 0xDF, value ? 0x20 : 0);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetOcpTrim(Decimal value)
		{
			try
			{
				lock (syncThread)
				{
					byte num = (byte)(registers["RegOcp"].Value & 0xE0);
					if (value <= 120.0M)
						num = (byte)(num | ((byte)((value - 45M) / 5M) & 0x0F));
					else if (value > 120M && value <= 240.0M)
						num = (byte)(num | ((byte)((value + 30M) / 10M) & 0x1F));
					else
						num = (byte)(num | 0x1B);
					registers["RegOcp"].Value = num;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetAgcReferenceLevel(int value)
		{
			try
			{
				lock (syncThread)
					SetRegister("RegAgcRef", 0xC0, value & 0x3F);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetAgcStep(byte id, byte value)
		{
			try
			{
				lock (syncThread)
				{
					Register register;
					switch (id)
					{
						case 1:
							register = registers["RegAgcThresh1"];
							break;
						case 2:
						case 3:
							register = registers["RegAgcThresh2"];
							break;
						case 4:
						case 5:
							register = registers["RegAgcThresh3"];
							break;
						default:
							throw new Exception("Invalid AGC step ID!");
					}
					byte val = (byte)register.Value;
					byte val_new;
					switch (id)
					{
						case 1:
							val_new = (byte)((val & 0xE0) | value);
							break;
						case 2:
							val_new = (byte)((val & 0x0F) | (value << 4));
							break;
						case 3:
							val_new = (byte)((val & 0xF0) | (value & 0x0F));
							break;
						case 4:
							val_new = (byte)((val & 0x0F) | (value << 4));
							break;
						case 5:
							val_new = (byte)((val & 0xF0) | (value & 0x0F));
							break;
						default:
							throw new Exception("Invalid AGC step ID!");
					}
					register.Value = (uint)val_new;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetLnaGain(LnaGainEnum value)
		{
			try
			{
				lock (syncThread)
					SetRegister("RegLna", 0x1F, (int)value << 5);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetForceRxBandLowFrequencyOn(bool value)
		{
			try
			{
				lock (syncThread)
					SetRegister("RegLna", 0xFB, value ? 4 : 0);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetLnaBoost(bool value)
		{
			try
			{
				lock (syncThread)
					SetRegister("RegLna", 0xFC, value ? 3 : 0);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetFifoAddrPtr(byte value)
		{
			try
			{
				lock (syncThread)
					registers["RegFifoAddrPtr"].Value = (uint)value;
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetFifoTxBaseAddr(byte value)
		{
			try
			{
				lock (syncThread)
					registers["RegFifoTxBaseAddr"].Value = (uint)value;
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetFifoRxBaseAddr(byte value)
		{
			try
			{
				lock (syncThread)
					registers["RegFifoRxBaseAddr"].Value = (uint)value;
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetRxTimeoutMask(bool value)
		{
			try
			{
				lock (syncThread)
					SetRegister("RegIrqFlagsMask", 0x7F, value ? 0x80 : 0);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetRxDoneMask(bool value)
		{
			try
			{
				lock (syncThread)
					SetRegister("RegIrqFlagsMask", 0xBF, value ? 0x40 : 0);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetPayloadCrcErrorMask(bool value)
		{
			try
			{
				lock (syncThread)
					SetRegister("RegIrqFlagsMask", 0xDF, value ? 0x20 : 0);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetValidHeaderMask(bool value)
		{
			try
			{
				lock (syncThread)
					SetRegister("RegIrqFlagsMask", 0xEF, value ? 0x10 : 0);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetTxDoneMask(bool value)
		{
			try
			{
				lock (syncThread)
					SetRegister("RegIrqFlagsMask", 0xF7, value ? 0x08 : 0);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetCadDoneMask(bool value)
		{
			try
			{
				lock (syncThread)
					SetRegister("RegIrqFlagsMask", 0xFB, value ? 0x04 : 0);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetFhssChangeChannelMask(bool value)
		{
			try
			{
				lock (syncThread)
					SetRegister("RegIrqFlagsMask", 0xFD, value ? 0x02 : 0);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetCadDetectedMask(bool value)
		{
			try
			{
				lock (syncThread)
					SetRegister("RegIrqFlagsMask", 0xFE, value ? 0x01 : 0);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void ClrAllIrq()
		{
			try
			{
				lock (syncThread)
				{
					WriteRegister(registers["RegIrqFlags"], 255);
					ReadRegister(registers["RegIrqFlags"]);
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void ClrRxTimeoutIrq()
		{
			try
			{
				lock (syncThread)
					SetRegister("RegIrqFlags", 0x7F, 0x80);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void ClrRxDoneIrq()
		{
			try
			{
				lock (syncThread)
					SetRegister("RegIrqFlags", 0xBF, 0x40);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void ClrPayloadCrcErrorIrq()
		{
			try
			{
				lock (syncThread)
					SetRegister("RegIrqFlags", 0xDF, 0x20);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void ClrValidHeaderIrq()
		{
			try
			{
				lock (syncThread)
					SetRegister("RegIrqFlags", 0xEF, 0x10);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void ClrTxDoneIrq()
		{
			try
			{
				lock (syncThread)
					SetRegister("RegIrqFlags", 0xF7, 0x08);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void ClrCadDoneIrq()
		{
			try
			{
				lock (syncThread)
					SetRegister("RegIrqFlags", 0xFB, 0x04);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void ClrFhssChangeChannelIrq()
		{
			try
			{
				lock (syncThread)
					SetRegister("RegIrqFlags", 0xFD, 0x02);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void ClrCadDetectedIrq()
		{
			try
			{
				lock (syncThread)
					SetRegister("RegIrqFlags", 0xFE, 0x01);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetBandwidth(byte value)
		{
			try
			{
				lock (syncThread)
				{
					if (value < 7 || value == 7 && (SpreadingFactor == 11 || SpreadingFactor == 12) || value == 8 && SpreadingFactor == 12)
						SetLowDatarateOptimize(true);
					else
						SetLowDatarateOptimize(false);

					SetRegister("RegModemConfig1", 0x0F, (int)value << 4);

					if (value == 9 && FrequencyRf >= 640000000M)
					{
						SetRegister("RegTest36", 254, 0);
						SetRegister("RegTest3A", 192, 36);
					}
					else if (value == 9)
					{
						SetRegister("RegTest36", 254, 0);
						SetRegister("RegTest3A", 192, 63);
					}
					else
					{
						SetRegister("RegTest36", 255, 1);
						ReadRegister(registers["RegTest3A"]);
					}
					ReadRegister(registers["RegModemConfig2"]);
					ReadRegister(registers["RegSymbTimeoutLsb"]);
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetCodingRate(byte value)
		{
			try
			{
				lock (syncThread)
					SetRegister("RegModemConfig1", 0xF1, (int)value << 1);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetImplicitHeaderModeOn(bool value)
		{
			try
			{
				lock (syncThread)
					SetRegister("RegModemConfig1", 0xFE, value ? 1 : 0);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetSpreadingFactor(byte value)
		{
			try
			{
				lock (syncThread)
				{
					if (Bandwidth < 7 || Bandwidth == 7 && (value == 11 || value == 12) || Bandwidth == 8 && value == 12)
						SetLowDatarateOptimize(true);
					else
						SetLowDatarateOptimize(false);

					SetRegister("RegModemConfig2", 0x0F, (int)value << 4);
					ReadRegister(registers["RegSymbTimeoutLsb"]);
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetTxContinuousOn(bool value)
		{
			try
			{
				SetRegister("RegModemConfig2", 247, value ? 8 : 0);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetPayloadCrcOn(bool value)
		{
			try
			{
				lock (syncThread)
					SetRegister("RegModemConfig2", 0xFB, value ? 4 : 0);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetSymbTimeout(Decimal value)
		{
			try
			{
				lock (syncThread)
				{
					SetRegister("RegModemConfig2", 252, (int)(((ulong)(value / SymbolTime) >> 8) & 3UL));
					SetRegister("RegSymbTimeoutLsb", 0, (int)(value / SymbolTime));
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetPreambleLength(int value)
		{
			try
			{
				lock (syncThread)
				{
					value -= 4;
					registers["RegPreambleMsb"].Value = (uint)(value >> 8);
					registers["RegPreambleLsb"].Value = (uint)value;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetPayloadLength(byte value)
		{
			try
			{
				lock (syncThread)
					registers["RegPayloadLength"].Value = (uint)value;
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetPayloadMaxLength(byte value)
		{
			try
			{
				lock (syncThread)
					registers["RegPayloadMaxLength"].Value = (uint)value;
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetFreqHoppingPeriod(byte value)
		{
			try
			{
				lock (syncThread)
					registers["RegHopPeriod"].Value = (uint)value;
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetLowDatarateOptimize(bool value)
		{
			try
			{
				lock (syncThread)
					SetRegister("RegModemConfig3", 0xF7, value ? 8 : 0);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetAgcAutoOn(bool value)
		{
			try
			{
				lock (syncThread)
					SetRegister("RegModemConfig3", 0xFB, value ? 4 : 0);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetDioMapping(byte id, DioMappingEnum value)
		{
			try
			{
				lock (syncThread)
				{
					Register register;
					switch (id)
					{
						case 0:
						case 1:
						case 2:
						case 3:
							register = registers["RegDioMapping1"];
							break;
						case 4:
						case 5:
							register = registers["RegDioMapping2"];
							break;
						default:
							throw new Exception("Invalid DIO ID!");
					}
					uint local_0 = register.Value;
					uint local_0_2;
					switch (id)
					{
						case 0:
							local_0_2 = local_0 & 63U | (uint)value << 6;
							break;
						case 1:
							local_0_2 = local_0 & 207U | (uint)value << 4;
							break;
						case 2:
							local_0_2 = local_0 & 243U | (uint)value << 2;
							break;
						case 3:
							local_0_2 = (uint)((DioMappingEnum)(local_0 & 252U) | value & DioMappingEnum.DIO_MAP_11);
							break;
						case 4:
							local_0_2 = local_0 & 63U | (uint)value << 6;
							break;
						case 5:
							local_0_2 = local_0 & 207U | (uint)value << 4;
							break;
						default:
							throw new Exception("Invalid DIO ID!");
					}
					register.Value = local_0_2;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetFastHopOn(bool value)
		{
			try
			{
				lock (syncThread)
					SetRegister("RegPllHop", 0x7F, value ? 0x80 : 0);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetTcxoInputOn(bool value)
		{
			try
			{
				lock (syncThread)
					SetRegister("RegTcxo", 39, value ? 0x10 : 0);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetPa20dBm(bool value)
		{
			try
			{
				lock (syncThread)
				{
					if (value == Pa20dBm)
						return;

					registers["RegPaDac"].Value = value ? 135U : 132U;
					if (value)
						SetPaMode(PaSelectEnum.PA_BOOST);
					ReadRegister(registers["RegPaConfig"]);
					ReadRegister(registers["RegOcp"]);
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetPllBandwidth(Decimal value)
		{
			try
			{
				lock (syncThread)
					SetRegister("RegPll", 0x3F, (int)((value / 75000M) - 1M) << 6);
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		private void PacketHandlerStart()
		{
			lock (syncThread)
			{
				try
				{
					SetModeLeds(OperatingModeEnum.Sleep);
					packetNumber = 0;
					SetOperatingMode(OperatingModeEnum.Sleep, true);
					ReadRegister(registers["RegRxNbBytes"]);
					ReadRegister(registers["RegRxPacketCntValueMsb"]);
					ReadRegister(registers["RegRxPacketCntValueLsb"]);
					ReadRegister(registers["RegPktSnrValue"]);
					ReadRegister(registers["RegPktRssiValue"]);
					ReadRegister(registers["RegFifoRxCurrentAddr"]);
					ReadRegister(registers["RegModemStat"]);
					ReadRegister(registers["RegRxHeaderCntValueMsb"]);
					ReadRegister(registers["RegRxHeaderCntValueLsb"]);
					ReadRegister(registers["RegHopChannel"]);
					if (PacketModeTx)
					{
						if ((int)PayloadLength == 0)
						{
							MessageBox.Show("Message must be at least one byte long", "SX1276SKA-PacketHandler", MessageBoxButtons.OK, MessageBoxIcon.Hand);
							throw new Exception("Message must be at least one byte long");
						}
						SetDioMapping((byte)0, DioMappingEnum.DIO_MAP_01);
					}
					else if (PacketModeRxSingle)
					{
						SetDioMapping((byte)0, DioMappingEnum.DIO_MAP_00);
					}
					else
					{
						SetDioMapping((byte)0, DioMappingEnum.DIO_MAP_00);
						SetFifoAddrPtr(FifoRxBaseAddr);
					}
					frameTransmitted = false;
					frameReceived = false;
					if (PacketModeTx)
						firstTransmit = true;
					else if (PacketModeRxSingle)
					{
						SetOperatingMode(OperatingModeEnum.RxSingle, true);
						OnPacketHandlerReceived();
					}
					else
					{
						SetOperatingMode(OperatingModeEnum.Rx, true);
						OnPacketHandlerReceived();
					}
					PacketHandlerLog.Start();
					isPacketHandlerStarted = true;
					OnPacketHandlerStarted();
				}
				catch (Exception exception_0)
				{
					OnError(1, exception_0.Message);
					PacketHandlerStop();
				}
			}
		}

		private void PacketHandlerStop()
		{
			try
			{
				lock (syncThread)
				{
					isPacketHandlerStarted = false;
					PacketHandlerLog.Stop();
					if (!PacketModeTx && PacketModeRxSingle)
					{
						PacketModeRxSingle = false;
						ReadRegister(registers["RegOpMode"]);
						OnPacketHandlerStoped();
					}
					Console.WriteLine("IRQ: AllIrq [0x{0}] - Stop", registers["RegIrqFlags"].Value.ToString("X02"));
					ClrAllIrq();
					ReadRegister(registers["RegIrqFlags"]);
					SetOperatingMode(Mode);
					frameTransmitted = false;
					frameReceived = false;
					firstTransmit = false;
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
			finally
			{
				OnPacketHandlerStoped();
			}
		}

		private void PacketHandlerTransmit()
		{
			lock (syncThread)
			{
				try
				{
					SetModeLeds(OperatingModeEnum.Tx);
					if (maxPacketNumber != 0 && packetNumber >= maxPacketNumber || !isPacketHandlerStarted)
					{
						PacketHandlerStop();
					}
					else
					{
						SetOperatingMode(OperatingModeEnum.Stdby, true);
						Thread.Sleep(100);
						SetFifoAddrPtr(FifoTxBaseAddr);
						++packetNumber;
						if (PacketUsePer)
						{
							if ((int)PayloadLength < 9)
								Array.Resize<byte>(ref payload, 9);
							Payload[0] = 0;
							Payload[1] = (byte)(packetNumber >> 24);
							Payload[2] = (byte)(packetNumber >> 16);
							Payload[3] = (byte)(packetNumber >> 8);
							Payload[4] = (byte)packetNumber;
							Payload[5] = 80;
							Payload[6] = 69;
							Payload[7] = 82;
							Payload[8] = (byte)(
								(uint)Payload[0] +
								(uint)Payload[1] +
								(uint)Payload[2] +
								(uint)Payload[3] +
								(uint)Payload[4] +
								(uint)Payload[5] +
								(uint)Payload[6] +
								(uint)Payload[7]
								);
							OnPropertyChanged("Payload");
						}
						frameTransmitted = WriteFifo(Payload);
						SetOperatingMode(OperatingModeEnum.Tx, true);
					}
				}
				catch (Exception exception_0)
				{
					PacketHandlerStop();
					OnError(1, exception_0.Message);
				}
				finally
				{
					SetModeLeds(OperatingModeEnum.Sleep);
				}
			}
		}

		private void PacketHandlerReceive()
		{
			lock (syncThread)
			{
				try
				{
					byte[] local_0 = new byte[0];
					SetModeLeds(OperatingModeEnum.Rx);
					if (!PacketModeTx)
					{
						if (PacketModeRxSingle)
						{
							SetFifoAddrPtr(FifoRxBaseAddr);
							local_0 = !ImplicitHeaderModeOn ? new byte[(int)RxNbBytes] : new byte[(int)PayloadLength];
							isPacketHandlerStarted = false;
						}
						else if (ImplicitHeaderModeOn)
						{
							SetFifoAddrPtr(FifoRxCurrentAddr);
							local_0 = new byte[(int)PayloadLength];
						}
						else
						{
							SetFifoAddrPtr(FifoRxCurrentAddr);
							local_0 = new byte[(int)RxNbBytes];
						}
					}
					frameReceived = ReadFifo(ref local_0);
					Payload = local_0;
					if (!PayloadCrcError)
						++packetNumber;
					OnPacketHandlerReceived();
					pktLnaValues.Clear();
					pktRssiValues.Clear();
					if (!isPacketHandlerStarted)
						PacketHandlerStop();
					SetModeLeds(OperatingModeEnum.Sleep);
				}
				catch (Exception exception_0)
				{
					PacketHandlerStop();
					OnError(1, exception_0.Message);
				}
			}
		}

		private void OnPacketHandlerStarted()
		{
			if (PacketHandlerStarted == null)
				return;
			PacketHandlerStarted(this, new EventArgs());
		}

		private void OnPacketHandlerStoped()
		{
			if (PacketHandlerStoped == null)
				return;
			PacketHandlerStoped(this, new EventArgs());
		}

		private void OnPacketHandlerTransmitted()
		{
			if (PacketHandlerTransmitted != null)
			{
				Console.WriteLine("Pkt#: {0}", packetNumber);
				PacketHandlerTransmitted(this, new PacketStatusEventArg(packetNumber, maxPacketNumber));
			}
		}

		private void OnPacketHandlerReceived()
		{
			if (PacketHandlerReceived != null)
			{
				Console.WriteLine("Pkt#: {0} - RxPkt#: {1}", packetNumber, (registers["RegRxPacketCntValueMsb"].Value << 8 | registers["RegRxPacketCntValueLsb"].Value));
				PacketHandlerReceived(this, new PacketStatusEventArg(packetNumber, maxPacketNumber));
			}
		}

		public void SetMessageLength(int value)
		{
			try
			{
				lock (syncThread)
				{ }
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetMessage(byte[] value)
		{
			try
			{
				lock (syncThread)
				{
					Payload = value;
					SetPayloadLength((byte)Payload.Length);
				}
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetPacketHandlerStartStop(bool value)
		{
			try
			{
				lock (syncThread)
				{
					if (value)
						PacketHandlerStart();
					else
						PacketHandlerStop();
				}
			}
			catch (Exception ex)
			{
				PacketHandlerStop();
				OnError(1, ex.Message);
			}
		}

		public void SetMaxPacketNumber(int value)
		{
			try
			{
				lock (syncThread)
					maxPacketNumber = value;
			}
			catch (Exception ex)
			{
				OnError(1, ex.Message);
			}
		}

		public void SetPacketHandlerLogEnable(bool value)
		{
			try
			{
				lock (syncThread)
					LogEnabled = value;
			}
			catch (Exception ex)
			{
				LogEnabled = false;
				OnError(1, ex.Message);
			}
		}

		private void packet_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			OnPropertyChanged(e.PropertyName);
		}

		private void registers_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			lock (syncThread)
			{
				if (e.PropertyName == "Value")
				{
					UpdateRegisterValue((Register)sender);
					if (readLock == 0 && !Write((byte)((Register)sender).Address, (byte)((Register)sender).Value))
						OnError(1, "Unable to write register " + ((Register)sender).Name);
					if (((Register)sender).Name == "RegOpMode")
					{
						Console.WriteLine("RegOpMode (PC): 0x{0}", ((Register)sender).Value.ToString("X02"));
						if (Mode == OperatingModeEnum.Rx || Mode == OperatingModeEnum.RxSingle || Mode == OperatingModeEnum.Cad)
						{
							ReadRegister(registers["RegLna"]);
							ReadRegister(registers["RegRssiValue"]);
						}
						ReadRegister(registers["RegIrqFlags"]);
					}
				}
			}
		}

		private void device_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Version")
			{
				PopulateRegisters();
				ReadRegisters();
			}
		}

		private void usbDevice_Opened(object sender, EventArgs e)
		{
			isOpen = true;
			if (!SKGetVersion())
				OnError(1, "Unable to read SK version.");
			else if (!SKReset())
				OnError(1, "Unable to reset the SK");
			else
			{
				regUpdateThreadContinue = true;
				regUpdateThread = new Thread(new ThreadStart(RegUpdateThread));
				regUpdateThread.Start();
				OnConnected();
			}
		}

		private void usbDevice_Closed(object sender, EventArgs e)
		{
			spectrumOn = false;
			isOpen = false;
			regUpdateThreadContinue = false;
			OnDisconnected();
			OnError(0, "-");
		}

		private void device_Dio0Changed(object sender, SX1276LR.IoChangedEventArgs e)
		{
			lock (syncThread)
			{
				if (isPacketHandlerStarted && (e.State || firstTransmit))
				{
					firstTransmit = false;
					ReadRegister(registers["RegIrqFlags"]);
					if (PacketModeTx)
					{
						OnPacketHandlerTransmitted();
						Console.WriteLine("IRQ: TxDoneIrq [0x{0}] - Dio0Tx", registers["RegIrqFlags"].Value.ToString("X02"));
						ClrAllIrq();
						Console.WriteLine("IRQ: TxDoneIrq [0x{0}] - Dio0Tx", registers["RegIrqFlags"].Value.ToString("X02"));
						PacketHandlerTransmit();
					}
					else
					{
						ReadRegister(registers["RegModemStat"]);
						ReadRegister(registers["RegHopChannel"]);
						ReadRegister(registers["RegRxHeaderCntValueMsb"]);
						ReadRegister(registers["RegRxHeaderCntValueLsb"]);
						ReadRegister(registers["RegRxPacketCntValueMsb"]);
						ReadRegister(registers["RegRxPacketCntValueLsb"]);
						ReadRegister(registers["RegRxNbBytes"]);
						ReadRegister(registers["RegFifoRxCurrentAddr"]);
						ReadRegister(registers["RegPktSnrValue"]);
						ReadRegister(registers["RegPktRssiValue"]);
						PacketHandlerReceive();
						Console.WriteLine("IRQ: RxDoneIrq [0x{0}] - Dio0Rx", registers["RegIrqFlags"].Value.ToString("X02"));
						ClrAllIrq();
						Console.WriteLine("IRQ: RxDoneIrq [0x{0}] - Dio0Rx", registers["RegIrqFlags"].Value.ToString("X02"));
					}
				}
			}
		}

		private void device_Dio1Changed(object sender, SX1276LR.IoChangedEventArgs e)
		{
		}

		private void device_Dio2Changed(object sender, SX1276LR.IoChangedEventArgs e)
		{
		}

		private void device_Dio3Changed(object sender, SX1276LR.IoChangedEventArgs e)
		{
		}

		private void device_Dio4Changed(object sender, SX1276LR.IoChangedEventArgs e)
		{
		}

		private void device_Dio5Changed(object sender, SX1276LR.IoChangedEventArgs e)
		{
		}

		private void SpectrumProcess()
		{
			Decimal num = SpectrumFrequencyMin + SpectrumFrequencyStep * (Decimal)SpectrumFrequencyId;
			byte data1 = (byte)((long)(num / frequencyStep) >> 16);
			byte data2 = (byte)((long)(num / frequencyStep) >> 8);
			byte data3 = (byte)(long)(num / frequencyStep);

			if (!Write((byte)registers["RegFrfMsb"].Address, data1))
				OnError(1, "Unable to write register " + registers["RegFrfMsb"].Name);
			if (!Write((byte)registers["RegFrfMid"].Address, data2))
				OnError(1, "Unable to write register " + registers["RegFrfMid"].Name);
			if (!Write((byte)registers["RegFrfLsb"].Address, data3))
				OnError(1, "Unable to write register " + registers["RegFrfLsb"].Name);
			ReadRegister(registers["RegRssiValue"]);
			++SpectrumFrequencyId;
			if (SpectrumFrequencyId < SpectrumNbFrequenciesMax)
				return;
			SpectrumFrequencyId = 0;
		}

		private void RegUpdateThread()
		{
			int num = 0;
			byte pinsState = 0;
			while (regUpdateThreadContinue)
			{
				if (!IsOpen)
				{
					Application.DoEvents();
					Thread.Sleep(10);
				}
				else
				{
					try
					{
						lock (syncThread)
						{
							if (SKGetPinsState(ref pinsState))
							{
								if ((pinsState & 0x20) == 0x20)
									OnDio5Changed(true);
								else
									OnDio5Changed(false);
								if ((pinsState & 0x10) == 0x10)
									OnDio4Changed(true);
								else
									OnDio4Changed(false);
								if ((pinsState & 0x08) == 0x08)
									OnDio3Changed(true);
								else
									OnDio3Changed(false);
								if ((pinsState & 0x04) == 0x04)
									OnDio2Changed(true);
								else
									OnDio2Changed(false);
								if ((pinsState & 0x02) == 0x02)
									OnDio1Changed(true);
								else
									OnDio1Changed(false);
								if ((pinsState & 0x01) == 0x01)
									OnDio0Changed(true);
								else
									OnDio0Changed(false);
							}
							if (!monitor)
							{
								Thread.Sleep(10);
								continue;
							}
							if (num % 10 == 0)
							{
								ReadRegister(registers["RegIrqFlags"]);
								ReadRegister(registers["RegModemStat"]);
								if (RxTimeout && PacketModeRxSingle && isPacketHandlerStarted)
									PacketHandlerStop();
								byte local_2 = 0;
								if (!Read((byte)1, ref local_2))
									throw new Exception("Unable to read register: RegOpMode");
								if ((local_2 & 0x80) == 0)
								{
									SetLoraOn(true);
									WriteRegisters();
									ReadRegisters();
									if (isPacketHandlerStarted && PacketModeTx)
									{
										firstTransmit = true;
										SetDioMapping((byte)0, DioMappingEnum.DIO_MAP_01);
										SetOperatingMode(OperatingModeEnum.Tx);
									}
								}
								if (Mode == OperatingModeEnum.Cad)
									ReadRegister(registers["RegOpMode"]);
								if (Mode == OperatingModeEnum.Rx || isPacketHandlerStarted && !PacketModeTx)
								{
									if (isDebugOn && isReceiving)
										ReadRegister(registers["RegLna"]);
									if (!SpectrumOn)
									{
										if (RfPaSwitchEnabled == 2)
										{
											RfPaSwitchSel = RfPaSwitchSelEnum.RF_IO_RFIO;
											ReadRegister(registers["RegRssiValue"]);
											RfPaSwitchSel = RfPaSwitchSelEnum.RF_IO_PA_BOOST;
											ReadRegister(registers["RegRssiValue"]);
										}
										else
											ReadRegister(registers["RegRssiValue"]);
									}
									else
										SpectrumProcess();
								}
							}
							if (num >= 200)
							{
								if (restartRx)
								{
									restartRx = false;
									ReadRegister(registers["RegLna"]);
								}
								num = 0;
							}
						}
					}
					catch { }
					++num;
					Thread.Sleep(1);
				}
			}
		}

		private void OnPropertyChanged(string propName)
		{
			if (PropertyChanged != null)
				PropertyChanged((object)this, new PropertyChangedEventArgs(propName));
		}

		public void Dispose()
		{
			Close();
		}

		public class IoChangedEventArgs : EventArgs
		{
			private bool state;

			public bool State
			{
				get { return state; }
				private set { state = value; }
			}

			public IoChangedEventArgs(bool state)
			{
				State = state;
			}
		}

	}
}

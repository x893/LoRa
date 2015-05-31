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
			get
			{
				return pktRssiValues;
			}
			set
			{
				if (pktRssiValues == value)
					return;
				pktRssiValues = value;
			}
		}

		public List<int> PktLnaValues
		{
			get
			{
				return pktLnaValues;
			}
			set
			{
				if (pktLnaValues == value)
					return;
				pktLnaValues = value;
			}
		}

		public object SyncThread
		{
			get
			{
				return syncThread;
			}
		}

		public string DeviceName
		{
			get
			{
				return deviceName;
			}
		}

		public Version FwVersion
		{
			get
			{
				return fwVersion;
			}
			set
			{
				if (!(fwVersion != value))
					return;
				fwVersion = value;
				OnPropertyChanged("FwVersion");
			}
		}

		public HidDevice UsbDevice
		{
			get
			{
				return usbDevice;
			}
		}

		public bool IsOpen
		{
			get
			{
				return isOpen;
			}
			set
			{
				isOpen = value;
				OnPropertyChanged("IsOpen");
			}
		}

		public int SPISpeed
		{
			get
			{
				return spiSpeed;
			}
			set
			{
				spiSpeed = value;
				OnPropertyChanged("SPISpeed");
			}
		}

		public RegisterCollection Registers
		{
			get
			{
				return registers;
			}
			set
			{
				registers = value;
				OnPropertyChanged("Registers");
			}
		}

		public bool Test
		{
			get
			{
				return test;
			}
			set
			{
				test = value;
			}
		}

		public ILog PacketHandlerLog
		{
			get
			{
				return packetHandlerLog;
			}
			set
			{
				packetHandlerLog = value;
				OnPropertyChanged("PacketHandlerLog");
			}
		}

		public bool PacketModeTx
		{
			get
			{
				return packetModeTx;
			}
			set
			{
				packetModeTx = value;
				OnPropertyChanged("PacketModeTx");
				OnPropertyChanged("PayloadLength");
			}
		}

		public bool PacketModeRxSingle
		{
			get
			{
				return packetModeRxSingle;
			}
			set
			{
				packetModeRxSingle = value;
				OnPropertyChanged("PacketModeRxSingle");
				OnPropertyChanged("PayloadLength");
			}
		}

		public bool PacketUsePer
		{
			get
			{
				return packetUsePer;
			}
			set
			{
				packetUsePer = value;
				OnPropertyChanged("PacketUsePer");
			}
		}

		public bool IsDebugOn
		{
			get
			{
				return isDebugOn;
			}
			set
			{
				isDebugOn = value;
				OnPropertyChanged("IsDebugOn");
			}
		}

		public Decimal FrequencyXo
		{
			get
			{
				return frequencyXo;
			}
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
			get
			{
				return frequencyStep;
			}
			set
			{
				frequencyStep = value;
				OnPropertyChanged("FrequencyStep");
			}
		}

		public Decimal SymbolRate
		{
			get
			{
				return (Decimal)(BandwidthHz / Math.Pow(2.0, (double)SpreadingFactor));
			}
		}

		public Decimal SymbolTime
		{
			get
			{
				return new Decimal(1) / SymbolRate;
			}
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
			get
			{
				return spectrumOn;
			}
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
			get
			{
				return spectrumFreqSpan;
			}
			set
			{
				spectrumFreqSpan = value;
				OnPropertyChanged("SpectrumFreqSpan");
			}
		}

		public Decimal SpectrumFrequencyMax
		{
			get
			{
				return FrequencyRf + SpectrumFrequencySpan / new Decimal(20, 0, 0, false, (byte)1);
			}
		}

		public Decimal SpectrumFrequencyMin
		{
			get
			{
				return FrequencyRf - SpectrumFrequencySpan / new Decimal(20, 0, 0, false, (byte)1);
			}
		}

		public int SpectrumNbFrequenciesMax
		{
			get
			{
				return (int)((SpectrumFrequencyMax - SpectrumFrequencyMin) / SpectrumFrequencyStep);
			}
		}

		public Decimal SpectrumFrequencyStep
		{
			get
			{
				return (Decimal)BandwidthHz / new Decimal(30, 0, 0, false, (byte)1);
			}
		}

		public int SpectrumFrequencyId
		{
			get
			{
				return spectrumFreqId;
			}
			set
			{
				spectrumFreqId = value;
				OnPropertyChanged("SpectrumFreqId");
			}
		}

		public Decimal SpectrumRssiValue
		{
			get
			{
				return spectrumRssiValue;
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
				lowFrequencyModeOn = value;
				OnPropertyChanged("LowFrequencyModeOn");
			}
		}

		public bool AccessSharedFskReg
		{
			get
			{
				return accessSharedFskReg;
			}
			set
			{
				accessSharedFskReg = value;
				OnPropertyChanged("AccessSharedFskReg");
			}
		}

		public OperatingModeEnum Mode
		{
			get
			{
				return mode;
			}
			set
			{
				mode = value;
				OnPropertyChanged("Mode");
			}
		}

		public BandEnum Band
		{
			get
			{
				return band;
			}
			set
			{
				band = value;
				OnPropertyChanged("Band");
			}
		}

		public Decimal FrequencyRf
		{
			get
			{
				return frequencyRf;
			}
			set
			{
				frequencyRf = value;
				OnPropertyChanged("FrequencyRf");
				FrequencyRfCheck(value);
			}
		}

		public PaSelectEnum PaSelect
		{
			get
			{
				return paSelect;
			}
			set
			{
				paSelect = value;
				OnPropertyChanged("PaSelect");
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
				maxOutputPower = value;
				OnPropertyChanged("MaxOutputPower");
			}
		}

		public Decimal OutputPower
		{
			get
			{
				return outputPower;
			}
			set
			{
				outputPower = value;
				OnPropertyChanged("OutputPower");
			}
		}

		public bool ForceTxBandLowFrequencyOn
		{
			get
			{
				return forceTxBandLowFrequencyOn;
			}
			set
			{
				forceTxBandLowFrequencyOn = value;
				OnPropertyChanged("ForceTxBandLowFrequencyOn");
			}
		}

		public PaRampEnum PaRamp
		{
			get
			{
				return paRamp;
			}
			set
			{
				paRamp = value;
				OnPropertyChanged("PaRamp");
			}
		}

		public bool OcpOn
		{
			get
			{
				return ocpOn;
			}
			set
			{
				ocpOn = value;
				OnPropertyChanged("OcpOn");
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
				ocpTrim = value;
				OnPropertyChanged("OcpTrim");
			}
		}

		public LnaGainEnum LnaGain
		{
			get
			{
				return lnaGain;
			}
			set
			{
				lnaGain = value;
				OnPropertyChanged("LnaGain");
			}
		}

		public bool ForceRxBandLowFrequencyOn
		{
			get
			{
				return forceRxBandLowFrequencyOn;
			}
			set
			{
				forceRxBandLowFrequencyOn = value;
				OnPropertyChanged("ForceRxBandLowFrequencyOn");
			}
		}

		public bool LnaBoost
		{
			get
			{
				return lnaBoost;
			}
			set
			{
				lnaBoost = value;
				OnPropertyChanged("LnaBoost");
			}
		}

		public byte FifoAddrPtr
		{
			get
			{
				return fifoAddrPtr;
			}
			set
			{
				fifoAddrPtr = value;
				OnPropertyChanged("FifoAddrPtr");
			}
		}

		public byte FifoTxBaseAddr
		{
			get
			{
				return fifoTxBaseAddr;
			}
			set
			{
				fifoTxBaseAddr = value;
				OnPropertyChanged("FifoTxBaseAddr");
			}
		}

		public byte FifoRxBaseAddr
		{
			get
			{
				return fifoRxBaseAddr;
			}
			set
			{
				fifoRxBaseAddr = value;
				OnPropertyChanged("FifoRxBaseAddr");
			}
		}

		public byte FifoRxCurrentAddr
		{
			get
			{
				return fifoRxCurrentAddr;
			}
		}

		public bool RxTimeoutMask
		{
			get
			{
				return rxTimeoutMask;
			}
			set
			{
				rxTimeoutMask = value;
				OnPropertyChanged("RxTimeoutMask");
			}
		}

		public bool RxDoneMask
		{
			get
			{
				return rxDoneMask;
			}
			set
			{
				rxDoneMask = value;
				OnPropertyChanged("RxDoneMask");
			}
		}

		public bool PayloadCrcErrorMask
		{
			get
			{
				return payloadCrcErrorMask;
			}
			set
			{
				payloadCrcErrorMask = value;
				OnPropertyChanged("PayloadCrcErrorMask");
			}
		}

		public bool ValidHeaderMask
		{
			get
			{
				return validHeaderMask;
			}
			set
			{
				validHeaderMask = value;
				OnPropertyChanged("ValidHeaderMask");
			}
		}

		public bool TxDoneMask
		{
			get
			{
				return txDoneMask;
			}
			set
			{
				txDoneMask = value;
				OnPropertyChanged("TxDoneMask");
			}
		}

		public bool CadDoneMask
		{
			get
			{
				return cadDoneMask;
			}
			set
			{
				cadDoneMask = value;
				OnPropertyChanged("CadDoneMask");
			}
		}

		public bool FhssChangeChannelMask
		{
			get
			{
				return fhssChangeChannelMask;
			}
			set
			{
				fhssChangeChannelMask = value;
				OnPropertyChanged("FhssChangeChannelMask");
			}
		}

		public bool CadDetectedMask
		{
			get
			{
				return cadDetectedMask;
			}
			set
			{
				cadDetectedMask = value;
				OnPropertyChanged("CadDetectedMask");
			}
		}

		public bool RxTimeout
		{
			get
			{
				return rxTimeout;
			}
		}

		public bool RxDone
		{
			get
			{
				return rxDone;
			}
		}

		public bool PayloadCrcError
		{
			get
			{
				return payloadCrcError;
			}
		}

		public bool ValidHeader
		{
			get
			{
				return validHeader;
			}
		}

		public bool TxDone
		{
			get
			{
				return txDone;
			}
		}

		public bool CadDone
		{
			get
			{
				return cadDone;
			}
		}

		public bool FhssChangeChannel
		{
			get
			{
				return fhssChangeChannel;
			}
		}

		public bool CadDetected
		{
			get
			{
				return cadDetected;
			}
		}

		public byte RxNbBytes
		{
			get
			{
				return rxNbBytes;
			}
		}

		public ushort ValidHeaderCnt
		{
			get
			{
				return validHeaderCnt;
			}
		}

		public ushort ValidPacketCnt
		{
			get
			{
				return validPacketCnt;
			}
		}

		public byte RxPayloadCodingRate
		{
			get
			{
				return rxPayloadCodingRate;
			}
		}

		public bool ModemClear
		{
			get
			{
				return modemClear;
			}
		}

		public bool HeaderInfoValid
		{
			get
			{
				return headerInfoValid;
			}
		}

		public bool RxOnGoing
		{
			get
			{
				return rxOnGoing;
			}
		}

		public bool SignalSynchronized
		{
			get
			{
				return signalSynchronized;
			}
		}

		public bool SignalDetected
		{
			get
			{
				return signalDetected;
			}
		}

		public sbyte PktSnrValue
		{
			get
			{
				return pktSnrValue;
			}
		}

		public Decimal PktRssiValue
		{
			get
			{
				return pktRssiValue;
			}
		}

		public Decimal RssiValue
		{
			get
			{
				return rssiValue;
			}
		}

		public bool PllTimeout
		{
			get
			{
				return pllTimeout;
			}
		}

		public bool RxPayloadCrcOn
		{
			get
			{
				return rxPayloadCrcOn;
			}
		}

		public byte HopChannel
		{
			get
			{
				return hopChannel;
			}
		}

		public byte Bandwidth
		{
			get
			{
				return bandwidth;
			}
			set
			{
				bandwidth = value;
				OnPropertyChanged("Bandwidth");
				BandwidthCheck(value);
			}
		}

		public byte CodingRate
		{
			get
			{
				return codingRate;
			}
			set
			{
				codingRate = value;
				OnPropertyChanged("CodingRate");
			}
		}

		public bool ImplicitHeaderModeOn
		{
			get
			{
				return implicitHeaderModeOn;
			}
			set
			{
				implicitHeaderModeOn = value;
				OnPropertyChanged("ImplicitHeaderModeOn");
			}
		}

		public byte SpreadingFactor
		{
			get
			{
				return spreadingFactor;
			}
			set
			{
				spreadingFactor = value;
				OnPropertyChanged("SpreadingFactor");
			}
		}

		public bool TxContinuousModeOn
		{
			get
			{
				return txContinuousModeOn;
			}
			set
			{
				txContinuousModeOn = value;
				OnPropertyChanged("TxContinuousModeOn");
			}
		}

		public bool PayloadCrcOn
		{
			get
			{
				return payloadCrcOn;
			}
			set
			{
				payloadCrcOn = value;
				OnPropertyChanged("PayloadCrcOn");
			}
		}

		public Decimal SymbTimeout
		{
			get
			{
				return symbTimeout;
			}
			set
			{
				symbTimeout = value;
				OnPropertyChanged("SymbTimeout");
			}
		}

		public ushort PreambleLength
		{
			get
			{
				return preambleLength;
			}
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
				byte data = (byte)0;
				if (!Read((byte)66, ref data))
					throw new Exception("Unable to read register RegVersion");
				if (!Read((byte)66, ref data))
					throw new Exception("Unable to read register RegVersion");
				Version = new Version(((int)data & 240) >> 4, (int)data & 15);
			}
			registers = new RegisterCollection();
			byte num1 = (byte)0;
			RegisterCollection registerCollection1 = registers;
			string name1 = "RegFifo";
			int num2 = (int)num1;
			int num3 = 1;
			byte num4 = (byte)(num2 + num3);
			int num5 = 0;
			int num6 = 1;
			int num7 = 1;
			Register register1 = new Register(name1, (uint)num2, (uint)num5, num6 != 0, num7 != 0);
			registerCollection1.Add(register1);
			RegisterCollection registerCollection2 = registers;
			string name2 = "RegOpMode";
			int num8 = (int)num4;
			int num9 = 1;
			byte num10 = (byte)(num8 + num9);
			int num11 = 137;
			int num12 = 0;
			int num13 = 1;
			Register register2 = new Register(name2, (uint)num8, (uint)num11, num12 != 0, num13 != 0);
			registerCollection2.Add(register2);
			for (int index = 2; index < 4; ++index)
				registers.Add(new Register("RegRes" + num10.ToString("X02"), (uint)num10++, 0U, false, true));
			RegisterCollection registerCollection3 = registers;
			string name3 = "RegFdevMsb";
			int num14 = (int)num10;
			int num15 = 1;
			byte num16 = (byte)(num14 + num15);
			int num17 = 0;
			int num18 = 0;
			int num19 = 1;
			Register register3 = new Register(name3, (uint)num14, (uint)num17, num18 != 0, num19 != 0);
			registerCollection3.Add(register3);
			RegisterCollection registerCollection4 = registers;
			string name4 = "RegRes05";
			int num20 = (int)num16;
			int num21 = 1;
			byte num22 = (byte)(num20 + num21);
			int num23 = 82;
			int num24 = 0;
			int num25 = 1;
			Register register4 = new Register(name4, (uint)num20, (uint)num23, num24 != 0, num25 != 0);
			registerCollection4.Add(register4);
			RegisterCollection registerCollection5 = registers;
			string name5 = "RegFrfMsb";
			int num26 = (int)num22;
			int num27 = 1;
			byte num28 = (byte)(num26 + num27);
			int num29 = 228;
			int num30 = 0;
			int num31 = 1;
			Register register5 = new Register(name5, (uint)num26, (uint)num29, num30 != 0, num31 != 0);
			registerCollection5.Add(register5);
			RegisterCollection registerCollection6 = registers;
			string name6 = "RegFrfMid";
			int num32 = (int)num28;
			int num33 = 1;
			byte num34 = (byte)(num32 + num33);
			int num35 = 192;
			int num36 = 0;
			int num37 = 1;
			Register register6 = new Register(name6, (uint)num32, (uint)num35, num36 != 0, num37 != 0);
			registerCollection6.Add(register6);
			RegisterCollection registerCollection7 = registers;
			string name7 = "RegFrfLsb";
			int num38 = (int)num34;
			int num39 = 1;
			byte num40 = (byte)(num38 + num39);
			int num41 = 0;
			int num42 = 0;
			int num43 = 1;
			Register register7 = new Register(name7, (uint)num38, (uint)num41, num42 != 0, num43 != 0);
			registerCollection7.Add(register7);
			RegisterCollection registerCollection8 = registers;
			string name8 = "RegPaConfig";
			int num44 = (int)num40;
			int num45 = 1;
			byte num46 = (byte)(num44 + num45);
			int num47 = 15;
			int num48 = 0;
			int num49 = 1;
			Register register8 = new Register(name8, (uint)num44, (uint)num47, num48 != 0, num49 != 0);
			registerCollection8.Add(register8);
			RegisterCollection registerCollection9 = registers;
			string name9 = "RegPaRamp";
			int num50 = (int)num46;
			int num51 = 1;
			byte num52 = (byte)(num50 + num51);
			int num53 = 25;
			int num54 = 0;
			int num55 = 1;
			Register register9 = new Register(name9, (uint)num50, (uint)num53, num54 != 0, num55 != 0);
			registerCollection9.Add(register9);
			RegisterCollection registerCollection10 = registers;
			string name10 = "RegOcp";
			int num56 = (int)num52;
			int num57 = 1;
			byte num58 = (byte)(num56 + num57);
			int num59 = 43;
			int num60 = 0;
			int num61 = 1;
			Register register10 = new Register(name10, (uint)num56, (uint)num59, num60 != 0, num61 != 0);
			registerCollection10.Add(register10);
			RegisterCollection registerCollection11 = registers;
			string name11 = "RegLna";
			int num62 = (int)num58;
			int num63 = 1;
			byte num64 = (byte)(num62 + num63);
			int num65 = 32;
			int num66 = 0;
			int num67 = 1;
			Register register11 = new Register(name11, (uint)num62, (uint)num65, num66 != 0, num67 != 0);
			registerCollection11.Add(register11);
			RegisterCollection registerCollection12 = registers;
			string name12 = "RegFifoAddrPtr";
			int num68 = (int)num64;
			int num69 = 1;
			byte num70 = (byte)(num68 + num69);
			int num71 = 0;
			int num72 = 0;
			int num73 = 1;
			Register register12 = new Register(name12, (uint)num68, (uint)num71, num72 != 0, num73 != 0);
			registerCollection12.Add(register12);
			RegisterCollection registerCollection13 = registers;
			string name13 = "RegFifoTxBaseAddr";
			int num74 = (int)num70;
			int num75 = 1;
			byte num76 = (byte)(num74 + num75);
			int num77 = 128;
			int num78 = 0;
			int num79 = 1;
			Register register13 = new Register(name13, (uint)num74, (uint)num77, num78 != 0, num79 != 0);
			registerCollection13.Add(register13);
			RegisterCollection registerCollection14 = registers;
			string name14 = "RegFifoRxBaseAddr";
			int num80 = (int)num76;
			int num81 = 1;
			byte num82 = (byte)(num80 + num81);
			int num83 = 0;
			int num84 = 0;
			int num85 = 1;
			Register register14 = new Register(name14, (uint)num80, (uint)num83, num84 != 0, num85 != 0);
			registerCollection14.Add(register14);
			RegisterCollection registerCollection15 = registers;
			string name15 = "RegFifoRxCurrentAddr";
			int num86 = (int)num82;
			int num87 = 1;
			byte num88 = (byte)(num86 + num87);
			int num89 = 0;
			int num90 = 1;
			int num91 = 1;
			Register register15 = new Register(name15, (uint)num86, (uint)num89, num90 != 0, num91 != 0);
			registerCollection15.Add(register15);
			RegisterCollection registerCollection16 = registers;
			string name16 = "RegIrqFlagsMask";
			int num92 = (int)num88;
			int num93 = 1;
			byte num94 = (byte)(num92 + num93);
			int num95 = 0;
			int num96 = 0;
			int num97 = 1;
			Register register16 = new Register(name16, (uint)num92, (uint)num95, num96 != 0, num97 != 0);
			registerCollection16.Add(register16);
			RegisterCollection registerCollection17 = registers;
			string name17 = "RegIrqFlags";
			int num98 = (int)num94;
			int num99 = 1;
			byte num100 = (byte)(num98 + num99);
			int num101 = 0;
			int num102 = 0;
			int num103 = 1;
			Register register17 = new Register(name17, (uint)num98, (uint)num101, num102 != 0, num103 != 0);
			registerCollection17.Add(register17);
			RegisterCollection registerCollection18 = registers;
			string name18 = "RegRxNbBytes";
			int num104 = (int)num100;
			int num105 = 1;
			byte num106 = (byte)(num104 + num105);
			int num107 = 0;
			int num108 = 1;
			int num109 = 1;
			Register register18 = new Register(name18, (uint)num104, (uint)num107, num108 != 0, num109 != 0);
			registerCollection18.Add(register18);
			RegisterCollection registerCollection19 = registers;
			string name19 = "RegRxHeaderCntValueMsb";
			int num110 = (int)num106;
			int num111 = 1;
			byte num112 = (byte)(num110 + num111);
			int num113 = 0;
			int num114 = 1;
			int num115 = 1;
			Register register19 = new Register(name19, (uint)num110, (uint)num113, num114 != 0, num115 != 0);
			registerCollection19.Add(register19);
			RegisterCollection registerCollection20 = registers;
			string name20 = "RegRxHeaderCntValueLsb";
			int num116 = (int)num112;
			int num117 = 1;
			byte num118 = (byte)(num116 + num117);
			int num119 = 0;
			int num120 = 1;
			int num121 = 1;
			Register register20 = new Register(name20, (uint)num116, (uint)num119, num120 != 0, num121 != 0);
			registerCollection20.Add(register20);
			RegisterCollection registerCollection21 = registers;
			string name21 = "RegRxPacketCntValueMsb";
			int num122 = (int)num118;
			int num123 = 1;
			byte num124 = (byte)(num122 + num123);
			int num125 = 0;
			int num126 = 1;
			int num127 = 1;
			Register register21 = new Register(name21, (uint)num122, (uint)num125, num126 != 0, num127 != 0);
			registerCollection21.Add(register21);
			RegisterCollection registerCollection22 = registers;
			string name22 = "RegRxPacketCntValueLsb";
			int num128 = (int)num124;
			int num129 = 1;
			byte num130 = (byte)(num128 + num129);
			int num131 = 0;
			int num132 = 1;
			int num133 = 1;
			Register register22 = new Register(name22, (uint)num128, (uint)num131, num132 != 0, num133 != 0);
			registerCollection22.Add(register22);
			RegisterCollection registerCollection23 = registers;
			string name23 = "RegModemStat";
			int num134 = (int)num130;
			int num135 = 1;
			byte num136 = (byte)(num134 + num135);
			int num137 = 0;
			int num138 = 1;
			int num139 = 1;
			Register register23 = new Register(name23, (uint)num134, (uint)num137, num138 != 0, num139 != 0);
			registerCollection23.Add(register23);
			RegisterCollection registerCollection24 = registers;
			string name24 = "RegPktSnrValue";
			int num140 = (int)num136;
			int num141 = 1;
			byte num142 = (byte)(num140 + num141);
			int num143 = 0;
			int num144 = 1;
			int num145 = 1;
			Register register24 = new Register(name24, (uint)num140, (uint)num143, num144 != 0, num145 != 0);
			registerCollection24.Add(register24);
			RegisterCollection registerCollection25 = registers;
			string name25 = "RegPktRssiValue";
			int num146 = (int)num142;
			int num147 = 1;
			byte num148 = (byte)(num146 + num147);
			int num149 = 0;
			int num150 = 1;
			int num151 = 1;
			Register register25 = new Register(name25, (uint)num146, (uint)num149, num150 != 0, num151 != 0);
			registerCollection25.Add(register25);
			RegisterCollection registerCollection26 = registers;
			string name26 = "RegRssiValue";
			int num152 = (int)num148;
			int num153 = 1;
			byte num154 = (byte)(num152 + num153);
			int num155 = 0;
			int num156 = 1;
			int num157 = 1;
			Register register26 = new Register(name26, (uint)num152, (uint)num155, num156 != 0, num157 != 0);
			registerCollection26.Add(register26);
			RegisterCollection registerCollection27 = registers;
			string name27 = "RegHopChannel";
			int num158 = (int)num154;
			int num159 = 1;
			byte num160 = (byte)(num158 + num159);
			int num161 = 0;
			int num162 = 1;
			int num163 = 1;
			Register register27 = new Register(name27, (uint)num158, (uint)num161, num162 != 0, num163 != 0);
			registerCollection27.Add(register27);
			RegisterCollection registerCollection28 = registers;
			string name28 = "RegModemConfig1";
			int num164 = (int)num160;
			int num165 = 1;
			byte num166 = (byte)(num164 + num165);
			int num167 = 114;
			int num168 = 0;
			int num169 = 1;
			Register register28 = new Register(name28, (uint)num164, (uint)num167, num168 != 0, num169 != 0);
			registerCollection28.Add(register28);
			RegisterCollection registerCollection29 = registers;
			string name29 = "RegModemConfig2";
			int num170 = (int)num166;
			int num171 = 1;
			byte num172 = (byte)(num170 + num171);
			int num173 = 112;
			int num174 = 0;
			int num175 = 1;
			Register register29 = new Register(name29, (uint)num170, (uint)num173, num174 != 0, num175 != 0);
			registerCollection29.Add(register29);
			RegisterCollection registerCollection30 = registers;
			string name30 = "RegSymbTimeoutLsb";
			int num176 = (int)num172;
			int num177 = 1;
			byte num178 = (byte)(num176 + num177);
			int num179 = 100;
			int num180 = 0;
			int num181 = 1;
			Register register30 = new Register(name30, (uint)num176, (uint)num179, num180 != 0, num181 != 0);
			registerCollection30.Add(register30);
			RegisterCollection registerCollection31 = registers;
			string name31 = "RegPreambleMsb";
			int num182 = (int)num178;
			int num183 = 1;
			byte num184 = (byte)(num182 + num183);
			int num185 = 0;
			int num186 = 0;
			int num187 = 1;
			Register register31 = new Register(name31, (uint)num182, (uint)num185, num186 != 0, num187 != 0);
			registerCollection31.Add(register31);
			RegisterCollection registerCollection32 = registers;
			string name32 = "RegPreambleLsb";
			int num188 = (int)num184;
			int num189 = 1;
			byte num190 = (byte)(num188 + num189);
			int num191 = 8;
			int num192 = 0;
			int num193 = 1;
			Register register32 = new Register(name32, (uint)num188, (uint)num191, num192 != 0, num193 != 0);
			registerCollection32.Add(register32);
			RegisterCollection registerCollection33 = registers;
			string name33 = "RegPayloadLength";
			int num194 = (int)num190;
			int num195 = 1;
			byte num196 = (byte)(num194 + num195);
			int num197 = 1;
			int num198 = 0;
			int num199 = 1;
			Register register33 = new Register(name33, (uint)num194, (uint)num197, num198 != 0, num199 != 0);
			registerCollection33.Add(register33);
			RegisterCollection registerCollection34 = registers;
			string name34 = "RegMaxPayloadLength";
			int num200 = (int)num196;
			int num201 = 1;
			byte num202 = (byte)(num200 + num201);
			int num203 = (int)byte.MaxValue;
			int num204 = 0;
			int num205 = 1;
			Register register34 = new Register(name34, (uint)num200, (uint)num203, num204 != 0, num205 != 0);
			registerCollection34.Add(register34);
			RegisterCollection registerCollection35 = registers;
			string name35 = "RegHopPeriod";
			int num206 = (int)num202;
			int num207 = 1;
			byte num208 = (byte)(num206 + num207);
			int num209 = 0;
			int num210 = 0;
			int num211 = 1;
			Register register35 = new Register(name35, (uint)num206, (uint)num209, num210 != 0, num211 != 0);
			registerCollection35.Add(register35);
			RegisterCollection registerCollection36 = registers;
			string name36 = "RegFifoRxByteAddr";
			int num212 = (int)num208;
			int num213 = 1;
			byte num214 = (byte)(num212 + num213);
			int num215 = 0;
			int num216 = 0;
			int num217 = 1;
			Register register36 = new Register(name36, (uint)num212, (uint)num215, num216 != 0, num217 != 0);
			registerCollection36.Add(register36);
			RegisterCollection registerCollection37 = registers;
			string name37 = "RegModemConfig3";
			int num218 = (int)num214;
			int num219 = 1;
			byte num220 = (byte)(num218 + num219);
			int num221 = 4;
			int num222 = 0;
			int num223 = 1;
			Register register37 = new Register(name37, (uint)num218, (uint)num221, num222 != 0, num223 != 0);
			registerCollection37.Add(register37);
			for (int index = 39; index < 64; ++index)
				registers.Add(new Register("RegTest" + num220.ToString("X02"), (uint)num220++, 0U, false, true));
			RegisterCollection registerCollection38 = registers;
			string name38 = "RegDioMapping1";
			int num224 = (int)num220;
			int num225 = 1;
			byte num226 = (byte)(num224 + num225);
			int num227 = 0;
			int num228 = 0;
			int num229 = 1;
			Register register38 = new Register(name38, (uint)num224, (uint)num227, num228 != 0, num229 != 0);
			registerCollection38.Add(register38);
			RegisterCollection registerCollection39 = registers;
			string name39 = "RegDioMapping2";
			int num230 = (int)num226;
			int num231 = 1;
			byte num232 = (byte)(num230 + num231);
			int num233 = 0;
			int num234 = 0;
			int num235 = 1;
			Register register39 = new Register(name39, (uint)num230, (uint)num233, num234 != 0, num235 != 0);
			registerCollection39.Add(register39);
			RegisterCollection registerCollection40 = registers;
			string name40 = "RegVersion";
			int num236 = (int)num232;
			int num237 = 1;
			byte num238 = (byte)(num236 + num237);
			int num239 = 17;
			int num240 = 0;
			int num241 = 1;
			Register register40 = new Register(name40, (uint)num236, (uint)num239, num240 != 0, num241 != 0);
			registerCollection40.Add(register40);
			for (int index = 67; index < 68; ++index)
				registers.Add(new Register("RegTest" + num238.ToString("X02"), (uint)num238++, 0U, false, true));
			RegisterCollection registerCollection41 = registers;
			string name41 = "RegPllHop";
			int num242 = (int)num238;
			int num243 = 1;
			byte num244 = (byte)(num242 + num243);
			int num245 = 45;
			int num246 = 0;
			int num247 = 1;
			Register register41 = new Register(name41, (uint)num242, (uint)num245, num246 != 0, num247 != 0);
			registerCollection41.Add(register41);
			for (int index = 69; index < 75; ++index)
				registers.Add(new Register("RegTest" + num244.ToString("X02"), (uint)num244++, 0U, false, true));
			RegisterCollection registerCollection42 = registers;
			string name42 = "RegTcxo";
			int num248 = (int)num244;
			int num249 = 1;
			byte num250 = (byte)(num248 + num249);
			int num251 = 9;
			int num252 = 0;
			int num253 = 1;
			Register register42 = new Register(name42, (uint)num248, (uint)num251, num252 != 0, num253 != 0);
			registerCollection42.Add(register42);
			RegisterCollection registerCollection43 = registers;
			string name43 = "RegTest4C";
			int num254 = (int)num250;
			int num255 = 1;
			byte num256 = (byte)(num254 + num255);
			int num257 = 0;
			int num258 = 0;
			int num259 = 1;
			Register register43 = new Register(name43, (uint)num254, (uint)num257, num258 != 0, num259 != 0);
			registerCollection43.Add(register43);
			RegisterCollection registerCollection44 = registers;
			string name44 = "RegPaDac";
			int num260 = (int)num256;
			int num261 = 1;
			byte num262 = (byte)(num260 + num261);
			int num263 = 132;
			int num264 = 0;
			int num265 = 1;
			Register register44 = new Register(name44, (uint)num260, (uint)num263, num264 != 0, num265 != 0);
			registerCollection44.Add(register44);
			for (int index = 78; index < 91; ++index)
				registers.Add(new Register("RegTest" + num262.ToString("X02"), (uint)num262++, 0U, false, true));
			RegisterCollection registerCollection45 = registers;
			string name45 = "RegFormerTemp";
			int num266 = (int)num262;
			int num267 = 1;
			byte num268 = (byte)(num266 + num267);
			int num269 = 0;
			int num270 = 0;
			int num271 = 1;
			Register register45 = new Register(name45, (uint)num266, (uint)num269, num270 != 0, num271 != 0);
			registerCollection45.Add(register45);
			RegisterCollection registerCollection46 = registers;
			string name46 = "RegTest5C";
			int num272 = (int)num268;
			int num273 = 1;
			byte num274 = (byte)(num272 + num273);
			int num275 = 0;
			int num276 = 0;
			int num277 = 1;
			Register register46 = new Register(name46, (uint)num272, (uint)num275, num276 != 0, num277 != 0);
			registerCollection46.Add(register46);
			RegisterCollection registerCollection47 = registers;
			string name47 = "RegBitrateFrac";
			int num278 = (int)num274;
			int num279 = 1;
			byte num280 = (byte)(num278 + num279);
			int num281 = 0;
			int num282 = 0;
			int num283 = 1;
			Register register47 = new Register(name47, (uint)num278, (uint)num281, num282 != 0, num283 != 0);
			registerCollection47.Add(register47);
			for (int index = 94; index < 97; ++index)
				registers.Add(new Register("RegTest" + num280.ToString("X02"), (uint)num280++, 0U, false, true));
			RegisterCollection registerCollection48 = registers;
			string name48 = "RegAgcRef";
			int num284 = (int)num280;
			int num285 = 1;
			byte num286 = (byte)(num284 + num285);
			int num287 = 25;
			int num288 = 0;
			int num289 = 1;
			Register register48 = new Register(name48, (uint)num284, (uint)num287, num288 != 0, num289 != 0);
			registerCollection48.Add(register48);
			RegisterCollection registerCollection49 = registers;
			string name49 = "RegAgcThresh1";
			int num290 = (int)num286;
			int num291 = 1;
			byte num292 = (byte)(num290 + num291);
			int num293 = 12;
			int num294 = 0;
			int num295 = 1;
			Register register49 = new Register(name49, (uint)num290, (uint)num293, num294 != 0, num295 != 0);
			registerCollection49.Add(register49);
			RegisterCollection registerCollection50 = registers;
			string name50 = "RegAgcThresh2";
			int num296 = (int)num292;
			int num297 = 1;
			byte num298 = (byte)(num296 + num297);
			int num299 = 75;
			int num300 = 0;
			int num301 = 1;
			Register register50 = new Register(name50, (uint)num296, (uint)num299, num300 != 0, num301 != 0);
			registerCollection50.Add(register50);
			RegisterCollection registerCollection51 = registers;
			string name51 = "RegAgcThresh3";
			int num302 = (int)num298;
			int num303 = 1;
			byte num304 = (byte)(num302 + num303);
			int num305 = 204;
			int num306 = 0;
			int num307 = 1;
			Register register51 = new Register(name51, (uint)num302, (uint)num305, num306 != 0, num307 != 0);
			registerCollection51.Add(register51);
			for (int index = 101; index < 112; ++index)
				registers.Add(new Register("RegTest" + num304.ToString("X02"), (uint)num304++, 0U, false, true));
			RegisterCollection registerCollection52 = registers;
			string name52 = "RegPll";
			int num308 = (int)num304;
			int num309 = 1;
			byte num310 = (byte)(num308 + num309);
			int num311 = 208;
			int num312 = 0;
			int num313 = 1;
			Register register52 = new Register(name52, (uint)num308, (uint)num311, num312 != 0, num313 != 0);
			registerCollection52.Add(register52);
			for (int index = 113; index < 128; ++index)
				registers.Add(new Register("RegTest" + num310.ToString("X02"), (uint)num310++, 0U, false, true));
			foreach (Register register53 in registers)
				register53.PropertyChanged += new PropertyChangedEventHandler(registers_PropertyChanged);
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
					Console.WriteLine("RegOpMode (URV): 0x{0}", (object)r.Value.ToString("X02"));
					Mode = ((int)registers["RegModemConfig2"].Value & 8) != 8 || ((int)r.Value & 7) != 3 ? (OperatingModeEnum)((int)r.Value & 7) : OperatingModeEnum.TxContinuous;
					if ((int)registers["RegPayloadLength"].Value != (int)PayloadLength)
						registers["RegPayloadLength"].Value = (uint)PayloadLength;
					lock (syncThread)
					{
						SetModeLeds(Mode);
						break;
					}
				case "RegFdevMsb":
					Band = (BandEnum)((int)(registers["RegFdevMsb"].Value >> 6) & 3);
					break;
				case "RegFrfMsb":
				case "RegFrfMid":
				case "RegFrfLsb":
					FrequencyRf = (Decimal)((uint)((int)registers["RegFrfMsb"].Value << 16 | (int)registers["RegFrfMid"].Value << 8) | registers["RegFrfLsb"].Value) * FrequencyStep;
					BandwidthCheck(Bandwidth);
					break;
				case "RegPaConfig":
					PaSelect = ((int)r.Value & 128) == 128 ? PaSelectEnum.PA_BOOST : PaSelectEnum.RFO;
					if (PaSelect == PaSelectEnum.RFO)
					{
						maxOutputPower = new Decimal(108, 0, 0, false, (byte)1) + new Decimal(6, 0, 0, false, (byte)1) * (Decimal)(r.Value >> 4 & 7U);
						outputPower = MaxOutputPower - new Decimal(150, 0, 0, false, (byte)1) - (Decimal)(r.Value & 15U);
					}
					else if (!Pa20dBm)
					{
						maxOutputPower = new Decimal(17);
						outputPower = new Decimal(17) - new Decimal(150, 0, 0, false, (byte)1) - (Decimal)(r.Value & 15U);
					}
					else
					{
						maxOutputPower = new Decimal(20);
						outputPower = new Decimal(20) - new Decimal(150, 0, 0, false, (byte)1) - (Decimal)(r.Value & 15U);
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
					OcpTrim = (r.Value & 31U) > 15U ? ((r.Value & 31U) <= 15U || (r.Value & 31U) > 27U ? new Decimal(2400, 0, 0, false, (byte)1) : (Decimal)(-30L + (long)(uint)(10 * ((int)r.Value & 31)))) : (Decimal)((uint)(45 + 5 * ((int)r.Value & 15)));
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
								rfPaRssiValue = new Decimal(1277, 0, 0, true, (byte)1);
							rfIoRssiValue = rssiValue;
							OnPropertyChanged("RfIoRssiValue");
						}
						else if (RfPaSwitchSel == RfPaSwitchSelEnum.RF_IO_PA_BOOST)
						{
							if (RfPaSwitchEnabled == 1)
								rfIoRssiValue = new Decimal(1277, 0, 0, true, (byte)1);
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
			if (Connected == null)
				return;
			Connected((object)this, EventArgs.Empty);
		}

		private void OnDisconnected()
		{
			if (Disconected == null)
				return;
			Disconected((object)this, EventArgs.Empty);
		}

		private void OnError(byte status, string message)
		{
			if (Error == null)
				return;
			Error((object)this, new SemtechLib.General.Events.ErrorEventArgs(status, message));
		}

		private void OnOcpTrimLimitStatusChanged(LimitCheckStatusEnum status, string message)
		{
			if (OcpTrimLimitStatusChanged == null)
				return;
			OcpTrimLimitStatusChanged((object)this, new LimitCheckStatusEventArg(status, message));
		}

		private void OnFrequencyRfLimitStatusChanged(LimitCheckStatusEnum status, string message)
		{
			if (FrequencyRfLimitStatusChanged == null)
				return;
			FrequencyRfLimitStatusChanged((object)this, new LimitCheckStatusEventArg(status, message));
		}

		private void OnBandwidthLimitStatusChanged(LimitCheckStatusEnum status, string message)
		{
			if (BandwidthLimitStatusChanged == null)
				return;
			BandwidthLimitStatusChanged((object)this, new LimitCheckStatusEventArg(status, message));
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

		public bool SKReset()
		{
			lock (syncThread)
			{
				byte local_0 = (byte)0;
				SX1276LR.HidCommandsStatus local_1 = SX1276LR.HidCommandsStatus.SX_ERROR;
				ulong local_2 = ulong.MaxValue;
				byte[] local_3 = new byte[10];
				byte[] local_4 = new byte[2];
				try
				{
					if (IsOpen)
					{
						usbDevice.TxRxCommand(local_0, local_4, ref local_3);
						local_1 = (SX1276LR.HidCommandsStatus)local_3[0];
						local_2 = (ulong)((long)local_3[1] << 56 | (long)local_3[2] << 48 | (long)local_3[3] << 40 | (long)local_3[4] << 32 | (long)local_3[5] << 24 | (long)local_3[6] << 16 | (long)local_3[7] << 8) | (ulong)local_3[8];
						if (local_1 == SX1276LR.HidCommandsStatus.SX_OK)
							return true;
					}
					return false;
				}
				finally
				{
					Console.WriteLine("{0} ms: {1} with status {2}", (object)local_2, (object)Enum.GetName(typeof(SX1276LR.HidCommands), (object)(SX1276LR.HidCommands)local_0), (object)Enum.GetName(typeof(SX1276LR.HidCommandsStatus), (object)local_1));
				}
			}
		}

		public bool SKGetVersion()
		{
			lock (syncThread)
			{
				byte local_0 = (byte)1;
				SX1276LR.HidCommandsStatus local_1 = SX1276LR.HidCommandsStatus.SX_ERROR;
				ulong local_2 = ulong.MaxValue;
				byte[] local_3 = new byte[17];
				byte[] local_4 = new byte[2];
				try
				{
					usbDevice.TxRxCommand(local_0, local_4, ref local_3);
					local_1 = (SX1276LR.HidCommandsStatus)local_3[0];
					local_2 = (ulong)((long)local_3[1] << 56 | (long)local_3[2] << 48 | (long)local_3[3] << 40 | (long)local_3[4] << 32 | (long)local_3[5] << 24 | (long)local_3[6] << 16 | (long)local_3[7] << 8) | (ulong)local_3[8];
					if (local_1 != SX1276LR.HidCommandsStatus.SX_OK || (int)local_3[9] < 5)
						return false;
					Array.Copy((Array)local_3, 10, (Array)local_3, 0, (int)local_3[9]);
					Array.Resize<byte>(ref local_3, (int)local_3[9]);
					string local_6 = new ASCIIEncoding().GetString(local_3);
					fwVersion = local_6.Length <= 5 ? new Version(local_6) : new Version(local_6.Remove(4, 1));
					return true;
				}
				finally
				{
					Console.WriteLine("{0} ms: {1} with status {2}", (object)local_2, (object)Enum.GetName(typeof(SX1276LR.HidCommands), (object)(SX1276LR.HidCommands)local_0), (object)Enum.GetName(typeof(SX1276LR.HidCommandsStatus), (object)local_1));
				}
			}
		}

		public bool SKGetName()
		{
			lock (syncThread)
			{
				byte local_0 = (byte)2;
				SX1276LR.HidCommandsStatus local_1 = SX1276LR.HidCommandsStatus.SX_ERROR;
				ulong local_2 = ulong.MaxValue;
				byte[] local_3 = new byte[25];
				byte[] local_4 = new byte[2];
				try
				{
					usbDevice.TxRxCommand(local_0, local_4, ref local_3);
					local_1 = (SX1276LR.HidCommandsStatus)local_3[0];
					local_2 = (ulong)((long)local_3[1] << 56 | (long)local_3[2] << 48 | (long)local_3[3] << 40 | (long)local_3[4] << 32 | (long)local_3[5] << 24 | (long)local_3[6] << 16 | (long)local_3[7] << 8) | (ulong)local_3[8];
					if (local_1 == SX1276LR.HidCommandsStatus.SX_OK && (int)local_3[9] >= 9)
					{
						Array.Copy((Array)local_3, 10, (Array)local_3, 0, 10);
						Array.Resize<byte>(ref local_3, 9);
						deviceName = new ASCIIEncoding().GetString(local_3);
						return true;
					}
					deviceName = string.Empty;
					return false;
				}
				finally
				{
					Console.WriteLine("{0} ms: {1} with status {2}", (object)local_2, (object)Enum.GetName(typeof(SX1276LR.HidCommands), (object)(SX1276LR.HidCommands)local_0), (object)Enum.GetName(typeof(SX1276LR.HidCommandsStatus), (object)local_1));
				}
			}
		}

		public bool SKGetId(ref byte id)
		{
			lock (syncThread)
			{
				byte local_0 = (byte)3;
				byte[] local_2 = new byte[11];
				byte[] local_3 = new byte[2];
				usbDevice.TxRxCommand(local_0, local_3, ref local_2);
				SX1276LR.HidCommandsStatus local_1_1 = (SX1276LR.HidCommandsStatus)local_2[0];
				int temp_22 = (int)local_2[1];
				int temp_25 = (int)local_2[2];
				int temp_28 = (int)local_2[3];
				int temp_31 = (int)local_2[4];
				int temp_34 = (int)local_2[5];
				int temp_37 = (int)local_2[6];
				int temp_40 = (int)local_2[7];
				int temp_43 = (int)local_2[8];
				if (local_1_1 != SX1276LR.HidCommandsStatus.SX_OK || (int)local_2[9] != 1)
					return false;
				id = local_2[10];
				return true;
			}
		}

		public bool SKSetId(byte id)
		{
			lock (syncThread)
			{
				byte local_0 = (byte)4;
				byte[] local_2 = new byte[10];
				byte[] local_3 = new byte[3]
        {
          (byte) 0,
          (byte) 1,
          id
        };
				usbDevice.TxRxCommand(local_0, local_3, ref local_2);
				SX1276LR.HidCommandsStatus local_1_1 = (SX1276LR.HidCommandsStatus)local_2[0];
				int temp_28 = (int)local_2[1];
				int temp_31 = (int)local_2[2];
				int temp_34 = (int)local_2[3];
				int temp_37 = (int)local_2[4];
				int temp_40 = (int)local_2[5];
				int temp_43 = (int)local_2[6];
				int temp_46 = (int)local_2[7];
				int temp_49 = (int)local_2[8];
				return local_1_1 == SX1276LR.HidCommandsStatus.SX_OK;
			}
		}

		public bool SKSetRndId(ref byte id)
		{
			lock (syncThread)
			{
				byte local_0 = (byte)5;
				byte[] local_2 = new byte[11];
				byte[] local_3 = new byte[2];
				usbDevice.TxRxCommand(local_0, local_3, ref local_2);
				SX1276LR.HidCommandsStatus local_1_1 = (SX1276LR.HidCommandsStatus)local_2[0];
				int temp_22 = (int)local_2[1];
				int temp_25 = (int)local_2[2];
				int temp_28 = (int)local_2[3];
				int temp_31 = (int)local_2[4];
				int temp_34 = (int)local_2[5];
				int temp_37 = (int)local_2[6];
				int temp_40 = (int)local_2[7];
				int temp_43 = (int)local_2[8];
				if (local_1_1 != SX1276LR.HidCommandsStatus.SX_OK || (int)local_2[9] != 1)
					return false;
				id = local_2[10];
				return true;
			}
		}

		public bool SKGetPinState(byte pinId, ref byte state)
		{
			lock (syncThread)
			{
				byte local_0 = (byte)16;
				byte[] local_2 = new byte[11];
				byte[] local_3 = new byte[3]
        {
          (byte) 0,
          (byte) 1,
          pinId
        };
				usbDevice.TxRxCommand(local_0, local_3, ref local_2);
				SX1276LR.HidCommandsStatus local_1_1 = (SX1276LR.HidCommandsStatus)local_2[0];
				int temp_28 = (int)local_2[1];
				int temp_31 = (int)local_2[2];
				int temp_34 = (int)local_2[3];
				int temp_37 = (int)local_2[4];
				int temp_40 = (int)local_2[5];
				int temp_43 = (int)local_2[6];
				int temp_46 = (int)local_2[7];
				int temp_49 = (int)local_2[8];
				if (local_1_1 != SX1276LR.HidCommandsStatus.SX_OK || (int)local_2[9] != 1)
					return false;
				state = local_2[10];
				return true;
			}
		}

		public bool SKSetPinState(byte pinId, byte state)
		{
			lock (syncThread)
			{
				byte local_0 = (byte)17;
				byte[] local_2 = new byte[10];
				byte[] local_3 = new byte[4]
        {
          (byte) 0,
          (byte) 2,
          pinId,
          state
        };
				usbDevice.TxRxCommand(local_0, local_3, ref local_2);
				SX1276LR.HidCommandsStatus local_1_1 = (SX1276LR.HidCommandsStatus)local_2[0];
				int temp_31 = (int)local_2[1];
				int temp_34 = (int)local_2[2];
				int temp_37 = (int)local_2[3];
				int temp_40 = (int)local_2[4];
				int temp_43 = (int)local_2[5];
				int temp_46 = (int)local_2[6];
				int temp_49 = (int)local_2[7];
				int temp_52 = (int)local_2[8];
				return local_1_1 == SX1276LR.HidCommandsStatus.SX_OK;
			}
		}

		public bool SKGetPinDir(byte pinId, ref byte dir)
		{
			lock (syncThread)
			{
				byte local_0 = (byte)18;
				byte[] local_2 = new byte[11];
				byte[] local_3 = new byte[3]
        {
          (byte) 0,
          (byte) 1,
          pinId
        };
				usbDevice.TxRxCommand(local_0, local_3, ref local_2);
				SX1276LR.HidCommandsStatus local_1_1 = (SX1276LR.HidCommandsStatus)local_2[0];
				int temp_28 = (int)local_2[1];
				int temp_31 = (int)local_2[2];
				int temp_34 = (int)local_2[3];
				int temp_37 = (int)local_2[4];
				int temp_40 = (int)local_2[5];
				int temp_43 = (int)local_2[6];
				int temp_46 = (int)local_2[7];
				int temp_49 = (int)local_2[8];
				if (local_1_1 != SX1276LR.HidCommandsStatus.SX_OK || (int)local_2[9] != 1)
					return false;
				dir = local_2[10];
				return true;
			}
		}

		public bool SKSetPinDir(byte pinId, byte dir)
		{
			lock (syncThread)
			{
				byte local_0 = (byte)19;
				byte[] local_2 = new byte[10];
				byte[] local_3 = new byte[4]
        {
          (byte) 0,
          (byte) 2,
          pinId,
          dir
        };
				usbDevice.TxRxCommand(local_0, local_3, ref local_2);
				SX1276LR.HidCommandsStatus local_1_1 = (SX1276LR.HidCommandsStatus)local_2[0];
				int temp_31 = (int)local_2[1];
				int temp_34 = (int)local_2[2];
				int temp_37 = (int)local_2[3];
				int temp_40 = (int)local_2[4];
				int temp_43 = (int)local_2[5];
				int temp_46 = (int)local_2[6];
				int temp_49 = (int)local_2[7];
				int temp_52 = (int)local_2[8];
				return local_1_1 == SX1276LR.HidCommandsStatus.SX_OK;
			}
		}

		public bool SKGetPinsState(ref byte pinsState)
		{
			lock (syncThread)
			{
				byte local_0 = (byte)20;
				byte[] local_2 = new byte[11];
				byte[] local_3 = new byte[2];
				usbDevice.TxRxCommand(local_0, local_3, ref local_2);
				SX1276LR.HidCommandsStatus local_1_1 = (SX1276LR.HidCommandsStatus)local_2[0];
				int temp_22 = (int)local_2[1];
				int temp_25 = (int)local_2[2];
				int temp_28 = (int)local_2[3];
				int temp_31 = (int)local_2[4];
				int temp_34 = (int)local_2[5];
				int temp_37 = (int)local_2[6];
				int temp_40 = (int)local_2[7];
				int temp_43 = (int)local_2[8];
				if (local_1_1 != SX1276LR.HidCommandsStatus.SX_OK || (int)local_2[9] != 1)
					return false;
				pinsState = local_2[10];
				return true;
			}
		}

		public bool SKReadEeprom(byte id, byte address, ref byte[] data)
		{
			lock (syncThread)
			{
				byte local_0 = (byte)112;
				byte[] local_2 = new byte[42];
				byte[] local_3 = new byte[5]
        {
          (byte) 0,
          (byte) 3,
          (byte) 32,
          id,
          address
        };
				int local_4 = (int)address;
				while (local_4 < data.Length)
				{
					local_3[4] = (byte)local_4;
					usbDevice.TxRxCommand(local_0, local_3, ref local_2);
					SX1276LR.HidCommandsStatus local_1_1 = (SX1276LR.HidCommandsStatus)local_2[0];
					int temp_44 = (int)local_2[1];
					int temp_47 = (int)local_2[2];
					int temp_50 = (int)local_2[3];
					int temp_53 = (int)local_2[4];
					int temp_56 = (int)local_2[5];
					int temp_59 = (int)local_2[6];
					int temp_62 = (int)local_2[7];
					int temp_65 = (int)local_2[8];
					if (local_1_1 == SX1276LR.HidCommandsStatus.SX_OK)
					{
						if (32 == (int)local_2[9])
						{
							Array.Copy((Array)local_2, 10, (Array)data, local_4, 32);
						}
						else
						{
							data = (byte[])null;
							return false;
						}
					}
					local_4 += 32;
				}
				return true;
			}
		}

		public bool SKWriteEeprom(byte id, byte address, byte[] data)
		{
			lock (syncThread)
			{
				byte local_0 = (byte)113;
				byte[] local_2 = new byte[10];
				byte[] local_3 = new byte[37];
				local_3[0] = (byte)0;
				local_3[1] = (byte)35;
				local_3[2] = (byte)32;
				local_3[3] = id;
				int local_4 = (int)address;
				while (local_4 < data.Length)
				{
					local_3[4] = (byte)local_4;
					Array.Copy((Array)data, local_4, (Array)local_3, 5, 32);
					usbDevice.TxRxCommand(local_0, local_3, ref local_2);
					SX1276LR.HidCommandsStatus local_1_1 = (SX1276LR.HidCommandsStatus)local_2[0];
					int temp_47 = (int)local_2[1];
					int temp_50 = (int)local_2[2];
					int temp_53 = (int)local_2[3];
					int temp_56 = (int)local_2[4];
					int temp_59 = (int)local_2[5];
					int temp_62 = (int)local_2[6];
					int temp_65 = (int)local_2[7];
					int temp_68 = (int)local_2[8];
					if (local_1_1 != SX1276LR.HidCommandsStatus.SX_OK)
						return false;
					local_4 += 32;
				}
				return true;
			}
		}

		public bool SKDeviceRead(byte address, ref byte[] data)
		{
			lock (syncThread)
			{
				byte command = 0x80;
				byte[] inData = new byte[10 + data.Length];
				byte[] outData = new byte[4]
				{
					0,
					(byte) (data.Length + 2),
					(byte) data.Length,
					address
				};
				usbDevice.TxRxCommand(command, outData, ref inData);
				SX1276LR.HidCommandsStatus status = (SX1276LR.HidCommandsStatus)inData[0];
				int temp_46 = (int)inData[1];
				int temp_49 = (int)inData[2];
				int temp_52 = (int)inData[3];
				int temp_55 = (int)inData[4];
				int temp_58 = (int)inData[5];
				int temp_61 = (int)inData[6];
				int temp_64 = (int)inData[7];
				int temp_67 = (int)inData[8];
				if (status != SX1276LR.HidCommandsStatus.SX_OK || inData[9] != data.Length)
					return false;
				Array.Copy((Array)inData, 10, (Array)data, 0, data.Length);
				return true;
			}
		}

		public bool SKDeviceWrite(byte address, byte[] data)
		{
			lock (syncThread)
			{
				byte command = (byte)129;
				byte[] inData = new byte[10];
				byte[] outData = new byte[data.Length + 4];
				outData[0] = 0;
				outData[1] = (byte)(data.Length + 2);
				outData[2] = (byte)data.Length;
				outData[3] = address;
				Array.Copy((Array)data, 0, (Array)outData, 4, data.Length);
				usbDevice.TxRxCommand(command, outData, ref inData);
				SX1276LR.HidCommandsStatus status = (SX1276LR.HidCommandsStatus)inData[0];
				int temp_54 = (int)inData[1];
				int temp_57 = (int)inData[2];
				int temp_60 = (int)inData[3];
				int temp_63 = (int)inData[4];
				int temp_66 = (int)inData[5];
				int temp_69 = (int)inData[6];
				int temp_72 = (int)inData[7];
				int temp_75 = (int)inData[8];
				return status == SX1276LR.HidCommandsStatus.SX_OK;
			}
		}

		public bool Write(byte address, byte data)
		{
			return SKDeviceWrite(address, new byte[1]
			{
				data
			});
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
					bool local_0 = SpectrumOn;
					if (SpectrumOn)
						SpectrumOn = false;
					PacketHandlerStop();
					if (!SKReset())
						throw new Exception("Unable to reset the SK");
					ReadRegisters();
					Decimal local_1 = FrequencyRf;
					SetFrequencyRf(new Decimal(915000000));
					ImageCalStart();
					SetFrequencyRf(local_1);
					ImageCalStart();
					SetLoraOn(true);
					SetDefaultValues();
					ReadRegisters();
					RfPaSwitchEnabled = 0;
					RfPaSwitchSel = RfPaSwitchSelEnum.RF_IO_RFIO;
					if (!local_0)
						return;
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
					foreach (Register item_0 in registers)
					{
						if ((int)item_0.Address != 0 && !Write((byte)item_0.Address, (byte)item_0.Value))
							throw new Exception("Writing register " + item_0.Name);
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
			byte data = (byte)0;
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
						r.Value = (uint)data;
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
					foreach (Register item_0 in registers)
					{
						if ((int)item_0.Address != 0 && ((int)item_0.Address != 1 || !isPacketHandlerStarted))
							ReadRegister(item_0);
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
					foreach (Register item_0 in regs)
					{
						if ((int)item_0.Address != 0 && ((int)item_0.Address != 1 || !isPacketHandlerStarted))
							ReadRegister(item_0);
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
					Array.Copy((Array)data, sourceIndex, (Array)array, 0, 32);
					if (!Write((byte)0, array))
						return false;
					length -= 32;
					sourceIndex += 32;
				}
				else
				{
					Array.Resize<byte>(ref array, length);
					Array.Copy((Array)data, sourceIndex, (Array)array, 0, length);
					if (!Write((byte)0, array))
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
					if (!Read((byte)0, ref numArray))
						return false;
					Array.Copy((Array)numArray, 0, (Array)data, destinationIndex, 32);
					length -= 32;
					destinationIndex += 32;
				}
				else
				{
					Array.Resize<byte>(ref numArray, length);
					if (!Read((byte)0, ref numArray))
						return false;
					Array.Copy((Array)numArray, 0, (Array)data, destinationIndex, length);
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
					Console.WriteLine("IRQ: RxTimeoutIrq [0x{0}] - ReadIrqFlags", (object)registers["RegIrqFlags"].Value.ToString("X02"));
					ClrRxTimeoutIrq();
					PacketModeRxSingle = false;
					PacketHandlerStop();
				}
				int temp_13 = ValidHeader ? 1 : 0;
				if (!FhssChangeChannel)
					return;
				Console.WriteLine("IRQ: FhssChangeChannelIrq [0x{0}] - ReadIrqFlags", (object)registers["RegIrqFlags"].Value.ToString("X02"));
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
					SKSetPinState((byte)7, (byte)1);
					SKSetPinState((byte)8, (byte)0);
					break;
				case OperatingModeEnum.Rx:
				case OperatingModeEnum.RxSingle:
				case OperatingModeEnum.Cad:
					SKSetPinState((byte)7, (byte)0);
					SKSetPinState((byte)8, (byte)1);
					break;
				default:
					SKSetPinState((byte)6, (byte)1);
					SKSetPinState((byte)7, (byte)1);
					SKSetPinState((byte)8, (byte)1);
					break;
			}
		}

		public void SetDefaultValues()
		{
			if (IsOpen)
			{
				if (!Write((byte)registers["RegModemConfig2"].Address, new byte[2]
        {
          (byte) 115,
          byte.MaxValue
        }))
					throw new Exception("Unable to write register: " + registers["RegModemConfig2"].Name);
				if (!Write((byte)registers["RegModemConfig3"].Address, (byte)4))
					throw new Exception("Unable to write register: " + registers["RegModemConfig3"].Name);
			}
			else
			{
				registers["RegModemConfig2"].Value = 115U;
				registers["RegSymbTimeoutLsb"].Value = (uint)byte.MaxValue;
				registers["RegModemConfig3"].Value = 4U;
				ReadRegisters();
			}
		}

		public void SetLoraOn(bool enable)
		{
			if (enable)
			{
				SetOperatingMode(OperatingModeEnum.Sleep);
				byte data = (byte)0;
				if (!Read((byte)1, ref data))
					throw new Exception("Unable to read LoRa mode");
				if (!Write((byte)1, (byte)(((int)data | 128) & 248)))
					throw new Exception("Unable to write LoRa mode");
				SetOperatingMode(OperatingModeEnum.Stdby);
			}
			else
			{
				SetOperatingMode(OperatingModeEnum.Sleep);
				byte data = (byte)0;
				if (!Read((byte)1, ref data))
					throw new Exception("Unable to read FSK mode");
				if (!Write((byte)1, (byte)((uint)data & 120U)))
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
					byte local_1 = (byte)0;
					SetOperatingMode(OperatingModeEnum.Stdby);
					Read((byte)59, ref local_1);
					Write((byte)59, (byte)((uint)local_1 | 64U));
					DateTime local_2 = DateTime.Now;
					bool local_4_1;
					do
					{
						local_1 = (byte)0;
						Read((byte)59, ref local_1);
						local_4_1 = (DateTime.Now - local_2).TotalMilliseconds >= 1000.0;
					}
					while ((int)(byte)((uint)local_1 & 32U) == 32 && !local_4_1);
					if (local_4_1)
						throw new Exception("Image calibration timeout.");
				}
				finally
				{
					SetOperatingMode(local_0);
				}
			}
		}

		public void SetAccessSharedFskReg(bool value)
		{
			try
			{
				registers["RegOpMode"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegOpMode"].Value & 191U) | (value ? 64 : 0));
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
					registers["RegOpMode"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegOpMode"].Value & 247U) | (value ? 8 : 0));
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
					SKSetPinState((byte)11, (byte)0);
					SKSetPinState((byte)12, (byte)1);
				}
				else
				{
					SKSetPinState((byte)11, (byte)1);
					SKSetPinState((byte)12, (byte)0);
				}
				if (value == OperatingModeEnum.TxContinuous)
				{
					SetTxContinuousOn(true);
					value = OperatingModeEnum.Tx;
				}
				else
					SetTxContinuousOn(false);
				byte data = (byte)((uint)(byte)((uint)(byte)registers["RegOpMode"].Value & 248U) | (uint)(byte)value);
				if (!isQuiet)
				{
					registers["RegOpMode"].Value = (uint)data;
				}
				else
				{
					lock (syncThread)
					{
						if (!Write((byte)registers["RegOpMode"].Address, data))
							throw new Exception("Unable to write register " + registers["RegOpMode"].Name);
						if (Mode != OperatingModeEnum.Rx && Mode != OperatingModeEnum.RxSingle)
						{
							if (Mode != OperatingModeEnum.Cad)
								goto label_17;
						}
						ReadRegister(registers["RegLna"]);
					}
				}
			label_17:
				Console.WriteLine("RegOpMode (SOM): 0x{0}", (object)data.ToString("X02"));
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
					registers["RegFdevMsb"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegFdevMsb"].Value & 63U) | (uint)(byte)((uint)value << 6));
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
					byte local_0 = (byte)registers["RegFrfMsb"].Value;
					byte local_1 = (byte)registers["RegFrfMid"].Value;
					byte local_2 = (byte)registers["RegFrfLsb"].Value;
					byte local_0_1 = (byte)((long)(value / frequencyStep) >> 16);
					byte local_1_1 = (byte)((long)(value / frequencyStep) >> 8);
					byte local_2_1 = (byte)(long)(value / frequencyStep);
					frequencyRfCheckDisable = true;
					registers["RegFrfMsb"].Value = (uint)local_0_1;
					registers["RegFrfMid"].Value = (uint)local_1_1;
					frequencyRfCheckDisable = false;
					registers["RegFrfLsb"].Value = (uint)local_2_1;
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
				{
					byte num = (byte)this.registers["RegPaConfig"].Value;
					num = (byte)(num & 0x7F);

					switch (value)
					{
						case PaSelectEnum.PA_BOOST:
							num |= 0x80;
							break;
					}
					registers["RegPaConfig"].Value = num;
				}
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
				{
					// registers["RegPaConfig"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegPaConfig"].Value & 143U) | (uint)(byte)(((int)((value - new Decimal(108, 0, 0, false, (byte)1)) / new Decimal(6, 0, 0, false, (byte)1)) & 7) << 4));
					byte num = (byte)registers["RegPaConfig"].Value;
					num &= 0x8F;
					num = (byte)(num | ((byte)((((int)((value - 10.8M) / 0.6M)) & 7) << 4)));
					registers["RegPaConfig"].Value = num;
				}
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
					byte local_0_1 = (byte)((uint)(byte)registers["RegPaConfig"].Value & 240U);
					byte local_0_2;
					if (PaSelect == PaSelectEnum.RFO)
					{
						if (value < MaxOutputPower - new Decimal(150, 0, 0, false, (byte)1))
							value = MaxOutputPower - new Decimal(150, 0, 0, false, (byte)1);
						if (value > MaxOutputPower)
							value = MaxOutputPower;
						local_0_2 = (byte)((uint)local_0_1 | (uint)(byte)((int)(value - MaxOutputPower + new Decimal(150, 0, 0, false, (byte)1)) & 15));
					}
					else if (!Pa20dBm)
					{
						if (value < new Decimal(2))
							value = new Decimal(2);
						if (value > new Decimal(17))
							value = new Decimal(17);
						local_0_2 = (byte)((uint)local_0_1 | (uint)(byte)((int)(value - new Decimal(170, 0, 0, false, (byte)1) + new Decimal(150, 0, 0, false, (byte)1)) & 15));
					}
					else
					{
						if (value < new Decimal(5))
							value = new Decimal(5);
						if (value > new Decimal(20))
							value = new Decimal(20);
						local_0_2 = (byte)((uint)local_0_1 | (uint)(byte)((int)(value - new Decimal(200, 0, 0, false, (byte)1) + new Decimal(150, 0, 0, false, (byte)1)) & 15));
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
				{
					// registers["RegPaRamp"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegPaRamp"].Value & 239U) | (value ? 16 : 0));
					byte num = (byte)registers["RegPaRamp"].Value;
					num = (byte)(num & 0xEF);
					num = (byte)(num | (value ? 0x10 : 0));
					registers["RegPaRamp"].Value = num;
				}
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
				{
					// registers["RegPaRamp"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegPaRamp"].Value & 240U) | (uint)(byte)((uint)(byte)value & 15U));
					byte num = (byte)registers["RegPaRamp"].Value;
					num = (byte)((num & 0xF0) | ((byte)value & 0x0F));
					registers["RegPaRamp"].Value = num;
				}
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
				{
					// registers["RegOcp"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegOcp"].Value & 223U) | (value ? 32 : 0));
					byte num = (byte)registers["RegOcp"].Value;
					num = (byte)((num & 0xDF) | (value ? 0x20 : 0));
					registers["RegOcp"].Value = num;
				}
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
					// byte local_0_1 = (byte)((uint)(byte)registers["RegOcp"].Value & 0xE0);
					// registers["RegOcp"].Value = !(value <= new Decimal(1200, 0, 0, false, (byte)1)) ? (!(value > new Decimal(120)) || !(value <= new Decimal(2400, 0, 0, false, (byte)1)) ? (uint)(byte)((uint)local_0_1 | 27U) : (uint)(byte)((uint)local_0_1 | (uint)(byte)((uint)(byte)((value + new Decimal(30)) / new Decimal(10)) & 31U))) : (uint)(byte)((uint)local_0_1 | (uint)(byte)((uint)(byte)((value - new Decimal(45)) / new Decimal(5)) & 15U));
					byte num = (byte)(registers["RegOcp"].Value & 0xE0);
					if (value <= 120.0M)
					{
						num = (byte)(num | ((byte)((value - 45M) / 5M) & 0x0F));
					}
					else if (value > 120M && value <= 240.0M)
					{
						num = (byte)(num | ((byte)((value + 30M) / 10M) & 0x1F));
					}
					else
					{
						num = (byte)(num | 0x1B);
					}
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
				{
					// registers["RegAgcRef"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegAgcRef"].Value & 192U) | (uint)(byte)(value & 63));
					byte num = (byte)registers["RegAgcRef"].Value;
					num = (byte)((num & 0xC0) | ((byte)(value & 0x3F)));
					registers["RegAgcRef"].Value = num;
				}
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
				{
					// registers["RegLna"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegLna"].Value & 31U) | (uint)(byte)((uint)(byte)value << 5));
					byte num = (byte)registers["RegLna"].Value;
					num = (byte)((num & 0x1F) | ((byte)value << 5));
					registers["RegLna"].Value = num;

					ReadRegister(registers["RegLna"]);
				}
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
				{
					// registers["RegLna"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegLna"].Value & 251U) | (value ? 4 : 0));
					byte num = (byte)registers["RegLna"].Value;
					num = (byte)((num & 0xFB) | (value ? 4 : 0));
					registers["RegLna"].Value = num;
				}
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
				{
					// registers["RegLna"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegLna"].Value & 252U) | (value ? 3 : 0));
					byte num = (byte)registers["RegLna"].Value;
					num = (byte)((num & 0xFC) | (value ? 3 : 0));
					registers["RegLna"].Value = num;
				}
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
				{
					// registers["RegIrqFlagsMask"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegIrqFlagsMask"].Value & (uint)sbyte.MaxValue) | (value ? 128 : 0));
					byte num = (byte)registers["RegIrqFlagsMask"].Value;
					num = (byte)((num & 0x7f) | (value ? 0x80 : 0));
					registers["RegIrqFlagsMask"].Value = num;
				}
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
				{
					// registers["RegIrqFlagsMask"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegIrqFlagsMask"].Value & 191U) | (value ? 64 : 0));
					byte num = (byte)registers["RegIrqFlagsMask"].Value;
					num = (byte)((num & 0xBF) | (value ? 0x40 : 0));
					registers["RegIrqFlagsMask"].Value = num;
				}
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
				{
					// registers["RegIrqFlagsMask"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegIrqFlagsMask"].Value & 223U) | (value ? 32 : 0));
					byte num = (byte)registers["RegIrqFlagsMask"].Value;
					num = (byte)((num & 0xDF) | (value ? 0x20 : 0));
					registers["RegIrqFlagsMask"].Value = num;
				}
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
				{
					// registers["RegIrqFlagsMask"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegIrqFlagsMask"].Value & 239U) | (value ? 16 : 0));
					byte num = (byte)registers["RegIrqFlagsMask"].Value;
					num = (byte)((num & 0xef) | (value ? 0x10 : 0));
					registers["RegIrqFlagsMask"].Value = num;
				}
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
				{
					// registers["RegIrqFlagsMask"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegIrqFlagsMask"].Value & 247U) | (value ? 8 : 0));
					byte num = (byte)registers["RegIrqFlagsMask"].Value;
					num = (byte)((num & 0xF7) | (value ? 8 : 0));
					registers["RegIrqFlagsMask"].Value = num;
				}
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
				{
					// registers["RegIrqFlagsMask"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegIrqFlagsMask"].Value & 251U) | (value ? 4 : 0));
					byte num = (byte)registers["RegIrqFlagsMask"].Value;
					num = (byte)((num & 0xfb) | (value ? 4 : 0));
					registers["RegIrqFlagsMask"].Value = num;
				}
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
				{
					// registers["RegIrqFlagsMask"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegIrqFlagsMask"].Value & 253U) | (value ? 2 : 0));
					byte num = (byte)registers["RegIrqFlagsMask"].Value;
					num = (byte)((num & 0xFD) | (value ? 2 : 0));
					registers["RegIrqFlagsMask"].Value = num;
				}
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
				{
					// registers["RegIrqFlagsMask"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegIrqFlagsMask"].Value & 254U) | (value ? 1 : 0));
					byte num = (byte)registers["RegIrqFlagsMask"].Value;
					num = (byte)((num & 0xFE) | (value ? 1 : 0));
					registers["RegIrqFlagsMask"].Value = num;
				}
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
					WriteRegister(registers["RegIrqFlags"], byte.MaxValue);
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
				{
					// registers["RegIrqFlags"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegIrqFlags"].Value & (uint)sbyte.MaxValue) | 128U);
					byte num = (byte)registers["RegIrqFlags"].Value;
					num = (byte)((num & 0x7F) | 0x80);
					registers["RegIrqFlags"].Value = num;
				}
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
				{
					// registers["RegIrqFlags"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegIrqFlags"].Value & 191U) | 64U);
					byte num = (byte)registers["RegIrqFlags"].Value;
					num = (byte)((num & 0xBF) | 0x40);
					registers["RegIrqFlags"].Value = num;
				}
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
				{
					// registers["RegIrqFlags"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegIrqFlags"].Value & 223U) | 32U);
					byte num = (byte)registers["RegIrqFlags"].Value;
					num = (byte)((num & 0xDF) | 0x20);
					registers["RegIrqFlags"].Value = num;
				}
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
				{
					// registers["RegIrqFlags"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegIrqFlags"].Value & 239U) | 16U);
					byte num = (byte)registers["RegIrqFlags"].Value;
					num = (byte)((num & 0xEF) | 0x10);
					registers["RegIrqFlags"].Value = num;
				}
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
				{
					// registers["RegIrqFlags"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegIrqFlags"].Value & 247U) | 8U);
					byte num = (byte)registers["RegIrqFlags"].Value;
					num = (byte)((num & 0xF7) | 0x08);
					registers["RegIrqFlags"].Value = num;
				}
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
				{
					// registers["RegIrqFlags"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegIrqFlags"].Value & 251U) | 4U);
					byte num = (byte)registers["RegIrqFlags"].Value;
					num = (byte)((num & 0xFB) | 4);
					registers["RegIrqFlags"].Value = num;
				}
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
				{
					// registers["RegIrqFlags"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegIrqFlags"].Value & 253U) | 2U);
					byte num = (byte)registers["RegIrqFlags"].Value;
					num = (byte)((num & 0xFD) | 2);
					registers["RegIrqFlags"].Value = num;
				}
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
				{
					// registers["RegIrqFlags"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegIrqFlags"].Value & 254U) | 1U);
					byte num = (byte)registers["RegIrqFlags"].Value;
					num = (byte)((num & 0xFE) | 1);
					registers["RegIrqFlags"].Value = num;
				}
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
					if ((int)value < 7 || (int)value == 7 && ((int)SpreadingFactor == 11 || (int)SpreadingFactor == 12) || (int)value == 8 && (int)SpreadingFactor == 12)
						SetLowDatarateOptimize(true);
					else
						SetLowDatarateOptimize(false);
					registers["RegModemConfig1"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegModemConfig1"].Value & 15U) | (uint)(byte)((uint)value << 4));
					if ((int)value == 9 && FrequencyRf >= new Decimal(640000000))
					{
						registers["RegTest36"].Value &= 254U;
						registers["RegTest3A"].Value = (uint)((int)registers["RegTest3A"].Value & 192 | 36);
					}
					else if ((int)value == 9)
					{
						registers["RegTest36"].Value &= 254U;
						registers["RegTest3A"].Value = (uint)((int)registers["RegTest3A"].Value & 192 | 63);
					}
					else
					{
						registers["RegTest36"].Value |= 1U;
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
				{
					// registers["RegModemConfig1"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegModemConfig1"].Value & 241U) | (uint)(byte)((uint)value << 1));
					byte num = (byte)registers["RegModemConfig1"].Value;
					num = (byte)((num & 0xF1) | (value << 1));
					registers["RegModemConfig1"].Value = num;
				}
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
				{
					// registers["RegModemConfig1"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegModemConfig1"].Value & 254U) | (value ? 1 : 0));
					byte num = (byte)registers["RegModemConfig1"].Value;
					num = (byte)((num & 0xFE) | (value ? 1 : 0));
					registers["RegModemConfig1"].Value = num;
				}
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
					if ((int)Bandwidth < 7 || (int)Bandwidth == 7 && ((int)value == 11 || (int)value == 12) || (int)Bandwidth == 8 && (int)value == 12)
						SetLowDatarateOptimize(true);
					else
						SetLowDatarateOptimize(false);

					registers["RegModemConfig2"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegModemConfig2"].Value & 15U) | (uint)(byte)((uint)value << 4));
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
				registers["RegModemConfig2"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegModemConfig2"].Value & 247U) | (value ? 8 : 0));
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
				{
					// registers["RegModemConfig2"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegModemConfig2"].Value & 251U) | (value ? 4 : 0));
					byte num = (byte)registers["RegModemConfig2"].Value;
					num = (byte)((num & 0xFB) | (value ? 4 : 0));
					registers["RegModemConfig2"].Value = num;
				}
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
					byte local_0 = (byte)registers["RegModemConfig2"].Value;
					byte local_1 = (byte)registers["RegSymbTimeoutLsb"].Value;
					byte local_0_2 = (byte)((uint)(byte)((uint)local_0 & 252U) | (uint)(byte)((ulong)((long)(value / SymbolTime) >> 8) & 3UL));
					byte local_1_1 = (byte)(long)(value / SymbolTime);
					registers["RegModemConfig2"].Value = (uint)local_0_2;
					registers["RegSymbTimeoutLsb"].Value = (uint)local_1_1;
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
					registers["RegPreambleMsb"].Value = (uint)(byte)(value >> 8);
					registers["RegPreambleLsb"].Value = (uint)(byte)value;
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
				{
					// registers["RegModemConfig3"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegModemConfig3"].Value & 247U) | (value ? 8 : 0));
					byte num = (byte)registers["RegModemConfig3"].Value;
					num = (byte)((num & 0xF7) | (value ? 8 : 0));
					registers["RegModemConfig3"].Value = num;
				}
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
				{
					// registers["RegModemConfig3"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegModemConfig3"].Value & 251U) | (value ? 4 : 0));
					byte num = (byte)registers["RegModemConfig3"].Value;
					num = (byte)((num & 0xFB) | (value ? 4 : 0));
					registers["RegModemConfig3"].Value = num;
				}
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
					Register local_1;
					switch (id)
					{
						case (byte)0:
						case (byte)1:
						case (byte)2:
						case (byte)3:
							local_1 = registers["RegDioMapping1"];
							break;
						case (byte)4:
						case (byte)5:
							local_1 = registers["RegDioMapping2"];
							break;
						default:
							throw new Exception("Invalid DIO ID!");
					}
					uint local_0 = (uint)(byte)local_1.Value;
					uint local_0_2;
					switch (id)
					{
						case (byte)0:
							local_0_2 = local_0 & 63U | (uint)value << 6;
							break;
						case (byte)1:
							local_0_2 = local_0 & 207U | (uint)value << 4;
							break;
						case (byte)2:
							local_0_2 = local_0 & 243U | (uint)value << 2;
							break;
						case (byte)3:
							local_0_2 = (uint)((DioMappingEnum)(local_0 & 252U) | value & DioMappingEnum.DIO_MAP_11);
							break;
						case (byte)4:
							local_0_2 = local_0 & 63U | (uint)value << 6;
							break;
						case (byte)5:
							local_0_2 = local_0 & 207U | (uint)value << 4;
							break;
						default:
							throw new Exception("Invalid DIO ID!");
					}
					local_1.Value = local_0_2;
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
				{
					// registers["RegPllHop"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegPllHop"].Value & (uint)sbyte.MaxValue) | (value ? 128 : 0));
					byte num = (byte)registers["RegPllHop"].Value;
					num = (byte)((num & 0x7F) | (value ? 0x80 : 0));
					registers["RegPllHop"].Value = num;
				}
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
					registers["RegTcxo"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegTcxo"].Value & 239U) | (value ? 16 : 0));
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
					byte local_0 = (byte)registers["RegPaDac"].Value;
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
				{
					// registers["RegPll"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegPll"].Value & 63U) | (uint)(byte)((uint)(byte)Decimal.op_Decrement(value / new Decimal(75000)) << 6));

					byte num = (byte)this.registers["RegPll"].Value;
					num = (byte)(num & 0x3f);
					num = (byte)(num | ((byte)(((byte)Decimal.Subtract((value / 75000M), 1M) << 6))));
					this.registers["RegPll"].Value = num;
				}
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
							int temp_123 = (int)MessageBox.Show("Message must be at least one byte long", "SX1276SKA-PacketHandler", MessageBoxButtons.OK, MessageBoxIcon.Hand);
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
					Console.WriteLine("IRQ: AllIrq [0x{0}] - Stop", (object)registers["RegIrqFlags"].Value.ToString("X02"));
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
							Payload[0] = (byte)0;
							Payload[1] = (byte)(packetNumber >> 24);
							Payload[2] = (byte)(packetNumber >> 16);
							Payload[3] = (byte)(packetNumber >> 8);
							Payload[4] = (byte)packetNumber;
							Payload[5] = (byte)80;
							Payload[6] = (byte)69;
							Payload[7] = (byte)82;
							Payload[8] = (byte)((uint)Payload[0] + (uint)Payload[1] + (uint)Payload[2] + (uint)Payload[3] + (uint)Payload[4] + (uint)Payload[5] + (uint)Payload[6] + (uint)Payload[7]);
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
			PacketHandlerStarted((object)this, new EventArgs());
		}

		private void OnPacketHandlerStoped()
		{
			if (PacketHandlerStoped == null)
				return;
			PacketHandlerStoped((object)this, new EventArgs());
		}

		private void OnPacketHandlerTransmitted()
		{
			if (PacketHandlerTransmitted == null)
				return;
			Console.WriteLine("Pkt#: {0}", (object)packetNumber);
			PacketHandlerTransmitted((object)this, new PacketStatusEventArg(packetNumber, maxPacketNumber));
		}

		private void OnPacketHandlerReceived()
		{
			if (PacketHandlerReceived == null)
				return;
			Console.WriteLine("Pkt#: {0} - RxPkt#: {1}", (object)packetNumber, (object)(uint)((int)registers["RegRxPacketCntValueMsb"].Value << 8 | (int)registers["RegRxPacketCntValueLsb"].Value));
			PacketHandlerReceived((object)this, new PacketStatusEventArg(packetNumber, maxPacketNumber));
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
				if (!(e.PropertyName == "Value"))
					return;
				UpdateRegisterValue((Register)sender);
				if (readLock == 0 && !Write((byte)((Register)sender).Address, (byte)((Register)sender).Value))
					OnError(1, "Unable to write register " + ((Register)sender).Name);
				if (!(((Register)sender).Name == "RegOpMode"))
					return;
				Console.WriteLine("RegOpMode (PC): 0x{0}", (object)((Register)sender).Value.ToString("X02"));
				if (Mode == OperatingModeEnum.Rx || Mode == OperatingModeEnum.RxSingle || Mode == OperatingModeEnum.Cad)
				{
					ReadRegister(registers["RegLna"]);
					ReadRegister(registers["RegRssiValue"]);
				}
				ReadRegister(registers["RegIrqFlags"]);
			}
		}

		private void device_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "Version":
					PopulateRegisters();
					ReadRegisters();
					break;
			}
		}

		private void usbDevice_Opened(object sender, EventArgs e)
		{
			isOpen = true;
			if (!SKGetVersion())
				OnError(1, "Unable to read SK version.");
			else if (!SKReset())
			{
				OnError(1, "Unable to reset the SK");
			}
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
						Console.WriteLine("IRQ: TxDoneIrq [0x{0}] - Dio0Tx", (object)registers["RegIrqFlags"].Value.ToString("X02"));
						ClrAllIrq();
						Console.WriteLine("IRQ: TxDoneIrq [0x{0}] - Dio0Tx", (object)registers["RegIrqFlags"].Value.ToString("X02"));
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
						Console.WriteLine("IRQ: RxDoneIrq [0x{0}] - Dio0Rx", (object)registers["RegIrqFlags"].Value.ToString("X02"));
						ClrAllIrq();
						Console.WriteLine("IRQ: RxDoneIrq [0x{0}] - Dio0Rx", (object)registers["RegIrqFlags"].Value.ToString("X02"));
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
				get
				{
					return state;
				}
				private set
				{
					state = value;
				}
			}

			public IoChangedEventArgs(bool state)
			{
				State = state;
			}
		}

	}
}
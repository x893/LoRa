using SemtechLib.Devices.Common.Events;
using SemtechLib.Devices.Common.Hid;
using SemtechLib.Devices.Common.Interfaces;
using SemtechLib.Devices.SX1276.Enumerations;
using SemtechLib.Devices.SX1276.Events;
using SemtechLib.Devices.SX1276.General;
using SemtechLib.General;
using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SemtechLib.Devices.SX1276
{
	public class SX1276 : IDevice, INotifyPropertyChanged, IDisposable
	{
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
		private Decimal bitrate = new Decimal(4800);
		private Decimal fdev = new Decimal(5000);
		private Decimal frequencyRf = new Decimal(915000000);
		private Decimal maxOutputPower = new Decimal(132, 0, 0, false, (byte)1);
		private Decimal outputPower = new Decimal(132, 0, 0, false, (byte)1);
		private PaRampEnum paRamp = PaRampEnum.PaRamp_40;
		private bool ocpOn = true;
		private Decimal ocpTrim = new Decimal(100);
		private LnaGainEnum lnaGain = LnaGainEnum.G1;
		private bool agcAutoOn = true;
		private Decimal rssiOffset = new Decimal(0, 0, 0, false, (byte)1);
		private Decimal rssiSmoothing = new Decimal(80, 0, 0, false, (byte)1);
		private Decimal rssiCollisionThreshold = new Decimal(10);
		private Decimal rssiThreshold = new Decimal(1275, 0, 0, true, (byte)1);
		private Decimal rssiValue = new Decimal(1275, 0, 0, true, (byte)1);
		private Decimal prevRssiValue = new Decimal(1275, 0, 0, true, (byte)1);
		private Decimal rxBw = new Decimal(10416);
		private Decimal afcRxBw = new Decimal(25000);
		private bool bitSyncOn = true;
		private OokThreshTypeEnum ookThreshType = OokThreshTypeEnum.Peak;
		private Decimal ookPeakThreshStep = new Decimal(5, 0, 0, false, (byte)1);
		private Decimal[] ookPeakThreshStepTable = new Decimal[8]
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
		private byte ookFixedThreshold = 6;
		private Decimal ookAverageOffset = new Decimal(0, 0, 0, false, 1);
		private OokAverageThreshFiltEnum ookAverageThreshFilt = OokAverageThreshFiltEnum.COEF_2;
		private Decimal afcValue = new Decimal(0, 0, 0, false, 1);
		private Decimal feiValue = new Decimal(0, 0, 0, false, 1);
		private bool preambleDetectorOn = true;
		private byte preambleDetectorSize = 2;
		private byte preambleDetectorTol = 10;
		private ClockOutEnum clockOut = ClockOutEnum.CLOCK_OUT_111;
		private LowPowerSelection lowPowerSelection = LowPowerSelection.TO_LPM;
		private FromIdle fromIdle = FromIdle.TO_RX_ON_TMR1;
		private FromTransmit fromTransmit = FromTransmit.TO_RX;
		private byte timer1Coef = 245;
		private byte timer2Coef = 32;
		private bool autoImageCalOn = true;
		private TempThresholdEnum tempThreshold = TempThresholdEnum.THRESH_10;
		private Decimal tempValue = new Decimal(1650, 0, 0, false, (byte)1);
		private Decimal tempValueRoom = new Decimal(250, 0, 0, false, (byte)1);
		private Decimal tempValueCal = new Decimal(1650, 0, 0, false, (byte)1);
		private LowBatTrimEnum lowBatTrim = LowBatTrimEnum.Trim1_835;
		private Version version = new Version(0, 0);
		private byte agcReferenceLevel = 19;
		private byte agcStep1 = 14;
		private byte agcStep2 = 5;
		private byte agcStep3 = 11;
		private byte agcStep4 = 13;
		private byte agcStep5 = 11;
		private Decimal pllBandwidth = new Decimal(300000);
		private Decimal rfPaRssiValue = new Decimal(1275, 0, 0, true, 1);
		private Decimal rfIoRssiValue = new Decimal(1275, 0, 0, true, 1);
		private PseudoNoiseGenerator png = new PseudoNoiseGenerator();
		private OperatingModeEnum prevPngOpMode = OperatingModeEnum.Stdby;
		private bool lowFrequencyMode = true;
		private byte[] FifoData = new byte[66];
		private const byte HID_CMD_INDEX = 0;
		private const byte HID_CMD_OPT_INDEX = 0;
		private const byte HID_CMD_DATA_SIZE_INDEX = 1;
		private const byte HID_CMD_DATA_INDEX = 2;
		private const byte HID_CMD_ANS_INDEX = 0;
		private const byte HID_CMD_ANS_STAT_INDEX = 0;
		private const byte HID_CMD_ANS_TIME_STAMP_INDEX = 1;
		private const byte HID_CMD_ANS_DATA_SIZE_INDEX = 9;
		private const byte HID_CMD_ANS_DATA_INDEX = 10;
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
		private const int RSSI_OFFSET_LF = -159;
		private const int NOISE_FIGURE_LF = 4;
		private const int RSSI_OFFSET_HF = -156;
		private const int NOISE_FIGURE_HF = 6;
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
		private ILog log;
		private bool isDebugOn;
		private OperatingModeEnum prevMode;
		private LnaGainEnum prevLnaGain;
		private bool prevAgcAutoOn;
		private ModulationTypeEnum prevModulationType;
		private Decimal prevRssiThresh;
		private bool prevMonitorOn;
		private bool spectrumOn;
		private int spectrumFreqId;
		private ModulationTypeEnum modulationType;
		private BandEnum band;
		private PaSelectEnum paSelect;
		private byte modulationShaping;
		private bool forceTxBandLowFrequencyOn;
		private bool forceRxBandLowFrequencyOn;
		private bool lnaBoost;
		private bool restartRxOnCollision;
		private bool afcAutoOn;
		private RxTriggerEnum rxTriger;
		private OokPeakThreshDecEnum ookPeakThreshDec;
		private bool afcAutoClearOn;
		private Decimal timeoutRxRssi;
		private Decimal timeoutRxPreamble;
		private Decimal timeoutSignalSync;
		private Decimal interPacketRxDelay;
		private Packet packet;
		private IdleMode idleMode;
		private FromStart fromStart;
		private FromReceive fromReceive;
		private FromRxTimeout fromRxTimeout;
		private FromPacketReceived fromPacketReceived;
		private TimerResolution timer1Resolution;
		private TimerResolution timer2Resolution;
		private bool imageCalRunning;
		private bool tempChange;
		private bool tempMonitorOff;
		private bool tempCalDone;
		private bool tempMeasRunning;
		private bool lowBatOn;
		private bool modeReady;
		private bool rxReady;
		private bool txReady;
		private bool pllLock;
		private bool rssi;
		private bool timeout;
		private bool preambleDetect;
		private bool syncAddressMatch;
		private bool fifoFull;
		private bool fifoEmpty;
		private bool fifoLevel;
		private bool fifoOverrun;
		private bool packetSent;
		private bool payloadReady;
		private bool crcOk;
		private bool lowBat;
		private DioMappingEnum dio0Mapping;
		private DioMappingEnum dio1Mapping;
		private DioMappingEnum dio2Mapping;
		private DioMappingEnum dio3Mapping;
		private DioMappingEnum dio4Mapping;
		private DioMappingEnum dio5Mapping;
		private bool mapPreambleDetect;
		private bool fastHopOn;
		private bool tcxoInputOn;
		private bool pa20dBm;
		private Decimal tempDelta;
		private Decimal formerTemp;
		private Decimal bitrateFrac;
		private RfPaSwitchSelEnum rfPaSwitchSel;
		private RfPaSwitchSelEnum prevRfPaSwitchSel;
		private int prevRfPaSwitchEnabled;
		private int rfPaSwitchEnabled;
		private bool pngEnabled;
		private bool lnaBoostPrev;
		private bool isPacketHandlerStarted;
		private int packetNumber;
		private int maxPacketNumber;
		private bool frameTransmitted;
		private bool frameReceived;
		private bool firstTransmit;
		private bool SequencerStarted;

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

		public ILog Log
		{
			get
			{
				return log;
			}
			set
			{
				log = value;
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
				Fdev = (Decimal)(registers["RegFdevMsb"].Value << 8 | registers["RegFdevLsb"].Value) * FrequencyStep;
				Bitrate = frequencyXo / ((Decimal)(registers["RegBitrateMsb"].Value << 8 | registers["RegBitrateLsb"].Value) + (Decimal)registers["RegBitrateFrac"].Value / new Decimal(160, 0, 0, false, 1));
				OnPropertyChanged("FrequencyXo");
				int mant1;
				switch ((registers["RegRxBw"].Value & 24U) >> 3)
				{
					case 0U:
						mant1 = 16;
						break;
					case 1U:
						mant1 = 20;
						break;
					case 2U:
						mant1 = 24;
						break;
					default:
						throw new Exception("Invalid RxBwMant parameter");
				}
				RxBw = SX1276.ComputeRxBw(value, modulationType, mant1, (int)registers["RegRxBw"].Value & 7);
				int mant2;
				switch ((registers["RegAfcBw"].Value & 24U) >> 3)
				{
					case 0U:
						mant2 = 16;
						break;
					case 1U:
						mant2 = 20;
						break;
					case 2U:
						mant2 = 24;
						break;
					default:
						throw new Exception("Invalid RxBwMant parameter");
				}
				AfcRxBw = SX1276.ComputeRxBw(value, modulationType, mant2, (int)registers["RegAfcBw"].Value & 7);
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

		public Decimal Tbit
		{
			get
			{
				return new Decimal(1) / Bitrate;
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
					prevRssiThresh = RssiThreshold;
					SetRssiThresh(new Decimal(1275, 0, 0, true, 1));
					prevModulationType = ModulationType;
					SetModulationType(ModulationTypeEnum.OOK);
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
					SetRssiThresh(prevRssiThresh);
					SetModulationType(prevModulationType);
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
				return RxBw / new Decimal(30, 0, 0, false, (byte)1);
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

		public ModulationTypeEnum ModulationType
		{
			get
			{
				return modulationType;
			}
			set
			{
				modulationType = value;
				OnPropertyChanged("ModulationType");
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
				OnPropertyChanged("DataMode");
			}
		}

		public Decimal Bitrate
		{
			get
			{
				return bitrate;
			}
			set
			{
				bitrate = value;
				BitrateFdevCheck(value, fdev);
				OnPropertyChanged("Bitrate");
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

		public Decimal Fdev
		{
			get
			{
				return fdev;
			}
			set
			{
				fdev = value;
				BitrateFdevCheck(bitrate, value);
				OnPropertyChanged("Fdev");
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

		public byte ModulationShaping
		{
			get
			{
				return modulationShaping;
			}
			set
			{
				modulationShaping = value;
				OnPropertyChanged("ModulationShaping");
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

		public Decimal RxBwMin
		{
			get
			{
				return ComputeRxBwMin();
			}
		}

		public Decimal RxBwMax
		{
			get
			{
				return ComputeRxBwMax();
			}
		}

		public Decimal AfcRxBwMin
		{
			get
			{
				return ComputeRxBwMin();
			}
		}

		public Decimal AfcRxBwMax
		{
			get
			{
				return ComputeRxBwMax();
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

		public bool RestartRxOnCollision
		{
			get
			{
				return restartRxOnCollision;
			}
			set
			{
				restartRxOnCollision = value;
				OnPropertyChanged("RestartRxOnCollision");
			}
		}

		public bool AfcAutoOn
		{
			get
			{
				return afcAutoOn;
			}
			set
			{
				afcAutoOn = value;
				OnPropertyChanged("AfcAutoOn");
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

		public RxTriggerEnum RxTrigger
		{
			get
			{
				return rxTriger;
			}
			set
			{
				rxTriger = value;
				OnPropertyChanged("RxTrigger");
			}
		}

		public Decimal RssiOffset
		{
			get
			{
				return rssiOffset;
			}
			set
			{
				rssiOffset = value;
				OnPropertyChanged("RssiOffset");
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
				rssiSmoothing = value;
				OnPropertyChanged("RssiSmoothing");
			}
		}

		public Decimal RssiCollisionThreshold
		{
			get
			{
				return rssiCollisionThreshold;
			}
			set
			{
				rssiCollisionThreshold = value;
				OnPropertyChanged("RssiCollisionThreshold");
			}
		}

		public Decimal RssiThreshold
		{
			get
			{
				return rssiThreshold;
			}
			set
			{
				rssiThreshold = value;
				OnPropertyChanged("RssiThreshold");
			}
		}

		public Decimal RssiValue
		{
			get
			{
				return rssiValue;
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
				rxBw = value;
				OnPropertyChanged("RxBw");
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
				afcRxBw = value;
				OnPropertyChanged("AfcRxBw");
			}
		}

		public bool BitSyncOn
		{
			get
			{
				return bitSyncOn;
			}
			set
			{
				bitSyncOn = value;
				OnPropertyChanged("BitSyncOn");
			}
		}

		public OokThreshTypeEnum OokThreshType
		{
			get
			{
				return ookThreshType;
			}
			set
			{
				ookThreshType = value;
				OnPropertyChanged("OokThreshType");
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
				ookPeakThreshStep = value;
				OnPropertyChanged("OokPeakThreshStep");
			}
		}

		public Decimal[] OoPeakThreshStepTable
		{
			get
			{
				return ookPeakThreshStepTable;
			}
		}

		public byte OokFixedThreshold
		{
			get
			{
				return ookFixedThreshold;
			}
			set
			{
				ookFixedThreshold = value;
				OnPropertyChanged("OokFixedThreshold");
			}
		}

		public OokPeakThreshDecEnum OokPeakThreshDec
		{
			get
			{
				return ookPeakThreshDec;
			}
			set
			{
				ookPeakThreshDec = value;
				OnPropertyChanged("OokPeakThreshDec");
			}
		}

		public Decimal OokAverageOffset
		{
			get
			{
				return ookAverageOffset;
			}
			set
			{
				ookAverageOffset = value;
				OnPropertyChanged("OokAverageOffset");
			}
		}

		public OokAverageThreshFiltEnum OokAverageThreshFilt
		{
			get
			{
				return ookAverageThreshFilt;
			}
			set
			{
				ookAverageThreshFilt = value;
				OnPropertyChanged("OokAverageThreshFilt");
			}
		}

		public bool AfcAutoClearOn
		{
			get
			{
				return afcAutoClearOn;
			}
			set
			{
				afcAutoClearOn = value;
				OnPropertyChanged("AfcAutoClearOn");
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
				OnPropertyChanged("AfcValue");
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
				OnPropertyChanged("FeiValue");
			}
		}

		public bool PreambleDetectorOn
		{
			get
			{
				return preambleDetectorOn;
			}
			set
			{
				preambleDetectorOn = value;
				OnPropertyChanged("PreambleDetectorOn");
			}
		}

		public byte PreambleDetectorSize
		{
			get
			{
				return preambleDetectorSize;
			}
			set
			{
				preambleDetectorSize = value;
				OnPropertyChanged("PreambleDetectorSize");
			}
		}

		public byte PreambleDetectorTol
		{
			get
			{
				return preambleDetectorTol;
			}
			set
			{
				preambleDetectorTol = value;
				OnPropertyChanged("PreambleDetectorTol");
			}
		}

		public Decimal TimeoutRxRssi
		{
			get
			{
				return timeoutRxRssi;
			}
			set
			{
				timeoutRxRssi = value;
				OnPropertyChanged("TimeoutRxRssi");
			}
		}

		public Decimal TimeoutRxPreamble
		{
			get
			{
				return timeoutRxPreamble;
			}
			set
			{
				timeoutRxPreamble = value;
				OnPropertyChanged("TimeoutRxPreamble");
			}
		}

		public Decimal TimeoutSignalSync
		{
			get
			{
				return timeoutSignalSync;
			}
			set
			{
				timeoutSignalSync = value;
				OnPropertyChanged("TimeoutSignalSync");
			}
		}

		public Decimal InterPacketRxDelay
		{
			get
			{
				return interPacketRxDelay;
			}
			set
			{
				interPacketRxDelay = value;
				OnPropertyChanged("InterPacketRxDelay");
			}
		}

		public ClockOutEnum ClockOut
		{
			get
			{
				return clockOut;
			}
			set
			{
				clockOut = value;
				OnPropertyChanged("ClockOut");
			}
		}

		public Packet Packet
		{
			get
			{
				return packet;
			}
			set
			{
				packet = value;
				packet.PropertyChanged += new PropertyChangedEventHandler(packet_PropertyChanged);
				OnPropertyChanged("Packet");
			}
		}

		public IdleMode IdleMode
		{
			get
			{
				return idleMode;
			}
			set
			{
				idleMode = value;
				OnPropertyChanged("IdleMode");
			}
		}

		public FromStart FromStart
		{
			get
			{
				return fromStart;
			}
			set
			{
				fromStart = value;
				OnPropertyChanged("FromStart");
			}
		}

		public LowPowerSelection LowPowerSelection
		{
			get
			{
				return lowPowerSelection;
			}
			set
			{
				lowPowerSelection = value;
				OnPropertyChanged("LowPowerSelection");
			}
		}

		public FromIdle FromIdle
		{
			get
			{
				return fromIdle;
			}
			set
			{
				fromIdle = value;
				OnPropertyChanged("FromIdle");
			}
		}

		public FromTransmit FromTransmit
		{
			get
			{
				return fromTransmit;
			}
			set
			{
				fromTransmit = value;
				OnPropertyChanged("FromTransmit");
			}
		}

		public FromReceive FromReceive
		{
			get
			{
				return fromReceive;
			}
			set
			{
				fromReceive = value;
				OnPropertyChanged("FromReceive");
			}
		}

		public FromRxTimeout FromRxTimeout
		{
			get
			{
				return fromRxTimeout;
			}
			set
			{
				fromRxTimeout = value;
				OnPropertyChanged("FromRxTimeout");
			}
		}

		public FromPacketReceived FromPacketReceived
		{
			get
			{
				return fromPacketReceived;
			}
			set
			{
				fromPacketReceived = value;
				OnPropertyChanged("FromPacketReceived");
			}
		}

		public TimerResolution Timer1Resolution
		{
			get
			{
				return timer1Resolution;
			}
			set
			{
				timer1Resolution = value;
				OnPropertyChanged("Timer1Resolution");
			}
		}

		public TimerResolution Timer2Resolution
		{
			get
			{
				return timer2Resolution;
			}
			set
			{
				timer2Resolution = value;
				OnPropertyChanged("Timer2Resolution");
			}
		}

		public byte Timer1Coef
		{
			get
			{
				return timer1Coef;
			}
			set
			{
				timer1Coef = value;
				OnPropertyChanged("Timer1Coef");
			}
		}

		public byte Timer2Coef
		{
			get
			{
				return timer2Coef;
			}
			set
			{
				timer2Coef = value;
				OnPropertyChanged("Timer2Coef");
			}
		}

		public bool AutoImageCalOn
		{
			get
			{
				return autoImageCalOn;
			}
			set
			{
				autoImageCalOn = value;
				OnPropertyChanged("AutoImageCalOn");
			}
		}

		public bool ImageCalRunning
		{
			get
			{
				return imageCalRunning;
			}
		}

		public bool TempChange
		{
			get
			{
				return tempChange;
			}
		}

		public TempThresholdEnum TempThreshold
		{
			get
			{
				return tempThreshold;
			}
			set
			{
				tempThreshold = value;
				OnPropertyChanged("TempThreshold");
			}
		}

		public bool TempMonitorOff
		{
			get
			{
				return tempMonitorOff;
			}
			set
			{
				tempMonitorOff = value;
				OnPropertyChanged("TempMonitorOff");
			}
		}

		public Decimal TempValue
		{
			get
			{
				return tempValue;
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
				OnPropertyChanged("TempValueRoom");
			}
		}

		public Decimal TempValueCal
		{
			get
			{
				return tempValueCal;
			}
			set
			{
				tempValueCal = value;
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
				OnPropertyChanged("TempCalDone");
			}
		}

		public bool TempMeasRunning
		{
			get
			{
				return tempMeasRunning;
			}
		}

		public bool LowBatOn
		{
			get
			{
				return lowBatOn;
			}
			set
			{
				lowBatOn = value;
				OnPropertyChanged("LowBatOn");
			}
		}

		public LowBatTrimEnum LowBatTrim
		{
			get
			{
				return lowBatTrim;
			}
			set
			{
				lowBatTrim = value;
				OnPropertyChanged("LowBatTrim");
			}
		}

		public bool ModeReady
		{
			get
			{
				return modeReady;
			}
		}

		public bool RxReady
		{
			get
			{
				return rxReady;
			}
		}

		public bool TxReady
		{
			get
			{
				return txReady;
			}
		}

		public bool PllLock
		{
			get
			{
				return pllLock;
			}
		}

		public bool Rssi
		{
			get
			{
				return rssi;
			}
		}

		public bool Timeout
		{
			get
			{
				return timeout;
			}
		}

		public bool PreambleDetect
		{
			get
			{
				return preambleDetect;
			}
		}

		public bool SyncAddressMatch
		{
			get
			{
				return syncAddressMatch;
			}
		}

		public bool FifoFull
		{
			get
			{
				return fifoFull;
			}
		}

		public bool FifoEmpty
		{
			get
			{
				return fifoEmpty;
			}
		}

		public bool FifoLevel
		{
			get
			{
				return fifoLevel;
			}
		}

		public bool FifoOverrun
		{
			get
			{
				return fifoOverrun;
			}
		}

		public bool PacketSent
		{
			get
			{
				return packetSent;
			}
		}

		public bool PayloadReady
		{
			get
			{
				return payloadReady;
			}
		}

		public bool CrcOk
		{
			get
			{
				return crcOk;
			}
		}

		public bool LowBat
		{
			get
			{
				return lowBat;
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

		public bool MapPreambleDetect
		{
			get
			{
				return mapPreambleDetect;
			}
			set
			{
				mapPreambleDetect = value;
				OnPropertyChanged("MapPreambleDetect");
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

		public int AgcReference
		{
			get
			{
				return (int)Math.Round(10.0 * Math.Log10((double)(new Decimal(2) * RxBw)) - 174.0 + (double)AgcReferenceLevel, MidpointRounding.AwayFromZero);
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

		public Decimal TempDelta
		{
			get
			{
				return tempDelta;
			}
			set
			{
				tempDelta = value;
				OnPropertyChanged("TempDelta");
			}
		}

		public Decimal FormerTemp
		{
			get
			{
				return formerTemp;
			}
			set
			{
				formerTemp = value;
				OnPropertyChanged("FormerTemp");
			}
		}

		public Decimal BitrateFrac
		{
			get
			{
				return bitrateFrac;
			}
			set
			{
				bitrateFrac = value;
				OnPropertyChanged("BitrateFrac");
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
						OnError((byte)1, exception_0.Message);
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
						OnError((byte)1, exception_0.Message);
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

		private PseudoNoiseGenerator Png
		{
			get
			{
				return png;
			}
			set
			{
				png = value;
			}
		}

		public PnSequence PngSequence
		{
			get
			{
				return png.Pn;
			}
			set
			{
				png.Pn = value;
				OnPropertyChanged("PngSequence");
			}
		}

		public bool PngEnabled
		{
			get
			{
				return pngEnabled;
			}
			set
			{
				pngEnabled = value;
				if (value)
				{
					prevPngOpMode = Mode;
					SetOperatingMode(OperatingModeEnum.Tx);
				}
				else
					SetOperatingMode(prevPngOpMode);
				OnPropertyChanged("PngEnabled");
			}
		}

		public bool IsPacketHandlerStarted
		{
			get
			{
				return isPacketHandlerStarted;
			}
		}

		public event EventHandler Connected;

		public event EventHandler Disconected;

		public event SemtechLib.General.Events.ErrorEventHandler Error;

		public event SX1276.LimitCheckStatusChangedEventHandler OcpTrimLimitStatusChanged;

		public event SX1276.LimitCheckStatusChangedEventHandler FrequencyRfLimitStatusChanged;

		public event SX1276.LimitCheckStatusChangedEventHandler BitrateLimitStatusChanged;

		public event SX1276.LimitCheckStatusChangedEventHandler FdevLimitStatusChanged;

		public event SX1276.LimitCheckStatusChangedEventHandler SyncValueLimitChanged;

		public event SX1276.IoChangedEventHandler Dio0Changed;

		public event SX1276.IoChangedEventHandler Dio1Changed;

		public event SX1276.IoChangedEventHandler Dio2Changed;

		public event SX1276.IoChangedEventHandler Dio3Changed;

		public event SX1276.IoChangedEventHandler Dio4Changed;

		public event SX1276.IoChangedEventHandler Dio5Changed;

		public event EventHandler PacketHandlerStarted;

		public event EventHandler PacketHandlerStoped;

		public event PacketStatusEventHandler PacketHandlerTransmitted;

		public event PacketStatusEventHandler PacketHandlerReceived;

		public event PropertyChangedEventHandler PropertyChanged;

		public SX1276()
		{
			PropertyChanged += new PropertyChangedEventHandler(device_PropertyChanged);
			deviceName = "SX12xxEiger";
			usbDevice = new HidDevice(1146, 11, deviceName);
			usbDevice.Opened += new EventHandler(usbDevice_Opened);
			usbDevice.Closed += new EventHandler(usbDevice_Closed);
			Dio0Changed += new SX1276.IoChangedEventHandler(device_Dio0Changed);
			Dio1Changed += new SX1276.IoChangedEventHandler(device_Dio1Changed);
			Dio2Changed += new SX1276.IoChangedEventHandler(device_Dio2Changed);
			Dio3Changed += new SX1276.IoChangedEventHandler(device_Dio3Changed);
			Dio4Changed += new SX1276.IoChangedEventHandler(device_Dio4Changed);
			Dio5Changed += new SX1276.IoChangedEventHandler(device_Dio5Changed);
			PopulateRegisters();
		}

		private void OnDio0Changed(bool state)
		{
			if (Dio0Changed == null)
				return;
			Dio0Changed((object)this, new SX1276.IoChangedEventArgs(state));
		}

		private void OnDio1Changed(bool state)
		{
			if (Dio1Changed == null)
				return;
			Dio1Changed((object)this, new SX1276.IoChangedEventArgs(state));
		}

		private void OnDio2Changed(bool state)
		{
			if (Dio2Changed == null)
				return;
			Dio2Changed((object)this, new SX1276.IoChangedEventArgs(state));
		}

		private void OnDio3Changed(bool state)
		{
			if (Dio3Changed == null)
				return;
			Dio3Changed((object)this, new SX1276.IoChangedEventArgs(state));
		}

		private void OnDio4Changed(bool state)
		{
			if (Dio4Changed == null)
				return;
			Dio4Changed((object)this, new SX1276.IoChangedEventArgs(state));
		}

		private void OnDio5Changed(bool state)
		{
			if (Dio5Changed == null)
				return;
			Dio5Changed((object)this, new SX1276.IoChangedEventArgs(state));
		}

		private void PopulateRegisters()
		{
			if (IsOpen)
			{
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
			int num11 = 9;
			int num12 = 0;
			int num13 = 1;
			Register register2 = new Register(name2, (uint)num8, (uint)num11, num12 != 0, num13 != 0);
			registerCollection2.Add(register2);
			RegisterCollection registerCollection3 = registers;
			string name3 = "RegBitrateMsb";
			int num14 = (int)num10;
			int num15 = 1;
			byte num16 = (byte)(num14 + num15);
			int num17 = 26;
			int num18 = 0;
			int num19 = 1;
			Register register3 = new Register(name3, (uint)num14, (uint)num17, num18 != 0, num19 != 0);
			registerCollection3.Add(register3);
			RegisterCollection registerCollection4 = registers;
			string name4 = "RegBitrateLsb";
			int num20 = (int)num16;
			int num21 = 1;
			byte num22 = (byte)(num20 + num21);
			int num23 = 11;
			int num24 = 0;
			int num25 = 1;
			Register register4 = new Register(name4, (uint)num20, (uint)num23, num24 != 0, num25 != 0);
			registerCollection4.Add(register4);
			RegisterCollection registerCollection5 = registers;
			string name5 = "RegFdevMsb";
			int num26 = (int)num22;
			int num27 = 1;
			byte num28 = (byte)(num26 + num27);
			int num29 = 0;
			int num30 = 0;
			int num31 = 1;
			Register register5 = new Register(name5, (uint)num26, (uint)num29, num30 != 0, num31 != 0);
			registerCollection5.Add(register5);
			RegisterCollection registerCollection6 = registers;
			string name6 = "RegFdevLsb";
			int num32 = (int)num28;
			int num33 = 1;
			byte num34 = (byte)(num32 + num33);
			int num35 = 82;
			int num36 = 0;
			int num37 = 1;
			Register register6 = new Register(name6, (uint)num32, (uint)num35, num36 != 0, num37 != 0);
			registerCollection6.Add(register6);
			RegisterCollection registerCollection7 = registers;
			string name7 = "RegFrfMsb";
			int num38 = (int)num34;
			int num39 = 1;
			byte num40 = (byte)(num38 + num39);
			int num41 = 228;
			int num42 = 0;
			int num43 = 1;
			Register register7 = new Register(name7, (uint)num38, (uint)num41, num42 != 0, num43 != 0);
			registerCollection7.Add(register7);
			RegisterCollection registerCollection8 = registers;
			string name8 = "RegFrfMid";
			int num44 = (int)num40;
			int num45 = 1;
			byte num46 = (byte)(num44 + num45);
			int num47 = 192;
			int num48 = 0;
			int num49 = 1;
			Register register8 = new Register(name8, (uint)num44, (uint)num47, num48 != 0, num49 != 0);
			registerCollection8.Add(register8);
			RegisterCollection registerCollection9 = registers;
			string name9 = "RegFrfLsb";
			int num50 = (int)num46;
			int num51 = 1;
			byte num52 = (byte)(num50 + num51);
			int num53 = 0;
			int num54 = 0;
			int num55 = 1;
			Register register9 = new Register(name9, (uint)num50, (uint)num53, num54 != 0, num55 != 0);
			registerCollection9.Add(register9);
			RegisterCollection registerCollection10 = registers;
			string name10 = "RegPaConfig";
			int num56 = (int)num52;
			int num57 = 1;
			byte num58 = (byte)(num56 + num57);
			int num59 = 15;
			int num60 = 0;
			int num61 = 1;
			Register register10 = new Register(name10, (uint)num56, (uint)num59, num60 != 0, num61 != 0);
			registerCollection10.Add(register10);
			RegisterCollection registerCollection11 = registers;
			string name11 = "RegPaRamp";
			int num62 = (int)num58;
			int num63 = 1;
			byte num64 = (byte)(num62 + num63);
			int num65 = 25;
			int num66 = 0;
			int num67 = 1;
			Register register11 = new Register(name11, (uint)num62, (uint)num65, num66 != 0, num67 != 0);
			registerCollection11.Add(register11);
			RegisterCollection registerCollection12 = registers;
			string name12 = "RegOcp";
			int num68 = (int)num64;
			int num69 = 1;
			byte num70 = (byte)(num68 + num69);
			int num71 = 43;
			int num72 = 0;
			int num73 = 1;
			Register register12 = new Register(name12, (uint)num68, (uint)num71, num72 != 0, num73 != 0);
			registerCollection12.Add(register12);
			RegisterCollection registerCollection13 = registers;
			string name13 = "RegLna";
			int num74 = (int)num70;
			int num75 = 1;
			byte num76 = (byte)(num74 + num75);
			int num77 = 32;
			int num78 = 0;
			int num79 = 1;
			Register register13 = new Register(name13, (uint)num74, (uint)num77, num78 != 0, num79 != 0);
			registerCollection13.Add(register13);
			RegisterCollection registerCollection14 = registers;
			string name14 = "RegRxConfig";
			int num80 = (int)num76;
			int num81 = 1;
			byte num82 = (byte)(num80 + num81);
			int num83 = 8;
			int num84 = 0;
			int num85 = 1;
			Register register14 = new Register(name14, (uint)num80, (uint)num83, num84 != 0, num85 != 0);
			registerCollection14.Add(register14);
			RegisterCollection registerCollection15 = registers;
			string name15 = "RegRssiConfig";
			int num86 = (int)num82;
			int num87 = 1;
			byte num88 = (byte)(num86 + num87);
			int num89 = 2;
			int num90 = 0;
			int num91 = 1;
			Register register15 = new Register(name15, (uint)num86, (uint)num89, num90 != 0, num91 != 0);
			registerCollection15.Add(register15);
			RegisterCollection registerCollection16 = registers;
			string name16 = "RegRssiCollision";
			int num92 = (int)num88;
			int num93 = 1;
			byte num94 = (byte)(num92 + num93);
			int num95 = 10;
			int num96 = 0;
			int num97 = 1;
			Register register16 = new Register(name16, (uint)num92, (uint)num95, num96 != 0, num97 != 0);
			registerCollection16.Add(register16);
			RegisterCollection registerCollection17 = registers;
			string name17 = "RegRssiThresh";
			int num98 = (int)num94;
			int num99 = 1;
			byte num100 = (byte)(num98 + num99);
			int num101 = (int)byte.MaxValue;
			int num102 = 0;
			int num103 = 1;
			Register register17 = new Register(name17, (uint)num98, (uint)num101, num102 != 0, num103 != 0);
			registerCollection17.Add(register17);
			RegisterCollection registerCollection18 = registers;
			string name18 = "RegRssiValue";
			int num104 = (int)num100;
			int num105 = 1;
			byte num106 = (byte)(num104 + num105);
			int num107 = 0;
			int num108 = 0;
			int num109 = 1;
			Register register18 = new Register(name18, (uint)num104, (uint)num107, num108 != 0, num109 != 0);
			registerCollection18.Add(register18);
			RegisterCollection registerCollection19 = registers;
			string name19 = "RegRxBw";
			int num110 = (int)num106;
			int num111 = 1;
			byte num112 = (byte)(num110 + num111);
			int num113 = 21;
			int num114 = 0;
			int num115 = 1;
			Register register19 = new Register(name19, (uint)num110, (uint)num113, num114 != 0, num115 != 0);
			registerCollection19.Add(register19);
			RegisterCollection registerCollection20 = registers;
			string name20 = "RegAfcBw";
			int num116 = (int)num112;
			int num117 = 1;
			byte num118 = (byte)(num116 + num117);
			int num119 = 11;
			int num120 = 0;
			int num121 = 1;
			Register register20 = new Register(name20, (uint)num116, (uint)num119, num120 != 0, num121 != 0);
			registerCollection20.Add(register20);
			RegisterCollection registerCollection21 = registers;
			string name21 = "RegOokPeak";
			int num122 = (int)num118;
			int num123 = 1;
			byte num124 = (byte)(num122 + num123);
			int num125 = 40;
			int num126 = 0;
			int num127 = 1;
			Register register21 = new Register(name21, (uint)num122, (uint)num125, num126 != 0, num127 != 0);
			registerCollection21.Add(register21);
			RegisterCollection registerCollection22 = registers;
			string name22 = "RegOokFix";
			int num128 = (int)num124;
			int num129 = 1;
			byte num130 = (byte)(num128 + num129);
			int num131 = 12;
			int num132 = 0;
			int num133 = 1;
			Register register22 = new Register(name22, (uint)num128, (uint)num131, num132 != 0, num133 != 0);
			registerCollection22.Add(register22);
			RegisterCollection registerCollection23 = registers;
			string name23 = "RegOokAvg";
			int num134 = (int)num130;
			int num135 = 1;
			byte num136 = (byte)(num134 + num135);
			int num137 = 18;
			int num138 = 0;
			int num139 = 1;
			Register register23 = new Register(name23, (uint)num134, (uint)num137, num138 != 0, num139 != 0);
			registerCollection23.Add(register23);
			RegisterCollection registerCollection24 = registers;
			string name24 = "RegRes17";
			int num140 = (int)num136;
			int num141 = 1;
			byte num142 = (byte)(num140 + num141);
			int num143 = 71;
			int num144 = 0;
			int num145 = 1;
			Register register24 = new Register(name24, (uint)num140, (uint)num143, num144 != 0, num145 != 0);
			registerCollection24.Add(register24);
			RegisterCollection registerCollection25 = registers;
			string name25 = "RegRes18";
			int num146 = (int)num142;
			int num147 = 1;
			byte num148 = (byte)(num146 + num147);
			int num149 = 50;
			int num150 = 0;
			int num151 = 1;
			Register register25 = new Register(name25, (uint)num146, (uint)num149, num150 != 0, num151 != 0);
			registerCollection25.Add(register25);
			RegisterCollection registerCollection26 = registers;
			string name26 = "RegRes19";
			int num152 = (int)num148;
			int num153 = 1;
			byte num154 = (byte)(num152 + num153);
			int num155 = 62;
			int num156 = 0;
			int num157 = 1;
			Register register26 = new Register(name26, (uint)num152, (uint)num155, num156 != 0, num157 != 0);
			registerCollection26.Add(register26);
			RegisterCollection registerCollection27 = registers;
			string name27 = "RegAfcFei";
			int num158 = (int)num154;
			int num159 = 1;
			byte num160 = (byte)(num158 + num159);
			int num161 = 0;
			int num162 = 0;
			int num163 = 1;
			Register register27 = new Register(name27, (uint)num158, (uint)num161, num162 != 0, num163 != 0);
			registerCollection27.Add(register27);
			RegisterCollection registerCollection28 = registers;
			string name28 = "RegAfcMsb";
			int num164 = (int)num160;
			int num165 = 1;
			byte num166 = (byte)(num164 + num165);
			int num167 = 0;
			int num168 = 0;
			int num169 = 1;
			Register register28 = new Register(name28, (uint)num164, (uint)num167, num168 != 0, num169 != 0);
			registerCollection28.Add(register28);
			RegisterCollection registerCollection29 = registers;
			string name29 = "RegAfcLsb";
			int num170 = (int)num166;
			int num171 = 1;
			byte num172 = (byte)(num170 + num171);
			int num173 = 0;
			int num174 = 0;
			int num175 = 1;
			Register register29 = new Register(name29, (uint)num170, (uint)num173, num174 != 0, num175 != 0);
			registerCollection29.Add(register29);
			RegisterCollection registerCollection30 = registers;
			string name30 = "RegFeiMsb";
			int num176 = (int)num172;
			int num177 = 1;
			byte num178 = (byte)(num176 + num177);
			int num179 = 0;
			int num180 = 0;
			int num181 = 1;
			Register register30 = new Register(name30, (uint)num176, (uint)num179, num180 != 0, num181 != 0);
			registerCollection30.Add(register30);
			RegisterCollection registerCollection31 = registers;
			string name31 = "RegFeiLsb";
			int num182 = (int)num178;
			int num183 = 1;
			byte num184 = (byte)(num182 + num183);
			int num185 = 0;
			int num186 = 0;
			int num187 = 1;
			Register register31 = new Register(name31, (uint)num182, (uint)num185, num186 != 0, num187 != 0);
			registerCollection31.Add(register31);
			RegisterCollection registerCollection32 = registers;
			string name32 = "RegPreambleDetect";
			int num188 = (int)num184;
			int num189 = 1;
			byte num190 = (byte)(num188 + num189);
			int num191 = 64;
			int num192 = 0;
			int num193 = 1;
			Register register32 = new Register(name32, (uint)num188, (uint)num191, num192 != 0, num193 != 0);
			registerCollection32.Add(register32);
			RegisterCollection registerCollection33 = registers;
			string name33 = "RegRxTimeout1";
			int num194 = (int)num190;
			int num195 = 1;
			byte num196 = (byte)(num194 + num195);
			int num197 = 0;
			int num198 = 0;
			int num199 = 1;
			Register register33 = new Register(name33, (uint)num194, (uint)num197, num198 != 0, num199 != 0);
			registerCollection33.Add(register33);
			RegisterCollection registerCollection34 = registers;
			string name34 = "RegRxTimeout2";
			int num200 = (int)num196;
			int num201 = 1;
			byte num202 = (byte)(num200 + num201);
			int num203 = 0;
			int num204 = 0;
			int num205 = 1;
			Register register34 = new Register(name34, (uint)num200, (uint)num203, num204 != 0, num205 != 0);
			registerCollection34.Add(register34);
			RegisterCollection registerCollection35 = registers;
			string name35 = "RegRxTimeout3";
			int num206 = (int)num202;
			int num207 = 1;
			byte num208 = (byte)(num206 + num207);
			int num209 = 0;
			int num210 = 0;
			int num211 = 1;
			Register register35 = new Register(name35, (uint)num206, (uint)num209, num210 != 0, num211 != 0);
			registerCollection35.Add(register35);
			RegisterCollection registerCollection36 = registers;
			string name36 = "RegRxDelay";
			int num212 = (int)num208;
			int num213 = 1;
			byte num214 = (byte)(num212 + num213);
			int num215 = 0;
			int num216 = 0;
			int num217 = 1;
			Register register36 = new Register(name36, (uint)num212, (uint)num215, num216 != 0, num217 != 0);
			registerCollection36.Add(register36);
			RegisterCollection registerCollection37 = registers;
			string name37 = "RegOsc";
			int num218 = (int)num214;
			int num219 = 1;
			byte num220 = (byte)(num218 + num219);
			int num221 = 5;
			int num222 = 0;
			int num223 = 1;
			Register register37 = new Register(name37, (uint)num218, (uint)num221, num222 != 0, num223 != 0);
			registerCollection37.Add(register37);
			RegisterCollection registerCollection38 = registers;
			string name38 = "RegPreambleMsb";
			int num224 = (int)num220;
			int num225 = 1;
			byte num226 = (byte)(num224 + num225);
			int num227 = 0;
			int num228 = 0;
			int num229 = 1;
			Register register38 = new Register(name38, (uint)num224, (uint)num227, num228 != 0, num229 != 0);
			registerCollection38.Add(register38);
			RegisterCollection registerCollection39 = registers;
			string name39 = "RegPreambleLsb";
			int num230 = (int)num226;
			int num231 = 1;
			byte num232 = (byte)(num230 + num231);
			int num233 = 3;
			int num234 = 0;
			int num235 = 1;
			Register register39 = new Register(name39, (uint)num230, (uint)num233, num234 != 0, num235 != 0);
			registerCollection39.Add(register39);
			RegisterCollection registerCollection40 = registers;
			string name40 = "RegSyncConfig";
			int num236 = (int)num232;
			int num237 = 1;
			byte num238 = (byte)(num236 + num237);
			int num239 = 147;
			int num240 = 0;
			int num241 = 1;
			Register register40 = new Register(name40, (uint)num236, (uint)num239, num240 != 0, num241 != 0);
			registerCollection40.Add(register40);
			RegisterCollection registerCollection41 = registers;
			string name41 = "RegSyncValue1";
			int num242 = (int)num238;
			int num243 = 1;
			byte num244 = (byte)(num242 + num243);
			int num245 = 85;
			int num246 = 0;
			int num247 = 1;
			Register register41 = new Register(name41, (uint)num242, (uint)num245, num246 != 0, num247 != 0);
			registerCollection41.Add(register41);
			RegisterCollection registerCollection42 = registers;
			string name42 = "RegSyncValue2";
			int num248 = (int)num244;
			int num249 = 1;
			byte num250 = (byte)(num248 + num249);
			int num251 = 85;
			int num252 = 0;
			int num253 = 1;
			Register register42 = new Register(name42, (uint)num248, (uint)num251, num252 != 0, num253 != 0);
			registerCollection42.Add(register42);
			RegisterCollection registerCollection43 = registers;
			string name43 = "RegSyncValue3";
			int num254 = (int)num250;
			int num255 = 1;
			byte num256 = (byte)(num254 + num255);
			int num257 = 85;
			int num258 = 0;
			int num259 = 1;
			Register register43 = new Register(name43, (uint)num254, (uint)num257, num258 != 0, num259 != 0);
			registerCollection43.Add(register43);
			RegisterCollection registerCollection44 = registers;
			string name44 = "RegSyncValue4";
			int num260 = (int)num256;
			int num261 = 1;
			byte num262 = (byte)(num260 + num261);
			int num263 = 85;
			int num264 = 0;
			int num265 = 1;
			Register register44 = new Register(name44, (uint)num260, (uint)num263, num264 != 0, num265 != 0);
			registerCollection44.Add(register44);
			RegisterCollection registerCollection45 = registers;
			string name45 = "RegSyncValue5";
			int num266 = (int)num262;
			int num267 = 1;
			byte num268 = (byte)(num266 + num267);
			int num269 = 85;
			int num270 = 0;
			int num271 = 1;
			Register register45 = new Register(name45, (uint)num266, (uint)num269, num270 != 0, num271 != 0);
			registerCollection45.Add(register45);
			RegisterCollection registerCollection46 = registers;
			string name46 = "RegSyncValue6";
			int num272 = (int)num268;
			int num273 = 1;
			byte num274 = (byte)(num272 + num273);
			int num275 = 85;
			int num276 = 0;
			int num277 = 1;
			Register register46 = new Register(name46, (uint)num272, (uint)num275, num276 != 0, num277 != 0);
			registerCollection46.Add(register46);
			RegisterCollection registerCollection47 = registers;
			string name47 = "RegSyncValue7";
			int num278 = (int)num274;
			int num279 = 1;
			byte num280 = (byte)(num278 + num279);
			int num281 = 85;
			int num282 = 0;
			int num283 = 1;
			Register register47 = new Register(name47, (uint)num278, (uint)num281, num282 != 0, num283 != 0);
			registerCollection47.Add(register47);
			RegisterCollection registerCollection48 = registers;
			string name48 = "RegSyncValue8";
			int num284 = (int)num280;
			int num285 = 1;
			byte num286 = (byte)(num284 + num285);
			int num287 = 85;
			int num288 = 0;
			int num289 = 1;
			Register register48 = new Register(name48, (uint)num284, (uint)num287, num288 != 0, num289 != 0);
			registerCollection48.Add(register48);
			RegisterCollection registerCollection49 = registers;
			string name49 = "RegPacketConfig1";
			int num290 = (int)num286;
			int num291 = 1;
			byte num292 = (byte)(num290 + num291);
			int num293 = 144;
			int num294 = 0;
			int num295 = 1;
			Register register49 = new Register(name49, (uint)num290, (uint)num293, num294 != 0, num295 != 0);
			registerCollection49.Add(register49);
			RegisterCollection registerCollection50 = registers;
			string name50 = "RegPacketConfig2";
			int num296 = (int)num292;
			int num297 = 1;
			byte num298 = (byte)(num296 + num297);
			int num299 = 64;
			int num300 = 0;
			int num301 = 1;
			Register register50 = new Register(name50, (uint)num296, (uint)num299, num300 != 0, num301 != 0);
			registerCollection50.Add(register50);
			RegisterCollection registerCollection51 = registers;
			string name51 = "RegPayloadLength";
			int num302 = (int)num298;
			int num303 = 1;
			byte num304 = (byte)(num302 + num303);
			int num305 = 64;
			int num306 = 0;
			int num307 = 1;
			Register register51 = new Register(name51, (uint)num302, (uint)num305, num306 != 0, num307 != 0);
			registerCollection51.Add(register51);
			RegisterCollection registerCollection52 = registers;
			string name52 = "RegNodeAdrs";
			int num308 = (int)num304;
			int num309 = 1;
			byte num310 = (byte)(num308 + num309);
			int num311 = 0;
			int num312 = 0;
			int num313 = 1;
			Register register52 = new Register(name52, (uint)num308, (uint)num311, num312 != 0, num313 != 0);
			registerCollection52.Add(register52);
			RegisterCollection registerCollection53 = registers;
			string name53 = "RegBroadcastAdrs";
			int num314 = (int)num310;
			int num315 = 1;
			byte num316 = (byte)(num314 + num315);
			int num317 = 0;
			int num318 = 0;
			int num319 = 1;
			Register register53 = new Register(name53, (uint)num314, (uint)num317, num318 != 0, num319 != 0);
			registerCollection53.Add(register53);
			RegisterCollection registerCollection54 = registers;
			string name54 = "RegFifoThresh";
			int num320 = (int)num316;
			int num321 = 1;
			byte num322 = (byte)(num320 + num321);
			int num323 = 15;
			int num324 = 0;
			int num325 = 1;
			Register register54 = new Register(name54, (uint)num320, (uint)num323, num324 != 0, num325 != 0);
			registerCollection54.Add(register54);
			RegisterCollection registerCollection55 = registers;
			string name55 = "RegSeqConfig1";
			int num326 = (int)num322;
			int num327 = 1;
			byte num328 = (byte)(num326 + num327);
			int num329 = 0;
			int num330 = 0;
			int num331 = 1;
			Register register55 = new Register(name55, (uint)num326, (uint)num329, num330 != 0, num331 != 0);
			registerCollection55.Add(register55);
			RegisterCollection registerCollection56 = registers;
			string name56 = "RegSeqConfig2";
			int num332 = (int)num328;
			int num333 = 1;
			byte num334 = (byte)(num332 + num333);
			int num335 = 0;
			int num336 = 0;
			int num337 = 1;
			Register register56 = new Register(name56, (uint)num332, (uint)num335, num336 != 0, num337 != 0);
			registerCollection56.Add(register56);
			RegisterCollection registerCollection57 = registers;
			string name57 = "RegTimerResol";
			int num338 = (int)num334;
			int num339 = 1;
			byte num340 = (byte)(num338 + num339);
			int num341 = 0;
			int num342 = 0;
			int num343 = 1;
			Register register57 = new Register(name57, (uint)num338, (uint)num341, num342 != 0, num343 != 0);
			registerCollection57.Add(register57);
			RegisterCollection registerCollection58 = registers;
			string name58 = "RegTimer1Coef";
			int num344 = (int)num340;
			int num345 = 1;
			byte num346 = (byte)(num344 + num345);
			int num347 = 245;
			int num348 = 0;
			int num349 = 1;
			Register register58 = new Register(name58, (uint)num344, (uint)num347, num348 != 0, num349 != 0);
			registerCollection58.Add(register58);
			RegisterCollection registerCollection59 = registers;
			string name59 = "RegTimer2Coef";
			int num350 = (int)num346;
			int num351 = 1;
			byte num352 = (byte)(num350 + num351);
			int num353 = 32;
			int num354 = 0;
			int num355 = 1;
			Register register59 = new Register(name59, (uint)num350, (uint)num353, num354 != 0, num355 != 0);
			registerCollection59.Add(register59);
			RegisterCollection registerCollection60 = registers;
			string name60 = "RegImageCal";
			int num356 = (int)num352;
			int num357 = 1;
			byte num358 = (byte)(num356 + num357);
			int num359 = 130;
			int num360 = 0;
			int num361 = 1;
			Register register60 = new Register(name60, (uint)num356, (uint)num359, num360 != 0, num361 != 0);
			registerCollection60.Add(register60);
			RegisterCollection registerCollection61 = registers;
			string name61 = "RegTemp";
			int num362 = (int)num358;
			int num363 = 1;
			byte num364 = (byte)(num362 + num363);
			int num365 = 0;
			int num366 = 0;
			int num367 = 1;
			Register register61 = new Register(name61, (uint)num362, (uint)num365, num366 != 0, num367 != 0);
			registerCollection61.Add(register61);
			RegisterCollection registerCollection62 = registers;
			string name62 = "RegLowBat";
			int num368 = (int)num364;
			int num369 = 1;
			byte num370 = (byte)(num368 + num369);
			int num371 = 2;
			int num372 = 0;
			int num373 = 1;
			Register register62 = new Register(name62, (uint)num368, (uint)num371, num372 != 0, num373 != 0);
			registerCollection62.Add(register62);
			RegisterCollection registerCollection63 = registers;
			string name63 = "RegIrqFlags1";
			int num374 = (int)num370;
			int num375 = 1;
			byte num376 = (byte)(num374 + num375);
			int num377 = 128;
			int num378 = 0;
			int num379 = 1;
			Register register63 = new Register(name63, (uint)num374, (uint)num377, num378 != 0, num379 != 0);
			registerCollection63.Add(register63);
			RegisterCollection registerCollection64 = registers;
			string name64 = "RegIrqFlags2";
			int num380 = (int)num376;
			int num381 = 1;
			byte num382 = (byte)(num380 + num381);
			int num383 = 64;
			int num384 = 0;
			int num385 = 1;
			Register register64 = new Register(name64, (uint)num380, (uint)num383, num384 != 0, num385 != 0);
			registerCollection64.Add(register64);
			RegisterCollection registerCollection65 = registers;
			string name65 = "RegDioMapping1";
			int num386 = (int)num382;
			int num387 = 1;
			byte num388 = (byte)(num386 + num387);
			int num389 = 0;
			int num390 = 0;
			int num391 = 1;
			Register register65 = new Register(name65, (uint)num386, (uint)num389, num390 != 0, num391 != 0);
			registerCollection65.Add(register65);
			RegisterCollection registerCollection66 = registers;
			string name66 = "RegDioMapping2";
			int num392 = (int)num388;
			int num393 = 1;
			byte num394 = (byte)(num392 + num393);
			int num395 = 0;
			int num396 = 0;
			int num397 = 1;
			Register register66 = new Register(name66, (uint)num392, (uint)num395, num396 != 0, num397 != 0);
			registerCollection66.Add(register66);
			RegisterCollection registerCollection67 = registers;
			string name67 = "RegVersion";
			int num398 = (int)num394;
			int num399 = 1;
			byte num400 = (byte)(num398 + num399);
			int num401 = 17;
			int num402 = 0;
			int num403 = 1;
			Register register67 = new Register(name67, (uint)num398, (uint)num401, num402 != 0, num403 != 0);
			registerCollection67.Add(register67);
			for (int index = 67; index < 68; ++index)
				registers.Add(new Register("RegTest" + num400.ToString("X02"), (uint)num400++, 0U, false, true));
			RegisterCollection registerCollection68 = registers;
			string name68 = "RegPllHop";
			int num404 = (int)num400;
			int num405 = 1;
			byte num406 = (byte)(num404 + num405);
			int num407 = 45;
			int num408 = 0;
			int num409 = 1;
			Register register68 = new Register(name68, (uint)num404, (uint)num407, num408 != 0, num409 != 0);
			registerCollection68.Add(register68);
			for (int index = 69; index < 75; ++index)
				registers.Add(new Register("RegTest" + num406.ToString("X02"), (uint)num406++, 0U, false, true));
			RegisterCollection registerCollection69 = registers;
			string name69 = "RegTcxo";
			int num410 = (int)num406;
			int num411 = 1;
			byte num412 = (byte)(num410 + num411);
			int num413 = 9;
			int num414 = 0;
			int num415 = 1;
			Register register69 = new Register(name69, (uint)num410, (uint)num413, num414 != 0, num415 != 0);
			registerCollection69.Add(register69);
			RegisterCollection registerCollection70 = registers;
			string name70 = "RegTest4C";
			int num416 = (int)num412;
			int num417 = 1;
			byte num418 = (byte)(num416 + num417);
			int num419 = 0;
			int num420 = 0;
			int num421 = 1;
			Register register70 = new Register(name70, (uint)num416, (uint)num419, num420 != 0, num421 != 0);
			registerCollection70.Add(register70);
			RegisterCollection registerCollection71 = registers;
			string name71 = "RegPaDac";
			int num422 = (int)num418;
			int num423 = 1;
			byte num424 = (byte)(num422 + num423);
			int num425 = 132;
			int num426 = 0;
			int num427 = 1;
			Register register71 = new Register(name71, (uint)num422, (uint)num425, num426 != 0, num427 != 0);
			registerCollection71.Add(register71);
			for (int index = 78; index < 91; ++index)
				registers.Add(new Register("RegTest" + num424.ToString("X02"), (uint)num424++, 0U, false, true));
			RegisterCollection registerCollection72 = registers;
			string name72 = "RegFormerTemp";
			int num428 = (int)num424;
			int num429 = 1;
			byte num430 = (byte)(num428 + num429);
			int num431 = 0;
			int num432 = 0;
			int num433 = 1;
			Register register72 = new Register(name72, (uint)num428, (uint)num431, num432 != 0, num433 != 0);
			registerCollection72.Add(register72);
			RegisterCollection registerCollection73 = registers;
			string name73 = "RegTest5C";
			int num434 = (int)num430;
			int num435 = 1;
			byte num436 = (byte)(num434 + num435);
			int num437 = 0;
			int num438 = 0;
			int num439 = 1;
			Register register73 = new Register(name73, (uint)num434, (uint)num437, num438 != 0, num439 != 0);
			registerCollection73.Add(register73);
			RegisterCollection registerCollection74 = registers;
			string name74 = "RegBitrateFrac";
			int num440 = (int)num436;
			int num441 = 1;
			byte num442 = (byte)(num440 + num441);
			int num443 = 0;
			int num444 = 0;
			int num445 = 1;
			Register register74 = new Register(name74, (uint)num440, (uint)num443, num444 != 0, num445 != 0);
			registerCollection74.Add(register74);
			for (int index = 94; index < 97; ++index)
				registers.Add(new Register("RegTest" + num442.ToString("X02"), (uint)num442++, 0U, false, true));
			RegisterCollection registerCollection75 = registers;
			string name75 = "RegAgcRef";
			int num446 = (int)num442;
			int num447 = 1;
			byte num448 = (byte)(num446 + num447);
			int num449 = 25;
			int num450 = 0;
			int num451 = 1;
			Register register75 = new Register(name75, (uint)num446, (uint)num449, num450 != 0, num451 != 0);
			registerCollection75.Add(register75);
			RegisterCollection registerCollection76 = registers;
			string name76 = "RegAgcThresh1";
			int num452 = (int)num448;
			int num453 = 1;
			byte num454 = (byte)(num452 + num453);
			int num455 = 12;
			int num456 = 0;
			int num457 = 1;
			Register register76 = new Register(name76, (uint)num452, (uint)num455, num456 != 0, num457 != 0);
			registerCollection76.Add(register76);
			RegisterCollection registerCollection77 = registers;
			string name77 = "RegAgcThresh2";
			int num458 = (int)num454;
			int num459 = 1;
			byte num460 = (byte)(num458 + num459);
			int num461 = 75;
			int num462 = 0;
			int num463 = 1;
			Register register77 = new Register(name77, (uint)num458, (uint)num461, num462 != 0, num463 != 0);
			registerCollection77.Add(register77);
			RegisterCollection registerCollection78 = registers;
			string name78 = "RegAgcThresh3";
			int num464 = (int)num460;
			int num465 = 1;
			byte num466 = (byte)(num464 + num465);
			int num467 = 204;
			int num468 = 0;
			int num469 = 1;
			Register register78 = new Register(name78, (uint)num464, (uint)num467, num468 != 0, num469 != 0);
			registerCollection78.Add(register78);
			for (int index = 101; index < 112; ++index)
				registers.Add(new Register("RegTest" + num466.ToString("X02"), (uint)num466++, 0U, false, true));
			RegisterCollection registerCollection79 = registers;
			string name79 = "RegPll";
			int num470 = (int)num466;
			int num471 = 1;
			byte num472 = (byte)(num470 + num471);
			int num473 = 208;
			int num474 = 0;
			int num475 = 1;
			Register register79 = new Register(name79, (uint)num470, (uint)num473, num474 != 0, num475 != 0);
			registerCollection79.Add(register79);
			for (int index = 113; index < 128; ++index)
				registers.Add(new Register("RegTest" + num472.ToString("X02"), (uint)num472++, 0U, false, true));
			foreach (Register register80 in registers)
				register80.PropertyChanged += new PropertyChangedEventHandler(registers_PropertyChanged);
			Packet = new Packet();
		}

		private void UpdateRegisterTable()
		{
			if (lowFrequencyMode == LowFrequencyModeOn)
				return;
			lowFrequencyMode = LowFrequencyModeOn;
			ReadRegisters();
		}

		private void UpdateSyncValue()
		{
			int num = (int)registers["RegSyncValue1"].Address;
			for (int index = 0; index < packet.SyncValue.Length; ++index)
				packet.SyncValue[index] = (byte)registers[num + index].Value;
			SyncValueCheck(packet.SyncValue);
			OnPropertyChanged("SyncValue");
		}

		private void UpdateReceiverData()
		{
			OnPropertyChanged("RxBwMin");
			OnPropertyChanged("RxBwMax");
			switch ((registers["RegRxBw"].Value & 24U) >> 3)
			{
				case 0U:
					rxBw = SX1276.ComputeRxBw(frequencyXo, modulationType, 16, (int)registers["RegRxBw"].Value & 7);
					break;
				case 1U:
					rxBw = SX1276.ComputeRxBw(frequencyXo, modulationType, 20, (int)registers["RegRxBw"].Value & 7);
					break;
				case 2U:
					rxBw = SX1276.ComputeRxBw(frequencyXo, modulationType, 24, (int)registers["RegRxBw"].Value & 7);
					break;
				default:
					throw new Exception("Invalid RxBwMant parameter");
			}
			OnPropertyChanged("RxBw");
			OnPropertyChanged("AfcRxBwMin");
			OnPropertyChanged("AfcRxBwMax");
			switch ((registers["RegAfcBw"].Value & 24U) >> 3)
			{
				case 0U:
					afcRxBw = SX1276.ComputeRxBw(frequencyXo, modulationType, 16, (int)registers["RegAfcBw"].Value & 7);
					break;
				case 1U:
					afcRxBw = SX1276.ComputeRxBw(frequencyXo, modulationType, 20, (int)registers["RegAfcBw"].Value & 7);
					break;
				case 2U:
					afcRxBw = SX1276.ComputeRxBw(frequencyXo, modulationType, 24, (int)registers["RegAfcBw"].Value & 7);
					break;
				default:
					throw new Exception("Invalid RxBwMant parameter");
			}
			OnPropertyChanged("AfcRxBw");
		}

		private void UpdateRegisterValue(Register r)
		{
			switch (r.Name)
			{
				case "RegOpMode":
					if (((int)r.Value & 96) == 64 || ((int)r.Value & 96) == 96)
						r.Value &= 159U;
					ModulationType = (ModulationTypeEnum)((int)(r.Value >> 5) & 3);
					UpdateReceiverData();
					BitrateFdevCheck(bitrate, fdev);
					LowFrequencyModeOn = ((int)(r.Value >> 3) & 1) == 1;
					byte num2 = (byte)(r.Value & 7U);
					if ((int)num2 > 5)
						num2 = (byte)0;
					Mode = (OperatingModeEnum)num2;
					if (packet.Mode != Mode)
						packet.Mode = Mode;
					if ((long)((uint)(((int)registers["RegPacketConfig2"].Value & 7) << 8) | registers["RegPayloadLength"].Value) != (long)packet.PayloadLength)
					{
						registers["RegPacketConfig2"].Value = registers["RegPacketConfig2"].Value | (uint)(byte)((int)packet.PayloadLength >> 8 & 7);
						registers["RegPayloadLength"].Value = (uint)(byte)packet.PayloadLength;
					}
					lock (syncThread)
					{
						SetModeLeds(Mode);
						break;
					}
				case "RegBitrateMsb":
				case "RegBitrateLsb":
					if (((int)registers["RegBitrateMsb"].Value << 8 | (int)registers["RegBitrateLsb"].Value) == 0)
						registers["RegBitrateLsb"].Value = 1U;
					if (ModulationType == ModulationTypeEnum.FSK)
					{
						Bitrate = frequencyXo / ((Decimal)(registers["RegBitrateMsb"].Value << 8 | registers["RegBitrateLsb"].Value) + (Decimal)registers["RegBitrateFrac"].Value / new Decimal(160, 0, 0, false, (byte)1));
						break;
					}
					Bitrate = frequencyXo / (Decimal)(registers["RegBitrateMsb"].Value << 8 | registers["RegBitrateLsb"].Value);
					break;
				case "RegFdevMsb":
				case "RegFdevLsb":
					Band = (BandEnum)((int)(registers["RegFdevMsb"].Value >> 6) & 3);
					Fdev = (Decimal)((uint)(((int)registers["RegFdevMsb"].Value & 63) << 8) | registers["RegFdevLsb"].Value) * FrequencyStep;
					break;
				case "RegFrfMsb":
				case "RegFrfMid":
				case "RegFrfLsb":
					FrequencyRf = (Decimal)((uint)((int)registers["RegFrfMsb"].Value << 16 | (int)registers["RegFrfMid"].Value << 8) | registers["RegFrfLsb"].Value) * FrequencyStep;
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
					ModulationShaping = (byte)(r.Value >> 5 & 3U);
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
					break;
				case "RegRxConfig":
					RestartRxOnCollision = ((int)(r.Value >> 7) & 1) == 1;
					AfcAutoOn = ((int)(r.Value >> 4) & 1) == 1;
					AgcAutoOn = ((int)(r.Value >> 3) & 1) == 1;
					switch (r.Value & 7U)
					{
						case 1U:
							RxTrigger = RxTriggerEnum.RX_TRIGER_001;
							return;
						case 6U:
							RxTrigger = RxTriggerEnum.RX_TRIGER_110;
							return;
						case 7U:
							RxTrigger = RxTriggerEnum.RX_TRIGER_111;
							return;
						default:
							RxTrigger = RxTriggerEnum.RX_TRIGER_000;
							return;
					}
				case "RegRssiConfig":
					sbyte num3 = (sbyte)(r.Value >> 3);
					if ((num3 & 0x10) == 0x10)
						num3 = (sbyte)(-(sbyte)((int)(sbyte)((int)~num3 & 31) + 1));
					RssiOffset = (Decimal)num3;
					RssiSmoothing = (Decimal)Math.Pow(2.0, (double)(uint)(((int)r.Value & 7) + 1));
					break;
				case "RegRssiCollision":
					RssiCollisionThreshold = (Decimal)r.Value;
					break;
				case "RegRssiThresh":
					RssiThreshold = -(Decimal)r.Value / new Decimal(20, 0, 0, false, (byte)1);
					break;
				case "RegRssiValue":
					prevRssiValue = rssiValue;
					rssiValue = -(Decimal)r.Value / new Decimal(20, 0, 0, false, (byte)1);
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
					OnPropertyChanged("RssiValue");
					OnPropertyChanged("SpectrumData");
					break;
				case "RegRxBw":
					int mant1;
					switch ((r.Value & 24U) >> 3)
					{
						case 0U:
							mant1 = 16;
							break;
						case 1U:
							mant1 = 20;
							break;
						case 2U:
							mant1 = 24;
							break;
						default:
							throw new Exception("Invalid RxBwMant parameter");
					}
					rxBw = SX1276.ComputeRxBw(frequencyXo, modulationType, mant1, (int)r.Value & 7);
					OnPropertyChanged("RxBwMin");
					OnPropertyChanged("RxBwMax");
					OnPropertyChanged("RxBw");
					OnPropertyChanged("AgcReference");
					OnPropertyChanged("AgcThresh1");
					OnPropertyChanged("AgcThresh2");
					OnPropertyChanged("AgcThresh3");
					OnPropertyChanged("AgcThresh4");
					OnPropertyChanged("AgcThresh5");
					break;
				case "RegAfcBw":
					int mant2;
					switch ((r.Value & 24U) >> 3)
					{
						case 0U:
							mant2 = 16;
							break;
						case 1U:
							mant2 = 20;
							break;
						case 2U:
							mant2 = 24;
							break;
						default:
							throw new Exception("Invalid RxBwMant parameter");
					}
					afcRxBw = SX1276.ComputeRxBw(frequencyXo, modulationType, mant2, (int)r.Value & 7);
					OnPropertyChanged("AfcRxBwMin");
					OnPropertyChanged("AfcRxBwMax");
					OnPropertyChanged("AfcRxBw");
					break;
				case "RegOokPeak":
					BitSyncOn = ((int)(r.Value >> 5) & 1) == 1;
					OokThreshType = (OokThreshTypeEnum)((int)(r.Value >> 3) & 3);
					OokPeakThreshStep = OoPeakThreshStepTable[r.Value & 0x07];
					break;
				case "RegOokFix":
					OokFixedThreshold = (byte)r.Value;
					break;
				case "RegOokAvg":
					OokPeakThreshDec = (OokPeakThreshDecEnum)((int)(r.Value >> 5) & 7);
					OokAverageOffset = (Decimal)((uint)(((int)(r.Value >> 2) & 3) * 2));
					OokAverageThreshFilt = (OokAverageThreshFiltEnum)((int)r.Value & 3);
					break;
				case "RegAfcFei":
					AfcAutoClearOn = ((int)r.Value & 1) == 1;
					break;
				case "RegAfcMsb":
				case "RegAfcLsb":
					AfcValue = (Decimal)((short)((int)registers["RegAfcMsb"].Value << 8 | (int)registers["RegAfcLsb"].Value)) * FrequencyStep;
					break;
				case "RegFeiMsb":
				case "RegFeiLsb":
					FeiValue = (Decimal)((short)((int)registers["RegFeiMsb"].Value << 8 | (int)registers["RegFeiLsb"].Value)) * FrequencyStep;
					break;
				case "RegPreambleDetect":
					PreambleDetectorOn = ((int)(r.Value >> 7) & 1) == 1;
					PreambleDetectorSize = (byte)(((int)(r.Value >> 5) & 3) + 1);
					PreambleDetectorTol = (byte)(r.Value & 31U);
					break;
				case "RegRxTimeout1":
					TimeoutRxRssi = (Decimal)r.Value * new Decimal(16) * Tbit * new Decimal(1000);
					break;
				case "RegRxTimeout2":
					TimeoutRxPreamble = (Decimal)r.Value * new Decimal(16) * Tbit * new Decimal(1000);
					break;
				case "RegRxTimeout3":
					TimeoutSignalSync = (Decimal)r.Value * new Decimal(16) * Tbit * new Decimal(1000);
					break;
				case "RegRxDelay":
					InterPacketRxDelay = (Decimal)r.Value * new Decimal(4) * Tbit * new Decimal(1000);
					break;
				case "RegOsc":
					ClockOut = (ClockOutEnum)((int)r.Value & 7);
					break;
				case "RegPreambleMsb":
				case "RegPreambleLsb":
					packet.PreambleSize = (int)registers["RegPreambleMsb"].Value << 8 | (int)registers["RegPreambleLsb"].Value;
					break;
				case "RegSyncConfig":
					packet.AutoRestartRxOn = (AutoRestartRxEnum)((int)(r.Value >> 6) & 3);
					packet.PreamblePolarity = (PreamblePolarityEnum)((int)(r.Value >> 5) & 1);
					packet.SyncOn = ((int)(r.Value >> 4) & 1) == 1;
					packet.FifoFillCondition = (FifoFillConditionEnum)((int)(r.Value >> 3) & 1);
					packet.SyncSize = Mode != OperatingModeEnum.Rx || !packet.IoHomeOn ? (byte)(((int)r.Value & 7) + 1) : (byte)(r.Value & 7U);
					UpdateSyncValue();
					break;
				case "RegSyncValue1":
				case "RegSyncValue2":
				case "RegSyncValue3":
				case "RegSyncValue4":
				case "RegSyncValue5":
				case "RegSyncValue6":
				case "RegSyncValue7":
				case "RegSyncValue8":
					UpdateSyncValue();
					break;
				case "RegPacketConfig1":
					packet.PacketFormat = ((int)(r.Value >> 7) & 1) == 1 ? PacketFormatEnum.Variable : PacketFormatEnum.Fixed;
					packet.DcFree = (DcFreeEnum)((int)(r.Value >> 5) & 3);
					packet.CrcOn = ((int)(r.Value >> 4) & 1) == 1;
					packet.CrcAutoClearOff = ((int)(r.Value >> 3) & 1) == 1;
					packet.AddressFiltering = (AddressFilteringEnum)((int)(r.Value >> 1) & 3);
					packet.CrcIbmOn = ((int)r.Value & 1) == 1;
					break;
				case "RegPacketConfig2":
					packet.DataMode = (DataModeEnum)((int)(r.Value >> 6) & 1);
					packet.IoHomeOn = ((int)(r.Value >> 5) & 1) == 1;
					packet.SyncSize = Mode != OperatingModeEnum.Rx || !packet.IoHomeOn ? (byte)(((int)Registers["RegSyncConfig"].Value & 7) + 1) : (byte)(Registers["RegSyncConfig"].Value & 7U);
					UpdateSyncValue();
					packet.IoHomePwrFrameOn = ((int)(r.Value >> 4) & 1) == 1;
					packet.BeaconOn = ((int)(r.Value >> 3) & 1) == 1;
					packet.PayloadLength = (short)((int)packet.PayloadLength & (int)byte.MaxValue | ((int)r.Value & 7) << 8);
					OnPropertyChanged("Crc");
					break;
				case "RegPayloadLength":
					packet.PayloadLength = (short)((int)packet.PayloadLength & 1792 | (int)r.Value);
					break;
				case "RegNodeAdrs":
					packet.NodeAddress = (byte)r.Value;
					break;
				case "RegBroadcastAdrs":
					packet.BroadcastAddress = (byte)r.Value;
					break;
				case "RegFifoThresh":
					packet.TxStartCondition = ((int)(r.Value >> 7) & 1) == 1;
					packet.FifoThreshold = (byte)(r.Value & (uint)sbyte.MaxValue);
					break;
				case "RegSeqConfig1":
					IdleMode = (IdleMode)((int)(r.Value >> 5) & 1);
					FromStart = (FromStart)((int)(r.Value >> 3) & 3);
					LowPowerSelection = (LowPowerSelection)((int)(r.Value >> 2) & 1);
					FromIdle = (FromIdle)((int)(r.Value >> 1) & 1);
					FromTransmit = (FromTransmit)((int)r.Value & 1);
					break;
				case "RegSeqConfig2":
					FromReceive = (FromReceive)((int)(r.Value >> 5) & 7);
					FromRxTimeout = (FromRxTimeout)((int)(r.Value >> 3) & 3);
					FromPacketReceived = (FromPacketReceived)((int)r.Value & 7);
					break;
				case "RegTimerResol":
					Timer1Resolution = (TimerResolution)((int)(r.Value >> 2) & 3);
					Timer2Resolution = (TimerResolution)((int)r.Value & 3);
					break;
				case "RegTimer1Coef":
					Timer1Coef = (byte)r.Value;
					break;
				case "RegTimer2Coef":
					Timer2Coef = (byte)r.Value;
					break;
				case "RegImageCal":
					AutoImageCalOn = ((int)(r.Value >> 7) & 1) == 1;
					imageCalRunning = ((int)(r.Value >> 5) & 1) == 0;
					OnPropertyChanged("ImageCalRunning");
					tempChange = ((int)(r.Value >> 3) & 1) == 1;
					OnPropertyChanged("TempChange");
					TempThreshold = (TempThresholdEnum)((r.Value & 6U) >> 1);
					TempMonitorOff = ((int)r.Value & 1) == 1;
					break;
				case "RegTemp":
					tempValue = (Decimal)((byte)(r.Value & (uint)sbyte.MaxValue));
					if (((int)r.Value & 128) == 128)
						tempValue *= new Decimal(-1);
					tempDelta = tempValue - formerTemp;
					OnPropertyChanged("TempDelta");
					tempValue += tempValueRoom - tempValueCal;
					OnPropertyChanged("TempValue");
					break;
				case "RegLowBat":
					LowBatOn = ((int)(r.Value >> 3) & 1) == 1;
					LowBatTrim = (LowBatTrimEnum)((int)r.Value & 7);
					break;
				case "RegIrqFlags1":
					modeReady = ((int)(r.Value >> 7) & 1) == 1;
					OnPropertyChanged("ModeReady");
					bool flag = ((int)(r.Value >> 6) & 1) == 1;
					if (!rxReady && flag)
						restartRx = true;
					rxReady = flag;
					OnPropertyChanged("RxReady");
					txReady = ((int)(r.Value >> 5) & 1) == 1;
					OnPropertyChanged("TxReady");
					pllLock = ((int)(r.Value >> 4) & 1) == 1;
					OnPropertyChanged("PllLock");
					rssi = ((int)(r.Value >> 3) & 1) == 1;
					OnPropertyChanged("Rssi");
					timeout = ((int)(r.Value >> 2) & 1) == 1;
					OnPropertyChanged("Timeout");
					preambleDetect = ((int)(r.Value >> 1) & 1) == 1;
					OnPropertyChanged("PreambleDetect");
					syncAddressMatch = ((int)r.Value & 1) == 1;
					OnPropertyChanged("SyncAddressMatch");
					break;
				case "RegIrqFlags2":
					fifoFull = ((int)(r.Value >> 7) & 1) == 1;
					OnPropertyChanged("FifoFull");
					fifoEmpty = ((int)(r.Value >> 6) & 1) == 1;
					OnPropertyChanged("FifoEmpty");
					fifoLevel = ((int)(r.Value >> 5) & 1) == 1;
					OnPropertyChanged("FifoLevel");
					fifoOverrun = ((int)(r.Value >> 4) & 1) == 1;
					OnPropertyChanged("FifoOverrun");
					packetSent = ((int)(r.Value >> 3) & 1) == 1;
					OnPropertyChanged("PacketSent");
					payloadReady = ((int)(r.Value >> 2) & 1) == 1;
					OnPropertyChanged("PayloadReady");
					crcOk = ((int)(r.Value >> 1) & 1) == 1;
					OnPropertyChanged("CrcOk");
					lowBat = ((int)r.Value & 1) == 1;
					OnPropertyChanged("LowBat");
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
					MapPreambleDetect = ((int)r.Value & 1) == 1;
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
				case "RegFormerTemp":
					formerTemp = (Decimal)((byte)(r.Value & (uint)sbyte.MaxValue));
					if (((int)r.Value & 128) == 128)
						formerTemp *= new Decimal(-1);
					Decimal num4 = (Decimal)((byte)(registers["RegTemp"].Value & (uint)sbyte.MaxValue));
					if (((int)registers["RegTemp"].Value & 128) == 128)
						num4 *= new Decimal(-1);
					tempDelta = num4 - formerTemp;
					OnPropertyChanged("TempDelta");
					OnPropertyChanged("FormerTemp");
					break;
				case "RegBitrateFrac":
					BitrateFrac = (Decimal)(r.Value & 15U);
					if (ModulationType == ModulationTypeEnum.FSK)
					{
						Bitrate = frequencyXo / ((Decimal)(registers["RegBitrateMsb"].Value << 8 | registers["RegBitrateLsb"].Value) + (Decimal)registers["RegBitrateFrac"].Value / new Decimal(160, 0, 0, false, (byte)1));
						break;
					}
					Bitrate = frequencyXo / (Decimal)(registers["RegBitrateMsb"].Value << 8 | registers["RegBitrateLsb"].Value);
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

		private Decimal ComputeRxBwMin()
		{
			if (ModulationType == ModulationTypeEnum.FSK)
				return FrequencyXo / new Decimal(24) * (Decimal)Math.Pow(2.0, 9.0);
			return FrequencyXo / new Decimal(24) * (Decimal)Math.Pow(2.0, 10.0);
		}

		private Decimal ComputeRxBwMax()
		{
			if (ModulationType == ModulationTypeEnum.FSK)
				return FrequencyXo / new Decimal(16) * (Decimal)Math.Pow(2.0, 2.0);
			return FrequencyXo / new Decimal(16) * (Decimal)Math.Pow(2.0, 3.0);
		}

		public static Decimal ComputeRxBw(Decimal frequencyXo, ModulationTypeEnum mod, int mant, int exp)
		{
			if (mod == ModulationTypeEnum.FSK)
				return frequencyXo / (Decimal)mant * (Decimal)Math.Pow(2.0, (double)(exp + 2));
			return frequencyXo / (Decimal)mant * (Decimal)Math.Pow(2.0, (double)(exp + 3));
		}

		public static void ComputeRxBwMantExp(Decimal frequencyXo, ModulationTypeEnum mod, Decimal value, ref int mant, ref int exp)
		{
			Decimal num1 = new Decimal(0);
			Decimal num2 = new Decimal(10000000);
			for (int index = 0; index < 8; ++index)
			{
				int num3 = 16;
				while (num3 <= 24)
				{
					Decimal num4 = mod != ModulationTypeEnum.FSK ? frequencyXo / (Decimal)num3 * (Decimal)Math.Pow(2.0, (double)(index + 3)) : frequencyXo / (Decimal)num3 * (Decimal)Math.Pow(2.0, (double)(index + 2));
					if (Math.Abs(num4 - value) < num2)
					{
						num2 = Math.Abs(num4 - value);
						mant = num3;
						exp = index;
					}
					num3 += 4;
				}
			}
		}

		public static Decimal[] ComputeRxBwFreqTable(Decimal frequencyXo, ModulationTypeEnum mod)
		{
			Decimal[] numArray = new Decimal[24];
			int num1 = 0;
			for (int index = 0; index < 8; ++index)
			{
				int num2 = 16;
				while (num2 <= 24)
				{
					numArray[num1++] = mod != ModulationTypeEnum.FSK ? frequencyXo / (Decimal)num2 * (Decimal)Math.Pow(2.0, (double)(index + 3)) : frequencyXo / (Decimal)num2 * (Decimal)Math.Pow(2.0, (double)(index + 2));
					num2 += 4;
				}
			}
			return numArray;
		}

		private void BitrateFdevCheck(Decimal bitRate, Decimal fdev)
		{
			Decimal num1 = new Decimal(250000);
			Decimal num2 = new Decimal(1200);
			if (bitRateFdevCheckDisbale)
				return;
			if (modulationType == ModulationTypeEnum.OOK)
				num1 = new Decimal(32768);
			if (bitRate < num2 || bitRate > num1)
				OnBitrateLimitStatusChanged(LimitCheckStatusEnum.OUT_OF_RANGE, "The bitrate is out of range.\nThe valid range is [" + num2.ToString() + ", " + num1.ToString() + "]");
			else
				OnBitrateLimitStatusChanged(LimitCheckStatusEnum.OK, "");
			if (modulationType != ModulationTypeEnum.OOK)
			{
				if (fdev < new Decimal(600) || fdev > new Decimal(200000))
					OnFdevLimitStatusChanged(LimitCheckStatusEnum.OUT_OF_RANGE, "The frequency deviation is out of range.\nThe valid range is [" + 600.ToString() + ", " + 200000.ToString() + "]");
				else if (fdev + bitRate / new Decimal(2) > new Decimal(250000))
				{
					OnFdevLimitStatusChanged(LimitCheckStatusEnum.ERROR, "The single sided band width has been exceeded.\n Fdev + ( Bitrate / 2 ) > " + 250000.ToString() + " Hz");
				}
				else
				{
					Decimal num3 = new Decimal(20, 0, 0, false, (byte)1) * fdev / bitRate;
					if (new Decimal(4969, 0, 0, false, (byte)4) <= num3 && num3 <= new Decimal(100, 0, 0, false, (byte)1))
						OnFdevLimitStatusChanged(LimitCheckStatusEnum.OK, "");
					else
						OnFdevLimitStatusChanged(LimitCheckStatusEnum.ERROR, "The modulation index is out of range.\nThe valid range is [0.5, 10]");
				}
			}
			else
				OnFdevLimitStatusChanged(LimitCheckStatusEnum.OK, "");
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

		private void SyncValueCheck(byte[] value)
		{
			int num = 0;
			if (value == null)
				++num;
			else if ((int)value[0] == 0)
				++num;
			if (num != 0)
				OnSyncValueLimitChanged(LimitCheckStatusEnum.ERROR, "First sync word byte must be different of 0!");
			else
				OnSyncValueLimitChanged(LimitCheckStatusEnum.OK, "");
		}

		private void PreambleCheck()
		{
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

		private void OnBitrateLimitStatusChanged(LimitCheckStatusEnum status, string message)
		{
			if (BitrateLimitStatusChanged == null)
				return;
			BitrateLimitStatusChanged((object)this, new LimitCheckStatusEventArg(status, message));
		}

		private void OnFdevLimitStatusChanged(LimitCheckStatusEnum status, string message)
		{
			if (FdevLimitStatusChanged == null)
				return;
			FdevLimitStatusChanged((object)this, new LimitCheckStatusEventArg(status, message));
		}

		private void OnSyncValueLimitChanged(LimitCheckStatusEnum status, string message)
		{
			if (SyncValueLimitChanged == null)
				return;
			SyncValueLimitChanged((object)this, new LimitCheckStatusEventArg(status, message));
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
				OnError((byte)1, ex.Message);
			}
			return false;
		}

		public bool Close()
		{
			if (isOpen || usbDevice != null && usbDevice.IsOpen)
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
				SX1276.HidCommandsStatus local_1 = SX1276.HidCommandsStatus.SX_ERROR;
				ulong local_2 = ulong.MaxValue;
				byte[] local_3 = new byte[10];
				byte[] local_4 = new byte[2];
				try
				{
					if (IsOpen)
					{
						usbDevice.TxRxCommand(local_0, local_4, ref local_3);
						local_1 = (SX1276.HidCommandsStatus)local_3[0];
						local_2 = (ulong)((long)local_3[1] << 56 | (long)local_3[2] << 48 | (long)local_3[3] << 40 | (long)local_3[4] << 32 | (long)local_3[5] << 24 | (long)local_3[6] << 16 | (long)local_3[7] << 8) | (ulong)local_3[8];
						if (local_1 == SX1276.HidCommandsStatus.SX_OK)
							return true;
					}
					return false;
				}
				finally
				{
					Console.WriteLine("{0} ms: {1} with status {2}", (object)local_2, (object)Enum.GetName(typeof(SX1276.HidCommands), (object)(SX1276.HidCommands)local_0), (object)Enum.GetName(typeof(SX1276.HidCommandsStatus), (object)local_1));
				}
			}
		}

		public bool SKGetVersion()
		{
			lock (syncThread)
			{
				byte local_0 = (byte)1;
				SX1276.HidCommandsStatus local_1 = SX1276.HidCommandsStatus.SX_ERROR;
				ulong local_2 = ulong.MaxValue;
				byte[] local_3 = new byte[17];
				byte[] local_4 = new byte[2];
				try
				{
					usbDevice.TxRxCommand(local_0, local_4, ref local_3);
					local_1 = (SX1276.HidCommandsStatus)local_3[0];
					local_2 = (ulong)((long)local_3[1] << 56 | (long)local_3[2] << 48 | (long)local_3[3] << 40 | (long)local_3[4] << 32 | (long)local_3[5] << 24 | (long)local_3[6] << 16 | (long)local_3[7] << 8) | (ulong)local_3[8];
					if (local_1 != SX1276.HidCommandsStatus.SX_OK || (int)local_3[9] < 5)
						return false;
					Array.Copy((Array)local_3, 10, (Array)local_3, 0, (int)local_3[9]);
					Array.Resize<byte>(ref local_3, (int)local_3[9]);
					string local_6 = new ASCIIEncoding().GetString(local_3);
					fwVersion = local_6.Length <= 5 ? new Version(local_6) : new Version(local_6.Remove(4, 1));
					return true;
				}
				finally
				{
					Console.WriteLine("{0} ms: {1} with status {2}", (object)local_2, (object)Enum.GetName(typeof(SX1276.HidCommands), (object)(SX1276.HidCommands)local_0), (object)Enum.GetName(typeof(SX1276.HidCommandsStatus), (object)local_1));
				}
			}
		}

		public bool SKGetName()
		{
			lock (syncThread)
			{
				byte local_0 = (byte)2;
				SX1276.HidCommandsStatus local_1 = SX1276.HidCommandsStatus.SX_ERROR;
				ulong local_2 = ulong.MaxValue;
				byte[] local_3 = new byte[25];
				byte[] local_4 = new byte[2];
				try
				{
					usbDevice.TxRxCommand(local_0, local_4, ref local_3);
					local_1 = (SX1276.HidCommandsStatus)local_3[0];
					local_2 = (ulong)((long)local_3[1] << 56 | (long)local_3[2] << 48 | (long)local_3[3] << 40 | (long)local_3[4] << 32 | (long)local_3[5] << 24 | (long)local_3[6] << 16 | (long)local_3[7] << 8) | (ulong)local_3[8];
					if (local_1 == SX1276.HidCommandsStatus.SX_OK && (int)local_3[9] >= 9)
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
					Console.WriteLine("{0} ms: {1} with status {2}", (object)local_2, (object)Enum.GetName(typeof(SX1276.HidCommands), (object)(SX1276.HidCommands)local_0), (object)Enum.GetName(typeof(SX1276.HidCommandsStatus), (object)local_1));
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
				SX1276.HidCommandsStatus local_1_1 = (SX1276.HidCommandsStatus)local_2[0];
				int temp_22 = (int)local_2[1];
				int temp_25 = (int)local_2[2];
				int temp_28 = (int)local_2[3];
				int temp_31 = (int)local_2[4];
				int temp_34 = (int)local_2[5];
				int temp_37 = (int)local_2[6];
				int temp_40 = (int)local_2[7];
				int temp_43 = (int)local_2[8];
				if (local_1_1 != SX1276.HidCommandsStatus.SX_OK || (int)local_2[9] != 1)
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
				SX1276.HidCommandsStatus local_1_1 = (SX1276.HidCommandsStatus)local_2[0];
				int temp_28 = (int)local_2[1];
				int temp_31 = (int)local_2[2];
				int temp_34 = (int)local_2[3];
				int temp_37 = (int)local_2[4];
				int temp_40 = (int)local_2[5];
				int temp_43 = (int)local_2[6];
				int temp_46 = (int)local_2[7];
				int temp_49 = (int)local_2[8];
				return local_1_1 == SX1276.HidCommandsStatus.SX_OK;
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
				SX1276.HidCommandsStatus local_1_1 = (SX1276.HidCommandsStatus)local_2[0];
				int temp_22 = (int)local_2[1];
				int temp_25 = (int)local_2[2];
				int temp_28 = (int)local_2[3];
				int temp_31 = (int)local_2[4];
				int temp_34 = (int)local_2[5];
				int temp_37 = (int)local_2[6];
				int temp_40 = (int)local_2[7];
				int temp_43 = (int)local_2[8];
				if (local_1_1 != SX1276.HidCommandsStatus.SX_OK || (int)local_2[9] != 1)
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
				SX1276.HidCommandsStatus local_1_1 = (SX1276.HidCommandsStatus)local_2[0];
				int temp_28 = (int)local_2[1];
				int temp_31 = (int)local_2[2];
				int temp_34 = (int)local_2[3];
				int temp_37 = (int)local_2[4];
				int temp_40 = (int)local_2[5];
				int temp_43 = (int)local_2[6];
				int temp_46 = (int)local_2[7];
				int temp_49 = (int)local_2[8];
				if (local_1_1 != SX1276.HidCommandsStatus.SX_OK || (int)local_2[9] != 1)
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
				SX1276.HidCommandsStatus local_1_1 = (SX1276.HidCommandsStatus)local_2[0];
				int temp_31 = (int)local_2[1];
				int temp_34 = (int)local_2[2];
				int temp_37 = (int)local_2[3];
				int temp_40 = (int)local_2[4];
				int temp_43 = (int)local_2[5];
				int temp_46 = (int)local_2[6];
				int temp_49 = (int)local_2[7];
				int temp_52 = (int)local_2[8];
				return local_1_1 == SX1276.HidCommandsStatus.SX_OK;
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
				SX1276.HidCommandsStatus local_1_1 = (SX1276.HidCommandsStatus)local_2[0];
				int temp_28 = (int)local_2[1];
				int temp_31 = (int)local_2[2];
				int temp_34 = (int)local_2[3];
				int temp_37 = (int)local_2[4];
				int temp_40 = (int)local_2[5];
				int temp_43 = (int)local_2[6];
				int temp_46 = (int)local_2[7];
				int temp_49 = (int)local_2[8];
				if (local_1_1 != SX1276.HidCommandsStatus.SX_OK || (int)local_2[9] != 1)
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
				SX1276.HidCommandsStatus local_1_1 = (SX1276.HidCommandsStatus)local_2[0];
				int temp_31 = (int)local_2[1];
				int temp_34 = (int)local_2[2];
				int temp_37 = (int)local_2[3];
				int temp_40 = (int)local_2[4];
				int temp_43 = (int)local_2[5];
				int temp_46 = (int)local_2[6];
				int temp_49 = (int)local_2[7];
				int temp_52 = (int)local_2[8];
				return local_1_1 == SX1276.HidCommandsStatus.SX_OK;
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
				SX1276.HidCommandsStatus local_1_1 = (SX1276.HidCommandsStatus)local_2[0];
				int temp_22 = (int)local_2[1];
				int temp_25 = (int)local_2[2];
				int temp_28 = (int)local_2[3];
				int temp_31 = (int)local_2[4];
				int temp_34 = (int)local_2[5];
				int temp_37 = (int)local_2[6];
				int temp_40 = (int)local_2[7];
				int temp_43 = (int)local_2[8];
				if (local_1_1 != SX1276.HidCommandsStatus.SX_OK || (int)local_2[9] != 1)
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
					SX1276.HidCommandsStatus local_1_1 = (SX1276.HidCommandsStatus)local_2[0];
					int temp_44 = (int)local_2[1];
					int temp_47 = (int)local_2[2];
					int temp_50 = (int)local_2[3];
					int temp_53 = (int)local_2[4];
					int temp_56 = (int)local_2[5];
					int temp_59 = (int)local_2[6];
					int temp_62 = (int)local_2[7];
					int temp_65 = (int)local_2[8];
					if (local_1_1 == SX1276.HidCommandsStatus.SX_OK)
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
					SX1276.HidCommandsStatus local_1_1 = (SX1276.HidCommandsStatus)local_2[0];
					int temp_47 = (int)local_2[1];
					int temp_50 = (int)local_2[2];
					int temp_53 = (int)local_2[3];
					int temp_56 = (int)local_2[4];
					int temp_59 = (int)local_2[5];
					int temp_62 = (int)local_2[6];
					int temp_65 = (int)local_2[7];
					int temp_68 = (int)local_2[8];
					if (local_1_1 != SX1276.HidCommandsStatus.SX_OK)
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
				byte local_0 = 0x80;
				byte[] local_2 = new byte[10 + data.Length];
				byte[] local_3 = new byte[4]
				{
					0,
					(byte) (data.Length + 2),
					(byte) data.Length,
					address
				};
				usbDevice.TxRxCommand(local_0, local_3, ref local_2);
				SX1276.HidCommandsStatus local_1_1 = (SX1276.HidCommandsStatus)local_2[0];
				int temp_46 = (int)local_2[1];
				int temp_49 = (int)local_2[2];
				int temp_52 = (int)local_2[3];
				int temp_55 = (int)local_2[4];
				int temp_58 = (int)local_2[5];
				int temp_61 = (int)local_2[6];
				int temp_64 = (int)local_2[7];
				int temp_67 = (int)local_2[8];
				if (local_1_1 != SX1276.HidCommandsStatus.SX_OK || (int)local_2[9] != data.Length)
					return false;
				Array.Copy((Array)local_2, 10, (Array)data, 0, data.Length);
				return true;
			}
		}

		public bool SKDeviceWrite(byte address, byte[] data)
		{
			lock (syncThread)
			{
				byte local_0 = (byte)129;
				byte[] local_2 = new byte[10];
				byte[] local_3 = new byte[data.Length + 2 + 2];
				local_3[0] = (byte)0;
				local_3[1] = (byte)(data.Length + 2);
				local_3[2] = (byte)data.Length;
				local_3[3] = address;
				Array.Copy((Array)data, 0, (Array)local_3, 4, data.Length);
				usbDevice.TxRxCommand(local_0, local_3, ref local_2);
				SX1276.HidCommandsStatus local_1_1 = (SX1276.HidCommandsStatus)local_2[0];
				int temp_54 = (int)local_2[1];
				int temp_57 = (int)local_2[2];
				int temp_60 = (int)local_2[3];
				int temp_63 = (int)local_2[4];
				int temp_66 = (int)local_2[5];
				int temp_69 = (int)local_2[6];
				int temp_72 = (int)local_2[7];
				int temp_75 = (int)local_2[8];
				return local_1_1 == SX1276.HidCommandsStatus.SX_OK;
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
			OnError((byte)0, "-");
			StreamReader streamReader = new StreamReader((Stream)stream, Encoding.ASCII);
			int num1 = 1;
			int num2 = 0;
			string data = "";
			try
			{
				string str;
				while ((str = streamReader.ReadLine()) != null)
				{
					if ((int)str[0] == 35)
					{
						++num1;
					}
					else
					{
						if ((int)str[0] != 82 && (int)str[0] != 80 && (int)str[0] != 88)
							throw new Exception("At line " + num1.ToString() + ": A configuration line must start either by\n\"#\" for comments\nor a\n\"R\" for the register name.\nor a\n\"P\" for packet settings.\nor a\n\"X\" for crystal frequency.");
						string[] strArray = str.Split('\t');
						if (strArray.Length != 4)
						{
							if (strArray.Length != 2)
								throw new Exception("At line " + num1.ToString() + ": The number of columns is " + strArray.Length.ToString() + " and it should be 4 or 2.");
							if (strArray[0] == "PKT")
							{
								data = strArray[1];
							}
							else
							{
								if (!(strArray[0] == "XTAL"))
									throw new Exception("At line " + num1.ToString() + ": Invalid Packet or XTAL freuqncy");
								FrequencyXo = Convert.ToDecimal(strArray[1]);
							}
						}
						else
						{
							bool flag = true;
							for (int index = 0; index < registers.Count; ++index)
							{
								if (registers[index].Name == strArray[1])
								{
									flag = false;
									break;
								}
							}
							if (flag)
								throw new Exception("At line " + num1.ToString() + ": Invalid register name.");
							if (strArray[1] != "RegVersion")
							{
								registers[strArray[1]].Value = (uint)Convert.ToByte(strArray[3], 16);
								++num2;
							}
						}
						++num1;
					}
				}
				packet.SetSaveData(data);
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
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
			OnError((byte)0, "-");
			StreamWriter streamWriter = new StreamWriter((Stream)stream, Encoding.ASCII);
			try
			{
				streamWriter.WriteLine("#Type\tRegister Name\tAddress[Hex]\tValue[Hex]");
				for (int index = 0; index < registers.Count; ++index)
					streamWriter.WriteLine("REG\t{0}\t0x{1}\t0x{2}", (object)registers[index].Name, (object)registers[index].Address.ToString("X02"), (object)registers[index].Value.ToString("X02"));
				streamWriter.WriteLine("PKT\t{0}", (object)packet.GetSaveData());
				streamWriter.WriteLine("XTAL\t{0}", (object)FrequencyXo);
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
			byte data = (byte)0;
			if (!Read((byte)66, ref data))
				throw new Exception("Unable to read register RegVersion");
			if (!Read((byte)66, ref data))
				throw new Exception("Unable to read register RegVersion");
			return new Version(((int)data & 240) >> 4, (int)data & 15);
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
					tempCalDone = false;
					PacketHandlerStop();
					if (!SKReset())
						throw new Exception("Unable to reset the SK");
					ReadRegisters();
					Decimal local_1 = FrequencyRf;
					SetFrequencyRf(new Decimal(915000000));
					ImageCalStart();
					SetFrequencyRf(local_1);
					ImageCalStart();
					SetLoraOn(false);
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
					OnError((byte)1, exception_0.Message);
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
					OnError((byte)1, exception_0.Message);
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
					OnError((byte)1, exception_0.Message);
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
					OnError((byte)1, exception_0.Message);
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
					OnError((byte)1, exception_0.Message);
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
					OnError((byte)1, exception_0.Message);
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
			ReadRegister(registers["RegIrqFlags1"]);
			ReadRegister(registers["RegIrqFlags2"]);
		}

		public void SetModeLeds(OperatingModeEnum mode)
		{
			if (test)
				return;
			switch (mode)
			{
				case OperatingModeEnum.Tx:
					SKSetPinState((byte)7, (byte)1);
					SKSetPinState((byte)8, (byte)0);
					break;
				case OperatingModeEnum.Rx:
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
			TempCalDone = false;
			if (IsOpen)
				return;
			ReadRegisters();
		}

		private void SetLoraOn(bool enable)
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

		public void SetModulationType(ModulationTypeEnum value)
		{
			try
			{
				lock (syncThread)
				{
					registers["RegOpMode"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegOpMode"].Value & 159U) | (uint)(byte)((uint)(byte)value << 5));
					ReadRegister(registers["RegBitrateMsb"]);
					ReadRegister(registers["RegBitrateLsb"]);
					ReadRegister(registers["RegBitrateFrac"]);
				}
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
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
				OnError((byte)1, ex.Message);
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
				if (value == OperatingModeEnum.Tx)
				{
					SKSetPinState((byte)11, (byte)0);
					SKSetPinState((byte)12, (byte)1);
				}
				else
				{
					SKSetPinState((byte)11, (byte)1);
					SKSetPinState((byte)12, (byte)0);
				}
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
						if (packet.IoHomeOn)
							SetSyncSize(packet.SyncSize);
						if (Mode != OperatingModeEnum.Rx)
							return;
						ReadRegister(registers["RegLna"]);
						ReadRegister(registers["RegFeiMsb"]);
						ReadRegister(registers["RegFeiLsb"]);
						ReadRegister(registers["RegAfcMsb"]);
						ReadRegister(registers["RegAfcLsb"]);
					}
				}
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetBitrate(Decimal value)
		{
			try
			{
				lock (syncThread)
				{
					byte local_0;
					byte local_1;
					if (ModulationType == ModulationTypeEnum.FSK)
					{
						local_0 = (byte)((long)Math.Round(frequencyXo / value - BitrateFrac / new Decimal(16), MidpointRounding.AwayFromZero) >> 8);
						local_1 = (byte)(long)Math.Round(frequencyXo / value - BitrateFrac / new Decimal(16), MidpointRounding.AwayFromZero);
					}
					else
					{
						local_0 = (byte)((long)Math.Round(frequencyXo / value, MidpointRounding.AwayFromZero) >> 8);
						local_1 = (byte)(long)Math.Round(frequencyXo / value, MidpointRounding.AwayFromZero);
					}
					bitRateFdevCheckDisbale = true;
					registers["RegBitrateMsb"].Value = (uint)local_0;
					bitRateFdevCheckDisbale = false;
					registers["RegBitrateLsb"].Value = (uint)local_1;
				}
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
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
				OnError((byte)1, ex.Message);
			}
		}

		public void SetFdev(Decimal value)
		{
			try
			{
				lock (syncThread)
				{
					byte local_0 = (byte)(registers["RegFdevMsb"].Value & 63U);
					byte local_1 = (byte)registers["RegFdevLsb"].Value;
					byte local_0_1 = (byte)((long)(value / frequencyStep) >> 8);
					byte local_1_1 = (byte)(long)(value / frequencyStep);
					bitRateFdevCheckDisbale = true;
					registers["RegFdevMsb"].Value = (uint)(byte)((uint)local_0_1 & 63U);
					bitRateFdevCheckDisbale = false;
					registers["RegFdevLsb"].Value = (uint)local_1_1;
				}
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
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
				OnError((byte)1, ex.Message);
			}
		}

		public void SetPaMode(PaSelectEnum value)
		{
			try
			{
				lock (syncThread)
				{
					byte local_0_1 = (byte)((uint)(byte)registers["RegPaConfig"].Value & (uint)sbyte.MaxValue);
					switch (value)
					{
						case PaSelectEnum.PA_BOOST:
							local_0_1 |= 0x80;
							break;
					}
					registers["RegPaConfig"].Value = (uint)local_0_1;
				}
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetMaxOutputPower(Decimal value)
		{
			try
			{
				lock (syncThread)
					registers["RegPaConfig"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegPaConfig"].Value & 143U) | (uint)(byte)(((int)((value - new Decimal(108, 0, 0, false, (byte)1)) / new Decimal(6, 0, 0, false, (byte)1)) & 7) << 4));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
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
				OnError((byte)1, ex.Message);
			}
		}

		public void SetModulationShaping(byte value)
		{
			try
			{
				lock (syncThread)
					registers["RegPaRamp"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegPaRamp"].Value & 159U) | (uint)(byte)((uint)value << 5));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetForceTxBandLowFrequencyOn(bool value)
		{
			try
			{
				lock (syncThread)
					registers["RegPaRamp"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegPaRamp"].Value & 239U) | (value ? 16 : 0));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetPaRamp(PaRampEnum value)
		{
			try
			{
				lock (syncThread)
					registers["RegPaRamp"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegPaRamp"].Value & 240U) | (uint)(byte)((uint)(byte)value & 15U));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetOcpOn(bool value)
		{
			try
			{
				lock (syncThread)
					registers["RegOcp"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegOcp"].Value & 223U) | (value ? 32 : 0));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetOcpTrim(Decimal value)
		{
			try
			{
				lock (syncThread)
				{
					byte local_0_1 = (byte)((uint)(byte)registers["RegOcp"].Value & 224U);
					registers["RegOcp"].Value = !(value <= new Decimal(1200, 0, 0, false, (byte)1)) ? (!(value > new Decimal(120)) || !(value <= new Decimal(2400, 0, 0, false, (byte)1)) ? (uint)(byte)((uint)local_0_1 | 27U) : (uint)(byte)((uint)local_0_1 | (uint)(byte)((uint)(byte)((value + new Decimal(30)) / new Decimal(10)) & 31U))) : (uint)(byte)((uint)local_0_1 | (uint)(byte)((uint)(byte)((value - new Decimal(45)) / new Decimal(5)) & 15U));
				}
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetAgcReferenceLevel(int value)
		{
			try
			{
				lock (syncThread)
					registers["RegAgcRef"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegAgcRef"].Value & 192U) | (uint)(byte)(value & 63));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetAgcStep(byte id, byte value)
		{
			try
			{
				lock (syncThread)
				{
					Register local_1;
					switch (id)
					{
						case (byte)1:
							local_1 = registers["RegAgcThresh1"];
							break;
						case (byte)2:
						case (byte)3:
							local_1 = registers["RegAgcThresh2"];
							break;
						case (byte)4:
						case (byte)5:
							local_1 = registers["RegAgcThresh3"];
							break;
						default:
							throw new Exception("Invalid AGC step ID!");
					}
					byte local_0 = (byte)local_1.Value;
					byte local_0_2;
					switch (id)
					{
						case (byte)1:
							local_0_2 = (byte)((uint)(byte)((uint)local_0 & 224U) | (uint)value);
							break;
						case (byte)2:
							local_0_2 = (byte)((uint)(byte)((uint)local_0 & 15U) | (uint)(byte)((uint)value << 4));
							break;
						case (byte)3:
							local_0_2 = (byte)((uint)(byte)((uint)local_0 & 240U) | (uint)(byte)((uint)value & 15U));
							break;
						case (byte)4:
							local_0_2 = (byte)((uint)(byte)((uint)local_0 & 15U) | (uint)(byte)((uint)value << 4));
							break;
						case (byte)5:
							local_0_2 = (byte)((uint)(byte)((uint)local_0 & 240U) | (uint)(byte)((uint)value & 15U));
							break;
						default:
							throw new Exception("Invalid AGC step ID!");
					}
					local_1.Value = (uint)local_0_2;
				}
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetLnaGain(LnaGainEnum value)
		{
			try
			{
				lock (syncThread)
				{
					registers["RegLna"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegLna"].Value & 31U) | (uint)(byte)((uint)(byte)value << 5));
					ReadRegister(registers["RegLna"]);
				}
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetForceRxBandLowFrequencyOn(bool value)
		{
			try
			{
				lock (syncThread)
					registers["RegLna"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegLna"].Value & 251U) | (value ? 4 : 0));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetLnaBoost(bool value)
		{
			try
			{
				lock (syncThread)
					registers["RegLna"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegLna"].Value & 252U) | (value ? 3 : 0));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetRestartRxOnCollisionOn(bool value)
		{
			try
			{
				lock (syncThread)
					registers["RegRxConfig"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegRxConfig"].Value & (uint)sbyte.MaxValue) | (value ? 128 : 0));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetRestartRxWithoutPllLock()
		{
			try
			{
				lock (syncThread)
				{
					byte local_0 = (byte)0;
					ReadRegister(registers["RegRxConfig"], ref local_0);
					WriteRegister(registers["RegRxConfig"], (byte)((uint)local_0 | 64U));
					restartRx = true;
				}
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetRestartRxWithPllLock()
		{
			try
			{
				lock (syncThread)
				{
					byte local_0 = (byte)0;
					ReadRegister(registers["RegRxConfig"], ref local_0);
					WriteRegister(registers["RegRxConfig"], (byte)((uint)local_0 | 32U));
					restartRx = true;
				}
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetAfcAutoOn(bool value)
		{
			try
			{
				lock (syncThread)
					registers["RegRxConfig"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegRxConfig"].Value & 239U) | (value ? 16 : 0));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetAgcAutoOn(bool value)
		{
			try
			{
				lock (syncThread)
					registers["RegRxConfig"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegRxConfig"].Value & 247U) | (value ? 8 : 0));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetRxTrigger(RxTriggerEnum value)
		{
			try
			{
				lock (syncThread)
				{
					byte local_0_1 = (byte)((uint)(byte)registers["RegRxConfig"].Value & 248U);
					switch (value)
					{
						case RxTriggerEnum.RX_TRIGER_001:
							local_0_1 |= (byte)1;
							break;
						case RxTriggerEnum.RX_TRIGER_110:
							local_0_1 |= (byte)6;
							break;
						case RxTriggerEnum.RX_TRIGER_111:
							local_0_1 |= (byte)7;
							break;
					}
					registers["RegRxConfig"].Value = (uint)local_0_1;
				}
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetRssiOffset(Decimal value)
		{
			try
			{
				lock (syncThread)
				{
					lock (syncThread)
					{
						byte num = (byte)registers["RegRssiConfig"].Value;
						num = (byte)(num & 7);
						sbyte num2 = (sbyte)value;
						if (num2 < 0)
						{
							num2 = (sbyte)(~num2 & 0x1f);
							num2 = (sbyte)(num2 + 1);
							num2 = (sbyte)(-num2);
						}
						num = (byte)(num | ((byte)((num2 & 0x1f) << 3)));
						this.registers["RegRssiConfig"].Value = num;
						this.ReadRegister(this.registers["RegRssiValue"]);
					}
				}
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetRssiSmoothing(Decimal value)
		{
			try
			{
				lock (syncThread)
				{
					lock (syncThread)
						registers["RegRssiConfig"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegRssiConfig"].Value & 248U) | (uint)(byte)((uint)(int)(Math.Log((double)value, 2.0) - 1.0) & 7U));
				}
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetRssiCollisionThreshold(Decimal value)
		{
			try
			{
				lock (syncThread)
					registers["RegRssiCollision"].Value = (uint)value;
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetRssiThresh(Decimal value)
		{
			try
			{
				lock (syncThread)
					registers["RegRssiThresh"].Value = (uint)(-value * new Decimal(2));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetDccFastInitOn(bool value)
		{
			try
			{
				lock (syncThread)
					registers["RegRxBw"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegRxBw"].Value & 191U) | (value ? 64 : 0));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetDccForceOn(bool value)
		{
			try
			{
				lock (syncThread)
					registers["RegRxBw"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegRxBw"].Value & 223U) | (value ? 32 : 0));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetRxBw(Decimal value)
		{
			try
			{
				lock (syncThread)
				{
					byte local_0_1 = (byte)((uint)(byte)registers["RegRxBw"].Value & 224U);
					int local_1 = 0;
					int local_2 = 0;
					SX1276.ComputeRxBwMantExp(frequencyXo, ModulationType, value, ref local_2, ref local_1);
					byte local_0_2;
					switch (local_2)
					{
						case 16:
							local_0_2 = (byte)((uint)local_0_1 | (uint)(byte)(local_1 & 7));
							break;
						case 20:
							local_0_2 = (byte)((uint)local_0_1 | (uint)(byte)(8 | local_1 & 7));
							break;
						case 24:
							local_0_2 = (byte)((uint)local_0_1 | (uint)(byte)(16 | local_1 & 7));
							break;
						default:
							throw new Exception("Invalid RxBwMant parameter");
					}
					registers["RegRxBw"].Value = (uint)local_0_2;
				}
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetAfcRxBw(Decimal value)
		{
			try
			{
				lock (syncThread)
				{
					byte local_0_1 = (byte)((uint)(byte)registers["RegAfcBw"].Value & 224U);
					int local_1 = 0;
					int local_2 = 0;
					SX1276.ComputeRxBwMantExp(frequencyXo, ModulationType, value, ref local_2, ref local_1);
					byte local_0_2;
					switch (local_2)
					{
						case 16:
							local_0_2 = (byte)((uint)local_0_1 | (uint)(byte)(local_1 & 7));
							break;
						case 20:
							local_0_2 = (byte)((uint)local_0_1 | (uint)(byte)(8 | local_1 & 7));
							break;
						case 24:
							local_0_2 = (byte)((uint)local_0_1 | (uint)(byte)(16 | local_1 & 7));
							break;
						default:
							throw new Exception("Invalid RxBwMant parameter");
					}
					registers["RegAfcBw"].Value = (uint)local_0_2;
				}
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetBitSyncOn(bool value)
		{
			try
			{
				lock (syncThread)
					registers["RegOokPeak"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegOokPeak"].Value & 223U) | (value ? 32 : 0));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetOokThreshType(OokThreshTypeEnum value)
		{
			try
			{
				lock (syncThread)
					registers["RegOokPeak"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegOokPeak"].Value & 231U) | (uint)(byte)(((int)(byte)value & 3) << 3));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetOokPeakThreshStep(Decimal value)
		{
			try
			{
				lock (syncThread)
					registers["RegOokPeak"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegOokPeak"].Value & 248U) | (uint)(byte)((uint)(byte)Array.IndexOf<Decimal>(OoPeakThreshStepTable, value) & 7U));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetOokFixedThresh(byte value)
		{
			try
			{
				lock (syncThread)
					registers["RegOokFix"].Value = (uint)value;
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetOokPeakThreshDec(OokPeakThreshDecEnum value)
		{
			try
			{
				lock (syncThread)
					registers["RegOokAvg"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegOokAvg"].Value & 31U) | (uint)(byte)(((int)(byte)value & 7) << 5));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetOokPeakRecoveryOn(bool value)
		{
			try
			{
				lock (syncThread)
					registers["RegOokAvg"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegOokAvg"].Value & 239U) | (value ? 16 : 0));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetOokAverageBias(Decimal value)
		{
			try
			{
				lock (syncThread)
					registers["RegOokAvg"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegOokAvg"].Value & 243U) | (uint)(byte)((uint)(byte)(value / new Decimal(2)) << 2));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetOokAverageThreshFilt(OokAverageThreshFiltEnum value)
		{
			try
			{
				lock (syncThread)
					registers["RegOokAvg"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegOokAvg"].Value & 252U) | (uint)(byte)((uint)(byte)value & 3U));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetBarkerSyncThresh(byte value)
		{
			try
			{
				lock (syncThread)
					registers["RegRes17"].Value = (uint)(byte)((uint)value & (uint)sbyte.MaxValue);
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetBarkerSyncLossThresh(byte value)
		{
			try
			{
				lock (syncThread)
					registers["RegRes18"].Value = (uint)(byte)((uint)value & (uint)sbyte.MaxValue);
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetBarkerTrackingThresh(byte value)
		{
			try
			{
				lock (syncThread)
					registers["RegRes19"].Value = (uint)(byte)((uint)value & (uint)sbyte.MaxValue);
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetAgcStart()
		{
			try
			{
				lock (syncThread)
				{
					registers["RegAfcFei"].Value = (uint)(byte)((uint)(byte)registers["RegAfcFei"].Value | 16U);
					ReadRegister(registers["RegLna"]);
				}
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetFeiRange(FeiRangeEnum value)
		{
			try
			{
				lock (syncThread)
					registers["RegAfcFei"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegAfcFei"].Value & 247U) | (uint)(byte)((uint)(byte)value << 3));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetAfcClear()
		{
			try
			{
				lock (syncThread)
				{
					registers["RegAfcFei"].Value = (uint)(byte)((uint)(byte)registers["RegAfcFei"].Value | 2U);
					ReadRegister(registers["RegAfcMsb"]);
					ReadRegister(registers["RegAfcLsb"]);
				}
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetFeiRead()
		{
			try
			{
				lock (syncThread)
				{
					ReadRegister(registers["RegFeiMsb"]);
					ReadRegister(registers["RegFeiLsb"]);
				}
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetAfcAutoClearOn(bool value)
		{
			try
			{
				lock (syncThread)
					registers["RegAfcFei"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegAfcFei"].Value & 254U) | (value ? 1 : 0));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetPreambleDetectorOn(bool value)
		{
			try
			{
				lock (syncThread)
					registers["RegPreambleDetect"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegPreambleDetect"].Value & (uint)sbyte.MaxValue) | (value ? 128 : 0));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetPreambleDetectorSize(byte value)
		{
			try
			{
				lock (syncThread)
					registers["RegPreambleDetect"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegPreambleDetect"].Value & 159U) | (uint)(byte)((int)value - 1 << 5));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetPreambleDetectorTol(byte value)
		{
			try
			{
				lock (syncThread)
					registers["RegPreambleDetect"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegPreambleDetect"].Value & 224U) | (uint)(byte)((uint)value & 31U));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetTimeoutRssi(Decimal value)
		{
			try
			{
				lock (syncThread)
					registers["RegRxTimeout1"].Value = (uint)Math.Round(value / new Decimal(1000) / new Decimal(16) * Tbit, MidpointRounding.AwayFromZero);
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetTimeoutPreamble(Decimal value)
		{
			try
			{
				lock (syncThread)
					registers["RegRxTimeout2"].Value = (uint)Math.Round(value / new Decimal(1000) / new Decimal(16) * Tbit, MidpointRounding.AwayFromZero);
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetTimeoutSyncWord(Decimal value)
		{
			try
			{
				lock (syncThread)
					registers["RegRxTimeout3"].Value = (uint)Math.Round(value / new Decimal(1000) / new Decimal(16) * Tbit, MidpointRounding.AwayFromZero);
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetAutoRxRestartDelay(Decimal value)
		{
			try
			{
				lock (syncThread)
					registers["RegRxDelay"].Value = (uint)Math.Round(value / new Decimal(1000) / new Decimal(4) * Tbit, MidpointRounding.AwayFromZero);
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void RcCalTrig()
		{
			lock (syncThread)
			{
				byte local_0 = (byte)0;
				if (Mode == OperatingModeEnum.Stdby)
				{
					ReadRegister(registers["RegOsc"], ref local_0);
					WriteRegister(registers["RegOsc"], (byte)((uint)local_0 | 8U));
					DateTime local_1 = DateTime.Now;
					byte local_0_1;
					bool local_3_1;
					do
					{
						local_0_1 = (byte)0;
						ReadRegister(registers["RegOsc"], ref local_0_1);
						local_3_1 = (DateTime.Now - local_1).TotalMilliseconds >= 1000.0;
					}
					while ((int)(byte)((uint)local_0_1 & 8U) == 8 && !local_3_1);
					if (local_3_1)
						throw new Exception("RC oscillator calibration timeout.");
				}
				else
				{
					int temp_12 = (int)MessageBox.Show("The chip must be in Standby mode in order to calibrate the RC oscillator!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					throw new Exception("The chip must be in Standby mode in order to calibrate the RC oscillator!");
				}
			}
		}

		public void SetClockOut(ClockOutEnum value)
		{
			try
			{
				lock (syncThread)
					registers["RegOsc"].Value = (uint)((ClockOutEnum)(registers["RegOsc"].Value & 248U) | value & ClockOutEnum.CLOCK_OUT_111);
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
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
					SetDataMode(DataModeEnum.Packet);
					if (Mode == OperatingModeEnum.Tx)
					{
						if ((int)packet.MessageLength == 0)
						{
							int temp_49 = (int)MessageBox.Show("Message must be at least one byte long", "SX1276SKA-PacketHandler", MessageBoxButtons.OK, MessageBoxIcon.Hand);
							throw new Exception("Message must be at least one byte long");
						}
					}
					else
					{
						int temp_14 = (int)Mode;
					}
					frameTransmitted = false;
					frameReceived = false;
					if (Mode == OperatingModeEnum.Tx)
					{
						SetOperatingMode(OperatingModeEnum.Tx, true);
						firstTransmit = true;
					}
					else
					{
						SetOperatingMode(OperatingModeEnum.Rx, true);
						OnPacketHandlerReceived();
					}
					isPacketHandlerStarted = true;
					OnPacketHandlerStarted();
				}
				catch (Exception exception_0)
				{
					OnError((byte)1, exception_0.Message);
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
					SetOperatingMode(Mode);
					frameTransmitted = false;
					frameReceived = false;
					firstTransmit = false;
				}
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
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
						frameTransmitted = TransmitRfData(packet.ToArray());
						++packetNumber;
					}
				}
				catch (Exception exception_0)
				{
					PacketHandlerStop();
					OnError((byte)1, exception_0.Message);
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
					SetModeLeds(OperatingModeEnum.Rx);
					byte local_0 = (byte)0;
					ReadRegister(registers["RegRssiValue"], ref local_0);
					packet.Rssi = -(Decimal)local_0 / new Decimal(20, 0, 0, false, (byte)1);
					byte[] local_1;
					frameReceived = ReceiveRfData(out local_1);
					if (packet.PacketFormat == PacketFormatEnum.Fixed)
					{
						if (packet.AddressFiltering != AddressFilteringEnum.OFF)
						{
							packet.NodeAddressRx = local_1[0];
							Array.Copy((Array)local_1, 1, (Array)local_1, 0, local_1.Length - 1);
							Array.Resize<byte>(ref local_1, (int)packet.PayloadLength - 1);
						}
						else
							Array.Resize<byte>(ref local_1, (int)packet.PayloadLength);
					}
					else if (packet.PacketFormat == PacketFormatEnum.Variable)
					{
						int local_2 = (int)local_1[0];
						Array.Copy((Array)local_1, 1, (Array)local_1, 0, local_1.Length - 1);
						Array.Resize<byte>(ref local_1, local_2);
						if (packet.AddressFiltering != AddressFilteringEnum.OFF)
						{
							int local_2_1 = local_2 - 1;
							packet.NodeAddressRx = local_1[0];
							Array.Copy((Array)local_1, 1, (Array)local_1, 0, local_1.Length - 1);
							Array.Resize<byte>(ref local_1, local_2_1);
						}
					}
					packet.Message = local_1;
					++packetNumber;
					OnPacketHandlerReceived();
					if (!isPacketHandlerStarted)
						PacketHandlerStop();
					SetModeLeds(OperatingModeEnum.Sleep);
				}
				catch (Exception exception_0)
				{
					PacketHandlerStop();
					OnError((byte)1, exception_0.Message);
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
			PacketHandlerTransmitted((object)this, new PacketStatusEventArg(packetNumber, maxPacketNumber));
		}

		private void OnPacketHandlerReceived()
		{
			if (PacketHandlerReceived == null)
				return;
			PacketHandlerReceived((object)this, new PacketStatusEventArg(packetNumber, maxPacketNumber));
		}

		public void SetPreambleSize(int value)
		{
			try
			{
				lock (syncThread)
				{
					registers["RegPreambleMsb"].Value = (uint)(byte)(value >> 8);
					registers["RegPreambleLsb"].Value = (uint)(byte)value;
				}
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetAutoRestartRxOn(AutoRestartRxEnum value)
		{
			try
			{
				lock (syncThread)
					registers["RegSyncConfig"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegSyncConfig"].Value & 63U) | (uint)(byte)((uint)(byte)value << 6));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetPreamblePolarity(PreamblePolarityEnum value)
		{
			try
			{
				lock (syncThread)
					registers["RegSyncConfig"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegSyncConfig"].Value & 223U) | (uint)(byte)((uint)(byte)value << 5));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetSyncOn(bool value)
		{
			try
			{
				lock (syncThread)
					registers["RegSyncConfig"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegSyncConfig"].Value & 239U) | (value ? 16 : 0));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetFifoFillCondition(FifoFillConditionEnum value)
		{
			try
			{
				lock (syncThread)
					registers["RegSyncConfig"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegSyncConfig"].Value & 247U) | (uint)(byte)((uint)(byte)value << 3));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetSyncSize(byte value)
		{
			try
			{
				lock (syncThread)
				{
					byte local_0_1 = (byte)((uint)(byte)registers["RegSyncConfig"].Value & 248U);
					registers["RegSyncConfig"].Value = Mode != OperatingModeEnum.Rx || !packet.IoHomeOn ? (uint)(byte)((uint)local_0_1 | (uint)(byte)((int)value - 1 & 7)) : (uint)(byte)((uint)local_0_1 | (uint)(byte)((uint)value & 7U));
				}
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetSyncValue(byte[] value)
		{
			try
			{
				lock (syncThread)
				{
					byte local_0 = (byte)registers["RegSyncValue1"].Address;
					for (int local_1 = 0; local_1 < value.Length; ++local_1)
						registers[(int)local_0 + local_1].Value = (uint)value[local_1];
				}
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetPacketFormat(PacketFormatEnum value)
		{
			try
			{
				lock (syncThread)
					registers["RegPacketConfig1"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegPacketConfig1"].Value & (uint)sbyte.MaxValue) | (value == PacketFormatEnum.Variable ? 128 : 0));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetDcFree(DcFreeEnum value)
		{
			try
			{
				lock (syncThread)
					registers["RegPacketConfig1"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegPacketConfig1"].Value & 159U) | (uint)(byte)(((int)(byte)value & 3) << 5));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetCrcOn(bool value)
		{
			try
			{
				lock (syncThread)
					registers["RegPacketConfig1"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegPacketConfig1"].Value & 239U) | (value ? 16 : 0));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetCrcAutoClearOff(bool value)
		{
			try
			{
				lock (syncThread)
					registers["RegPacketConfig1"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegPacketConfig1"].Value & 247U) | (value ? 8 : 0));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetAddressFiltering(AddressFilteringEnum value)
		{
			try
			{
				lock (syncThread)
					registers["RegPacketConfig1"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegPacketConfig1"].Value & 249U) | (uint)(byte)(((int)(byte)value & 3) << 1));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetCrcIbmOn(bool value)
		{
			try
			{
				lock (syncThread)
					registers["RegPacketConfig1"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegPacketConfig1"].Value & 254U) | (value ? 1 : 0));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetDataMode(DataModeEnum value)
		{
			try
			{
				lock (syncThread)
					registers["RegPacketConfig2"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegPacketConfig2"].Value & 191U) | (uint)(byte)((uint)(byte)value << 6));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetIoHomeOn(bool value)
		{
			try
			{
				lock (syncThread)
					registers["RegPacketConfig2"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegPacketConfig2"].Value & 223U) | (value ? 32 : 0));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetIoHomePwrFrameOn(bool value)
		{
			try
			{
				lock (syncThread)
					registers["RegPacketConfig2"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegPacketConfig2"].Value & 239U) | (value ? 16 : 0));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetBeaconOn(bool value)
		{
			try
			{
				lock (syncThread)
					registers["RegPacketConfig2"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegPacketConfig2"].Value & 247U) | (value ? 8 : 0));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetFillFifo()
		{
			try
			{
				lock (syncThread)
					WriteFifo(packet.ToArray());
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetPayloadLength(short value)
		{
			try
			{
				lock (syncThread)
				{
					registers["RegPacketConfig2"].Value = registers["RegPacketConfig2"].Value | (uint)(byte)((int)value >> 8 & 7);
					registers["RegPayloadLength"].Value = (uint)(byte)value;
				}
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetNodeAddress(byte value)
		{
			try
			{
				lock (syncThread)
					registers["RegNodeAdrs"].Value = (uint)value;
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetBroadcastAddress(byte value)
		{
			try
			{
				lock (syncThread)
					registers["RegBroadcastAdrs"].Value = (uint)value;
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetTxStartCondition(bool value)
		{
			try
			{
				lock (syncThread)
					registers["RegFifoThresh"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegFifoThresh"].Value & (uint)sbyte.MaxValue) | (value ? 128 : 0));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetFifoThreshold(byte value)
		{
			try
			{
				lock (syncThread)
					registers["RegFifoThresh"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegFifoThresh"].Value & 128U) | (uint)(byte)((uint)value & (uint)sbyte.MaxValue));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
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
				OnError((byte)1, ex.Message);
			}
		}

		public void SetMessage(byte[] value)
		{
			try
			{
				lock (syncThread)
					packet.Message = value;
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
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
				OnError((byte)1, ex.Message);
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
				OnError((byte)1, ex.Message);
			}
		}

		public void SetPacketHandlerLogEnable(bool value)
		{
			try
			{
				lock (syncThread)
					packet.LogEnabled = value;
			}
			catch (Exception ex)
			{
				packet.LogEnabled = false;
				OnError((byte)1, ex.Message);
			}
		}

		public bool TransmitRfData(byte[] buffer)
		{
			lock (syncThread)
			{
				try
				{
					SetOperatingMode(OperatingModeEnum.Stdby, true);
					Thread.Sleep(100);
					bool local_0_1 = WriteFifo(buffer);
					SetOperatingMode(OperatingModeEnum.Tx, true);
					return local_0_1;
				}
				catch (Exception exception_0)
				{
					throw exception_0;
				}
			}
		}

		public bool ReceiveRfData(out byte[] buffer)
		{
			lock (syncThread)
			{
				try
				{
					SetOperatingMode(OperatingModeEnum.Stdby, true);
					buffer = FifoData;
					bool local_0_1 = ReadFifo(ref buffer);
					SetOperatingMode(OperatingModeEnum.Rx, true);
					return local_0_1;
				}
				catch (Exception exception_0)
				{
					throw exception_0;
				}
			}
		}

		private void packet_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			OnPropertyChanged(e.PropertyName);
		}

		public void SetSequencerStart()
		{
			try
			{
				lock (syncThread)
				{
					SequencerStarted = true;
					WriteRegister(registers["RegSeqConfig1"], (byte)((int)registers["RegSeqConfig1"].Value & 63 | 128));
				}
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetSequencerStop()
		{
			try
			{
				lock (syncThread)
				{
					SequencerStarted = false;
					WriteRegister(registers["RegSeqConfig1"], (byte)((int)registers["RegSeqConfig1"].Value & 63 | 64));
				}
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetIdleMode(IdleMode value)
		{
			try
			{
				lock (syncThread)
					registers["RegSeqConfig1"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegSeqConfig1"].Value & 223U) | (uint)(byte)((uint)(byte)value << 5));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetFromStart(FromStart value)
		{
			try
			{
				lock (syncThread)
					registers["RegSeqConfig1"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegSeqConfig1"].Value & 231U) | (uint)(byte)((uint)(byte)value << 3));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetLowPowerSelection(LowPowerSelection value)
		{
			try
			{
				lock (syncThread)
					registers["RegSeqConfig1"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegSeqConfig1"].Value & 251U) | (uint)(byte)((uint)(byte)value << 2));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetFromIdle(FromIdle value)
		{
			try
			{
				lock (syncThread)
					registers["RegSeqConfig1"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegSeqConfig1"].Value & 253U) | (uint)(byte)((uint)(byte)value << 1));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetFromTransmit(FromTransmit value)
		{
			try
			{
				lock (syncThread)
					registers["RegSeqConfig1"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegSeqConfig1"].Value & 254U) | (uint)(byte)value);
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetFromReceive(FromReceive value)
		{
			try
			{
				lock (syncThread)
					registers["RegSeqConfig2"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegSeqConfig2"].Value & 31U) | (uint)(byte)((uint)(byte)value << 5));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetFromRxTimeout(FromRxTimeout value)
		{
			try
			{
				lock (syncThread)
					registers["RegSeqConfig2"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegSeqConfig2"].Value & 231U) | (uint)(byte)((uint)(byte)value << 3));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetFromPacketReceived(FromPacketReceived value)
		{
			try
			{
				lock (syncThread)
					registers["RegSeqConfig2"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegSeqConfig2"].Value & 248U) | (uint)(byte)value);
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetTimer1Resolution(TimerResolution value)
		{
			try
			{
				lock (syncThread)
					registers["RegTimerResol"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegTimerResol"].Value & 243U) | (uint)(byte)((uint)(byte)value << 2));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetTimer2Resolution(TimerResolution value)
		{
			try
			{
				lock (syncThread)
					registers["RegTimerResol"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegTimerResol"].Value & 252U) | (uint)(byte)value);
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetTimer1Coef(byte value)
		{
			try
			{
				lock (syncThread)
					registers["RegTimer1Coef"].Value = (uint)value;
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetTimer2Coef(byte value)
		{
			try
			{
				lock (syncThread)
					registers["RegTimer2Coef"].Value = (uint)value;
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetRxCalAutoOn(bool value)
		{
			try
			{
				lock (syncThread)
					registers["RegImageCal"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegImageCal"].Value & (uint)sbyte.MaxValue) | (value ? 128 : 0));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
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
					ReadRegister(registers["RegImageCal"], ref local_1);
					WriteRegister(registers["RegImageCal"], (byte)((uint)local_1 | 64U));
					imageCalRunning = false;
					OnPropertyChanged("ImageCalRunning");
					DateTime local_2 = DateTime.Now;
					bool local_4_1;
					do
					{
						local_1 = (byte)0;
						ReadRegister(registers["RegImageCal"], ref local_1);
						local_4_1 = (DateTime.Now - local_2).TotalMilliseconds >= 1000.0;
					}
					while ((int)(byte)((uint)local_1 & 32U) == 32 && !local_4_1);
					if (local_4_1)
						throw new Exception("Image calibration timeout.");
				}
				finally
				{
					ReadRegister(registers["RegTemp"]);
					ReadRegister(registers["RegFormerTemp"]);
					SetOperatingMode(local_0);
				}
			}
		}

		public void SetTempThreshold(TempThresholdEnum value)
		{
			try
			{
				lock (syncThread)
					registers["RegImageCal"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegImageCal"].Value & 249U) | (uint)(byte)((uint)(byte)value << 1));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetTempMonitorOff(bool value)
		{
			try
			{
				lock (syncThread)
					registers["RegImageCal"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegImageCal"].Value & 254U) | (value ? 0 : 1));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetTempCalibrate(Decimal tempRoomValue)
		{
			lock (syncThread)
			{
				if (TempMonitorOff || Mode == OperatingModeEnum.Sleep || Mode == OperatingModeEnum.Stdby)
					return;
				TempCalDone = false;
				TempValueRoom = tempRoomValue;
				byte local_0 = (byte)0;
				ReadRegister(registers["RegTemp"], ref local_0);
				tempValueCal = (Decimal)((byte)((uint)local_0 & (uint)sbyte.MaxValue));
				if (((int)local_0 & 128) == 128)
					tempValueCal *= new Decimal(-1);
				ReadRegister(registers["RegTemp"]);
				ReadRegister(registers["RegFormerTemp"]);
				TempCalDone = true;
			}
		}

		public void SetLowBatOn(bool value)
		{
			try
			{
				lock (syncThread)
					registers["RegLowBat"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegLowBat"].Value & 247U) | (value ? 8 : 0));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetLowBatTrim(LowBatTrimEnum value)
		{
			try
			{
				lock (syncThread)
					registers["RegLowBat"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegLowBat"].Value & 248U) | (uint)(byte)value);
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void ClrRssiIrq()
		{
			try
			{
				lock (syncThread)
					registers["RegIrqFlags1"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegIrqFlags1"].Value & 247U) | 8U);
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void ClrPreambleDetectIrq()
		{
			try
			{
				lock (syncThread)
					registers["RegIrqFlags1"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegIrqFlags1"].Value & 253U) | 2U);
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void ClrSyncAddressMatchIrq()
		{
			try
			{
				lock (syncThread)
					registers["RegIrqFlags1"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegIrqFlags1"].Value & 254U) | 1U);
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void ClrFifoOverrunIrq()
		{
			try
			{
				lock (syncThread)
					registers["RegIrqFlags2"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegIrqFlags2"].Value & 239U) | 16U);
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void ClrLowBatIrq()
		{
			try
			{
				lock (syncThread)
					registers["RegIrqFlags2"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegIrqFlags2"].Value & 254U) | 1U);
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
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
				OnError((byte)1, ex.Message);
			}
		}

		public void SetDioPreambleIrqOn(bool value)
		{
			try
			{
				lock (syncThread)
					registers["RegDioMapping2"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegDioMapping2"].Value & 254U) | (value ? 1 : 0));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		public void SetFastHopOn(bool value)
		{
			try
			{
				lock (syncThread)
					registers["RegPllHop"].Value = (uint)(byte)((int)(byte)((uint)(byte)registers["RegPllHop"].Value & (uint)sbyte.MaxValue) | (value ? 128 : 0));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
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
				OnError((byte)1, ex.Message);
			}
		}

		public void SetPllBandwidth(Decimal value)
		{
			try
			{
				lock (syncThread)
				{
					// registers["RegPll"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegPll"].Value & 63U) | (uint)(byte)((uint)(byte)Decimal.op_Decrement(value / new Decimal(75000)) << 6));
					byte num = (byte)registers["RegPll"].Value;
					num = (byte)((num & 0x3F) | ((byte)(((byte)Decimal.Subtract(value / 75000M, 1M)) << 6)));
					registers["RegPll"].Value = num;
				}
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
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
				OnError((byte)1, ex.Message);
			}
		}

		public void SetBitrateFrac(Decimal value)
		{
			try
			{
				lock (syncThread)
					registers["RegBitrateFrac"].Value = (uint)(byte)((uint)(byte)((uint)(byte)registers["RegBitrateFrac"].Value & 240U) | (uint)(byte)((uint)(byte)value & 15U));
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		private void registers_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			lock (syncThread)
			{
				if (!(e.PropertyName == "Value"))
					return;
				UpdateRegisterValue((Register)sender);
				if (readLock == 0 && !Write((byte)((Register)sender).Address, (byte)((Register)sender).Value))
					OnError((byte)1, "Unable to write register " + ((Register)sender).Name);
				if (!(((Register)sender).Name == "RegOpMode"))
					return;
				if (Mode == OperatingModeEnum.Rx)
				{
					ReadRegister(registers["RegLna"]);
					ReadRegister(registers["RegFeiMsb"]);
					ReadRegister(registers["RegFeiLsb"]);
					ReadRegister(registers["RegAfcMsb"]);
					ReadRegister(registers["RegAfcLsb"]);
					ReadRegister(registers["RegRssiValue"]);
				}
				ReadIrqFlags();
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
				case "RxReady":
					int num = RxReady ? 1 : 0;
					break;
			}
		}

		private void usbDevice_Opened(object sender, EventArgs e)
		{
			isOpen = true;
			if (!SKGetVersion())
				OnError((byte)1, "Unable to read SK version.");
			else if (!SKReset())
			{
				OnError((byte)1, "Unable to reset the SK");
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
			OnError((byte)0, "-");
		}

		private void device_Dio0Changed(object sender, SX1276.IoChangedEventArgs e)
		{
			lock (syncThread)
			{
				if (!isPacketHandlerStarted || !e.State && !firstTransmit)
					return;
				firstTransmit = false;
				if (Mode == OperatingModeEnum.Tx)
				{
					OnPacketHandlerTransmitted();
					PacketHandlerTransmit();
				}
				else
				{
					if (Mode != OperatingModeEnum.Rx)
						return;
					PacketHandlerReceive();
				}
			}
		}

		private void device_Dio1Changed(object sender, SX1276.IoChangedEventArgs e)
		{
		}

		private void device_Dio2Changed(object sender, SX1276.IoChangedEventArgs e)
		{
		}

		private void device_Dio3Changed(object sender, SX1276.IoChangedEventArgs e)
		{
		}

		private void device_Dio4Changed(object sender, SX1276.IoChangedEventArgs e)
		{
		}

		private void device_Dio5Changed(object sender, SX1276.IoChangedEventArgs e)
		{
		}

		private void SpectrumProcess()
		{
			Decimal num = SpectrumFrequencyMin + SpectrumFrequencyStep * (Decimal)SpectrumFrequencyId;
			byte data1 = (byte)((long)(num / frequencyStep) >> 16);
			byte data2 = (byte)((long)(num / frequencyStep) >> 8);
			byte data3 = (byte)(long)(num / frequencyStep);
			if (!Write((byte)registers["RegFrfMsb"].Address, data1))
				OnError((byte)1, "Unable to write register " + registers["RegFrfMsb"].Name);
			if (!Write((byte)registers["RegFrfMid"].Address, data2))
				OnError((byte)1, "Unable to write register " + registers["RegFrfMid"].Name);
			if (!Write((byte)registers["RegFrfLsb"].Address, data3))
				OnError((byte)1, "Unable to write register " + registers["RegFrfLsb"].Name);
			SetRestartRxWithPllLock();
			ReadRegister(registers["RegRssiValue"]);
			++SpectrumFrequencyId;
			if (SpectrumFrequencyId < SpectrumNbFrequenciesMax)
				return;
			SpectrumFrequencyId = 0;
		}

		private void RegUpdateThread()
		{
			int num = 0;
			byte pinsState = (byte)0;
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
								if (((int)pinsState & 32) == 32)
									OnDio5Changed(true);
								else
									OnDio5Changed(false);
								if (((int)pinsState & 16) == 16)
									OnDio4Changed(true);
								else
									OnDio4Changed(false);
								if (((int)pinsState & 8) == 8)
									OnDio3Changed(true);
								else
									OnDio3Changed(false);
								if (((int)pinsState & 4) == 4)
									OnDio2Changed(true);
								else
									OnDio2Changed(false);
								if (((int)pinsState & 2) == 2)
									OnDio1Changed(true);
								else
									OnDio1Changed(false);
								if (((int)pinsState & 1) == 1)
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
								ReadIrqFlags();
								if (SequencerStarted)
									ReadRegister(registers["RegOpMode"]);
								if (Mode == OperatingModeEnum.Rx)
								{
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
								else if (Mode == OperatingModeEnum.Tx && PngEnabled && !FifoFull)
									WriteFifo(new byte[1]
                  {
                    png.NextByte()
                  });
								if (!TempMonitorOff && TempCalDone && (Mode != OperatingModeEnum.Sleep && Mode != OperatingModeEnum.Stdby))
								{
									tempMeasRunning = false;
									OnPropertyChanged("TempMeasRunning");
								}
							}
							if (num >= 200)
							{
								if (restartRx)
								{
									restartRx = false;
									ReadRegister(registers["RegLna"]);
									ReadRegister(registers["RegFeiMsb"]);
									ReadRegister(registers["RegFeiLsb"]);
									ReadRegister(registers["RegAfcMsb"]);
									ReadRegister(registers["RegAfcLsb"]);
								}
								if (!TempMonitorOff && TempCalDone && (Mode != OperatingModeEnum.Sleep && Mode != OperatingModeEnum.Stdby))
								{
									tempMeasRunning = true;
									OnPropertyChanged("TempMeasRunning");
									ReadRegister(registers["RegImageCal"]);
									ReadRegister(registers["RegTemp"]);
									ReadRegister(registers["RegFormerTemp"]);
								}
								num = 0;
							}
						}
					}
					catch
					{
					}
					++num;
					Thread.Sleep(1);
				}
			}
		}

		private void OnPropertyChanged(string propName)
		{
			if (PropertyChanged == null)
				return;
			PropertyChanged((object)this, new PropertyChangedEventArgs(propName));
		}

		public void Dispose()
		{
			Close();
		}

		public class IoChangedEventArgs : EventArgs
		{
			public bool State;

			public IoChangedEventArgs(bool state)
			{
				State = state;
			}
		}

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

		public delegate void IoChangedEventHandler(object sender, SX1276.IoChangedEventArgs e);
	}
}

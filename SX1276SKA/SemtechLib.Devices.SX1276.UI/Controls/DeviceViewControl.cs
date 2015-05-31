using SemtechLib.Controls;
using SemtechLib.Devices.Common.Events;
using SemtechLib.Devices.Common.Interfaces;
using SemtechLib.Devices.SX1276;
using SemtechLib.Devices.SX1276.Enumerations;
using SemtechLib.Devices.SX1276.Events;
using SemtechLib.General.Events;
using SemtechLib.General.Interfaces;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SemtechLib.Devices.SX1276.UI.Controls
{
	public class DeviceViewControl : UserControl, IDeviceView, IDisposable, INotifyDocumentationChanged
	{
		private delegate void DevicePropertyChangedDelegate(object sender, PropertyChangedEventArgs e);
		private delegate void DevicePacketHandlerStartedDelegate(object sender, EventArgs e);
		private delegate void DevicePacketHandlerStopedDelegate(object sender, EventArgs e);
		private delegate void DevicePacketHandlerTransmittedDelegate(object sender, PacketStatusEventArg e);

		private IContainer components;
		private TabControl tabControl1;
		private TabPage tabCommon;
		private TabPage tabTransmitter;
		private TabPage tabReceiver;
		private TabPage tabIrqMap;
		private TabPage tabPacketHandler;
		private CommonViewControl commonViewControl1;
		private GroupBoxEx gBoxOperatingMode;
		private RadioButton rBtnTransmitter;
		private RadioButton rBtnReceiver;
		private RadioButton rBtnSynthesizerRx;
		private RadioButton rBtnStandby;
		private RadioButton rBtnSleep;
		private TransmitterViewControl transmitterViewControl1;
		private ReceiverViewControl receiverViewControl1;
		private IrqMapViewControl irqMapViewControl1;
		private Led ledModeReady;
		private Label lbModeReady;
		private Label label19;
		private Label label18;
		private Led ledPllLock;
		private Label label17;
		private Led ledTxReady;
		private Led ledRxReady;
		private Led ledSyncAddressMatch;
		private Label label23;
		private Label label22;
		private Led ledPreamble;
		private Label label21;
		private Label label20;
		private Led ledTimeout;
		private Led ledRssi;
		private Led ledFifoOverrun;
		private Led ledFifoLevel;
		private Label label27;
		private Led ledFifoEmpty;
		private Label label26;
		private Label label25;
		private Label label24;
		private Led ledFifoFull;
		private Led ledLowBat;
		private Led ledCrcOk;
		private Led ledPayloadReady;
		private Led ledPacketSent;
		private Label label31;
		private Label label30;
		private Label label29;
		private Label label28;
		private GroupBoxEx gBoxIrqFlags;
		private PacketHandlerView packetHandlerView1;
		private TabPage tabTemperature;
		private TemperatureViewControl temperatureViewControl1;
		private RadioButton rBtnSynthesizerTx;
		private RadioButton rBtnSynthesizer;
		private Led ledFifoNotEmpty;
		private TabPage tabSequencer;
		private SequencerViewControl sequencerViewControl1;
		private SX1276 device;

		public IDevice Device
		{
			get
			{
				return (IDevice)this.device;
			}
			set
			{
				if (this.device == value)
					return;
				this.device = (SX1276)value;
				this.device.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(this.device_PropertyChanged);
				this.device.OcpTrimLimitStatusChanged += new SX1276.LimitCheckStatusChangedEventHandler(this.device_OcpTrimLimitStatusChanged);
				this.device.BitrateLimitStatusChanged += new SX1276.LimitCheckStatusChangedEventHandler(this.device_BitrateLimitStatusChanged);
				this.device.FdevLimitStatusChanged += new SX1276.LimitCheckStatusChangedEventHandler(this.device_FdevLimitStatusChanged);
				this.device.FrequencyRfLimitStatusChanged += new SX1276.LimitCheckStatusChangedEventHandler(this.device_FrequencyRfLimitStatusChanged);
				this.device.SyncValueLimitChanged += new SX1276.LimitCheckStatusChangedEventHandler(this.device_SyncValueLimitChanged);
				this.device.PacketHandlerStarted += new EventHandler(this.device_PacketHandlerStarted);
				this.device.PacketHandlerStoped += new EventHandler(this.device_PacketHandlerStoped);
				this.device.PacketHandlerTransmitted += new PacketStatusEventHandler(this.device_PacketHandlerTransmitted);
				this.device.PacketHandlerReceived += new PacketStatusEventHandler(this.device_PacketHandlerReceived);
				this.commonViewControl1.FrequencyXo = this.device.FrequencyXo;
				this.commonViewControl1.FrequencyStep = this.device.FrequencyStep;
				this.commonViewControl1.ModulationType = this.device.ModulationType;
				this.commonViewControl1.ModulationShaping = this.device.ModulationShaping;
				this.commonViewControl1.Bitrate = this.device.Bitrate;
				this.commonViewControl1.Fdev = this.device.Fdev;
				this.commonViewControl1.FrequencyRf = this.device.FrequencyRf;
				this.LoadTestPage(this.device);
			}
		}

		public new bool Enabled
		{
			get
			{
				return base.Enabled;
			}
			set
			{
				if (base.Enabled == value)
					return;
				base.Enabled = value;
			}
		}

		public event SemtechLib.General.Events.ErrorEventHandler Error;

		public event DocumentationChangedEventHandler DocumentationChanged;

		public DeviceViewControl()
		{
			this.InitializeComponent();
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
			this.tabControl1 = new TabControl();
			this.tabCommon = new TabPage();
			this.commonViewControl1 = new CommonViewControl();
			this.tabTransmitter = new TabPage();
			this.transmitterViewControl1 = new TransmitterViewControl();
			this.tabReceiver = new TabPage();
			this.receiverViewControl1 = new ReceiverViewControl();
			this.tabIrqMap = new TabPage();
			this.irqMapViewControl1 = new IrqMapViewControl();
			this.tabPacketHandler = new TabPage();
			this.packetHandlerView1 = new PacketHandlerView();
			this.tabSequencer = new TabPage();
			this.sequencerViewControl1 = new SequencerViewControl();
			this.tabTemperature = new TabPage();
			this.temperatureViewControl1 = new TemperatureViewControl();
			this.rBtnSynthesizer = new RadioButton();
			this.gBoxOperatingMode = new GroupBoxEx();
			this.rBtnTransmitter = new RadioButton();
			this.rBtnReceiver = new RadioButton();
			this.rBtnSynthesizerTx = new RadioButton();
			this.rBtnSynthesizerRx = new RadioButton();
			this.rBtnStandby = new RadioButton();
			this.rBtnSleep = new RadioButton();
			this.gBoxIrqFlags = new GroupBoxEx();
			this.ledLowBat = new Led();
			this.lbModeReady = new Label();
			this.ledCrcOk = new Led();
			this.ledRxReady = new Led();
			this.ledPayloadReady = new Led();
			this.ledTxReady = new Led();
			this.ledPacketSent = new Led();
			this.label17 = new Label();
			this.label31 = new Label();
			this.ledPllLock = new Led();
			this.label30 = new Label();
			this.label18 = new Label();
			this.label29 = new Label();
			this.label19 = new Label();
			this.label28 = new Label();
			this.ledModeReady = new Led();
			this.ledFifoOverrun = new Led();
			this.ledRssi = new Led();
			this.ledFifoLevel = new Led();
			this.ledTimeout = new Led();
			this.label27 = new Label();
			this.label20 = new Label();
			this.ledFifoEmpty = new Led();
			this.label21 = new Label();
			this.label26 = new Label();
			this.ledPreamble = new Led();
			this.label25 = new Label();
			this.label22 = new Label();
			this.label24 = new Label();
			this.label23 = new Label();
			this.ledFifoFull = new Led();
			this.ledSyncAddressMatch = new Led();
			this.ledFifoNotEmpty = new Led();
			this.tabControl1.SuspendLayout();
			this.tabCommon.SuspendLayout();
			this.tabTransmitter.SuspendLayout();
			this.tabReceiver.SuspendLayout();
			this.tabIrqMap.SuspendLayout();
			this.tabPacketHandler.SuspendLayout();
			this.tabSequencer.SuspendLayout();
			this.tabTemperature.SuspendLayout();
			this.gBoxOperatingMode.SuspendLayout();
			this.gBoxIrqFlags.SuspendLayout();
			this.SuspendLayout();
			this.tabControl1.Controls.Add((Control)this.tabCommon);
			this.tabControl1.Controls.Add((Control)this.tabTransmitter);
			this.tabControl1.Controls.Add((Control)this.tabReceiver);
			this.tabControl1.Controls.Add((Control)this.tabIrqMap);
			this.tabControl1.Controls.Add((Control)this.tabPacketHandler);
			this.tabControl1.Controls.Add((Control)this.tabSequencer);
			this.tabControl1.Controls.Add((Control)this.tabTemperature);
			this.tabControl1.Location = new Point(3, 3);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new Size(807, 519);
			this.tabControl1.TabIndex = 0;
			this.tabCommon.Controls.Add((Control)this.commonViewControl1);
			this.tabCommon.Location = new Point(4, 22);
			this.tabCommon.Name = "tabCommon";
			this.tabCommon.Padding = new Padding(3);
			this.tabCommon.Size = new Size(799, 493);
			this.tabCommon.TabIndex = 0;
			this.tabCommon.Text = "Common";
			this.tabCommon.UseVisualStyleBackColor = true;
			this.commonViewControl1.Band = BandEnum.AUTO;
			CommonViewControl commonViewControl1 = this.commonViewControl1;
			int[] bits1 = new int[4];
			bits1[0] = 4800;
			Decimal num1 = new Decimal(bits1);
			commonViewControl1.Bitrate = num1;
			this.commonViewControl1.BitrateFrac = new Decimal(new int[4]);
			this.commonViewControl1.FastHopOn = true;
			CommonViewControl commonViewControl2 = this.commonViewControl1;
			int[] bits2 = new int[4];
			bits2[0] = 5002;
			Decimal num2 = new Decimal(bits2);
			commonViewControl2.Fdev = num2;
			this.commonViewControl1.ForceRxBandLowFrequencyOn = true;
			this.commonViewControl1.ForceTxBandLowFrequencyOn = true;
			CommonViewControl commonViewControl3 = this.commonViewControl1;
			int[] bits3 = new int[4];
			bits3[0] = 915000000;
			Decimal num3 = new Decimal(bits3);
			commonViewControl3.FrequencyRf = num3;
			CommonViewControl commonViewControl4 = this.commonViewControl1;
			int[] bits4 = new int[4];
			bits4[0] = 61;
			Decimal num4 = new Decimal(bits4);
			commonViewControl4.FrequencyStep = num4;
			CommonViewControl commonViewControl5 = this.commonViewControl1;
			int[] bits5 = new int[4];
			bits5[0] = 32000000;
			Decimal num5 = new Decimal(bits5);
			commonViewControl5.FrequencyXo = num5;
			this.commonViewControl1.Location = new Point(0, 0);
			this.commonViewControl1.LowBatOn = true;
			this.commonViewControl1.LowBatTrim = LowBatTrimEnum.Trim1_835;
			this.commonViewControl1.LowFrequencyModeOn = true;
			this.commonViewControl1.ModulationShaping = (byte)0;
			this.commonViewControl1.ModulationType = ModulationTypeEnum.FSK;
			this.commonViewControl1.Name = "commonViewControl1";
			this.commonViewControl1.Size = new Size(799, 493);
			this.commonViewControl1.TabIndex = 0;
			this.commonViewControl1.TcxoInputOn = true;
			this.commonViewControl1.FrequencyXoChanged += new DecimalEventHandler(this.commonViewControl1_FrequencyXoChanged);
			this.commonViewControl1.ModulationTypeChanged += new ModulationTypeEventHandler(this.commonViewControl1_ModulationTypeChanged);
			this.commonViewControl1.ModulationShapingChanged += new ByteEventHandler(this.commonViewControl1_ModulationShapingChanged);
			this.commonViewControl1.BitrateChanged += new DecimalEventHandler(this.commonViewControl1_BitrateChanged);
			this.commonViewControl1.BitrateFracChanged += new DecimalEventHandler(this.commonViewControl1_BitrateFracChanged);
			this.commonViewControl1.FdevChanged += new DecimalEventHandler(this.commonViewControl1_FdevChanged);
			this.commonViewControl1.BandChanged += new BandEventHandler(this.commonViewControl1_BandChanged);
			this.commonViewControl1.ForceTxBandLowFrequencyOnChanged += new BooleanEventHandler(this.commonViewControl1_ForceTxBandLowFrequencyOnChanged);
			this.commonViewControl1.ForceRxBandLowFrequencyOnChanged += new BooleanEventHandler(this.commonViewControl1_ForceRxBandLowFrequencyOnChanged);
			this.commonViewControl1.LowFrequencyModeOnChanged += new BooleanEventHandler(this.commonViewControl1_LowFrequencyModeOnChanged);
			this.commonViewControl1.FrequencyRfChanged += new DecimalEventHandler(this.commonViewControl1_FrequencyRfChanged);
			this.commonViewControl1.FastHopOnChanged += new BooleanEventHandler(this.commonViewControl1_FastHopOnChanged);
			this.commonViewControl1.TcxoInputChanged += new BooleanEventHandler(this.commonViewControl1_TcxoInputChanged);
			this.commonViewControl1.RcCalibrationChanged += new EventHandler(this.commonViewControl1_RcCalibrationChanged);
			this.commonViewControl1.LowBatOnChanged += new BooleanEventHandler(this.commonViewControl1_LowBatOnChanged);
			this.commonViewControl1.LowBatTrimChanged += new LowBatTrimEventHandler(this.commonViewControl1_LowBatTrimChanged);
			this.commonViewControl1.DocumentationChanged += new DocumentationChangedEventHandler(this.commonViewControl1_DocumentationChanged);
			this.tabTransmitter.Controls.Add((Control)this.transmitterViewControl1);
			this.tabTransmitter.Location = new Point(4, 22);
			this.tabTransmitter.Name = "tabTransmitter";
			this.tabTransmitter.Padding = new Padding(3);
			this.tabTransmitter.Size = new Size(799, 493);
			this.tabTransmitter.TabIndex = 1;
			this.tabTransmitter.Text = "Transmitter";
			this.tabTransmitter.UseVisualStyleBackColor = true;
			this.transmitterViewControl1.Location = new Point(0, 0);
			this.transmitterViewControl1.MaxOutputPower = new Decimal(new int[4]
      {
        132,
        0,
        0,
        65536
      });
			this.transmitterViewControl1.Name = "transmitterViewControl1";
			this.transmitterViewControl1.OcpOn = true;
			this.transmitterViewControl1.OcpTrim = new Decimal(new int[4]
      {
        1000,
        0,
        0,
        65536
      });
			this.transmitterViewControl1.OutputPower = new Decimal(new int[4]
      {
        132,
        0,
        0,
        65536
      });
			this.transmitterViewControl1.Pa20dBm = false;
			this.transmitterViewControl1.PaRamp = PaRampEnum.PaRamp_40;
			this.transmitterViewControl1.PaSelect = PaSelectEnum.RFO;
			TransmitterViewControl transmitterViewControl = this.transmitterViewControl1;
			int[] bits6 = new int[4];
			bits6[0] = 300000;
			Decimal num6 = new Decimal(bits6);
			transmitterViewControl.PllBandwidth = num6;
			this.transmitterViewControl1.Size = new Size(799, 493);
			this.transmitterViewControl1.TabIndex = 0;
			this.transmitterViewControl1.PaModeChanged += new PaModeEventHandler(this.transmitterViewControl1_PaModeChanged);
			this.transmitterViewControl1.OutputPowerChanged += new DecimalEventHandler(this.transmitterViewControl1_OutputPowerChanged);
			this.transmitterViewControl1.MaxOutputPowerChanged += new DecimalEventHandler(this.transmitterViewControl1_MaxOutputPowerChanged);
			this.transmitterViewControl1.PaRampChanged += new PaRampEventHandler(this.transmitterViewControl1_PaRampChanged);
			this.transmitterViewControl1.OcpOnChanged += new BooleanEventHandler(this.transmitterViewControl1_OcpOnChanged);
			this.transmitterViewControl1.OcpTrimChanged += new DecimalEventHandler(this.transmitterViewControl1_OcpTrimChanged);
			this.transmitterViewControl1.Pa20dBmChanged += new BooleanEventHandler(this.transmitterViewControl1_Pa20dBmChanged);
			this.transmitterViewControl1.PllBandwidthChanged += new DecimalEventHandler(this.transmitterViewControl1_PllBandwidthChanged);
			this.transmitterViewControl1.DocumentationChanged += new DocumentationChangedEventHandler(this.transmitterViewControl1_DocumentationChanged);
			this.tabReceiver.Controls.Add((Control)this.receiverViewControl1);
			this.tabReceiver.Location = new Point(4, 22);
			this.tabReceiver.Name = "tabReceiver";
			this.tabReceiver.Padding = new Padding(3);
			this.tabReceiver.Size = new Size(799, 493);
			this.tabReceiver.TabIndex = 2;
			this.tabReceiver.Text = "Receiver";
			this.tabReceiver.UseVisualStyleBackColor = true;
			this.receiverViewControl1.AfcAutoClearOn = true;
			this.receiverViewControl1.AfcAutoOn = true;
			ReceiverViewControl receiverViewControl1 = this.receiverViewControl1;
			int[] bits7 = new int[4];
			bits7[0] = 50000;
			Decimal num7 = new Decimal(bits7);
			receiverViewControl1.AfcRxBw = num7;
			ReceiverViewControl receiverViewControl2 = this.receiverViewControl1;
			int[] bits8 = new int[4];
			bits8[0] = 400000;
			Decimal num8 = new Decimal(bits8);
			receiverViewControl2.AfcRxBwMax = num8;
			ReceiverViewControl receiverViewControl3 = this.receiverViewControl1;
			int[] bits9 = new int[4];
			bits9[0] = 3125;
			Decimal num9 = new Decimal(bits9);
			receiverViewControl3.AfcRxBwMin = num9;
			ReceiverViewControl receiverViewControl4 = this.receiverViewControl1;
			int[] bits10 = new int[4];
			bits10[3] = 65536;
			Decimal num10 = new Decimal(bits10);
			receiverViewControl4.AfcValue = num10;
			this.receiverViewControl1.AgcAutoOn = true;
			this.receiverViewControl1.AgcReference = -80;
			this.receiverViewControl1.AgcReferenceLevel = 19;
			this.receiverViewControl1.AgcStep1 = (byte)16;
			this.receiverViewControl1.AgcStep2 = (byte)7;
			this.receiverViewControl1.AgcStep3 = (byte)11;
			this.receiverViewControl1.AgcStep4 = (byte)9;
			this.receiverViewControl1.AgcStep5 = (byte)11;
			this.receiverViewControl1.AgcThresh1 = 0;
			this.receiverViewControl1.AgcThresh2 = 0;
			this.receiverViewControl1.AgcThresh3 = 0;
			this.receiverViewControl1.AgcThresh4 = 0;
			this.receiverViewControl1.AgcThresh5 = 0;
			ReceiverViewControl receiverViewControl5 = this.receiverViewControl1;
			int[] bits11 = new int[4];
			bits11[0] = 4800;
			Decimal num11 = new Decimal(bits11);
			receiverViewControl5.Bitrate = num11;
			this.receiverViewControl1.BitSyncOn = true;
			this.receiverViewControl1.DataMode = DataModeEnum.Packet;
			ReceiverViewControl receiverViewControl6 = this.receiverViewControl1;
			int[] bits12 = new int[4];
			bits12[3] = 65536;
			Decimal num12 = new Decimal(bits12);
			receiverViewControl6.FeiValue = num12;
			ReceiverViewControl receiverViewControl7 = this.receiverViewControl1;
			int[] bits13 = new int[4];
			bits13[0] = 32000000;
			Decimal num13 = new Decimal(bits13);
			receiverViewControl7.FrequencyXo = num13;
			this.receiverViewControl1.InterPacketRxDelay = new Decimal(new int[4]);
			this.receiverViewControl1.LnaBoost = true;
			this.receiverViewControl1.Location = new Point(0, 0);
			this.receiverViewControl1.ModulationType = ModulationTypeEnum.FSK;
			this.receiverViewControl1.Name = "receiverViewControl1";
			this.receiverViewControl1.OokAverageOffset = new Decimal(new int[4]);
			this.receiverViewControl1.OokAverageThreshFilt = OokAverageThreshFiltEnum.COEF_2;
			this.receiverViewControl1.OokFixedThreshold = (byte)6;
			this.receiverViewControl1.OokPeakThreshDec = OokPeakThreshDecEnum.EVERY_1_CHIPS_1_TIMES;
			this.receiverViewControl1.OokPeakThreshStep = new Decimal(new int[4]
      {
        5,
        0,
        0,
        65536
      });
			this.receiverViewControl1.OokThreshType = OokThreshTypeEnum.Peak;
			this.receiverViewControl1.PreambleDetectorOn = true;
			this.receiverViewControl1.PreambleDetectorSize = (byte)1;
			this.receiverViewControl1.PreambleDetectorTol = (byte)0;
			this.receiverViewControl1.RestartRxOnCollision = true;
			this.receiverViewControl1.RssiCollisionThreshold = new Decimal(new int[4]);
			this.receiverViewControl1.RssiOffset = new Decimal(new int[4]);
			ReceiverViewControl receiverViewControl8 = this.receiverViewControl1;
			int[] bits14 = new int[4];
			bits14[0] = 2;
			Decimal num14 = new Decimal(bits14);
			receiverViewControl8.RssiSmoothing = num14;
			this.receiverViewControl1.RssiThreshold = new Decimal(new int[4]
      {
        116,
        0,
        0,
        int.MinValue
      });
			this.receiverViewControl1.RssiValue = new Decimal(new int[4]
      {
        1275,
        0,
        0,
        -2147418112
      });
			this.receiverViewControl1.RxBw = new Decimal(new int[4]
      {
        1890233003,
        -2135170438,
        564688631,
        1572864
      });
			ReceiverViewControl receiverViewControl9 = this.receiverViewControl1;
			int[] bits15 = new int[4];
			bits15[0] = 500000;
			Decimal num15 = new Decimal(bits15);
			receiverViewControl9.RxBwMax = num15;
			ReceiverViewControl receiverViewControl10 = this.receiverViewControl1;
			int[] bits16 = new int[4];
			bits16[0] = 3906;
			Decimal num16 = new Decimal(bits16);
			receiverViewControl10.RxBwMin = num16;
			this.receiverViewControl1.Size = new Size(799, 493);
			this.receiverViewControl1.TabIndex = 0;
			this.receiverViewControl1.TimeoutRxPreamble = new Decimal(new int[4]);
			this.receiverViewControl1.TimeoutRxRssi = new Decimal(new int[4]);
			this.receiverViewControl1.TimeoutSignalSync = new Decimal(new int[4]);
			this.receiverViewControl1.AgcReferenceLevelChanged += new Int32EventHandler(this.receiverViewControl1_AgcReferenceLevelChanged);
			this.receiverViewControl1.AgcStepChanged += new AgcStepEventHandler(this.receiverViewControl1_AgcStepChanged);
			this.receiverViewControl1.LnaGainChanged += new LnaGainEventHandler(this.receiverViewControl1_LnaGainChanged);
			this.receiverViewControl1.LnaBoostChanged += new BooleanEventHandler(this.receiverViewControl1_LnaBoostChanged);
			this.receiverViewControl1.RestartRxOnCollisionOnChanged += new BooleanEventHandler(this.receiverViewControl1_RestartRxOnCollisionOnChanged);
			this.receiverViewControl1.RestartRxWithoutPllLockChanged += new EventHandler(this.receiverViewControl1_RestartRxWithoutPllLockChanged);
			this.receiverViewControl1.RestartRxWithPllLockChanged += new EventHandler(this.receiverViewControl1_RestartRxWithPllLockChanged);
			this.receiverViewControl1.AfcAutoOnChanged += new BooleanEventHandler(this.receiverViewControl1_AfcAutoOnChanged);
			this.receiverViewControl1.AgcAutoOnChanged += new BooleanEventHandler(this.receiverViewControl1_AgcAutoOnChanged);
			this.receiverViewControl1.RxTriggerChanged += new RxTriggerEventHandler(this.receiverViewControl1_RxTriggerChanged);
			this.receiverViewControl1.RssiOffsetChanged += new DecimalEventHandler(this.receiverViewControl1_RssiOffsetChanged);
			this.receiverViewControl1.RssiSmoothingChanged += new DecimalEventHandler(this.receiverViewControl1_RssiSmoothingChanged);
			this.receiverViewControl1.RssiCollisionThresholdChanged += new DecimalEventHandler(this.receiverViewControl1_RssiCollisionThresholdChanged);
			this.receiverViewControl1.RssiThreshChanged += new DecimalEventHandler(this.receiverViewControl1_RssiThreshChanged);
			this.receiverViewControl1.RxBwChanged += new DecimalEventHandler(this.receiverViewControl1_RxBwChanged);
			this.receiverViewControl1.AfcRxBwChanged += new DecimalEventHandler(this.receiverViewControl1_AfcRxBwChanged);
			this.receiverViewControl1.BitSyncOnChanged += new BooleanEventHandler(this.receiverViewControl1_BitSyncOnChanged);
			this.receiverViewControl1.OokThreshTypeChanged += new OokThreshTypeEventHandler(this.receiverViewControl1_OokThreshTypeChanged);
			this.receiverViewControl1.OokPeakThreshStepChanged += new DecimalEventHandler(this.receiverViewControl1_OokPeakThreshStepChanged);
			this.receiverViewControl1.OokPeakThreshDecChanged += new OokPeakThreshDecEventHandler(this.receiverViewControl1_OokPeakThreshDecChanged);
			this.receiverViewControl1.OokAverageThreshFiltChanged += new OokAverageThreshFiltEventHandler(this.receiverViewControl1_OokAverageThreshFiltChanged);
			this.receiverViewControl1.OokAverageBiasChanged += new DecimalEventHandler(this.receiverViewControl1_OokAverageBiasChanged);
			this.receiverViewControl1.OokFixedThreshChanged += new ByteEventHandler(this.receiverViewControl1_OokFixedThreshChanged);
			this.receiverViewControl1.AgcStartChanged += new EventHandler(this.receiverViewControl1_AgcStartChanged);
			this.receiverViewControl1.FeiReadChanged += new EventHandler(this.receiverViewControl1_FeiReadChanged);
			this.receiverViewControl1.AfcAutoClearOnChanged += new BooleanEventHandler(this.receiverViewControl1_AfcAutoClearOnChanged);
			this.receiverViewControl1.AfcClearChanged += new EventHandler(this.receiverViewControl1_AfcClearChanged);
			this.receiverViewControl1.PreambleDetectorOnChanged += new BooleanEventHandler(this.receiverViewControl1_PreambleDetectorOnChanged);
			this.receiverViewControl1.PreambleDetectorSizeChanged += new ByteEventHandler(this.receiverViewControl1_PreambleDetectorSizeChanged);
			this.receiverViewControl1.PreambleDetectorTolChanged += new ByteEventHandler(this.receiverViewControl1_PreambleDetectorTolChanged);
			this.receiverViewControl1.TimeoutRssiChanged += new DecimalEventHandler(this.receiverViewControl1_TimeoutRssiChanged);
			this.receiverViewControl1.TimeoutPreambleChanged += new DecimalEventHandler(this.receiverViewControl1_TimeoutPreambleChanged);
			this.receiverViewControl1.TimeoutSyncWordChanged += new DecimalEventHandler(this.receiverViewControl1_TimeoutSyncWordChanged);
			this.receiverViewControl1.AutoRxRestartDelayChanged += new DecimalEventHandler(this.receiverViewControl1_AutoRxRestartDelayChanged);
			this.receiverViewControl1.DocumentationChanged += new DocumentationChangedEventHandler(this.receiverViewControl1_DocumentationChanged);
			this.tabIrqMap.Controls.Add((Control)this.irqMapViewControl1);
			this.tabIrqMap.Location = new Point(4, 22);
			this.tabIrqMap.Name = "tabIrqMap";
			this.tabIrqMap.Padding = new Padding(3);
			this.tabIrqMap.Size = new Size(799, 493);
			this.tabIrqMap.TabIndex = 3;
			this.tabIrqMap.Text = "IRQ & Map";
			this.tabIrqMap.UseVisualStyleBackColor = true;
			this.irqMapViewControl1.BitSyncOn = true;
			this.irqMapViewControl1.DataMode = DataModeEnum.Packet;
			IrqMapViewControl irqMapViewControl = this.irqMapViewControl1;
			int[] bits17 = new int[4];
			bits17[0] = 32000000;
			Decimal num17 = new Decimal(bits17);
			irqMapViewControl.FrequencyXo = num17;
			this.irqMapViewControl1.Location = new Point(0, 0);
			this.irqMapViewControl1.MapPreambleDetect = false;
			this.irqMapViewControl1.Mode = OperatingModeEnum.Stdby;
			this.irqMapViewControl1.Name = "irqMapViewControl1";
			this.irqMapViewControl1.Size = new Size(799, 493);
			this.irqMapViewControl1.TabIndex = 0;
			this.irqMapViewControl1.DioPreambleIrqOnChanged += new BooleanEventHandler(this.irqMapViewControl1_DioPreambleIrqOnChanged);
			this.irqMapViewControl1.DioMappingChanged += new DioMappingEventHandler(this.irqMapViewControl1_DioMappingChanged);
			this.irqMapViewControl1.ClockOutChanged += new ClockOutEventHandler(this.irqMapViewControl1_ClockOutChanged);
			this.irqMapViewControl1.DocumentationChanged += new DocumentationChangedEventHandler(this.irqMapViewControl1_DocumentationChanged);
			this.tabPacketHandler.Controls.Add((Control)this.packetHandlerView1);
			this.tabPacketHandler.Location = new Point(4, 22);
			this.tabPacketHandler.Name = "tabPacketHandler";
			this.tabPacketHandler.Padding = new Padding(3);
			this.tabPacketHandler.Size = new Size(799, 493);
			this.tabPacketHandler.TabIndex = 4;
			this.tabPacketHandler.Text = "Packet Handler";
			this.tabPacketHandler.UseVisualStyleBackColor = true;
			this.packetHandlerView1.AddressFiltering = AddressFilteringEnum.OFF;
			this.packetHandlerView1.AutoRestartRxOn = AutoRestartRxEnum.PLL_WAIT_ON;
			this.packetHandlerView1.BeaconOn = true;
			this.packetHandlerView1.Bitrate = new Decimal(new int[4]);
			this.packetHandlerView1.BitSyncOn = true;
			this.packetHandlerView1.BroadcastAddress = (byte)0;
			this.packetHandlerView1.Crc = (ushort)0;
			this.packetHandlerView1.CrcAutoClearOff = false;
			this.packetHandlerView1.CrcIbmOn = true;
			this.packetHandlerView1.CrcOn = true;
			this.packetHandlerView1.DataMode = DataModeEnum.Packet;
			this.packetHandlerView1.DcFree = DcFreeEnum.OFF;
			this.packetHandlerView1.FifoFillCondition = FifoFillConditionEnum.OnSyncAddressIrq;
			this.packetHandlerView1.FifoThreshold = (byte)15;
			this.packetHandlerView1.IoHomeOn = true;
			this.packetHandlerView1.IoHomePwrFrameOn = true;
			this.packetHandlerView1.Location = new Point(0, 0);
			this.packetHandlerView1.LogEnabled = false;
			this.packetHandlerView1.MaxPacketNumber = 0;
			this.packetHandlerView1.Message = new byte[0];
			this.packetHandlerView1.MessageLength = 0;
			this.packetHandlerView1.Mode = OperatingModeEnum.Stdby;
			this.packetHandlerView1.Name = "packetHandlerView1";
			this.packetHandlerView1.NodeAddress = (byte)0;
			this.packetHandlerView1.NodeAddressRx = (byte)0;
			this.packetHandlerView1.PacketFormat = PacketFormatEnum.Fixed;
			this.packetHandlerView1.PacketNumber = 0;
			this.packetHandlerView1.PayloadLength = (short)66;
			this.packetHandlerView1.PreamblePolarity = PreamblePolarityEnum.POLARITY_AA;
			this.packetHandlerView1.PreambleSize = 3;
			this.packetHandlerView1.Size = new Size(799, 493);
			this.packetHandlerView1.StartStop = false;
			this.packetHandlerView1.SyncOn = true;
			this.packetHandlerView1.SyncSize = (byte)4;
			this.packetHandlerView1.SyncValue = new byte[4]
      {
        (byte) 170,
        (byte) 170,
        (byte) 170,
        (byte) 170
      };
			this.packetHandlerView1.TabIndex = 0;
			this.packetHandlerView1.TxStartCondition = true;
			this.packetHandlerView1.Error += new SemtechLib.General.Events.ErrorEventHandler(this.packetHandlerView1_Error);
			this.packetHandlerView1.DataModeChanged += new DataModeEventHandler(this.packetHandlerView1_DataModeChanged);
			this.packetHandlerView1.PreambleSizeChanged += new Int32EventHandler(this.packetHandlerView1_PreambleSizeChanged);
			this.packetHandlerView1.AutoRestartRxChanged += new AutoRestartRxEventHandler(this.packetHandlerView1_AutoRestartRxChanged);
			this.packetHandlerView1.PreamblePolarityChanged += new PreamblePolarityEventHandler(this.packetHandlerView1_PreamblePolarityChanged);
			this.packetHandlerView1.SyncOnChanged += new BooleanEventHandler(this.packetHandlerView1_SyncOnChanged);
			this.packetHandlerView1.FifoFillConditionChanged += new FifoFillConditionEventHandler(this.packetHandlerView1_FifoFillConditionChanged);
			this.packetHandlerView1.SyncSizeChanged += new ByteEventHandler(this.packetHandlerView1_SyncSizeChanged);
			this.packetHandlerView1.SyncValueChanged += new ByteArrayEventHandler(this.packetHandlerView1_SyncValueChanged);
			this.packetHandlerView1.PacketFormatChanged += new PacketFormatEventHandler(this.packetHandlerView1_PacketFormatChanged);
			this.packetHandlerView1.DcFreeChanged += new DcFreeEventHandler(this.packetHandlerView1_DcFreeChanged);
			this.packetHandlerView1.CrcOnChanged += new BooleanEventHandler(this.packetHandlerView1_CrcOnChanged);
			this.packetHandlerView1.CrcAutoClearOffChanged += new BooleanEventHandler(this.packetHandlerView1_CrcAutoClearOffChanged);
			this.packetHandlerView1.AddressFilteringChanged += new AddressFilteringEventHandler(this.packetHandlerView1_AddressFilteringChanged);
			this.packetHandlerView1.PayloadLengthChanged += new Int16EventHandler(this.packetHandlerView1_PayloadLengthChanged);
			this.packetHandlerView1.NodeAddressChanged += new ByteEventHandler(this.packetHandlerView1_NodeAddressChanged);
			this.packetHandlerView1.BroadcastAddressChanged += new ByteEventHandler(this.packetHandlerView1_BroadcastAddressChanged);
			this.packetHandlerView1.TxStartConditionChanged += new BooleanEventHandler(this.packetHandlerView1_TxStartConditionChanged);
			this.packetHandlerView1.FifoThresholdChanged += new ByteEventHandler(this.packetHandlerView1_FifoThresholdChanged);
			this.packetHandlerView1.MessageLengthChanged += new Int32EventHandler(this.packetHandlerView1_MessageLengthChanged);
			this.packetHandlerView1.MessageChanged += new ByteArrayEventHandler(this.packetHandlerView1_MessageChanged);
			this.packetHandlerView1.StartStopChanged += new BooleanEventHandler(this.packetHandlerView1_StartStopChanged);
			this.packetHandlerView1.MaxPacketNumberChanged += new Int32EventHandler(this.packetHandlerView1_MaxPacketNumberChanged);
			this.packetHandlerView1.PacketHandlerLogEnableChanged += new BooleanEventHandler(this.packetHandlerView1_PacketHandlerLogEnableChanged);
			this.packetHandlerView1.CrcIbmChanged += new BooleanEventHandler(this.packetHandlerView1_CrcIbmChanged);
			this.packetHandlerView1.IoHomeOnChanged += new BooleanEventHandler(this.packetHandlerView1_IoHomeOnChanged);
			this.packetHandlerView1.IoHomePwrFrameOnChanged += new BooleanEventHandler(this.packetHandlerView1_IoHomePwrFrameOnChanged);
			this.packetHandlerView1.BeaconOnChanged += new BooleanEventHandler(this.packetHandlerView1_BeaconOnChanged);
			this.packetHandlerView1.FillFifoChanged += new EventHandler(this.packetHandlerView1_FillFifoChanged);
			this.packetHandlerView1.DocumentationChanged += new DocumentationChangedEventHandler(this.packetHandlerView1_DocumentationChanged);
			this.tabSequencer.Controls.Add((Control)this.sequencerViewControl1);
			this.tabSequencer.Location = new Point(4, 22);
			this.tabSequencer.Name = "tabSequencer";
			this.tabSequencer.Size = new Size(799, 493);
			this.tabSequencer.TabIndex = 6;
			this.tabSequencer.Text = "Sequencer";
			this.tabSequencer.UseVisualStyleBackColor = true;
			this.sequencerViewControl1.FromIdle = FromIdle.TO_RX_ON_TMR1;
			this.sequencerViewControl1.FromPacketReceived = FromPacketReceived.TO_IDLE;
			this.sequencerViewControl1.FromReceive = FromReceive.UNUSED_1;
			this.sequencerViewControl1.FromRxTimeout = FromRxTimeout.TO_RX_RESTART;
			this.sequencerViewControl1.FromStart = FromStart.TO_LOW_POWER_SELECTION;
			this.sequencerViewControl1.FromTransmit = FromTransmit.TO_RX;
			this.sequencerViewControl1.IdleMode = IdleMode.STANDBY;
			this.sequencerViewControl1.Location = new Point(0, 0);
			this.sequencerViewControl1.LowPowerSelection = LowPowerSelection.TO_LPM;
			this.sequencerViewControl1.Name = "sequencerViewControl1";
			this.sequencerViewControl1.Size = new Size(799, 493);
			this.sequencerViewControl1.TabIndex = 0;
			this.sequencerViewControl1.Timer1Coef = (byte)245;
			this.sequencerViewControl1.Timer1Resolution = TimerResolution.OFF;
			this.sequencerViewControl1.Timer2Coef = (byte)32;
			this.sequencerViewControl1.Timer2Resolution = TimerResolution.OFF;
			this.sequencerViewControl1.SequencerStartChanged += new EventHandler(this.sequencerViewControl1_SequencerStartChanged);
			this.sequencerViewControl1.SequencerStopChanged += new EventHandler(this.sequencerViewControl1_SequencerStopChanged);
			this.sequencerViewControl1.IdleModeChanged += new IdleModeEventHandler(this.sequencerViewControl1_IdleModeChanged);
			this.sequencerViewControl1.FromStartChanged += new FromStartEventHandler(this.sequencerViewControl1_FromStartChanged);
			this.sequencerViewControl1.LowPowerSelectionChanged += new LowPowerSelectionEventHandler(this.sequencerViewControl1_LowPowerSelectionChanged);
			this.sequencerViewControl1.FromIdleChanged += new FromIdleEventHandler(this.sequencerViewControl1_FromIdleChanged);
			this.sequencerViewControl1.FromTransmitChanged += new FromTransmitEventHandler(this.sequencerViewControl1_FromTransmitChanged);
			this.sequencerViewControl1.FromReceiveChanged += new FromReceiveEventHandler(this.sequencerViewControl1_FromReceiveChanged);
			this.sequencerViewControl1.FromRxTimeoutChanged += new FromRxTimeoutEventHandler(this.sequencerViewControl1_FromRxTimeoutChanged);
			this.sequencerViewControl1.FromPacketReceivedChanged += new FromPacketReceivedEventHandler(this.sequencerViewControl1_FromPacketReceivedChanged);
			this.sequencerViewControl1.Timer1ResolutionChanged += new TimerResolutionEventHandler(this.sequencerViewControl1_Timer1ResolutionChanged);
			this.sequencerViewControl1.Timer2ResolutionChanged += new TimerResolutionEventHandler(this.sequencerViewControl1_Timer2ResolutionChanged);
			this.sequencerViewControl1.Timer1CoefChanged += new ByteEventHandler(this.sequencerViewControl1_Timer1CoefChanged);
			this.sequencerViewControl1.Timer2CoefChanged += new ByteEventHandler(this.sequencerViewControl1_Timer2CoefChanged);
			this.sequencerViewControl1.DocumentationChanged += new DocumentationChangedEventHandler(this.sequencerViewControl1_DocumentationChanged);
			this.tabTemperature.Controls.Add((Control)this.temperatureViewControl1);
			this.tabTemperature.Location = new Point(4, 22);
			this.tabTemperature.Name = "tabTemperature";
			this.tabTemperature.Padding = new Padding(3);
			this.tabTemperature.Size = new Size(799, 493);
			this.tabTemperature.TabIndex = 5;
			this.tabTemperature.Text = "Temperature";
			this.tabTemperature.UseVisualStyleBackColor = true;
			this.temperatureViewControl1.AutoImageCalOn = true;
			this.temperatureViewControl1.ImageCalRunning = false;
			this.temperatureViewControl1.Location = new Point(0, 0);
			this.temperatureViewControl1.Mode = OperatingModeEnum.Stdby;
			this.temperatureViewControl1.Name = "temperatureViewControl1";
			this.temperatureViewControl1.Size = new Size(799, 493);
			this.temperatureViewControl1.TabIndex = 0;
			this.temperatureViewControl1.TempCalDone = false;
			this.temperatureViewControl1.TempChange = false;
			TemperatureViewControl temperatureViewControl1 = this.temperatureViewControl1;
			int[] bits18 = new int[4];
			bits18[3] = 65536;
			Decimal num18 = new Decimal(bits18);
			temperatureViewControl1.TempDelta = num18;
			this.temperatureViewControl1.TempMeasRunning = false;
			this.temperatureViewControl1.TempMonitorOff = true;
			this.temperatureViewControl1.TempThreshold = TempThresholdEnum.THRESH_05;
			TemperatureViewControl temperatureViewControl2 = this.temperatureViewControl1;
			int[] bits19 = new int[4];
			bits19[0] = 25;
			Decimal num19 = new Decimal(bits19);
			temperatureViewControl2.TempValue = num19;
			this.temperatureViewControl1.TempValueRoom = new Decimal(new int[4]
      {
        250,
        0,
        0,
        65536
      });
			this.temperatureViewControl1.RxCalAutoOnChanged += new BooleanEventHandler(this.temperatureViewControl1_RxCalAutoOnChanged);
			this.temperatureViewControl1.RxCalibrationChanged += new EventHandler(this.temperatureViewControl1_RxCalibrationChanged);
			this.temperatureViewControl1.TempThresholdChanged += new TempThresholdEventHandler(this.temperatureViewControl1_TempThresholdChanged);
			this.temperatureViewControl1.TempCalibrateChanged += new DecimalEventHandler(this.temperatureViewControl1_TempCalibrateChanged);
			this.temperatureViewControl1.TempMeasOnChanged += new BooleanEventHandler(this.temperatureViewControl1_TempMeasOnChanged);
			this.temperatureViewControl1.DocumentationChanged += new DocumentationChangedEventHandler(this.temperatureViewControl1_DocumentationChanged);
			this.rBtnSynthesizer.AutoSize = true;
			this.rBtnSynthesizer.Location = new Point(16, 51);
			this.rBtnSynthesizer.Name = "rBtnSynthesizer";
			this.rBtnSynthesizer.Size = new Size(79, 17);
			this.rBtnSynthesizer.TabIndex = 2;
			this.rBtnSynthesizer.Text = "Synthesizer";
			this.rBtnSynthesizer.UseVisualStyleBackColor = true;
			this.rBtnSynthesizer.CheckedChanged += new EventHandler(this.rBtnOperatingMode_CheckedChanged);
			this.gBoxOperatingMode.Controls.Add((Control)this.rBtnTransmitter);
			this.gBoxOperatingMode.Controls.Add((Control)this.rBtnReceiver);
			this.gBoxOperatingMode.Controls.Add((Control)this.rBtnSynthesizerTx);
			this.gBoxOperatingMode.Controls.Add((Control)this.rBtnSynthesizerRx);
			this.gBoxOperatingMode.Controls.Add((Control)this.rBtnStandby);
			this.gBoxOperatingMode.Controls.Add((Control)this.rBtnSleep);
			this.gBoxOperatingMode.Location = new Point(816, 411);
			this.gBoxOperatingMode.Name = "gBoxOperatingMode";
			this.gBoxOperatingMode.Size = new Size(189, 107);
			this.gBoxOperatingMode.TabIndex = 2;
			this.gBoxOperatingMode.TabStop = false;
			this.gBoxOperatingMode.Text = "Operating mode";
			this.gBoxOperatingMode.MouseEnter += new EventHandler(this.control_MouseEnter);
			this.gBoxOperatingMode.MouseLeave += new EventHandler(this.control_MouseLeave);
			this.rBtnTransmitter.AutoSize = true;
			this.rBtnTransmitter.Location = new Point(94, 51);
			this.rBtnTransmitter.Name = "rBtnTransmitter";
			this.rBtnTransmitter.Size = new Size(77, 17);
			this.rBtnTransmitter.TabIndex = 4;
			this.rBtnTransmitter.Text = "Transmitter";
			this.rBtnTransmitter.UseVisualStyleBackColor = true;
			this.rBtnTransmitter.CheckedChanged += new EventHandler(this.rBtnOperatingMode_CheckedChanged);
			this.rBtnReceiver.AutoSize = true;
			this.rBtnReceiver.Location = new Point(94, 80);
			this.rBtnReceiver.Name = "rBtnReceiver";
			this.rBtnReceiver.Size = new Size(68, 17);
			this.rBtnReceiver.TabIndex = 3;
			this.rBtnReceiver.Text = "Receiver";
			this.rBtnReceiver.UseVisualStyleBackColor = true;
			this.rBtnReceiver.CheckedChanged += new EventHandler(this.rBtnOperatingMode_CheckedChanged);
			this.rBtnSynthesizerTx.AutoSize = true;
			this.rBtnSynthesizerTx.Location = new Point(16, 51);
			this.rBtnSynthesizerTx.Name = "rBtnSynthesizerTx";
			this.rBtnSynthesizerTx.Size = new Size(70, 17);
			this.rBtnSynthesizerTx.TabIndex = 2;
			this.rBtnSynthesizerTx.Text = "Synth. Tx";
			this.rBtnSynthesizerTx.UseVisualStyleBackColor = true;
			this.rBtnSynthesizerTx.CheckedChanged += new EventHandler(this.rBtnOperatingMode_CheckedChanged);
			this.rBtnSynthesizerRx.AutoSize = true;
			this.rBtnSynthesizerRx.Location = new Point(16, 80);
			this.rBtnSynthesizerRx.Name = "rBtnSynthesizerRx";
			this.rBtnSynthesizerRx.Size = new Size(71, 17);
			this.rBtnSynthesizerRx.TabIndex = 2;
			this.rBtnSynthesizerRx.Text = "Synth. Rx";
			this.rBtnSynthesizerRx.UseVisualStyleBackColor = true;
			this.rBtnSynthesizerRx.CheckedChanged += new EventHandler(this.rBtnOperatingMode_CheckedChanged);
			this.rBtnStandby.AutoSize = true;
			this.rBtnStandby.Checked = true;
			this.rBtnStandby.Location = new Point(94, 20);
			this.rBtnStandby.Name = "rBtnStandby";
			this.rBtnStandby.Size = new Size(64, 17);
			this.rBtnStandby.TabIndex = 1;
			this.rBtnStandby.TabStop = true;
			this.rBtnStandby.Text = "Standby";
			this.rBtnStandby.UseVisualStyleBackColor = true;
			this.rBtnStandby.CheckedChanged += new EventHandler(this.rBtnOperatingMode_CheckedChanged);
			this.rBtnSleep.AutoSize = true;
			this.rBtnSleep.Location = new Point(16, 20);
			this.rBtnSleep.Name = "rBtnSleep";
			this.rBtnSleep.Size = new Size(52, 17);
			this.rBtnSleep.TabIndex = 0;
			this.rBtnSleep.Text = "Sleep";
			this.rBtnSleep.UseVisualStyleBackColor = true;
			this.rBtnSleep.CheckedChanged += new EventHandler(this.rBtnOperatingMode_CheckedChanged);
			this.gBoxIrqFlags.Controls.Add((Control)this.ledLowBat);
			this.gBoxIrqFlags.Controls.Add((Control)this.lbModeReady);
			this.gBoxIrqFlags.Controls.Add((Control)this.ledCrcOk);
			this.gBoxIrqFlags.Controls.Add((Control)this.ledRxReady);
			this.gBoxIrqFlags.Controls.Add((Control)this.ledPayloadReady);
			this.gBoxIrqFlags.Controls.Add((Control)this.ledTxReady);
			this.gBoxIrqFlags.Controls.Add((Control)this.ledPacketSent);
			this.gBoxIrqFlags.Controls.Add((Control)this.label17);
			this.gBoxIrqFlags.Controls.Add((Control)this.label31);
			this.gBoxIrqFlags.Controls.Add((Control)this.ledPllLock);
			this.gBoxIrqFlags.Controls.Add((Control)this.label30);
			this.gBoxIrqFlags.Controls.Add((Control)this.label18);
			this.gBoxIrqFlags.Controls.Add((Control)this.label29);
			this.gBoxIrqFlags.Controls.Add((Control)this.label19);
			this.gBoxIrqFlags.Controls.Add((Control)this.label28);
			this.gBoxIrqFlags.Controls.Add((Control)this.ledModeReady);
			this.gBoxIrqFlags.Controls.Add((Control)this.ledFifoOverrun);
			this.gBoxIrqFlags.Controls.Add((Control)this.ledRssi);
			this.gBoxIrqFlags.Controls.Add((Control)this.ledFifoLevel);
			this.gBoxIrqFlags.Controls.Add((Control)this.ledTimeout);
			this.gBoxIrqFlags.Controls.Add((Control)this.label27);
			this.gBoxIrqFlags.Controls.Add((Control)this.label20);
			this.gBoxIrqFlags.Controls.Add((Control)this.ledFifoEmpty);
			this.gBoxIrqFlags.Controls.Add((Control)this.label21);
			this.gBoxIrqFlags.Controls.Add((Control)this.label26);
			this.gBoxIrqFlags.Controls.Add((Control)this.ledPreamble);
			this.gBoxIrqFlags.Controls.Add((Control)this.label25);
			this.gBoxIrqFlags.Controls.Add((Control)this.label22);
			this.gBoxIrqFlags.Controls.Add((Control)this.label24);
			this.gBoxIrqFlags.Controls.Add((Control)this.label23);
			this.gBoxIrqFlags.Controls.Add((Control)this.ledFifoFull);
			this.gBoxIrqFlags.Controls.Add((Control)this.ledSyncAddressMatch);
			this.gBoxIrqFlags.Location = new Point(816, 25);
			this.gBoxIrqFlags.Name = "gBoxIrqFlags";
			this.gBoxIrqFlags.Size = new Size(189, 380);
			this.gBoxIrqFlags.TabIndex = 1;
			this.gBoxIrqFlags.TabStop = false;
			this.gBoxIrqFlags.Text = "Irq flags";
			this.gBoxIrqFlags.MouseEnter += new EventHandler(this.control_MouseEnter);
			this.gBoxIrqFlags.MouseLeave += new EventHandler(this.control_MouseLeave);
			this.ledLowBat.BackColor = Color.Transparent;
			this.ledLowBat.LedColor = Color.Green;
			this.ledLowBat.LedSize = new Size(11, 11);
			this.ledLowBat.Location = new Point(34, 352);
			this.ledLowBat.Name = "ledLowBat";
			this.ledLowBat.Size = new Size(15, 15);
			this.ledLowBat.TabIndex = 30;
			this.ledLowBat.Text = "led1";
			this.ledLowBat.Click += new EventHandler(this.ledLowBat_Click);
			this.lbModeReady.AutoSize = true;
			this.lbModeReady.Location = new Point(55, 20);
			this.lbModeReady.Name = "lbModeReady";
			this.lbModeReady.Size = new Size(65, 13);
			this.lbModeReady.TabIndex = 1;
			this.lbModeReady.Text = "ModeReady";
			this.ledCrcOk.BackColor = Color.Transparent;
			this.ledCrcOk.LedColor = Color.Green;
			this.ledCrcOk.LedSize = new Size(11, 11);
			this.ledCrcOk.Location = new Point(34, 331);
			this.ledCrcOk.Name = "ledCrcOk";
			this.ledCrcOk.Size = new Size(15, 15);
			this.ledCrcOk.TabIndex = 28;
			this.ledCrcOk.Text = "led1";
			this.ledRxReady.BackColor = Color.Transparent;
			this.ledRxReady.LedColor = Color.Green;
			this.ledRxReady.LedSize = new Size(11, 11);
			this.ledRxReady.Location = new Point(34, 40);
			this.ledRxReady.Name = "ledRxReady";
			this.ledRxReady.Size = new Size(15, 15);
			this.ledRxReady.TabIndex = 2;
			this.ledRxReady.Text = "led1";
			this.ledPayloadReady.BackColor = Color.Transparent;
			this.ledPayloadReady.LedColor = Color.Green;
			this.ledPayloadReady.LedSize = new Size(11, 11);
			this.ledPayloadReady.Location = new Point(34, 310);
			this.ledPayloadReady.Name = "ledPayloadReady";
			this.ledPayloadReady.Size = new Size(15, 15);
			this.ledPayloadReady.TabIndex = 26;
			this.ledPayloadReady.Text = "led1";
			this.ledTxReady.BackColor = Color.Transparent;
			this.ledTxReady.LedColor = Color.Green;
			this.ledTxReady.LedSize = new Size(11, 11);
			this.ledTxReady.Location = new Point(34, 61);
			this.ledTxReady.Name = "ledTxReady";
			this.ledTxReady.Size = new Size(15, 15);
			this.ledTxReady.TabIndex = 4;
			this.ledTxReady.Text = "led1";
			this.ledPacketSent.BackColor = Color.Transparent;
			this.ledPacketSent.LedColor = Color.Green;
			this.ledPacketSent.LedSize = new Size(11, 11);
			this.ledPacketSent.Location = new Point(34, 289);
			this.ledPacketSent.Margin = new Padding(3, 6, 3, 3);
			this.ledPacketSent.Name = "ledPacketSent";
			this.ledPacketSent.Size = new Size(15, 15);
			this.ledPacketSent.TabIndex = 24;
			this.ledPacketSent.Text = "led1";
			this.label17.AutoSize = true;
			this.label17.Location = new Point(55, 83);
			this.label17.Name = "label17";
			this.label17.Size = new Size(42, 13);
			this.label17.TabIndex = 7;
			this.label17.Text = "PllLock";
			this.label31.AutoSize = true;
			this.label31.Location = new Point(55, 290);
			this.label31.Name = "label31";
			this.label31.Size = new Size(63, 13);
			this.label31.TabIndex = 25;
			this.label31.Text = "PacketSent";
			this.ledPllLock.BackColor = Color.Transparent;
			this.ledPllLock.LedColor = Color.Green;
			this.ledPllLock.LedSize = new Size(11, 11);
			this.ledPllLock.Location = new Point(34, 82);
			this.ledPllLock.Margin = new Padding(3, 3, 3, 6);
			this.ledPllLock.Name = "ledPllLock";
			this.ledPllLock.Size = new Size(15, 15);
			this.ledPllLock.TabIndex = 6;
			this.ledPllLock.Text = "led1";
			this.label30.AutoSize = true;
			this.label30.Location = new Point(55, 311);
			this.label30.Name = "label30";
			this.label30.Size = new Size(76, 13);
			this.label30.TabIndex = 27;
			this.label30.Text = "PayloadReady";
			this.label18.AutoSize = true;
			this.label18.Location = new Point(55, 62);
			this.label18.Name = "label18";
			this.label18.Size = new Size(50, 13);
			this.label18.TabIndex = 5;
			this.label18.Text = "TxReady";
			this.label29.AutoSize = true;
			this.label29.Location = new Point(55, 332);
			this.label29.Name = "label29";
			this.label29.Size = new Size(37, 13);
			this.label29.TabIndex = 29;
			this.label29.Text = "CrcOk";
			this.label19.AutoSize = true;
			this.label19.Location = new Point(55, 41);
			this.label19.Name = "label19";
			this.label19.Size = new Size(51, 13);
			this.label19.TabIndex = 3;
			this.label19.Text = "RxReady";
			this.label28.AutoSize = true;
			this.label28.Location = new Point(55, 353);
			this.label28.Name = "label28";
			this.label28.Size = new Size(43, 13);
			this.label28.TabIndex = 31;
			this.label28.Text = "LowBat";
			this.ledModeReady.BackColor = Color.Transparent;
			this.ledModeReady.LedColor = Color.Green;
			this.ledModeReady.LedSize = new Size(11, 11);
			this.ledModeReady.Location = new Point(34, 19);
			this.ledModeReady.Name = "ledModeReady";
			this.ledModeReady.Size = new Size(15, 15);
			this.ledModeReady.TabIndex = 0;
			this.ledModeReady.Text = "Mode Ready";
			this.ledFifoOverrun.BackColor = Color.Transparent;
			this.ledFifoOverrun.LedColor = Color.Green;
			this.ledFifoOverrun.LedSize = new Size(11, 11);
			this.ledFifoOverrun.Location = new Point(34, 262);
			this.ledFifoOverrun.Margin = new Padding(3, 3, 3, 6);
			this.ledFifoOverrun.Name = "ledFifoOverrun";
			this.ledFifoOverrun.Size = new Size(15, 15);
			this.ledFifoOverrun.TabIndex = 22;
			this.ledFifoOverrun.Text = "led1";
			this.ledFifoOverrun.Click += new EventHandler(this.ledFifoOverrun_Click);
			this.ledRssi.BackColor = Color.Transparent;
			this.ledRssi.LedColor = Color.Green;
			this.ledRssi.LedSize = new Size(11, 11);
			this.ledRssi.Location = new Point(34, 109);
			this.ledRssi.Margin = new Padding(3, 6, 3, 3);
			this.ledRssi.Name = "ledRssi";
			this.ledRssi.Size = new Size(15, 15);
			this.ledRssi.TabIndex = 8;
			this.ledRssi.Text = "led1";
			this.ledRssi.Click += new EventHandler(this.ledRssi_Click);
			this.ledFifoLevel.BackColor = Color.Transparent;
			this.ledFifoLevel.LedColor = Color.Green;
			this.ledFifoLevel.LedSize = new Size(11, 11);
			this.ledFifoLevel.Location = new Point(34, 241);
			this.ledFifoLevel.Name = "ledFifoLevel";
			this.ledFifoLevel.Size = new Size(15, 15);
			this.ledFifoLevel.TabIndex = 20;
			this.ledFifoLevel.Text = "led1";
			this.ledTimeout.BackColor = Color.Transparent;
			this.ledTimeout.LedColor = Color.Green;
			this.ledTimeout.LedSize = new Size(11, 11);
			this.ledTimeout.Location = new Point(34, 130);
			this.ledTimeout.Name = "ledTimeout";
			this.ledTimeout.Size = new Size(15, 15);
			this.ledTimeout.TabIndex = 10;
			this.ledTimeout.Text = "led1";
			this.label27.AutoSize = true;
			this.label27.Location = new Point(55, 200);
			this.label27.Name = "label27";
			this.label27.Size = new Size(40, 13);
			this.label27.TabIndex = 17;
			this.label27.Text = "FifoFull";
			this.label20.AutoSize = true;
			this.label20.Location = new Point(55, 173);
			this.label20.Name = "label20";
			this.label20.Size = new Size(99, 13);
			this.label20.TabIndex = 15;
			this.label20.Text = "SyncAddressMatch";
			this.ledFifoEmpty.BackColor = Color.Transparent;
			this.ledFifoEmpty.LedColor = Color.Green;
			this.ledFifoEmpty.LedSize = new Size(11, 11);
			this.ledFifoEmpty.Location = new Point(34, 220);
			this.ledFifoEmpty.Name = "ledFifoEmpty";
			this.ledFifoEmpty.Size = new Size(15, 15);
			this.ledFifoEmpty.TabIndex = 18;
			this.ledFifoEmpty.Text = "led1";
			this.label21.AutoSize = true;
			this.label21.Location = new Point(55, 152);
			this.label21.Name = "label21";
			this.label21.Size = new Size(51, 13);
			this.label21.TabIndex = 13;
			this.label21.Text = "Preamble";
			this.label26.AutoSize = true;
			this.label26.Location = new Point(55, 221);
			this.label26.Name = "label26";
			this.label26.Size = new Size(53, 13);
			this.label26.TabIndex = 19;
			this.label26.Text = "FifoEmpty";
			this.ledPreamble.BackColor = Color.Transparent;
			this.ledPreamble.LedColor = Color.Green;
			this.ledPreamble.LedSize = new Size(11, 11);
			this.ledPreamble.Location = new Point(34, 151);
			this.ledPreamble.Name = "ledPreamble";
			this.ledPreamble.Size = new Size(15, 15);
			this.ledPreamble.TabIndex = 12;
			this.ledPreamble.Text = "led1";
			this.ledPreamble.Click += new EventHandler(this.ledPreamble_Click);
			this.label25.AutoSize = true;
			this.label25.Location = new Point(55, 242);
			this.label25.Name = "label25";
			this.label25.Size = new Size(50, 13);
			this.label25.TabIndex = 21;
			this.label25.Text = "FifoLevel";
			this.label22.AutoSize = true;
			this.label22.Location = new Point(55, 131);
			this.label22.Name = "label22";
			this.label22.Size = new Size(45, 13);
			this.label22.TabIndex = 11;
			this.label22.Text = "Timeout";
			this.label24.AutoSize = true;
			this.label24.Location = new Point(55, 263);
			this.label24.Name = "label24";
			this.label24.Size = new Size(62, 13);
			this.label24.TabIndex = 23;
			this.label24.Text = "FifoOverrun";
			this.label23.AutoSize = true;
			this.label23.Location = new Point(55, 110);
			this.label23.Name = "label23";
			this.label23.Size = new Size(27, 13);
			this.label23.TabIndex = 9;
			this.label23.Text = "Rssi";
			this.ledFifoFull.BackColor = Color.Transparent;
			this.ledFifoFull.LedColor = Color.Green;
			this.ledFifoFull.LedSize = new Size(11, 11);
			this.ledFifoFull.Location = new Point(34, 199);
			this.ledFifoFull.Margin = new Padding(3, 6, 3, 3);
			this.ledFifoFull.Name = "ledFifoFull";
			this.ledFifoFull.Size = new Size(15, 15);
			this.ledFifoFull.TabIndex = 16;
			this.ledFifoFull.Text = "led1";
			this.ledSyncAddressMatch.BackColor = Color.Transparent;
			this.ledSyncAddressMatch.LedColor = Color.Green;
			this.ledSyncAddressMatch.LedSize = new Size(11, 11);
			this.ledSyncAddressMatch.Location = new Point(34, 172);
			this.ledSyncAddressMatch.Margin = new Padding(3, 3, 3, 6);
			this.ledSyncAddressMatch.Name = "ledSyncAddressMatch";
			this.ledSyncAddressMatch.Size = new Size(15, 15);
			this.ledSyncAddressMatch.TabIndex = 14;
			this.ledSyncAddressMatch.Text = "led1";
			this.ledSyncAddressMatch.Click += new EventHandler(this.ledSyncAddressMatch_Click);
			this.ledFifoNotEmpty.BackColor = Color.Transparent;
			this.ledFifoNotEmpty.LedColor = Color.Green;
			this.ledFifoNotEmpty.LedSize = new Size(11, 11);
			this.ledFifoNotEmpty.Location = new Point(34, 220);
			this.ledFifoNotEmpty.Name = "ledFifoNotEmpty";
			this.ledFifoNotEmpty.Size = new Size(15, 15);
			this.ledFifoNotEmpty.TabIndex = 18;
			this.ledFifoNotEmpty.Text = "led1";
			this.AutoScaleDimensions = new SizeF(6f, 13f);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.Controls.Add((Control)this.gBoxOperatingMode);
			this.Controls.Add((Control)this.tabControl1);
			this.Controls.Add((Control)this.gBoxIrqFlags);
			this.Name = "DeviceViewControl";
			this.Size = new Size(1008, 525);
			this.tabControl1.ResumeLayout(false);
			this.tabCommon.ResumeLayout(false);
			this.tabTransmitter.ResumeLayout(false);
			this.tabReceiver.ResumeLayout(false);
			this.tabIrqMap.ResumeLayout(false);
			this.tabPacketHandler.ResumeLayout(false);
			this.tabSequencer.ResumeLayout(false);
			this.tabTemperature.ResumeLayout(false);
			this.gBoxOperatingMode.ResumeLayout(false);
			this.gBoxOperatingMode.PerformLayout();
			this.gBoxIrqFlags.ResumeLayout(false);
			this.gBoxIrqFlags.PerformLayout();
			this.ResumeLayout(false);
		}

		private void LoadTestPage(SX1276 device)
		{
			try
			{
				if (!File.Exists(Application.StartupPath + "\\SemtechLib.Devices.SX1276.Test.dll"))
					return;
				Type type = Assembly.LoadFile(Application.StartupPath + "\\SemtechLib.Devices.SX1276.Test.dll").GetType("SemtechLib.Devices.SX1276.Test.Controls.TestTabPage");
				object instance = Activator.CreateInstance(type);
				type.InvokeMember("SuspendLayout", BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod, (Binder)null, instance, (object[])null);
				this.SuspendLayout();
				type.GetProperty("Location").SetValue(instance, (object)new Point(4, 22), (object[])null);
				type.GetProperty("Name").SetValue(instance, (object)"tabTest", (object[])null);
				type.GetProperty("Size").SetValue(instance, (object)new Size(799, 493), (object[])null);
				type.GetProperty("TabIndex").SetValue(instance, (object)6, (object[])null);
				type.GetProperty("Text").SetValue(instance, (object)"R&D Tests", (object[])null);
				type.GetProperty("UseVisualStyleBackColor").SetValue(instance, (object)true, (object[])null);
				type.GetProperty("SX1276").SetValue(instance, (object)device, (object[])null);
				this.tabControl1.Controls.Add((Control)instance);
				type.InvokeMember("ResumeLayout", BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod, (Binder)null, instance, new object[1]
        {
          (object) false
        });
				this.ResumeLayout(false);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
		}

		private void OnDevicePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "FrequencyXo":
					this.commonViewControl1.FrequencyXo = this.device.FrequencyXo;
					this.receiverViewControl1.FrequencyXo = this.device.FrequencyXo;
					this.irqMapViewControl1.FrequencyXo = this.device.FrequencyXo;
					break;
				case "FrequencyStep":
					this.commonViewControl1.FrequencyStep = this.device.FrequencyStep;
					break;
				case "SpectrumOn":
					if (this.device.SpectrumOn)
					{
						this.DisableControls();
						this.packetHandlerView1.Enabled = false;
						break;
					}
					this.EnableControls();
					this.packetHandlerView1.Enabled = true;
					break;
				case "ModulationType":
					this.commonViewControl1.ModulationType = this.device.ModulationType;
					this.receiverViewControl1.ModulationType = this.device.ModulationType;
					break;
				case "ModulationShaping":
					this.commonViewControl1.ModulationShaping = this.device.ModulationShaping;
					break;
				case "Mode":
					this.rBtnSleep.CheckedChanged -= new EventHandler(this.rBtnOperatingMode_CheckedChanged);
					this.rBtnStandby.CheckedChanged -= new EventHandler(this.rBtnOperatingMode_CheckedChanged);
					this.rBtnSynthesizerRx.CheckedChanged -= new EventHandler(this.rBtnOperatingMode_CheckedChanged);
					this.rBtnSynthesizerTx.CheckedChanged -= new EventHandler(this.rBtnOperatingMode_CheckedChanged);
					this.rBtnReceiver.CheckedChanged -= new EventHandler(this.rBtnOperatingMode_CheckedChanged);
					this.rBtnTransmitter.CheckedChanged -= new EventHandler(this.rBtnOperatingMode_CheckedChanged);
					switch (this.device.Mode)
					{
						case OperatingModeEnum.Sleep:
							this.rBtnSleep.Checked = true;
							break;
						case OperatingModeEnum.Stdby:
							this.rBtnStandby.Checked = true;
							break;
						case OperatingModeEnum.FsTx:
							this.rBtnSynthesizerTx.Checked = true;
							break;
						case OperatingModeEnum.Tx:
							this.rBtnTransmitter.Checked = true;
							break;
						case OperatingModeEnum.FsRx:
							this.rBtnSynthesizerRx.Checked = true;
							break;
						case OperatingModeEnum.Rx:
							this.rBtnReceiver.Checked = true;
							break;
					}
					this.rBtnSleep.CheckedChanged += new EventHandler(this.rBtnOperatingMode_CheckedChanged);
					this.rBtnStandby.CheckedChanged += new EventHandler(this.rBtnOperatingMode_CheckedChanged);
					this.rBtnSynthesizerRx.CheckedChanged += new EventHandler(this.rBtnOperatingMode_CheckedChanged);
					this.rBtnSynthesizerTx.CheckedChanged += new EventHandler(this.rBtnOperatingMode_CheckedChanged);
					this.rBtnReceiver.CheckedChanged += new EventHandler(this.rBtnOperatingMode_CheckedChanged);
					this.rBtnTransmitter.CheckedChanged += new EventHandler(this.rBtnOperatingMode_CheckedChanged);
					this.irqMapViewControl1.Mode = this.device.Mode;
					this.packetHandlerView1.Mode = this.device.Mode;
					this.temperatureViewControl1.Mode = this.device.Mode;
					break;
				case "Bitrate":
					this.commonViewControl1.Bitrate = this.device.Bitrate;
					this.receiverViewControl1.Bitrate = this.device.Bitrate;
					this.packetHandlerView1.Bitrate = this.device.Bitrate;
					break;
				case "BitrateFrac":
					this.commonViewControl1.BitrateFrac = this.device.BitrateFrac;
					break;
				case "Fdev":
					this.commonViewControl1.Fdev = this.device.Fdev;
					break;
				case "Band":
					this.commonViewControl1.Band = this.device.Band;
					break;
				case "LowFrequencyModeOn":
					this.commonViewControl1.LowFrequencyModeOn = this.device.LowFrequencyModeOn;
					this.receiverViewControl1.LowFrequencyModeOn = this.device.LowFrequencyModeOn;
					break;
				case "FrequencyRf":
					this.commonViewControl1.FrequencyRf = this.device.FrequencyRf;
					break;
				case "PaSelect":
					this.transmitterViewControl1.PaSelect = this.device.PaSelect;
					break;
				case "MaxOutputPower":
					this.transmitterViewControl1.MaxOutputPower = this.device.MaxOutputPower;
					break;
				case "OutputPower":
					this.transmitterViewControl1.OutputPower = this.device.OutputPower;
					break;
				case "ForceTxBandLowFrequencyOn":
					this.commonViewControl1.ForceTxBandLowFrequencyOn = this.device.ForceTxBandLowFrequencyOn;
					break;
				case "PaRamp":
					this.transmitterViewControl1.PaRamp = this.device.PaRamp;
					break;
				case "OcpOn":
					this.transmitterViewControl1.OcpOn = this.device.OcpOn;
					break;
				case "OcpTrim":
					this.transmitterViewControl1.OcpTrim = this.device.OcpTrim;
					break;
				case "Pa20dBm":
					this.transmitterViewControl1.Pa20dBm = this.device.Pa20dBm;
					break;
				case "RxBwMin":
					this.receiverViewControl1.RxBwMin = this.device.RxBwMin;
					break;
				case "RxBwMax":
					this.receiverViewControl1.RxBwMax = this.device.RxBwMax;
					break;
				case "AfcRxBwMin":
					this.receiverViewControl1.AfcRxBwMin = this.device.AfcRxBwMin;
					break;
				case "AfcRxBwMax":
					this.receiverViewControl1.AfcRxBwMax = this.device.AfcRxBwMax;
					break;
				case "LnaGain":
					this.receiverViewControl1.LnaGain = this.device.LnaGain;
					break;
				case "ForceRxBandLowFrequencyOn":
					this.commonViewControl1.ForceRxBandLowFrequencyOn = this.device.ForceRxBandLowFrequencyOn;
					break;
				case "LnaBoost":
					this.receiverViewControl1.LnaBoost = this.device.LnaBoost;
					break;
				case "RestartRxOnCollision":
					this.receiverViewControl1.RestartRxOnCollision = this.device.RestartRxOnCollision;
					break;
				case "AfcAutoOn":
					this.receiverViewControl1.AfcAutoOn = this.device.AfcAutoOn;
					break;
				case "AgcAutoOn":
					this.receiverViewControl1.AgcAutoOn = this.device.AgcAutoOn;
					break;
				case "RxTrigger":
					this.receiverViewControl1.RxTrigger = this.device.RxTrigger;
					break;
				case "RssiOffset":
					this.receiverViewControl1.RssiOffset = this.device.RssiOffset;
					break;
				case "RssiSmoothing":
					this.receiverViewControl1.RssiSmoothing = this.device.RssiSmoothing;
					break;
				case "RssiCollisionThreshold":
					this.receiverViewControl1.RssiCollisionThreshold = this.device.RssiCollisionThreshold;
					break;
				case "RssiThreshold":
					this.receiverViewControl1.RssiThreshold = this.device.RssiThreshold;
					break;
				case "RssiValue":
					this.receiverViewControl1.RssiValue = this.device.RssiValue;
					break;
				case "RxBw":
					this.receiverViewControl1.RxBw = this.device.RxBw;
					break;
				case "AfcRxBw":
					this.receiverViewControl1.AfcRxBw = this.device.AfcRxBw;
					break;
				case "BitSyncOn":
					this.receiverViewControl1.BitSyncOn = this.device.BitSyncOn;
					this.irqMapViewControl1.BitSyncOn = this.device.BitSyncOn;
					this.packetHandlerView1.BitSyncOn = this.device.BitSyncOn;
					break;
				case "OokThreshType":
					this.receiverViewControl1.OokThreshType = this.device.OokThreshType;
					break;
				case "OokPeakThreshStep":
					this.receiverViewControl1.OokPeakThreshStep = this.device.OokPeakThreshStep;
					break;
				case "OokFixedThreshold":
					this.receiverViewControl1.OokFixedThreshold = this.device.OokFixedThreshold;
					break;
				case "OokPeakThreshDec":
					this.receiverViewControl1.OokPeakThreshDec = this.device.OokPeakThreshDec;
					break;
				case "OokAverageOffset":
					this.receiverViewControl1.OokAverageOffset = this.device.OokAverageOffset;
					break;
				case "OokAverageThreshFilt":
					this.receiverViewControl1.OokAverageThreshFilt = this.device.OokAverageThreshFilt;
					break;
				case "AfcAutoClearOn":
					this.receiverViewControl1.AfcAutoClearOn = this.device.AfcAutoClearOn;
					break;
				case "AfcValue":
					this.receiverViewControl1.AfcValue = this.device.AfcValue;
					break;
				case "FeiValue":
					this.receiverViewControl1.FeiValue = this.device.FeiValue;
					break;
				case "PreambleDetectorOn":
					this.receiverViewControl1.PreambleDetectorOn = this.device.PreambleDetectorOn;
					break;
				case "PreambleDetectorSize":
					this.receiverViewControl1.PreambleDetectorSize = this.device.PreambleDetectorSize;
					break;
				case "PreambleDetectorTol":
					this.receiverViewControl1.PreambleDetectorTol = this.device.PreambleDetectorTol;
					break;
				case "TimeoutRxRssi":
					this.receiverViewControl1.TimeoutRxRssi = this.device.TimeoutRxRssi;
					break;
				case "TimeoutRxPreamble":
					this.receiverViewControl1.TimeoutRxPreamble = this.device.TimeoutRxPreamble;
					break;
				case "TimeoutSignalSync":
					this.receiverViewControl1.TimeoutSignalSync = this.device.TimeoutSignalSync;
					break;
				case "InterPacketRxDelay":
					this.receiverViewControl1.InterPacketRxDelay = this.device.InterPacketRxDelay;
					break;
				case "ClockOut":
					this.irqMapViewControl1.ClockOut = this.device.ClockOut;
					break;
				case "Packet":
					this.packetHandlerView1.DataMode = this.device.Packet.DataMode;
					this.packetHandlerView1.PreambleSize = this.device.Packet.PreambleSize;
					this.packetHandlerView1.AutoRestartRxOn = this.device.Packet.AutoRestartRxOn;
					this.packetHandlerView1.PreamblePolarity = this.device.Packet.PreamblePolarity;
					this.packetHandlerView1.SyncOn = this.device.Packet.SyncOn;
					this.packetHandlerView1.FifoFillCondition = this.device.Packet.FifoFillCondition;
					this.packetHandlerView1.SyncSize = this.device.Packet.SyncSize;
					this.packetHandlerView1.SyncValue = this.device.Packet.SyncValue;
					this.packetHandlerView1.PacketFormat = this.device.Packet.PacketFormat;
					this.packetHandlerView1.DcFree = this.device.Packet.DcFree;
					this.packetHandlerView1.CrcOn = this.device.Packet.CrcOn;
					this.packetHandlerView1.CrcAutoClearOff = this.device.Packet.CrcAutoClearOff;
					this.packetHandlerView1.AddressFiltering = this.device.Packet.AddressFiltering;
					this.packetHandlerView1.CrcIbmOn = this.device.Packet.CrcIbmOn;
					this.packetHandlerView1.IoHomeOn = this.device.Packet.IoHomeOn;
					this.packetHandlerView1.IoHomePwrFrameOn = this.device.Packet.IoHomePwrFrameOn;
					this.packetHandlerView1.BeaconOn = this.device.Packet.BeaconOn;
					this.packetHandlerView1.PayloadLength = this.device.Packet.PayloadLength;
					this.packetHandlerView1.NodeAddress = this.device.Packet.NodeAddress;
					this.packetHandlerView1.BroadcastAddress = this.device.Packet.BroadcastAddress;
					this.packetHandlerView1.TxStartCondition = this.device.Packet.TxStartCondition;
					this.packetHandlerView1.FifoThreshold = this.device.Packet.FifoThreshold;
					this.packetHandlerView1.MessageLength = (int)this.device.Packet.MessageLength;
					this.packetHandlerView1.Message = this.device.Packet.Message;
					this.packetHandlerView1.Crc = this.device.Packet.Crc;
					break;
				case "PreambleSize":
					this.packetHandlerView1.PreambleSize = this.device.Packet.PreambleSize;
					break;
				case "AutoRestartRxOn":
					this.packetHandlerView1.AutoRestartRxOn = this.device.Packet.AutoRestartRxOn;
					break;
				case "PreamblePolarity":
					this.packetHandlerView1.PreamblePolarity = this.device.Packet.PreamblePolarity;
					break;
				case "SyncOn":
					this.packetHandlerView1.SyncOn = this.device.Packet.SyncOn;
					break;
				case "FifoFillCondition":
					this.packetHandlerView1.FifoFillCondition = this.device.Packet.FifoFillCondition;
					break;
				case "SyncSize":
					this.packetHandlerView1.SyncSize = this.device.Packet.SyncSize;
					break;
				case "SyncValue":
					this.packetHandlerView1.SyncValue = this.device.Packet.SyncValue;
					break;
				case "PacketFormat":
					this.packetHandlerView1.PacketFormat = this.device.Packet.PacketFormat;
					break;
				case "DcFree":
					this.packetHandlerView1.DcFree = this.device.Packet.DcFree;
					break;
				case "CrcOn":
					this.packetHandlerView1.CrcOn = this.device.Packet.CrcOn;
					break;
				case "CrcAutoClearOff":
					this.packetHandlerView1.CrcAutoClearOff = this.device.Packet.CrcAutoClearOff;
					break;
				case "AddressFiltering":
					this.packetHandlerView1.AddressFiltering = this.device.Packet.AddressFiltering;
					break;
				case "CrcIbmOn":
					this.packetHandlerView1.CrcIbmOn = this.device.Packet.CrcIbmOn;
					break;
				case "DataMode":
					this.packetHandlerView1.DataMode = this.device.Packet.DataMode;
					this.receiverViewControl1.DataMode = this.device.Packet.DataMode;
					this.irqMapViewControl1.DataMode = this.device.Packet.DataMode;
					break;
				case "IoHomeOn":
					this.packetHandlerView1.IoHomeOn = this.device.Packet.IoHomeOn;
					break;
				case "IoHomePwrFrameOn":
					this.packetHandlerView1.IoHomePwrFrameOn = this.device.Packet.IoHomePwrFrameOn;
					break;
				case "BeaconOn":
					this.packetHandlerView1.BeaconOn = this.device.Packet.BeaconOn;
					break;
				case "PayloadLength":
					this.packetHandlerView1.PayloadLength = this.device.Packet.PayloadLength;
					break;
				case "NodeAddress":
					this.packetHandlerView1.NodeAddress = this.device.Packet.NodeAddress;
					break;
				case "NodeAddressRx":
					this.packetHandlerView1.NodeAddressRx = this.device.Packet.NodeAddressRx;
					break;
				case "BroadcastAddress":
					this.packetHandlerView1.BroadcastAddress = this.device.Packet.BroadcastAddress;
					break;
				case "TxStartCondition":
					this.packetHandlerView1.TxStartCondition = this.device.Packet.TxStartCondition;
					break;
				case "FifoThreshold":
					this.packetHandlerView1.FifoThreshold = this.device.Packet.FifoThreshold;
					break;
				case "MessageLength":
					this.packetHandlerView1.MessageLength = (int)this.device.Packet.MessageLength;
					break;
				case "Message":
					this.packetHandlerView1.Message = this.device.Packet.Message;
					break;
				case "Crc":
					this.packetHandlerView1.Crc = this.device.Packet.Crc;
					break;
				case "LogEnabled":
					this.packetHandlerView1.LogEnabled = this.device.Packet.LogEnabled;
					break;
				case "IdleMode":
					this.sequencerViewControl1.IdleMode = this.device.IdleMode;
					break;
				case "FromStart":
					this.sequencerViewControl1.FromStart = this.device.FromStart;
					break;
				case "LowPowerSelection":
					this.sequencerViewControl1.LowPowerSelection = this.device.LowPowerSelection;
					break;
				case "FromIdle":
					this.sequencerViewControl1.FromIdle = this.device.FromIdle;
					break;
				case "FromTransmit":
					this.sequencerViewControl1.FromTransmit = this.device.FromTransmit;
					break;
				case "FromReceive":
					this.sequencerViewControl1.FromReceive = this.device.FromReceive;
					break;
				case "FromRxTimeout":
					this.sequencerViewControl1.FromRxTimeout = this.device.FromRxTimeout;
					break;
				case "FromPacketReceived":
					this.sequencerViewControl1.FromPacketReceived = this.device.FromPacketReceived;
					break;
				case "Timer1Resolution":
					this.sequencerViewControl1.Timer1Resolution = this.device.Timer1Resolution;
					break;
				case "Timer2Resolution":
					this.sequencerViewControl1.Timer2Resolution = this.device.Timer2Resolution;
					break;
				case "Timer1Coef":
					this.sequencerViewControl1.Timer1Coef = this.device.Timer1Coef;
					break;
				case "Timer2Coef":
					this.sequencerViewControl1.Timer2Coef = this.device.Timer2Coef;
					break;
				case "AutoImageCalOn":
					this.temperatureViewControl1.AutoImageCalOn = this.device.AutoImageCalOn;
					break;
				case "ImageCalRunning":
					this.temperatureViewControl1.ImageCalRunning = this.device.ImageCalRunning;
					break;
				case "TempChange":
					this.temperatureViewControl1.TempChange = this.device.TempChange;
					break;
				case "TempThreshold":
					this.temperatureViewControl1.TempThreshold = this.device.TempThreshold;
					break;
				case "TempMonitorOff":
					this.temperatureViewControl1.TempMonitorOff = this.device.TempMonitorOff;
					break;
				case "TempValue":
					this.temperatureViewControl1.TempValue = this.device.TempValue;
					break;
				case "TempValueRoom":
					this.temperatureViewControl1.TempValueRoom = this.device.TempValueRoom;
					break;
				case "TempCalDone":
					this.temperatureViewControl1.TempCalDone = this.device.TempCalDone;
					break;
				case "TempMeasRunning":
					this.temperatureViewControl1.TempMeasRunning = this.device.TempMeasRunning;
					break;
				case "LowBatOn":
					this.commonViewControl1.LowBatOn = this.device.LowBatOn;
					break;
				case "LowBatTrim":
					this.commonViewControl1.LowBatTrim = this.device.LowBatTrim;
					break;
				case "ModeReady":
					this.ledModeReady.Checked = this.device.ModeReady;
					break;
				case "RxReady":
					this.ledRxReady.Checked = this.device.RxReady;
					break;
				case "TxReady":
					this.ledTxReady.Checked = this.device.TxReady;
					break;
				case "PllLock":
					this.ledPllLock.Checked = this.device.PllLock;
					break;
				case "Rssi":
					this.ledRssi.Checked = this.device.Rssi;
					break;
				case "Timeout":
					this.ledTimeout.Checked = this.device.Timeout;
					break;
				case "PreambleDetect":
					this.ledPreamble.Checked = this.device.PreambleDetect;
					break;
				case "SyncAddressMatch":
					this.ledSyncAddressMatch.Checked = this.device.SyncAddressMatch;
					break;
				case "FifoFull":
					this.ledFifoFull.Checked = this.device.FifoFull;
					break;
				case "FifoEmpty":
					this.ledFifoEmpty.Checked = this.device.FifoEmpty;
					break;
				case "FifoLevel":
					this.ledFifoLevel.Checked = this.device.FifoLevel;
					break;
				case "FifoOverrun":
					this.ledFifoOverrun.Checked = this.device.FifoOverrun;
					break;
				case "PacketSent":
					this.ledPacketSent.Checked = this.device.PacketSent;
					break;
				case "PayloadReady":
					this.ledPayloadReady.Checked = this.device.PayloadReady;
					break;
				case "CrcOk":
					this.ledCrcOk.Checked = this.device.CrcOk;
					break;
				case "LowBat":
					this.ledLowBat.Checked = this.device.LowBat;
					break;
				case "Dio0Mapping":
					this.irqMapViewControl1.Dio0Mapping = this.device.Dio0Mapping;
					break;
				case "Dio1Mapping":
					this.irqMapViewControl1.Dio1Mapping = this.device.Dio1Mapping;
					break;
				case "Dio2Mapping":
					this.irqMapViewControl1.Dio2Mapping = this.device.Dio2Mapping;
					break;
				case "Dio3Mapping":
					this.irqMapViewControl1.Dio3Mapping = this.device.Dio3Mapping;
					break;
				case "Dio4Mapping":
					this.irqMapViewControl1.Dio4Mapping = this.device.Dio4Mapping;
					break;
				case "Dio5Mapping":
					this.irqMapViewControl1.Dio5Mapping = this.device.Dio5Mapping;
					break;
				case "MapPreambleDetect":
					this.irqMapViewControl1.MapPreambleDetect = this.device.MapPreambleDetect;
					break;
				case "AgcReference":
					this.receiverViewControl1.AgcReference = this.device.AgcReference;
					break;
				case "AgcThresh1":
					this.receiverViewControl1.AgcThresh1 = this.device.AgcThresh1;
					break;
				case "AgcThresh2":
					this.receiverViewControl1.AgcThresh2 = this.device.AgcThresh2;
					break;
				case "AgcThresh3":
					this.receiverViewControl1.AgcThresh3 = this.device.AgcThresh3;
					break;
				case "AgcThresh4":
					this.receiverViewControl1.AgcThresh4 = this.device.AgcThresh4;
					break;
				case "AgcThresh5":
					this.receiverViewControl1.AgcThresh5 = this.device.AgcThresh5;
					break;
				case "AgcReferenceLevel":
					this.receiverViewControl1.AgcReferenceLevel = (int)this.device.AgcReferenceLevel;
					break;
				case "AgcStep1":
					this.receiverViewControl1.AgcStep1 = this.device.AgcStep1;
					break;
				case "AgcStep2":
					this.receiverViewControl1.AgcStep2 = this.device.AgcStep2;
					break;
				case "AgcStep3":
					this.receiverViewControl1.AgcStep3 = this.device.AgcStep3;
					break;
				case "AgcStep4":
					this.receiverViewControl1.AgcStep4 = this.device.AgcStep4;
					break;
				case "AgcStep5":
					this.receiverViewControl1.AgcStep5 = this.device.AgcStep5;
					break;
				case "FastHopOn":
					this.commonViewControl1.FastHopOn = this.device.FastHopOn;
					break;
				case "TcxoInputOn":
					this.commonViewControl1.TcxoInputOn = this.device.TcxoInputOn;
					break;
				case "PllBandwidth":
					this.transmitterViewControl1.PllBandwidth = this.device.PllBandwidth;
					break;
				case "TempDelta":
					this.temperatureViewControl1.TempDelta = this.device.TempDelta;
					break;
				case "PngEnabled":
					if (this.device.PngEnabled)
					{
						this.DisableControls();
						this.packetHandlerView1.Enabled = false;
						break;
					}
					this.EnableControls();
					this.packetHandlerView1.Enabled = true;
					break;
			}
		}

		private void OnDevicePacketHandlerStarted(object sender, EventArgs e)
		{
			this.DisableControls();
		}

		private void OnDevicePacketHandlerStoped(object sender, EventArgs e)
		{
			this.EnableControls();
			this.packetHandlerView1.StartStop = false;
		}

		private void OnDevicePacketHandlerTransmitted(object sender, PacketStatusEventArg e)
		{
			this.packetHandlerView1.PacketNumber = e.Number;
		}

		private void OnDevicePacketHandlerReceived(object sender, PacketStatusEventArg e)
		{
			this.packetHandlerView1.PacketNumber = e.Number;
		}

		private void OnError(byte status, string message)
		{
			if (this.Error == null)
				return;
			this.Error((object)this, new SemtechLib.General.Events.ErrorEventArgs(status, message));
		}

		public new void Dispose()
		{
			base.Dispose();
		}

		public void DisableControls()
		{
			this.commonViewControl1.Enabled = false;
			this.transmitterViewControl1.Enabled = false;
			this.receiverViewControl1.Enabled = false;
			this.irqMapViewControl1.Enabled = false;
			this.temperatureViewControl1.Enabled = false;
			this.gBoxOperatingMode.Enabled = false;
		}

		public void EnableControls()
		{
			this.commonViewControl1.Enabled = true;
			this.transmitterViewControl1.Enabled = true;
			this.receiverViewControl1.Enabled = true;
			this.irqMapViewControl1.Enabled = true;
			this.temperatureViewControl1.Enabled = true;
			this.gBoxOperatingMode.Enabled = true;
		}

		private void device_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (this.InvokeRequired)
				this.BeginInvoke((Delegate)new DeviceViewControl.DevicePropertyChangedDelegate(this.OnDevicePropertyChanged), sender, (object)e);
			else
				this.OnDevicePropertyChanged(sender, e);
		}

		private void device_OcpTrimLimitStatusChanged(object sender, LimitCheckStatusEventArg e)
		{
			this.transmitterViewControl1.UpdateOcpTrimLimits(e.Status, e.Message);
		}

		private void device_BitrateLimitStatusChanged(object sender, LimitCheckStatusEventArg e)
		{
			this.commonViewControl1.UpdateBitrateLimits(e.Status, e.Message);
		}

		private void device_FdevLimitStatusChanged(object sender, LimitCheckStatusEventArg e)
		{
			this.commonViewControl1.UpdateFdevLimits(e.Status, e.Message);
			this.receiverViewControl1.UpdateRxBwLimits(e.Status, e.Message);
		}

		private void device_FrequencyRfLimitStatusChanged(object sender, LimitCheckStatusEventArg e)
		{
			this.commonViewControl1.UpdateFrequencyRfLimits(e.Status, e.Message);
		}

		private void device_SyncValueLimitChanged(object sender, LimitCheckStatusEventArg e)
		{
			this.packetHandlerView1.UpdateSyncValueLimits(e.Status, e.Message);
		}

		private void device_PacketHandlerStarted(object sender, EventArgs e)
		{
			if (this.InvokeRequired)
				this.BeginInvoke((Delegate)new DeviceViewControl.DevicePacketHandlerStartedDelegate(this.OnDevicePacketHandlerStarted), sender, (object)e);
			else
				this.OnDevicePacketHandlerStarted(sender, e);
		}

		private void device_PacketHandlerStoped(object sender, EventArgs e)
		{
			if (this.InvokeRequired)
				this.BeginInvoke((Delegate)new DeviceViewControl.DevicePacketHandlerStopedDelegate(this.OnDevicePacketHandlerStoped), sender, (object)e);
			else
				this.OnDevicePacketHandlerStoped(sender, e);
		}

		private void device_PacketHandlerTransmitted(object sender, PacketStatusEventArg e)
		{
			if (this.InvokeRequired)
				this.BeginInvoke((Delegate)new DeviceViewControl.DevicePacketHandlerTransmittedDelegate(this.OnDevicePacketHandlerTransmitted), sender, (object)e);
			else
				this.OnDevicePacketHandlerTransmitted(sender, e);
		}

		private void device_PacketHandlerReceived(object sender, PacketStatusEventArg e)
		{
			if (this.InvokeRequired)
				this.BeginInvoke((Delegate)new DeviceViewControl.DevicePacketHandlerTransmittedDelegate(this.OnDevicePacketHandlerTransmitted), sender, (object)e);
			else
				this.OnDevicePacketHandlerReceived(sender, e);
		}

		private void commonViewControl1_DocumentationChanged(object sender, DocumentationChangedEventArgs e)
		{
			this.OnDocumentationChanged(e);
		}

		private void commonViewControl1_FrequencyXoChanged(object sender, DecimalEventArg e)
		{
			this.device.FrequencyXo = this.commonViewControl1.FrequencyXo;
		}

		private void rBtnOperatingMode_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				if (this.rBtnSleep.Checked)
					this.device.SetOperatingMode(OperatingModeEnum.Sleep);
				else if (this.rBtnStandby.Checked)
					this.device.SetOperatingMode(OperatingModeEnum.Stdby);
				else if (this.rBtnSynthesizerRx.Checked)
					this.device.SetOperatingMode(OperatingModeEnum.FsRx);
				else if (this.rBtnSynthesizerTx.Checked)
					this.device.SetOperatingMode(OperatingModeEnum.FsTx);
				else if (this.rBtnReceiver.Checked)
					this.device.SetOperatingMode(OperatingModeEnum.Rx);
				else if (this.rBtnTransmitter.Checked)
					this.device.SetOperatingMode(OperatingModeEnum.Tx);
				this.irqMapViewControl1.Mode = this.device.Mode;
				this.packetHandlerView1.Mode = this.device.Mode;
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_ModulationTypeChanged(object sender, ModulationTypeEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetModulationType(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_ModulationShapingChanged(object sender, ByteEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetModulationShaping(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_BitrateChanged(object sender, DecimalEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetBitrate(e.Value);
				this.receiverViewControl1.Bitrate = this.device.Bitrate;
				this.packetHandlerView1.Bitrate = this.device.Bitrate;
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_BitrateFracChanged(object sender, DecimalEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetBitrateFrac(e.Value);
				this.receiverViewControl1.Bitrate = this.device.Bitrate;
				this.packetHandlerView1.Bitrate = this.device.Bitrate;
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_FdevChanged(object sender, DecimalEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetFdev(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_BandChanged(object sender, BandEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetBand(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_LowFrequencyModeOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetLowFrequencyModeOn(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_FrequencyRfChanged(object sender, DecimalEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetFrequencyRf(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_FastHopOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetFastHopOn(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_TcxoInputChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetTcxoInputOn(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_RcCalibrationChanged(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.RcCalTrig();
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_LowBatOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetLowBatOn(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_LowBatTrimChanged(object sender, LowBatTrimEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetLowBatTrim(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void transmitterViewControl1_DocumentationChanged(object sender, DocumentationChangedEventArgs e)
		{
			this.OnDocumentationChanged(e);
		}

		private void transmitterViewControl1_PaModeChanged(object sender, PaModeEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetPaMode(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void transmitterViewControl1_MaxOutputPowerChanged(object sender, DecimalEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetMaxOutputPower(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void transmitterViewControl1_OutputPowerChanged(object sender, DecimalEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetOutputPower(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_ForceTxBandLowFrequencyOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetForceTxBandLowFrequencyOn(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void transmitterViewControl1_PaRampChanged(object sender, PaRampEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetPaRamp(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void transmitterViewControl1_OcpOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetOcpOn(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void transmitterViewControl1_OcpTrimChanged(object sender, DecimalEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetOcpTrim(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void transmitterViewControl1_Pa20dBmChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetPa20dBm(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void transmitterViewControl1_PllBandwidthChanged(object sender, DecimalEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetPllBandwidth(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_DocumentationChanged(object sender, DocumentationChangedEventArgs e)
		{
			this.OnDocumentationChanged(e);
		}

		private void receiverViewControl1_AgcReferenceLevelChanged(object sender, Int32EventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetAgcReferenceLevel(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_AgcStepChanged(object sender, AgcStepEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetAgcStep(e.Id, e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_LnaGainChanged(object sender, LnaGainEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetLnaGain(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void commonViewControl1_ForceRxBandLowFrequencyOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetForceRxBandLowFrequencyOn(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_LnaBoostChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetLnaBoost(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_RestartRxOnCollisionOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetRestartRxOnCollisionOn(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_RestartRxWithoutPllLockChanged(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetRestartRxWithoutPllLock();
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_RestartRxWithPllLockChanged(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetRestartRxWithPllLock();
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_AfcAutoOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetAfcAutoOn(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_AgcAutoOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetAgcAutoOn(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_RxTriggerChanged(object sender, RxTriggerEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetRxTrigger(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_RssiOffsetChanged(object sender, DecimalEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetRssiOffset(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_RssiSmoothingChanged(object sender, DecimalEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetRssiSmoothing(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_RssiCollisionThresholdChanged(object sender, DecimalEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetRssiCollisionThreshold(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_RssiThreshChanged(object sender, DecimalEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetRssiThresh(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_DccFastInitOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetDccFastInitOn(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_DccForceOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetDccForceOn(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_RxBwChanged(object sender, DecimalEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetRxBw(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_AfcRxBwChanged(object sender, DecimalEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetAfcRxBw(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_BitSyncOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetBitSyncOn(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_OokThreshTypeChanged(object sender, OokThreshTypeEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetOokThreshType(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_OokPeakThreshStepChanged(object sender, DecimalEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetOokPeakThreshStep(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_OokPeakThreshDecChanged(object sender, OokPeakThreshDecEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetOokPeakThreshDec(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_OokAverageThreshFiltChanged(object sender, OokAverageThreshFiltEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetOokAverageThreshFilt(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_OokPeakRecoveryOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetOokPeakRecoveryOn(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_OokAverageBiasChanged(object sender, DecimalEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetOokAverageBias(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_OokFixedThreshChanged(object sender, ByteEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetOokFixedThresh(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_BarkerSyncThreshChanged(object sender, ByteEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetBarkerSyncThresh(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_BarkerSyncLossThreshChanged(object sender, ByteEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetBarkerSyncLossThresh(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_BarkerTrackingThreshChanged(object sender, ByteEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetBarkerTrackingThresh(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_AgcStartChanged(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetAgcStart();
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_FeiRangeChanged(object sender, FeiRangeEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetFeiRange(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_FeiReadChanged(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetFeiRead();
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_AfcAutoClearOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetAfcAutoClearOn(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_AfcClearChanged(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetAfcClear();
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_PreambleDetectorOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetPreambleDetectorOn(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_PreambleDetectorSizeChanged(object sender, ByteEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetPreambleDetectorSize(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_PreambleDetectorTolChanged(object sender, ByteEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetPreambleDetectorTol(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_TimeoutRssiChanged(object sender, DecimalEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetTimeoutRssi(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_TimeoutPreambleChanged(object sender, DecimalEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetTimeoutPreamble(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_TimeoutSyncWordChanged(object sender, DecimalEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetTimeoutSyncWord(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void receiverViewControl1_AutoRxRestartDelayChanged(object sender, DecimalEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetAutoRxRestartDelay(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void irqMapViewControl1_DocumentationChanged(object sender, DocumentationChangedEventArgs e)
		{
			this.OnDocumentationChanged(e);
		}

		private void irqMapViewControl1_DioMappingChanged(object sender, DioMappingEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetDioMapping(e.Id, e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void irqMapViewControl1_DioPreambleIrqOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetDioPreambleIrqOn(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void irqMapViewControl1_ClockOutChanged(object sender, ClockOutEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetClockOut(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_DocumentationChanged(object sender, DocumentationChangedEventArgs e)
		{
			this.OnDocumentationChanged(e);
		}

		private void packetHandlerView1_Error(object sender, SemtechLib.General.Events.ErrorEventArgs e)
		{
			this.OnError(e.Status, e.Message);
		}

		private void packetHandlerView1_PreambleSizeChanged(object sender, Int32EventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetPreambleSize(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_SyncOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetSyncOn(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_FifoFillConditionChanged(object sender, FifoFillConditionEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetFifoFillCondition(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_SyncSizeChanged(object sender, ByteEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetSyncSize(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_SyncValueChanged(object sender, ByteArrayEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetSyncValue(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_PacketFormatChanged(object sender, PacketFormatEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetPacketFormat(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_DcFreeChanged(object sender, DcFreeEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetDcFree(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_CrcOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetCrcOn(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_CrcAutoClearOffChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetCrcAutoClearOff(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_AddressFilteringChanged(object sender, AddressFilteringEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetAddressFiltering(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_PayloadLengthChanged(object sender, Int16EventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetPayloadLength(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_NodeAddressChanged(object sender, ByteEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetNodeAddress(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_BroadcastAddressChanged(object sender, ByteEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetBroadcastAddress(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_TxStartConditionChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetTxStartCondition(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_FifoThresholdChanged(object sender, ByteEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetFifoThreshold(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_MessageLengthChanged(object sender, Int32EventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetMessageLength(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_MessageChanged(object sender, ByteArrayEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetMessage(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_StartStopChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetPacketHandlerStartStop(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_MaxPacketNumberChanged(object sender, Int32EventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetMaxPacketNumber(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_PacketHandlerLogEnableChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetPacketHandlerLogEnable(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void sequencerViewControl1_DocumentationChanged(object sender, DocumentationChangedEventArgs e)
		{
			this.OnDocumentationChanged(e);
		}

		private void sequencerViewControl1_SequencerStartChanged(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetSequencerStart();
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void sequencerViewControl1_SequencerStopChanged(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetSequencerStop();
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void sequencerViewControl1_IdleModeChanged(object sender, IdleModeEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetIdleMode(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void sequencerViewControl1_FromStartChanged(object sender, FromStartEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetFromStart(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void sequencerViewControl1_LowPowerSelectionChanged(object sender, LowPowerSelectionEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetLowPowerSelection(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void sequencerViewControl1_FromIdleChanged(object sender, FromIdleEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetFromIdle(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void sequencerViewControl1_FromTransmitChanged(object sender, FromTransmitEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetFromTransmit(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void sequencerViewControl1_FromReceiveChanged(object sender, FromReceiveEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetFromReceive(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void sequencerViewControl1_FromRxTimeoutChanged(object sender, FromRxTimeoutEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetFromRxTimeout(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void sequencerViewControl1_FromPacketReceivedChanged(object sender, FromPacketReceivedEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetFromPacketReceived(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void sequencerViewControl1_Timer1ResolutionChanged(object sender, TimerResolutionEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetTimer1Resolution(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void sequencerViewControl1_Timer2ResolutionChanged(object sender, TimerResolutionEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetTimer2Resolution(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void sequencerViewControl1_Timer1CoefChanged(object sender, ByteEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetTimer1Coef(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void sequencerViewControl1_Timer2CoefChanged(object sender, ByteEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetTimer2Coef(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void temperatureViewControl1_DocumentationChanged(object sender, DocumentationChangedEventArgs e)
		{
			this.OnDocumentationChanged(e);
		}

		private void temperatureViewControl1_RxCalAutoOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetRxCalAutoOn(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void temperatureViewControl1_RxCalibrationChanged(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.ImageCalStart();
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void temperatureViewControl1_TempMeasOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetTempMonitorOff(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void temperatureViewControl1_TempThresholdChanged(object sender, TempThresholdEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetTempThreshold(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void temperatureViewControl1_TempCalibrateChanged(object sender, DecimalEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetTempCalibrate(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void control_MouseEnter(object sender, EventArgs e)
		{
			if (sender == this.gBoxIrqFlags)
			{
				this.OnDocumentationChanged(new DocumentationChangedEventArgs("IrqMap", "Irq flags"));
			}
			else
			{
				if (sender != this.gBoxOperatingMode)
					return;
				this.OnDocumentationChanged(new DocumentationChangedEventArgs("Common", "Operating mode"));
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

		private void packetHandlerView1_DataModeChanged(object sender, DataModeEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetDataMode(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_AutoRestartRxChanged(object sender, AutoRestartRxEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetAutoRestartRxOn(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_PreamblePolarityChanged(object sender, PreamblePolarityEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetPreamblePolarity(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_CrcIbmChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetCrcIbmOn(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_IoHomeOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetIoHomeOn(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_IoHomePwrFrameOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetIoHomePwrFrameOn(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_BeaconOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetBeaconOn(e.Value);
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void packetHandlerView1_FillFifoChanged(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetFillFifo();
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void ledRssi_Click(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.ClrRssiIrq();
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void ledPreamble_Click(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.ClrPreambleDetectIrq();
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void ledSyncAddressMatch_Click(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.ClrSyncAddressMatchIrq();
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void ledFifoOverrun_Click(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.ClrFifoOverrunIrq();
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void ledLowBat_Click(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.ClrLowBatIrq();
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}
	}
}
using SemtechLib.Controls;
using SemtechLib.Devices.Common.Events;
using SemtechLib.Devices.Common.Interfaces;
using SemtechLib.Devices.SX1276LR;
using SemtechLib.Devices.SX1276LR.Enumerations;
using SemtechLib.Devices.SX1276LR.Events;
using SemtechLib.Devices.SX1276LR.UI.Forms;
using SemtechLib.General;
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

namespace SemtechLib.Devices.SX1276LR.UI.Controls
{
	public class DeviceViewControl : UserControl, IDeviceView, IDisposable, INotifyDocumentationChanged
	{
		private delegate void DevicePropertyChangedDelegate(object sender, PropertyChangedEventArgs e);
		private delegate void DevicePacketHandlerStartedDelegate(object sender, EventArgs e);
		private delegate void DevicePacketHandlerStopedDelegate(object sender, EventArgs e);
		private delegate void DevicePacketHandlerTransmittedDelegate(object sender, PacketStatusEventArg e);

		private ApplicationSettings appSettings;
		private SX1276LR device;
		private IContainer components;
		private TabControl tabControl1;
		private TabPage tabCommon;
		private CommonViewControl commonViewControl1;
		private GroupBoxEx gBoxOperatingMode;
		private RadioButton rBtnReceiver;
		private RadioButton rBtnReceiverSingle;
		private RadioButton rBtnReceiverCad;
		private RadioButton rBtnSynthesizerRx;
		private RadioButton rBtnStandby;
		private RadioButton rBtnSleep;
		private RadioButton rBtnTransmitterContinuous;
		private Led ledRxTimeout;
		private Label lbModeReady;
		private Label label19;
		private Label label18;
		private Led ledValidHeader;
		private Label label17;
		private Led ledPayloadCrcError;
		private Led ledRxDone;
		private Led ledCadDetected;
		private Label label23;
		private Label label22;
		private Led ledFhssChangeChannel;
		private Label label21;
		private Label label20;
		private Led ledCadDone;
		private Led ledTxDone;
		private GroupBoxEx gBoxIrqFlags;
		private RadioButton rBtnSynthesizerTx;
		private RadioButton rBtnSynthesizer;
		private Led ledFifoNotEmpty;
		private Led ledModeReady;
		private TabPage tabLoRa;
		private LoRaViewControl loRaViewControl1;
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

		public ApplicationSettings AppSettings
		{
			get
			{
				return this.appSettings;
			}
			set
			{
				this.appSettings = value;
			}
		}

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
				this.device = (SX1276LR)value;
				this.device.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(this.device_PropertyChanged);
				this.device.OcpTrimLimitStatusChanged += new SX1276LR.LimitCheckStatusChangedEventHandler(this.device_OcpTrimLimitStatusChanged);
				this.device.FrequencyRfLimitStatusChanged += new SX1276LR.LimitCheckStatusChangedEventHandler(this.device_FrequencyRfLimitStatusChanged);
				this.device.BandwidthLimitStatusChanged += new SX1276LR.LimitCheckStatusChangedEventHandler(this.device_BandwidthLimitStatusChanged);
				this.device.PacketHandlerStarted += new EventHandler(this.device_PacketHandlerStarted);
				this.device.PacketHandlerStoped += new EventHandler(this.device_PacketHandlerStoped);
				this.device.PacketHandlerTransmitted += new PacketStatusEventHandler(this.device_PacketHandlerTransmitted);
				this.device.PacketHandlerReceived += new PacketStatusEventHandler(this.device_PacketHandlerReceived);
				this.commonViewControl1.FrequencyXo = this.device.FrequencyXo;
				this.commonViewControl1.FrequencyStep = this.device.FrequencyStep;
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

		private void LoadTestPage(SX1276LR device)
		{
			try
			{
				if (!File.Exists(Application.StartupPath + "\\SemtechLib.Devices.SX1276LR.Test.dll"))
					return;
				Type type = Assembly.LoadFile(Application.StartupPath + "\\SemtechLib.Devices.SX1276LR.Test.dll").GetType("SemtechLib.Devices.SX1276LR.Test.Controls.TestTabPage");
				object instance = Activator.CreateInstance(type);
				type.InvokeMember("SuspendLayout", BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod, (Binder)null, instance, (object[])null);
				this.SuspendLayout();
				type.GetProperty("Location").SetValue(instance, (object)new Point(4, 22), (object[])null);
				type.GetProperty("Name").SetValue(instance, (object)"tabTest", (object[])null);
				type.GetProperty("Size").SetValue(instance, (object)new Size(799, 493), (object[])null);
				type.GetProperty("TabIndex").SetValue(instance, (object)6, (object[])null);
				type.GetProperty("Text").SetValue(instance, (object)"R&D Tests", (object[])null);
				type.GetProperty("UseVisualStyleBackColor").SetValue(instance, (object)true, (object[])null);
				type.GetProperty("SX1276LR").SetValue(instance, (object)device, (object[])null);
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
				case "IsDebugOn":
					this.loRaViewControl1.IsDebugOn = this.device.IsDebugOn;
					break;
				case "FrequencyXo":
					this.commonViewControl1.FrequencyXo = this.device.FrequencyXo;
					break;
				case "FrequencyStep":
					this.commonViewControl1.FrequencyStep = this.device.FrequencyStep;
					break;
				case "SymbolTime":
					this.loRaViewControl1.SymbolTime = this.device.SymbolTime;
					break;
				case "Mode":
					this.rBtnSleep.CheckedChanged -= new EventHandler(this.rBtnOperatingMode_CheckedChanged);
					this.rBtnStandby.CheckedChanged -= new EventHandler(this.rBtnOperatingMode_CheckedChanged);
					this.rBtnSynthesizerRx.CheckedChanged -= new EventHandler(this.rBtnOperatingMode_CheckedChanged);
					this.rBtnSynthesizerTx.CheckedChanged -= new EventHandler(this.rBtnOperatingMode_CheckedChanged);
					this.rBtnReceiver.CheckedChanged -= new EventHandler(this.rBtnOperatingMode_CheckedChanged);
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
						case OperatingModeEnum.FsRx:
							this.rBtnSynthesizerRx.Checked = true;
							break;
						case OperatingModeEnum.Rx:
							this.rBtnReceiver.Checked = true;
							break;
						case OperatingModeEnum.RxSingle:
							this.rBtnReceiverSingle.Checked = true;
							break;
						case OperatingModeEnum.Cad:
							this.rBtnReceiverCad.Checked = true;
							break;
					}
					this.rBtnSleep.CheckedChanged += new EventHandler(this.rBtnOperatingMode_CheckedChanged);
					this.rBtnStandby.CheckedChanged += new EventHandler(this.rBtnOperatingMode_CheckedChanged);
					this.rBtnSynthesizerRx.CheckedChanged += new EventHandler(this.rBtnOperatingMode_CheckedChanged);
					this.rBtnSynthesizerTx.CheckedChanged += new EventHandler(this.rBtnOperatingMode_CheckedChanged);
					this.rBtnReceiver.CheckedChanged += new EventHandler(this.rBtnOperatingMode_CheckedChanged);
					this.loRaViewControl1.Mode = this.device.Mode;
					break;
				case "Band":
					this.commonViewControl1.Band = this.device.Band;
					break;
				case "LowFrequencyModeOn":
					this.commonViewControl1.LowFrequencyModeOn = this.device.LowFrequencyModeOn;
					break;
				case "FrequencyRf":
					this.commonViewControl1.FrequencyRf = this.device.FrequencyRf;
					break;
				case "FastHopOn":
					this.commonViewControl1.FastHopOn = this.device.FastHopOn;
					break;
				case "PaSelect":
					this.commonViewControl1.PaSelect = this.device.PaSelect;
					break;
				case "MaxOutputPower":
					this.commonViewControl1.MaxOutputPower = this.device.MaxOutputPower;
					break;
				case "OutputPower":
					this.commonViewControl1.OutputPower = this.device.OutputPower;
					break;
				case "ForceTxBandLowFrequencyOn":
					this.commonViewControl1.ForceTxBandLowFrequencyOn = this.device.ForceTxBandLowFrequencyOn;
					break;
				case "PaRamp":
					this.commonViewControl1.PaRamp = this.device.PaRamp;
					break;
				case "OcpOn":
					this.commonViewControl1.OcpOn = this.device.OcpOn;
					break;
				case "OcpTrim":
					this.commonViewControl1.OcpTrim = this.device.OcpTrim;
					break;
				case "Pa20dBm":
					this.commonViewControl1.Pa20dBm = this.device.Pa20dBm;
					break;
				case "PllBandwidth":
					this.commonViewControl1.PllBandwidth = this.device.PllBandwidth;
					break;
				case "LnaGain":
					this.commonViewControl1.LnaGain = this.device.LnaGain;
					break;
				case "ForceRxBandLowFrequencyOn":
					this.commonViewControl1.ForceRxBandLowFrequencyOn = this.device.ForceRxBandLowFrequencyOn;
					break;
				case "LnaBoost":
					this.commonViewControl1.LnaBoost = this.device.LnaBoost;
					break;
				case "AgcReference":
					this.commonViewControl1.AgcReference = this.device.AgcReference;
					break;
				case "AgcThresh1":
					this.commonViewControl1.AgcThresh1 = this.device.AgcThresh1;
					break;
				case "AgcThresh2":
					this.commonViewControl1.AgcThresh2 = this.device.AgcThresh2;
					break;
				case "AgcThresh3":
					this.commonViewControl1.AgcThresh3 = this.device.AgcThresh3;
					break;
				case "AgcThresh4":
					this.commonViewControl1.AgcThresh4 = this.device.AgcThresh4;
					break;
				case "AgcThresh5":
					this.commonViewControl1.AgcThresh5 = this.device.AgcThresh5;
					break;
				case "AgcReferenceLevel":
					this.commonViewControl1.AgcReferenceLevel = (int)this.device.AgcReferenceLevel;
					break;
				case "AgcStep1":
					this.commonViewControl1.AgcStep1 = this.device.AgcStep1;
					break;
				case "AgcStep2":
					this.commonViewControl1.AgcStep2 = this.device.AgcStep2;
					break;
				case "AgcStep3":
					this.commonViewControl1.AgcStep3 = this.device.AgcStep3;
					break;
				case "AgcStep4":
					this.commonViewControl1.AgcStep4 = this.device.AgcStep4;
					break;
				case "AgcStep5":
					this.commonViewControl1.AgcStep5 = this.device.AgcStep5;
					break;
				case "RxTimeout":
					this.ledRxTimeout.Checked = this.device.RxTimeout;
					break;
				case "RxDone":
					this.ledRxDone.Checked = this.device.RxDone;
					break;
				case "PayloadCrcError":
					this.ledPayloadCrcError.Checked = this.device.PayloadCrcError;
					break;
				case "ValidHeader":
					this.ledValidHeader.Checked = this.device.ValidHeader;
					break;
				case "TxDone":
					this.ledTxDone.Checked = this.device.TxDone;
					break;
				case "CadDone":
					this.ledCadDone.Checked = this.device.CadDone;
					break;
				case "FhssChangeChannel":
					this.ledFhssChangeChannel.Checked = this.device.FhssChangeChannel;
					break;
				case "CadDetected":
					this.ledCadDetected.Checked = this.device.CadDetected;
					break;
				case "RxTimeoutMask":
					this.loRaViewControl1.RxTimeoutMask = this.device.RxTimeoutMask;
					break;
				case "RxDoneMask":
					this.loRaViewControl1.RxDoneMask = this.device.RxDoneMask;
					break;
				case "PayloadCrcErrorMask":
					this.loRaViewControl1.PayloadCrcErrorMask = this.device.PayloadCrcErrorMask;
					break;
				case "ValidHeaderMask":
					this.loRaViewControl1.ValidHeaderMask = this.device.ValidHeaderMask;
					break;
				case "TxDoneMask":
					this.loRaViewControl1.TxDoneMask = this.device.TxDoneMask;
					break;
				case "CadDoneMask":
					this.loRaViewControl1.CadDoneMask = this.device.CadDoneMask;
					break;
				case "FhssChangeChannelMask":
					this.loRaViewControl1.FhssChangeChannelMask = this.device.FhssChangeChannelMask;
					break;
				case "CadDetectedMask":
					this.loRaViewControl1.CadDetectedMask = this.device.CadDetectedMask;
					break;
				case "ImplicitHeaderModeOn":
					this.loRaViewControl1.ImplicitHeaderModeOn = this.device.ImplicitHeaderModeOn;
					break;
				case "AgcAutoOn":
					this.commonViewControl1.AgcAutoOn = this.device.AgcAutoOn;
					break;
				case "SymbTimeout":
					this.loRaViewControl1.SymbTimeout = this.device.SymbTimeout;
					break;
				case "PayloadCrcOn":
					this.loRaViewControl1.PayloadCrcOn = this.device.PayloadCrcOn;
					break;
				case "CodingRate":
					this.loRaViewControl1.CodingRate = this.device.CodingRate;
					break;
				case "PayloadLength":
					this.loRaViewControl1.PayloadLength = this.device.PayloadLength;
					break;
				case "PreambleLength":
					this.loRaViewControl1.PreambleLength = (int)this.device.PreambleLength;
					break;
				case "Bandwidth":
					this.loRaViewControl1.Bandwidth = this.device.Bandwidth;
					break;
				case "SpreadingFactor":
					this.loRaViewControl1.SpreadingFactor = this.device.SpreadingFactor;
					break;
				case "FreqHoppingPeriod":
					this.loRaViewControl1.FreqHoppingPeriod = this.device.FreqHoppingPeriod;
					break;
				case "RxNbBytes":
					this.loRaViewControl1.RxNbBytes = this.device.RxNbBytes;
					break;
				case "PllTimeout":
					this.loRaViewControl1.PllTimeout = this.device.PllTimeout;
					break;
				case "RxPayloadCrcOn":
					this.loRaViewControl1.RxPayloadCrcOn = this.device.RxPayloadCrcOn;
					break;
				case "RxPayloadCodingRate":
					this.loRaViewControl1.RxPayloadCodingRate = this.device.RxPayloadCodingRate;
					break;
				case "ValidHeaderCnt":
					this.loRaViewControl1.ValidHeaderCnt = this.device.ValidHeaderCnt;
					break;
				case "ValidPacketCnt":
					this.loRaViewControl1.ValidPacketCnt = this.device.ValidPacketCnt;
					break;
				case "ModemClear":
					this.loRaViewControl1.ModemClear = this.device.ModemClear;
					this.ledModemClear.Checked = this.device.ModemClear;
					break;
				case "HeaderInfoValid":
					this.loRaViewControl1.HeaderInfoValid = this.device.HeaderInfoValid;
					this.ledHeaderInfoValid.Checked = this.device.HeaderInfoValid;
					break;
				case "RxOnGoing":
					this.loRaViewControl1.RxOnGoing = this.device.RxOnGoing;
					this.ledRxOnGoing.Checked = this.device.RxOnGoing;
					break;
				case "SignalSynchronized":
					this.loRaViewControl1.SignalSynchronized = this.device.SignalSynchronized;
					this.ledSignalSynchronized.Checked = this.device.SignalSynchronized;
					break;
				case "SignalDetected":
					this.loRaViewControl1.SignalDetected = this.device.SignalDetected;
					this.ledSignalDetected.Checked = this.device.SignalDetected;
					break;
				case "PktSnrValue":
					this.loRaViewControl1.PktSnrValue = this.device.PktSnrValue;
					break;
				case "RssiValue":
					this.loRaViewControl1.RssiValue = this.device.RssiValue;
					break;
				case "PktRssiValue":
					this.loRaViewControl1.PktRssiValue = this.device.PktRssiValue;
					break;
				case "HopChannel":
					this.loRaViewControl1.HopChannel = this.device.HopChannel;
					break;
				case "FifoRxCurrentAddr":
					this.loRaViewControl1.FifoRxCurrentAddr = this.device.FifoRxCurrentAddr;
					break;
				case "LowDatarateOptimize":
					this.loRaViewControl1.LowDatarateOptimize = this.device.LowDatarateOptimize;
					break;
				case "TcxoInputOn":
					this.commonViewControl1.TcxoInputOn = this.device.TcxoInputOn;
					break;
				case "Dio0Mapping":
					this.commonViewControl1.Dio0Mapping = this.device.Dio0Mapping;
					break;
				case "Dio1Mapping":
					this.commonViewControl1.Dio1Mapping = this.device.Dio1Mapping;
					break;
				case "Dio2Mapping":
					this.commonViewControl1.Dio2Mapping = this.device.Dio2Mapping;
					break;
				case "Dio3Mapping":
					this.commonViewControl1.Dio3Mapping = this.device.Dio3Mapping;
					break;
				case "Dio4Mapping":
					this.commonViewControl1.Dio4Mapping = this.device.Dio4Mapping;
					break;
				case "Dio5Mapping":
					this.commonViewControl1.Dio5Mapping = this.device.Dio5Mapping;
					break;
				case "Payload":
					this.loRaViewControl1.Payload = this.device.Payload;
					break;
				case "LogEnabled":
					this.loRaViewControl1.LogEnabled = this.device.LogEnabled;
					break;
				case "PacketModeTx":
					this.loRaViewControl1.PacketModeTx = this.device.PacketModeTx;
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
			this.loRaViewControl1.StartStop = false;
		}

		private void OnDevicePacketHandlerTransmitted(object sender, PacketStatusEventArg e)
		{
			this.loRaViewControl1.PacketNumber = e.Number;
		}

		private void OnDevicePacketHandlerReceived(object sender, PacketStatusEventArg e)
		{
			this.loRaViewControl1.PacketNumber = e.Number;
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
			this.gBoxOperatingMode.Enabled = false;
		}

		public void EnableControls()
		{
			this.commonViewControl1.Enabled = true;
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
			this.commonViewControl1.UpdateOcpTrimLimits(e.Status, e.Message);
		}

		private void device_FrequencyRfLimitStatusChanged(object sender, LimitCheckStatusEventArg e)
		{
			this.commonViewControl1.UpdateFrequencyRfLimits(e.Status, e.Message);
		}

		private void device_BandwidthLimitStatusChanged(object sender, LimitCheckStatusEventArg e)
		{
			this.loRaViewControl1.UpdateBandwidthLimits(e.Status, e.Message);
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
				else if (this.rBtnSynthesizerTx.Checked)
					this.device.SetOperatingMode(OperatingModeEnum.FsTx);
				else if (this.rBtnTransmitterContinuous.Checked)
				{
					this.device.PacketModeTx = true;
					this.device.SetOperatingMode(OperatingModeEnum.TxContinuous);
				}
				else if (this.rBtnSynthesizerRx.Checked)
					this.device.SetOperatingMode(OperatingModeEnum.FsRx);
				else if (this.rBtnReceiver.Checked)
				{
					this.device.PacketModeRxSingle = false;
					this.device.PacketModeTx = false;
					this.device.SetOperatingMode(OperatingModeEnum.Rx);
				}
				else if (this.rBtnReceiverSingle.Checked)
				{
					this.device.PacketModeRxSingle = true;
					this.device.PacketModeTx = false;
					this.loRaViewControl1.StartStop = true;
				}
				else if (this.rBtnReceiverCad.Checked)
					this.device.SetOperatingMode(OperatingModeEnum.Cad);
				this.loRaViewControl1.Mode = this.device.Mode;
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

		private void commonViewControl1_PaModeChanged(object sender, PaModeEventArg e)
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

		private void commonViewControl1_MaxOutputPowerChanged(object sender, DecimalEventArg e)
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

		private void commonViewControl1_OutputPowerChanged(object sender, DecimalEventArg e)
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

		private void commonViewControl1_PaRampChanged(object sender, PaRampEventArg e)
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

		private void commonViewControl1_OcpOnChanged(object sender, BooleanEventArg e)
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

		private void commonViewControl1_OcpTrimChanged(object sender, DecimalEventArg e)
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

		private void commonViewControl1_Pa20dBmChanged(object sender, BooleanEventArg e)
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

		private void commonViewControl1_PllBandwidthChanged(object sender, DecimalEventArg e)
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

		private void commonViewControl1_AgcReferenceLevelChanged(object sender, Int32EventArg e)
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

		private void commonViewControl1_AgcStepChanged(object sender, AgcStepEventArg e)
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

		private void commonViewControl1_LnaGainChanged(object sender, LnaGainEventArg e)
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

		private void commonViewControl1_LnaBoostChanged(object sender, BooleanEventArg e)
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

		private void commonViewControl1_AgcAutoOnChanged(object sender, BooleanEventArg e)
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

		private void commonViewControl1_DioMappingChanged(object sender, DioMappingEventArg e)
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

		private void commonViewControl1_ClockOutChanged(object sender, ClockOutEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
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

		private void loRaViewControl1_Error(object sender, SemtechLib.General.Events.ErrorEventArgs e)
		{
			this.OnError(e.Status, e.Message);
		}

		private void ledRxTimeout_Click(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.ClrRxTimeoutIrq();
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

		private void ledRxDone_Click(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.ClrRxDoneIrq();
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

		private void ledPayloadCrcError_Click(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.ClrPayloadCrcErrorIrq();
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

		private void ledValidHeader_Click(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.ClrValidHeaderIrq();
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

		private void ledTxDone_Click(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.ClrTxDoneIrq();
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

		private void ledCadDone_Click(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.ClrCadDoneIrq();
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

		private void ledFhssChangeChannel_Click(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.ClrFhssChangeChannelIrq();
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

		private void ledCadDetected_Click(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.ClrCadDetectedIrq();
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

		private void loRaViewControl1_RxTimeoutMaskChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetRxTimeoutMask(e.Value);
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

		private void loRaViewControl1_RxDoneMaskChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetRxDoneMask(e.Value);
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

		private void loRaViewControl1_PayloadCrcErrorMaskChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetPayloadCrcErrorMask(e.Value);
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

		private void loRaViewControl1_ValidHeaderMaskChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetValidHeaderMask(e.Value);
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

		private void loRaViewControl1_TxDoneMaskChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetTxDoneMask(e.Value);
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

		private void loRaViewControl1_CadDoneMaskChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetCadDoneMask(e.Value);
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

		private void loRaViewControl1_FhssChangeChannelMaskChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetFhssChangeChannelMask(e.Value);
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

		private void loRaViewControl1_CadDetectedMaskChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetCadDetectedMask(e.Value);
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

		private void loRaViewControl1_ImplicitHeaderModeOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetImplicitHeaderModeOn(e.Value);
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

		private void loRaViewControl1_SymbTimeoutChanged(object sender, DecimalEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetSymbTimeout(e.Value);
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

		private void loRaViewControl1_PayloadCrcOnChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetPayloadCrcOn(e.Value);
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

		private void loRaViewControl1_CodingRateChanged(object sender, ByteEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetCodingRate(e.Value);
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

		private void loRaViewControl1_PayloadLengthChanged(object sender, ByteEventArg e)
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

		private void loRaViewControl1_PreambleLengthChanged(object sender, Int32EventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetPreambleLength(e.Value);
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

		private void loRaViewControl1_BandwidthChanged(object sender, ByteEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetBandwidth(e.Value);
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

		private void loRaViewControl1_SpreadingFactorChanged(object sender, ByteEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetSpreadingFactor(e.Value);
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

		private void loRaViewControl1_FreqHoppingPeriodChanged(object sender, ByteEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetFreqHoppingPeriod(e.Value);
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

		private void loRaViewControl1_MessageChanged(object sender, ByteArrayEventArg e)
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

		private void loRaViewControl1_StartStopChanged(object sender, BooleanEventArg e)
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

		private void loRaViewControl1_MaxPacketNumberChanged(object sender, Int32EventArg e)
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

		private void loRaViewControl1_PacketHandlerLog(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				int num = (int)new PacketLogForm()
				{
					Device = ((IDevice)this.device),
					AppSettings = this.appSettings
				}.ShowDialog();
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

		private void loRaViewControl1_PacketModeTxChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.PacketModeTx = e.Value;
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

		private void loRaViewControl1_LowDatarateOptimizeChanged(object sender, BooleanEventArg e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				this.OnError((byte)0, "-");
				this.device.SetLowDatarateOptimize(e.Value);
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
			this.tabLoRa = new TabPage();
			this.loRaViewControl1 = new LoRaViewControl();
			this.rBtnSynthesizer = new RadioButton();
			this.gBoxOperatingMode = new GroupBoxEx();
			this.rBtnTransmitterContinuous = new RadioButton();
			this.rBtnReceiverSingle = new RadioButton();
			this.rBtnReceiverCad = new RadioButton();
			this.rBtnSynthesizerTx = new RadioButton();
			this.rBtnSynthesizerRx = new RadioButton();
			this.rBtnStandby = new RadioButton();
			this.rBtnSleep = new RadioButton();
			this.rBtnReceiver = new RadioButton();
			this.gBoxIrqFlags = new GroupBoxEx();
			this.lbModeReady = new Label();
			this.ledRxDone = new Led();
			this.ledPayloadCrcError = new Led();
			this.label17 = new Label();
			this.ledValidHeader = new Led();
			this.label18 = new Label();
			this.label19 = new Label();
			this.ledRxTimeout = new Led();
			this.ledTxDone = new Led();
			this.ledCadDone = new Led();
			this.label20 = new Label();
			this.label21 = new Label();
			this.ledFhssChangeChannel = new Led();
			this.label22 = new Label();
			this.label23 = new Label();
			this.ledCadDetected = new Led();
			this.ledFifoNotEmpty = new Led();
			this.ledModeReady = new Led();
			this.groupBoxEx1 = new GroupBoxEx();
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
			this.tabControl1.SuspendLayout();
			this.tabCommon.SuspendLayout();
			this.tabLoRa.SuspendLayout();
			this.gBoxOperatingMode.SuspendLayout();
			this.gBoxIrqFlags.SuspendLayout();
			this.groupBoxEx1.SuspendLayout();
			this.SuspendLayout();
			this.tabControl1.Controls.Add((Control)this.tabCommon);
			this.tabControl1.Controls.Add((Control)this.tabLoRa);
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
			this.commonViewControl1.AgcAutoOn = true;
			this.commonViewControl1.AgcReference = 19;
			this.commonViewControl1.AgcReferenceLevel = 19;
			this.commonViewControl1.AgcStep1 = (byte)16;
			this.commonViewControl1.AgcStep2 = (byte)7;
			this.commonViewControl1.AgcStep3 = (byte)11;
			this.commonViewControl1.AgcStep4 = (byte)9;
			this.commonViewControl1.AgcStep5 = (byte)11;
			this.commonViewControl1.AgcThresh1 = 0;
			this.commonViewControl1.AgcThresh2 = 0;
			this.commonViewControl1.AgcThresh3 = 0;
			this.commonViewControl1.AgcThresh4 = 0;
			this.commonViewControl1.AgcThresh5 = 0;
			this.commonViewControl1.Band = BandEnum.AUTO;
			this.commonViewControl1.Dio0Mapping = DioMappingEnum.DIO_MAP_00;
			this.commonViewControl1.Dio1Mapping = DioMappingEnum.DIO_MAP_00;
			this.commonViewControl1.Dio2Mapping = DioMappingEnum.DIO_MAP_00;
			this.commonViewControl1.Dio3Mapping = DioMappingEnum.DIO_MAP_00;
			this.commonViewControl1.Dio4Mapping = DioMappingEnum.DIO_MAP_00;
			this.commonViewControl1.Dio5Mapping = DioMappingEnum.DIO_MAP_00;
			this.commonViewControl1.FastHopOn = true;
			this.commonViewControl1.ForceRxBandLowFrequencyOn = true;
			this.commonViewControl1.ForceTxBandLowFrequencyOn = true;
			CommonViewControl commonViewControl1 = this.commonViewControl1;
			int[] bits1 = new int[4];
			bits1[0] = 915000000;
			Decimal num1 = new Decimal(bits1);
			commonViewControl1.FrequencyRf = num1;
			CommonViewControl commonViewControl2 = this.commonViewControl1;
			int[] bits2 = new int[4];
			bits2[0] = 61;
			Decimal num2 = new Decimal(bits2);
			commonViewControl2.FrequencyStep = num2;
			CommonViewControl commonViewControl3 = this.commonViewControl1;
			int[] bits3 = new int[4];
			bits3[0] = 32000000;
			Decimal num3 = new Decimal(bits3);
			commonViewControl3.FrequencyXo = num3;
			this.commonViewControl1.LnaBoost = true;
			this.commonViewControl1.Location = new Point(0, 0);
			this.commonViewControl1.LowFrequencyModeOn = true;
			this.commonViewControl1.MaxOutputPower = new Decimal(new int[4]
      {
        132,
        0,
        0,
        65536
      });
			this.commonViewControl1.Name = "commonViewControl1";
			this.commonViewControl1.OcpOn = true;
			this.commonViewControl1.OcpTrim = new Decimal(new int[4]
      {
        1000,
        0,
        0,
        65536
      });
			this.commonViewControl1.OutputPower = new Decimal(new int[4]
      {
        132,
        0,
        0,
        65536
      });
			this.commonViewControl1.Pa20dBm = false;
			this.commonViewControl1.PaRamp = PaRampEnum.PaRamp_40;
			this.commonViewControl1.PaSelect = PaSelectEnum.RFO;
			CommonViewControl commonViewControl4 = this.commonViewControl1;
			int[] bits4 = new int[4];
			bits4[0] = 300000;
			Decimal num4 = new Decimal(bits4);
			commonViewControl4.PllBandwidth = num4;
			this.commonViewControl1.Size = new Size(799, 493);
			this.commonViewControl1.TabIndex = 0;
			this.commonViewControl1.TcxoInputOn = true;
			this.commonViewControl1.FrequencyXoChanged += new DecimalEventHandler(this.commonViewControl1_FrequencyXoChanged);
			this.commonViewControl1.BandChanged += new BandEventHandler(this.commonViewControl1_BandChanged);
			this.commonViewControl1.ForceTxBandLowFrequencyOnChanged += new BooleanEventHandler(this.commonViewControl1_ForceTxBandLowFrequencyOnChanged);
			this.commonViewControl1.ForceRxBandLowFrequencyOnChanged += new BooleanEventHandler(this.commonViewControl1_ForceRxBandLowFrequencyOnChanged);
			this.commonViewControl1.LowFrequencyModeOnChanged += new BooleanEventHandler(this.commonViewControl1_LowFrequencyModeOnChanged);
			this.commonViewControl1.FrequencyRfChanged += new DecimalEventHandler(this.commonViewControl1_FrequencyRfChanged);
			this.commonViewControl1.FastHopOnChanged += new BooleanEventHandler(this.commonViewControl1_FastHopOnChanged);
			this.commonViewControl1.PaModeChanged += new PaModeEventHandler(this.commonViewControl1_PaModeChanged);
			this.commonViewControl1.OutputPowerChanged += new DecimalEventHandler(this.commonViewControl1_OutputPowerChanged);
			this.commonViewControl1.MaxOutputPowerChanged += new DecimalEventHandler(this.commonViewControl1_MaxOutputPowerChanged);
			this.commonViewControl1.PaRampChanged += new PaRampEventHandler(this.commonViewControl1_PaRampChanged);
			this.commonViewControl1.OcpOnChanged += new BooleanEventHandler(this.commonViewControl1_OcpOnChanged);
			this.commonViewControl1.OcpTrimChanged += new DecimalEventHandler(this.commonViewControl1_OcpTrimChanged);
			this.commonViewControl1.Pa20dBmChanged += new BooleanEventHandler(this.commonViewControl1_Pa20dBmChanged);
			this.commonViewControl1.PllBandwidthChanged += new DecimalEventHandler(this.commonViewControl1_PllBandwidthChanged);
			this.commonViewControl1.AgcReferenceLevelChanged += new Int32EventHandler(this.commonViewControl1_AgcReferenceLevelChanged);
			this.commonViewControl1.AgcStepChanged += new AgcStepEventHandler(this.commonViewControl1_AgcStepChanged);
			this.commonViewControl1.LnaGainChanged += new LnaGainEventHandler(this.commonViewControl1_LnaGainChanged);
			this.commonViewControl1.LnaBoostChanged += new BooleanEventHandler(this.commonViewControl1_LnaBoostChanged);
			this.commonViewControl1.AgcAutoOnChanged += new BooleanEventHandler(this.commonViewControl1_AgcAutoOnChanged);
			this.commonViewControl1.TcxoInputChanged += new BooleanEventHandler(this.commonViewControl1_TcxoInputChanged);
			this.commonViewControl1.DioMappingChanged += new DioMappingEventHandler(this.commonViewControl1_DioMappingChanged);
			this.commonViewControl1.DocumentationChanged += new DocumentationChangedEventHandler(this.commonViewControl1_DocumentationChanged);
			this.tabLoRa.Controls.Add((Control)this.loRaViewControl1);
			this.tabLoRa.Location = new Point(4, 22);
			this.tabLoRa.Name = "tabLoRa";
			this.tabLoRa.Padding = new Padding(3);
			this.tabLoRa.Size = new Size(799, 493);
			this.tabLoRa.TabIndex = 1;
			this.tabLoRa.Text = "LoRa";
			this.tabLoRa.UseVisualStyleBackColor = true;
			this.loRaViewControl1.Bandwidth = (byte)7;
			this.loRaViewControl1.CadDetectedMask = true;
			this.loRaViewControl1.CadDoneMask = true;
			this.loRaViewControl1.CodingRate = (byte)1;
			this.loRaViewControl1.FhssChangeChannelMask = true;
			this.loRaViewControl1.FreqHoppingPeriod = (byte)0;
			LoRaViewControl loRaViewControl = this.loRaViewControl1;
			int[] bits5 = new int[4];
			bits5[0] = 32000000;
			Decimal num5 = new Decimal(bits5);
			loRaViewControl.FrequencyXo = num5;
			this.loRaViewControl1.ImplicitHeaderModeOn = true;
			this.loRaViewControl1.Location = new Point(0, 0);
			this.loRaViewControl1.LogEnabled = false;
			this.loRaViewControl1.MaxPacketNumber = 0;
			this.loRaViewControl1.LowDatarateOptimize = true;
			this.loRaViewControl1.Mode = OperatingModeEnum.Stdby;
			this.loRaViewControl1.Name = "loRaViewControl1";
			this.loRaViewControl1.PacketModeTx = false;
			this.loRaViewControl1.PacketNumber = 0;
			this.loRaViewControl1.Payload = new byte[0];
			this.loRaViewControl1.PayloadCrcErrorMask = true;
			this.loRaViewControl1.PayloadCrcOn = false;
			this.loRaViewControl1.PayloadLength = (byte)14;
			this.loRaViewControl1.PreambleLength = 8;
			this.loRaViewControl1.RxDoneMask = true;
			this.loRaViewControl1.RxTimeoutMask = true;
			this.loRaViewControl1.Size = new Size(799, 493);
			this.loRaViewControl1.SpreadingFactor = (byte)7;
			this.loRaViewControl1.StartStop = false;
			this.loRaViewControl1.SymbolTime = new Decimal(new int[4]
      {
        1024,
        0,
        0,
        393216
      });
			this.loRaViewControl1.SymbTimeout = new Decimal(new int[4]
      {
        1024,
        0,
        0,
        262144
      });
			this.loRaViewControl1.TabIndex = 0;
			this.loRaViewControl1.TxDoneMask = true;
			this.loRaViewControl1.ValidHeaderMask = true;
			this.loRaViewControl1.Error += new SemtechLib.General.Events.ErrorEventHandler(this.loRaViewControl1_Error);
			this.loRaViewControl1.RxTimeoutMaskChanged += new BooleanEventHandler(this.loRaViewControl1_RxTimeoutMaskChanged);
			this.loRaViewControl1.RxDoneMaskChanged += new BooleanEventHandler(this.loRaViewControl1_RxDoneMaskChanged);
			this.loRaViewControl1.PayloadCrcErrorMaskChanged += new BooleanEventHandler(this.loRaViewControl1_PayloadCrcErrorMaskChanged);
			this.loRaViewControl1.ValidHeaderMaskChanged += new BooleanEventHandler(this.loRaViewControl1_ValidHeaderMaskChanged);
			this.loRaViewControl1.TxDoneMaskChanged += new BooleanEventHandler(this.loRaViewControl1_TxDoneMaskChanged);
			this.loRaViewControl1.CadDoneMaskChanged += new BooleanEventHandler(this.loRaViewControl1_CadDoneMaskChanged);
			this.loRaViewControl1.FhssChangeChannelMaskChanged += new BooleanEventHandler(this.loRaViewControl1_FhssChangeChannelMaskChanged);
			this.loRaViewControl1.CadDetectedMaskChanged += new BooleanEventHandler(this.loRaViewControl1_CadDetectedMaskChanged);
			this.loRaViewControl1.ImplicitHeaderModeOnChanged += new BooleanEventHandler(this.loRaViewControl1_ImplicitHeaderModeOnChanged);
			this.loRaViewControl1.SymbTimeoutChanged += new DecimalEventHandler(this.loRaViewControl1_SymbTimeoutChanged);
			this.loRaViewControl1.PayloadCrcOnChanged += new BooleanEventHandler(this.loRaViewControl1_PayloadCrcOnChanged);
			this.loRaViewControl1.CodingRateChanged += new ByteEventHandler(this.loRaViewControl1_CodingRateChanged);
			this.loRaViewControl1.PayloadLengthChanged += new ByteEventHandler(this.loRaViewControl1_PayloadLengthChanged);
			this.loRaViewControl1.PreambleLengthChanged += new Int32EventHandler(this.loRaViewControl1_PreambleLengthChanged);
			this.loRaViewControl1.BandwidthChanged += new ByteEventHandler(this.loRaViewControl1_BandwidthChanged);
			this.loRaViewControl1.SpreadingFactorChanged += new ByteEventHandler(this.loRaViewControl1_SpreadingFactorChanged);
			this.loRaViewControl1.FreqHoppingPeriodChanged += new ByteEventHandler(this.loRaViewControl1_FreqHoppingPeriodChanged);
			this.loRaViewControl1.MessageChanged += new ByteArrayEventHandler(this.loRaViewControl1_MessageChanged);
			this.loRaViewControl1.StartStopChanged += new BooleanEventHandler(this.loRaViewControl1_StartStopChanged);
			this.loRaViewControl1.MaxPacketNumberChanged += new Int32EventHandler(this.loRaViewControl1_MaxPacketNumberChanged);
			this.loRaViewControl1.PacketHandlerLog += new EventHandler(this.loRaViewControl1_PacketHandlerLog);
			this.loRaViewControl1.PacketModeTxChanged += new BooleanEventHandler(this.loRaViewControl1_PacketModeTxChanged);
			this.loRaViewControl1.LowDatarateOptimizeChanged += new BooleanEventHandler(this.loRaViewControl1_LowDatarateOptimizeChanged);
			this.loRaViewControl1.DocumentationChanged += new DocumentationChangedEventHandler(this.commonViewControl1_DocumentationChanged);
			this.rBtnSynthesizer.AutoSize = true;
			this.rBtnSynthesizer.Location = new Point(16, 51);
			this.rBtnSynthesizer.Name = "rBtnSynthesizer";
			this.rBtnSynthesizer.Size = new Size(79, 17);
			this.rBtnSynthesizer.TabIndex = 2;
			this.rBtnSynthesizer.Text = "Synthesizer";
			this.rBtnSynthesizer.UseVisualStyleBackColor = true;
			this.rBtnSynthesizer.CheckedChanged += new EventHandler(this.rBtnOperatingMode_CheckedChanged);
			this.gBoxOperatingMode.Controls.Add((Control)this.rBtnTransmitterContinuous);
			this.gBoxOperatingMode.Controls.Add((Control)this.rBtnReceiverSingle);
			this.gBoxOperatingMode.Controls.Add((Control)this.rBtnReceiverCad);
			this.gBoxOperatingMode.Controls.Add((Control)this.rBtnSynthesizerTx);
			this.gBoxOperatingMode.Controls.Add((Control)this.rBtnSynthesizerRx);
			this.gBoxOperatingMode.Controls.Add((Control)this.rBtnStandby);
			this.gBoxOperatingMode.Controls.Add((Control)this.rBtnSleep);
			this.gBoxOperatingMode.Controls.Add((Control)this.rBtnReceiver);
			this.gBoxOperatingMode.Location = new Point(816, 367);
			this.gBoxOperatingMode.Name = "gBoxOperatingMode";
			this.gBoxOperatingMode.Size = new Size(189, 151);
			this.gBoxOperatingMode.TabIndex = 3;
			this.gBoxOperatingMode.TabStop = false;
			this.gBoxOperatingMode.Text = "Operating mode";
			this.gBoxOperatingMode.MouseEnter += new EventHandler(this.control_MouseEnter);
			this.gBoxOperatingMode.MouseLeave += new EventHandler(this.control_MouseLeave);
			this.rBtnTransmitterContinuous.AutoSize = true;
			this.rBtnTransmitterContinuous.Location = new Point(91, 87);
			this.rBtnTransmitterContinuous.Name = "rBtnTransmitterContinuous";
			this.rBtnTransmitterContinuous.Size = new Size(92, 17);
			this.rBtnTransmitterContinuous.TabIndex = 5;
			this.rBtnTransmitterContinuous.Text = "Tx continuous";
			this.rBtnTransmitterContinuous.UseVisualStyleBackColor = true;
			this.rBtnTransmitterContinuous.CheckedChanged += new EventHandler(this.rBtnOperatingMode_CheckedChanged);
			this.rBtnReceiverSingle.AutoSize = true;
			this.rBtnReceiverSingle.Location = new Point(12, 110);
			this.rBtnReceiverSingle.Name = "rBtnReceiverSingle";
			this.rBtnReceiverSingle.Size = new Size(70, 17);
			this.rBtnReceiverSingle.TabIndex = 6;
			this.rBtnReceiverSingle.Text = "Rx Single";
			this.rBtnReceiverSingle.UseVisualStyleBackColor = true;
			this.rBtnReceiverSingle.CheckedChanged += new EventHandler(this.rBtnOperatingMode_CheckedChanged);
			this.rBtnReceiverCad.AutoSize = true;
			this.rBtnReceiverCad.Location = new Point(91, 110);
			this.rBtnReceiverCad.Name = "rBtnReceiverCad";
			this.rBtnReceiverCad.Size = new Size(47, 17);
			this.rBtnReceiverCad.TabIndex = 7;
			this.rBtnReceiverCad.Text = "CAD";
			this.rBtnReceiverCad.UseVisualStyleBackColor = true;
			this.rBtnReceiverCad.CheckedChanged += new EventHandler(this.rBtnOperatingMode_CheckedChanged);
			this.rBtnSynthesizerTx.AutoSize = true;
			this.rBtnSynthesizerTx.Location = new Point(91, 64);
			this.rBtnSynthesizerTx.Name = "rBtnSynthesizerTx";
			this.rBtnSynthesizerTx.Size = new Size(70, 17);
			this.rBtnSynthesizerTx.TabIndex = 3;
			this.rBtnSynthesizerTx.Text = "Synth. Tx";
			this.rBtnSynthesizerTx.UseVisualStyleBackColor = true;
			this.rBtnSynthesizerTx.CheckedChanged += new EventHandler(this.rBtnOperatingMode_CheckedChanged);
			this.rBtnSynthesizerRx.AutoSize = true;
			this.rBtnSynthesizerRx.Location = new Point(12, 64);
			this.rBtnSynthesizerRx.Name = "rBtnSynthesizerRx";
			this.rBtnSynthesizerRx.Size = new Size(71, 17);
			this.rBtnSynthesizerRx.TabIndex = 2;
			this.rBtnSynthesizerRx.Text = "Synth. Rx";
			this.rBtnSynthesizerRx.UseVisualStyleBackColor = true;
			this.rBtnSynthesizerRx.CheckedChanged += new EventHandler(this.rBtnOperatingMode_CheckedChanged);
			this.rBtnStandby.AutoSize = true;
			this.rBtnStandby.Checked = true;
			this.rBtnStandby.Location = new Point(91, 41);
			this.rBtnStandby.Name = "rBtnStandby";
			this.rBtnStandby.Size = new Size(64, 17);
			this.rBtnStandby.TabIndex = 1;
			this.rBtnStandby.TabStop = true;
			this.rBtnStandby.Text = "Standby";
			this.rBtnStandby.UseVisualStyleBackColor = true;
			this.rBtnStandby.CheckedChanged += new EventHandler(this.rBtnOperatingMode_CheckedChanged);
			this.rBtnSleep.AutoSize = true;
			this.rBtnSleep.Location = new Point(12, 41);
			this.rBtnSleep.Name = "rBtnSleep";
			this.rBtnSleep.Size = new Size(52, 17);
			this.rBtnSleep.TabIndex = 0;
			this.rBtnSleep.Text = "Sleep";
			this.rBtnSleep.UseVisualStyleBackColor = true;
			this.rBtnSleep.CheckedChanged += new EventHandler(this.rBtnOperatingMode_CheckedChanged);
			this.rBtnReceiver.AutoSize = true;
			this.rBtnReceiver.Location = new Point(12, 87);
			this.rBtnReceiver.Name = "rBtnReceiver";
			this.rBtnReceiver.Size = new Size(38, 17);
			this.rBtnReceiver.TabIndex = 4;
			this.rBtnReceiver.Text = "Rx";
			this.rBtnReceiver.UseVisualStyleBackColor = true;
			this.rBtnReceiver.CheckedChanged += new EventHandler(this.rBtnOperatingMode_CheckedChanged);
			this.gBoxIrqFlags.Controls.Add((Control)this.lbModeReady);
			this.gBoxIrqFlags.Controls.Add((Control)this.ledRxDone);
			this.gBoxIrqFlags.Controls.Add((Control)this.ledPayloadCrcError);
			this.gBoxIrqFlags.Controls.Add((Control)this.label17);
			this.gBoxIrqFlags.Controls.Add((Control)this.ledValidHeader);
			this.gBoxIrqFlags.Controls.Add((Control)this.label18);
			this.gBoxIrqFlags.Controls.Add((Control)this.label19);
			this.gBoxIrqFlags.Controls.Add((Control)this.ledRxTimeout);
			this.gBoxIrqFlags.Controls.Add((Control)this.ledTxDone);
			this.gBoxIrqFlags.Controls.Add((Control)this.ledCadDone);
			this.gBoxIrqFlags.Controls.Add((Control)this.label20);
			this.gBoxIrqFlags.Controls.Add((Control)this.label21);
			this.gBoxIrqFlags.Controls.Add((Control)this.ledFhssChangeChannel);
			this.gBoxIrqFlags.Controls.Add((Control)this.label22);
			this.gBoxIrqFlags.Controls.Add((Control)this.label23);
			this.gBoxIrqFlags.Controls.Add((Control)this.ledCadDetected);
			this.gBoxIrqFlags.Location = new Point(816, 25);
			this.gBoxIrqFlags.Name = "gBoxIrqFlags";
			this.gBoxIrqFlags.Size = new Size(189, 188);
			this.gBoxIrqFlags.TabIndex = 1;
			this.gBoxIrqFlags.TabStop = false;
			this.gBoxIrqFlags.Text = "Irq flags";
			this.gBoxIrqFlags.MouseEnter += new EventHandler(this.control_MouseEnter);
			this.gBoxIrqFlags.MouseLeave += new EventHandler(this.control_MouseLeave);
			this.lbModeReady.AutoSize = true;
			this.lbModeReady.Location = new Point(55, 20);
			this.lbModeReady.Name = "lbModeReady";
			this.lbModeReady.Size = new Size(58, 13);
			this.lbModeReady.TabIndex = 1;
			this.lbModeReady.Text = "RxTimeout";
			this.ledRxDone.BackColor = Color.Transparent;
			this.ledRxDone.LedColor = Color.Green;
			this.ledRxDone.LedSize = new Size(11, 11);
			this.ledRxDone.Location = new Point(34, 40);
			this.ledRxDone.Name = "ledRxDone";
			this.ledRxDone.Size = new Size(15, 15);
			this.ledRxDone.TabIndex = 2;
			this.ledRxDone.Text = "Rx done";
			this.ledRxDone.Click += new EventHandler(this.ledRxDone_Click);
			this.ledPayloadCrcError.BackColor = Color.Transparent;
			this.ledPayloadCrcError.LedColor = Color.Green;
			this.ledPayloadCrcError.LedSize = new Size(11, 11);
			this.ledPayloadCrcError.Location = new Point(34, 61);
			this.ledPayloadCrcError.Name = "ledPayloadCrcError";
			this.ledPayloadCrcError.Size = new Size(15, 15);
			this.ledPayloadCrcError.TabIndex = 4;
			this.ledPayloadCrcError.Text = "Payload CRC error";
			this.ledPayloadCrcError.Click += new EventHandler(this.ledPayloadCrcError_Click);
			this.label17.AutoSize = true;
			this.label17.Location = new Point(55, 83);
			this.label17.Name = "label17";
			this.label17.Size = new Size(65, 13);
			this.label17.TabIndex = 7;
			this.label17.Text = "ValidHeader";
			this.ledValidHeader.BackColor = Color.Transparent;
			this.ledValidHeader.LedColor = Color.Green;
			this.ledValidHeader.LedSize = new Size(11, 11);
			this.ledValidHeader.Location = new Point(34, 82);
			this.ledValidHeader.Name = "ledValidHeader";
			this.ledValidHeader.Size = new Size(15, 15);
			this.ledValidHeader.TabIndex = 6;
			this.ledValidHeader.Text = "Valid header";
			this.ledValidHeader.Click += new EventHandler(this.ledValidHeader_Click);
			this.label18.AutoSize = true;
			this.label18.Location = new Point(55, 62);
			this.label18.Name = "label18";
			this.label18.Size = new Size(83, 13);
			this.label18.TabIndex = 5;
			this.label18.Text = "PayloadCrcError";
			this.label19.AutoSize = true;
			this.label19.Location = new Point(55, 41);
			this.label19.Name = "label19";
			this.label19.Size = new Size(46, 13);
			this.label19.TabIndex = 3;
			this.label19.Text = "RxDone";
			this.ledRxTimeout.BackColor = Color.Transparent;
			this.ledRxTimeout.LedColor = Color.Green;
			this.ledRxTimeout.LedSize = new Size(11, 11);
			this.ledRxTimeout.Location = new Point(34, 19);
			this.ledRxTimeout.Name = "ledRxTimeout";
			this.ledRxTimeout.Size = new Size(15, 15);
			this.ledRxTimeout.TabIndex = 0;
			this.ledRxTimeout.Text = "Rx timeout";
			this.ledRxTimeout.Click += new EventHandler(this.ledRxTimeout_Click);
			this.ledTxDone.BackColor = Color.Transparent;
			this.ledTxDone.LedColor = Color.Green;
			this.ledTxDone.LedSize = new Size(11, 11);
			this.ledTxDone.Location = new Point(34, 103);
			this.ledTxDone.Name = "ledTxDone";
			this.ledTxDone.Size = new Size(15, 15);
			this.ledTxDone.TabIndex = 8;
			this.ledTxDone.Text = "Tx done";
			this.ledTxDone.Click += new EventHandler(this.ledTxDone_Click);
			this.ledCadDone.BackColor = Color.Transparent;
			this.ledCadDone.LedColor = Color.Green;
			this.ledCadDone.LedSize = new Size(11, 11);
			this.ledCadDone.Location = new Point(34, 124);
			this.ledCadDone.Name = "ledCadDone";
			this.ledCadDone.Size = new Size(15, 15);
			this.ledCadDone.TabIndex = 10;
			this.ledCadDone.Text = "CAD done";
			this.ledCadDone.Click += new EventHandler(this.ledCadDone_Click);
			this.label20.AutoSize = true;
			this.label20.Location = new Point(55, 167);
			this.label20.Name = "label20";
			this.label20.Size = new Size(70, 13);
			this.label20.TabIndex = 15;
			this.label20.Text = "CadDetected";
			this.label21.AutoSize = true;
			this.label21.Location = new Point(55, 146);
			this.label21.Name = "label21";
			this.label21.Size = new Size(105, 13);
			this.label21.TabIndex = 13;
			this.label21.Text = "FhssChangeChannel";
			this.ledFhssChangeChannel.BackColor = Color.Transparent;
			this.ledFhssChangeChannel.LedColor = Color.Green;
			this.ledFhssChangeChannel.LedSize = new Size(11, 11);
			this.ledFhssChangeChannel.Location = new Point(34, 145);
			this.ledFhssChangeChannel.Name = "ledFhssChangeChannel";
			this.ledFhssChangeChannel.Size = new Size(15, 15);
			this.ledFhssChangeChannel.TabIndex = 12;
			this.ledFhssChangeChannel.Text = "FHSS change channel";
			this.ledFhssChangeChannel.Click += new EventHandler(this.ledFhssChangeChannel_Click);
			this.label22.AutoSize = true;
			this.label22.Location = new Point(55, 125);
			this.label22.Name = "label22";
			this.label22.Size = new Size(52, 13);
			this.label22.TabIndex = 11;
			this.label22.Text = "CadDone";
			this.label23.AutoSize = true;
			this.label23.Location = new Point(55, 104);
			this.label23.Name = "label23";
			this.label23.Size = new Size(45, 13);
			this.label23.TabIndex = 9;
			this.label23.Text = "TxDone";
			this.ledCadDetected.BackColor = Color.Transparent;
			this.ledCadDetected.LedColor = Color.Green;
			this.ledCadDetected.LedSize = new Size(11, 11);
			this.ledCadDetected.Location = new Point(34, 166);
			this.ledCadDetected.Name = "ledCadDetected";
			this.ledCadDetected.Size = new Size(15, 15);
			this.ledCadDetected.TabIndex = 14;
			this.ledCadDetected.Text = "CAD detected";
			this.ledCadDetected.Click += new EventHandler(this.ledCadDetected_Click);
			this.ledFifoNotEmpty.BackColor = Color.Transparent;
			this.ledFifoNotEmpty.LedColor = Color.Green;
			this.ledFifoNotEmpty.LedSize = new Size(11, 11);
			this.ledFifoNotEmpty.Location = new Point(34, 220);
			this.ledFifoNotEmpty.Name = "ledFifoNotEmpty";
			this.ledFifoNotEmpty.Size = new Size(15, 15);
			this.ledFifoNotEmpty.TabIndex = 18;
			this.ledFifoNotEmpty.Text = "led1";
			this.ledModeReady.BackColor = Color.Transparent;
			this.ledModeReady.LedColor = Color.Green;
			this.ledModeReady.LedSize = new Size(11, 11);
			this.ledModeReady.Location = new Point(34, 19);
			this.ledModeReady.Name = "ledModeReady";
			this.ledModeReady.Size = new Size(15, 15);
			this.ledModeReady.TabIndex = 0;
			this.ledModeReady.Text = "Rx timeout";
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
			this.groupBoxEx1.Location = new Point(816, 219);
			this.groupBoxEx1.Name = "groupBoxEx1";
			this.groupBoxEx1.Size = new Size(189, 142);
			this.groupBoxEx1.TabIndex = 2;
			this.groupBoxEx1.TabStop = false;
			this.groupBoxEx1.Text = "Modem status";
			this.label42.AutoSize = true;
			this.label42.Location = new Point(58, 28);
			this.label42.Name = "label42";
			this.label42.Size = new Size(68, 13);
			this.label42.TabIndex = 1;
			this.label42.Text = "Modem clear";
			this.ledSignalDetected.BackColor = Color.Transparent;
			this.ledSignalDetected.LedColor = Color.Green;
			this.ledSignalDetected.LedSize = new Size(11, 11);
			this.ledSignalDetected.Location = new Point(34, 111);
			this.ledSignalDetected.Name = "ledSignalDetected";
			this.ledSignalDetected.Size = new Size(15, 15);
			this.ledSignalDetected.TabIndex = 8;
			this.ledSignalDetected.Text = "Signal detected";
			this.label45.AutoSize = true;
			this.label45.Location = new Point(58, 112);
			this.label45.Name = "label45";
			this.label45.Size = new Size(81, 13);
			this.label45.TabIndex = 9;
			this.label45.Text = "Signal detected";
			this.ledSignalSynchronized.BackColor = Color.Transparent;
			this.ledSignalSynchronized.LedColor = Color.Green;
			this.ledSignalSynchronized.LedSize = new Size(11, 11);
			this.ledSignalSynchronized.Location = new Point(34, 90);
			this.ledSignalSynchronized.Name = "ledSignalSynchronized";
			this.ledSignalSynchronized.Size = new Size(15, 15);
			this.ledSignalSynchronized.TabIndex = 6;
			this.ledSignalSynchronized.Text = "Signal synchronized";
			this.label43.AutoSize = true;
			this.label43.Location = new Point(58, 91);
			this.label43.Name = "label43";
			this.label43.Size = new Size(101, 13);
			this.label43.TabIndex = 7;
			this.label43.Text = "Signal synchronized";
			this.ledRxOnGoing.BackColor = Color.Transparent;
			this.ledRxOnGoing.LedColor = Color.Green;
			this.ledRxOnGoing.LedSize = new Size(11, 11);
			this.ledRxOnGoing.Location = new Point(34, 69);
			this.ledRxOnGoing.Name = "ledRxOnGoing";
			this.ledRxOnGoing.Size = new Size(15, 15);
			this.ledRxOnGoing.TabIndex = 4;
			this.ledRxOnGoing.Text = "Rx on going";
			this.label41.AutoSize = true;
			this.label41.Location = new Point(58, 70);
			this.label41.Name = "label41";
			this.label41.Size = new Size(64, 13);
			this.label41.TabIndex = 5;
			this.label41.Text = "Rx on going";
			this.ledHeaderInfoValid.BackColor = Color.Transparent;
			this.ledHeaderInfoValid.LedColor = Color.Green;
			this.ledHeaderInfoValid.LedSize = new Size(11, 11);
			this.ledHeaderInfoValid.Location = new Point(34, 48);
			this.ledHeaderInfoValid.Name = "ledHeaderInfoValid";
			this.ledHeaderInfoValid.Size = new Size(15, 15);
			this.ledHeaderInfoValid.TabIndex = 2;
			this.ledHeaderInfoValid.Text = "Header info valid";
			this.label44.AutoSize = true;
			this.label44.Location = new Point(58, 49);
			this.label44.Name = "label44";
			this.label44.Size = new Size(87, 13);
			this.label44.TabIndex = 3;
			this.label44.Text = "Header info valid";
			this.ledModemClear.BackColor = Color.Transparent;
			this.ledModemClear.LedColor = Color.Green;
			this.ledModemClear.LedSize = new Size(11, 11);
			this.ledModemClear.Location = new Point(34, 27);
			this.ledModemClear.Name = "ledModemClear";
			this.ledModemClear.Size = new Size(15, 15);
			this.ledModemClear.TabIndex = 0;
			this.ledModemClear.Text = "Modem clear";
			this.AutoScaleDimensions = new SizeF(6f, 13f);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add((Control)this.groupBoxEx1);
			this.Controls.Add((Control)this.gBoxOperatingMode);
			this.Controls.Add((Control)this.tabControl1);
			this.Controls.Add((Control)this.gBoxIrqFlags);
			this.Name = "DeviceViewControl";
			this.Size = new Size(1008, 525);
			this.tabControl1.ResumeLayout(false);
			this.tabCommon.ResumeLayout(false);
			this.tabLoRa.ResumeLayout(false);
			this.gBoxOperatingMode.ResumeLayout(false);
			this.gBoxOperatingMode.PerformLayout();
			this.gBoxIrqFlags.ResumeLayout(false);
			this.gBoxIrqFlags.PerformLayout();
			this.groupBoxEx1.ResumeLayout(false);
			this.groupBoxEx1.PerformLayout();
			this.ResumeLayout(false);
		}
	}
}

// Decompiled with JetBrains decompiler
// Type: SemtechLib.Devices.SX1276.UI.Controls.SequencerViewControl
// Assembly: SemtechLib.Devices.SX1276.UI, Version=2.0.0.0, Culture=neutral, PublicKeyToken=a3cee8af388a4f11
// MVID: 2B98C92B-3345-4D34-A253-90690D8C71AF
// Assembly location: C:\Tools\Semtech\SX1276SKA\SemtechLib.Devices.SX1276.UI.dll

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
	public class SequencerViewControl : UserControl, INotifyDocumentationChanged
	{
		private Decimal timer1Resol = new Decimal(64, 0, 0, false, (byte)3);
		private Decimal timer2Resol = new Decimal(64, 0, 0, false, (byte)3);
		private IContainer components;
		private ErrorProvider errorProvider;
		private Button btnSequencerStop;
		private Button btnSequencerStart;
		private Label label1;
		private Label label22;
		private ComboBox cBoxTimer1Resolution;
		private ComboBox cBoxFromPacketReceived;
		private ComboBox cBoxFromRxTimeout;
		private ComboBox cBoxFromReceive;
		private ComboBox cBoxFromTransmit;
		private ComboBox cBoxFromIdle;
		private ComboBox cBoxSeqLowPowerState;
		private ComboBox cBoxFromStart;
		private ComboBox cBoxSeqLowPowerMode;
		private Label label3;
		private Label label2;
		private ComboBox cBoxSeqTimer2Resolution;
		private NumericUpDownEx nudSeqTimer2Coefficient;
		private NumericUpDownEx nudSeqTimer1Coefficient;
		private Label label5;
		private Label label4;
		private Label label7;
		private Label label9;
		private Label label8;
		private Label label6;
		private Label lblTimer2Value;
		private Label lblTimer1Value;
		private Label label12;
		private Label label11;
		private Label label20;
		private Label label19;
		private Label label18;
		private Label label17;
		private Label label16;
		private Label label15;
		private Label label14;
		private Label label13;
		private Label label10;
		private Led ledSequencerModeStatus;
		private ToolTip toolTip1;

		public IdleMode IdleMode
		{
			get
			{
				return (IdleMode)this.cBoxSeqLowPowerMode.SelectedIndex;
			}
			set
			{
				this.cBoxSeqLowPowerMode.SelectedIndexChanged -= new EventHandler(this.cBoxSeqLowPowerMode_SelectedIndexChanged);
				this.cBoxSeqLowPowerMode.SelectedIndex = (int)value;
				this.cBoxSeqLowPowerMode.SelectedIndexChanged += new EventHandler(this.cBoxSeqLowPowerMode_SelectedIndexChanged);
			}
		}

		public FromStart FromStart
		{
			get
			{
				return (FromStart)this.cBoxFromStart.SelectedIndex;
			}
			set
			{
				this.cBoxFromStart.SelectedIndexChanged -= new EventHandler(this.cBoxFromStart_SelectedIndexChanged);
				this.cBoxFromStart.SelectedIndex = (int)value;
				this.cBoxFromStart.SelectedIndexChanged += new EventHandler(this.cBoxFromStart_SelectedIndexChanged);
			}
		}

		public LowPowerSelection LowPowerSelection
		{
			get
			{
				return (LowPowerSelection)this.cBoxSeqLowPowerState.SelectedIndex;
			}
			set
			{
				this.cBoxSeqLowPowerState.SelectedIndexChanged -= new EventHandler(this.cBoxSeqLowPowerState_SelectedIndexChanged);
				this.cBoxSeqLowPowerState.SelectedIndex = (int)value;
				this.cBoxSeqLowPowerState.SelectedIndexChanged += new EventHandler(this.cBoxSeqLowPowerState_SelectedIndexChanged);
			}
		}

		public FromIdle FromIdle
		{
			get
			{
				return (FromIdle)this.cBoxFromIdle.SelectedIndex;
			}
			set
			{
				this.cBoxFromIdle.SelectedIndexChanged -= new EventHandler(this.cBoxFromIdle_SelectedIndexChanged);
				this.cBoxFromIdle.SelectedIndex = (int)value;
				this.cBoxFromIdle.SelectedIndexChanged += new EventHandler(this.cBoxFromIdle_SelectedIndexChanged);
			}
		}

		public FromTransmit FromTransmit
		{
			get
			{
				return (FromTransmit)this.cBoxFromTransmit.SelectedIndex;
			}
			set
			{
				this.cBoxFromTransmit.SelectedIndexChanged -= new EventHandler(this.cBoxFromTransmit_SelectedIndexChanged);
				this.cBoxFromTransmit.SelectedIndex = (int)value;
				this.cBoxFromTransmit.SelectedIndexChanged += new EventHandler(this.cBoxFromTransmit_SelectedIndexChanged);
			}
		}

		public FromReceive FromReceive
		{
			get
			{
				return (FromReceive)this.cBoxFromReceive.SelectedIndex;
			}
			set
			{
				this.cBoxFromReceive.SelectedIndexChanged -= new EventHandler(this.cBoxFromReceive_SelectedIndexChanged);
				this.cBoxFromReceive.SelectedIndex = (int)value;
				this.cBoxFromReceive.SelectedIndexChanged += new EventHandler(this.cBoxFromReceive_SelectedIndexChanged);
			}
		}

		public FromRxTimeout FromRxTimeout
		{
			get
			{
				return (FromRxTimeout)this.cBoxFromRxTimeout.SelectedIndex;
			}
			set
			{
				this.cBoxFromRxTimeout.SelectedIndexChanged -= new EventHandler(this.cBoxFromRxTimeout_SelectedIndexChanged);
				this.cBoxFromRxTimeout.SelectedIndex = (int)value;
				this.cBoxFromRxTimeout.SelectedIndexChanged += new EventHandler(this.cBoxFromRxTimeout_SelectedIndexChanged);
			}
		}

		public FromPacketReceived FromPacketReceived
		{
			get
			{
				return (FromPacketReceived)this.cBoxFromPacketReceived.SelectedIndex;
			}
			set
			{
				this.cBoxFromPacketReceived.SelectedIndexChanged -= new EventHandler(this.cBoxFromPacketReceived_SelectedIndexChanged);
				this.cBoxFromPacketReceived.SelectedIndex = (int)value;
				this.cBoxFromPacketReceived.SelectedIndexChanged += new EventHandler(this.cBoxFromPacketReceived_SelectedIndexChanged);
			}
		}

		public TimerResolution Timer1Resolution
		{
			get
			{
				return (TimerResolution)this.cBoxTimer1Resolution.SelectedIndex;
			}
			set
			{
				this.cBoxTimer1Resolution.SelectedIndexChanged -= new EventHandler(this.cBoxTimer1Resolution_SelectedIndexChanged);
				this.cBoxTimer1Resolution.SelectedIndex = (int)value;
				switch (value)
				{
					case TimerResolution.Res000064:
						this.timer1Resol = new Decimal(64, 0, 0, false, (byte)3);
						this.nudSeqTimer1Coefficient.Enabled = true;
						break;
					case TimerResolution.Res004100:
						this.timer1Resol = new Decimal(41, 0, 0, false, (byte)1);
						this.nudSeqTimer1Coefficient.Enabled = true;
						break;
					case TimerResolution.Res262000:
						this.timer1Resol = new Decimal(262);
						this.nudSeqTimer1Coefficient.Enabled = true;
						break;
					default:
						this.nudSeqTimer1Coefficient.Enabled = false;
						break;
				}
				if (value == TimerResolution.OFF)
					this.lblTimer1Value.Text = "OFF";
				else
					this.lblTimer1Value.Text = ((int)(timer1Resol * nudSeqTimer1Coefficient.Value * new Decimal(10000, 0, 0, false, (byte)1))).ToString();
				if (this.Timer2Resolution != TimerResolution.OFF && value == TimerResolution.OFF)
					this.errorProvider.SetError((Control)this.lblTimer2Value, "When Timer2 is enabled the Timer1 must be enabled also.");
				else
					this.errorProvider.SetError((Control)this.lblTimer2Value, "");
				this.cBoxTimer1Resolution.SelectedIndexChanged += new EventHandler(this.cBoxTimer1Resolution_SelectedIndexChanged);
			}
		}

		public TimerResolution Timer2Resolution
		{
			get
			{
				return (TimerResolution)this.cBoxSeqTimer2Resolution.SelectedIndex;
			}
			set
			{
				this.cBoxSeqTimer2Resolution.SelectedIndexChanged -= new EventHandler(this.cBoxSeqTimer2Resolution_SelectedIndexChanged);
				this.cBoxSeqTimer2Resolution.SelectedIndex = (int)value;
				switch (value)
				{
					case TimerResolution.Res000064:
						this.timer2Resol = new Decimal(64, 0, 0, false, (byte)3);
						this.nudSeqTimer2Coefficient.Enabled = true;
						break;
					case TimerResolution.Res004100:
						this.timer2Resol = new Decimal(41, 0, 0, false, (byte)1);
						this.nudSeqTimer2Coefficient.Enabled = true;
						break;
					case TimerResolution.Res262000:
						this.timer2Resol = new Decimal(262);
						this.nudSeqTimer2Coefficient.Enabled = true;
						break;
					default:
						this.nudSeqTimer2Coefficient.Enabled = false;
						break;
				}
				if (value == TimerResolution.OFF)
					this.lblTimer2Value.Text = "OFF";
				else
					this.lblTimer2Value.Text = ((int)(this.timer2Resol * this.nudSeqTimer2Coefficient.Value * new Decimal(10000, 0, 0, false, (byte)1))).ToString();
				if (this.Timer1Resolution == TimerResolution.OFF && value != TimerResolution.OFF)
					this.errorProvider.SetError((Control)this.lblTimer2Value, "When Timer2 is enabled the Timer1 must be enabled also.");
				else
					this.errorProvider.SetError((Control)this.lblTimer2Value, "");
				this.cBoxSeqTimer2Resolution.SelectedIndexChanged += new EventHandler(this.cBoxSeqTimer2Resolution_SelectedIndexChanged);
			}
		}

		public byte Timer1Coef
		{
			get
			{
				return (byte)this.nudSeqTimer1Coefficient.Value;
			}
			set
			{
				try
				{
					this.nudSeqTimer1Coefficient.ValueChanged -= new EventHandler(this.nudSeqTimer1Coefficient_ValueChanged);
					this.nudSeqTimer1Coefficient.Value = (Decimal)value;
					if (this.Timer1Resolution == TimerResolution.OFF)
						this.lblTimer1Value.Text = "OFF";
					else
						this.lblTimer1Value.Text = ((int)(this.timer1Resol * this.nudSeqTimer1Coefficient.Value * new Decimal(10000, 0, 0, false, (byte)1))).ToString();
				}
				catch
				{
				}
				finally
				{
					this.nudSeqTimer1Coefficient.ValueChanged += new EventHandler(this.nudSeqTimer1Coefficient_ValueChanged);
				}
			}
		}

		public byte Timer2Coef
		{
			get
			{
				return (byte)this.nudSeqTimer2Coefficient.Value;
			}
			set
			{
				try
				{
					this.nudSeqTimer2Coefficient.ValueChanged -= new EventHandler(this.nudSeqTimer2Coefficient_ValueChanged);
					this.nudSeqTimer2Coefficient.Value = (Decimal)value;
					if (this.Timer2Resolution == TimerResolution.OFF)
						this.lblTimer2Value.Text = "OFF";
					else
						this.lblTimer2Value.Text = ((int)(this.timer2Resol * this.nudSeqTimer2Coefficient.Value * new Decimal(10000, 0, 0, false, (byte)1))).ToString();
				}
				catch
				{
				}
				finally
				{
					this.nudSeqTimer2Coefficient.ValueChanged += new EventHandler(this.nudSeqTimer2Coefficient_ValueChanged);
				}
			}
		}

		public event EventHandler SequencerStartChanged;

		public event EventHandler SequencerStopChanged;

		public event IdleModeEventHandler IdleModeChanged;

		public event FromStartEventHandler FromStartChanged;

		public event LowPowerSelectionEventHandler LowPowerSelectionChanged;

		public event FromIdleEventHandler FromIdleChanged;

		public event FromTransmitEventHandler FromTransmitChanged;

		public event FromReceiveEventHandler FromReceiveChanged;

		public event FromRxTimeoutEventHandler FromRxTimeoutChanged;

		public event FromPacketReceivedEventHandler FromPacketReceivedChanged;

		public event TimerResolutionEventHandler Timer1ResolutionChanged;

		public event TimerResolutionEventHandler Timer2ResolutionChanged;

		public event ByteEventHandler Timer1CoefChanged;

		public event ByteEventHandler Timer2CoefChanged;

		public event DocumentationChangedEventHandler DocumentationChanged;

		public SequencerViewControl()
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(SequencerViewControl));
			this.errorProvider = new ErrorProvider(this.components);
			this.lblTimer2Value = new Label();
			this.btnSequencerStop = new Button();
			this.btnSequencerStart = new Button();
			this.label1 = new Label();
			this.label22 = new Label();
			this.cBoxTimer1Resolution = new ComboBox();
			this.cBoxSeqLowPowerMode = new ComboBox();
			this.cBoxFromStart = new ComboBox();
			this.cBoxSeqLowPowerState = new ComboBox();
			this.cBoxFromIdle = new ComboBox();
			this.cBoxFromTransmit = new ComboBox();
			this.cBoxFromReceive = new ComboBox();
			this.cBoxFromRxTimeout = new ComboBox();
			this.cBoxFromPacketReceived = new ComboBox();
			this.cBoxSeqTimer2Resolution = new ComboBox();
			this.label2 = new Label();
			this.label3 = new Label();
			this.label4 = new Label();
			this.label5 = new Label();
			this.label6 = new Label();
			this.label7 = new Label();
			this.label8 = new Label();
			this.label9 = new Label();
			this.lblTimer1Value = new Label();
			this.label11 = new Label();
			this.label12 = new Label();
			this.label10 = new Label();
			this.label13 = new Label();
			this.label14 = new Label();
			this.label15 = new Label();
			this.label16 = new Label();
			this.label17 = new Label();
			this.label18 = new Label();
			this.label19 = new Label();
			this.label20 = new Label();
			this.nudSeqTimer2Coefficient = new NumericUpDownEx();
			this.nudSeqTimer1Coefficient = new NumericUpDownEx();
			this.ledSequencerModeStatus = new Led();
			this.toolTip1 = new ToolTip(this.components);
			((ISupportInitialize)this.errorProvider).BeginInit();
			this.nudSeqTimer2Coefficient.BeginInit();
			this.nudSeqTimer1Coefficient.BeginInit();
			this.SuspendLayout();
			this.errorProvider.ContainerControl = (ContainerControl)this;
			this.lblTimer2Value.BackColor = Color.Transparent;
			this.lblTimer2Value.BorderStyle = BorderStyle.Fixed3D;
			this.errorProvider.SetIconPadding((Control)this.lblTimer2Value, 30);
			this.lblTimer2Value.Location = new Point(502, 391);
			this.lblTimer2Value.Margin = new Padding(3);
			this.lblTimer2Value.Name = "lblTimer2Value";
			this.lblTimer2Value.Size = new Size(98, 20);
			this.lblTimer2Value.TabIndex = 35;
			this.lblTimer2Value.Text = "0";
			this.lblTimer2Value.TextAlign = ContentAlignment.MiddleLeft;
			this.btnSequencerStop.Location = new Point(447, 75);
			this.btnSequencerStop.Name = "btnSequencerStop";
			this.btnSequencerStop.Size = new Size(89, 23);
			this.btnSequencerStop.TabIndex = 2;
			this.btnSequencerStop.Text = "Stop";
			this.btnSequencerStop.UseVisualStyleBackColor = true;
			this.btnSequencerStop.Click += new EventHandler(this.btnSequencerStop_Click);
			this.btnSequencerStart.Location = new Point(352, 75);
			this.btnSequencerStart.Name = "btnSequencerStart";
			this.btnSequencerStart.Size = new Size(89, 23);
			this.btnSequencerStart.TabIndex = 1;
			this.btnSequencerStart.Text = "Start";
			this.btnSequencerStart.UseVisualStyleBackColor = true;
			this.btnSequencerStart.Click += new EventHandler(this.btnSequencerStart_Click);
			this.label1.AutoSize = true;
			this.label1.Location = new Point(194, 320);
			this.label1.Margin = new Padding(3);
			this.label1.Name = "label1";
			this.label1.Size = new Size(90, 13);
			this.label1.TabIndex = 19;
			this.label1.Text = "Timer 1 resolution";
			this.label22.AutoSize = true;
			this.label22.Location = new Point(307, 343);
			this.label22.Name = "label22";
			this.label22.Size = new Size(18, 13);
			this.label22.TabIndex = 22;
			this.label22.Text = "µs";
			this.cBoxTimer1Resolution.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxTimer1Resolution.FormattingEnabled = true;
			this.cBoxTimer1Resolution.Items.AddRange(new object[4]
      {
        (object) "OFF",
        (object) "64",
        (object) "4'100",
        (object) "262'000"
      });
			this.cBoxTimer1Resolution.Location = new Point(177, 339);
			this.cBoxTimer1Resolution.Name = "cBoxTimer1Resolution";
			this.cBoxTimer1Resolution.Size = new Size(124, 21);
			this.cBoxTimer1Resolution.TabIndex = 21;
			this.cBoxTimer1Resolution.SelectedIndexChanged += new EventHandler(this.cBoxTimer1Resolution_SelectedIndexChanged);
			this.cBoxSeqLowPowerMode.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxSeqLowPowerMode.FormattingEnabled = true;
			this.cBoxSeqLowPowerMode.Items.AddRange(new object[2]
      {
        (object) "Standby",
        (object) "Sleep"
      });
			this.cBoxSeqLowPowerMode.Location = new Point(352, 104);
			this.cBoxSeqLowPowerMode.Name = "cBoxSeqLowPowerMode";
			this.cBoxSeqLowPowerMode.Size = new Size(124, 21);
			this.cBoxSeqLowPowerMode.TabIndex = 4;
			this.cBoxSeqLowPowerMode.SelectedIndexChanged += new EventHandler(this.cBoxSeqLowPowerMode_SelectedIndexChanged);
			this.cBoxFromStart.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxFromStart.FormattingEnabled = true;
			this.cBoxFromStart.Items.AddRange(new object[4]
      {
        (object) "To LowPowerSelection ",
        (object) "To Rx",
        (object) "To Tx",
        (object) "To Tx on FifoLevel"
      });
			this.cBoxFromStart.Location = new Point(352, 131);
			this.cBoxFromStart.Name = "cBoxFromStart";
			this.cBoxFromStart.Size = new Size(124, 21);
			this.cBoxFromStart.TabIndex = 6;
			this.cBoxFromStart.SelectedIndexChanged += new EventHandler(this.cBoxFromStart_SelectedIndexChanged);
			this.cBoxSeqLowPowerState.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxSeqLowPowerState.FormattingEnabled = true;
			this.cBoxSeqLowPowerState.Items.AddRange(new object[2]
      {
        (object) "Sequencer OFF",
        (object) "Idle"
      });
			this.cBoxSeqLowPowerState.Location = new Point(352, 158);
			this.cBoxSeqLowPowerState.Name = "cBoxSeqLowPowerState";
			this.cBoxSeqLowPowerState.Size = new Size(124, 21);
			this.cBoxSeqLowPowerState.TabIndex = 8;
			this.cBoxSeqLowPowerState.SelectedIndexChanged += new EventHandler(this.cBoxSeqLowPowerState_SelectedIndexChanged);
			this.cBoxFromIdle.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxFromIdle.FormattingEnabled = true;
			this.cBoxFromIdle.Items.AddRange(new object[2]
      {
        (object) "To Tx",
        (object) "To Rx"
      });
			this.cBoxFromIdle.Location = new Point(352, 185);
			this.cBoxFromIdle.Name = "cBoxFromIdle";
			this.cBoxFromIdle.Size = new Size(124, 21);
			this.cBoxFromIdle.TabIndex = 10;
			this.cBoxFromIdle.SelectedIndexChanged += new EventHandler(this.cBoxFromIdle_SelectedIndexChanged);
			this.cBoxFromTransmit.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxFromTransmit.FormattingEnabled = true;
			this.cBoxFromTransmit.Items.AddRange(new object[2]
      {
        (object) "To LowPowerSelection",
        (object) "To Rx"
      });
			this.cBoxFromTransmit.Location = new Point(352, 212);
			this.cBoxFromTransmit.Name = "cBoxFromTransmit";
			this.cBoxFromTransmit.Size = new Size(124, 21);
			this.cBoxFromTransmit.TabIndex = 12;
			this.cBoxFromTransmit.SelectedIndexChanged += new EventHandler(this.cBoxFromTransmit_SelectedIndexChanged);
			this.cBoxFromReceive.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxFromReceive.FormattingEnabled = true;
			this.cBoxFromReceive.Items.AddRange(new object[8]
      {
        (object) "Unused",
        (object) "To PacketReceived on PayloadReady",
        (object) "To LowPowerSelection on PayloadReady",
        (object) "To PacketReceived on CrcOk",
        (object) "To Sequencer OFF on RSSI",
        (object) "To Sequencer OFF on SyncAddress",
        (object) "To Sequencer OFF on PreambleDetect",
        (object) "Unused"
      });
			this.cBoxFromReceive.Location = new Point(352, 239);
			this.cBoxFromReceive.Name = "cBoxFromReceive";
			this.cBoxFromReceive.Size = new Size(124, 21);
			this.cBoxFromReceive.TabIndex = 14;
			this.cBoxFromReceive.SelectedIndexChanged += new EventHandler(this.cBoxFromReceive_SelectedIndexChanged);
			this.cBoxFromRxTimeout.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxFromRxTimeout.FormattingEnabled = true;
			this.cBoxFromRxTimeout.Items.AddRange(new object[4]
      {
        (object) "To Rx on RxRestart",
        (object) "To Tx",
        (object) "To LowPowerSelection",
        (object) "To Sequencer OFF"
      });
			this.cBoxFromRxTimeout.Location = new Point(352, 266);
			this.cBoxFromRxTimeout.Name = "cBoxFromRxTimeout";
			this.cBoxFromRxTimeout.Size = new Size(124, 21);
			this.cBoxFromRxTimeout.TabIndex = 16;
			this.cBoxFromRxTimeout.SelectedIndexChanged += new EventHandler(this.cBoxFromRxTimeout_SelectedIndexChanged);
			this.cBoxFromPacketReceived.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxFromPacketReceived.FormattingEnabled = true;
			this.cBoxFromPacketReceived.Items.AddRange(new object[5]
      {
        (object) "To Sequencer OFF",
        (object) "To Tx on FifoEmpty",
        (object) "To LowPowerSelection",
        (object) "To Rx via FsRx on frequency change",
        (object) "To Rx"
      });
			this.cBoxFromPacketReceived.Location = new Point(352, 293);
			this.cBoxFromPacketReceived.Name = "cBoxFromPacketReceived";
			this.cBoxFromPacketReceived.Size = new Size(124, 21);
			this.cBoxFromPacketReceived.TabIndex = 18;
			this.cBoxFromPacketReceived.SelectedIndexChanged += new EventHandler(this.cBoxFromPacketReceived_SelectedIndexChanged);
			this.cBoxSeqTimer2Resolution.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxSeqTimer2Resolution.FormattingEnabled = true;
			this.cBoxSeqTimer2Resolution.Items.AddRange(new object[4]
      {
        (object) "OFF",
        (object) "64",
        (object) "4'100",
        (object) "262'000"
      });
			this.cBoxSeqTimer2Resolution.Location = new Point(177, 391);
			this.cBoxSeqTimer2Resolution.Name = "cBoxSeqTimer2Resolution";
			this.cBoxSeqTimer2Resolution.Size = new Size(124, 21);
			this.cBoxSeqTimer2Resolution.TabIndex = 30;
			this.cBoxSeqTimer2Resolution.SelectedIndexChanged += new EventHandler(this.cBoxSeqTimer2Resolution_SelectedIndexChanged);
			this.label2.AutoSize = true;
			this.label2.Location = new Point(307, 395);
			this.label2.Name = "label2";
			this.label2.Size = new Size(18, 13);
			this.label2.TabIndex = 31;
			this.label2.Text = "µs";
			this.label3.AutoSize = true;
			this.label3.Location = new Point(194, 372);
			this.label3.Margin = new Padding(3);
			this.label3.Name = "label3";
			this.label3.Size = new Size(90, 13);
			this.label3.TabIndex = 28;
			this.label3.Text = "Timer 2 resolution";
			this.label4.AutoSize = true;
			this.label4.Location = new Point(367, 320);
			this.label4.Margin = new Padding(3);
			this.label4.Name = "label4";
			this.label4.Size = new Size(94, 13);
			this.label4.TabIndex = 20;
			this.label4.Text = "Timer 1 coefficient";
			this.label5.AutoSize = true;
			this.label5.Location = new Point(367, 372);
			this.label5.Margin = new Padding(3);
			this.label5.Name = "label5";
			this.label5.Size = new Size(94, 13);
			this.label5.TabIndex = 29;
			this.label5.Text = "Timer 2 coefficient";
			this.label6.AutoSize = true;
			this.label6.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
			this.label6.Location = new Point(331, 343);
			this.label6.Name = "label6";
			this.label6.Size = new Size(15, 13);
			this.label6.TabIndex = 23;
			this.label6.Text = "X";
			this.label7.AutoSize = true;
			this.label7.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
			this.label7.Location = new Point(331, 395);
			this.label7.Name = "label7";
			this.label7.Size = new Size(15, 13);
			this.label7.TabIndex = 32;
			this.label7.Text = "X";
			this.label8.AutoSize = true;
			this.label8.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
			this.label8.Location = new Point(482, 343);
			this.label8.Name = "label8";
			this.label8.Size = new Size(14, 13);
			this.label8.TabIndex = 25;
			this.label8.Text = "=";
			this.label9.AutoSize = true;
			this.label9.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
			this.label9.Location = new Point(482, 395);
			this.label9.Name = "label9";
			this.label9.Size = new Size(14, 13);
			this.label9.TabIndex = 34;
			this.label9.Text = "=";
			this.lblTimer1Value.BackColor = Color.Transparent;
			this.lblTimer1Value.BorderStyle = BorderStyle.Fixed3D;
			this.lblTimer1Value.Location = new Point(502, 339);
			this.lblTimer1Value.Margin = new Padding(3);
			this.lblTimer1Value.Name = "lblTimer1Value";
			this.lblTimer1Value.Size = new Size(98, 20);
			this.lblTimer1Value.TabIndex = 26;
			this.lblTimer1Value.Text = "0";
			this.lblTimer1Value.TextAlign = ContentAlignment.MiddleLeft;
			this.label11.AutoSize = true;
			this.label11.Location = new Point(606, 343);
			this.label11.Name = "label11";
			this.label11.Size = new Size(18, 13);
			this.label11.TabIndex = 27;
			this.label11.Text = "µs";
			this.label12.AutoSize = true;
			this.label12.Location = new Point(606, 395);
			this.label12.Name = "label12";
			this.label12.Size = new Size(18, 13);
			this.label12.TabIndex = 36;
			this.label12.Text = "µs";
			this.label10.AutoSize = true;
			this.label10.Location = new Point(174, 80);
			this.label10.Name = "label10";
			this.label10.Size = new Size(62, 13);
			this.label10.TabIndex = 0;
			this.label10.Text = "Sequencer:";
			this.label13.AutoSize = true;
			this.label13.Location = new Point(174, 107);
			this.label13.Name = "label13";
			this.label13.Size = new Size(56, 13);
			this.label13.TabIndex = 3;
			this.label13.Text = "Idle mode:";
			this.label14.AutoSize = true;
			this.label14.Location = new Point(174, 134);
			this.label14.Name = "label14";
			this.label14.Size = new Size(102, 13);
			this.label14.TabIndex = 5;
			this.label14.Text = "Transition from start:";
			this.label15.AutoSize = true;
			this.label15.Location = new Point(174, 161);
			this.label15.Name = "label15";
			this.label15.Size = new Size(107, 13);
			this.label15.TabIndex = 7;
			this.label15.Text = "Low power selection:";
			this.label16.AutoSize = true;
			this.label16.Location = new Point(174, 188);
			this.label16.Name = "label16";
			this.label16.Size = new Size(98, 13);
			this.label16.TabIndex = 9;
			this.label16.Text = "Transition from idle:";
			this.label17.AutoSize = true;
			this.label17.Location = new Point(174, 215);
			this.label17.Name = "label17";
			this.label17.Size = new Size(118, 13);
			this.label17.TabIndex = 11;
			this.label17.Text = "Transition from transmit:";
			this.label18.AutoSize = true;
			this.label18.Location = new Point(174, 242);
			this.label18.Name = "label18";
			this.label18.Size = new Size(117, 13);
			this.label18.TabIndex = 13;
			this.label18.Text = "Transition from receive:";
			this.label19.AutoSize = true;
			this.label19.Location = new Point(174, 269);
			this.label19.Name = "label19";
			this.label19.Size = new Size(132, 13);
			this.label19.TabIndex = 15;
			this.label19.Text = "Transition from Rx timeout:";
			this.label20.AutoSize = true;
			this.label20.Location = new Point(174, 296);
			this.label20.Name = "label20";
			this.label20.Size = new Size(159, 13);
			this.label20.TabIndex = 17;
			this.label20.Text = "Transition from packet received:";
			this.nudSeqTimer2Coefficient.Location = new Point(352, 391);
			NumericUpDownEx numericUpDownEx1 = this.nudSeqTimer2Coefficient;
			int[] bits1 = new int[4];
			bits1[0] = (int)byte.MaxValue;
			Decimal num1 = new Decimal(bits1);
			numericUpDownEx1.Maximum = num1;
			this.nudSeqTimer2Coefficient.Name = "nudSeqTimer2Coefficient";
			this.nudSeqTimer2Coefficient.Size = new Size(124, 20);
			this.nudSeqTimer2Coefficient.TabIndex = 33;
			this.nudSeqTimer2Coefficient.ThousandsSeparator = true;
			this.nudSeqTimer2Coefficient.ValueChanged += new EventHandler(this.nudSeqTimer2Coefficient_ValueChanged);
			this.nudSeqTimer1Coefficient.Location = new Point(352, 339);
			NumericUpDownEx numericUpDownEx2 = this.nudSeqTimer1Coefficient;
			int[] bits2 = new int[4];
			bits2[0] = (int)byte.MaxValue;
			Decimal num2 = new Decimal(bits2);
			numericUpDownEx2.Maximum = num2;
			this.nudSeqTimer1Coefficient.Name = "nudSeqTimer1Coefficient";
			this.nudSeqTimer1Coefficient.Size = new Size(124, 20);
			this.nudSeqTimer1Coefficient.TabIndex = 24;
			this.nudSeqTimer1Coefficient.ThousandsSeparator = true;
			this.nudSeqTimer1Coefficient.ValueChanged += new EventHandler(this.nudSeqTimer1Coefficient_ValueChanged);
			this.ledSequencerModeStatus.BackColor = Color.Transparent;
			this.ledSequencerModeStatus.LedColor = Color.Green;
			this.ledSequencerModeStatus.LedSize = new Size(11, 11);
			this.ledSequencerModeStatus.Location = new Point(542, 79);
			this.ledSequencerModeStatus.Name = "ledSequencerModeStatus";
			this.ledSequencerModeStatus.Size = new Size(15, 15);
			this.ledSequencerModeStatus.TabIndex = 37;
			this.ledSequencerModeStatus.Text = "Sequencer mode status";
			this.toolTip1.SetToolTip((Control)this.ledSequencerModeStatus, componentResourceManager.GetString("ledSequencerModeStatus.ToolTip"));
			this.AutoScaleDimensions = new SizeF(6f, 13f);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.Controls.Add((Control)this.ledSequencerModeStatus);
			this.Controls.Add((Control)this.lblTimer2Value);
			this.Controls.Add((Control)this.btnSequencerStop);
			this.Controls.Add((Control)this.lblTimer1Value);
			this.Controls.Add((Control)this.btnSequencerStart);
			this.Controls.Add((Control)this.nudSeqTimer2Coefficient);
			this.Controls.Add((Control)this.nudSeqTimer1Coefficient);
			this.Controls.Add((Control)this.label5);
			this.Controls.Add((Control)this.label4);
			this.Controls.Add((Control)this.label3);
			this.Controls.Add((Control)this.label20);
			this.Controls.Add((Control)this.label19);
			this.Controls.Add((Control)this.label18);
			this.Controls.Add((Control)this.label17);
			this.Controls.Add((Control)this.label16);
			this.Controls.Add((Control)this.label15);
			this.Controls.Add((Control)this.label14);
			this.Controls.Add((Control)this.label13);
			this.Controls.Add((Control)this.label10);
			this.Controls.Add((Control)this.label1);
			this.Controls.Add((Control)this.label2);
			this.Controls.Add((Control)this.label7);
			this.Controls.Add((Control)this.label9);
			this.Controls.Add((Control)this.label8);
			this.Controls.Add((Control)this.label6);
			this.Controls.Add((Control)this.label12);
			this.Controls.Add((Control)this.label11);
			this.Controls.Add((Control)this.label22);
			this.Controls.Add((Control)this.cBoxFromPacketReceived);
			this.Controls.Add((Control)this.cBoxFromRxTimeout);
			this.Controls.Add((Control)this.cBoxFromReceive);
			this.Controls.Add((Control)this.cBoxFromTransmit);
			this.Controls.Add((Control)this.cBoxFromIdle);
			this.Controls.Add((Control)this.cBoxSeqLowPowerState);
			this.Controls.Add((Control)this.cBoxFromStart);
			this.Controls.Add((Control)this.cBoxSeqLowPowerMode);
			this.Controls.Add((Control)this.cBoxSeqTimer2Resolution);
			this.Controls.Add((Control)this.cBoxTimer1Resolution);
			this.Name = "SequencerViewControl";
			this.Size = new Size(799, 493);
			((ISupportInitialize)this.errorProvider).EndInit();
			this.nudSeqTimer2Coefficient.EndInit();
			this.nudSeqTimer1Coefficient.EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		private void OnSequencerStartChanged()
		{
			if (this.SequencerStartChanged == null)
				return;
			this.SequencerStartChanged((object)this, EventArgs.Empty);
		}

		private void OnSequencerStopChanged()
		{
			if (this.SequencerStopChanged == null)
				return;
			this.SequencerStopChanged((object)this, EventArgs.Empty);
		}

		private void OnIdleModeChanged(IdleMode value)
		{
			if (this.IdleModeChanged == null)
				return;
			this.IdleModeChanged((object)this, new IdleModeEventArg(value));
		}

		private void OnFromStartChanged(FromStart value)
		{
			if (this.FromStartChanged == null)
				return;
			this.FromStartChanged((object)this, new FromStartEventArg(value));
		}

		private void OnLowPowerSelectionChanged(LowPowerSelection value)
		{
			if (this.LowPowerSelectionChanged == null)
				return;
			this.LowPowerSelectionChanged((object)this, new LowPowerSelectionEventArg(value));
		}

		private void OnFromIdleChanged(FromIdle value)
		{
			if (this.FromIdleChanged == null)
				return;
			this.FromIdleChanged((object)this, new FromIdleEventArg(value));
		}

		private void OnFromTransmitChanged(FromTransmit value)
		{
			if (this.FromTransmitChanged == null)
				return;
			this.FromTransmitChanged((object)this, new FromTransmitEventArg(value));
		}

		private void OnFromReceiveChanged(FromReceive value)
		{
			if (this.FromReceiveChanged == null)
				return;
			this.FromReceiveChanged((object)this, new FromReceiveEventArg(value));
		}

		private void OnFromRxTimeoutChanged(FromRxTimeout value)
		{
			if (this.FromRxTimeoutChanged == null)
				return;
			this.FromRxTimeoutChanged((object)this, new FromRxTimeoutEventArg(value));
		}

		private void OnFromPacketReceivedChanged(FromPacketReceived value)
		{
			if (this.FromPacketReceivedChanged == null)
				return;
			this.FromPacketReceivedChanged((object)this, new FromPacketReceivedEventArg(value));
		}

		private void OnTimer1ResolutionChanged(TimerResolution value)
		{
			if (this.Timer1ResolutionChanged == null)
				return;
			this.Timer1ResolutionChanged((object)this, new TimerResolutionEventArg(value));
		}

		private void OnTimer2ResolutionChanged(TimerResolution value)
		{
			if (this.Timer2ResolutionChanged == null)
				return;
			this.Timer2ResolutionChanged((object)this, new TimerResolutionEventArg(value));
		}

		private void OnTimer1CoefChanged(byte value)
		{
			if (this.Timer1CoefChanged == null)
				return;
			this.Timer1CoefChanged((object)this, new ByteEventArg(value));
		}

		private void OnTimer2CoefChanged(byte value)
		{
			if (this.Timer2CoefChanged == null)
				return;
			this.Timer2CoefChanged((object)this, new ByteEventArg(value));
		}

		private void btnSequencerStart_Click(object sender, EventArgs e)
		{
			this.ledSequencerModeStatus.Checked = true;
			this.OnSequencerStartChanged();
		}

		private void btnSequencerStop_Click(object sender, EventArgs e)
		{
			this.ledSequencerModeStatus.Checked = false;
			this.OnSequencerStopChanged();
		}

		private void cBoxSeqLowPowerMode_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.IdleMode = (IdleMode)this.cBoxSeqLowPowerMode.SelectedIndex;
			this.OnIdleModeChanged(this.IdleMode);
		}

		private void cBoxFromStart_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.FromStart = (FromStart)this.cBoxFromStart.SelectedIndex;
			this.OnFromStartChanged(this.FromStart);
		}

		private void cBoxSeqLowPowerState_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.LowPowerSelection = (LowPowerSelection)this.cBoxSeqLowPowerState.SelectedIndex;
			this.OnLowPowerSelectionChanged(this.LowPowerSelection);
		}

		private void cBoxFromIdle_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.FromIdle = (FromIdle)this.cBoxFromIdle.SelectedIndex;
			this.OnFromIdleChanged(this.FromIdle);
		}

		private void cBoxFromTransmit_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.FromTransmit = (FromTransmit)this.cBoxFromTransmit.SelectedIndex;
			this.OnFromTransmitChanged(this.FromTransmit);
		}

		private void cBoxFromReceive_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.FromReceive = (FromReceive)this.cBoxFromReceive.SelectedIndex;
			this.OnFromReceiveChanged(this.FromReceive);
		}

		private void cBoxFromRxTimeout_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.FromRxTimeout = (FromRxTimeout)this.cBoxFromRxTimeout.SelectedIndex;
			this.OnFromRxTimeoutChanged(this.FromRxTimeout);
		}

		private void cBoxFromPacketReceived_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.FromPacketReceived = (FromPacketReceived)this.cBoxFromPacketReceived.SelectedIndex;
			this.OnFromPacketReceivedChanged(this.FromPacketReceived);
		}

		private void cBoxTimer1Resolution_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.Timer1Resolution = (TimerResolution)this.cBoxTimer1Resolution.SelectedIndex;
			this.OnTimer1ResolutionChanged(this.Timer1Resolution);
		}

		private void cBoxSeqTimer2Resolution_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.Timer2Resolution = (TimerResolution)this.cBoxSeqTimer2Resolution.SelectedIndex;
			this.OnTimer2ResolutionChanged(this.Timer2Resolution);
		}

		private void nudSeqTimer1Coefficient_ValueChanged(object sender, EventArgs e)
		{
			this.Timer1Coef = (byte)this.nudSeqTimer1Coefficient.Value;
			this.OnTimer1CoefChanged(this.Timer1Coef);
		}

		private void nudSeqTimer2Coefficient_ValueChanged(object sender, EventArgs e)
		{
			this.Timer2Coef = (byte)this.nudSeqTimer2Coefficient.Value;
			this.OnTimer2CoefChanged(this.Timer2Coef);
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
	}
}

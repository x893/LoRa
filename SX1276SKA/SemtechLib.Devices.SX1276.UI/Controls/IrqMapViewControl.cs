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
	public class IrqMapViewControl : UserControl, INotifyDocumentationChanged
	{
		private Decimal frequencyXo = new Decimal(32000000);
		private OperatingModeEnum mode = OperatingModeEnum.Undefined;
		private DataModeEnum dataMode = DataModeEnum.Packet;
		private bool bitSync = true;
		private IContainer components;
		private ErrorProvider errorProvider;
		private Label label1;
		private Label label2;
		private Label label3;
		private Label label4;
		private Label label5;
		private Label label6;
		private Label label7;
		private Label lblOperatingMode;
		private ComboBox cBoxDio5Mapping;
		private ComboBox cBoxDio4Mapping;
		private ComboBox cBoxDio3Mapping;
		private ComboBox cBoxDio2Mapping;
		private ComboBox cBoxDio1Mapping;
		private ComboBox cBoxDio0Mapping;
		private Label lblDataMode;
		private Label label13;
		private Label label15;
		private ComboBox cBoxClockOut;
		private Label label16;
		private Label lblBitSynchroniser;
		private Label label8;
		private GroupBoxEx gBoxDeviceStatus;
		private GroupBoxEx gBoxClockOut;
		private GroupBoxEx gBoxDioMapping;
		private GroupBoxEx gBoxDioSettings;
		private Panel pnlPreambleIrq;
		private RadioButton rBtnPreambleIrqOff;
		private RadioButton rBtnPreambleIrqOn;
		private Label label9;

		public Decimal FrequencyXo
		{
			get
			{
				return frequencyXo;
			}
			set
			{
				int num1 = (int)ClockOut;
				frequencyXo = value;
				cBoxClockOut.Items.Clear();
				int num2 = 1;
				while (num2 <= 32)
				{
					cBoxClockOut.Items.Add((object)Math.Round(frequencyXo / (Decimal)num2, MidpointRounding.AwayFromZero).ToString());
					num2 <<= 1;
				}
				cBoxClockOut.Items.Add((object)"RC");
				cBoxClockOut.Items.Add((object)"OFF");
				ClockOut = (ClockOutEnum)num1;
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
				if (mode == value)
					return;
				mode = value;
				PopulateDioCbox();
				switch (mode)
				{
					case OperatingModeEnum.Sleep:
						lblOperatingMode.Text = "Sleep";
						break;
					case OperatingModeEnum.Stdby:
						lblOperatingMode.Text = "Standby";
						break;
					case OperatingModeEnum.FsTx:
						lblOperatingMode.Text = "Synthesizer Tx";
						break;
					case OperatingModeEnum.Tx:
						lblOperatingMode.Text = "Transmitter";
						break;
					case OperatingModeEnum.FsRx:
						lblOperatingMode.Text = "Synthesizer Rx";
						break;
					case OperatingModeEnum.Rx:
						lblOperatingMode.Text = "Receiver";
						break;
				}
			}
		}

		public DataModeEnum DataMode
		{
			get
			{
				return dataMode;
			}
			set
			{
				dataMode = value;
				PopulateDioCbox();
				switch (dataMode)
				{
					case DataModeEnum.Continuous:
						lblDataMode.Text = "Continuous";
						break;
					case DataModeEnum.Packet:
						lblDataMode.Text = "Packet";
						break;
				}
			}
		}

		public bool BitSyncOn
		{
			get
			{
				return bitSync;
			}
			set
			{
				bitSync = value;
				if (bitSync)
					lblBitSynchroniser.Text = "ON";
				else
					lblBitSynchroniser.Text = "OFF";
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

		public bool MapPreambleDetect
		{
			get
			{
				return rBtnPreambleIrqOn.Checked;
			}
			set
			{
				rBtnPreambleIrqOn.CheckedChanged -= new EventHandler(rBtnPreambleIrq_CheckedChanged);
				rBtnPreambleIrqOff.CheckedChanged -= new EventHandler(rBtnPreambleIrq_CheckedChanged);
				if (value)
				{
					rBtnPreambleIrqOn.Checked = true;
					rBtnPreambleIrqOff.Checked = false;
				}
				else
				{
					rBtnPreambleIrqOn.Checked = false;
					rBtnPreambleIrqOff.Checked = true;
				}
				PopulateDioCbox();
				rBtnPreambleIrqOn.CheckedChanged += new EventHandler(rBtnPreambleIrq_CheckedChanged);
				rBtnPreambleIrqOff.CheckedChanged += new EventHandler(rBtnPreambleIrq_CheckedChanged);
			}
		}

		public ClockOutEnum ClockOut
		{
			get
			{
				return (ClockOutEnum)cBoxClockOut.SelectedIndex;
			}
			set
			{
				try
				{
					cBoxClockOut.SelectedIndexChanged -= new EventHandler(cBoxClockOut_SelectedIndexChanged);
					cBoxClockOut.SelectedIndex = (int)value;
					cBoxClockOut.SelectedIndexChanged += new EventHandler(cBoxClockOut_SelectedIndexChanged);
				}
				catch
				{
					cBoxClockOut.SelectedIndexChanged += new EventHandler(cBoxClockOut_SelectedIndexChanged);
				}
			}
		}

		public event BooleanEventHandler DioPreambleIrqOnChanged;

		public event DioMappingEventHandler DioMappingChanged;

		public event ClockOutEventHandler ClockOutChanged;

		public event DocumentationChangedEventHandler DocumentationChanged;

		public IrqMapViewControl()
		{
			InitializeComponent();
		}

		private void PopulateDioCbox()
		{
			int[] numArray = new int[6]
			{
				(int) Dio0Mapping,
				(int) Dio1Mapping,
				(int) Dio2Mapping,
				(int) Dio3Mapping,
				(int) Dio4Mapping,
				(int) Dio5Mapping
			};
			cBoxDio0Mapping.Items.Clear();
			cBoxDio1Mapping.Items.Clear();
			cBoxDio2Mapping.Items.Clear();
			cBoxDio3Mapping.Items.Clear();
			cBoxDio4Mapping.Items.Clear();
			cBoxDio5Mapping.Items.Clear();
			switch (dataMode)
			{
				case DataModeEnum.Continuous:
					switch (mode)
					{
						case OperatingModeEnum.Sleep:
							cBoxDio0Mapping.Items.AddRange(new object[4] { "-", "-", "-", "-" });
							cBoxDio1Mapping.Items.AddRange(new object[4] { "-", "-", "-", "-" });
							cBoxDio2Mapping.Items.AddRange(new object[4] { "-", "-", "-", "-" });
							cBoxDio3Mapping.Items.AddRange(new object[4] { "-", "-", "-", "-" });
							cBoxDio4Mapping.Items.AddRange(new object[4] { "-", "-", "-", "-" });
							cBoxDio5Mapping.Items.AddRange(new object[4] { "ClkOut", "-", "-", "-" });
							break;
						case OperatingModeEnum.Stdby:
							cBoxDio0Mapping.Items.AddRange(new object[4] { "-", "-", "-", "-" });
							cBoxDio1Mapping.Items.AddRange(new object[4] { "-", "-", "-", "-" });
							cBoxDio2Mapping.Items.AddRange(new object[4] { "-", "-", "-", "-" });
							cBoxDio3Mapping.Items.AddRange(new object[4] { "-", "-", "-", "LowBat" });
							cBoxDio4Mapping.Items.AddRange(new object[4] { "-", "-", "-", "ModeReady" });
							cBoxDio5Mapping.Items.AddRange(new object[4] { "ClkOut", "-", "-", "ModeReady" });
							break;
						case OperatingModeEnum.FsTx:
						case OperatingModeEnum.FsRx:
							cBoxDio0Mapping.Items.AddRange(new object[4] { "-", "-", "-", "-" });
							cBoxDio1Mapping.Items.AddRange(new object[4] { "-", "-", "-", "-" });
							cBoxDio2Mapping.Items.AddRange(new object[4] { "-", "-", "-", "-" });
							cBoxDio3Mapping.Items.AddRange(new object[4] { "-", "-", "-", "LowBat" });
							cBoxDio4Mapping.Items.AddRange(new object[4] { "LowBat", "PllLock", "-", "ModeReady" });
							cBoxDio5Mapping.Items.AddRange(new object[4] { "ClkOut", "PllLock", "-", "ModeReady" });
							break;
						case OperatingModeEnum.Tx:
							cBoxDio0Mapping.Items.AddRange(new object[4] { "TxReady", "-", "-", "-" });
							cBoxDio1Mapping.Items.AddRange(new object[4] { "Dclk", "-", "-", "-" });
							cBoxDio2Mapping.Items.AddRange(new object[4] { "Data", "Data", "Data", "Data" });
							cBoxDio3Mapping.Items.AddRange(new object[4] { "-", "-", "-", "LowBat" });
							cBoxDio4Mapping.Items.AddRange(new object[4] { "LowBat", "PllLock", "-", "ModeReady" });
							cBoxDio5Mapping.Items.AddRange(new object[4] { "ClkOut", "PllLock", "-", "ModeReady" });
							break;
						case OperatingModeEnum.Rx:
							if (MapPreambleDetect)
							{
								cBoxDio0Mapping.Items.AddRange(new object[4] { "SyncAddress", "Preamble", "RxReady", "-" });
								cBoxDio1Mapping.Items.AddRange(new object[4] { "Dclk", "Preamble", "-", "-" });
								cBoxDio2Mapping.Items.AddRange(new object[4] { "Data", "Data", "Data", "Data" });
								cBoxDio3Mapping.Items.AddRange(new object[4] { "Timeout", "Preamble", "-", "LowBat" });
								cBoxDio4Mapping.Items.AddRange(new object[4] { "LowBat", "PllLock", "Timeout", "ModeReady" });
								cBoxDio5Mapping.Items.AddRange(new object[4] { "ClkOut", "PllLock", "Preamble", "ModeReady" });
								break;
							}
							cBoxDio0Mapping.Items.AddRange(new object[4] { "SyncAddress", "Rssi", "RxReady", "-" });
							cBoxDio1Mapping.Items.AddRange(new object[4] { "Dclk", "Rssi", "-", "-" });
							cBoxDio2Mapping.Items.AddRange(new object[4] { "Data", "Data", "Data", "Data" });
							cBoxDio3Mapping.Items.AddRange(new object[4] { "Timeout", "Rssi", "-", "LowBat" });
							cBoxDio4Mapping.Items.AddRange(new object[4] { "LowBat", "PllLock", "Timeout", "ModeReady" });
							cBoxDio5Mapping.Items.AddRange(new object[4] { "ClkOut", "PllLock", "Rssi", "ModeReady" });
							break;
					}
					break;
				case DataModeEnum.Packet:
					switch (mode)
					{
						case OperatingModeEnum.Sleep:
							cBoxDio0Mapping.Items.AddRange(new object[4] { "-", "-", "-", "-" });
							cBoxDio1Mapping.Items.AddRange(new object[4] { "FifoLevel", "FifoEmpty", "FifoFull", "-" });
							cBoxDio2Mapping.Items.AddRange(new object[4] { "FifoFull", "-", "FifoFull", "FifoFull" });
							cBoxDio3Mapping.Items.AddRange(new object[4] { "FifoEmpty", "-", "FifoEmpty", "FifoEmpty" });
							cBoxDio4Mapping.Items.AddRange(new object[4] { "-", "-", "-", "-" });
							cBoxDio5Mapping.Items.AddRange(new object[4] { "ClkOut", "-", "-", "-" }); break;
						case OperatingModeEnum.Stdby:
							cBoxDio0Mapping.Items.AddRange(new object[4] { "-", "-", "-", "LowBat" });
							cBoxDio1Mapping.Items.AddRange(new object[4] { "FifoLevel", "FifoEmpty", "FifoFull", "-" });
							cBoxDio2Mapping.Items.AddRange(new object[4] { "FifoFull", "-", "FifoFull", "FifoFull" });
							cBoxDio3Mapping.Items.AddRange(new object[4] { "FifoEmpty", "-", "FifoEmpty", "FifoEmpty" });
							cBoxDio4Mapping.Items.AddRange(new object[4] { "LowBat", "-", "-", "-" });
							cBoxDio5Mapping.Items.AddRange(new object[4] { "ClkOut", "-", "-", "ModeReady" });
							break;
						case OperatingModeEnum.FsTx:
						case OperatingModeEnum.FsRx:
							cBoxDio0Mapping.Items.AddRange(new object[4] { "-", "-", "-", "LowBat" });
							cBoxDio1Mapping.Items.AddRange(new object[4] { "FifoLevel", "FifoEmpty", "FifoFull", "-" });
							cBoxDio2Mapping.Items.AddRange(new object[4] { "FifoFull", "-", "FifoFull", "FifoFull" });
							cBoxDio3Mapping.Items.AddRange(new object[4] { "FifoEmpty", "-", "FifoEmpty", "FifoEmpty" });
							cBoxDio4Mapping.Items.AddRange(new object[4] { "LowBat", "PllLock", "-", "-" });
							cBoxDio5Mapping.Items.AddRange(new object[4] { "ClkOut", "PllLock", "-", "ModeReady" });
							break;
						case OperatingModeEnum.Tx:
							cBoxDio0Mapping.Items.AddRange(new object[4] { "PacketSent", "-", "-", "LowBat" });
							cBoxDio1Mapping.Items.AddRange(new object[4] { "FifoLevel", "FifoEmpty", "FifoFull", "-" });
							cBoxDio2Mapping.Items.AddRange(new object[4] { "FifoFull", "-", "FifoFull", "FifoFull" });
							cBoxDio3Mapping.Items.AddRange(new object[4] { "FifoEmpty", "TxReady", "FifoEmpty", "FifoEmpty" });
							cBoxDio4Mapping.Items.AddRange(new object[4] { "LowBat", "PllLock", "-", "-" });
							cBoxDio5Mapping.Items.AddRange(new object[4] { "ClkOut", "PllLock", "Data", "ModeReady" });
							break;
						case OperatingModeEnum.Rx:
							if (MapPreambleDetect)
							{
								cBoxDio0Mapping.Items.AddRange(new object[4] { "PayloadReady", "CrcOk", "-", "LowBat" });
								cBoxDio1Mapping.Items.AddRange(new object[4] { "FifoLevel", "FifoEmpty", "FifoFull", "-" });
								cBoxDio2Mapping.Items.AddRange(new object[4] { "FifoFull", "RxReady", "Timeout", "SyncAddress" });
								cBoxDio3Mapping.Items.AddRange(new object[4] { "FifoEmpty", "-", "FifoEmpty", "FifoEmpty" });
								cBoxDio4Mapping.Items.AddRange(new object[4] { "LowBat", "PllLock", "Timeout", "Preamble" });
								cBoxDio5Mapping.Items.AddRange(new object[4] { "ClkOut", "PllLock", "Data", "ModeReady" });
								break;
							}
							cBoxDio0Mapping.Items.AddRange(new object[4] { "PayloadReady", "CrcOk", "-", "LowBat" });
							cBoxDio1Mapping.Items.AddRange(new object[4] { "FifoLevel", "FifoEmpty", "FifoFull", "-" });
							cBoxDio2Mapping.Items.AddRange(new object[4] { "FifoFull", "RxReady", "Timeout", "SyncAddress" });
							cBoxDio3Mapping.Items.AddRange(new object[4] { "FifoEmpty", "-", "FifoEmpty", "FifoEmpty" });
							cBoxDio4Mapping.Items.AddRange(new object[4] { "LowBat", "PllLock", "Timeout", "Rssi" });
							cBoxDio5Mapping.Items.AddRange(new object[4] { "ClkOut", "PllLock", "Data", "ModeReady" });
							break;
					}
					break;
			}
			Dio0Mapping = (DioMappingEnum)numArray[0];
			Dio1Mapping = (DioMappingEnum)numArray[1];
			Dio2Mapping = (DioMappingEnum)numArray[2];
			Dio3Mapping = (DioMappingEnum)numArray[3];
			Dio4Mapping = (DioMappingEnum)numArray[4];
			Dio5Mapping = (DioMappingEnum)numArray[5];
		}

		private void OnDioMappingChanged(byte id, DioMappingEnum value)
		{
			if (DioMappingChanged == null)
				return;
			DioMappingChanged((object)this, new DioMappingEventArg(id, value));
		}

		private void OnClockOutChanged(ClockOutEnum value)
		{
			if (ClockOutChanged == null)
				return;
			ClockOutChanged((object)this, new ClockOutEventArg(value));
		}

		private void OnDioPreambleIrqOnChanged(bool value)
		{
			if (DioPreambleIrqOnChanged == null)
				return;
			DioPreambleIrqOnChanged((object)this, new BooleanEventArg(value));
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

		private void cBoxClockOut_SelectedIndexChanged(object sender, EventArgs e)
		{
			OnClockOutChanged((ClockOutEnum)cBoxClockOut.SelectedIndex);
		}

		private void rBtnPreambleIrq_CheckedChanged(object sender, EventArgs e)
		{
			MapPreambleDetect = rBtnPreambleIrqOn.Checked;
			OnDioPreambleIrqOnChanged(MapPreambleDetect);
		}

		private void control_MouseEnter(object sender, EventArgs e)
		{
			if (sender == gBoxDeviceStatus)
				OnDocumentationChanged(new DocumentationChangedEventArgs("Common", "Device status"));
			else if (sender == gBoxDioMapping)
			{
				OnDocumentationChanged(new DocumentationChangedEventArgs("Irq mapping", "DIO mapping"));
			}
			else
			{
				if (sender != gBoxClockOut)
					return;
				OnDocumentationChanged(new DocumentationChangedEventArgs("Irq mapping", "Clock out"));
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
			this.errorProvider = new ErrorProvider(this.components);
			this.gBoxClockOut = new GroupBoxEx();
			this.cBoxClockOut = new ComboBox();
			this.label15 = new Label();
			this.label16 = new Label();
			this.gBoxDioMapping = new GroupBoxEx();
			this.cBoxDio5Mapping = new ComboBox();
			this.cBoxDio4Mapping = new ComboBox();
			this.label2 = new Label();
			this.label7 = new Label();
			this.cBoxDio3Mapping = new ComboBox();
			this.cBoxDio0Mapping = new ComboBox();
			this.label3 = new Label();
			this.cBoxDio1Mapping = new ComboBox();
			this.label4 = new Label();
			this.cBoxDio2Mapping = new ComboBox();
			this.label5 = new Label();
			this.label6 = new Label();
			this.gBoxDioSettings = new GroupBoxEx();
			this.pnlPreambleIrq = new Panel();
			this.rBtnPreambleIrqOff = new RadioButton();
			this.rBtnPreambleIrqOn = new RadioButton();
			this.label9 = new Label();
			this.gBoxDeviceStatus = new GroupBoxEx();
			this.lblBitSynchroniser = new Label();
			this.lblOperatingMode = new Label();
			this.label13 = new Label();
			this.label1 = new Label();
			this.label8 = new Label();
			this.lblDataMode = new Label();
			((ISupportInitialize)this.errorProvider).BeginInit();
			this.gBoxClockOut.SuspendLayout();
			this.gBoxDioMapping.SuspendLayout();
			this.gBoxDioSettings.SuspendLayout();
			this.pnlPreambleIrq.SuspendLayout();
			this.gBoxDeviceStatus.SuspendLayout();
			this.SuspendLayout();
			this.errorProvider.ContainerControl = (ContainerControl)this;
			this.gBoxClockOut.Controls.Add((Control)this.cBoxClockOut);
			this.gBoxClockOut.Controls.Add((Control)this.label15);
			this.gBoxClockOut.Controls.Add((Control)this.label16);
			this.gBoxClockOut.Location = new Point(253, 386);
			this.gBoxClockOut.Name = "gBoxClockOut";
			this.gBoxClockOut.Size = new Size(293, 45);
			this.gBoxClockOut.TabIndex = 2;
			this.gBoxClockOut.TabStop = false;
			this.gBoxClockOut.Text = "Clock out";
			this.gBoxClockOut.MouseEnter += new EventHandler(this.control_MouseEnter);
			this.gBoxClockOut.MouseLeave += new EventHandler(this.control_MouseLeave);
			this.cBoxClockOut.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxClockOut.FormattingEnabled = true;
			this.cBoxClockOut.Location = new Point(162, 19);
			this.cBoxClockOut.Name = "cBoxClockOut";
			this.cBoxClockOut.Size = new Size(100, 21);
			this.cBoxClockOut.TabIndex = 1;
			this.cBoxClockOut.SelectedIndexChanged += new EventHandler(this.cBoxClockOut_SelectedIndexChanged);
			this.label15.AutoSize = true;
			this.label15.Location = new Point(5, 23);
			this.label15.Name = "label15";
			this.label15.Size = new Size(60, 13);
			this.label15.TabIndex = 0;
			this.label15.Text = "Frequency:";
			this.label15.TextAlign = ContentAlignment.MiddleLeft;
			this.label16.AutoSize = true;
			this.label16.Location = new Point(268, 23);
			this.label16.Name = "label16";
			this.label16.Size = new Size(20, 13);
			this.label16.TabIndex = 2;
			this.label16.Text = "Hz";
			this.label16.TextAlign = ContentAlignment.MiddleLeft;
			this.gBoxDioMapping.Controls.Add((Control)this.cBoxDio5Mapping);
			this.gBoxDioMapping.Controls.Add((Control)this.cBoxDio4Mapping);
			this.gBoxDioMapping.Controls.Add((Control)this.label2);
			this.gBoxDioMapping.Controls.Add((Control)this.label7);
			this.gBoxDioMapping.Controls.Add((Control)this.cBoxDio3Mapping);
			this.gBoxDioMapping.Controls.Add((Control)this.cBoxDio0Mapping);
			this.gBoxDioMapping.Controls.Add((Control)this.label3);
			this.gBoxDioMapping.Controls.Add((Control)this.cBoxDio1Mapping);
			this.gBoxDioMapping.Controls.Add((Control)this.label4);
			this.gBoxDioMapping.Controls.Add((Control)this.cBoxDio2Mapping);
			this.gBoxDioMapping.Controls.Add((Control)this.label5);
			this.gBoxDioMapping.Controls.Add((Control)this.label6);
			this.gBoxDioMapping.Location = new Point(253, 201);
			this.gBoxDioMapping.Name = "gBoxDioMapping";
			this.gBoxDioMapping.Size = new Size(293, 179);
			this.gBoxDioMapping.TabIndex = 1;
			this.gBoxDioMapping.TabStop = false;
			this.gBoxDioMapping.Text = "DIO mapping";
			this.gBoxDioMapping.MouseEnter += new EventHandler(this.control_MouseEnter);
			this.gBoxDioMapping.MouseLeave += new EventHandler(this.control_MouseLeave);
			this.cBoxDio5Mapping.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxDio5Mapping.FormattingEnabled = true;
			this.cBoxDio5Mapping.Location = new Point(162, 19);
			this.cBoxDio5Mapping.Name = "cBoxDio5Mapping";
			this.cBoxDio5Mapping.Size = new Size(100, 21);
			this.cBoxDio5Mapping.TabIndex = 1;
			this.cBoxDio5Mapping.SelectedIndexChanged += new EventHandler(this.cBoxDio5Mapping_SelectedIndexChanged);
			this.cBoxDio4Mapping.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxDio4Mapping.FormattingEnabled = true;
			this.cBoxDio4Mapping.Location = new Point(162, 46);
			this.cBoxDio4Mapping.Name = "cBoxDio4Mapping";
			this.cBoxDio4Mapping.Size = new Size(100, 21);
			this.cBoxDio4Mapping.TabIndex = 3;
			this.cBoxDio4Mapping.SelectedIndexChanged += new EventHandler(this.cBoxDio4Mapping_SelectedIndexChanged);
			this.label2.AutoSize = true;
			this.label2.Location = new Point(6, 23);
			this.label2.Margin = new Padding(3);
			this.label2.Name = "label2";
			this.label2.Size = new Size(35, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "DIO5:";
			this.label2.TextAlign = ContentAlignment.MiddleLeft;
			this.label7.AutoSize = true;
			this.label7.Location = new Point(6, 158);
			this.label7.Margin = new Padding(3);
			this.label7.Name = "label7";
			this.label7.Size = new Size(35, 13);
			this.label7.TabIndex = 10;
			this.label7.Text = "DIO0:";
			this.label7.TextAlign = ContentAlignment.MiddleLeft;
			this.cBoxDio3Mapping.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxDio3Mapping.FormattingEnabled = true;
			this.cBoxDio3Mapping.Location = new Point(162, 73);
			this.cBoxDio3Mapping.Name = "cBoxDio3Mapping";
			this.cBoxDio3Mapping.Size = new Size(100, 21);
			this.cBoxDio3Mapping.TabIndex = 5;
			this.cBoxDio3Mapping.SelectedIndexChanged += new EventHandler(this.cBoxDio3Mapping_SelectedIndexChanged);
			this.cBoxDio0Mapping.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxDio0Mapping.FormattingEnabled = true;
			this.cBoxDio0Mapping.Location = new Point(162, 154);
			this.cBoxDio0Mapping.Name = "cBoxDio0Mapping";
			this.cBoxDio0Mapping.Size = new Size(100, 21);
			this.cBoxDio0Mapping.TabIndex = 11;
			this.cBoxDio0Mapping.SelectedIndexChanged += new EventHandler(this.cBoxDio0Mapping_SelectedIndexChanged);
			this.label3.AutoSize = true;
			this.label3.Location = new Point(6, 50);
			this.label3.Margin = new Padding(3);
			this.label3.Name = "label3";
			this.label3.Size = new Size(35, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "DIO4:";
			this.label3.TextAlign = ContentAlignment.MiddleLeft;
			this.cBoxDio1Mapping.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxDio1Mapping.FormattingEnabled = true;
			this.cBoxDio1Mapping.Location = new Point(162, (int)sbyte.MaxValue);
			this.cBoxDio1Mapping.Name = "cBoxDio1Mapping";
			this.cBoxDio1Mapping.Size = new Size(100, 21);
			this.cBoxDio1Mapping.TabIndex = 9;
			this.cBoxDio1Mapping.SelectedIndexChanged += new EventHandler(this.cBoxDio1Mapping_SelectedIndexChanged);
			this.label4.AutoSize = true;
			this.label4.Location = new Point(6, 77);
			this.label4.Margin = new Padding(3);
			this.label4.Name = "label4";
			this.label4.Size = new Size(35, 13);
			this.label4.TabIndex = 4;
			this.label4.Text = "DIO3:";
			this.label4.TextAlign = ContentAlignment.MiddleLeft;
			this.cBoxDio2Mapping.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cBoxDio2Mapping.FormattingEnabled = true;
			this.cBoxDio2Mapping.Location = new Point(162, 100);
			this.cBoxDio2Mapping.Name = "cBoxDio2Mapping";
			this.cBoxDio2Mapping.Size = new Size(100, 21);
			this.cBoxDio2Mapping.TabIndex = 7;
			this.cBoxDio2Mapping.SelectedIndexChanged += new EventHandler(this.cBoxDio2Mapping_SelectedIndexChanged);
			this.label5.AutoSize = true;
			this.label5.Location = new Point(6, 104);
			this.label5.Margin = new Padding(3);
			this.label5.Name = "label5";
			this.label5.Size = new Size(35, 13);
			this.label5.TabIndex = 6;
			this.label5.Text = "DIO2:";
			this.label5.TextAlign = ContentAlignment.MiddleLeft;
			this.label6.AutoSize = true;
			this.label6.Location = new Point(6, 131);
			this.label6.Margin = new Padding(3);
			this.label6.Name = "label6";
			this.label6.Size = new Size(35, 13);
			this.label6.TabIndex = 8;
			this.label6.Text = "DIO1:";
			this.label6.TextAlign = ContentAlignment.MiddleLeft;
			this.gBoxDioSettings.Controls.Add((Control)this.pnlPreambleIrq);
			this.gBoxDioSettings.Controls.Add((Control)this.label9);
			this.gBoxDioSettings.Location = new Point(253, 145);
			this.gBoxDioSettings.Name = "gBoxDioSettings";
			this.gBoxDioSettings.Size = new Size(293, 50);
			this.gBoxDioSettings.TabIndex = 0;
			this.gBoxDioSettings.TabStop = false;
			this.gBoxDioSettings.Text = "DIO settings";
			this.gBoxDioSettings.MouseEnter += new EventHandler(this.control_MouseEnter);
			this.gBoxDioSettings.MouseLeave += new EventHandler(this.control_MouseLeave);
			this.pnlPreambleIrq.AutoSize = true;
			this.pnlPreambleIrq.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.pnlPreambleIrq.Controls.Add((Control)this.rBtnPreambleIrqOff);
			this.pnlPreambleIrq.Controls.Add((Control)this.rBtnPreambleIrqOn);
			this.pnlPreambleIrq.Location = new Point(162, 19);
			this.pnlPreambleIrq.Name = "pnlPreambleIrq";
			this.pnlPreambleIrq.Size = new Size(102, 20);
			this.pnlPreambleIrq.TabIndex = 3;
			this.rBtnPreambleIrqOff.AutoSize = true;
			this.rBtnPreambleIrqOff.Location = new Point(54, 3);
			this.rBtnPreambleIrqOff.Margin = new Padding(3, 0, 3, 0);
			this.rBtnPreambleIrqOff.Name = "rBtnPreambleIrqOff";
			this.rBtnPreambleIrqOff.Size = new Size(45, 17);
			this.rBtnPreambleIrqOff.TabIndex = 1;
			this.rBtnPreambleIrqOff.Text = "OFF";
			this.rBtnPreambleIrqOff.UseVisualStyleBackColor = true;
			this.rBtnPreambleIrqOff.CheckedChanged += new EventHandler(this.rBtnPreambleIrq_CheckedChanged);
			this.rBtnPreambleIrqOn.AutoSize = true;
			this.rBtnPreambleIrqOn.Checked = true;
			this.rBtnPreambleIrqOn.Location = new Point(3, 3);
			this.rBtnPreambleIrqOn.Margin = new Padding(3, 0, 3, 0);
			this.rBtnPreambleIrqOn.Name = "rBtnPreambleIrqOn";
			this.rBtnPreambleIrqOn.Size = new Size(41, 17);
			this.rBtnPreambleIrqOn.TabIndex = 0;
			this.rBtnPreambleIrqOn.TabStop = true;
			this.rBtnPreambleIrqOn.Text = "ON";
			this.rBtnPreambleIrqOn.UseVisualStyleBackColor = true;
			this.rBtnPreambleIrqOn.CheckedChanged += new EventHandler(this.rBtnPreambleIrq_CheckedChanged);
			this.label9.AutoSize = true;
			this.label9.Location = new Point(6, 24);
			this.label9.Name = "label9";
			this.label9.Size = new Size(76, 13);
			this.label9.TabIndex = 2;
			this.label9.Text = "Preamble IRQ:";
			this.gBoxDeviceStatus.Controls.Add((Control)this.lblBitSynchroniser);
			this.gBoxDeviceStatus.Controls.Add((Control)this.lblOperatingMode);
			this.gBoxDeviceStatus.Controls.Add((Control)this.label13);
			this.gBoxDeviceStatus.Controls.Add((Control)this.label1);
			this.gBoxDeviceStatus.Controls.Add((Control)this.label8);
			this.gBoxDeviceStatus.Controls.Add((Control)this.lblDataMode);
			this.gBoxDeviceStatus.Location = new Point(253, 62);
			this.gBoxDeviceStatus.Name = "gBoxDeviceStatus";
			this.gBoxDeviceStatus.Size = new Size(293, 77);
			this.gBoxDeviceStatus.TabIndex = 0;
			this.gBoxDeviceStatus.TabStop = false;
			this.gBoxDeviceStatus.Text = "Device status";
			this.gBoxDeviceStatus.MouseEnter += new EventHandler(this.control_MouseEnter);
			this.gBoxDeviceStatus.MouseLeave += new EventHandler(this.control_MouseLeave);
			this.lblBitSynchroniser.AutoSize = true;
			this.lblBitSynchroniser.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
			this.lblBitSynchroniser.Location = new Point(159, 19);
			this.lblBitSynchroniser.Margin = new Padding(3);
			this.lblBitSynchroniser.Name = "lblBitSynchroniser";
			this.lblBitSynchroniser.Size = new Size(25, 13);
			this.lblBitSynchroniser.TabIndex = 1;
			this.lblBitSynchroniser.Text = "ON";
			this.lblBitSynchroniser.TextAlign = ContentAlignment.MiddleLeft;
			this.lblOperatingMode.AutoSize = true;
			this.lblOperatingMode.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
			this.lblOperatingMode.Location = new Point(159, 57);
			this.lblOperatingMode.Margin = new Padding(3);
			this.lblOperatingMode.Name = "lblOperatingMode";
			this.lblOperatingMode.Size = new Size(39, 13);
			this.lblOperatingMode.TabIndex = 5;
			this.lblOperatingMode.Text = "Sleep";
			this.lblOperatingMode.TextAlign = ContentAlignment.MiddleLeft;
			this.label13.AutoSize = true;
			this.label13.Location = new Point(6, 38);
			this.label13.Margin = new Padding(3);
			this.label13.Name = "label13";
			this.label13.Size = new Size(62, 13);
			this.label13.TabIndex = 2;
			this.label13.Text = "Data mode:";
			this.label13.TextAlign = ContentAlignment.MiddleLeft;
			this.label1.AutoSize = true;
			this.label1.Location = new Point(6, 57);
			this.label1.Margin = new Padding(3);
			this.label1.Name = "label1";
			this.label1.Size = new Size(85, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Operating mode:";
			this.label1.TextAlign = ContentAlignment.MiddleLeft;
			this.label8.AutoSize = true;
			this.label8.Location = new Point(6, 19);
			this.label8.Margin = new Padding(3);
			this.label8.Name = "label8";
			this.label8.Size = new Size(86, 13);
			this.label8.TabIndex = 0;
			this.label8.Text = "Bit Synchronizer:";
			this.label8.TextAlign = ContentAlignment.MiddleLeft;
			this.lblDataMode.AutoSize = true;
			this.lblDataMode.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
			this.lblDataMode.Location = new Point(159, 38);
			this.lblDataMode.Margin = new Padding(3);
			this.lblDataMode.Name = "lblDataMode";
			this.lblDataMode.Size = new Size(47, 13);
			this.lblDataMode.TabIndex = 3;
			this.lblDataMode.Text = "Packet";
			this.lblDataMode.TextAlign = ContentAlignment.MiddleLeft;
			this.AutoScaleDimensions = new SizeF(6f, 13f);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.Controls.Add((Control)this.gBoxClockOut);
			this.Controls.Add((Control)this.gBoxDioMapping);
			this.Controls.Add((Control)this.gBoxDioSettings);
			this.Controls.Add((Control)this.gBoxDeviceStatus);
			this.Name = "IrqMapViewControl";
			this.Size = new Size(799, 493);
			((ISupportInitialize)this.errorProvider).EndInit();
			this.gBoxClockOut.ResumeLayout(false);
			this.gBoxClockOut.PerformLayout();
			this.gBoxDioMapping.ResumeLayout(false);
			this.gBoxDioMapping.PerformLayout();
			this.gBoxDioSettings.ResumeLayout(false);
			this.gBoxDioSettings.PerformLayout();
			this.pnlPreambleIrq.ResumeLayout(false);
			this.pnlPreambleIrq.PerformLayout();
			this.gBoxDeviceStatus.ResumeLayout(false);
			this.gBoxDeviceStatus.PerformLayout();
			this.ResumeLayout(false);
		}
	}
}
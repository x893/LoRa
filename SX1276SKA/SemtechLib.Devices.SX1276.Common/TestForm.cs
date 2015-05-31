using SemtechLib.Controls;
using SemtechLib.Devices.Common.Interfaces;
using SemtechLib.Devices.SX1276;
using SemtechLib.Devices.SX1276LR;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SemtechLib.Devices.SX1276.Common.UI.Forms
{
	public class TestForm : Form
	{
		private delegate void DeviceDataChangedDelegate(object sender, PropertyChangedEventArgs e);

		private string previousValue = "";
		private byte currentAddrValue;
		private byte currentDataValue;
		private byte newAddrValue;
		private byte newDataValue;
		private IDevice device;
		private SX1276 device1;
		private SemtechLib.Devices.SX1276LR.SX1276LR device2;
		private bool testEnabled;
		private IContainer components;
		private TableLayoutPanel tlRegisters;
		private Button btnWrite;
		private Button btnRead;
		private StatusStrip ssStatus;
		private ToolStripStatusLabel tsLblStatus;
		private Label lblAddress;
		private Label lblDataWrite;
		private TextBox tBoxRegAddress;
		private TextBox tBoxRegValue;
		private GroupBoxEx groupBox3;
		private BackgroundWorker backgroundWorker1;
		private GroupBoxEx groupBox4;
		private Panel pnlRfPaSwitchEnable;
		private RadioButton rBtnRfPaSwitchAuto;
		private RadioButton rBtnRfPaSwitchOff;
		private Label label2;
		private Label label1;
		private Label label4;
		private Label label3;
		private RadioButton rBtnRfPaSwitchManual;
		private PictureBox pBoxRfOut4;
		private PictureBox pBoxRfOut3;
		private PictureBox pBoxRfOut2;
		private PictureBox pBoxRfOut1;
		private Panel pnlRfPaSwitchSel;
		private RadioButton rBtnRfPaSwitchPaIo;
		private RadioButton rBtnRfPaSwitchIoPa;
		private Label label44;
		private Label label5;
		private Label label34;
		private Label label32;
		private Label label43;
		private Label label6;
		private Label label35;
		private Label label37;
		private Label label42;
		private Label label33;
		private Label label38;
		private Label label36;
		private Label label41;
		private Label label31;
		private Label label39;
		private Label label40;

		public TestForm()
		{
			InitializeComponent();
		}

		public IDevice Device
		{
			set
			{
				if (device == value)
					return;
				if (value.GetType() == typeof(SX1276))
				{
					device = (IDevice)(device1 = (SX1276)value);
					device.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(device_PropertyChanged);
					switch (device1.RfPaSwitchEnabled)
					{
						case 0:
							rBtnRfPaSwitchAuto.Checked = false;
							rBtnRfPaSwitchManual.Checked = false;
							rBtnRfPaSwitchOff.Checked = true;
							break;
						case 1:
							rBtnRfPaSwitchAuto.Checked = false;
							rBtnRfPaSwitchManual.Checked = true;
							rBtnRfPaSwitchOff.Checked = false;
							break;
						case 2:
							rBtnRfPaSwitchAuto.Checked = true;
							rBtnRfPaSwitchManual.Checked = false;
							rBtnRfPaSwitchOff.Checked = false;
							break;
					}
				}
				else
				{
					if (!(value.GetType() == typeof(SemtechLib.Devices.SX1276LR.SX1276LR)))
						return;
					device = (IDevice)(device2 = (SemtechLib.Devices.SX1276LR.SX1276LR)value);
					device.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(device_PropertyChanged);
					switch (device2.RfPaSwitchEnabled)
					{
						case 0:
							rBtnRfPaSwitchAuto.Checked = false;
							rBtnRfPaSwitchManual.Checked = false;
							rBtnRfPaSwitchOff.Checked = true;
							break;
						case 1:
							rBtnRfPaSwitchAuto.Checked = false;
							rBtnRfPaSwitchManual.Checked = true;
							rBtnRfPaSwitchOff.Checked = false;
							break;
						case 2:
							rBtnRfPaSwitchAuto.Checked = true;
							rBtnRfPaSwitchManual.Checked = false;
							rBtnRfPaSwitchOff.Checked = false;
							break;
					}
				}
			}
		}

		public bool TestEnabled
		{
			get
			{
				return testEnabled;
			}
			set
			{
				testEnabled = value;
				OnTestEnabledChanged(EventArgs.Empty);
			}
		}

		public event EventHandler TestEnabledChanged;

		private void UpdatePaSwitchSelCheck()
		{
			pBoxRfOut1.Visible = false;
			pBoxRfOut2.Visible = false;
			pBoxRfOut3.Visible = false;
			pBoxRfOut4.Visible = false;
			if (device.GetType() == typeof(SX1276))
			{
				if (device1.Mode == SemtechLib.Devices.SX1276.Enumerations.OperatingModeEnum.Tx)
				{
					switch (device1.PaSelect)
					{
						case SemtechLib.Devices.SX1276.Enumerations.PaSelectEnum.RFO:
							switch (device1.RfPaSwitchSel)
							{
								case SemtechLib.Devices.SX1276.Enumerations.RfPaSwitchSelEnum.RF_IO_RFIO:
									pBoxRfOut2.Visible = true;
									return;
								case SemtechLib.Devices.SX1276.Enumerations.RfPaSwitchSelEnum.RF_IO_PA_BOOST:
									pBoxRfOut4.Visible = true;
									return;
								default:
									return;
							}
						case SemtechLib.Devices.SX1276.Enumerations.PaSelectEnum.PA_BOOST:
							switch (device1.RfPaSwitchSel)
							{
								case SemtechLib.Devices.SX1276.Enumerations.RfPaSwitchSelEnum.RF_IO_RFIO:
									pBoxRfOut1.Visible = true;
									return;
								case SemtechLib.Devices.SX1276.Enumerations.RfPaSwitchSelEnum.RF_IO_PA_BOOST:
									pBoxRfOut3.Visible = true;
									return;
								default:
									return;
							}
					}
				}
				else
				{
					switch (device1.RfPaSwitchSel)
					{
						case SemtechLib.Devices.SX1276.Enumerations.RfPaSwitchSelEnum.RF_IO_RFIO:
							pBoxRfOut2.Visible = true;
							break;
						case SemtechLib.Devices.SX1276.Enumerations.RfPaSwitchSelEnum.RF_IO_PA_BOOST:
							pBoxRfOut4.Visible = true;
							break;
					}
				}
			}
			else
			{
				if (!(device.GetType() == typeof(SemtechLib.Devices.SX1276LR.SX1276LR)))
					return;
				if (device2.Mode == SemtechLib.Devices.SX1276LR.Enumerations.OperatingModeEnum.Tx)
				{
					switch (device2.PaSelect)
					{
						case SemtechLib.Devices.SX1276LR.Enumerations.PaSelectEnum.RFO:
							switch (device2.RfPaSwitchSel)
							{
								case SemtechLib.Devices.SX1276LR.Enumerations.RfPaSwitchSelEnum.RF_IO_RFIO:
									pBoxRfOut2.Visible = true;
									return;
								case SemtechLib.Devices.SX1276LR.Enumerations.RfPaSwitchSelEnum.RF_IO_PA_BOOST:
									pBoxRfOut4.Visible = true;
									return;
								default:
									return;
							}
						case SemtechLib.Devices.SX1276LR.Enumerations.PaSelectEnum.PA_BOOST:
							switch (device2.RfPaSwitchSel)
							{
								case SemtechLib.Devices.SX1276LR.Enumerations.RfPaSwitchSelEnum.RF_IO_RFIO:
									pBoxRfOut1.Visible = true;
									return;
								case SemtechLib.Devices.SX1276LR.Enumerations.RfPaSwitchSelEnum.RF_IO_PA_BOOST:
									pBoxRfOut3.Visible = true;
									return;
								default:
									return;
							}
					}
				}
				else
				{
					switch (device2.RfPaSwitchSel)
					{
						case SemtechLib.Devices.SX1276LR.Enumerations.RfPaSwitchSelEnum.RF_IO_RFIO:
							pBoxRfOut2.Visible = true;
							break;
						case SemtechLib.Devices.SX1276LR.Enumerations.RfPaSwitchSelEnum.RF_IO_PA_BOOST:
							pBoxRfOut4.Visible = true;
							break;
					}
				}
			}
		}

		private void OnDevicePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "Mode":
				case "PaSelect":
					UpdatePaSwitchSelCheck();
					break;
				case "RfPaSwitchSel":
					rBtnRfPaSwitchPaIo.CheckedChanged -= new EventHandler(rBtnRfPaSwitchSel_CheckedChanged);
					rBtnRfPaSwitchIoPa.CheckedChanged -= new EventHandler(rBtnRfPaSwitchSel_CheckedChanged);
					if (device.GetType() == typeof(SX1276))
					{
						if (device1.RfPaSwitchEnabled != 2)
						{
							UpdatePaSwitchSelCheck();
							switch (device1.RfPaSwitchSel)
							{
								case SemtechLib.Devices.SX1276.Enumerations.RfPaSwitchSelEnum.RF_IO_RFIO:
									rBtnRfPaSwitchPaIo.Checked = true;
									rBtnRfPaSwitchIoPa.Checked = false;
									break;
								case SemtechLib.Devices.SX1276.Enumerations.RfPaSwitchSelEnum.RF_IO_PA_BOOST:
									rBtnRfPaSwitchPaIo.Checked = false;
									rBtnRfPaSwitchIoPa.Checked = true;
									break;
							}
						}
					}
					else if (device.GetType() == typeof(SemtechLib.Devices.SX1276LR.SX1276LR) && device2.RfPaSwitchEnabled != 2)
					{
						UpdatePaSwitchSelCheck();
						switch (device2.RfPaSwitchSel)
						{
							case SemtechLib.Devices.SX1276LR.Enumerations.RfPaSwitchSelEnum.RF_IO_RFIO:
								rBtnRfPaSwitchPaIo.Checked = true;
								rBtnRfPaSwitchIoPa.Checked = false;
								break;
							case SemtechLib.Devices.SX1276LR.Enumerations.RfPaSwitchSelEnum.RF_IO_PA_BOOST:
								rBtnRfPaSwitchPaIo.Checked = false;
								rBtnRfPaSwitchIoPa.Checked = true;
								break;
						}
					}
					rBtnRfPaSwitchPaIo.CheckedChanged += new EventHandler(rBtnRfPaSwitchSel_CheckedChanged);
					rBtnRfPaSwitchIoPa.CheckedChanged += new EventHandler(rBtnRfPaSwitchSel_CheckedChanged);
					break;
				case "RfPaSwitchEnabled":
					rBtnRfPaSwitchAuto.CheckedChanged -= new EventHandler(rBtnRfPaSwitchEnable_CheckedChanged);
					rBtnRfPaSwitchManual.CheckedChanged -= new EventHandler(rBtnRfPaSwitchEnable_CheckedChanged);
					rBtnRfPaSwitchOff.CheckedChanged -= new EventHandler(rBtnRfPaSwitchEnable_CheckedChanged);
					if (device.GetType() == typeof(SX1276))
					{
						switch (device1.RfPaSwitchEnabled)
						{
							case 0:
								rBtnRfPaSwitchAuto.Checked = false;
								rBtnRfPaSwitchManual.Checked = false;
								rBtnRfPaSwitchOff.Checked = true;
								break;
							case 1:
								rBtnRfPaSwitchAuto.Checked = false;
								rBtnRfPaSwitchManual.Checked = true;
								rBtnRfPaSwitchOff.Checked = false;
								break;
							case 2:
								rBtnRfPaSwitchAuto.Checked = true;
								rBtnRfPaSwitchManual.Checked = false;
								rBtnRfPaSwitchOff.Checked = false;
								break;
						}
					}
					else if (device.GetType() == typeof(SemtechLib.Devices.SX1276LR.SX1276LR))
					{
						switch (device2.RfPaSwitchEnabled)
						{
							case 0:
								rBtnRfPaSwitchAuto.Checked = false;
								rBtnRfPaSwitchManual.Checked = false;
								rBtnRfPaSwitchOff.Checked = true;
								break;
							case 1:
								rBtnRfPaSwitchAuto.Checked = false;
								rBtnRfPaSwitchManual.Checked = true;
								rBtnRfPaSwitchOff.Checked = false;
								break;
							case 2:
								rBtnRfPaSwitchAuto.Checked = true;
								rBtnRfPaSwitchManual.Checked = false;
								rBtnRfPaSwitchOff.Checked = false;
								break;
						}
					}
					pnlRfPaSwitchSel.Enabled = rBtnRfPaSwitchManual.Checked;
					rBtnRfPaSwitchAuto.CheckedChanged += new EventHandler(rBtnRfPaSwitchEnable_CheckedChanged);
					rBtnRfPaSwitchManual.CheckedChanged += new EventHandler(rBtnRfPaSwitchEnable_CheckedChanged);
					rBtnRfPaSwitchOff.CheckedChanged += new EventHandler(rBtnRfPaSwitchEnable_CheckedChanged);
					break;
			}
		}

		private void OnTestEnabledChanged(EventArgs e)
		{
			if (TestEnabledChanged == null)
				return;
			TestEnabledChanged((object)this, e);
		}

		private void device_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (InvokeRequired)
				BeginInvoke((Delegate)new TestForm.DeviceDataChangedDelegate(OnDevicePropertyChanged), sender, (object)e);
			else
				OnDevicePropertyChanged(sender, e);
		}

		private void TestForm_Load(object sender, EventArgs e)
		{
		}

		private void TestForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
			Hide();
			TestEnabled = false;
		}

		private void TestForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
			{
				e.Handled = true;
				SendKeys.Send("{TAB}");
			}
			else
			{
				if (e.KeyData != (Keys.T | Keys.Control | Keys.Alt))
					return;
				Hide();
				TestEnabled = false;
			}
		}

		private void TestForm_Activated(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				tsLblStatus.Text = "-";
				Refresh();
			}
			catch (Exception ex)
			{
				tsLblStatus.Text = ex.Message;
			}
			finally
			{
				Refresh();
				Cursor = Cursors.Default;
			}
		}

		private void btnWrite_Click(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				tsLblStatus.Text = "-";
				Refresh();
				bool flag = false;
				if (device.GetType() == typeof(SX1276))
					flag = device1.Write(newAddrValue, newDataValue);
				else if (device.GetType() == typeof(SemtechLib.Devices.SX1276LR.SX1276LR))
					flag = device2.Write(newAddrValue, newDataValue);
				if (!flag)
					throw new Exception("ERROR: Writing command");
				currentAddrValue = newAddrValue;
				tBoxRegAddress.ForeColor = SystemColors.WindowText;
				currentDataValue = newDataValue;
				tBoxRegValue.ForeColor = SystemColors.WindowText;
			}
			catch (Exception ex)
			{
				tsLblStatus.Text = ex.Message;
			}
			finally
			{
				Refresh();
				Cursor = Cursors.Default;
			}
		}

		private void btnRead_Click(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				tsLblStatus.Text = "-";
				Refresh();
				byte data = (byte)0;
				bool flag = false;
				if (device.GetType() == typeof(SX1276))
					flag = device1.Read(newAddrValue, ref data);
				else if (device.GetType() == typeof(SemtechLib.Devices.SX1276LR.SX1276LR))
					flag = device2.Read(newAddrValue, ref data);
				if (!flag)
					throw new Exception("ERROR: Reading command");
				currentAddrValue = newAddrValue;
				tBoxRegAddress.ForeColor = SystemColors.WindowText;
				tBoxRegValue.Text = "0x" + data.ToString("X02");
				currentDataValue = newDataValue = data;
				tBoxRegValue.ForeColor = SystemColors.WindowText;
			}
			catch (Exception ex)
			{
				tsLblStatus.Text = ex.Message;
			}
			finally
			{
				Refresh();
				Cursor = Cursors.Default;
			}
		}

		private void tBox_TextChanged(object sender, EventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			try
			{
				if (textBox == tBoxRegAddress)
				{
					byte num = currentAddrValue;
					if (textBox.Text != "0x" + num.ToString("X02"))
						textBox.ForeColor = Color.Red;
					else
						textBox.ForeColor = SystemColors.WindowText;
					if (textBox.Text == "0x")
						return;
					newAddrValue = Convert.ToByte(textBox.Text, 16);
				}
				else
				{
					if (textBox != tBoxRegValue)
						return;
					byte num = currentDataValue;
					if (textBox.Text != "0x" + num.ToString("X02"))
						textBox.ForeColor = Color.Red;
					else
						textBox.ForeColor = SystemColors.WindowText;
					if (textBox.Text == "0x")
						return;
					newDataValue = Convert.ToByte(textBox.Text, 16);
				}
			}
			catch (Exception)
			{
			}
		}

		private void txtBox_Enter(object sender, EventArgs e)
		{
			previousValue = ((Control)sender).Text;
		}

		private void txtBox_Validating(object sender, CancelEventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			byte num1 = (byte)0;
			byte num2 = byte.MaxValue;
			try
			{
				int num3 = (int)Convert.ToByte(textBox.Text, 16);
			}
			catch (Exception ex)
			{
				int num3 = (int)MessageBox.Show(ex.Message + "\rInput Format: Hex 0x" + num1.ToString("X02") + " - 0x" + num2.ToString("X02"), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				textBox.Text = previousValue;
			}
		}

		private void txtBox_Validated(object sender, EventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			textBox.Text = "0x" + Convert.ToByte(textBox.Text, 16).ToString("X02");
		}

		private void rBtnRfPaSwitchEnable_CheckedChanged(object sender, EventArgs e)
		{
			if (device.GetType() == typeof(SX1276))
				device1.RfPaSwitchEnabled = !rBtnRfPaSwitchAuto.Checked ? (!rBtnRfPaSwitchManual.Checked ? 0 : 1) : 2;
			else if (device.GetType() == typeof(SemtechLib.Devices.SX1276LR.SX1276LR))
				device2.RfPaSwitchEnabled = !rBtnRfPaSwitchAuto.Checked ? (!rBtnRfPaSwitchManual.Checked ? 0 : 1) : 2;
			pnlRfPaSwitchSel.Enabled = rBtnRfPaSwitchManual.Checked;
		}

		private void rBtnRfPaSwitchSel_CheckedChanged(object sender, EventArgs e)
		{
			if (device.GetType() == typeof(SX1276))
			{
				device1.RfPaSwitchSel = rBtnRfPaSwitchPaIo.Checked ? SemtechLib.Devices.SX1276.Enumerations.RfPaSwitchSelEnum.RF_IO_RFIO : SemtechLib.Devices.SX1276.Enumerations.RfPaSwitchSelEnum.RF_IO_PA_BOOST;
			}
			else
			{
				if (!(device.GetType() == typeof(SemtechLib.Devices.SX1276LR.SX1276LR)))
					return;
				device2.RfPaSwitchSel = rBtnRfPaSwitchPaIo.Checked ? SemtechLib.Devices.SX1276LR.Enumerations.RfPaSwitchSelEnum.RF_IO_RFIO : SemtechLib.Devices.SX1276LR.Enumerations.RfPaSwitchSelEnum.RF_IO_PA_BOOST;
			}
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
			ComponentResourceManager resources = new ComponentResourceManager(typeof(TestForm));
			this.ssStatus = new StatusStrip();
			this.tsLblStatus = new ToolStripStatusLabel();
			this.backgroundWorker1 = new BackgroundWorker();
			this.groupBox4 = new GroupBoxEx();
			this.pBoxRfOut4 = new PictureBox();
			this.pBoxRfOut3 = new PictureBox();
			this.pBoxRfOut2 = new PictureBox();
			this.pBoxRfOut1 = new PictureBox();
			this.pnlRfPaSwitchSel = new Panel();
			this.rBtnRfPaSwitchPaIo = new RadioButton();
			this.rBtnRfPaSwitchIoPa = new RadioButton();
			this.label44 = new Label();
			this.label5 = new Label();
			this.label34 = new Label();
			this.label32 = new Label();
			this.label43 = new Label();
			this.label6 = new Label();
			this.label35 = new Label();
			this.label37 = new Label();
			this.label42 = new Label();
			this.label33 = new Label();
			this.label38 = new Label();
			this.label36 = new Label();
			this.label41 = new Label();
			this.label31 = new Label();
			this.label39 = new Label();
			this.label40 = new Label();
			this.label4 = new Label();
			this.label3 = new Label();
			this.label2 = new Label();
			this.label1 = new Label();
			this.pnlRfPaSwitchEnable = new Panel();
			this.rBtnRfPaSwitchAuto = new RadioButton();
			this.rBtnRfPaSwitchManual = new RadioButton();
			this.rBtnRfPaSwitchOff = new RadioButton();
			this.groupBox3 = new GroupBoxEx();
			this.btnRead = new Button();
			this.tlRegisters = new TableLayoutPanel();
			this.lblAddress = new Label();
			this.lblDataWrite = new Label();
			this.tBoxRegAddress = new TextBox();
			this.tBoxRegValue = new TextBox();
			this.btnWrite = new Button();
			this.ssStatus.SuspendLayout();
			this.groupBox4.SuspendLayout();
			((ISupportInitialize)this.pBoxRfOut4).BeginInit();
			((ISupportInitialize)this.pBoxRfOut3).BeginInit();
			((ISupportInitialize)this.pBoxRfOut2).BeginInit();
			((ISupportInitialize)this.pBoxRfOut1).BeginInit();
			this.pnlRfPaSwitchSel.SuspendLayout();
			this.pnlRfPaSwitchEnable.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.tlRegisters.SuspendLayout();
			this.SuspendLayout();
			this.ssStatus.Items.AddRange(new ToolStripItem[1]
      {
        (ToolStripItem) this.tsLblStatus
      });
			this.ssStatus.Location = new Point(0, 298);
			this.ssStatus.Name = "ssStatus";
			this.ssStatus.Size = new Size(319, 22);
			this.ssStatus.TabIndex = 1;
			this.ssStatus.Text = "statusStrip1";
			this.tsLblStatus.Name = "tsLblStatus";
			this.tsLblStatus.Size = new Size(12, 17);
			this.tsLblStatus.Text = "-";
			this.backgroundWorker1.WorkerReportsProgress = true;
			this.groupBox4.Controls.Add((Control)this.pBoxRfOut4);
			this.groupBox4.Controls.Add((Control)this.pBoxRfOut3);
			this.groupBox4.Controls.Add((Control)this.pBoxRfOut2);
			this.groupBox4.Controls.Add((Control)this.pBoxRfOut1);
			this.groupBox4.Controls.Add((Control)this.pnlRfPaSwitchSel);
			this.groupBox4.Controls.Add((Control)this.label44);
			this.groupBox4.Controls.Add((Control)this.label5);
			this.groupBox4.Controls.Add((Control)this.label34);
			this.groupBox4.Controls.Add((Control)this.label32);
			this.groupBox4.Controls.Add((Control)this.label43);
			this.groupBox4.Controls.Add((Control)this.label6);
			this.groupBox4.Controls.Add((Control)this.label35);
			this.groupBox4.Controls.Add((Control)this.label37);
			this.groupBox4.Controls.Add((Control)this.label42);
			this.groupBox4.Controls.Add((Control)this.label33);
			this.groupBox4.Controls.Add((Control)this.label38);
			this.groupBox4.Controls.Add((Control)this.label36);
			this.groupBox4.Controls.Add((Control)this.label41);
			this.groupBox4.Controls.Add((Control)this.label31);
			this.groupBox4.Controls.Add((Control)this.label39);
			this.groupBox4.Controls.Add((Control)this.label40);
			this.groupBox4.Controls.Add((Control)this.label4);
			this.groupBox4.Controls.Add((Control)this.label3);
			this.groupBox4.Controls.Add((Control)this.label2);
			this.groupBox4.Controls.Add((Control)this.label1);
			this.groupBox4.Controls.Add((Control)this.pnlRfPaSwitchEnable);
			this.groupBox4.Location = new Point(12, 118);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new Size(295, 167);
			this.groupBox4.TabIndex = 4;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Antena switch control";
			this.groupBox4.Visible = false;
			this.pBoxRfOut4.Image = (Image)resources.GetObject("pBoxRfOut4.Image");
			this.pBoxRfOut4.Location = new Point(272, 105);
			this.pBoxRfOut4.Name = "pBoxRfOut4";
			this.pBoxRfOut4.Size = new Size(16, 16);
			this.pBoxRfOut4.TabIndex = 21;
			this.pBoxRfOut4.TabStop = false;
			this.pBoxRfOut4.Visible = false;
			this.pBoxRfOut3.Image = (Image)resources.GetObject("pBoxRfOut3.Image");
			this.pBoxRfOut3.Location = new Point(272, 86);
			this.pBoxRfOut3.Name = "pBoxRfOut3";
			this.pBoxRfOut3.Size = new Size(16, 16);
			this.pBoxRfOut3.TabIndex = 22;
			this.pBoxRfOut3.TabStop = false;
			this.pBoxRfOut3.Visible = false;
			this.pBoxRfOut2.Image = (Image)resources.GetObject("pBoxRfOut2.Image");
			this.pBoxRfOut2.Location = new Point(272, 67);
			this.pBoxRfOut2.Name = "pBoxRfOut2";
			this.pBoxRfOut2.Size = new Size(16, 16);
			this.pBoxRfOut2.TabIndex = 24;
			this.pBoxRfOut2.TabStop = false;
			this.pBoxRfOut1.Image = (Image)resources.GetObject("pBoxRfOut1.Image");
			this.pBoxRfOut1.Location = new Point(272, 48);
			this.pBoxRfOut1.Name = "pBoxRfOut1";
			this.pBoxRfOut1.Size = new Size(16, 16);
			this.pBoxRfOut1.TabIndex = 23;
			this.pBoxRfOut1.TabStop = false;
			this.pBoxRfOut1.Visible = false;
			this.pnlRfPaSwitchSel.AutoSize = true;
			this.pnlRfPaSwitchSel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.pnlRfPaSwitchSel.Controls.Add((Control)this.rBtnRfPaSwitchPaIo);
			this.pnlRfPaSwitchSel.Controls.Add((Control)this.rBtnRfPaSwitchIoPa);
			this.pnlRfPaSwitchSel.Enabled = false;
			this.pnlRfPaSwitchSel.Location = new Point(70, 47);
			this.pnlRfPaSwitchSel.Name = "pnlRfPaSwitchSel";
			this.pnlRfPaSwitchSel.Size = new Size(20, 72);
			this.pnlRfPaSwitchSel.TabIndex = 4;
			this.rBtnRfPaSwitchPaIo.AutoSize = true;
			this.rBtnRfPaSwitchPaIo.Checked = true;
			this.rBtnRfPaSwitchPaIo.Location = new Point(3, 3);
			this.rBtnRfPaSwitchPaIo.MinimumSize = new Size(0, 30);
			this.rBtnRfPaSwitchPaIo.Name = "rBtnRfPaSwitchPaIo";
			this.rBtnRfPaSwitchPaIo.Size = new Size(14, 30);
			this.rBtnRfPaSwitchPaIo.TabIndex = 0;
			this.rBtnRfPaSwitchPaIo.TabStop = true;
			this.rBtnRfPaSwitchPaIo.UseVisualStyleBackColor = true;
			this.rBtnRfPaSwitchPaIo.CheckedChanged += new EventHandler(this.rBtnRfPaSwitchSel_CheckedChanged);
			this.rBtnRfPaSwitchIoPa.AutoSize = true;
			this.rBtnRfPaSwitchIoPa.Location = new Point(3, 39);
			this.rBtnRfPaSwitchIoPa.Margin = new Padding(3, 4, 3, 3);
			this.rBtnRfPaSwitchIoPa.MinimumSize = new Size(0, 30);
			this.rBtnRfPaSwitchIoPa.Name = "rBtnRfPaSwitchIoPa";
			this.rBtnRfPaSwitchIoPa.Size = new Size(14, 30);
			this.rBtnRfPaSwitchIoPa.TabIndex = 0;
			this.rBtnRfPaSwitchIoPa.UseVisualStyleBackColor = true;
			this.rBtnRfPaSwitchIoPa.CheckedChanged += new EventHandler(this.rBtnRfPaSwitchSel_CheckedChanged);
			this.label44.AutoSize = true;
			this.label44.Location = new Point(96, 107);
			this.label44.Margin = new Padding(3);
			this.label44.Name = "label44";
			this.label44.Size = new Size(22, 13);
			this.label44.TabIndex = 16;
			this.label44.Text = "Pin";
			this.label5.AutoSize = true;
			this.label5.Location = new Point(96, 50);
			this.label5.Margin = new Padding(3);
			this.label5.Name = "label5";
			this.label5.Size = new Size(22, 13);
			this.label5.TabIndex = 15;
			this.label5.Text = "Pin";
			this.label34.AutoSize = true;
			this.label34.Location = new Point(194, 69);
			this.label34.Margin = new Padding(3);
			this.label34.Name = "label34";
			this.label34.Size = new Size(25, 13);
			this.label34.TabIndex = 20;
			this.label34.Text = "<=>";
			this.label32.AutoSize = true;
			this.label32.Location = new Point(124, 69);
			this.label32.Margin = new Padding(3);
			this.label32.Name = "label32";
			this.label32.Size = new Size(32, 13);
			this.label32.TabIndex = 19;
			this.label32.Text = "RFIO";
			this.label43.AutoSize = true;
			this.label43.Location = new Point(194, 107);
			this.label43.Margin = new Padding(3);
			this.label43.Name = "label43";
			this.label43.Size = new Size(25, 13);
			this.label43.TabIndex = 18;
			this.label43.Text = "<=>";
			this.label6.AutoSize = true;
			this.label6.Location = new Point(96, 69);
			this.label6.Margin = new Padding(3);
			this.label6.Name = "label6";
			this.label6.Size = new Size(22, 13);
			this.label6.TabIndex = 17;
			this.label6.Text = "Pin";
			this.label35.AutoSize = true;
			this.label35.Location = new Point(225, 69);
			this.label35.Margin = new Padding(3);
			this.label35.Name = "label35";
			this.label35.Size = new Size(38, 13);
			this.label35.TabIndex = 7;
			this.label35.Text = "RF_IO";
			this.label37.AutoSize = true;
			this.label37.Location = new Point(96, 88);
			this.label37.Margin = new Padding(3);
			this.label37.Name = "label37";
			this.label37.Size = new Size(22, 13);
			this.label37.TabIndex = 8;
			this.label37.Text = "Pin";
			this.label42.AutoSize = true;
			this.label42.Location = new Point(225, 107);
			this.label42.Margin = new Padding(3);
			this.label42.Name = "label42";
			this.label42.Size = new Size(41, 13);
			this.label42.TabIndex = 5;
			this.label42.Text = "RF_PA";
			this.label33.AutoSize = true;
			this.label33.Location = new Point(194, 50);
			this.label33.Margin = new Padding(3);
			this.label33.Name = "label33";
			this.label33.Size = new Size(25, 13);
			this.label33.TabIndex = 6;
			this.label33.Text = "<=>";
			this.label38.AutoSize = true;
			this.label38.Location = new Point(124, 88);
			this.label38.Margin = new Padding(3);
			this.label38.Name = "label38";
			this.label38.Size = new Size(64, 13);
			this.label38.TabIndex = 9;
			this.label38.Text = "PA_BOOST";
			this.label36.AutoSize = true;
			this.label36.Location = new Point(225, 50);
			this.label36.Margin = new Padding(3);
			this.label36.Name = "label36";
			this.label36.Size = new Size(41, 13);
			this.label36.TabIndex = 13;
			this.label36.Text = "RF_PA";
			this.label41.AutoSize = true;
			this.label41.Location = new Point(225, 88);
			this.label41.Margin = new Padding(3);
			this.label41.Name = "label41";
			this.label41.Size = new Size(38, 13);
			this.label41.TabIndex = 10;
			this.label41.Text = "RF_IO";
			this.label31.AutoSize = true;
			this.label31.Location = new Point(124, 50);
			this.label31.Margin = new Padding(3);
			this.label31.Name = "label31";
			this.label31.Size = new Size(64, 13);
			this.label31.TabIndex = 11;
			this.label31.Text = "PA_BOOST";
			this.label39.AutoSize = true;
			this.label39.Location = new Point(194, 88);
			this.label39.Margin = new Padding(3);
			this.label39.Name = "label39";
			this.label39.Size = new Size(25, 13);
			this.label39.TabIndex = 12;
			this.label39.Text = "<=>";
			this.label40.AutoSize = true;
			this.label40.Location = new Point(124, 107);
			this.label40.Margin = new Padding(3);
			this.label40.Name = "label40";
			this.label40.Size = new Size(32, 13);
			this.label40.TabIndex = 14;
			this.label40.Text = "RFIO";
			this.label4.Location = new Point(45, 130);
			this.label4.Name = "label4";
			this.label4.Size = new Size(192, 28);
			this.label4.TabIndex = 3;
			this.label4.Text = "To be used only on antenna diversity ref design.";
			this.label3.AutoSize = true;
			this.label3.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
			this.label3.Location = new Point(6, 130);
			this.label3.Name = "label3";
			this.label3.Size = new Size(38, 13);
			this.label3.TabIndex = 3;
			this.label3.Text = "Note:";
			this.label2.AutoSize = true;
			this.label2.Location = new Point(6, 77);
			this.label2.Name = "label2";
			this.label2.Size = new Size(54, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Selection:";
			this.label1.AutoSize = true;
			this.label1.Location = new Point(6, 24);
			this.label1.Name = "label1";
			this.label1.Size = new Size(42, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Switch:";
			this.pnlRfPaSwitchEnable.AutoSize = true;
			this.pnlRfPaSwitchEnable.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.pnlRfPaSwitchEnable.Controls.Add((Control)this.rBtnRfPaSwitchAuto);
			this.pnlRfPaSwitchEnable.Controls.Add((Control)this.rBtnRfPaSwitchManual);
			this.pnlRfPaSwitchEnable.Controls.Add((Control)this.rBtnRfPaSwitchOff);
			this.pnlRfPaSwitchEnable.Location = new Point(70, 19);
			this.pnlRfPaSwitchEnable.Name = "pnlRfPaSwitchEnable";
			this.pnlRfPaSwitchEnable.Size = new Size(170, 23);
			this.pnlRfPaSwitchEnable.TabIndex = 1;
			this.rBtnRfPaSwitchAuto.AutoSize = true;
			this.rBtnRfPaSwitchAuto.Location = new Point(3, 3);
			this.rBtnRfPaSwitchAuto.Name = "rBtnRfPaSwitchAuto";
			this.rBtnRfPaSwitchAuto.Size = new Size(47, 17);
			this.rBtnRfPaSwitchAuto.TabIndex = 0;
			this.rBtnRfPaSwitchAuto.Text = "Auto";
			this.rBtnRfPaSwitchAuto.UseVisualStyleBackColor = true;
			this.rBtnRfPaSwitchAuto.CheckedChanged += new EventHandler(this.rBtnRfPaSwitchEnable_CheckedChanged);
			this.rBtnRfPaSwitchManual.AutoSize = true;
			this.rBtnRfPaSwitchManual.Location = new Point(56, 2);
			this.rBtnRfPaSwitchManual.Name = "rBtnRfPaSwitchManual";
			this.rBtnRfPaSwitchManual.Size = new Size(60, 17);
			this.rBtnRfPaSwitchManual.TabIndex = 0;
			this.rBtnRfPaSwitchManual.Text = "Manual";
			this.rBtnRfPaSwitchManual.UseVisualStyleBackColor = true;
			this.rBtnRfPaSwitchManual.CheckedChanged += new EventHandler(this.rBtnRfPaSwitchEnable_CheckedChanged);
			this.rBtnRfPaSwitchOff.AutoSize = true;
			this.rBtnRfPaSwitchOff.Checked = true;
			this.rBtnRfPaSwitchOff.Location = new Point(122, 3);
			this.rBtnRfPaSwitchOff.Name = "rBtnRfPaSwitchOff";
			this.rBtnRfPaSwitchOff.Size = new Size(45, 17);
			this.rBtnRfPaSwitchOff.TabIndex = 0;
			this.rBtnRfPaSwitchOff.TabStop = true;
			this.rBtnRfPaSwitchOff.Text = "OFF";
			this.rBtnRfPaSwitchOff.UseVisualStyleBackColor = true;
			this.rBtnRfPaSwitchOff.CheckedChanged += new EventHandler(this.rBtnRfPaSwitchEnable_CheckedChanged);
			this.groupBox3.Controls.Add((Control)this.btnRead);
			this.groupBox3.Controls.Add((Control)this.tlRegisters);
			this.groupBox3.Controls.Add((Control)this.btnWrite);
			this.groupBox3.Location = new Point(12, 12);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new Size(295, 100);
			this.groupBox3.TabIndex = 0;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Registers";
			this.btnRead.Location = new Point(151, 68);
			this.btnRead.Name = "btnRead";
			this.btnRead.Size = new Size(65, 23);
			this.btnRead.TabIndex = 2;
			this.btnRead.Text = "Read";
			this.btnRead.UseVisualStyleBackColor = true;
			this.btnRead.Click += new EventHandler(this.btnRead_Click);
			this.tlRegisters.AutoSize = true;
			this.tlRegisters.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tlRegisters.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
			this.tlRegisters.ColumnCount = 2;
			this.tlRegisters.ColumnStyles.Add(new ColumnStyle());
			this.tlRegisters.ColumnStyles.Add(new ColumnStyle());
			this.tlRegisters.Controls.Add((Control)this.lblAddress, 0, 0);
			this.tlRegisters.Controls.Add((Control)this.lblDataWrite, 1, 0);
			this.tlRegisters.Controls.Add((Control)this.tBoxRegAddress, 0, 1);
			this.tlRegisters.Controls.Add((Control)this.tBoxRegValue, 1, 1);
			this.tlRegisters.Location = new Point(75, 19);
			this.tlRegisters.Name = "tlRegisters";
			this.tlRegisters.RowCount = 2;
			this.tlRegisters.RowStyles.Add(new RowStyle());
			this.tlRegisters.RowStyles.Add(new RowStyle());
			this.tlRegisters.Size = new Size(145, 43);
			this.tlRegisters.TabIndex = 0;
			this.lblAddress.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.lblAddress.Location = new Point(4, 1);
			this.lblAddress.Name = "lblAddress";
			this.lblAddress.Size = new Size(65, 20);
			this.lblAddress.TabIndex = 0;
			this.lblAddress.Text = "Address";
			this.lblAddress.TextAlign = ContentAlignment.MiddleCenter;
			this.lblDataWrite.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.lblDataWrite.Location = new Point(76, 1);
			this.lblDataWrite.Name = "lblDataWrite";
			this.lblDataWrite.Size = new Size(65, 20);
			this.lblDataWrite.TabIndex = 1;
			this.lblDataWrite.Text = "Data";
			this.lblDataWrite.TextAlign = ContentAlignment.MiddleCenter;
			this.tBoxRegAddress.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.tBoxRegAddress.Location = new Point(1, 22);
			this.tBoxRegAddress.Margin = new Padding(0);
			this.tBoxRegAddress.MaxLength = 4;
			this.tBoxRegAddress.Name = "tBoxRegAddress";
			this.tBoxRegAddress.Size = new Size(71, 20);
			this.tBoxRegAddress.TabIndex = 2;
			this.tBoxRegAddress.Text = "0x00";
			this.tBoxRegAddress.TextAlign = HorizontalAlignment.Center;
			this.tBoxRegAddress.TextChanged += new EventHandler(this.tBox_TextChanged);
			this.tBoxRegAddress.Enter += new EventHandler(this.txtBox_Enter);
			this.tBoxRegAddress.Validating += new CancelEventHandler(this.txtBox_Validating);
			this.tBoxRegAddress.Validated += new EventHandler(this.txtBox_Validated);
			this.tBoxRegValue.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.tBoxRegValue.Location = new Point(73, 22);
			this.tBoxRegValue.Margin = new Padding(0);
			this.tBoxRegValue.MaxLength = 4;
			this.tBoxRegValue.Name = "tBoxRegValue";
			this.tBoxRegValue.Size = new Size(71, 20);
			this.tBoxRegValue.TabIndex = 3;
			this.tBoxRegValue.Text = "0x00";
			this.tBoxRegValue.TextAlign = HorizontalAlignment.Center;
			this.tBoxRegValue.TextChanged += new EventHandler(this.tBox_TextChanged);
			this.tBoxRegValue.Enter += new EventHandler(this.txtBox_Enter);
			this.tBoxRegValue.Validating += new CancelEventHandler(this.txtBox_Validating);
			this.tBoxRegValue.Validated += new EventHandler(this.txtBox_Validated);
			this.btnWrite.Location = new Point(79, 68);
			this.btnWrite.Name = "btnWrite";
			this.btnWrite.Size = new Size(65, 23);
			this.btnWrite.TabIndex = 1;
			this.btnWrite.Text = "Write";
			this.btnWrite.UseVisualStyleBackColor = true;
			this.btnWrite.Click += new EventHandler(this.btnWrite_Click);
			this.AutoScaleDimensions = new SizeF(6f, 13f);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.ClientSize = new Size(319, 320);
			this.Controls.Add((Control)this.groupBox4);
			this.Controls.Add((Control)this.groupBox3);
			this.Controls.Add((Control)this.ssStatus);
			this.DoubleBuffered = true;
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
			this.Icon = (Icon)resources.GetObject("$this.Icon");
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MaximumSize = new Size(1200, 1200);
			this.Name = "TestForm";
			this.StartPosition = FormStartPosition.Manual;
			this.Text = "Test";
			this.Activated += new EventHandler(this.TestForm_Activated);
			this.FormClosing += new FormClosingEventHandler(this.TestForm_FormClosing);
			this.Load += new EventHandler(this.TestForm_Load);
			this.KeyDown += new KeyEventHandler(this.TestForm_KeyDown);
			this.ssStatus.ResumeLayout(false);
			this.ssStatus.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			((ISupportInitialize)this.pBoxRfOut4).EndInit();
			((ISupportInitialize)this.pBoxRfOut3).EndInit();
			((ISupportInitialize)this.pBoxRfOut2).EndInit();
			((ISupportInitialize)this.pBoxRfOut1).EndInit();
			this.pnlRfPaSwitchSel.ResumeLayout(false);
			this.pnlRfPaSwitchSel.PerformLayout();
			this.pnlRfPaSwitchEnable.ResumeLayout(false);
			this.pnlRfPaSwitchEnable.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.tlRegisters.ResumeLayout(false);
			this.tlRegisters.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
	}
}
using SemtechLib.Controls;
using SemtechLib.Devices.Common.Events;
using SemtechLib.Devices.Common.Interfaces;
using SemtechLib.Devices.SX1276;
using SemtechLib.Devices.SX1276.Common.UI.Forms;
using SemtechLib.Devices.SX1276.UI.Forms;
using SemtechLib.Devices.SX1276LR;
using SemtechLib.General;
using SemtechLib.General.Interfaces;
using SemtechLib.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace SX1276SKA
{
	public class MainForm : Form
	{
		private delegate void DeviceDataChangedDelegate(object sender, PropertyChangedEventArgs e);
		private delegate void DevicePacketHandlerStartedDelegate(object sender, EventArgs e);
		private delegate void DevicePacketHandlerStopedDelegate(object sender, EventArgs e);
		private delegate void DevicePacketHandlerTransmittedDelegate(object sender, PacketStatusEventArg e);
		private delegate void ConnectedDelegate();
		private delegate void DisconnectedDelegate();
		private delegate void ErrorDelegate(byte status, string message);

		#region Private variables
		private List<IDevice> deviceList = new List<IDevice>()
		{
			(IDevice) new SX1276(),
			(IDevice) new SX1276LR()
		};
		private List<IDeviceView> deviceViewList = new List<IDeviceView>()
		{
			(IDeviceView) new SemtechLib.Devices.SX1276.UI.Controls.DeviceViewControl(),
			(IDeviceView) new SemtechLib.Devices.SX1276LR.UI.Controls.DeviceViewControl()
		};

		private string fskConfigFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		private string fskConfigFileName = "sx1276ska-Fsk.cfg";
		private string loRaConfigFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		private string loRaConfigFileName = "sx1276ska-LoRa.cfg";
		private bool IsLoRaOn = true;
		private const string RleaseCandidate = "";
		private const string ApplicationVersion = "";

		private IContainer components;
		private MenuStripEx msMainMenu;
		private ToolStripMenuItem fileToolStripMenuItem;
		private ToolStripMenuItem exitToolStripMenuItem;
		private ToolStripMenuItem helpToolStripMenuItem;
		private ToolStripMenuItem aboutToolStripMenuItem;
		private ToolStripMenuItem connectToolStripMenuItem;
		private ToolStripSeparator mFileSeparator1;
		private ToolStripMenuItem btnOpenConfig;
		private ToolStripMenuItem btnSaveConfig;
		private ToolStripSeparator mFileSeparator2;
		private ToolStripMenuItem btnSaveAsConfig;
		private OpenFileDialog ofConfigFileOpenDlg;
		private SaveFileDialog sfConfigFileSaveDlg;
		private ToolStripStatusLabel tsLblConfigFileName;
		private ToolStripStatusLabel tsLblSeparator2;
		private ToolStripStatusLabel tsLblSeparator3;
		private ToolStripStatusLabel tsLblSeparator4;
		private StatusStrip ssMainStatus;
		private StatusStrip ssMainStatus1;
		private ToolStripMenuItem usersGuideToolStripMenuItem;
		private ToolStripSeparator mHelpSeparator2;
		private ToolStripStatusLabel tsLblStatus;
		private ToolStripSeparator toolStripSeparator3;
		private ToolTip tipMainForm;
		private ToolStripButton tsBtnRefresh;
		private ToolStripMenuItem actionToolStripMenuItem;
		private ToolStripMenuItem refreshToolStripMenuItem;
		private ToolStripContainer toolStripContainer1;
		private ToolStripEx tsActionToolbar;
		private ToolStripLabel toolStripLabel1;
		private ToolStripMenuItem showHelpToolStripMenuItem;
		private ToolStripSeparator mHelpSeparator1;
		private IDeviceView deviceViewControl;
		private ToolStripMenuItem showRegistersToolStripMenuItem;
		private ToolStripButton tsBtnShowRegisters;
		private ToolStripMenuItem resetToolStripMenuItem;
		private ToolStripButton tsBtnReset;
		private ToolStripStatusLabel tsLblSeparator1;
		private ToolStripStatusLabel tsChipVersion;
		private ToolStripMenuItem toolsToolStripMenuItem;
		private ToolStripStatusLabel tsLblChipVersion;
		private ToolStripMenuItem rssiAnalyserToolStripMenuItem;
		private ToolStripMenuItem spectrumAnalyserToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripSeparator toolStripSeparator2;
		private ToolStripMenuItem monitorToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator4;
		private ToolStripLabel tsLblMonitor;
		private ToolStripButton tsBtnMonitorOn;
		private ToolStripButton tsBtnMonitorOff;
		private ToolStripMenuItem monitorOffToolStripMenuItem;
		private ToolStripMenuItem monitorOnToolStripMenuItem;
		private ToolStripMenuItem startuptimeToolStripMenuItem;
		private ToolStripButton tsBtnStartupTime;
		private ToolStripStatusLabel tsLblVersion;
		private ToolStripStatusLabel tsVersion;
		private ToolStripStatusLabel tsLblFwVersion;
		private ToolStripStatusLabel tsFwVersion;
		private ToolStripStatusLabel tsLblConnectionStatus;
		private ToolStripLed ledStatus;
		private ToolStripButton tsBtnFwUpdate;
		private ToolStripButton tsBtnOpenFile;
		private ToolStripButton tsBtnSaveFile;
		private ToolStripSeparator tbFileSeparator1;
		private ToolStripButton btnOpenDevice;
		private ToolStripSeparator toolStripSeparator7;
		private ToolStripLabel tsLblModem;
		private ToolStripButton tsModemLoRa;
		private ToolStripButton tsModemFSK;
		private ToolStripSeparator toolStripSeparator5;
		private ToolStripSeparator toolStripSeparator6;
		private ToolStripButton tsBtnShowHelp;
		private ToolStripMenuItem modemToolStripMenuItem;
		private ToolStripMenuItem miModemFSK;
		private ToolStripMenuItem miModemLoRa;
		private ToolStripSeparator tsSeparatorPerModeOn;
		private ToolStripLabel tsLblPerModeOn;
		private ToolStripSeparator tsSeparatorDebugOn;
		private ToolStripLabel tsLblDebugOn;
		private ApplicationSettings appSettings;
		private TestForm frmTest;
		private HelpForm frmHelp;
		private RegistersForm frmRegisters;
		private RssiAnalyserForm frmRssiAnalyser;
		private SpectrumAnalyserForm frmSpectrumAnalyser;
		private Form frmPacketLog;
		private RxTxStartupTimeForm frmStartupTime;
		private IDevice device;
		private FileStream configFileStream;
		private bool isFskConfigFileOpen;
		private bool isLoRaConfigFileOpen;
		private bool IsLoRaPacketUsePerOn;
		private bool IsDebugOn;
		private bool appTestArg;
		#endregion

		public MainForm(bool testMode)
		{
			appTestArg = testMode;

			InitializeComponent();

			deviceList[0].SetNotificationWindowHandle(Handle, false);
			deviceList[0].Test = AppTestArg;
			deviceList[0].Error += new SemtechLib.General.Events.ErrorEventHandler(device_Error);
			deviceList[0].Connected += new EventHandler(device_Connected);
			deviceList[0].Disconected += new EventHandler(device_Disconected);
			deviceList[0].PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(device_PropertyChanged);
			deviceList[0].PacketHandlerStarted += new EventHandler(device_PacketHandlerStarted);
			deviceList[0].PacketHandlerStoped += new EventHandler(device_PacketHandlerStoped);

			deviceList[1].SetNotificationWindowHandle(Handle, false);
			deviceList[1].Test = AppTestArg;
			deviceList[1].Error += new SemtechLib.General.Events.ErrorEventHandler(device_Error);
			deviceList[1].Connected += new EventHandler(device_Connected);
			deviceList[1].Disconected += new EventHandler(device_Disconected);
			deviceList[1].PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(device_PropertyChanged);
			deviceList[1].PacketHandlerStarted += new EventHandler(device_PacketHandlerStarted);
			deviceList[1].PacketHandlerStoped += new EventHandler(device_PacketHandlerStoped);

			toolStripContainer1.BottomToolStripPanel.SuspendLayout();
			toolStripContainer1.TopToolStripPanel.SuspendLayout();
			toolStripContainer1.SuspendLayout();

			deviceViewList[0].Name = "sx1276ViewControl";
			((Control)deviceViewList[0]).Location = new Point(0, 0);
			((Control)deviceViewList[0]).Size = new Size(1008, 525);
			deviceViewList[0].Device = deviceList[0];
			deviceViewList[0].Dock = DockStyle.Fill;
			deviceViewList[0].Enabled = false;
			deviceViewList[0].Visible = true;
			deviceViewList[0].TabIndex = 0;
			deviceViewList[0].Error += new SemtechLib.General.Events.ErrorEventHandler(deviceViewControl_Error);
			deviceViewList[0].DocumentationChanged += new DocumentationChangedEventHandler(deviceViewControl_DocumentationChanged);

			deviceViewList[1].Name = "sx1276LoRaViewControl";
			((Control)deviceViewList[1]).Location = new Point(0, 0);
			((Control)deviceViewList[1]).Size = new Size(1008, 525);
			deviceViewList[1].Device = deviceList[1];
			deviceViewList[1].Dock = DockStyle.Fill;
			deviceViewList[1].Enabled = false;
			deviceViewList[1].Visible = false;
			deviceViewList[1].TabIndex = 0;
			deviceViewList[1].Error += new SemtechLib.General.Events.ErrorEventHandler(deviceViewControl_Error);
			deviceViewList[1].DocumentationChanged += new DocumentationChangedEventHandler(deviceViewControl_DocumentationChanged);

			toolStripContainer1.ContentPanel.Controls.Add((Control)deviceViewList[0]);
			toolStripContainer1.ContentPanel.Controls.Add((Control)deviceViewList[1]);

			deviceViewControl = deviceViewList[0];

			toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
			toolStripContainer1.BottomToolStripPanel.PerformLayout();
			toolStripContainer1.TopToolStripPanel.ResumeLayout(false);

			toolStripContainer1.TopToolStripPanel.PerformLayout();

			toolStripContainer1.ResumeLayout(false);

			toolStripContainer1.PerformLayout();

			try
			{
				appSettings = new ApplicationSettings();
			}
			catch (Exception ex)
			{
				SetError(ex.Message);
			}

			if (!appTestArg)
				Text = AssemblyTitle ?? "";
			else
				Text = AssemblyTitle + " - ..::: TEST :::..";
		}

		public bool AppTestArg
		{
			get
			{
				return appTestArg;
			}
		}

		public string AssemblyTitle
		{
			get
			{
				object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
				if (customAttributes.Length > 0)
				{
					AssemblyTitleAttribute assemblyTitleAttribute = (AssemblyTitleAttribute)customAttributes[0];
					if (assemblyTitleAttribute.Title != "")
						return assemblyTitleAttribute.Title;
				}
				return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
			}
		}

		public string AssemblyVersion
		{
			get
			{
				AssemblyName name = Assembly.GetExecutingAssembly().GetName();
				if (name.Version.ToString() != "")
					return name.Version.ToString();
				return "-.-.-.-";
			}
		}

		protected override void Dispose(bool disposing)
		{
			appSettings.Dispose();
			deviceViewControl.Dispose();
			if (device != null)
				device.Dispose();
			if (disposing && components != null)
				components.Dispose();
			base.Dispose(disposing);
		}

		public void DisableControls()
		{
			if (frmRegisters != null)
				frmRegisters.RegistersFormEnabled = false;
			tsBtnOpenFile.Enabled = false;
			tsBtnSaveFile.Enabled = false;
			btnOpenConfig.Enabled = false;
			btnSaveConfig.Enabled = false;
			btnSaveAsConfig.Enabled = false;
			tsBtnRefresh.Enabled = false;
			tsLblMonitor.Enabled = false;
			tsBtnMonitorOff.Enabled = false;
			tsBtnMonitorOn.Enabled = false;
			refreshToolStripMenuItem.Enabled = false;
			monitorToolStripMenuItem.Enabled = false;
		}

		public void EnableControls()
		{
			if (frmRegisters != null)
				frmRegisters.RegistersFormEnabled = true;
			tsBtnOpenFile.Enabled = true;
			tsBtnSaveFile.Enabled = true;
			btnOpenConfig.Enabled = true;
			btnSaveConfig.Enabled = true;
			btnSaveAsConfig.Enabled = true;
			tsBtnRefresh.Enabled = true;
			tsLblMonitor.Enabled = true;
			tsBtnMonitorOff.Enabled = true;
			tsBtnMonitorOn.Enabled = true;
			refreshToolStripMenuItem.Enabled = true;
			monitorToolStripMenuItem.Enabled = true;
		}

		private void ClearError()
		{
			tsLblStatus.Text = string.Empty;
			Refresh();
		}
		private void SetError(string message)
		{
			tsLblStatus.Text = "ERROR: " + message;
			Refresh();
		}

		private void OnError(byte status, string message)
		{
			if (status != 0)
				tsLblStatus.Text = "ERROR: " + message;
			else
				tsLblStatus.Text = message;
			Refresh();
		}

		private void OnConnected()
		{
			try
			{
				ClearError();
				ledStatus.Checked = device.IsOpen;
				btnOpenDevice.Text = "Disconnect";
				btnOpenDevice.Image = (Image)Resources.Connected;
				connectToolStripMenuItem.Text = "Disconnect";
				connectToolStripMenuItem.Image = (Image)Resources.Connected;
				device.Reset();
				tsBtnOpenFile.Enabled = true;
				tsBtnSaveFile.Enabled = true;
				btnOpenConfig.Enabled = true;
				btnSaveConfig.Enabled = true;
				btnSaveAsConfig.Enabled = true;
				tsLblModem.Enabled = true;
				tsModemLoRa.Enabled = true;
				tsModemFSK.Enabled = true;
				tsBtnReset.Enabled = true;
				tsBtnRefresh.Enabled = true;
				tsBtnShowRegisters.Enabled = true;
				tsLblMonitor.Enabled = true;
				tsBtnMonitorOn.Enabled = true;
				tsBtnMonitorOff.Enabled = true;
				tsBtnStartupTime.Enabled = true;
				modemToolStripMenuItem.Enabled = true;
				resetToolStripMenuItem.Enabled = true;
				refreshToolStripMenuItem.Enabled = true;
				showRegistersToolStripMenuItem.Enabled = true;
				monitorToolStripMenuItem.Enabled = true;
				startuptimeToolStripMenuItem.Enabled = true;
				rssiAnalyserToolStripMenuItem.Enabled = true;
				spectrumAnalyserToolStripMenuItem.Enabled = true;
				deviceViewControl.Enabled = true;
				if (frmTest != null)
					frmTest.Device = device;
				if (frmRegisters != null)
					frmRegisters.Device = device;
				tsFwVersion.Text = device.FwVersion.Build == 0 ? device.FwVersion.ToString() : string.Format("{0}.{1}.B{2}", (object)device.FwVersion.Major.ToString(), (object)device.FwVersion.Minor.ToString(), (object)device.FwVersion.Build.ToString());
				tsChipVersion.Text = device.Version.ToString();
			}
			catch (Exception ex)
			{
				SetError(ex.Message);
			}
		}

		private void OnDisconnected()
		{
			try
			{
				ClearError();
				ledStatus.Checked = device.IsOpen;
				btnOpenDevice.Text = "Connect";
				btnOpenDevice.Image = (Image)Resources.Disconnected;
				connectToolStripMenuItem.Text = "Connect";
				connectToolStripMenuItem.Image = (Image)Resources.Disconnected;
				tsBtnOpenFile.Enabled = false;
				tsBtnSaveFile.Enabled = false;
				btnOpenConfig.Enabled = false;
				btnSaveConfig.Enabled = false;
				btnSaveAsConfig.Enabled = false;
				tsLblModem.Enabled = false;
				tsModemLoRa.Enabled = false;
				tsModemFSK.Enabled = false;
				tsBtnReset.Enabled = false;
				tsBtnRefresh.Enabled = false;
				tsBtnShowRegisters.Enabled = false;
				tsLblMonitor.Enabled = false;
				tsBtnMonitorOn.Enabled = false;
				tsBtnMonitorOff.Enabled = false;
				tsBtnStartupTime.Enabled = false;
				modemToolStripMenuItem.Enabled = false;
				resetToolStripMenuItem.Enabled = false;
				refreshToolStripMenuItem.Enabled = false;
				showRegistersToolStripMenuItem.Enabled = false;
				monitorToolStripMenuItem.Enabled = false;
				startuptimeToolStripMenuItem.Enabled = false;
				rssiAnalyserToolStripMenuItem.Enabled = false;
				spectrumAnalyserToolStripMenuItem.Enabled = false;
				showHelpToolStripMenuItem.Enabled = false;
				tsBtnShowHelp.Enabled = false;
				deviceViewControl.Enabled = false;
				if (frmTest != null)
					frmTest.Close();
				if (frmRegisters != null)
					frmRegisters.Close();
				if (frmRssiAnalyser != null)
					frmRssiAnalyser.Close();
				if (frmSpectrumAnalyser != null)
					frmSpectrumAnalyser.Close();
				if (frmStartupTime == null)
					return;
				frmStartupTime.Close();
			}
			catch (Exception ex)
			{
				SetError(ex.Message);
			}
		}

		protected override void WndProc(ref Message m)
		{
			if (device != null)
				device.ProcessWinMessage(m.Msg, m.WParam, m.LParam);
			base.WndProc(ref m);
		}

		private bool IsFormLocatedInScreen(Screen[] screens)
		{
			int upperBound = screens.GetUpperBound(0);
			bool flag = false;
			for (int index = 0; index <= upperBound; ++index)
			{
				if (Left < screens[index].WorkingArea.Left || Top < screens[index].WorkingArea.Top || (Left > screens[index].WorkingArea.Right || Top > screens[index].WorkingArea.Bottom))
				{
					flag = false;
				}
				else
				{
					flag = true;
					break;
				}
			}
			return flag;
		}

		private void InitializeDevice(bool isLoRaOn)
		{
			IsLoRaOn = isLoRaOn;

			if (device != null)
				btnOpenDevice_Click(btnOpenDevice, EventArgs.Empty);

			if (isLoRaOn)
			{
				startuptimeToolStripMenuItem.Visible = false;
				tsBtnStartupTime.Visible = false;
				toolsToolStripMenuItem.Visible = false;
				deviceViewList[1].Visible = true;
				deviceViewList[0].Visible = false;
				device = deviceList[1];
				deviceViewControl = deviceViewList[1];
			}
			else
			{
				startuptimeToolStripMenuItem.Visible = true;
				tsBtnStartupTime.Visible = true;
				toolsToolStripMenuItem.Visible = true;
				deviceViewList[0].Visible = true;
				deviceViewList[1].Visible = false;
				device = deviceList[0];
				deviceViewControl = deviceViewList[0];
			}

			btnOpenDevice_Click(btnOpenDevice, EventArgs.Empty);
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			try
			{
				tsVersion.Text = string.Format(
					"{0}.{1}.{2}",
					Assembly.GetExecutingAssembly().GetName().Version.Major,
					Assembly.GetExecutingAssembly().GetName().Version.Minor,
					Assembly.GetExecutingAssembly().GetName().Version.Build
					);
				string value = appSettings.GetValue("Top");
				if (!string.IsNullOrEmpty(value))
				{
					try
					{
						Top = int.Parse(value);
					}
					catch
					{
						MessageBox.Show(this, "Error getting Top value.");
					}
				}

				value = appSettings.GetValue("Left");
				if (!string.IsNullOrEmpty(value))
				{
					try
					{
						Left = int.Parse(value);
					}
					catch
					{
						MessageBox.Show(this, "Error getting Left value.");
					}
				}
				Screen[] allScreens = Screen.AllScreens;
				if (!IsFormLocatedInScreen(allScreens))
				{
					Top = allScreens[0].WorkingArea.Top;
					Left = allScreens[0].WorkingArea.Left;
				}

				value = appSettings.GetValue("FskConfigFilePath");
				if (string.IsNullOrEmpty(value))
				{
					value = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
					appSettings.SetValue("FskConfigFilePath", value);
				}
				fskConfigFilePath = value;

				value = appSettings.GetValue("FskConfigFileName");
				if (string.IsNullOrEmpty(value))
				{
					value = "sx1276ska-Fsk.cfg";
					appSettings.SetValue("FskConfigFileName", value);
				}
				fskConfigFileName = value;

				value = appSettings.GetValue("LoRaConfigFilePath");
				if (string.IsNullOrEmpty(value))
				{
					value = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
					appSettings.SetValue("LoRaConfigFilePath", value);
				}
				loRaConfigFilePath = value;

				value = appSettings.GetValue("LoRaConfigFileName");
				if (string.IsNullOrEmpty(value))
				{
					value = "sx1276ska-LoRa.cfg";
					appSettings.SetValue("LoRaConfigFileName", value);
				}
				loRaConfigFileName = value;

				value = appSettings.GetValue("IsLoRaOn");
				if (!string.IsNullOrEmpty(value))
				{
					try
					{
						IsLoRaOn = bool.Parse(value);
					}
					catch
					{
						MessageBox.Show(this, "Error getting IsLoRaOn value.");
					}
				}

				value = appSettings.GetValue("IsLoRaPacketUsePerOn");
				if (!string.IsNullOrEmpty(value))
				{
					try
					{
						IsLoRaPacketUsePerOn = bool.Parse(value);
					}
					catch
					{
						MessageBox.Show(this, "Error getting IsLoRaPacketUsePerOn value.");
					}
				}

				value = appSettings.GetValue("IsDebugOn");
				if (!string.IsNullOrEmpty(value))
				{
					try
					{
						IsDebugOn = bool.Parse(value);
					}
					catch
					{
						MessageBox.Show(this, "Error getting IsDebugOn value.");
					}
				}

				tsLblConfigFileName.Text = "Config File: -";
				isFskConfigFileOpen = false;
				isLoRaConfigFileOpen = false;
				if (IsLoRaOn)
				{
					miModemFSK.Checked = false;
					tsModemFSK.Checked = false;
					miModemLoRa.Checked = true;
					tsModemLoRa.Checked = true;
					((SemtechLib.Devices.SX1276LR.UI.Controls.DeviceViewControl)deviceViewList[1]).AppSettings = appSettings;
				}
				else
				{
					miModemFSK.Checked = true;
					tsModemFSK.Checked = true;
					miModemLoRa.Checked = false;
					tsModemLoRa.Checked = false;
				}

				InitializeDevice(IsLoRaOn);
				device.IsDebugOn = IsDebugOn;
				tsLblDebugOn.Visible = IsDebugOn;
				tsSeparatorDebugOn.Visible = IsDebugOn;

				if (IsLoRaOn)
				{
					value = appSettings.GetValue("PacketLogPath");
					if (string.IsNullOrEmpty(value))
					{
						value = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
						appSettings.SetValue("PacketLogPath", value);
					}
					((SX1276LR)device).PacketHandlerLog.Path = value;

					value = appSettings.GetValue("PacketLogFileName");
					if (string.IsNullOrEmpty(value))
					{
						value = "sx1276-LoRa-pkt.log";
						appSettings.SetValue("PacketLogFileName", value);
					}
					((SX1276LR)device).PacketHandlerLog.FileName = value;

					value = appSettings.GetValue("PacketLogMaxSamples");
					if (string.IsNullOrEmpty(value))
					{
						value = "0";
						appSettings.SetValue("PacketLogMaxSamples", value);
					}
					((SX1276LR)device).PacketHandlerLog.MaxSamples = ulong.Parse(value);

					value = appSettings.GetValue("PacketLogIsAppend");
					if (string.IsNullOrEmpty(value))
					{
						value = true.ToString();
						appSettings.SetValue("PacketLogIsAppend", value);
					}
					((SX1276LR)device).PacketHandlerLog.IsAppend = bool.Parse(value);

					value = appSettings.GetValue("PacketLogEnabled");
					if (string.IsNullOrEmpty(value))
					{
						value = false.ToString();
						appSettings.SetValue("PacketLogEnabled", value);
					}
					((SX1276LR)device).PacketHandlerLog.Enabled = bool.Parse(value);

					((SX1276LR)device).PacketUsePer = IsLoRaPacketUsePerOn;
					tsLblPerModeOn.Visible = IsLoRaPacketUsePerOn;
					tsSeparatorPerModeOn.Visible = IsLoRaPacketUsePerOn;
				}
				else
				{
					tsLblPerModeOn.Visible = false;
					tsSeparatorPerModeOn.Visible = false;
				}
			}
			catch (Exception ex)
			{
				SetError(ex.Message);
			}
		}

		private void Mainform_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (device == null)
				return;
			device.Close();
		}

		private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			try
			{
				appSettings.SetValue("Top", Top.ToString());
				appSettings.SetValue("Left", Left.ToString());
				appSettings.SetValue("FskConfigFilePath", fskConfigFilePath);
				appSettings.SetValue("FskConfigFileName", fskConfigFileName);
				appSettings.SetValue("LoRaConfigFilePath", loRaConfigFilePath);
				appSettings.SetValue("LoRaConfigFileName", loRaConfigFileName);
				appSettings.SetValue("IsLoRaOn", IsLoRaOn.ToString());
				appSettings.SetValue("IsLoRaPacketUsePerOn", IsLoRaPacketUsePerOn.ToString());
				appSettings.SetValue("IsDebugOn", IsDebugOn.ToString());
			}
			catch (Exception ex)
			{
				SetError(ex.Message);
				Refresh();
			}
		}

		private void Mainform_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
			{
				e.Handled = true;
				SendKeys.Send("{TAB}");
			}
			else if (e.KeyData == (Keys.N | Keys.Control | Keys.Alt))
			{
				if (!(btnOpenDevice.Text == "Connect"))
					return;
				deviceViewControl.Enabled = !deviceViewControl.Enabled;
				if (deviceViewControl.Enabled)
				{
					device.ReadRegisters();
					tsBtnOpenFile.Enabled = true;
					tsBtnSaveFile.Enabled = true;
					btnOpenConfig.Enabled = true;
					btnSaveConfig.Enabled = true;
					btnSaveAsConfig.Enabled = true;
					tsLblModem.Enabled = true;
					tsModemLoRa.Enabled = true;
					tsModemFSK.Enabled = true;
					tsBtnReset.Enabled = true;
					tsBtnRefresh.Enabled = true;
					tsBtnShowRegisters.Enabled = true;
					tsLblMonitor.Enabled = false;
					tsBtnMonitorOff.Enabled = false;
					tsBtnMonitorOn.Enabled = false;
					tsBtnStartupTime.Enabled = true;
					modemToolStripMenuItem.Enabled = true;
					resetToolStripMenuItem.Enabled = true;
					refreshToolStripMenuItem.Enabled = true;
					showRegistersToolStripMenuItem.Enabled = true;
					monitorToolStripMenuItem.Enabled = false;
					startuptimeToolStripMenuItem.Enabled = true;
				}
				else
				{
					tsBtnOpenFile.Enabled = false;
					tsBtnSaveFile.Enabled = false;
					btnOpenConfig.Enabled = false;
					btnSaveConfig.Enabled = false;
					btnSaveAsConfig.Enabled = false;
					tsLblModem.Enabled = false;
					tsModemLoRa.Enabled = false;
					tsModemFSK.Enabled = false;
					tsBtnReset.Enabled = false;
					tsBtnRefresh.Enabled = false;
					tsBtnShowRegisters.Enabled = false;
					tsLblMonitor.Enabled = false;
					tsBtnMonitorOff.Enabled = false;
					tsBtnMonitorOn.Enabled = false;
					tsBtnStartupTime.Enabled = false;
					modemToolStripMenuItem.Enabled = false;
					resetToolStripMenuItem.Enabled = false;
					refreshToolStripMenuItem.Enabled = false;
					showRegistersToolStripMenuItem.Enabled = false;
					monitorToolStripMenuItem.Enabled = false;
					startuptimeToolStripMenuItem.Enabled = false;
				}
			}
			else if (e.KeyData == (Keys.T | Keys.Control | Keys.Alt))
			{
				if (frmTest == null)
				{
					frmTest = new TestForm();
					frmTest.FormClosed += new FormClosedEventHandler(frmTest_FormClosed);
					frmTest.Disposed += new EventHandler(frmTest_Disposed);
					frmTest.Device = device;
					frmTest.TestEnabled = false;
				}
				if (!frmTest.TestEnabled)
				{
					frmTest.TestEnabled = true;
					frmTest.Location = new Point()
					{
						X = Location.X + Width / 2 - frmTest.Width / 2,
						Y = Location.Y + Height / 2 - frmTest.Height / 2
					};
					frmTest.Show();
				}
				else
				{
					frmTest.TestEnabled = false;
					frmTest.Hide();
				}
			}
			else if (e.KeyData == (Keys.P | Keys.Control | Keys.Alt))
			{
				if (!IsLoRaOn)
					return;
				((SX1276LR)device).PacketUsePer = !((SX1276LR)device).PacketUsePer;
				tsLblPerModeOn.Visible = ((SX1276LR)device).PacketUsePer;
				tsSeparatorPerModeOn.Visible = ((SX1276LR)device).PacketUsePer;
				IsLoRaPacketUsePerOn = ((SX1276LR)device).PacketUsePer;
				appSettings.SetValue("IsLoRaPacketUsePerOn", IsLoRaPacketUsePerOn.ToString());
			}
			else
			{
				if (e.KeyData != (Keys.D | Keys.Control | Keys.Alt))
					return;
				device.IsDebugOn = !device.IsDebugOn;
				tsLblDebugOn.Visible = device.IsDebugOn;
				tsSeparatorDebugOn.Visible = device.IsDebugOn;
				IsDebugOn = device.IsDebugOn;
				appSettings.SetValue("IsDebugOn", IsDebugOn.ToString());
			}
		}

		private void frmRssiAnalyser_FormClosed(object sender, FormClosedEventArgs e)
		{
			rssiAnalyserToolStripMenuItem.Checked = false;
		}

		private void frmRssiAnalyser_Disposed(object sender, EventArgs e)
		{
			frmRssiAnalyser = (RssiAnalyserForm)null;
		}

		private void frmSpectrumAnalyser_FormClosed(object sender, FormClosedEventArgs e)
		{
			spectrumAnalyserToolStripMenuItem.Checked = false;
		}

		private void frmSpectrumAnalyser_Disposed(object sender, EventArgs e)
		{
			frmSpectrumAnalyser = (SpectrumAnalyserForm)null;
		}

		private void frmTest_FormClosed(object sender, FormClosedEventArgs e)
		{
		}

		private void frmTest_Disposed(object sender, EventArgs e)
		{
			frmTest = (TestForm)null;
		}

		private void frmHelp_FormClosed(object sender, FormClosedEventArgs e)
		{
			tsBtnShowHelp.Checked = false;
			showHelpToolStripMenuItem.Checked = false;
		}

		private void frmHelp_Disposed(object sender, EventArgs e)
		{
			frmHelp = (HelpForm)null;
		}

		private void frmRegisters_FormClosed(object sender, FormClosedEventArgs e)
		{
			tsBtnShowRegisters.Checked = false;
			showRegistersToolStripMenuItem.Checked = false;
		}

		private void frmRegisters_Disposed(object sender, EventArgs e)
		{
			frmRegisters = (RegistersForm)null;
		}

		private void frmPacketLog_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (!(device.GetType() == typeof(SX1276)))
				return;
			device.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(device_PropertyChanged);
			((SX1276)device).Packet.LogEnabled = false;
			device.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(device_PropertyChanged);
		}

		private void frmPacketLog_Disposed(object sender, EventArgs e)
		{
			frmPacketLog = (Form)null;
		}

		private void frmStartupTime_FormClosed(object sender, FormClosedEventArgs e)
		{
			tsBtnStartupTime.Checked = false;
			startuptimeToolStripMenuItem.Checked = false;
		}

		private void frmStartupTime_Disposed(object sender, EventArgs e)
		{
			frmStartupTime = (RxTxStartupTimeForm)null;
		}

		private void deviceViewControl_DocumentationChanged(object sender, DocumentationChangedEventArgs e)
		{
			if (frmHelp == null)
				return;
			frmHelp.UpdateDocument(e);
		}

		private void deviceViewControl_Error(object sender, SemtechLib.General.Events.ErrorEventArgs e)
		{
			if (InvokeRequired)
				BeginInvoke((Delegate)new MainForm.ErrorDelegate(OnError), e.Status, e.Message);
			else
				OnError(e.Status, e.Message);
		}

		private void device_Error(object sender, SemtechLib.General.Events.ErrorEventArgs e)
		{
			if (InvokeRequired)
				BeginInvoke((Delegate)new MainForm.ErrorDelegate(OnError), e.Status, e.Message);
			else
				OnError(e.Status, e.Message);
		}

		private void device_Connected(object sender, EventArgs e)
		{
			if (InvokeRequired)
				BeginInvoke((Delegate)new MainForm.ConnectedDelegate(OnConnected), null);
			else
				OnConnected();
		}

		private void device_Disconected(object sender, EventArgs e)
		{
			if (InvokeRequired)
				BeginInvoke((Delegate)new MainForm.DisconnectedDelegate(OnDisconnected), null);
			else
				OnDisconnected();
		}

		private void OnDevicePorpertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "Monitor":
					if (!device.Monitor)
					{
						monitorOffToolStripMenuItem.Checked = true;
						tsBtnMonitorOff.Checked = true;
						monitorOnToolStripMenuItem.Checked = false;
						tsBtnMonitorOn.Checked = false;
					}
					else
					{
						monitorOffToolStripMenuItem.Checked = false;
						tsBtnMonitorOff.Checked = false;
						monitorOnToolStripMenuItem.Checked = true;
						tsBtnMonitorOn.Checked = true;
					}
					break;
				case "SpectrumOn":
					if (device.GetType() == typeof(SX1276))
					{
						if (((SX1276)device).SpectrumOn)
							DisableControls();
						else
							EnableControls();
					}
					break;
				case "LogEnabled":
					if (device.GetType() == typeof(SX1276))
					{
						if (((SX1276)device).Packet.LogEnabled)
						{
							if (frmPacketLog != null)
								frmPacketLog.Close();
							if (frmPacketLog == null)
							{
								frmPacketLog = (Form)new PacketLogForm();
								frmPacketLog.FormClosed += new FormClosedEventHandler(frmPacketLog_FormClosed);
								frmPacketLog.Disposed += new EventHandler(frmPacketLog_Disposed);
								((PacketLogForm)frmPacketLog).Device = device;
								((PacketLogForm)frmPacketLog).AppSettings = appSettings;
							}
							frmPacketLog.Show();
						}
						else
						{
							if (frmPacketLog != null)
								frmPacketLog.Close();
						}
					}
					break;
			}
		}

		private void OnDevicePacketHandlerStarted(object sender, EventArgs e)
		{
			DisableControls();
		}

		private void OnDevicePacketHandlerStoped(object sender, EventArgs e)
		{
			EnableControls();
		}

		private void device_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (InvokeRequired)
				BeginInvoke((Delegate)new MainForm.DeviceDataChangedDelegate(OnDevicePorpertyChanged), sender, e);
			else
				OnDevicePorpertyChanged(sender, e);
		}

		private void device_PacketHandlerStarted(object sender, EventArgs e)
		{
			if (InvokeRequired)
				BeginInvoke((Delegate)new MainForm.DevicePacketHandlerStartedDelegate(OnDevicePacketHandlerStarted), sender, e);
			else
				OnDevicePacketHandlerStarted(sender, e);
		}

		private void device_PacketHandlerStoped(object sender, EventArgs e)
		{
			if (InvokeRequired)
				BeginInvoke((Delegate)new MainForm.DevicePacketHandlerStopedDelegate(OnDevicePacketHandlerStoped), sender, e);
			else
				OnDevicePacketHandlerStoped(sender, e);
		}

		private void btnOpenDevice_Click(object sender, EventArgs e)
		{
			Cursor = Cursors.WaitCursor;

			btnOpenDevice.Enabled = false;
			connectToolStripMenuItem.Enabled = false;

			ClearError();

			try
			{
				if (btnOpenDevice.Text == "Connect")
				{
					if (device.Open())
						return;
					SetError("Unable to open " + device.DeviceName + " device");
				}
				else
				{
					if (device == null)
						return;
					device.Close();
				}
			}
			catch (Exception ex)
			{
				btnOpenDevice.Text = "Connect";
				btnOpenDevice.Image = (Image)Resources.Disconnected;
				connectToolStripMenuItem.Text = "Connect";
				connectToolStripMenuItem.Image = (Image)Resources.Disconnected;
				if (device != null)
					device.Close();
				device.ReadRegisters();
				SetError(ex.Message);
				Refresh();
			}
			finally
			{
				btnOpenDevice.Enabled = true;
				connectToolStripMenuItem.Enabled = true;
				Cursor = Cursors.Default;
			}
		}

		private void btnOpenConfig_Click(object sender, EventArgs e)
		{
			ClearError();
			Validate();
			if (!IsLoRaOn)
			{
				try
				{
					ofConfigFileOpenDlg.InitialDirectory = fskConfigFilePath;
					ofConfigFileOpenDlg.FileName = fskConfigFileName;
					if (ofConfigFileOpenDlg.ShowDialog() == DialogResult.OK)
					{
						string[] strArray = ofConfigFileOpenDlg.FileName.Split('\\');
						fskConfigFileName = strArray[strArray.Length - 1];
						fskConfigFilePath = "";
						int index;
						for (index = 0; index < strArray.Length - 2; ++index)
						{
							MainForm mainForm = this;
							string str = mainForm.fskConfigFilePath + strArray[index] + "\\";
							mainForm.fskConfigFilePath = str;
						}
						fskConfigFilePath += strArray[index];
						configFileStream = new FileStream(fskConfigFilePath + "\\" + fskConfigFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
						device.OpenConfig(ref configFileStream);
						isFskConfigFileOpen = true;
						tsLblConfigFileName.Text = "Config File: " + fskConfigFileName;
						btnSaveConfig.Text = "Save Config \"" + fskConfigFileName + "\"";
					}
					else
						isFskConfigFileOpen = false;
				}
				catch (Exception ex)
				{
					isFskConfigFileOpen = false;
					SetError(ex.Message);
				}
			}
			else
			{
				try
				{
					ofConfigFileOpenDlg.InitialDirectory = loRaConfigFilePath;
					ofConfigFileOpenDlg.FileName = loRaConfigFileName;
					if (ofConfigFileOpenDlg.ShowDialog() == DialogResult.OK)
					{
						string[] strArray = ofConfigFileOpenDlg.FileName.Split('\\');
						loRaConfigFileName = strArray[strArray.Length - 1];
						loRaConfigFilePath = "";
						int index;
						for (index = 0; index < strArray.Length - 2; ++index)
						{
							MainForm mainForm = this;
							string str = mainForm.loRaConfigFilePath + strArray[index] + "\\";
							mainForm.loRaConfigFilePath = str;
						}
						loRaConfigFilePath += strArray[index];
						configFileStream = new FileStream(loRaConfigFilePath + "\\" + loRaConfigFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
						device.OpenConfig(ref configFileStream);
						isLoRaConfigFileOpen = true;
						tsLblConfigFileName.Text = "Config File: " + loRaConfigFileName;
						btnSaveConfig.Text = "Save Config \"" + loRaConfigFileName + "\"";
					}
					else
						isLoRaConfigFileOpen = false;
				}
				catch (Exception ex)
				{
					isLoRaConfigFileOpen = false;
					SetError(ex.Message);
				}
			}
		}

		private void btnSaveConfig_Click(object sender, EventArgs e)
		{
			Validate();
			try
			{
				ClearError();
				if (!IsLoRaOn)
				{
					if (isFskConfigFileOpen)
					{
						if (MessageBox.Show("Do you want to overwrite the current config file?", "Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
							return;
						configFileStream = new FileStream(fskConfigFilePath + "\\" + fskConfigFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
						device.SaveConfig(ref configFileStream);
					}
					else
						btnSaveAsConfig_Click(sender, e);
				}
				else if (isLoRaConfigFileOpen)
				{
					if (MessageBox.Show("Do you want to overwrite the current config file?", "Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
						return;
					configFileStream = new FileStream(loRaConfigFilePath + "\\" + loRaConfigFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
					device.SaveConfig(ref configFileStream);
				}
				else
					btnSaveAsConfig_Click(sender, e);
			}
			catch (Exception ex)
			{
				SetError(ex.Message);
			}
		}

		private void btnSaveAsConfig_Click(object sender, EventArgs e)
		{
			Validate();
			if (!IsLoRaOn)
			{
				try
				{
					ClearError();
					sfConfigFileSaveDlg.InitialDirectory = fskConfigFilePath;
					sfConfigFileSaveDlg.FileName = fskConfigFileName;
					if (sfConfigFileSaveDlg.ShowDialog() != DialogResult.OK)
						return;
					string[] strArray = sfConfigFileSaveDlg.FileName.Split('\\');
					fskConfigFileName = strArray[strArray.Length - 1];
					fskConfigFilePath = "";
					int index;
					for (index = 0; index < strArray.Length - 2; ++index)
					{
						MainForm mainForm = this;
						string str = mainForm.fskConfigFilePath + strArray[index] + "\\";
						mainForm.fskConfigFilePath = str;
					}
					fskConfigFilePath += strArray[index];
					configFileStream = new FileStream(fskConfigFilePath + "\\" + fskConfigFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
					tsLblConfigFileName.Text = "Config File: " + fskConfigFileName;
					btnSaveConfig.Text = "Save Config \"" + fskConfigFileName + "\"";
					device.SaveConfig(ref configFileStream);
					isFskConfigFileOpen = true;
				}
				catch (Exception ex)
				{
					SetError(ex.Message);
					isFskConfigFileOpen = false;
				}
			}
			else
			{
				try
				{
					ClearError();
					sfConfigFileSaveDlg.InitialDirectory = loRaConfigFilePath;
					sfConfigFileSaveDlg.FileName = loRaConfigFileName;
					if (sfConfigFileSaveDlg.ShowDialog() != DialogResult.OK)
						return;
					string[] strArray = sfConfigFileSaveDlg.FileName.Split('\\');
					loRaConfigFileName = strArray[strArray.Length - 1];
					loRaConfigFilePath = "";
					int index;
					for (index = 0; index < strArray.Length - 2; ++index)
					{
						MainForm mainForm = this;
						string str = mainForm.loRaConfigFilePath + strArray[index] + "\\";
						mainForm.loRaConfigFilePath = str;
					}
					loRaConfigFilePath += strArray[index];
					configFileStream = new FileStream(loRaConfigFilePath + "\\" + loRaConfigFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
					tsLblConfigFileName.Text = "Config File: " + loRaConfigFileName;
					btnSaveConfig.Text = "Save Config \"" + loRaConfigFileName + "\"";
					device.SaveConfig(ref configFileStream);
					isLoRaConfigFileOpen = true;
				}
				catch (Exception ex)
				{
					SetError(ex.Message);
					isLoRaConfigFileOpen = false;
				}
			}
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void modemToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				ClearError();
				tsLblConfigFileName.Text = "Config File: -";
				if (sender == miModemFSK || sender == tsModemFSK)
				{	// Use FSK
					miModemFSK.Checked = true;
					tsModemFSK.Checked = true;
					miModemLoRa.Checked = false;
					tsModemLoRa.Checked = false;
					tsLblPerModeOn.Visible = false;
					tsSeparatorPerModeOn.Visible = false;
					InitializeDevice(false);
				}
				else
				{	// Use LoRa
					miModemFSK.Checked = false;
					tsModemFSK.Checked = false;
					miModemLoRa.Checked = true;
					tsModemLoRa.Checked = true;
					tsLblPerModeOn.Visible = IsLoRaPacketUsePerOn;
					tsSeparatorPerModeOn.Visible = IsLoRaPacketUsePerOn;
					InitializeDevice(true);
				}
				appSettings.SetValue("IsLoRaOn", IsLoRaOn.ToString());
			}
			catch (Exception ex)
			{
				SetError(ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void resetToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				ClearError();
				device.Reset();
				tsChipVersion.Text = device.Version.ToString();
			}
			catch (Exception ex)
			{
				SetError(ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				ClearError();
				device.ReadRegisters();
				tsChipVersion.Text = device.Version.ToString();
			}
			catch (Exception ex)
			{
				SetError(ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void showRegistersToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (showRegistersToolStripMenuItem.Checked || tsBtnShowRegisters.Checked)
			{
				showRegistersToolStripMenuItem.Checked = false;
				tsBtnShowRegisters.Checked = false;
				if (frmRegisters != null)
					frmRegisters.Hide();
				if (frmSpectrumAnalyser == null && !device.IsPacketHandlerStarted)
					return;
				frmRegisters.RegistersFormEnabled = true;
			}
			else
			{
				showRegistersToolStripMenuItem.Checked = true;
				tsBtnShowRegisters.Checked = true;
				if (frmRegisters == null)
				{
					frmRegisters = new RegistersForm();
					frmRegisters.FormClosed += new FormClosedEventHandler(frmRegisters_FormClosed);
					frmRegisters.Disposed += new EventHandler(frmRegisters_Disposed);
					frmRegisters.Device = device;
					frmRegisters.AppSettings = appSettings;
				}
				if (frmSpectrumAnalyser != null || device.IsPacketHandlerStarted)
					frmRegisters.RegistersFormEnabled = false;
				frmRegisters.Show();
			}
		}

		private void monitorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				ClearError();
				if (sender == monitorOffToolStripMenuItem || sender == tsBtnMonitorOff)
					device.Monitor = false;
				else
					device.Monitor = true;
			}
			catch (Exception ex)
			{
				SetError(ex.Message);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void startuptimeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (startuptimeToolStripMenuItem.Checked || tsBtnStartupTime.Checked)
			{
				startuptimeToolStripMenuItem.Checked = false;
				tsBtnStartupTime.Checked = false;
				if (frmStartupTime == null)
					return;
				frmStartupTime.Hide();
			}
			else
			{
				startuptimeToolStripMenuItem.Checked = true;
				tsBtnStartupTime.Checked = true;
				if (frmStartupTime == null)
				{
					frmStartupTime = new RxTxStartupTimeForm();
					frmStartupTime.FormClosed += new FormClosedEventHandler(frmStartupTime_FormClosed);
					frmStartupTime.Disposed += new EventHandler(frmStartupTime_Disposed);
					frmStartupTime.Device = device;
					frmStartupTime.AppSettings = appSettings;
				}
				frmStartupTime.Show();
			}
		}

		private void rssiAnalyserToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (rssiAnalyserToolStripMenuItem.Checked)
			{
				if (frmRssiAnalyser != null)
					frmRssiAnalyser.Close();
				rssiAnalyserToolStripMenuItem.Checked = false;
			}
			else
			{
				if (frmSpectrumAnalyser != null)
					frmSpectrumAnalyser.Close();
				if (frmRssiAnalyser == null)
				{
					frmRssiAnalyser = new RssiAnalyserForm();
					frmRssiAnalyser.FormClosed += new FormClosedEventHandler(frmRssiAnalyser_FormClosed);
					frmRssiAnalyser.Disposed += new EventHandler(frmRssiAnalyser_Disposed);
					frmRssiAnalyser.Device = device;
					frmRssiAnalyser.AppSettings = appSettings;
				}
				frmRssiAnalyser.Show();
				rssiAnalyserToolStripMenuItem.Checked = true;
			}
		}

		private void spectrumAnalyserToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (spectrumAnalyserToolStripMenuItem.Checked)
			{
				if (frmSpectrumAnalyser != null)
					frmSpectrumAnalyser.Close();
				spectrumAnalyserToolStripMenuItem.Checked = false;
			}
			else
			{
				if (frmRssiAnalyser != null)
					frmRssiAnalyser.Close();
				if (frmSpectrumAnalyser == null)
				{
					frmSpectrumAnalyser = new SpectrumAnalyserForm();
					frmSpectrumAnalyser.FormClosed += new FormClosedEventHandler(frmSpectrumAnalyser_FormClosed);
					frmSpectrumAnalyser.Disposed += new EventHandler(frmSpectrumAnalyser_Disposed);
					frmSpectrumAnalyser.Device = device;
					frmSpectrumAnalyser.AppSettings = appSettings;
				}
				frmSpectrumAnalyser.Show();
				spectrumAnalyserToolStripMenuItem.Checked = true;
			}
		}

		private void showHelpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (showHelpToolStripMenuItem.Checked || tsBtnShowHelp.Checked)
			{
				showHelpToolStripMenuItem.Checked = false;
				tsBtnShowHelp.Checked = false;
				if (frmHelp == null)
					return;
				frmHelp.Hide();
			}
			else
			{
				tsBtnShowHelp.Checked = true;
				if (frmHelp == null)
				{
					frmHelp = new HelpForm();
					frmHelp.FormClosed += new FormClosedEventHandler(frmHelp_FormClosed);
					frmHelp.Disposed += new EventHandler(frmHelp_Disposed);
				}
				frmHelp.Location = new Point()
				{
					X = Location.X + Width,
					Y = Location.Y
				};
				frmHelp.Show();
			}
		}

		private void usersGuideToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (File.Exists(Application.StartupPath + "\\SX1276SKA_usersguide.pdf"))
			{
				Process.Start(Application.StartupPath + "\\SX1276SKA_usersguide.pdf");
			}
			else
			{
				int num = (int)MessageBox.Show("Unable to find the user's guide document!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AboutBox ab = new AboutBox();
			ab.Version = device.Version.ToString(2);
			ab.ShowDialog();
		}

		#region InitializeComponent()
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.ssMainStatus = new System.Windows.Forms.StatusStrip();
			this.tsLblVersion = new System.Windows.Forms.ToolStripStatusLabel();
			this.tsVersion = new System.Windows.Forms.ToolStripStatusLabel();
			this.tsLblSeparator1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.tsLblFwVersion = new System.Windows.Forms.ToolStripStatusLabel();
			this.tsFwVersion = new System.Windows.Forms.ToolStripStatusLabel();
			this.tsBtnFwUpdate = new System.Windows.Forms.ToolStripButton();
			this.tsLblSeparator2 = new System.Windows.Forms.ToolStripStatusLabel();
			this.tsLblChipVersion = new System.Windows.Forms.ToolStripStatusLabel();
			this.tsChipVersion = new System.Windows.Forms.ToolStripStatusLabel();
			this.tsLblSeparator3 = new System.Windows.Forms.ToolStripStatusLabel();
			this.tsLblConfigFileName = new System.Windows.Forms.ToolStripStatusLabel();
			this.tsLblSeparator4 = new System.Windows.Forms.ToolStripStatusLabel();
			this.tsLblConnectionStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.ledStatus = new SemtechLib.Controls.ToolStripLed();
			this.tsLblStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.ssMainStatus1 = new System.Windows.Forms.StatusStrip();
			this.msMainMenu = new SemtechLib.Controls.MenuStripEx();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mFileSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.btnOpenConfig = new System.Windows.Forms.ToolStripMenuItem();
			this.btnSaveConfig = new System.Windows.Forms.ToolStripMenuItem();
			this.btnSaveAsConfig = new System.Windows.Forms.ToolStripMenuItem();
			this.mFileSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.actionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.modemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.miModemFSK = new System.Windows.Forms.ToolStripMenuItem();
			this.miModemLoRa = new System.Windows.Forms.ToolStripMenuItem();
			this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showRegistersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.monitorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.monitorOffToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.monitorOnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.startuptimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.rssiAnalyserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.spectrumAnalyserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mHelpSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.usersGuideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mHelpSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tsBtnRefresh = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.ofConfigFileOpenDlg = new System.Windows.Forms.OpenFileDialog();
			this.sfConfigFileSaveDlg = new System.Windows.Forms.SaveFileDialog();
			this.tipMainForm = new System.Windows.Forms.ToolTip(this.components);
			this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
			this.tsActionToolbar = new SemtechLib.Controls.ToolStripEx();
			this.tsBtnOpenFile = new System.Windows.Forms.ToolStripButton();
			this.tsBtnSaveFile = new System.Windows.Forms.ToolStripButton();
			this.tbFileSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.btnOpenDevice = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
			this.tsLblModem = new System.Windows.Forms.ToolStripLabel();
			this.tsModemLoRa = new System.Windows.Forms.ToolStripButton();
			this.tsModemFSK = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.tsBtnReset = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.tsBtnStartupTime = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.tsBtnShowRegisters = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.tsLblMonitor = new System.Windows.Forms.ToolStripLabel();
			this.tsBtnMonitorOn = new System.Windows.Forms.ToolStripButton();
			this.tsBtnMonitorOff = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.tsBtnShowHelp = new System.Windows.Forms.ToolStripButton();
			this.tsSeparatorPerModeOn = new System.Windows.Forms.ToolStripSeparator();
			this.tsLblPerModeOn = new System.Windows.Forms.ToolStripLabel();
			this.tsSeparatorDebugOn = new System.Windows.Forms.ToolStripSeparator();
			this.tsLblDebugOn = new System.Windows.Forms.ToolStripLabel();
			this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
			this.ssMainStatus.SuspendLayout();
			this.ssMainStatus1.SuspendLayout();
			this.msMainMenu.SuspendLayout();
			this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
			this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
			this.toolStripContainer1.SuspendLayout();
			this.tsActionToolbar.SuspendLayout();
			this.SuspendLayout();
			// 
			// ssMainStatus
			// 
			this.ssMainStatus.Dock = System.Windows.Forms.DockStyle.None;
			this.ssMainStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsLblVersion,
            this.tsVersion,
            this.tsLblSeparator1,
            this.tsLblFwVersion,
            this.tsFwVersion,
            this.tsBtnFwUpdate,
            this.tsLblSeparator2,
            this.tsLblChipVersion,
            this.tsChipVersion,
            this.tsLblSeparator3,
            this.tsLblConfigFileName,
            this.tsLblSeparator4,
            this.tsLblConnectionStatus,
            this.ledStatus});
			this.ssMainStatus.Location = new System.Drawing.Point(0, 22);
			this.ssMainStatus.Name = "ssMainStatus";
			this.ssMainStatus.ShowItemToolTips = true;
			this.ssMainStatus.Size = new System.Drawing.Size(1008, 22);
			this.ssMainStatus.SizingGrip = false;
			this.ssMainStatus.TabIndex = 3;
			this.ssMainStatus.Text = "Main status";
			// 
			// tsLblVersion
			// 
			this.tsLblVersion.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
			this.tsLblVersion.Name = "tsLblVersion";
			this.tsLblVersion.Size = new System.Drawing.Size(49, 16);
			this.tsLblVersion.Text = "Version:";
			// 
			// tsVersion
			// 
			this.tsVersion.AutoSize = false;
			this.tsVersion.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
			this.tsVersion.Name = "tsVersion";
			this.tsVersion.Size = new System.Drawing.Size(48, 16);
			this.tsVersion.Text = "-";
			// 
			// tsLblSeparator1
			// 
			this.tsLblSeparator1.Margin = new System.Windows.Forms.Padding(3);
			this.tsLblSeparator1.Name = "tsLblSeparator1";
			this.tsLblSeparator1.Size = new System.Drawing.Size(10, 16);
			this.tsLblSeparator1.Text = "|";
			// 
			// tsLblFwVersion
			// 
			this.tsLblFwVersion.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
			this.tsLblFwVersion.Name = "tsLblFwVersion";
			this.tsLblFwVersion.Size = new System.Drawing.Size(101, 16);
			this.tsLblFwVersion.Text = "Firmware Version:";
			// 
			// tsFwVersion
			// 
			this.tsFwVersion.AutoSize = false;
			this.tsFwVersion.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
			this.tsFwVersion.Name = "tsFwVersion";
			this.tsFwVersion.Size = new System.Drawing.Size(48, 16);
			this.tsFwVersion.Text = "-";
			// 
			// tsBtnFwUpdate
			// 
			this.tsBtnFwUpdate.Name = "tsBtnFwUpdate";
			this.tsBtnFwUpdate.Size = new System.Drawing.Size(23, 20);
			// 
			// tsLblSeparator2
			// 
			this.tsLblSeparator2.Margin = new System.Windows.Forms.Padding(3);
			this.tsLblSeparator2.Name = "tsLblSeparator2";
			this.tsLblSeparator2.Size = new System.Drawing.Size(10, 16);
			this.tsLblSeparator2.Text = "|";
			// 
			// tsLblChipVersion
			// 
			this.tsLblChipVersion.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
			this.tsLblChipVersion.Name = "tsLblChipVersion";
			this.tsLblChipVersion.Size = new System.Drawing.Size(76, 16);
			this.tsLblChipVersion.Text = "Chip version:";
			// 
			// tsChipVersion
			// 
			this.tsChipVersion.AutoSize = false;
			this.tsChipVersion.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
			this.tsChipVersion.Name = "tsChipVersion";
			this.tsChipVersion.Size = new System.Drawing.Size(48, 16);
			this.tsChipVersion.Text = "-";
			// 
			// tsLblSeparator3
			// 
			this.tsLblSeparator3.Margin = new System.Windows.Forms.Padding(3);
			this.tsLblSeparator3.Name = "tsLblSeparator3";
			this.tsLblSeparator3.Size = new System.Drawing.Size(10, 16);
			this.tsLblSeparator3.Text = "|";
			// 
			// tsLblConfigFileName
			// 
			this.tsLblConfigFileName.AutoToolTip = true;
			this.tsLblConfigFileName.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsLblConfigFileName.Margin = new System.Windows.Forms.Padding(3);
			this.tsLblConfigFileName.Name = "tsLblConfigFileName";
			this.tsLblConfigFileName.Size = new System.Drawing.Size(379, 16);
			this.tsLblConfigFileName.Spring = true;
			this.tsLblConfigFileName.Text = "Config File:";
			this.tsLblConfigFileName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tsLblConfigFileName.ToolTipText = "Shows the active Config file when File-> Open/Save is used";
			// 
			// tsLblSeparator4
			// 
			this.tsLblSeparator4.Margin = new System.Windows.Forms.Padding(3);
			this.tsLblSeparator4.Name = "tsLblSeparator4";
			this.tsLblSeparator4.Size = new System.Drawing.Size(10, 16);
			this.tsLblSeparator4.Text = "|";
			// 
			// tsLblConnectionStatus
			// 
			this.tsLblConnectionStatus.Margin = new System.Windows.Forms.Padding(3);
			this.tsLblConnectionStatus.Name = "tsLblConnectionStatus";
			this.tsLblConnectionStatus.Size = new System.Drawing.Size(106, 16);
			this.tsLblConnectionStatus.Text = "Connection status:";
			// 
			// ledStatus
			// 
			this.ledStatus.BackColor = System.Drawing.Color.Transparent;
			this.ledStatus.Checked = false;
			this.ledStatus.LedAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.ledStatus.LedColor = System.Drawing.Color.Green;
			this.ledStatus.LedSize = new System.Drawing.Size(11, 11);
			this.ledStatus.Margin = new System.Windows.Forms.Padding(3);
			this.ledStatus.Name = "ledStatus";
			this.ledStatus.Size = new System.Drawing.Size(15, 16);
			this.ledStatus.Text = "Connection status";
			// 
			// tsLblStatus
			// 
			this.tsLblStatus.Margin = new System.Windows.Forms.Padding(3);
			this.tsLblStatus.Name = "tsLblStatus";
			this.tsLblStatus.Size = new System.Drawing.Size(12, 16);
			this.tsLblStatus.Text = "-";
			this.tsLblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tsLblStatus.ToolTipText = "Shows SKA messages.";
			// 
			// ssMainStatus1
			// 
			this.ssMainStatus1.Dock = System.Windows.Forms.DockStyle.None;
			this.ssMainStatus1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsLblStatus});
			this.ssMainStatus1.Location = new System.Drawing.Point(0, 0);
			this.ssMainStatus1.Name = "ssMainStatus1";
			this.ssMainStatus1.ShowItemToolTips = true;
			this.ssMainStatus1.Size = new System.Drawing.Size(1008, 22);
			this.ssMainStatus1.SizingGrip = false;
			this.ssMainStatus1.TabIndex = 3;
			this.ssMainStatus1.Text = "Main status 1";
			// 
			// msMainMenu
			// 
			this.msMainMenu.ClickThrough = true;
			this.msMainMenu.Dock = System.Windows.Forms.DockStyle.None;
			this.msMainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.actionToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
			this.msMainMenu.Location = new System.Drawing.Point(0, 0);
			this.msMainMenu.Name = "msMainMenu";
			this.msMainMenu.Size = new System.Drawing.Size(1008, 24);
			this.msMainMenu.SuppressHighlighting = false;
			this.msMainMenu.TabIndex = 0;
			this.msMainMenu.Text = "File";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectToolStripMenuItem,
            this.mFileSeparator1,
            this.btnOpenConfig,
            this.btnSaveConfig,
            this.btnSaveAsConfig,
            this.mFileSeparator2,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// connectToolStripMenuItem
			// 
			this.connectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("connectToolStripMenuItem.Image")));
			this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
			this.connectToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
			this.connectToolStripMenuItem.Text = "&Connect";
			this.connectToolStripMenuItem.Visible = false;
			this.connectToolStripMenuItem.Click += new System.EventHandler(this.btnOpenDevice_Click);
			// 
			// mFileSeparator1
			// 
			this.mFileSeparator1.Name = "mFileSeparator1";
			this.mFileSeparator1.Size = new System.Drawing.Size(159, 6);
			this.mFileSeparator1.Visible = false;
			// 
			// btnOpenConfig
			// 
			this.btnOpenConfig.Enabled = false;
			this.btnOpenConfig.Image = ((System.Drawing.Image)(resources.GetObject("loadToolStripMenuItem.Image")));
			this.btnOpenConfig.Name = "btnOpenConfig";
			this.btnOpenConfig.Size = new System.Drawing.Size(162, 22);
			this.btnOpenConfig.Text = "&Open Config...";
			this.btnOpenConfig.Click += new System.EventHandler(this.btnOpenConfig_Click);
			// 
			// btnSaveConfig
			// 
			this.btnSaveConfig.Enabled = false;
			this.btnSaveConfig.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
			this.btnSaveConfig.Name = "btnSaveConfig";
			this.btnSaveConfig.Size = new System.Drawing.Size(162, 22);
			this.btnSaveConfig.Text = "&Save Config";
			this.btnSaveConfig.Click += new System.EventHandler(this.btnSaveConfig_Click);
			// 
			// btnSaveAsConfig
			// 
			this.btnSaveAsConfig.Enabled = false;
			this.btnSaveAsConfig.Image = ((System.Drawing.Image)(resources.GetObject("saveAsToolStripMenuItem.Image")));
			this.btnSaveAsConfig.Name = "btnSaveAsConfig";
			this.btnSaveAsConfig.Size = new System.Drawing.Size(162, 22);
			this.btnSaveAsConfig.Text = "Save Config &As...";
			this.btnSaveAsConfig.Click += new System.EventHandler(this.btnSaveAsConfig_Click);
			// 
			// mFileSeparator2
			// 
			this.mFileSeparator2.Name = "mFileSeparator2";
			this.mFileSeparator2.Size = new System.Drawing.Size(159, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
			this.exitToolStripMenuItem.Text = "&Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// actionToolStripMenuItem
			// 
			this.actionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.modemToolStripMenuItem,
            this.resetToolStripMenuItem,
            this.refreshToolStripMenuItem,
            this.showRegistersToolStripMenuItem,
            this.monitorToolStripMenuItem,
            this.startuptimeToolStripMenuItem});
			this.actionToolStripMenuItem.Name = "actionToolStripMenuItem";
			this.actionToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
			this.actionToolStripMenuItem.Text = "&Action";
			// 
			// modemToolStripMenuItem
			// 
			this.modemToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miModemFSK,
            this.miModemLoRa});
			this.modemToolStripMenuItem.Enabled = false;
			this.modemToolStripMenuItem.Name = "modemToolStripMenuItem";
			this.modemToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
			this.modemToolStripMenuItem.Text = "M&odem";
			// 
			// modemFskToolStripMenuItem
			// 
			this.miModemFSK.Name = "modemFskToolStripMenuItem";
			this.miModemFSK.Size = new System.Drawing.Size(100, 22);
			this.miModemFSK.Text = "FSK";
			this.miModemFSK.Click += new System.EventHandler(this.modemToolStripMenuItem_Click);
			// 
			// modemLoRaToolStripMenuItem
			// 
			this.miModemLoRa.Checked = true;
			this.miModemLoRa.CheckState = System.Windows.Forms.CheckState.Checked;
			this.miModemLoRa.Name = "modemLoRaToolStripMenuItem";
			this.miModemLoRa.Size = new System.Drawing.Size(100, 22);
			this.miModemLoRa.Text = "&LoRa";
			this.miModemLoRa.Click += new System.EventHandler(this.modemToolStripMenuItem_Click);
			// 
			// resetToolStripMenuItem
			// 
			this.resetToolStripMenuItem.Enabled = false;
			this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
			this.resetToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
			this.resetToolStripMenuItem.Text = "R&eset";
			this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
			// 
			// refreshToolStripMenuItem
			// 
			this.refreshToolStripMenuItem.Enabled = false;
			this.refreshToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("refreshToolStripMenuItem.Image")));
			this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
			this.refreshToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
			this.refreshToolStripMenuItem.Text = "&Refresh";
			this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
			// 
			// showRegistersToolStripMenuItem
			// 
			this.showRegistersToolStripMenuItem.Enabled = false;
			this.showRegistersToolStripMenuItem.Name = "showRegistersToolStripMenuItem";
			this.showRegistersToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
			this.showRegistersToolStripMenuItem.Text = "&Show registers";
			this.showRegistersToolStripMenuItem.Click += new System.EventHandler(this.showRegistersToolStripMenuItem_Click);
			// 
			// monitorToolStripMenuItem
			// 
			this.monitorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.monitorOffToolStripMenuItem,
            this.monitorOnToolStripMenuItem});
			this.monitorToolStripMenuItem.Enabled = false;
			this.monitorToolStripMenuItem.Name = "monitorToolStripMenuItem";
			this.monitorToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
			this.monitorToolStripMenuItem.Text = "&Monitor";
			// 
			// monitorOffToolStripMenuItem
			// 
			this.monitorOffToolStripMenuItem.Name = "monitorOffToolStripMenuItem";
			this.monitorOffToolStripMenuItem.Size = new System.Drawing.Size(95, 22);
			this.monitorOffToolStripMenuItem.Text = "OFF";
			this.monitorOffToolStripMenuItem.Click += new System.EventHandler(this.monitorToolStripMenuItem_Click);
			// 
			// monitorOnToolStripMenuItem
			// 
			this.monitorOnToolStripMenuItem.Checked = true;
			this.monitorOnToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.monitorOnToolStripMenuItem.Name = "monitorOnToolStripMenuItem";
			this.monitorOnToolStripMenuItem.Size = new System.Drawing.Size(95, 22);
			this.monitorOnToolStripMenuItem.Text = "&ON";
			this.monitorOnToolStripMenuItem.Click += new System.EventHandler(this.monitorToolStripMenuItem_Click);
			// 
			// startuptimeToolStripMenuItem
			// 
			this.startuptimeToolStripMenuItem.Enabled = false;
			this.startuptimeToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("startuptimeToolStripMenuItem.Image")));
			this.startuptimeToolStripMenuItem.Name = "startuptimeToolStripMenuItem";
			this.startuptimeToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
			this.startuptimeToolStripMenuItem.Text = "Startup &time...";
			this.startuptimeToolStripMenuItem.Click += new System.EventHandler(this.startuptimeToolStripMenuItem_Click);
			// 
			// toolsToolStripMenuItem
			// 
			this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rssiAnalyserToolStripMenuItem,
            this.spectrumAnalyserToolStripMenuItem});
			this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
			this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
			this.toolsToolStripMenuItem.Text = "Tools";
			// 
			// rssiAnalyserToolStripMenuItem
			// 
			this.rssiAnalyserToolStripMenuItem.Enabled = false;
			this.rssiAnalyserToolStripMenuItem.Name = "rssiAnalyserToolStripMenuItem";
			this.rssiAnalyserToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.rssiAnalyserToolStripMenuItem.Text = "RSSI analyser";
			this.rssiAnalyserToolStripMenuItem.Click += new System.EventHandler(this.rssiAnalyserToolStripMenuItem_Click);
			// 
			// spectrumAnalyserToolStripMenuItem
			// 
			this.spectrumAnalyserToolStripMenuItem.Enabled = false;
			this.spectrumAnalyserToolStripMenuItem.Name = "spectrumAnalyserToolStripMenuItem";
			this.spectrumAnalyserToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
			this.spectrumAnalyserToolStripMenuItem.Text = "Spectrum analyser";
			this.spectrumAnalyserToolStripMenuItem.Click += new System.EventHandler(this.spectrumAnalyserToolStripMenuItem_Click);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showHelpToolStripMenuItem,
            this.mHelpSeparator1,
            this.usersGuideToolStripMenuItem,
            this.mHelpSeparator2,
            this.aboutToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.helpToolStripMenuItem.Text = "&Help";
			// 
			// showHelpToolStripMenuItem
			// 
			this.showHelpToolStripMenuItem.Enabled = false;
			this.showHelpToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("showHelpToolStripMenuItem.Image")));
			this.showHelpToolStripMenuItem.Name = "showHelpToolStripMenuItem";
			this.showHelpToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
			this.showHelpToolStripMenuItem.Text = "Help";
			this.showHelpToolStripMenuItem.Click += new System.EventHandler(this.showHelpToolStripMenuItem_Click);
			// 
			// mHelpSeparator1
			// 
			this.mHelpSeparator1.Name = "mHelpSeparator1";
			this.mHelpSeparator1.Size = new System.Drawing.Size(228, 6);
			// 
			// usersGuideToolStripMenuItem
			// 
			this.usersGuideToolStripMenuItem.Name = "usersGuideToolStripMenuItem";
			this.usersGuideToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
			this.usersGuideToolStripMenuItem.Text = "&User\'s Guide...";
			this.usersGuideToolStripMenuItem.Click += new System.EventHandler(this.usersGuideToolStripMenuItem_Click);
			// 
			// mHelpSeparator2
			// 
			this.mHelpSeparator2.Name = "mHelpSeparator2";
			this.mHelpSeparator2.Size = new System.Drawing.Size(228, 6);
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("aboutToolStripMenuItem.Image")));
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
			this.aboutToolStripMenuItem.Text = "&About SX1276 Evaluation Kit...";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
			// 
			// tsBtnRefresh
			// 
			this.tsBtnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsBtnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnRefresh.Image")));
			this.tsBtnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsBtnRefresh.Name = "tsBtnRefresh";
			this.tsBtnRefresh.Size = new System.Drawing.Size(23, 22);
			this.tsBtnRefresh.Text = "Refresh";
			this.tsBtnRefresh.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
			// 
			// ofConfigFileOpenDlg
			// 
			this.ofConfigFileOpenDlg.DefaultExt = "*.cfg";
			this.ofConfigFileOpenDlg.Filter = "Config Files(*.cfg)|*.cfg|AllFiles(*.*)|*.*";
			// 
			// sfConfigFileSaveDlg
			// 
			this.sfConfigFileSaveDlg.DefaultExt = "*.cfg";
			this.sfConfigFileSaveDlg.Filter = "Config Files(*.cfg)|*.cfg|AllFiles(*.*)|*.*";
			// 
			// tipMainForm
			// 
			this.tipMainForm.ShowAlways = true;
			// 
			// toolStripContainer1
			// 
			// 
			// toolStripContainer1.BottomToolStripPanel
			// 
			this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.ssMainStatus1);
			this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.ssMainStatus);
			this.toolStripContainer1.BottomToolStripPanel.MaximumSize = new System.Drawing.Size(0, 44);
			// 
			// toolStripContainer1.ContentPanel
			// 
			this.toolStripContainer1.ContentPanel.AutoScroll = true;
			this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1008, 524);
			this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.toolStripContainer1.LeftToolStripPanelVisible = false;
			this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
			this.toolStripContainer1.Name = "toolStripContainer1";
			this.toolStripContainer1.RightToolStripPanelVisible = false;
			this.toolStripContainer1.Size = new System.Drawing.Size(1008, 618);
			this.toolStripContainer1.TabIndex = 0;
			this.toolStripContainer1.Text = "toolStripContainer1";
			// 
			// toolStripContainer1.TopToolStripPanel
			// 
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.msMainMenu);
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsActionToolbar);
			this.toolStripContainer1.TopToolStripPanel.MaximumSize = new System.Drawing.Size(0, 50);
			// 
			// tsActionToolbar
			// 
			this.tsActionToolbar.ClickThrough = true;
			this.tsActionToolbar.Dock = System.Windows.Forms.DockStyle.None;
			this.tsActionToolbar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.tsActionToolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsBtnOpenFile,
            this.tsBtnSaveFile,
            this.tbFileSeparator1,
            this.btnOpenDevice,
            this.toolStripSeparator7,
            this.tsLblModem,
            this.tsModemLoRa,
            this.tsModemFSK,
            this.toolStripSeparator5,
            this.tsBtnReset,
            this.toolStripSeparator1,
            this.tsBtnRefresh,
            this.tsBtnStartupTime,
            this.toolStripSeparator2,
            this.tsBtnShowRegisters,
            this.toolStripSeparator4,
            this.tsLblMonitor,
            this.tsBtnMonitorOn,
            this.tsBtnMonitorOff,
            this.toolStripSeparator6,
            this.tsBtnShowHelp,
            this.tsSeparatorPerModeOn,
            this.tsLblPerModeOn,
            this.tsSeparatorDebugOn,
            this.tsLblDebugOn});
			this.tsActionToolbar.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
			this.tsActionToolbar.Location = new System.Drawing.Point(3, 24);
			this.tsActionToolbar.Name = "tsActionToolbar";
			this.tsActionToolbar.Size = new System.Drawing.Size(698, 25);
			this.tsActionToolbar.SuppressHighlighting = false;
			this.tsActionToolbar.TabIndex = 2;
			this.tsActionToolbar.Text = "Action";
			// 
			// tsBtnOpenFile
			// 
			this.tsBtnOpenFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsBtnOpenFile.Enabled = false;
			this.tsBtnOpenFile.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnOpenFile.Image")));
			this.tsBtnOpenFile.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsBtnOpenFile.Name = "tsBtnOpenFile";
			this.tsBtnOpenFile.Size = new System.Drawing.Size(23, 22);
			this.tsBtnOpenFile.Text = "Open Config file";
			this.tsBtnOpenFile.Click += new System.EventHandler(this.btnOpenConfig_Click);
			// 
			// tsBtnSaveFile
			// 
			this.tsBtnSaveFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsBtnSaveFile.Enabled = false;
			this.tsBtnSaveFile.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnSaveFile.Image")));
			this.tsBtnSaveFile.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsBtnSaveFile.Name = "tsBtnSaveFile";
			this.tsBtnSaveFile.Size = new System.Drawing.Size(23, 22);
			this.tsBtnSaveFile.Text = "Save Config file";
			this.tsBtnSaveFile.Click += new System.EventHandler(this.btnSaveConfig_Click);
			// 
			// tbFileSeparator1
			// 
			this.tbFileSeparator1.Name = "tbFileSeparator1";
			this.tbFileSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// btnOpenDevice
			// 
			this.btnOpenDevice.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnOpenDevice.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnOpenDevice.Image")));
			this.btnOpenDevice.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnOpenDevice.Name = "btnOpenDevice";
			this.btnOpenDevice.Size = new System.Drawing.Size(23, 22);
			this.btnOpenDevice.Text = "Connect";
			this.btnOpenDevice.Click += new System.EventHandler(this.btnOpenDevice_Click);
			// 
			// toolStripSeparator7
			// 
			this.toolStripSeparator7.Name = "toolStripSeparator7";
			this.toolStripSeparator7.Size = new System.Drawing.Size(6, 25);
			// 
			// tsLblModem
			// 
			this.tsLblModem.Name = "tsLblModem";
			this.tsLblModem.Size = new System.Drawing.Size(52, 22);
			this.tsLblModem.Text = "Modem:";
			// 
			// tsBtnModemLoRa
			// 
			this.tsModemLoRa.Checked = true;
			this.tsModemLoRa.CheckState = System.Windows.Forms.CheckState.Checked;
			this.tsModemLoRa.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsModemLoRa.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnModemLoRa.Image")));
			this.tsModemLoRa.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsModemLoRa.Name = "tsBtnModemLoRa";
			this.tsModemLoRa.Size = new System.Drawing.Size(37, 22);
			this.tsModemLoRa.Text = "LoRa";
			this.tsModemLoRa.ToolTipText = "Enables the SX1276 LoRa modem";
			this.tsModemLoRa.Click += new System.EventHandler(this.modemToolStripMenuItem_Click);
			// 
			// tsBtnModemFsk
			// 
			this.tsModemFSK.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsModemFSK.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnModemFsk.Image")));
			this.tsModemFSK.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsModemFSK.Name = "tsBtnModemFsk";
			this.tsModemFSK.Size = new System.Drawing.Size(30, 22);
			this.tsModemFSK.Text = "FSK";
			this.tsModemFSK.ToolTipText = "Enables the SX1276 FSK modem";
			this.tsModemFSK.Click += new System.EventHandler(this.modemToolStripMenuItem_Click);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
			// 
			// tsBtnReset
			// 
			this.tsBtnReset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsBtnReset.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnReset.Image")));
			this.tsBtnReset.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsBtnReset.Name = "tsBtnReset";
			this.tsBtnReset.Size = new System.Drawing.Size(39, 22);
			this.tsBtnReset.Text = "Reset";
			this.tsBtnReset.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// tsBtnStartupTime
			// 
			this.tsBtnStartupTime.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsBtnStartupTime.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnStartupTime.Image")));
			this.tsBtnStartupTime.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsBtnStartupTime.Name = "tsBtnStartupTime";
			this.tsBtnStartupTime.Size = new System.Drawing.Size(23, 22);
			this.tsBtnStartupTime.Text = "Startup time";
			this.tsBtnStartupTime.Click += new System.EventHandler(this.startuptimeToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// tsBtnShowRegisters
			// 
			this.tsBtnShowRegisters.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsBtnShowRegisters.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			this.tsBtnShowRegisters.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnShowRegisters.Image")));
			this.tsBtnShowRegisters.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsBtnShowRegisters.Name = "tsBtnShowRegisters";
			this.tsBtnShowRegisters.Size = new System.Drawing.Size(33, 22);
			this.tsBtnShowRegisters.Text = "Reg";
			this.tsBtnShowRegisters.ToolTipText = "Displays SX1276 raw registers window";
			this.tsBtnShowRegisters.Click += new System.EventHandler(this.showRegistersToolStripMenuItem_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
			// 
			// tsLblMonitor
			// 
			this.tsLblMonitor.Name = "tsLblMonitor";
			this.tsLblMonitor.Size = new System.Drawing.Size(53, 22);
			this.tsLblMonitor.Text = "Monitor:";
			// 
			// tsBtnMonitorOn
			// 
			this.tsBtnMonitorOn.Checked = true;
			this.tsBtnMonitorOn.CheckState = System.Windows.Forms.CheckState.Checked;
			this.tsBtnMonitorOn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsBtnMonitorOn.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnMonitorOn.Image")));
			this.tsBtnMonitorOn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsBtnMonitorOn.Name = "tsBtnMonitorOn";
			this.tsBtnMonitorOn.Size = new System.Drawing.Size(29, 22);
			this.tsBtnMonitorOn.Text = "ON";
			this.tsBtnMonitorOn.ToolTipText = "Enables the SX1276 monitor mode";
			this.tsBtnMonitorOn.Click += new System.EventHandler(this.monitorToolStripMenuItem_Click);
			// 
			// tsBtnMonitorOff
			// 
			this.tsBtnMonitorOff.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.tsBtnMonitorOff.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnMonitorOff.Image")));
			this.tsBtnMonitorOff.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsBtnMonitorOff.Name = "tsBtnMonitorOff";
			this.tsBtnMonitorOff.Size = new System.Drawing.Size(32, 22);
			this.tsBtnMonitorOff.Text = "OFF";
			this.tsBtnMonitorOff.ToolTipText = "Disables the SX1276 monitor mode";
			this.tsBtnMonitorOff.Click += new System.EventHandler(this.monitorToolStripMenuItem_Click);
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
			// 
			// tsBtnShowHelp
			// 
			this.tsBtnShowHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsBtnShowHelp.Enabled = false;
			this.tsBtnShowHelp.Image = ((System.Drawing.Image)(resources.GetObject("tsBtnShowHelp.Image")));
			this.tsBtnShowHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsBtnShowHelp.Name = "tsBtnShowHelp";
			this.tsBtnShowHelp.Size = new System.Drawing.Size(23, 22);
			this.tsBtnShowHelp.Text = "Help";
			this.tsBtnShowHelp.Click += new System.EventHandler(this.showHelpToolStripMenuItem_Click);
			// 
			// tsSeparatorPerModeOn
			// 
			this.tsSeparatorPerModeOn.Name = "tsSeparatorPerModeOn";
			this.tsSeparatorPerModeOn.Size = new System.Drawing.Size(6, 25);
			this.tsSeparatorPerModeOn.Visible = false;
			// 
			// tsLblPerModeOn
			// 
			this.tsLblPerModeOn.ForeColor = System.Drawing.Color.Red;
			this.tsLblPerModeOn.Name = "tsLblPerModeOn";
			this.tsLblPerModeOn.Size = new System.Drawing.Size(82, 22);
			this.tsLblPerModeOn.Text = "PER mode ON";
			this.tsLblPerModeOn.Visible = false;
			// 
			// tsSeparatorDebugOn
			// 
			this.tsSeparatorDebugOn.Name = "tsSeparatorDebugOn";
			this.tsSeparatorDebugOn.Size = new System.Drawing.Size(6, 25);
			this.tsSeparatorDebugOn.Visible = false;
			// 
			// tsLblDebugOn
			// 
			this.tsLblDebugOn.ForeColor = System.Drawing.Color.Red;
			this.tsLblDebugOn.Name = "tsLblDebugOn";
			this.tsLblDebugOn.Size = new System.Drawing.Size(85, 22);
			this.tsLblDebugOn.Text = "DBG mode ON";
			this.tsLblDebugOn.Visible = false;
			// 
			// toolStripLabel1
			// 
			this.toolStripLabel1.Name = "toolStripLabel1";
			this.toolStripLabel1.Size = new System.Drawing.Size(62, 22);
			this.toolStripLabel1.Text = "Product ID:";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1008, 618);
			this.Controls.Add(this.toolStripContainer1);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MainMenuStrip = this.msMainMenu;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "SX1276 Evaluation Kit";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Mainform_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Mainform_KeyDown);
			this.ssMainStatus.ResumeLayout(false);
			this.ssMainStatus.PerformLayout();
			this.ssMainStatus1.ResumeLayout(false);
			this.ssMainStatus1.PerformLayout();
			this.msMainMenu.ResumeLayout(false);
			this.msMainMenu.PerformLayout();
			this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
			this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
			this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
			this.toolStripContainer1.TopToolStripPanel.PerformLayout();
			this.toolStripContainer1.ResumeLayout(false);
			this.toolStripContainer1.PerformLayout();
			this.tsActionToolbar.ResumeLayout(false);
			this.tsActionToolbar.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion
	}
}

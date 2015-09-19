using LoRaModem;
using MyCSLib.Controls;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RFM9x_GUI
{
	public class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
			tscbChipVer.SelectedIndex = 0;
			ucLoRa.ChipVer = ucLoRa.ChipSet.RF96;
		}

		#region InitializeComponent
		private IContainer components;
		private MenuStrip menuStrip;
		private ToolStripMenuItem fileToolStripMenuItem;
		private ToolStripMenuItem aboutToolStripMenuItem;
		private ToolStripMenuItem aboutHopeRFToolStripMenuItem;
		private ToolStripMenuItem openConfigToolStripMenuItem;
		private ToolStripMenuItem saveConfigFileToolStripMenuItem;
		private ToolStripMenuItem exitToolStripMenuItem;

		private ToolStrip toolStrip;
		private ToolStripButton tSBOpenFile;
		private ToolStripButton tSBSave;
		private ToolStripButton btnConnect;
		private ToolStripButton tSBReg;
		private ToolStripLabel toolStripLabel1;
		private StatusStrip statusStrip;
		private ToolStripStatusLabel tSSLConnect;
		private ToolStripLed ConnectLed;
		private ucLoRa ucLoRa;
		private ToolStripComboBox tscbChipVer;
		private ToolStripLabel ChipType;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripSeparator toolStripSeparator2;
		private ToolStripSeparator toolStripSeparator3;
		private ToolStripSeparator toolStripSeparator4;
		private ToolStripSeparator toolStripSeparator5;

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveConfigFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutHopeRFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.tSBOpenFile = new System.Windows.Forms.ToolStripButton();
			this.tSBSave = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.btnConnect = new System.Windows.Forms.ToolStripButton();
			this.ConnectLed = new MyCSLib.Controls.ToolStripLed();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.tSBReg = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.ChipType = new System.Windows.Forms.ToolStripLabel();
			this.tscbChipVer = new System.Windows.Forms.ToolStripComboBox();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.tSSLConnect = new System.Windows.Forms.ToolStripStatusLabel();
			this.ucLoRa = new LoRaModem.ucLoRa();
			this.menuStrip.SuspendLayout();
			this.toolStrip.SuspendLayout();
			this.statusStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip
			// 
			this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.aboutToolStripMenuItem});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
			this.menuStrip.Size = new System.Drawing.Size(900, 28);
			this.menuStrip.TabIndex = 0;
			this.menuStrip.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openConfigToolStripMenuItem,
            this.saveConfigFileToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// openConfigToolStripMenuItem
			// 
			this.openConfigToolStripMenuItem.Name = "openConfigToolStripMenuItem";
			this.openConfigToolStripMenuItem.Size = new System.Drawing.Size(195, 26);
			this.openConfigToolStripMenuItem.Text = "&Open Config File";
			this.openConfigToolStripMenuItem.Click += new System.EventHandler(this.openConfigToolStripMenuItem_Click);
			// 
			// saveConfigFileToolStripMenuItem
			// 
			this.saveConfigFileToolStripMenuItem.Name = "saveConfigFileToolStripMenuItem";
			this.saveConfigFileToolStripMenuItem.Size = new System.Drawing.Size(195, 26);
			this.saveConfigFileToolStripMenuItem.Text = "&Save Config File";
			this.saveConfigFileToolStripMenuItem.Click += new System.EventHandler(this.saveConfigFileToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(192, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(195, 26);
			this.exitToolStripMenuItem.Text = "&Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutHopeRFToolStripMenuItem});
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(62, 24);
			this.aboutToolStripMenuItem.Text = "About";
			// 
			// aboutHopeRFToolStripMenuItem
			// 
			this.aboutHopeRFToolStripMenuItem.Name = "aboutHopeRFToolStripMenuItem";
			this.aboutHopeRFToolStripMenuItem.Size = new System.Drawing.Size(182, 26);
			this.aboutHopeRFToolStripMenuItem.Text = "&About HopeRF";
			this.aboutHopeRFToolStripMenuItem.Click += new System.EventHandler(this.aboutHopeRFToolStripMenuItem_Click);
			// 
			// toolStrip
			// 
			this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tSBOpenFile,
            this.tSBSave,
            this.toolStripSeparator2,
            this.btnConnect,
            this.ConnectLed,
            this.toolStripSeparator3,
            this.toolStripLabel1,
            this.toolStripSeparator4,
            this.tSBReg,
            this.toolStripSeparator5,
            this.ChipType,
            this.tscbChipVer});
			this.toolStrip.Location = new System.Drawing.Point(0, 28);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.Size = new System.Drawing.Size(900, 33);
			this.toolStrip.TabIndex = 1;
			this.toolStrip.Text = "toolStrip1";
			// 
			// tSBOpenFile
			// 
			this.tSBOpenFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tSBOpenFile.Image = ((System.Drawing.Image)(resources.GetObject("tSBOpenFile.Image")));
			this.tSBOpenFile.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tSBOpenFile.Name = "tSBOpenFile";
			this.tSBOpenFile.Size = new System.Drawing.Size(24, 30);
			this.tSBOpenFile.Text = "toolStripButton1";
			this.tSBOpenFile.ToolTipText = "open config file";
			this.tSBOpenFile.Click += new System.EventHandler(this.tSBOpenFile_Click);
			// 
			// tSBSave
			// 
			this.tSBSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tSBSave.Image = ((System.Drawing.Image)(resources.GetObject("tSBSave.Image")));
			this.tSBSave.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tSBSave.Name = "tSBSave";
			this.tSBSave.Size = new System.Drawing.Size(24, 30);
			this.tSBSave.Text = "toolStripButton1";
			this.tSBSave.ToolTipText = "save config file";
			this.tSBSave.Click += new System.EventHandler(this.tSBSave_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 33);
			// 
			// btnConnect
			// 
			this.btnConnect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnConnect.Image = ((System.Drawing.Image)(resources.GetObject("btnConnect.Image")));
			this.btnConnect.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnConnect.Name = "btnConnect";
			this.btnConnect.Size = new System.Drawing.Size(24, 30);
			this.btnConnect.Text = "toolStripButton1";
			this.btnConnect.ToolTipText = "Connect/Disconnect";
			this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
			// 
			// ConnectLed
			// 
			this.ConnectLed.BackColor = System.Drawing.Color.Transparent;
			this.ConnectLed.Checked = false;
			this.ConnectLed.LedAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.ConnectLed.LedColor = System.Drawing.Color.Red;
			this.ConnectLed.LedSize = new System.Drawing.Size(11, 11);
			this.ConnectLed.Name = "ConnectLed";
			this.ConnectLed.Size = new System.Drawing.Size(20, 30);
			this.ConnectLed.Text = "toolStripLed1";
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 33);
			// 
			// toolStripLabel1
			// 
			this.toolStripLabel1.Name = "toolStripLabel1";
			this.toolStripLabel1.Size = new System.Drawing.Size(101, 30);
			this.toolStripLabel1.Text = "Modem: LoRa";
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(6, 33);
			// 
			// tSBReg
			// 
			this.tSBReg.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tSBReg.Image = ((System.Drawing.Image)(resources.GetObject("tSBReg.Image")));
			this.tSBReg.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tSBReg.Name = "tSBReg";
			this.tSBReg.Size = new System.Drawing.Size(24, 30);
			this.tSBReg.Text = "toolStripButton2";
			this.tSBReg.ToolTipText = "View";
			this.tSBReg.Click += new System.EventHandler(this.tSBReg_Click);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(6, 33);
			// 
			// ChipType
			// 
			this.ChipType.Name = "ChipType";
			this.ChipType.Size = new System.Drawing.Size(70, 30);
			this.ChipType.Text = "ChipType";
			// 
			// tscbChipVer
			// 
			this.tscbChipVer.Items.AddRange(new object[] {
            "RFM95/96/97/98",
            "RFM92/93"});
			this.tscbChipVer.Name = "tscbChipVer";
			this.tscbChipVer.Size = new System.Drawing.Size(160, 33);
			this.tscbChipVer.Text = "RFM95/96/97/98";
			// 
			// statusStrip
			// 
			this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tSSLConnect});
			this.statusStrip.Location = new System.Drawing.Point(0, 766);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Padding = new System.Windows.Forms.Padding(19, 0, 1, 0);
			this.statusStrip.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.statusStrip.Size = new System.Drawing.Size(900, 22);
			this.statusStrip.TabIndex = 2;
			this.statusStrip.Text = "statusStrip1";
			// 
			// tSSLConnect
			// 
			this.tSSLConnect.Name = "tSSLConnect";
			this.tSSLConnect.Size = new System.Drawing.Size(0, 17);
			// 
			// ucLoRa
			// 
			this.ucLoRa.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ucLoRa.FrequencyRf = new decimal(new int[] {
            433999994,
            0,
            0,
            0});
			this.ucLoRa.FrequencyStep = new decimal(new int[] {
            61,
            0,
            0,
            0});
			this.ucLoRa.Location = new System.Drawing.Point(0, 61);
			this.ucLoRa.Margin = new System.Windows.Forms.Padding(5);
			this.ucLoRa.Name = "ucLoRa";
			this.ucLoRa.Size = new System.Drawing.Size(900, 705);
			this.ucLoRa.TabIndex = 0;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(900, 788);
			this.Controls.Add(this.ucLoRa);
			this.Controls.Add(this.statusStrip);
			this.Controls.Add(this.toolStrip);
			this.Controls.Add(this.menuStrip);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "RFM9x_GUI V1.1";
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		#region tSBReg_Click
		private void tSBReg_Click(object sender, EventArgs e)
		{
			RegTable regTable = new RegTable();
			if (ucLoRa.ChipVer == ucLoRa.ChipSet.RF92)
			{
				regTable.tbRegOpMode.Text = HexString(ucLoRa.rfm92.RegOpMode.Value);
				regTable.tbRegFrMsb.Text = HexString(ucLoRa.rfm92.RegFrMsb.Value);
				regTable.tbRegFrMid.Text = HexString(ucLoRa.rfm92.RegFrMid.Value);
				regTable.tbRegFrLsb.Text = HexString(ucLoRa.rfm92.RegFrLsb.Value);
				regTable.tbRegPaConfig.Text = HexString(ucLoRa.rfm92.RegPaConfig.Value);
				regTable.tbRegPaRamp.Text = HexString(ucLoRa.rfm92.RegPaRamp.Value);
				regTable.tbRegOcp.Text = HexString(ucLoRa.rfm92.RegOcp.Value);
				regTable.tbRegLna.Text = HexString(ucLoRa.rfm92.RegLna.Value);
				regTable.tbRegIrqFlagsMask.Text = HexString(ucLoRa.rfm92.RegIrqFlagsMask.Value);
				regTable.tbRegModemConfig1.Text = HexString(ucLoRa.rfm92.RegModemConfig1.Value);
				regTable.tbRegModemConfig2.Text = HexString(ucLoRa.rfm92.RegModemConfig2.Value);
				regTable.tbRegSymbTimeoutLsb.Text = HexString(ucLoRa.rfm92.RegSymbTimeoutLsb.Value);
				regTable.tbRegPreambleMsb.Text = HexString(ucLoRa.rfm92.RegPreambleMsb.Value);
				regTable.tbRegPreambleLsb.Text = HexString(ucLoRa.rfm92.RegPreambleLsb.Value);
				regTable.tbRegModemConfig3.Text = "--";
				regTable.tbRegTcxo.Text = HexString(ucLoRa.rfm92.RegTcxo.Value);
				regTable.tbRegPaDac.Text = HexString(ucLoRa.rfm92.RegPaDac.Value);
				regTable.tbRegPllHf.Text = HexString(ucLoRa.rfm92.RegPllHf.Value);
			}
			else
			{
				regTable.tbRegOpMode.Text = HexString(ucLoRa.rfm96.RegOpMode.Value);
				regTable.tbRegFrMsb.Text = HexString(ucLoRa.rfm96.RegFrMsb.Value);
				regTable.tbRegFrMid.Text = HexString(ucLoRa.rfm96.RegFrMid.Value);
				regTable.tbRegFrLsb.Text = HexString(ucLoRa.rfm96.RegFrLsb.Value);
				regTable.tbRegPaConfig.Text = HexString(ucLoRa.rfm96.RegPaConfig.Value);
				regTable.tbRegPaRamp.Text = HexString(ucLoRa.rfm96.RegPaRamp.Value);
				regTable.tbRegOcp.Text = HexString(ucLoRa.rfm96.RegOcp.Value);
				regTable.tbRegLna.Text = HexString(ucLoRa.rfm96.RegLna.Value);
				regTable.tbRegIrqFlagsMask.Text = HexString(ucLoRa.rfm96.RegIrqFlagsMask.Value);
				regTable.tbRegModemConfig1.Text = HexString(ucLoRa.rfm96.RegModemConfig1.Value);
				regTable.tbRegModemConfig2.Text = HexString(ucLoRa.rfm96.RegModemConfig2.Value);
				regTable.tbRegSymbTimeoutLsb.Text = HexString(ucLoRa.rfm96.RegSymbTimeoutLsb.Value);
				regTable.tbRegPreambleMsb.Text = HexString(ucLoRa.rfm96.RegPreambleMsb.Value);
				regTable.tbRegPreambleLsb.Text = HexString(ucLoRa.rfm96.RegPreambleLsb.Value);
				regTable.tbRegModemConfig3.Text = HexString(ucLoRa.rfm96.RegModemConfig3.Value);
				regTable.tbRegTcxo.Text = HexString(ucLoRa.rfm96.RegTcxo.Value);
				regTable.tbRegPaDac.Text = HexString(ucLoRa.rfm96.RegPaDac.Value);
				regTable.tbRegPllHf.Text = HexString(ucLoRa.rfm96.RegPllHf.Value);
			}
			regTable.Show();
		}
		#endregion

		#region HexString
		private string HexString(byte xd)
		{
			string str = "0x";
			if (((int)xd >> 4 & 15) < 10)
			{
				str += ((int)xd >> 4 & 15).ToString();
			}
			else
			{
				switch ((int)xd >> 4 & 15)
				{
					case 10:
						str += "A";
						break;
					case 11:
						str += "B";
						break;
					case 12:
						str += "C";
						break;
					case 13:
						str += "D";
						break;
					case 14:
						str += "E";
						break;
					case 15:
						str += "F";
						break;
				}
			}
			if (((int)xd & 15) < 10)
			{
				str += ((int)xd & 15).ToString();
			}
			else
			{
				switch ((int)xd & 15)
				{
					case 10:
						str += "A";
						break;
					case 11:
						str += "B";
						break;
					case 12:
						str += "C";
						break;
					case 13:
						str += "D";
						break;
					case 14:
						str += "E";
						break;
					case 15:
						str += "F";
						break;
				}
			}
			return str;
		}
		#endregion

		#region StringToHex
		private byte StringToHex(string str)
		{
			byte hex = 0;

			if (str[2] >= '0' && str[2] <= '9')
				hex = (byte)(str[2] - '0');
			else if (str[2] >= 'A' && str[2] <= 'F')
				hex = (byte)(str[2] - 'A' + 10);
			else if ((int)str[2] >= 'a' && (int)str[2] <= 'f')
				hex = (byte)(str[2] - 'a' + 10);

			hex = (byte)(hex << 4);

			if (str[3] >= '0' && str[3] <= '9')
				hex |= (byte)(str[3] - '0');
			else if (str[3] >= 'A' && str[3] <= 'F')
				hex |= (byte)(str[3] - 'A' + 10);
			else if (str[3] >= 'a' && str[3] <= 'f')
				hex |= (byte)(str[3] - 'a' + 10);

			return hex;
		}
		#endregion

		#region btnConnect_Click
		private void btnConnect_Click(object sender, EventArgs e)
		{
			if (ConnectLed.LedColor == Color.Red)
			{
				if (ucLoRa.TryConnect())
				{
					ucLoRa.gbOpMode.Enabled = true;
					ConnectLed.LedColor = Color.Lime;
					tscbChipVer.Enabled = false;
					if (ucLoRa.ChipVer == ucLoRa.ChipSet.RF92)
						tscbChipVer.SelectedIndex = 1;
					else
						tscbChipVer.SelectedIndex = 0;
				}
				else
				{
					MessageBox.Show("Did not find HopeRF-DK!");
				}
			}
			else
			{
				ucLoRa.ftdi.LCDClear();
				ucLoRa.ftdi.Close();
				tscbChipVer.Enabled = true;
				ucLoRa.gbOpMode.Enabled = false;
				ConnectLed.LedColor = Color.Red;
			}
		}
		#endregion

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Dispose(true);
		}

		private void openConfigToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenCfgFile();
		}

		private void tSBOpenFile_Click(object sender, EventArgs e)
		{
			OpenCfgFile();
		}

		private void saveConfigFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveCfgFile();
		}

		private void tSBSave_Click(object sender, EventArgs e)
		{
			SaveCfgFile();
		}
		#region OpenCfgFile
		private void OpenCfgFile()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Title = "OpenCfgFile";
			openFileDialog.Filter = "HopeRF RF-Config Files(*.hpflr)|*.hpflr";
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				using (StreamReader streamReader = new StreamReader(openFileDialog.FileName))
				{
					string line = null;
					while ((line = streamReader.ReadLine()) != null)
					{
						if (!string.IsNullOrEmpty(line) && line.Contains("Chipset Type"))
							break;
					}

					if (line == null)
						MessageBox.Show("File format is wrong, does not recognize the chip models!");
					else
					{
						ucLoRa.ChipSet chipSet = line.Contains("RF92") ? ucLoRa.ChipSet.RF92 : ucLoRa.ChipSet.RF96;
						if (ConnectLed.LedColor == Color.Lime)
						{
							if (ucLoRa.ChipVer != chipSet)
							{
								streamReader.Close();
								MessageBox.Show("File type does not match with the hardware connection type!");
								openFileDialog.Dispose();
								return;
							}
						}
						else if (chipSet == ucLoRa.ChipSet.RF92)
						{
							ucLoRa.ChipVer = ucLoRa.ChipSet.RF92;
							tscbChipVer.SelectedIndex = 1;
						}
						else
						{
							ucLoRa.ChipVer = ucLoRa.ChipSet.RF96;
							tscbChipVer.SelectedIndex = 0;
						}
						while ((line = streamReader.ReadLine()) != null)
						{
							if (!string.IsNullOrEmpty(line) && line.Contains("const unsigned int RegTable"))
								break;
						}
						if (line == null)
							MessageBox.Show("File format is wrong, does not recognize the register values!");
						else
						{
							string str2 = streamReader.ReadLine();
							if (chipSet == ucLoRa.ChipSet.RF92)
							{
								for (; str2 != null; str2 = streamReader.ReadLine())
								{
									string str3 = "";
									string[] strArray1 = new string[5];
									char[] chArray = new char[2] { '0', 'x' };
									if (str2.Contains("0x"))
									{
										byte num1;
										for (num1 = (byte)0; (int)num1 < 17; ++num1)
										{
											switch (num1)
											{
												case (byte)0:
													str3 = "RegFrMsb";
													break;
												case (byte)1:
													str3 = "RegFrMid";
													break;
												case (byte)2:
													str3 = "RegFrLsb";
													break;
												case (byte)3:
													str3 = "RegPaConfig";
													break;
												case (byte)4:
													str3 = "RegPaRamp";
													break;
												case (byte)5:
													str3 = "RegOcp";
													break;
												case (byte)6:
													str3 = "RegLna";
													break;
												case (byte)7:
													str3 = "RegIrqFlagsMask";
													break;
												case (byte)8:
													str3 = "RegModemConfig1";
													break;
												case (byte)9:
													str3 = "RegModemConfig2";
													break;
												case (byte)10:
													str3 = "RegSymbTimeoutLsb";
													break;
												case (byte)11:
													str3 = "RegPreambleMsb";
													break;
												case (byte)12:
													str3 = "RegPreambleLsb";
													break;
												case (byte)13:
													str3 = "RegPayloadLength";
													break;
												case (byte)14:
													str3 = "RegTcxo";
													break;
												case (byte)15:
													str3 = "RegPaDac";
													break;
												case (byte)16:
													str3 = "RegPllHf";
													break;
											}
											if (str2.Contains(str3))
												break;
										}
										string[] strArray2 = str2.Trim(' ').Split('+');
										strArray2[0] = strArray2[0].Trim(' ');
										strArray2[1] = strArray2[1].Trim(' ');
										byte num2 = StringToHex(strArray2[1]);
										switch (num1)
										{
											case (byte)0:
												ucLoRa.rfm92.RegFrMsb.Value = num2;
												continue;
											case (byte)1:
												ucLoRa.rfm92.RegFrMid.Value = num2;
												continue;
											case (byte)2:
												ucLoRa.rfm92.RegFrLsb.Value = num2;
												continue;
											case (byte)3:
												ucLoRa.rfm92.RegPaConfig.Value = num2;
												continue;
											case (byte)4:
												ucLoRa.rfm92.RegPaRamp.Value = num2;
												continue;
											case (byte)5:
												ucLoRa.rfm92.RegOcp.Value = num2;
												continue;
											case (byte)6:
												ucLoRa.rfm92.RegLna.Value = num2;
												continue;
											case (byte)7:
												ucLoRa.rfm92.RegIrqFlagsMask.Value = num2;
												continue;
											case (byte)8:
												ucLoRa.rfm92.RegModemConfig1.Value = num2;
												continue;
											case (byte)9:
												ucLoRa.rfm92.RegModemConfig2.Value = num2;
												continue;
											case (byte)10:
												ucLoRa.rfm92.RegSymbTimeoutLsb.Value = num2;
												continue;
											case (byte)11:
												ucLoRa.rfm92.RegPreambleMsb.Value = num2;
												continue;
											case (byte)12:
												ucLoRa.rfm92.RegPreambleLsb.Value = num2;
												continue;
											case (byte)13:
												ucLoRa.rfm92.RegPayloadLength.Value = num2;
												continue;
											case (byte)14:
												ucLoRa.rfm92.RegTcxo.Value = num2;
												continue;
											case (byte)15:
												ucLoRa.rfm92.RegPaDac.Value = num2;
												continue;
											case (byte)16:
												ucLoRa.rfm92.RegPllHf.Value = num2;
												continue;
											default:
												continue;
										}
									}
								}
							}
							else
							{
								for (; str2 != null; str2 = streamReader.ReadLine())
								{
									string str3 = "";
									string[] strArray1 = new string[5];
									char[] chArray = new char[2] { '0', 'x' };
									if (str2.Contains("0x"))
									{
										byte num1;
										for (num1 = (byte)0; (int)num1 < 18; ++num1)
										{
											switch (num1)
											{
												case (byte)0:
													str3 = "RegFrMsb";
													break;
												case (byte)1:
													str3 = "RegFrMid";
													break;
												case (byte)2:
													str3 = "RegFrLsb";
													break;
												case (byte)3:
													str3 = "RegPaConfig";
													break;
												case (byte)4:
													str3 = "RegPaRamp";
													break;
												case (byte)5:
													str3 = "RegOcp";
													break;
												case (byte)6:
													str3 = "RegLna";
													break;
												case (byte)7:
													str3 = "RegIrqFlagsMask";
													break;
												case (byte)8:
													str3 = "RegModemConfig1";
													break;
												case (byte)9:
													str3 = "RegModemConfig2";
													break;
												case (byte)10:
													str3 = "RegSymbTimeoutLsb";
													break;
												case (byte)11:
													str3 = "RegPreambleMsb";
													break;
												case (byte)12:
													str3 = "RegPreambleLsb";
													break;
												case (byte)13:
													str3 = "RegPayloadLength";
													break;
												case (byte)14:
													str3 = "RegTcxo";
													break;
												case (byte)15:
													str3 = "RegPaDac";
													break;
												case (byte)16:
													str3 = "RegPllHf";
													break;
												case (byte)17:
													str3 = "RegModemConfig3";
													break;
											}
											if (str2.Contains(str3))
												break;
										}
										string[] strArray2 = str2.Trim(' ').Split('+');
										strArray2[0] = strArray2[0].Trim(' ');
										strArray2[1] = strArray2[1].Trim(' ');
										byte num2 = StringToHex(strArray2[1]);
										switch (num1)
										{
											case (byte)0:
												ucLoRa.rfm96.RegFrMsb.Value = num2;
												continue;
											case (byte)1:
												ucLoRa.rfm96.RegFrMid.Value = num2;
												continue;
											case (byte)2:
												ucLoRa.rfm96.RegFrLsb.Value = num2;
												continue;
											case (byte)3:
												ucLoRa.rfm96.RegPaConfig.Value = num2;
												continue;
											case (byte)4:
												ucLoRa.rfm96.RegPaRamp.Value = num2;
												continue;
											case (byte)5:
												ucLoRa.rfm96.RegOcp.Value = num2;
												continue;
											case (byte)6:
												ucLoRa.rfm96.RegLna.Value = num2;
												continue;
											case (byte)7:
												ucLoRa.rfm96.RegIrqFlagsMask.Value = num2;
												continue;
											case (byte)8:
												ucLoRa.rfm96.RegModemConfig1.Value = num2;
												continue;
											case (byte)9:
												ucLoRa.rfm96.RegModemConfig2.Value = num2;
												continue;
											case (byte)10:
												ucLoRa.rfm96.RegSymbTimeoutLsb.Value = num2;
												continue;
											case (byte)11:
												ucLoRa.rfm96.RegPreambleMsb.Value = num2;
												continue;
											case (byte)12:
												ucLoRa.rfm96.RegPreambleLsb.Value = num2;
												continue;
											case (byte)13:
												ucLoRa.rfm96.RegPayloadLength.Value = num2;
												continue;
											case (byte)14:
												ucLoRa.rfm96.RegTcxo.Value = num2;
												continue;
											case (byte)15:
												ucLoRa.rfm96.RegPaDac.Value = num2;
												continue;
											case (byte)16:
												ucLoRa.rfm96.RegPllHf.Value = num2;
												continue;
											case (byte)17:
												ucLoRa.rfm96.RegModemConfig3.Value = num2;
												continue;
											default:
												continue;
										}
									}
								}
							}
							ucLoRa.SetcbBW();
							ucLoRa.SetAllValue();
						}
					}
					streamReader.Close();
				}
			}
			openFileDialog.Dispose();
		}
		#endregion

		#region SaveCfgFile
		private void SaveCfgFile()
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Title = "SaveCfgFile";
			saveFileDialog.Filter = "HopeRF files(*.hpflr)|*.hpflr";
			saveFileDialog.CreatePrompt = true;
			saveFileDialog.OverwritePrompt = true;
			ucLoRa.UpdataAllValue();

			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				StreamWriter streamWriter = new StreamWriter(saveFileDialog.FileName);
				streamWriter.WriteLine("/****************************************");
				streamWriter.WriteLine("Hope Microelectronics co., Ltd");
				streamWriter.WriteLine("HopeRF LoRa-series COB config parameters");
				streamWriter.WriteLine("Website: http://www.HopeRF.com");
				streamWriter.WriteLine("         http://www.HopeRF.cn");
				streamWriter.WriteLine("    Tel: +86-755-82973805");
				streamWriter.WriteLine("    Fax: +86-755-82973550");
				streamWriter.WriteLine(" E-mail: sales@hoperf.com");
				streamWriter.WriteLine("****************************************/");
				streamWriter.WriteLine("");
				streamWriter.WriteLine("/****************************************");

				bool rf92chip;
				if (ucLoRa.ChipVer == ucLoRa.ChipSet.RF92)
				{
					streamWriter.WriteLine("Chipset Type: RF92/RF93");
					rf92chip = true;
				}
				else
				{
					streamWriter.WriteLine("Chipset Type: RF95/RF96/RF97/RF98");
					rf92chip = false;
				}

				streamWriter.WriteLine("****************************************/");
				streamWriter.WriteLine("");
				streamWriter.WriteLine("#define    RegFifo                 0x0000");
				streamWriter.WriteLine("#define    RegOpMode               0x0100");
				streamWriter.WriteLine("#define    RegFrMsb                0x0600");
				streamWriter.WriteLine("#define    RegFrMid                0x0700");
				streamWriter.WriteLine("#define    RegFrLsb                0x0800");
				streamWriter.WriteLine("#define    RegPaConfig             0x0900");
				streamWriter.WriteLine("#define    RegPaRamp               0x0A00");
				streamWriter.WriteLine("#define    RegOcp                  0x0B00");
				streamWriter.WriteLine("#define    RegLna                  0x0C00");
				streamWriter.WriteLine("#define    RegFifoAddrPtr          0x0D00");
				streamWriter.WriteLine("#define    RegFifoTxBaseAddr       0x0E00");
				streamWriter.WriteLine("#define    RegFifoRxBaseAddr       0x0F00");
				streamWriter.WriteLine("#define    RegFifoRxCurrentaddr    0x1000");
				streamWriter.WriteLine("#define    RegIrqFlagsMask         0x1100");
				streamWriter.WriteLine("#define    RegIrqFlags             0x1200");
				streamWriter.WriteLine("#define    RegRxNbBytes            0x1300");
				streamWriter.WriteLine("#define    RegRxHeaderCntValueMsb  0x1400");
				streamWriter.WriteLine("#define    RegRxHeaderCntValueLsb  0x1500");
				streamWriter.WriteLine("#define    RegRxPacketCntValueMsb  0x1600");
				streamWriter.WriteLine("#define    RegRxPacketCntValueLsb  0x1700");
				streamWriter.WriteLine("#define    RegModemStat            0x1800");
				streamWriter.WriteLine("#define    RegPktSnrValue          0x1900");
				streamWriter.WriteLine("#define    RegPktRssiValue         0x1A00");
				streamWriter.WriteLine("#define    RegRssiValue            0x1B00");
				streamWriter.WriteLine("#define    RegHopChannel           0x1C00");
				streamWriter.WriteLine("#define    RegModemConfig1         0x1D00");
				streamWriter.WriteLine("#define    RegModemConfig2         0x1E00");
				streamWriter.WriteLine("#define    RegSymbTimeoutLsb       0x1F00");
				streamWriter.WriteLine("#define    RegPreambleMsb          0x2000");
				streamWriter.WriteLine("#define    RegPreambleLsb          0x2100");
				streamWriter.WriteLine("#define    RegPayloadLength        0x2200");
				streamWriter.WriteLine("#define    RegMaxPayloadLength     0x2300");
				streamWriter.WriteLine("#define    RegHopPeriod            0x2400");
				streamWriter.WriteLine("#define    RegFifoRxByteAddr       0x2500");
				streamWriter.WriteLine("#define    RegModemConfig3         0x2600  //only for RF95/96/97/98");

				if (rf92chip)
				{
					streamWriter.WriteLine("#define    RegTcxo                 0x5800");
					streamWriter.WriteLine("#define    RegPaDac                0x5A00");
					streamWriter.WriteLine("#define    RegPllHf                0x5C00");
				}
				else
				{
					streamWriter.WriteLine("#define    RegTcxo                 0x4B00");
					streamWriter.WriteLine("#define    RegPaDac                0x4D00");
					streamWriter.WriteLine("#define    RegPllHf                0x7000");
				}

				streamWriter.WriteLine();
				streamWriter.WriteLine();

				if (rf92chip)
				{
					streamWriter.WriteLine("const unsigned int RegTable[17] = ");
					streamWriter.WriteLine("{");
					streamWriter.WriteLine("        " + ucLoRa.rfm92.RegFrMsb.Name + " + " + HexString(ucLoRa.rfm92.RegFrMsb.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm92.RegFrMid.Name + " + " + HexString(ucLoRa.rfm92.RegFrMid.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm92.RegFrLsb.Name + " + " + HexString(ucLoRa.rfm92.RegFrLsb.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm92.RegPaConfig.Name + " + " + HexString(ucLoRa.rfm92.RegPaConfig.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm92.RegPaRamp.Name + " + " + HexString(ucLoRa.rfm92.RegPaRamp.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm92.RegOcp.Name + " + " + HexString(ucLoRa.rfm92.RegOcp.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm92.RegLna.Name + " + " + HexString(ucLoRa.rfm92.RegLna.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm92.RegIrqFlagsMask.Name + " + " + HexString(ucLoRa.rfm92.RegIrqFlagsMask.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm92.RegModemConfig1.Name + " + " + HexString(ucLoRa.rfm92.RegModemConfig1.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm92.RegModemConfig2.Name + " + " + HexString(ucLoRa.rfm92.RegModemConfig2.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm92.RegSymbTimeoutLsb.Name + " + " + HexString(ucLoRa.rfm92.RegSymbTimeoutLsb.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm92.RegPreambleMsb.Name + " + " + HexString(ucLoRa.rfm92.RegPreambleMsb.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm92.RegPreambleLsb.Name + " + " + HexString(ucLoRa.rfm92.RegPreambleLsb.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm92.RegPayloadLength.Name + " + " + HexString(ucLoRa.rfm92.RegPayloadLength.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm92.RegTcxo.Name + " + " + HexString(ucLoRa.rfm92.RegTcxo.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm92.RegPaDac.Name + " + " + HexString(ucLoRa.rfm92.RegPaDac.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm92.RegPllHf.Name + " + " + HexString(ucLoRa.rfm92.RegPllHf.Value) + ";");
				}
				else
				{
					streamWriter.WriteLine("const unsigned int RegTable[18] = ");
					streamWriter.WriteLine("{");
					streamWriter.WriteLine("        " + ucLoRa.rfm96.RegFrMsb.Name + " + " + HexString(ucLoRa.rfm96.RegFrMsb.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm96.RegFrMid.Name + " + " + HexString(ucLoRa.rfm96.RegFrMid.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm96.RegFrLsb.Name + " + " + HexString(ucLoRa.rfm96.RegFrLsb.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm96.RegPaConfig.Name + " + " + HexString(ucLoRa.rfm96.RegPaConfig.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm96.RegPaRamp.Name + " + " + HexString(ucLoRa.rfm96.RegPaRamp.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm96.RegOcp.Name + " + " + HexString(ucLoRa.rfm96.RegOcp.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm96.RegLna.Name + " + " + HexString(ucLoRa.rfm96.RegLna.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm96.RegIrqFlagsMask.Name + " + " + HexString(ucLoRa.rfm96.RegIrqFlagsMask.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm96.RegModemConfig1.Name + " + " + HexString(ucLoRa.rfm96.RegModemConfig1.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm96.RegModemConfig2.Name + " + " + HexString(ucLoRa.rfm96.RegModemConfig2.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm96.RegModemConfig3.Name + " + " + HexString(ucLoRa.rfm96.RegModemConfig3.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm96.RegSymbTimeoutLsb.Name + " + " + HexString(ucLoRa.rfm96.RegSymbTimeoutLsb.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm96.RegPreambleMsb.Name + " + " + HexString(ucLoRa.rfm96.RegPreambleMsb.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm96.RegPreambleLsb.Name + " + " + HexString(ucLoRa.rfm96.RegPreambleLsb.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm96.RegPayloadLength.Name + " + " + HexString(ucLoRa.rfm96.RegPayloadLength.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm96.RegTcxo.Name + " + " + HexString(ucLoRa.rfm96.RegTcxo.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm96.RegPaDac.Name + " + " + HexString(ucLoRa.rfm96.RegPaDac.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa.rfm96.RegPllHf.Name + " + " + HexString(ucLoRa.rfm96.RegPllHf.Value) + ";");
				}
				streamWriter.WriteLine("};");
				streamWriter.Flush();
				streamWriter.Close();
			}
			saveFileDialog.Dispose();
		}
		#endregion

		private void aboutHopeRFToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int num = (int)new about().ShowDialog();
		}
	}
}

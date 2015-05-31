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
		private IContainer components;
		private MenuStrip menuStrip1;
		private ToolStripMenuItem fileToolStripMenuItem;
		private ToolStripMenuItem aboutToolStripMenuItem;
		private ToolStripMenuItem aboutHopeRFToolStripMenuItem;
		private ToolStripMenuItem openConfigToolStripMenuItem;
		private ToolStripMenuItem saveConfigFileToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripMenuItem exitToolStripMenuItem;
		private ToolStrip toolStrip1;
		private ToolStripButton tSBOpenFile;
		private ToolStripButton tSBSave;
		private ToolStripSeparator toolStripSeparator2;
		private ToolStripButton btnConnect;
		private ToolStripSeparator toolStripSeparator3;
		private ToolStripLabel toolStripLabel1;
		private ToolStripSeparator toolStripSeparator4;
		private ToolStripButton tSBReg;
		private StatusStrip statusStrip1;
		private ToolStripStatusLabel tSSLConnect;
		private ToolStripLed ConnectLed;
		private ucLoRa ucLoRa1;
		private ToolStripComboBox tscbChipVer;
		private ToolStripSeparator toolStripSeparator5;
		private ToolStripLabel ChipType;

		public MainForm()
		{
			InitializeComponent();
			tscbChipVer.SelectedIndex = 0;
			ucLoRa1.ChipVer = ucLoRa.ChipSet.RF96;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveConfigFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutHopeRFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
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
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.tSSLConnect = new System.Windows.Forms.ToolStripStatusLabel();
			this.ucLoRa1 = new LoRaModem.ucLoRa();
			this.menuStrip1.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.aboutToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(675, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openConfigToolStripMenuItem,
            this.saveConfigFileToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// openConfigToolStripMenuItem
			// 
			this.openConfigToolStripMenuItem.Name = "openConfigToolStripMenuItem";
			this.openConfigToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
			this.openConfigToolStripMenuItem.Text = "&Open Config File";
			this.openConfigToolStripMenuItem.Click += new System.EventHandler(this.openConfigToolStripMenuItem_Click);
			// 
			// saveConfigFileToolStripMenuItem
			// 
			this.saveConfigFileToolStripMenuItem.Name = "saveConfigFileToolStripMenuItem";
			this.saveConfigFileToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
			this.saveConfigFileToolStripMenuItem.Text = "&Save Config File";
			this.saveConfigFileToolStripMenuItem.Click += new System.EventHandler(this.saveConfigFileToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(160, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
			this.exitToolStripMenuItem.Text = "&Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutHopeRFToolStripMenuItem});
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
			this.aboutToolStripMenuItem.Text = "About";
			// 
			// aboutHopeRFToolStripMenuItem
			// 
			this.aboutHopeRFToolStripMenuItem.Name = "aboutHopeRFToolStripMenuItem";
			this.aboutHopeRFToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.aboutHopeRFToolStripMenuItem.Text = "&About HopeRF";
			this.aboutHopeRFToolStripMenuItem.Click += new System.EventHandler(this.aboutHopeRFToolStripMenuItem_Click);
			// 
			// toolStrip1
			// 
			this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
			this.toolStrip1.Location = new System.Drawing.Point(0, 24);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(675, 27);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// tSBOpenFile
			// 
			this.tSBOpenFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tSBOpenFile.Image = ((System.Drawing.Image)(resources.GetObject("tSBOpenFile.Image")));
			this.tSBOpenFile.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tSBOpenFile.Name = "tSBOpenFile";
			this.tSBOpenFile.Size = new System.Drawing.Size(23, 24);
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
			this.tSBSave.Size = new System.Drawing.Size(23, 24);
			this.tSBSave.Text = "toolStripButton1";
			this.tSBSave.ToolTipText = "save config file";
			this.tSBSave.Click += new System.EventHandler(this.tSBSave_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
			// 
			// btnConnect
			// 
			this.btnConnect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnConnect.Image = ((System.Drawing.Image)(resources.GetObject("btnConnect.Image")));
			this.btnConnect.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnConnect.Name = "btnConnect";
			this.btnConnect.Size = new System.Drawing.Size(23, 24);
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
			this.ConnectLed.Size = new System.Drawing.Size(15, 24);
			this.ConnectLed.Text = "toolStripLed1";
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 27);
			// 
			// toolStripLabel1
			// 
			this.toolStripLabel1.Name = "toolStripLabel1";
			this.toolStripLabel1.Size = new System.Drawing.Size(81, 24);
			this.toolStripLabel1.Text = "Modem: LoRa";
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(6, 27);
			// 
			// tSBReg
			// 
			this.tSBReg.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tSBReg.Image = ((System.Drawing.Image)(resources.GetObject("tSBReg.Image")));
			this.tSBReg.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tSBReg.Name = "tSBReg";
			this.tSBReg.Size = new System.Drawing.Size(23, 24);
			this.tSBReg.Text = "toolStripButton2";
			this.tSBReg.ToolTipText = "View";
			this.tSBReg.Click += new System.EventHandler(this.tSBReg_Click);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(6, 27);
			// 
			// ChipType
			// 
			this.ChipType.Name = "ChipType";
			this.ChipType.Size = new System.Drawing.Size(58, 24);
			this.ChipType.Text = "ChipType";
			// 
			// tscbChipVer
			// 
			this.tscbChipVer.Items.AddRange(new object[] {
            "RFM95/96/97/98",
            "RFM92/93"});
			this.tscbChipVer.Name = "tscbChipVer";
			this.tscbChipVer.Size = new System.Drawing.Size(121, 27);
			this.tscbChipVer.Text = "RFM95/96/97/98";
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tSSLConnect});
			this.statusStrip1.Location = new System.Drawing.Point(0, 618);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.statusStrip1.Size = new System.Drawing.Size(675, 22);
			this.statusStrip1.TabIndex = 2;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// tSSLConnect
			// 
			this.tSSLConnect.Name = "tSSLConnect";
			this.tSSLConnect.Size = new System.Drawing.Size(0, 17);
			// 
			// ucLoRa1
			// 
			this.ucLoRa1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ucLoRa1.FrequencyRf = new decimal(new int[] {
            915999973,
            0,
            0,
            0});
			this.ucLoRa1.FrequencyStep = new decimal(new int[] {
            61,
            0,
            0,
            0});
			this.ucLoRa1.Location = new System.Drawing.Point(0, 51);
			this.ucLoRa1.Name = "ucLoRa1";
			this.ucLoRa1.Size = new System.Drawing.Size(675, 567);
			this.ucLoRa1.TabIndex = 0;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(675, 640);
			this.Controls.Add(this.ucLoRa1);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.menuStrip1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip1;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "RFM9x_GUI V1.1";
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
		{
		}

		private void tSBReg_Click(object sender, EventArgs e)
		{
			RegTable regTable = new RegTable();
			if (ucLoRa1.ChipVer == ucLoRa.ChipSet.RF92)
			{
				regTable.tbRegOpMode.Text = toHexString(ucLoRa1.rfm92.RegOpMode.Value);
				regTable.tbRegFrMsb.Text = toHexString(ucLoRa1.rfm92.RegFrMsb.Value);
				regTable.tbRegFrMid.Text = toHexString(ucLoRa1.rfm92.RegFrMid.Value);
				regTable.tbRegFrLsb.Text = toHexString(ucLoRa1.rfm92.RegFrLsb.Value);
				regTable.tbRegPaConfig.Text = toHexString(ucLoRa1.rfm92.RegPaConfig.Value);
				regTable.tbRegPaRamp.Text = toHexString(ucLoRa1.rfm92.RegPaRamp.Value);
				regTable.tbRegOcp.Text = toHexString(ucLoRa1.rfm92.RegOcp.Value);
				regTable.tbRegLna.Text = toHexString(ucLoRa1.rfm92.RegLna.Value);
				regTable.tbRegIrqFlagsMask.Text = toHexString(ucLoRa1.rfm92.RegIrqFlagsMask.Value);
				regTable.tbRegModemConfig1.Text = toHexString(ucLoRa1.rfm92.RegModemConfig1.Value);
				regTable.tbRegModemConfig2.Text = toHexString(ucLoRa1.rfm92.RegModemConfig2.Value);
				regTable.tbRegSymbTimeoutLsb.Text = toHexString(ucLoRa1.rfm92.RegSymbTimeoutLsb.Value);
				regTable.tbRegPreambleMsb.Text = toHexString(ucLoRa1.rfm92.RegPreambleMsb.Value);
				regTable.tbRegPreambleLsb.Text = toHexString(ucLoRa1.rfm92.RegPreambleLsb.Value);
				regTable.tbRegModemConfig3.Text = "--";
				regTable.tbRegTcxo.Text = toHexString(ucLoRa1.rfm92.RegTcxo.Value);
				regTable.tbRegPaDac.Text = toHexString(ucLoRa1.rfm92.RegPaDac.Value);
				regTable.tbRegPllHf.Text = toHexString(ucLoRa1.rfm92.RegPllHf.Value);
			}
			else
			{
				regTable.tbRegOpMode.Text = toHexString(ucLoRa1.rfm96.RegOpMode.Value);
				regTable.tbRegFrMsb.Text = toHexString(ucLoRa1.rfm96.RegFrMsb.Value);
				regTable.tbRegFrMid.Text = toHexString(ucLoRa1.rfm96.RegFrMid.Value);
				regTable.tbRegFrLsb.Text = toHexString(ucLoRa1.rfm96.RegFrLsb.Value);
				regTable.tbRegPaConfig.Text = toHexString(ucLoRa1.rfm96.RegPaConfig.Value);
				regTable.tbRegPaRamp.Text = toHexString(ucLoRa1.rfm96.RegPaRamp.Value);
				regTable.tbRegOcp.Text = toHexString(ucLoRa1.rfm96.RegOcp.Value);
				regTable.tbRegLna.Text = toHexString(ucLoRa1.rfm96.RegLna.Value);
				regTable.tbRegIrqFlagsMask.Text = toHexString(ucLoRa1.rfm96.RegIrqFlagsMask.Value);
				regTable.tbRegModemConfig1.Text = toHexString(ucLoRa1.rfm96.RegModemConfig1.Value);
				regTable.tbRegModemConfig2.Text = toHexString(ucLoRa1.rfm96.RegModemConfig2.Value);
				regTable.tbRegSymbTimeoutLsb.Text = toHexString(ucLoRa1.rfm96.RegSymbTimeoutLsb.Value);
				regTable.tbRegPreambleMsb.Text = toHexString(ucLoRa1.rfm96.RegPreambleMsb.Value);
				regTable.tbRegPreambleLsb.Text = toHexString(ucLoRa1.rfm96.RegPreambleLsb.Value);
				regTable.tbRegModemConfig3.Text = toHexString(ucLoRa1.rfm96.RegModemConfig3.Value);
				regTable.tbRegTcxo.Text = toHexString(ucLoRa1.rfm96.RegTcxo.Value);
				regTable.tbRegPaDac.Text = toHexString(ucLoRa1.rfm96.RegPaDac.Value);
				regTable.tbRegPllHf.Text = toHexString(ucLoRa1.rfm96.RegPllHf.Value);
			}
			regTable.Show();
		}

		private string toHexString(byte xd)
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

		private byte StringToHex(string str)
		{
			byte num1 = (byte)0;
			if ((int)str[2] >= 48 && (int)str[2] <= 57)
				num1 = (byte)((uint)str[2] - 48U);
			else if ((int)str[2] >= 65 && (int)str[2] <= 70)
			{
				switch (str[2])
				{
					case 'A':
						num1 = (byte)10;
						break;
					case 'B':
						num1 = (byte)11;
						break;
					case 'C':
						num1 = (byte)12;
						break;
					case 'D':
						num1 = (byte)13;
						break;
					case 'E':
						num1 = (byte)14;
						break;
					case 'F':
						num1 = (byte)15;
						break;
				}
			}
			else if ((int)str[2] >= 97 && (int)str[2] <= 102)
			{
				switch (str[2])
				{
					case 'a':
						num1 = (byte)10;
						break;
					case 'b':
						num1 = (byte)11;
						break;
					case 'c':
						num1 = (byte)12;
						break;
					case 'd':
						num1 = (byte)13;
						break;
					case 'e':
						num1 = (byte)14;
						break;
					case 'f':
						num1 = (byte)15;
						break;
				}
			}
			byte num2 = (byte)((uint)num1 << 4);
			if ((int)str[3] >= 48 && (int)str[3] <= 57)
				num2 |= (byte)((uint)str[3] - 48U);
			else if ((int)str[3] >= 65 && (int)str[3] <= 70)
			{
				switch (str[3])
				{
					case 'A':
						num2 |= (byte)10;
						break;
					case 'B':
						num2 |= (byte)11;
						break;
					case 'C':
						num2 |= (byte)12;
						break;
					case 'D':
						num2 |= (byte)13;
						break;
					case 'E':
						num2 |= (byte)14;
						break;
					case 'F':
						num2 |= (byte)15;
						break;
				}
			}
			else if ((int)str[3] >= 97 && (int)str[3] <= 102)
			{
				switch (str[3])
				{
					case 'a':
						num2 |= (byte)10;
						break;
					case 'b':
						num2 |= (byte)11;
						break;
					case 'c':
						num2 |= (byte)12;
						break;
					case 'd':
						num2 |= (byte)13;
						break;
					case 'e':
						num2 |= (byte)14;
						break;
					case 'f':
						num2 |= (byte)15;
						break;
				}
			}
			return num2;
		}

		private void btnConnect_Click(object sender, EventArgs e)
		{
			if (ConnectLed.LedColor == Color.Red)
			{
				if (ucLoRa1.TryConnect())
				{
					ucLoRa1.gbOpMode.Enabled = true;
					ConnectLed.LedColor = Color.Lime;
					tscbChipVer.Enabled = false;
					if (ucLoRa1.ChipVer == ucLoRa.ChipSet.RF92)
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
				ucLoRa1.ftdi.LCDClear();
				ucLoRa1.ftdi.Close();
				tscbChipVer.Enabled = true;
				ucLoRa1.gbOpMode.Enabled = false;
				ConnectLed.LedColor = Color.Red;
			}
		}

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

		private void OpenCfgFile()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Title = "OpenCfgFile";
			openFileDialog.Filter = "HopeRF RF-Config Files(*.hpflr)|*.hpflr";
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				StreamReader streamReader = new StreamReader(openFileDialog.FileName);
				string str1 = "";
				while (str1 != null)
				{
					str1 = streamReader.ReadLine();
					if (str1 != "" && str1.Contains("Chipset Type"))
						break;
				}
				if (str1 == null)
				{
					streamReader.Close();
					MessageBox.Show("File format is wrong, does not recognize the chip models!");
				}
				else
				{
					ucLoRa.ChipSet chipSet = !str1.Contains("RF92") ? ucLoRa.ChipSet.RF96 : ucLoRa.ChipSet.RF92;
					if (ConnectLed.LedColor == Color.Lime)
					{
						if (ucLoRa1.ChipVer != chipSet)
						{
							streamReader.Close();
							MessageBox.Show("File type does not match with the hardware connection type!");
							openFileDialog.Dispose();
							return;
						}
					}
					else if (chipSet == ucLoRa.ChipSet.RF92)
					{
						ucLoRa1.ChipVer = ucLoRa.ChipSet.RF92;
						tscbChipVer.SelectedIndex = 1;
					}
					else
					{
						ucLoRa1.ChipVer = ucLoRa.ChipSet.RF96;
						tscbChipVer.SelectedIndex = 0;
					}
					while (str1 != null)
					{
						str1 = streamReader.ReadLine();
						if (str1 != "" && str1.Contains("const unsigned int RegTable"))
							break;
					}
					if (str1 == null)
					{
						streamReader.Close();
						MessageBox.Show("File format is wrong, does not recognize the register values!");
					}
					else
					{
						string str2 = streamReader.ReadLine();
						if (chipSet == ucLoRa.ChipSet.RF92)
						{
							for (; str2 != null; str2 = streamReader.ReadLine())
							{
								string str3 = "";
								string[] strArray1 = new string[5];
								char[] chArray = new char[2]
                {
                  '0',
                  'x'
                };
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
											ucLoRa1.rfm92.RegFrMsb.Value = num2;
											continue;
										case (byte)1:
											ucLoRa1.rfm92.RegFrMid.Value = num2;
											continue;
										case (byte)2:
											ucLoRa1.rfm92.RegFrLsb.Value = num2;
											continue;
										case (byte)3:
											ucLoRa1.rfm92.RegPaConfig.Value = num2;
											continue;
										case (byte)4:
											ucLoRa1.rfm92.RegPaRamp.Value = num2;
											continue;
										case (byte)5:
											ucLoRa1.rfm92.RegOcp.Value = num2;
											continue;
										case (byte)6:
											ucLoRa1.rfm92.RegLna.Value = num2;
											continue;
										case (byte)7:
											ucLoRa1.rfm92.RegIrqFlagsMask.Value = num2;
											continue;
										case (byte)8:
											ucLoRa1.rfm92.RegModemConfig1.Value = num2;
											continue;
										case (byte)9:
											ucLoRa1.rfm92.RegModemConfig2.Value = num2;
											continue;
										case (byte)10:
											ucLoRa1.rfm92.RegSymbTimeoutLsb.Value = num2;
											continue;
										case (byte)11:
											ucLoRa1.rfm92.RegPreambleMsb.Value = num2;
											continue;
										case (byte)12:
											ucLoRa1.rfm92.RegPreambleLsb.Value = num2;
											continue;
										case (byte)13:
											ucLoRa1.rfm92.RegPayloadLength.Value = num2;
											continue;
										case (byte)14:
											ucLoRa1.rfm92.RegTcxo.Value = num2;
											continue;
										case (byte)15:
											ucLoRa1.rfm92.RegPaDac.Value = num2;
											continue;
										case (byte)16:
											ucLoRa1.rfm92.RegPllHf.Value = num2;
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
								char[] chArray = new char[2]
                {
                  '0',
                  'x'
                };
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
											ucLoRa1.rfm96.RegFrMsb.Value = num2;
											continue;
										case (byte)1:
											ucLoRa1.rfm96.RegFrMid.Value = num2;
											continue;
										case (byte)2:
											ucLoRa1.rfm96.RegFrLsb.Value = num2;
											continue;
										case (byte)3:
											ucLoRa1.rfm96.RegPaConfig.Value = num2;
											continue;
										case (byte)4:
											ucLoRa1.rfm96.RegPaRamp.Value = num2;
											continue;
										case (byte)5:
											ucLoRa1.rfm96.RegOcp.Value = num2;
											continue;
										case (byte)6:
											ucLoRa1.rfm96.RegLna.Value = num2;
											continue;
										case (byte)7:
											ucLoRa1.rfm96.RegIrqFlagsMask.Value = num2;
											continue;
										case (byte)8:
											ucLoRa1.rfm96.RegModemConfig1.Value = num2;
											continue;
										case (byte)9:
											ucLoRa1.rfm96.RegModemConfig2.Value = num2;
											continue;
										case (byte)10:
											ucLoRa1.rfm96.RegSymbTimeoutLsb.Value = num2;
											continue;
										case (byte)11:
											ucLoRa1.rfm96.RegPreambleMsb.Value = num2;
											continue;
										case (byte)12:
											ucLoRa1.rfm96.RegPreambleLsb.Value = num2;
											continue;
										case (byte)13:
											ucLoRa1.rfm96.RegPayloadLength.Value = num2;
											continue;
										case (byte)14:
											ucLoRa1.rfm96.RegTcxo.Value = num2;
											continue;
										case (byte)15:
											ucLoRa1.rfm96.RegPaDac.Value = num2;
											continue;
										case (byte)16:
											ucLoRa1.rfm96.RegPllHf.Value = num2;
											continue;
										case (byte)17:
											ucLoRa1.rfm96.RegModemConfig3.Value = num2;
											continue;
										default:
											continue;
									}
								}
							}
						}
						streamReader.Close();
						ucLoRa1.SetcbBW();
						ucLoRa1.SetAllValue();
					}
				}
			}
			openFileDialog.Dispose();
		}

		private void SaveCfgFile()
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Title = "SaveCfgFile";
			saveFileDialog.Filter = "HopeRF files(*.hpflr)|*.hpflr";
			saveFileDialog.CreatePrompt = true;
			saveFileDialog.OverwritePrompt = true;
			ucLoRa1.UpdataAllValue();
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
				bool flag;
				if (ucLoRa1.ChipVer == ucLoRa.ChipSet.RF92)
				{
					streamWriter.WriteLine("Chipset Type: RF92/RF93");
					flag = true;
				}
				else
				{
					streamWriter.WriteLine("Chipset Type: RF95/RF96/RF97/RF98");
					flag = false;
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
				if (flag)
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
				streamWriter.WriteLine("");
				streamWriter.WriteLine("");
				if (flag)
				{
					streamWriter.WriteLine("const unsigned int RegTable[17] = ");
					streamWriter.WriteLine("{");
					streamWriter.WriteLine("        " + ucLoRa1.rfm92.RegFrMsb.Name + " + " + toHexString(ucLoRa1.rfm92.RegFrMsb.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm92.RegFrMid.Name + " + " + toHexString(ucLoRa1.rfm92.RegFrMid.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm92.RegFrLsb.Name + " + " + toHexString(ucLoRa1.rfm92.RegFrLsb.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm92.RegPaConfig.Name + " + " + toHexString(ucLoRa1.rfm92.RegPaConfig.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm92.RegPaRamp.Name + " + " + toHexString(ucLoRa1.rfm92.RegPaRamp.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm92.RegOcp.Name + " + " + toHexString(ucLoRa1.rfm92.RegOcp.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm92.RegLna.Name + " + " + toHexString(ucLoRa1.rfm92.RegLna.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm92.RegIrqFlagsMask.Name + " + " + toHexString(ucLoRa1.rfm92.RegIrqFlagsMask.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm92.RegModemConfig1.Name + " + " + toHexString(ucLoRa1.rfm92.RegModemConfig1.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm92.RegModemConfig2.Name + " + " + toHexString(ucLoRa1.rfm92.RegModemConfig2.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm92.RegSymbTimeoutLsb.Name + " + " + toHexString(ucLoRa1.rfm92.RegSymbTimeoutLsb.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm92.RegPreambleMsb.Name + " + " + toHexString(ucLoRa1.rfm92.RegPreambleMsb.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm92.RegPreambleLsb.Name + " + " + toHexString(ucLoRa1.rfm92.RegPreambleLsb.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm92.RegPayloadLength.Name + " + " + toHexString(ucLoRa1.rfm92.RegPayloadLength.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm92.RegTcxo.Name + " + " + toHexString(ucLoRa1.rfm92.RegTcxo.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm92.RegPaDac.Name + " + " + toHexString(ucLoRa1.rfm92.RegPaDac.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm92.RegPllHf.Name + " + " + toHexString(ucLoRa1.rfm92.RegPllHf.Value) + ";");
				}
				else
				{
					streamWriter.WriteLine("const unsigned int RegTable[18] = ");
					streamWriter.WriteLine("{");
					streamWriter.WriteLine("        " + ucLoRa1.rfm96.RegFrMsb.Name + " + " + toHexString(ucLoRa1.rfm96.RegFrMsb.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm96.RegFrMid.Name + " + " + toHexString(ucLoRa1.rfm96.RegFrMid.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm96.RegFrLsb.Name + " + " + toHexString(ucLoRa1.rfm96.RegFrLsb.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm96.RegPaConfig.Name + " + " + toHexString(ucLoRa1.rfm96.RegPaConfig.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm96.RegPaRamp.Name + " + " + toHexString(ucLoRa1.rfm96.RegPaRamp.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm96.RegOcp.Name + " + " + toHexString(ucLoRa1.rfm96.RegOcp.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm96.RegLna.Name + " + " + toHexString(ucLoRa1.rfm96.RegLna.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm96.RegIrqFlagsMask.Name + " + " + toHexString(ucLoRa1.rfm96.RegIrqFlagsMask.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm96.RegModemConfig1.Name + " + " + toHexString(ucLoRa1.rfm96.RegModemConfig1.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm96.RegModemConfig2.Name + " + " + toHexString(ucLoRa1.rfm96.RegModemConfig2.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm96.RegModemConfig3.Name + " + " + toHexString(ucLoRa1.rfm96.RegModemConfig3.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm96.RegSymbTimeoutLsb.Name + " + " + toHexString(ucLoRa1.rfm96.RegSymbTimeoutLsb.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm96.RegPreambleMsb.Name + " + " + toHexString(ucLoRa1.rfm96.RegPreambleMsb.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm96.RegPreambleLsb.Name + " + " + toHexString(ucLoRa1.rfm96.RegPreambleLsb.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm96.RegPayloadLength.Name + " + " + toHexString(ucLoRa1.rfm96.RegPayloadLength.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm96.RegTcxo.Name + " + " + toHexString(ucLoRa1.rfm96.RegTcxo.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm96.RegPaDac.Name + " + " + toHexString(ucLoRa1.rfm96.RegPaDac.Value) + ";");
					streamWriter.WriteLine("        " + ucLoRa1.rfm96.RegPllHf.Name + " + " + toHexString(ucLoRa1.rfm96.RegPllHf.Value) + ";");
				}
				streamWriter.WriteLine("};");
				streamWriter.Flush();
				streamWriter.Close();
			}
			saveFileDialog.Dispose();
		}

		private void aboutHopeRFToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int num = (int)new about().ShowDialog();
		}
	}
}
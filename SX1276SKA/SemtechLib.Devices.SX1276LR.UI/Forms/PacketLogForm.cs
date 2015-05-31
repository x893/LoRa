using SemtechLib.Devices.Common.Interfaces;
using SemtechLib.Devices.SX1276LR;
using SemtechLib.Devices.SX1276LR.General;
using SemtechLib.General;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SemtechLib.Devices.SX1276LR.UI.Forms
{
	public class PacketLogForm : Form
	{
		private delegate void DeviceDataChangedDelegate(object sender, PropertyChangedEventArgs e);

		private ApplicationSettings appSettings;
		private IDevice device;
		private IContainer components;
		private Button btnLogBrowseFile;
		private Button btnClose;
		private SaveFileDialog sfLogSaveFileDlg;
		private Label label1;
		private Label lblFileName;
		private Label label3;
		private RadioButton rBtnFileModeAppendOn;
		private RadioButton rBtnFileModeAppendOff;
		private Panel panel1;
		private Panel panel13;
		private RadioButton rBtnLogOff;
		private RadioButton rBtnLogOn;
		private Label label2;

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
			set
			{
				if (this.device == value)
					return;
				this.device = value;
				((SX1276LR)this.device).PacketHandlerLog.PropertyChanged += new PropertyChangedEventHandler(this.log_PropertyChanged);
				this.rBtnLogOn.Checked = ((SX1276LR)this.device).PacketHandlerLog.Enabled;
				this.rBtnLogOff.Checked = !((SX1276LR)this.device).PacketHandlerLog.Enabled;
				this.rBtnFileModeAppendOn.Checked = ((SX1276LR)this.device).PacketHandlerLog.IsAppend;
				this.rBtnFileModeAppendOff.Checked = !((SX1276LR)this.device).PacketHandlerLog.IsAppend;
				this.lblFileName.Text = ((SX1276LR)this.device).PacketHandlerLog.FileName;
			}
		}

		public PacketLogForm()
		{
			this.InitializeComponent();
		}

		private bool IsFormLocatedInScreen(Form frm, Screen[] screens)
		{
			int upperBound = screens.GetUpperBound(0);
			bool flag = false;
			for (int index = 0; index <= upperBound; ++index)
			{
				if (frm.Left < screens[index].WorkingArea.Left || frm.Top < screens[index].WorkingArea.Top || (frm.Left > screens[index].WorkingArea.Right || frm.Top > screens[index].WorkingArea.Bottom))
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

		private void OnError(byte status, string message)
		{
			this.Refresh();
		}

		private void log_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "FileName":
					this.lblFileName.Text = ((SX1276LR)this.device).PacketHandlerLog.FileName;
					break;
			}
		}

		private void PacketLogForm_Load(object sender, EventArgs e)
		{
			try
			{
				string s1 = this.appSettings.GetValue("PacketLogTop");
				if (s1 != null)
				{
					try
					{
						this.Top = int.Parse(s1);
					}
					catch
					{
						int num = (int)MessageBox.Show((IWin32Window)this, "Error getting Top value.");
					}
				}
				string s2 = this.appSettings.GetValue("PacketLogLeft");
				if (s2 != null)
				{
					try
					{
						this.Left = int.Parse(s2);
					}
					catch
					{
						int num = (int)MessageBox.Show((IWin32Window)this, "Error getting Left value.");
					}
				}
				Screen[] allScreens = Screen.AllScreens;
				if (!this.IsFormLocatedInScreen((Form)this, allScreens))
				{
					this.Top = allScreens[0].WorkingArea.Top;
					this.Left = allScreens[0].WorkingArea.Left;
				}
				this.rBtnLogOn.Checked = ((SX1276LR)this.device).PacketHandlerLog.Enabled;
				this.rBtnLogOff.Checked = !((SX1276LR)this.device).PacketHandlerLog.Enabled;
				this.rBtnFileModeAppendOn.Checked = ((SX1276LR)this.device).PacketHandlerLog.IsAppend;
				this.rBtnFileModeAppendOff.Checked = !((SX1276LR)this.device).PacketHandlerLog.IsAppend;
				this.lblFileName.Text = ((SX1276LR)this.device).PacketHandlerLog.FileName;
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
		}

		private void PacketLogForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			try
			{
				this.appSettings.SetValue("PacketLogTop", this.Top.ToString());
				this.appSettings.SetValue("PacketLogLeft", this.Left.ToString());
				this.appSettings.SetValue("PacketLogPath", ((SX1276LR)this.device).PacketHandlerLog.Path);
				this.appSettings.SetValue("PacketLogFileName", ((SX1276LR)this.device).PacketHandlerLog.FileName);
				this.appSettings.SetValue("PacketLogMaxSamples", ((SX1276LR)this.device).PacketHandlerLog.MaxSamples.ToString());
				this.appSettings.SetValue("PacketLogIsAppend", ((SX1276LR)this.device).PacketHandlerLog.IsAppend.ToString());
				this.appSettings.SetValue("PacketLogEnabled", ((SX1276LR)this.device).PacketHandlerLog.Enabled.ToString());
			}
			catch (Exception)
			{
			}
		}

		private void btnLogBrowseFile_Click(object sender, EventArgs e)
		{
			this.OnError((byte)0, "-");
			try
			{
				this.sfLogSaveFileDlg.InitialDirectory = ((SX1276LR)this.device).PacketHandlerLog.Path;
				this.sfLogSaveFileDlg.FileName = ((SX1276LR)this.device).PacketHandlerLog.FileName;
				if (this.sfLogSaveFileDlg.ShowDialog() != DialogResult.OK)
					return;
				string[] strArray = this.sfLogSaveFileDlg.FileName.Split('\\');
				((SX1276LR)this.device).PacketHandlerLog.FileName = strArray[strArray.Length - 1];
				((SX1276LR)this.device).PacketHandlerLog.Path = "";
				int index;
				for (index = 0; index < strArray.Length - 2; ++index)
				{
					ILog packetHandlerLog = ((SX1276LR)this.device).PacketHandlerLog;
					string str = packetHandlerLog.Path + strArray[index] + "\\";
					packetHandlerLog.Path = str;
				}
				((SX1276LR)this.device).PacketHandlerLog.Path += strArray[index];
			}
			catch (Exception ex)
			{
				this.OnError((byte)1, ex.Message);
			}
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void rBtnFileModeAppendOn_CheckedChanged(object sender, EventArgs e)
		{
			((SX1276LR)this.device).PacketHandlerLog.IsAppend = this.rBtnFileModeAppendOn.Checked;
		}

		private void rBtnFileModeAppendOff_CheckedChanged(object sender, EventArgs e)
		{
			((SX1276LR)this.device).PacketHandlerLog.IsAppend = this.rBtnFileModeAppendOn.Checked;
		}

		private void rBtnLogOn_CheckedChanged(object sender, EventArgs e)
		{
			((SX1276LR)this.device).PacketHandlerLog.Enabled = this.rBtnLogOn.Checked;
		}

		private void rBtnLogOff_CheckedChanged(object sender, EventArgs e)
		{
			((SX1276LR)this.device).PacketHandlerLog.Enabled = this.rBtnLogOn.Checked;
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
			ComponentResourceManager resources = new ComponentResourceManager(typeof(PacketLogForm));
			this.btnLogBrowseFile = new Button();
			this.btnClose = new Button();
			this.sfLogSaveFileDlg = new SaveFileDialog();
			this.label1 = new Label();
			this.lblFileName = new Label();
			this.label3 = new Label();
			this.rBtnFileModeAppendOn = new RadioButton();
			this.rBtnFileModeAppendOff = new RadioButton();
			this.panel1 = new Panel();
			this.panel13 = new Panel();
			this.rBtnLogOff = new RadioButton();
			this.rBtnLogOn = new RadioButton();
			this.label2 = new Label();
			this.panel1.SuspendLayout();
			this.panel13.SuspendLayout();
			this.SuspendLayout();
			this.btnLogBrowseFile.Location = new Point(318, 34);
			this.btnLogBrowseFile.Name = "btnLogBrowseFile";
			this.btnLogBrowseFile.Size = new Size(75, 23);
			this.btnLogBrowseFile.TabIndex = 4;
			this.btnLogBrowseFile.Text = "Browse...";
			this.btnLogBrowseFile.UseVisualStyleBackColor = true;
			this.btnLogBrowseFile.Click += new EventHandler(this.btnLogBrowseFile_Click);
			this.btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			this.btnClose.Location = new Point(166, 117);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new Size(75, 23);
			this.btnClose.TabIndex = 7;
			this.btnClose.Text = "OK";
			this.btnClose.UseVisualStyleBackColor = true;
			this.btnClose.Click += new EventHandler(this.btnClose_Click);
			this.sfLogSaveFileDlg.DefaultExt = "*.log";
			this.sfLogSaveFileDlg.Filter = "Log Files(*.log)|*.log|AllFiles(*.*)|*.*";
			this.label1.AutoSize = true;
			this.label1.Location = new Point(11, 39);
			this.label1.Name = "label1";
			this.label1.Size = new Size(26, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "File:";
			this.lblFileName.BorderStyle = BorderStyle.Fixed3D;
			this.lblFileName.Location = new Point(66, 35);
			this.lblFileName.Name = "lblFileName";
			this.lblFileName.Size = new Size(246, 20);
			this.lblFileName.TabIndex = 3;
			this.lblFileName.Text = "-";
			this.lblFileName.TextAlign = ContentAlignment.MiddleLeft;
			this.label3.AutoSize = true;
			this.label3.Location = new Point(11, 75);
			this.label3.Name = "label3";
			this.label3.Size = new Size(37, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Mode:";
			this.rBtnFileModeAppendOn.AutoSize = true;
			this.rBtnFileModeAppendOn.Checked = true;
			this.rBtnFileModeAppendOn.Location = new Point(3, 3);
			this.rBtnFileModeAppendOn.Name = "rBtnFileModeAppendOn";
			this.rBtnFileModeAppendOn.Size = new Size(126, 17);
			this.rBtnFileModeAppendOn.TabIndex = 0;
			this.rBtnFileModeAppendOn.TabStop = true;
			this.rBtnFileModeAppendOn.Text = "Append to current file";
			this.rBtnFileModeAppendOn.UseVisualStyleBackColor = true;
			this.rBtnFileModeAppendOn.CheckedChanged += new EventHandler(this.rBtnFileModeAppendOn_CheckedChanged);
			this.rBtnFileModeAppendOff.AutoSize = true;
			this.rBtnFileModeAppendOff.Location = new Point(3, 26);
			this.rBtnFileModeAppendOff.Name = "rBtnFileModeAppendOff";
			this.rBtnFileModeAppendOff.Size = new Size(144, 17);
			this.rBtnFileModeAppendOff.TabIndex = 1;
			this.rBtnFileModeAppendOff.Text = "Create new file each time";
			this.rBtnFileModeAppendOff.UseVisualStyleBackColor = true;
			this.rBtnFileModeAppendOff.CheckedChanged += new EventHandler(this.rBtnFileModeAppendOff_CheckedChanged);
			this.panel1.AutoSize = true;
			this.panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel1.Controls.Add((Control)this.rBtnFileModeAppendOn);
			this.panel1.Controls.Add((Control)this.rBtnFileModeAppendOff);
			this.panel1.Location = new Point(66, 58);
			this.panel1.Name = "panel1";
			this.panel1.Size = new Size(150, 46);
			this.panel1.TabIndex = 6;
			this.panel13.AutoSize = true;
			this.panel13.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel13.Controls.Add((Control)this.rBtnLogOff);
			this.panel13.Controls.Add((Control)this.rBtnLogOn);
			this.panel13.Location = new Point(66, 12);
			this.panel13.Name = "panel13";
			this.panel13.Size = new Size(102, 20);
			this.panel13.TabIndex = 1;
			this.rBtnLogOff.AutoSize = true;
			this.rBtnLogOff.Checked = true;
			this.rBtnLogOff.Location = new Point(54, 3);
			this.rBtnLogOff.Margin = new Padding(3, 0, 3, 0);
			this.rBtnLogOff.Name = "rBtnLogOff";
			this.rBtnLogOff.Size = new Size(45, 17);
			this.rBtnLogOff.TabIndex = 1;
			this.rBtnLogOff.TabStop = true;
			this.rBtnLogOff.Text = "OFF";
			this.rBtnLogOff.UseVisualStyleBackColor = true;
			this.rBtnLogOff.CheckedChanged += new EventHandler(this.rBtnLogOff_CheckedChanged);
			this.rBtnLogOn.AutoSize = true;
			this.rBtnLogOn.Location = new Point(3, 3);
			this.rBtnLogOn.Margin = new Padding(3, 0, 3, 0);
			this.rBtnLogOn.Name = "rBtnLogOn";
			this.rBtnLogOn.Size = new Size(41, 17);
			this.rBtnLogOn.TabIndex = 0;
			this.rBtnLogOn.Text = "ON";
			this.rBtnLogOn.UseVisualStyleBackColor = true;
			this.rBtnLogOn.CheckedChanged += new EventHandler(this.rBtnLogOn_CheckedChanged);
			this.label2.AutoSize = true;
			this.label2.Location = new Point(11, 16);
			this.label2.Name = "label2";
			this.label2.Size = new Size(28, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Log:";
			this.AcceptButton = (IButtonControl)this.btnClose;
			this.AutoScaleDimensions = new SizeF(6f, 13f);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.ClientSize = new Size(406, 152);
			this.Controls.Add((Control)this.panel13);
			this.Controls.Add((Control)this.panel1);
			this.Controls.Add((Control)this.lblFileName);
			this.Controls.Add((Control)this.label3);
			this.Controls.Add((Control)this.label2);
			this.Controls.Add((Control)this.label1);
			this.Controls.Add((Control)this.btnLogBrowseFile);
			this.Controls.Add((Control)this.btnClose);
			this.DoubleBuffered = true;
			this.Icon = (Icon)resources.GetObject("$this.Icon");
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.Name = "PacketLogForm";
			this.Opacity = 0.9;
			this.Text = "Packet log settings";
			this.FormClosed += new FormClosedEventHandler(this.PacketLogForm_FormClosed);
			this.Load += new EventHandler(this.PacketLogForm_Load);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel13.ResumeLayout(false);
			this.panel13.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
	}
}
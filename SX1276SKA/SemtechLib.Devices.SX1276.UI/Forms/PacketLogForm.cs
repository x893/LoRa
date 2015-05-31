using SemtechLib.Devices.Common.Interfaces;
using SemtechLib.Devices.SX1276.General;
using SemtechLib.General;
using SemtechLib.General.Events;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SemtechLib.Devices.SX1276.UI.Forms
{
	public class PacketLogForm : Form
	{
		private delegate void DeviceDataChangedDelegate(object sender, PropertyChangedEventArgs e);

		private int tickStart = Environment.TickCount;
		private PacketLog log = new PacketLog();
		private ApplicationSettings appSettings;
		private IDevice device;
		private string previousValue;
		private IContainer components;
		private GroupBox groupBox5;
		private Button btnLogBrowseFile;
		private ProgressBar pBarLog;
		private TableLayoutPanel tableLayoutPanel3;
		private TextBox tBoxLogMaxSamples;
		private Label lblCommandsLogMaxSamples;
		private CheckBox cBtnLogOnOff;
		private Button btnClose;
		private SaveFileDialog sfLogSaveFileDlg;

		public ApplicationSettings AppSettings
		{
			get
			{
				return appSettings;
			}
			set
			{
				appSettings = value;
			}
		}

		public IDevice Device
		{
			set
			{
				if (device == value)
					return;
				device = value;
				Log.Device = device;
			}
		}

		public PacketLog Log
		{
			get
			{
				return log;
			}
		}

		public PacketLogForm()
		{
			InitializeComponent();
			log.PropertyChanged += new PropertyChangedEventHandler(log_PropertyChanged);
			log.Stoped += new EventHandler(log_Stoped);
			log.ProgressChanged += new ProgressEventHandler(log_ProgressChanged);
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

		private void UpdateProgressBarStyle()
		{
			if ((long)log.MaxSamples == 0L && cBtnLogOnOff.Checked)
				pBarLog.Style = ProgressBarStyle.Marquee;
			else
				pBarLog.Style = ProgressBarStyle.Continuous;
		}

		private void OnError(byte status, string message)
		{
			Refresh();
		}

		private void tBoxLogMaxSamples_Enter(object sender, EventArgs e)
		{
			previousValue = tBoxLogMaxSamples.Text;
		}

		private void tBoxLogMaxSamples_Validating(object sender, CancelEventArgs e)
		{
			try
			{
				long num = (long)Convert.ToUInt64(tBoxLogMaxSamples.Text);
			}
			catch (Exception ex)
			{
				int num = (int)MessageBox.Show(ex.Message + (object)"\rInput Format: " + (string)(object)0 + " - " + ulong.MaxValue.ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				tBoxLogMaxSamples.Text = previousValue;
			}
		}

		private void tBoxLogMaxSamples_Validated(object sender, EventArgs e)
		{
			log.MaxSamples = ulong.Parse(tBoxLogMaxSamples.Text);
		}

		private void log_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "MaxSamples":
					tBoxLogMaxSamples.Text = log.MaxSamples.ToString();
					break;
			}
		}

		private void log_ProgressChanged(object sender, ProgressEventArg e)
		{
			if (base.InvokeRequired)
			{
				base.BeginInvoke((MethodInvoker)(() =>
				{
					pBarLog.Value = (int)e.Progress;
				}), null);
			}
			else
			{
				pBarLog.Value = (int)e.Progress;
			}
		}

		private void log_Stoped(object sender, EventArgs e)
		{
			if (InvokeRequired)
			{
				BeginInvoke((MethodInvoker)(() =>
				{
					cBtnLogOnOff.Checked = false;
					cBtnLogOnOff.Text = "Start";
					tBoxLogMaxSamples.Enabled = true;
					btnLogBrowseFile.Enabled = true;
					log.Stop();
					UpdateProgressBarStyle();
				}), null);
			}
			else
			{
				cBtnLogOnOff.Checked = false;
				cBtnLogOnOff.Text = "Start";
				tBoxLogMaxSamples.Enabled = true;
				btnLogBrowseFile.Enabled = true;
				log.Stop();
				UpdateProgressBarStyle();
			}
		}

		private void PacketLogForm_Load(object sender, EventArgs e)
		{
			try
			{
				string s1 = appSettings.GetValue("PacketLogTop");
				if (s1 != null)
				{
					try
					{
						Top = int.Parse(s1);
					}
					catch
					{
						int num = (int)MessageBox.Show((IWin32Window)this, "Error getting Top value.");
					}
				}
				string s2 = appSettings.GetValue("PacketLogLeft");
				if (s2 != null)
				{
					try
					{
						Left = int.Parse(s2);
					}
					catch
					{
						int num = (int)MessageBox.Show((IWin32Window)this, "Error getting Left value.");
					}
				}
				Screen[] allScreens = Screen.AllScreens;
				if (!IsFormLocatedInScreen((Form)this, allScreens))
				{
					Top = allScreens[0].WorkingArea.Top;
					Left = allScreens[0].WorkingArea.Left;
				}
				string folderPath = appSettings.GetValue("PacketLogPath");
				if (folderPath == null)
				{
					folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
					appSettings.SetValue("PacketLogPath", folderPath);
				}
				log.Path = folderPath;
				string str = appSettings.GetValue("PacketLogFileName");
				if (str == null)
				{
					str = "sx1276-pkt.log";
					appSettings.SetValue("PacketLogFileName", str);
				}
				log.FileName = str;
				string s3 = appSettings.GetValue("PacketLogMaxSamples");
				if (s3 == null)
				{
					s3 = "1000";
					appSettings.SetValue("PacketLogMaxSamples", s3);
				}
				log.MaxSamples = ulong.Parse(s3);
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		private void PacketLogForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			try
			{
				appSettings.SetValue("PacketLogTop", Top.ToString());
				appSettings.SetValue("PacketLogLeft", Left.ToString());
				appSettings.SetValue("PacketLogPath", log.Path);
				appSettings.SetValue("PacketLogFileName", log.FileName);
				appSettings.SetValue("PacketLogMaxSamples", log.MaxSamples.ToString());
			}
			catch (Exception)
			{
			}
		}

		private void btnLogBrowseFile_Click(object sender, EventArgs e)
		{
			OnError((byte)0, "-");
			try
			{
				sfLogSaveFileDlg.InitialDirectory = log.Path;
				sfLogSaveFileDlg.FileName = log.FileName;
				if (sfLogSaveFileDlg.ShowDialog() != DialogResult.OK)
					return;
				string[] strArray = sfLogSaveFileDlg.FileName.Split('\\');
				log.FileName = strArray[strArray.Length - 1];
				log.Path = "";
				int index;
				for (index = 0; index < strArray.Length - 2; ++index)
				{
					PacketLog packetLog = log;
					string str = packetLog.Path + strArray[index] + "\\";
					packetLog.Path = str;
				}
				log.Path += strArray[index];
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		private void cBtnLogOnOff_CheckedChanged(object sender, EventArgs e)
		{
			OnError((byte)0, "-");
			try
			{
				if (cBtnLogOnOff.Checked)
				{
					cBtnLogOnOff.Text = "Stop";
					tBoxLogMaxSamples.Enabled = false;
					btnLogBrowseFile.Enabled = false;
					log.Start();
				}
				else
				{
					cBtnLogOnOff.Text = "Start";
					tBoxLogMaxSamples.Enabled = true;
					btnLogBrowseFile.Enabled = true;
					log.Stop();
				}
			}
			catch (Exception ex)
			{
				cBtnLogOnOff.Checked = false;
				cBtnLogOnOff.Text = "Start";
				tBoxLogMaxSamples.Enabled = true;
				btnLogBrowseFile.Enabled = true;
				log.Stop();
				OnError((byte)1, ex.Message);
			}
			finally
			{
				UpdateProgressBarStyle();
			}
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			Close();
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
			ComponentResourceManager resources = new ComponentResourceManager(typeof(PacketLogForm));
			this.groupBox5 = new GroupBox();
			this.btnLogBrowseFile = new Button();
			this.pBarLog = new ProgressBar();
			this.tableLayoutPanel3 = new TableLayoutPanel();
			this.tBoxLogMaxSamples = new TextBox();
			this.lblCommandsLogMaxSamples = new Label();
			this.cBtnLogOnOff = new CheckBox();
			this.btnClose = new Button();
			this.sfLogSaveFileDlg = new SaveFileDialog();
			this.groupBox5.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			this.SuspendLayout();
			this.groupBox5.Controls.Add((Control)this.btnLogBrowseFile);
			this.groupBox5.Controls.Add((Control)this.pBarLog);
			this.groupBox5.Controls.Add((Control)this.tableLayoutPanel3);
			this.groupBox5.Controls.Add((Control)this.cBtnLogOnOff);
			this.groupBox5.Location = new Point(12, 12);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new Size(209, 103);
			this.groupBox5.TabIndex = 4;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Log control";
			this.btnLogBrowseFile.Location = new Point(15, 70);
			this.btnLogBrowseFile.Name = "btnLogBrowseFile";
			this.btnLogBrowseFile.Size = new Size(75, 23);
			this.btnLogBrowseFile.TabIndex = 2;
			this.btnLogBrowseFile.Text = "Browse...";
			this.btnLogBrowseFile.UseVisualStyleBackColor = true;
			this.btnLogBrowseFile.Click += new EventHandler(this.btnLogBrowseFile_Click);
			this.pBarLog.Location = new Point(15, 51);
			this.pBarLog.Name = "pBarLog";
			this.pBarLog.Size = new Size(179, 13);
			this.pBarLog.Step = 1;
			this.pBarLog.Style = ProgressBarStyle.Continuous;
			this.pBarLog.TabIndex = 1;
			this.tableLayoutPanel3.AutoSize = true;
			this.tableLayoutPanel3.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel3.ColumnCount = 2;
			this.tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle());
			this.tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle());
			this.tableLayoutPanel3.Controls.Add((Control)this.tBoxLogMaxSamples, 1, 0);
			this.tableLayoutPanel3.Controls.Add((Control)this.lblCommandsLogMaxSamples, 0, 0);
			this.tableLayoutPanel3.Location = new Point(15, 19);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 1;
			this.tableLayoutPanel3.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel3.Size = new Size(179, 26);
			this.tableLayoutPanel3.TabIndex = 0;
			this.tBoxLogMaxSamples.Location = new Point(94, 3);
			this.tBoxLogMaxSamples.Name = "tBoxLogMaxSamples";
			this.tBoxLogMaxSamples.Size = new Size(82, 20);
			this.tBoxLogMaxSamples.TabIndex = 1;
			this.tBoxLogMaxSamples.Text = "1000";
			this.tBoxLogMaxSamples.TextAlign = HorizontalAlignment.Center;
			this.tBoxLogMaxSamples.Enter += new EventHandler(this.tBoxLogMaxSamples_Enter);
			this.tBoxLogMaxSamples.Validating += new CancelEventHandler(this.tBoxLogMaxSamples_Validating);
			this.tBoxLogMaxSamples.Validated += new EventHandler(this.tBoxLogMaxSamples_Validated);
			this.lblCommandsLogMaxSamples.Location = new Point(3, 0);
			this.lblCommandsLogMaxSamples.Name = "lblCommandsLogMaxSamples";
			this.lblCommandsLogMaxSamples.Size = new Size(85, 23);
			this.lblCommandsLogMaxSamples.TabIndex = 0;
			this.lblCommandsLogMaxSamples.Text = "Max samples:";
			this.lblCommandsLogMaxSamples.TextAlign = ContentAlignment.MiddleLeft;
			this.cBtnLogOnOff.Appearance = Appearance.Button;
			this.cBtnLogOnOff.CheckAlign = ContentAlignment.MiddleCenter;
			this.cBtnLogOnOff.Location = new Point(119, 70);
			this.cBtnLogOnOff.Name = "cBtnLogOnOff";
			this.cBtnLogOnOff.Size = new Size(75, 23);
			this.cBtnLogOnOff.TabIndex = 3;
			this.cBtnLogOnOff.Text = "Start";
			this.cBtnLogOnOff.TextAlign = ContentAlignment.MiddleCenter;
			this.cBtnLogOnOff.UseVisualStyleBackColor = true;
			this.cBtnLogOnOff.CheckedChanged += new EventHandler(this.cBtnLogOnOff_CheckedChanged);
			this.btnClose.Location = new Point(79, 121);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new Size(75, 23);
			this.btnClose.TabIndex = 2;
			this.btnClose.Text = "Close";
			this.btnClose.UseVisualStyleBackColor = true;
			this.btnClose.Click += new EventHandler(this.btnClose_Click);
			this.sfLogSaveFileDlg.DefaultExt = "*.log";
			this.sfLogSaveFileDlg.Filter = "Log Files(*.log)|*.log|AllFiles(*.*)|*.*";
			this.AcceptButton = (IButtonControl)this.btnClose;
			this.AutoScaleDimensions = new SizeF(6f, 13f);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.ClientSize = new Size(233, 155);
			this.Controls.Add((Control)this.btnClose);
			this.Controls.Add((Control)this.groupBox5);
			this.DoubleBuffered = true;
			this.Icon = (Icon)resources.GetObject("$this.Icon");
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.Name = "PacketLogForm";
			this.Opacity = 0.9;
			this.Text = "Packet Log";
			this.FormClosed += new FormClosedEventHandler(this.PacketLogForm_FormClosed);
			this.Load += new EventHandler(this.PacketLogForm_Load);
			this.groupBox5.ResumeLayout(false);
			this.groupBox5.PerformLayout();
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel3.PerformLayout();
			this.ResumeLayout(false);
		}
	}
}

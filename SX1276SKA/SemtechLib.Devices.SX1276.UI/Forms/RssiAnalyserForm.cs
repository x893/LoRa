using SemtechLib.Controls;
using SemtechLib.Devices.Common.Interfaces;
using SemtechLib.Devices.SX1276;
using SemtechLib.Devices.SX1276.General;
using SemtechLib.Devices.SX1276.UI.Controls;
using SemtechLib.General;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

namespace SemtechLib.Devices.SX1276.UI.Forms
{
	public class RssiAnalyserForm : Form
	{
		private delegate void DeviceDataChangedDelegate(object sender, PropertyChangedEventArgs e);

		private int tickStart = Environment.TickCount;
		private DataLog log = new DataLog();
		private ApplicationSettings appSettings;
		private SX1276 device;
		private double time;
		private string previousValue;
		private IContainer components;
		private Panel panel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private Panel panel2;
		private NumericUpDownEx nudRssiThresh;
		private System.Windows.Forms.Label label55;
		private GroupBoxEx groupBox5;
		private Button btnLogBrowseFile;
		private ProgressBar pBarLog;
		private TableLayoutPanel tableLayoutPanel3;
		private TextBox tBoxLogMaxSamples;
		private System.Windows.Forms.Label lblCommandsLogMaxSamples;
		private CheckBox cBtnLogOnOff;
		private SaveFileDialog sfLogSaveFileDlg;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label5;
		private SemtechLib.Devices.SX1276.UI.Controls.RssiGraphControl graph;
		private System.Windows.Forms.Label label4;

		public RssiAnalyserForm()
		{
			InitializeComponent();

			graph.MouseWheel += new MouseEventHandler(graph_MouseWheel);

			log.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(log_PropertyChanged);
			log.Stoped += new EventHandler(log_Stoped);
			log.ProgressChanged += new ProgressChangedEventHandler(log_ProgressChanged);
		}

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
				device = (SX1276)value;
				Log.Device = device;
				device.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(device_PropertyChanged);
				CreateThreshold();
				nudRssiThresh.Value = device.RssiThreshold;
				UpdateThreshold((double)nudRssiThresh.Value);
			}
		}

		public DataLog Log
		{
			get
			{
				return log;
			}
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

		private void CreateThreshold()
		{
			GraphPane graphPane = graph.PaneList[0];
			double num = 0.0;
			LineObj lineObj = new LineObj(Color.Green, 0.0, num, 1.0, num);
			lineObj.Location.CoordinateFrame = CoordType.XChartFractionYScale;
			lineObj.IsVisible = true;
			graphPane.GraphObjList.Add((GraphObj)lineObj);
			graphPane.AxisChange();
			graph.Invalidate();
		}

		public void UpdateThreshold(double thres)
		{
			GraphPane graphPane = graph.PaneList[0];
			(graphPane.GraphObjList[0] as LineObj).Location.Y = thres;
			(graphPane.GraphObjList[0] as LineObj).Location.Y1 = thres;
			if (thres < graphPane.YAxis.Scale.Max && thres > graphPane.YAxis.Scale.Min)
				(graphPane.GraphObjList[0] as LineObj).IsVisible = true;
			else
				(graphPane.GraphObjList[0] as LineObj).IsVisible = false;
			graphPane.AxisChange();
			graph.Invalidate();
		}

		private void UpdateProgressBarStyle()
		{
			if ((long)log.MaxSamples == 0L && cBtnLogOnOff.Checked)
				pBarLog.Style = ProgressBarStyle.Marquee;
			else
				pBarLog.Style = ProgressBarStyle.Continuous;
		}

		private void OnDevicePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "RssiValue":
					if (device.RfPaSwitchEnabled == 0)
					{
						time = (double)(Environment.TickCount - tickStart) / 1000.0;
						graph.UpdateLineGraph(time, (double)device.RssiValue);
						break;
					}
					break;
				case "RfPaSwitchEnabled":
					label9.Visible = device.RfPaSwitchEnabled != 0;
					label7.Visible = device.RfPaSwitchEnabled != 0;
					label5.Visible = device.RfPaSwitchEnabled != 0;
					label4.Visible = device.RfPaSwitchEnabled != 0;
					label2.Visible = device.RfPaSwitchEnabled == 0;
					label8.Visible = device.RfPaSwitchEnabled == 0;
					break;
				case "RfPaRssiValue":
					if (device.RfPaSwitchEnabled == 1)
					{
						time = (double)(Environment.TickCount - tickStart) / 1000.0;
						graph.UpdateLineGraph(1, time, (double)device.RfPaRssiValue);
						graph.UpdateLineGraph(2, time, (double)device.RfIoRssiValue);
						break;
					}
					break;
				case "RfIoRssiValue":
					if (device.RfPaSwitchEnabled != 0)
					{
						time = (double)(Environment.TickCount - tickStart) / 1000.0;
						graph.UpdateLineGraph(1, time, (double)device.RfPaRssiValue);
						graph.UpdateLineGraph(2, time, (double)device.RfIoRssiValue);
						break;
					}
					break;
				case "RssiThreshold":
					nudRssiThresh.Value = device.RssiThreshold;
					break;
			}
			UpdateThreshold((double)nudRssiThresh.Value);
		}

		private void OnError(byte status, string message)
		{
			Refresh();
		}

		private void device_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (InvokeRequired)
				BeginInvoke((Delegate)new RssiAnalyserForm.DeviceDataChangedDelegate(OnDevicePropertyChanged), sender, (object)e);
			else
				OnDevicePropertyChanged(sender, e);
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

		private void log_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			if (InvokeRequired)
				BeginInvoke((MethodInvoker)(() => pBarLog.Value = e.ProgressPercentage), null);
			else
				pBarLog.Value = e.ProgressPercentage;
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

		private void RssiAnalyserForm_Load(object sender, EventArgs e)
		{
			try
			{
				string s1 = appSettings.GetValue("RssiAnalyserTop");
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
				string s2 = appSettings.GetValue("RssiAnalyserLeft");
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
				string folderPath = appSettings.GetValue("LogPath");
				if (folderPath == null)
				{
					folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
					appSettings.SetValue("LogPath", folderPath);
				}
				log.Path = folderPath;
				string str = appSettings.GetValue("LogFileName");
				if (str == null)
				{
					str = "sx1276-Rssi.log";
					appSettings.SetValue("LogFileName", str);
				}
				log.FileName = str;
				string s3 = appSettings.GetValue("LogMaxSamples");
				if (s3 == null)
				{
					s3 = "1000";
					appSettings.SetValue("LogMaxSamples", s3);
				}
				log.MaxSamples = ulong.Parse(s3);
			}
			catch (Exception ex)
			{
				OnError((byte)1, ex.Message);
			}
		}

		private void RssiAnalyserForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			try
			{
				appSettings.SetValue("RssiAnalyserTop", Top.ToString());
				appSettings.SetValue("RssiAnalyserLeft", Left.ToString());
				appSettings.SetValue("LogPath", log.Path);
				appSettings.SetValue("LogFileName", log.FileName);
				appSettings.SetValue("LogMaxSamples", log.MaxSamples.ToString());
			}
			catch (Exception)
			{
			}
		}

		private void nudRssiThresh_ValueChanged(object sender, EventArgs e)
		{
			device.SetRssiThresh(nudRssiThresh.Value);
		}

		private void graph_MouseWheel(object sender, MouseEventArgs e)
		{
			UpdateThreshold((double)nudRssiThresh.Value);
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
					DataLog dataLog = log;
					string str = dataLog.Path + strArray[index] + "\\";
					dataLog.Path = str;
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

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();
			base.Dispose(disposing);
		}
		/*
		 * this.graph = new RssiGraphControl();
		 * this.panel2.Controls.Add(this.graph);
		 */
		private void InitializeComponent()
		{
			this.components = (IContainer)new Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RssiAnalyserForm));
			this.panel1 = new System.Windows.Forms.Panel();
			this.groupBox5 = new SemtechLib.Controls.GroupBoxEx();
			this.btnLogBrowseFile = new System.Windows.Forms.Button();
			this.pBarLog = new System.Windows.Forms.ProgressBar();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this.tBoxLogMaxSamples = new System.Windows.Forms.TextBox();
			this.lblCommandsLogMaxSamples = new System.Windows.Forms.Label();
			this.cBtnLogOnOff = new System.Windows.Forms.CheckBox();
			this.nudRssiThresh = new SemtechLib.Controls.NumericUpDownEx();
			this.label55 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.sfLogSaveFileDlg = new System.Windows.Forms.SaveFileDialog();
			this.graph = new SemtechLib.Devices.SX1276.UI.Controls.RssiGraphControl();
			this.panel1.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudRssiThresh)).BeginInit();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.BackColor = System.Drawing.Color.Black;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.groupBox5);
			this.panel1.Controls.Add(this.nudRssiThresh);
			this.panel1.Controls.Add(this.label55);
			this.panel1.Controls.Add(this.label3);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.label9);
			this.panel1.Controls.Add(this.label7);
			this.panel1.Controls.Add(this.label5);
			this.panel1.Controls.Add(this.label4);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.label8);
			this.panel1.Location = new System.Drawing.Point(553, 0);
			this.panel1.Margin = new System.Windows.Forms.Padding(0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(223, 366);
			this.panel1.TabIndex = 1;
			// 
			// groupBox5
			// 
			this.groupBox5.BackColor = System.Drawing.Color.Transparent;
			this.groupBox5.Controls.Add(this.btnLogBrowseFile);
			this.groupBox5.Controls.Add(this.pBarLog);
			this.groupBox5.Controls.Add(this.tableLayoutPanel3);
			this.groupBox5.Controls.Add(this.cBtnLogOnOff);
			this.groupBox5.ForeColor = System.Drawing.Color.Gray;
			this.groupBox5.Location = new System.Drawing.Point(9, 196);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(209, 103);
			this.groupBox5.TabIndex = 8;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Log control";
			// 
			// btnLogBrowseFile
			// 
			this.btnLogBrowseFile.BackColor = System.Drawing.SystemColors.Control;
			this.btnLogBrowseFile.ForeColor = System.Drawing.SystemColors.ControlText;
			this.btnLogBrowseFile.Location = new System.Drawing.Point(15, 70);
			this.btnLogBrowseFile.Name = "btnLogBrowseFile";
			this.btnLogBrowseFile.Size = new System.Drawing.Size(75, 23);
			this.btnLogBrowseFile.TabIndex = 2;
			this.btnLogBrowseFile.Text = "Browse...";
			this.btnLogBrowseFile.UseVisualStyleBackColor = false;
			this.btnLogBrowseFile.Click += new System.EventHandler(this.btnLogBrowseFile_Click);
			// 
			// pBarLog
			// 
			this.pBarLog.Location = new System.Drawing.Point(15, 51);
			this.pBarLog.Name = "pBarLog";
			this.pBarLog.Size = new System.Drawing.Size(179, 13);
			this.pBarLog.Step = 1;
			this.pBarLog.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.pBarLog.TabIndex = 1;
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.AutoSize = true;
			this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel3.ColumnCount = 2;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel3.Controls.Add(this.tBoxLogMaxSamples, 1, 0);
			this.tableLayoutPanel3.Controls.Add(this.lblCommandsLogMaxSamples, 0, 0);
			this.tableLayoutPanel3.Location = new System.Drawing.Point(15, 19);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 1;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.Size = new System.Drawing.Size(179, 26);
			this.tableLayoutPanel3.TabIndex = 0;
			// 
			// tBoxLogMaxSamples
			// 
			this.tBoxLogMaxSamples.Location = new System.Drawing.Point(94, 3);
			this.tBoxLogMaxSamples.Name = "tBoxLogMaxSamples";
			this.tBoxLogMaxSamples.Size = new System.Drawing.Size(82, 20);
			this.tBoxLogMaxSamples.TabIndex = 1;
			this.tBoxLogMaxSamples.Text = "1000";
			this.tBoxLogMaxSamples.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tBoxLogMaxSamples.Enter += new System.EventHandler(this.tBoxLogMaxSamples_Enter);
			this.tBoxLogMaxSamples.Validating += new System.ComponentModel.CancelEventHandler(this.tBoxLogMaxSamples_Validating);
			this.tBoxLogMaxSamples.Validated += new System.EventHandler(this.tBoxLogMaxSamples_Validated);
			// 
			// lblCommandsLogMaxSamples
			// 
			this.lblCommandsLogMaxSamples.ForeColor = System.Drawing.Color.Gray;
			this.lblCommandsLogMaxSamples.Location = new System.Drawing.Point(3, 0);
			this.lblCommandsLogMaxSamples.Name = "lblCommandsLogMaxSamples";
			this.lblCommandsLogMaxSamples.Size = new System.Drawing.Size(85, 23);
			this.lblCommandsLogMaxSamples.TabIndex = 0;
			this.lblCommandsLogMaxSamples.Text = "Max samples:";
			this.lblCommandsLogMaxSamples.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cBtnLogOnOff
			// 
			this.cBtnLogOnOff.Appearance = System.Windows.Forms.Appearance.Button;
			this.cBtnLogOnOff.BackColor = System.Drawing.SystemColors.Control;
			this.cBtnLogOnOff.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.cBtnLogOnOff.ForeColor = System.Drawing.SystemColors.ControlText;
			this.cBtnLogOnOff.Location = new System.Drawing.Point(119, 70);
			this.cBtnLogOnOff.Name = "cBtnLogOnOff";
			this.cBtnLogOnOff.Size = new System.Drawing.Size(75, 23);
			this.cBtnLogOnOff.TabIndex = 3;
			this.cBtnLogOnOff.Text = "Start";
			this.cBtnLogOnOff.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.cBtnLogOnOff.UseVisualStyleBackColor = false;
			this.cBtnLogOnOff.CheckedChanged += new System.EventHandler(this.cBtnLogOnOff_CheckedChanged);
			// 
			// nudRssiThresh
			// 
			this.nudRssiThresh.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.nudRssiThresh.DecimalPlaces = 1;
			this.nudRssiThresh.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
			this.nudRssiThresh.Location = new System.Drawing.Point(117, 171);
			this.nudRssiThresh.Margin = new System.Windows.Forms.Padding(0);
			this.nudRssiThresh.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this.nudRssiThresh.Minimum = new decimal(new int[] {
            1275,
            0,
            0,
            -2147418112});
			this.nudRssiThresh.Name = "nudRssiThresh";
			this.nudRssiThresh.Size = new System.Drawing.Size(60, 20);
			this.nudRssiThresh.TabIndex = 7;
			this.nudRssiThresh.ThousandsSeparator = true;
			this.nudRssiThresh.Value = new decimal(new int[] {
            114,
            0,
            0,
            -2147483648});
			this.nudRssiThresh.ValueChanged += new System.EventHandler(this.nudRssiThresh_ValueChanged);
			// 
			// label55
			// 
			this.label55.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label55.AutoSize = true;
			this.label55.BackColor = System.Drawing.Color.Transparent;
			this.label55.ForeColor = System.Drawing.Color.Gray;
			this.label55.Location = new System.Drawing.Point(6, 171);
			this.label55.Margin = new System.Windows.Forms.Padding(0);
			this.label55.Name = "label55";
			this.label55.Size = new System.Drawing.Size(54, 13);
			this.label55.TabIndex = 6;
			this.label55.Text = "Threshold";
			this.label55.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label3
			// 
			this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label3.AutoSize = true;
			this.label3.ForeColor = System.Drawing.Color.Green;
			this.label3.Location = new System.Drawing.Point(6, 90);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(82, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "RSSI Threshold";
			// 
			// label1
			// 
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label1.BackColor = System.Drawing.Color.Green;
			this.label1.Location = new System.Drawing.Point(117, 95);
			this.label1.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(25, 2);
			this.label1.TabIndex = 3;
			this.label1.Text = "label7";
			// 
			// label9
			// 
			this.label9.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label9.AutoSize = true;
			this.label9.ForeColor = System.Drawing.Color.Aqua;
			this.label9.Location = new System.Drawing.Point(6, 29);
			this.label9.Margin = new System.Windows.Forms.Padding(3);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(69, 13);
			this.label9.TabIndex = 0;
			this.label9.Text = "RF_PA RSSI";
			this.label9.Visible = false;
			// 
			// label7
			// 
			this.label7.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label7.BackColor = System.Drawing.Color.Aqua;
			this.label7.Location = new System.Drawing.Point(117, 34);
			this.label7.Margin = new System.Windows.Forms.Padding(3);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(25, 2);
			this.label7.TabIndex = 1;
			this.label7.Text = "label7";
			this.label7.Visible = false;
			// 
			// label5
			// 
			this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label5.AutoSize = true;
			this.label5.ForeColor = System.Drawing.Color.Yellow;
			this.label5.Location = new System.Drawing.Point(6, 48);
			this.label5.Margin = new System.Windows.Forms.Padding(3);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(66, 13);
			this.label5.TabIndex = 0;
			this.label5.Text = "RF_IO RSSI";
			this.label5.Visible = false;
			// 
			// label4
			// 
			this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label4.BackColor = System.Drawing.Color.Yellow;
			this.label4.Location = new System.Drawing.Point(117, 53);
			this.label4.Margin = new System.Windows.Forms.Padding(3);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(25, 2);
			this.label4.TabIndex = 1;
			this.label4.Text = "label7";
			this.label4.Visible = false;
			// 
			// label2
			// 
			this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label2.AutoSize = true;
			this.label2.ForeColor = System.Drawing.Color.Red;
			this.label2.Location = new System.Drawing.Point(6, 67);
			this.label2.Margin = new System.Windows.Forms.Padding(3);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(32, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "RSSI";
			// 
			// label8
			// 
			this.label8.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.label8.BackColor = System.Drawing.Color.Red;
			this.label8.Location = new System.Drawing.Point(117, 72);
			this.label8.Margin = new System.Windows.Forms.Padding(3);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(25, 2);
			this.label8.TabIndex = 1;
			this.label8.Text = "label7";
			// 
			// panel2
			// 
			this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel2.Controls.Add(this.graph);
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Margin = new System.Windows.Forms.Padding(0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(553, 366);
			this.panel2.TabIndex = 0;
			// 
			// sfLogSaveFileDlg
			// 
			this.sfLogSaveFileDlg.DefaultExt = "*.log";
			this.sfLogSaveFileDlg.Filter = "Log Files(*.log)|*.log|AllFiles(*.*)|*.*";
			// 
			// graph
			// 
			this.graph.Dock = System.Windows.Forms.DockStyle.Fill;
			this.graph.Location = new System.Drawing.Point(0, 0);
			this.graph.Name = "graph";
			this.graph.Size = new System.Drawing.Size(551, 364);
			this.graph.TabIndex = 0;
			// 
			// RssiAnalyserForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(776, 366);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "RssiAnalyserForm";
			this.Text = "Rssi analyser";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.RssiAnalyserForm_FormClosed);
			this.Load += new System.EventHandler(this.RssiAnalyserForm_Load);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.groupBox5.ResumeLayout(false);
			this.groupBox5.PerformLayout();
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudRssiThresh)).EndInit();
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
	}
}
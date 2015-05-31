using SemtechLib.Devices.Common.Interfaces;
using SemtechLib.Devices.SX1276.UI.Controls;
using SemtechLib.General;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SemtechLib.Devices.SX1276.UI.Forms
{
	public class RegistersForm : Form, INotifyPropertyChanged
	{
		private ApplicationSettings appSettings;
		private IDevice device;
		private bool registersFormEnabled;
		private IContainer components;
		private RegisterTableControl registerTableControl1;
		private StatusStrip statusStrip1;
		private ToolStripStatusLabel ssLblStatus;
		private Panel panel1;

		public event PropertyChangedEventHandler PropertyChanged;

		public RegistersForm()
		{
			InitializeComponent();
		}

		public ApplicationSettings AppSettings
		{
			get { return appSettings; }
			set { appSettings = value; }
		}

		public IDevice Device
		{
			set
			{
				try
				{
					device = value;
					device.PropertyChanged += new PropertyChangedEventHandler(device_PropertyChanged);
					registerTableControl1.Registers = device.Registers;
					device.ReadRegisters();
				}
				catch (Exception ex)
				{
					OnError(1, ex.Message);
				}
			}
		}

		public bool RegistersFormEnabled
		{
			get
			{
				return registersFormEnabled;
			}
			set
			{
				registersFormEnabled = value;
				panel1.Enabled = value;
				OnPropertyChanged("RegistersFormEnabled");
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

		private void OnError(byte status, string message)
		{
			if ((int)status != 0)
				ssLblStatus.Text = "ERROR: " + message;
			else
				ssLblStatus.Text = message;
			Refresh();
		}

		private void RegistersForm_Load(object sender, EventArgs e)
		{
			string value = appSettings.GetValue("RegistersTop");
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
			value = appSettings.GetValue("RegistersLeft");
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
			if (IsFormLocatedInScreen((Form)this, allScreens))
				return;
			Top = allScreens[0].WorkingArea.Top;
			Left = allScreens[0].WorkingArea.Left;
		}

		private void RegistersForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			try
			{
				appSettings.SetValue("RegistersTop", Top.ToString());
				appSettings.SetValue("RegistersLeft", Left.ToString());
			}
			catch { }
		}

		private void device_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "Registers":
				case "Version":
					registerTableControl1.Registers = device.Registers;
					break;
			}
		}

		private void OnPropertyChanged(string propName)
		{
			if (PropertyChanged != null)
				PropertyChanged((object)this, new PropertyChangedEventArgs(propName));
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
			ComponentResourceManager resources = new ComponentResourceManager(typeof(RegistersForm));
			this.statusStrip1 = new StatusStrip();
			this.ssLblStatus = new ToolStripStatusLabel();
			this.panel1 = new Panel();
			this.registerTableControl1 = new RegisterTableControl();
			this.statusStrip1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			this.statusStrip1.Items.Add(this.ssLblStatus);
			this.statusStrip1.Location = new Point(0, 244);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new Size(292, 22);
			this.statusStrip1.TabIndex = 1;
			this.statusStrip1.Text = "statusStrip1";
			this.ssLblStatus.Name = "ssLblStatus";
			this.ssLblStatus.Size = new Size(11, 17);
			this.ssLblStatus.Text = "-";
			this.panel1.AutoSize = true;
			this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel1.Controls.Add((Control)this.registerTableControl1);
			this.panel1.Dock = DockStyle.Fill;
			this.panel1.Location = new Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new Size(292, 244);
			this.panel1.TabIndex = 0;
			this.registerTableControl1.AutoSize = true;
			this.registerTableControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.registerTableControl1.Location = new Point(3, 3);
			this.registerTableControl1.Name = "registerTableControl1";
			this.registerTableControl1.Size = new Size(208, 25);
			this.registerTableControl1.Split = 4U;
			this.registerTableControl1.TabIndex = 0;
			this.AutoScaleDimensions = new SizeF(6f, 13f);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ClientSize = new Size(292, 266);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.statusStrip1);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = (Icon)resources.GetObject("$this.Icon");
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.Name = "RegistersForm";
			this.Text = "SX1276 Registers display";
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
	}
}

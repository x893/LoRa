using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SemtechLib.Devices.SX1276.UI.Forms
{
	public class TemperatureCalibrationForm : Form
	{
		private IContainer components;
		private Label label1;
		private NumericUpDown nudTempRoom;
		private Label label2;
		private Label label3;
		private Button btnOk;

		public Decimal TempValueRoom
		{
			get
			{
				return this.nudTempRoom.Value;
			}
			set
			{
				this.nudTempRoom.Value = value;
			}
		}

		public TemperatureCalibrationForm()
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
			ComponentResourceManager resources = new ComponentResourceManager(typeof(TemperatureCalibrationForm));
			this.label1 = new Label();
			this.nudTempRoom = new NumericUpDown();
			this.label2 = new Label();
			this.label3 = new Label();
			this.btnOk = new Button();
			this.nudTempRoom.BeginInit();
			this.SuspendLayout();
			this.label1.AutoSize = true;
			this.label1.Location = new Point(12, 77);
			this.label1.Name = "label1";
			this.label1.Size = new Size(125, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Actual room temperature:";
			this.nudTempRoom.Location = new Point(143, 73);
			NumericUpDown numericUpDown1 = this.nudTempRoom;
			int[] bits1 = new int[4];
			bits1[0] = 85;
			Decimal num1 = new Decimal(bits1);
			numericUpDown1.Maximum = num1;
			this.nudTempRoom.Minimum = new Decimal(new int[4]
      {
        40,
        0,
        0,
        int.MinValue
      });
			this.nudTempRoom.Name = "nudTempRoom";
			this.nudTempRoom.Size = new Size(39, 20);
			this.nudTempRoom.TabIndex = 2;
			NumericUpDown numericUpDown2 = this.nudTempRoom;
			int[] bits2 = new int[4];
			bits2[0] = 25;
			Decimal num2 = new Decimal(bits2);
			numericUpDown2.Value = num2;
			this.label2.AutoSize = true;
			this.label2.Location = new Point(188, 77);
			this.label2.Name = "label2";
			this.label2.Size = new Size(18, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "°C";
			this.label3.AutoSize = true;
			this.label3.Font = new Font("Microsoft Sans Serif", 10f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
			this.label3.Location = new Point(13, 10);
			this.label3.MaximumSize = new Size(238, 0);
			this.label3.Name = "label3";
			this.label3.Size = new Size(218, 51);
			this.label3.TabIndex = 0;
			this.label3.Text = "Please enter the actual room temperature measured on an auxiliary thermometer!";
			this.label3.TextAlign = ContentAlignment.MiddleCenter;
			this.btnOk.DialogResult = DialogResult.OK;
			this.btnOk.Location = new Point(85, 99);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new Size(75, 23);
			this.btnOk.TabIndex = 4;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = true;
			this.AutoScaleDimensions = new SizeF(6f, 13f);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.ClientSize = new Size(244, 133);
			this.Controls.Add((Control)this.btnOk);
			this.Controls.Add((Control)this.nudTempRoom);
			this.Controls.Add((Control)this.label2);
			this.Controls.Add((Control)this.label1);
			this.Controls.Add((Control)this.label3);
			this.DoubleBuffered = true;
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.Icon = (Icon)resources.GetObject("$this.Icon");
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TemperatureCalibrationForm";
			this.ShowInTaskbar = false;
			this.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Temperature Calibration";
			this.nudTempRoom.EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
	}
}
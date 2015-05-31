using SemtechLib.General;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace SemtechLib.Devices.SX1276LR.UI.Controls
{
	public class RegisterTableControl : UserControl
	{
		private delegate void RegisterPropertyChangedDelegate(object sender, PropertyChangedEventArgs e);

		private int LABEL_SIZE_WIDTH = 200;
		private int LABEL_SIZE_HEIGHT = 20;
		private RegisterCollection registers = new RegisterCollection();
		private uint split = 1U;
		private string previousValue = "";
		private IContainer components;
		private ErrorProvider errorProvider;
		private Timer tmrChangeValidated;
		private TableLayoutPanel panel;
		private Label label;
		private List<Label> labelList;
		private int tabIndex;
		private int invisibleCnt;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public RegisterCollection Registers
		{
			get
			{
				return this.registers;
			}
			set
			{
				this.registers = value;
				foreach (Register register in value)
					register.PropertyChanged += new PropertyChangedEventHandler(this.register_PropertyChanged);
				this.BuildTableHeader();
				this.BuildTable();
			}
		}

		[DefaultValue(1)]
		public uint Split
		{
			get
			{
				return this.split;
			}
			set
			{
				this.split = (int)value != 0 ? value : 1U;
				this.BuildTable();
			}
		}

		public RegisterTableControl()
		{
			this.InitializeComponent();
			this.registers = new RegisterCollection();
			this.BuildTableHeader();
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
			this.errorProvider = new ErrorProvider(this.components);
			this.tmrChangeValidated = new Timer(this.components);
			((ISupportInitialize)this.errorProvider).BeginInit();
			this.SuspendLayout();
			this.errorProvider.ContainerControl = (ContainerControl)this;
			this.tmrChangeValidated.Interval = 5000;
			this.tmrChangeValidated.Tick += new EventHandler(this.tmrChangeValidated_Tick);
			this.AutoScaleDimensions = new SizeF(6f, 13f);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.Name = "RegisterTableControl";
			this.Size = new Size(0, 0);
			((ISupportInitialize)this.errorProvider).EndInit();
			this.ResumeLayout(false);
		}

		private void BuildTableHeader()
		{
			if (this.panel != null)
				this.Controls.Remove((Control)this.panel);
			this.panel = new TableLayoutPanel();
			this.labelList = new List<Label>();
			this.panel.SuspendLayout();
		}

		private void AddHeaderLabel(int col, int row)
		{
			for (int index = 0; index < 3; ++index)
			{
				this.label = new Label();
				this.label.AutoSize = false;
				this.label.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
				this.label.TextAlign = ContentAlignment.MiddleCenter;
				this.label.TabIndex = this.tabIndex++;
				this.labelList.Add(this.label);
			}
			this.labelList[col].Text = "Register";
			this.labelList[col].Size = new Size(this.LABEL_SIZE_WIDTH, this.LABEL_SIZE_HEIGHT);
			this.labelList[col + 1].Text = "Addr";
			this.labelList[col + 1].Size = new Size(39, 20);
			this.labelList[col + 2].Text = "Value";
			this.labelList[col + 2].Size = new Size(39, 20);
			this.panel.Controls.Add((Control)this.labelList[col], col, row);
			this.panel.Controls.Add((Control)this.labelList[col + 1], col + 1, row);
			this.panel.Controls.Add((Control)this.labelList[col + 2], col + 2, row);
		}

		private void BuildTable()
		{
			this.panel.Padding = new Padding(0, 0, 0, 0);
			this.panel.AutoSize = true;
			this.panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.panel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
			this.panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			this.panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			this.panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			this.panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			this.panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			this.panel.Location = new Point(0, 0);
			this.panel.TabIndex = this.tabIndex++;
			this.AddHeaderLabel(0, 0);
			int num = 0;
			int row = 1;
			this.invisibleCnt = 0;
			foreach (Register register in this.registers)
			{
				if (!register.Visible)
					++this.invisibleCnt;
			}
			for (int index = 0; index < this.registers.Count; ++index)
			{
				if (this.registers[index].Visible)
				{
					this.label = new Label();
					this.label.AutoSize = false;
					this.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
					this.label.TextAlign = ContentAlignment.MiddleLeft;
					this.label.TabIndex = this.tabIndex++;
					this.label.Margin = new Padding(0);
					this.label.Size = new Size(this.LABEL_SIZE_WIDTH, this.LABEL_SIZE_HEIGHT);
					this.label.Text = this.registers[index].Name;
					this.panel.Controls.Add((Control)this.label, num, row);
					this.label = new Label();
					this.label.AutoSize = false;
					this.label.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
					this.label.TextAlign = ContentAlignment.MiddleCenter;
					this.label.TabIndex = this.tabIndex++;
					this.label.Margin = new Padding(0);
					this.label.Size = new Size(39, 20);
					this.label.Text = "0x" + this.registers[index].Address.ToString("X02");
					this.panel.Controls.Add((Control)this.label, num + 1, row);
					TextBox textBox = new TextBox();
					textBox.AutoSize = false;
					textBox.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
					textBox.TextAlign = HorizontalAlignment.Center;
					textBox.MaxLength = 4;
					textBox.TabIndex = this.tabIndex++;
					textBox.Tag = (object)("0x" + this.registers[index].Address.ToString("X02"));
					textBox.Margin = new Padding(0);
					textBox.Size = new Size(45, 20);
					textBox.Text = "0x" + this.registers[index].Value.ToString("X02");
					textBox.ReadOnly = this.registers[index].ReadOnly;
					textBox.Validated += new EventHandler(this.tBox_Validated);
					textBox.Enter += new EventHandler(this.tBox_Enter);
					textBox.Validating += new CancelEventHandler(this.tBox_Validating);
					textBox.TextChanged += new EventHandler(this.tBox_TextChanged);
					this.panel.Controls.Add((Control)textBox, num + 2, row++);
					if ((long)row > (long)(this.registers.Count - this.invisibleCnt) / (long)this.split)
					{
						row = 1;
						num += 3;
						if ((long)num < (long)(this.split * 3U) || (long)(this.registers.Count - this.invisibleCnt) % (long)this.split != 0L)
							this.AddHeaderLabel(num, 0);
					}
				}
			}
			this.panel.ResumeLayout(false);
			this.panel.PerformLayout();
			this.Controls.Add((Control)this.panel);
		}

		private int GetIndexFromTextBox(TextBox tBox)
		{
			int row = 0;
			int num = 0;
			foreach (Control control in (ArrangedElementCollection)this.panel.Controls)
			{
				if (control is TextBox && control == tBox)
				{
					num = this.panel.GetColumn(control);
					row = this.panel.GetRow(control);
					break;
				}
			}
			return this.registers.IndexOf(this.registers[this.panel.GetControlFromPosition(num - 2, row).Text]);
		}

		private void OnRegisterPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (!(e.PropertyName == "Value"))
				return;
			foreach (Control control in (ArrangedElementCollection)this.panel.Controls)
			{
				if (control is TextBox)
				{
					TextBox textBox = (TextBox)control;
					if ((int)Convert.ToUInt32((string)control.Tag, 16) == (int)((Register)sender).Address)
					{
						if (textBox.Text != "0x" + ((Register)sender).Value.ToString("X02"))
							textBox.ForeColor = Color.Red;
						textBox.Text = "0x" + ((Register)sender).Value.ToString("X02");
						break;
					}
				}
			}
		}

		private void register_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (this.InvokeRequired)
				this.BeginInvoke((Delegate)new RegisterTableControl.RegisterPropertyChangedDelegate(this.OnRegisterPropertyChanged), sender, (object)e);
			else
				this.OnRegisterPropertyChanged(sender, e);
		}

		private void tBox_TextChanged(object sender, EventArgs e)
		{
			TextBox tBox = (TextBox)sender;
			try
			{
				if (tBox.Text != "0x" + this.registers[this.GetIndexFromTextBox(tBox)].Value.ToString("X02"))
					tBox.ForeColor = Color.Red;
				else
					this.tmrChangeValidated.Enabled = true;
			}
			catch (Exception)
			{
			}
		}

		private void tBox_Enter(object sender, EventArgs e)
		{
			this.previousValue = ((Control)sender).Text;
		}

		private void tBox_Validating(object sender, CancelEventArgs e)
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
				textBox.Text = this.previousValue;
			}
		}

		private void tBox_Validated(object sender, EventArgs e)
		{
			TextBox tBox = (TextBox)sender;
			byte num = Convert.ToByte(tBox.Text, 16);
			if (!tBox.Text.StartsWith("0x", true, (CultureInfo)null))
				tBox.Text = "0x" + num.ToString("X02");
			if ((int)this.registers[this.GetIndexFromTextBox(tBox)].Value == (int)num)
				return;
			this.registers[this.GetIndexFromTextBox(tBox)].Value = (uint)num;
			this.tmrChangeValidated.Enabled = true;
		}

		private void tmrChangeValidated_Tick(object sender, EventArgs e)
		{
			this.tmrChangeValidated.Enabled = false;
			foreach (Control control in (ArrangedElementCollection)this.panel.Controls)
			{
				if (control is TextBox)
				{
					TextBox tBox = (TextBox)control;
					if ((int)this.registers[this.GetIndexFromTextBox(tBox)].Value == (int)Convert.ToByte(tBox.Text, 16))
						tBox.ForeColor = SystemColors.WindowText;
				}
			}
		}
	}
}
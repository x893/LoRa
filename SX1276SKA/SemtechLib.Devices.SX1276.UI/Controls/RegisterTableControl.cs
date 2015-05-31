using SemtechLib.General;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace SemtechLib.Devices.SX1276.UI.Controls
{
	public class RegisterTableControl : UserControl
	{
		private delegate void RegisterPropertyChangedDelegate(object sender, PropertyChangedEventArgs e);

		private int LABEL_SIZE_WIDTH = 150;
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
				return registers;
			}
			set
			{
				registers = value;
				foreach (Register register in value)
					register.PropertyChanged += new PropertyChangedEventHandler(register_PropertyChanged);
				BuildTableHeader();
				BuildTable();
			}
		}

		[DefaultValue(1)]
		public uint Split
		{
			get
			{
				return split;
			}
			set
			{
				split = (int)value != 0 ? value : 1U;
				BuildTable();
			}
		}

		public RegisterTableControl()
		{
			InitializeComponent();
			registers = new RegisterCollection();
			BuildTableHeader();
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
			this.errorProvider = new ErrorProvider(this.components);
			this.tmrChangeValidated = new Timer(this.components);
			((ISupportInitialize)this.errorProvider).BeginInit();
			this.SuspendLayout();
			this.errorProvider.ContainerControl = (ContainerControl)this;
			this.tmrChangeValidated.Interval = 5000;
			this.tmrChangeValidated.Tick += new EventHandler(this.tmrChangeValidated_Tick);
			this.AutoScaleDimensions = new SizeF(6f, 13f);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Name = "RegisterTableControl";
			this.Size = new Size(0, 0);
			((ISupportInitialize)this.errorProvider).EndInit();
			this.ResumeLayout(false);
		}

		private void BuildTableHeader()
		{
			if (panel != null)
				Controls.Remove((Control)panel);
			panel = new TableLayoutPanel();
			labelList = new List<Label>();
			panel.SuspendLayout();
		}

		private void AddHeaderLabel(int col, int row)
		{
			for (int index = 0; index < 3; ++index)
			{
				label = new Label();
				label.AutoSize = false;
				label.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
				label.TextAlign = ContentAlignment.MiddleCenter;
				label.TabIndex = tabIndex++;
				labelList.Add(label);
			}
			labelList[col].Text = "Register";
			labelList[col].Size = new Size(LABEL_SIZE_WIDTH, LABEL_SIZE_HEIGHT);
			labelList[col + 1].Text = "Addr";
			labelList[col + 1].Size = new Size(39, 20);
			labelList[col + 2].Text = "Value";
			labelList[col + 2].Size = new Size(39, 20);
			panel.Controls.Add((Control)labelList[col], col, row);
			panel.Controls.Add((Control)labelList[col + 1], col + 1, row);
			panel.Controls.Add((Control)labelList[col + 2], col + 2, row);
		}

		private void BuildTable()
		{
			panel.Padding = new Padding(0, 0, 0, 0);
			panel.AutoSize = true;
			panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
			panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			panel.Location = new Point(0, 0);
			panel.TabIndex = tabIndex++;
			AddHeaderLabel(0, 0);
			int num = 0;
			int row = 1;
			invisibleCnt = 0;
			foreach (Register register in registers)
			{
				if (!register.Visible)
					++invisibleCnt;
			}
			for (int index = 0; index < registers.Count; ++index)
			{
				if (registers[index].Visible)
				{
					label = new Label();
					label.AutoSize = false;
					Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
					label.TextAlign = ContentAlignment.MiddleLeft;
					label.TabIndex = tabIndex++;
					label.Margin = new Padding(0);
					label.Size = new Size(LABEL_SIZE_WIDTH, LABEL_SIZE_HEIGHT);
					label.Text = registers[index].Name;
					panel.Controls.Add((Control)label, num, row);
					label = new Label();
					label.AutoSize = false;
					label.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
					label.TextAlign = ContentAlignment.MiddleCenter;
					label.TabIndex = tabIndex++;
					label.Margin = new Padding(0);
					label.Size = new Size(39, 20);
					label.Text = "0x" + registers[index].Address.ToString("X02");
					panel.Controls.Add((Control)label, num + 1, row);
					TextBox textBox = new TextBox();
					textBox.AutoSize = false;
					textBox.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
					textBox.TextAlign = HorizontalAlignment.Center;
					textBox.MaxLength = 4;
					textBox.TabIndex = tabIndex++;
					textBox.Tag = (object)("0x" + registers[index].Address.ToString("X02"));
					textBox.Margin = new Padding(0);
					textBox.Size = new Size(45, 20);
					textBox.Text = "0x" + registers[index].Value.ToString("X02");
					textBox.ReadOnly = registers[index].ReadOnly;
					textBox.Validated += new EventHandler(tBox_Validated);
					textBox.Enter += new EventHandler(tBox_Enter);
					textBox.Validating += new CancelEventHandler(tBox_Validating);
					textBox.TextChanged += new EventHandler(tBox_TextChanged);
					panel.Controls.Add((Control)textBox, num + 2, row++);
					if ((long)row > (long)(registers.Count - invisibleCnt) / (long)split)
					{
						row = 1;
						num += 3;
						if ((long)num < (long)(split * 3U) || (long)(registers.Count - invisibleCnt) % (long)split != 0L)
							AddHeaderLabel(num, 0);
					}
				}
			}
			panel.ResumeLayout(false);
			panel.PerformLayout();
			Controls.Add((Control)panel);
		}

		private int GetIndexFromTextBox(TextBox tBox)
		{
			int row = 0;
			int num = 0;
			foreach (Control control in (ArrangedElementCollection)panel.Controls)
			{
				if (control is TextBox && control == tBox)
				{
					num = panel.GetColumn(control);
					row = panel.GetRow(control);
					break;
				}
			}
			return registers.IndexOf(registers[panel.GetControlFromPosition(num - 2, row).Text]);
		}

		private void OnRegisterPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (!(e.PropertyName == "Value"))
				return;
			foreach (Control control in (ArrangedElementCollection)panel.Controls)
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
			if (InvokeRequired)
				BeginInvoke((Delegate)new RegisterTableControl.RegisterPropertyChangedDelegate(OnRegisterPropertyChanged), sender, (object)e);
			else
				OnRegisterPropertyChanged(sender, e);
		}

		private void tBox_TextChanged(object sender, EventArgs e)
		{
			TextBox tBox = (TextBox)sender;
			try
			{
				if (tBox.Text != "0x" + registers[GetIndexFromTextBox(tBox)].Value.ToString("X02"))
					tBox.ForeColor = Color.Red;
				else
					tmrChangeValidated.Enabled = true;
			}
			catch (Exception)
			{
			}
		}

		private void tBox_Enter(object sender, EventArgs e)
		{
			previousValue = ((Control)sender).Text;
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
				textBox.Text = previousValue;
			}
		}

		private void tBox_Validated(object sender, EventArgs e)
		{
			TextBox tBox = (TextBox)sender;
			byte num = Convert.ToByte(tBox.Text, 16);
			if (!tBox.Text.StartsWith("0x", true, (CultureInfo)null))
				tBox.Text = "0x" + num.ToString("X02");
			if ((int)registers[GetIndexFromTextBox(tBox)].Value == (int)num)
				return;
			registers[GetIndexFromTextBox(tBox)].Value = (uint)num;
			tmrChangeValidated.Enabled = true;
		}

		private void tmrChangeValidated_Tick(object sender, EventArgs e)
		{
			tmrChangeValidated.Enabled = false;
			foreach (Control control in (ArrangedElementCollection)panel.Controls)
			{
				if (control is TextBox)
				{
					TextBox tBox = (TextBox)control;
					if ((int)registers[GetIndexFromTextBox(tBox)].Value == (int)Convert.ToByte(tBox.Text, 16))
						tBox.ForeColor = SystemColors.WindowText;
				}
			}
		}
	}
}

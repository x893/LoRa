using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MyCSLib.Controls
{
	public class BytesBox : TextBox
	{
		public ContextMenuStrip CMenu = new ContextMenuStrip();
		public ToolStripMenuItem CM_Type = new ToolStripMenuItem();
		public ToolStripMenuItem CM_Clear = new ToolStripMenuItem();
		private bool _IsHex = true;

		[Category("输入格式设置")]
		[Description("True:Hex;False:ASCII")]
		public bool IsHex
		{
			get
			{
				return this._IsHex;
			}
			set
			{
				this._IsHex = value;
				if (this._IsHex)
					this.CM_Type.Text = "ASCII";
				else
					this.CM_Type.Text = "Hex";
			}
		}

		public BytesBox()
		{
			this.CM_Type.Name = "CM_Type";
			this.CM_Type.Size = new Size((int)sbyte.MaxValue, 22);
			this.CM_Type.Text = "ASCII";
			this.CM_Type.Click += new EventHandler(this.CM_Type_Click);
			this.CM_Clear.Name = "CM_Clear";
			this.CM_Clear.Size = new Size((int)sbyte.MaxValue, 22);
			this.CM_Clear.Text = "清空";
			this.CM_Clear.Click += new EventHandler(this.CM_Clear_Click);
			this.CMenu.Name = "CMenu";
			this.CMenu.ShowImageMargin = false;
			this.CMenu.Size = new Size(128, 48);
			this.CMenu.Items.AddRange(new ToolStripItem[2]
      {
        (ToolStripItem) this.CM_Type,
        (ToolStripItem) this.CM_Clear
      });
			this.ContextMenuStrip = this.CMenu;
		}

		private void CM_Type_Click(object sender, EventArgs e)
		{
			if (this.IsHex)
			{
				this.IsHex = false;
				if (this.Text.Length <= 0)
					return;
				string[] strArray = this.Text.Trim().Split(' ');
				byte[] bytes = new byte[strArray.Length];
				for (int index = 0; index < strArray.Length; ++index)
					bytes[index] = (byte)Convert.ToInt32(strArray[index], 16);
				this.Text = new ASCIIEncoding().GetString(bytes);
			}
			else
			{
				this.IsHex = true;
				if (this.Text.Length > 0)
				{
					byte[] bytes = new ASCIIEncoding().GetBytes(this.Text.Trim());
					StringBuilder stringBuilder = new StringBuilder();
					for (int index = 0; index < bytes.Length; ++index)
						stringBuilder.AppendFormat("{0:x2}", (object)bytes[index]);
					this.Text = stringBuilder.ToString();
				}
			}
		}

		public void ChangeText()
		{
			if (!this.IsHex)
			{
				if (this.Text.Length <= 0)
					return;
				string[] strArray = this.Text.Trim().Split(' ');
				byte[] bytes = new byte[strArray.Length];
				for (int index = 0; index < strArray.Length; ++index)
					bytes[index] = (byte)Convert.ToInt32(strArray[index], 16);
				this.Text = new ASCIIEncoding().GetString(bytes);
			}
			else if (this.Text.Length > 0)
			{
				byte[] bytes = new ASCIIEncoding().GetBytes(this.Text.Trim());
				StringBuilder stringBuilder = new StringBuilder();
				for (int index = 0; index < bytes.Length; ++index)
					stringBuilder.AppendFormat("{0:x2}", (object)bytes[index]);
				this.Text = stringBuilder.ToString();
			}
		}

		private void CM_Clear_Click(object sender, EventArgs e)
		{
		}

		protected override void OnTextChanged(EventArgs e)
		{
			if (this._IsHex)
			{
				string str = this.Text.Replace(" ", "");
				int num = str.Length / 2;
				int startIndex = 2;
				for (int index = 0; index < num; ++index)
				{
					str = str.Insert(startIndex, " ");
					startIndex += 3;
				}
				this.Text = str.TrimEnd().ToUpper();
			}
			this.SelectionStart = this.Text.Length;
		}

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			if (this._IsHex)
			{
				if ((int)e.KeyChar >= 48 && (int)e.KeyChar <= 57 || (int)e.KeyChar >= 65 && (int)e.KeyChar <= 70 || ((int)e.KeyChar >= 97 && (int)e.KeyChar <= 102 || ((int)e.KeyChar == 8 || (int)e.KeyChar == 3)) || (int)e.KeyChar == 24)
				{
					e.Handled = false;
					return;
				}
			}
			else if ((int)e.KeyChar >= 32 && (int)e.KeyChar <= 126 || ((int)e.KeyChar == 8 || (int)e.KeyChar == 13) || (int)e.KeyChar == 3 || (int)e.KeyChar == 24)
			{
				e.Handled = false;
				return;
			}
			if ((int)e.KeyChar == 22 && this.CheckPaste())
				e.Handled = false;
			else
				e.Handled = true;
		}

		private bool CheckPaste()
		{
			try
			{
				char[] chArray = Clipboard.GetDataObject().GetData(DataFormats.Text).ToString().ToCharArray();
				if (this._IsHex)
				{
					foreach (char ch in chArray)
					{
						if (((int)ch < 48 || (int)ch > 57) && ((int)ch < 65 || (int)ch > 70) && ((int)ch < 97 || (int)ch > 102) && (int)ch != 32)
						{
							MessageBox.Show(
								"Paste the data contains illegal characters, can contain only numbers 0-9, capital letters AF, af lowercase letters and spaces!",
								"Paste illegal",
								MessageBoxButtons.OK, MessageBoxIcon.Hand);
							return false;
						}
					}
				}
				else
				{
					foreach (char ch in chArray)
					{
						if (((int)ch < 32 || (int)ch > 126) && (int)ch != 10 && (int)ch != 13)
						{
							MessageBox.Show(
								"Paste the data contains illegal characters, can only contain ASCII characters!",
								"Paste illegal",
								MessageBoxButtons.OK, MessageBoxIcon.Hand);
							return false;
						}
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				return false;
			}
		}

		public Command GetCMD()
		{
			try
			{
				if (this.Text.Trim() == string.Empty)
				{
					MessageBox.Show("Does not allow is empty!");
					return (Command)null;
				}
				Command command = new Command();
				command.IsHex = this._IsHex;
				if (command.IsHex)
				{
					string[] strArray = this.Text.Trim().Split(' ');
					command.DataBytes = new byte[strArray.Length];
					for (int index = 0; index < strArray.Length; ++index)
						command.DataBytes[index] = (byte)Convert.ToInt32(strArray[index], 16);
				}
				else
					command.DataBytes = new ASCIIEncoding().GetBytes(this.Text.Trim());
				return command;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				return (Command)null;
			}
		}

		public void SetCMD(Command Cmd)
		{
			try
			{
				this.IsHex = Cmd.IsHex;
				if (this.IsHex)
				{
					StringBuilder stringBuilder = new StringBuilder();
					for (int index = 0; index < Cmd.DataBytes.Length; ++index)
						stringBuilder.AppendFormat("{0:x2}", (object)Cmd.DataBytes[index]);
					this.Text = stringBuilder.ToString();
				}
				else
					this.Text = new ASCIIEncoding().GetString(Cmd.DataBytes);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		public string GetText()
		{
			return this.Text;
		}
	}
}

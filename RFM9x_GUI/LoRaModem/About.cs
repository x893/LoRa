using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LoRaModem
{
	public class about : Form
	{
		private IContainer components;
		private PictureBox pictureBox1;
		private RichTextBox richTextBox1;
		private RichTextBox richTextBox2;

		public about()
		{
			InitializeComponent();
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
			ComponentResourceManager resources = new ComponentResourceManager(typeof(about));
			this.pictureBox1 = new PictureBox();
			this.richTextBox1 = new RichTextBox();
			this.richTextBox2 = new RichTextBox();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			this.SuspendLayout();
			this.pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
			this.pictureBox1.Location = new Point(14, 16);
			this.pictureBox1.Margin = new Padding(3, 4, 3, 4);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new Size(128, 128);
			this.pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			this.richTextBox1.Font = new Font("Arial Unicode MS", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte)134);
			this.richTextBox1.Location = new Point(148, 13);
			this.richTextBox1.Margin = new Padding(3, 4, 3, 4);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.ReadOnly = true;
			this.richTextBox1.Size = new Size(333, 131);
			this.richTextBox1.TabIndex = 1;
			this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
			this.richTextBox2.Location = new Point(14, 151);
			this.richTextBox2.Name = "richTextBox2";
			this.richTextBox2.ReadOnly = true;
			this.richTextBox2.ScrollBars = RichTextBoxScrollBars.None;
			this.richTextBox2.Size = new Size(467, 249);
			this.richTextBox2.TabIndex = 2;
			this.richTextBox2.Text = resources.GetString("richTextBox2.Text");
			this.AutoScaleDimensions = new SizeF(7f, 16f);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.ClientSize = new Size(496, 412);
			this.Controls.Add((Control)this.richTextBox2);
			this.Controls.Add((Control)this.richTextBox1);
			this.Controls.Add((Control)this.pictureBox1);
			this.Font = new Font("Arial Unicode MS", 9f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
			this.Icon = (Icon)resources.GetObject("$this.Icon");
			this.Margin = new Padding(3, 4, 3, 4);
			this.MaximizeBox = false;
			this.Name = "about";
			this.Text = "About HopeRF";
			((ISupportInitialize)this.pictureBox1).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
	}
}

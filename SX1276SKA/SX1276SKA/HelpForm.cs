using SemtechLib.General.Interfaces;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace SX1276SKA
{
	public class HelpForm : Form
	{
		private string docPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName) + "\\Doc";
		private IContainer components;
		private WebBrowser docViewer;

		public HelpForm()
		{
			InitializeComponent();
			if (!File.Exists(docPath + "\\overview.html"))
				return;
			docViewer.Navigate(docPath + "\\overview.html");
		}

		public void UpdateDocument(DocumentationChangedEventArgs e)
		{
			string str = docPath + "\\" + e.DocFolder + "\\" + e.DocName + ".html";
			if (File.Exists(str))
			{
				docViewer.Navigate(str);
			}
			else
			{
				if (!File.Exists(docPath + "\\overview.html"))
					return;
				docViewer.Navigate(docPath + "\\overview.html");
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HelpForm));
			this.docViewer = new System.Windows.Forms.WebBrowser();
			this.SuspendLayout();
			// 
			// docViewer
			// 
			this.docViewer.AllowWebBrowserDrop = false;
			this.docViewer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.docViewer.IsWebBrowserContextMenuEnabled = false;
			this.docViewer.Location = new System.Drawing.Point(0, 0);
			this.docViewer.MinimumSize = new System.Drawing.Size(20, 20);
			this.docViewer.Name = "docViewer";
			this.docViewer.Size = new System.Drawing.Size(292, 594);
			this.docViewer.TabIndex = 2;
			this.docViewer.TabStop = false;
			this.docViewer.Url = new System.Uri("", System.UriKind.Relative);
			this.docViewer.WebBrowserShortcutsEnabled = false;
			// 
			// HelpForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 594);
			this.Controls.Add(this.docViewer);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "HelpForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "HelpForm";
			this.ResumeLayout(false);

		}
	}
}
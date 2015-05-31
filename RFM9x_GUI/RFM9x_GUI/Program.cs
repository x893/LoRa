using System;
using System.Windows.Forms;

namespace RFM9x_GUI
{
	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run((Form)new MainForm());
		}
	}
}

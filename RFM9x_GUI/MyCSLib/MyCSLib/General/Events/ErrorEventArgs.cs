using System;
using System.Runtime.InteropServices;

namespace MyCSLib.General.Events
{
	[ComVisible(true)]
	[Serializable]
	public delegate void ErrorEventHandler(object sender, ErrorEventArgs e);

	public class ErrorEventArgs : EventArgs
	{
		private byte status;
		private string message;

		public byte Status
		{
			get
			{
				return this.status;
			}
		}

		public string Message
		{
			get
			{
				return this.message;
			}
		}

		public ErrorEventArgs(byte status, string message)
		{
			this.status = status;
			this.message = message;
		}
	}
}

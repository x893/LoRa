using System;

namespace SemtechLib.General.Events
{
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

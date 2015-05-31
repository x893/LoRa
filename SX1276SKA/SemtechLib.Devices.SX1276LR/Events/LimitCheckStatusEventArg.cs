using SemtechLib.Devices.SX1276LR.Enumerations;
using System;

namespace SemtechLib.Devices.SX1276LR.Events
{
	public class LimitCheckStatusEventArg : EventArgs
	{
		private LimitCheckStatusEnum status;
		private string message;

		public LimitCheckStatusEnum Status
		{
			get
			{
				return status;
			}
		}

		public string Message
		{
			get
			{
				return message;
			}
		}

		public LimitCheckStatusEventArg(LimitCheckStatusEnum status, string message)
		{
			this.status = status;
			this.message = message;
		}
	}
}

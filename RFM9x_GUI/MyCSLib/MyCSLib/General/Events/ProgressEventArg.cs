using System;
using System.Runtime.InteropServices;

namespace MyCSLib.General.Events
{
	[ComVisible(true)]
	[Serializable]
	public delegate void ProgressEventHandler(object sender, ProgressEventArg e);

	public class ProgressEventArg : EventArgs
	{
		private ulong progress;

		public ulong Progress
		{
			get
			{
				return this.progress;
			}
		}

		public ProgressEventArg(ulong progress)
		{
			this.progress = progress;
		}
	}
}

using System;

namespace SemtechLib.General.Events
{
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

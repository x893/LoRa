using System;

namespace SemtechLib.General.Interfaces
{
	public class DocumentationChangedEventArgs : EventArgs
	{
		private string docFolder;
		private string docName;

		public string DocFolder
		{
			get
			{
				return this.docFolder;
			}
		}

		public string DocName
		{
			get
			{
				return this.docName;
			}
		}

		public DocumentationChangedEventArgs(string docFolder, string docName)
		{
			this.docFolder = docFolder;
			this.docName = docName;
		}
	}
}

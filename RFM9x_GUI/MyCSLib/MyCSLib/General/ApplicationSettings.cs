using System;
using System.Collections;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using System.Xml;

namespace MyCSLib.General
{
	public sealed class ApplicationSettings : IDisposable
	{
		private const string m_fileName = "ApplicationSettings.xml";
		private const string m_RootElement = "ApplicationSettings";
		private const string m_SettingElement = "Setting";
		private const string m_PathSeperator = "/";
		private XmlDocument m_Document;

		public XmlDocument XmlDocument
		{
			get { return m_Document; }
		}

		public ApplicationSettings()
		{
			m_Document = ApplicationSettings.OpenDocument();
		}

		public bool SetValue(string Name, string Value)
		{
			foreach (XmlNode xmlNode in this.m_Document.SelectNodes("/ApplicationSettings/Setting"))
			{
				if (xmlNode.Attributes["Name"].Value.Equals(Name))
				{
					xmlNode.Attributes["Value"].Value = Value;
					return false;
				}
			}
			XmlNode xmlNode1 = m_Document.SelectSingleNode("/ApplicationSettings");
			XmlNode newChild = (XmlNode)m_Document.CreateElement("Setting");
			newChild.Attributes.Append(m_Document.CreateAttribute("Name"));
			newChild.Attributes.Append(m_Document.CreateAttribute("Value"));
			newChild.Attributes["Name"].Value = Name;
			newChild.Attributes["Value"].Value = Value;
			xmlNode1.AppendChild(newChild);
			return true;
		}

		public bool RemoveValue(string Name)
		{
			foreach (XmlNode oldChild in m_Document.SelectNodes("/ApplicationSettings/Setting"))
			{
				if (oldChild.Attributes["Name"].Value.Equals(Name))
				{
					oldChild.ParentNode.RemoveChild(oldChild);
					return true;
				}
			}
			return false;
		}

		public string GetValue(string Name)
		{
			foreach (XmlNode xmlNode in m_Document.SelectNodes("/ApplicationSettings/Setting"))
			{
				if (xmlNode.Attributes["Name"].Value.Equals(Name))
					return xmlNode.Attributes["Value"].Value;
			}
			return null;
		}

		public void ClearSettings()
		{
			m_Document = ApplicationSettings.CreateDocument();
		}

		public Hashtable GetSettings()
		{
			XmlNodeList xmlNodeList = m_Document.SelectNodes("/ApplicationSettings/Setting");
			Hashtable hashtable = new Hashtable(xmlNodeList.Count);
			foreach (XmlNode xmlNode in xmlNodeList)
				hashtable.Add(xmlNode.Attributes["Name"].Value, xmlNode.Attributes["Value"].Value);
			return hashtable;
		}

		public void SaveConfiguration()
		{
			ApplicationSettings.SaveDocument(m_Document, "ApplicationSettings.xml");
		}

		private static XmlDocument OpenDocument()
		{
			IsolatedStorageFileStream storageFileStream;
			try
			{
				storageFileStream = new IsolatedStorageFileStream("ApplicationSettings.xml", FileMode.Open, FileAccess.Read);
			}
			catch (FileNotFoundException)
			{
				return ApplicationSettings.CreateDocument();
			}
			XmlDocument xmlDocument = new XmlDocument();
			XmlTextReader xmlTextReader = new XmlTextReader((Stream)storageFileStream);
			xmlDocument.Load((XmlReader)xmlTextReader);
			xmlTextReader.Close();
			storageFileStream.Close();
			return xmlDocument;
		}

		private static XmlDocument CreateDocument()
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.CreateXmlDeclaration("1.0", (string)null, "yes");
			XmlElement element = xmlDocument.CreateElement("ApplicationSettings");
			xmlDocument.AppendChild((XmlNode)element);
			return xmlDocument;
		}

		private static void SaveDocument(XmlDocument document, string filename)
		{
			IsolatedStorageFileStream storageFileStream = new IsolatedStorageFileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
			storageFileStream.SetLength(0L);
			XmlTextWriter xmlTextWriter = new XmlTextWriter((Stream)storageFileStream, (Encoding)new UnicodeEncoding());
			xmlTextWriter.Formatting = Formatting.Indented;
			document.Save((XmlWriter)xmlTextWriter);
			xmlTextWriter.Close();
			storageFileStream.Close();
		}

		public void Dispose()
		{
			ApplicationSettings.SaveDocument(m_Document, "ApplicationSettings.xml");
		}
	}
}
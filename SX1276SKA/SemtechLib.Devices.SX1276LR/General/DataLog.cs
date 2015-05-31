using SemtechLib.Devices.Common.Interfaces;
using SemtechLib.Devices.SX1276LR;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace SemtechLib.Devices.SX1276LR.General
{
	public class DataLog : ILog, INotifyPropertyChanged
	{
		public event ProgressChangedEventHandler ProgressChanged;
		public event EventHandler Stoped;
		public event PropertyChangedEventHandler PropertyChanged;

		private bool isAppend = true;
		private string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		private string fileName = "sx1276LR-Rssi.log";
		private ulong maxSamples = 1000UL;
		private CultureInfo ci = CultureInfo.InvariantCulture;
		private FileStream fileStream;
		private StreamWriter streamWriter;
		private bool state;
		private ulong samples;
		private IDevice device;
		private bool enabled;

		public IDevice Device
		{
			set
			{
				if (device == value)
					return;
				device = value;
				device.PropertyChanged += new PropertyChangedEventHandler(device_PropertyChanged);
			}
		}

		public bool Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				if (enabled == value)
					return;
				enabled = value;
				OnPropertyChanged("Enabled");
			}
		}

		public bool IsAppend
		{
			get
			{
				return isAppend;
			}
			set
			{
				isAppend = value;
				OnPropertyChanged("IsAppend");
			}
		}

		public string Path
		{
			get
			{
				return path;
			}
			set
			{
				path = value;
				OnPropertyChanged("Path");
			}
		}

		public string FileName
		{
			get
			{
				return fileName;
			}
			set
			{
				fileName = value;
				OnPropertyChanged("FileName");
			}
		}

		public ulong MaxSamples
		{
			get
			{
				return maxSamples;
			}
			set
			{
				maxSamples = value;
				OnPropertyChanged("MaxSamples");
			}
		}

		public DataLog()
		{
		}

		public DataLog(IDevice device)
		{
			Device = device;
		}

		private void OnProgressChanged(int progress)
		{
			if (ProgressChanged != null)
				ProgressChanged((object)this, new ProgressChangedEventArgs(progress, new object()));
		}

		private void OnStop()
		{
			if (Stoped != null)
				Stoped((object)this, EventArgs.Empty);
		}

		private void GenerateFileHeader()
		{
			string str = ((SX1276LR)device).RfPaSwitchEnabled == 0 ? "#\tTime\tRSSI" : "#\tTime\tRF_PA RSSI\tRF_IO RSSI";
			streamWriter.WriteLine("#\tSX1276LR data log generated the " + DateTime.Now.ToShortDateString() + " at " + DateTime.Now.ToShortTimeString());
			streamWriter.WriteLine(str);
		}

		private void Update()
		{
			string str1 = "\t";
			if (device == null || !state)
				return;
			if (samples < maxSamples || (long)maxSamples == 0L)
			{
				string str2;
				if (((SX1276LR)device).RfPaSwitchEnabled != 0)
					str2 = str1 + DateTime.Now.ToString("HH:mm:ss.fff", (IFormatProvider)ci) + "\t" + ((SX1276LR)device).RfPaRssiValue.ToString("F1") + "\t" + ((SX1276LR)device).RfIoRssiValue.ToString("F1");
				else
					str2 = str1 + DateTime.Now.ToString("HH:mm:ss.fff", (IFormatProvider)ci) + "\t" + ((SX1276LR)device).RssiValue.ToString("F1");
				streamWriter.WriteLine(str2);
				if ((long)maxSamples != 0L)
				{
					++samples;
					OnProgressChanged((int)((Decimal)samples * new Decimal(100) / (Decimal)maxSamples));
				}
				else
					OnProgressChanged(0);
			}
			else
				OnStop();
		}

		public void Add(object value)
		{
		}

		public void Start()
		{
			try
			{
				fileStream = new FileStream(path + "\\" + fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
				streamWriter = new StreamWriter(fileStream, Encoding.ASCII);
				GenerateFileHeader();
				samples = 0;
				state = true;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public void Stop()
		{
			try
			{
				state = false;
				streamWriter.Close();
			}
			catch { }
		}

		private void device_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (((SX1276LR)device).RfPaSwitchEnabled != 0)
			{
				switch (e.PropertyName)
				{
					case "RfPaRssiValue":
						if (((SX1276LR)device).RfPaSwitchEnabled != 1)
							break;
						Update();
						break;
					case "RfIoRssiValue":
						Update();
						break;
				}
			}
			else
			{
				switch (e.PropertyName)
				{
					case "RssiValue":
						Update();
						break;
				}
			}
		}

		private void OnPropertyChanged(string propName)
		{
			if (PropertyChanged != null)
				PropertyChanged((object)this, new PropertyChangedEventArgs(propName));
		}
	}
}

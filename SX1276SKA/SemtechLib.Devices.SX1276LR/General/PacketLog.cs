using SemtechLib.Devices.Common.Events;
using SemtechLib.Devices.Common.Interfaces;
using SemtechLib.Devices.SX1276LR;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace SemtechLib.Devices.SX1276LR.General
{
	public class PacketLog : ILog, INotifyPropertyChanged
	{
		public enum PacketHandlerModeEnum
		{
			IDLE,
			RX,
			TX,
		}

		private bool isAppend = true;
		private string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		private string fileName = "sx1276-LoRa-pkt.log";
		private CultureInfo ci = CultureInfo.InvariantCulture;
		private FileStream fileStream;
		private StreamWriter streamWriter;
		private bool state;
		private ulong samples;
		private int packetNumber;
		private int maxPacketNumber;
		private SX1276LR device;
		private bool enabled;
		private ulong maxSamples;
		private bool UsePer;

		public IDevice Device
		{
			set
			{
				if (device == value)
					return;
				device = (SX1276LR)value;
				device.PropertyChanged += new PropertyChangedEventHandler(device_PropertyChanged);
				device.PacketHandlerStarted += new EventHandler(device_PacketHandlerStarted);
				device.PacketHandlerStoped += new EventHandler(device_PacketHandlerStoped);
				device.PacketHandlerTransmitted += new PacketStatusEventHandler(device_PacketHandlerTransmitted);
				device.PacketHandlerReceived += new PacketStatusEventHandler(device_PacketHandlerReceived);
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
				if (isAppend == value)
					return;
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
				if (!(path != value))
					return;
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
				if (!(fileName != value))
					return;
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
				if ((long)maxSamples == (long)value)
					return;
				maxSamples = value;
				OnPropertyChanged("MaxSamples");
			}
		}

		public event ProgressChangedEventHandler ProgressChanged;

		public event EventHandler Stoped;

		public event PropertyChangedEventHandler PropertyChanged;

		public PacketLog()
		{
		}

		public PacketLog(IDevice device)
		{
			Device = device;
		}

		private void OnProgressChanged(int progress)
		{
			if (ProgressChanged == null)
				return;
			ProgressChanged((object)this, new ProgressChangedEventArgs(progress, new object()));
		}

		private void OnStop()
		{
			if (Stoped == null)
				return;
			Stoped((object)this, EventArgs.Empty);
		}

		private void GenerateFileHeader()
		{
			streamWriter.WriteLine("#\tSX1276LR LoRa mode packet log generated the " + DateTime.Now.ToShortDateString() + " at " + DateTime.Now.ToShortTimeString());
			string str;
			if (device.PacketModeTx)
			{
				str = "#\tTime\tPkt #\tMode\tPayload";
			}
			else
			{
				UsePer = device.PacketUsePer;
				str = UsePer ? "#\tTime\tPkt #\tMode\tPayload\tCRC\tPkt Rssi [dBm]\tSNR [dB]\tHDR cnt\tPKT cnt\tRx CRC ON\tRx CR\tRx Len\tIRQ\tPER cnt\t" : "#\tTime\tPkt #\tMode\tPayload\tCRC\tPkt Rssi [dBm]\tSNR [dB]\tHDR cnt\tPKT cnt\tRx CRC ON\tRx CR\tRx Len\tIRQ";
			}
			streamWriter.WriteLine(str);
		}

		private void Update()
		{
			if (!Enabled)
				return;
			string str1 = "\t";
			if (device == null || streamWriter == null || !state)
				return;
			if (samples < maxSamples || (long)maxSamples == 0L)
			{
				string str2 = str1 + DateTime.Now.ToString("HH:mm:ss.fff", (IFormatProvider)ci) + "\t" + packetNumber.ToString() + "\t" + (device.PacketModeTx ? "Tx\t" : "Rx\t");
				if (device.Payload != null && device.Payload.Length != 0)
				{
					int index;
					for (index = 0; index < device.Payload.Length - 1; ++index)
						str2 = str2 + device.Payload[index].ToString("X02") + "-";
					str2 = str2 + device.Payload[index].ToString("X02") + "\t";
				}
				if (!device.PacketModeTx)
					str2 = !device.ImplicitHeaderModeOn ? (!device.RxPayloadCrcOn ? str2 + "OFF\t" : str2 + (device.PayloadCrcError ? "KO\t" : "OK\t")) : (!device.PayloadCrcOn ? str2 + "OFF\t" : str2 + (device.PayloadCrcError ? "KO\t" : "OK\t"));
				string str3 = str2 + (!device.PacketModeTx ? device.PktRssiValue.ToString("F1") + "\t" : "\t");
				string str4 = device.IsDebugOn ? str3 + (!device.PacketModeTx ? device.PktSnrValue.ToString("F1") + "\t" : "\t") : str3 + (!device.PacketModeTx ? ((int)device.PktSnrValue > 0 ? "> 0\t" : device.PktSnrValue.ToString("F1") + "\t") : "\t");
				if (!device.PacketModeTx)
				{
					string str5 = str4 + device.ValidHeaderCnt.ToString() + "\t" + device.ValidPacketCnt.ToString() + "\t" + device.RxPayloadCrcOn.ToString() + "\t";
					switch (device.RxPayloadCodingRate)
					{
						case (byte)1:
							str5 += "4/5\t";
							break;
						case (byte)2:
							str5 += "4/6\t";
							break;
						case (byte)3:
							str5 += "4/7\t";
							break;
						case (byte)4:
							str5 += "4/8\t";
							break;
					}
					str4 = (!device.ImplicitHeaderModeOn ? str5 + device.RxNbBytes.ToString() + "\t" : str5 + device.PayloadLength.ToString() + "\t") + device.Registers["RegIrqFlags"].Value.ToString("X02") + "\t";
					if (UsePer)
						str4 = str4 + ((int)device.Payload[1] << 24 | (int)device.Payload[2] << 16 | (int)device.Payload[3] << 8 | (int)device.Payload[4]).ToString() + "\t";
				}
				if (device.IsDebugOn)
				{
					string str5 = str4 + "\tLNA Gain: ";
					foreach (int num in device.PktLnaValues)
						str5 = str5 + num.ToString() + ", ";
					str5.TrimEnd(',');
					str4 = str5 + "\tRssi Values: ";
					foreach (double num in device.PktRssiValues)
						str4 = str4 + num.ToString("F1") + ", ";
					str4.TrimEnd(',');
				}
				streamWriter.WriteLine(str4);
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

		public void Start()
		{
			try
			{
				if (!Enabled)
					return;
				if (streamWriter != null)
				{
					streamWriter.Close();
					streamWriter = (StreamWriter)null;
				}
				if (fileStream != null)
				{
					fileStream.Close();
					fileStream = (FileStream)null;
				}
				if (!IsAppend)
				{
					if (File.Exists(path + "\\" + fileName))
					{
						switch (MessageBox.Show("Packet log file already exists.\nDo you want to replace it?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk))
						{
							case DialogResult.Yes:
								break;
							case DialogResult.No:
								SaveFileDialog saveFileDialog = new SaveFileDialog();
								saveFileDialog.DefaultExt = "*.log";
								saveFileDialog.Filter = "Log Files(*.log)|*.log|AllFiles(*.*)|*.*";
								saveFileDialog.InitialDirectory = device.PacketHandlerLog.Path;
								saveFileDialog.FileName = device.PacketHandlerLog.FileName;
								if (saveFileDialog.ShowDialog() == DialogResult.OK)
								{
									string[] strArray = saveFileDialog.FileName.Split('\\');
									device.PacketHandlerLog.FileName = strArray[strArray.Length - 1];
									device.PacketHandlerLog.Path = "";
									int index;
									for (index = 0; index < strArray.Length - 2; ++index)
									{
										ILog packetHandlerLog = device.PacketHandlerLog;
										string str = packetHandlerLog.Path + strArray[index] + "\\";
										packetHandlerLog.Path = str;
									}
									device.PacketHandlerLog.Path += strArray[index];
									break;
								}
								break;
							default:
								throw new Exception("Wrong dialog box return code.");
						}
					}
					fileStream = new FileStream(path + "\\" + fileName, FileMode.Create, FileAccess.Write, FileShare.Read);
				}
				else
					fileStream = new FileStream(path + "\\" + fileName, FileMode.Append, FileAccess.Write, FileShare.Read);

				streamWriter = new StreamWriter(fileStream, Encoding.ASCII);
				streamWriter.AutoFlush = true;
				GenerateFileHeader();
				samples = 0UL;
				state = true;
			}
			catch (Exception ex)
			{
				Stop();
				throw ex;
			}
		}

		public void Stop()
		{
			try
			{
				state = false;
				if (streamWriter != null)
				{
					streamWriter.Close();
					streamWriter = (StreamWriter)null;
				}
				if (fileStream == null)
					return;
				fileStream.Close();
				fileStream = (FileStream)null;
			}
			catch { }
		}

		private void device_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			string propertyName;
			if ((propertyName = e.PropertyName) == null)
				return;
			int num = propertyName == "RssiValue" ? 1 : 0;
		}

		private void device_PacketHandlerStarted(object sender, EventArgs e)
		{
		}

		private void device_PacketHandlerStoped(object sender, EventArgs e)
		{
		}

		private void device_PacketHandlerTransmitted(object sender, PacketStatusEventArg e)
		{
			maxPacketNumber = e.Max;
			packetNumber = e.Number;
			Update();
		}

		private void device_PacketHandlerReceived(object sender, PacketStatusEventArg e)
		{
			maxPacketNumber = e.Max;
			packetNumber = e.Number;
			Update();
		}

		private void OnPropertyChanged(string propName)
		{
			if (PropertyChanged == null)
				return;
			PropertyChanged((object)this, new PropertyChangedEventArgs(propName));
		}
	}
}

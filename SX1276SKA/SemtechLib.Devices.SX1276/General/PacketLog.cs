// Decompiled with JetBrains decompiler
// Type: SemtechLib.Devices.SX1276.General.PacketLog
// Assembly: SemtechLib.Devices.SX1276, Version=2.0.0.0, Culture=neutral, PublicKeyToken=a3cee8af388a4f11
// MVID: 64B2382D-7AA3-4D8B-BE9D-2E742AB27E64
// Assembly location: C:\Tools\Semtech\SX1276SKA\SemtechLib.Devices.SX1276.dll

using SemtechLib.Devices.Common.Events;
using SemtechLib.Devices.Common.Interfaces;
using SemtechLib.Devices.SX1276;
using SemtechLib.Devices.SX1276.Enumerations;
using SemtechLib.General.Events;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace SemtechLib.Devices.SX1276.General
{
  public class PacketLog : INotifyPropertyChanged
  {
    private string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
    private string fileName = "sx1276-pkt.log";
    private ulong maxSamples = 1000UL;
    private CultureInfo ci = CultureInfo.InvariantCulture;
    private FileStream fileStream;
    private StreamWriter streamWriter;
    private bool state;
    private ulong samples;
    private int packetNumber;
    private int maxPacketNumber;
    private IDevice device;

    public IDevice Device
    {
      set
      {
        if (this.device == value)
          return;
        this.device = value;
        this.device.PropertyChanged += new PropertyChangedEventHandler(this.device_PropertyChanged);
        this.device.PacketHandlerStarted += new EventHandler(this.device_PacketHandlerStarted);
        this.device.PacketHandlerStoped += new EventHandler(this.device_PacketHandlerStoped);
        this.device.PacketHandlerTransmitted += new PacketStatusEventHandler(this.device_PacketHandlerTransmitted);
        this.device.PacketHandlerReceived += new PacketStatusEventHandler(this.device_PacketHandlerReceived);
      }
    }

    public string Path
    {
      get
      {
        return this.path;
      }
      set
      {
        this.path = value;
        this.OnPropertyChanged("Path");
      }
    }

    public string FileName
    {
      get
      {
        return this.fileName;
      }
      set
      {
        this.fileName = value;
        this.OnPropertyChanged("FileName");
      }
    }

    public ulong MaxSamples
    {
      get
      {
        return this.maxSamples;
      }
      set
      {
        this.maxSamples = value;
        this.OnPropertyChanged("MaxSamples");
      }
    }

    public event ProgressEventHandler ProgressChanged;

    public event EventHandler Stoped;

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnProgressChanged(ulong progress)
    {
      if (this.ProgressChanged == null)
        return;
      this.ProgressChanged((object) this, new ProgressEventArg(progress));
    }

    private void OnStop()
    {
      if (this.Stoped == null)
        return;
      this.Stoped((object) this, EventArgs.Empty);
    }

    private void GenerateFileHeader()
    {
      string str = "#\tTime\tMode\tRssi\tPkt Max\tPkt #\tPreamble Size\tSync\tLength\tNode Address\tMessage\tCRC";
      this.streamWriter.WriteLine("#\tSX1276 packet log generated the " + DateTime.Now.ToShortDateString() + " at " + DateTime.Now.ToShortTimeString());
      this.streamWriter.WriteLine(str);
    }

    private void Update()
    {
      string str1 = "\t";
      if (this.device == null || !this.state)
        return;
      if (this.samples < this.maxSamples || (long) this.maxSamples == 0L)
      {
        string str2 = str1 + DateTime.Now.ToString("HH:mm:ss.fff", (IFormatProvider) this.ci) + "\t" + (((SX1276) this.device).Mode == OperatingModeEnum.Tx ? "Tx\t" : (((SX1276) this.device).Mode == OperatingModeEnum.Rx ? "Rx\t" : "\t")) + (((SX1276) this.device).Mode == OperatingModeEnum.Rx ? ((SX1276) this.device).Packet.Rssi.ToString("F1") + "\t" : "\t") + this.maxPacketNumber.ToString() + "\t" + this.packetNumber.ToString() + "\t" + ((SX1276) this.device).Packet.PreambleSize.ToString() + "\t" + new MaskValidationType(((SX1276) this.device).Packet.SyncValue).StringValue + "\t" + ((SX1276) this.device).Packet.MessageLength.ToString("X02") + "\t";
        string str3 = (((SX1276) this.device).Mode != OperatingModeEnum.Rx ? str2 + (((SX1276) this.device).Packet.AddressFiltering != AddressFilteringEnum.OFF ? ((SX1276) this.device).Packet.NodeAddress.ToString("X02") : "") : str2 + (((SX1276) this.device).Packet.AddressFiltering != AddressFilteringEnum.OFF ? ((SX1276) this.device).Packet.NodeAddressRx.ToString("X02") : "")) + "\t";
        if (((SX1276) this.device).Packet.Message != null && ((SX1276) this.device).Packet.Message.Length != 0)
        {
          int index;
          for (index = 0; index < ((SX1276) this.device).Packet.Message.Length - 1; ++index)
            str3 = str3 + ((SX1276) this.device).Packet.Message[index].ToString("X02") + "-";
          str3 = str3 + ((SX1276) this.device).Packet.Message[index].ToString("X02") + "\t";
        }
        this.streamWriter.WriteLine(str3 + (((SX1276) this.device).Packet.CrcOn ? ((int) ((SX1276) this.device).Packet.Crc >> 8).ToString("X02") + "-" + ((int) ((SX1276) this.device).Packet.Crc & (int) byte.MaxValue).ToString("X02") + "\t" : "\t"));
        if ((long) this.maxSamples != 0L)
        {
          ++this.samples;
          this.OnProgressChanged((ulong) ((Decimal) this.samples * new Decimal(100) / (Decimal) this.maxSamples));
        }
        else
          this.OnProgressChanged(0UL);
      }
      else
        this.OnStop();
    }

    public void Start()
    {
      try
      {
        this.fileStream = new FileStream(this.path + "\\" + this.fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
        this.streamWriter = new StreamWriter((Stream) this.fileStream, Encoding.ASCII);
        this.GenerateFileHeader();
        this.samples = 0UL;
        this.state = true;
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
        this.state = false;
        this.streamWriter.Close();
      }
      catch (Exception)
      {
      }
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
      this.maxPacketNumber = e.Max;
      this.packetNumber = e.Number;
      this.Update();
    }

    private void device_PacketHandlerReceived(object sender, PacketStatusEventArg e)
    {
      this.maxPacketNumber = e.Max;
      this.packetNumber = e.Number;
      this.Update();
    }

    private void OnPropertyChanged(string propName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propName));
    }

    public enum PacketHandlerModeEnum
    {
      IDLE,
      RX,
      TX,
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: SemtechLib.Devices.SX1276.General.DataLog
// Assembly: SemtechLib.Devices.SX1276, Version=2.0.0.0, Culture=neutral, PublicKeyToken=a3cee8af388a4f11
// MVID: 64B2382D-7AA3-4D8B-BE9D-2E742AB27E64
// Assembly location: C:\Tools\Semtech\SX1276SKA\SemtechLib.Devices.SX1276.dll

using SemtechLib.Devices.SX1276;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace SemtechLib.Devices.SX1276.General
{
  public class DataLog : ILog, INotifyPropertyChanged
  {
    private string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
    private string fileName = "sx1276-Rssi.log";
    private ulong maxSamples = 1000UL;
    private CultureInfo ci = CultureInfo.InvariantCulture;
    private FileStream fileStream;
    private StreamWriter streamWriter;
    private bool state;
    private ulong samples;
    private SX1276 device;

    public SX1276 Device
    {
      set
      {
        if (this.device == value)
          return;
        this.device = value;
        this.device.PropertyChanged += new PropertyChangedEventHandler(this.device_PropertyChanged);
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

    public event ProgressChangedEventHandler ProgressChanged;

    public event EventHandler Stoped;

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnProgressChanged(int progress)
    {
      if (this.ProgressChanged == null)
        return;
      this.ProgressChanged((object) this, new ProgressChangedEventArgs(progress, new object()));
    }

    private void OnStop()
    {
      if (this.Stoped == null)
        return;
      this.Stoped((object) this, EventArgs.Empty);
    }

    private void GenerateFileHeader()
    {
      string str = this.device.RfPaSwitchEnabled == 0 ? "#\tTime\tRSSI" : "#\tTime\tRF_PA RSSI\tRF_IO RSSI";
      this.streamWriter.WriteLine("#\tSX1276 data log generated the " + DateTime.Now.ToShortDateString() + " at " + DateTime.Now.ToShortTimeString());
      this.streamWriter.WriteLine(str);
    }

    private void Update()
    {
      string str1 = "\t";
      if (this.device == null || !this.state)
        return;
      if (this.samples < this.maxSamples || (long) this.maxSamples == 0L)
      {
        string str2;
        if (this.device.RfPaSwitchEnabled != 0)
          str2 = str1 + DateTime.Now.ToString("HH:mm:ss.fff", (IFormatProvider) this.ci) + "\t" + this.device.RfPaRssiValue.ToString("F1") + "\t" + this.device.RfIoRssiValue.ToString("F1");
        else
          str2 = str1 + DateTime.Now.ToString("HH:mm:ss.fff", (IFormatProvider) this.ci) + "\t" + this.device.RssiValue.ToString("F1");
        this.streamWriter.WriteLine(str2);
        if ((long) this.maxSamples != 0L)
        {
          ++this.samples;
          this.OnProgressChanged((int) ((Decimal) this.samples * new Decimal(100) / (Decimal) this.maxSamples));
        }
        else
          this.OnProgressChanged(0);
      }
      else
        this.OnStop();
    }

    public void Add(object value)
    {
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
      if (this.device.RfPaSwitchEnabled != 0)
      {
        switch (e.PropertyName)
        {
          case "RfPaRssiValue":
            if (this.device.RfPaSwitchEnabled != 1)
              break;
            this.Update();
            break;
          case "RfIoRssiValue":
            this.Update();
            break;
        }
      }
      else
      {
        switch (e.PropertyName)
        {
          case "RssiValue":
            this.Update();
            break;
        }
      }
    }

    private void OnPropertyChanged(string propName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propName));
    }
  }
}

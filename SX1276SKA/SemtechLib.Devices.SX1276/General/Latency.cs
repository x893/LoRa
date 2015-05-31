// Decompiled with JetBrains decompiler
// Type: SemtechLib.Devices.SX1276.General.Latency
// Assembly: SemtechLib.Devices.SX1276, Version=2.0.0.0, Culture=neutral, PublicKeyToken=a3cee8af388a4f11
// MVID: 64B2382D-7AA3-4D8B-BE9D-2E742AB27E64
// Assembly location: C:\Tools\Semtech\SX1276SKA\SemtechLib.Devices.SX1276.dll

namespace SemtechLib.Devices.SX1276.General
{
  public class Latency
  {
    private double delay;
    private double precision;

    public double Delay
    {
      get
      {
        return this.delay;
      }
      set
      {
        this.delay = value;
      }
    }

    public double Precision
    {
      get
      {
        return this.precision;
      }
      set
      {
        this.precision = value;
      }
    }

    public Latency(double delay, double precision)
    {
      this.delay = delay;
      this.precision = precision;
    }

    public static Latency operator +(Latency a, Latency b)
    {
      return new Latency(a.Delay + b.Delay, a.Precision + b.Precision);
    }

    public static Latency operator -(Latency a, Latency b)
    {
      return new Latency(a.Delay - b.Delay, a.Precision - b.Precision);
    }

    public override string ToString()
    {
      return this.delay.ToString() + "," + this.precision.ToString();
    }
  }
}

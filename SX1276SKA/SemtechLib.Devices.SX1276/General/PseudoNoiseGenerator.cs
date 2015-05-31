// Decompiled with JetBrains decompiler
// Type: SemtechLib.Devices.SX1276.General.PseudoNoiseGenerator
// Assembly: SemtechLib.Devices.SX1276, Version=2.0.0.0, Culture=neutral, PublicKeyToken=a3cee8af388a4f11
// MVID: 64B2382D-7AA3-4D8B-BE9D-2E742AB27E64
// Assembly location: C:\Tools\Semtech\SX1276SKA\SemtechLib.Devices.SX1276.dll

using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.General
{
  public class PseudoNoiseGenerator
  {
    private ushort initialValue = ushort.MaxValue;
    private PnSequence pn = PnSequence.PN15;
    private ushort lfsr;
    private uint period;

    public ushort InitialValue
    {
      get
      {
        return this.initialValue;
      }
      set
      {
        this.initialValue = value;
        this.lfsr = this.initialValue;
        this.period = 0U;
      }
    }

    public PnSequence Pn
    {
      get
      {
        return this.pn;
      }
      set
      {
        this.pn = value;
        this.lfsr = this.initialValue;
        this.period = 0U;
      }
    }

    public PseudoNoiseGenerator()
    {
      this.lfsr = ushort.MaxValue;
      this.period = 0U;
      this.pn = PnSequence.PN15;
    }

    public PseudoNoiseGenerator(ushort initalValue, PnSequence pnSequence)
    {
      this.InitialValue = this.initialValue;
      this.lfsr = this.InitialValue;
      this.period = 0U;
      this.pn = pnSequence;
    }

    public byte NextBit()
    {
      byte num = (byte) ((uint) this.lfsr & 1U);
      switch (this.pn)
      {
        case PnSequence.PN9:
          this.lfsr = (ushort) ((ulong) ((int) this.lfsr >> 1) ^ (ulong) -((uint) this.lfsr & 1U) & 544UL);
          break;
        case PnSequence.PN15:
          this.lfsr = (ushort) ((ulong) ((int) this.lfsr >> 1) ^ (ulong) -((uint) this.lfsr & 1U) & 49152UL);
          break;
        case PnSequence.PN16:
          this.lfsr = (ushort) ((ulong) ((int) this.lfsr >> 1) ^ (ulong) -((uint) this.lfsr & 1U) & 46080UL);
          break;
      }
      ++this.period;
      return num;
    }

    public byte NextByte()
    {
      byte num = (byte) 0;
      for (int index = 0; index < 8; ++index)
        num = (byte) ((uint) (byte) ((uint) num | (uint) this.NextBit()) << 1);
      return num;
    }
  }
}

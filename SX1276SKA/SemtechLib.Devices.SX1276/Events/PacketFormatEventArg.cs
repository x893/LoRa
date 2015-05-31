// Decompiled with JetBrains decompiler
// Type: SemtechLib.Devices.SX1276.Events.PacketFormatEventArg
// Assembly: SemtechLib.Devices.SX1276, Version=2.0.0.0, Culture=neutral, PublicKeyToken=a3cee8af388a4f11
// MVID: 64B2382D-7AA3-4D8B-BE9D-2E742AB27E64
// Assembly location: C:\Tools\Semtech\SX1276SKA\SemtechLib.Devices.SX1276.dll

using SemtechLib.Devices.SX1276.Enumerations;
using System;

namespace SemtechLib.Devices.SX1276.Events
{
  public class PacketFormatEventArg : EventArgs
  {
    private PacketFormatEnum value;

    public PacketFormatEnum Value
    {
      get
      {
        return this.value;
      }
    }

    public PacketFormatEventArg(PacketFormatEnum value)
    {
      this.value = value;
    }
  }
}

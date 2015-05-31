// Decompiled with JetBrains decompiler
// Type: SemtechLib.Devices.SX1276.Events.TimerResolutionEventHandler
// Assembly: SemtechLib.Devices.SX1276, Version=2.0.0.0, Culture=neutral, PublicKeyToken=a3cee8af388a4f11
// MVID: 64B2382D-7AA3-4D8B-BE9D-2E742AB27E64
// Assembly location: C:\Tools\Semtech\SX1276SKA\SemtechLib.Devices.SX1276.dll

using System;
using System.Runtime.InteropServices;

namespace SemtechLib.Devices.SX1276.Events
{
  [ComVisible(true)]
  [Serializable]
  public delegate void TimerResolutionEventHandler(object sender, TimerResolutionEventArg e);
}

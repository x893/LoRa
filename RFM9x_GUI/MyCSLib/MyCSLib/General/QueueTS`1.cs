// Decompiled with JetBrains decompiler
// Type: MyCSLib.General.QueueTS`1
// Assembly: MyCSLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 70B056C4-DB31-49BD-9F27-15818C65F327
// Assembly location: C:\Tools\HopeRF\RFM9x_GUI_1_1_0_9\MyCSLib.dll

using System;
using System.Collections.Generic;
using System.Threading;

namespace MyCSLib.General
{
  public class QueueTS<T> : Queue<T>, IDisposable
  {
    private object sync;
    private bool isDisposed;

    public new int Count
    {
      get
      {
        lock (this.sync)
          return base.Count;
      }
    }

    public QueueTS()
    {
      this.sync = new object();
      this.isDisposed = false;
    }

    public QueueTS(IEnumerable<T> collection)
      : base(collection)
    {
      this.sync = new object();
      this.isDisposed = false;
    }

    public QueueTS(int capacity)
      : base(capacity)
    {
      this.sync = new object();
      this.isDisposed = false;
    }

    public new void Clear()
    {
      lock (this.sync)
        base.Clear();
    }

    public new T Dequeue()
    {
      lock (this.sync)
        return base.Dequeue();
    }

    public new void Enqueue(T item)
    {
      lock (this.sync)
      {
        base.Enqueue(item);
        Monitor.Pulse(this.sync);
      }
    }

    public void Dispose()
    {
      if (this.isDisposed)
        return;
      this.isDisposed = true;
      lock (this.sync)
      {
        base.Clear();
        Monitor.PulseAll(this.sync);
      }
    }
  }
}

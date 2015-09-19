using System;
using System.Collections.Generic;
using System.Threading;

namespace MyCSLib.General
{
	public class QueueTS<T> : Queue<T>, IDisposable
	{
		private object m_sync;
		private bool m_isDisposed;

		public new int Count
		{
			get
			{
				lock (m_sync)
					return base.Count;
			}
		}

		public QueueTS()
		{
			m_sync = new object();
			m_isDisposed = false;
		}

		public QueueTS(IEnumerable<T> collection)
			: base(collection)
		{
			m_sync = new object();
			m_isDisposed = false;
		}

		public QueueTS(int capacity)
			: base(capacity)
		{
			m_sync = new object();
			m_isDisposed = false;
		}

		public new void Clear()
		{
			lock (m_sync)
				base.Clear();
		}

		public new T Dequeue()
		{
			lock (m_sync)
				return base.Dequeue();
		}

		public new void Enqueue(T item)
		{
			lock (m_sync)
			{
				base.Enqueue(item);
				Monitor.Pulse(m_sync);
			}
		}

		public void Dispose()
		{
			if (m_isDisposed)
				return;
			m_isDisposed = true;
			lock (m_sync)
			{
				base.Clear();
				Monitor.PulseAll(m_sync);
			}
		}
	}
}

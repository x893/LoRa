using System.Collections.Generic;

namespace MyCSLib.General
{
	public class CircularQueue<T> : Queue<T>
	{
		public int MaxLength { get; set; }

		public CircularQueue(int maxLength)
		{
			this.MaxLength = maxLength;
		}

		public void Add(T item)
		{
			if (this.Count < this.MaxLength)
			{
				this.Enqueue(item);
			}
			else
			{
				this.Dequeue();
				this.Enqueue(item);
			}
		}
	}
}

using System.Collections.Generic;

namespace MyCSLib.General
{
	public class CircularQueue<T> : Queue<T>
	{
		public int MaxLength { get; set; }

		public CircularQueue(int maxLength)
		{
			MaxLength = maxLength;
		}

		public void Add(T item)
		{
			if (Count < MaxLength)
				Enqueue(item);
			else
			{
				Dequeue();
				Enqueue(item);
			}
		}
	}
}

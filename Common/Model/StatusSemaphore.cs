using System.Threading;
using Common.Enums;

namespace Common.Model
{
	public class StatusSemaphore : SemaphoreSlim 
	{
		public NotifyStatus Status { get; set; }

		public StatusSemaphore(int initialCount) : base(initialCount)
		{
		}

		public StatusSemaphore(int initialCount, int maxCount) : base(initialCount, maxCount)
		{
		}
	}
}
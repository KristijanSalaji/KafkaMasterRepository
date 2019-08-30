using System;
using Common.Enums;

namespace Common.Model
{
	public class NotifyEventArgs : EventArgs
	{
		public NotifyStatus NotifyStatus { get; set; }

		public NotifyEventArgs(NotifyStatus notifyStatus)
		{
			NotifyStatus = notifyStatus;
		}
	}
}
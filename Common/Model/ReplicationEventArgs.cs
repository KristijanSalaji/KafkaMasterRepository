using System;

namespace Common.Model
{
	public class ReplicationEventArgs<R> : EventArgs
	{
		public R Replica { get; set; }

		public ReplicationEventArgs(R replica)
		{
			Replica = replica;
		}
	}
}
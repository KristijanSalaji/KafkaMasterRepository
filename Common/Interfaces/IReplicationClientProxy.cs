using System;
using Common.Model;

namespace Common.Interfaces
{
	public interface IReplicationClientProxy<R> : IReplicationClient<R>, IReplicationClientCallback<R>, IInitializeProxy
	{
		event EventHandler<ReplicationEventArgs<R>> DeliverReplicaEvent;
	}
}
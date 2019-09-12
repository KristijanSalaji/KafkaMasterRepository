using System;
using Common.Model;
using Common.Model.CustomEventHandler;

namespace Common.Interfaces
{
	public interface IReplicationClientProxy<R> : IReplicationClient<R>, IReplicationClientCallback<R>, IInitializeProxy
	{
		event EventHandler<ReplicationEventArgs<R>> DeliverReplicaEvent;

		event ByteEventHandler RequestIntegrityUpdateEvent;
	}
}
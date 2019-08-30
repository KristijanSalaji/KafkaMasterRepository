using System;
using System.ServiceModel;
using Common.Model;

namespace Common.Interfaces
{
	[ServiceContract]
	public interface IReplicationServiceProxy<R> : IReplicationService, IReplicationServiceCallback<R>, IInitializeProxy
	{
		event EventHandler<ReplicationEventArgs<R>> ForwardReplicaEvent;
	}
}
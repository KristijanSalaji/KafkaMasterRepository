using System.ServiceModel;

namespace Common.Interfaces
{
	[ServiceContract]
	public interface IReplicationServiceCallback<R>
	{
		[OperationContract]
		bool ForwardReplica(R replication);
	}
}
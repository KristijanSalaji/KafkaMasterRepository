using System.ServiceModel;

namespace Common.Interfaces
{
	[ServiceContract]
	public interface IReplicationClientCallback<R>
	{
		[OperationContract]
		byte[] GetIntegrityUpdate();

		[OperationContract]
		bool DeliverReplica(R replication);
	}
}
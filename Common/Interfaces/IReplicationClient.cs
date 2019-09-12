using System.ServiceModel;
using System.ServiceModel.Channels;
using Common.Enums;
using Common.Model;

namespace Common.Interfaces
{
	[ServiceContract(CallbackContract = typeof(IReplicationClientCallback<Message<Topic>>))]
	public interface IReplicationClient<R>
	{
		[OperationContract]
		bool RegisterToReplicationService();

		[OperationContract]
		bool SendReplica(R replication);

		[OperationContract]
		byte[] RequestIntegrityUpdate();
	}
}
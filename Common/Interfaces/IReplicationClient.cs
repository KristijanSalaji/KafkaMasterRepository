using System.ServiceModel;

namespace Common.Interfaces
{
	[ServiceContract]
	public interface IReplicationClient<R>
	{
		[OperationContract]
		bool RegisterToReplicationService();

		#region Hot

		[OperationContract]
		bool SendReplica(R replication);

		#endregion

		#region StandBy

		[OperationContract]
		byte[] RequestIntegrityUpdate();

		#endregion
	}
}
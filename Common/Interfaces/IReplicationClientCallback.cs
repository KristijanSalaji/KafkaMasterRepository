using System.ServiceModel;

namespace Common.Interfaces
{
	[ServiceContract]
	public interface IReplicationClientCallback<R>
	{
		#region Hot

		[OperationContract]
		byte[] GetIntegrityUpdate();

		#endregion

		#region StandBy

		[OperationContract]
		bool DeliverReplica(R replication);

		#endregion
	}
}
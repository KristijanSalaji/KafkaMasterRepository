using System.ServiceModel;

namespace Common.Interfaces
{
	[ServiceContract]
	public interface IReplicationService
	{
		[OperationContract]
		void IntegrityUpdate();
	}
}
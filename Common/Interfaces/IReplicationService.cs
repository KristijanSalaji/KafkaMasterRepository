using System.Collections.Generic;
using System.ServiceModel;

namespace Common.Interfaces
{
	[ServiceContract(CallbackContract = typeof(IReplicationServiceCallback<>))]
	public interface IReplicationService
	{
		[OperationContract]
		byte[] ForwardIntegrityUpdate();

		[OperationContract]
		bool RegisterToPartner();
	}
}
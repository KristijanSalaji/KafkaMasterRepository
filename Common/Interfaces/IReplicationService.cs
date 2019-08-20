using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Common.Enums;
using Common.Model;

namespace Common.Interfaces
{
	[ServiceContract(CallbackContract = typeof(IReplicationServiceCallback<Message<KafkaTopic>>))]
	public interface IReplicationService
	{
		[OperationContract]
		byte[] ForwardIntegrityUpdate();

		[OperationContract]
		bool RegisterToPartner();
	}
}
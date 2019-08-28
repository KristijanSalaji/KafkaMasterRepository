using System.Collections.Generic;
using Common.Model;
using System.ServiceModel;
using Common.Enums;

namespace Common.Interfaces
{
	[ServiceContract(CallbackContract = typeof(INotifyCallback))]
	public interface IProducer<T>
	{
		[OperationContract]
		NotifyStatus PublishSync(Message<T> message);

		[OperationContract(IsOneWay = true)]
		void PublishAsync(Message<T> message);
	}
}
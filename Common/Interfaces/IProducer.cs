using System.Collections.Generic;
using Common.Model;
using System.ServiceModel;

namespace Common.Interfaces
{
	[ServiceContract]
	public interface IProducer<T>
	{
		[OperationContract(IsOneWay = true)]
		void PublishAsync(Message<T> message);

		[OperationContract(IsOneWay = true)]
		void PublishStreamAsync(List<Message<T>> messages);

		[OperationContract]
		bool PublishSync(Message<T> message);

		[OperationContract]
		bool PublishStreamSync(List<Message<T>> messages);
	}
}
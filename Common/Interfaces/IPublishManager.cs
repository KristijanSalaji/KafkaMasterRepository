using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using Common.Enums;
using Common.Model;

namespace Common.Interfaces
{
	[ServiceContract]
	public interface IPublishManager<T>
	{
		[OperationContract(IsOneWay = true)]
		void PublishSync(Message<T> message);

		[OperationContract(IsOneWay = true)]
		void PublishAsync(Message<T> message);
	}
}
using System;
using System.ServiceModel;

namespace Common.Interfaces
{
	[ServiceContract]
	public interface IBrokerPublishProxy<T> : IProducer<T>, INotifyCallback, IInitializeProxy
	{
		
	}
}
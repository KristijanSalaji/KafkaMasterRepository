using System.ServiceModel;
using Common.Enums;
using Common.Model;

namespace Common.Interfaces
{
	[ServiceContract]
	public interface IBroker<T> : IProducer<T>, IConsumer<T>
	{
		
	}
}
using System.ServiceModel;

namespace Common.Interfaces
{
	[ServiceContract]
	public interface IBrokerRequestProxy<T> : IConsumer<T>, IInitializeProxy
	{
		
	}
}
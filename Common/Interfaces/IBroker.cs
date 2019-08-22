using System.ServiceModel;
using Common.Enums;
using Common.Model;

namespace Common.Interfaces
{
	[ServiceContract(CallbackContract = typeof(INotifyCallback))]
	public interface IBroker<T> : IProducer<T>, IConsumer<T>
	{
		
	}
}
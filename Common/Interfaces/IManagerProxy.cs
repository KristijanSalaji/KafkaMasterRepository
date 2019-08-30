using System.ServiceModel;

namespace Common.Interfaces
{
	[ServiceContract]
	public interface IManagerProxy<T> : IPublishManager<T>, INotifyCallback, IInitializeProxy
	{
		
	}
}
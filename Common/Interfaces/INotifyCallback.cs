using System.ServiceModel;
using Common.Enums;

namespace Common.Interfaces
{
	[ServiceContract]
	public interface INotifyCallback
	{
		[OperationContract(IsOneWay = true)]
		void Notify(NotifyStatus status);
	}
}
using System.ServiceModel;

namespace Common.Interfaces
{
	[ServiceContract]
	public interface INotifyCallback
	{
		[OperationContract(IsOneWay = true)]
		void Notify(string message);
	}
}
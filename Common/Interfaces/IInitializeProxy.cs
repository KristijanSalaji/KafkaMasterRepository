using System.ServiceModel;

namespace Common.Interfaces
{
	[ServiceContract]
	public interface IInitializeProxy
	{
		[OperationContract]
		void Initialize(string ipAddress, string port, string endpoint);
	}
}
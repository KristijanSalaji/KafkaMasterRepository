using System;
using System.ServiceModel;
using Common.Enums;
using Common.Model;

namespace Common.Interfaces
{
	[ServiceContract]
	public interface INotifyCallback
	{
		[OperationContract(IsOneWay = true)]
		void Notify(NotifyStatus status);

		//delegate void NotifyDelegate(NotifyStatus status);

		event EventHandler<NotifyEventArgs> NotifyEvent;
	}
}
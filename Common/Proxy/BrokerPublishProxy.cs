using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Common.Enums;
using Common.Interfaces;
using Common.Model;

namespace Common.Proxy
{
	public class BrokerPublishProxy<T> : IBrokerPublishProxy<T>
	{
		private IProducer<T> proxy;

		public event EventHandler<NotifyEventArgs> NotifyEvent;

		//#region Notify event

		//public delegate void NotifyDelegate(NotifyStatus status);

		//private event NotifyDelegate notifyEvent;

		//public event NotifyDelegate NotifyEvent
		//{
		//	add
		//	{
		//		if (notifyEvent == null || !notifyEvent.GetInvocationList().Contains(value))
		//		{
		//			notifyEvent += value;
		//		}
		//	}
		//	remove { notifyEvent -= value; }
		//}

		//#endregion

		public BrokerPublishProxy()
		{
				
		}

		public void Initialize(string ipAddress, string port, string endpoint)
		{
			var factory = new DuplexChannelFactory<IProducer<T>>(this,
				new NetTcpBinding() { OpenTimeout = TimeSpan.MaxValue },
				new EndpointAddress($"net.tcp://{ipAddress}:{port}/Broker/{endpoint}"));

			proxy = factory.CreateChannel();
		}

		public void Notify(NotifyStatus status)
		{
			if (NotifyEvent != null)
			{
				NotifyEvent.Invoke(this, new NotifyEventArgs(status));
			}
		}

		public virtual void PublishAsync(Message<T> message)
		{
			try
			{
				proxy.PublishAsync(message);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Exception while publishing message: {e.Message}");
				throw;
			}
		}

		public virtual NotifyStatus PublishSync(Message<T> message)
		{
			try
			{
				return proxy.PublishSync(message);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Exception while publishing message: {e.Message}");
				throw;
			}
		}
	}
}
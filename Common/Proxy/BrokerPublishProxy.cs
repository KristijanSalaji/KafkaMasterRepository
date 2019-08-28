using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Common.Enums;
using Common.Interfaces;
using Common.Model;

namespace Common.Proxy
{
	public class BrokerPublishProxy<T> : IProducer<T>, INotifyCallback
	{
		private IProducer<T> proxy;

		#region Notify event

		public delegate void NotifyDelegate(NotifyStatus status);

		private event NotifyDelegate notifyEvent;

		public event NotifyDelegate NotifyEvent
		{
			add
			{
				if (notifyEvent == null || !notifyEvent.GetInvocationList().Contains(value))
				{
					notifyEvent += value;
				}
			}
			remove { notifyEvent -= value; }
		}

		#endregion

		public BrokerPublishProxy()
		{
				
		}

		public void Initialize(string ipAddress, string port, string endpoint)
		{
			var factory = new DuplexChannelFactory<IBroker<T>>(this,
				new NetTcpBinding() { OpenTimeout = TimeSpan.MaxValue },
				new EndpointAddress($"net.tcp://{ipAddress}:{port}/Broker/{endpoint}"));

			proxy = factory.CreateChannel();
		}

		public void Notify(NotifyStatus status)
		{
			if (notifyEvent != null)
			{
				notifyEvent.Invoke(status);
			}
		}

		public void PublishAsync(Message<T> message)
		{
			try
			{
				proxy.PublishAsync(message);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Excpetion while publishing message: {e.Message}");
				throw;
			}
		}

		public NotifyStatus PublishSync(Message<T> message)
		{
			try
			{
				return proxy.PublishSync(message);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Excpetion while publishing message: {e.Message}");
				throw;
			}
		}
	}
}
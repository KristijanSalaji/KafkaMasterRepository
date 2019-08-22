using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Common.Interfaces;
using Common.Model;

namespace Common.Proxy
{
	public class ProducerProxy<T> : IProducer<T>, INotifyCallback
	{
		private readonly IProducer<T> proxy;

		#region Notify event

		public delegate void NotifyDelegate(string message);

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

		public ProducerProxy(string ipAddress, string port, string endpoint)
		{
			var factory = new DuplexChannelFactory<IBroker<T>>(this,
				new NetTcpBinding() { OpenTimeout = TimeSpan.MaxValue },
				new EndpointAddress($"net.tcp://{ipAddress}:{port}/Broker/{endpoint}"));

			proxy = factory.CreateChannel();
		}

		public void Notify(string message)
		{
			if (notifyEvent != null)
			{
				notifyEvent.Invoke(message);
			}
		}

		public void Publish(Message<T> message)
		{
			try
			{
				proxy.Publish(message);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Excpetion while publishing message: {e.Message}");
				throw;
			}
		}

		public bool PublishStream(List<Message<T>> messages)
		{
			try
			{
				return proxy.PublishStream(messages);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Excpetion while publishing message stream: {e.Message}");
				return false;
			}
		}
	}
}
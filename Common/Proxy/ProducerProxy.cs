using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Common.Enums;
using Common.Interfaces;
using Common.Model;

namespace Common.Proxy
{
	public class ProducerProxy<T> : IProducer<T>, INotifyCallback
	{
		private readonly IProducer<T> proxy;

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

		#region Notify Stream event

		//TODO list status

		public delegate void NotifyStreamDelegate(NotifyStatus status);

		private event NotifyStreamDelegate notifyStreamEvent;

		public event NotifyStreamDelegate NotifyStreamEvent
		{
			add
			{
				if (notifyStreamEvent == null || !notifyStreamEvent.GetInvocationList().Contains(value))
				{
					notifyStreamEvent += value;
				}
			}
			remove { notifyStreamEvent -= value; }
		}

		#endregion

		public ProducerProxy(string ipAddress, string port, string endpoint)
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

		public void NotifyStream(NotifyStatus status)
		{
			if (notifyStreamEvent != null)
			{
				notifyStreamEvent.Invoke(status);
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

		public void PublishStreamAsync(List<Message<T>> messages)
		{
			try
			{
				proxy.PublishStreamAsync(messages);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Excpetion while publishing message stream: {e.Message}");
				throw ;
			}
		}

		public bool PublishSync(Message<T> message)
		{
			throw new NotImplementedException();
		}

		public bool PublishStreamSync(List<Message<T>> messages)
		{
			throw new NotImplementedException();
		}
	}
}
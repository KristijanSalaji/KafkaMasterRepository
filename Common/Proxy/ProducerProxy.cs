using System;
using System.Collections.Generic;
using System.ServiceModel;
using Common.Interfaces;
using Common.Model;

namespace Common.Proxy
{
	public class ProducerProxy<T> : IProducer<T>
	{
		private readonly IProducer<T> proxy;

		public ProducerProxy(string ipAddress, string port, string endpoint)
		{
			var factory = new ChannelFactory<IBroker<T>>(
				new NetTcpBinding() { OpenTimeout = TimeSpan.MaxValue },
				new EndpointAddress($"net.tcp://{ipAddress}:{port}/Broker/{endpoint}"));

			proxy = factory.CreateChannel();
		}

		public bool Publish(Message<T> message)
		{
			try
			{
				return proxy.Publish(message);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Excpetion while publishing message: {e.Message}");
				return false;
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
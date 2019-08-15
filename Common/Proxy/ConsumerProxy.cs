using System;
using System.Collections.Generic;
using System.ServiceModel;
using Common.Interfaces;
using Common.Model;

namespace Common.Proxy
{
	public class ConsumerProxy<T> : IConsumer<T>
	{
		private readonly IConsumer<T> proxy;

		public ConsumerProxy(string ipAddress, string port, string endpoint)
		{
			var factory = new ChannelFactory<IBroker<T>>(
				new NetTcpBinding() { OpenTimeout = TimeSpan.MaxValue },
				new EndpointAddress($"net.tcp://{ipAddress}:{port}/Broker/{endpoint}"));

			proxy = factory.CreateChannel();
		}

		public Message<T> Request(SingleRequest<T> request)
		{
			try
			{
				return proxy.Request(request);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Excpetion while processing request: {e.Message}");
				return null;
			}
		}

		public List<Message<T>> RequestStream(StreamRequest<T> request)
		{
			try
			{
				return proxy.RequestStream(request);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Excpetion while processing stream request: {e.Message}");
				return null;
			}
		}
	}
}
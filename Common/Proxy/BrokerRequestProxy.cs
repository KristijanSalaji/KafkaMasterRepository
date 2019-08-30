using System;
using System.Collections.Generic;
using System.ServiceModel;
using Common.Interfaces;
using Common.Model;

namespace Common.Proxy
{
	public class BrokerRequestProxy<T> : IBrokerRequestProxy<T>
	{
		private IConsumer<T> proxy;

		public BrokerRequestProxy()
		{
				
		}

		public void Initialize(string ipAddress, string port, string endpoint)
		{
			var factory = new ChannelFactory<IConsumer<T>>(
				new NetTcpBinding() { OpenTimeout = TimeSpan.MaxValue },
				new EndpointAddress($"net.tcp://{ipAddress}:{port}/Broker/{endpoint}"));

			proxy = factory.CreateChannel();
		}

		public Message<T> SingleRequest(SingleRequest<T> request)
		{
			try
			{
				return proxy.SingleRequest(request);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Exception while processing request: {e.Message}");
				return null;
			}
		}

		public List<Message<T>> MultipleRequest(MultipleRequest<T> request)
		{
			try
			{
				return proxy.MultipleRequest(request);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Exception while processing stream request: {e.Message}");
				return null;
			}
		}
	}
}
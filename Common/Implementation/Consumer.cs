using System;
using System.Collections.Generic;
using System.Configuration;
using Common.Interfaces;
using Common.Model;
using Common.Proxy;

namespace Common.Implementation
{
	public class Consumer<T> : IConsumer<T>
	{
		private readonly IBrokerRequestProxy<T> brokerRequestProxy;

		public Consumer()
		{
			var ipAddress = ConfigurationManager.AppSettings["ipAddress"];
			var port = ConfigurationManager.AppSettings["port"];
			var endpoint = ConfigurationManager.AppSettings["endpoint"];

			brokerRequestProxy = new BrokerRequestProxy<T>();
			brokerRequestProxy.Initialize(ipAddress, port, endpoint);
		}

		public Message<T> SingleRequest(SingleRequest<T> request)
		{
			try
			{
				return brokerRequestProxy.SingleRequest(request);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Exception while sending request: {e.Message}");
				return null;
			}
		}

		public List<Message<T>> MultipleRequest(MultipleRequest<T> request)
		{
			try
			{
				return brokerRequestProxy.MultipleRequest(request);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Exception while sending request: {e.Message}");
				return null;
			}
		}
	}
}
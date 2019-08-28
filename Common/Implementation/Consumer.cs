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
		private readonly BrokerRequestProxy<T> brokerRequestProxy;

		public Consumer()
		{
			var ipAddress = ConfigurationManager.AppSettings["ipAddress"];
			var port = ConfigurationManager.AppSettings["port"];
			var endpoint = ConfigurationManager.AppSettings["endpoint"];

			brokerRequestProxy = new BrokerRequestProxy<T>();
			brokerRequestProxy.Initialize(ipAddress, port, endpoint);
		}

		public Message<T> Request(SingleRequest<T> request)
		{
			try
			{
				return brokerRequestProxy.Request(request);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Exception while sending request: {e.Message}");
				return null;
			}
		}

		public List<Message<T>> RequestStream(StreamRequest<T> request)
		{
			try
			{
				return brokerRequestProxy.RequestStream(request);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Exception while sending request: {e.Message}");
				return null;
			}
		}
	}
}
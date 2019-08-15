using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using Common.Interfaces;
using Common.Model;
using Common.Proxy;

namespace ProducerApp
{
	public class Producer<T> : IProducer<T>
	{
		private readonly ProducerProxy<T> proxy;
		public Producer()
		{
			var ipAddress = ConfigurationManager.AppSettings["ipAddress"];
			var port = ConfigurationManager.AppSettings["port"];
			var endpoint = ConfigurationManager.AppSettings["endpoint"];

			proxy = new ProducerProxy<T>(ipAddress, port, endpoint); 
		}

		public bool Publish(Message<T> message)
		{
			try
			{
				return proxy.Publish(message);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Exception while publishing message from producer: {e.Message}");
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
				Console.WriteLine($"Exception while publishing message from producer: {e.Message}");
				return false;
			}
		}
	}
}
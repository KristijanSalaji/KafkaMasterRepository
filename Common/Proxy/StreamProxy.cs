using System;
using System.Collections.Generic;
using System.ServiceModel;
using Common.Enums;
using Common.Interfaces;
using Common.Model;

namespace Common.Proxy
{
	public class StreamProxy<T> : IBroker<T>
	{
		private readonly IBroker<T> proxy;

		public StreamProxy(string ipAddress, string port, string endpoint)
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
				Console.WriteLine($"Excpetion while publishing message stream: {e.Message}");
				return null;
			}
		}
	}
}
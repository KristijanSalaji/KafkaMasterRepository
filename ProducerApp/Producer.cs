using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using Common.Interfaces;
using Common.Model;
using Common.Proxy;

namespace ProducerApp
{
	public class Producer<T> : IProducer<T>
	{
		private readonly ProducerProxy<T> proxy;
		private readonly Semaphore notifySemaphore;

		public Producer()
		{
			var ipAddress = ConfigurationManager.AppSettings["ipAddress"];
			var port = ConfigurationManager.AppSettings["port"];
			var endpoint = ConfigurationManager.AppSettings["endpoint"];

			notifySemaphore = new Semaphore(0,1);

			proxy = new ProducerProxy<T>(ipAddress, port, endpoint);
			proxy.NotifyEvent += ProxyOnNotifyEvent;
		}

		private void ProxyOnNotifyEvent(string message)
		{
			Console.WriteLine("Notify client with data: " + message);
			notifySemaphore.Release(1);
		}

		public void Publish(Message<T> message)
		{
			try
			{
				proxy.Publish(message);
				notifySemaphore.WaitOne();
			}
			catch (Exception e)
			{
				Console.WriteLine($"Exception while publishing message from producer: {e.Message}");
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
				Console.WriteLine($"Exception while publishing message from producer: {e.Message}");
				return false;
			}
		}
	}
}
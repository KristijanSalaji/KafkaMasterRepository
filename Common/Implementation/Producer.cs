using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using Common.Enums;
using Common.Interfaces;
using Common.Model;
using Common.Proxy;

namespace Common.Implementation
{
	public class Producer<T> : IProducer<T>
	{
		private readonly ProducerProxy<T> proxy;
		private readonly Semaphore notifySemaphore;
		private readonly Semaphore notifyStreamSemaphore;

		public Producer()
		{
			var ipAddress = ConfigurationManager.AppSettings["ipAddress"];
			var port = ConfigurationManager.AppSettings["port"];
			var endpoint = ConfigurationManager.AppSettings["endpoint"];

			notifySemaphore = new Semaphore(0, 1);
			notifyStreamSemaphore = new Semaphore(0, 1);

			proxy = new ProducerProxy<T>(ipAddress, port, endpoint);
			proxy.NotifyEvent += ProxyOnNotifyEvent;
			proxy.NotifyStreamEvent += ProxyOnNotifyStreamEvent;
		}

		private void ProxyOnNotifyEvent(NotifyStatus status)
		{
			Console.WriteLine("Notify client with status " + status);

			if (status == NotifyStatus.Secceeded)
			{
				notifySemaphore.Release(1);
			}
			else
			{
				//TODO logika kada je status failed
			}
		}

		private void ProxyOnNotifyStreamEvent(NotifyStatus status)
		{
			if (status == NotifyStatus.Secceeded)
			{
				notifyStreamSemaphore.Release(1);
			}
			else
			{
				//TODO logika kada je stream status failed
			}
		}

		public void PublishAsync(Message<T> message)
		{
			try
			{
				proxy.PublishAsync(message);
				notifySemaphore.WaitOne();
			}
			catch (Exception e)
			{
				Console.WriteLine($"Exception while publishing message from producer: {e.Message}");
				throw;
			}
		}

		public void PublishStreamAsync(List<Message<T>> messages)
		{
			try
			{
				proxy.PublishStreamAsync(messages);
				notifyStreamSemaphore.WaitOne();
			}
			catch (Exception e)
			{
				Console.WriteLine($"Exception while publishing message from producer: {e.Message}");
				throw;
			}
		}

		public bool PublishSync(Message<T> message)
		{
			try
			{
				return proxy.PublishSync(message); }
			catch (Exception e)
			{
				Console.WriteLine($"Exception while publishing message from producer: {e.Message}");
				return false;
			}
		}

		public bool PublishStreamSync(List<Message<T>> messages)
		{
			try
			{
				return proxy.PublishStreamSync(messages);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Exception while publishing message from producer: {e.Message}");
				return false;
			}
		}
	}
}
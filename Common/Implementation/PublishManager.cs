using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using Common.Enums;
using Common.Interfaces;
using Common.Model;
using Common.Proxy;
using Common.CallbackHandler;

namespace Common.Implementation
{
	public class PublishManager<T> : IPublishManager<T>
	{
		public bool SendData { get; set; }

		private readonly Queue<Message<T>> asyncQueue;
		private ProducerProxy<T> proxy;
		private readonly Semaphore notifySemaphore;
		private readonly CallbackHandler<INotifyCallback> producerCallbackHandler;

		public PublishManager()
		{
			SendData = true;

			producerCallbackHandler = new CallbackHandler<INotifyCallback>();

			notifySemaphore = new Semaphore(0,1);

			asyncQueue = new Queue<Message<T>>();

			var sendThread = new Thread(SendDataProcess);
			sendThread.Start();
		}

		public void CreateProxy()
		{
			var ipAddress = ConfigurationManager.AppSettings["ipAddress"];
			var port = ConfigurationManager.AppSettings["port"];
			var endpoint = ConfigurationManager.AppSettings["endpoint"];

			proxy = new ProducerProxy<T>(ipAddress, port, endpoint);
			proxy.NotifyEvent += ProxyOnNotifyEvent;
		}

		private void ProxyOnNotifyEvent(NotifyStatus status)
		{
			Console.WriteLine("Notify client with status " + status);

			if (status == NotifyStatus.Secceeded)
			{
				asyncQueue.Dequeue();
			}

			notifySemaphore.Release(1);
		}

		#region IPublishManager

		public void PublishAsync(Message<T> message)
		{
			try
			{
				asyncQueue.Enqueue(message);
			}
			catch (Exception e)
			{
				Console.WriteLine("Publish async error: " + e.Message);
				throw;
			}
		}

		public void PublishSync(Message<T> message)
		{
			try
			{
				producerCallbackHandler.GetCallback().Notify(proxy.PublishSync(message));
			}
			catch (Exception e)
			{
				Console.WriteLine("Publish sync error: " + e.Message);
				throw;
			}
		}

		#endregion

		#region Private

		private void SendDataProcess()
		{
			while (SendData)
			{
				if (asyncQueue == null || asyncQueue.Count == 0)
				{
					Thread.Sleep(500);
					continue;
				}

				var message = asyncQueue.Peek();
				proxy.PublishAsync(message);
				notifySemaphore.WaitOne();
			}
		}

		#endregion
	}
}
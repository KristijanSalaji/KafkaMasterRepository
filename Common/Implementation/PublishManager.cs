using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using Common.Enums;
using Common.Interfaces;
using Common.Model;
using Common.Proxy;
using Common.CallbackHandler;
using Common.Converter;

namespace Common.Implementation
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class PublishManager<T> : IPublishManager<T>
	{
		public bool SendData { get; set; }

		private readonly Queue<Message<T>> asyncQueue;
		private BrokerPublishProxy<T> brokerPublishProxy;
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
			var ipAddress = ConfigurationManager.AppSettings["brokerIpAddress"];
			var port = ConfigurationManager.AppSettings["brokerPort"];
			var endpoint = ConfigurationManager.AppSettings["brokerEndpoint"];

			brokerPublishProxy = new BrokerPublishProxy<T>(ipAddress, port, endpoint);
			brokerPublishProxy.NotifyEvent += BrokerPublishProxyOnNotifyEvent;
		}

		private void BrokerPublishProxyOnNotifyEvent(NotifyStatus status)
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
				Console.WriteLine($"Message with data {message.Data.ToObject<string>()} successfully enqueued!");
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
				producerCallbackHandler.GetCallback().Notify(brokerPublishProxy.PublishSync(message));
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
				brokerPublishProxy.PublishAsync(message);
				notifySemaphore.WaitOne();
			}
		}

		#endregion
	}
}
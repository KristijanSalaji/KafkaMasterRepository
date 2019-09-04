using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
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
		public NotifyStatus NotifyStatus { get; set; }
		public int Waitingtime { get; set; }

		private readonly Queue<Message<T>> asyncQueue;
		private readonly Semaphore notifySemaphore;
		private const int queueCapacity = 2000000;

		private IBrokerPublishProxy<T> brokerPublishProxy;
		private readonly ICallbackHandler<INotifyCallback> producerCallbackHandler;

		public PublishManager()
		{
			SendData = true;
			NotifyStatus = NotifyStatus.Failed;
			Waitingtime = 500;

			producerCallbackHandler = new CallbackHandler<INotifyCallback>();

			notifySemaphore = new Semaphore(0,1);

			asyncQueue = new Queue<Message<T>>(queueCapacity);
		}

		#region Test constructor

		public PublishManager(IBrokerPublishProxy<T> proxy, ICallbackHandler<INotifyCallback> cbHandler, Queue<Message<T>> queue)
		{
			this.brokerPublishProxy = proxy;
			this.producerCallbackHandler = cbHandler;
			this.asyncQueue = queue;

			SendData = true;
			NotifyStatus = NotifyStatus.Failed;
			Waitingtime = 500;
			notifySemaphore = new Semaphore(0, 1);
		}

		#endregion

		public void CreateProxy()
		{
			var ipAddress = ConfigurationManager.AppSettings["brokerIpAddress"];
			var port = ConfigurationManager.AppSettings["brokerPort"];
			var endpoint = ConfigurationManager.AppSettings["brokerEndpoint"];

			brokerPublishProxy = new BrokerPublishProxy<T>();
			brokerPublishProxy.NotifyEvent += BrokerPublishProxyOnNotifyEvent;
			brokerPublishProxy.Initialize(ipAddress, port, endpoint);
		}

		public void BrokerPublishProxyOnNotifyEvent(object sender, NotifyEventArgs args)
		{
			//Console.WriteLine("Notify client with status " + args.NotifyStatus);

			if (args.NotifyStatus == NotifyStatus.Secceeded)
			{
				asyncQueue.Dequeue();
			}

			notifySemaphore.Release(1);
		}

		public void StartAsyncSendDataProcess()
		{
			var sendThread = new Thread(AsyncSendDataProcess);
			sendThread.Start();
		}

		public int GetAsyncQueueCount()
		{
			if (asyncQueue == null) return -1;

			return asyncQueue.Count;
		}

		#region IPublishManager

		public void PublishAsync(Message<T> message)
		{
			try
			{
				asyncQueue.Enqueue(message);
				//Console.WriteLine($"Message with data {message.Data.ToObject<string>()} successfully enqueued!");
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
				NotifyStatus = brokerPublishProxy.PublishSync(message);
				producerCallbackHandler.GetCallback().Notify(NotifyStatus);
				//Console.WriteLine($"Message with data {message.Data.ToObject<string>()} and status {NotifyStatus}");
			}
			catch (Exception e)
			{
				Console.WriteLine("Publish sync error: " + e.Message);
				throw;
			}
		}

		#endregion

		public void AsyncSendDataProcess()
		{
			while (SendData)
			{
				if (asyncQueue == null || asyncQueue.Count == 0)
				{
					Thread.Sleep(Waitingtime);
					continue;
				}

				try
				{
					var message = asyncQueue.Peek();
					brokerPublishProxy.PublishAsync(message);
					notifySemaphore.WaitOne();
				}
				catch (Exception e)
				{
					Console.WriteLine($"Error while sending async message from queue: {e.Message}");
					throw;
				}
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security;
using System.Threading;
using Common.Enums;
using Common.Interfaces;
using Common.Model;
using Common.Proxy;

namespace Common.Implementation
{
	public class Producer<T> : IProducer<T>
	{
		private readonly IManagerProxy<T> managerProxy;

		private readonly StatusSemaphore syncSemaphore;

		public Producer()
		{
			var ipAddress = ConfigurationManager.AppSettings["ipAddress"];
			var endpoint = ConfigurationManager.AppSettings["endpoint"];
			var port = ConfigurationManager.AppSettings["port"];

			syncSemaphore = new StatusSemaphore(0,1);

			managerProxy = new ManagerProxy<T>();
			managerProxy.NotifyEvent += ManagerProxyOnNotifyEvent;
			managerProxy.Initialize(ipAddress, port, endpoint);
		}

		private void ManagerProxyOnNotifyEvent(object sender, NotifyEventArgs args)
		{
			syncSemaphore.Status = args.NotifyStatus;
			syncSemaphore.Release(1);
		}

		public void PublishAsync(Message<T> message)
		{
			try
			{
				managerProxy.PublishAsync(message);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Exception while publishing message from producer: {e.Message}");
				throw;
			}
		}

		public NotifyStatus PublishSync(Message<T> message)
		{
			try
			{
				managerProxy.PublishSync(message);
				syncSemaphore.Wait();
				return syncSemaphore.Status;
			}
			catch (Exception e)
			{
				Console.WriteLine($"Exception while publishing message from producer: {e.Message}");
				throw;
			}
		}
	}
}
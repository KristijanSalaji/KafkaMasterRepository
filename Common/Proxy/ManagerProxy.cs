﻿using System;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using Common.Enums;
using Common.Interfaces;
using Common.Model;

namespace Common.Proxy
{
	public class ManagerProxy<T> : IPublishManager<T>, INotifyCallback
	{
		private readonly IPublishManager<T> proxy;

		#region Notify event

		public delegate void NotifyDelegate(NotifyStatus status);

		private event NotifyDelegate notifyEvent;

		public event NotifyDelegate NotifyEvent
		{
			add
			{
				if (notifyEvent == null || !notifyEvent.GetInvocationList().Contains(value))
				{
					notifyEvent += value;
				}
			}
			remove { notifyEvent -= value; }
		}

		#endregion

		public ManagerProxy(string ipAddress, string port, string endpoint)
		{
			var factory = new DuplexChannelFactory<IPublishManager<T>>(this,
				new NetNamedPipeBinding() { OpenTimeout = TimeSpan.MaxValue },
				new EndpointAddress($"net.pipe://{ipAddress}:{port}/Manager/{endpoint}"));

			proxy = factory.CreateChannel();
		}

		#region Notify callback

		public void Notify(NotifyStatus status)
		{
			if (notifyEvent != null)
			{
				notifyEvent.Invoke(status);
			}
		}

		#endregion

		#region IPublishManager

		public void PublishAsync(Message<T> message)
		{
			try
			{
				proxy.PublishAsync(message);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Publish async error: {e.Message}");
				throw;
			}
		}

		public void PublishSync(Message<T> message)
		{
			try
			{
				proxy.PublishSync(message);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Publish sync error: {e.Message}");
				throw;
			}
		}

		#endregion
	}
}
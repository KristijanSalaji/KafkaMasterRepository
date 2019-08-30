using System;
using System.Collections.Generic;
using System.Threading;
using Common.CallbackHandler;
using Common.Converter;
using Common.Enums;
using Common.Implementation;
using Common.Interfaces;
using Common.Model;
using Common.Proxy;
using NSubstitute;
using NUnit.Framework;

namespace KafkaTest
{
	[TestFixture]
	public class PublishManagerTest
	{
		#region Moq

		private class ProxyMoq<T> : BrokerPublishProxy<T>
		{
			public override NotifyStatus PublishSync(Message<T> message)
			{
				if (message == null) return NotifyStatus.Failed;

				return NotifyStatus.Secceeded;
			}

			public override void PublishAsync(Message<T> message)
			{
				
			}
		}

		private class ProducerMoq : INotifyCallback
		{
			public event EventHandler<NotifyEventArgs> NotifyEvent;

			public void Notify(NotifyStatus status)
			{
				
			}
		}

		#endregion

		private PublishManager<Topic> publishManager;
		private PublishManager<Topic> invalidPublishManager;
		private Message<Topic> testMessage;

		[SetUp]
		public void Initialize()
		{
			var managerTest = new PublishManager<Topic>();

			const string messageData = "Test data";
			testMessage = new Message<Topic>() { Topic = Topic.FirstT, Data = messageData.ToByteArray() };

			var producerMoq = new ProducerMoq();
			var producerCbHandler = Substitute.For<ICallbackHandler<INotifyCallback>>();
			producerCbHandler.GetCallback().Returns(producerMoq);

			var proxyMoq = new ProxyMoq<Topic>();

			publishManager = new PublishManager<Topic>(proxyMoq,producerCbHandler,new Queue<Message<Topic>>());
			invalidPublishManager = new PublishManager<Topic>(null,null,null);
		}

		[Test]
		public void PublishAsyncDataToManager()
		{
			Assert.DoesNotThrow(() => publishManager.PublishAsync(testMessage));
			Assert.AreEqual(publishManager.GetAsyncQueueCount(), 1);
		}

		[Test]
		public void PublishAsyncDataToManagerWhenQueueIsNull()
		{
			Assert.Catch<Exception>(() => invalidPublishManager.PublishAsync(testMessage));
			Assert.AreEqual(invalidPublishManager.GetAsyncQueueCount(), -1);
		}

		[Test]
		public void PublishSyncDataToManager()
		{
			Assert.DoesNotThrow(() => publishManager.PublishSync(testMessage));
			Assert.AreEqual(publishManager.NotifyStatus, NotifyStatus.Secceeded);
		}

		[Test]
		public void PublishSyncDataToManagerWhenProxyIsNull()
		{
			Assert.Catch<Exception>(() => invalidPublishManager.PublishSync(testMessage));
			Assert.AreEqual(invalidPublishManager.NotifyStatus, NotifyStatus.Failed);
		}

		[Test]
		public void PublishAsyncDataToBroker()
		{
			publishManager.StartAsyncSendDataProcess();
			publishManager.PublishAsync(testMessage);
			Thread.Sleep(publishManager.Waitingtime);
			publishManager.BrokerPublishProxyOnNotifyEvent(this,new NotifyEventArgs(NotifyStatus.Secceeded));

			Assert.AreEqual(publishManager.GetAsyncQueueCount(), 0);
			publishManager.SendData = false;
		}

		[Test]
		public void PublishAsyncDataToBrokerFailed()
		{
			publishManager.StartAsyncSendDataProcess();
			publishManager.PublishAsync(testMessage);
			Thread.Sleep(publishManager.Waitingtime);
			publishManager.BrokerPublishProxyOnNotifyEvent(this, new NotifyEventArgs(NotifyStatus.Failed));

			Assert.AreEqual(publishManager.GetAsyncQueueCount(), 1);
			publishManager.SendData = false;
		}

		[Test]
		public void PublishAsyncDataToBrokerWhenProxyIsNull()
		{
			var localPublishManager = new PublishManager<Topic>(null, null, new Queue<Message<Topic>>());
			localPublishManager.PublishAsync(testMessage);

			Assert.AreEqual(localPublishManager.GetAsyncQueueCount(), 1);
			Assert.Catch<Exception>(() => localPublishManager.AsyncSendDataProcess());
		}
	}
}
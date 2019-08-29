using System;
using System.Collections.Generic;
using Common.CallbackHandler;
using Common.Converter;
using Common.Enums;
using Common.Implementation;
using Common.Interfaces;
using Common.Model;
using NSubstitute;
using NUnit.Framework;

namespace KafkaTest
{
	[TestFixture]
	public class BrokerTest
	{
		#region Moq

		private class ClientMoq : INotifyCallback
		{
			public void Notify(NotifyStatus status)
			{

			}
		}

		#endregion

		#region PublishSync test

		[Test]
		public void PublishWhenTopicDidNotExistTest()
		{
			var broker = new Broker<Topic>(State.StandBy);

			var testDataString = "TEST";
			var response = broker.PublishSync(new Message<Topic>()
			{ Topic = Topic.FirstT, Data = testDataString.ToByteArray() });

			Assert.AreEqual(response, NotifyStatus.Failed);
		}

		[Test]
		public void PublishMessageWithNullValue()
		{
			var broker = new Broker<Topic>(State.StandBy);

			Assert.Catch<ArgumentNullException>(() =>broker.PublishSync(null));
		}

		[Test]
		public void PublishWhenTopicExist()
		{
			var broker = new Broker<Topic>(State.StandBy);
			broker.AddTopic(Topic.FirstT);

			var testDataString = "TEST";
			var response = broker.PublishSync(new Message<Topic>()
			{ Topic = Topic.FirstT, Data = testDataString.ToByteArray() });

			Assert.AreEqual(response, NotifyStatus.Secceeded);

			response = broker.PublishSync(new Message<Topic>()
			{ Topic = Topic.FirstT, Data = testDataString.ToByteArray() });

			Assert.AreEqual(response, NotifyStatus.Secceeded);
		}

		#endregion

		#region SingleRequest test

		[Test]
		public void RequestMessageWithInvalidTopic()
		{
			var broker = new Broker<Topic>(State.StandBy);

			var message = broker.SingleRequest(new SingleRequest<Topic>() { Topic = Topic.FirstT, Offset = 0 });

			Assert.IsNull(message);
		}

		[Test]
		public void RequestMessageWithInvalidOffset()
		{
			var broker = new Broker<Topic>(State.StandBy);
			broker.AddTopic(Topic.FirstT);

			var testDataString = "TEST";
			var response = broker.PublishSync(new Message<Topic>()
			{ Topic = Topic.FirstT, Data = testDataString.ToByteArray() });

			Assert.AreEqual(response, NotifyStatus.Secceeded);

			var message = broker.SingleRequest(new SingleRequest<Topic>() { Topic = Topic.FirstT, Offset = 4 });

			Assert.IsNull(message);
		}

		[Test]
		public void RequestMessageWithValidParametars()
		{
			var broker = new Broker<Topic>(State.StandBy);
			broker.AddTopic(Topic.FirstT);

			var testDataString = "TEST";
			var response = broker.PublishSync(new Message<Topic>()
			{ Topic = Topic.FirstT, Data = testDataString.ToByteArray() });

			Assert.AreEqual(response, NotifyStatus.Secceeded);

			var message = broker.SingleRequest(new SingleRequest<Topic>() { Topic = Topic.FirstT, Offset = 0 });

			Assert.AreEqual(message.Data.ToObject<string>(), testDataString);
			Assert.AreEqual(message.Topic, Topic.FirstT);
		}

		[Test]
		public void RequestMessageWithNullParametar()
		{
			var broker = new Broker<Topic>(State.StandBy);

			Assert.Catch<ArgumentNullException>(() => broker.PublishSync(null));
		}


		[Test]
		public void RequestStream()
		{
			var broker = new Broker<Topic>(State.StandBy);
			broker.AddTopic(Topic.FirstT);

			int count = 20;

			var stream = new List<Message<Topic>>(count);

			var testDataString = "TEST";

			for (int i = 0; i < count; i++)
			{
				var message = new Message<Topic>()
				{ Topic = Topic.FirstT, Data = testDataString.ToByteArray() };

				var response = broker.PublishSync(message);

				Assert.AreEqual(response, NotifyStatus.Secceeded);
			}


			var topicCount = broker.TopicCount(Topic.FirstT);

			Assert.AreEqual(count, topicCount);

			var retVal = broker.MultipleRequest(new MultipleRequest<Topic>() { Topic = Topic.FirstT, Offset = 0, Count = 17 });

			Assert.AreEqual(retVal.Count, 17);
		}

		[Test]
		public void RequestStreamWithNullValues()
		{
			var broker = new Broker<Topic>(State.StandBy);

			var retVal = broker.MultipleRequest(null);

			Assert.IsNull(retVal);
		}

		[Test]
		public void RequestStreamWithBadCount()
		{
			var broker = new Broker<Topic>(State.StandBy);
			broker.AddTopic(Topic.FirstT);

			int count = 10;

			var stream = new List<Message<Topic>>(count);

			var testDataString = "TEST";

			for (int i = 0; i < count; i++)
			{
				var message = new Message<Topic>()
				{ Topic = Topic.FirstT, Data = testDataString.ToByteArray() };

				var response = broker.PublishSync(message);

				Assert.AreEqual(response, NotifyStatus.Secceeded);

			}

			var topicCount = broker.TopicCount(Topic.FirstT);

			Assert.AreEqual(count, topicCount);

			var retVal = broker.MultipleRequest(new MultipleRequest<Topic>() { Topic = Topic.FirstT, Offset = 0, Count = 17 });

			Assert.AreEqual(retVal.Count, count);
		}

		#endregion

		#region Topic test

		[Test]
		public void TopicCountTest()
		{
			var broker = new Broker<Topic>(State.StandBy);
			broker.AddTopic(Topic.FirstT);

			var testDataString = "TEST";
			var response = broker.PublishSync(new Message<Topic>()
			{ Topic = Topic.FirstT, Data = testDataString.ToByteArray() });

			Assert.AreEqual(response, NotifyStatus.Secceeded);
			Assert.AreEqual(1, broker.TopicCount(Topic.FirstT));
			Assert.AreEqual(-1, broker.TopicCount(Topic.SecondT));
		}

		[Test]
		public void AddTopic()
		{
			var broker = new Broker<Topic>(State.StandBy);

			var result = broker.AddTopic(Topic.FirstT);

			Assert.IsTrue(result);
		}

		[Test]
		public void DeleteTopic()
		{
			var broker = new Broker<Topic>(State.StandBy);

			var result = broker.AddTopic(Topic.FirstT);

			Assert.IsTrue(result);

			result = broker.DeleteTopic(Topic.FirstT);

			Assert.IsTrue(result);
		}

		[Test]
		public void AddTopicWhenTopicIsAlreadyAdded()
		{
			var broker = new Broker<Topic>(State.StandBy);

			var result = broker.AddTopic(Topic.FirstT);

			Assert.IsTrue(result);

			result = broker.AddTopic(Topic.FirstT);

			Assert.IsTrue(result);
		}

		[Test]
		public void DeleteTopicWhichDontExist()
		{
			var broker = new Broker<Topic>(State.StandBy);

			var result = broker.DeleteTopic(Topic.FirstT);

			Assert.IsTrue(result);
		}

		#endregion

		#region Publish async

		[Test]
		public void PublishAsyncToBroker()
		{
			var clientMoq = new ClientMoq();
			var clientCbHandler = Substitute.For<ICallbackHandler<INotifyCallback>>();
			clientCbHandler.GetCallback().Returns(clientMoq);

			var broker = new Broker<Topic>(State.StandBy, clientCbHandler);

			var testData = "Test data";
			Assert.DoesNotThrow(() => broker.PublishAsync(new Message<Topic>() {Topic = Topic.FirstT, Data = testData.ToByteArray()}));
		}

		[Test]
		public void PublishAsyncToBrokerWhenCallbackIsNull()
		{
			var broker = new Broker<Topic>(State.StandBy, null);

			var testData = "Test data";
			Assert.Catch<Exception>(() => broker.PublishAsync(new Message<Topic>() { Topic = Topic.FirstT, Data = testData.ToByteArray() }));
		}

		#endregion
	}
}
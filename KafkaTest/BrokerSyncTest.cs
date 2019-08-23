using System.Collections.Generic;
using Common.Converter;
using Common.Enums;
using Common.Implementation;
using Common.Model;
using NUnit.Framework;

namespace KafkaTest
{
	[TestFixture]
	public class BrokerSyncTest
	{
		#region PublishSync test

		[Test]
		public void PublishWhenTopicDidNotExistTest()
		{
			var broker = new Broker<Topic>(State.StandBy);

			var testDataString = "TEST";
			var response = broker.PublishSync(new Message<Topic>()
			{ Topic = Topic.FirstT, Data = testDataString.ToByteArray() });

			Assert.IsFalse(response);
		}

		[Test]
		public void PublishMessageWithNullValue()
		{
			var broker = new Broker<Topic>(State.StandBy);

			var response = broker.PublishSync(null);

			Assert.IsFalse(response);
		}

		[Test]
		public void PublishWhenTopicExist()
		{
			var broker = new Broker<Topic>(State.StandBy);
			broker.AddTopic(Topic.FirstT);

			var testDataString = "TEST";
			var response = broker.PublishSync(new Message<Topic>()
			{ Topic = Topic.FirstT, Data = testDataString.ToByteArray() });

			Assert.IsTrue(response);

			response = broker.PublishSync(new Message<Topic>()
			{ Topic = Topic.FirstT, Data = testDataString.ToByteArray() });

			Assert.IsTrue(response);
		}

		[Test]
		public void PublishStreamSync()
		{
			var broker = new Broker<Topic>(State.StandBy);
			broker.AddTopic(Topic.FirstT);

			int count = 20;

			var stream = new List<Message<Topic>>(count);

			var testDataString = "TEST";

			for (int i = 0; i < count; i++)
			{
				stream.Add(new Message<Topic>()
				{ Topic = Topic.FirstT, Data = testDataString.ToByteArray() });
			}

			var response = broker.PublishStreamSync(stream);

			Assert.IsTrue(response);

			var topicCount = broker.TopicCount(Topic.FirstT);

			Assert.AreEqual(count, topicCount);
		}

		[Test]
		public void PublishStreamWithNullValues()
		{
			var broker = new Broker<Topic>(State.StandBy);

			int count = 20;

			var stream = new List<Message<Topic>>(count);

			for (int i = 0; i < count; i++)
			{
				stream.Add(null);
			}

			var response = broker.PublishStreamSync(stream);

			Assert.IsFalse(response);

			var topicCount = broker.TopicCount(Topic.FirstT);

			Assert.AreEqual(-1, topicCount);
		}

		#endregion

		#region Request test

		[Test]
		public void RequestMessageWithInvalidTopic()
		{
			var broker = new Broker<Topic>(State.StandBy);

			var message = broker.Request(new SingleRequest<Topic>() { Topic = Topic.FirstT, Offset = 0 });

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

			Assert.IsTrue(response);

			var message = broker.Request(new SingleRequest<Topic>() { Topic = Topic.FirstT, Offset = 4 });

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

			Assert.IsTrue(response);

			var message = broker.Request(new SingleRequest<Topic>() { Topic = Topic.FirstT, Offset = 0 });

			Assert.AreEqual(message.Data.ToObject<string>(), testDataString);
			Assert.AreEqual(message.Topic, Topic.FirstT);
		}

		[Test]
		public void RequestMessageWithNullParametar()
		{
			var broker = new Broker<Topic>(State.StandBy);

			var response = broker.Request(null);

			Assert.IsNull(response);
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
				stream.Add(new Message<Topic>()
				{ Topic = Topic.FirstT, Data = testDataString.ToByteArray() });
			}

			var response = broker.PublishStreamSync(stream);

			Assert.IsTrue(response);

			var topicCount = broker.TopicCount(Topic.FirstT);

			Assert.AreEqual(count, topicCount);

			var retVal = broker.RequestStream(new StreamRequest<Topic>() { Topic = Topic.FirstT, Offset = 0, Count = 17 });

			Assert.AreEqual(retVal.Count, 17);
		}

		[Test]
		public void RequestStreamWithNullValues()
		{
			var broker = new Broker<Topic>(State.StandBy);

			var retVal = broker.RequestStream(null);

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
				stream.Add(new Message<Topic>()
				{ Topic = Topic.FirstT, Data = testDataString.ToByteArray() });
			}

			var response = broker.PublishStreamSync(stream);

			Assert.IsTrue(response);

			var topicCount = broker.TopicCount(Topic.FirstT);

			Assert.AreEqual(count, topicCount);

			var retVal = broker.RequestStream(new StreamRequest<Topic>() { Topic = Topic.FirstT, Offset = 0, Count = 17 });

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

			Assert.IsTrue(response);
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
	}
}
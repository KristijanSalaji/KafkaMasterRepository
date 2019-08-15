using System.Collections.Generic;
using System.Net.Http;
using BrokerApp;
using Common.Converter;
using Common.Enums;
using Common.Model;
using NUnit.Framework;

namespace KafkaTest
{
	[TestFixture]
	public class BrokerTest
	{
		#region Publish test

		[Test]
		public void PublishWhenTopicDidNotExistTest()
		{
			var broker = new Broker<KafkaTopic>();

			var testDataString = "TEST";
			var response = broker.Publish(new Message<KafkaTopic>()
				{Topic = KafkaTopic.FirstT, Data = testDataString.ToByteArray()});

			Assert.IsFalse(response);
		}

		[Test]
		public void PublishMessageWithNullValue()
		{
			var broker = new Broker<KafkaTopic>();

			var response = broker.Publish(null);

			Assert.IsFalse(response);
		}

		[Test]
		public void PublishWhenTopicExist()
		{
			var broker = new Broker<KafkaTopic>();
			broker.AddTopic(KafkaTopic.FirstT);

			var testDataString = "TEST";
			var response = broker.Publish(new Message<KafkaTopic>()
				{ Topic = KafkaTopic.FirstT, Data = testDataString.ToByteArray() });

			Assert.IsTrue(response);

			response = broker.Publish(new Message<KafkaTopic>()
				{ Topic = KafkaTopic.FirstT, Data = testDataString.ToByteArray() });

			Assert.IsTrue(response);
		}

		[Test]
		public void PublishStream()
		{
			var broker = new Broker<KafkaTopic>();
			broker.AddTopic(KafkaTopic.FirstT);

			int count = 20;

			var stream = new List<Message<KafkaTopic>>(count);

			var testDataString = "TEST";

			for (int i = 0; i < count; i++)
			{
				stream.Add(new Message<KafkaTopic>()
					{ Topic = KafkaTopic.FirstT, Data = testDataString.ToByteArray() });
			}

			var response = broker.PublishStream(stream);

			Assert.IsTrue(response);

			var topicCount = broker.TopicCount(KafkaTopic.FirstT);

			Assert.AreEqual(count, topicCount);
		}

		[Test]
		public void PublishStreamWithNullValues()
		{
			var broker = new Broker<KafkaTopic>();

			int count = 20;

			var stream = new List<Message<KafkaTopic>>(count);

			for (int i = 0; i < count; i++)
			{
				stream.Add(null);
			}

			var response = broker.PublishStream(stream);

			Assert.IsFalse(response);

			var topicCount = broker.TopicCount(KafkaTopic.FirstT);

			Assert.AreEqual(-1, topicCount);
		}

		#endregion

		#region Request test

		[Test]
		public void RequestMessageWithInvalidTopic()
		{
			var broker = new Broker<KafkaTopic>();

			var message = broker.Request(new SingleRequest<KafkaTopic>() {Topic = KafkaTopic.FirstT, Offset = 0});

			Assert.IsNull(message);
		}

		[Test]
		public void RequestMessageWithInvalidOffset()
		{
			var broker = new Broker<KafkaTopic>();
			broker.AddTopic(KafkaTopic.FirstT);

			var testDataString = "TEST";
			var response = broker.Publish(new Message<KafkaTopic>()
				{ Topic = KafkaTopic.FirstT, Data = testDataString.ToByteArray() });

			Assert.IsTrue(response);

			var message = broker.Request(new SingleRequest<KafkaTopic>() {Topic = KafkaTopic.FirstT, Offset = 4});

			Assert.IsNull(message);
		}

		[Test]
		public void RequestMessageWithValidParametars()
		{
			var broker = new Broker<KafkaTopic>();
			broker.AddTopic(KafkaTopic.FirstT);

			var testDataString = "TEST";
			var response = broker.Publish(new Message<KafkaTopic>()
				{ Topic = KafkaTopic.FirstT, Data = testDataString.ToByteArray() });

			Assert.IsTrue(response);

			var message = broker.Request(new SingleRequest<KafkaTopic>() {Topic = KafkaTopic.FirstT, Offset = 0});

			Assert.AreEqual(message.Data.ToObject<string>(),testDataString);
			Assert.AreEqual(message.Topic, KafkaTopic.FirstT);
		}

		[Test]
		public void RequestMessageWithNullParametar()
		{
			var broker = new Broker<KafkaTopic>();

			var response = broker.Request(null);

			Assert.IsNull(response);
		}


		[Test]
		public void RequestStream()
		{
			var broker = new Broker<KafkaTopic>();
			broker.AddTopic(KafkaTopic.FirstT);

			int count = 20;

			var stream = new List<Message<KafkaTopic>>(count);

			var testDataString = "TEST";

			for (int i = 0; i < count; i++)
			{
				stream.Add(new Message<KafkaTopic>()
					{ Topic = KafkaTopic.FirstT, Data = testDataString.ToByteArray() });
			}

			var response = broker.PublishStream(stream);

			Assert.IsTrue(response);

			var topicCount = broker.TopicCount(KafkaTopic.FirstT);

			Assert.AreEqual(count, topicCount);

			var retVal = broker.RequestStream(new StreamRequest<KafkaTopic>() {Topic = KafkaTopic.FirstT, Offset = 0, Count = 17});

			Assert.AreEqual(retVal.Count,17);
		}

		[Test]
		public void RequestStreamWithNullValues()
		{
			var broker = new Broker<KafkaTopic>();

			var retVal = broker.RequestStream(null);

			Assert.IsNull(retVal);
		}

		[Test]
		public void RequestStreamWithBadCount()
		{
			var broker = new Broker<KafkaTopic>();
			broker.AddTopic(KafkaTopic.FirstT);

			int count = 10;

			var stream = new List<Message<KafkaTopic>>(count);

			var testDataString = "TEST";

			for (int i = 0; i < count; i++)
			{
				stream.Add(new Message<KafkaTopic>()
					{ Topic = KafkaTopic.FirstT, Data = testDataString.ToByteArray() });
			}

			var response = broker.PublishStream(stream);

			Assert.IsTrue(response);

			var topicCount = broker.TopicCount(KafkaTopic.FirstT);

			Assert.AreEqual(count, topicCount);

			var retVal = broker.RequestStream(new StreamRequest<KafkaTopic>() { Topic = KafkaTopic.FirstT, Offset = 0, Count = 17 });

			Assert.AreEqual(retVal.Count, count);
		}

		#endregion

		#region Topic test

		[Test]
		public void TopicCountTest()
		{
			var broker = new Broker<KafkaTopic>();
			broker.AddTopic(KafkaTopic.FirstT);

			var testDataString = "TEST";
			var response = broker.Publish(new Message<KafkaTopic>()
				{ Topic = KafkaTopic.FirstT, Data = testDataString.ToByteArray() });

			Assert.IsTrue(response);
			Assert.AreEqual(1, broker.TopicCount(KafkaTopic.FirstT));
			Assert.AreEqual(-1, broker.TopicCount(KafkaTopic.SecondT));
		}

		[Test]
		public void AddTopic()
		{
			var broker = new Broker<KafkaTopic>();

			var result = broker.AddTopic(KafkaTopic.FirstT);

			Assert.IsTrue(result);
		}

		[Test]
		public void DeleteTopic()
		{
			var broker = new Broker<KafkaTopic>();

			var result = broker.AddTopic(KafkaTopic.FirstT);

			Assert.IsTrue(result);

			result = broker.DeleteTopic(KafkaTopic.FirstT);

			Assert.IsTrue(result);
		}

		[Test]
		public void AddTopicWhenTopicIsAlreadyAdded()
		{
			var broker = new Broker<KafkaTopic>();

			var result = broker.AddTopic(KafkaTopic.FirstT);

			Assert.IsTrue(result);

			result = broker.AddTopic(KafkaTopic.FirstT);

			Assert.IsTrue(result);
		}

		[Test]
		public void DeleteTopicWhichDontExist()
		{
			var broker = new Broker<KafkaTopic>();

			var result = broker.DeleteTopic(KafkaTopic.FirstT);

			Assert.IsTrue(result);
		}

		#endregion
	}
}
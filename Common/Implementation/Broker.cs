using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;
using System.Threading;
using Common.Converter;
using Common.Enums;
using Common.Interfaces;
using Common.Model;
using Common.Proxy;

namespace Common.Implementation
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class Broker<T> : IBroker<T>
	{
		private readonly IDictionary<T, List<Record<T>>> streamData;
		private readonly ReaderWriterLockSlim streamDataLocker;
		private ReplicationClientProxy<Message<T>> replicationClientProxy;
		private State state;

		public Broker(State state)
		{
			this.state = state;

			streamData = new Dictionary<T, List<Record<T>>>();
			streamDataLocker = new ReaderWriterLockSlim();
			InitializeReplicationClientProxy();
		}

		#region Contract implementation

		public bool Publish(Message<T> message)
		{
			try
			{
				WriteRecord(message);

				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error while publishing message: {e.Message}");
				return false;
			}
		}

		public bool PublishStream(List<Message<T>> messages)
		{
			try
			{
				foreach (var message in messages)
				{
					WriteRecord(message);
				}

				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error while publishing message: {e.Message}");
				return false;
			}
		}

		public Message<T> Request(SingleRequest<T> request)
		{
			try
			{
				var record = ReadRecord(request);

				if (record != null)
				{
					return new Message<T>() { Topic = record.Topic, Data = record.Data };
				}

				return null;
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error while processing request: {e.Message}");
				return null;
			}
		}

		public List<Message<T>> RequestStream(StreamRequest<T> request)
		{
			try
			{
				CheckRequest(request);

				var retVal = new List<Message<T>>(request.Count);

				for (int i = 0; i < request.Count; i++)
				{
					if (streamData[request.Topic].Count > request.Offset)
					{
						retVal.Add(ReadRecord(request));
						request.Offset++;
					}
					else
					{
						break;
					}
				}

				return retVal;
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error while processing stream request: {e.Message}");
				return null;
			}
		}

		#endregion

		#region Public methods

		public int TopicCount(T topic)
		{
			if (streamData.ContainsKey(topic))
			{
				return streamData[topic].Count;
			}

			return -1;
		}

		public bool AddTopic(T topic)
		{
			if (streamData.ContainsKey(topic)) return true;

			streamData.Add(topic, new List<Record<T>>());

			return true;
		}

		public bool DeleteTopic(T topic)
		{
			if (!streamData.ContainsKey(topic)) return true;

			return streamData.Remove(topic);
		}

		#endregion

		#region Private methods

		private void InitializeReplicationClientProxy()
		{
			var ipAddress = ConfigurationManager.AppSettings["replicationServiceIpAddress"];
			var port = ConfigurationManager.AppSettings["replicationServicePort"];
			var endpoint = ConfigurationManager.AppSettings["replicationServiceEndpoint"];

			replicationClientProxy = new ReplicationClientProxy<Message<T>>(ipAddress, port, endpoint);
			replicationClientProxy.DeliverReplicaEvent += WriteRecord;
			replicationClientProxy.RegisterToReplicationService();
		}

		private void CheckRequest(SingleRequest<T> request)
		{
			if (request == null) throw new ArgumentNullException("Request can not be null!");
		}

		private void CheckMessage(Message<T> message)
		{
			if (message == null) throw new ArgumentNullException("message can not be null!");
		}

		private void WriteRecord(Message<T> message)
		{
			CheckMessage(message);

			var record = new Record<T>()
			{ Topic = message.Topic, Data = message.Data, Offset = streamData.ContainsKey(message.Topic) ? streamData[message.Topic].Count : 0 };


			if (streamData.ContainsKey(record.Topic))
			{
				streamDataLocker.EnterWriteLock();

				streamData[message.Topic].Add(record);

				if (state == State.Hot) replicationClientProxy.SendReplica(message);

				streamDataLocker.ExitWriteLock();
			}
			else
			{
				streamDataLocker.ExitWriteLock();

				throw new KeyNotFoundException("Topic doesn't exist!");
			}

			Console.WriteLine($"Message is received on {message.Topic} topic with data: {message.Data.ToObject<string>()}");
		}

		private Record<T> ReadRecord(SingleRequest<T> request)
		{
			CheckRequest(request);

			Record<T> record = null;

			streamDataLocker.EnterReadLock();

			if (streamData.ContainsKey(request.Topic) && streamData[request.Topic].Count >= request.Offset)
			{
				record = streamData[request.Topic][request.Offset];
			}

			streamDataLocker.ExitReadLock();

			return record;
		}

		#endregion
	}
}
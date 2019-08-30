using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;
using System.Threading;
using Common.CallbackHandler;
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
		private IReplicationClientProxy<Message<T>> replicationClientProxy;
		private State state;
		private readonly ICallbackHandler<INotifyCallback> clientCallbackHandler;

		public Broker(State state)
		{
			this.state = state;

			streamData = new Dictionary<T, List<Record<T>>>();
			streamDataLocker = new ReaderWriterLockSlim();
			clientCallbackHandler = new CallbackHandler<INotifyCallback>();
			//InitializeReplicationClientProxy();
		}

		#region Test constructor

		public Broker(State state, ICallbackHandler<INotifyCallback> clientCallbackHandler)
		{
			this.state = state;
			this.clientCallbackHandler = clientCallbackHandler;

			streamData = new Dictionary<T, List<Record<T>>>();
			streamDataLocker = new ReaderWriterLockSlim();
		}

		#endregion

		#region Contract implementation

		public void PublishAsync(Message<T> message)
		{
			try
			{
				var status = WriteRecord(message);
				Console.WriteLine($"Message status:{status}  with data: {message.Data.ToObject<string>()}");
				clientCallbackHandler.GetCallback().Notify(status);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error while publishing message: {e.Message}");
				throw;
			}
		}

		public NotifyStatus PublishSync(Message<T> message)
		{
			try
			{
				return WriteRecord(message);
				
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error while publishing message: {e.Message}");
				throw;
			}
		}

		public Message<T> SingleRequest(SingleRequest<T> request)
		{
			try
			{
				var record = ReadRecord(request);

				return record != null ? new Message<T>() { Topic = record.Topic, Data = record.Data } : null;
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error while processing request: {e.Message}");
				throw;
			}
		}

		public List<Message<T>> MultipleRequest(MultipleRequest<T> request)
		{
			try
			{
				CheckRequest(request);

				var retVal = new List<Message<T>>(request.Count);

				for (var i = 0; i < request.Count; i++)
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

		public void InitializeReplicationClientProxy()
		{
			var ipAddress = ConfigurationManager.AppSettings["replicationServiceIpAddress"];
			var port = ConfigurationManager.AppSettings["replicationServicePort"];
			var endpoint = ConfigurationManager.AppSettings["replicationServiceEndpoint"];

			replicationClientProxy = new ReplicationClientProxy<Message<T>>();
			replicationClientProxy.DeliverReplicaEvent += ReplicaDelivered;
			replicationClientProxy.Initialize(ipAddress, port, endpoint);
			replicationClientProxy.RegisterToReplicationService();
		}

		private void ReplicaDelivered(object sender, ReplicationEventArgs<Message<T>> args)
		{
			WriteRecord(args.Replica);
		}

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

		private void CheckRequest(SingleRequest<T> request)
		{
			if (request == null) throw new ArgumentNullException("SingleRequest can not be null!");
		}

		private void CheckMessage(Message<T> message)
		{
			if (message == null) throw new ArgumentNullException("message can not be null!");
		}

		private NotifyStatus WriteRecord(Message<T> message)
		{
			CheckMessage(message);

			var status = NotifyStatus.Failed;

			var record = new Record<T>()
			{
				Topic = message.Topic, Data = message.Data,
				Offset = streamData.ContainsKey(message.Topic) ? streamData[message.Topic].Count : 0
			};


			if (streamData.ContainsKey(record.Topic))
			{
				streamDataLocker.EnterWriteLock();

				streamData[message.Topic].Add(record);

				if (state == State.Hot) replicationClientProxy?.SendReplica(message);

				status = NotifyStatus.Secceeded;

				streamDataLocker.ExitWriteLock();

				Console.WriteLine($"Message is received on {message.Topic} topic with data: {message.Data.ToObject<string>()}");
			}

			return status;
		}

		private Record<T> ReadRecord(SingleRequest<T> request)
		{
			CheckRequest(request);

			Record<T> record = null;

			streamDataLocker.EnterReadLock();

			if (streamData.ContainsKey(request.Topic) && streamData[request.Topic].Count > request.Offset)
			{
				record = streamData[request.Topic][request.Offset];
			}

			streamDataLocker.ExitReadLock();

			return record;
		}

		#endregion
	}
}
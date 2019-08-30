using System;
using System.Data.Odbc;
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
	public class ReplicationServiceTest
	{
		#region Moq

		private class ClientMoq<T> : IReplicationClient<T>, IReplicationClientCallback<T>
		{
			public bool DeliverReplica(T replication)
			{
				return replication != null;
			}

			public byte[] GetIntegrityUpdate()
			{
				var response = "Integrity update test";
				return response.ToByteArray();
			}

			public bool RegisterToReplicationService()
			{
				return true;
			}

			public byte[] RequestIntegrityUpdate()
			{
				var response = "Integrity update test";
				return response.ToByteArray();
			}

			public bool SendReplica(T replication)
			{
				return replication != null;
			}
		}

		private class PartnerMoq<T> : IReplicationService, IReplicationServiceCallback<T>
		{
			public byte[] ForwardIntegrityUpdate()
			{
				var response = "Integrity update test";
				return response.ToByteArray();
			}

			public bool ForwardReplica(T replication)
			{
				return replication != null;
			}

			public bool RegisterToPartner()
			{
				return true;
			}
		}

		private class ProxyMoq<T> : IReplicationServiceProxy<Message<T>>
		{
			public event EventHandler<ReplicationEventArgs<Message<T>>> ForwardReplicaEvent;

			public byte[] ForwardIntegrityUpdate()
			{
				var response = "Integrity update test";
				return response.ToByteArray();
			}

			public bool ForwardReplica(Message<T> replication)
			{
				return replication != null;
			}

			public void Initialize(string ipAddress, string port, string endpoint)
			{
				
			}

			public bool RegisterToPartner()
			{
				return true;
			}
		}
		#endregion

		private ReplicationService<Message<Topic>> replicationService;
		private ReplicationService<Message<Topic>> invalidReplicationService;
		private Message<Topic> testMessage;

		[SetUp]
		public void Initialize()
		{
			const string messageData = "Test data";
			testMessage = new Message<Topic>() {Topic = Topic.FirstT, Data = messageData.ToByteArray()};

			var clientMoq = new ClientMoq<Message<Topic>>();
			var clientCbHandler = Substitute.For<ICallbackHandler<IReplicationClientCallback<Message<Topic>>>>();
			clientCbHandler.GetCallback().Returns(clientMoq);

			var partnerMoq = new PartnerMoq<Message<Topic>>();
			var partnerCbHandler = Substitute.For<ICallbackHandler<IReplicationServiceCallback<Message<Topic>>>>();
			partnerCbHandler.GetCallback().Returns(partnerMoq);

			var proxyMoq = new ProxyMoq<Topic>();

			replicationService = new ReplicationService<Message<Topic>>(clientCbHandler, partnerCbHandler, proxyMoq);
			invalidReplicationService = new ReplicationService<Message<Topic>>(null, null, null);
		}

		[Test]
		public void ClientRegisterToReplicationService()
		{
			var response = replicationService.RegisterToReplicationService();

			Assert.IsTrue(response);
		}

		[Test]
		public void ClientRegisterToReplicationServiceWhenCallbackInNull()
		{
			Assert.Catch<Exception>(() => invalidReplicationService.RegisterToReplicationService());
		}

		[Test]
		public void ForwardIntegirtyUpdateRequestToClient()
		{
			var response = replicationService.RegisterToReplicationService();

			Assert.IsTrue(response);

			var integrityUpdateData = replicationService.ForwardIntegrityUpdate();

			Assert.AreEqual(integrityUpdateData.ToObject<string>(), "Integrity update test");
		}

		[Test]
		public void ForwardIntegirtyUpdateRequestToClientWhenCallbackIsNull()
		{
			Assert.Catch<Exception>(() => invalidReplicationService.ForwardIntegrityUpdate());
		}

		[Test]
		public void PartnerRegisterToReplicationService()
		{
			var response = replicationService.RegisterToPartner();

			Assert.IsTrue(response);
		}

		[Test]
		public void PartnerRegisterToReplicationServiceWhenCallbackIsNull()
		{
			Assert.Catch<Exception>(() => invalidReplicationService.RegisterToPartner());
		}

		[Test]
		public void ForwardReplicaToClient()
		{
			var response = replicationService.RegisterToReplicationService();

			Assert.IsTrue(response);

			response = replicationService.ForwardReplica(testMessage);

			Assert.IsTrue(response);
		}

		[Test]
		public void ForwardReplicaToClientWhenReplicaIsNull()
		{
			var response = replicationService.RegisterToReplicationService();

			Assert.IsTrue(response);

			response = replicationService.ForwardReplica(null);

			Assert.IsFalse(response);
		}

		[Test]
		public void ForwardReplicaToClientWhenCallbackIsNull()
		{
			Assert.Catch(() => replicationService.ForwardReplica(testMessage));
		}

		[Test]
		public void SendReplicaToPartner()
		{
			var response = replicationService.RegisterToPartner();

			Assert.IsTrue(response);

			response = replicationService.SendReplica(testMessage);

			Assert.IsTrue(response);
		}

		[Test]
		public void SendReplicaToPartnerWhenReplicaIsNull()
		{
			var response = replicationService.RegisterToPartner();

			Assert.IsTrue(response);

			response = replicationService.SendReplica(null);

			Assert.IsFalse(response);
		}

		[Test]
		public void SendReplicaToPartnerWhenCallbackIsNull()
		{
			Assert.IsFalse(replicationService.SendReplica(testMessage));
		}

		[Test]
		public void RequsetIntegrityUpdateFromPartner()
		{
			var data = replicationService.RequestIntegrityUpdate();

			Assert.AreEqual(data.ToObject<string>(), "Integrity update test");
		}

		[Test]
		public void RequsetIntegrityUpdateFromPartnerWhenProxyIsNull()
		{
			Assert.Catch<Exception>(() => invalidReplicationService.RequestIntegrityUpdate());
		}

		[Test]
		public void DelivarReplicaToclient()
		{
			var response = replicationService.RegisterToReplicationService();

			Assert.IsTrue(response);
			Assert.DoesNotThrow(() => replicationService.DeliverReplica(this, new ReplicationEventArgs<Message<Topic>>(testMessage)));
			
		}

		[Test]
		public void DelivarReplicaToclientWhenCallbackIsNull()
		{
			Assert.Catch<Exception>(() => replicationService.DeliverReplica(this, new ReplicationEventArgs<Message<Topic>>(testMessage)));
		}
	}
}
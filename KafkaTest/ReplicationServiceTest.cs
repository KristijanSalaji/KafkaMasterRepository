using System;
using Common.CallbackHandler;
using Common.Enums;
using Common.Implementation;
using Common.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace KafkaTest
{
	[TestFixture]
	public class ReplicationServiceTest
	{
		private ReplicationService<Topic> replicationService;

		[SetUp]
		public void Initialize()
		{
			var clientMoq = new ClientMoq<Topic>();
			var clientCbHandler = Substitute.For<ICallbackHandler<IReplicationClientCallback<Topic>>>();
			clientCbHandler.GetCallback().Returns(clientMoq);

			var partnerMoq = new PartnerMoq<Topic>();
			var partnerCbHandler = Substitute.For<ICallbackHandler<IReplicationServiceCallback<Topic>>>();
			partnerCbHandler.GetCallback().Returns(partnerMoq);

			replicationService = new ReplicationService<Topic>(clientCbHandler, partnerCbHandler);
		}

		[Test]
		public void ClientRegisterToReplicationService()
		{
			var response = replicationService.RegisterToReplicationService();

			Assert.IsTrue(response);
		}

		private class ClientMoq<T> : IReplicationClient<T>, IReplicationClientCallback<T>
		{
			public bool DeliverReplica(T replication)
			{
				return replication != null;
			}

			public byte[] GetIntegrityUpdate()
			{
				return new byte[10];
			}

			public bool RegisterToReplicationService()
			{
				return true;
			}

			public byte[] RequestIntegrityUpdate()
			{
				return new byte[10];
			}

			public bool SendReplica(T replication)
			{
				return replication != null;
			}
		}

		private class PartnerMoq<T> : IReplicationServiceCallback<T>
		{
			public bool ForwardReplica(T replication)
			{
				return replication != null;
			}
		}
	}
}
using System;
using System.ServiceModel;
using Common.CallbackHandler;
using Common.Enums;
using Common.Interfaces;
using Common.Proxy;
using System.Configuration;

namespace Common.Implementation
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class ReplicationService<R> : IReplicationService, IReplicationServiceCallback<R>, IReplicationClient<R>
	{
		//public State State { get; set; }

		private State state;

		private IReplicationClientCallback<R> clientCallback; //broker
		private IReplicationServiceCallback<R> partnerCallback;

		private readonly ICallbackHandler<IReplicationClientCallback<R>> clientCallbackHandler;
		private readonly ICallbackHandler<IReplicationServiceCallback<R>> serviceCallbackHandler;

		private readonly ReplicationServiceProxy<R> partnerServiceProxy;

		public ReplicationService(State state)
		{
			this.state = state;

			clientCallbackHandler = new CallbackHandler<IReplicationClientCallback<R>>();
			serviceCallbackHandler = new CallbackHandler<IReplicationServiceCallback<R>>();

			if (this.state == State.StandBy)
			{
				partnerServiceProxy = new ReplicationServiceProxy<R>(ConfigurationManager.AppSettings["partnerIpAddress"], ConfigurationManager.AppSettings["partnerPort"], "replication");
				partnerServiceProxy.RegisterToPartner();
				partnerServiceProxy.ForwardReplicaEvent += DeliverReplica;
			}
		}

		private void DeliverReplica(R replication)
		{
			if (clientCallback != null) clientCallback.DeliverReplica(replication);
		}

		#region IReplicationService implementation

		//izlozene metode prema standby rs

		public byte[] ForwardIntegrityUpdate()
		{
			return clientCallback.GetIntegrityUpdate();
		}

		public bool RegisterToPartner()
		{
			partnerCallback = serviceCallbackHandler.GetCallback();
			return true;
		}

		#endregion

		#region IReplicationServiceCallback Implementation

		//hot rs koristi ove metode da bi prosledio podatke standby rs

		public bool ForwardReplica(R replication)
		{
			return clientCallback.DeliverReplica(replication);
		}

		#endregion

		#region IReplicationClient implementation

		// izlozene metode prema klijentu (u nasem slucaju je to broker)

		// hot strana poziva
		public bool RegisterToReplicationService()
		{
			clientCallback = clientCallbackHandler.GetCallback();
			return true;
		}

		public bool SendReplica(R replication)
		{
			if (partnerCallback == null) return false;
			
			return partnerCallback.ForwardReplica(replication);
		}

		//standby strana poziva
		public byte[] RequestIntegrityUpdate()
		{
			return partnerServiceProxy.ForwardIntegrityUpdate();
		}

		#endregion

	}
}
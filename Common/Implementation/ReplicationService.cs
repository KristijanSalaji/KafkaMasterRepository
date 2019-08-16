using System;
using Common.CallbackHandler;
using Common.Interfaces;

namespace Common.Implementation
{
	public class ReplicationService<R> : IReplicationService, IReplicationServiceCallback<R>, IReplicationClient<R>
	{
		private IReplicationClientCallback<R> client; //broker
		private IReplicationServiceCallback<R> partner;

		private readonly ICallbackHandler<IReplicationClientCallback<R>> clientCallbackHandler;
		private readonly ICallbackHandler<IReplicationServiceCallback<R>> serviceCallbackHandler;

		public ReplicationService()
		{
			clientCallbackHandler = new CallbackHandler<IReplicationClientCallback<R>>();
			serviceCallbackHandler = new CallbackHandler<IReplicationServiceCallback<R>>();
		}

		#region IReplicationService implementation

		public byte[] ForwardIntegrityUpdate()
		{
			return client.GetIntegrityUpdate();
		}

		public bool RegisterToPartner()
		{
			partner = serviceCallbackHandler.GetCallback();
			return true;
		}

		#endregion

		#region IReplicationServiceCallback Implementation

		public bool ForwardReplica(R replication)
		{
			return partner.ForwardReplica(replication);
		}

		#endregion

		#region IReplicationClient implementation

		public bool RegisterToReplicationService()
		{
			client = clientCallbackHandler.GetCallback();
			return true;
		}

		public bool SendReplica(R replication)
		{
			return partner.ForwardReplica(replication);
		}

		public byte[] RequestIntegrityUpdate()
		{
			//proxy forward integrity update
			return new byte[1];
		}

		#endregion

	}
}
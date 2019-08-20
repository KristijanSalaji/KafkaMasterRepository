using System;
using Common.Interfaces;

namespace Common.Proxy
{
	public class ReplicationClientProxy<R> : IReplicationClient<R>, IReplicationClientCallback<R>
	{

		#region IReplicationClient

		public bool RegisterToReplicationService()
		{
			throw new NotImplementedException();
		}

		public byte[] RequestIntegrityUpdate()
		{
			throw new NotImplementedException();
		}

		public bool SendReplica(R replication)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IReplicationClientCallback

		public bool DeliverReplica(R replication)
		{
			throw new NotImplementedException();
		}

		public byte[] GetIntegrityUpdate()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
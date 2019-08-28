using System;
using System.Linq;
using System.ServiceModel;
using Common.Interfaces;

namespace Common.Proxy
{
	public class ReplicationServiceProxy<R> : IReplicationService, IReplicationServiceCallback<R>
	{
		private IReplicationService proxy;

		#region Forward replica event

		public delegate void ForwardReplicaDelegate(R replication);

		private event ForwardReplicaDelegate forwardReplicaEvent;

		public event ForwardReplicaDelegate ForwardReplicaEvent
		{
			add
			{
				if (forwardReplicaEvent == null || !forwardReplicaEvent.GetInvocationList().Contains(value))
				{
					forwardReplicaEvent += value;
				}
			}
			remove { forwardReplicaEvent -= value; }
		}

		#endregion

		public ReplicationServiceProxy()
		{
				
		}

		public void Initialize(string ipAddress, string port, string endpoint)
		{
			var factory = new DuplexChannelFactory<IReplicationService>(this,
				new NetTcpBinding() { OpenTimeout = TimeSpan.MaxValue },
				new EndpointAddress($"net.tcp://{ipAddress}:{port}/ReplicationService/{endpoint}"));

			proxy = factory.CreateChannel();
		}

		#region IReplicationService

		public byte[] ForwardIntegrityUpdate()
		{
			try
			{
				return proxy.ForwardIntegrityUpdate();
			}
			catch (Exception e)
			{
				Console.WriteLine($"Integrity update error: {e.Message}");
				throw;
			}
		}

		public bool RegisterToPartner()
		{
			try
			{
				return proxy.RegisterToPartner();
			}
			catch (Exception e)
			{
				Console.WriteLine($"Register error: {e.Message}");
				throw;
			}
		}

		#endregion

		#region IReplicationServiceCallback

		public bool ForwardReplica(R replication)
		{
			if (forwardReplicaEvent == null) return false;

			forwardReplicaEvent.Invoke(replication);
			return true;
		}

		#endregion
	}
}
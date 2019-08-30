using System;
using System.Linq;
using System.ServiceModel;
using Common.Interfaces;
using Common.Model;

namespace Common.Proxy
{
	public class ReplicationServiceProxy<R> : IReplicationServiceProxy<R>
	{
		private IReplicationService proxy;

		public event EventHandler<ReplicationEventArgs<R>> ForwardReplicaEvent;

		//#region Forward replica event

		//public delegate void ForwardReplicaDelegate(R replication);

		//private event ForwardReplicaDelegate forwardReplicaEvent;

		//public event ForwardReplicaDelegate ForwardReplicaEvent
		//{
		//	add
		//	{
		//		if (forwardReplicaEvent == null || !forwardReplicaEvent.GetInvocationList().Contains(value))
		//		{
		//			forwardReplicaEvent += value;
		//		}
		//	}
		//	remove { forwardReplicaEvent -= value; }
		//}

		//#endregion

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

		public virtual byte[] ForwardIntegrityUpdate()
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

		public virtual bool RegisterToPartner()
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

		public virtual bool ForwardReplica(R replication)
		{
			if (ForwardReplicaEvent == null) return false;

			ForwardReplicaEvent.Invoke(this, new ReplicationEventArgs<R>(replication));
			return true;
		}

		#endregion
	}
}
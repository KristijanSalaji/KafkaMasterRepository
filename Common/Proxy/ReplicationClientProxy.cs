using System;
using System.Linq;
using System.ServiceModel;
using Common.Interfaces;
using Common.Model;

namespace Common.Proxy
{
	public class ReplicationClientProxy<R> : IReplicationClientProxy<R>
	{

		private IReplicationClient<R> proxy;

		public event EventHandler<ReplicationEventArgs<R>> DeliverReplicaEvent;

		//#region Deliver replica event

		//public delegate void DeliverReplicaDelegate(R replication);

		//private event DeliverReplicaDelegate deliverReplicaEvent;

		//public event DeliverReplicaDelegate DeliverReplicaEvent
		//{
		//	add
		//	{
		//		if (deliverReplicaEvent == null || !deliverReplicaEvent.GetInvocationList().Contains(value))
		//		{
		//			deliverReplicaEvent += value;
		//		}
		//	}
		//	remove { deliverReplicaEvent -= value; }
		//}

		//#endregion

		public ReplicationClientProxy()
		{
				
		}

		public void Initialize(string ipAddress, string port, string endpoint)
		{
			var factory = new DuplexChannelFactory<IReplicationClient<R>>(this,
				new NetTcpBinding() { OpenTimeout = TimeSpan.MaxValue },
				new EndpointAddress($"net.tcp://{ipAddress}:{port}/ReplicationClient/{endpoint}"));

			proxy = factory.CreateChannel();
		}

		#region IReplicationClient

		public bool RegisterToReplicationService()
		{
			try
			{
				return proxy.RegisterToReplicationService();
			}
			catch (Exception e)
			{
				Console.WriteLine($"Register to replication service error: {e.Message}");
				throw;
			}
		}

		public byte[] RequestIntegrityUpdate()
		{
			try
			{
				return proxy.RequestIntegrityUpdate();
			}
			catch (Exception e)
			{
				Console.WriteLine($"SingleRequest integrity update error: {e.Message}");
				throw;
			}
		}

		public bool SendReplica(R replication)
		{
			try
			{
				return proxy.SendReplica(replication);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Send replica error: {e.Message}");
				throw;
			}
		}

		#endregion

		#region IReplicationClientCallback

		public bool DeliverReplica(R replication)
		{
			if (DeliverReplicaEvent == null) return false;

			DeliverReplicaEvent.Invoke(this, new ReplicationEventArgs<R>(replication));
			return true;
		}

		public byte[] GetIntegrityUpdate()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
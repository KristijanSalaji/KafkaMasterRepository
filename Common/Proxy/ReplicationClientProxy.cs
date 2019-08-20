using System;
using System.ServiceModel;
using Common.Interfaces;

namespace Common.Proxy
{
	public class ReplicationClientProxy<R> : IReplicationClient<R>, IReplicationClientCallback<R>
	{

		private IReplicationClient<R> proxy;

		public ReplicationClientProxy(string ipAddress, string port, string endpoint)
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
				Console.WriteLine($"Request integrity update error: {e.Message}");
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
			throw new NotImplementedException();
		}

		public byte[] GetIntegrityUpdate()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
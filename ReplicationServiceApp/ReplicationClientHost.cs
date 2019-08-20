using System;
using System.ServiceModel;
using Common.Interfaces;

namespace ReplicationServiceApp
{
	public class ReplicationClientHost<R>
	{
		private ServiceHost host;

		private string info;

		public ReplicationClientHost()
		{

		}

		public bool Initialize(string address, string port, string endpoint, IReplicationClient<R> replicationService)
		{
			try
			{
				host = new ServiceHost(replicationService, new Uri($"net.tcp://{address}:{port}"));
				host.AddServiceEndpoint(typeof(IReplicationClient<R>), new NetTcpBinding(), $"ReplicationClient/{endpoint}");
				info = $"net.tcp://{address}:{port}/ReplicationClient/{endpoint}";

				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error while initializing replication client host: {e.Message}");
				return false;
			}
		}

		public void Open()
		{
			if (host.State == CommunicationState.Opened) return;

			try
			{
				host.Open();
				Console.WriteLine($"Replication client host successfully opened with {info} endpoint...");
			}
			catch (CommunicationObjectFaultedException ex)
			{
				Console.WriteLine($"Error while opening replication client host: {ex.Message}");
			}
		}

		public void Close()
		{
			if (host.State != CommunicationState.Opened) return;

			try
			{
				host.Close();
				Console.WriteLine($"Replication client host successfully closed with {info} endpoint...");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error while closing replication client host: {ex.Message}");
			}
		}
	}
}
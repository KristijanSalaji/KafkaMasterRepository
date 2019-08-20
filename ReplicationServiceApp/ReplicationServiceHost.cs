using Common.Implementation;
using System;
using System.ServiceModel;
using Common.Enums;
using Common.Interfaces;
using Common.Model;

namespace ReplicationServiceApp
{
	public class ReplicationServiceHost
	{
		private ServiceHost host;

		private string info;

		public ReplicationServiceHost()
		{

		}

		public bool Initialize(string address, string port, string endpoint, IReplicationService replicationService)
		{
			try
			{
				host = new ServiceHost(replicationService, new Uri($"net.tcp://{address}:{port}"));
				host.AddServiceEndpoint(typeof(IReplicationService), new NetTcpBinding(), $"ReplicationService/{endpoint}");
				info = $"net.tcp://{address}:{port}/ReplicationService/{endpoint}";

				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error while initializing replication service host: {e.Message}");
				return false;
			}
		}

		public void Open()
		{
			if (host.State == CommunicationState.Opened) return;

			try
			{
				host.Open();
				Console.WriteLine($"Replication service host successfully opened with {info} endpoint...");
			}
			catch (CommunicationObjectFaultedException ex)
			{
				Console.WriteLine($"Error while oppening replication service host: {ex.Message}");
			}
		}

		public void Close()
		{
			if (host.State != CommunicationState.Opened) return;

			try
			{
				host.Close();
				Console.WriteLine($"Replication service host successfully closed with {info} endpoint...");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error while closing replication service host: {ex.Message}");
			}
		}
	}
}
using Common.Interfaces;
using System;
using System.ServiceModel;

namespace ManagerApp
{
	public class ManagerHost<T>
	{
		private ServiceHost host;

		private string info;

		public ManagerHost()
		{

		}

		public bool Initialize(string address, string port, string endpoint, IPublishManager<T> manager)
		{
			try
			{
				host = new ServiceHost(manager, new Uri($"net.pipe://{address}/Manager/{endpoint}"));
				host.AddServiceEndpoint(typeof(IPublishManager<T>), new NetNamedPipeBinding(), "");
				info = $"net.tcp://{address}/Manager/{endpoint}";

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
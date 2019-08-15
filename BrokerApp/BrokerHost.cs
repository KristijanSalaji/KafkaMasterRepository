using System;
using System.ServiceModel;
using Common.Interfaces;

namespace BrokerApp
{
	public class BrokerHost<T>
	{
		private ServiceHost host;

		private string info;

		public BrokerHost()
		{
			
		}

		public bool Initialize(string address, string port, string endpoint, Broker<T> broker)
		{
			try
			{
				host = new ServiceHost(broker, new Uri($"net.tcp://{address}:{port}"));
				host.AddServiceEndpoint(typeof(IBroker<T>), new NetTcpBinding(), $"Broker/{endpoint}");
				info = $"net.tcp://{address}:{port}/Broker/{endpoint}";

				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error while initializing broker host: {e.Message}");
				return false;
			}
		}

		public void Open()
		{
			if (host.State == CommunicationState.Opened) return;

			try
			{
				host.Open();
				Console.WriteLine($"Broker host successfully opened with {info} endpoint...");
			}
			catch (CommunicationObjectFaultedException ex)
			{
				Console.WriteLine($"Error while oppening broker host: {ex.Message}");
			}
		}

		public void Close()
		{
			if (host.State != CommunicationState.Opened) return;

			try
			{
				host.Close();
				Console.WriteLine($"Broker host successfully closed with {info} endpoint...");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error while closing broker host: {ex.Message}");
			}
		}
	}
}
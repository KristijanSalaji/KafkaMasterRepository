using System;
using System.Configuration;
using System.Dynamic;
using Common.Enums;

namespace BrokerApp
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Initialize broker host...");
			var brokerHost = InitializeHost();

			brokerHost.Open();

			Console.WriteLine("Press any key for exit...");
			Console.ReadLine();

			brokerHost.Close();
		}

		private static BrokerHost<KafkaTopic> InitializeHost()
		{
			var ipAddress = ConfigurationManager.AppSettings["ipAddress"];
			var port = ConfigurationManager.AppSettings["port"];
			var endpoint = ConfigurationManager.AppSettings["endpoint"];

			var brokerHost = new BrokerHost<KafkaTopic>();
			var broker = new Broker<KafkaTopic>();

			broker.AddTopic(KafkaTopic.FirstT);
			broker.AddTopic(KafkaTopic.SecondT);
			broker.AddTopic(KafkaTopic.ThirdT);

			brokerHost.Initialize(ipAddress, port, endpoint, broker);

			return brokerHost;
		}
	}
}

using System;
using System.Configuration;
using System.Dynamic;
using System.Threading;
using Common.Enums;
using Common.Implementation;

namespace BrokerApp
{
	class Program
	{
		static void Main(string[] args)
		{
			Thread.Sleep(2000);

			Console.WriteLine("Initialize broker host...");
			var brokerHost = InitializeHost();

			brokerHost.Open();

			Console.WriteLine("Press any key for exit...");
			Console.ReadLine();

			brokerHost.Close();
		}

		private static BrokerHost<Topic> InitializeHost()
		{
			var ipAddress = ConfigurationManager.AppSettings["ipAddress"];
			var port = ConfigurationManager.AppSettings["port"];
			var endpoint = ConfigurationManager.AppSettings["endpoint"];
			var state =(State)Enum.Parse( typeof(State),ConfigurationManager.AppSettings["state"]);

			var brokerHost = new BrokerHost<Topic>();
			var broker = new Broker<Topic>(state);

			broker.InitializeReplicationClientProxy();

			broker.AddTopic(Topic.FirstT);
			broker.AddTopic(Topic.SecondT);
			broker.AddTopic(Topic.ThirdT);

			if (state == State.StandBy) broker.RequestIntegiryUpdate();

			brokerHost.Initialize(ipAddress, port, endpoint, broker);

			return brokerHost;
		}
	}
}

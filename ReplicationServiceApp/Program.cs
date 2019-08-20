using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Enums;
using Common.Model;
using System.Configuration;
using Common.Implementation;

namespace ReplicationServiceApp
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Initialize replication service host...");
			var state = (State)Enum.Parse(typeof(State), ConfigurationManager.AppSettings["state"]);
			var replicationServiceHost = InitializeHost(state);

			replicationServiceHost.Open();

			Console.WriteLine("Press any key for exit...");
			Console.ReadLine();

			replicationServiceHost.Close();
		}

		private static ReplicationServiceHost InitializeHost(State state)
		{
			var ipAddress = ConfigurationManager.AppSettings["ipAddress"];
			var port = ConfigurationManager.AppSettings["port"];
			var endpoint = ConfigurationManager.AppSettings["endpoint"];

			var replicationServiceHost = new ReplicationServiceHost();
			var replicationService = new ReplicationService<Message<KafkaTopic>>(state);


			replicationServiceHost.Initialize(ipAddress, port, endpoint, replicationService);
			return replicationServiceHost;
		}
	}
}

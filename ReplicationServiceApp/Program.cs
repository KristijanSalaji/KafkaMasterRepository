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
			Console.WriteLine("Initialize hosts...");
			var state = (State)Enum.Parse(typeof(State), ConfigurationManager.AppSettings["state"]);
			ReplicationServiceHost replicationServiceHost;
			ReplicationClientHost<Message<KafkaTopic>> replicationClientHost;

			InitializeHosts(state,out replicationServiceHost,out replicationClientHost);

			replicationServiceHost.Open();
			replicationClientHost.Open();

			Console.WriteLine("Press any key for exit...");
			Console.ReadLine();

			replicationServiceHost.Close();
			replicationClientHost.Close();
		}

		private static void InitializeHosts(State state, out ReplicationServiceHost replicationServiceHost, out ReplicationClientHost<Message<KafkaTopic>> replicationClientHost) 
		{
			var ipAddress = ConfigurationManager.AppSettings["ipAddress"];
			var port = ConfigurationManager.AppSettings["port"];
			var endpoint = ConfigurationManager.AppSettings["endpoint"];

			var replicationService = new ReplicationService<Message<KafkaTopic>>(state);

			replicationServiceHost = new ReplicationServiceHost();
			replicationServiceHost.Initialize(ipAddress, port, endpoint, replicationService);

			replicationClientHost= new ReplicationClientHost<Message<KafkaTopic>>();
			replicationClientHost.Initialize(ipAddress, port, endpoint, replicationService);
		}
	}
}

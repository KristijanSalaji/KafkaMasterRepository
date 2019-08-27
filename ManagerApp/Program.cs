using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Enums;
using Common.Implementation;
using Common.Interfaces;
using Common.Model;
using System.Configuration;

namespace ManagerApp
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Initialize hosts...");
			//var state = (State)Enum.Parse(typeof(State), ConfigurationManager.AppSettings["state"]);
			ManagerHost<Topic> managerHost;

			InitializeHosts(out managerHost);

			managerHost.Open();
		

			Console.WriteLine("Press any key for exit...");
			Console.ReadLine();

			managerHost.Close();

		}

		private static void InitializeHosts(out ManagerHost<Topic> managerHost)
		{
			var ipAddress = ConfigurationManager.AppSettings["ipAddress"];
			var port = ConfigurationManager.AppSettings["port"];
			var endpoint = ConfigurationManager.AppSettings["endpoint"];

			var manager = new PublishManager<Topic>();

			managerHost = new ManagerHost<Topic>();
			managerHost.Initialize(ipAddress, port, endpoint, manager);

		}
	}
}

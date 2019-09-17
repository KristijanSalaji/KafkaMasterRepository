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
			ManagerHost<Topic> managerHost;

			var manager = InitializeHosts(out managerHost);

			managerHost.Open();
			
			Console.WriteLine("Press any key for exit...");
			Console.ReadLine();

			manager.SendData = false;
			managerHost.Close();
		}

		private static PublishManager<Topic> InitializeHosts(out ManagerHost<Topic> managerHost)
		{
			var ipAddress = ConfigurationManager.AppSettings["ipAddress"];
			var port = ConfigurationManager.AppSettings["port"];
			var endpoint = ConfigurationManager.AppSettings["endpoint"];

			var manager = new PublishManager<Topic>();
			manager.StartAsyncSendDataProcess();
			manager.CreateProxy();

			managerHost = new ManagerHost<Topic>();
			managerHost.Initialize(ipAddress, port, endpoint, manager);

			return manager;
		}
	}
}

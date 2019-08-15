﻿using Common.Converter;
using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Model;

namespace ConsumerApp
{
	class Program
	{
		private static int offset = 0;

		private static bool decision = true;

		static void Main(string[] args)
		{
			Console.WriteLine("Enter topic number (0,1,2): ");
			var topic = (KafkaTopic)Int32.Parse(Console.ReadLine());

			var t = new Thread(() => Work(topic));
			t.Start();

			Console.WriteLine("Press any key to finish... ");
			Console.ReadLine();

			decision = false;

			Console.WriteLine("Total count: {0}", offset);
			Console.ReadLine();
		}

		private static void Work(KafkaTopic topic)
		{
			var consumer = new Consumer<KafkaTopic>();

			while (decision)
			{
				var request = new SingleRequest<KafkaTopic>() {Topic = topic, Offset = offset};

				var message = consumer.Request(request);

				if (message != null)
				{
					Console.WriteLine($"Message data: {message.Data.ToObject<string>()}");
					offset++;
				}

				Thread.Sleep(80);
			}
		}
	}
}
﻿using Common.Converter;
using Common.Enums;
using Common.Model;
using System;
using System.Threading;
using Common.Implementation;

namespace ProducerApp
{
	class Program
	{
		private static int count = 0;
		private static bool decision = true;

		static void Main(string[] args)
		{
			Console.WriteLine("Enter topic number (0,1,2): ");
			var topic = (Topic) Int32.Parse(Console.ReadLine());

			Console.WriteLine("enter 0 for async or 1 for sync communication: ");
			if (Int32.Parse(Console.ReadLine()) == 0)
			{
				var t = new Thread(() => WorkAsync(topic));
				t.Start();
			}
			else
			{
				var t = new Thread(() => WorkSync(topic));
				t.Start();
			}

			Console.WriteLine("Press any key to finish... ");
			Console.ReadLine();

			decision = false;

			Console.WriteLine("Total count: {0}", count);
			Console.ReadLine();
		}

		private static void WorkAsync(Topic topic)
		{
			var producer = new Producer<Topic>();

			while (decision)
			{
				var dataString = "Test message " + count;
				producer.PublishAsync(new Message<Topic>() { Topic = topic, Data = dataString.ToByteArray() });

				count++;
			}
		}

		private static void WorkSync(Topic topic)
		{
			var producer = new Producer<Topic>();

			while (decision)
			{
				var dataString = "Test message " + count;
				var state = producer.PublishSync(new Message<Topic>() { Topic = topic, Data = dataString.ToByteArray() });

				count++;
			}
		}

		private static void SendOne(Topic topic)
		{
			var producer = new Producer<Topic>();

			var dataString = "Test message";
			producer.PublishAsync(new Message<Topic>() { Topic = topic, Data = dataString.ToByteArray() });

			producer.PublishSync(new Message<Topic>() {Topic = topic, Data = dataString.ToByteArray()});
		}
	}
}

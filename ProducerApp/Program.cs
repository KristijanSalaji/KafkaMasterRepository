using Common.Converter;
using Common.Enums;
using Common.Model;
using System;
using System.Threading;

namespace ProducerApp
{
	class Program
	{
		private static int count = 0;
		private static bool decision = true;

		static void Main(string[] args)
		{
			Console.WriteLine("Enter topic number (0,1,2): ");
			var topic = (KafkaTopic) Int32.Parse(Console.ReadLine());

			var t = new Thread(() => Work(topic));
			t.Start();

			Console.WriteLine("Press any key to finish... ");
			Console.ReadLine();

			decision = false;

			Console.WriteLine("Total count: {0}", count);
			Console.ReadLine();
		}

		private static void Work(KafkaTopic topic)
		{
			var producer = new Producer<KafkaTopic>();

			while (decision)
			{
				var dataString = "Test poruka " + count;
				var response = producer.Publish(new Message<KafkaTopic>() { Topic = topic, Data = dataString.ToByteArray() });

				if (response)
				{
					count++;
					Console.WriteLine("Message {0} successfully sent", count);
				}

				Thread.Sleep(50);
			}
		}
	}
}

using Common.Converter;
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

		private static Semaphore waitSemaphore;

		static void Main(string[] args)
		{
			waitSemaphore = new Semaphore(0,1);

			Console.WriteLine("Enter topic number (0,1,2): ");
			var topic = (Topic) Int32.Parse(Console.ReadLine());

			var t = new Thread(() => Work(topic));
			t.Start();

			//waitSemaphore.WaitOne();
			Console.WriteLine("Press any key to finish... ");
			Console.ReadLine();

			decision = false;

			Console.WriteLine("Total count: {0}", count);
			Console.ReadLine();
		}

		private static void Work(Topic topic)
		{
			var producer = new Producer<Topic>();

			while (decision)
			{
				var dataString = "Test message " + count;
				producer.PublishAsync(new Message<Topic>() { Topic = topic, Data = dataString.ToByteArray() });

				count++;
				Console.WriteLine("Message {0} successfully sent", count);


				Thread.Sleep(10);
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

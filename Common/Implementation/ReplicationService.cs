using System;
using System.ServiceModel;
using Common.CallbackHandler;
using Common.Enums;
using Common.Interfaces;
using Common.Proxy;
using System.Configuration;

namespace Common.Implementation
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class ReplicationService<R> : IReplicationService, IReplicationServiceCallback<R>, IReplicationClient<R>
	{
		//public State State { get; set; }

		private State state;

		private IReplicationClientCallback<R> clientCallback; //broker
		private IReplicationServiceCallback<R> partnerCallback;

		private readonly ICallbackHandler<IReplicationClientCallback<R>> clientCallbackHandler;
		private readonly ICallbackHandler<IReplicationServiceCallback<R>> serviceCallbackHandler;

		private readonly ReplicationServiceProxy<R> partnerServiceProxy;

		public ReplicationService(State state)
		{
			this.state = state;

			clientCallbackHandler = new CallbackHandler<IReplicationClientCallback<R>>();
			serviceCallbackHandler = new CallbackHandler<IReplicationServiceCallback<R>>();

			if (this.state == State.StandBy)
			{
				var partnerIpAddress = ConfigurationManager.AppSettings["partnerIpAddress"];
				var partnerPort = ConfigurationManager.AppSettings["partnerPort"];
				var partnerEndpoint = ConfigurationManager.AppSettings["endpoint"];

				partnerServiceProxy = new ReplicationServiceProxy<R>();
				partnerServiceProxy.ForwardReplicaEvent += DeliverReplica;
				partnerServiceProxy.Initialize(partnerIpAddress, partnerPort, partnerEndpoint);
				partnerServiceProxy.RegisterToPartner();
			}
		}

		private void DeliverReplica(R replication)
		{
			try
			{
				clientCallback?.DeliverReplica(replication);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Deliver replica to client error: {e.Message}");
				throw;
			}
		}

		#region IReplicationService implementation

		//izlozene metode prema standby rs

		public byte[] ForwardIntegrityUpdate()
		{
			try
			{
				return clientCallback?.GetIntegrityUpdate();
			}
			catch (Exception e)
			{
				Console.WriteLine($"Forward integrity update error: {e.Message}");
				throw;
			}
		}

		public bool RegisterToPartner()
		{
			try
			{
				partnerCallback = serviceCallbackHandler.GetCallback();
				Console.WriteLine("Partner is successfully registered!");
				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine($"Register to partner error: {e.Message}");
				throw;
			}
		}

		#endregion

		#region IReplicationServiceCallback Implementation

		//hot rs koristi ove metode da bi prosledio podatke standby rs

		public bool ForwardReplica(R replication)
		{
			if (clientCallback == null) return false;

			try
			{
				return clientCallback.DeliverReplica(replication);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Forward replica error: {e.Message}");
				throw;
			}
		}

		#endregion

		#region IReplicationClient implementation

		// izlozene metode prema klijentu (u nasem slucaju je to broker)

		// hot strana poziva
		public bool RegisterToReplicationService()
		{
			try
			{
				clientCallback = clientCallbackHandler.GetCallback();
				Console.WriteLine("Broker is successfully registered!");
				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine($"Register to replication service error {e.Message}");
				throw;
			}
		}

		public bool SendReplica(R replication)
		{
			if (partnerCallback == null) return false;

			try
			{
				return partnerCallback.ForwardReplica(replication);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error while sending replica: {e.Message}");
				throw;
			}
		}

		//standby strana poziva
		public byte[] RequestIntegrityUpdate()
		{
			try
			{
				return partnerServiceProxy?.ForwardIntegrityUpdate();
			}
			catch (Exception e)
			{
				Console.WriteLine($"Request integrity update error: {e.Message}");
				throw;
			}
		}

		#endregion

	}
}
using System;
using System.ServiceModel;
using Common.CallbackHandler;
using Common.Enums;
using Common.Interfaces;
using Common.Proxy;
using System.Configuration;
using Common.Model;

namespace Common.Implementation
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class ReplicationService<R> : IReplicationService, IReplicationServiceCallback<R>, IReplicationClient<R>
	{
		private State state;

		private IReplicationClientCallback<R> clientCallback; //broker
		private IReplicationServiceCallback<R> partnerCallback;

		private readonly ICallbackHandler<IReplicationClientCallback<R>> clientCallbackHandler;
		private readonly ICallbackHandler<IReplicationServiceCallback<R>> serviceCallbackHandler;

		private readonly IReplicationServiceProxy<R> partnerServiceProxy;

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

		#region Test constructor

		public ReplicationService(ICallbackHandler<IReplicationClientCallback<R>> clientCallbackHandler, ICallbackHandler<IReplicationServiceCallback<R>> serviceCallbackHandler, IReplicationServiceProxy<R> proxy)
		{
			this.clientCallbackHandler = clientCallbackHandler;
			this.serviceCallbackHandler = serviceCallbackHandler;
			this.partnerServiceProxy = proxy;
			state = State.Hot;
		}

		#endregion

		#region Forward replica event method

		public void DeliverReplica(object sender,ReplicationEventArgs<R> args)
		{
			try
			{
				clientCallback.DeliverReplica(args.Replica);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Deliver replica to client error: {e.Message}");
				throw;
			}
		}

		#endregion

		#region IReplicationService implementation

		//izlozene metode prema standby rs

		public byte[] ForwardIntegrityUpdate()
		{
			try
			{
				return clientCallback.GetIntegrityUpdate();
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
			if (replication == null) return false;

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
			if (replication == null) return false;

			try
			{
				return partnerCallback.ForwardReplica(replication);
			}
			catch
			{
				return false;
			}
		}

		//standby strana poziva
		public byte[] RequestIntegrityUpdate()
		{
			try
			{
				return partnerServiceProxy.ForwardIntegrityUpdate();
			}
			catch (Exception e)
			{
				Console.WriteLine($"SingleRequest integrity update error: {e.Message}");
				throw;
			}
		}

		#endregion

	}
}
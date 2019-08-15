﻿using System.Collections.Generic;
using Common.Model;
using System.ServiceModel;

namespace Common.Interfaces
{
	[ServiceContract]
	public interface IProducer<T>
	{
		[OperationContract]
		bool Publish(Message<T> message);

		[OperationContract]
		bool PublishStream(List<Message<T>> messages);
	}
}
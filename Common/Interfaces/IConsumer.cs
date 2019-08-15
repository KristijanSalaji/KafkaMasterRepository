﻿using Common.Enums;
using Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
	[ServiceContract]
	public interface IConsumer<T>
	{
		[OperationContract]
		Message<T> Request(SingleRequest<T> request);

		[OperationContract]
		List<Message<T>> RequestStream(StreamRequest<T> request);
	}
}

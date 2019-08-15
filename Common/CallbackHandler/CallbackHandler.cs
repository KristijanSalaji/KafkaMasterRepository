using System;
using System.ServiceModel;

namespace Common.CallbackHandler
{
	public class CallbackHandler<T> : ICallbackHandler<T>
	{
		public T GetCallback()
		{
			if (OperationContext.Current == null)
			{
				throw new ArgumentNullException("Operation context is null!");
			}

			return OperationContext.Current.GetCallbackChannel<T>();
		}
	}
}
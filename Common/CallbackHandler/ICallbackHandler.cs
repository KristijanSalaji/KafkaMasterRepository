namespace Common.CallbackHandler
{
	public interface ICallbackHandler<T>
	{
		T GetCallback();
	}
}
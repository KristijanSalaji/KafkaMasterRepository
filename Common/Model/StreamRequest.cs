namespace Common.Model
{
	public class StreamRequest<T> : SingleRequest<T>
	{
		public int Count { get; set; }
	}
}
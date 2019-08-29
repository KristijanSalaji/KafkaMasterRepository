namespace Common.Model
{
	public class MultipleRequest<T> : SingleRequest<T>
	{
		public int Count { get; set; }
	}
}
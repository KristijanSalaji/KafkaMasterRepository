namespace Common.Model
{
	public class SingleRequest<T>
	{
		public T Topic { get; set; }

		public int Offset { get; set; }
	}
}
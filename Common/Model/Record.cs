namespace Common.Model
{
	public class Record<T> : Message<T>
	{
		public int Offset { get; set; }
	}
}
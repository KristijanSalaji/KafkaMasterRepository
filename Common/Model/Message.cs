using Common.Enums;

namespace Common.Model
{
	public class Message<T>
	{
		public T Topic { get; set; }

		public byte[] Data { get; set; }
	}
}
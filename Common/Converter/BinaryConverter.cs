using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Common.Converter
{
	public static class BinaryConverter
	{
		public static byte[] ToByteArray<T>(this T obj)
		{
			var bf = new BinaryFormatter();
			using (var ms = new MemoryStream())
			{
				bf.Serialize(ms, obj);
				return ms.ToArray();
			}
		}

		public static T ToObject<T>(this byte[] arrBytes)
		{
			using (var memStream = new MemoryStream())
			{
				var binForm = new BinaryFormatter();
				memStream.Write(arrBytes, 0, arrBytes.Length);
				memStream.Seek(0, SeekOrigin.Begin);
				var obj = binForm.Deserialize(memStream);
				return (T)obj;
			}
		}

		public static T DeepCopy<T>(this T obj)
		{
			var array = obj.ToByteArray();
			return array.ToObject<T>();
		}
	}
}

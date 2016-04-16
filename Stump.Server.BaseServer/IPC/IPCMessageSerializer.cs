using ProtoBuf.Meta;
using Stump.Core.Reflection;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
namespace Stump.Server.BaseServer.IPC
{
	public class IPCMessageSerializer : Singleton<IPCMessageSerializer>
	{
		private int m_idCounter = 50;
		public RuntimeTypeModel Model
		{
			get;
			private set;
		}
		public IPCMessageSerializer()
		{
			this.Model = TypeModel.Create();
			this.Model.AutoAddMissingTypes = true;
			this.Model.AutoAddProtoContractTypesOnly = true;
			this.RegisterMessages(Assembly.GetExecutingAssembly());
		}
		public void RegisterMessages(Assembly assembly)
		{
			foreach (Type current in 
				from x in assembly.GetTypes()
				where x.IsSubclassOf(typeof(IPCMessage))
				select x)
			{
				if (!(current == typeof(IPCMessage)))
				{
					this.Model[typeof(IPCMessage)].AddSubType(this.m_idCounter++, current);
				}
			}
		}
		public void RegisterMessage(Type type)
		{
			this.Model[typeof(IPCMessage)].AddSubType(this.m_idCounter++, type);
		}
		public void RegisterMessage(Type type, int id)
		{
			this.Model[typeof(IPCMessage)].AddSubType(id, type);
		}
		public IPCMessage Deserialize(byte[] buffer)
		{
			return this.Deserialize(buffer, 0, buffer.Length);
		}
		public IPCMessage Deserialize(byte[] buffer, int offset, int count)
		{
			return (IPCMessage)this.Model.Deserialize(new MemoryStream(buffer, offset, count), null, typeof(IPCMessage));
		}
		public T Deserialize<T>(byte[] buffer, int offset, int count)
		{
			return (T)((object)this.Model.Deserialize(new MemoryStream(buffer, offset, count), null, typeof(T)));
		}
		public T Deserialize<T>(byte[] buffer)
		{
			return this.Deserialize<T>(buffer, 0, buffer.Length);
		}
		public byte[] Serialize(object obj)
		{
			MemoryStream memoryStream = new MemoryStream();
			this.Model.Serialize(memoryStream, obj);
			return memoryStream.ToArray();
		}
		public byte[] SerializeWithLength(object obj)
		{
			MemoryStream memoryStream = new MemoryStream();
			this.Model.Serialize(memoryStream, obj);
			long length = memoryStream.Length;
			byte b = IPCMessageSerializer.ComputeTypeLen(length);
			MemoryStream memoryStream2 = new MemoryStream();
			memoryStream2.WriteByte(b);
			for (int i = (int)(b - 1); i >= 0; i--)
			{
				memoryStream2.WriteByte((byte)(length >> 8 * i & 255L));
			}
			memoryStream2.Write(memoryStream.ToArray(), 0, (int)memoryStream.Length);
			return memoryStream2.ToArray();
		}
		private static byte ComputeTypeLen(long len)
		{
			byte result;
			if (len < 256L)
			{
				result = 1;
			}
			else
			{
				if (len < 65536L)
				{
					result = 2;
				}
				else
				{
					if (len < 16777216L)
					{
						result = 3;
					}
					else
					{
						result = (byte)Math.Floor(Math.Log((double)len, 256.0) + 1.0);
					}
				}
			}
			return result;
		}
	}
}

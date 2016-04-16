using Stump.Core.IO;
using Stump.Core.Pool.New;
using Stump.DofusProtocol.Messages;
using Stump.Server.BaseServer.Network;
using System.Linq;

namespace Stump.Server.WorldServer.Core.Network
{
	public class WorldClientCollection : System.Collections.Generic.IEnumerable<WorldClient>, IPacketReceiver, System.Collections.IEnumerable
	{
		private WorldClient m_singleClient;
		private System.Collections.Generic.List<WorldClient> m_underlyingList = new System.Collections.Generic.List<WorldClient>();
		public int Count
		{
			get
			{
				return (this.m_singleClient != null) ? 1 : this.m_underlyingList.Count;
			}
		}
		public WorldClientCollection()
		{
		}
		public WorldClientCollection(System.Collections.Generic.IEnumerable<WorldClient> clients)
		{
			this.m_underlyingList = clients.ToList<WorldClient>();
		}
		public WorldClientCollection(WorldClient client)
		{
			this.m_singleClient = client;
		}
		public void Send(Message message)
		{
			if (this.m_singleClient != null)
			{
				this.m_singleClient.Send(message);
			}
			else
			{
				bool flag = false;
				try
				{
					System.Threading.Monitor.Enter(this, ref flag);
					SegmentStream stream = BufferManager.Default.CheckOutStream();
                    CustomDataWriter writer = new CustomDataWriter(stream);
					message.Pack(writer);
					System.Collections.Generic.List<WorldClient> list = new System.Collections.Generic.List<WorldClient>();
					foreach (WorldClient current in this.m_underlyingList)
					{
						if (current != null)
						{
							current.Send(stream);
							current.OnMessageSent(message);
						}
						if (current == null || !current.Connected)
						{
							list.Add(current);
						}
					}
					foreach (WorldClient current2 in list)
					{
						this.Remove(current2);
					}
				}
				finally
				{
					if (flag)
					{
						System.Threading.Monitor.Exit(this);
					}
				}
			}
		}
		public void Add(WorldClient client)
		{
			bool flag = false;
			try
			{
				System.Threading.Monitor.Enter(this, ref flag);
				if (this.m_singleClient != null)
				{
					this.m_underlyingList.Add(this.m_singleClient);
					this.m_underlyingList.Add(client);
					this.m_singleClient = null;
				}
				else
				{
					this.m_underlyingList.Add(client);
				}
			}
			finally
			{
				if (flag)
				{
					System.Threading.Monitor.Exit(this);
				}
			}
		}
		public void Remove(WorldClient client)
		{
			bool flag = false;
			try
			{
				System.Threading.Monitor.Enter(this, ref flag);
				if (this.m_singleClient == client)
				{
					this.m_singleClient = null;
				}
				else
				{
					this.m_underlyingList.Remove(client);
				}
			}
			finally
			{
				if (flag)
				{
					System.Threading.Monitor.Exit(this);
				}
			}
		}
		public System.Collections.Generic.IEnumerator<WorldClient> GetEnumerator()
		{
			System.Collections.Generic.IEnumerator<WorldClient> result;
			if (this.m_singleClient != null)
			{
				result = new WorldClient[]
				{
					this.m_singleClient
				}.AsEnumerable<WorldClient>().GetEnumerator();
			}
			else
			{
				result = this.m_underlyingList.GetEnumerator();
			}
			return result;
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
		public static implicit operator WorldClientCollection(WorldClient client)
		{
			return new WorldClientCollection(client);
		}
	}
}

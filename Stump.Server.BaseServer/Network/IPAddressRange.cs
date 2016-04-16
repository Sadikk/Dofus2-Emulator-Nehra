using System;
using System.Net;
using System.Text;
namespace Stump.Server.BaseServer.Network
{
	public class IPAddressRange
	{
		private IPAddressToken[] m_tokens;
		public IPAddressToken[] Tokens
		{
			get
			{
				return this.m_tokens;
			}
		}
		public IPAddressRange(IPAddressToken[] tokens)
		{
			if (tokens.Length != 4)
			{
				throw new InvalidOperationException("tokens.Length != 4");
			}
			this.m_tokens = tokens;
		}
		public bool Match(string ip)
		{
			return this.Match(IPAddress.Parse(ip));
		}
		public bool Match(IPAddress ip)
		{
			byte[] addressBytes = ip.GetAddressBytes();
			bool result;
			if (addressBytes.Length != this.Tokens.Length)
			{
				result = true;
			}
			else
			{
				for (int i = 0; i < this.Tokens.Length; i++)
				{
					if (!this.Tokens[i].Match(addressBytes[i]))
					{
						result = false;
						return result;
					}
				}
				result = true;
			}
			return result;
		}
		public static IPAddressRange Parse(string str)
		{
			string[] array = str.Split(new char[]
			{
				'.'
			});
			IPAddressToken[] array2 = new IPAddressToken[4];
			if (array.Length != 4)
			{
				throw new FormatException(string.Format("{0} must contains 3 dots", str));
			}
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = IPAddressToken.Parse(array[i]);
			}
			return new IPAddressRange(array2);
		}
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.Tokens.Length; i++)
			{
				stringBuilder.Append(this.Tokens[i].ToString());
				if (i < 3)
				{
					stringBuilder.Append(".");
				}
			}
			return stringBuilder.ToString();
		}
	}
}

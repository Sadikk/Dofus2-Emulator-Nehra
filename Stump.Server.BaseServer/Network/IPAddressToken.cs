using System;
namespace Stump.Server.BaseServer.Network
{
	public class IPAddressToken
	{
		public byte Number;
		public bool Star;
		public Tuple<byte, byte> Range;
		public IPAddressToken(bool star)
		{
			this.Star = star;
		}
		public IPAddressToken(byte low, byte high)
		{
			this.Range = Tuple.Create<byte, byte>(low, high);
		}
		public IPAddressToken(byte number)
		{
			this.Number = number;
		}
		public bool Match(byte x)
		{
			bool result;
			if (this.Star)
			{
				result = true;
			}
			else
			{
				if (this.Range != null)
				{
					result = (this.Range.Item1 <= x && x <= this.Range.Item2);
				}
				else
				{
					result = (x == this.Number);
				}
			}
			return result;
		}
		public static IPAddressToken Parse(string str)
		{
			str = str.Trim();
			IPAddressToken result;
			if (str == "*")
			{
				result = new IPAddressToken(true);
			}
			else
			{
				if (str.Contains("-"))
				{
					byte low = byte.Parse(str.Substring(0, str.IndexOf("-")).Trim());
					byte high = byte.Parse(str.Remove(0, str.IndexOf("-") + 1).Trim());
					result = new IPAddressToken(low, high);
				}
				else
				{
					result = new IPAddressToken(byte.Parse(str));
				}
			}
			return result;
		}
		public override string ToString()
		{
			string result;
			if (this.Star)
			{
				result = "*";
			}
			else
			{
				if (this.Range != null)
				{
					result = this.Range.Item1 + "-" + this.Range.Item2;
				}
				else
				{
					result = this.Number.ToString();
				}
			}
			return result;
		}
	}
}

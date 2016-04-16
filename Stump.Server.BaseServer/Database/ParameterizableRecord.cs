using Stump.Core.Extensions;
using Stump.ORM;
using Stump.ORM.SubSonic.SQLGeneration.Schema;
using System;
using System.Text;
namespace Stump.Server.BaseServer.Database
{
	public abstract class ParameterizableRecord
	{
		[Ignore]
		public char ParameterSeparatorChar
		{
			get;
			set;
		}
		[NullString]
		public string Parameter0
		{
			get;
			set;
		}
		[NullString]
		public string Parameter1
		{
			get;
			set;
		}
		[NullString]
		public string Parameter2
		{
			get;
			set;
		}
		[NullString]
		public string Parameter3
		{
			get;
			set;
		}
		[NullString]
		public string Parameter4
		{
			get;
			set;
		}
		[NullString]
		public string AdditionalParameters
		{
			get;
			set;
		}
		public ParameterizableRecord()
		{
			this.ParameterSeparatorChar = '|';
		}
		public T GetParameter<T>(uint parameter, bool defaultIfEmpty = false) where T : IConvertible
		{
			T result;
			if (parameter <= 4u)
			{
				switch (parameter)
				{
				case 0u:
					result = this.GetParameterInteral<T>(parameter, this.Parameter0, defaultIfEmpty);
					return result;
				case 1u:
					result = this.GetParameterInteral<T>(parameter, this.Parameter1, defaultIfEmpty);
					return result;
				case 2u:
					result = this.GetParameterInteral<T>(parameter, this.Parameter2, defaultIfEmpty);
					return result;
				case 3u:
					result = this.GetParameterInteral<T>(parameter, this.Parameter3, defaultIfEmpty);
					return result;
				case 4u:
					result = this.GetParameterInteral<T>(parameter, this.Parameter4, defaultIfEmpty);
					return result;
				}
			}
			if (string.IsNullOrEmpty(this.AdditionalParameters))
			{
				if (!defaultIfEmpty)
				{
					throw new Exception(string.Format("Parameter {0} is empty, cannot be converted to {1}", parameter, typeof(T)));
				}
				result = default(T);
			}
			else
			{
				string[] array = this.AdditionalParameters.Split(new char[]
				{
					this.ParameterSeparatorChar
				});
				if ((long)array.Length <= (long)((ulong)(parameter - 5u)))
				{
					if (!defaultIfEmpty)
					{
						throw new Exception(string.Format("Parameter {0} is empty, cannot be converted to {1}", parameter, typeof(T)));
					}
					result = default(T);
				}
				else
				{
					result = (T)((object)Convert.ChangeType(array[(int)((UIntPtr)(parameter - 5u))], typeof(T)));
				}
			}
			return result;
		}
		private T GetParameterInteral<T>(uint parameterNum, string parameterStr, bool defaultIfEmpty)
		{
			T result;
			if (string.IsNullOrEmpty(parameterStr))
			{
				if (!defaultIfEmpty)
				{
					throw new Exception(string.Format("Parameter {0} is empty, cannot be converted to {1}", parameterNum, typeof(T)));
				}
				result = default(T);
			}
			else
			{
				result = (T)((object)Convert.ChangeType(parameterStr, typeof(T)));
			}
			return result;
		}
		public void SetParameter<T>(uint parameter, T value) where T : IConvertible
		{
			string text = (string)Convert.ChangeType(value, typeof(string));
			if (parameter <= 4u)
			{
				switch (parameter)
				{
				case 0u:
					this.Parameter0 = text;
					break;
				case 1u:
					this.Parameter1 = text;
					break;
				case 2u:
					this.Parameter2 = text;
					break;
				case 3u:
					this.Parameter3 = text;
					break;
				case 4u:
					this.Parameter4 = text;
					break;
				}
			}
			else
			{
				int num = this.AdditionalParameters.CountOccurences(this.ParameterSeparatorChar);
				if ((ulong)(parameter - 5u) > (ulong)((long)num))
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(this.AdditionalParameters);
					int num2 = 0;
					while ((long)num2 < (long)((ulong)(parameter - 5u) - (ulong)((long)num)))
					{
						stringBuilder.Append(this.ParameterSeparatorChar);
						num2++;
					}
					stringBuilder.Append(text);
					this.AdditionalParameters = stringBuilder.ToString();
				}
				else
				{
					int num3 = 0;
					int num4 = 5;
					while ((ulong)parameter != (ulong)((long)num4))
					{
						if (num3 == -1)
						{
							throw new Exception(string.Format("Cannot find parameter {0} in '{1}', something went wrong", parameter, this.AdditionalParameters));
						}
						num3 = this.AdditionalParameters.IndexOf(this.ParameterSeparatorChar, num3);
						num4++;
					}
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(this.AdditionalParameters);
					int num5 = this.AdditionalParameters.IndexOf(this.ParameterSeparatorChar, num3);
					stringBuilder.Remove(num3 + 1, (num5 == -1) ? (this.AdditionalParameters.Length - (num3 + 1)) : (num5 - (num3 + 1)));
					stringBuilder.Insert(num3 + 1, text);
					this.AdditionalParameters = stringBuilder.ToString();
				}
			}
		}
	}
}

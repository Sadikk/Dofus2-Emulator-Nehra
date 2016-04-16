using System;
using System.Collections;
using System.Reflection;
using System.Text;

namespace Stump.Core.Reflection
{
    public class ObjectDumper
    {
        private const int DepthDisplayLimit = 5;
        private int m_level;
        private readonly int m_indentSize;
        private readonly StringBuilder m_stringBuilder;
        public Func<MemberInfo, bool> MemberPredicate
        {
            get;
            set;
        }

        public ObjectDumper(int indentSize)
        {
            this.m_indentSize = indentSize;
            this.m_stringBuilder = new StringBuilder();
        }

        public static string Dump(object element)
        {
            return ObjectDumper.Dump(element, 2);
        }

        public static string Dump(object element, int indentSize)
        {
            ObjectDumper objectDumper = new ObjectDumper(indentSize);
            return objectDumper.DumpElement(element);
        }

        public string DumpElement(object element)
        {
            string result;
            if (this.m_level > 5)
            {
                result = "... (limit reached)";
            }
            else
            {
                if (element == null || element is ValueType || element is string)
                {
                    this.Write(this.FormatValue(element), new object[0]);
                }
                else
                {
                    Type type = element.GetType();
                    if (!typeof(IEnumerable).IsAssignableFrom(type))
                    {
                        this.Write("{{{0}}}", new object[]
						{
							type.FullName
						});
                        this.m_level++;
                    }
                    IEnumerable enumerable = element as IEnumerable;
                    if (enumerable != null)
                    {
                        int num = 0;
                        foreach (object current in enumerable)
                        {
                            num++;
                            if (current is IEnumerable && !(current is string))
                            {
                                this.m_level++;
                                this.DumpElement(current);
                                this.m_level--;
                            }
                            else
                            {
                                this.DumpElement(current);
                            }
                        }
                        if (num == 0)
                        {
                            this.Write("-Empty-", new object[0]);
                        }
                    }
                    else
                    {
                        MemberInfo[] members = element.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        MemberInfo[] array = members;
                        for (int i = 0; i < array.Length; i++)
                        {
                            MemberInfo memberInfo = array[i];
                            if (!(memberInfo is EventInfo))
                            {
                                FieldInfo fieldInfo = memberInfo as FieldInfo;
                                PropertyInfo propertyInfo = memberInfo as PropertyInfo;
                                if ((!(fieldInfo == null) || !(propertyInfo == null)) && (this.MemberPredicate == null || this.MemberPredicate(memberInfo)))
                                {
                                    Type type2 = (fieldInfo != null) ? fieldInfo.FieldType : propertyInfo.PropertyType;
                                    if (!(propertyInfo != null) || propertyInfo.GetIndexParameters().Length <= 0)
                                    {
                                        object obj = (fieldInfo != null) ? fieldInfo.GetValue(element) : propertyInfo.GetValue(element, null);
                                        if (!(obj is MulticastDelegate))
                                        {
                                            if (type2.IsValueType || type2 == typeof(string))
                                            {
                                                this.Write("{0}: {1}", new object[]
												{
													memberInfo.Name,
													this.FormatValue(obj)
												});
                                            }
                                            else
                                            {
                                                this.Write("{0}: {1}", new object[]
												{
													memberInfo.Name,
													typeof(IEnumerable).IsAssignableFrom(type2) ? "..." : "{ }"
												});
                                                this.m_level++;
                                                this.DumpElement(obj);
                                                this.m_level--;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (!typeof(IEnumerable).IsAssignableFrom(type))
                    {
                        this.m_level--;
                    }
                }
                result = this.m_stringBuilder.ToString();
            }
            return result;
        }

        private void Write(string value, params object[] args)
        {
            string str = new string(' ', this.m_level * this.m_indentSize);
            if (args != null)
            {
                value = string.Format(value, args);
            }
            this.m_stringBuilder.AppendLine(str + value);
        }

        private string FormatValue(object o)
        {
            string result;
            if (o == null)
            {
                result = "null";
            }
            else
            {
                if (o is DateTime)
                {
                    result = ((DateTime)o).ToShortDateString();
                }
                else
                {
                    if (o is string)
                    {
                        result = string.Format("\"{0}\"", o);
                    }
                    else
                    {
                        if (o is ValueType)
                        {
                            result = o.ToString();
                        }
                        else
                        {
                            if (o is IEnumerable)
                            {
                                result = "...";
                            }
                            else
                            {
                                result = "{ }";
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}
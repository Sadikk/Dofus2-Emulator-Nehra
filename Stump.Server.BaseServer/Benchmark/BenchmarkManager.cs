using Stump.Core.Attributes;
using Stump.Core.Reflection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
namespace Stump.Server.BaseServer.Benchmark
{
	public class BenchmarkManager : Singleton<BenchmarkManager>
	{
		private readonly List<BenchmarkEntry> m_entries = new List<BenchmarkEntry>();
		[Variable(true)]
		public static bool Enable = true;
		[Variable(true)]
		public static BenchmarkingType BenchmarkingType = BenchmarkingType.Complete;
		[Variable(true)]
		public static int EntriesLimit = 1000;
		public ReadOnlyCollection<BenchmarkEntry> Entries
		{
			get
			{
				return this.m_entries.AsReadOnly();
			}
		}
		public void RegisterEntry(BenchmarkEntry entry)
		{
			lock (this.m_entries)
			{
				this.m_entries.Add(entry);
			}
			if (this.m_entries.Count >= BenchmarkManager.EntriesLimit)
			{
				lock (this.m_entries)
				{
					this.m_entries.RemoveRange(0, BenchmarkManager.EntriesLimit / 4);
				}
			}
		}
		public void ClearResults()
		{
			this.m_entries.Clear();
		}
		public BenchmarkEntry[] GetEntries(Type message)
		{
			return (
				from entry in this.m_entries
				where entry.MessageType == message
				select entry).ToArray<BenchmarkEntry>();
		}
		public BenchmarkEntry[] SortEntries()
		{
			return (
				from entry in this.m_entries
				orderby entry.Timestamp descending
				select entry).ToArray<BenchmarkEntry>();
		}
		public BenchmarkEntry[] SortEntries(int limit)
		{
			return (
				from entry in this.m_entries
				orderby entry.Timestamp descending
				select entry).Take(limit).ToArray<BenchmarkEntry>();
		}
		public Dictionary<Type, Tuple<TimeSpan, int>> GetEntriesSummary()
		{
			BenchmarkEntry[] source = this.SortEntries();
			Dictionary<Type, Tuple<TimeSpan, int>> dictionary = new Dictionary<Type, Tuple<TimeSpan, int>>();
			foreach (IGrouping<Type, BenchmarkEntry> current in 
				from entry in source
				group entry by entry.MessageType)
			{
				long ticks = (long)current.Average((BenchmarkEntry entry) => entry.Timestamp.Ticks);
				dictionary.Add(current.Key, Tuple.Create<TimeSpan, int>(new TimeSpan(ticks), current.Count<BenchmarkEntry>()));
			}
			return dictionary;
		}
		public BenchmarkEntry GetHighestEntry(Type message)
		{
			return (
				from entry in this.m_entries
				where entry.MessageType == message
				orderby entry.Timestamp descending
				select entry).First<BenchmarkEntry>();
		}
		public string GenerateReport()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Benchmarking report - {0} entries\n", this.m_entries.Count);
			Dictionary<Type, Tuple<TimeSpan, int>> entriesSummary = this.GetEntriesSummary();
			foreach (KeyValuePair<Type, Tuple<TimeSpan, int>> current in entriesSummary)
			{
				stringBuilder.AppendFormat("{0} {1}ms ({2} entries)\n", current.Key.Name, current.Value.Item1.TotalMilliseconds, current.Value.Item2);
			}
			return stringBuilder.ToString();
		}
	}
}

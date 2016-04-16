using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
namespace Stump.Server.BaseServer.Database.Patchs
{
	public class PatchFile
	{
		public string Path
		{
			get;
			private set;
		}
		public string FileName
		{
			get;
			private set;
		}
		public uint ForRevision
		{
			get;
			private set;
		}
		public uint ToRevision
		{
			get;
			private set;
		}
		public PatchFile(string path)
		{
			this.Path = path;
			this.FileName = System.IO.Path.GetFileNameWithoutExtension(this.Path);
			this.Parse();
		}
		private void Parse()
		{
			Match match;
			if (!(match = Regex.Match(this.FileName, "([0-9]+)_to_([0-9]+)")).Success)
			{
				throw new Exception(string.Format("Cannot parse file {0}, right syntax is : A_to_B.sql", this.FileName));
			}
			this.ForRevision = uint.Parse(match.Groups[1].Value);
			this.ToRevision = uint.Parse(match.Groups[2].Value);
		}
		public static IEnumerable<PatchFile> GeneratePatchSequenceExecution(IEnumerable<PatchFile> files, uint forRevision, uint toRevision)
		{
			IEnumerable<PatchFile> result;
			if (files.Count<PatchFile>() <= 0)
			{
				result = Enumerable.Empty<PatchFile>();
			}
			else
			{
				List<IEnumerable<PatchFile>> list = new List<IEnumerable<PatchFile>>();
				using (IEnumerator<PatchFile> enumerator = (
					from entry in files
					where entry.ForRevision == forRevision
					select entry).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PatchFile file = enumerator.Current;
						if (file.ForRevision == forRevision && file.ToRevision == toRevision)
						{
							result = new PatchFile[]
							{
								file
							};
							return result;
						}
						List<PatchFile> list2 = new List<PatchFile>
						{
							file
						};
						list2.AddRange(PatchFile.GeneratePatchSequenceExecution(
							from entry in files
							where entry.ForRevision >= file.ToRevision
							select entry, file.ToRevision, toRevision));
						if (list2.Count > 1)
						{
							list.Add(list2);
						}
					}
				}
				if (list.Count <= 0)
				{
					result = Enumerable.Empty<PatchFile>();
				}
				else
				{
					result = (
						from entry in list
						orderby entry.Count<PatchFile>()
						select entry).First<IEnumerable<PatchFile>>();
				}
			}
			return result;
		}
	}
}

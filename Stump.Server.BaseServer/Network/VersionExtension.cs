using Stump.Core.Attributes;
using Stump.DofusProtocol.Types;
using System;
namespace Stump.Server.BaseServer.Network
{
	public static class VersionExtension
	{
		[Variable(true)]
		public static VersionCheckingSeverity Severity = VersionCheckingSeverity.Light;
		[Variable(true)]
		public static Stump.DofusProtocol.Types.Version ExpectedVersion = new Stump.DofusProtocol.Types.Version(2, 10, 0, 70907, 1, 0);
		[Variable(true)]
		public static int ActualProtocol = 1428;
		[Variable(true)]
		public static int ProtocolRequired = 1428;
		public static bool IsUpToDate(this Stump.DofusProtocol.Types.Version versionToCompare)
		{
			bool result;
			switch (VersionExtension.Severity)
			{
			case VersionCheckingSeverity.None:
				result = true;
				break;
			case VersionCheckingSeverity.Light:
				result = (VersionExtension.ExpectedVersion.major == versionToCompare.major && VersionExtension.ExpectedVersion.minor == versionToCompare.minor && VersionExtension.ExpectedVersion.release == versionToCompare.release);
				break;
			case VersionCheckingSeverity.Medium:
				result = (VersionExtension.ExpectedVersion.major == versionToCompare.major && VersionExtension.ExpectedVersion.minor == versionToCompare.minor && VersionExtension.ExpectedVersion.release == versionToCompare.release && VersionExtension.ExpectedVersion.revision == versionToCompare.revision);
				break;
			case VersionCheckingSeverity.Heavy:
				result = (VersionExtension.ExpectedVersion.major == versionToCompare.major && VersionExtension.ExpectedVersion.minor == versionToCompare.minor && VersionExtension.ExpectedVersion.release == versionToCompare.release && VersionExtension.ExpectedVersion.revision == versionToCompare.revision && VersionExtension.ExpectedVersion.patch == versionToCompare.patch);
				break;
			default:
				result = false;
				break;
			}
			return result;
		}
	}
}

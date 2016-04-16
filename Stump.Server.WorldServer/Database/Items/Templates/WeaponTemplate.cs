using Stump.DofusProtocol.Classes;
using Stump.DofusProtocol.Tools.D2o;
using Stump.ORM.SubSonic.SQLGeneration.Schema;

namespace Stump.Server.WorldServer.Database.Items.Templates
{
	[D2OClass("Weapon", "com.ankamagames.dofus.datacenter.items", true), TableName("items_templates_weapons")]
	public class WeaponTemplate : ItemTemplate
	{
		public int ApCost
		{
			get;
			set;
		}
		public int MinRange
		{
			get;
			set;
		}
		public int WeaponRange
		{
			get;
			set;
		}
		public bool CastInLine
		{
			get;
			set;
		}
		public bool CastInDiagonal
		{
			get;
			set;
		}
		public bool CastTestLos
		{
			get;
			set;
		}
		public int CriticalHitProbability
		{
			get;
			set;
		}
		public int CriticalHitBonus
		{
			get;
			set;
		}
		public int CriticalFailureProbability
		{
			get;
			set;
		}
		public override void AssignFields(object d2oObject)
		{
			base.AssignFields(d2oObject);
			Weapon weapon = (Weapon)d2oObject;
			this.ApCost = weapon.apCost;
			this.MinRange = weapon.minRange;
			this.WeaponRange = weapon.range;
			this.CastInLine = weapon.castInLine;
			this.CastInDiagonal = weapon.castInDiagonal;
			this.CastTestLos = weapon.castTestLos;
			this.CriticalHitProbability = weapon.criticalHitProbability;
			this.CriticalHitBonus = weapon.criticalHitBonus;
			this.CriticalFailureProbability = weapon.criticalFailureProbability;
		}
	}
}

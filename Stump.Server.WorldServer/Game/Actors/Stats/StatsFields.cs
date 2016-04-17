using Stump.Core.Attributes;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Database.Characters;
using Stump.Server.WorldServer.Database.Monsters;
using Stump.Server.WorldServer.Game.Actors.Interfaces;
using Stump.Server.WorldServer.Game.Actors.RolePlay.TaxCollectors;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Actors.Stats
{
	public class StatsFields
	{
		[Variable]
		public static int MPLimit = 6;
		[Variable]
		public static int APLimit = 12;
		[Variable]
		public static int RangeLimit = 6;
		private static readonly StatsFormulasHandler FormulasChanceDependant = (IStatsOwner owner) => (short)(owner.Stats[PlayerFields.Chance] / 10.0);
		private static readonly StatsFormulasHandler FormulasWisdomDependant = (IStatsOwner owner) => (short)(owner.Stats[PlayerFields.Wisdom] / 10.0);
		private static readonly StatsFormulasHandler FormulasAgilityDependant = (IStatsOwner owner) => (short)(owner.Stats[PlayerFields.Agility] / 10.0);
		public System.Collections.Generic.Dictionary<PlayerFields, StatsData> Fields
		{
			get;
			private set;
		}
		public IStatsOwner Owner
		{
			get;
			private set;
		}
		public StatsHealth Health
		{
			get
			{
				return this[PlayerFields.Health] as StatsHealth;
			}
		}
		public StatsAP AP
		{
			get
			{
				return this[PlayerFields.AP] as StatsAP;
			}
		}
		public StatsMP MP
		{
			get
			{
				return this[PlayerFields.MP] as StatsMP;
			}
		}
		public StatsData Vitality
		{
			get
			{
				return this[PlayerFields.Vitality];
			}
		}
		public StatsData Strength
		{
			get
			{
				return this[PlayerFields.Strength];
			}
		}
		public StatsData Wisdom
		{
			get
			{
				return this[PlayerFields.Wisdom];
			}
		}
		public StatsData Chance
		{
			get
			{
				return this[PlayerFields.Chance];
			}
		}
		public StatsData Agility
		{
			get
			{
				return this[PlayerFields.Agility];
			}
		}
		public StatsData Intelligence
		{
			get
			{
				return this[PlayerFields.Intelligence];
			}
		}
		public StatsData this[PlayerFields name]
		{
			get
			{
				StatsData statsData;
				return this.Fields.TryGetValue(name, out statsData) ? statsData : null;
			}
		}
		public StatsFields(IStatsOwner owner)
		{
			this.Owner = owner;
		}
		public int GetTotal(PlayerFields name)
		{
			StatsData statsData = this[name];
			return (statsData == null) ? 0 : statsData.Total;
		}
		public void Initialize(CharacterRecord record)
		{
			this.Fields = new System.Collections.Generic.Dictionary<PlayerFields, StatsData>();
			this.Fields.Add(PlayerFields.Initiative, new StatsInitiative(this.Owner, 0));
			this.Fields.Add(PlayerFields.Prospecting, new StatsData(this.Owner, PlayerFields.Prospecting, (int)((short)record.Prospection), StatsFields.FormulasChanceDependant));
			this.Fields.Add(PlayerFields.AP, new StatsAP(this.Owner, (int)((short)record.AP), StatsFields.APLimit));
			this.Fields.Add(PlayerFields.MP, new StatsMP(this.Owner, (int)((short)record.MP), StatsFields.MPLimit));
			this.Fields.Add(PlayerFields.Strength, new StatsData(this.Owner, PlayerFields.Strength, record.Strength, null));
			this.Fields.Add(PlayerFields.Vitality, new StatsData(this.Owner, PlayerFields.Vitality, record.Vitality, null));
			this.Fields.Add(PlayerFields.Health, new StatsHealth(this.Owner, (int)((short)record.BaseHealth), (int)((short)record.DamageTaken)));
			this.Fields.Add(PlayerFields.Wisdom, new StatsData(this.Owner, PlayerFields.Wisdom, record.Wisdom, null));
			this.Fields.Add(PlayerFields.Chance, new StatsData(this.Owner, PlayerFields.Chance, record.Chance, null));
			this.Fields.Add(PlayerFields.Agility, new StatsData(this.Owner, PlayerFields.Agility, record.Agility, null));
			this.Fields.Add(PlayerFields.Intelligence, new StatsData(this.Owner, PlayerFields.Intelligence, record.Intelligence, null));
			this.Fields.Add(PlayerFields.Range, new StatsData(this.Owner, PlayerFields.Range, 0, StatsFields.RangeLimit));
			this.Fields.Add(PlayerFields.SummonLimit, new StatsData(this.Owner, PlayerFields.SummonLimit, 1, null));
			this.Fields.Add(PlayerFields.DamageReflection, new StatsData(this.Owner, PlayerFields.DamageReflection, 0, null));
			this.Fields.Add(PlayerFields.CriticalHit, new StatsData(this.Owner, PlayerFields.CriticalHit, 0, null));
			this.Fields.Add(PlayerFields.CriticalMiss, new StatsData(this.Owner, PlayerFields.CriticalMiss, 0, null));
			this.Fields.Add(PlayerFields.HealBonus, new StatsData(this.Owner, PlayerFields.HealBonus, 0, null));
			this.Fields.Add(PlayerFields.DamageBonus, new StatsData(this.Owner, PlayerFields.DamageBonus, 0, null));
			this.Fields.Add(PlayerFields.WeaponDamageBonus, new StatsData(this.Owner, PlayerFields.WeaponDamageBonus, 0, null));
			this.Fields.Add(PlayerFields.DamageBonusPercent, new StatsData(this.Owner, PlayerFields.DamageBonusPercent, 0, null));
			this.Fields.Add(PlayerFields.TrapBonus, new StatsData(this.Owner, PlayerFields.TrapBonus, 0, null));
			this.Fields.Add(PlayerFields.TrapBonusPercent, new StatsData(this.Owner, PlayerFields.TrapBonusPercent, 0, null));
			this.Fields.Add(PlayerFields.PermanentDamagePercent, new StatsData(this.Owner, PlayerFields.PermanentDamagePercent, 0, null));
			this.Fields.Add(PlayerFields.TackleBlock, new StatsData(this.Owner, PlayerFields.TackleBlock, 0, StatsFields.FormulasAgilityDependant));
			this.Fields.Add(PlayerFields.TackleEvade, new StatsData(this.Owner, PlayerFields.TackleEvade, 0, StatsFields.FormulasAgilityDependant));
			this.Fields.Add(PlayerFields.APAttack, new StatsData(this.Owner, PlayerFields.APAttack, 0, StatsFields.FormulasWisdomDependant));
			this.Fields.Add(PlayerFields.MPAttack, new StatsData(this.Owner, PlayerFields.MPAttack, 0, StatsFields.FormulasWisdomDependant));
			this.Fields.Add(PlayerFields.PushDamageBonus, new StatsData(this.Owner, PlayerFields.PushDamageBonus, 0, null));
			this.Fields.Add(PlayerFields.CriticalDamageBonus, new StatsData(this.Owner, PlayerFields.CriticalDamageBonus, 0, null));
			this.Fields.Add(PlayerFields.NeutralDamageBonus, new StatsData(this.Owner, PlayerFields.NeutralDamageBonus, 0, null));
			this.Fields.Add(PlayerFields.EarthDamageBonus, new StatsData(this.Owner, PlayerFields.EarthDamageBonus, 0, null));
			this.Fields.Add(PlayerFields.WaterDamageBonus, new StatsData(this.Owner, PlayerFields.WaterDamageBonus, 0, null));
			this.Fields.Add(PlayerFields.AirDamageBonus, new StatsData(this.Owner, PlayerFields.AirDamageBonus, 0, null));
			this.Fields.Add(PlayerFields.FireDamageBonus, new StatsData(this.Owner, PlayerFields.FireDamageBonus, 0, null));
            this.Fields.Add(PlayerFields.GlyphBonusPercent, new StatsData(Owner, PlayerFields.GlyphBonusPercent, 0, null)); //Maybe something wrong
			this.Fields.Add(PlayerFields.DodgeAPProbability, new StatsData(this.Owner, PlayerFields.DodgeAPProbability, 0, StatsFields.FormulasWisdomDependant));
			this.Fields.Add(PlayerFields.DodgeMPProbability, new StatsData(this.Owner, PlayerFields.DodgeMPProbability, 0, StatsFields.FormulasWisdomDependant));
			this.Fields.Add(PlayerFields.NeutralResistPercent, new StatsData(this.Owner, PlayerFields.NeutralResistPercent, 0, null));
			this.Fields.Add(PlayerFields.EarthResistPercent, new StatsData(this.Owner, PlayerFields.EarthResistPercent, 0, null));
			this.Fields.Add(PlayerFields.WaterResistPercent, new StatsData(this.Owner, PlayerFields.WaterResistPercent, 0, null));
			this.Fields.Add(PlayerFields.AirResistPercent, new StatsData(this.Owner, PlayerFields.AirResistPercent, 0, null));
			this.Fields.Add(PlayerFields.FireResistPercent, new StatsData(this.Owner, PlayerFields.FireResistPercent, 0, null));
			this.Fields.Add(PlayerFields.NeutralElementReduction, new StatsData(this.Owner, PlayerFields.NeutralElementReduction, 0, null));
			this.Fields.Add(PlayerFields.EarthElementReduction, new StatsData(this.Owner, PlayerFields.EarthElementReduction, 0, null));
			this.Fields.Add(PlayerFields.WaterElementReduction, new StatsData(this.Owner, PlayerFields.WaterElementReduction, 0, null));
			this.Fields.Add(PlayerFields.AirElementReduction, new StatsData(this.Owner, PlayerFields.AirElementReduction, 0, null));
			this.Fields.Add(PlayerFields.FireElementReduction, new StatsData(this.Owner, PlayerFields.FireElementReduction, 0, null));
			this.Fields.Add(PlayerFields.PushDamageReduction, new StatsData(this.Owner, PlayerFields.PushDamageReduction, 0, null));
			this.Fields.Add(PlayerFields.CriticalDamageReduction, new StatsData(this.Owner, PlayerFields.CriticalDamageReduction, 0, null));
			this.Fields.Add(PlayerFields.PvpNeutralResistPercent, new StatsData(this.Owner, PlayerFields.PvpNeutralResistPercent, 0, null));
			this.Fields.Add(PlayerFields.PvpEarthResistPercent, new StatsData(this.Owner, PlayerFields.PvpEarthResistPercent, 0, null));
			this.Fields.Add(PlayerFields.PvpWaterResistPercent, new StatsData(this.Owner, PlayerFields.PvpWaterResistPercent, 0, null));
			this.Fields.Add(PlayerFields.PvpAirResistPercent, new StatsData(this.Owner, PlayerFields.PvpAirResistPercent, 0, null));
			this.Fields.Add(PlayerFields.PvpFireResistPercent, new StatsData(this.Owner, PlayerFields.PvpFireResistPercent, 0, null));
			this.Fields.Add(PlayerFields.PvpNeutralElementReduction, new StatsData(this.Owner, PlayerFields.PvpNeutralElementReduction, 0, null));
			this.Fields.Add(PlayerFields.PvpEarthElementReduction, new StatsData(this.Owner, PlayerFields.PvpEarthElementReduction, 0, null));
			this.Fields.Add(PlayerFields.PvpWaterElementReduction, new StatsData(this.Owner, PlayerFields.PvpWaterElementReduction, 0, null));
			this.Fields.Add(PlayerFields.PvpAirElementReduction, new StatsData(this.Owner, PlayerFields.PvpAirElementReduction, 0, null));
			this.Fields.Add(PlayerFields.PvpFireElementReduction, new StatsData(this.Owner, PlayerFields.PvpFireElementReduction, 0, null));
			this.Fields.Add(PlayerFields.GlobalDamageReduction, new StatsData(this.Owner, PlayerFields.GlobalDamageReduction, 0, null));
			this.Fields.Add(PlayerFields.DamageMultiplicator, new StatsData(this.Owner, PlayerFields.DamageMultiplicator, 0, null));
			this.Fields.Add(PlayerFields.PhysicalDamage, new StatsData(this.Owner, PlayerFields.PhysicalDamage, 0, null));
			this.Fields.Add(PlayerFields.MagicDamage, new StatsData(this.Owner, PlayerFields.MagicDamage, 0, null));
			this.Fields.Add(PlayerFields.PhysicalDamageReduction, new StatsData(this.Owner, PlayerFields.PhysicalDamageReduction, 0, null));
			this.Fields.Add(PlayerFields.MagicDamageReduction, new StatsData(this.Owner, PlayerFields.MagicDamageReduction, 0, null));
			this.Fields.Add(PlayerFields.WaterDamageArmor, new StatsData(this.Owner, PlayerFields.WaterDamageArmor, 0, null));
			this.Fields.Add(PlayerFields.EarthDamageArmor, new StatsData(this.Owner, PlayerFields.EarthDamageArmor, 0, null));
			this.Fields.Add(PlayerFields.NeutralDamageArmor, new StatsData(this.Owner, PlayerFields.NeutralDamageArmor, 0, null));
			this.Fields.Add(PlayerFields.AirDamageArmor, new StatsData(this.Owner, PlayerFields.AirDamageArmor, 0, null));
			this.Fields.Add(PlayerFields.FireDamageArmor, new StatsData(this.Owner, PlayerFields.FireDamageArmor, 0, null));
			this.Fields.Add(PlayerFields.Erosion, new StatsData(this.Owner, PlayerFields.Erosion, 10, null));
            this.Fields.Add(PlayerFields.Shield, new StatsData(this.Owner, PlayerFields.Shield, 0, null));
		}
		public void Initialize(MonsterGrade record)
		{
			this.Fields = new System.Collections.Generic.Dictionary<PlayerFields, StatsData>();
			this.Fields.Add(PlayerFields.Initiative, new StatsInitiative(this.Owner, 0));
			this.Fields.Add(PlayerFields.Prospecting, new StatsData(this.Owner, PlayerFields.Prospecting, 100, StatsFields.FormulasChanceDependant));
			this.Fields.Add(PlayerFields.AP, new StatsAP(this.Owner, (int)((short)record.ActionPoints)));
			this.Fields.Add(PlayerFields.MP, new StatsMP(this.Owner, (int)((short)record.MovementPoints)));
			this.Fields.Add(PlayerFields.Strength, new StatsData(this.Owner, PlayerFields.Strength, (int)record.Strength, null));
			this.Fields.Add(PlayerFields.Vitality, new StatsData(this.Owner, PlayerFields.Vitality, (int)record.Vitality, null));
			this.Fields.Add(PlayerFields.Health, new StatsHealth(this.Owner, (int)((short)record.LifePoints), 0));
			this.Fields.Add(PlayerFields.Wisdom, new StatsData(this.Owner, PlayerFields.Wisdom, (int)record.Wisdom, null));
			this.Fields.Add(PlayerFields.Chance, new StatsData(this.Owner, PlayerFields.Chance, (int)record.Chance, null));
			this.Fields.Add(PlayerFields.Agility, new StatsData(this.Owner, PlayerFields.Agility, (int)record.Agility, null));
			this.Fields.Add(PlayerFields.Intelligence, new StatsData(this.Owner, PlayerFields.Intelligence, (int)record.Intelligence, null));
			this.Fields.Add(PlayerFields.Range, new StatsData(this.Owner, PlayerFields.Range, 0, null));
			this.Fields.Add(PlayerFields.SummonLimit, new StatsData(this.Owner, PlayerFields.SummonLimit, 1, null));
			this.Fields.Add(PlayerFields.DamageReflection, new StatsData(this.Owner, PlayerFields.DamageReflection, 0, null));
			this.Fields.Add(PlayerFields.CriticalHit, new StatsData(this.Owner, PlayerFields.CriticalHit, 0, null));
			this.Fields.Add(PlayerFields.CriticalMiss, new StatsData(this.Owner, PlayerFields.CriticalMiss, 0, null));
			this.Fields.Add(PlayerFields.HealBonus, new StatsData(this.Owner, PlayerFields.HealBonus, 0, null));
			this.Fields.Add(PlayerFields.DamageBonus, new StatsData(this.Owner, PlayerFields.DamageBonus, 0, null));
			this.Fields.Add(PlayerFields.WeaponDamageBonus, new StatsData(this.Owner, PlayerFields.WeaponDamageBonus, 0, null));
			this.Fields.Add(PlayerFields.DamageBonusPercent, new StatsData(this.Owner, PlayerFields.DamageBonusPercent, 0, null));
			this.Fields.Add(PlayerFields.TrapBonus, new StatsData(this.Owner, PlayerFields.TrapBonus, 0, null));
			this.Fields.Add(PlayerFields.TrapBonusPercent, new StatsData(this.Owner, PlayerFields.TrapBonusPercent, 0, null));
			this.Fields.Add(PlayerFields.PermanentDamagePercent, new StatsData(this.Owner, PlayerFields.PermanentDamagePercent, 0, null));
			this.Fields.Add(PlayerFields.TackleBlock, new StatsData(this.Owner, PlayerFields.TackleBlock, (int)record.TackleBlock, StatsFields.FormulasAgilityDependant));
			this.Fields.Add(PlayerFields.TackleEvade, new StatsData(this.Owner, PlayerFields.TackleEvade, (int)record.TackleEvade, StatsFields.FormulasAgilityDependant));
			this.Fields.Add(PlayerFields.APAttack, new StatsData(this.Owner, PlayerFields.APAttack, 0, null));
			this.Fields.Add(PlayerFields.MPAttack, new StatsData(this.Owner, PlayerFields.MPAttack, 0, null));
			this.Fields.Add(PlayerFields.PushDamageBonus, new StatsData(this.Owner, PlayerFields.PushDamageBonus, 0, null));
			this.Fields.Add(PlayerFields.CriticalDamageBonus, new StatsData(this.Owner, PlayerFields.CriticalDamageBonus, 0, null));
			this.Fields.Add(PlayerFields.NeutralDamageBonus, new StatsData(this.Owner, PlayerFields.NeutralDamageBonus, 0, null));
			this.Fields.Add(PlayerFields.EarthDamageBonus, new StatsData(this.Owner, PlayerFields.EarthDamageBonus, 0, null));
			this.Fields.Add(PlayerFields.WaterDamageBonus, new StatsData(this.Owner, PlayerFields.WaterDamageBonus, 0, null));
			this.Fields.Add(PlayerFields.AirDamageBonus, new StatsData(this.Owner, PlayerFields.AirDamageBonus, 0, null));
			this.Fields.Add(PlayerFields.FireDamageBonus, new StatsData(this.Owner, PlayerFields.FireDamageBonus, 0, null));
			this.Fields.Add(PlayerFields.DodgeAPProbability, new StatsData(this.Owner, PlayerFields.DodgeAPProbability, (int)((short)record.PaDodge), StatsFields.FormulasWisdomDependant));
			this.Fields.Add(PlayerFields.DodgeMPProbability, new StatsData(this.Owner, PlayerFields.DodgeMPProbability, (int)((short)record.PmDodge), StatsFields.FormulasWisdomDependant));
			this.Fields.Add(PlayerFields.NeutralResistPercent, new StatsData(this.Owner, PlayerFields.NeutralResistPercent, (int)((short)record.NeutralResistance), null));
			this.Fields.Add(PlayerFields.EarthResistPercent, new StatsData(this.Owner, PlayerFields.EarthResistPercent, (int)((short)record.EarthResistance), null));
			this.Fields.Add(PlayerFields.WaterResistPercent, new StatsData(this.Owner, PlayerFields.WaterResistPercent, (int)((short)record.WaterResistance), null));
			this.Fields.Add(PlayerFields.AirResistPercent, new StatsData(this.Owner, PlayerFields.AirResistPercent, (int)((short)record.AirResistance), null));
			this.Fields.Add(PlayerFields.FireResistPercent, new StatsData(this.Owner, PlayerFields.FireResistPercent, (int)((short)record.FireResistance), null));
			this.Fields.Add(PlayerFields.NeutralElementReduction, new StatsData(this.Owner, PlayerFields.NeutralElementReduction, 0, null));
			this.Fields.Add(PlayerFields.EarthElementReduction, new StatsData(this.Owner, PlayerFields.EarthElementReduction, 0, null));
			this.Fields.Add(PlayerFields.WaterElementReduction, new StatsData(this.Owner, PlayerFields.WaterElementReduction, 0, null));
			this.Fields.Add(PlayerFields.AirElementReduction, new StatsData(this.Owner, PlayerFields.AirElementReduction, 0, null));
			this.Fields.Add(PlayerFields.FireElementReduction, new StatsData(this.Owner, PlayerFields.FireElementReduction, 0, null));
			this.Fields.Add(PlayerFields.PushDamageReduction, new StatsData(this.Owner, PlayerFields.PushDamageReduction, 0, null));
			this.Fields.Add(PlayerFields.CriticalDamageReduction, new StatsData(this.Owner, PlayerFields.CriticalDamageReduction, 0, null));
			this.Fields.Add(PlayerFields.PvpNeutralResistPercent, new StatsData(this.Owner, PlayerFields.PvpNeutralResistPercent, 0, null));
			this.Fields.Add(PlayerFields.PvpEarthResistPercent, new StatsData(this.Owner, PlayerFields.PvpEarthResistPercent, 0, null));
			this.Fields.Add(PlayerFields.PvpWaterResistPercent, new StatsData(this.Owner, PlayerFields.PvpWaterResistPercent, 0, null));
			this.Fields.Add(PlayerFields.PvpAirResistPercent, new StatsData(this.Owner, PlayerFields.PvpAirResistPercent, 0, null));
			this.Fields.Add(PlayerFields.PvpFireResistPercent, new StatsData(this.Owner, PlayerFields.PvpFireResistPercent, 0, null));
			this.Fields.Add(PlayerFields.PvpNeutralElementReduction, new StatsData(this.Owner, PlayerFields.PvpNeutralElementReduction, 0, null));
			this.Fields.Add(PlayerFields.PvpEarthElementReduction, new StatsData(this.Owner, PlayerFields.PvpEarthElementReduction, 0, null));
			this.Fields.Add(PlayerFields.PvpWaterElementReduction, new StatsData(this.Owner, PlayerFields.PvpWaterElementReduction, 0, null));
			this.Fields.Add(PlayerFields.PvpAirElementReduction, new StatsData(this.Owner, PlayerFields.PvpAirElementReduction, 0, null));
			this.Fields.Add(PlayerFields.PvpFireElementReduction, new StatsData(this.Owner, PlayerFields.PvpFireElementReduction, 0, null));
			this.Fields.Add(PlayerFields.GlobalDamageReduction, new StatsData(this.Owner, PlayerFields.GlobalDamageReduction, 0, null));
			this.Fields.Add(PlayerFields.DamageMultiplicator, new StatsData(this.Owner, PlayerFields.DamageMultiplicator, 0, null));
			this.Fields.Add(PlayerFields.PhysicalDamage, new StatsData(this.Owner, PlayerFields.PhysicalDamage, 0, null));
			this.Fields.Add(PlayerFields.MagicDamage, new StatsData(this.Owner, PlayerFields.MagicDamage, 0, null));
			this.Fields.Add(PlayerFields.PhysicalDamageReduction, new StatsData(this.Owner, PlayerFields.PhysicalDamageReduction, 0, null));
			this.Fields.Add(PlayerFields.MagicDamageReduction, new StatsData(this.Owner, PlayerFields.MagicDamageReduction, 0, null));
			this.Fields.Add(PlayerFields.WaterDamageArmor, new StatsData(this.Owner, PlayerFields.WaterDamageArmor, 0, null));
			this.Fields.Add(PlayerFields.EarthDamageArmor, new StatsData(this.Owner, PlayerFields.EarthDamageArmor, 0, null));
			this.Fields.Add(PlayerFields.NeutralDamageArmor, new StatsData(this.Owner, PlayerFields.NeutralDamageArmor, 0, null));
			this.Fields.Add(PlayerFields.AirDamageArmor, new StatsData(this.Owner, PlayerFields.AirDamageArmor, 0, null));
			this.Fields.Add(PlayerFields.FireDamageArmor, new StatsData(this.Owner, PlayerFields.FireDamageArmor, 0, null));
			this.Fields.Add(PlayerFields.Erosion, new StatsData(this.Owner, PlayerFields.Erosion, 10, null));
			foreach (System.Collections.Generic.KeyValuePair<PlayerFields, short> current in record.Stats)
			{
				this.Fields[current.Key].Base = (int)current.Value;
			}
		}
		public void Initialize(TaxCollectorNpc taxCollector)
		{
			this.Fields = new System.Collections.Generic.Dictionary<PlayerFields, StatsData>();
			this.Fields.Add(PlayerFields.Initiative, new StatsInitiative(this.Owner, 0));
			this.Fields.Add(PlayerFields.Prospecting, new StatsData(this.Owner, PlayerFields.Prospecting, taxCollector.Guild.TaxCollectorProspecting, StatsFields.FormulasChanceDependant));
			this.Fields.Add(PlayerFields.AP, new StatsAP(this.Owner, TaxCollectorNpc.BaseAP));
			this.Fields.Add(PlayerFields.MP, new StatsMP(this.Owner, TaxCollectorNpc.BaseMP));
			this.Fields.Add(PlayerFields.Strength, new StatsData(this.Owner, PlayerFields.Strength, 0, null));
			this.Fields.Add(PlayerFields.Vitality, new StatsData(this.Owner, PlayerFields.Vitality, 0, null));
			this.Fields.Add(PlayerFields.Health, new StatsHealth(this.Owner, taxCollector.Guild.TaxCollectorHealth, 0));
			this.Fields.Add(PlayerFields.Wisdom, new StatsData(this.Owner, PlayerFields.Wisdom, taxCollector.Guild.TaxCollectorWisdom, null));
			this.Fields.Add(PlayerFields.Chance, new StatsData(this.Owner, PlayerFields.Chance, 0, null));
			this.Fields.Add(PlayerFields.Agility, new StatsData(this.Owner, PlayerFields.Agility, 0, null));
			this.Fields.Add(PlayerFields.Intelligence, new StatsData(this.Owner, PlayerFields.Intelligence, 0, null));
			this.Fields.Add(PlayerFields.Range, new StatsData(this.Owner, PlayerFields.Range, 0, null));
			this.Fields.Add(PlayerFields.SummonLimit, new StatsData(this.Owner, PlayerFields.SummonLimit, 1, null));
			this.Fields.Add(PlayerFields.DamageReflection, new StatsData(this.Owner, PlayerFields.DamageReflection, 0, null));
			this.Fields.Add(PlayerFields.CriticalHit, new StatsData(this.Owner, PlayerFields.CriticalHit, 0, null));
			this.Fields.Add(PlayerFields.CriticalMiss, new StatsData(this.Owner, PlayerFields.CriticalMiss, 0, null));
			this.Fields.Add(PlayerFields.HealBonus, new StatsData(this.Owner, PlayerFields.HealBonus, 0, null));
			this.Fields.Add(PlayerFields.DamageBonus, new StatsData(this.Owner, PlayerFields.DamageBonus, taxCollector.Guild.TaxCollectorDamageBonuses, null));
			this.Fields.Add(PlayerFields.WeaponDamageBonus, new StatsData(this.Owner, PlayerFields.WeaponDamageBonus, 0, null));
			this.Fields.Add(PlayerFields.DamageBonusPercent, new StatsData(this.Owner, PlayerFields.DamageBonusPercent, 0, null));
			this.Fields.Add(PlayerFields.TrapBonus, new StatsData(this.Owner, PlayerFields.TrapBonus, 0, null));
			this.Fields.Add(PlayerFields.TrapBonusPercent, new StatsData(this.Owner, PlayerFields.TrapBonusPercent, 0, null));
			this.Fields.Add(PlayerFields.PermanentDamagePercent, new StatsData(this.Owner, PlayerFields.PermanentDamagePercent, 0, null));
			this.Fields.Add(PlayerFields.TackleBlock, new StatsData(this.Owner, PlayerFields.TackleBlock, 50, StatsFields.FormulasAgilityDependant));
			this.Fields.Add(PlayerFields.TackleEvade, new StatsData(this.Owner, PlayerFields.TackleEvade, 50, StatsFields.FormulasAgilityDependant));
			this.Fields.Add(PlayerFields.APAttack, new StatsData(this.Owner, PlayerFields.APAttack, 50, null));
			this.Fields.Add(PlayerFields.MPAttack, new StatsData(this.Owner, PlayerFields.MPAttack, 50, null));
			this.Fields.Add(PlayerFields.PushDamageBonus, new StatsData(this.Owner, PlayerFields.PushDamageBonus, 0, null));
			this.Fields.Add(PlayerFields.CriticalDamageBonus, new StatsData(this.Owner, PlayerFields.CriticalDamageBonus, 0, null));
			this.Fields.Add(PlayerFields.NeutralDamageBonus, new StatsData(this.Owner, PlayerFields.NeutralDamageBonus, 0, null));
			this.Fields.Add(PlayerFields.EarthDamageBonus, new StatsData(this.Owner, PlayerFields.EarthDamageBonus, 0, null));
			this.Fields.Add(PlayerFields.WaterDamageBonus, new StatsData(this.Owner, PlayerFields.WaterDamageBonus, 0, null));
			this.Fields.Add(PlayerFields.AirDamageBonus, new StatsData(this.Owner, PlayerFields.AirDamageBonus, 0, null));
			this.Fields.Add(PlayerFields.FireDamageBonus, new StatsData(this.Owner, PlayerFields.FireDamageBonus, 0, null));
			this.Fields.Add(PlayerFields.DodgeAPProbability, new StatsData(this.Owner, PlayerFields.DodgeAPProbability, TaxCollectorNpc.BaseResistance, StatsFields.FormulasWisdomDependant));
			this.Fields.Add(PlayerFields.DodgeMPProbability, new StatsData(this.Owner, PlayerFields.DodgeMPProbability, TaxCollectorNpc.BaseResistance, StatsFields.FormulasWisdomDependant));
			this.Fields.Add(PlayerFields.NeutralResistPercent, new StatsData(this.Owner, PlayerFields.NeutralResistPercent, TaxCollectorNpc.BaseResistance, null));
			this.Fields.Add(PlayerFields.EarthResistPercent, new StatsData(this.Owner, PlayerFields.EarthResistPercent, TaxCollectorNpc.BaseResistance, null));
			this.Fields.Add(PlayerFields.WaterResistPercent, new StatsData(this.Owner, PlayerFields.WaterResistPercent, TaxCollectorNpc.BaseResistance, null));
			this.Fields.Add(PlayerFields.AirResistPercent, new StatsData(this.Owner, PlayerFields.AirResistPercent, TaxCollectorNpc.BaseResistance, null));
			this.Fields.Add(PlayerFields.FireResistPercent, new StatsData(this.Owner, PlayerFields.FireResistPercent, TaxCollectorNpc.BaseResistance, null));
			this.Fields.Add(PlayerFields.NeutralElementReduction, new StatsData(this.Owner, PlayerFields.NeutralElementReduction, 0, null));
			this.Fields.Add(PlayerFields.EarthElementReduction, new StatsData(this.Owner, PlayerFields.EarthElementReduction, 0, null));
			this.Fields.Add(PlayerFields.WaterElementReduction, new StatsData(this.Owner, PlayerFields.WaterElementReduction, 0, null));
			this.Fields.Add(PlayerFields.AirElementReduction, new StatsData(this.Owner, PlayerFields.AirElementReduction, 0, null));
			this.Fields.Add(PlayerFields.FireElementReduction, new StatsData(this.Owner, PlayerFields.FireElementReduction, 0, null));
			this.Fields.Add(PlayerFields.PushDamageReduction, new StatsData(this.Owner, PlayerFields.PushDamageReduction, 0, null));
			this.Fields.Add(PlayerFields.CriticalDamageReduction, new StatsData(this.Owner, PlayerFields.CriticalDamageReduction, 0, null));
			this.Fields.Add(PlayerFields.PvpNeutralResistPercent, new StatsData(this.Owner, PlayerFields.PvpNeutralResistPercent, 0, null));
			this.Fields.Add(PlayerFields.PvpEarthResistPercent, new StatsData(this.Owner, PlayerFields.PvpEarthResistPercent, 0, null));
			this.Fields.Add(PlayerFields.PvpWaterResistPercent, new StatsData(this.Owner, PlayerFields.PvpWaterResistPercent, 0, null));
			this.Fields.Add(PlayerFields.PvpAirResistPercent, new StatsData(this.Owner, PlayerFields.PvpAirResistPercent, 0, null));
			this.Fields.Add(PlayerFields.PvpFireResistPercent, new StatsData(this.Owner, PlayerFields.PvpFireResistPercent, 0, null));
			this.Fields.Add(PlayerFields.PvpNeutralElementReduction, new StatsData(this.Owner, PlayerFields.PvpNeutralElementReduction, 0, null));
			this.Fields.Add(PlayerFields.PvpEarthElementReduction, new StatsData(this.Owner, PlayerFields.PvpEarthElementReduction, 0, null));
			this.Fields.Add(PlayerFields.PvpWaterElementReduction, new StatsData(this.Owner, PlayerFields.PvpWaterElementReduction, 0, null));
			this.Fields.Add(PlayerFields.PvpAirElementReduction, new StatsData(this.Owner, PlayerFields.PvpAirElementReduction, 0, null));
			this.Fields.Add(PlayerFields.PvpFireElementReduction, new StatsData(this.Owner, PlayerFields.PvpFireElementReduction, 0, null));
			this.Fields.Add(PlayerFields.GlobalDamageReduction, new StatsData(this.Owner, PlayerFields.GlobalDamageReduction, 0, null));
			this.Fields.Add(PlayerFields.DamageMultiplicator, new StatsData(this.Owner, PlayerFields.DamageMultiplicator, 0, null));
			this.Fields.Add(PlayerFields.PhysicalDamage, new StatsData(this.Owner, PlayerFields.PhysicalDamage, 0, null));
			this.Fields.Add(PlayerFields.MagicDamage, new StatsData(this.Owner, PlayerFields.MagicDamage, 0, null));
			this.Fields.Add(PlayerFields.PhysicalDamageReduction, new StatsData(this.Owner, PlayerFields.PhysicalDamageReduction, 0, null));
			this.Fields.Add(PlayerFields.MagicDamageReduction, new StatsData(this.Owner, PlayerFields.MagicDamageReduction, 0, null));
			this.Fields.Add(PlayerFields.WaterDamageArmor, new StatsData(this.Owner, PlayerFields.WaterDamageArmor, 0, null));
			this.Fields.Add(PlayerFields.EarthDamageArmor, new StatsData(this.Owner, PlayerFields.EarthDamageArmor, 0, null));
			this.Fields.Add(PlayerFields.NeutralDamageArmor, new StatsData(this.Owner, PlayerFields.NeutralDamageArmor, 0, null));
			this.Fields.Add(PlayerFields.AirDamageArmor, new StatsData(this.Owner, PlayerFields.AirDamageArmor, 0, null));
			this.Fields.Add(PlayerFields.FireDamageArmor, new StatsData(this.Owner, PlayerFields.FireDamageArmor, 0, null));
			this.Fields.Add(PlayerFields.Erosion, new StatsData(this.Owner, PlayerFields.Erosion, 10, null));
		}
		public StatsFields CloneAndChangeOwner(IStatsOwner owner)
		{
			StatsFields statsFields = new StatsFields(owner);
			statsFields.Fields = this.Fields.ToDictionary((System.Collections.Generic.KeyValuePair<PlayerFields, StatsData> x) => x.Key, (System.Collections.Generic.KeyValuePair<PlayerFields, StatsData> x) => x.Value.CloneAndChangeOwner(owner));
			return statsFields;
		}
	}
}

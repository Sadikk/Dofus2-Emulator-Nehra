using Stump.Core.Reflection;
using Stump.Core.Threading;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Look;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Actors.Stats;
using Stump.Server.WorldServer.Game.Effects;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Fights;
using Stump.Server.WorldServer.Game.Fights.Buffs;
using Stump.Server.WorldServer.Game.Fights.Results;
using Stump.Server.WorldServer.Game.Fights.Teams;
using Stump.Server.WorldServer.Game.Items.Player;
using Stump.Server.WorldServer.Game.Maps.Cells;
using Stump.Server.WorldServer.Game.Maps.Cells.Shapes;
using Stump.Server.WorldServer.Game.Spells;
using Stump.Server.WorldServer.Handlers.Basic;
using Stump.Server.WorldServer.Handlers.Context;
using System;
using System.Drawing;
using System.Linq;
using Stump.Server.WorldServer.Game.Effects.Spells;

namespace Stump.Server.WorldServer.Game.Actors.Fight
{
	public sealed class CharacterFighter : NamedFighter
	{
        // FIELDS
		private int m_criticalWeaponBonus;
		private int m_damageTakenBeforeFight;
		private short m_earnedDishonor;
		private int m_earnedExp;
		private int m_guildEarnedExp;
		private short m_earnedHonor;
		private bool m_isUsingWeapon;

        // PROPERTIES
		public Character Character
		{
			get;
			private set;
		}
		public ReadyChecker PersonalReadyChecker
		{
			get;
			set;
		}
		public override int Id
		{
			get
			{
				return this.Character.Id;
			}
		}
		public override string Name
		{
			get
			{
				return this.Character.Name;
			}
		}
		public override ActorLook Look
		{
			get;
			set;
		}
		public override ObjectPosition MapPosition
		{
			get
			{
				return this.Character.Position;
			}
		}
		public override short Level
		{
			get
			{
				return this.Character.Level;
			}
		}
		public override StatsFields Stats
		{
			get
			{
				return this.Character.Stats;
			}
		}
		public bool IsDisconnected
		{
			get;
			private set;
		}
        public int? RemainingRounds
        {
            get;
            private set;
        }

        // CONSTRUCTORS
		public CharacterFighter(Character character, FightTeam team) : base(team)
		{
			this.Character = character;
			this.Look = this.Character.Look.Clone();
			this.Look.RemoveAuras();
			Cell cell;

			if (base.Fight.FindRandomFreeCell(this, out cell, false))
			{
				this.Position = new ObjectPosition(character.Map, cell, character.Direction);
				this.InitializeCharacterFighter();
			}
		}

        // METHODS
		private void InitializeCharacterFighter()
		{
			this.m_damageTakenBeforeFight = this.Stats.Health.DamageTaken;
			if (base.Fight.FightType == FightTypeEnum.FIGHT_TYPE_CHALLENGE)
			{
				this.Stats.Health.DamageTaken = 0;
			}
		}
		public override ObjectPosition GetLeaderBladePosition()
		{
			return this.Character.GetPositionBeforeMove();
		}
		public void ToggleTurnReady(bool ready)
		{
			if (this.PersonalReadyChecker != null)
			{
				this.PersonalReadyChecker.ToggleReady(this, ready);
			}
			else
			{
				if (base.Fight.ReadyChecker != null)
				{
					base.Fight.ReadyChecker.ToggleReady(this, ready);
				}
			}
		}
		public override bool CastSpell(Spell spell, Cell cell)
		{
			bool result;
			if (!base.IsFighterTurn())
			{
				result = false;
			}
			else
			{
				if (spell.Id != 0 || this.Character.Inventory.TryGetItem(CharacterInventoryPositionEnum.ACCESSORY_POSITION_WEAPON) == null)
				{
					result = base.CastSpell(spell, cell);
				}
				else
				{
					BasePlayerItem basePlayerItem = this.Character.Inventory.TryGetItem(CharacterInventoryPositionEnum.ACCESSORY_POSITION_WEAPON);
					WeaponTemplate weaponTemplate = basePlayerItem.Template as WeaponTemplate;
					if (weaponTemplate == null || !this.CanUseWeapon(cell, weaponTemplate))
					{
						result = false;
					}
					else
					{
						base.Fight.StartSequence(SequenceTypeEnum.SEQUENCE_WEAPON);
						AsyncRandom asyncRandom = new AsyncRandom();
						FightSpellCastCriticalEnum fightSpellCastCriticalEnum = this.RollCriticalDice(weaponTemplate);
						if (fightSpellCastCriticalEnum == FightSpellCastCriticalEnum.CRITICAL_FAIL)
						{
							this.OnWeaponUsed(weaponTemplate, cell, fightSpellCastCriticalEnum, false);
							this.UseAP((short)weaponTemplate.ApCost);
							base.Fight.EndSequence(SequenceTypeEnum.SEQUENCE_WEAPON, false);
							base.PassTurn();
							result = false;
						}
						else
						{
							if (fightSpellCastCriticalEnum == FightSpellCastCriticalEnum.CRITICAL_HIT)
							{
								this.m_criticalWeaponBonus = weaponTemplate.CriticalHitBonus;
							}
							this.m_isUsingWeapon = true;
							System.Collections.Generic.IEnumerable<EffectDice> enumerable = (
								from entry in basePlayerItem.Effects
								where Singleton<EffectManager>.Instance.IsUnRandomableWeaponEffect(entry.EffectId)
								select entry).OfType<EffectDice>();
							System.Collections.Generic.List<SpellEffectHandler> list = new System.Collections.Generic.List<SpellEffectHandler>();
							foreach (EffectDice current in enumerable)
							{
								if (current.Random <= 0 || asyncRandom.NextDouble() <= (double)current.Random / 100.0)
								{
									SpellEffectHandler spellEffectHandler = Singleton<EffectManager>.Instance.GetSpellEffectHandler(current, this, spell, cell, fightSpellCastCriticalEnum == FightSpellCastCriticalEnum.CRITICAL_HIT);
									spellEffectHandler.EffectZone = new Zone(weaponTemplate.Type.ZoneShape, (byte)weaponTemplate.Type.ZoneSize, spellEffectHandler.CastPoint.OrientationTo(spellEffectHandler.TargetedPoint, true));
									spellEffectHandler.Targets = (SpellTargetType.ALLY_1 | SpellTargetType.ALLY_2 | SpellTargetType.ALLY_SUMMONS | SpellTargetType.ALLY_STATIC_SUMMONS | SpellTargetType.ALLY_3 | SpellTargetType.ALLY_4 | SpellTargetType.ALLY_5 | SpellTargetType.ENNEMY_1 | SpellTargetType.ENNEMY_2 | SpellTargetType.ENNEMY_SUMMONS | SpellTargetType.ENNEMY_STATIC_SUMMONS | SpellTargetType.ENNEMY_3 | SpellTargetType.ENNEMY_4 | SpellTargetType.ENNEMY_5);
									list.Add(spellEffectHandler);
								}
							}
							bool silentCast = list.Any((SpellEffectHandler entry) => entry.RequireSilentCast());
							this.OnWeaponUsed(weaponTemplate, cell, fightSpellCastCriticalEnum, silentCast);
							this.UseAP((short)weaponTemplate.ApCost);
							foreach (SpellEffectHandler spellEffectHandler in list)
							{
								spellEffectHandler.Apply();
							}
							base.Fight.EndSequence(SequenceTypeEnum.SEQUENCE_WEAPON, false);
							this.m_isUsingWeapon = false;
							this.m_criticalWeaponBonus = 0;
							base.Fight.CheckFightEnd();
							result = true;
						}
					}
				}
			}
			return result;
		}
        public override SpellCastResult CanCastSpell(Spell spell, Cell cell)
        {
            SpellCastResult spellCastResult = base.CanCastSpell(spell, cell);
            SpellCastResult result;
            if (spellCastResult == SpellCastResult.OK)
            {
                result = spellCastResult;
            }
            else
            {
                switch (spellCastResult)
                {
                    case SpellCastResult.NO_LOS:
                        this.Character.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 174, new object[0]);
                        goto IL_111;
                    case SpellCastResult.CELL_NOT_FREE:
                    case SpellCastResult.UNWALKABLE_CELL:
                        this.Character.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 172, new object[0]);
                        goto IL_111;
                    case SpellCastResult.NOT_ENOUGH_AP:
                        this.Character.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 170, new object[] { base.AP, spell.CurrentSpellLevel.ApCost });
                        goto IL_111;
                    case SpellCastResult.HAS_NOT_SPELL:
                        this.Character.SendInformationMessage(TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 169, new object[0]);
                        goto IL_111;
                }
                BasicHandler.SendTextInformationMessage(this.Character.Client, TextInformationTypeEnum.TEXT_INFORMATION_ERROR, 175);
                this.Character.SendServerMessage("(" + spellCastResult + ")", Color.Red);

            IL_111:
                result = spellCastResult;
            }
            return result;
        }
		public override int CalculateDamage(int damage, EffectSchoolEnum type)
		{
			int result;
			if (this.Character.GodMode)
			{
				result = 32767;
			}
			else
			{
				result = base.CalculateDamage((this.m_isUsingWeapon ? (this.m_criticalWeaponBonus + this.Stats[PlayerFields.WeaponDamageBonus]) : 0) + damage, type);
			}
			return result;
		}
		public bool CanUseWeapon(Cell cell, WeaponTemplate weapon)
		{
			bool result;
			if (!base.IsFighterTurn())
			{
				result = false;
			}
			else
			{
				MapPoint mapPoint = new MapPoint(cell);
				result = ((ulong)mapPoint.DistanceToCell(this.Position.Point) <= (ulong)((long)weapon.WeaponRange) && (ulong)mapPoint.DistanceToCell(this.Position.Point) >= (ulong)((long)weapon.MinRange) && base.AP >= weapon.ApCost && base.Fight.CanBeSeen(cell, this.Position.Cell, false));
			}
			return result;
		}
		public override Spell GetSpell(int id)
		{
			return this.Character.Spells.GetSpell(id);
		}
		public override bool HasSpell(int id)
		{
			return this.Character.Spells.HasSpell(id);
		}

		public FightSpellCastCriticalEnum RollCriticalDice(WeaponTemplate weapon)
        {
            AsyncRandom asyncRandom = new AsyncRandom();

            FightSpellCastCriticalEnum result = FightSpellCastCriticalEnum.NORMAL;
            if (weapon.CriticalHitProbability != 0u && asyncRandom.Next(100) < this.CalculateCriticRate((uint)weapon.CriticalHitProbability))
            {
                result = FightSpellCastCriticalEnum.CRITICAL_HIT;
            }

            return result;
        }
		public override void ResetFightProperties()
		{
			base.ResetFightProperties();
			if (base.Fight is FightDuel)
			{
				this.Stats.Health.DamageTaken = this.m_damageTakenBeforeFight;
			}
			else
			{
				if (this.Stats.Health.Total <= 0)
				{
					this.Stats.Health.DamageTaken = (int)((short)(this.Stats.Health.TotalMax - 1));
				}
			}
		}
		
        public void EnterDisconnectedState()
		{
			this.IsDisconnected = true;
            this.RemainingRounds = new int?(20);

            this.Character.Record.LeftFightId = this.Fight.Id;
		}
        public void LeaveDisconnectedState()
        {
            this.IsDisconnected = false;
            this.RemainingRounds = null;

            this.Character.Record.LeftFightId = null;
        }

		public override IFightResult GetFightResult()
		{
			return new FightPlayerResult(this, base.GetFighterOutcome(), base.Loot);
		}
		public override FightTeamMemberInformations GetFightTeamMemberInformations()
		{
			return new FightTeamMemberCharacterInformations(this.Id, this.Name, (byte)this.Character.Level);
		}
		public override GameFightFighterInformations GetGameFightFighterInformations(WorldClient client = null)
		{
			return new GameFightCharacterInformations(
                this.Id, 
                this.Look.GetEntityLook(), 
                this.GetEntityDispositionInformations(client), 
                base.Team.Id,
                0, 
                base.IsAlive(), 
                this.GetGameFightMinimalStats(client), 
                Enumerable.Empty<ushort>(), 
                this.Name, 
                new PlayerStatus(), 
                (byte)this.Character.Level, 
                this.Character.GetActorAlignmentInformations(), 
                (sbyte)this.Character.Breed.Id,
                Character.Sex == SexTypeEnum.SEX_FEMALE);
		}
		public override string ToString()
		{
			return this.Character.ToString();
		}
		public override bool UseAP(short amount)
		{
			bool result;
			if (!this.Character.GodMode)
			{
				result = base.UseAP(amount);
			}
			else
			{
				base.UseAP(amount);
				this.RegainAP(amount);
				result = true;
			}
			return result;
		}
        public override bool UseMP(short amount)
        {
            bool result;
            if (!this.Character.GodMode)
            {
                result = base.UseMP(amount);
            }
            else
            {
                base.UseMP(amount);
                base.RegainMP(amount);
                result = true;
            }
            return result;
        }
		public override bool LostAP(short amount)
		{
			bool result;
			if (!this.Character.GodMode)
			{
				result = base.LostAP(amount);
			}
			else
			{
				base.LostAP(amount);
				this.RegainAP(amount);
				result = true;
			}
			return result;
		}
		public override bool LostMP(short amount)
		{
			return this.Character.GodMode || base.LostMP(amount);
		}
		public override int InflictDamage(Damage damage)
		{
			int result;
			if (!this.Character.GodMode)
			{
				result = base.InflictDamage(damage);
			}
			else
			{
				damage.GenerateDamages();
				this.OnBeforeDamageInflicted(damage);
				base.TriggerBuffs(BuffTriggerType.BEFORE_ATTACKED, damage);
				this.OnDamageReducted(damage.Source, damage.Amount);
				base.TriggerBuffs(BuffTriggerType.AFTER_ATTACKED, damage);
				this.OnDamageInflicted(damage);
				result = 0;
			}
			return result;
		}

        public override GameFightFighterLightInformations GetGameFightFighterLightInformations()
        {
            return new GameFightFighterNamedLightInformations(Character.Sex == SexTypeEnum.SEX_MALE, IsAlive(), Id, 0, (ushort)Level, (sbyte)Character.BreedId, Character.Name);
        }

        protected override void OnTurnStarted()
        {
            base.OnTurnStarted();

            lock (this.Character.LoggoutSync)
            {
                if (this.IsDisconnected)
                {
                    this.RemainingRounds--;
                    if (this.RemainingRounds == 0)
                    {
                        this.LeaveFight();
                    }
                    else
                    {
                        BasicHandler.SendTextInformationMessage(this.Fight.Clients, TextInformationTypeEnum.TEXT_INFORMATION_MESSAGE, 162, this.Character.Name, this.RemainingRounds.Value);
                        if (this.Team.GetAllFighters(entry => !(entry is SummonedFighter)).Count() > 1)
                        {
                            this.PassTurn();
                        }
                    }
                }
            }
        }

        public override void RefreshWaitTime(uint remainingDurationSeconds)
        {
            this._waitTime = (uint)Fights.Fight.TurnTime;

            if (remainingDurationSeconds > 0)
            {
                var loc14 = this.GetBasicTurnDuration();
                var loc15 = Math.Floor(remainingDurationSeconds / 2.0);
                if (loc14 + loc15 > 60)
                {
                    loc15 = 60 - loc14;
                }

                if (loc15 > 0)
                {
                    this._waitTime += (uint)loc15;
                }
            }
        }
        public override void RefreshFighterStatsListMessage()
        {
            ContextHandler.SendFighterStatsListMessage(this.Character.Client, this);
        }

        public void ChangeSource(Character character)
        {
            if (character.Id != this.Character.Id)
            {
                throw new Exception("The ChangeSource method can only accept the same character record.");
            }

            this.Character = character;
        }

        private uint GetBasicTurnDuration()
        {
            var loc1 = this.AP;
            var loc2 = this.MP;

            return (uint)(15 + loc1 + loc2);
        }
	}
}

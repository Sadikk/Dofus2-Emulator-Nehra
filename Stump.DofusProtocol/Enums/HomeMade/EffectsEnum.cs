namespace Stump.DofusProtocol.Enums
{
    public enum EffectsEnum : int
    {
        /// <summary>
        ///Teleports to the targeted map
        /// </summary>
        Effect_2 = 2,
        /// <summary>
        ///Set respawn point
        /// </summary>
        Effect_3 = 3,
        /// <summary>
        ///Teleports to the targeted cell
        /// </summary>
        Effect_Teleport = 4,
        /// <summary>
        ///Pushes back #1 cell(s)
        /// </summary>
        Effect_PushBack = 5,
        /// <summary>
        ///Attracts #1 cell(s)
        /// </summary>
        Effect_PullForward = 6,
        /// <summary>
        ///Get a divorce
        /// </summary>
        Effect_Divorce = 7,
        /// <summary>
        ///Switches positions
        /// </summary>
        Effect_SwitchPosition = 8,
        /// <summary>
        ///Avoids #1% of hits by moving back #2 cell(s)
        /// </summary>
        Effect_Dodge = 9,
        /// <summary>
        ///#3 emote
        /// </summary>
        Effect_Attitude = 10,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_12 = 12,
        /// <summary>
        ///Changes the playing time of a player
        /// </summary>
        Effect_ChangeTimeOfPlayCharacter = 13,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_30 = 30,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_31 = 31,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_32 = 32,
        /// <summary>
        ///Begins a quest
        /// </summary>
        Effect_BeginMission = 34,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_36 = 36,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_37 = 37,
        /// <summary>
        ///Carries the target
        /// </summary>
        Effect_Carry = 50,
        /// <summary>
        ///Throws the target
        /// </summary>
        Effect_Throw = 51,
        /// <summary>
        ///Steals #1{~1~2 to }#2 MP
        /// </summary>
        Effect_StealMP_77 = 77,
        /// <summary>
        ///+#1{~1~2 to }#2 MP
        /// </summary>
        Effect_AddMP = 78,
        /// <summary>
        ///#3% healed by x#2, or else damage suffered x#1
        /// </summary>
        Effect_79 = 79,
        /// <summary>
        ///#1{~1~2 to }#2 (HP restored)
        /// </summary>
        Effect_HealHP_81 = 81,
        /// <summary>
        ///#1{~1~2 to }#2 HP (fixed Neutral steal)
        /// </summary>
        Effect_StealHPFix = 82,
        /// <summary>
        ///Steals #1{~1~2 to }#2 AP
        /// </summary>
        Effect_StealAP_84 = 84,
        /// <summary>
        ///#1{~1~2 to }#2% of attacker's HP (Water damage)
        /// </summary>
        Effect_DamagePercentWater = 85,
        /// <summary>
        ///#1{~1~2 to }#2% of attacker's HP (Earth damage)
        /// </summary>
        Effect_DamagePercentEarth = 86,
        /// <summary>
        ///#1{~1~2 to }#2% of attacker's HP (Air damage)
        /// </summary>
        Effect_DamagePercentAir = 87,
        /// <summary>
        ///#1{~1~2 to }#2% of attacker's HP (Fire damage)
        /// </summary>
        Effect_DamagePercentFire = 88,
        /// <summary>
        ///#1{~1~2 to }#2% of attacker's HP (Neutral damage)
        /// </summary>
        Effect_DamagePercentNeutral = 89,
        /// <summary>
        ///Gives #1{~1~2 to }#2% of own HP
        /// </summary>
        Effect_GiveHPPercent = 90,
        /// <summary>
        ///#1{~1~2 to }#2 (Water steal)
        /// </summary>
        Effect_StealHPWater = 91,
        /// <summary>
        ///#1{~1~2 to }#2 (Earth steal)
        /// </summary>
        Effect_StealHPEarth = 92,
        /// <summary>
        ///#1{~1~2 to }#2 (Air steal)
        /// </summary>
        Effect_StealHPAir = 93,
        /// <summary>
        ///#1{~1~2 to }#2 (Fire steal)
        /// </summary>
        Effect_StealHPFire = 94,
        /// <summary>
        ///#1{~1~2 to }#2 (Neutral steal)
        /// </summary>
        Effect_StealHPNeutral = 95,
        /// <summary>
        ///#1{~1~2 to }#2 (Water damage)
        /// </summary>
        Effect_DamageWater = 96,
        /// <summary>
        ///#1{~1~2 to }#2 (Earth damage)
        /// </summary>
        Effect_DamageEarth = 97,
        /// <summary>
        ///#1{~1~2 to }#2 (Air damage)
        /// </summary>
        Effect_DamageAir = 98,
        /// <summary>
        ///#1{~1~2 to }#2 (Fire damage)
        /// </summary>
        Effect_DamageFire = 99,
        /// <summary>
        ///#1{~1~2 to }#2 (Neutral damage)
        /// </summary>
        Effect_DamageNeutral = 100,
        /// <summary>
        ///-#1{~1~2 to }#2 AP
        /// </summary>
        Effect_RemoveAP = 101,
        /// <summary>
        ///Reduces damage by #1{~1~2 to }#2
        /// </summary>
        Effect_AddGlobalDamageReduction_105 = 105,
        /// <summary>
        ///Reflects a spell of level #2 maximum
        /// </summary>
        Effect_ReflectSpell = 106,
        /// <summary>
        ///Reflects #1{~1~2 to }#2 damage
        /// </summary>
        Effect_AddDamageReflection = 107,
        /// <summary>
        ///#1{~1~2 to }#2 (HP restored)
        /// </summary>
        Effect_HealHP_108 = 108,
        /// <summary>
        ///#1{~1~2 to }#2 (damage to the caster)
        /// </summary>
        Effect_109 = 109,
        /// <summary>
        ///#1{~1~2 to }#2 HP
        /// </summary>
        Effect_AddHealth = 110,
        /// <summary>
        ///#1{~1~2 to }#2 AP
        /// </summary>
        Effect_AddAP_111 = 111,
        /// <summary>
        ///#1{~1~2 to }#2 Damage
        /// </summary>
        Effect_AddDamageBonus = 112,
        /// <summary>
        ///Restores #1{~1~2 to }#2 HP or doubles damage
        /// </summary>
        Effect_DoubleDamageOrRestoreHP = 113,
        /// <summary>
        ///Multiply damage by #1
        /// </summary>
        Effect_AddDamageMultiplicator = 114,
        /// <summary>
        ///#1{~1~2 to }#2 Critical Hits
        /// </summary>
        Effect_AddCriticalHit = 115,
        /// <summary>
        ///-#1{~1~2 to -}#2 Range
        /// </summary>
        Effect_SubRange = 116,
        /// <summary>
        ///#1{~1~2 to }#2 Range
        /// </summary>
        Effect_AddRange = 117,
        /// <summary>
        ///#1{~1~2 to }#2 Strength
        /// </summary>
        Effect_AddStrength = 118,
        /// <summary>
        ///#1{~1~2 to }#2 Agility
        /// </summary>
        Effect_AddAgility = 119,
        /// <summary>
        ///+#1{~1~2 to }#2 AP
        /// </summary>
        Effect_RegainAP = 120,
        /// <summary>
        ///#1{~1~2 to }#2 Damage
        /// </summary>
        Effect_AddDamageBonus_121 = 121,
        /// <summary>
        ///#1{~1~2 to }#2 Critical Failures
        /// </summary>
        Effect_AddCriticalMiss = 122,
        /// <summary>
        ///#1{~1~2 to }#2 Chance
        /// </summary>
        Effect_AddChance = 123,
        /// <summary>
        ///#1{~1~2 to }#2 Wisdom
        /// </summary>
        Effect_AddWisdom = 124,
        /// <summary>
        ///#1{~1~2 to }#2 Vitality
        /// </summary>
        Effect_AddVitality = 125,
        /// <summary>
        ///#1{~1~2 to }#2 Intelligence
        /// </summary>
        Effect_AddIntelligence = 126,
        /// <summary>
        ///-#1{~1~2 to -}#2 MP
        /// </summary>
        Effect_LostMP = 127,
        /// <summary>
        ///#1{~1~2 to }#2 MP
        /// </summary>
        Effect_AddMP_128 = 128,
        /// <summary>
        ///Seteal Kamas
        /// </summary>
        Effect_StealKamas = 130,
        /// <summary>
        ///Using #1 AP will cause a loss of #2 HP
        /// </summary>
        Effect_LoseHPByUsingAP = 131,
        /// <summary>
        ///Dispels magic effects
        /// </summary>
        Effect_DispelMagicEffects = 132,
        /// <summary>
        ///Caster loses #1{~1~2 to }#2 AP
        /// </summary>
        Effect_LosingAP = 133,
        /// <summary>
        ///Caster loses #1{~1~2 to }#2 MP
        /// </summary>
        Effect_LosingMP = 134,
        /// <summary>
        ///Reduces caster's range by #1{~1~2 to }#2
        /// </summary>
        Effect_SubRange_135 = 135,
        /// <summary>
        ///Increases caster's range by #1{~1~2 to }#2
        /// </summary>
        Effect_AddRange_136 = 136,
        /// <summary>
        ///Increases caster's physical damage by #1{~1~2 to }#2
        /// </summary>
        Effect_AddPhysicalDamage_137 = 137,
        /// <summary>
        ///#1{~1~2 to }#2% Power
        /// </summary>
        Effect_IncreaseDamage_138 = 138,
        /// <summary>
        ///Restores #1{~1~2 to }#2 energy points
        /// </summary>
        Effect_RestoreEnergyPoints = 139,
        /// <summary>
        ///Turn cancelled
        /// </summary>
        Effect_SkipTurn = 140,
        /// <summary>
        ///Kills the target
        /// </summary>
        Effect_Kill = 141,
        /// <summary>
        ///#1{~1~2 to }#2 Physical Damage
        /// </summary>
        Effect_AddPhysicalDamage_142 = 142,
        /// <summary>
        ///#1{~1~2 to }#2 (HP restored)
        /// </summary>
        Effect_HealHP_143 = 143,
        /// <summary>
        ///#1{~1~2 to }#2 (fixed Neutral damage)
        /// </summary>
        Effect_DamageFix = 144,
        /// <summary>
        ///-#1{~1~2 to }#2 Damage
        /// </summary>
        Effect_SubDamageBonus = 145,
        /// <summary>
        ///Changes speech
        /// </summary>
        Effect_ChangesWords = 146,
        /// <summary>
        ///Revives an ally
        /// </summary>
        Effect_ReviveAlly = 147,
        /// <summary>
        ///Someone's following you!
        /// </summary>
        Effect_Followed = 148,
        /// <summary>
        ///Changes appearance
        /// </summary>
        Effect_ChangeAppearance = 149,
        /// <summary>
        ///Makes the character invisible
        /// </summary>
        Effect_Invisibility = 150,
        /// <summary>
        ///-#1{~1~2 to -}#2 Chance
        /// </summary>
        Effect_SubChance = 152,
        /// <summary>
        ///-#1{~1~2 to -}#2 Vitality
        /// </summary>
        Effect_SubVitality = 153,
        /// <summary>
        ///-#1{~1~2 to -}#2 Agility
        /// </summary>
        Effect_SubAgility = 154,
        /// <summary>
        ///-#1{~1~2 to -}#2 Intelligence
        /// </summary>
        Effect_SubIntelligence = 155,
        /// <summary>
        ///-#1{~1~2 to -}#2 Wisdom
        /// </summary>
        Effect_SubWisdom = 156,
        /// <summary>
        ///-#1{~1~2 to -}#2 Strength
        /// </summary>
        Effect_SubStrength = 157,
        /// <summary>
        ///#1{~1~2 to }#2 pods
        /// </summary>
        Effect_IncreaseWeight = 158,
        /// <summary>
        ///-#1{~1~2 to }#2 pods
        /// </summary>
        Effect_DecreaseWeight = 159,
        /// <summary>
        ///#1{~1~2 to }#2 AP Loss Resistance
        /// </summary>
        Effect_AddDodgeAPProbability = 160,
        /// <summary>
        ///#1{~1~2 to }#2 MP Loss Resistance
        /// </summary>
        Effect_AddDodgeMPProbability = 161,
        /// <summary>
        ///-#1{~1~2 to }#2 AP Loss Resistance
        /// </summary>
        Effect_SubDodgeAPProbability = 162,
        /// <summary>
        ///-#1{~1~2 to }#2 MP Loss Resistance
        /// </summary>
        Effect_SubDodgeMPProbability = 163,
        /// <summary>
        ///Reduces damage by #1%
        /// </summary>
        Effect_AddGlobalDamageReduction = 164,
        /// <summary>
        ///#2% #1 Damage
        /// </summary>
        Effect_AddDamageBonusPercent = 165,
        /// <summary>
        ///Returns #1{~1~2 to }#2 AP
        /// </summary>
        Effect_166 = 166,
        /// <summary>
        ///-#1{~1~2 to }#2 AP
        /// </summary>
        Effect_SubAP = 168,
        /// <summary>
        ///-#1{~1~2 to }#2 MP
        /// </summary>
        Effect_SubMP = 169,
        /// <summary>
        ///-#1{~1~2 to }#2 Critical Hits
        /// </summary>
        Effect_SubCriticalHit = 171,
        /// <summary>
        ///-#1{~1~2 to }#2 Magic Reduction
        /// </summary>
        Effect_SubMagicDamageReduction = 172,
        /// <summary>
        ///-#1{~1~2 to }#2 Physical Reduction
        /// </summary>
        Effect_SubPhysicalDamageReduction = 173,
        /// <summary>
        ///#1{~1~2 to }#2 Initiative
        /// </summary>
        Effect_AddInitiative = 174,
        /// <summary>
        ///-#1{~1~2 to }#2 Initiative
        /// </summary>
        Effect_SubInitiative = 175,
        /// <summary>
        ///#1{~1~2 to }#2 Prospecting
        /// </summary>
        Effect_AddProspecting = 176,
        /// <summary>
        ///-#1{~1~2 to }#2 Prospecting
        /// </summary>
        Effect_SubProspecting = 177,
        /// <summary>
        ///#1{~1~2 to }#2 Heals
        /// </summary>
        Effect_AddHealBonus = 178,
        /// <summary>
        ///-#1{~1~2 to }#2 Heals
        /// </summary>
        Effect_SubHealBonus = 179,
        /// <summary>
        ///Creates a double of the caster
        /// </summary>
        Effect_Double = 180,
        /// <summary>
        ///Summons #1
        /// </summary>
        Effect_Summon = 181,
        /// <summary>
        ///#1{~1~2 to }#2 Summons
        /// </summary>
        Effect_AddSummonLimit = 182,
        /// <summary>
        ///#1{~1~2 to }#2 Magic Reduction
        /// </summary>
        Effect_AddMagicDamageReduction = 183,
        /// <summary>
        ///#1{~1~2 to }#2 Physical Reduction
        /// </summary>
        Effect_AddPhysicalDamageReduction = 184,
        /// <summary>
        ///Summons: #1 (static)
        /// </summary>
        Effect_185 = 185,
        /// <summary>
        ///-#1{~1~2 to }#2 Power
        /// </summary>
        Effect_SubDamageBonusPercent = 186,
        /// <summary>
        ///Switches alignment
        /// </summary>
        Effect_188 = 188,
        /// <summary>
        ///
        /// </summary>
        Effect_192 = 192,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_193 = 193,
        /// <summary>
        ///#1{~1~2 to }#2 kama gain
        /// </summary>
        Effect_GiveKamas = 194,
        /// <summary>
        ///Transform into #1
        /// </summary>
        Effect_197 = 197,
        /// <summary>
        ///Put an item on the ground
        /// </summary>
        Effect_201 = 201,
        /// <summary>
        ///Reveals all invisible items
        /// </summary>
        Effect_RevealsInvisible = 202,
        /// <summary>
        ///Revive the target
        /// </summary>
        Effect_206 = 206,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_AddEarthResistPercent = 210,
        /// <summary>
        ///#1{~1~2 to }#2% Water Resistance
        /// </summary>
        Effect_AddWaterResistPercent = 211,
        /// <summary>
        ///#1{~1~2 to }#2% Air Resistance
        /// </summary>
        Effect_AddAirResistPercent = 212,
        /// <summary>
        ///#1{~1~2 to }#2% Fire Resistance
        /// </summary>
        Effect_AddFireResistPercent = 213,
        /// <summary>
        ///#1{~1~2 to }#2% Neutral Resistance
        /// </summary>
        Effect_AddNeutralResistPercent = 214,
        /// <summary>
        ///-#1{~1~2 to }#2% Earth Resistance
        /// </summary>
        Effect_SubEarthResistPercent = 215,
        /// <summary>
        ///-#1{~1~2 to }#2% Water Resistance
        /// </summary>
        Effect_SubWaterResistPercent = 216,
        /// <summary>
        ///-#1{~1~2 to }#2% Air Resistance
        /// </summary>
        Effect_SubAirResistPercent = 217,
        /// <summary>
        ///-#1{~1~2 to }#2% Fire Resistance
        /// </summary>
        Effect_SubFireResistPercent = 218,
        /// <summary>
        ///-#1{~1~2 to }#2% Neutral Resistance
        /// </summary>
        Effect_SubNeutralResistPercent = 219,
        /// <summary>
        ///Reflects #1{~1~2 to }#2 damage
        /// </summary>
        Effect_ForwardDamade = 220,
        /// <summary>
        ///What's in there?
        /// </summary>
        Effect_WhatsInside = 221,
        /// <summary>
        ///What's in there?
        /// </summary>
        Effect_WhatsInside2 = 222,
        /// <summary>
        ///#1{~1~2 to }#2 Trap Damage
        /// </summary>
        Effect_AddTrapBonus = 225,
        /// <summary>
        ///#1{~1~2 to }#2 Power (traps)
        /// </summary>
        Effect_AddTrapGeneralBonusPercent = 226,
        /// <summary>
        ///Get a mount!
        /// </summary>
        Effect_RetriveMount = 229,
        /// <summary>
        ///#1 Energy lost
        /// </summary>
        Effect_LoseEnergy = 230,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_239 = 239,
        /// <summary>
        ///#1{~1~2 to }#2 Earth Resistance
        /// </summary>
        Effect_AddEarthElementReduction = 240,
        /// <summary>
        ///#1{~1~2 to }#2 Water Resistance
        /// </summary>
        Effect_AddWaterElementReduction = 241,
        /// <summary>
        ///#1{~1~2 to }#2 Air Resistance
        /// </summary>
        Effect_AddAirElementReduction = 242,
        /// <summary>
        ///#1{~1~2 to }#2 Fire Resistance
        /// </summary>
        Effect_AddFireElementReduction = 243,
        /// <summary>
        ///#1{~1~2 to }#2 Neutral Resistance
        /// </summary>
        Effect_AddNeutralElementReduction = 244,
        /// <summary>
        ///-#1{~1~2 to }#2 Earth Resistance
        /// </summary>
        Effect_SubEarthElementReduction = 245,
        /// <summary>
        ///-#1{~1~2 to }#2 Water Resistance
        /// </summary>
        Effect_SubWaterElementReduction = 246,
        /// <summary>
        ///-#1{~1~2 to }#2 Air Resistance
        /// </summary>
        Effect_SubAirElementReduction = 247,
        /// <summary>
        ///-#1{~1~2 to }#2 Fire Resistance
        /// </summary>
        Effect_SubFireElementReduction = 248,
        /// <summary>
        ///-#1{~1~2 to }#2 Neutral Resistance
        /// </summary>
        Effect_SubNeutralElementReduction = 249,
        /// <summary>
        ///#1{~1~2 to }#2% Earth Resistance in PvP
        /// </summary>
        Effect_AddPvpEarthResistPercent = 250,
        /// <summary>
        ///#1{~1~2 to }#2% Water Resistance in PvP
        /// </summary>
        Effect_AddPvpWaterResistPercent = 251,
        /// <summary>
        ///#1{~1~2 to }#2% Air Resistance in PvP
        /// </summary>
        Effect_AddPvpAirResistPercent = 252,
        /// <summary>
        ///#1{~1~2 to }#2% Fire Resistance in PvP
        /// </summary>
        Effect_AddPvpFireResistPercent = 253,
        /// <summary>
        ///#1{~1~2 to }#2% Neutral Resistance in PvP
        /// </summary>
        Effect_AddPvpNeutralResistPercent = 254,
        /// <summary>
        ///-#1{~1~2 to }#2% Earth Resistance in PvP
        /// </summary>
        Effect_SubPvpEarthResistPercent = 255,
        /// <summary>
        ///-#1{~1~2 to }#2% Water Resistance in PvP
        /// </summary>
        Effect_SubPvpWaterResistPercent = 256,
        /// <summary>
        ///-#1{~1~2 to }#2% Air Resistance in PvP
        /// </summary>
        Effect_SubPvpAirResistPercent = 257,
        /// <summary>
        ///-#1{~1~2 to }#2% Fire Resistance in PvP
        /// </summary>
        Effect_SubPvpFireResistPercent = 258,
        /// <summary>
        ///-#1{~1~2 to }#2% Neutral Resistance in PvP
        /// </summary>
        Effect_SubPvpNeutralResistPercent = 259,
        /// <summary>
        ///#1{~1~2 to }#2 Earth Resistance in PvP
        /// </summary>
        Effect_AddPvpEarthElementReduction = 260,
        /// <summary>
        ///#1{~1~2 to }#2 Water Resistance in PvP
        /// </summary>
        Effect_AddPvpWaterElementReduction = 261,
        /// <summary>
        ///#1{~1~2 to }#2 Air Resistance in PvP
        /// </summary>
        Effect_AddPvpAirElementReduction = 262,
        /// <summary>
        ///#1{~1~2 to }#2 Fire Resistance in PvP
        /// </summary>
        Effect_AddPvpFireElementReduction = 263,
        /// <summary>
        ///#1{~1~2 to }#2 Neutral Resistance in PvP
        /// </summary>
        Effect_AddPvpNeutralElementReduction = 264,
        /// <summary>
        ///Reduces damage by #1{~1~2 to }#2
        /// </summary>
        Effect_AddArmorDamageReduction = 265,
        /// <summary>
        ///Steals #1{~1~2 to -}#2 Chance
        /// </summary>
        Effect_StealChance = 266,
        /// <summary>
        ///Steals #1{~1~2 to -}#2 Vitality
        /// </summary>
        Effect_StealVitality = 267,
        /// <summary>
        ///Steals #1{~1~2 to -}#2 Agility
        /// </summary>
        Effect_StealAgility = 268,
        /// <summary>
        ///Steals #1{~1~2 to -}#2 Intelligence
        /// </summary>
        Effect_StealIntelligence = 269,
        /// <summary>
        ///Steals #1{~1~2 to -}#2 Wisdom
        /// </summary>
        Effect_StealWisdom = 270,
        /// <summary>
        ///Steals #1{~1~2 to -}#2 Strength
        /// </summary>
        Effect_StealStrength = 271,
        /// <summary>
        ///#1{~1~2 to }#2% of attacker's missing HP (Water damage)
        /// </summary>
        Effect_275 = 275,
        /// <summary>
        ///#1{~1~2 to }#2% of attacker's missing HP (Earth damage)
        /// </summary>
        Effect_276 = 276,
        /// <summary>
        ///#1{~1~2 to }#2% of attacker's missing HP (Air damage)
        /// </summary>
        Effect_277 = 277,
        /// <summary>
        ///#1{~1~2 to }#2% of attacker's missing HP (Fire damage)
        /// </summary>
        Effect_278 = 278,
        /// <summary>
        ///#1{~1~2 to }#2% of attacker's missing HP (Neutral damage)
        /// </summary>
        Effect_279 = 279,
        /// <summary>
        ///Increases range of #1 by #3
        /// </summary>
        Effect_IncreaseALOfSpell = 281,
        /// <summary>
        ///#1's range becomes modifiable
        /// </summary>
        Effect_AlSpellModifiable = 282,
        /// <summary>
        ///#3 extra Damage to #1
        /// </summary>
        Effect_AddSpellDamage = 283,
        /// <summary>
        ///#3 extra Heals to #1
        /// </summary>
        Effect_AddSpellHealt = 284,
        /// <summary>
        ///Reduces #1's AP cost by #3
        /// </summary>
        Effect_ReducePACost = 285,
        /// <summary>
        ///Reduces #1's cooldown period by #3
        /// </summary>
        Effect_WaitSpellTurn = 286,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_PercentGC = 287,
        /// <summary>
        ///#1 is no longer linear
        /// </summary>
        Effect_288 = 288,
        /// <summary>
        ///#1 no longer requires a line of sight
        /// </summary>
        Effect_289 = 289,
        /// <summary>
        ///Increases the maximum number of times #1 can be cast per turn by #3
        /// </summary>
        Effect_290 = 290,
        /// <summary>
        ///Increases the maximum number of times #1 can be cast per target by #3
        /// </summary>
        Effect_291 = 291,
        /// <summary>
        ///Sets #1's cooldown period to #3
        /// </summary>
        Effect_292 = 292,
        /// <summary>
        ///Increases #1's basic damage by #3
        /// </summary>
        Effect_SpellBoost = 293,
        /// <summary>
        ///Reduces #1's range by #3
        /// </summary>
        Effect_294 = 294,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_310 = 310,
        /// <summary>
        ///Steals #1{~1~2 to }#2 range
        /// </summary>
        Effect_StealRange = 320,
        /// <summary>
        ///Change a colour
        /// </summary>
        Effect_333 = 333,
        /// <summary>
        ///Change appearance
        /// </summary>
        Effect_ChangeAppearance_335 = 335,
        /// <summary>
        ///
        /// </summary>
        Effect_350 = 350,
        /// <summary>
        ///
        /// </summary>
        Effect_351 = 351,
        /// <summary>
        ///Sets a grade #2 trap
        /// </summary>
        Effect_Trap = 400,
        /// <summary>
        ///Places a grade #2 glyph
        /// </summary>
        Effect_Glyph = 401,
        /// <summary>
        ///Places a grade #2 glyph
        /// </summary>
        Effect_Glyph_402 = 402,
        /// <summary>
        ///Kills and replaces with a summon
        /// </summary>
        Effect_KillAndReplace = 405,
        /// <summary>
        ///Removes the effects of #2
        /// </summary>
        Effect_RemoveEffects = 406,
        /// <summary>
        ///#1{~1~2 to }#2 (HP restored)
        /// </summary>
        Effect_407 = 407,
        /// <summary>
        ///#1{~1~2 to }#2 AP Reduction
        /// </summary>
        Effect_AddAPAttack = 410,
        /// <summary>
        ///-#1{~1~2 to }#2 AP Reduction
        /// </summary>
        Effect_SubAPAttack = 411,
        /// <summary>
        ///#1{~1~2 to }#2 MP Reduction
        /// </summary>
        Effect_AddMPAttack = 412,
        /// <summary>
        ///-#1{~1~2 to }#2 MP Reduction
        /// </summary>
        Effect_SubMPAttack = 413,
        /// <summary>
        ///#1{~1~2 to }#2 Pushback Damage
        /// </summary>
        Effect_AddPushDamageBonus = 414,
        /// <summary>
        ///-#1{~1~2 to }#2 Pushback Damage
        /// </summary>
        Effect_SubPushDamageBonus = 415,
        /// <summary>
        ///#1{~1~2 to }#2 Pushback Resistance
        /// </summary>
        Effect_AddPushDamageReduction = 416,
        /// <summary>
        ///-#1{~1~2 to }#2 Pushback Resistance
        /// </summary>
        Effect_SubPushDamageReduction = 417,
        /// <summary>
        ///#1{~1~2 to }#2 Critical Damage
        /// </summary>
        Effect_AddCriticalDamageBonus = 418,
        /// <summary>
        ///-#1{~1~2 to }#2 Critical Damage
        /// </summary>
        Effect_SubCriticalDamageBonus = 419,
        /// <summary>
        ///#1{~1~2 to }#2 Critical Resistance
        /// </summary>
        Effect_AddCriticalDamageReduction = 420,
        /// <summary>
        ///-#1{~1~2 to }#2 Critical Resistance
        /// </summary>
        Effect_SubCriticalDamageReduction = 421,
        /// <summary>
        ///#1{~1~2 to }#2 Earth Damage
        /// </summary>
        Effect_AddEarthDamageBonus = 422,
        /// <summary>
        ///-#1{~1~2 to }#2 Earth Damage
        /// </summary>
        Effect_SubEarthDamageBonus = 423,
        /// <summary>
        ///#1{~1~2 to }#2 Fire Damage
        /// </summary>
        Effect_AddFireDamageBonus = 424,
        /// <summary>
        ///-#1{~1~2 to }#2 Fire Damage
        /// </summary>
        Effect_SubFireDamageBonus = 425,
        /// <summary>
        ///#1{~1~2 to }#2 Water Damage
        /// </summary>
        Effect_AddWaterDamageBonus = 426,
        /// <summary>
        ///-#1{~1~2 to }#2 Water Damage
        /// </summary>
        Effect_SubWaterDamageBonus = 427,
        /// <summary>
        ///#1{~1~2 to }#2 Air Damage
        /// </summary>
        Effect_AddAirDamageBonus = 428,
        /// <summary>
        ///-#1{~1~2 to }#2 Air Damage
        /// </summary>
        Effect_SubAirDamageBonus = 429,
        /// <summary>
        ///#1{~1~2 to }#2 Neutral Damage
        /// </summary>
        Effect_AddNeutralDamageBonus = 430,
        /// <summary>
        ///-#1{~1~2 to }#2 Neutral Damage
        /// </summary>
        Effect_SubNeutralDamageBonus = 431,
        /// <summary>
        ///Steals #1{~1~2 to }#2 AP
        /// </summary>
        Effect_StealAP_440 = 440,
        /// <summary>
        ///Steals #1{~1~2 to }#2 MP
        /// </summary>
        Effect_StealMP_441 = 441,
        /// <summary>
        ///Positions the compass
        /// </summary>
        Effect_509 = 509,
        /// <summary>
        ///Place a prism
        /// </summary>
        Effect_Summon_Prism = 513,
        /// <summary>
        ///[!] Afficher les percepteurs les plus riches
        /// </summary>
        Effect_516 = 516,
        /// <summary>
        ///
        /// </summary>
        Effect_517 = 517,
        /// <summary>
        ///Teleport to save point
        /// </summary>
        Effect_Teleport_SavePoint = 600,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_601 = 601,
        /// <summary>
        ///Saves your position
        /// </summary>
        Effect_602 = 602,
        /// <summary>
        ///Learn the #3 profession
        /// </summary>
        Effect_603 = 603,
        /// <summary>
        ///Teaches the spell #3
        /// </summary>
        Effect_LearnSpell = 604,
        /// <summary>
        ///+#1{~1~2 to }#2 XP
        /// </summary>
        Effect_605 = 605,
        /// <summary>
        ///+#1{~1~2 to }#2 Wisdom
        /// </summary>
        Effect_AddPermanentWisdom = 606,
        /// <summary>
        ///+#1{~1~2 to }#2 Strength
        /// </summary>
        Effect_AddPermanentStrength = 607,
        /// <summary>
        ///+#1{~1~2 to }#2 Chance
        /// </summary>
        Effect_AddPermanentChance = 608,
        /// <summary>
        ///+#1{~1~2 to }#2 Agility
        /// </summary>
        Effect_AddPermanentAgility = 609,
        /// <summary>
        ///+#1{~1~2 to }#2 Vitality
        /// </summary>
        Effect_AddPermanentVitality = 610,
        /// <summary>
        ///+#1{~1~2 to }#2 Intelligence
        /// </summary>
        Effect_AddPermanentIntelligence = 611,
        /// <summary>
        ///+#1{~1~2 to }#2 characteristic points
        /// </summary>
        Effect_612 = 612,
        /// <summary>
        ///+#1{~1~2 to }#2 spell point(s)
        /// </summary>
        Effect_AddSpellPoints = 613,
        /// <summary>
        ///+#1 #2 XP
        /// </summary>
        Effect_614 = 614,
        /// <summary>
        ///Forget the #3 profession
        /// </summary>
        Effect_615 = 615,
        /// <summary>
        ///Forget one level of the spell #3
        /// </summary>
        Effect_616 = 616,
        /// <summary>
        ///Consult #3
        /// </summary>
        Effect_620 = 620,
        /// <summary>
        ///Summons a level #1 #3
        /// </summary>
        Effect_621 = 621,
        /// <summary>
        ///Teleport to your house
        /// </summary>
        Effect_622 = 622,
        /// <summary>
        ///#3 (#2)
        /// </summary>
        Effect_623 = 623,
        /// <summary>
        ///Forget one level of the spell #3
        /// </summary>
        Effect_624 = 624,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_625 = 625,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_626 = 626,
        /// <summary>
        ///Recreates original map
        /// </summary>
        Effect_627 = 627,
        /// <summary>
        ///Summons #3
        /// </summary>
        Effect_628 = 628,
        /// <summary>
        ///
        /// </summary>
        Effect_630 = 630,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_631 = 631,
        /// <summary>
        ///
        /// </summary>
        Effect_632 = 632,
        /// <summary>
        ///Adds #3 Honour points
        /// </summary>
        Effect_640 = 640,
        /// <summary>
        ///Adds #3 Disgrace points
        /// </summary>
        Effect_641 = 641,
        /// <summary>
        ///Removes #3 Honour points
        /// </summary>
        Effect_642 = 642,
        /// <summary>
        ///Removes #3 Disgrace points
        /// </summary>
        Effect_643 = 643,
        /// <summary>
        ///Resuscitates allies on your map
        /// </summary>
        Effect_645 = 645,
        /// <summary>
        ///#1{~1~2 to }#2 (HP restored)
        /// </summary>
        Effect_646 = 646,
        /// <summary>
        ///Frees enemy souls
        /// </summary>
        Effect_647 = 647,
        /// <summary>
        ///Frees an enemy soul
        /// </summary>
        Effect_648 = 648,
        /// <summary>
        ///Pretend to be #3
        /// </summary>
        Effect_649 = 649,
        /// <summary>
        ///
        /// </summary>
        Effect_652 = 652,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_654 = 654,
        /// <summary>
        ///No additional effects
        /// </summary>
        Effect_666 = 666,
        /// <summary>
        ///Level #5 Incarnation
        /// </summary>
        Effect_669 = 669,
        /// <summary>
        ///#1{~1~2 to }#2% of attacker's HP (Neutral damage)
        /// </summary>
        Effect_670 = 670,
        /// <summary>
        ///#1{~1~2 to }#2% of attacker's HP (fixed Neutral damage)
        /// </summary>
        Effect_671 = 671,
        /// <summary>
        ///#1{~1~2 to }#2% of attacker's HP (Neutral damage)
        /// </summary>
        Effect_Punishment_Damage = 672,
        /// <summary>
        ///Links the #1 profession
        /// </summary>
        Effect_699 = 699,
        /// <summary>
        ///Change the attack element
        /// </summary>
        Effect_700 = 700,
        /// <summary>
        ///Power: #1{~1~2 to }#2
        /// </summary>
        Effect_701 = 701,
        /// <summary>
        ///+#1{~1~2 to }#2 Durability Point(s)
        /// </summary>
        Effect_702 = 702,
        /// <summary>
        ///#1% chance of capturing a power #3 soul
        /// </summary>
        Effect_705 = 705,
        /// <summary>
        ///#1% chance of capturing a mount
        /// </summary>
        Effect_706 = 706,
        /// <summary>
        ///Use custom set n°#3
        /// </summary>
        Effect_707 = 707,
        /// <summary>
        ///Additional cost
        /// </summary>
        Effect_710 = 710,
        /// <summary>
        ///#1: #3
        /// </summary>
        Effect_715 = 715,
        /// <summary>
        ///#1: #3
        /// </summary>
        Effect_716 = 716,
        /// <summary>
        ///#1: #3
        /// </summary>
        Effect_717 = 717,
        /// <summary>
        ///Number of victims: #2
        /// </summary>
        Effect_720 = 720,
        /// <summary>
        ///Title: #3
        /// </summary>
        Effect_724 = 724,
        /// <summary>
        ///Rename guild: #4
        /// </summary>
        Effect_725 = 725,
        /// <summary>
        ///Ornament: #3
        /// </summary>
        Effect_726 = 726,
        /// <summary>
        ///Teleport to the nearest allied prism
        /// </summary>
        Effect_730 = 730,
        /// <summary>
        ///Automatically attack characters from enemy alliances
        /// </summary>
        Effect_731 = 731,
        /// <summary>
        ///Resistance to automatic attacks from enemy players: #1{~1~2 to }#2
        /// </summary>
        Effect_732 = 732,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_740 = 740,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_741 = 741,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_742 = 742,
        /// <summary>
        ///Increases chance of capture by #1{~1~2 to }#2%
        /// </summary>
        Effect_750 = 750,
        /// <summary>
        ///#1{~1~2 to }#2% Mount XP Bonus
        /// </summary>
        Effect_MountXpBonus = 751,
        /// <summary>
        ///#1{~1~2 to }#2 Dodge
        /// </summary>
        Effect_AddDodge = 752,
        /// <summary>
        ///#1{~1~2 to }#2 Lock
        /// </summary>
        Effect_AddLock = 753,
        /// <summary>
        ///-#1{~1~2 to }#2 Dodge
        /// </summary>
        Effect_SubDodge = 754,
        /// <summary>
        ///-#1{~1~2 to }#2 Lock
        /// </summary>
        Effect_SubLock = 755,
        /// <summary>
        ///Effect will disappear when you move
        /// </summary>
        Effect_760 = 760,
        /// <summary>
        ///Damage interception
        /// </summary>
        Effect_Sacrifice = 765,
        /// <summary>
        ///Clockwise confusion: #1{~1~2 to }#2 degrees
        /// </summary>
        Effect_770 = 770,
        /// <summary>
        ///Clockwise confusion: #1{~1~2 to }#2 Pi/2
        /// </summary>
        Effect_771 = 771,
        /// <summary>
        ///Clockwise confusion: #1{~1~2 to }#2 Pi/4
        /// </summary>
        Effect_772 = 772,
        /// <summary>
        ///Anticlockwise confusion: #1{~1~2 to }#2 degrees
        /// </summary>
        Effect_773 = 773,
        /// <summary>
        ///Anticlockwise confusion: #1{~1~2 to }#2 Pi/2
        /// </summary>
        Effect_774 = 774,
        /// <summary>
        ///Anticlockwise confusion: #1{~1~2 to }#2 Pi/4
        /// </summary>
        Effect_775 = 775,
        /// <summary>
        ///#1{~1~2 to }#2% Erosion
        /// </summary>
        Effect_AddErosion = 776,
        /// <summary>
        ///Set respawn point
        /// </summary>
        Effect_778 = 778,
        /// <summary>
        ///Summons the last ally to die at #1{~1~2 to }#2% HP
        /// </summary>
        Effect_780 = 780,
        /// <summary>
        ///Spell and weapon effects minimised
        /// </summary>
        Effect_781 = 781,
        /// <summary>
        ///Spell and weapon effects maximised
        /// </summary>
        Effect_782 = 782,
        /// <summary>
        ///Repels to the targeted cell
        /// </summary>
        Effect_RepelsTo = 783,
        /// <summary>
        ///Return to original position
        /// </summary>
        Effect_Rollback = 784,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_785 = 785,
        /// <summary>
        ///Heals upon attack
        /// </summary>
        Effect_786 = 786,
        /// <summary>
        ///#1
        /// </summary>
        Effect_787 = 787,
        /// <summary>
        ///Punishment of #2 for #3 turn(s)
        /// </summary>
        Effect_Punishment = 788,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_789 = 789,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_790 = 790,
        /// <summary>
        ///Prepare #1{~1~2 to }#2 mercenary scrolls
        /// </summary>
        Effect_791 = 791,
        /// <summary>
        ///#1
        /// </summary>
        Effect_792 = 792,
        /// <summary>
        ///#1
        /// </summary>
        Effect_InflictDamagePercentInArea = 793,
        /// <summary>
        ///Hunting weapon
        /// </summary>
        Effect_Rewind = 795,
        /// <summary>
        ///Restore respawn point
        /// </summary>
        Effect_796 = 796,
        /// <summary>
        ///Health points - #3
        /// </summary>
        Effect_PointsOfLife_3 = 800,
        /// <summary>
        ///Received on #1
        /// </summary>
        Effect_Received = 805,
        /// <summary>
        ///State - #1
        /// </summary>
        Effect_Corpulence = 806,
        /// <summary>
        ///Last meal - #1
        /// </summary>
        Effect_LastMeal = 807,
        /// <summary>
        ///Last fed - #1
        /// </summary>
        Effect_LastFed = 808,
        /// <summary>
        ///Size: #3 cells
        /// </summary>
        Effect_810 = 810,
        /// <summary>
        ///#3 remaining fight(s)
        /// </summary>
        Effect_RemainingFights = 811,
        /// <summary>
        ///Remaining fights: #2/#3
        /// </summary>
        Effect_812 = 812,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_813 = 813,
        /// <summary>
        ///#1
        /// </summary>
        Effect_814 = 814,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_815 = 815,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_816 = 816,
        /// <summary>
        ///Teleport
        /// </summary>
        Effect_825 = 825,
        /// <summary>
        ///Teleport
        /// </summary>
        Effect_826 = 826,
        /// <summary>
        ///Forget a spell
        /// </summary>
        Effect_831 = 831,
        /// <summary>
        ///Starts a fight against #2
        /// </summary>
        Effect_905 = 905,
        /// <summary>
        ///
        /// </summary>
        Effect_911 = 911,
        /// <summary>
        ///
        /// </summary>
        Effect_916 = 916,
        /// <summary>
        ///
        /// </summary>
        Effect_917 = 917,
        /// <summary>
        ///Increases serenity, decreases aggressiveness
        /// </summary>
        Effect_930 = 930,
        /// <summary>
        ///Improves aggressiveness, decreases serenity
        /// </summary>
        Effect_931 = 931,
        /// <summary>
        ///Increases stamina
        /// </summary>
        Effect_932 = 932,
        /// <summary>
        ///Decreases stamina
        /// </summary>
        Effect_933 = 933,
        /// <summary>
        ///Increases love
        /// </summary>
        Effect_934 = 934,
        /// <summary>
        ///Decreases love
        /// </summary>
        Effect_935 = 935,
        /// <summary>
        ///Speeds up maturity
        /// </summary>
        Effect_936 = 936,
        /// <summary>
        ///Slows down maturity
        /// </summary>
        Effect_937 = 937,
        /// <summary>
        ///Increases the capacity of a #3
        /// </summary>
        Effect_939 = 939,
        /// <summary>
        ///Improved abilities
        /// </summary>
        Effect_940 = 940,
        /// <summary>
        ///Temporarily remove a Breeding item
        /// </summary>
        Effect_946 = 946,
        /// <summary>
        ///Remove an item from a Paddock
        /// </summary>
        Effect_947 = 947,
        /// <summary>
        ///Paddock Item
        /// </summary>
        Effect_948 = 948,
        /// <summary>
        ///Get on/off a mount
        /// </summary>
        Effect_949 = 949,
        /// <summary>
        ///#3 state
        /// </summary>
        Effect_AddState = 950,
        /// <summary>
        ///Removes the #3 state
        /// </summary>
        Effect_SubState = 951,
        /// <summary>
        ///#3 state deactivated
        /// </summary>
        Effect_952 = 952,
        /// <summary>
        ///Alignment = #3
        /// </summary>
        Effect_960 = 960,
        /// <summary>
        ///Rank #3
        /// </summary>
        Effect_961 = 961,
        /// <summary>
        ///Level #3
        /// </summary>
        Effect_962 = 962,
        /// <summary>
        ///Created #3 day(s) ago
        /// </summary>
        Effect_963 = 963,
        /// <summary>
        ///Name: #4
        /// </summary>
        Effect_964 = 964,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_LivingObjectId = 970,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_LivingObjectMood = 971,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_LivingObjectSkin = 972,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_LivingObjectCategory = 973,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_LivingObjectLevel = 974,
        /// <summary>
        ///Linked to the character
        /// </summary>
        Effect_NonExchangeable_981 = 981,
        /// <summary>
        ///Linked to the account
        /// </summary>
        Effect_NonExchangeable_982 = 982,
        /// <summary>
        ///Exchangeable: #1
        /// </summary>
        Effect_983 = 983,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_984 = 984,
        /// <summary>
        ///Maged by #4
        /// </summary>
        Effect_985 = 985,
        /// <summary>
        ///Prepares #1{~1~2 to }#2 scrolls
        /// </summary>
        Effect_986 = 986,
        /// <summary>
        ///Belongs to #4
        /// </summary>
        Effect_987 = 987,
        /// <summary>
        ///Crafted by #4
        /// </summary>
        Effect_988 = 988,
        /// <summary>
        ///Seeks #4
        /// </summary>
        Effect_989 = 989,
        /// <summary>
        ///#4
        /// </summary>
        Effect_990 = 990,
        /// <summary>
        ///!! Invalid Certificate !!
        /// </summary>
        Effect_994 = 994,
        /// <summary>
        ///View mount characteristics
        /// </summary>
        Effect_995 = 995,
        /// <summary>
        ///Belongs to #4
        /// </summary>
        Effect_996 = 996,
        /// <summary>
        ///Name: #4
        /// </summary>
        Effect_997 = 997,
        /// <summary>
        ///Validity: #1d #2h #3m
        /// </summary>
        Effect_998 = 998,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_999 = 999,
        /// <summary>
        ///2
        /// </summary>
        Effect_1002 = 1002,
        /// <summary>
        ///Reduces the maximum bonus by #1{~1~2 to }#2
        /// </summary>
        Effect_1003 = 1003,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_1004 = 1004,
        /// <summary>
        ///Reduces the maximum bonus by #1{~1~2 to }#2
        /// </summary>
        Effect_1005 = 1005,
        /// <summary>
        ///Reduces the minimum bonus by #1{~1~2 to }#2
        /// </summary>
        Effect_1006 = 1006,
        /// <summary>
        ///Effectiveness: #1{~1~2 to }#2
        /// </summary>
        Effect_1007 = 1007,
        /// <summary>
        ///Summons #1
        /// </summary>
        Effect_SummonBomb = 1008,
        /// <summary>
        ///Activate a bomb
        /// </summary>
        Effect_ActivateBomb = 1009,
        /// <summary>
        ///Places a grade #2 glyph
        /// </summary>
        Effect_1010 = 1010,
        /// <summary>
        ///Summons #1
        /// </summary>
        Effect_1011 = 1011,
        /// <summary>
        ///#1{~1~2 to }#2 (Neutral damage)
        /// </summary>
        Effect_1012 = 1012, //TODO 
        /// <summary>
        ///#1{~1~2 to }#2 (Air damage)
        /// </summary>
        Effect_1013 = 1013,
        /// <summary>
        ///#1{~1~2 to }#2 (Water damage)
        /// </summary>
        Effect_1014 = 1014,
        /// <summary>
        ///#1{~1~2 to }#2 (Fire damage)
        /// </summary>
        Effect_1015 = 1015,
        /// <summary>
        ///#1{~1~2 to }#2 (Earth damage)
        /// </summary>
        Effect_1016 = 1016,
        /// <summary>
        ///#1
        /// </summary>
        Effect_1017 = 1017,
        /// <summary>
        ///#1
        /// </summary>
        Effect_1018 = 1018,
        /// <summary>
        ///#1
        /// </summary>
        Effect_1019 = 1019,
        /// <summary>
        ///Pushes back #1 cell(s)
        /// </summary>
        Effect_1021 = 1021,
        /// <summary>
        ///Attracts #1 cell(s)
        /// </summary>
        Effect_1022 = 1022,
        /// <summary>
        ///Switches positions
        /// </summary>
        Effect_1023 = 1023,
        /// <summary>
        ///Creates illusions
        /// </summary>
        Effect_1024 = 1024,
        /// <summary>
        ///Trigger traps
        /// </summary>
        Effect_1025 = 1025,
        /// <summary>
        ///Trigger glyphs
        /// </summary>
        Effect_TriggerGlyphs = 1026,
        /// <summary>
        ///#1{~1~2 to }#2% Combo Damage
        /// </summary>
        Effect_AddComboDamage = 1027,
        /// <summary>
        ///Trigger powders
        /// </summary>
        Effect_1028 = 1028,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_1029 = 1029,
        /// <summary>
        ///Place grade #2 powder
        /// </summary>
        Effect_1030 = 1030,
        /// <summary>
        ///Ends turn
        /// </summary>
        Effect_EndTurn = 1031,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_1032 = 1032,
        /// <summary>
        ///-#1{~1~2 to -}#2% Vitality
        /// </summary>
        Effect_SubHealPercent_1033 = 1033,
        /// <summary>
        ///Summons the last ally killed with #1{~1~2 to }#2% of their HP
        /// </summary>
        Effect_1034 = 1034,
        /// <summary>
        ///#1: +#3 turns until recast
        /// </summary>
        Effect_1035 = 1035,
        /// <summary>
        ///#1: -#3 turns until recast
        /// </summary>
        Effect_1036 = 1036,
        /// <summary>
        ///HP restored: #1 {~1~2 to}#2
        /// </summary>
        Effect_1037 = 1037,
        /// <summary>
        ///Aura: #1
        /// </summary>
        Effect_1038 = 1038,
        /// <summary>
        ///#1{~1~2 to }#2% of HP to shield
        /// </summary>
        Effect_AddPercentShield = 1039,
        /// <summary>
        ///#1{~1~2 to }#2 Shield
        /// </summary>
        Effect_Shield = 1040,
        /// <summary>
        ///Retreats #1 cell(s)
        /// </summary>
        Effect_RetreatCell = 1041,
        /// <summary>
        ///Advances #1 cell(s)
        /// </summary>
        Effect_AdvanceCell = 1042,
        /// <summary>
        ///Attract to the selected cell
        /// </summary>
        Effect_1043 = 1043,
        /// <summary>
        ///Immunity: #1
        /// </summary>
        Effect_1044 = 1044,
        /// <summary>
        ///#1: #3 turns until recast
        /// </summary>
        Effect_EditTurn = 1045,
        /// <summary>
        ///Using #1 MP will cause a loss of #2 HP
        /// </summary>
        Effect_1046 = 1046,
        /// <summary>
        ///-#1{~1~2 to }#2 HP
        /// </summary>
        Effect_1047 = 1047,
        /// <summary>
        ///-#1{~1~2 to }#2% HP
        /// </summary>
        Effect_SubHealPercent = 1048,
        /// <summary>
        ///+#1{~1~2 to}level #2
        /// </summary>
        Effect_1049 = 1049,
        /// <summary>
        ///+ #1 level in the #2 profession
        /// </summary>
        Effect_1050 = 1050,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_1051 = 1051,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_1052 = 1052,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_1053 = 1053,
        /// <summary>
        ///#1{~1~2 to }#2 Power (spells)
        /// </summary>
        Effect_IncreaseDamage_1054 = 1054,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_1055 = 1055,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_1057 = 1057,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_1058 = 1058,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_1059 = 1059,
        /// <summary>
        ///[!] Augmente la taille.
        /// </summary>
        Effect_1060 = 1060,
        /// <summary>
        ///Damage sharing
        /// </summary>
        Effect_DamageSharing = 1061,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_1062 = 1062,
        /// <summary>
        ///#1{~1~2 to }#2 (fixed Earth damage)
        /// </summary>
        Effect_1063 = 1063,
        /// <summary>
        ///#1{~1~2 to }#2 (fixed Air damage)
        /// </summary>
        Effect_1064 = 1064,
        /// <summary>
        ///#1{~1~2 to }#2 (fixed Water damage)
        /// </summary>
        Effect_1065 = 1065,
        /// <summary>
        ///#1{~1~2 to }#2 (fixed Fire damage)
        /// </summary>
        Effect_1066 = 1066,
        /// <summary>
        ///#1{~1~2 to }#2% of the target's HP (Air damage)
        /// </summary>
        Effect_1067 = 1067,
        /// <summary>
        ///#1{~1~2 to }#2% of the target's HP (Water damage)
        /// </summary>
        Effect_1068 = 1068,
        /// <summary>
        ///#1{~1~2 to }#2% of the target's HP (Fire damage)
        /// </summary>
        Effect_1069 = 1069,
        /// <summary>
        ///#1{~1~2 to }#2% of the target's HP (Earth damage)
        /// </summary>
        Effect_1070 = 1070,
        /// <summary>
        ///#1{~1~2 to }#2% of the target's HP (Neutral damage)
        /// </summary>
        Effect_1071 = 1071,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_1072 = 1072,
        /// <summary>
        ///Change attack element
        /// </summary>
        Effect_1073 = 1073,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_1074 = 1074,
        /// <summary>
        ///-#1{~1~2 to }#2 duration of effects
        /// </summary>
        Effect_ReduceEffectsDuration = 1075,
        /// <summary>
        ///#1{~1~2 to }#2% Resistance
        /// </summary>
        Effect_AddResistances = 1076,
        /// <summary>
        ///-#1{~1~2 to -}#2% Resistance
        /// </summary>
        Effect_SubResistances = 1077,
        /// <summary>
        ///#1{~1~2 to }#2% Vitality
        /// </summary>
        Effect_AddVitalityPercent = 1078,
        /// <summary>
        ///-#1{~1~2 to -}#2 AP
        /// </summary>
        Effect_SubAPTelefrag = 1079,
        /// <summary>
        ///-#1{~1~2 to -}#2 MP
        /// </summary>
        Effect_SubMP_1080 = 1080,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_1081 = 1081,
        /// <summary>
        ///Wrapped by: #4
        /// </summary>
        Effect_1082 = 1082,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_1083 = 1083,
        /// <summary>
        ///#1
        /// </summary>
        Effect_1084 = 1084,
        /// <summary>
        ///Quantity: #1
        /// </summary>
        Effect_1085 = 1085,
        /// <summary>
        ///For: #4
        /// </summary>
        Effect_1086 = 1086,
        /// <summary>
        ///Write a character's name
        /// </summary>
        Effect_1087 = 1087,
        /// <summary>
        ///Places a grade #2 glyph-aura
        /// </summary>
        Effect_Spawn_Glyph_Aura = 1091,
        /// <summary>
        ///#1{~1~2 to }#2% of the target's eroded HP inflicted as Neutral damage
        /// </summary>
        Effect_1092 = 1092,
        /// <summary>
        ///#1{~1~2 to }#2% of the target's eroded HP inflicted as Air damage
        /// </summary>
        Effect_1093 = 1093,
        /// <summary>
        ///#1{~1~2 to }#2% of the target's eroded HP inflicted as Fire damage
        /// </summary>
        Effect_1094 = 1094,
        /// <summary>
        ///#1{~1~2 to }#2% of the target's eroded HP inflicted as Water damage
        /// </summary>
        Effect_1095 = 1095,
        /// <summary>
        ///#1{~1~2 to }#2% of the target's eroded HP inflicted as Earth damage
        /// </summary>
        Effect_1096 = 1096,
        /// <summary>
        ///Creates illusions
        /// </summary>
        Effect_CreateIllusions = 1097,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_1098 = 1098,
        /// <summary>
        ///Teleports the target to the cell where they started their turn
        /// </summary>
        Effect_1099 = 1099,
        /// <summary>
        ///Teleports to previous position
        /// </summary>
        Effect_TPPreviousPosition = 1100,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_1101 = 1101,
        /// <summary>
        ///
        /// </summary>
        Effect_1102 = 1102,
        /// <summary>
        ///Pushes back #1 cell(s)
        /// </summary>
        Effect_PushBack_1103 = 1103,
        /// <summary>
        ///Teleports the caster to the other side of the target
        /// </summary>
        Effect_SymmetricTPTarget = 1104,
        /// <summary>
        ///Teleports the target to the other side of the caster
        /// </summary>
        Effect_SymmetricTPCaster = 1105,
        /// <summary>
        ///Symmetric teleportation
        /// </summary>
        Effect_SymmetricTPCenter = 1106,
        /// <summary>
        ///Rename the guild
        /// </summary>
        Effect_RenameGuild = 1107,
        /// <summary>
        ///Change the guild's emblem
        /// </summary>
        Effect_ChangeGuildEmblem = 1108,
        /// <summary>
        ///#1{~1~2 to }#2% (HP restored)
        /// </summary>
        Effect_RestoreHPPercent = 1109,
        /// <summary>
        ///#3 loot
        /// </summary>
        Effect_1111 = 1111,
        /// <summary>
        ///#1{~1~2 to }#2% of the caster's eroded HP inflicted as Neutral damage
        /// </summary>
        Effect_1118 = 1118,
        /// <summary>
        ///#1{~1~2 to }#2% of the caster's eroded HP inflicted as Air damage
        /// </summary>
        Effect_1119 = 1119,
        /// <summary>
        ///#1{~1~2 to }#2% of the caster's eroded HP inflicted as Fire damage
        /// </summary>
        Effect_1120 = 1120,
        /// <summary>
        ///#1{~1~2 to }#2% of the caster's eroded HP inflicted as Water damage
        /// </summary>
        Effect_1121 = 1121,
        /// <summary>
        ///#1{~1~2 to }#2% of the caster's eroded HP inflicted as Earth damage
        /// </summary>
        Effect_1122 = 1122,
        /// <summary>
        ///Redistributes #1{~1~2 à }#2% of damage suffered.
        /// </summary>
        Effect_1123 = 1123,
        /// <summary>
        ///
        /// </summary>
        Effect_1124 = 1124,
        /// <summary>
        ///
        /// </summary>
        Effect_1125 = 1125,
        /// <summary>
        ///
        /// </summary>
        Effect_1126 = 1126,
        /// <summary>
        ///
        /// </summary>
        Effect_1127 = 1127,
        /// <summary>
        ///
        /// </summary>
        Effect_1128 = 1128,
        /// <summary>
        ///Send to Krosmaster
        /// </summary>
        Effect_1129 = 1129,
        /// <summary>
        ///#2 HP (Air) lost for every #1 AP used
        /// </summary>
        Effect_DamageAirPerAP = 1131,
        /// <summary>
        ///#2 HP (Water) lost for every #1 AP used
        /// </summary>
        Effect_DamageWaterPerAP = 1132,
        /// <summary>
        ///#2 HP (Fire) lost for every #1 AP used
        /// </summary>
        Effect_DamageFirePerAP = 1133,
        /// <summary>
        ///#2 HP (Neutral) lost for every #1 AP used
        /// </summary>
        Effect_DamageNeutralPerAP = 1134,
        /// <summary>
        ///#2 HP (Earth) lost for every #1 AP used
        /// </summary>
        Effect_DamageEarthPerAP = 1135,
        /// <summary>
        ///#2 HP (Air) lost for every #1 MP used
        /// </summary>
        Effect_DamageAirPerMP = 1136,
        /// <summary>
        ///#2 HP (Water) lost for every #1 MP used
        /// </summary>
        Effect_DamageWaterPerMP = 1137,
        /// <summary>
        ///#2 HP (Fire) lost for every #1 MP used
        /// </summary>
        Effect_DamageFirePerMP = 1138,
        /// <summary>
        ///#2 HP (Neutral) lost for every #1 MP used
        /// </summary>
        Effect_DamageNeutralPerMP = 1139,
        /// <summary>
        ///#2 HP (Earth) lost for every #1 MP used
        /// </summary>
        Effect_DamageEarthPerMP = 1140,
        /// <summary>
        ///
        /// </summary>
        Effect_1141 = 1141,
        /// <summary>
        ///
        /// </summary>
        Effect_1142 = 1142,
        /// <summary>
        ///#1{~1~2 to }#2 Power (weapons)
        /// </summary>
        Effect_GeneralDamageWeapond = 1144,
        /// <summary>
        ///Change the alliance's emblem
        /// </summary>
        Effect_ChangeAllianceEmblem = 1145,
        /// <summary>
        ///Rename the alliance
        /// </summary>
        Effect_ChangeAllianceName = 1146,
        /// <summary>
        ///
        /// </summary>
        Effect_1149 = 1149,
        /// <summary>
        ///
        /// </summary>
        Effect_1150 = 1150,
        /// <summary>
        ///Afterimage of #1
        /// </summary>
        Effect_Appearance1 = 1151,
        /// <summary>
        ///
        /// </summary>
        Effect_1152 = 1152,
        /// <summary>
        ///Summons a Perceptor
        /// </summary>
        Effect_Summon_TaxCollector = 1153,
        /// <summary>
        ///
        /// </summary>
        Effect_1154 = 1154,
        /// <summary>
        ///Teleport
        /// </summary>
        Effect_Teleports = 1155,
        /// <summary>
        ///#1
        /// </summary>
        Effect_CastSpell = 1160,
        /// <summary>
        ///#1
        /// </summary>
        Effect_1161 = 1161,
        /// <summary>
        ///Positions the compass
        /// </summary>
        Effect_CompassGuides = 1162,
        /// <summary>
        ///x#1% damage sustained
        /// </summary>
        Effect_DamageSustained = 1163,
        /// <summary>
        ///Damage received will heal.
        /// </summary>
        Effect_RecievedDamagesHealt = 1164,
        /// <summary>
        ///Places a grade #2 glyph
        /// </summary>
        Effect_SpawnGlyphLevelTwo = 1165,
        /// <summary>
        ///#1{~1~2 to }#2 Power (glyphs)
        /// </summary>
        Effect_GeneralDamageGlyphs = 1166,
        /// <summary>
        ///
        /// </summary>
        Effect_1168 = 1168,
        /// <summary>
        ///
        /// </summary>
        Effect_1169 = 1169,
        /// <summary>
        ///
        /// </summary>
        Effect_1170 = 1170,
        /// <summary>
        ///Increases final damage inflicted by #1%
        /// </summary>
        Effect_AddFinalDamage = 1171,
        /// <summary>
        ///Reduces final damage inflicted by #1%
        /// </summary>
        Effect_SubFinalDamage = 1172,
        /// <summary>
        ///
        /// </summary>
        Effect_1173 = 1173,
        /// <summary>
        ///Deletes experience gains
        /// </summary>
        Effect_DeleteWinExp = 1174,
        /// <summary>
        ///#1
        /// </summary>
        Effect_1175 = 1175,
        /// <summary>
        ///Afterimage of #1
        /// </summary>
        Effect_1176 = 1176,
        /// <summary>
        ///
        /// </summary>
        Effect_1177 = 1177,
        /// <summary>
        ///
        /// </summary>
        Effect_1178 = 1178,
        /// <summary>
        ///Compatible with: #1
        /// </summary>
        Effect_CompatibleWith = 1179,
        /// <summary>
        ///Read #3
        /// </summary>
        Effect_Read = 1180,
        /// <summary>
        ///Places a portal (+#3% damage)
        /// </summary>
        Effect_SpawnPortal = 1181,
        /// <summary>
        ///Portal teleportation
        /// </summary>
        Effect_PortalTeleport = 1182,
        /// <summary>
        ///Deactivate a portal
        /// </summary>
        Effect_DesactivePortal = 1183,
        /// <summary>
        ///[!] Expérience du niveau : #3
        /// </summary>
        Effect_1184 = 1184,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_1185 = 1185,
        /// <summary>
        ///#1
        /// </summary>
        Effect_2017 = 2017,
        /// <summary>
        ///
        /// </summary>
        Effect_2018 = 2018,
        /// <summary>
        ///
        /// </summary>
        Effect_2019 = 2019,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_CureRecievedPercentDamage = 2020,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_2021 = 2021,
        /// <summary>
        ///#1
        /// </summary>
        Effect_2160 = 2160,
        /// <summary>
        ///#1
        /// </summary>
        Effect_2792 = 2792,
        /// <summary>
        ///#1
        /// </summary>
        Effect_2793 = 2793,
        /// <summary>
        ///[!] #1
        /// </summary>
        Effect_2794 = 2794,
        /// <summary>
        ///#1
        /// </summary>
        Effect_2795 = 2795,
        /// <summary>
        ///[!] Tue la cible pour la remplacer par l'invocation : #1
        /// </summary>
        Effect_KillAndSummon = 2796,
        /// <summary>
        ///Null Text
        /// </summary>
        Effect_TurtleLightningStrike = 2797,
        /// <summary>
        /// Increase the power of the Sadida's tree
        /// </summary>
        Effect_Foilage = 134868,
    }
}

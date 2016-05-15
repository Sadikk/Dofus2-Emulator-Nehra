using NLog;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Classes;
using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Database;
using Stump.Server.BaseServer.Initialization;
using Stump.Server.WorldServer.Database.Effects;
using Stump.Server.WorldServer.Database.Items.Templates;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Effects.Handlers;
using Stump.Server.WorldServer.Game.Effects.Handlers.Items;
using Stump.Server.WorldServer.Game.Effects.Handlers.Usables;
using Stump.Server.WorldServer.Game.Effects.Instances;
using Stump.Server.WorldServer.Game.Items.Player;
using System;
using System.Linq;
using System.Reflection;
using Stump.Server.WorldServer.Game.Effects.Spells;
using Stump.Server.WorldServer.Game.Effects.Spells.TargetMask;
using System.Collections.Generic;

namespace Stump.Server.WorldServer.Game.Effects
{
	public class EffectManager : DataManager<EffectManager>
	{
        public class TargetMaskHandler
        {
            public TargetMaskHandler(object container, Type containerType, char pattern, TargetMaskHandlerAttribute handlerAttribute, Func<object, FightActor, FightActor, EffectBase, object, bool> func)
            {
                Container = container;
                ContainerType = containerType;
                Pattern = pattern;
                Attribute = handlerAttribute;
                Func = func;
            }

            public object Container
            {
                get;
                private set;
            }

            public Type ContainerType
            {
                get;
                private set;
            }

            public char Pattern
            {
                get;
                private set;
            }

            public TargetMaskHandlerAttribute Attribute
            {
                get;
                private set;
            }

            public Func<object, FightActor, FightActor, EffectBase, object, bool> Func
            {
                get;
                private set;
            }
        }

        private delegate ItemEffectHandler ItemEffectConstructor(EffectBase effect, Character target, BasePlayerItem item);
		private delegate ItemEffectHandler ItemSetEffectConstructor(EffectBase effect, Character target, ItemSetTemplate itemSet, bool apply);
		private delegate UsableEffectHandler UsableEffectConstructor(EffectBase effect, Character target, BasePlayerItem item);
		private delegate SpellEffectHandler SpellEffectConstructor(EffectDice effect, FightActor caster, Stump.Server.WorldServer.Game.Spells.Spell spell, Cell targetedCell, bool critical);
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();
		private System.Collections.Generic.Dictionary<short, EffectTemplate> m_effects = new System.Collections.Generic.Dictionary<short, EffectTemplate>();
		private readonly System.Collections.Generic.Dictionary<EffectsEnum, EffectManager.ItemEffectConstructor> m_itemsEffectHandler = new System.Collections.Generic.Dictionary<EffectsEnum, EffectManager.ItemEffectConstructor>();
		private readonly System.Collections.Generic.Dictionary<EffectsEnum, EffectManager.ItemSetEffectConstructor> m_itemsSetEffectHandler = new System.Collections.Generic.Dictionary<EffectsEnum, EffectManager.ItemSetEffectConstructor>();
		private readonly System.Collections.Generic.Dictionary<EffectsEnum, EffectManager.UsableEffectConstructor> m_usablesEffectHandler = new System.Collections.Generic.Dictionary<EffectsEnum, EffectManager.UsableEffectConstructor>();
		private readonly System.Collections.Generic.Dictionary<EffectsEnum, EffectManager.SpellEffectConstructor> m_spellsEffectHandler = new System.Collections.Generic.Dictionary<EffectsEnum, EffectManager.SpellEffectConstructor>();
		private readonly System.Collections.Generic.Dictionary<EffectsEnum, System.Collections.Generic.List<System.Type>> m_effectsHandlers = new System.Collections.Generic.Dictionary<EffectsEnum, System.Collections.Generic.List<System.Type>>();
		private readonly EffectsEnum[] m_unRandomablesEffects = new EffectsEnum[]
		{
			EffectsEnum.Effect_DamageWater,
			EffectsEnum.Effect_DamageEarth,
			EffectsEnum.Effect_DamageAir,
			EffectsEnum.Effect_DamageFire,
			EffectsEnum.Effect_DamageNeutral,
			EffectsEnum.Effect_StealHPWater,
			EffectsEnum.Effect_StealHPEarth,
			EffectsEnum.Effect_StealHPAir,
			EffectsEnum.Effect_StealHPFire,
			EffectsEnum.Effect_StealHPNeutral,
			EffectsEnum.Effect_RemoveAP,
			EffectsEnum.Effect_RemainingFights
		};
        private readonly System.Collections.Generic.Dictionary<char, System.Collections.Generic.List<TargetMaskHandler>> m_targetMaskHandlers = new System.Collections.Generic.Dictionary<char, System.Collections.Generic.List<TargetMaskHandler>>();
        [Initialization(InitializationPass.Third)]
		public override void Initialize()
		{
			this.m_effects = base.Database.Fetch<EffectTemplate>(EffectTemplateRelator.FetchQuery, new object[0]).ToDictionary((EffectTemplate entry) => (short)entry.Id);
			this.InitializeEffectHandlers();
            this.InitializeTargetMaskHandlers();
		}

        private void InitializeTargetMaskHandlers()
        {
            foreach (System.Type type in
                from entry in System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                select entry
                )
            {
                var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Instance |
                    BindingFlags.Public | BindingFlags.NonPublic);
                object container = null;
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(TargetMaskHandlerAttribute), false) as TargetMaskHandlerAttribute[];

                    if (attributes == null || attributes.Length == 0)
                        continue;
                    var parameters = method.GetParameters();

                    if (parameters.Length != 4)
                    {
                        throw new ArgumentException(string.Format("Method handler {0} has incorrect parameters. Right definition is Handler(FightActor, FightActor, EffectBase, object)", method));
                    }

                    if (!method.IsStatic && container == null || method.IsStatic && container != null)
                        return;

                    Func<object, FightActor, FightActor, EffectBase, object, bool> handlerDelegate;
                    try
                    {
                        handlerDelegate = (Func<object, FightActor, FightActor, EffectBase, object, bool>)method.CreateFuncDelegate(typeof(bool), typeof(FightActor), typeof(FightActor), typeof(EffectBase), typeof(object));
                    }
                    catch (Exception)
                    {
                        throw new ArgumentException(string.Format("Method handler {0} has incorrect parameters. Right definition is Handler(FightActor, FightActor, EffectBase, object))", method));
                    }

                    foreach (var attribute in attributes)
                    {
                        if (attribute == null)
                        {
                            throw new ArgumentNullException("Attribute");
                        }
                        if(handlerDelegate == null)
                        {
                            throw new ArgumentNullException("Handler");
                        }

                        if (!m_targetMaskHandlers.ContainsKey(attribute.Pattern))
                            m_targetMaskHandlers.Add(attribute.Pattern, new System.Collections.Generic.List<TargetMaskHandler>());

                        m_targetMaskHandlers[attribute.Pattern].Add(new TargetMaskHandler(container, method.DeclaringType, attribute.Pattern, attribute, handlerDelegate));
                    }
                }
            }
        }

        private void InitializeEffectHandlers()
		{
			foreach (System.Type current in 
				from entry in System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
				where entry.IsSubclassOf(typeof(EffectHandler)) && !entry.IsAbstract
				select entry)
			{
				if (current.GetCustomAttribute<DefaultEffectHandlerAttribute>() == null)
				{
					EffectHandlerAttribute[] array = current.GetCustomAttributes<EffectHandlerAttribute>().ToArray<EffectHandlerAttribute>();
					if (array.Length == 0)
					{
						EffectManager.logger.Error("EffectHandler '{0}' has no EffectHandlerAttribute", current.Name);
					}
					else
					{
						foreach (EffectsEnum current2 in array.Select((EffectHandlerAttribute entry) => entry.Effect))
						{
							if (current.IsSubclassOf(typeof(ItemEffectHandler)))
							{
								System.Reflection.ConstructorInfo constructor = current.GetConstructor(new System.Type[]
								{
									typeof(EffectBase),
									typeof(Character),
									typeof(BasePlayerItem)
								});
								this.m_itemsEffectHandler.Add(current2, constructor.CreateDelegate<EffectManager.ItemEffectConstructor>());
								System.Reflection.ConstructorInfo constructor2 = current.GetConstructor(new System.Type[]
								{
									typeof(EffectBase),
									typeof(Character),
									typeof(ItemSetTemplate),
									typeof(bool)
								});
								if (constructor2 != null)
								{
									this.m_itemsSetEffectHandler.Add(current2, constructor2.CreateDelegate<EffectManager.ItemSetEffectConstructor>());
								}
							}
							else
							{
								if (current.IsSubclassOf(typeof(UsableEffectHandler)))
								{
									System.Reflection.ConstructorInfo constructor = current.GetConstructor(new System.Type[]
									{
										typeof(EffectBase),
										typeof(Character),
										typeof(BasePlayerItem)
									});
									this.m_usablesEffectHandler.Add(current2, constructor.CreateDelegate<EffectManager.UsableEffectConstructor>());
								}
								else
								{
									if (current.IsSubclassOf(typeof(SpellEffectHandler)))
									{
										System.Reflection.ConstructorInfo constructor = current.GetConstructor(new System.Type[]
										{
											typeof(EffectDice),
											typeof(FightActor),
											typeof(Stump.Server.WorldServer.Game.Spells.Spell),
											typeof(Cell),
											typeof(bool)
										});
										this.m_spellsEffectHandler.Add(current2, constructor.CreateDelegate<EffectManager.SpellEffectConstructor>());
									}
								}
							}
							if (!this.m_effectsHandlers.ContainsKey(current2))
							{
								this.m_effectsHandlers.Add(current2, new System.Collections.Generic.List<System.Type>());
							}
							this.m_effectsHandlers[current2].Add(current);
						}
					}
				}
			}
		}
		public EffectBase ConvertExportedEffect(EffectInstance effect)
		{
			EffectBase result;
			if (effect is EffectInstanceLadder)
			{
				result = new EffectLadder(effect as EffectInstanceLadder);
			}
			else
			{
				if (effect is EffectInstanceCreature)
				{
					result = new EffectCreature(effect as EffectInstanceCreature);
				}
				else
				{
					if (effect is EffectInstanceDate)
					{
						result = new EffectDate(effect as EffectInstanceDate);
					}
					else
					{
						if (effect is EffectInstanceDice)
						{
							result = new EffectDice(effect as EffectInstanceDice);
						}
						else
						{
							if (effect is EffectInstanceDuration)
							{
								result = new EffectDuration(effect as EffectInstanceDuration);
							}
							else
							{
								if (effect is EffectInstanceMinMax)
								{
									result = new EffectMinMax(effect as EffectInstanceMinMax);
								}
								else
								{
									if (effect is EffectInstanceMount)
									{
										result = new EffectMount(effect as EffectInstanceMount);
									}
									else
									{
										if (effect is EffectInstanceString)
										{
											result = new EffectString(effect as EffectInstanceString);
										}
										else
										{
											if (effect is EffectInstanceInteger)
											{
												result = new EffectInteger(effect as EffectInstanceInteger);
											}
											else
											{
												result = new EffectBase(effect);
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}
		public System.Collections.Generic.IEnumerable<EffectBase> ConvertExportedEffect(System.Collections.Generic.IEnumerable<EffectInstance> effects)
		{
			return effects.Select(new Func<EffectInstance, EffectBase>(this.ConvertExportedEffect));
		}
		public EffectTemplate GetTemplate(short id)
		{
			return (!this.m_effects.ContainsKey(id)) ? null : this.m_effects[id];
		}
		public System.Collections.Generic.IEnumerable<EffectTemplate> GetTemplates()
		{
			return this.m_effects.Values;
		}
		public void AddItemEffectHandler(ItemEffectHandler handler)
		{
			System.Type type = handler.GetType();
			if (type.GetCustomAttribute<DefaultEffectHandlerAttribute>() != null)
			{
				throw new System.Exception("Default handler cannot be added");
			}
			EffectHandlerAttribute[] array = type.GetCustomAttributes<EffectHandlerAttribute>().ToArray<EffectHandlerAttribute>();
			if (array.Length == 0)
			{
				throw new System.Exception(string.Format("EffectHandler '{0}' has no EffectHandlerAttribute", type.Name));
			}
			System.Reflection.ConstructorInfo constructor = type.GetConstructor(new System.Type[]
			{
				typeof(EffectBase),
				typeof(Character),
				typeof(BasePlayerItem)
			});
			if (constructor == null)
			{
				throw new System.Exception("No valid constructors found !");
			}
			foreach (EffectsEnum current in 
				from entry in array
				select entry.Effect)
			{
				this.m_itemsEffectHandler.Add(current, constructor.CreateDelegate<EffectManager.ItemEffectConstructor>());
				if (!this.m_effectsHandlers.ContainsKey(current))
				{
					this.m_effectsHandlers.Add(current, new System.Collections.Generic.List<System.Type>());
				}
				this.m_effectsHandlers[current].Add(type);
			}
		}
		public ItemEffectHandler GetItemEffectHandler(EffectBase effect, Character target, BasePlayerItem item)
		{
			EffectManager.ItemEffectConstructor itemEffectConstructor;
			ItemEffectHandler result;
			if (this.m_itemsEffectHandler.TryGetValue(effect.EffectId, out itemEffectConstructor))
			{
				result = itemEffectConstructor(effect, target, item);
			}
			else
			{
				result = new DefaultItemEffect(effect, target, item);
			}
			return result;
		}
		public ItemEffectHandler GetItemEffectHandler(EffectBase effect, Character target, ItemSetTemplate itemSet, bool apply)
		{
			EffectManager.ItemSetEffectConstructor itemSetEffectConstructor;
			ItemEffectHandler result;
			if (this.m_itemsSetEffectHandler.TryGetValue(effect.EffectId, out itemSetEffectConstructor))
			{
				result = itemSetEffectConstructor(effect, target, itemSet, apply);
			}
			else
			{
				result = new DefaultItemEffect(effect, target, itemSet, apply);
			}
			return result;
		}
		public void AddUsableEffectHandler(UsableEffectHandler handler)
		{
			System.Type type = handler.GetType();
			if (type.GetCustomAttribute<DefaultEffectHandlerAttribute>() != null)
			{
				throw new System.Exception("Default handler cannot be added");
			}
			EffectHandlerAttribute[] array = type.GetCustomAttributes<EffectHandlerAttribute>().ToArray<EffectHandlerAttribute>();
			if (array.Length == 0)
			{
				throw new System.Exception(string.Format("EffectHandler '{0}' has no EffectHandlerAttribute", type.Name));
			}
			System.Reflection.ConstructorInfo constructor = type.GetConstructor(new System.Type[]
			{
				typeof(EffectBase),
				typeof(Character),
				typeof(BasePlayerItem)
			});
			if (constructor == null)
			{
				throw new System.Exception("No valid constructors found !");
			}
			foreach (EffectsEnum current in 
				from entry in array
				select entry.Effect)
			{
				this.m_usablesEffectHandler.Add(current, constructor.CreateDelegate<EffectManager.UsableEffectConstructor>());
				if (!this.m_effectsHandlers.ContainsKey(current))
				{
					this.m_effectsHandlers.Add(current, new System.Collections.Generic.List<System.Type>());
				}
				this.m_effectsHandlers[current].Add(type);
			}
		}
		public UsableEffectHandler GetUsableEffectHandler(EffectBase effect, Character target, BasePlayerItem item)
		{
			EffectManager.UsableEffectConstructor usableEffectConstructor;
			UsableEffectHandler result;
			if (this.m_usablesEffectHandler.TryGetValue(effect.EffectId, out usableEffectConstructor))
			{
				result = usableEffectConstructor(effect, target, item);
			}
			else
			{
				result = new DefaultUsableEffectHandler(effect, target, item);
			}
			return result;
		}
		public void AddSpellEffectHandler(SpellEffectHandler handler)
		{
			System.Type type = handler.GetType();
			if (type.GetCustomAttribute<DefaultEffectHandlerAttribute>() != null)
			{
				throw new System.Exception("Default handler cannot be added");
			}
			EffectHandlerAttribute[] array = type.GetCustomAttributes<EffectHandlerAttribute>().ToArray<EffectHandlerAttribute>();
			if (array.Length == 0)
			{
				throw new System.Exception(string.Format("EffectHandler '{0}' has no EffectHandlerAttribute", type.Name));
			}
			System.Reflection.ConstructorInfo constructor = type.GetConstructor(new System.Type[]
			{
				typeof(EffectDice),
				typeof(FightActor),
				typeof(Stump.Server.WorldServer.Game.Spells.Spell),
				typeof(Cell),
				typeof(bool)
			});
			if (constructor == null)
			{
				throw new System.Exception("No valid constructors found !");
			}
			foreach (EffectsEnum current in 
				from entry in array
				select entry.Effect)
			{
				this.m_spellsEffectHandler.Add(current, constructor.CreateDelegate<EffectManager.SpellEffectConstructor>());
				if (!this.m_effectsHandlers.ContainsKey(current))
				{
					this.m_effectsHandlers.Add(current, new System.Collections.Generic.List<System.Type>());
				}
				this.m_effectsHandlers[current].Add(type);
			}
		}
		public SpellEffectHandler GetSpellEffectHandler(EffectDice effect, FightActor caster, Stump.Server.WorldServer.Game.Spells.Spell spell, Cell targetedCell, bool critical)
		{
			EffectManager.SpellEffectConstructor spellEffectConstructor;
			SpellEffectHandler result;
			if (this.m_spellsEffectHandler.TryGetValue(effect.EffectId, out spellEffectConstructor))
			{
				result = spellEffectConstructor(effect, caster, spell, targetedCell, critical);
			}
			else
			{
				result = new DefaultSpellEffect(effect, caster, spell, targetedCell, critical);
			}
			return result;
		}
        public IEnumerable<TargetMaskHandler> GetTargetMaskHandlers(char pattern, object token)
        {
            List<TargetMaskHandler> handlersList;
            if (m_targetMaskHandlers.TryGetValue(pattern, out handlersList))
            {
                foreach (var handler in handlersList)
                {
                    if (token != null && handler.TokenType.IsInstanceOfType(token))
                    {
                        yield return handler;
                    }
                }
            }
        }
        public bool IsEffectHandledBy(EffectsEnum effect, System.Type handlerType)
		{
			return this.m_effectsHandlers.ContainsKey(effect) && this.m_effectsHandlers[effect].Contains(handlerType);
		}
		public bool IsUnRandomableWeaponEffect(EffectsEnum effect)
		{
			return this.m_unRandomablesEffects.Contains(effect);
		}
		public EffectInstance GuessRealEffect(EffectInstance effect)
		{
			EffectInstance result;
			if (!(effect is EffectInstanceDice))
			{
				result = effect;
			}
			else
			{
				EffectInstanceDice effectInstanceDice = effect as EffectInstanceDice;
				if (effectInstanceDice.value == 0 && effectInstanceDice.diceNum > 0u && effectInstanceDice.diceSide > 0u)
				{
					result = new EffectInstanceMinMax
					{
						duration = effectInstanceDice.duration,
						effectId = effectInstanceDice.effectId,
						max = effectInstanceDice.diceSide,
						min = effectInstanceDice.diceNum,
						modificator = effectInstanceDice.modificator,
						random = effectInstanceDice.random,
						targetId = effectInstanceDice.targetId,
						trigger = effectInstanceDice.trigger,
						zoneShape = effectInstanceDice.zoneShape,
						zoneSize = effectInstanceDice.zoneSize
					};
				}
				else
				{
					if (effectInstanceDice.value == 0 && effectInstanceDice.diceNum == 0u && effectInstanceDice.diceSide > 0u)
					{
						result = new EffectInstanceMinMax
						{
							duration = effectInstanceDice.duration,
							effectId = effectInstanceDice.effectId,
							max = effectInstanceDice.diceSide,
							min = effectInstanceDice.diceNum,
							modificator = effectInstanceDice.modificator,
							random = effectInstanceDice.random,
							targetId = effectInstanceDice.targetId,
							trigger = effectInstanceDice.trigger,
							zoneShape = effectInstanceDice.zoneShape,
							zoneSize = effectInstanceDice.zoneSize
						};
					}
					else
					{
						result = effect;
					}
				}
			}
			return result;
		}
		public byte[] SerializeEffect(EffectInstance effectInstance)
		{
			return this.ConvertExportedEffect(effectInstance).Serialize();
		}
		public byte[] SerializeEffect(EffectBase effect)
		{
			return effect.Serialize();
		}
		public byte[] SerializeEffects(System.Collections.Generic.IEnumerable<EffectBase> effects)
		{
			System.Collections.Generic.List<byte> list = new System.Collections.Generic.List<byte>();
			foreach (EffectBase current in effects)
			{
				list.AddRange(current.Serialize());
			}
			return list.ToArray();
		}
		public byte[] SerializeEffects(System.Collections.Generic.IEnumerable<EffectInstance> effects)
		{
			System.Collections.Generic.List<byte> list = new System.Collections.Generic.List<byte>();
			foreach (EffectInstance current in effects)
			{
				list.AddRange(this.SerializeEffect(current));
			}
			return list.ToArray();
		}
		public System.Collections.Generic.List<EffectBase> DeserializeEffects(byte[] buffer)
		{
			System.Collections.Generic.List<EffectBase> list = new System.Collections.Generic.List<EffectBase>();
			int num = 0;
			while (num + 1 < buffer.Length)
			{
				list.Add(this.DeserializeEffect(buffer, ref num));
			}
			return list;
		}
		public EffectBase DeserializeEffect(byte[] buffer, ref int index)
		{
			if (buffer.Length < index)
			{
				throw new System.Exception("buffer too small to contain an Effect");
			}
			byte b = buffer[index];
			EffectBase effectBase;
			switch (b)
			{
			case 1:
				effectBase = new EffectBase();
				break;
			case 2:
				effectBase = new EffectCreature();
				break;
			case 3:
				effectBase = new EffectDate();
				break;
			case 4:
				effectBase = new EffectDice();
				break;
			case 5:
				effectBase = new EffectDuration();
				break;
			case 6:
				effectBase = new EffectInteger();
				break;
			case 7:
				effectBase = new EffectLadder();
				break;
			case 8:
				effectBase = new EffectMinMax();
				break;
			case 9:
				effectBase = new EffectMount();
				break;
			case 10:
				effectBase = new EffectString();
				break;
			default:
				throw new System.Exception(string.Format("Incorrect identifier : {0}", b));
			}
			index++;
			effectBase.DeSerialize(buffer, ref index);
			return effectBase;
		}
	}
}

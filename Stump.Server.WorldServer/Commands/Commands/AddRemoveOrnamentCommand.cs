using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.BaseServer.Commands.Patterns;
using Stump.Server.WorldServer.Commands.Trigger;
using Stump.Server.WorldServer.Database.Tinsel;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class AddRemoveOrnamentCommand : AddRemoveCommand
	{
		public AddRemoveOrnamentCommand()
		{
			base.Aliases = new string[]
			{
				"ornament"
			};
			base.RequiredRole = RoleEnum.GameMaster;
			base.Description = "Add or remove an ornament on the target";
			base.AddParameter<Character>("target", "t", "Target", null, false, ParametersConverter.CharacterConverter);
			base.AddParameter<short>("id", "id", "Id of the ornament", 0, true, null);
			base.AddParameter<bool>("all", "a", "Add/remove all ornaments", false, true, null);
		}
		public Character GetTarget(TriggerBase trigger)
		{
			Character character = null;
			if (trigger.IsArgumentDefined("target"))
			{
				character = trigger.Get<Character>("target");
			}
			else
			{
				if (trigger is GameTrigger)
				{
					character = (trigger as GameTrigger).Character;
				}
			}
			if (character == null)
			{
				throw new System.Exception("Target is not defined");
			}
			return character;
		}
		public override void ExecuteAdd(TriggerBase trigger)
		{
			Character target = this.GetTarget(trigger);
			if (trigger.Get<bool>("all"))
			{
				foreach (System.Collections.Generic.KeyValuePair<ushort, OrnamentRecord> current in Singleton<TinselManager>.Instance.Ornaments)
				{
					target.AddOrnament(current.Key);
				}
				trigger.ReplyBold("{0} learned all ornaments", new object[]
				{
					target
				});
			}
			else
			{
				if (!trigger.IsArgumentDefined("id"))
				{
					trigger.ReplyError("Define at least one argument (id or -all)");
				}
				else
				{
					ushort num = trigger.Get<ushort>("id");
					if (!Singleton<TinselManager>.Instance.Ornaments.ContainsKey(num))
					{
						trigger.ReplyError("Ornament {0} doesn't exists");
					}
					else
					{
						target.AddOrnament(num);
						trigger.ReplyBold("{0} learned ornament {1}", new object[]
						{
							target,
							num
						});
					}
				}
			}
		}
		public override void ExecuteRemove(TriggerBase trigger)
		{
			Character target = this.GetTarget(trigger);
			if (trigger.Get<bool>("all"))
			{
				target.RemoveAllOrnament();
				target.ResetOrnament();
				trigger.ReplyBold("{0} forgot all ornaments", new object[]
				{
					target
				});
			}
			else
			{
				if (!trigger.IsArgumentDefined("id"))
				{
					trigger.ReplyError("Define at least one argument (id or -all)");
				}
				else
				{
					ushort num = trigger.Get<ushort>("id");
					if (!Singleton<TinselManager>.Instance.Ornaments.ContainsKey(num))
					{
						trigger.ReplyError("Ornament {0} doesn't exists");
					}
					else
					{
						target.RemoveOrnament(num);
						if (target.SelectedOrnament == num)
						{
							target.ResetOrnament();
						}
						trigger.ReplyBold("{0} forgot ornament {1}", new object[]
						{
							target,
							num
						});
					}
				}
			}
		}
	}
}

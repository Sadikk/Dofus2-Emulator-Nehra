using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.BaseServer.Commands.Patterns;
using Stump.Server.WorldServer.Commands.Trigger;
using Stump.Server.WorldServer.Database.Tinsel;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class AddRemoveTitleCommand : AddRemoveCommand
	{
		public AddRemoveTitleCommand()
		{
			base.Aliases = new string[]
			{
				"title"
			};
			base.RequiredRole = RoleEnum.GameMaster;
			base.Description = "Add or remove a title on the target";
			base.AddParameter<Character>("target", "t", "Target", null, false, ParametersConverter.CharacterConverter);
			base.AddParameter<short>("id", "id", "Id of the title", 0, true, null);
			base.AddParameter<bool>("all", "a", "Add/remove all titles", false, true, null);
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
				foreach (System.Collections.Generic.KeyValuePair<ushort, TitleRecord> current in Singleton<TinselManager>.Instance.Titles)
				{
					target.AddTitle(current.Key);
				}
				trigger.ReplyBold("{0} learned all titles", new object[]
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
					if (!Singleton<TinselManager>.Instance.Titles.ContainsKey(num))
					{
						trigger.ReplyError("Title {0} doesn't exists");
					}
					else
					{
						target.AddTitle(num);
						trigger.ReplyBold("{0} learned title {1}", new object[]
						{
							target,
							Singleton<TinselManager>.Instance.Titles[num].Name
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
				foreach (System.Collections.Generic.KeyValuePair<ushort, TitleRecord> current in Singleton<TinselManager>.Instance.Titles)
				{
					target.RemoveTitle(current.Key);
				}
				target.ResetTitle();
				trigger.ReplyBold("{0} forgot all titles", new object[]
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
					if (!Singleton<TinselManager>.Instance.Titles.ContainsKey(num))
					{
						trigger.ReplyError("Title {0} doesn't exists");
					}
					else
					{
						target.RemoveTitle(num);
						if (target.SelectedTitle == num)
						{
							target.ResetTitle();
						}
						trigger.ReplyBold("{0} forgot title {1}", new object[]
						{
							target,
							Singleton<TinselManager>.Instance.Titles[num].Name
						});
					}
				}
			}
		}
	}
}

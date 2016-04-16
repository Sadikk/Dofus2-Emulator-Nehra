using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Database.I18n;
using Stump.Server.WorldServer.Game;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Monsters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Npcs;
using Stump.Server.WorldServer.Game.Breeds;
using Stump.Server.WorldServer.Game.Effects;
using Stump.Server.WorldServer.Game.Interactives;
using Stump.Server.WorldServer.Game.Items;
using Stump.Server.WorldServer.Game.Spells;
using System.Drawing;
using System.Threading.Tasks;
namespace Stump.Server.WorldServer.Commands.Commands
{
	public class ReloadCommands : CommandBase
	{
		public System.Collections.Generic.Dictionary<string, object> m_entries = new System.Collections.Generic.Dictionary<string, object>
		{

			{
				"npcs",
				Singleton<NpcManager>.Instance
			},

			{
				"monsters",
				Singleton<MonsterManager>.Instance
			},

			{
				"items",
				Singleton<ItemManager>.Instance
			},

			{
				"world",
				Singleton<World>.Instance
			},

			{
				"spells",
				Singleton<SpellManager>.Instance
			},

			{
				"effects",
				Singleton<EffectManager>.Instance
			},

			{
				"interactives",
				Singleton<InteractiveManager>.Instance
			},

			{
				"breeds",
				Singleton<BreedManager>.Instance
			},

			{
				"experiences",
				Singleton<ExperienceManager>.Instance
			},

			{
				"langs",
				Singleton<TextManager>.Instance
			}
		};
		public ReloadCommands()
		{
			base.Aliases = new string[]
			{
				"reload"
			};
			base.RequiredRole = RoleEnum.Administrator;
			base.Description = "Reload manager";
			base.AddParameter<string>("name", "n", "Name of the manager to reload", null, true, null);
		}
		public override void Execute(TriggerBase trigger)
		{
			if (!trigger.IsArgumentDefined("name"))
			{
				trigger.Reply("Entries : " + string.Join(", ", this.m_entries.Keys));
			}
			else
			{
				string name = trigger.Get<string>("name").ToLower();
				object entry;
				if (!this.m_entries.TryGetValue(name, out entry))
				{
					trigger.ReplyError("{0} not a valid name.", new object[]
					{
						name
					});
					trigger.ReplyError("Entries : " + string.Join(", ", this.m_entries.Keys));
				}
				else
				{
					System.Reflection.MethodInfo method = entry.GetType().GetMethod("Initialize", new System.Type[0]);
					if (method == null)
					{
						trigger.ReplyError("Cannot reload {0} : method Initialize() not found", new object[]
						{
							name
						});
					}
					else
					{
						Singleton<World>.Instance.SendAnnounce("[RELOAD] Reloading " + name + " ... WORLD PAUSED", Color.DodgerBlue);
						Task.Factory.StartNew(delegate
						{
							Singleton<World>.Instance.Pause();
							try
							{
								method.Invoke(entry, new object[0]);
							}
							finally
							{
								Singleton<World>.Instance.Resume();
							}
							Singleton<World>.Instance.SendAnnounce("[RELOAD] " + name + " reloaded ... WORLD RESUMED", Color.DodgerBlue);
						});
					}
				}
			}
		}
	}
}

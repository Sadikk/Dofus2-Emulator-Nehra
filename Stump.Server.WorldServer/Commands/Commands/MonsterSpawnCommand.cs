using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Trigger;
using Stump.Server.WorldServer.Database.Monsters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Monsters;
using Stump.Server.WorldServer.Game.Maps;
using Stump.Server.WorldServer.Game.Maps.Cells;

namespace Stump.Server.WorldServer.Commands.Commands
{
	public class MonsterSpawnCommand : SubCommand
	{
		public MonsterSpawnCommand()
		{
			base.Aliases = new string[]
			{
				"spawn"
			};
			base.RequiredRole = RoleEnum.GameMaster;
			base.Description = "Spawn a monster on the current location";
			base.ParentCommand = typeof(MonsterCommands);
			base.AddParameter<MonsterTemplate>("monster", "m", "Monster template Id", null, false, ParametersConverter.MonsterTemplateConverter);
			base.AddParameter<sbyte>("grade", "g", "Monster grade", 0, true, null);
			base.AddParameter<sbyte>("id", "id", "Monster group id", 0, true, null);
			base.AddParameter<Map>("map", "map", "Map id", null, true, ParametersConverter.MapConverter);
			base.AddParameter<short>("cell", "cell", "Cell id", 0, true, null);
			base.AddParameter<DirectionsEnum>("direction", "dir", "Direction", DirectionsEnum.DIRECTION_EAST, true, ParametersConverter.GetEnumConverter<DirectionsEnum>());
		}
		public override void Execute(TriggerBase trigger)
		{
			MonsterTemplate monsterTemplate = trigger.Get<MonsterTemplate>("monster");
			ObjectPosition objectPosition = null;
			if (monsterTemplate.Grades.Count <= (int)trigger.Get<sbyte>("grade"))
			{
				trigger.ReplyError("Unexistant grade '{0}' for this monster", new object[]
				{
					trigger.Get<sbyte>("grade")
				});
			}
			else
			{
				MonsterGrade monsterGrade = monsterTemplate.Grades[(int)trigger.Get<sbyte>("grade")];
				if (monsterGrade.Template.EntityLook == null)
				{
					trigger.ReplyError("Cannot display this monster");
				}
				else
				{
					if (trigger.IsArgumentDefined("map") && trigger.IsArgumentDefined("cell") && trigger.IsArgumentDefined("direction"))
					{
						Map map = trigger.Get<Map>("map");
						short cellId = trigger.Get<short>("cell");
						DirectionsEnum direction = trigger.Get<DirectionsEnum>("direction");
						objectPosition = new ObjectPosition(map, cellId, direction);
					}
					else
					{
						if (trigger is GameTrigger)
						{
							objectPosition = (trigger as GameTrigger).Character.Position;
						}
					}
					if (objectPosition == null)
					{
						trigger.ReplyError("Position of monster is not defined");
					}
					else
					{
						MonsterGroup monsterGroup;
						if (trigger.IsArgumentDefined("id"))
						{
							monsterGroup = objectPosition.Map.GetActor<MonsterGroup>((int)trigger.Get<sbyte>("id"));
							if (monsterGroup == null)
							{
								trigger.ReplyError("Group with id '{0}' not found", new object[]
								{
									trigger.Get<sbyte>("id")
								});
								return;
							}
							monsterGroup.AddMonster(new Monster(monsterGrade, monsterGroup));
						}
						else
						{
							monsterGroup = objectPosition.Map.SpawnMonsterGroup(monsterGrade, objectPosition);
						}
						trigger.Reply("Monster '{0}' added to the group '{1}'", new object[]
						{
							monsterTemplate.Id,
							monsterGroup.Id
						});
					}
				}
			}
		}
	}
}

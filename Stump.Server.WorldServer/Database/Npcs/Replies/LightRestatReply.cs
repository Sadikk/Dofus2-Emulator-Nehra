using Stump.Server.BaseServer.Database;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Npcs;

namespace Stump.Server.WorldServer.Database.Npcs.Replies
{
	[Discriminator("LightRestats", typeof(NpcReply), new System.Type[]
	{
		typeof(NpcReplyRecord)
	})]
	public class LightRestatReply : NpcReply
	{
		public LightRestatReply(NpcReplyRecord record) : base(record)
		{
		}
		public override bool Execute(Npc npc, Character character)
		{
			bool result;
			if (!base.Execute(npc, character))
			{
				result = false;
			}
			else
			{
				character.Stats.Agility.Base = (int)character.PermanentAddedAgility;
				character.Stats.Strength.Base = (int)character.PermanentAddedStrength;
				character.Stats.Vitality.Base = (int)character.PermanentAddedVitality;
				character.Stats.Wisdom.Base = (int)character.PermanentAddedWisdom;
				character.Stats.Intelligence.Base = (int)character.PermanentAddedIntelligence;
				character.Stats.Chance.Base = (int)character.PermanentAddedChance;
				character.StatsPoints = (ushort)(character.Level * 5);
				character.RefreshStats();
				if (RestatReply.RestatOnce)
				{
					character.CanRestat = false;
				}
				result = true;
			}
			return result;
		}
	}
}

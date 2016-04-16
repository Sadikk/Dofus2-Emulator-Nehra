using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using System;

namespace Stump.Server.WorldServer.Game.Conditions.Criterions
{
    public class GuildRightsCriterion : Criterion
    {
        public const string Identifier = "Px";
        public GuildRightsBitEnum Right { get; set; }

        public override bool Eval(Character character)
        {
            return (character.GuildMember != null && character.GuildMember.HasRight(this.Right));
        }

        public override void Build()
        {
            GuildRightsBitEnum right;
            if (!Enum.TryParse(base.Literal, out right))
            {
                throw new System.Exception(string.Format("Cannot build LevelCriterion, {0} is not a valid right", base.Literal));
            }
            this.Right = right;
        }

        public override string ToString()
        {
            return base.FormatToString(GuildRightsCriterion.Identifier);
        }
    }
}

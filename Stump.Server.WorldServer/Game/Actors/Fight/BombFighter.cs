using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Database.Monsters;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Fights.Teams;
using Stump.Server.WorldServer.Handlers.Context;

namespace Stump.Server.WorldServer.Game.Actors.Fight
{
    public class BombFighter : SummonedMonster
    {
        // FIELDS
        public const short BUFF_PERCENT = 20;

        private byte m_row;
        private FightTemporaryBoostEffect m_effect;
 
        // PROPERTIES
        public short ComboPercent
        {
            get
            {
                return (short)(this.m_row * BUFF_PERCENT);
            }
        }
        public bool CanBeBoosted
        {
            get
            {
                return this.m_row <= 3;
            }
        }

        // CONSTRUCTORS
        public BombFighter(int id, FightTeam team, FightActor summoner, MonsterGrade template, Cell cell)
            : base(id, team, summoner, template, cell)
        {
            this.m_row = 1;

            base.Look = this.Monster.Template.EntityLook.Clone();
        }

        // METHODS
        public void BuffBomb()
        {
            if (this.CanBeBoosted)
            {
                this.m_row++;

                bool update;
                if (update = (this.m_effect == null))
                {
                    this.m_effect = this.GetDispellableEffect();
                }
                else
                {
                    this.m_effect.delta = this.ComboPercent;
                }

                base.Look.ChangeScale((short)(80 + (this.m_row * BUFF_PERCENT)));

                ContextHandler.SendGameActionFightDispellableEffectMessage(base.Fight.Clients, 1027, this, this.m_effect, update);
                ContextHandler.SendGameActionFightChangeLookMessage(base.Fight.Clients, this, this);
            }
        }

        private FightTemporaryBoostEffect GetDispellableEffect()
        {
            var spellLevel = Stump.Core.Reflection.Singleton<Spells.SpellManager>.Instance.GetSpellLevel(19067);
            var effect = spellLevel.Effects[0];

            return new FightTemporaryBoostEffect(7, base.Id, 0, 1, (ushort)spellLevel.Spell.Id, (uint)effect.Id, 0, this.ComboPercent);
        }
    }
}

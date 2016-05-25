using NLog;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.Server.WorldServer.AI.Fights.Brain;
using Stump.Server.WorldServer.Game.Actors.Interfaces;
using Stump.Server.WorldServer.Game.Fights.Teams;
using Stump.Server.WorldServer.Game.Spells;
using Stump.Server.WorldServer.Handlers.Chat;
using System;
using System.Linq;

namespace Stump.Server.WorldServer.Game.Actors.Fight
{
    public abstract class AIFighter : FightActor, INamedActor
    {
        protected new static Logger logger = LogManager.GetCurrentClassLogger();

        public Brain Brain
        {
            get;
            protected set;
        }
        public bool Frozen
        {
            get;
            set;
        }
        public System.Collections.Generic.Dictionary<int, Spell> Spells
        {
            get;
            private set;
        }
        public abstract string Name
        {
            get;
        }
        public override bool IsReady
        {
            get
            {
                return true;
            }
            protected set
            {
            }
        }

        protected AIFighter(FightTeam team, System.Collections.Generic.IEnumerable<Spell> spells)
            : base(team)
        {
            this.Spells = spells.ToDictionary((Spell entry) => entry.Id);
            this.Brain = Singleton<BrainManager>.Instance.GetDefaultBrain(this);
            base.Fight.TurnStarted += new Action<Fights.Fight, FightActor>(this.OnTurnStarted);
        }
        protected AIFighter(FightTeam team, System.Collections.Generic.IEnumerable<Spell> spells, int identifier)
            : base(team)
        {
            this.Spells = spells.ToDictionary((Spell entry) => entry.Id);
            this.Brain = Singleton<BrainManager>.Instance.GetBrain(identifier, this);
            base.Fight.TurnStarted += new Action<Fights.Fight, FightActor>(this.OnTurnStarted);
        }

        public override Spell GetSpell(int id)
        {
            return this.Spells.ContainsKey(id) ? this.Spells[id] : null;
        }
        public override bool HasSpell(int id)
        {
            return this.Spells.ContainsKey(id);
        }
        private void OnTurnStarted(Fights.Fight fight, FightActor currentfighter)
        {
            if (base.IsFighterTurn() && !this.Frozen) 
            {
                this.PlayIA();
            }
        }

        protected virtual void PlayIA()
        {
            try
            {
                this.Brain.Play();
            }
            catch (System.Exception ex)
            {
                AIFighter.logger.Error<AIFighter, System.Exception>("Monster {0}, AI engine failed : {1}", this, ex);
                if (Brain.DebugMode)
                {
                    this.Say("My AI has just failed :s (" + ex.Message + ")");
                }
            }
            finally
            {
                base.Fight.StopTurn();
            }
        }

        public void Say(string msg)
        {
            ChatHandler.SendChatServerMessage(base.Fight.Clients, this, ChatActivableChannelsEnum.CHANNEL_GLOBAL, msg);
        }
    }
}

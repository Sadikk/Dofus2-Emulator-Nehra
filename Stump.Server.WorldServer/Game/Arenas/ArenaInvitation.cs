using Stump.Core.Attributes;
using Stump.Server.WorldServer.Game.Notifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stump.Core.Threading;
using Stump.Server.WorldServer.Core.Network;

namespace Stump.Server.WorldServer.Game.Arenas
{
    public class ArenaInvitation : Notification
    {
        // FIELDS
        [Variable]
        public static short ArenaInvitationDuration = 60;

        private readonly object m_lock = new object();
        private List<int> m_acceptedIds;
        private bool m_disposed = false;

        // PROPERTIES
        public ArenaPartyCreation RedTeam
        {
            get;
            private set;
        }
        public ArenaPartyCreation BlueTeam
        {
            get;
            private set;
        }

        // CONSTRUCTORS
        public ArenaInvitation(ArenaPartyCreation redTeam, ArenaPartyCreation blueTeam)
        {
            this.m_acceptedIds = new List<int>();

            this.RedTeam = redTeam;
            this.BlueTeam = blueTeam;

            //this.ForEach(new System.Action<ArenaPartyCreation>((ArenaPartyCreation entry) => entry.SendFightProposition(this)));

            Task.Factory.StartNewDelayed(ArenaInvitationDuration * 1000, this.TimerElapsed);
        }

        // METHODS
        public void Accept(WorldClient client)
        {

        }
        public void Deny(WorldClient client)
        {

        }

        public override void Display()
        {

        }

        private void TimerElapsed()
        {
            lock (this.m_lock)
            {

                this.Dispose();
            }
        }

        private void Foreach(Action<ArenaPartyCreation> action)
        {
            lock (this.m_lock)
            {
                action(this.RedTeam);
                action(this.BlueTeam);
            }
        }

        public void Dispose()
        {
            if (!this.m_disposed)
            {
                lock (this.m_lock)
                {
                    if (!this.m_disposed)
                    {
                        //this.Foreach(entry =>
                        //{
                        //    entry.SetArenaInvitation(null);
                        //});
                        //this.m_disposed = true;
                    }
                }
            }
        }
    }
}

using Stump.Core.Extensions;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.Interfaces;
using Stump.Server.WorldServer.Game.Actors.Look;
using Stump.Server.WorldServer.Game.Actors.RolePlay;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Alliances;
using Stump.Server.WorldServer.Game.Dialogs;
using Stump.Server.WorldServer.Game.Dialogs.Prisms;
using Stump.Server.WorldServer.Game.Maps.Cells;
using System;
using System.Collections.Generic;

namespace Stump.Server.WorldServer.Game.Prisms
{
    public class PrismNpc : RolePlayActor, IAutoMovedEntity, IContextDependant, IInteractNpc
    {
        // FIELDS
        private readonly List<IDialog> m_openedDialogs = new List<IDialog>();
        private readonly WorldMapPrismRecord m_record;
        private readonly int m_contextId;
        private ActorLook m_look;

        // PROPERTIES
        public override int Id
        {
            get
            {
                return this.m_contextId;
            }
        }
        public int GlobalId
        {
            get
            {
                return this.m_record.Id;
            }
            protected set
            {
                this.m_record.Id = value;
            }
        }
        public WorldMapPrismRecord Record
        {
            get
            {
                return this.m_record;
            }
        }
        public Alliance Alliance { get; private set; }
        public override ActorLook Look
        {
            get
            {
                return this.m_look ?? this.RefreshLook();
            }
        }
        public DateTime NextMoveDate { get; set; }
        public DateTime LastMoveDate { get; private set; }
        public bool IsDirty { get; private set; }

        // CONSTRUCTORS
        public PrismNpc(int globalId, int contextId, ObjectPosition position, Alliance alliance)
        {
            this.m_contextId = contextId;
            this.Position = position;
            this.Alliance = alliance;
            this.m_record = new WorldMapPrismRecord
            {
                Id = globalId,
                Map = this.Position.Map,
                Cell = (int)this.Position.Cell.Id,
                AllianceId = alliance.Id,
                Date = DateTime.Now
            };
            this.IsDirty = true;
        }
        public PrismNpc(WorldMapPrismRecord record, int contextId)
        {
            this.m_record = record;
            this.m_contextId = contextId;
            if (!record.MapId.HasValue)
            {
                throw new Exception("Prism's map not found");
            }

            this.Position = new ObjectPosition(record.Map, record.Map.Cells[this.m_record.Cell], DirectionsEnum.DIRECTION_EAST);
            this.Alliance = Singleton<AllianceManager>.Instance.TryGetAlliance(this.Record.AllianceId);
        }

        // METHODS
        public ActorLook RefreshLook()
        {
            this.m_look = new ActorLook
            {
                BonesID = 2211
            };
            //if (this.Alliance.Emblem.Template != null)
            //{
            //    this.m_look.AddSkin((short)this.Alliance.Emblem.Template.IconId);
            //    this.m_look.AddSkin((short)this.Alliance.Emblem.Template.SkinId);
            //}
            //this.m_look.AddColor(1, this.Alliance.Emblem.BackgroundColor);
            //this.m_look.AddColor(2, this.Alliance.Emblem.SymbolColor);
            //this.m_look.AddColor(3, this.Alliance.Emblem.BackgroundColor);
            //this.m_look.AddColor(4, this.Alliance.Emblem.SymbolColor);
            //this.m_look.AddColor(5, this.Alliance.Emblem.BackgroundColor);
            //this.m_look.AddColor(6, this.Alliance.Emblem.SymbolColor);
            //this.m_look.AddColor(7, this.Alliance.Emblem.BackgroundColor);
            //this.m_look.AddColor(8, this.Alliance.Emblem.SymbolColor);

            return this.m_look;
        }

        public override GameContextActorInformations GetGameContextActorInformations(Actors.RolePlay.Characters.Character character)
        {
            return new GameRolePlayPrismInformations((int)this.Id, this.Look.GetEntityLook(), this.GetEntityDispositionInformations(), this.GetAlliancePrismInformation());
        }
        public PrismInformation GetAlliancePrismInformation()
        {
            return new AlliancePrismInformation(
                (sbyte)PrismListenEnum.PRISM_LISTEN_MINE, 
                (sbyte)PrismStateEnum.PRISM_STATE_NORMAL, 
                this.Record.Date.GetUnixTimeStamp(), 
                this.Record.Date.GetUnixTimeStamp(),
                0,
                this.Alliance.GetAllianceInformations());
        }
        public PrismInformation GetAllianceInsiderPrismInformation()
        {
            return new AllianceInsiderPrismInformation(
                (sbyte)PrismListenEnum.PRISM_LISTEN_MINE,
                (sbyte)PrismStateEnum.PRISM_STATE_NORMAL,
                this.Record.Date.GetUnixTimeStamp(),
                this.Record.Date.GetUnixTimeStamp(),
                0,
                0,
                0,
                0,
                "Taykyue",
                new uint[0]);
        }

        public void OnDialogOpened(IDialog dialog)
        {
            this.m_openedDialogs.Add(dialog);
        }
        public void OnDialogClosed(IDialog dialog)
        {
            this.m_openedDialogs.Remove(dialog);
        }

        public void InteractWith(NpcActionTypeEnum actionType, Character dialoguer)
        {
            if (this.CanInteractWith(actionType, dialoguer))
            {
                var infoDialog = new PrismInfoDialog(dialoguer, this);
                infoDialog.Open();
            }
        }

        public bool CanInteractWith(NpcActionTypeEnum action, Character dialoguer)
        {
            throw new NotImplementedException();
        }
    }
}

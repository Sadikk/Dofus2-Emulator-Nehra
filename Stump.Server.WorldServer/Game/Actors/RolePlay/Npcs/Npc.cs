using Stump.Core.Cache;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Database.Npcs;
using Stump.Server.WorldServer.Database.Npcs.Actions;
using Stump.Server.WorldServer.Game.Actors.Interfaces;
using Stump.Server.WorldServer.Game.Actors.Look;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Maps.Cells;
using System;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Actors.RolePlay.Npcs
{
	public sealed class Npc : RolePlayActor, IContextDependant, IInteractNpc
	{
		private readonly System.Collections.Generic.List<NpcAction> m_actions = new System.Collections.Generic.List<NpcAction>();
		private readonly ObjectValidator<GameContextActorInformations> m_gameContextActorInformations;
		public event Action<Npc, NpcActionTypeEnum, NpcAction, Character> Interacted;
		public NpcTemplate Template
		{
			get;
			private set;
		}
		public NpcSpawn Spawn
		{
			get;
			private set;
		}
		public override int Id
		{
			get;
			protected set;
		}
		public int TemplateId
		{
			get
			{
				return this.Template.Id;
			}
		}
		public override ActorLook Look
		{
			get;
			set;
		}
		public System.Collections.Generic.List<NpcAction> Actions
		{
			get
			{
				return this.m_actions;
			}
		}
		public Npc(int id, NpcTemplate template, ObjectPosition position, ActorLook look)
		{
			this.Id = id;
			this.Template = template;
			this.Position = position;
			this.Look = look;
			this.m_gameContextActorInformations = new ObjectValidator<GameContextActorInformations>(new Func<GameContextActorInformations>(this.BuildGameContextActorInformations));
			this.m_actions.AddRange(this.Template.Actions);
		}
		public Npc(int id, NpcSpawn spawn) : this(id, spawn.Template, spawn.GetPosition(), spawn.Look)
		{
			this.Spawn = spawn;
		}
		private void OnInteracted(NpcActionTypeEnum actionType, NpcAction action, Character character)
		{
			Action<Npc, NpcActionTypeEnum, NpcAction, Character> interacted = this.Interacted;
			if (interacted != null)
			{
				interacted(this, actionType, action, character);
			}
		}
		public void Refresh()
		{
			this.m_gameContextActorInformations.Invalidate();
			if (base.Map != null)
			{
				base.Map.Refresh(this);
			}
		}
		public void InteractWith(NpcActionTypeEnum actionType, Character dialoguer)
		{
			if (this.CanInteractWith(actionType, dialoguer))
			{
				NpcAction npcAction = this.Actions.First((NpcAction entry) => entry.ActionType == actionType && entry.CanExecute(this, dialoguer));
				npcAction.Execute(this, dialoguer);
				this.OnInteracted(actionType, npcAction, dialoguer);
			}
		}
		public bool CanInteractWith(NpcActionTypeEnum action, Character dialoguer)
		{
			return !(dialoguer.Map != this.Position.Map) && this.Actions.Count > 0 && this.Actions.Any((NpcAction entry) => entry.ActionType == action && entry.CanExecute(this, dialoguer));
		}
		public void SpeakWith(Character dialoguer)
		{
			if (this.CanInteractWith(NpcActionTypeEnum.ACTION_TALK, dialoguer))
			{
				this.InteractWith(NpcActionTypeEnum.ACTION_TALK, dialoguer);
			}
		}
		public override string ToString()
		{
			return string.Format("{0} ({1}) [{2}]", this.Template.Name, this.Id, this.TemplateId);
		}
		private GameContextActorInformations BuildGameContextActorInformations()
		{
            return new GameRolePlayNpcInformations(this.Id, this.Look.GetEntityLook(), this.GetEntityDispositionInformations(), (ushort)this.Template.Id, this.Template.Gender != 0u, (ushort)this.Template.SpecialArtworkId);
		}
		public override GameContextActorInformations GetGameContextActorInformations(Character character)
		{
			return this.m_gameContextActorInformations;
		}
	}
}

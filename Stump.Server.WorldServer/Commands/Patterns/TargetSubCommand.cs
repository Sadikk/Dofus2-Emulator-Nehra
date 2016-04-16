using Stump.Server.BaseServer.Commands;
using Stump.Server.WorldServer.Commands.Trigger;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using System;
namespace Stump.Server.WorldServer.Commands.Commands.Patterns
{
	public abstract class TargetSubCommand : SubCommand
	{
		protected void AddTargetParameter(bool optional = false, string description = "Defined target")
		{
			base.AddParameter<Character[]>("target", "t", description, null, optional, ParametersConverter.CharactersConverter);
		}
		public Character[] GetTargets(TriggerBase trigger)
		{
			Character[] array = null;
			if (trigger.IsArgumentDefined("target"))
			{
				array = trigger.Get<Character[]>("target");
			}
			else
			{
				if (trigger is GameTrigger)
				{
					array = new Character[]
					{
						(trigger as GameTrigger).Character
					};
				}
			}
			if (array == null)
			{
				throw new System.Exception("Target is not defined");
			}
			if (array.Length == 0)
			{
				throw new System.Exception("No target found");
			}
			return array;
		}
		public Character GetTarget(TriggerBase trigger)
		{
			Character[] targets = this.GetTargets(trigger);
			if (targets.Length > 1)
			{
				throw new System.Exception("Only 1 target allowed");
			}
			return targets[0];
		}
	}
}

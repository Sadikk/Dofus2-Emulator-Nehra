using Stump.Core.Attributes;
using Stump.Core.Timers;
using Stump.Server.WorldServer.Game.Actors.Fight;
using Stump.Server.WorldServer.Handlers.Context;
using System;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Fights
{
	public class ReadyChecker
	{
		[Variable(true)]
		public static readonly int CheckTimeout = 5000;
		private readonly System.Collections.Generic.List<CharacterFighter> m_fighters;
		private readonly Fight m_fight;
		private bool m_started;
		private TimedTimerEntry m_timer;
		public event System.Action<ReadyChecker> Success;
		public event Action<ReadyChecker, CharacterFighter[]> Timeout;
		private void NotifySuccess()
		{
			System.Action<ReadyChecker> success = this.Success;
			if (success != null)
			{
				success(this);
			}
		}
		private void NotifyTimeout(CharacterFighter[] laggers)
		{
			Action<ReadyChecker, CharacterFighter[]> timeout = this.Timeout;
			if (timeout != null)
			{
				timeout(this, laggers);
			}
		}
		public ReadyChecker(Fight fight, System.Collections.Generic.IEnumerable<CharacterFighter> actors)
		{
			this.m_fight = fight;
			this.m_fighters = actors.ToList<CharacterFighter>();
		}
		public void Start()
		{
			if (!this.m_started)
			{
				this.m_started = true;
				if (this.m_fighters.Count <= 0)
				{
					this.NotifySuccess();
				}
				else
				{
					foreach (CharacterFighter current in this.m_fighters)
					{
						ContextHandler.SendGameFightTurnReadyRequestMessage(current.Character.Client, this.m_fight.TimeLine.Current);
					}
					this.m_timer = this.m_fight.Map.Area.CallDelayed(ReadyChecker.CheckTimeout, new Action(this.TimedOut));
				}
			}
		}
		public void Cancel()
		{
			this.m_started = false;
			if (this.m_timer != null)
			{
				this.m_timer.Dispose();
			}
		}
		public void ToggleReady(CharacterFighter actor, bool ready = true)
		{
			if (this.m_started)
			{
				if (ready && this.m_fighters.Contains(actor))
				{
					this.m_fighters.Remove(actor);
				}
				else
				{
					if (!ready)
					{
						this.m_fighters.Add(actor);
					}
				}
				if (this.m_fighters.Count == 0)
				{
					if (this.m_timer != null)
					{
						this.m_timer.Dispose();
					}
					this.NotifySuccess();
				}
			}
		}
		private void TimedOut()
		{
			if (this.m_started && this.m_fighters.Count > 0)
			{
				this.NotifyTimeout(this.m_fighters.ToArray());
			}
		}

		public static ReadyChecker RequestCheck(Fight fight, Action success, System.Action<CharacterFighter[]> fail)
		{
			ReadyChecker readyChecker = new ReadyChecker(fight, fight.GetAllFighters<CharacterFighter>((CharacterFighter entry) => !entry.HasLeft() && !entry.IsDisconnected).ToList<CharacterFighter>());
			readyChecker.Success += delegate(ReadyChecker chk)
			{
				success();
			};
			readyChecker.Timeout += delegate(ReadyChecker chk, CharacterFighter[] laggers)
			{
				fail(laggers);
			};
			readyChecker.Start();

			return readyChecker;
		}
	}
}

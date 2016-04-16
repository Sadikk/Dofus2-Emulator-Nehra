using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Dialogs
{
	public abstract class RequestBox
	{
		public Character Source
		{
			get;
			protected set;
		}
		public Character Target
		{
			get;
			protected set;
		}
		protected RequestBox(Character source, Character target)
		{
			this.Source = source;
			this.Target = target;
		}
		public void Open()
		{
			this.Source.OpenRequestBox(this);
			this.Target.OpenRequestBox(this);
			this.OnOpen();
		}
		protected virtual void OnOpen()
		{
		}
		public void Accept()
		{
			this.OnAccept();
			this.Close();
		}
		protected virtual void OnAccept()
		{
		}
		public void Deny()
		{
			this.OnDeny();
			this.Close();
		}
		protected virtual void OnDeny()
		{
		}
		public void Cancel()
		{
			this.OnCancel();
			this.Close();
		}
		protected virtual void OnCancel()
		{
		}
		protected void Close()
		{
			this.Source.ResetRequestBox();
			this.Target.ResetRequestBox();
		}
	}
}

using Stump.DofusProtocol.Types;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Fights.Results.Data
{
	public class FightPvpData : FightResultAdditionalData
	{
		public byte Grade
		{
			get;
			set;
		}
		public ushort MinHonorForGrade
		{
			get;
			set;
		}
		public ushort MaxHonorForGrade
		{
			get;
			set;
		}
		public ushort Honor
		{
			get;
			set;
		}
		public short HonorDelta
		{
			get;
			set;
		}
		public ushort Dishonor
		{
			get;
			set;
		}
		public short DishonorDelta
		{
			get;
			set;
		}
		public FightPvpData(Character character) : base(character)
		{
		}
		public override Stump.DofusProtocol.Types.FightResultAdditionalData GetFightResultAdditionalData()
		{
            return new FightResultPvpData(this.Grade, (ushort)this.MinHonorForGrade, (ushort)this.MaxHonorForGrade, (ushort)this.Honor, this.HonorDelta);
		}
		public override void Apply()
		{
			if (this.HonorDelta > 0)
			{
				base.Character.AddHonor((ushort)this.HonorDelta);
			}
			else
			{
				if (this.HonorDelta < 0)
				{
					base.Character.SubHonor((ushort)(-(ushort)this.HonorDelta));
				}
			}
			if (this.HonorDelta > 0)
			{
				base.Character.AddDishonor((ushort)this.DishonorDelta);
			}
			else
			{
				if (this.HonorDelta < 0)
				{
					base.Character.SubDishonor((ushort)(-(ushort)this.DishonorDelta));
				}
			}
		}
	}
}

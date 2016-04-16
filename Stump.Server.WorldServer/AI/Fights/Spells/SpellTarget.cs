using Stump.Server.WorldServer.Game.Actors.Fight;

namespace Stump.Server.WorldServer.AI.Fights.Spells
{
	public class SpellTarget
	{
		public double MinFire;
		public double MaxFire;
		public double MinWater;
		public double MaxWater;
		public double MinEarth;
		public double MaxEarth;
		public double MinAir;
		public double MaxAir;
		public double MinNeutral;
		public double MaxNeutral;
		public double MinHeal;
		public double MaxHeal;
		public double Fire
		{
			get
			{
				return (this.MinFire + this.MaxFire) / 2.0;
			}
		}
		public double Air
		{
			get
			{
				return (this.MinAir + this.MaxAir) / 2.0;
			}
		}
		public double Earth
		{
			get
			{
				return (this.MinEarth + this.MaxEarth) / 2.0;
			}
		}
		public double Water
		{
			get
			{
				return (this.MinWater + this.MaxWater) / 2.0;
			}
		}
		public double Neutral
		{
			get
			{
				return (this.MinEarth + this.MaxEarth) / 2.0;
			}
		}
		public double Heal
		{
			get
			{
				return (this.MinHeal + this.MaxHeal) / 2.0;
			}
		}
		public double Curse
		{
			get;
			set;
		}
		public double Boost
		{
			get;
			set;
		}
		public double MinDamage
		{
			get
			{
				return this.MinFire + this.MinAir + this.MinEarth + this.MinWater + this.MinNeutral + this.MaxHeal + this.Curse + this.Boost;
			}
		}
		public double MaxDamage
		{
			get
			{
				return this.MaxFire + this.MaxAir + this.MaxEarth + this.MaxWater + this.MaxNeutral + this.MinHeal + this.Curse + this.Boost;
			}
		}
		public double Damage
		{
			get
			{
				return (this.MinDamage + this.MaxDamage) / 2.0;
			}
		}
		public FightActor Target
		{
			get;
			set;
		}
		public void Add(SpellTarget dmg)
		{
			this.MinFire += dmg.MinFire;
			this.MaxFire += dmg.MaxFire;
			this.MinWater += dmg.MinWater;
			this.MaxWater += dmg.MaxWater;
			this.MinEarth += dmg.MinEarth;
			this.MaxEarth += dmg.MaxEarth;
			this.MinAir += dmg.MinAir;
			this.MaxAir += dmg.MaxAir;
			this.MinNeutral += dmg.MinNeutral;
			this.MaxNeutral += dmg.MaxNeutral;
			this.MinHeal += dmg.MinHeal;
			this.MaxHeal += dmg.MaxHeal;
			this.Curse += dmg.Curse;
			this.Boost += dmg.Boost;
		}
		public void Multiply(double ratio)
		{
			this.MinFire *= ratio;
			this.MaxFire *= ratio;
			this.MinWater *= ratio;
			this.MaxWater *= ratio;
			this.MinEarth *= ratio;
			this.MaxEarth *= ratio;
			this.MinAir *= ratio;
			this.MaxAir *= ratio;
			this.MinNeutral *= ratio;
			this.MaxNeutral *= ratio;
			this.MinHeal *= ratio;
			this.MaxHeal *= ratio;
			this.Curse *= ratio;
			this.Boost *= ratio;
		}
	}
}

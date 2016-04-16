using Stump.Core.Attributes;
using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Database;
using Stump.Server.BaseServer.Initialization;
using Stump.Server.WorldServer.Database.Breeds;
using System;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Breeds
{
	public class BreedManager : DataManager<BreedManager>
	{
		[Variable]
		public static readonly System.Collections.Generic.List<PlayableBreedEnum> AvailableBreeds = new System.Collections.Generic.List<PlayableBreedEnum>
		{
			PlayableBreedEnum.Feca,
			PlayableBreedEnum.Osamodas,
			PlayableBreedEnum.Enutrof,
			PlayableBreedEnum.Sram,
			PlayableBreedEnum.Xelor,
			PlayableBreedEnum.Ecaflip,
			PlayableBreedEnum.Eniripsa,
			PlayableBreedEnum.Iop,
			PlayableBreedEnum.Cra,
			PlayableBreedEnum.Sadida,
			PlayableBreedEnum.Sacrieur,
			PlayableBreedEnum.Pandawa,
			PlayableBreedEnum.Roublard,
			PlayableBreedEnum.Zobal,
            PlayableBreedEnum.Steamer,
            PlayableBreedEnum.Eliotrope
		};
		private System.Collections.Generic.Dictionary<int, Breed> m_breeds = new System.Collections.Generic.Dictionary<int, Breed>();
		private System.Collections.Generic.Dictionary<int, Head> m_heads = new System.Collections.Generic.Dictionary<int, Head>();
		public ushort AvailableBreedsFlags
		{
			get
			{
                return (ushort)BreedManager.AvailableBreeds.Aggregate(0, (int current, PlayableBreedEnum breedEnum) => current | 1 << breedEnum - PlayableBreedEnum.Feca);
			}
		}
		[Initialization(InitializationPass.Third)]
		public override void Initialize()
		{
			base.Initialize();
			foreach (Breed current in base.Database.Query<Breed, BreedItem, BreedSpell, Breed>(new Func<Breed, BreedItem, BreedSpell, Breed>(new BreedRelator().Map), BreedRelator.FetchQuery, new object[0]))
			{
				this.m_breeds.Add(current.Id, current);
			}
			this.m_heads = base.Database.Query<Head>(HeadRelator.FetchQuery, new object[0]).ToDictionary((Head x) => x.Id);
		}
		public Breed GetBreed(PlayableBreedEnum breed)
		{
			return this.GetBreed((int)breed);
		}
		public Breed GetBreed(int id)
		{
			Breed result;
			this.m_breeds.TryGetValue(id, out result);
			return result;
		}
		public Head GetHead(int id)
		{
			Head result;
			this.m_heads.TryGetValue(id, out result);
			return result;
		}
		public bool IsBreedAvailable(int id)
		{
            return true;
			//return BreedManager.AvailableBreeds.Contains((PlayableBreedEnum)id);
		}
		public void AddBreed(Breed breed, bool defineId = false)
		{
			if (defineId)
			{
				int id = this.m_breeds.Keys.Max() + 1;
				breed.Id = id;
			}
			if (this.m_breeds.ContainsKey(breed.Id))
			{
				throw new System.Exception(string.Format("Breed with id {0} already exists", breed.Id));
			}
			this.m_breeds.Add(breed.Id, breed);
			base.Database.Insert(breed);
		}
		public void RemoveBreed(Breed breed)
		{
			this.RemoveBreed(breed.Id);
		}
		public void RemoveBreed(int id)
		{
			if (!this.m_breeds.ContainsKey(id))
			{
				throw new System.Exception(string.Format("Breed with id {0} does not exist", id));
			}
			Breed poco = this.m_breeds[id];
			this.m_breeds.Remove(id);
			base.Database.Delete(poco);
		}
	}
}

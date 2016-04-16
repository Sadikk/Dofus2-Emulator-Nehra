using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;

namespace Stump.Server.WorldServer.Game.Conditions.Criterions
{
	public class MapCharactersCriterion : Criterion
	{
		public const string Identifier = "MK";
		public int? MapId
		{
			get;
			set;
		}
		public int CharactersCount
		{
			get;
			set;
		}
		public override bool Eval(Character character)
		{
			int count = character.Map.Clients.Count;
			bool result;
			if (this.MapId.HasValue)
			{
				result = (base.Compare<int>(character.Map.Id, this.MapId.Value) && base.Compare<int>(count, this.CharactersCount));
			}
			else
			{
				result = base.Compare<int>(count, this.CharactersCount);
			}
			return result;
		}
		public override void Build()
		{
			string[] array = base.Literal.Split(new char[]
			{
				','
			});
			if (array.Length == 1)
			{
				this.MapId = null;
				int charactersCount;
				if (!int.TryParse(base.Literal, out charactersCount))
				{
					throw new System.Exception(string.Format("Cannot build MapCharactersCriterion, {0} is not a valid characters count", base.Literal));
				}
				this.CharactersCount = charactersCount;
			}
			else
			{
				if (array.Length != 2)
				{
					throw new System.Exception(string.Format("Cannot build MapCharactersCriterion, {0} is mal formatted : 'id,count' or 'count'", array[1]));
				}
				int value;
				if (!int.TryParse(array[0], out value))
				{
					throw new System.Exception(string.Format("Cannot build MapCharactersCriterion, {0} is not a valid map id", array[0]));
				}
				this.MapId = new int?(value);
				int charactersCount;
				if (!int.TryParse(array[1], out charactersCount))
				{
					throw new System.Exception(string.Format("Cannot build MapCharactersCriterion, {0} is not a valid characters count", array[1]));
				}
				this.CharactersCount = charactersCount;
			}
		}
		public override string ToString()
		{
			return base.FormatToString("MK");
		}
	}
}

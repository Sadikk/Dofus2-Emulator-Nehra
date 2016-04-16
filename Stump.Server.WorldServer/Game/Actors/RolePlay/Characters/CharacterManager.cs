using NLog;
using Stump.Core.Attributes;
using Stump.Core.IO;
using Stump.Core.Reflection;
using Stump.DofusProtocol.Enums;
using Stump.ORM;
using Stump.Server.BaseServer.Database;
using Stump.Server.BaseServer.IPC;
using Stump.Server.BaseServer.IPC.Messages;
using Stump.Server.WorldServer.Core.IPC;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Database.Breeds;
using Stump.Server.WorldServer.Database.Characters;
using Stump.Server.WorldServer.Database.Items;
using Stump.Server.WorldServer.Database.Shortcuts;
using Stump.Server.WorldServer.Game.Actors.Look;
using Stump.Server.WorldServer.Game.Breeds;
using Stump.Server.WorldServer.Game.Guilds;
using Stump.Server.WorldServer.Game.Spells;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
namespace Stump.Server.WorldServer.Game.Actors.RolePlay.Characters
{
	public class CharacterManager : DataManager<CharacterManager>
	{
		private const string Vowels = "aeiouy";
		private const string Consonants = "bcdfghjklmnpqrstvwxz";
		private static readonly Logger logger = LogManager.GetCurrentClassLogger();
		[Variable(true)]
		public static uint MaxCharacterSlot = 5u;
		private static readonly Regex m_nameCheckerRegex = new Regex("^[A-Z][a-z]{2,9}(?:-[A-Z][a-z]{2,9}|[a-z]{1,10})$", RegexOptions.Compiled);
		public event Action<CharacterRecord> CreatingCharacter;
		private void OnCreatingCharacter(CharacterRecord record)
		{
			Action<CharacterRecord> creatingCharacter = this.CreatingCharacter;
			if (creatingCharacter != null)
			{
				creatingCharacter(record);
			}
		}
		public CharacterRecord GetCharacterById(int id)
		{
			return Database.Query<CharacterRecord>(string.Format(CharacterRelator.FetchById, id), new object[0]).FirstOrDefault<CharacterRecord>();
		}
		public CharacterRecord GetCharacterByName(string name)
		{
			return Database.Query<CharacterRecord>(CharacterRelator.FetchByName, new object[]
			{
				name
			}).FirstOrDefault<CharacterRecord>();
		}
		public List<CharacterRecord> GetCharactersByAccount(WorldClient client)
		{
			List<CharacterRecord> result;
			if (client.Account.Characters == null || client.Account.Characters.Count == 0)
			{
				result = new List<CharacterRecord>();
			}
			else
			{
				List<int> list = (
					from x in client.Account.Characters
					where x.WorldId == WorldServer.ServerInformation.Id
					select x.CharacterId).ToList<int>();
				if (list.Count == 0)
				{
					result = new List<CharacterRecord>();
				}
				else
				{
					List<CharacterRecord> characters = Database.Fetch<CharacterRecord>(string.Format(CharacterRelator.FetchByMultipleId, list.ToCSV(",")), new object[0]);
					if (characters.Count == client.Account.Characters.Count)
					{
						result = characters;
					}
					else
					{
						foreach (int current in 
							from id in list
							where characters.All((CharacterRecord character) => character.Id != id)
							where IPCAccessor.Instance.IsConnected
							select id)
						{
							IPCAccessor.Instance.Send(new DeleteCharacterMessage(client.Account.Id, current));
						}
						result = characters;
					}
				}
			}
			return result;
		}
		public bool DoesNameExist(string name)
		{
			return Database.ExecuteScalar<object>("SELECT 1 FROM characters WHERE Name=@0", new object[]
			{
				name
			}) != null;
		}
		public void CreateCharacter(WorldClient client, string name, sbyte breedId, bool sex, IEnumerable<int> colors, int headId, Action successCallback, Action<CharacterCreationResultEnum> failCallback)
		{
			if ((long)client.Characters.Count >= (long)((ulong)MaxCharacterSlot) && client.UserGroup.Role <= RoleEnum.Player)
			{
				failCallback(CharacterCreationResultEnum.ERR_TOO_MANY_CHARACTERS);
			}
			else
			{
				if (this.DoesNameExist(name))
				{
					failCallback(CharacterCreationResultEnum.ERR_NAME_ALREADY_EXISTS);
				}
				else
				{
					if (!m_nameCheckerRegex.IsMatch(name))
					{
						failCallback(CharacterCreationResultEnum.ERR_INVALID_NAME);
					}
					else
					{
						Breed breed = Singleton<BreedManager>.Instance.GetBreed((int)breedId);
						if (breed == null || !client.Account.CanUseBreed((int)breedId) || !Singleton<BreedManager>.Instance.IsBreedAvailable((int)breedId))
						{
							failCallback(CharacterCreationResultEnum.ERR_NOT_ALLOWED);
						}
						else
						{
							Head head = Singleton<BreedManager>.Instance.GetHead(headId);
							if ((ulong)head.Breed != (ulong)((long)breedId) || head.Gender == 1u != sex)
							{
								failCallback(CharacterCreationResultEnum.ERR_NO_REASON);
							}
							else
							{
								ActorLook look = breed.GetLook(sex ? SexTypeEnum.SEX_FEMALE : SexTypeEnum.SEX_MALE, true);
								int num = 0;
								uint[] array = (!sex) ? breed.MaleColors : breed.FemaleColors;
								foreach (int current in colors)
								{
									if (array.Length > num)
									{
										look.AddColor(num + 1, (current == -1) ? Color.FromArgb((int)array[num]) : Color.FromArgb(current));
									}
									num++;
								}
								short[] skins = head.Skins;
								for (int i = 0; i < skins.Length; i++)
								{
									short skin = skins[i];
									look.AddSkin(skin);
								}
								CharacterRecord record;
								using (Transaction transaction = Database.GetTransaction())
								{
									record = new CharacterRecord(breed)
									{
										Experience = Singleton<ExperienceManager>.Instance.GetCharacterLevelExperience(breed.StartLevel),
										Name = name,
										Sex = sex ? SexTypeEnum.SEX_FEMALE : SexTypeEnum.SEX_MALE,
										Head = headId,
										EntityLook = look,
										CreationDate = DateTime.Now,
										LastUsage = new DateTime?(DateTime.Now),
										AlignmentSide = AlignmentSideEnum.ALIGNMENT_NEUTRAL,
										WarnOnConnection = true,
										WarnOnLevel = true
									};
									Database.Insert(record);
									IOrderedEnumerable<BreedSpell> source = 
										from spell in breed.Spells
										where spell.ObtainLevel <= (int)breed.StartLevel
										orderby spell.ObtainLevel, spell.Spell
										select spell;
									int num2 = 0;
									foreach (CharacterSpellRecord current2 in 
										from learnableSpell in source
										select Singleton<SpellManager>.Instance.CreateSpellRecord(record, Singleton<SpellManager>.Instance.GetSpellTemplate(learnableSpell.Spell)))
									{
										Database.Insert(current2);
										SpellShortcut poco = new SpellShortcut(record, num2, (short)current2.SpellId);
										Database.Insert(poco);
										num2++;
									}
									foreach (PlayerItemRecord current3 in 
										from startItem in breed.Items
										select startItem.GenerateItemRecord(record))
									{
										Database.Insert(current3);
									}
									this.OnCreatingCharacter(record);
									if (client.Characters == null)
									{
										client.Characters = new List<CharacterRecord>();
									}
									client.Characters.Insert(0, record);
									transaction.Complete();
								}
								IPCAccessor.Instance.SendRequest(new AddCharacterMessage(client.Account.Id, record.Id), delegate(CommonOKMessage x)
								{
									successCallback();
								}, delegate(IPCErrorMessage x)
								{
									this.Database.Delete(record);
									failCallback(CharacterCreationResultEnum.ERR_NO_REASON);
								});
								logger.Debug("Character {0} created", record.Name);
							}
						}
					}
				}
			}
		}
		public void DeleteCharacterOnAccount(CharacterRecord character, WorldClient client)
		{
			GuildMember guildMember = Singleton<GuildManager>.Instance.TryGetGuildMember(character.Id);
			if (guildMember != null)
			{
				Singleton<GuildManager>.Instance.DeleteGuildMember(guildMember);
			}
			Database.Delete(character);
			client.Characters.Remove(character);
			IPCAccessor.Instance.Send(new DeleteCharacterMessage(client.Account.Id, character.Id));
		}
		public string GenerateName()
		{
			string text;
			do
			{
				Random random = new Random();
				int num = random.Next(5, 10);
				text = string.Empty;
				bool flag = random.Next(0, 2) == 0;
				text += GetChar(flag, random).ToString(CultureInfo.InvariantCulture).ToUpper();
				flag = !flag;
				for (int i = 0; i < num - 1; i++)
				{
					text += GetChar(flag, random);
					flag = !flag;
				}
			}
			while (this.DoesNameExist(text));
			return text;
		}
		private static char GetChar(bool vowel, Random rand)
		{
			return vowel ? RandomVowel(rand) : RandomConsonant(rand);
		}
		private static char RandomVowel(Random rand)
		{
			return "aeiouy"[rand.Next(0, "aeiouy".Length - 1)];
		}
		private static char RandomConsonant(Random rand)
		{
			return "bcdfghjklmnpqrstvwxz"[rand.Next(0, "bcdfghjklmnpqrstvwxz".Length - 1)];
		}
	}
}

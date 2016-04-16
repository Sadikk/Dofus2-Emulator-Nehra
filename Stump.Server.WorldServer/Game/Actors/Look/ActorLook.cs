using Stump.Core.Cache;
using Stump.Core.Extensions;
using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Types;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

namespace Stump.Server.WorldServer.Game.Actors.Look
{
	public class ActorLook
	{
        // FIELDS
		private const short PET_SIZE = 75;

        private readonly ObjectValidator<EntityLook> m_entityLook;
		private System.Collections.Generic.List<short> m_scales = new System.Collections.Generic.List<short>();
		private System.Collections.Generic.List<short> m_skins = new System.Collections.Generic.List<short>();
		private System.Collections.Generic.List<SubActorLook> m_subLooks = new System.Collections.Generic.List<SubActorLook>();
		private System.Collections.Generic.Dictionary<int, Color> m_colors = new System.Collections.Generic.Dictionary<int, Color>();
		private short m_bonesID;

        // PROPERTIES
		public short BonesID
		{
			get
			{
				return this.m_bonesID;
			}
			set
			{
				this.m_bonesID = value;
				this.m_entityLook.Invalidate();
			}
		}
		public System.Collections.ObjectModel.ReadOnlyCollection<short> Skins
		{
			get
			{
				return this.m_skins.AsReadOnly();
			}
		}
		public System.Collections.ObjectModel.ReadOnlyCollection<short> Scales
		{
			get
			{
				return this.m_scales.AsReadOnly();
			}
		}
		public System.Collections.ObjectModel.ReadOnlyCollection<SubActorLook> SubLooks
		{
			get
			{
				return this.m_subLooks.AsReadOnly();
			}
		}
		public ActorLook PetLook
		{
			get
			{
				var subActorLook = this.m_subLooks.FirstOrDefault((SubActorLook x) => x.BindingCategory == SubEntityBindingPointCategoryEnum.HOOK_POINT_CATEGORY_PET);
				return (subActorLook != null) ? subActorLook.Look : null;
			}
		}
		public ActorLook AuraLook
		{
			get
			{
				var subActorLook = this.m_subLooks.FirstOrDefault((SubActorLook x) => x.BindingCategory == SubEntityBindingPointCategoryEnum.HOOK_POINT_CATEGORY_BASE_FOREGROUND);
				return (subActorLook != null) ? subActorLook.Look : null;
			}
		}
		public ReadOnlyDictionary<int, Color> Colors
		{
			get
			{
				return new ReadOnlyDictionary<int, Color>(this.m_colors);
			}
		}
		public ObjectValidator<EntityLook> EntityLookValidator
		{
			get
			{
				return this.m_entityLook;
			}
		}

        // CONSTRUCTORS
		public ActorLook()
		{
			this.m_entityLook = new ObjectValidator<EntityLook>(new Func<EntityLook>(this.BuildEntityLook));
		}
		public ActorLook(short bones, System.Collections.Generic.IEnumerable<short> skins, System.Collections.Generic.Dictionary<int, Color> indexedColors, System.Collections.Generic.IEnumerable<short> scales, System.Collections.Generic.IEnumerable<SubActorLook> subLooks) : this()
		{
			this.m_bonesID = bones;
			this.m_skins = skins.ToList<short>();
			this.m_colors = indexedColors;
			this.m_scales = scales.ToList<short>();
			this.m_subLooks = subLooks.ToList<SubActorLook>();
		}
		
        // METHODS
        public void SetSkins(params short[] skins)
		{
			this.m_skins = skins.ToList<short>();
			this.m_entityLook.Invalidate();
		}
		public void AddSkin(short skin)
		{
			this.m_skins.Add(skin);
			this.m_entityLook.Invalidate();
		}

		public void SetScales(params short[] scales)
		{
			this.m_scales = scales.ToList<short>();
			this.m_entityLook.Invalidate();
		}
		public void AddScale(short scale)
		{
			this.m_scales.Add(scale);
			this.m_entityLook.Invalidate();
		}

        public void SetColors(params Color[] colors)
        {
            var index = 1;
            this.m_colors = colors.ToDictionary((Color x) => index++);
            this.m_entityLook.Invalidate();
        }
        public void SetColors(int[] indexes, Color[] colors)
        {
            if (indexes.Length != colors.Length)
            {
                throw new System.ArgumentException("indexes.Length != colors.Length");
            }
            this.m_colors.Clear();
            for (var i = 0; i < indexes.Length; i++)
            {
                this.m_colors.Add(indexes[i], colors[i]);
            }
            this.m_entityLook.Invalidate();
        }
		public void AddColor(int index, Color color)
		{
			this.m_colors.Add(index, color);
			this.m_entityLook.Invalidate();
		}

        public void SetSubLooks(params SubActorLook[] subLooks)
        {
            foreach (var current in this.m_subLooks)
            {
                current.SubEntityValidator.ObjectInvalidated -= new System.Action<ObjectValidator<SubEntity>>(this.OnSubEntityInvalidated);
            }
            this.m_subLooks = subLooks.ToList<SubActorLook>();
            this.m_entityLook.Invalidate();
            foreach (var current in this.m_subLooks)
            {
                current.SubEntityValidator.ObjectInvalidated += new System.Action<ObjectValidator<SubEntity>>(this.OnSubEntityInvalidated);
            }
        }
		public void AddSubLook(SubActorLook subLook)
		{
			this.m_subLooks.Add(subLook);
			this.m_entityLook.Invalidate();
			subLook.SubEntityValidator.ObjectInvalidated += new System.Action<ObjectValidator<SubEntity>>(this.OnSubEntityInvalidated);
		}

		public void SetPetSkin(short skin)
		{
			var actorLook = this.PetLook;
			this.AddSubLook(new SubActorLook(0, SubEntityBindingPointCategoryEnum.HOOK_POINT_CATEGORY_PET, actorLook = new ActorLook()));
			actorLook.SetScales(new short[]
			{
				75
			});
			actorLook.BonesID = skin;
		}
		public void RemovePets()
		{
			this.m_subLooks.RemoveAll((SubActorLook x) => x.BindingCategory == SubEntityBindingPointCategoryEnum.HOOK_POINT_CATEGORY_PET);
			this.m_entityLook.Invalidate();
		}

		public void SetAuraSkin(short skin)
		{
			var actorLook = this.AuraLook;
			if (actorLook == null)
			{
				this.AddSubLook(new SubActorLook(0, SubEntityBindingPointCategoryEnum.HOOK_POINT_CATEGORY_BASE_FOREGROUND, actorLook = new ActorLook()));
			}
			actorLook.BonesID = skin;
		}
		public void RemoveAuras()
		{
			this.m_subLooks.RemoveAll((SubActorLook x) => x.BindingCategory == SubEntityBindingPointCategoryEnum.HOOK_POINT_CATEGORY_BASE_FOREGROUND);
			this.m_entityLook.Invalidate();
		}

        public void ChangeScale(short scale)
        {
            this.SetScales(scale);
        }

		private void OnSubEntityInvalidated(ObjectValidator<SubEntity> obj)
		{
			this.m_entityLook.Invalidate();
		}

		private EntityLook BuildEntityLook()
		{
			return new EntityLook((ushort)this.BonesID, this.Skins.Select(entry => (ushort)entry), (
				from x in this.Colors
				select x.Key << 24 | (x.Value.ToArgb() & 16777215)).ToArray<int>(), this.Scales, (
				from x in this.SubLooks
				select x.GetSubEntity()).ToArray<SubEntity>());
		}

		public EntityLook GetEntityLook()
		{
			return this.m_entityLook;
		}
		public ActorLook Clone()
		{
			var actorLook = new ActorLook();
			actorLook.BonesID = this.m_bonesID;
			actorLook.m_colors = this.m_colors.ToDictionary((System.Collections.Generic.KeyValuePair<int, Color> x) => x.Key, (System.Collections.Generic.KeyValuePair<int, Color> x) => x.Value);
			actorLook.m_skins = this.m_skins.ToList<short>();
			actorLook.m_scales = this.m_scales.ToList<short>();
			actorLook.m_subLooks = (
				from x in this.m_subLooks
				select new SubActorLook(x.BindingIndex, x.BindingCategory, x.Look.Clone())).ToList<SubActorLook>();
			return actorLook;
		}

		public override string ToString()
		{
			var stringBuilder = new System.Text.StringBuilder();
			stringBuilder.Append("{");
			var num = 0;
			stringBuilder.Append(this.BonesID);
			if (this.Skins == null || !this.Skins.Any<short>())
			{
				num++;
			}
			else
			{
				stringBuilder.Append("|".ConcatCopy(num + 1));
				num = 0;
				stringBuilder.Append(string.Join<short>(",", this.Skins));
			}
			if (this.Colors == null || !this.Colors.Any<System.Collections.Generic.KeyValuePair<int, Color>>())
			{
				num++;
			}
			else
			{
				stringBuilder.Append("|".ConcatCopy(num + 1));
				num = 0;
				stringBuilder.Append(string.Join(",", 
					from entry in this.Colors
					select entry.Key + "=" + entry.Value.ToArgb()));
			}
			if (this.Scales == null || !this.Scales.Any<short>())
			{
				num++;
			}
			else
			{
				stringBuilder.Append("|".ConcatCopy(num + 1));
				num = 0;
				stringBuilder.Append(string.Join<short>(",", this.Scales));
			}
			if (this.SubLooks == null || !this.SubLooks.Any<SubActorLook>())
			{
				num++;
			}
			else
			{
				stringBuilder.Append("|".ConcatCopy(num + 1));
				stringBuilder.Append(string.Join<SubActorLook>(",", 
					from entry in this.SubLooks
					select entry));
			}
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}

		public static ActorLook Parse(string str)
		{
			if (string.IsNullOrEmpty(str) || str[0] != '{')
			{
				throw new System.Exception("Incorrect EntityLook format : " + str);
			}
			var i = 1;
			var num = str.IndexOf('|');
			if (num == -1)
			{
				num = str.IndexOf("}");
				if (num == -1)
				{
					throw new System.Exception("Incorrect EntityLook format : " + str);
				}
			}
			var bones = short.Parse(str.Substring(i, num - i));
			i = num + 1;
			var skins = new short[0];
			if ((num = str.IndexOf('|', i)) != -1 || (num = str.IndexOf('}', i)) != -1)
			{
				skins = ActorLook.ParseCollection<short>(str.Substring(i, num - i), new Func<string, short>(short.Parse));
				i = num + 1;
			}
			var source = new Tuple<int, int>[0];
			if ((num = str.IndexOf('|', i)) != -1 || (num = str.IndexOf('}', i)) != -1)
			{
				source = ActorLook.ParseCollection<Tuple<int, int>>(str.Substring(i, num - i), new Func<string, Tuple<int, int>>(ActorLook.ParseIndexedColor));
				i = num + 1;
			}
			var scales = new short[0];
			if ((num = str.IndexOf('|', i)) != -1 || (num = str.IndexOf('}', i)) != -1)
			{
				scales = ActorLook.ParseCollection<short>(str.Substring(i, num - i), new Func<string, short>(short.Parse));
				i = num + 1;
			}
			var list = new System.Collections.Generic.List<SubActorLook>();
			while (i < str.Length)
			{
				var num2 = str.IndexOf('@', i, 3);
				var num3 = str.IndexOf('=', num2 + 1, 3);
				var category = byte.Parse(str.Substring(i, num2 - i));
				var b = byte.Parse(str.Substring(num2 + 1, num3 - (num2 + 1)));
				var num4 = 0;
				var num5 = num3 + 1;
				var stringBuilder = new System.Text.StringBuilder();
				do
				{
					stringBuilder.Append(str[num5]);
					if (str[num5] == '{')
					{
						num4++;
					}
					else
					{
						if (str[num5] == '}')
						{
							num4--;
						}
					}
					num5++;
				}
				while (num4 > 0);
				list.Add(new SubActorLook((sbyte)b, (SubEntityBindingPointCategoryEnum)category, ActorLook.Parse(stringBuilder.ToString())));
				i = num5 + 1;
			}
			return new ActorLook(bones, skins, source.ToDictionary((Tuple<int, int> x) => x.Item1, (Tuple<int, int> x) => Color.FromArgb(x.Item2)), scales, list.ToArray());
		}

		private static Tuple<int, int> ParseIndexedColor(string str)
		{
			var num = str.IndexOf("=");
			var flag = str[num + 1] == '#';
			var item = int.Parse(str.Substring(0, num));
			var item2 = int.Parse(str.Substring(num + (flag ? 2 : 1), str.Length - (num + (flag ? 2 : 1))), flag ? System.Globalization.NumberStyles.HexNumber : System.Globalization.NumberStyles.Integer);
			return Tuple.Create<int, int>(item, item2);
		}

		private static T[] ParseCollection<T>(string str, Func<string, T> converter)
		{
			T[] result;
			if (string.IsNullOrEmpty(str))
			{
				result = new T[0];
			}
			else
			{
				var num = 0;
				var num2 = str.IndexOf(',', 0);
				if (num2 == -1)
				{
					result = new T[]
					{
						converter(str)
					};
				}
				else
				{
					var array = new T[str.CountOccurences(',', num, str.Length - num) + 1];
					var num3 = 0;
					while (num2 != -1)
					{
						array[num3] = converter(str.Substring(num, num2 - num));
						num = num2 + 1;
						num2 = str.IndexOf(',', num);
						num3++;
					}
					array[num3] = converter(str.Substring(num, str.Length - num));
					result = array;
				}
			}
			return result;
		}
	}
}

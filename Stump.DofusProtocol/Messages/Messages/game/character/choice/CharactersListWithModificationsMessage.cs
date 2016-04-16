









// Generated on 07/24/2015 23:19:51
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class CharactersListWithModificationsMessage : CharactersListMessage
    {
        public const ushort Id = 6120;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public IEnumerable<Types.CharacterToRecolorInformation> charactersToRecolor;
        public IEnumerable<int> charactersToRename;
        public IEnumerable<int> unusableCharacters;
        public IEnumerable<Types.CharacterToRelookInformation> charactersToRelook;
        
        public CharactersListWithModificationsMessage()
        {
        }
        
        public CharactersListWithModificationsMessage(IEnumerable<Types.CharacterBaseInformations> characters, bool hasStartupActions, IEnumerable<Types.CharacterToRecolorInformation> charactersToRecolor, IEnumerable<int> charactersToRename, IEnumerable<int> unusableCharacters, IEnumerable<Types.CharacterToRelookInformation> charactersToRelook)
         : base(characters, hasStartupActions)
        {
            this.charactersToRecolor = charactersToRecolor;
            this.charactersToRename = charactersToRename;
            this.unusableCharacters = unusableCharacters;
            this.charactersToRelook = charactersToRelook;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            base.Serialize(writer);
            writer.WriteUShort((ushort)charactersToRecolor.Count());
            foreach (var entry in charactersToRecolor)
            {
                 entry.Serialize(writer);
            }
            writer.WriteUShort((ushort)charactersToRename.Count());
            foreach (var entry in charactersToRename)
            {
                 writer.WriteInt(entry);
            }
            writer.WriteUShort((ushort)unusableCharacters.Count());
            foreach (var entry in unusableCharacters)
            {
                 writer.WriteInt(entry);
            }
            writer.WriteUShort((ushort)charactersToRelook.Count());
            foreach (var entry in charactersToRelook)
            {
                 entry.Serialize(writer);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            base.Deserialize(reader);
            var limit = reader.ReadShort();
            charactersToRecolor = new Types.CharacterToRecolorInformation[limit];
            for (int i = 0; i < limit; i++)
            {
                 (charactersToRecolor as Types.CharacterToRecolorInformation[])[i] = new Types.CharacterToRecolorInformation();
                 (charactersToRecolor as Types.CharacterToRecolorInformation[])[i].Deserialize(reader);
            }
            limit = reader.ReadShort();
            charactersToRename = new int[limit];
            for (int i = 0; i < limit; i++)
            {
                 (charactersToRename as int[])[i] = reader.ReadInt();
            }
            limit = reader.ReadShort();
            unusableCharacters = new int[limit];
            for (int i = 0; i < limit; i++)
            {
                 (unusableCharacters as int[])[i] = reader.ReadInt();
            }
            limit = reader.ReadShort();
            charactersToRelook = new Types.CharacterToRelookInformation[limit];
            for (int i = 0; i < limit; i++)
            {
                 (charactersToRelook as Types.CharacterToRelookInformation[])[i] = new Types.CharacterToRelookInformation();
                 (charactersToRelook as Types.CharacterToRelookInformation[])[i].Deserialize(reader);
            }
        }
        
    }
    
}










// Generated on 07/24/2015 23:19:55
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class GameFightSpectateMessage : Message
    {
        public const ushort Id = 6069;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public IEnumerable<Types.FightDispellableEffectExtendedInformations> effects;
        public IEnumerable<Types.GameActionMark> marks;
        public ushort gameTurn;
        public int fightStart;
        public IEnumerable<Types.Idol> idols;
        
        public GameFightSpectateMessage()
        {
        }
        
        public GameFightSpectateMessage(IEnumerable<Types.FightDispellableEffectExtendedInformations> effects, IEnumerable<Types.GameActionMark> marks, ushort gameTurn, int fightStart, IEnumerable<Types.Idol> idols)
        {
            this.effects = effects;
            this.marks = marks;
            this.gameTurn = gameTurn;
            this.fightStart = fightStart;
            this.idols = idols;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUShort((ushort)effects.Count());
            foreach (var entry in effects)
            {
                 entry.Serialize(writer);
            }
            writer.WriteUShort((ushort)marks.Count());
            foreach (var entry in marks)
            {
                 entry.Serialize(writer);
            }
            writer.WriteVarUhShort(gameTurn);
            writer.WriteInt(fightStart);
            writer.WriteUShort((ushort)idols.Count());
            foreach (var entry in idols)
            {
                 entry.Serialize(writer);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            var limit = reader.ReadShort();
            effects = new Types.FightDispellableEffectExtendedInformations[limit];
            for (int i = 0; i < limit; i++)
            {
                 (effects as Types.FightDispellableEffectExtendedInformations[])[i] = new Types.FightDispellableEffectExtendedInformations();
                 (effects as Types.FightDispellableEffectExtendedInformations[])[i].Deserialize(reader);
            }
            limit = reader.ReadShort();
            marks = new Types.GameActionMark[limit];
            for (int i = 0; i < limit; i++)
            {
                 (marks as Types.GameActionMark[])[i] = new Types.GameActionMark();
                 (marks as Types.GameActionMark[])[i].Deserialize(reader);
            }
            gameTurn = reader.ReadVarUhShort();
            if (gameTurn < 0)
                throw new Exception("Forbidden value on gameTurn = " + gameTurn + ", it doesn't respect the following condition : gameTurn < 0");
            fightStart = reader.ReadInt();
            if (fightStart < 0)
                throw new Exception("Forbidden value on fightStart = " + fightStart + ", it doesn't respect the following condition : fightStart < 0");
            limit = reader.ReadShort();
            idols = new Types.Idol[limit];
            for (int i = 0; i < limit; i++)
            {
                 (idols as Types.Idol[])[i] = new Types.Idol();
                 (idols as Types.Idol[])[i].Deserialize(reader);
            }
        }
        
    }
    
}
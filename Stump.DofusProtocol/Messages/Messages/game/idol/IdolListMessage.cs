









// Generated on 07/24/2015 23:20:07
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class IdolListMessage : Message
    {
        public const ushort Id = 6585;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public IEnumerable<ushort> chosenIdols;
        public IEnumerable<ushort> partyChosenIdols;
        public IEnumerable<Types.PartyIdol> partyIdols;
        
        public IdolListMessage()
        {
        }
        
        public IdolListMessage(IEnumerable<ushort> chosenIdols, IEnumerable<ushort> partyChosenIdols, IEnumerable<Types.PartyIdol> partyIdols)
        {
            this.chosenIdols = chosenIdols;
            this.partyChosenIdols = partyChosenIdols;
            this.partyIdols = partyIdols;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUShort((ushort)chosenIdols.Count());
            foreach (var entry in chosenIdols)
            {
                 writer.WriteVarUhShort(entry);
            }
            writer.WriteUShort((ushort)partyChosenIdols.Count());
            foreach (var entry in partyChosenIdols)
            {
                 writer.WriteVarUhShort(entry);
            }
            writer.WriteUShort((ushort)partyIdols.Count());
            foreach (var entry in partyIdols)
            {
                 writer.WriteShort(entry.TypeId);
                 entry.Serialize(writer);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            var limit = reader.ReadShort();
            chosenIdols = new ushort[limit];
            for (int i = 0; i < limit; i++)
            {
                 (chosenIdols as ushort[])[i] = reader.ReadVarUhShort();
            }
            limit = reader.ReadShort();
            partyChosenIdols = new ushort[limit];
            for (int i = 0; i < limit; i++)
            {
                 (partyChosenIdols as ushort[])[i] = reader.ReadVarUhShort();
            }
            limit = reader.ReadShort();
            partyIdols = new Types.PartyIdol[limit];
            for (int i = 0; i < limit; i++)
            {
                 (partyIdols as Types.PartyIdol[])[i] = Types.ProtocolTypeManager.GetInstance<Types.PartyIdol>(reader.ReadShort());
                 (partyIdols as Types.PartyIdol[])[i].Deserialize(reader);
            }
        }
        
    }
    
}










// Generated on 07/24/2015 23:20:07
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class IdolFightPreparationUpdateMessage : Message
    {
        public const ushort Id = 6586;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public sbyte idolSource;
        public IEnumerable<Types.Idol> idols;
        
        public IdolFightPreparationUpdateMessage()
        {
        }
        
        public IdolFightPreparationUpdateMessage(sbyte idolSource, IEnumerable<Types.Idol> idols)
        {
            this.idolSource = idolSource;
            this.idols = idols;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteSByte(idolSource);
            writer.WriteUShort((ushort)idols.Count());
            foreach (var entry in idols)
            {
                 writer.WriteShort(entry.TypeId);
                 entry.Serialize(writer);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            idolSource = reader.ReadSByte();
            if (idolSource < 0)
                throw new Exception("Forbidden value on idolSource = " + idolSource + ", it doesn't respect the following condition : idolSource < 0");
            var limit = reader.ReadShort();
            idols = new Types.Idol[limit];
            for (int i = 0; i < limit; i++)
            {
                 (idols as Types.Idol[])[i] = Types.ProtocolTypeManager.GetInstance<Types.Idol>(reader.ReadShort());
                 (idols as Types.Idol[])[i].Deserialize(reader);
            }
        }
        
    }
    
}
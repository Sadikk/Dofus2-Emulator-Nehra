









// Generated on 07/24/2015 23:20:04
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class SpellForgottenMessage : Message
    {
        public const ushort Id = 5834;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public IEnumerable<ushort> spellsId;
        public ushort boostPoint;
        
        public SpellForgottenMessage()
        {
        }
        
        public SpellForgottenMessage(IEnumerable<ushort> spellsId, ushort boostPoint)
        {
            this.spellsId = spellsId;
            this.boostPoint = boostPoint;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUShort((ushort)spellsId.Count());
            foreach (var entry in spellsId)
            {
                 writer.WriteVarUhShort(entry);
            }
            writer.WriteVarUhShort(boostPoint);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            var limit = reader.ReadShort();
            spellsId = new ushort[limit];
            for (int i = 0; i < limit; i++)
            {
                 (spellsId as ushort[])[i] = reader.ReadVarUhShort();
            }
            boostPoint = reader.ReadVarUhShort();
            if (boostPoint < 0)
                throw new Exception("Forbidden value on boostPoint = " + boostPoint + ", it doesn't respect the following condition : boostPoint < 0");
        }
        
    }
    
}
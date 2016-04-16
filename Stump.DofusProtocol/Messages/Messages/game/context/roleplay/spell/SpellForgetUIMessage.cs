









// Generated on 07/24/2015 23:20:04
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class SpellForgetUIMessage : Message
    {
        public const ushort Id = 5565;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public bool open;
        
        public SpellForgetUIMessage()
        {
        }
        
        public SpellForgetUIMessage(bool open)
        {
            this.open = open;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteBoolean(open);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            open = reader.ReadBoolean();
        }
        
    }
    
}
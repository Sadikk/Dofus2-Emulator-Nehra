









// Generated on 07/24/2015 23:20:16
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class ShortcutBarAddErrorMessage : Message
    {
        public const ushort Id = 6227;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public sbyte error;
        
        public ShortcutBarAddErrorMessage()
        {
        }
        
        public ShortcutBarAddErrorMessage(sbyte error)
        {
            this.error = error;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteSByte(error);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            error = reader.ReadSByte();
            if (error < 0)
                throw new Exception("Forbidden value on error = " + error + ", it doesn't respect the following condition : error < 0");
        }
        
    }
    
}
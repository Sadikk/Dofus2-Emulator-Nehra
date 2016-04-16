









// Generated on 07/24/2015 23:20:18
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class DownloadPartMessage : Message
    {
        public const ushort Id = 1503;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public string id;
        
        public DownloadPartMessage()
        {
        }
        
        public DownloadPartMessage(string id)
        {
            this.id = id;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUTF(id);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            id = reader.ReadUTF();
        }
        
    }
    
}
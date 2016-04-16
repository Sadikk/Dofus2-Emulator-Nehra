









// Generated on 07/24/2015 23:19:49
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class AllianceJoinedMessage : Message
    {
        public const ushort Id = 6402;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public Types.AllianceInformations allianceInfo;
        public bool enabled;
        
        public AllianceJoinedMessage()
        {
        }
        
        public AllianceJoinedMessage(Types.AllianceInformations allianceInfo, bool enabled)
        {
            this.allianceInfo = allianceInfo;
            this.enabled = enabled;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            allianceInfo.Serialize(writer);
            writer.WriteBoolean(enabled);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            allianceInfo = new Types.AllianceInformations();
            allianceInfo.Deserialize(reader);
            enabled = reader.ReadBoolean();
        }
        
    }
    
}
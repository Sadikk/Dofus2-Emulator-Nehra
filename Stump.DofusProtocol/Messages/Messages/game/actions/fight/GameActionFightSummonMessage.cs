









// Generated on 07/24/2015 23:19:48
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class GameActionFightSummonMessage : AbstractGameActionMessage
    {
        public const ushort Id = 5825;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public Types.GameFightFighterInformations summon;
        
        public GameActionFightSummonMessage()
        {
        }
        
        public GameActionFightSummonMessage(ushort actionId, int sourceId, Types.GameFightFighterInformations summon)
         : base(actionId, sourceId)
        {
            this.summon = summon;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            base.Serialize(writer);
            writer.WriteShort(summon.TypeId);
            summon.Serialize(writer);
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            base.Deserialize(reader);
            summon = Types.ProtocolTypeManager.GetInstance<Types.GameFightFighterInformations>(reader.ReadShort());
            summon.Deserialize(reader);
        }
        
    }
    
}
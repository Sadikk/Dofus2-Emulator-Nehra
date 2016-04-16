// Generated on 07/24/2015 23:19:55
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class GameFightTurnStartMessage : Message
    {
        public const ushort Id = 714;
        public override ushort MessageId
        {
            get { return Id; }
        }

        public int id;
        public uint waitTime;

        public GameFightTurnStartMessage()
        {
        }

        public GameFightTurnStartMessage(int id, uint waitTime)
        {
            this.id = id;
            this.waitTime = waitTime;
        }

        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteInt(id);
            writer.WriteVarUhInt(waitTime);
        }

        public override void Deserialize(ICustomDataInput reader)
        {
            id = reader.ReadInt();
            waitTime = reader.ReadVarUhInt();
            if (waitTime < 0)
                throw new Exception("Forbidden value on waitTime = " + waitTime + ", it doesn't respect the following condition : waitTime < 0");
        }
    }
}
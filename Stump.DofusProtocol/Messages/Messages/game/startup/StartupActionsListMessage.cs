









// Generated on 07/24/2015 23:20:17
using System;
using System.Collections.Generic;
using System.Linq;
using Stump.DofusProtocol.Types;
using Stump.DofusProtocol.Messages;
using Stump.Core.IO;

namespace Stump.DofusProtocol.Messages
{
    public class StartupActionsListMessage : Message
    {
        public const ushort Id = 1301;
        public override ushort MessageId
        {
            get { return Id; }
        }
        
        public IEnumerable<Types.StartupActionAddObject> actions;
        
        public StartupActionsListMessage()
        {
        }
        
        public StartupActionsListMessage(IEnumerable<Types.StartupActionAddObject> actions)
        {
            this.actions = actions;
        }
        
        public override void Serialize(ICustomDataOutput writer)
        {
            writer.WriteUShort((ushort)actions.Count());
            foreach (var entry in actions)
            {
                 entry.Serialize(writer);
            }
        }
        
        public override void Deserialize(ICustomDataInput reader)
        {
            var limit = reader.ReadShort();
            actions = new Types.StartupActionAddObject[limit];
            for (int i = 0; i < limit; i++)
            {
                 (actions as Types.StartupActionAddObject[])[i] = new Types.StartupActionAddObject();
                 (actions as Types.StartupActionAddObject[])[i].Deserialize(reader);
            }
        }
        
    }
    
}
using Stump.DofusProtocol.Enums;
using Stump.Server.BaseServer.Commands;
using System;

namespace Stump.Server.AuthServer.Commands
{
    public static class ParametersConverter
    {
        public static ConverterHandler<RoleEnum> RoleConverter = delegate(string entry, TriggerBase trigger)
        {
            RoleEnum roleEnum;
            RoleEnum result;
            if (Enum.TryParse<RoleEnum>(entry, true, out roleEnum))
            {
                result = roleEnum;
            }
            else
            {
                byte value;
                if (!byte.TryParse(entry, out value))
                {
                    throw new ArgumentException("entry is not RoleEnum");
                }
                result = (RoleEnum)Enum.ToObject(typeof(RoleEnum), value);
            }
            return result;
        };
    }
}
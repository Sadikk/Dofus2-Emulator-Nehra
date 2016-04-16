using Stump.DofusProtocol.Messages;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Core.Network;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using System;
namespace Stump.Server.WorldServer.Handlers.Titles
{
	public class TitleHandler : WorldHandlerContainer
	{
        private TitleHandler() { }

        [WorldHandler(TitleSelectRequestMessage.Id)]
		public static void HandleTitleSelectRequestMessage(WorldClient client, TitleSelectRequestMessage message)
		{
			if (message.titleId != 0)
			{
				client.Character.SelectTitle(message.titleId);
			}
			else
			{
				client.Character.ResetTitle();
			}
		}
        [WorldHandler(OrnamentSelectRequestMessage.Id)]
		public static void HandleOrnamentSelectRequestMessage(WorldClient client, OrnamentSelectRequestMessage message)
		{
			if (message.ornamentId != 0)
			{
                client.Character.SelectOrnament(message.ornamentId);
			}
			else
			{
				client.Character.ResetOrnament();
			}
		}
        [WorldHandler(TitlesAndOrnamentsListRequestMessage.Id)]
		public static void HandleTitlesAndOrnamentsListRequestMessage(WorldClient client, TitlesAndOrnamentsListRequestMessage message)
		{
			TitleHandler.SendTitlesAndOrnamentsListMessage(client, client.Character);
		}

		public static void SendTitleSelectErrorMessage(IPacketReceiver client)
		{
			client.Send(new TitleSelectErrorMessage());
		}
		public static void SendTitleSelectedMessage(IPacketReceiver client, short title)
		{
            client.Send(new TitleSelectedMessage((ushort)title));
		}
		public static void SendOrnamentSelectedMessage(IPacketReceiver client, short ornament)
		{
            client.Send(new OrnamentSelectedMessage((ushort)ornament));
		}
		public static void SendTitlesAndOrnamentsListMessage(IPacketReceiver client, Character character)
		{
            client.Send(new TitlesAndOrnamentsListMessage(
                character.Titles, 
                character.Ornaments, 
                Convert.ToUInt16(character.SelectedTitle.HasValue ? character.SelectedTitle.Value : 0), 
                Convert.ToUInt16(character.SelectedOrnament.HasValue ? character.SelectedOrnament.Value : 0)));
		}
		public static void SendTitleGainedMessage(IPacketReceiver client, short title)
		{
            client.Send(new TitleGainedMessage((ushort)title));
		}
		public static void SendTitleLostMessage(IPacketReceiver client, short title)
		{
            client.Send(new TitleLostMessage((ushort)title));
		}
		public static void SendOrnamentGainedMessage(IPacketReceiver client, short ornament)
		{
			client.Send(new OrnamentGainedMessage(ornament));
		}
	}
}

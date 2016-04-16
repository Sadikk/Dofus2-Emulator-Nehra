using Stump.DofusProtocol.Enums;
using Stump.DofusProtocol.Messages;
using Stump.Server.BaseServer.Network;
using Stump.Server.WorldServer.Database.World;
using Stump.Server.WorldServer.Game.Actors.RolePlay.Characters;
using Stump.Server.WorldServer.Game.Interactives;
using Stump.Server.WorldServer.Game.Maps;
using Stump.Server.WorldServer.Game.Maps.Cells;
using Stump.Server.WorldServer.Handlers.Dialogs;
using System.Drawing;
using System.Linq;
namespace Stump.Server.WorldServer.Game.Dialogs.Interactives
{
	public class ZaapDialog : IDialog
	{
		private System.Collections.Generic.List<Map> m_destinations = new System.Collections.Generic.List<Map>();
		public DialogTypeEnum DialogType
		{
			get
			{
				return DialogTypeEnum.DIALOG_TELEPORTER;
			}
		}
		public Character Character
		{
			get;
			private set;
		}
		public InteractiveObject Zaap
		{
			get;
			private set;
		}
		public ZaapDialog(Character character, InteractiveObject zaap)
		{
			this.Character = character;
			this.Zaap = zaap;
		}
		public ZaapDialog(Character character, InteractiveObject zaap, System.Collections.Generic.IEnumerable<Map> destinations)
		{
			this.Character = character;
			this.Zaap = zaap;
			this.m_destinations = destinations.ToList<Map>();
		}
		public void AddDestination(Map map)
		{
			this.m_destinations.Add(map);
		}
		public void Open()
		{
			this.Character.SetDialog(this);
			this.SendZaapListMessage(this.Character.Client);
		}
		public void Close()
		{
			this.Character.CloseDialog(this);
			DialogHandler.SendLeaveDialogMessage(this.Character.Client, this.DialogType);
		}
		public void Teleport(Map map)
		{
			if (this.m_destinations.Contains(map))
			{
				Cell cell = map.GetCell(280);
				if (map.Zaap != null)
				{
					cell = map.GetCell((int)map.Zaap.Position.Point.GetCellInDirection(DirectionsEnum.DIRECTION_SOUTH_WEST, 1).CellId);
					if (!cell.Walkable)
					{
						MapPoint[] array = map.Zaap.Position.Point.GetAdjacentCells((short entry) => map.GetCell((int)entry).Walkable).ToArray<MapPoint>();
						if (array.Length == 0)
						{
							throw new System.Exception(string.Format("Cannot find a free adjacent cell near the zaap (id:{0}) on map {1}", map.Zaap.Id, map.Id));
						}
						cell = map.GetCell((int)array[0].CellId);
					}
				}
				this.Character.Teleport(map, cell);
				this.Close();
			}
		}
		public void SendZaapListMessage(IPacketReceiver client)
		{
			client.Send(new ZaapListMessage(0, 
				from entry in this.m_destinations
				select entry.Id, this.m_destinations.Select((Map entry) => (ushort)entry.SubArea.Id), this.m_destinations.Select(this.GetCostTo), Enumerable.Repeat<sbyte>((sbyte)TeleporterTypeEnum.TELEPORTER_ZAAP, m_destinations.Count), this.Zaap.Map.Id));
		}
		public ushort GetCostTo(Map map)
		{
			Point position = map.Position;
			Point position2 = this.Zaap.Map.Position;
            return (ushort)System.Math.Floor(System.Math.Sqrt((double)((position2.X - position.X) * (position2.X - position.X) + (position2.Y - position.Y) * (position2.Y - position.Y))) * 10.0);
		}
	}
}

using Stump.Core.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stump.DbSynchronizer.Protocol.DLM
{
    public class CellData
    {
        // FIELDS
        public int id;
        private int m_floor;
        private byte m_losmov;
        public byte speed;
        public byte mapChangeData;
        public byte moveZone;
        private byte m_arrow;
        private bool m_los;
        private bool m_mov;
        private bool m_visible;
        private bool m_farmCell;
        private bool m_blue;
        private bool m_red;
        private bool m_nonWalkableDuringRP;
        private bool m_nonWalkableDuringFight;
        private Map m_map;

        // PROPERTIES
        public Map Map
        {
            get
            {
                return this.m_map;
            }
        }
        public byte LosMov
        {
            get
            {
                return this.m_losmov;
            }
        }
        public bool Mov
        {
            get
            {
                return this.m_mov;
            }
        }
        public bool Los
        {
            get
            {
                return this.m_los;
            }
        }
        public bool NonWalkableDuringFight
        {
            get
            {
                return this.m_nonWalkableDuringFight;
            }
        }
        public bool Red
        {
            get
            {
                return this.m_red;
            }
        }
        public bool Blue
        {
            get
            {
                return this.m_blue;
            }
        }
        public bool FarmCell
        {
            get
            {
                return this.m_farmCell;
            }
        }
        public bool Visible
        {
            get
            {
                return this.m_visible;
            }
        }
        public bool NonWalkableDuringRP
        {
            get
            {
                return this.m_nonWalkableDuringRP;
            }
        }
        public int Floor
        {
            get
            {
                return this.m_floor;
            }
        }
        public bool UseTopArrow
        {
            get
            {
                return (this.m_arrow & 1) != 0;
            }
        }
        public bool UseBottomArrow
        {
            get
            {
                return (this.m_arrow & 2) != 0;
            }
        }
        public bool UseRightArrow
        {
            get
            {
                return (this.m_arrow & 4) != 0;
            }
        }
        public bool UseLeftArrow
        {
            get
            {
                return (this.m_arrow & 8) != 0;
            }
        }

        // CONSTRUCTORS
        public CellData(Map parent, int id)
        {
            this.id = id;
            this.m_map = parent;
        }

        // METHODS
        public void FromRaw(IDataReader reader)
        {
            try
            {
                this.m_floor = reader.ReadByte() * 10;
                if (this.m_floor == -1280)
                {
                    return;
                }
                this.m_losmov = reader.ReadByte();
                this.speed = reader.ReadByte();
                this.mapChangeData = reader.ReadByte();
                if (this.m_map.mapVersion > 5)
                {
                    this.moveZone = reader.ReadByte();
                }
                if (this.m_map.mapVersion > 7)
                {
                    var tmpBits = reader.ReadByte();
                    this.m_arrow = (byte)(15 & tmpBits);
                    if (this.UseTopArrow)
                    {
                        this.m_map.topArrowCell.Add(this.id);
                    }
                    if (this.UseBottomArrow)
                    {
                        this.m_map.bottomArrowCell.Add(this.id);
                    }
                    if (this.UseLeftArrow)
                    {
                        this.m_map.leftArrowCell.Add(this.id);
                    }
                    if (this.UseRightArrow)
                    {
                        this.m_map.rightArrowCell.Add(this.id);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            this.m_los = (this.m_losmov & 2) >> 1 == 1;
            this.m_mov = (this.m_losmov & 1) == 1;
            this.m_visible = (this.m_losmov & 64) >> 6 == 1;
            this.m_farmCell = (this.m_losmov & 32) >> 5 == 1;
            this.m_blue = (this.m_losmov & 16) >> 4 == 1;
            this.m_red = (this.m_losmov & 8) >> 3 == 1;
            this.m_nonWalkableDuringRP = (this.m_losmov & 128) >> 7 == 1;
            this.m_nonWalkableDuringFight = (this.m_losmov & 4) >> 2 == 1;
        }
    }
}

using Stump.Core.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stump.DbSynchronizer.Protocol.DLM.Elements
{
    public enum ElementTypesEnum
    {
        GRAPHICAL = 2,
        SOUND = 33
    }

    public abstract class BasicElement
    {
        // FIELDS
        private Cell m_cell;

        // PROPERTIES
        public Cell Cell
        {
            get
            {
                return this.m_cell;
            }
        }
        public virtual int ElementType
        {
            get
            {
                return -1;
            }
        }

        // CONSTRUCTORS
        public BasicElement(Cell parent)
        {
            this.m_cell = parent;
        }

        // METHODS
        public static BasicElement GetElementFromType(int param1, Cell param2)
        {
            switch ((ElementTypesEnum)param1)
            {
                case ElementTypesEnum.GRAPHICAL:
                    return new GraphicalElement(param2);

                case ElementTypesEnum.SOUND:
                    return new SoundElement(param2);
                default:
                    throw new Exception("Un élément de type inconnu " + param1 + " a été trouvé sur la cellule " + param2.cellId + "!");
            }
        }

        public abstract void FromRaw(IDataReader reader, int mapVersion);
    }
}

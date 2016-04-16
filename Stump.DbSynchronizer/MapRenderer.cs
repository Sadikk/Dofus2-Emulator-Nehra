using Stump.Core.Reflection;
using Stump.DbSynchronizer.Protocol.DLM;
using Stump.DbSynchronizer.Protocol.DLM.Elements;
using Stump.DbSynchronizer.Protocol.Elements;
using Stump.DbSynchronizer.Protocol.Elements.Subtypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stump.DbSynchronizer
{
    public class MapRenderer
    {
        // FIELDS
        private Map m_map;

        // PROPERTIES

        // CONSTRUCTORS

        // METHODS
        public void Render(Map map)
        {
            this.m_map = map;

            foreach (var layer in this.m_map.layers)
            {
                var layerId = layer.layerId;
                if (layer.cellsCount != 0)
                {
                    for (int i = 0; i < layer.cellsCount; i++)
                    {
                        var cell = layer.cells[i];
                        var currentCellId = cell.cellId;

                        foreach (GraphicalElement ele in cell.elements)
                        {
                            var elementData = Singleton<Element>.Instance.GetElementData((int)ele.elementId);
                            if (elementData is NormalGraphicalElementData)
                            {

                            }
                        }
                    }
                }
            }
        }
    }
}

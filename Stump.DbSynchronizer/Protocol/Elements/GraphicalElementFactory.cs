using Stump.DbSynchronizer.Protocol.Elements.Subtypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stump.DbSynchronizer.Protocol.Elements
{
    public class GraphicalElementFactory
    {
        // FIELDS

        // PROPERTIES

        // CONSTRUCTORS

        // METHODS
        public static GraphicalElementData GetGraphicalElementData(int id, int type)
        {
            switch ((GraphicalElementTypesEnum)type)
            {
                case GraphicalElementTypesEnum.NORMAL:
                    return new NormalGraphicalElementData(id, type);
                case GraphicalElementTypesEnum.BOUNDING_BOX:
                    return new BoundingBoxGraphicalElementData(id, type);
                case GraphicalElementTypesEnum.ANIMATED:
                    return new AnimatedGraphicalElementData(id, type);
                case GraphicalElementTypesEnum.ENTITY:
                    return new EntityGraphicalElementData(id, type);
                case GraphicalElementTypesEnum.PARTICLES:
                    return new ParticlesGraphicalElementData(id, type);
                case GraphicalElementTypesEnum.BLENDED:
                    return new BlendedGraphicalElementData(id, type);
                default:
                    return null;
            }
        }
    }
}

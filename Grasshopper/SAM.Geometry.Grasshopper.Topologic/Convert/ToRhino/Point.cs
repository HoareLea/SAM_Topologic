using Rhino.Geometry;
using System.Collections.Generic;
using Topologic;

namespace SAM.Geometry.Grasshopper.Topologic
{
    public static partial class Convert
    {
        public static Point3d ToRhino(this Vertex vertex)
        {
            IList<double> coordinates = vertex.Coordinates;
            return new Point3d(coordinates[0], coordinates[1], coordinates[2]);
        }
    }
}
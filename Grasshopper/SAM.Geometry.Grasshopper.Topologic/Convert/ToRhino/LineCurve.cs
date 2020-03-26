using Rhino.Geometry;
using System;
using System.Collections.Generic;
using Topologic;

namespace SAM.Geometry.Grasshopper.Topologic
{
    public static partial class Convert
    {
        public static LineCurve ToRhino(this Edge edge)
        {
            Vertex startVertex = edge.StartVertex;
            Point3d ghStartPoint = ToRhino(startVertex);
            Vertex endVertex = edge.EndVertex;
            Point3d ghEndPoint = ToRhino(endVertex);

            return new LineCurve(ghStartPoint, ghEndPoint);
        }
    }
}

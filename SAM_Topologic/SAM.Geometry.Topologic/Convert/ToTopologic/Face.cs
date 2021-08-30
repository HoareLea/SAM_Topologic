using SAM.Geometry.Spatial;
using System.Collections.Generic;
using Topologic;

namespace SAM.Geometry.Topologic
{
    public static partial class Convert
    {
        public static global::Topologic.Face ToTopologic(this Face3D face3D)
        {
            IClosedPlanar3D closedPlanar3D = face3D.GetExternalEdge3D();
            if (closedPlanar3D is ICurvable3D)
            {
                List<Edge> edges = new List<Edge>();
                foreach (ICurve3D curve3D in ((ICurvable3D)closedPlanar3D).GetCurves())
                {
                    Point3D point3D_1 = curve3D.GetStart();
                    Point3D point3D_2 = curve3D.GetEnd();
                    if(point3D_1 == point3D_2)
                    {
                        continue;
                    }

                    Edge edge = Edge.ByStartVertexEndVertex(ToTopologic(point3D_1), ToTopologic(point3D_2));
                    edges.Add(edge);
                }

                return global::Topologic.Face.ByEdges(edges);
            }
            return null;
        }
    }
}
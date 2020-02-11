using System.Collections.Generic;

using Topologic;

using SAM.Geometry.Spatial;

namespace SAM.Geometry.Topologic
{
    public static partial class Convert
    {
        public static global::Topologic.Face ToTopologic(this Face3D face)
        {
            IClosedPlanar3D closedPlanar3D = face.ToClosedPlanar3D();
            if(closedPlanar3D is ICurvable3D)
            {
                List<Edge> edges = new List<Edge>();
                foreach (ICurve3D curve3D in ((ICurvable3D)closedPlanar3D).GetCurves())
                {
                    Edge edge = Edge.ByStartVertexEndVertex(ToTopologic(curve3D.GetStart()), ToTopologic(curve3D.GetEnd()));
                    edges.Add(edge);
                }

                return global::Topologic.Face.ByEdges(edges);
            }
            return null;
        }
    }
}

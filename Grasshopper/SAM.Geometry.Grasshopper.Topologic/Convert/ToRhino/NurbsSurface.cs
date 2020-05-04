using System;
using System.Collections.Generic;

namespace SAM.Geometry.Grasshopper.Topologic
{
    public static partial class Convert
    {
        public static Rhino.Geometry.NurbsSurface ToRhino_NurbsSurface(this global::Topologic.NurbsSurface nurbsSurface)
        {
            int uDegree = nurbsSurface.UDegree;
            int vDegree = nurbsSurface.VDegree;
            bool isRational = nurbsSurface.IsURational && nurbsSurface.IsVRational;
            int uCount = nurbsSurface.NumOfUControlVertices;
            int vCount = nurbsSurface.NumOfVControlVertices;

            Rhino.Geometry.NurbsSurface ghNurbsSurface = Rhino.Geometry.NurbsSurface.Create(
                3,
                isRational,
                uDegree + 1,
                vDegree + 1,
                uCount,
                vCount
                );

            int i = 0;
            for (int u = 0; u < uCount; ++u)
            {
                for (int v = 0; v < vCount; ++v)
                {
                    global::Topologic.Vertex controlVertex = nurbsSurface.ControlVertex(u, v);
                    ghNurbsSurface.Points.SetPoint(u, v, ToRhino(controlVertex));
                    ++i;
                }
            }

            List<double> uKnots = nurbsSurface.UKnots;
            uKnots = uKnots.GetRange(1, uKnots.Count - 2);
            for (int u = 0; u < uKnots.Count; u++)
            {
                ghNurbsSurface.KnotsU[u] = uKnots[u];
            }

            List<double> vKnots = nurbsSurface.VKnots;
            vKnots = vKnots.GetRange(1, vKnots.Count - 2);
            for (int v = 0; v < vKnots.Count; v++)
            {
                ghNurbsSurface.KnotsV[v] = vKnots[v];
            }

            if (!ghNurbsSurface.IsValid)
            {
                throw new Exception("A valid surface cannot be created from this Face.");
            }

            return ghNurbsSurface;
        }
    }
}
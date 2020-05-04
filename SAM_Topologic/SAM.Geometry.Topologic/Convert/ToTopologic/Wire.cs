using SAM.Geometry.Spatial;
using System.Collections.Generic;
using Topologic;

namespace SAM.Geometry.Topologic
{
    public static partial class Convert
    {
        public static Wire ToTopologic(this ICurvable3D curvable3D)
        {
            List<Edge> edges = new List<Edge>();

            foreach (ICurve3D curve3D in curvable3D.GetCurves())
                edges.Add(curve3D.ToTopologic());

            return Wire.ByEdges(edges);
        }

        public static Wire ToTopologic(this Polygon3D polygon3D)
        {
            List<Edge> edges = new List<Edge>();

            foreach (ICurve3D curve3D in polygon3D.GetSegments())
                edges.Add(curve3D.ToTopologic());

            return Wire.ByEdges(edges);
        }
    }
}
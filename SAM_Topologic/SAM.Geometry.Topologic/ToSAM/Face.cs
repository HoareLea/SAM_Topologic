using SAM.Geometry.Spatial;
using System.Collections.Generic;
using Topologic;

namespace SAM.Geometry.Topologic
{
    public static partial class Convert
    {
        public static Face3D ToSAM(this global::Topologic.Face face)
        {
            if (face == null)
                return null;

            List<Polygon3D> polygon3Ds = new List<Polygon3D>() { ToSAM(face.ExternalBoundary) };

            List<Wire> wires = face.InternalBoundaries;
            if (wires != null && wires.Count > 0)
                wires.ForEach(x => polygon3Ds.Add(ToSAM(x)));

            return Face3D.Create(polygon3Ds);
        }
    }
}

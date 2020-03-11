using System.Collections.Generic;

using Topologic;

using SAM.Geometry.Spatial;


namespace SAM.Geometry.Topologic
{
    public static partial class Convert
    {
        public static Face3D ToSAM(this global::Topologic.Face face)
        {
            if (face == null)
                return null;

            List<Polygon3D> polygon3Ds = new List<Polygon3D>() { ToSAM_Polygon3D(face.ExternalBoundary) };

            List<Wire> wires = face.InternalBoundaries;
            if (wires != null && wires.Count > 0)
                wires.ForEach(x => polygon3Ds.Add(ToSAM_Polygon3D(x)));

            return Face3D.Create(polygon3Ds);
        }
    }
}

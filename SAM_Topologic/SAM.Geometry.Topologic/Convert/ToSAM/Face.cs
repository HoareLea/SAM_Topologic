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

            Polygon3D polygon3D = ToSAM_Polygon3D(face.ExternalBoundary);
            if (polygon3D == null)
                return null;

            List<Polygon3D> polygon3Ds = new List<Polygon3D>() { polygon3D };

            List<Wire> wires = face.InternalBoundaries;
            if (wires != null && wires.Count > 0)
            {
                foreach (Wire wire in wires)
                {
                    polygon3D = ToSAM_Polygon3D(wire);
                    if (polygon3D == null)
                        continue;

                    polygon3Ds.Add(polygon3D);
                }
            }

            return Face3D.Create(polygon3Ds);
        }
    }
}
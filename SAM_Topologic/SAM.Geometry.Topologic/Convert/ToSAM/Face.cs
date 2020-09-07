using SAM.Geometry.Spatial;
using System.Collections.Generic;
using System.Linq;
using Topologic;
using Topologic.Utilities;

namespace SAM.Geometry.Topologic
{
    public static partial class Convert
    {
        public static Face3D ToSAM(this global::Topologic.Face face)
        {
            if (face == null)
                return null;

            Polygon3D polygon3D = null;

            Vector3D normal = new Vector3D(FaceUtility.NormalAtParameters(face, 0.5, 0.5));
            if (normal != null)
                polygon3D = Spatial.Create.Polygon3D(normal, face.ExternalBoundary.Vertices?.ToList().ConvertAll(x => x.ToSAM()));

            if (polygon3D == null)
                polygon3D = ToSAM_Polygon3D(face.ExternalBoundary);

            if (polygon3D == null)
                return null;

            List<Polygon3D> polygon3Ds = new List<Polygon3D>() { polygon3D };

            IList<Wire> wires = face.InternalBoundaries;
            if (wires != null && wires.Count > 0)
            {
                foreach (Wire wire in wires)
                {
                    polygon3D = null;
                    
                    if(normal != null)
                        polygon3D = Spatial.Create.Polygon3D(normal, wire.Vertices?.ToList().ConvertAll(x => x.ToSAM()));

                    if (polygon3D == null)
                        polygon3D = ToSAM_Polygon3D(wire);

                    if (polygon3D == null)
                        continue;
                    
                    polygon3Ds.Add(polygon3D);
                }
            }

            Face3D result = Face3D.Create(polygon3Ds);
            return result;
        }
    }
}
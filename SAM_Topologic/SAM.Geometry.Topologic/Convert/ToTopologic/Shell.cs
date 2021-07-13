using SAM.Geometry.Spatial;
using System.Collections.Generic;

namespace SAM.Geometry.Topologic
{
    public static partial class Convert
    {
        public static global::Topologic.Shell ToTopologic(this Spatial.Shell shell, double tolerance = Core.Tolerance.Distance)
        {
            List<Face3D> face3Ds = shell?.Face3Ds;
            if (face3Ds == null || face3Ds.Count == 0)
                return null;

            global::Topologic.Shell result = global::Topologic.Shell.ByFaces(face3Ds.ConvertAll(x => x.ToTopologic()), tolerance);

            return result;
        }
    }
}
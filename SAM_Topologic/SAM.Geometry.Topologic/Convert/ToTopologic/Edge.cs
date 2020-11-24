using SAM.Geometry.Spatial;
using Topologic;

namespace SAM.Geometry.Topologic
{
    public static partial class Convert
    {
        public static Edge ToTopologic(this ICurve3D curve3D)
        {
            if (curve3D == null)
                return null;

            Point3D point3D_1 = curve3D.GetStart();
            if (point3D_1 == null)
                return null;

            Point3D point3D_2 = curve3D.GetEnd();
            if (point3D_2 == null)
                return null;

            if (point3D_1 == point3D_2)
                return null;

            return Edge.ByStartVertexEndVertex(ToTopologic(point3D_1), ToTopologic(point3D_2));
        }
    }
}
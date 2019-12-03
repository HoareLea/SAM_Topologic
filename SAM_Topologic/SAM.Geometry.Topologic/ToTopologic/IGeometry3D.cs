using SAM.Geometry.Spatial;

namespace SAM.Geometry.Topologic
{
    public static partial class Convert
    {
        public static object ToTopologic(this IGeometry3D geometry)
        {
            return ToTopologic(geometry as dynamic);
        }
    }
}

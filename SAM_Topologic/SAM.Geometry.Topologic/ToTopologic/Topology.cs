using SAM.Geometry.Spatial;

namespace SAM.Geometry.Topologic
{
    public static partial class Convert
    {
        public static global::Topologic.Topology ToTopologic(this IGeometry3D geometry)
        {
            return ToTopologic(geometry as dynamic);
        }
    }
}

using SAM.Geometry.Spatial;

namespace SAM.Geometry.Topologic
{
    public static partial class Convert
    {
        public static global::Topologic.Topology ToTopologic(this ISAMGeometry3D sAMGeometry)
        {
            return ToTopologic(sAMGeometry as dynamic);
        }
    }
}

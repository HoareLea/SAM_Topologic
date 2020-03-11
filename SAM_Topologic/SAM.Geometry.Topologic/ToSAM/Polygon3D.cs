using SAM.Geometry.Spatial;
using Topologic;

namespace SAM.Geometry.Topologic
{
    public static partial class Convert
    {
        public static Polygon3D ToSAM_Polygon3D(this Wire wire)
        {
            return Spatial.Create.Polygon3D(wire.Vertices.ConvertAll(x => x.ToSAM()));
        }
    }
}

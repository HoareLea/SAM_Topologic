using SAM.Geometry.Spatial;
using Topologic;

namespace SAM.Geometry.Topologic.ToSAM
{
    public static partial class Convert
    {
        public static Polygon3D ToSAM(this Wire wire)
        {
            return new Polygon3D(wire.Vertices.ConvertAll(x => x.ToSAM()));
        }
    }
}

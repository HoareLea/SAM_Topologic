using SAM.Geometry.Spatial;
using Topologic;

namespace SAM.Geometry.Topologic
{
    public static partial class Convert
    {
        public static Polygon3D ToSAM(this Wire wire)
        {
            return new Polygon3D(wire.Vertices.ConvertAll(x => x.ToSAM()));
        }

        public static Polygon3D ToSAM(this global::Topologic.Face face)
        {
            return new Polygon3D(ToSAM(face.ExternalBoundary));
        }
    }
}

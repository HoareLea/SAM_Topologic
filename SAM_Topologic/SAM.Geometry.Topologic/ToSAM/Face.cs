using SAM.Geometry.Spatial;
using Topologic;

namespace SAM.Geometry.Topologic
{
    public static partial class Convert
    {
        public static Spatial.Face ToSAM(this global::Topologic.Face face)
        {
            return new Spatial.Face(ToSAM(face.ExternalBoundary));
        }
    }
}

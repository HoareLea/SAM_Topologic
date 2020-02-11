using SAM.Geometry.Spatial;
using Topologic;

namespace SAM.Geometry.Topologic
{
    public static partial class Convert
    {
        public static Face3D ToSAM(this global::Topologic.Face face)
        {
            return new Face3D(ToSAM(face.ExternalBoundary));
        }
    }
}

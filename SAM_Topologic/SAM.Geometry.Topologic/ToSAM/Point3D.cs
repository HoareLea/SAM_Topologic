using SAM.Geometry.Spatial;
using Topologic;

namespace SAM.Geometry.Topologic
{
    public static partial class Convert
    {
        public static Point3D ToSAM(this Vertex vertex)
        {
            return new Point3D(vertex.X, vertex.Y, vertex.Z);
        }
    }
}

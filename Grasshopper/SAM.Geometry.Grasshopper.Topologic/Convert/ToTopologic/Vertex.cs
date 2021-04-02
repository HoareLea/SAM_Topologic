using Rhino.Geometry;
using Topologic;

namespace SAM.Geometry.Grasshopper.Topologic
{
    public static partial class Convert
    {
        public static Vertex ToTopologic(this Point3d point3d)
        {
            return Vertex.ByCoordinates(point3d.X, point3d.Y, point3d.Z);
        }

        public static Vertex ToTopologic(this Point3f point3f)
        {
            return Vertex.ByCoordinates(point3f.X, point3f.Y, point3f.Z);
        }
    }
}
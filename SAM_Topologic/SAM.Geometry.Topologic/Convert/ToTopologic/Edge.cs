using SAM.Geometry.Spatial;
using Topologic;

namespace SAM.Geometry.Topologic
{
    public static partial class Convert
    {
        public static Edge ToTopologic(this ICurve3D curve3D)
        {
            return Edge.ByStartVertexEndVertex(ToTopologic(curve3D.GetStart()), ToTopologic(curve3D.GetEnd()));
        }
    }
}
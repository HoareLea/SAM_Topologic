using SAM.Geometry.Spatial;
using Topologic;

namespace SAM.Geometry.Topologic
{
    public static partial class Convert
    {
        public static Segment3D ToSAM(this Edge edge)
        {
            return new Segment3D(edge.StartVertex.ToSAM(), edge.EndVertex.ToSAM());
        }
    }
}
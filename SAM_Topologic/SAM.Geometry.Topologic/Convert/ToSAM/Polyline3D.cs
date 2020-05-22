using SAM.Geometry.Spatial;
using System.Collections.Generic;
using Topologic;

namespace SAM.Geometry.Topologic
{
    public static partial class Convert
    {
        public static Polyline3D ToSAM(this Wire wire)
        {
            List<Vertex> vertices = wire.Vertices;
            if (vertices == null || vertices.Count < 3)
                return null;

            return new Polyline3D(vertices.ConvertAll(x => x.ToSAM()), wire.IsClosed);
        }
    }
}
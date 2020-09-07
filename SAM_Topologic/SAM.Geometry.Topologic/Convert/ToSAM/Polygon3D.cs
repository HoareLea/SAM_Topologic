using SAM.Geometry.Spatial;
using System.Collections.Generic;
using System.Linq;
using Topologic;

namespace SAM.Geometry.Topologic
{
    public static partial class Convert
    {
        public static Polygon3D ToSAM_Polygon3D(this Wire wire)
        {
            IList<Vertex> vertices = wire.Vertices;
            if (vertices == null || vertices.Count < 3)
                return null;

            return Spatial.Create.Polygon3D(vertices?.ToList().ConvertAll(x => x.ToSAM()));
        }
    }
}
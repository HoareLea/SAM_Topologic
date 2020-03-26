using SAM.Geometry.Spatial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topologic;

namespace SAM.Geometry.Topologic
{
    public static partial class Convert
    {
        public static Vertex ToTopologic(this Point3D point3D)
        {
            return Vertex.ByCoordinates(point3D.X, point3D.Y, point3D.Z);
        }
    }
}

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
        public static Edge ToTopologic(this Segment3D segment3D)
        {
            return Edge.ByStartVertexEndVertex(ToTopologic(segment3D[0]), ToTopologic(segment3D[1]));
        }
    }
}

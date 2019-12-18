using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel.Types;
using Topologic;

namespace SAM.Geometry.Grasshopper.Topologic
{
    public static partial class Convert
    {
        public static Spatial.IGeometry3D ToSAM(this Topology topology)
        {
            Vertex vertex = topology as Vertex;
            if (vertex != null)
                return  Topologic.Convert.ToSAM(vertex);

            Edge edge = topology as Edge;
            if (edge != null)
                return Topologic.Convert.ToSAM(edge);

            Wire wire = topology as Wire;
            if (wire != null)
                return Topologic.Convert.ToSAM(wire);

            Face face = topology as Face;
            if (face != null)
                return Topologic.Convert.ToSAM(face);

            return null;
        }
    }
}

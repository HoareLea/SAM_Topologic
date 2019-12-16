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
        public static Wire ToTopologic(this Polygon3D polygon3D)
        {
            List<Edge> edges = new List<Edge>();

            foreach (Segment3D segment3D in polygon3D.GetSegments())
                edges.Add(segment3D.ToTopologic());
            
            return Wire.ByEdges(edges);
        }
    }
}

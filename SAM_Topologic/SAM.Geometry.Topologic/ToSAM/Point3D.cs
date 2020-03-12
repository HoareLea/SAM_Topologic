using SAM.Geometry.Spatial;
using System;
using Topologic;

namespace SAM.Geometry.Topologic
{
    public static partial class Convert
    {
        //due to issue with plane calculation we rounding 
        public static Point3D ToSAM(this Vertex vertex, int @decimals = 9)
        {
            return new Point3D(Math.Round(vertex.X, decimals), Math.Round(vertex.Y, decimals), Math.Round(vertex.Z, decimals));
        }
    }
}

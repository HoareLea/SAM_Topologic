﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAM_TopologicGeometryDynamo
{
    public static class IGeometry3D
    {
        public static SAM.Geometry.Spatial.IGeometry3D ToSAMGeometry(Topologic.Topology topology)
        {
            return SAM.Geometry.Topologic.Convert.ToSAM(topology as dynamic);
        }
    }
}
using System;
using System.Linq;
using System.Collections.Generic;

using Topologic;

using Autodesk.DesignScript.Runtime;
using SAM.Analytical;

namespace SAMTopologicAnalyticalDynamo
{
    /// <summary>
    /// SAM Analytical Panel
    /// </summary>
    public static class Panel
    {
        /// <summary>
        /// Extract Space Adjacency information for Panels
        /// </summary>
        /// <param name="panels">SAM Analytical Panels</param>
        /// <param name="spaces">Topologic Topologies</param>
        /// <param name="tolerance">Tolerance</param>
        /// <search>
        /// Topologic, SpaceAdjacency, Analytical Panel
        /// </search>
        [MultiReturn(new[] { "geometries", "names" })]
        public static Dictionary<string, object> SpaceAdjacency(IEnumerable<SAM.Analytical.Panel> panels, IEnumerable<Space> spaces, double tolerance = SAM.Geometry.Tolerance.MicroDistance)
        {
            List<SAM.Geometry.Spatial.IGeometry3D> geometryList = null;
            List<Tuple<string, int>> tupleList = null;
            SAM.Analytical.Topologic.Query.TryGetSpaceAdjacency(panels, spaces, tolerance, out geometryList, out tupleList);

            return new Dictionary<string, object>
            {
                {"geometries", geometryList },
                {"names", tupleList.ConvertAll(x => x.Item1) }
            };
        }
    }
}

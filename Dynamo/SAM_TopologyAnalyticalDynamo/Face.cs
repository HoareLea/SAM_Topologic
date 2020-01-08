using System.Collections.Generic;

using Topologic;

using Autodesk.DesignScript.Runtime;
using System;

namespace SAMTopologicAnalyticalDynamo
{
    /// <summary>
    /// Topologic Face
    /// </summary>
    public static class Face
    {
        /// <summary>
        /// Extract Space Adjacency information from spaces
        /// </summary>
        /// <param name="faces">Topologic Faces</param>
        /// <param name="topologies">Topologic Topologies</param>
        /// <param name="tolerance">Tolerance</param>
        /// <search>
        /// Topologic, SpaceAdjacency, Face
        /// </search>
        [MultiReturn(new[] { "geometries", "names" })]
        public static Dictionary<string, object> SpaceAdjacency(IEnumerable<Topologic.Face> faces, IEnumerable<Topology> topologies, double tolerance)
        {
            List<SAM.Geometry.Spatial.IGeometry3D> geometryList = null;
            List<Tuple<string, int>> tupleList = null;
            SAM.Analytical.Topologic.Query.TryGetSpaceAdjacency(faces, topologies, tolerance, out geometryList, out tupleList);

            return new Dictionary<string, object>
            {
                {"geometries", geometryList },
                {"names", tupleList.ConvertAll(x => x.Item1) }
            };
        }
    }
}

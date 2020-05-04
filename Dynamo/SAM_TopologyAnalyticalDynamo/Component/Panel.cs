using Autodesk.DesignScript.Runtime;
using SAM.Analytical;
using System.Collections.Generic;

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
        /// <search>Topologic, SpaceAdjacency, Analytical Panel</search>
        [MultiReturn(new[] { "SAMGeometries", "names" })]
        public static Dictionary<string, object> SpaceAdjacency(IEnumerable<SAM.Analytical.Panel> panels, IEnumerable<Space> spaces, double tolerance = SAM.Core.Tolerance.Distance)
        {
            List<SAM.Geometry.Spatial.ISAMGeometry3D> sAMGeometryList = null;
            List<List<string>> names = null;
            SAM.Analytical.Topologic.Query.TryGetSpaceAdjacency(panels, spaces, tolerance, out sAMGeometryList, out names);

            return new Dictionary<string, object>
            {
                {"SAMGeometries", sAMGeometryList },
                {"names", names }
            };
        }

        public static object ToTopology(SAM.Analytical.Panel panel)
        {
            return SAM.Analytical.Topologic.Convert.ToTopologic(panel);
        }
    }
}
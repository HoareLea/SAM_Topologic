using System;
using System.Linq;
using System.Collections.Generic;

using Topologic;

using Autodesk.DesignScript.Runtime;
using SAM.Analytical;

namespace SAMTopologicAnalyticalDynamo
{
    /// <summary>
    /// SAM Topologic AdjacencyCluster
    /// </summary>
    public static class AdjacencyCluster
    {
        public static SAM.Analytical.Topologic.AdjacencyCluster ByPanelsAndSpaces(IEnumerable<SAM.Analytical.Panel> panels, IEnumerable<Space> spaces, double tolerance = SAM.Geometry.Tolerance.MicroDistance, bool updatePanels = true)
        {
            SAM.Analytical.Topologic.AdjacencyCluster adjacencyModel = new SAM.Analytical.Topologic.AdjacencyCluster(spaces, panels);
            adjacencyModel.Calculate(tolerance, updatePanels);
            return adjacencyModel;
        }

        public static IEnumerable<SAM.Analytical.Panel> Panels(SAM.Analytical.Topologic.AdjacencyCluster adjacencyCluster)
        {
            return adjacencyCluster.GetPanels();
        }

        public static IEnumerable<SAM.Analytical.Space> Spaces(SAM.Analytical.Topologic.AdjacencyCluster adjacencyCluster)
        {
            return adjacencyCluster.GetSpaces();
        }

        public static IEnumerable<SAM.Analytical.Panel> InternalPanels(SAM.Analytical.Topologic.AdjacencyCluster adjacencyCluster)
        {
            return adjacencyCluster.GetInternalPanels();
        }

        public static IEnumerable<SAM.Analytical.Panel> ExternalPanels(SAM.Analytical.Topologic.AdjacencyCluster adjacencyCluster)
        {
            return adjacencyCluster.GetExternalPanels();
        }

        public static IEnumerable<SAM.Analytical.Panel> ShadingPanels(SAM.Analytical.Topologic.AdjacencyCluster adjacencyCluster)
        {
            return adjacencyCluster.GetShadingPanels();
        }

        public static Topology Topology(SAM.Analytical.Topologic.AdjacencyCluster adjacencyCluster)
        {
            return adjacencyCluster.Topology;
        }
    }
}

using System;
using System.Linq;
using System.Collections.Generic;

using Topologic;

using Autodesk.DesignScript.Runtime;
using SAM.Analytical;

namespace SAMTopologicAnalyticalDynamo
{
    /// <summary>
    /// SAM Topologic AdjacencyModel
    /// </summary>
    public static class AdjacencyModel
    {
        public static SAM.Analytical.Topologic.AdjacencyModel ByPanelsAndSpaces(IEnumerable<SAM.Analytical.Panel> panels, IEnumerable<Space> spaces, double tolerance = SAM.Geometry.Tolerance.MicroDistance, bool updatePanels = true)
        {
            SAM.Analytical.Topologic.AdjacencyModel adjacencyModel = new SAM.Analytical.Topologic.AdjacencyModel(spaces, panels);
            adjacencyModel.Calculate(tolerance, updatePanels);
            return adjacencyModel;
        }

        public static IEnumerable<SAM.Analytical.Panel> Panels(SAM.Analytical.Topologic.AdjacencyModel adjacencyModel)
        {
            return adjacencyModel.GetPanels();
        }

        public static IEnumerable<SAM.Analytical.Space> Spaces(SAM.Analytical.Topologic.AdjacencyModel adjacencyModel)
        {
            return adjacencyModel.GetSpaces();
        }

        public static IEnumerable<SAM.Analytical.Panel> InternalPanels(SAM.Analytical.Topologic.AdjacencyModel adjacencyModel)
        {
            return adjacencyModel.GetInternalPanels();
        }

        public static IEnumerable<SAM.Analytical.Panel> ExternalPanels(SAM.Analytical.Topologic.AdjacencyModel adjacencyModel)
        {
            return adjacencyModel.GetExternalPanels();
        }

        public static IEnumerable<SAM.Analytical.Panel> ShadingPanels(SAM.Analytical.Topologic.AdjacencyModel adjacencyModel)
        {
            return adjacencyModel.GetShadingPanels();
        }
    }
}

using SAM.Analytical;
using System.Collections.Generic;
using Topologic;

namespace SAMTopologicAnalyticalDynamo
{
    /// <summary>
    /// SAM Topologic AdjacencyCluster
    /// </summary>
    public static class AdjacencyCluster
    {
        /// <summary>
        /// Creates AdjacencyCluster by the panels and spaces.
        /// </summary>
        /// <param name="panels">The SAM panels.</param>
        /// <param name="spaces">The spaces.</param>
        /// <param name="tolerance">The tolerance.</param>
        /// <param name="updatePanels">if set to <c>true</c> [update panels].</param>
        /// <returns name="adjacencyCluster"><see cref="SAM.Analytical.AdjacencyCluster"/></returns>
        public static SAM.Analytical.AdjacencyCluster ByPanelsAndSpaces(IEnumerable<SAM.Analytical.Panel> panels, IEnumerable<Space> spaces, double tolerance = SAM.Core.Tolerance.MacroDistance, bool updatePanels = true)
        {
            Topology topology = null;

            SAM.Analytical.AdjacencyCluster adjacencyModel = SAM.Analytical.Topologic.Create.AdjacencyCluster(spaces, panels, out topology, SAM.Core.Tolerance.MacroDistance, updatePanels, true, null, tolerance);
            return adjacencyModel;
        }

        /// <summary>
        /// Query Panels the specified adjacency cluster.
        /// </summary>
        /// <param name="adjacencyCluster">The adjacency cluster.</param>
        /// <returns name="Panels">SAM Panels</returns>
        /// <search>Topologic, getpanels, GetPanels</search>
        public static IEnumerable<SAM.Analytical.Panel> Panels(SAM.Analytical.AdjacencyCluster adjacencyCluster)
        {
            return adjacencyCluster.GetPanels();
        }

        /// <summary>
        /// Query Spaces from SAM Adjacency cluster.
        /// </summary>
        /// <param name="adjacencyCluster">The adjacency cluster.</param>
        /// <returns name="spaces"> SAM Spaces</returns>
        /// <search>Topologic, QuerySpaces</search>
        public static IEnumerable<Space> Spaces(SAM.Analytical.AdjacencyCluster adjacencyCluster)
        {
            return adjacencyCluster.GetSpaces();
        }

        /// <summary>
        /// Query Internals panels from SAM Adjacency cluster.
        /// </summary>
        /// <param name="adjacencyCluster">The SAM adjacency cluster.</param>
        /// <returns name="panels">List of Panels
        ///   <see cref="IEnumerable{SAM.Analytical.Panel}"/>
        /// </returns>
        /// <search>IntenralPanels, internalpanels</search>
        public static IEnumerable<SAM.Analytical.Panel> InternalPanels(SAM.Analytical.AdjacencyCluster adjacencyCluster)
        {
            return adjacencyCluster.GetInternalPanels();
        }

        /// <summary>
        /// Query Externals the panels from SAM Adjacency cluster.
        /// </summary>
        /// <param name="adjacencyCluster">The SAM adjacency cluster.</param>
        /// <returns>
        ///   <see cref="IEnumerable{SAM.Analytical.Panel}"/>
        /// </returns>
        /// <search>ExternalPanels, externalpanels</search>
        public static IEnumerable<SAM.Analytical.Panel> ExternalPanels(SAM.Analytical.AdjacencyCluster adjacencyCluster)
        {
            return adjacencyCluster.GetExternalPanels();
        }

        /// <summary>
        /// Query Shadings panels from SAM Adjacency cluster.
        /// </summary>
        /// <param name="adjacencyCluster">The adjacency cluster.</param>
        /// <returns name="shadingPanel">Shading Panels
        ///   <see cref="IEnumerable{SAM.Analytical.Panel}"/>
        /// </returns>
        /// <search>ShadingPanels, shadingpanels</search>
        public static IEnumerable<SAM.Analytical.Panel> ShadingPanels(SAM.Analytical.AdjacencyCluster adjacencyCluster)
        {
            return adjacencyCluster.GetShadingPanels();
        }

        /// <summary>
        /// Query Adjacencies/Connected Spaces from the specified adjacency cluster for Panels.
        /// </summary>
        /// <param name="adjacencyCluster">The adjacency cluster.</param>
        /// <param name="sAMObject">The s am object.</param>
        /// <returns name="adjacencies">Adjacencies for Panels</returns>
        /// <search>Adjacencies, adjacencies</search>
        public static IEnumerable<object> Adjacencies(SAM.Analytical.AdjacencyCluster adjacencyCluster, SAM.Core.SAMObject sAMObject)
        {
            if (sAMObject is SAM.Analytical.Panel)
                return adjacencyCluster.GetSpaces((SAM.Analytical.Panel)sAMObject);

            if (sAMObject is Space)
                return adjacencyCluster.GetPanels((Space)sAMObject);

            return null;
        }
    }
}
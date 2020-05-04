using SAM.Analytical;
using System.Collections.Generic;

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
        /// <returns name="adjacencyCluster" > 
        ///   <see cref="SAM.Analytical.Topologic.AdjacencyCluster"/>
        /// </returns>
        public static SAM.Analytical.Topologic.AdjacencyCluster ByPanelsAndSpaces(IEnumerable<SAM.Analytical.Panel> panels, IEnumerable<Space> spaces, double tolerance = SAM.Core.Tolerance.MacroDistance, bool updatePanels = true)
        {
            SAM.Analytical.Topologic.AdjacencyCluster adjacencyModel = new SAM.Analytical.Topologic.AdjacencyCluster(spaces, panels);
            adjacencyModel.Calculate(tolerance, updatePanels);
            return adjacencyModel;
        }

        /// <summary>
        /// Query Panels the specified adjacency cluster.
        /// </summary>
        /// <param name="adjacencyCluster">The adjacency cluster.</param>
        /// <returns name="Panels">SAM Panels</returns>
        /// <search>Topologic, getpanels, GetPanels</search>
        public static IEnumerable<SAM.Analytical.Panel> Panels(SAM.Analytical.Topologic.AdjacencyCluster adjacencyCluster)
        {
            return adjacencyCluster.GetPanels();
        }

        /// <summary>
        /// Query Spaces from SAM Adjacency cluster.
        /// </summary>
        /// <param name="adjacencyCluster">The adjacency cluster.</param>
        /// <returns name="spaces"> SAM Spaces</returns>
        /// <search>Topologic, QuerySpaces</search>
        public static IEnumerable<SAM.Analytical.Space> Spaces(SAM.Analytical.Topologic.AdjacencyCluster adjacencyCluster)
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
        public static IEnumerable<SAM.Analytical.Panel> InternalPanels(SAM.Analytical.Topologic.AdjacencyCluster adjacencyCluster)
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
        public static IEnumerable<SAM.Analytical.Panel> ExternalPanels(SAM.Analytical.Topologic.AdjacencyCluster adjacencyCluster)
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
        public static IEnumerable<SAM.Analytical.Panel> ShadingPanels(SAM.Analytical.Topologic.AdjacencyCluster adjacencyCluster)
        {
            return adjacencyCluster.GetShadingPanels();
        }

        /// <summary>
        /// Query Topologies the specified adjacency cluster.
        /// </summary>
        /// <param name="adjacencyCluster">The adjacency cluster.</param>
        /// <returns name="Topology">Topology objects</returns>
        /// <search>Topology, topology</search>
        public static object Topology(SAM.Analytical.Topologic.AdjacencyCluster adjacencyCluster)
        {
            return adjacencyCluster.Topology;
        }

        /// <summary>
        /// Query Adjacencies/Connected Spaces from the specified adjacency cluster for Panels.
        /// </summary>
        /// <param name="adjacencyCluster">The adjacency cluster.</param>
        /// <param name="sAMObject">The s am object.</param>
        /// <returns name="adjacencies">Adjacencies for Panels</returns>
        /// <search>Adjacencies, adjacencies</search>
        public static IEnumerable<object> Adjacencies(SAM.Analytical.Topologic.AdjacencyCluster adjacencyCluster, SAM.Core.SAMObject sAMObject)
        {
            if (sAMObject is SAM.Analytical.Panel)
                return adjacencyCluster.GetPanelSpaces(sAMObject.Guid);

            if (sAMObject is Space)
                return adjacencyCluster.GetSpacePanels(sAMObject.Guid);

            return null;
        }
    }
}
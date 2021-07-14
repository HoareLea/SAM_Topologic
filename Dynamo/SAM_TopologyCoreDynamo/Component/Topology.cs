using System.Collections.Generic;
using System.Linq;

namespace SAM_TopologicCoreDynamo
{
    public static class Topology
    {
        public static string Analyze(object topology)
        {
            return ((Topologic.Topology)topology).Analyze();
        }

        public static IEnumerable<object> ToCellComplex(object topology)
        {
            return ((Topologic.Topology)topology).CellComplexes;
        }

        public static IEnumerable<object> ToCells(object topology)
        {
            return ((Topologic.Topology)topology).Cells;
        }

        public static IEnumerable<object> ToFaces(object topology)
        {
            return ((Topologic.Topology)topology).Faces;
        }

        public static List<Topologic.Wire> ToWires(Topologic.Topology topology)
        {
            return topology?.Wires?.ToList();
        }

        public static List<Topologic.Vertex> ToVertices(Topologic.Topology topology)
        {
            return topology?.Vertices?.ToList();
        }
    }
}
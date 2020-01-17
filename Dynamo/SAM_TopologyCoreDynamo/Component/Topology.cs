using System.Collections.Generic;

namespace SAM_TopologicCoreDynamo
{
    public static class Topology
    {
        public static string Analyze(Topologic.Topology topology)
        {
            return topology.Analyze();
        }

        public static List<Topologic.CellComplex> ToCellComplex(Topologic.Topology topology)
        {
            return topology.CellComplexes;
        }

        public static List<Topologic.Cell> ToCells(Topologic.Topology topology)
        {
            return topology.Cells;
        }

        public static List<Topologic.Face> ToFaces(Topologic.Topology topology)
        {
            return topology.Faces;
        }

        public static List<Topologic.Wire> ToWires(Topologic.Topology topology)
        {
            return topology.Wires;
        }

        public static List<Topologic.Vertex> ToVertices(Topologic.Topology topology)
        {
            return topology.Vertices;
        }
    }
}

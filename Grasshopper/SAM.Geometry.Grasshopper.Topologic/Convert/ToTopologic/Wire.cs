using Rhino.Geometry;
using System.Collections.Generic;
using Topologic;

namespace SAM.Geometry.Grasshopper.Topologic
{
    public static partial class Convert
    {
        public static Wire ToTopologic(this PolyCurve polyCurve)
        {
            Curve[] curves = polyCurve.Explode();
            List<Edge> edges = new List<Edge>();
            foreach (Curve curve in curves)
            {
                Topology topology = curve.ToTopologic();

                Edge edge = topology as Edge;
                if (edge != null)
                {
                    edges.Add(edge);
                    continue;
                }

                Wire wire = topology as Wire;
                if (wire != null)
                {
                    edges.AddRange(wire.Edges);
                    continue;
                }
            }

            return Wire.ByEdges(edges);
        }

        public static Wire ToTopologic(PolylineCurve polylineCurve)
        {
            int count = polylineCurve.PointCount;
            if (count < 1)
            {
                return null;
            }

            List<Vertex> vertices = new List<Vertex>();
            List<int> indices = new List<int>();
            for (int i = 0; i < count; ++i)
            {
                Vertex vertex = polylineCurve.Point(i).ToTopologic();
                vertices.Add(vertex);
                indices.Add(i);
            }

            if (polylineCurve.IsClosed)
            {
                List<IList<int>> listOfIndices = new List<IList<int>>();
                listOfIndices.Add(indices);
                return Topology.ByVerticesIndices(vertices, listOfIndices)[0].Wires[0];
            }
            else
            {
                List<IList<int>> listOfIndices = new List<IList<int>>();
                listOfIndices.Add(indices);
                return Topology.ByVerticesIndices(vertices, listOfIndices)[0] as Wire;
            }
        }
    }
}
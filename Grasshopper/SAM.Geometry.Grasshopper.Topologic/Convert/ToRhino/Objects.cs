using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using Topologic;

namespace SAM.Geometry.Grasshopper.Topologic
{
    public static partial class Convert
    {
        public static List<object> ToRhino(this Topology topology, double tolerance = Core.Tolerance.Distance)
        {
            if (topology == null)
            {
                return null;
            }

            List<Object> geometries = new List<Object>();
            Vertex vertex = topology as Vertex;
            if (vertex != null)
            {
                geometries.Add(ToRhino(vertex));
                return geometries;
            }

            Edge edge = topology as Edge;
            if (edge != null)
            {
                geometries.Add(ToRhino(edge));
                return geometries;
            }

            Wire wire = topology as Wire;
            if (wire != null)
                return ToRhino(wire)?.Cast<object>().ToList();

            global::Topologic.Face face = topology as global::Topologic.Face;
            if (face != null)
            {
                geometries.Add(ToRhino(face, tolerance));
                return geometries;
            }

            Shell shell = topology as Shell;
            if (shell != null)
            {
                return ToRhino_Breps(shell, tolerance)?.Cast<object>().ToList();
            }

            Cell cell = topology as Cell;
            if (cell != null)
            {
                return ToRhino_Breps(cell, tolerance)?.Cast<object>().ToList();
            }

            CellComplex cellComplex = topology as CellComplex;
            if (cellComplex != null)
            {
                return ToRhino_Breps((CellComplex)cellComplex, tolerance)?.Cast<object>().ToList();
            }

            Cluster cluster = topology as Cluster;
            if (cluster != null)
            {
                return ToRhino(cluster.SubTopologies, tolerance);
            }

            Aperture aperture = topology as Aperture;
            if (aperture != null)
            {
                return ToRhino(aperture.Topology, tolerance);
            }

            throw new Exception("The type of the input topology is not recognized.");
        }

        public static List<object> ToRhino(this IEnumerable<Topology> topologies, double tolerance = Core.Tolerance.Distance)
        {
            List<object> result = new List<object>();
            foreach (Topology subTopology in topologies)
            {
                object ghGeometry = ToRhino(subTopology, tolerance);

                List<object> ghGeometryAsList = ghGeometry as List<object>;
                if (ghGeometryAsList != null)
                {
                    result.AddRange(ghGeometryAsList);
                }
                else
                {
                    result.Add(ghGeometry);
                }
            }
            return result;
        }
    }
}

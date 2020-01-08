using System.Linq;
using System.Collections.Generic;

using Topologic;
using System;

namespace SAM.Analytical.Topologic
{
    public static partial class Query
    {
        public static bool TryGetSpaceAdjacency(this IEnumerable<Panel> panels, IEnumerable<Space> spaces, double tolerance, out List<Geometry.Spatial.IGeometry3D> geometryList, out List<Tuple<string, int>> tupleList)
        {
            List<Topology> topologyList = new List<Topology>();
            foreach (Space space in spaces)
            {
                if (space == null)
                    continue;

                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                dictionary["Name"] = space.Name;

                Vertex vertex = Geometry.Topologic.Convert.ToTopologic(space.Location);
                vertex = (Vertex)vertex.SetDictionary(dictionary);
                topologyList.Add(vertex);
            }

            List<Face> faceList = new List<Face>();
            foreach (Panel panel in panels)
            {
                if (panel == null)
                    continue;

                Face face = Analytical.Topologic.Convert.ToTopologic(panel);
                if (face == null)
                    continue;

                faceList.Add(face);
            }

            return TryGetSpaceAdjacency(faceList, topologyList, tolerance, out geometryList, out tupleList);
        }

        private static bool TryGetSpaceAdjacency(this IEnumerable<Face> faces, IEnumerable<Topology> topologies, double tolerance, out List<Geometry.Spatial.IGeometry3D> geometryList, out List<Tuple<string, int>> tupleList)
        {
            CellComplex cellComplex = CellComplex.ByFaces(faces, tolerance);
            if (cellComplex == null)
            {
                geometryList = null;
                tupleList = null;
                return false;
            }

            if (topologies != null)
                cellComplex = (CellComplex)cellComplex.AddContents(topologies.ToList(), 32);

            int index = 0;

            tupleList = new List<Tuple<string, int>>();
            geometryList = new List<Geometry.Spatial.IGeometry3D>();
            foreach (Face face in cellComplex.Faces)
            {
                geometryList.Add(Geometry.Topologic.Convert.ToSAM(face));
                foreach (Cell cell in face.Cells)
                {
                    foreach (Topology topology in cell.Contents)
                    {
                        Vertex vertex = topology as Vertex;
                        if (vertex == null)
                            continue;

                        tupleList.Add(new Tuple<string, int>(vertex.Dictionary["Name"] as string, index));
                    }
                }

                index++;
            }

            return true;
        }

    }
}


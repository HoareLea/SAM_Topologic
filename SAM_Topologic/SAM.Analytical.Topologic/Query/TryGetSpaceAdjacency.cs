using System.Linq;
using System.Collections.Generic;

using Topologic;


namespace SAM.Analytical.Topologic
{
    public static partial class Query
    {
        public static bool TryGetSpaceAdjacency(IEnumerable<Face> faces, IEnumerable<Topology> topologies, double tolerance, out List<Geometry.Spatial.IGeometry3D> geometryList, out Dictionary<string, int> dictionary)
        {
            CellComplex cellComplex = CellComplex.ByFaces(faces, tolerance);
            if (cellComplex == null)
            {
                geometryList = null;
                dictionary = null;
                return false;
            }
                

            if (topologies != null)
                cellComplex = (CellComplex)cellComplex.AddContents(topologies.ToList(), 32);

            int index = 0;

            dictionary = new Dictionary<string, int>();
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

                        dictionary[vertex.Dictionary["Name"] as string] = index;
                    }
                }

                index++;
            }

            return true;
        }
    }
}


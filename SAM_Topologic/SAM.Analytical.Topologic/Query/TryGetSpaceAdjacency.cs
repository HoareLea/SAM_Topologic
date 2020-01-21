using System.Linq;
using System.Collections.Generic;

using Topologic;
using System;

namespace SAM.Analytical.Topologic
{
    public static partial class Query
    {
        public static bool TryGetSpaceAdjacency(this IEnumerable<Panel> panels, IEnumerable<Space> spaces, double tolerance, out List<Geometry.Spatial.IGeometry3D> geometryList, out List<List<string>> names)
        {

            Geometry.Spatial.BoundingBox3D boundingBox3D = null;
            
            List<Face> faceList = new List<Face>();
            foreach (Panel panel in panels)
            {
                if (panel == null)
                    continue;

                Face face = Convert.ToTopologic(panel);
                if (face == null)
                    continue;

                if (boundingBox3D == null)
                {
                    boundingBox3D = panel.GetBoundingBox();
                }               
                else
                {
                    Geometry.Spatial.BoundingBox3D boundingBox3D_Temp = panel.GetBoundingBox();
                    if (boundingBox3D_Temp != null)
                        boundingBox3D = new Geometry.Spatial.BoundingBox3D(new Geometry.Spatial.BoundingBox3D[] { boundingBox3D, boundingBox3D_Temp });
                }

                faceList.Add(face);
            }

            List<Topology> topologyList = new List<Topology>();
            foreach (Space space in spaces)
            {
                if (space == null)
                    continue;

                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                dictionary["Name"] = space.Name;

                Geometry.Spatial.Point3D point3D = space.Location;
                if (point3D.Z - boundingBox3D.Max.Z >= 0)
                    point3D = (Geometry.Spatial.Point3D)point3D.GetMoved(new Geometry.Spatial.Vector3D(0, 0, (boundingBox3D.Max.Z - boundingBox3D.Min.Z) / 2));

                if (point3D.Z - boundingBox3D.Min.Z <= 0)
                    point3D = (Geometry.Spatial.Point3D)point3D.GetMoved(new Geometry.Spatial.Vector3D(0, 0, (boundingBox3D.Max.Z - boundingBox3D.Min.Z) / 2));

                Vertex vertex = Geometry.Topologic.Convert.ToTopologic(point3D);
                vertex = (Vertex)vertex.SetDictionary(dictionary);
                topologyList.Add(vertex);
            }

            return TryGetSpaceAdjacency(faceList, topologyList, tolerance, out geometryList, out names);
        }

        private static bool TryGetSpaceAdjacency(this IEnumerable<Face> faces, IEnumerable<Topology> topologies, double tolerance, out List<Geometry.Spatial.IGeometry3D> geometryList, out List<List<string>> names)
        {
            CellComplex cellComplex = CellComplex.ByFaces(faces, tolerance);
            if (cellComplex == null)
            {
                geometryList = null;
                names = null;
                return false;
            }

            if (topologies != null)
                cellComplex = (CellComplex)cellComplex.AddContents(topologies.ToList(), 32);

            int index = 0;

            names = new List<List<string>>();

            geometryList = new List<Geometry.Spatial.IGeometry3D>();
            foreach (Face face in cellComplex.Faces)
            {
                List<string> nameList = new List<string>();
                names.Add(nameList);

                geometryList.Add(Geometry.Topologic.Convert.ToSAM(face));
                foreach (Cell cell in face.Cells)
                {
                    foreach (Topology topology in cell.Contents)
                    {
                        Vertex vertex = topology as Vertex;
                        if (vertex == null)
                            continue;

                        nameList.Add(vertex.Dictionary["Name"] as string);
                    }
                }

                index++;
            }

            return true;
        }
    }
}


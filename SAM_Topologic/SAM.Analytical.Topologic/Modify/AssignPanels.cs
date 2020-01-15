using System.Linq;
using System.Collections.Generic;

using Topologic;
using System;

namespace SAM.Analytical.Topologic
{
    public static partial class Modify
    {
        public static IEnumerable<Space> AssignPanels(this IEnumerable<Space> spaces, IEnumerable<Panel> panels, double tolerance = Geometry.Tolerance.MacroDistance)
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

                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                dictionary["Panel"] = panel.Guid.ToString();
                face = (Face)face.SetDictionary(dictionary);

                faceList.Add(face);
            }

            if (faceList == null || faceList.Count == 0)
                return null;

            List<Topology> topologyList = new List<Topology>();
            foreach (Space space in spaces)
            {
                if (space == null)
                    continue;

                Geometry.Spatial.Point3D point3D = space.Location;
                if (point3D.Z - boundingBox3D.Max.Z >= 0)
                    point3D = point3D.GetMoved(new Geometry.Spatial.Vector3D(0, 0, (boundingBox3D.Max.Z - boundingBox3D.Min.Z) / 2));

                if (point3D.Z - boundingBox3D.Min.Z <= 0)
                    point3D = point3D.GetMoved(new Geometry.Spatial.Vector3D(0, 0, (boundingBox3D.Max.Z - boundingBox3D.Min.Z) / 2));

                Vertex vertex = Geometry.Topologic.Convert.ToTopologic(point3D);

                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                dictionary["Space"] = space.Guid.ToString();
                vertex = (Vertex)vertex.SetDictionary(dictionary);
                topologyList.Add(vertex);
            }

            if (topologyList == null || topologyList.Count == 0)
                return null;

            CellComplex cellComplex = CellComplex.ByFaces(faceList, tolerance);
            if (cellComplex == null)
                return null;

            if (topologyList != null)
                cellComplex = (CellComplex)cellComplex.AddContents(topologyList, 32);

            List<Space> result = new List<Space>();
            foreach (Cell cell in cellComplex.Cells)
            {
                Space space_Old = null;

                foreach (Topology topology in cell.Contents)
                {
                    Vertex vertex = topology as Vertex;
                    if (vertex == null)
                        continue;

                    string value = vertex.Dictionary["Space"] as string;
                    Guid guid_Space;
                    if (!Guid.TryParse(value, out guid_Space))
                        continue;

                    space_Old = spaces.ToList().Find(x => x.Guid.Equals(guid_Space));
                    break;
                }

                if (space_Old == null)
                    continue;

                List<Panel> panelList = new List<Panel>();
                foreach(Face face_New in cell.Faces)
                {
                    Vertex vertex = global::Topologic.Utilities.FaceUtility.InternalVertex(face_New, tolerance);
                    if (vertex == null)
                        continue;

                    Face face_Old = null;
                    foreach(Face face in faceList)
                    {
                        if(global::Topologic.Utilities.FaceUtility.IsInside(face, vertex, tolerance))
                        {
                            face_Old = face;
                            break;
                        }
                    }

                    if (face_Old == null)
                        continue;

                    string value = face_Old.Dictionary["Panel"] as string;
                    Guid guid_panel;
                    if (!Guid.TryParse(value, out guid_panel))
                        continue;
                    
                    Panel panel = panels.ToList().Find(x => x.Guid.Equals(guid_panel));
                    if (panel == null)
                        continue;

                    panelList.Add(new Panel(Guid.NewGuid(), panel, Geometry.Topologic.Convert.ToSAM(face_New)));
                }

                Space space_New = new Space(space_Old.Guid, space_Old, panelList);
                if (space_New != null)
                    result.Add(space_New);
            }

            return result;
        }
    }
}


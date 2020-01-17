using SAM.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topologic;

namespace SAM.Analytical.Topologic
{
    public class AdjacencyCluster
    {
        private Topology topology;
        
        private Dictionary<Type, Dictionary<Guid, SAMObject>> dictionary_SAMObjects;
        private Dictionary<Type, Dictionary<Guid, HashSet<Guid>>> dictionary_Relations;

        public AdjacencyCluster()
        {
            dictionary_SAMObjects = new Dictionary<Type, Dictionary<Guid, SAMObject>>();
            dictionary_Relations = new Dictionary<Type, Dictionary<Guid, HashSet<Guid>>>();
        }

        public AdjacencyCluster(IEnumerable<Space> spaces, IEnumerable<Panel> panels)
        {
            dictionary_SAMObjects = new Dictionary<Type, Dictionary<Guid, SAMObject>>();
            dictionary_Relations = new Dictionary<Type, Dictionary<Guid, HashSet<Guid>>>();

            foreach (Space space in spaces)
                Add(space);

            foreach (Panel panel in panels)
                Add(panel);
        }


        private IEnumerable<T> GetSAMObjects<T>()
        {
            return dictionary_SAMObjects[typeof(T)].Values.Cast<T>();
        }

        private bool Add(SAMObject sAMObject)
        {
            if (sAMObject == null)
                return false;

            Dictionary<Guid, SAMObject> dictionary;
            if(!dictionary_SAMObjects.TryGetValue(sAMObject.GetType(), out dictionary))
            {
                dictionary = new Dictionary<Guid, SAMObject>();
                dictionary_SAMObjects[sAMObject.GetType()] = dictionary;
            }

            dictionary[sAMObject.Guid] = sAMObject;
            return true;
        }


        public bool Calculate(double tolerance = Geometry.Tolerance.MacroDistance, bool updatePanels = true)
        {
            topology = null;

            dictionary_Relations = new Dictionary<Type, Dictionary<Guid, HashSet<Guid>>>();

            Geometry.Spatial.BoundingBox3D boundingBox3D = null;

            List<Face> faceList = new List<Face>();

            Dictionary<Guid, SAMObject> dictionary_Panel = dictionary_SAMObjects[typeof(Panel)];
            if (dictionary_Panel == null || dictionary_Panel.Count == 0)
                return false;

            foreach (Panel panel in dictionary_Panel.Values)
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
                return false;

            List<Topology> topologyList = new List<Topology>();
            Dictionary<Guid, SAMObject> dictionary_Space = dictionary_SAMObjects[typeof(Space)];
            if (dictionary_Space != null && dictionary_Space.Count > 0)
            {
                foreach (Space space in dictionary_Space.Values)
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
            }


            if (topologyList == null || topologyList.Count == 0)
                return false;

            List<CellComplex> cellComplexList = null;
            try
            {
                Cluster cluster = Cluster.ByTopologies(faceList);
                topology = cluster.SelfMerge();

                cellComplexList = topology.CellComplexes;

                //cellComplex = CellComplex.ByFaces(faceList, tolerance);
            }
            catch(Exception exception)
            {
                cellComplexList = null;
            }

            if (cellComplexList == null)
                return false;

            //Michal D. Indea
            if (cellComplexList.Count != 1)
                return false;

            CellComplex cellComplex = cellComplexList[0];

            if (topologyList != null)
                cellComplex = (CellComplex)cellComplex.AddContents(topologyList, 32);

            Dictionary<Guid, HashSet<Guid>> dictionary_Relations_Panel;
            if (!dictionary_Relations.TryGetValue(typeof(Panel), out dictionary_Relations_Panel))
            {
                dictionary_Relations_Panel = new Dictionary<Guid, HashSet<Guid>>();
                dictionary_Relations[typeof(Panel)] = dictionary_Relations_Panel;
            }

            Dictionary<Guid, HashSet<Guid>> dictionary_Relations_Space;
            if (!dictionary_Relations.TryGetValue(typeof(Space), out dictionary_Relations_Space))
            {
                dictionary_Relations_Space = new Dictionary<Guid, HashSet<Guid>>();
                dictionary_Relations[typeof(Space)] = dictionary_Relations_Space;
            }

            foreach (Face face_New in cellComplex.Faces)
            {
                Vertex vertex = global::Topologic.Utilities.FaceUtility.InternalVertex(face_New, tolerance);
                if (vertex == null)
                    continue;

                Face face_Old = null;
                foreach (Face face in faceList)
                {
                    if (global::Topologic.Utilities.FaceUtility.IsInside(face, vertex, tolerance))
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

                Panel panel_Old = (Panel)dictionary_Panel[guid_panel];
                if (panel_Old == null)
                    continue;

                Panel panel_New = new Panel(panel_Old.Guid, panel_Old, Geometry.Topologic.Convert.ToSAM(face_New));
                if (updatePanels)
                    dictionary_SAMObjects[typeof(Panel)][panel_Old.Guid] = panel_New;

                if (dictionary_Space == null || dictionary_Space.Count == 0)
                    continue;

                HashSet<Guid> guids_Space;
                if (!dictionary_Relations_Panel.TryGetValue(panel_New.Guid, out guids_Space))
                {
                    guids_Space = new HashSet<Guid>();
                    dictionary_Relations_Panel[panel_New.Guid] = guids_Space;
                }

                foreach (Cell cell in face_New.Cells)
                {
                    Space space = null;

                    foreach (Topology topology in cell.Contents)
                    {
                        Vertex vertex_Space = topology as Vertex;
                        if (vertex_Space == null)
                            continue;

                        string value_Space = vertex_Space.Dictionary["Space"] as string;
                        Guid guid_Space;
                        if (!Guid.TryParse(value_Space, out guid_Space))
                            continue;

                        space = (Space)dictionary_Space[guid_Space];
                        break;
                    }

                    if (space == null)
                        continue;

                    guids_Space.Add(space.Guid);

                    HashSet<Guid> guids_Panel;
                    if (!dictionary_Relations_Space.TryGetValue(space.Guid, out guids_Panel))
                    {
                        guids_Panel = new HashSet<Guid>();
                        dictionary_Relations_Space[space.Guid] = guids_Panel;
                    }

                    guids_Panel.Add(panel_New.Guid);
                }
            }

            return true;
        }

        public bool Add(Panel panel)
        {
            if (panel == null)
                return false;
            
            return Add((SAMObject)panel);
        }

        public bool Add(Space space)
        {
            if (space == null)
                return false;

            return Add((SAMObject)space);
        }

        public IEnumerable<Panel> GetPanels()
        {
            return GetSAMObjects<Panel>();
        }

        public IEnumerable<Space> GetSpaces()
        {
            return GetSAMObjects<Space>();
        }

        public List<Panel> GetSpacePanels(Guid guid)
        {
            HashSet<Guid> guids;
            if (!dictionary_Relations[typeof(Space)].TryGetValue(guid, out guids))
                return null;

            List<Panel> panels = new List<Panel>();
            foreach (Guid guid_Temp in guids)
                panels.Add((Panel)dictionary_SAMObjects[typeof(Panel)][guid_Temp]);

            return panels;
        }

        public List<Panel> GetSpacePanels(Space space)
        {
            if (space == null)
                return null;

            return GetSpacePanels(space.Guid);
        }

        public List<Space> GetPanelSpaces(Guid guid)
        {
            HashSet<Guid> guids;
            if (!dictionary_Relations[typeof(Panel)].TryGetValue(guid, out guids))
                return null;

            List<Space> spaces = new List<Space>();
            foreach (Guid guid_Temp in guids)
                spaces.Add((Space)dictionary_SAMObjects[typeof(Space)][guid_Temp]);

            return spaces;
        }

        public List<Space> GetPanelSpaces(Panel panel)
        {
            return GetPanelSpaces(panel.Guid);
        }

        public List<Panel> GetInternalPanels()
        {
            List<Panel> panels = new List<Panel>();
            foreach (KeyValuePair<Guid, HashSet<Guid>> keyValuePair in dictionary_Relations[typeof(Panel)])
            {
                if (keyValuePair.Value.Count < 2)
                    continue;

                panels.Add((Panel)dictionary_SAMObjects[typeof(Panel)][keyValuePair.Key]);
            }

            return panels;
        }

        public List<Panel> GetExternalPanels()
        {
            List<Panel> panels = new List<Panel>();
            foreach (KeyValuePair<Guid, HashSet<Guid>> keyValuePair in dictionary_Relations[typeof(Panel)])
            {
                if (keyValuePair.Value.Count > 1)
                    continue;

                panels.Add((Panel)dictionary_SAMObjects[typeof(Panel)][keyValuePair.Key]);
            }

            return panels;
        }

        public List<Panel> GetShadingPanels()
        {
            Dictionary<Guid, HashSet<Guid>> dictionary;
            if(!dictionary_Relations.TryGetValue(typeof(Panel), out dictionary))
                return dictionary_SAMObjects[typeof(Panel)].Values.Cast<Panel>().ToList();


            List<Panel> panels = new List<Panel>();
            foreach (Panel panel in dictionary_SAMObjects[typeof(Panel)].Values)
            {
                HashSet<Guid> guids;
                if (dictionary.TryGetValue(panel.Guid, out guids))
                    if (guids != null && guids.Count > 0)
                        continue;

                panels.Add(panel);
            }

            return panels;
        }

        public Topology Topology
        {
            get
            {
                return topology;
            }
        }
    }
}

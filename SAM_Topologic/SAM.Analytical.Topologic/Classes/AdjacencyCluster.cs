using System;
using System.Collections.Generic;
using System.Linq;

using Topologic;

using SAM.Core;

namespace SAM.Analytical.Topologic
{
    public class AdjacencyCluster : SAMObject
    {
        private string reportPath;
        
        private Topology topology;
        
        private Dictionary<Type, Dictionary<Guid, SAMObject>> dictionary_SAMObjects;
        private Dictionary<Type, Dictionary<Guid, HashSet<Guid>>> dictionary_Relations;

        public AdjacencyCluster()
        {
            topology = null;
            dictionary_SAMObjects = new Dictionary<Type, Dictionary<Guid, SAMObject>>();
            dictionary_Relations = new Dictionary<Type, Dictionary<Guid, HashSet<Guid>>>();
        }

        public AdjacencyCluster(IEnumerable<Space> spaces, IEnumerable<Panel> panels)
        {
            topology = null;
            
            dictionary_SAMObjects = new Dictionary<Type, Dictionary<Guid, SAMObject>>();
            dictionary_Relations = new Dictionary<Type, Dictionary<Guid, HashSet<Guid>>>();

            if(spaces != null)
            {
                foreach (Space space in spaces)
                    Add(space);
            }


            foreach (Panel panel in panels)
                Add(panel);
        }


        private IEnumerable<T> GetSAMObjects<T>()
        {
            Type type = typeof(T);

            Dictionary<Guid, SAMObject> dictionary;
            if (!dictionary_SAMObjects.TryGetValue(type, out dictionary))
                return null;
            
            return dictionary_SAMObjects[type].Values.Cast<T>();
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

        private bool Report(params object[] values)
        {
            if (string.IsNullOrWhiteSpace(reportPath))
                return false;

            string value = null;

            if(values != null)
            {
                List<string> valueList = new List<string>();
                foreach(object @object in values)
                {
                    if(@object == null)
                        valueList.Add(string.Empty);
                    else
                        valueList.Add(@object.ToString());
                }

                value = string.Join("\t", valueList);
            }

            if (value == null)
                value = string.Empty;

            value = string.Format("[{0}]\t {1}", DateTime.Now.ToString("HH:mm:ss tt"), value);

            System.IO.File.AppendAllText(reportPath, value + "\n");
            return true;
        }


        public bool Calculate(double tolerance = Geometry.Tolerance.MacroDistance, bool updatePanels = true)
        {

            Report(string.Format("Method Name: {0}, Tolerance: {1}, Update Panels: {2}", "Calculate", tolerance, updatePanels));

            try
            {
                topology = null;

                dictionary_Relations = new Dictionary<Type, Dictionary<Guid, HashSet<Guid>>>();

                Geometry.Spatial.BoundingBox3D boundingBox3D = null;

                List<Face> faceList = new List<Face>();

                Dictionary<Guid, SAMObject> dictionary_Panel = dictionary_SAMObjects[typeof(Panel)];
                if (dictionary_Panel == null || dictionary_Panel.Count == 0)
                    return false;

                int index = 1;
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
                    Report(string.Format("Face {0:D4} added. Panel [{1}]", index, panel.Guid));
                    index++;
                }

                if (faceList == null || faceList.Count == 0)
                    return false;

                List<Topology> topologyList = new List<Topology>();

                index = 1;
                Dictionary<Guid, SAMObject> dictionary_Space;
                if (dictionary_SAMObjects.TryGetValue(typeof(Space), out dictionary_Space) && dictionary_Space != null && dictionary_Space.Count > 0)
                {
                    foreach (Space space in dictionary_Space.Values)
                    {
                        if (space == null)
                            continue;

                        Geometry.Spatial.Point3D point3D = space.Location;
                        if (point3D.Z - boundingBox3D.Max.Z >= 0)
                            point3D = (Geometry.Spatial.Point3D)point3D.GetMoved(new Geometry.Spatial.Vector3D(0, 0, (boundingBox3D.Max.Z - boundingBox3D.Min.Z) / 2));

                        if (point3D.Z - boundingBox3D.Min.Z <= 0)
                            point3D = (Geometry.Spatial.Point3D)point3D.GetMoved(new Geometry.Spatial.Vector3D(0, 0, (boundingBox3D.Max.Z - boundingBox3D.Min.Z) / 2));

                        Vertex vertex = Geometry.Topologic.Convert.ToTopologic(point3D);

                        Dictionary<string, object> dictionary = new Dictionary<string, object>();
                        dictionary["Space"] = space.Guid.ToString();
                        vertex = (Vertex)vertex.SetDictionary(dictionary);
                        topologyList.Add(vertex);

                        Report(string.Format("Vertex added. Space {0:D4} [{1}] {2}", index, space.Guid, point3D.ToString()));
                        index++;
                    }
                }

                if (dictionary_Space == null)
                    dictionary_Space = new Dictionary<Guid, SAMObject>();



                List<CellComplex> cellComplexList = null;
                try
                {
                    Report(string.Format("Trying to make CellComplex By Cells"));
                    Cluster cluster = Cluster.ByTopologies(faceList);
                    Report(string.Format("Cluster.ByTopologies Done"));
                    topology = cluster.SelfMerge();
                    Report(string.Format("Cluster SelfMerge Done"));
                    if (topology.Cells == null || topology.Cells.Count == 0)
                        topology = null;
                    else
                        topology = CellComplex.ByCells(topology.Cells);

                    cellComplexList = new List<CellComplex>() { (CellComplex)topology };
                    Report(string.Format("CellComplex By Cells"));
                }
                catch (Exception exception)
                {
                    Report(string.Format("Cannot create CellComplex By Cells"));
                    Report(string.Format("Exception Message: {0}", exception.Message));
                    cellComplexList = null;
                }


                if (topology == null)
                {
                    try
                    {
                        topology = CellComplex.ByFaces(faceList, tolerance);
                        cellComplexList = new List<CellComplex>() { (CellComplex)topology };
                        Report(string.Format("CellComplex By Faces"));
                    }
                    catch (Exception exception)
                    {
                        Report(string.Format("Cannot create CellComplex By Faces"));
                        Report(string.Format("Exception Message: {0}", exception.Message));

                        cellComplexList = null;
                    }
                }

                if (cellComplexList == null)
                    return false;

                //Michal D. Indea
                if (cellComplexList.Count != 1)
                    return false;

                CellComplex cellComplex = cellComplexList[0];
                

                if(dictionary_Space.Count == 0)
                {
                    List<Vertex> vertices = cellComplex.Cells.ConvertAll(x => global::Topologic.Utilities.CellUtility.InternalVertex(x, Geometry.Tolerance.MacroDistance));
                    index = 1;

                    Dictionary<Guid, SAMObject> dictionary_SAMObjects_Space;
                    if (!dictionary_SAMObjects.TryGetValue(typeof(Space), out dictionary_SAMObjects_Space))
                    {
                        dictionary_SAMObjects_Space = new Dictionary<Guid, SAMObject>();
                        dictionary_SAMObjects[typeof(Space)] = dictionary_SAMObjects_Space;
                    }

                    foreach(Vertex vertex in vertices)
                    {
                        Space space = new Space("Cell " + index, Geometry.Topologic.Convert.ToSAM(vertex));
                        dictionary_Space[space.Guid] = space;
                        index++;

                        dictionary_SAMObjects_Space[space.Guid] = space;

                        Dictionary<string, object> dictionary = new Dictionary<string, object>();
                        dictionary["Space"] = space.Guid.ToString();
                        Vertex vertex_Temp = (Vertex)vertex.SetDictionary(dictionary);
                        topologyList.Add(vertex_Temp);
                    }
                        
                }

                Report(string.Format("Single CellComplex created"));

                if (topologyList != null && topologyList.Count > 0)
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

                Report(string.Format("Dictionaries created"));

                HashSet<Guid> guids_Updated = new HashSet<Guid>();
                Dictionary<Guid, SAMObject> dictionary_Panel_New = new Dictionary<Guid, SAMObject>();
                foreach (Face face_New in cellComplex.Faces)
                {
                    Report(string.Format("Analyzing face and looking for Internal vertex"));
                    //global::Topologic.Utilities.FaceUtility.InternalVertex(face_New, tolerance);
                    //global::Topologic.Utilities.FaceUtility.VertexAtParameters(face_New, 0.5, 0.5)
                    Vertex vertex = global::Topologic.Utilities.FaceUtility.VertexAtParameters(face_New, 0.5, 0.5);//global::Topologic.Utilities.FaceUtility.InternalVertex(face_New, tolerance);
                    if (vertex == null)
                        continue;

                    Report(string.Format("Vertex for face found:", string.Format("X={0};Y={1};Z={2}", vertex.X, vertex.Y, vertex.Z)));

                    Face face_Old = null;
                    //TODO: More sophisticated method for face finding (to be checked)
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

                    Report(string.Format("Panel found. Panel [{0}]", value));

                    Panel panel_New = null;

                    if (updatePanels)
                    {
                        if (guids_Updated.Contains(panel_Old.Guid))
                        {
                            panel_New = new Panel(Guid.NewGuid(), panel_Old, Geometry.Topologic.Convert.ToSAM(face_New));
                            Report(string.Format("Creating new Panel for Old Panel [{0}]. New Panel [{1}]", panel_Old.Guid, panel_New.Guid));
                            //break;
                        }
                        else
                        {
                            panel_New = new Panel(panel_Old.Guid, panel_Old, Geometry.Topologic.Convert.ToSAM(face_New));
                            guids_Updated.Add(panel_Old.Guid);
                            Report(string.Format("Updating Panel [{0}] with new geometry", panel_New.Guid));
                        }

                        //dictionary_SAMObjects[typeof(Panel)][panel_New.Guid] = panel_New;
                        dictionary_Panel_New[panel_New.Guid] = panel_New;
                    }
                    else
                    {
                        panel_New = new Panel(panel_Old.Guid, panel_Old, Geometry.Topologic.Convert.ToSAM(face_New));
                        Report(string.Format("Creating temporary Panel for Panel [{0}]", panel_New.Guid));
                    }


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
                        Report(string.Format("Analyzing Cell"));

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

                        Report(string.Format("Space [{0}] added for Panel [{1}]", space.Guid, panel_New.Guid));

                        HashSet<Guid> guids_Panel;
                        if (!dictionary_Relations_Space.TryGetValue(space.Guid, out guids_Panel))
                        {
                            guids_Panel = new HashSet<Guid>();
                            dictionary_Relations_Space[space.Guid] = guids_Panel;
                        }

                        guids_Panel.Add(panel_New.Guid);

                        Report(string.Format("Panel [{0}] added for Space [{1}]", panel_New.Guid, space.Guid));
                    }

                    Report("Face finished");
                }

                if(updatePanels)
                {
                    dictionary_SAMObjects[typeof(Panel)] = dictionary_Panel_New;
                }

                Report("Sucesfully completed");

                return true;
            }
            catch (Exception exception)
            {
                Report(string.Format("Exception Message: {0}", exception.Message));
            }

            return false;
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
            Dictionary<Guid, HashSet<Guid>> dictionary;
            if (!dictionary_Relations.TryGetValue(typeof(Panel), out dictionary))
                return null;
            
            List<Panel> panels = new List<Panel>();
            foreach (KeyValuePair<Guid, HashSet<Guid>> keyValuePair in dictionary)
            {
                if (keyValuePair.Value.Count < 2)
                    continue;

                panels.Add((Panel)dictionary_SAMObjects[typeof(Panel)][keyValuePair.Key]);
            }

            //TODO: MOve away in the future to another object
            for(int i =0; i < panels.Count; i++ )
            {
                if (panels[i].PanelType == PanelType.Wall || panels[i].PanelType == PanelType.WallExternal)
                {
                    Panel panel = new Panel(panels[i], new Construction("Basic Wall: SIM_INT_SLD_Partition"));
                    panel = new Panel(panel, PanelType.WallInternal);
                    //panel.Name = panel.Construction.Name;
                    panels[i] = panel;
                }
                else if (panels[i].PanelType == PanelType.Roof || panels[i].PanelType == PanelType.Floor || panels[i].PanelType == PanelType.FloorExposed || panels[i].PanelType == PanelType.SlabOnGrade)
                {
                    Panel panel = new Panel(panels[i], new Construction("Floor: SIM_INT_SLD_FLR FLR02"));
                    panel = new Panel(panel, PanelType.FloorInternal);
                    panels[i] = panel;
                }
            }
            //

            return panels;
        }

        public List<Panel> GetExternalPanels()
        {
            Dictionary<Guid, HashSet<Guid>> dictionary;
            if (!dictionary_Relations.TryGetValue(typeof(Panel), out dictionary))
                return null;

            List<Panel> panels = new List<Panel>();
            foreach (KeyValuePair<Guid, HashSet<Guid>> keyValuePair in dictionary)
            {
                if (keyValuePair.Value.Count > 1)
                    continue;

                panels.Add((Panel)dictionary_SAMObjects[typeof(Panel)][keyValuePair.Key]);
            }

            //TODO: MOve away in the future to another object 2
            for (int i = 0; i < panels.Count; i++)
            {
                if (panels[i].PanelType == PanelType.Wall)
                {
                    Panel panel = new Panel(panels[i], new Construction("Basic Wall: SIM_EXT_SLD"));
                    panel = new Panel(panel, PanelType.WallExternal);
                    //panel.Name = panel.Construction.Name;
                    panels[i] = panel;
                }
                else if (panels[i].PanelType == PanelType.Floor)
                {
                    Panel panel = new Panel(panels[i], new Construction("Floor: SIM_EXT_GRD_FLR FLR01"));
                    panel = new Panel(panel, PanelType.SlabOnGrade);
                    panels[i] = panel;
                }
            }
            //

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

            //TODO: MOve away in the future to another object 2
            for (int i = 0; i < panels.Count; i++)
            {
                if (panels[i].PanelType == PanelType.Wall)
                {
                    Panel panel = new Panel(panels[i], new Construction("Basic Wall: SIM_EXT_SLD"));
                    panel = new Panel(panel, PanelType.WallExternal);
                    //panel.Name = panel.Construction.Name;
                    panels[i] = panel;
                }

            }
            //

            return panels;
        }

        public Topology Topology
        {
            get
            {
                return topology;
            }
        }

        public string ReportPath
        {
            get
            {
                return reportPath;
            }
            set
            {
                reportPath = value;
            }
        }
    }
}

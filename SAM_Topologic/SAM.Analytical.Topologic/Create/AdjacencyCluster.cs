﻿using SAM.Core;
using SAM.Geometry.Spatial;
using SAM.Geometry.Topologic;
using System;
using System.Collections.Generic;
using System.Linq;
using Topologic;
using Topologic.Utilities;

namespace SAM.Analytical.Topologic
{
    public static partial class Create
    {
        public static AdjacencyCluster AdjacencyCluster(IEnumerable<Space> spaces, IEnumerable<Panel> panels, out List<Topology> topologies, out List<Panel> redundantPanels, double minArea = Tolerance.MacroDistance, bool updatePanels = true, bool tryCellComplexByCells = true, Log log = null, double silverSpacing = Tolerance.MacroDistance, double tolerance = Tolerance.Distance)
        {
            Core.Modify.Add(log, "Method Name: {0}, Tolerance: {1}, Update Panels: {2}", "SAM.Analytical.Topologic.Create.AdjacencyCluster", tolerance, updatePanels);

            topologies = null;
            redundantPanels = null;

            AdjacencyCluster result = new AdjacencyCluster();
            result.AddObjects(spaces);
            result.AddObjects(panels);

            List<Face> faces = new List<Face>();

            List<Panel> panels_Temp = result.GetObjects<Panel>();
            if(panels_Temp == null || panels_Temp.Count == 0)
            {
                return result;
            }

            int index = 1;
            foreach (Panel panel in panels_Temp)
            {
                if (panel == null)
                    continue;

                Face face = Convert.ToTopologic(panel);
                if (face == null)
                    continue;

                faces.Add(face);
                Core.Modify.Add(log, "Face {0:D4} added. Panel [{1}]", index, panel.Guid);
                index++;
            }

            if (faces == null || faces.Count == 0)
                return null;

            topologies = new List<Topology>();
            List<Cell> cells = null;
            if (tryCellComplexByCells)
            {
                try
                {
                    Core.Modify.Add(log, "Trying to make CellComplex By Cells");
                    Cluster cluster = Cluster.ByTopologies(faces as IList<Topology>);
                    Core.Modify.Add(log, "Cluster.ByTopologies Done");
                    Topology topology = cluster.SelfMerge();
                    Core.Modify.Add(log, "Cluster SelfMerge Done");
                    if (topology.Cells != null && topology.Cells.Count != 0)
                    {
                        cells = topology.Cells?.ToList();
                        CellComplex cellComplex = null;
                        try
                        {
                            cellComplex = CellComplex.ByCells(cells);
                        }
                        catch (Exception exception)
                        {
                            Core.Modify.Add(log, "Cells could not be taken from CellComplex");
                            Core.Modify.Add(log, "Exception Message: {0}", exception.Message);
                        }
                        
                        if (cellComplex != null && cellComplex.Cells != null && cellComplex.Cells.Count != 0)
                        {
                            topologies.Add(cellComplex);

                            cells = cellComplex.Cells?.ToList();
                        }
                        else
                        {
                            topologies.Add(topology);
                            Core.Modify.Add(log, "Cells taken from Cluster");
                        }
                    }

                }
                catch (Exception exception)
                {
                    Core.Modify.Add(log, "Cannot create CellComplex By Cells or Cells form Cluster SelfMerge");
                    Core.Modify.Add(log, "Exception Message: {0}", exception.Message);
                    cells = null;
                }
            }

            if (cells == null)
            {
                try
                {
                    Core.Modify.Add(log, "Trying to make CellComplex By Faces");
                    CellComplex cellComplex = CellComplex.ByFaces(faces, tolerance);
                    topologies.Add(cellComplex);
                    cells = cellComplex.Cells?.ToList();
                    Core.Modify.Add(log, "CellComplex By Faces Created");
                }
                catch (Exception exception)
                {
                    Core.Modify.Add(log, "Cannot create CellComplex By Faces");
                    Core.Modify.Add(log, "Exception Message: {0}", exception.Message);

                    cells = null;
                }
            }

            if (cells == null || cells.Count == 0)
            {
                Core.Modify.Add(log, "No cells found");
                return null;
            }

            List<Geometry.Spatial.Shell> shells = cells.ToSAM();
            Core.Modify.Add(log, "Single CellComplex converted to shells");

            if (shells == null)
                return null;

            //Matching spaces with shells
            Dictionary<Geometry.Spatial.Shell, List<Space>> dictionary_Spaces = new Dictionary<Geometry.Spatial.Shell, List<Space>>();
            if (spaces != null)
            {
                foreach (Space space in spaces)
                {
                    if (space == null || space.Location == null || !space.IsPlaced())
                        continue;

                    List<Geometry.Spatial.Shell> shells_Temp = Analytical.Query.SpaceShells(shells, space.Location, silverSpacing, tolerance);
                    if (shells_Temp == null || shells_Temp.Count == 0)
                        continue;

                    foreach(Geometry.Spatial.Shell shell in shells_Temp)
                    {
                        if(!dictionary_Spaces.TryGetValue(shell, out List<Space> spaces_Shell))
                        {
                            spaces_Shell = new List<Space>();
                            dictionary_Spaces[shell] = spaces_Shell;
                        }
                        spaces_Shell.Add(space);
                    }
                }
            }

            HashSet<Guid> guids_Updated = new HashSet<Guid>();

            Dictionary<Panel, Face3D> dictionary_Panel_Face3D = new Dictionary<Panel, Face3D>();
            result.GetObjects<Panel>().ForEach(x => dictionary_Panel_Face3D[x] = x.GetFace3D());

            index = 1;
            List<Tuple<Panel, Point3D>> tuples_InternalPoint3D = new List<Tuple<Panel, Point3D>>();

            for (int i=0; i < shells.Count; i++)
            {
                Geometry.Spatial.Shell shell = shells[i];
                if (shell == null)
                    return null;

                Core.Modify.Add(log, "Simplifying shell");
                //shell.Simplify(tolerance); // Low tolerance cause of rounding issues
                shell.Simplify();

                Core.Modify.Add(log, "Extracting faces from shell");
                List<Face3D> face3Ds = shell?.Face3Ds;
                if (face3Ds == null)
                {
                    Core.Modify.Add(log, "No face2Ds found in Shell");
                    continue;
                }

                dictionary_Spaces.TryGetValue(shell, out List<Space> spaces_Shell);

                if (spaces_Shell == null || spaces_Shell.Count == 0)
                {
                    Core.Modify.Add(log, "Creating new Space");

                    Point3D location = shell.CalculatedInternalPoint3D(silverSpacing, tolerance);
                    if (location == null)
                        continue;

                    Space space = new Space("Cell " + index, location);
                    index++;

                    if (!result.AddObject(space))
                        continue;

                    spaces_Shell = new List<Space>() { space };
                    dictionary_Spaces[shell] = spaces_Shell;
                }

                if (spaces_Shell == null || spaces_Shell.Count == 0)
                    continue;

                double volume = double.NaN;
                if(cells[i] != null)
                {
                    Core.Modify.Add(log, "Calculating Volume");
                    volume = CellUtility.Volume(cells[i]);

                    foreach (Space space_Shell in spaces_Shell)
                        space_Shell.SetValue(SpaceParameter.Volume, volume);
                }

                Core.Modify.Add(log, "Upadting Panels");
                foreach (Face3D face3D in face3Ds)
                {
                    if (minArea != 0 && face3D.GetArea() <= minArea)
                    {
                        Core.Modify.Add(log, "Face3D is too small");
                        continue;
                    }

                    Core.Modify.Add(log, "Looking for existing Panel");
                    Tuple <Panel, Point3D> tuple_InternalPoint3D = tuples_InternalPoint3D.Find(x => face3D.Inside(x.Item2, tolerance));
                    if(tuple_InternalPoint3D != null)
                    {
                        Core.Modify.Add(log, "Existing Panel found: {0}", tuple_InternalPoint3D.Item1.Guid);
                        foreach (Space space in spaces_Shell)
                        {
                            if (result.AddRelation(space, tuple_InternalPoint3D.Item1))
                                Core.Modify.Add(log, "Space [{0}] and Panel [{1}] relation added", space.Guid, tuple_InternalPoint3D.Item1.Guid);
                        }
                        continue;
                    }

                    Core.Modify.Add(log, "Looking for old Panel");
                    //Panel panel_Old = Query.SimilarPanel(face3D, dictionary_Panel_Face3D);
                    //if (panel_Old == null)
                    //    continue;

                    List<Panel> panels_Old = Query.SimilarPanels(face3D, dictionary_Panel_Face3D);
                    if (panels_Old == null || panels_Old.Count == 0)
                        continue;

                    Panel panel_Old = panels_Old.First();
                    if(panels_Old.Count > 1)
                    {
                        if (redundantPanels == null)
                            redundantPanels = new List<Panel>();

                        panels_Old.RemoveAt(0);
                        redundantPanels.AddRange(panels_Old);
                    }

                    Core.Modify.Add(log, "Old Panel found: {0}", panel_Old.Guid);

                    Panel panel_New = null;

                    if (updatePanels)
                    {
                        if (guids_Updated.Contains(panel_Old.Guid))
                        {
                            panel_New = Analytical.Create.Panel(Guid.NewGuid(), panel_Old, face3D);
                            Core.Modify.Add(log, "Creating new Panel for Old Panel [{0}]. New Panel [{1}]", panel_Old.Guid, panel_New.Guid);
                        }
                        else
                        {
                            panel_New = Analytical.Create.Panel(panel_Old.Guid, panel_Old, face3D);
                            guids_Updated.Add(panel_Old.Guid);
                            Core.Modify.Add(log, "Updating Panel [{0}] with new geometry", panel_New.Guid);
                        }

                        result.AddObject(panel_New);
                    }
                    else
                    {
                        panel_New = Analytical.Create.Panel(panel_Old.Guid, panel_Old, face3D);
                        Core.Modify.Add(log, "Creating temporary Panel for Panel [{0}]", panel_New.Guid);
                    }

                    if (panel_New == null)
                        continue;

                    tuples_InternalPoint3D.Add(new Tuple<Panel, Point3D>(panel_New, face3D.InternalPoint3D(tolerance)));

                    foreach (Space space in spaces_Shell)
                    {
                        if (result.AddRelation(space, panel_New))
                            Core.Modify.Add(log, "Space [{0}] and Panel [{1}] relation added", space.Guid, panel_New.Guid);
                    }

                    Core.Modify.Add(log, "Adding face finished");
                }
            }

            if(redundantPanels != null && redundantPanels.Count != 0)
            {
                Core.Modify.Add(log, "Solving Redundant Panels");
                foreach(Panel panel in redundantPanels)
                    result.RemoveObject<Panel>(panel.Guid);
            }

            List<Panel> panels_Shading = Analytical.Query.CutShading(result.GetPanels(), panels, tolerance);
            if(panels_Shading != null || panels_Shading.Count != 0)
            {
                foreach (Panel panel_Shading in panels_Shading)
                    result.AddObject(panel_Shading);
            }

            Core.Modify.Add(log, "AdjacencyCluster verification");
            Log log_AdjacencyCluster = Analytical.Create.Log(result);
            if (log != null)
                log.AddRange(log_AdjacencyCluster);

            Core.Modify.Add(log, "Process completed");
            return result;
        }
    }
}
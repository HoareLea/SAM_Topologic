// SPDX-License-Identifier: LGPL-3.0-or-later
// Copyright (c) 2020–2026 Michal Dengusiak & Jakub Ziolkowski and contributors

using Grasshopper.Kernel;
using SAM.Analytical.Grasshopper.Topologic.Properties;
using SAM.Core;
using SAM.Core.Grasshopper;
using SAM.Geometry.Spatial;
using System;
using System.Collections.Generic;
using Topologic;

namespace SAM.Analytical.Grasshopper.Topologic
{
    public class CreateSAMAdjacencyCluster : GH_SAMVariableOutputParameterComponent
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("aa942ea4-aa3d-48ee-a56c-3cb3e8be51ea");

        /// <summary>
        /// The latest version of this component
        /// </summary>
        public override string LatestComponentVersion => "1.0.2";

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.SAM_Topologic3a;

        /// <summary>
        /// Initializes a new instance of the SAM_point3D class.
        /// </summary>
        public CreateSAMAdjacencyCluster()
          : base("Create.SAMAdjacencyCluster", "Create.SAMAdjacencyCluster",
              "Create SAM Adjacency Cluster \n* use node SAMAdjacencyCluster.UpdatePanelTypes after to fix PanelTypes",
              "SAM", "Topologic")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override GH_SAMParam[] Inputs
        {
            get
            {
                List<GH_SAMParam> result = new List<GH_SAMParam>();

                result.Add(new GH_SAMParam(new GooPanelParam() { Name = "_panels", NickName = "_panels", Description = "SAM Analytical Panels", Access = GH_ParamAccess.list, DataMapping = GH_DataMapping.Flatten }, ParamVisibility.Binding));
                result.Add(new GH_SAMParam(new GooSpaceParam() { Name = "spaces_", NickName = "spaces_", Description = "SAM Analytical Spaces", Access = GH_ParamAccess.list, Optional = true, DataMapping = GH_DataMapping.Flatten }, ParamVisibility.Binding));

                global::Grasshopper.Kernel.Parameters.Param_Number param_tolerance = new global::Grasshopper.Kernel.Parameters.Param_Number() { Name = "tolerance_", NickName = "tolerance_", Description = string.Format("Topologic CellComplex default {0}", 0.0001), Access = GH_ParamAccess.item };
                param_tolerance.SetPersistentData(0.0001);
                result.Add(new GH_SAMParam(param_tolerance, ParamVisibility.Binding));

                global::Grasshopper.Kernel.Parameters.Param_Boolean param_tryCellComplexByCells = new global::Grasshopper.Kernel.Parameters.Param_Boolean() { Name = "tryCellComplexByCells_", NickName = "tryCellComplexByCells_", Description = "Try to Create Cell Complex By Cells", Access = GH_ParamAccess.item };
                param_tryCellComplexByCells.SetPersistentData(false);
                result.Add(new GH_SAMParam(param_tryCellComplexByCells, ParamVisibility.Binding));

                global::Grasshopper.Kernel.Parameters.Param_Number param_minArea = new global::Grasshopper.Kernel.Parameters.Param_Number() { Name = "minArea_", NickName = "minArea_", Description = "Minimal Acceptable area of Aperture", Access = GH_ParamAccess.item };
                param_minArea.SetPersistentData(Tolerance.MacroDistance);
                result.Add(new GH_SAMParam(param_minArea, ParamVisibility.Binding));

                global::Grasshopper.Kernel.Parameters.Param_Number param_silverSpacing = new global::Grasshopper.Kernel.Parameters.Param_Number() { Name = "silverSpacing_", NickName = "silverSpacing_", Description = string.Format("Silver spacing for point in Space calculation {0}", Tolerance.MacroDistance), Access = GH_ParamAccess.item };
                param_silverSpacing.SetPersistentData(Tolerance.MacroDistance);
                result.Add(new GH_SAMParam(param_silverSpacing, ParamVisibility.Binding));

                global::Grasshopper.Kernel.Parameters.Param_Boolean param_run = new global::Grasshopper.Kernel.Parameters.Param_Boolean() { Name = "_run", NickName = "_run", Description = "Run", Access = GH_ParamAccess.item };
                param_run.SetPersistentData(false);
                result.Add(new GH_SAMParam(param_run, ParamVisibility.Binding));

                return result.ToArray();
            }
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override GH_SAMParam[] Outputs
        {
            get
            {
                List<GH_SAMParam> result = new List<GH_SAMParam>();
                result.Add(new GH_SAMParam(new GooAdjacencyClusterParam() { Name = "AdjacencyCluster", NickName = "AdjacencyCluster", Description = "SAM AdjacencyCluster", Access = GH_ParamAccess.item }, ParamVisibility.Binding));
                result.Add(new GH_SAMParam(new global::Grasshopper.Kernel.Parameters.Param_GenericObject() { Name = "Topologies", NickName = "Topologies", Description = "Topologies", Access = GH_ParamAccess.list }, ParamVisibility.Binding));
                result.Add(new GH_SAMParam(new GooPanelParam() { Name = "Panels", NickName = "Panels", Description = "SAM Analytical Panels", Access = GH_ParamAccess.list }, ParamVisibility.Binding));
                result.Add(new GH_SAMParam(new GooSpaceParam() { Name = "Spaces", NickName = "Spaces", Description = "SAM Analytical Spaces", Access = GH_ParamAccess.list }, ParamVisibility.Binding));
                result.Add(new GH_SAMParam(new GooPanelParam() { Name = "InternalPanels", NickName = "InternalPanels", Description = "SAM Analytical Internal Panels", Access = GH_ParamAccess.list }, ParamVisibility.Binding));
                result.Add(new GH_SAMParam(new GooPanelParam() { Name = "ExternalPanels", NickName = "ExternalPanels", Description = "SAM Analytical External Panels", Access = GH_ParamAccess.list }, ParamVisibility.Binding));
                result.Add(new GH_SAMParam(new GooPanelParam() { Name = "ShadingPanels", NickName = "ShadingPanels", Description = "SAM Analytical Shading Panels", Access = GH_ParamAccess.list }, ParamVisibility.Binding));
                result.Add(new GH_SAMParam(new GooPanelParam() { Name = "RedundantPanels", NickName = "RedundantPanels", Description = "SAM Analytical Redundant Panels", Access = GH_ParamAccess.list }, ParamVisibility.Binding));
                result.Add(new GH_SAMParam(new GooLogParam() { Name = "Log", NickName = "Log", Description = "Log", Access = GH_ParamAccess.item }, ParamVisibility.Binding));
                result.Add(new GH_SAMParam(new global::Grasshopper.Kernel.Parameters.Param_Boolean() { Name = "Sucessfull", NickName = "Sucessfull", Description = "Run successfully?", Access = GH_ParamAccess.item }, ParamVisibility.Binding));
                return result.ToArray();
            }
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="dataAccess">
        /// The DA object is used to retrieve from inputs and store in outputs.
        /// </param>
        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            // NOTE: the baseline (legacy) component reset output[2] ("Panels") to `false` on every
            // early-error path below via a hardcoded positional dataAccess.SetData(2, false) call —
            // almost certainly meant to reset "Sucessfull" (originally at index 9) but never fixed.
            // Preserved exactly (mapped to the "Panels" output by name) rather than corrected, per
            // migration scope; flagged as a pre-existing issue in the PR description, not fixed here.
            int panelsIndex = Params.IndexOfOutputParam("Panels");

            bool run = false;
            int index = Params.IndexOfInputParam("_run");
            if (index == -1 || !dataAccess.GetData(index, ref run))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                if (panelsIndex != -1) dataAccess.SetData(panelsIndex, false);
                return;
            }
            if (!run)
                return;

            List<Panel> panels = new List<Panel>();
            index = Params.IndexOfInputParam("_panels");
            if (index == -1 || !dataAccess.GetDataList(index, panels) || panels == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                if (panelsIndex != -1) dataAccess.SetData(panelsIndex, false);
                return;
            }

            List<Space> spaces = new List<Space>();
            index = Params.IndexOfInputParam("spaces_");
            if (index != -1)
            {
                dataAccess.GetDataList(index, spaces);
            }

            double tolerance = double.NaN;
            index = Params.IndexOfInputParam("tolerance_");
            if (index == -1 || !dataAccess.GetData(index, ref tolerance) || double.IsNaN(tolerance))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                if (panelsIndex != -1) dataAccess.SetData(panelsIndex, false);
                return;
            }

            bool tryCellComplexByCells = false;
            index = Params.IndexOfInputParam("tryCellComplexByCells_");
            if (index == -1 || !dataAccess.GetData(index, ref tryCellComplexByCells))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                if (panelsIndex != -1) dataAccess.SetData(panelsIndex, false);
                return;
            }

            //string reportPath = null;
            //if (dataAccess.GetData(4, ref reportPath))
            //{
            //    if (System.IO.File.Exists(reportPath))
            //        System.IO.File.Delete(reportPath);
            //}

            double minArea = Tolerance.MacroDistance;
            index = Params.IndexOfInputParam("minArea_");
            if (index != -1)
            {
                dataAccess.GetData(index, ref minArea);
            }

            double silverSpacing = Tolerance.MacroDistance;
            index = Params.IndexOfInputParam("silverSpacing_");
            if (index != -1)
            {
                dataAccess.GetData(index, ref silverSpacing);
            }

            List<Topology> topologies = null;
            Log log = new Log();

            List<Panel> panels_Redundant = null;
            AdjacencyCluster adjacencyCluster = Analytical.Topologic.Create.AdjacencyCluster(spaces, panels, out topologies, out panels_Redundant, minArea, true, tryCellComplexByCells, log, silverSpacing, tolerance);

            if (adjacencyCluster != null)
            {
                List<Space> spaces_Temp = adjacencyCluster.GetSpaces();
                if (spaces_Temp == null || spaces_Temp.Count == 0)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No spaces have been detected");
                }
                else
                {
                    List<Point3D> locations = spaces_Temp.ConvertAll(x => x.Location);

                    if (locations.RemoveAll(x => x == null) > 0)
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "There are spaces without Location Points");

                    List<Space> spaces_Unbounded = new List<Space>();
                    List<Space> spaces_Multiple = new List<Space>();

                    HashSet<int> indexes_Multiple = new HashSet<int>();
                    List<List<Space>> spacesList_Locations = adjacencyCluster.GetSpaces(locations);
                    for (int i = 0; i < spacesList_Locations.Count; i++)
                    {
                        List<Space> spaces_Locations = spacesList_Locations[i];
                        Point3D point3D_Location = locations[i];

                        if (spaces_Locations == null)
                        {
                            spaces_Unbounded.Add(spaces_Temp.Find(x => point3D_Location.AlmostEquals(x.Location)));
                            continue;
                        }

                        if (spacesList_Locations.Count > 2)
                            indexes_Multiple.Add(i);
                    }

                    foreach (Space space in spaces_Unbounded)
                    {
                        string text = "There are unbounded spaces in topology model";
                        if (!string.IsNullOrWhiteSpace(space.Name))
                            text += " " + space.Name;

                        text += " " + "Guid: " + space.Guid;

                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, text);
                    }

                    foreach (Space space in spaces_Multiple)
                    {
                        string text = "There are multiple spaces in topology cell";
                        if (!string.IsNullOrWhiteSpace(space.Name))
                            text += " " + space.Name;

                        text += " " + "Guid: " + space.Guid;

                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, text);
                    }

                }

                index = Params.IndexOfOutputParam("AdjacencyCluster");
                if (index != -1)
                {
                    dataAccess.SetData(index, new GooAdjacencyCluster(adjacencyCluster));
                }
            }
            else
            {
                index = Params.IndexOfOutputParam("AdjacencyCluster");
                if (index != -1)
                {
                    dataAccess.SetData(index, null);
                }
            }

            index = Params.IndexOfOutputParam("Topologies");
            if (index != -1) dataAccess.SetDataList(index, topologies);

            if (panelsIndex != -1) dataAccess.SetDataList(panelsIndex, adjacencyCluster?.GetPanels());

            index = Params.IndexOfOutputParam("Spaces");
            if (index != -1) dataAccess.SetDataList(index, adjacencyCluster?.GetSpaces());

            index = Params.IndexOfOutputParam("InternalPanels");
            if (index != -1) dataAccess.SetDataList(index, adjacencyCluster?.GetInternalPanels());

            index = Params.IndexOfOutputParam("ExternalPanels");
            if (index != -1) dataAccess.SetDataList(index, adjacencyCluster?.GetExternalPanels());

            index = Params.IndexOfOutputParam("ShadingPanels");
            if (index != -1) dataAccess.SetDataList(index, adjacencyCluster?.GetShadingPanels());

            index = Params.IndexOfOutputParam("RedundantPanels");
            if (index != -1) dataAccess.SetDataList(index, panels_Redundant);

            index = Params.IndexOfOutputParam("Log");
            if (index != -1) dataAccess.SetData(index, new GooLog(log));

            index = Params.IndexOfOutputParam("Sucessfull");
            if (index != -1) dataAccess.SetData(index, adjacencyCluster != null);
        }
    }
}

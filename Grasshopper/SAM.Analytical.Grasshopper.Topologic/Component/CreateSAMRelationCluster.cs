﻿using Grasshopper.Kernel;
using SAM.Analytical.Grasshopper.Topologic.Properties;
using SAM.Core;
using System;
using System.Collections.Generic;
using Topologic;

namespace SAM.Analytical.Grasshopper.Topologic
{
    public class CreateSAMRelationCluster : GH_Component
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("a90bbb0c-ed58-44a0-99d9-4c3505bb282a");

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.SAM_Topologic;

        /// <summary>
        /// Initializes a new instance of the SAM_point3D class.
        /// </summary>
        public CreateSAMRelationCluster()
          : base("Create.SAMRelationCluster", "Create.SAMRelationCluster",
              "Create SAM Relation Cluster",
              "SAM", "Topologic")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager inputParamManager)
        {
            int index;

            index = inputParamManager.AddParameter(new GooPanelParam(), "_panels", "_panels", "SAM Analytical Panels", GH_ParamAccess.list);
            inputParamManager[index].DataMapping = GH_DataMapping.Flatten;

            GooSpaceParam gooSpaceParam = new GooSpaceParam();
            gooSpaceParam.Optional = true;
            index = inputParamManager.AddParameter(gooSpaceParam, "spaces_", "spaces_", "SAM Analytical Spaces", GH_ParamAccess.list);
            inputParamManager[index].DataMapping = GH_DataMapping.Flatten;

            inputParamManager.AddNumberParameter("tolerance_", "tolerance_", string.Format("Topologic CellComplex default {0}", Core.Tolerance.MacroDistance), GH_ParamAccess.item, Core.Tolerance.MacroDistance);
            //inputParamManager.AddBooleanParameter("_run_", "_run_", "Run", GH_ParamAccess.item, false);
            inputParamManager.AddBooleanParameter("tryCellComplexByCells_", "tryCellComplexByCells_", "Try to Create Cell Complex By Cells", GH_ParamAccess.item, false);
            inputParamManager.AddTextParameter("reportPath_", "reportPath_", "Report Path to write each step in text file", GH_ParamAccess.item, string.Empty);
            inputParamManager.AddNumberParameter("minArea_", "minArea_", "Minimal Acceptable area of Aperture", GH_ParamAccess.item, Core.Tolerance.MacroDistance);
            inputParamManager.AddBooleanParameter("_run_", "_run_", "Run", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager outputParamManager)
        {
            outputParamManager.AddParameter(new Core.Grasshopper.GooRelationClusterParam(), "RelationCluster", "RelationCluster", "SAM RelationCluster", GH_ParamAccess.item);
            outputParamManager.AddGenericParameter("Topology", "Topology", "Topology", GH_ParamAccess.item);
            outputParamManager.AddParameter(new GooPanelParam(), "Panels", "Panels", "SAM Analytical Panels", GH_ParamAccess.list);
            outputParamManager.AddParameter(new GooSpaceParam(), "Spaces", "Spaces", "SAM Analytical Spaces", GH_ParamAccess.list);
            outputParamManager.AddParameter(new GooPanelParam(), "InternalPanels", "InternalPanels", "SAM Analytical Internal Panels", GH_ParamAccess.list);
            outputParamManager.AddParameter(new GooPanelParam(), "ExternalPanels", "ExternalPanels", "SAM Analytical External Panels", GH_ParamAccess.list);
            outputParamManager.AddParameter(new GooPanelParam(), "ShadingPanels", "ShadingPanels", "SAM Analytical Shading Panels", GH_ParamAccess.list);
            outputParamManager.AddBooleanParameter("Sucessfull", "Sucessfull", "Run successfully?", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="dataAccess">
        /// The DA object is used to retrieve from inputs and store in outputs.
        /// </param>
        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            bool run = false;
            if (!dataAccess.GetData(6, ref run))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                dataAccess.SetData(2, false);
                return;
            }
            if (!run)
                return;

            List<Panel> panels = new List<Panel>();
            if (!dataAccess.GetDataList(0, panels) || panels == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                dataAccess.SetData(2, false);
                return;
            }

            List<Space> spaces = new List<Space>();
            dataAccess.GetDataList(1, spaces);

            double tolerance = double.NaN;
            if (!dataAccess.GetData(2, ref tolerance) || double.IsNaN(tolerance))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                dataAccess.SetData(2, false);
                return;
            }

            string reportPath = null;
            if (dataAccess.GetData(4, ref reportPath))
            {
                if (System.IO.File.Exists(reportPath))
                    System.IO.File.Delete(reportPath);
            }

            bool tryCellComplexByCells = false;
            if (!dataAccess.GetData(3, ref tryCellComplexByCells))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                dataAccess.SetData(2, false);
                return;
            }

            double minArea = Core.Tolerance.MacroDistance;
            dataAccess.GetData(5, ref minArea);

            Topology topology = null;
            Log log = null;
            if (!string.IsNullOrEmpty(reportPath))
                log = new Log();

            RelationCluster relationCluster = Analytical.Topologic.Create.RelationCluster(spaces, panels, out topology, minArea, true, tryCellComplexByCells, log);

            dataAccess.SetData(0, new Core.Grasshopper.GooRelationCluster(relationCluster));
            dataAccess.SetData(1, topology);
            dataAccess.SetDataList(2, relationCluster.GetObjects<Panel>());
            dataAccess.SetDataList(3, relationCluster.GetObjects<Space>());
            dataAccess.SetDataList(4, relationCluster.InternalPanels());
            dataAccess.SetDataList(5, relationCluster.ExternalPanels());
            dataAccess.SetDataList(6, relationCluster.ShadingPanels());
            dataAccess.SetData(7, relationCluster != null);
        }
    }
}
using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using SAM.Analytical.Grasshopper.Topologic.Properties;


namespace SAM.Analytical.Grasshopper.Topologic
{
    public class SAMAnalyticalCreateAdjacencyCluster : GH_Component
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("2e04adb9-97ab-4634-83cf-67ce76249588");

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.SAM_Topologic;


        /// <summary>
        /// Initializes a new instance of the SAM_point3D class.
        /// </summary>
        public SAMAnalyticalCreateAdjacencyCluster()
          : base("SAMAnalytical.CreateAdjacencyCluster", "SAMAnalytical.CreateAdjacencyCluster",
              "Convert SAM Analytical Panel To Topologic Cellcomplex and then rerun for each Face list of Adjacent Space Names",
              "SAM", "Topologic")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager inputParamManager)
        {
            inputParamManager.AddParameter(new GooPanelParam(), "_panels", "_panels", "SAM Analytical Panels", GH_ParamAccess.list);

            GooSpaceParam gooSpaceParam = new GooSpaceParam();
            gooSpaceParam.Optional = true;
            inputParamManager.AddParameter(gooSpaceParam, "_spaces", "_spaces", "SAM Analytical Spaces", GH_ParamAccess.list);

            inputParamManager.AddNumberParameter("_tolerance_", "_tolerance_", string.Format("Topologic CellComplex default {0}", Geometry.Tolerance.MacroDistance), GH_ParamAccess.item, Geometry.Tolerance.MacroDistance);
            inputParamManager.AddBooleanParameter("_run_", "_run_", "Run", GH_ParamAccess.item, false);
            inputParamManager.AddBooleanParameter("_tryCellComplexByCells_", "_CCompl_", "Try to Create Cell Complex By Cells", GH_ParamAccess.item, false);
            inputParamManager.AddTextParameter("_reportPath_", "_reportPath_", "Report Path to write each step in text file", GH_ParamAccess.item, string.Empty);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager outputParamManager)
        {
            outputParamManager.AddParameter(new GooAdjacencyClusterParam(), "AdjacencyCluster", "AdjacencyCluster", "sAM AdjacencyCluster", GH_ParamAccess.item);
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
        /// <param name="dataAccess">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            bool run = false;
            if (!dataAccess.GetData(3, ref run))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                dataAccess.SetData(2, false);
                return;
            }
            if (!run)
                return;


            List<Panel> panelList = new List<Panel>();
            if (!dataAccess.GetDataList(0, panelList) || panelList == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                dataAccess.SetData(2, false);
                return;
            }

            List<Space> spaceList = new List<Space>();
            dataAccess.GetDataList(1, spaceList);

            double tolerance = double.NaN;
            if (!dataAccess.GetData(2, ref tolerance) || double.IsNaN(tolerance))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                dataAccess.SetData(2, false);
                return;
            }

            string reportPath = null;
            if(dataAccess.GetData(5, ref reportPath))
            {
                if (System.IO.File.Exists(reportPath))
                    System.IO.File.Delete(reportPath);
            }

            bool tryCellComplexByCells = false;
            if (!dataAccess.GetData(4, ref tryCellComplexByCells))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                dataAccess.SetData(2, false);
                return;
            }

            Analytical.Topologic.AdjacencyCluster adjacencyCluster = new Analytical.Topologic.AdjacencyCluster(spaceList, panelList);
            adjacencyCluster.ReportPath = reportPath;
            bool result = adjacencyCluster.Calculate(tolerance, tryCellComplexByCells);

            //AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, string.Join("\n", adjacencyCluster.Report));

            dataAccess.SetData(0, adjacencyCluster);
            dataAccess.SetData(1, adjacencyCluster.Topology);
            dataAccess.SetDataList(2, adjacencyCluster.GetPanels());
            dataAccess.SetDataList(3, adjacencyCluster.GetSpaces());
            dataAccess.SetDataList(4, adjacencyCluster.GetInternalPanels());
            dataAccess.SetDataList(5, adjacencyCluster.GetExternalPanels());
            dataAccess.SetDataList(6, adjacencyCluster.GetShadingPanels());
            dataAccess.SetData(7, result);
        }
    }
}
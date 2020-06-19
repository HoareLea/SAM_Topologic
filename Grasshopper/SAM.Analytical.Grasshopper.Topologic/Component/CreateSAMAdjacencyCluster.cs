using Grasshopper.Kernel;
using SAM.Analytical.Grasshopper.Topologic.Properties;
using SAM.Core;
using SAM.Geometry.Spatial;
using System;
using System.Collections.Generic;
using System.Linq;
using Topologic;

namespace SAM.Analytical.Grasshopper.Topologic
{
    public class CreateSAMAdjacencyCluster : GH_Component
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
        public CreateSAMAdjacencyCluster()
          : base("Create.SAMAdjacencyCluster", "Create.SAMAdjacencyCluster",
              "Create SAM Adjacency Cluster",
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

            inputParamManager.AddNumberParameter("tolerance_", "tolerance_", string.Format("Topologic CellComplex default {0}", Tolerance.MacroDistance), GH_ParamAccess.item, Tolerance.MacroDistance);
            //inputParamManager.AddBooleanParameter("_run_", "_run_", "Run", GH_ParamAccess.item, false);
            inputParamManager.AddBooleanParameter("tryCellComplexByCells_", "tryCellComplexByCells_", "Try to Create Cell Complex By Cells", GH_ParamAccess.item, false);
            inputParamManager.AddTextParameter("reportPath_", "reportPath_", "Report Path to write each step in text file", GH_ParamAccess.item, string.Empty);
            inputParamManager.AddNumberParameter("minArea_", "minArea_", "Minimal Acceptable area of Aperture", GH_ParamAccess.item, Tolerance.MacroDistance);
            inputParamManager.AddBooleanParameter("_run_", "_run_", "Run", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager outputParamManager)
        {
            outputParamManager.AddParameter(new GooAdjacencyClusterParam(), "AdjacencyCluster", "AdjacencyCluster", "SAM AdjacencyCluster", GH_ParamAccess.item);
            outputParamManager.AddGenericParameter("Topologies", "Topologies", "Topologies", GH_ParamAccess.list);
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

            List<Topology> topologies = null;
            Log log = null;
            if (!string.IsNullOrEmpty(reportPath))
                log = new Log();

            AdjacencyCluster adjacencyCluster = Analytical.Topologic.Create.AdjacencyCluster(spaces, panels, out topologies, minArea, true, tryCellComplexByCells, log, tolerance);

            if (!string.IsNullOrEmpty(reportPath))
                log.Write(reportPath);

            if (adjacencyCluster != null)
            {
                List<Space> spaces_Temp = adjacencyCluster.GetSpaces();
                if(spaces_Temp == null || spaces_Temp.Count == 0)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No spaces have been detected");
                }
                else
                {
                    List<Point3D> locations = spaces_Temp.ConvertAll(x => x.Location);
                    locations.RemoveAll(x => x == null);
                    IEnumerable<Guid> guids = adjacencyCluster.GetSpaces(locations)?.FindAll(x => x != null).ConvertAll(x => x.Guid);
                    if(guids == null || guids.Count() != guids.Distinct().Count())
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Number of provided spaces does not match with the topology model");
                }

                dataAccess.SetData(0, new GooAdjacencyCluster(adjacencyCluster));
            } 
            else
            {
                dataAccess.SetData(0, null);
            }
                
            dataAccess.SetDataList(1, topologies);
            dataAccess.SetDataList(2, adjacencyCluster?.GetPanels());
            dataAccess.SetDataList(3, adjacencyCluster?.GetSpaces());
            dataAccess.SetDataList(4, adjacencyCluster?.GetInternalPanels());
            dataAccess.SetDataList(5, adjacencyCluster?.GetExternalPanels());
            dataAccess.SetDataList(6, adjacencyCluster?.GetShadingPanels());
            dataAccess.SetData(7, adjacencyCluster != null);
        }
    }
}
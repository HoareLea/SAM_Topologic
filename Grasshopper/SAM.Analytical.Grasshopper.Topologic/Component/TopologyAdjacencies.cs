using Grasshopper.Kernel;
using SAM.Analytical.Grasshopper.Topologic.Properties;
using SAM.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SAM.Analytical.Grasshopper.Topologic
{
    public class TopologyAdjacencies : GH_Component
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("a4a56d41-a2b2-4484-b8d9-787b5beedeb8");

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.SAM_Topologic;

        /// <summary>
        /// Initializes a new instance of the SAM_point3D class.
        /// </summary>
        public TopologyAdjacencies()
          : base("Topology.Adjacencies", "Topology.Adjacencies",
              "Create AdjacenciesList/Connected Spaces List from  SAM AdjacencyCluster and SAM Analytical Panels based on Topologic calculation",
              "SAM", "Topologic")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager inputParamManager)
        {
            inputParamManager.AddParameter(new GooAdjacencyClusterParam(), "_adjacencyCluster", "_adjacencyCluster", "SAM AdjacencyCluster", GH_ParamAccess.item);
            inputParamManager.AddParameter(new Core.Grasshopper.GooSAMObjectParam<SAMObject>(), "_SAMAnalytical", "_SAMAnalytical", "SAM Analytical Object", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager outputParamManager)
        {
            outputParamManager.AddParameter(new Core.Grasshopper.GooSAMObjectParam<SAMObject>(), "AdjacenciesList", "AdjacenciesList", "AdjacenciesList", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="dataAccess">
        /// The DA object is used to retrieve from inputs and store in outputs.
        /// </param>
        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            Analytical.Topologic.AdjacencyCluster adjacencyCluster = null;

            if (!dataAccess.GetData(0, ref adjacencyCluster))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            SAMObject sAMObject = null;
            if (!dataAccess.GetData(1, ref sAMObject))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            IEnumerable<SAMObject> result = null;
            if (sAMObject is Space)
                result = adjacencyCluster.GetSpacePanels(sAMObject.Guid);
            else if (sAMObject is Panel)
                result = adjacencyCluster.GetPanelSpaces(sAMObject.Guid);

            if (result == null)
            {
                dataAccess.SetDataList(0, null);
                return;
            }

            if (result.Count() == 0)
            {
                dataAccess.SetDataList(0, result);
                return;
            }

            dataAccess.SetDataList(0, result.ToList().ConvertAll(x => new Core.Grasshopper.GooSAMObject<SAMObject>(x)));
        }
    }
}
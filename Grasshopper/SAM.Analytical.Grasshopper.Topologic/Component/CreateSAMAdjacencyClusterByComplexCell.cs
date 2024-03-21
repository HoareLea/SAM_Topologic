using Grasshopper.Kernel;
using SAM.Analytical.Grasshopper.Topologic.Properties;
using SAM.Core.Grasshopper;
using System;
using Topologic;

namespace SAM.Analytical.Grasshopper.Topologic
{
    public class CreateSAMAdjacencyClusterByCellComplex : GH_SAMComponent
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("870f9db6-cede-456a-9e25-5d39646a0620");

        /// <summary>
        /// The latest version of this component
        /// </summary>
        public override string LatestComponentVersion => "1.0.1";

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.SAM_Topologic3a;

        /// <summary>
        /// Initializes a new instance of the SAM_point3D class.
        /// </summary>
        public CreateSAMAdjacencyClusterByCellComplex()
          : base("Create.SAMAdjacencyClusterByCellComplex", "Create.SAMAdjacencyClusterByCellComplex",
              "Create SAM Adjacency Cluster By CellComplex",
              "SAM", "Topologic")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager inputParamManager)
        {
            inputParamManager.AddGenericParameter("_cellComplex", "_cellComplex", "Topology CellComplex", GH_ParamAccess.item);
            inputParamManager.AddBooleanParameter("_run", "_run", "Run", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager outputParamManager)
        {
            outputParamManager.AddParameter(new GooAdjacencyClusterParam(), "adjacencyCluster", "adjacencyCluster", "SAM AdjacencyCluster", GH_ParamAccess.item);
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
            if (!dataAccess.GetData(1, ref run))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                dataAccess.SetData(0, null);
                return;
            }
            if (!run)
                return;

            CellComplex cellComplex = null;
            if (!dataAccess.GetData(0, ref cellComplex))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            AdjacencyCluster adjacencyCluster = Analytical.Topologic.Create.AdjacencyCluster(cellComplex);

            dataAccess.SetData(0, adjacencyCluster);
        }
    }
}
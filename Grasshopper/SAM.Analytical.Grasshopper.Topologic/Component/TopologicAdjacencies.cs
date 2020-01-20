using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using SAM.Analytical.Grasshopper.Topologic.Properties;
using Topologic;

namespace SAM.Analytical.Grasshopper.Topologic
{
    public class TopologicAdjacencies : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SAM_point3D class.
        /// </summary>
        public TopologicAdjacencies()
          : base("TopologicAdjacencies", "TopologicAdjacencies",
              "Create AdjacenciesList/Connected Spaces List from  CellComplex AdjacencyCluster and Analytical Panels based on Topologic calculation",
              "SAM", "Topologic")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager inputParamManager)
        {
            inputParamManager.AddGenericParameter("AdjacencyCluster", "AdjacencyCluster", "AdjacencyCluster", GH_ParamAccess.item);
            inputParamManager.AddGenericParameter("Panels", "Panels", "Panels", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager outputParamManager)
        {
            outputParamManager.AddGenericParameter("AdjacenciesList", "AdjacenciesList", "AdjacenciesList", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="dataAccess">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            Analytical.Topologic.AdjacencyCluster adjacencyCluster = null;

            if (!dataAccess.GetData<Analytical.Topologic.AdjacencyCluster>(0, ref adjacencyCluster))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            Core.SAMObject sAMObject = null;

            if (!dataAccess.GetData<Core.SAMObject>(1, ref sAMObject))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            IEnumerable<Core.SAMObject> result = null;
            if (sAMObject is Space)
                result = adjacencyCluster.GetSpacePanels(sAMObject.Guid);
            else if (sAMObject is Panel)
                result = adjacencyCluster.GetPanelSpaces(sAMObject.Guid);

            dataAccess.SetDataList(0, result);
            return;

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Resources.SAM_Small;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("a4a56d41-a2b2-4484-b8d9-787b5beedeb8"); }
        }
    }
}
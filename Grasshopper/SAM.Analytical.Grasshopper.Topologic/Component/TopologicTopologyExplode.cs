using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using SAM.Analytical.Grasshopper.Topologic.Properties;
using Topologic;

namespace SAM.Analytical.Grasshopper.Topologic
{
    public class TopologicTopologyExplode : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SAM_point3D class.
        /// </summary>
        public TopologicTopologyExplode()
          : base("TopologicTopologyExplode", "CellComplexByFaces",
              "Create Topologic CellComplex by Topologic Face",
              "SAM", "Topologic")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager inputParamManager)
        {
            inputParamManager.AddGenericParameter("Topology", "_topology", "Topology", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager outputParamManager)
        {
            outputParamManager.AddGenericParameter("CellComplexes", "CellComplexes", "Topologic CellComplexes", GH_ParamAccess.list);
            outputParamManager.AddGenericParameter("Cells", "Cells", "Topologic Cells", GH_ParamAccess.list);
            outputParamManager.AddGenericParameter("Faces", "Faces", "Topologic Faces", GH_ParamAccess.list);
            outputParamManager.AddGenericParameter("Wires", "Wires", "Topologic Wires", GH_ParamAccess.list);
            outputParamManager.AddGenericParameter("Vertices", "Vertices", "Topologic Vertices", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="dataAccess">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            Topology topology = null;

            if (!dataAccess.GetData<Topology>(0, ref topology))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            dataAccess.SetData(0, topology.CellComplexes);
            dataAccess.SetData(1, topology.Cells);
            dataAccess.SetData(2, topology.Faces);
            dataAccess.SetData(3, topology.Wires);
            dataAccess.SetData(4, topology.Vertices);
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
            get { return new Guid("7aff072b-fe18-4ea3-8f2a-b64bc1cac958"); }
        }
    }
}
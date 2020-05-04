using Grasshopper.Kernel;
using SAM.Geometry.Grasshopper.Topologic.Properties;
using System;
using Topologic;

namespace SAM.Geometry.Grasshopper.Topologic
{
    public class TopologySAMGeometry : GH_Component
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("0fca6d19-95ee-4461-9be1-ab784aa800db");

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.SAM_Topologic;

        /// <summary>
        /// Initializes a new instance of the SAMGeometryByGHGeometry class.
        /// </summary>
        public TopologySAMGeometry()
          : base("Topology.SAMGeometry", "Topology.SAMGeometry",
              "Convert Topologic Geometry to SAM Geometry",
              "SAM", "Topologic")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager inputParamManager)
        {
            inputParamManager.AddGenericParameter("_topology", "_topology", "Topologic Geometry", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager outputParamManager)
        {
            outputParamManager.AddParameter(new GooSAMGeometryParam(), "SAMGeometry", "SAMGeometry", "SAM Geometry", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="dataAccess">
        /// The DA object is used to retrieve from inputs and store in outputs.
        /// </param>
        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            Topology topology = null;
            if (!dataAccess.GetData(0, ref topology) || topology == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            Spatial.ISAMGeometry3D sAMGeometry3D = Convert.ToSAM(topology);
            if (sAMGeometry3D == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Cannot convert geometry");
                return;
            }

            dataAccess.SetData(0, new GooSAMGeometry(sAMGeometry3D));
        }
    }
}
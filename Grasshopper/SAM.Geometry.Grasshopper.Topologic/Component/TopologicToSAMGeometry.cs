using System;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using SAM.Geometry.Grasshopper.Topologic.Properties;
using Topologic;

namespace SAM.Geometry.Grasshopper.Topologic
{
    public class TopologicToSAMGeometry : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SAMGeometryByGHGeometry class.
        /// </summary>
        public TopologicToSAMGeometry()
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
            outputParamManager.AddGenericParameter("SAMGeometry", "SAMGeometry", "SAM Geometry", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="dataAccess">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            GH_ObjectWrapper objectWrapper = null;

            if (!dataAccess.GetData(0, ref objectWrapper) || objectWrapper.Value == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            Topology topology = objectWrapper.Value as Topology;
            if(topology == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            Spatial.IGeometry3D geometry3D = Convert.ToSAM(topology);
            if(geometry3D == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Cannot convert geometry");
                return;
            }

            dataAccess.SetData(0, geometry3D);
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
                return Resources.SAM_Topologic;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("0fca6d19-95ee-4461-9be1-ab784aa800db"); }
        }
    }
}
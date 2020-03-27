using System;
using System.Collections.Generic;

using Grasshopper.Kernel;

using Topologic;

using SAM.Geometry.Grasshopper.Topologic.Properties;

namespace SAM.Geometry.Grasshopper.Topologic
{
    public class TopologyGeometry : GH_Component
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("247cdc4d-ebe1-4083-a150-b412bcc31412");

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.SAM_Topologic;

        public TopologyGeometry()
          : base("Topology.Geometry", "Topology.Geometry", "Convert Topologic Geometry to Grasshopper Geometry", "SAM", "Topologic")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager manager)
        {
            manager.AddGenericParameter("_topology", "_topology", "Topology Geometry", GH_ParamAccess.item);
            manager.AddNumberParameter("_tolerance_", "_tolerance_", "Tolerance", GH_ParamAccess.item, Core.Tolerance.Distance);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Geometry", "Geometry", "Geometry", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Topology topology = null;
            double tolerance = Core.Tolerance.Distance;

            if (!DA.GetData(0, ref topology))
                return;

            if (!DA.GetData(1, ref tolerance))
                return;

            if (topology == null)
                return;

            List<object> geometries = Convert.ToRhino(topology, tolerance);
            
            DA.SetDataList(0, geometries);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;

using Topologic;

using SAM.Geometry.Grasshopper.Topologic.Properties;

namespace TopologicGH
{
    public class TopologyGeometry : GH_Component
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("8bea7d74f-0834-4f92-b845-fe6f76f0ba84");

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
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("_topology", "_topology", "Topology Geometry", GH_ParamAccess.item);
            pManager.AddNumberParameter("_tolerance_", "_tolerance_", "Tolerance", GH_ParamAccess.item, SAM.Core.Tolerance.Distance);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
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
            double tolerance = SAM.Core.Tolerance.Distance;

            if (!DA.GetData(0, ref topology))
                return;

            if (!DA.GetData(1, ref tolerance))
                return;

            if (topology == null)
                return;

            List<object> geometries = SAM.Geometry.Grasshopper.Topologic.Convert.ToRhino(topology, tolerance);
            
            DA.SetDataList(0, geometries);
        }
    }
}

using Grasshopper.Kernel;
using SAM.Core.Grasshopper;
using SAM.Geometry.Grasshopper.Topologic.Properties;
using System;
using System.Collections.Generic;
using Topologic;

namespace SAM.Geometry.Grasshopper.Topologic
{
    public class TopologyTriangulate : GH_SAMComponent
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("6f863686-1a99-46f7-af45-7bca77137366");

        /// <summary>
        /// The latest version of this component
        /// </summary>
        public override string LatestComponentVersion => "1.0.0";

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.SAM_Topologic;

        public TopologyTriangulate()
          : base("Topology.Triangulate", "Topology.Triangulate", "Triangulates Topology (Faces only)", "SAM", "Topologic")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager manager)
        {
            manager.AddGenericParameter("_topology", "_topology", "Topology to be triangulated \nCurrently works on Faces", GH_ParamAccess.item);
            manager.AddNumberParameter("_tolerance_", "_tolerance_", "Tolerance", GH_ParamAccess.item, Core.Tolerance.MacroDistance);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager manager)
        {
            manager.AddGenericParameter("topologies", "topologies", "Topologies", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Topology topology = null;

            if (!DA.GetData(0, ref topology))
            { 
                return; 
            }

            if(!(topology is global::Topologic.Face))
            {

            }

            double tolerance = Core.Tolerance.MacroDistance;
            if (!DA.GetData(1, ref tolerance))
            {
                return;
            }

            IEnumerable<global::Topologic.Face> faces = global::Topologic.Utilities.FaceUtility.Triangulate((global::Topologic.Face)topology, tolerance);

            DA.SetDataList(0, faces);
        }
    }
}
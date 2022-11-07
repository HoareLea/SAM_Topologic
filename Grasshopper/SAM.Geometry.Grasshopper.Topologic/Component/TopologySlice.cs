using Grasshopper.Kernel;
using SAM.Core.Grasshopper;
using SAM.Geometry.Grasshopper.Topologic.Properties;
using System;
using Topologic;

namespace SAM.Geometry.Grasshopper.Topologic
{
    public class TopologySlice : GH_SAMComponent
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("d6480664-06b2-4e0c-839c-d21f8da9fa3c");

        /// <summary>
        /// The latest version of this component
        /// </summary>
        public override string LatestComponentVersion => "1.0.0";

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.SAM_Topologic;

        public TopologySlice()
          : base("Topology.Slice", "Topology.Slice", "Slices the input Topology with another Topology", "SAM", "Topologic")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager manager)
        {
            manager.AddGenericParameter("_topology", "_topology", "Topology will be sliced", GH_ParamAccess.item);
            manager.AddGenericParameter("_sliceTopology", "_sliceTopology", "Slice Topology", GH_ParamAccess.item);
            manager[1].Optional = false;
            manager.AddBooleanParameter("_transferDictionary_", "_transferDictionary_", "Transfer Dictionary", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager manager)
        {
            manager.AddGenericParameter("Topology", "Topology", "Topology", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Topology topology = null;
            Topology topology_Slice = null;
            bool transferDictionary = false;

            if (!DA.GetData(0, ref topology))
            { 
                return; 
            }

            if (!DA.GetData(1, ref topology_Slice)) 
            { 
                return; 
            }

            if (!DA.GetData(2, ref transferDictionary)) 
            { 
                return; 
            }

            if (topology == null) 
            { 
                return; 
            }

            Topology topology_New = topology.Slice(topology_Slice, transferDictionary);

            DA.SetData(0, topology_New);
        }
    }
}
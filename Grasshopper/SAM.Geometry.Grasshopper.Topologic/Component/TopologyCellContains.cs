using Grasshopper.Kernel;
using SAM.Core.Grasshopper;
using SAM.Geometry.Grasshopper.Topologic.Properties;
using System;
using Topologic;

namespace SAM.Geometry.Grasshopper.Topologic
{
    public class TopologyCellContains : GH_SAMComponent
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("1c6c3645-52b9-4d10-acf4-5b79d4351d88");

        /// <summary>
        /// The latest version of this component
        /// </summary>
        public override string LatestComponentVersion => "1.0.0";

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.SAM_Topologic;

        public TopologyCellContains()
          : base("Topology.CellContains", "Topology.CellContains", "Check if a Vertex is contained in a Cell or not", "SAM", "Topologic")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager manager)
        {
            int index = -1;

            index = manager.AddGenericParameter("_cell", "_cell", "Cell", GH_ParamAccess.item);
            manager[index].DataMapping = GH_DataMapping.Flatten;

            index = manager.AddGenericParameter("_vertex", "_vertex", "Vertex", GH_ParamAccess.item);
            manager[index].DataMapping = GH_DataMapping.Graft;

            manager.AddBooleanParameter("_allowOnBoundary", "_allowOnBoundary", "Allow On Boundary", GH_ParamAccess.item);
            manager.AddNumberParameter("_tolerance_", "_tolerance_", "Tolerance", GH_ParamAccess.item, SAM.Core.Tolerance.MacroDistance);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager manager)
        {
            manager.AddBooleanParameter("Contains", "Contains", "Contains", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare a variable for the input String
            Cell cell = null;
            Vertex vertex = null;
            bool allowOnBoundary = false;
            double tolerance = Core.Tolerance.MacroDistance;

            if (!DA.GetData(0, ref cell)) 
            { 
                return; 
            }

            if (!DA.GetData(1, ref vertex)) 
            { return; 
            }

            if (!DA.GetData(2, ref allowOnBoundary)) 
            { 
                return; 
            }

            if (!DA.GetData(3, ref tolerance)) 
            { 
                return; 
            }

            if (cell == null) 
            { 
                return; 
            }

            if (vertex == null) 
            { 
                return; 
            }

            bool isContained = global::Topologic.Utilities.CellUtility.Contains(cell, vertex, allowOnBoundary, tolerance);

            DA.SetData(0, isContained);
        }
    }
}
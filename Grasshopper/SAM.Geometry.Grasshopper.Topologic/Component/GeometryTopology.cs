using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using SAM.Core.Grasshopper;
using SAM.Geometry.Grasshopper.Topologic.Properties;
using System;

namespace SAM.Geometry.Grasshopper.Topologic
{
    public class GeometryTopology : GH_SAMComponent
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("1c98cd19-b430-48e5-a626-4d103df4fe1c");

        /// <summary>
        /// The latest version of this component
        /// </summary>
        public override string LatestComponentVersion => "1.0.0";

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.SAM_Topologic3a;

        /// <summary>
        /// Initializes a new instance of the SAM_point3D class.
        /// </summary>
        public GeometryTopology()
          : base("Geometry.Topology", "Geometry.Topology",
              "Convert Rhino Geometry To Topologic Geometry",
              "SAM", "Topologic")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager inputParamManager)
        {
            inputParamManager.AddGeometryParameter("_geometry", "_geometry", "Rhino Geometry", GH_ParamAccess.item);
            inputParamManager.AddNumberParameter("_tolerance_", "_tolerance_", "Tolerance", GH_ParamAccess.item, Core.Tolerance.MacroDistance);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager outputParamManager)
        {
            outputParamManager.AddGenericParameter("Topology", "Topology", "Topology Geometry", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="dataAccess">
        /// The DA object is used to retrieve from inputs and store in outputs.
        /// </param>
        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            object @object = null;
            double tolerance = 0.0001;
            if (!dataAccess.GetData(0, ref @object)) 
            { 
                return; 
            }

            if (!dataAccess.GetData(1, ref tolerance)) 
            { 
                return; 
            }

            if (@object == null) 
            { 
                return; 
            }
            
            Type type = @object.GetType();

            global::Topologic.Topology topology = null;
            GH_Point ghPoint = @object as GH_Point;
            if (ghPoint != null)
            {
                topology = ghPoint.Value.ToTopologic();
                dataAccess.SetData(0, topology);
                return;
            }

            GH_Line ghLine = @object as GH_Line;
            if (ghLine != null)
            {
                topology = ghLine.Value.ToTopologic();
                dataAccess.SetData(0, topology);
                return;
            }

            GH_Curve ghCurve = @object as GH_Curve;
            if (ghCurve != null)
            {
                topology = ghCurve.Value.ToTopologic();
                dataAccess.SetData(0, topology);
                return;
            }

            GH_Surface ghSurface = @object as GH_Surface;
            if (ghSurface != null)
            {
                topology = ghSurface.Value.ToTopologic(tolerance);
                dataAccess.SetData(0, topology);
                return;
            }

            GH_Brep ghBrep = @object as GH_Brep;
            if (ghBrep != null)
            {
                topology = ghBrep.Value.ToTopologic(tolerance);
                dataAccess.SetData(0, topology);
                return;
            }

            GH_Box ghBox = @object as GH_Box;
            if (ghBox != null)
            {
                topology = ghBox.Value.ToTopologic();
                dataAccess.SetData(0, topology);
                return;
            }

            GH_Mesh ghMesh = @object as GH_Mesh;
            if (ghMesh != null)
            {
                topology = ghMesh.Value.ToTopologic();
                dataAccess.SetData(0, topology);
                return;
            }

            throw new Exception("Cannot convert geometry.");
        }
    }
}
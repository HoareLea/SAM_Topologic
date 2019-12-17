using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using SAM.Analytical.Grasshopper.Topologic.Properties;

namespace SAM.Analytical.Grasshopper.Topologic
{
    public class TopologicAnalyticalGeometry : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SAM_point3D class.
        /// </summary>
        public TopologicAnalyticalGeometry()
          : base("TopologicAnalyticalGeometry", "TopoGeo",
              "Convert SAM Analytical To Topologic Geometry",
              "SAM", "Topologic")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager inputParamManager)
        {
            inputParamManager.AddGenericParameter("_analyticalObject", "_analyticalObject", "SAM Analytical Object: Panel", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager outputParamManager)
        {
            outputParamManager.AddGenericParameter("topologicGeometry", "TopoGeo", "Topologic Geometry", GH_ParamAccess.item);
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

            object obj = objectWrapper.Value;

            Panel panel = obj as Panel;
            if(panel != null)
            {
                dataAccess.SetData(0, Analytical.Topologic.Convert.ToTopologic((Panel)obj));
                return;
            }

            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Cannot convert geometry");

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
            get { return new Guid("d3dc979f-71b4-444d-8b75-9c29d1f7e769"); }
        }
    }
}
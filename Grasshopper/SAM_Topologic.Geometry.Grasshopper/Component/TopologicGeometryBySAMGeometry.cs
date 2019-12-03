using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using SAM_Topologic.Geometry.Grasshopper.Properties;
using SAM.Geometry.Spatial;

namespace SAM.Geometry.Grasshopper
{
    public class TopologicGeometryBySAMGeometry : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SAM_point3D class.
        /// </summary>
        public TopologicGeometryBySAMGeometry()
          : base("TopologicGeometryBySAMGeometry", "TopoGeo",
              "Convert SAM Geometry To Topologic Geometry",
              "SAM", "Topologic")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager inputParamManager)
        {
            inputParamManager.AddGenericParameter("SAMGeometry", "SAMgeo", "SAM Geometry", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager outputParamManager)
        {
            outputParamManager.AddGenericParameter("TopologicGeometry", "TopoGeo", "Topologic Geometry", GH_ParamAccess.item);
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

            Point3D point3D = obj as Point3D;
            if (point3D != null)
            {
                dataAccess.SetData(0, Topologic.Convert.ToTopologic(point3D));
                return;
            }

            Segment3D segment3D = obj as Segment3D;
            if (segment3D != null)
            {
                dataAccess.SetData(0, Topologic.Convert.ToTopologic(segment3D));
                return;
            }

            Polygon3D polygon3D = obj as Polygon3D;
            if (polygon3D != null)
            {
                dataAccess.SetData(0, Topologic.Convert.ToTopologic(polygon3D));
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
            get { return new Guid("8904de02-93b6-4d21-8d04-2ee1acb1e53c"); }
        }
    }
}
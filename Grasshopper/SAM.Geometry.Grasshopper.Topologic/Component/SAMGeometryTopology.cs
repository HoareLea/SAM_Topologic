using System;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using SAM.Geometry.Grasshopper.Topologic.Properties;
using SAM.Geometry.Spatial;

namespace SAM.Geometry.Grasshopper.Topologic
{
    public class SAMGeometryTopology : GH_Component
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("8904de02-93b6-4d21-8d04-2ee1acb1e53c");

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.SAM_Topologic;

        /// <summary>
        /// Initializes a new instance of the SAM_point3D class.
        /// </summary>
        public SAMGeometryTopology()
          : base("SAMGeometry.Topology", "SAMGeometry.Topology",
              "Convert SAM Geometry To Topologic Geometry",
              "SAM", "Topologic")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager inputParamManager)
        {
            inputParamManager.AddParameter(new GooSAMGeometryParam(), "_SAMGeometry", "_SAMGeometry", "SAM Geometry: Polygon3D, Segment3D, Point3D", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager outputParamManager)
        {
            outputParamManager.AddGenericParameter("Topology", "Topology", "Topology Geometry: Wire, Edge, Vertex", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="dataAccess">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            ISAMGeometry sAMGeometry = null;
            if (!dataAccess.GetData(0, ref sAMGeometry) || sAMGeometry == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            Point3D point3D = sAMGeometry as Point3D;
            if (point3D != null)
            {
                dataAccess.SetData(0, Geometry.Topologic.Convert.ToTopologic(point3D));
                return;
            }

            ICurve3D curve3D = sAMGeometry as ICurve3D;
            if (curve3D != null)
            {
                dataAccess.SetData(0, Geometry.Topologic.Convert.ToTopologic(curve3D));
                return;
            }

            Polygon3D polygon3D = sAMGeometry as Polygon3D;
            if (polygon3D != null)
            {
                dataAccess.SetData(0, Geometry.Topologic.Convert.ToTopologic(polygon3D));
                return;
            }

            Face3D face3D = sAMGeometry as Face3D;
            if (face3D != null)
            {
                dataAccess.SetData(0, Geometry.Topologic.Convert.ToTopologic(face3D));
                return;
            }

            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Cannot convert geometry");
        }
    }
}
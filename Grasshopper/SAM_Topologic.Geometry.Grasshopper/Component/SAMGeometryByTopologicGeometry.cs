using System;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using SAM_Topologic.Geometry.Grasshopper.Properties;
using Topologic;

namespace SAM.Geometry.Grasshopper
{
    public class SAMGeometryByTopologicGeometry : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SAMGeometryByGHGeometry class.
        /// </summary>
        public SAMGeometryByTopologicGeometry()
          : base("SAMGeometryByTopologicGeometry", "SAMgeo",
              "Description Convert SAM Geometry into Topologic Geometry",
              "SAM", "Topologic")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager inputParamManager)
        {
            inputParamManager.AddGenericParameter("TopologicGeometry", "TopoGeo", "Topologic Geometry", GH_ParamAccess.item);
        }


        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager outputParamManager)
        {
            outputParamManager.AddGenericParameter("SAMGeometry", "SAMgeo", "SAM Geometry", GH_ParamAccess.item);
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

            Vertex vertex = obj as Vertex;
            if (vertex != null)
            { 
                dataAccess.SetData(0, Topologic.Convert.ToSAM(vertex));
                return;
            }

            Edge edge = obj as Edge;
            if (edge != null)
            {
                dataAccess.SetData(0, Topologic.Convert.ToSAM(edge));
                return;
            }

            Wire wire = obj as Wire;
            if (wire != null)
            {
                dataAccess.SetData(0, Topologic.Convert.ToSAM(wire));
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
            get { return new Guid("0fca6d19-95ee-4461-9be1-ab784aa800db"); }
        }
    }
}
using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using SAM_Topologic.Analytical.Grasshopper.Properties;
using Topologic;

namespace SAM.Analytical.Grasshopper
{
    public class UpdatePanels : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SAM_point3D class.
        /// </summary>
        public UpdatePanels()
          : base("UpdatePanels", "TopoGeo",
              "Convert SAM Geometry To Topologic Geometry",
              "SAM", "Topologic")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager inputParamManager)
        {
            inputParamManager.AddGenericParameter("Panels", "SAMgeo", "SAM Geometry", GH_ParamAccess.list);
            inputParamManager.AddGenericParameter("Points", "SAMgeo", "SAM Geometry", GH_ParamAccess.list);
            inputParamManager.AddGenericParameter("Dictionary", "SAMgeo", "SAM Geometry", GH_ParamAccess.list);
            inputParamManager.AddGenericParameter("Tolerance", "SAMgeo", "SAM Geometry", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager outputParamManager)
        {
            outputParamManager.AddGenericParameter("Panels", "TopoGeo", "Topologic Geometry", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="dataAccess">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            List<GH_ObjectWrapper> objectWrapperList = new List<GH_ObjectWrapper>();

            if (!dataAccess.GetDataList(0, objectWrapperList) || objectWrapperList == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }


            List<Face> faceList = new List<Face>();
            foreach(GH_ObjectWrapper gHObjectWraper in objectWrapperList)
            {
                Panel panel = gHObjectWraper.Value as Panel;
                if (panel == null)
                    continue;

                Face face = Topologic.Convert.ToTopologic(panel);
                if (face == null)
                    continue;

                faceList.Add(face);
            }

            GH_ObjectWrapper objectWrapper = null;
            if (!dataAccess.GetData(0, ref objectWrapper) || objectWrapper.Value == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            GH_Number gHNumber = objectWrapper.Value as GH_Number;
            if(gHNumber == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            CellComplex cellComplex = CellComplex.ByFaces(faceList, gHNumber.Value);
            if(cellComplex == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }



            dataAccess.SetDataList(0, cellComplex.Faces);
            return;

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
            get { return new Guid("8df2e1bf-81fd-4d9e-b02b-4b6389769fa2"); }
        }
    }
}
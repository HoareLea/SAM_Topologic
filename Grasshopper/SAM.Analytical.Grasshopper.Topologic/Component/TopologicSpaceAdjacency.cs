using System;
using System.Collections.Generic;

using Topologic;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using SAM.Analytical.Grasshopper.Topologic.Properties;

namespace SAM.Analytical.Grasshopper.Topologic
{
    public class TopologicSpaceAdjacency : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SAM_point3D class.
        /// </summary>
        public TopologicSpaceAdjacency()
          : base("TopologicSpaceAdjacency", "TopoGeo",
              "Convert SAM Analytical Panel To Topologic Cellcomplex and then rerun for each Face list of Adjacent Space Names",
              "SAM", "Topologic")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager inputParamManager)
        {
            inputParamManager.AddGenericParameter("_panels", "_panels", "SAM Analytical Panels", GH_ParamAccess.list);
            inputParamManager.AddGenericParameter("_spaces", "_spaces", "SAM Analytical Spaces", GH_ParamAccess.list);
            inputParamManager.AddNumberParameter("_tolerance_", "_tolerance_", "Topologic CellComplex default 0.001", GH_ParamAccess.item, SAM.Geometry.Tolerance.MacroDistance);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager outputParamManager)
        {
            outputParamManager.AddGenericParameter("geometry", "geometry", "Face Geometry", GH_ParamAccess.list);
            outputParamManager.AddGenericParameter("faceAdjSpaceNames", "TopoGeo", "List of Adj Space Names for each face", GH_ParamAccess.list);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="dataAccess">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            List<GH_ObjectWrapper> objectWrapperList = null;

            objectWrapperList = new List<GH_ObjectWrapper>();

            if (!dataAccess.GetDataList(0, objectWrapperList) || objectWrapperList == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            List<Panel> panelList = new List<Panel>();
            foreach(GH_ObjectWrapper gHObjectWraper in objectWrapperList)
            {
                Panel panel = gHObjectWraper.Value as Panel;
                if (panel == null)
                    continue;

                panelList.Add(panel);
            }

            objectWrapperList = new List<GH_ObjectWrapper>();

            if (!dataAccess.GetDataList(1, objectWrapperList) || objectWrapperList == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            
            List<Space> spaceList = new List<Space>();
            foreach (GH_ObjectWrapper gHObjectWraper in objectWrapperList)
            {
                Space space = gHObjectWraper.Value as Space;
                if (space == null)
                    continue;

                spaceList.Add(space);
            }

            GH_ObjectWrapper objectWrapper = null;
            if (!dataAccess.GetData(2, ref objectWrapper) || objectWrapper.Value == null)
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

            List<Geometry.Spatial.IGeometry3D> geometryList = new List<Geometry.Spatial.IGeometry3D>();
            List<List<string>> names = new List<List<string>>();

            if (Analytical.Topologic.Query.TryGetSpaceAdjacency(panelList, spaceList, gHNumber.Value, out geometryList, out names))
            {
                DataTree<string> dataTree = new DataTree<string>();
                for (int i = 0; i < names.Count; i++)
                {
                    GH_Path path = new GH_Path(i);
                    foreach (string name in names[i])
                        dataTree.Add(name, path);
                }

                dataAccess.SetDataList(0, geometryList);
                dataAccess.SetDataTree(1, dataTree);
            }
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
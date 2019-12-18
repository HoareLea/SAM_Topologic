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

            List<Face> faceList = new List<Face>();
            foreach(GH_ObjectWrapper gHObjectWraper in objectWrapperList)
            {
                Panel panel = gHObjectWraper.Value as Panel;
                if (panel == null)
                    continue;

                Face face = Analytical.Topologic.Convert.ToTopologic(panel);
                if (face == null)
                    continue;

                faceList.Add(face);
            }

            objectWrapperList = new List<GH_ObjectWrapper>();

            if (!dataAccess.GetDataList(1, objectWrapperList) || objectWrapperList == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            
            List<Topology> topologyList = new List<Topology>();
            foreach (GH_ObjectWrapper gHObjectWraper in objectWrapperList)
            {
                Space space = gHObjectWraper.Value as Space;
                if (space == null)
                    continue;

                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                dictionary["Name"] = space.Name;

                Vertex vertex = Geometry.Topologic.Convert.ToTopologic(space.Location);
                vertex = (Vertex)vertex.SetDictionary(dictionary);
                topologyList.Add(vertex);
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

            CellComplex cellComplex = CellComplex.ByFaces(faceList, gHNumber.Value);
            if(cellComplex == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            cellComplex = (CellComplex)cellComplex.AddContents(topologyList, 32);

            int index_1 = 0;

            DataTree<string> nameDataTree = new DataTree<string>();
            List<Geometry.Spatial.IGeometry3D> geometryList = new List<Geometry.Spatial.IGeometry3D>();
            foreach (Face face in cellComplex.Faces)
            {
                geometryList.Add(Geometry.Topologic.Convert.ToSAM(face));
                foreach (Cell cell in face.Cells)
                {
                    foreach(Topology topology in cell.Contents)
                    {
                        Vertex vertex = topology as Vertex;
                        if (vertex == null)
                            continue;

                        GH_Path path = new GH_Path(index_1);
                        nameDataTree.Add(vertex.Dictionary["Name"] as string, path);
                    }
                }

                index_1++;
            }

            dataAccess.SetDataList(0, geometryList);
            dataAccess.SetDataTree(1, nameDataTree);

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
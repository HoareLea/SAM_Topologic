using System;
using System.Collections.Generic;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using SAM.Analytical.Grasshopper.Topologic.Properties;
using Topologic;

namespace SAM.Analytical.Grasshopper.Topologic
{
    public class TopologicAdjacencyInformation : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SAM_point3D class.
        /// </summary>
        public TopologicAdjacencyInformation()
          : base("AdjacencyInformation", "CellComplexByFaces",
              "Create Topologic CellComplex by Topologic Face",
              "SAM", "Topologic")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager inputParamManager)
        {
            inputParamManager.AddGenericParameter("AdjacencyCluster", "AdjacencyCluster", "AdjacencyCluster", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager outputParamManager)
        {
            outputParamManager.AddGenericParameter("Geometry", "Geometry", "Geometry", GH_ParamAccess.list);
            outputParamManager.AddGenericParameter("Names", "Names", "Names", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="dataAccess">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            Analytical.Topologic.AdjacencyCluster adjacencyCluster = null;

            if (!dataAccess.GetData<Analytical.Topologic.AdjacencyCluster>(0, ref adjacencyCluster))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            List<Panel> panelList = new List<Panel>();

            List<Panel> panelList_Temp = null;

            panelList_Temp = adjacencyCluster.GetExternalPanels();
            if (panelList_Temp != null && panelList_Temp.Count != 0)
                panelList.AddRange(panelList_Temp);

            panelList_Temp = adjacencyCluster.GetInternalPanels();
            if (panelList_Temp != null && panelList_Temp.Count != 0)
                panelList.AddRange(panelList_Temp);

            List<SAM.Geometry.Spatial.Surface> surfaces = new List<Geometry.Spatial.Surface>();

            DataTree<string> dataTree = new DataTree<string>();
            for (int i=0; i < panelList.Count; i++)
            {
                Panel panel = panelList[i];

                List<Space> spaces = adjacencyCluster.GetPanelSpaces(panel.Guid);
                GH_Path path = new GH_Path(i);
                foreach (string name in spaces.ConvertAll(x => x.Name))
                    dataTree.Add(name, path);
            }

            dataAccess.SetDataList(0, surfaces);
            dataAccess.SetDataTree(1, dataTree);
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
            get { return new Guid("1189208f-a10b-4693-ba93-fc7f86dc2bad"); }
        }
    }
}
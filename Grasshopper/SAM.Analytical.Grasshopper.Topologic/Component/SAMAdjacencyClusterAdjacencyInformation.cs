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
    public class SAMAdjacencyClusterAdjacencyInformation : GH_Component
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("1189208f-a10b-4693-ba93-fc7f86dc2bad");

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.SAM_Topologic;

        /// <summary>
        /// Initializes a new instance of the SAM_point3D class.
        /// </summary>
        public SAMAdjacencyClusterAdjacencyInformation()
          : base("SAMAdjacencyCluster.AdjacencyInformation", "SAMAdjacencyCluster.AdjacencyInformation",
              "Outout Geometry and Ajd names from Panels",
              "SAM", "Topologic")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager inputParamManager)
        {
            inputParamManager.AddGenericParameter("_adjacencyCluster", "_adjacencyCluster", "SAM AdjacencyCluster", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager outputParamManager)
        {
            outputParamManager.AddGeometryParameter("Geometry", "Geometry", "GH Geometry from SAM Analytical Panel", GH_ParamAccess.list);
            outputParamManager.AddTextParameter("SpaceAdjNames", "SpaceAdjNames", "Space Adjacency Names, to which Space each Panel is connected", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="dataAccess">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            Analytical.Topologic.AdjacencyCluster adjacencyCluster = null;

            if (!dataAccess.GetData(0, ref adjacencyCluster))
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

            List<IGH_GeometricGoo> geometricGoos = new List<IGH_GeometricGoo>();

            DataTree<string> dataTree = new DataTree<string>();
            for (int i=0; i < panelList.Count; i++)
            {
                Panel panel = panelList[i];

                geometricGoos.Add(Geometry.Grasshopper.Convert.ToGrasshopper( panel.GetFace()));

                List<Space> spaces = adjacencyCluster.GetPanelSpaces(panel.Guid);
                GH_Path path = new GH_Path(i);
                foreach (string name in spaces.ConvertAll(x => x.Name))
                    dataTree.Add(name, path);
            }

            dataAccess.SetDataList(0, geometricGoos);
            dataAccess.SetDataTree(1, dataTree);
            return;

        }
    }
}
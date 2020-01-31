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
            inputParamManager.AddParameter(new GooAdjacencyClusterParam(), "_adjacencyCluster", "_adjacencyCluster", "SAM AdjacencyCluster", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager outputParamManager)
        {
            outputParamManager.AddGenericParameter("PanelTypes", "PanelTypes", "SAM Analytical PanelTypes", GH_ParamAccess.list);
            outputParamManager.AddGeometryParameter("Geometries", "Geometries", "GH Geometries from SAM Analytical Panels", GH_ParamAccess.tree);
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

            Dictionary<Panel, PanelType> dictionary = new Dictionary<Panel, PanelType>();

            List<Panel> panelList = null;

            //Internal
            panelList = adjacencyCluster.GetExternalPanels();
            if (panelList != null && panelList.Count != 0)
            {
                foreach (Panel panel in panelList)
                {
                    PanelType panelType = panel.PanelType;

                    switch (panelType)
                    {
                        case PanelType.Wall:
                            panelType = PanelType.WallExternal;
                            break;
                        case PanelType.Floor:
                            panelType = PanelType.SlabOnGrade;
                            break;
                    }

                    dictionary[panel] = panelType;
                }
            }

            //External
            panelList = adjacencyCluster.GetExternalPanels();
            if (panelList != null && panelList.Count != 0)
            {
                foreach (Panel panel in panelList)
                {
                    PanelType panelType = panel.PanelType;

                    switch (panelType)
                    {
                        case PanelType.Wall:
                            panelType = PanelType.WallInternal;
                            break;
                        case PanelType.Floor:
                            panelType = PanelType.FloorInternal;
                            break;
                    }

                    dictionary[panel] = panelType;
                }
            }

            //Shading
            panelList = adjacencyCluster.GetShadingPanels();
            if (panelList != null && panelList.Count != 0)
            {
                foreach (Panel panel in panelList)
                    dictionary[panel] = PanelType.Shade;
            }

            DataTree<string> dataTree_Names = new DataTree<string>();
            DataTree<IGH_GeometricGoo> dataTree_GeometricGoos = new DataTree<IGH_GeometricGoo>();
            int i = 0;
            foreach (KeyValuePair<Panel, PanelType> keyValuePair in dictionary)
            {
                Panel panel = keyValuePair.Key;

                List<Space> spaces = adjacencyCluster.GetPanelSpaces(panel.Guid);
                GH_Path path = new GH_Path(i);
                foreach (string name in spaces.ConvertAll(x => x.Name))
                    dataTree_Names.Add(name, path);

                dataTree_GeometricGoos.Add(Geometry.Grasshopper.Convert.ToGrasshopper(panel.GetFace()), path);

                i++;
            }

            dataAccess.SetDataList(0, dictionary.Values);
            dataAccess.SetDataTree(1, dataTree_GeometricGoos);
            dataAccess.SetDataTree(2, dataTree_Names);
            return;
        }
    }
}
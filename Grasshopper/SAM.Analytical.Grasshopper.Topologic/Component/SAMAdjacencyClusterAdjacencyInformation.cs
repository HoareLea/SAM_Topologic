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
            
            inputParamManager.AddTextParameter("_wallInternalConstructionPrefix_", "_wicp_", "Wall Internal Construction Prefix", GH_ParamAccess.item, "Basic Wall: SIM_INT_");
            inputParamManager.AddTextParameter("_wallInternalConstructionSufix_", "_wics_", "Wall Internal Construction Sufix", GH_ParamAccess.item, "SLD_Partition");

            inputParamManager.AddTextParameter("_wallExternalConstructionPrefix_", "_wecp_", "Wall External Construction Prefix", GH_ParamAccess.item, "Basic Wall: SIM_EXT_");
            inputParamManager.AddTextParameter("_wallExternalConstructionSufix_", "_wecs_", "Wall External Construction Sufix", GH_ParamAccess.item, "SLD");

            inputParamManager.AddTextParameter("_wallShadingConstructionPrefix_", "_wscp_", "Wall Shading Construction Prefix", GH_ParamAccess.item, "Basic Wall: SIM_EXT_");
            inputParamManager.AddTextParameter("_wallShadingConstructionSufix_", "_wscs_", "Wall Shading Construction Sufix", GH_ParamAccess.item, "SLD");

            inputParamManager.AddTextParameter("_floorInternalConstructionPrefix_", "_ficp_", "Floor Internal Construction Prefix", GH_ParamAccess.item, "Floor: SIM_INT_");
            inputParamManager.AddTextParameter("_floorInternalConstructionSufix_", "_fics_", "Floor Internal Construction Sufix", GH_ParamAccess.item, "SLD_FLR FLR02");

            inputParamManager.AddTextParameter("_floorExternalConstructionPrefix_", "_fecp_", "Floor External Construction Prefix", GH_ParamAccess.item, "Floor: SIM_EXT_GRD_");
            inputParamManager.AddTextParameter("_floorExternalConstructionSufix_", "_fecs_", "Floor External Construction Sufix", GH_ParamAccess.item, "FLR FLR01");

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager outputParamManager)
        {
            outputParamManager.AddParameter(new GooPanelParam(), "Panels", "Panels", "SAM Analytical Panels", GH_ParamAccess.list);
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

            string wallInternalConstructionPrefix = null;
            if(!dataAccess.GetData(1, ref wallInternalConstructionPrefix))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            string wallInternalConstructionSufix = null;
            if (!dataAccess.GetData(2, ref wallInternalConstructionSufix))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            string wallExternalConstructionPrefix = null;
            if (!dataAccess.GetData(3, ref wallExternalConstructionPrefix))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            string wallExternalConstructionSufix = null;
            if (!dataAccess.GetData(4, ref wallExternalConstructionSufix))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            string wallShadingConstructionPrefix = null;
            if (!dataAccess.GetData(5, ref wallShadingConstructionPrefix))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            string wallShadingConstructionSufix = null;
            if (!dataAccess.GetData(6, ref wallShadingConstructionSufix))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            string floorInternalConstructionPrefix = null;
            if (!dataAccess.GetData(7, ref floorInternalConstructionPrefix))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            string floorInternalConstructionSufix = null;
            if (!dataAccess.GetData(8, ref floorInternalConstructionSufix))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            string floorExternalConstructionPrefix = null;
            if (!dataAccess.GetData(9, ref floorExternalConstructionPrefix))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            string floorExternalConstructionSufix = null;
            if (!dataAccess.GetData(10, ref floorExternalConstructionSufix))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            Dictionary<Panel, PanelType> dictionary = new Dictionary<Panel, PanelType>();

            List<Panel> panelList = null;

            //Internal
            panelList = adjacencyCluster.GetInternalPanels();
            if (panelList != null && panelList.Count != 0)
            {
                foreach (Panel panel in panelList)
                {
                    PanelType panelType = panel.PanelType;

                    Panel panel_New = panel;

                    if (panelType == PanelType.Wall || panelType == PanelType.WallExternal)
                    {
                        if (!panel_New.Construction.Name.StartsWith(wallInternalConstructionPrefix))
                        {
                            panelType = PanelType.WallInternal;

                            panel_New = new Panel(panel, new Construction(wallInternalConstructionPrefix + wallInternalConstructionSufix));
                            panel_New = new Panel(panel_New, panelType);
                        }
                    }
                    else if (panelType == PanelType.Roof || panelType == PanelType.Floor || panelType == PanelType.FloorExposed || panelType == PanelType.SlabOnGrade)
                    {
                        if (!panel_New.Construction.Name.StartsWith(floorInternalConstructionPrefix))
                        {
                            panelType = PanelType.FloorInternal;

                            panel_New = new Panel(panel, new Construction(floorInternalConstructionPrefix + floorInternalConstructionSufix));
                            panel_New = new Panel(panel_New, panelType);
                        }
                    }


                    dictionary[panel_New] = panelType;
                }
            }

            //External
            panelList = adjacencyCluster.GetExternalPanels();
            if (panelList != null && panelList.Count != 0)
            {
                foreach (Panel panel in panelList)
                {
                    PanelType panelType = panel.PanelType;

                    Panel panel_New = panel;
                    
                    if (panelType == PanelType.Wall)
                    {
                        if (!panel_New.Construction.Name.StartsWith(wallExternalConstructionPrefix))
                        {
                            panelType = PanelType.WallExternal;

                            panel_New = new Panel(panel, new Construction(wallExternalConstructionPrefix + wallExternalConstructionSufix));
                            panel_New = new Panel(panel_New, panelType);
                        }
                    }
                    else if (panelType == PanelType.Floor)
                    {
                        if (!panel_New.Construction.Name.StartsWith(floorExternalConstructionPrefix))
                        {
                            panelType = PanelType.SlabOnGrade;

                            panel_New = new Panel(panel, new Construction(floorExternalConstructionPrefix + floorExternalConstructionSufix));
                            panel_New = new Panel(panel_New, panelType);
                        }
                    }


                    dictionary[panel_New] = panelType;
                }
            }

            //Shading
            panelList = adjacencyCluster.GetShadingPanels();
            if (panelList != null && panelList.Count != 0)
            {
                foreach (Panel panel in panelList)
                {
                    PanelType panelType = panel.PanelType;

                    Panel panel_New = panel;

                    if (panelType == PanelType.Wall)
                    {
                        if (!panel_New.Construction.Name.StartsWith(wallShadingConstructionPrefix))
                        {
                            panelType = PanelType.WallExternal;

                            panel_New = new Panel(panel, new Construction(wallShadingConstructionPrefix + wallShadingConstructionSufix));
                            panel_New = new Panel(panel_New, panelType);
                        }
                    }

                    dictionary[panel_New] = panelType;
                }
            }

            DataTree<string> dataTree_Names = new DataTree<string>();
            DataTree<IGH_GeometricGoo> dataTree_GeometricGoos = new DataTree<IGH_GeometricGoo>();
            List<GooPanel> gooPanels = new List<GooPanel>();
            int count = 0;
            foreach (KeyValuePair<Panel, PanelType> keyValuePair in dictionary)
            {
                Panel panel = keyValuePair.Key;
                gooPanels.Add(new GooPanel(new Panel(panel, keyValuePair.Value)));

                GH_Path path = new GH_Path(count);

                List<Space> spaces = adjacencyCluster.GetPanelSpaces(panel.Guid);
                if (spaces != null && spaces.Count > 0)
                {
                    foreach (string name in spaces.ConvertAll(x => x.Name))
                        dataTree_Names.Add(name, path);
                }

                dataTree_GeometricGoos.Add(Geometry.Grasshopper.Convert.ToGrasshopper(panel.GetFace3D()), path);

                count++;
            }

            dataAccess.SetDataList(0, gooPanels);
            dataAccess.SetDataList(1, dictionary.Values);
            dataAccess.SetDataTree(2, dataTree_GeometricGoos);
            dataAccess.SetDataTree(3, dataTree_Names);
            return;
        }
    }
}
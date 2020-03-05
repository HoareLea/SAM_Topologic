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

            string wallInternalConstructionPrefix = "Basic Wall: SIM_INT_";
            string wallInternalConstructionSufix = "SLD_Partition";
            string wallExternalConstructionPrefix = "Basic Wall: SIM_EXT_";
            string wallExternalConstructionSufix = "SLD";
            string wallShadingConstructionPrefix = "Basic Wall: SIM_EXT_";
            string wallShadingConstructionSufix = "SLD";
            string floorInternalConstructionPrefix = "Floor: SIM_INT_";
            string floorInternalConstructionSufix = "SLD_FLR FLR02";
            string floorExternalConstructionPrefix = "Floor: SIM_EXT_GRD_";
            string floorExternalConstructionSufix = "FLR FLR01";
            string roofExternalConstructionPrefix = "Basic Roof: SIM_EXT_";
            string roofExternalConstructionSufix = "SLD_Roof DA01";
            string floorExposedConstructionPrefix = "Floor: SIM_EXT_";
            string floorExposedConstructionSufix = "SLD_FLR Exposed";
            string slabOnGradeConstructionPrefix = "Floor: SIM_EXT_";
            string slabOnGradeConstructionSufix = "GRD_FLR FLR01";

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
                    else if (panelType == PanelType.Floor || panelType == PanelType.Roof)
                    {
                        Geometry.Spatial.Vector3D vector3D_Normal = panel.PlanarBoundary3D?.GetNormal();

                        PanelType panelType_Normal = Query.PanelType(vector3D_Normal);
                        if (panelType_Normal == PanelType.Floor)
                        {
                            double elevation = Query.MaxElevation(panel_New);

                            if(elevation == 0)
                            {
                                if (!panel_New.Construction.Name.StartsWith(floorExternalConstructionPrefix))
                                {
                                    panelType = PanelType.SlabOnGrade;

                                    panel_New = new Panel(panel, new Construction(floorExternalConstructionPrefix + floorExternalConstructionSufix));
                                    panel_New = new Panel(panel_New, panelType);
                                }
                            }
                            else if (elevation < 0)
                            {
                                if (!panel_New.Construction.Name.StartsWith(slabOnGradeConstructionPrefix))
                                {
                                    panelType = PanelType.SlabOnGrade;

                                    panel_New = new Panel(panel, new Construction(slabOnGradeConstructionPrefix + slabOnGradeConstructionSufix));
                                    panel_New = new Panel(panel_New, panelType);
                                }
                            }
                            else
                            {
                                if (!panel_New.Construction.Name.StartsWith(floorExposedConstructionPrefix))
                                {
                                    panelType = PanelType.FloorExposed;

                                    panel_New = new Panel(panel, new Construction(floorExposedConstructionPrefix + floorExposedConstructionSufix));
                                    panel_New = new Panel(panel_New, panelType);
                                }
                            }
                        }
                        else if (panelType_Normal == PanelType.Roof)

                        {
                            if (!panel_New.Construction.Name.StartsWith(roofExternalConstructionPrefix))
                            {
                                panelType = PanelType.Roof;

                                panel_New = new Panel(panel, new Construction(roofExternalConstructionPrefix + roofExternalConstructionSufix));
                                panel_New = new Panel(panel_New, panelType);
                            }
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
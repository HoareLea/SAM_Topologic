﻿using System;
using System.Collections.Generic;

using Topologic;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using SAM.Analytical.Grasshopper.Topologic.Properties;

namespace SAM.Analytical.Grasshopper.Topologic
{
    public class TopologicCreateAdjacencyCluster : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SAM_point3D class.
        /// </summary>
        public TopologicCreateAdjacencyCluster()
          : base("CreateAdjacencyCluster", "TopoGeo",
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
            inputParamManager.AddNumberParameter("_tolerance_", "_tolerance_", string.Format("Topologic CellComplex default {0}", Geometry.Tolerance.MacroDistance), GH_ParamAccess.item, Geometry.Tolerance.MacroDistance);
            inputParamManager.AddBooleanParameter("_run_", "_run_", "Run", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager outputParamManager)
        {
            outputParamManager.AddGenericParameter("AdjacencyCluster", "AdjacencyCluster", "AdjacencyCluster", GH_ParamAccess.item);
            outputParamManager.AddGenericParameter("Topology", "Topology", "Topology", GH_ParamAccess.item);
            outputParamManager.AddBooleanParameter("Sucessfull", "Sucessfull", "Run successfully?", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="dataAccess">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            bool run = false;
            if (!dataAccess.GetData<bool>(3, ref run))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                dataAccess.SetData(2, false);
                return;
            }
            if (!run)
                return;

            List<GH_ObjectWrapper> objectWrapperList = null;

            objectWrapperList = new List<GH_ObjectWrapper>();

            if (!dataAccess.GetDataList(0, objectWrapperList) || objectWrapperList == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                dataAccess.SetData(2, false);
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
                dataAccess.SetData(2, false);
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

            double tolerance = double.NaN;
            if (!dataAccess.GetData<double>(2, ref tolerance))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                dataAccess.SetData(2, false);
                return;
            }

            if(double.IsNaN(tolerance))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                dataAccess.SetData(2, false);
                return;
            }

            bool result = false;

            SAM.Analytical.Topologic.AdjacencyCluster adjacencyCluster = new Analytical.Topologic.AdjacencyCluster(spaceList, panelList);
            result = adjacencyCluster.Calculate(tolerance);

            dataAccess.SetData(0, adjacencyCluster);
            dataAccess.SetData(1, adjacencyCluster.Topology);
            dataAccess.SetData(2, result);
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
            get { return new Guid("2e04adb9-97ab-4634-83cf-67ce76249588"); }
        }
    }
}
// SPDX-License-Identifier: LGPL-3.0-or-later
// Copyright (c) 2020–2026 Michal Dengusiak & Jakub Ziolkowski and contributors

using Grasshopper.Kernel;
using SAM.Analytical.Grasshopper.Topologic.Properties;
using SAM.Core;
using SAM.Core.Grasshopper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SAM.Analytical.Grasshopper.Topologic
{
    public class TopologyAdjacencies : GH_SAMVariableOutputParameterComponent
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("a4a56d41-a2b2-4484-b8d9-787b5beedeb8");

        /// <summary>
        /// The latest version of this component
        /// </summary>
        public override string LatestComponentVersion => "1.0.2";

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.SAM_Topologic3a;

        /// <summary>
        /// Initializes a new instance of the SAM_point3D class.
        /// </summary>
        public TopologyAdjacencies()
          : base("Topology.Adjacencies", "Topology.Adjacencies",
              "Create AdjacenciesList/Connected Spaces List from  SAM AdjacencyCluster and SAM Analytical Panels based on Topologic calculation",
              "SAM", "Topologic")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override GH_SAMParam[] Inputs
        {
            get
            {
                List<GH_SAMParam> result = new List<GH_SAMParam>();
                result.Add(new GH_SAMParam(new GooAdjacencyClusterParam() { Name = "_adjacencyCluster", NickName = "_adjacencyCluster", Description = "SAM AdjacencyCluster", Access = GH_ParamAccess.item }, ParamVisibility.Binding));
                result.Add(new GH_SAMParam(new GooJSAMObjectParam<SAMObject>() { Name = "_SAMAnalytical", NickName = "_SAMAnalytical", Description = "SAM Analytical Object", Access = GH_ParamAccess.item }, ParamVisibility.Binding));
                return result.ToArray();
            }
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override GH_SAMParam[] Outputs
        {
            get
            {
                List<GH_SAMParam> result = new List<GH_SAMParam>();
                result.Add(new GH_SAMParam(new GooJSAMObjectParam<SAMObject>() { Name = "AdjacenciesList", NickName = "AdjacenciesList", Description = "AdjacenciesList", Access = GH_ParamAccess.list }, ParamVisibility.Binding));
                return result.ToArray();
            }
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="dataAccess">
        /// The DA object is used to retrieve from inputs and store in outputs.
        /// </param>
        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            AdjacencyCluster adjacencyCluster = null;

            int index = Params.IndexOfInputParam("_adjacencyCluster");
            if (index == -1 || !dataAccess.GetData(index, ref adjacencyCluster))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            SAMObject sAMObject = null;
            index = Params.IndexOfInputParam("_SAMAnalytical");
            if (index == -1 || !dataAccess.GetData(index, ref sAMObject))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            IEnumerable<SAMObject> result = null;
            if (sAMObject is Space)
                result = adjacencyCluster.GetPanels((Space)sAMObject);
            else if (sAMObject is Panel)
                result = adjacencyCluster.GetSpaces((Panel)sAMObject);

            index = Params.IndexOfOutputParam("AdjacenciesList");

            if (result == null)
            {
                if (index != -1)
                {
                    dataAccess.SetDataList(index, null);
                }
                return;
            }

            if (result.Count() == 0)
            {
                if (index != -1)
                {
                    dataAccess.SetDataList(index, result);
                }
                return;
            }

            if (index != -1)
            {
                dataAccess.SetDataList(index, result.ToList().ConvertAll(x => new GooJSAMObject<SAMObject>(x)));
            }
        }
    }
}

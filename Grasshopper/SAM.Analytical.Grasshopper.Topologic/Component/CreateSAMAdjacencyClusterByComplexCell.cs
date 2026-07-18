// SPDX-License-Identifier: LGPL-3.0-or-later
// Copyright (c) 2020–2026 Michal Dengusiak & Jakub Ziolkowski and contributors

using Grasshopper.Kernel;
using SAM.Analytical.Grasshopper.Topologic.Properties;
using SAM.Core.Grasshopper;
using System;
using System.Collections.Generic;
using Topologic;

namespace SAM.Analytical.Grasshopper.Topologic
{
    public class CreateSAMAdjacencyClusterByCellComplex : GH_SAMVariableOutputParameterComponent
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("870f9db6-cede-456a-9e25-5d39646a0620");

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
        public CreateSAMAdjacencyClusterByCellComplex()
          : base("Create.SAMAdjacencyClusterByCellComplex", "Create.SAMAdjacencyClusterByCellComplex",
              "Create SAM Adjacency Cluster By CellComplex",
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
                result.Add(new GH_SAMParam(new global::Grasshopper.Kernel.Parameters.Param_GenericObject() { Name = "_cellComplex", NickName = "_cellComplex", Description = "Topology CellComplex", Access = GH_ParamAccess.item }, ParamVisibility.Binding));

                global::Grasshopper.Kernel.Parameters.Param_Boolean param_run = new global::Grasshopper.Kernel.Parameters.Param_Boolean() { Name = "_run", NickName = "_run", Description = "Run", Access = GH_ParamAccess.item };
                param_run.SetPersistentData(false);
                result.Add(new GH_SAMParam(param_run, ParamVisibility.Binding));

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
                result.Add(new GH_SAMParam(new GooAdjacencyClusterParam() { Name = "adjacencyCluster", NickName = "adjacencyCluster", Description = "SAM AdjacencyCluster", Access = GH_ParamAccess.item }, ParamVisibility.Binding));
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
            int outputIndex = Params.IndexOfOutputParam("adjacencyCluster");

            bool run = false;
            int index = Params.IndexOfInputParam("_run");
            if (index == -1 || !dataAccess.GetData(index, ref run))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                if (outputIndex != -1)
                {
                    dataAccess.SetData(outputIndex, null);
                }
                return;
            }
            if (!run)
                return;

            CellComplex cellComplex = null;
            index = Params.IndexOfInputParam("_cellComplex");
            if (index == -1 || !dataAccess.GetData(index, ref cellComplex))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            AdjacencyCluster adjacencyCluster = Analytical.Topologic.Create.AdjacencyCluster(cellComplex);

            if (outputIndex != -1)
            {
                dataAccess.SetData(outputIndex, adjacencyCluster);
            }
        }
    }
}

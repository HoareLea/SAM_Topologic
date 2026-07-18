// SPDX-License-Identifier: LGPL-3.0-or-later
// Copyright (c) 2020–2026 Michal Dengusiak & Jakub Ziolkowski and contributors

using Grasshopper.Kernel;
using SAM.Analytical.Grasshopper.Topologic.Properties;
using SAM.Core;
using SAM.Core.Grasshopper;
using System;
using System.Collections.Generic;

namespace SAM.Analytical.Grasshopper.Topologic
{
    public class SAMAnalyticalTopology : GH_SAMVariableOutputParameterComponent
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("d3dc979f-71b4-444d-8b75-9c29d1f7e769");

        /// <summary>
        /// The latest version of this component
        /// </summary>
        public override string LatestComponentVersion => "1.0.3";

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.SAM_Topologic3a;

        /// <summary>
        /// Initializes a new instance of the SAM_point3D class.
        /// </summary>
        public SAMAnalyticalTopology()
          : base("SAMAnalytical.Topology", "SAMAnalytical.Topology",
              "Convert SAM Analytical To Topologic Geometry ie. SAM Panel to Topology Face",
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
                result.Add(new GH_SAMParam(new global::Grasshopper.Kernel.Parameters.Param_GenericObject() { Name = "Topology", NickName = "Topology", Description = "Topologic Geometry", Access = GH_ParamAccess.list }, ParamVisibility.Binding));
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
            SAMObject sAMObject = null;
            int index = Params.IndexOfInputParam("_SAMAnalytical");
            if (index == -1 || !dataAccess.GetData(index, ref sAMObject))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            int outputIndex = Params.IndexOfOutputParam("Topology");

            if (sAMObject is Panel)
            {
                if (outputIndex != -1)
                {
                    dataAccess.SetDataList(outputIndex, new List<global::Topologic.Face> { Analytical.Topologic.Convert.ToTopologic((Panel)sAMObject) });
                }
                return;
            }
            else if(sAMObject is AdjacencyCluster)
            {
                if (outputIndex != -1)
                {
                    dataAccess.SetDataList(outputIndex, null);
                }

                AdjacencyCluster adjacencyCluster = sAMObject as AdjacencyCluster;
                List<Geometry.Spatial.Shell> shells = adjacencyCluster.GetShells();
                if(shells != null && outputIndex != -1)
                {
                    dataAccess.SetDataList(outputIndex, shells.ConvertAll(x => Geometry.Topologic.Convert.ToTopologic_Cell(x)));
                }

                return;
            }
            else if (sAMObject is AnalyticalModel)
            {
                if (outputIndex != -1)
                {
                    dataAccess.SetDataList(outputIndex, null);
                }

                AdjacencyCluster adjacencyCluster = (sAMObject as AnalyticalModel).AdjacencyCluster;
                List<Geometry.Spatial.Shell> shells = adjacencyCluster.GetShells();
                if (shells != null && outputIndex != -1)
                {
                    dataAccess.SetDataList(outputIndex, shells.ConvertAll(x => Geometry.Topologic.Convert.ToTopologic_Cell(x)));
                }

                return;
            }

            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
        }
    }
}

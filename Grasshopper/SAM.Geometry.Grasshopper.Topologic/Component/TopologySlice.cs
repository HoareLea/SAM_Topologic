// SPDX-License-Identifier: LGPL-3.0-or-later
// Copyright (c) 2020–2026 Michal Dengusiak & Jakub Ziolkowski and contributors

using Grasshopper.Kernel;
using SAM.Core.Grasshopper;
using System;
using System.Collections.Generic;
using Topologic;

namespace SAM.Geometry.Grasshopper.Topologic
{
    public class TopologySlice : GH_SAMVariableOutputParameterComponent
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("d6480664-06b2-4e0c-839c-d21f8da9fa3c");

        /// <summary>
        /// The latest version of this component
        /// </summary>
        public override string LatestComponentVersion => "1.0.1";

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.SAM_Topologic3a;

        public TopologySlice()
          : base("Topology.Slice", "Topology.Slice", "Slices the input Topology with another Topology", "SAM", "Topologic")
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
                result.Add(new GH_SAMParam(new global::Grasshopper.Kernel.Parameters.Param_GenericObject() { Name = "_topology", NickName = "_topology", Description = "Topology will be sliced", Access = GH_ParamAccess.item }, ParamVisibility.Binding));
                result.Add(new GH_SAMParam(new global::Grasshopper.Kernel.Parameters.Param_GenericObject() { Name = "_sliceTopology", NickName = "_sliceTopology", Description = "Slice Topology", Access = GH_ParamAccess.item, Optional = false }, ParamVisibility.Binding));

                global::Grasshopper.Kernel.Parameters.Param_Boolean param_Boolean = new global::Grasshopper.Kernel.Parameters.Param_Boolean() { Name = "_transferDictionary_", NickName = "_transferDictionary_", Description = "Transfer Dictionary", Access = GH_ParamAccess.item };
                param_Boolean.SetPersistentData(false);
                result.Add(new GH_SAMParam(param_Boolean, ParamVisibility.Binding));

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
                result.Add(new GH_SAMParam(new global::Grasshopper.Kernel.Parameters.Param_GenericObject() { Name = "Topology", NickName = "Topology", Description = "Topology", Access = GH_ParamAccess.item }, ParamVisibility.Binding));
                return result.ToArray();
            }
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Topology topology = null;
            Topology topology_Slice = null;
            bool transferDictionary = false;

            int index = Params.IndexOfInputParam("_topology");
            if (index == -1 || !DA.GetData(index, ref topology))
            {
                return;
            }

            index = Params.IndexOfInputParam("_sliceTopology");
            if (index == -1 || !DA.GetData(index, ref topology_Slice))
            {
                return;
            }

            index = Params.IndexOfInputParam("_transferDictionary_");
            if (index == -1 || !DA.GetData(index, ref transferDictionary))
            {
                return;
            }

            if (topology == null)
            {
                return;
            }

            Topology topology_New = topology.Slice(topology_Slice, transferDictionary);

            index = Params.IndexOfOutputParam("Topology");
            if (index != -1)
            {
                DA.SetData(index, topology_New);
            }
        }
    }
}

// SPDX-License-Identifier: LGPL-3.0-or-later
// Copyright (c) 2020–2026 Michal Dengusiak & Jakub Ziolkowski and contributors

using Grasshopper.Kernel;
using SAM.Core.Grasshopper;
using System;
using System.Collections.Generic;
using Topologic;

namespace SAM.Geometry.Grasshopper.Topologic
{
    public class TopologySAMGeometry : GH_SAMVariableOutputParameterComponent
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("0fca6d19-95ee-4461-9be1-ab784aa800db");

        /// <summary>
        /// The latest version of this component
        /// </summary>
        public override string LatestComponentVersion => "1.0.1";

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.SAM_Topologic3a;

        /// <summary>
        /// Initializes a new instance of the SAMGeometryByGHGeometry class.
        /// </summary>
        public TopologySAMGeometry()
          : base("Topology.SAMGeometry", "Topology.SAMGeometry",
              "Convert Topologic Geometry to SAM Geometry",
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
                result.Add(new GH_SAMParam(new global::Grasshopper.Kernel.Parameters.Param_GenericObject() { Name = "_topology", NickName = "_topology", Description = "Topologic Geometry", Access = GH_ParamAccess.item }, ParamVisibility.Binding));
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
                result.Add(new GH_SAMParam(new GooSAMGeometryParam() { Name = "SAMGeometry", NickName = "SAMGeometry", Description = "SAM Geometry", Access = GH_ParamAccess.item }, ParamVisibility.Binding));
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
            Topology topology = null;
            int index = Params.IndexOfInputParam("_topology");
            if (index == -1 || !dataAccess.GetData(index, ref topology) || topology == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            Spatial.ISAMGeometry3D sAMGeometry3D = Convert.ToSAM(topology);
            if (sAMGeometry3D == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Cannot convert geometry");
                return;
            }

            index = Params.IndexOfOutputParam("SAMGeometry");
            if (index != -1)
            {
                dataAccess.SetData(index, new GooSAMGeometry(sAMGeometry3D));
            }
        }
    }
}

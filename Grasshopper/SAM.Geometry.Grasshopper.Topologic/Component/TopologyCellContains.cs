// SPDX-License-Identifier: LGPL-3.0-or-later
// Copyright (c) 2020–2026 Michal Dengusiak & Jakub Ziolkowski and contributors

using Grasshopper.Kernel;
using SAM.Core.Grasshopper;
using System;
using System.Collections.Generic;
using Topologic;

namespace SAM.Geometry.Grasshopper.Topologic
{
    public class TopologyCellContains : GH_SAMVariableOutputParameterComponent
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("1c6c3645-52b9-4d10-acf4-5b79d4351d88");

        /// <summary>
        /// The latest version of this component
        /// </summary>
        public override string LatestComponentVersion => "1.0.1";

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.SAM_Topologic3a;

        public TopologyCellContains()
          : base("Topology.CellContains", "Topology.CellContains", "Check if a Vertex is contained in a Cell or not", "SAM", "Topologic")
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
                result.Add(new GH_SAMParam(new global::Grasshopper.Kernel.Parameters.Param_GenericObject() { Name = "_cell", NickName = "_cell", Description = "Cell", Access = GH_ParamAccess.item, DataMapping = GH_DataMapping.Flatten }, ParamVisibility.Binding));
                result.Add(new GH_SAMParam(new global::Grasshopper.Kernel.Parameters.Param_GenericObject() { Name = "_vertex", NickName = "_vertex", Description = "Vertex", Access = GH_ParamAccess.item, DataMapping = GH_DataMapping.Graft }, ParamVisibility.Binding));
                result.Add(new GH_SAMParam(new global::Grasshopper.Kernel.Parameters.Param_Boolean() { Name = "_allowOnBoundary", NickName = "_allowOnBoundary", Description = "Allow On Boundary", Access = GH_ParamAccess.item }, ParamVisibility.Binding));

                global::Grasshopper.Kernel.Parameters.Param_Number param_Number = new global::Grasshopper.Kernel.Parameters.Param_Number() { Name = "_tolerance_", NickName = "_tolerance_", Description = "Tolerance", Access = GH_ParamAccess.item };
                param_Number.SetPersistentData(SAM.Core.Tolerance.MacroDistance);
                result.Add(new GH_SAMParam(param_Number, ParamVisibility.Binding));

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
                result.Add(new GH_SAMParam(new global::Grasshopper.Kernel.Parameters.Param_Boolean() { Name = "Contains", NickName = "Contains", Description = "Contains", Access = GH_ParamAccess.item }, ParamVisibility.Binding));
                return result.ToArray();
            }
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Declare a variable for the input String
            Cell cell = null;
            Vertex vertex = null;
            bool allowOnBoundary = false;
            double tolerance = Core.Tolerance.MacroDistance;

            int index = Params.IndexOfInputParam("_cell");
            if (index == -1 || !DA.GetData(index, ref cell))
            {
                return;
            }

            index = Params.IndexOfInputParam("_vertex");
            if (index == -1 || !DA.GetData(index, ref vertex))
            {
                return;
            }

            index = Params.IndexOfInputParam("_allowOnBoundary");
            if (index == -1 || !DA.GetData(index, ref allowOnBoundary))
            {
                return;
            }

            index = Params.IndexOfInputParam("_tolerance_");
            if (index == -1 || !DA.GetData(index, ref tolerance))
            {
                return;
            }

            if (cell == null)
            {
                return;
            }

            if (vertex == null)
            {
                return;
            }

            bool isContained = global::Topologic.Utilities.CellUtility.Contains(cell, vertex, allowOnBoundary, tolerance);

            index = Params.IndexOfOutputParam("Contains");
            if (index != -1)
            {
                DA.SetData(index, isContained);
            }
        }
    }
}

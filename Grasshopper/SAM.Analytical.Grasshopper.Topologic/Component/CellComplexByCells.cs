// SPDX-License-Identifier: LGPL-3.0-or-later
// Copyright (c) 2020–2026 Michal Dengusiak & Jakub Ziolkowski and contributors

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using SAM.Analytical.Grasshopper.Topologic.Properties;
using SAM.Core.Grasshopper;
using System;
using System.Collections.Generic;
using Topologic;

namespace SAM.Analytical.Grasshopper.Topologic
{
    public class CellComplexByCells: GH_SAMVariableOutputParameterComponent
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("b59073da-e1e1-4d7e-bc60-fdfe5bf2b924");

        /// <summary>
        /// The latest version of this component
        /// </summary>
        public override string LatestComponentVersion => "1.0.1";

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.SAM_Topologic3a;

        /// <summary>
        /// Initializes a new instance of the SAM_point3D class.
        /// </summary>
        public CellComplexByCells()
          : base("CellComplex.ByCells", "CellComplex.ByCells",
              "Convert Topologic Cells to Topologic CellComplex",
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
                result.Add(new GH_SAMParam(new global::Grasshopper.Kernel.Parameters.Param_GenericObject() { Name = "_cells", NickName = "_cells", Description = "Topology Cells", Access = GH_ParamAccess.list }, ParamVisibility.Binding));
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
                result.Add(new GH_SAMParam(new global::Grasshopper.Kernel.Parameters.Param_GenericObject() { Name = "CellComplex", NickName = "CellComplex", Description = "Topology CellComplex", Access = GH_ParamAccess.item }, ParamVisibility.Binding));
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
            List<GH_ObjectWrapper> gH_ObjectWrappers = new List<GH_ObjectWrapper>();

            int index = Params.IndexOfInputParam("_cells");
            if (index == -1 || !dataAccess.GetDataList(index, gH_ObjectWrappers) || gH_ObjectWrappers.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            List<Cell> cells = new List<Cell>();
            foreach(GH_ObjectWrapper gH_ObjectWrapper in gH_ObjectWrappers)
            {
                Cell cell = gH_ObjectWrapper.Value as Cell;
                if(cell != null)
                {
                    cells.Add(cell);
                }
            }


            if (cells == null || cells.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            CellComplex cellComplex = CellComplex.ByCells(cells);

            index = Params.IndexOfOutputParam("CellComplex");
            if (index != -1)
            {
                dataAccess.SetData(index, cellComplex);
            }
        }
    }
}

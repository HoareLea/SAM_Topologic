// SPDX-License-Identifier: LGPL-3.0-or-later
// Copyright (c) 2020–2026 Michal Dengusiak & Jakub Ziolkowski and contributors

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using SAM.Core.Grasshopper;
using SAM.Geometry.Grasshopper.Topologic.Properties;
using System;
using System.Collections.Generic;

namespace SAM.Geometry.Grasshopper.Topologic
{
    public class GeometryTopology : GH_SAMVariableOutputParameterComponent
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("1c98cd19-b430-48e5-a626-4d103df4fe1c");

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
        public GeometryTopology()
          : base("Geometry.Topology", "Geometry.Topology",
              "Convert Rhino Geometry To Topologic Geometry",
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
                result.Add(new GH_SAMParam(new global::Grasshopper.Kernel.Parameters.Param_Geometry() { Name = "_geometry", NickName = "_geometry", Description = "Rhino Geometry", Access = GH_ParamAccess.item }, ParamVisibility.Binding));

                global::Grasshopper.Kernel.Parameters.Param_Number param_Number = new global::Grasshopper.Kernel.Parameters.Param_Number() { Name = "_tolerance_", NickName = "_tolerance_", Description = "Tolerance", Access = GH_ParamAccess.item };
                param_Number.SetPersistentData(Core.Tolerance.MacroDistance);
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
                result.Add(new GH_SAMParam(new global::Grasshopper.Kernel.Parameters.Param_GenericObject() { Name = "Topology", NickName = "Topology", Description = "Topology Geometry", Access = GH_ParamAccess.item }, ParamVisibility.Binding));
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
            object @object = null;
            double tolerance = 0.0001;

            int index = Params.IndexOfInputParam("_geometry");
            if (index == -1 || !dataAccess.GetData(index, ref @object))
            {
                return;
            }

            index = Params.IndexOfInputParam("_tolerance_");
            if (index == -1 || !dataAccess.GetData(index, ref tolerance))
            {
                return;
            }

            if (@object == null)
            {
                return;
            }

            Type type = @object.GetType();

            int outputIndex = Params.IndexOfOutputParam("Topology");

            global::Topologic.Topology topology = null;
            GH_Point ghPoint = @object as GH_Point;
            if (ghPoint != null)
            {
                topology = ghPoint.Value.ToTopologic();
                if (outputIndex != -1) dataAccess.SetData(outputIndex, topology);
                return;
            }

            GH_Line ghLine = @object as GH_Line;
            if (ghLine != null)
            {
                topology = ghLine.Value.ToTopologic();
                if (outputIndex != -1) dataAccess.SetData(outputIndex, topology);
                return;
            }

            GH_Curve ghCurve = @object as GH_Curve;
            if (ghCurve != null)
            {
                topology = ghCurve.Value.ToTopologic();
                if (outputIndex != -1) dataAccess.SetData(outputIndex, topology);
                return;
            }

            GH_Surface ghSurface = @object as GH_Surface;
            if (ghSurface != null)
            {
                topology = ghSurface.Value.ToTopologic(tolerance);
                if (outputIndex != -1) dataAccess.SetData(outputIndex, topology);
                return;
            }

            GH_Brep ghBrep = @object as GH_Brep;
            if (ghBrep != null)
            {
                topology = ghBrep.Value.ToTopologic(tolerance);
                if (outputIndex != -1) dataAccess.SetData(outputIndex, topology);
                return;
            }

            GH_Box ghBox = @object as GH_Box;
            if (ghBox != null)
            {
                topology = ghBox.Value.ToTopologic();
                if (outputIndex != -1) dataAccess.SetData(outputIndex, topology);
                return;
            }

            GH_Mesh ghMesh = @object as GH_Mesh;
            if (ghMesh != null)
            {
                topology = ghMesh.Value.ToTopologic();
                if (outputIndex != -1) dataAccess.SetData(outputIndex, topology);
                return;
            }

            throw new Exception("Cannot convert geometry.");
        }
    }
}

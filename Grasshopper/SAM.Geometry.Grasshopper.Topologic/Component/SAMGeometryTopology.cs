// SPDX-License-Identifier: LGPL-3.0-or-later
// Copyright (c) 2020–2026 Michal Dengusiak & Jakub Ziolkowski and contributors

using Grasshopper.Kernel;
using Rhino.Geometry;
using SAM.Core.Grasshopper;
using SAM.Geometry.Grasshopper.Topologic.Properties;
using SAM.Geometry.Spatial;
using System;
using System.Collections.Generic;

namespace SAM.Geometry.Grasshopper.Topologic
{
    public class SAMGeometryTopology : GH_SAMVariableOutputParameterComponent
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("8904de02-93b6-4d21-8d04-2ee1acb1e53c");

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
        public SAMGeometryTopology()
          : base("SAMGeometry.Topology", "SAMGeometry.Topology",
              "Convert SAM Geometry To Topologic Geometry",
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
                result.Add(new GH_SAMParam(new GooSAMGeometryParam() { Name = "_SAMGeometry", NickName = "_SAMGeometry", Description = "SAM Geometry: Polygon3D, Segment3D, Point3D", Access = GH_ParamAccess.item }, ParamVisibility.Binding));
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
                result.Add(new GH_SAMParam(new global::Grasshopper.Kernel.Parameters.Param_GenericObject() { Name = "Topology", NickName = "Topology", Description = "Topology Geometry: Wire, Edge, Vertex", Access = GH_ParamAccess.item }, ParamVisibility.Binding));
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
            ISAMGeometry sAMGeometry = null;
            int index = Params.IndexOfInputParam("_SAMGeometry");
            if (index == -1 || !dataAccess.GetData(index, ref sAMGeometry) || sAMGeometry == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            int outputIndex = Params.IndexOfOutputParam("Topology");

            Point3D point3D = sAMGeometry as Point3D;
            if (point3D != null)
            {
                if (outputIndex != -1) dataAccess.SetData(outputIndex, Geometry.Topologic.Convert.ToTopologic(point3D));
                return;
            }

            ICurve3D curve3D = sAMGeometry as ICurve3D;
            if (curve3D != null)
            {
                if (outputIndex != -1) dataAccess.SetData(outputIndex, Geometry.Topologic.Convert.ToTopologic(curve3D));
                return;
            }

            Polygon3D polygon3D = sAMGeometry as Polygon3D;
            if (polygon3D != null)
            {
                if (outputIndex != -1) dataAccess.SetData(outputIndex, Geometry.Topologic.Convert.ToTopologic(polygon3D));
                return;
            }

            Face3D face3D = sAMGeometry as Face3D;
            if (face3D != null)
            {
                if (outputIndex != -1) dataAccess.SetData(outputIndex, Geometry.Topologic.Convert.ToTopologic(face3D));
                return;
            }

            Shell shell = sAMGeometry as Shell;
            if (shell != null)
            {
                Brep brep = Rhino.Convert.ToRhino(shell);
                if(brep != null)
                {
                    if (outputIndex != -1) dataAccess.SetData(outputIndex, brep.ToTopologic(Core.Tolerance.MacroDistance));
                    return;
                }
            }

            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Cannot convert geometry");
        }
    }
}

using Grasshopper.Kernel;
using SAM.Geometry.Grasshopper.Topologic.Properties;
using SAM.Core.Grasshopper;
using System;
using Topologic;
using System.Collections.Generic;

namespace SAM.Geometry.Grasshopper.Topologic
{
    public class TopologyInspect : GH_SAMVariableOutputParameterComponent
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("7aff072b-fe18-4ea3-8f2a-b64bc1cac958");

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
        public TopologyInspect()
          : base("Topology.Inspect", "Topologic.Inspect",
              "Inspect Topology and try to get CellComplex, Cells, Faces, Wired and Vertices",
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

                global::Grasshopper.Kernel.Parameters.Param_GenericObject gerenricObject;

                gerenricObject = new global::Grasshopper.Kernel.Parameters.Param_GenericObject() { Name = "_topology", NickName = "_topology", Description = "Topology Inspect into CellComplex, Cells, Faces, Wired and Vertices", Access = GH_ParamAccess.item };
                result.Add(new GH_SAMParam(gerenricObject, ParamVisibility.Binding));

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
                result.Add(new GH_SAMParam(new global::Grasshopper.Kernel.Parameters.Param_GenericObject() { Name = "CellComplexes", NickName = "CellComplexes", Description = "Topology CellComplexes", Access = GH_ParamAccess.list }, ParamVisibility.Binding));
                result.Add(new GH_SAMParam(new global::Grasshopper.Kernel.Parameters.Param_GenericObject() { Name = "Cells", NickName = "Cells", Description = "Topology Cells", Access = GH_ParamAccess.list }, ParamVisibility.Binding));
                result.Add(new GH_SAMParam(new global::Grasshopper.Kernel.Parameters.Param_GenericObject() { Name = "Faces", NickName = "Faces", Description = "Topology Faces", Access = GH_ParamAccess.list }, ParamVisibility.Voluntary));
                result.Add(new GH_SAMParam(new global::Grasshopper.Kernel.Parameters.Param_GenericObject() { Name = "Wires", NickName = "Wires", Description = "Topology Wires", Access = GH_ParamAccess.list }, ParamVisibility.Voluntary));
                result.Add(new GH_SAMParam(new global::Grasshopper.Kernel.Parameters.Param_GenericObject() { Name = "Vertices", NickName = "Vertices", Description = "Topology Vertices", Access = GH_ParamAccess.list }, ParamVisibility.Voluntary));
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
            int index = -1;
            
            Topology topology = null;
            index = Params.IndexOfInputParam("_topology");
            if (index == -1 || !dataAccess.GetData(index, ref topology))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            index = Params.IndexOfOutputParam("CellComplexes");
            if (index != -1)
                dataAccess.SetDataList(index, topology?.CellComplexes);

            index = Params.IndexOfOutputParam("Cells");
            if (index != -1)
                dataAccess.SetDataList(index, topology?.Cells);

            index = Params.IndexOfOutputParam("Faces");
            if (index != -1)
                dataAccess.SetDataList(index, topology?.Faces);

            index = Params.IndexOfOutputParam("Wires");
            if (index != -1)
                dataAccess.SetDataList(index, topology?.Wires);

            index = Params.IndexOfOutputParam("Vertices");
            if (index != -1)
                dataAccess.SetDataList(index, topology?.Vertices);
        }
    }
}
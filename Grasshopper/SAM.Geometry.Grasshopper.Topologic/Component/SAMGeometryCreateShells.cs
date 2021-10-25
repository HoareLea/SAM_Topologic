using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using SAM.Core.Grasshopper;
using SAM.Geometry.Grasshopper.Topologic.Properties;
using SAM.Geometry.Spatial;
using System;
using System.Collections.Generic;

namespace SAM.Geometry.Grasshopper.Topologic
{
    public class SAMGeometryCreateShells : GH_SAMVariableOutputParameterComponent
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("8c9e2d12-4ba3-4f7b-b3e4-c427df372e90");

        /// <summary>
        /// The latest version of this component
        /// </summary>
        public override string LatestComponentVersion => "1.0.0";

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.SAM_Topologic;

        public override GH_Exposure Exposure => GH_Exposure.primary;

        /// <summary>
        /// Initializes a new instance of the SAM_point3D class.
        /// </summary>
        public SAMGeometryCreateShells()
          : base("SAMGeometry.CreateShells", "SAMGeometry.CreateShells",
              "SAM Create Shells",
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

                global::Grasshopper.Kernel.Parameters.Param_GenericObject genericObject;

                genericObject = new global::Grasshopper.Kernel.Parameters.Param_GenericObject() { Name = "face3Ds_", NickName = "face3Ds_", Description = "SAM Geometry Face3Ds", Access = GH_ParamAccess.list };
                genericObject.DataMapping = GH_DataMapping.Flatten;
                result.Add(new GH_SAMParam(genericObject, ParamVisibility.Binding));

                global::Grasshopper.Kernel.Parameters.Param_Number paramNumber;
                paramNumber = new global::Grasshopper.Kernel.Parameters.Param_Number() { Name = "tolerance_", NickName = "tolerance_", Description = "Tolerance", Access = GH_ParamAccess.item };
                paramNumber.SetPersistentData(Core.Tolerance.Distance);
                result.Add(new GH_SAMParam(paramNumber, ParamVisibility.Voluntary));

                global::Grasshopper.Kernel.Parameters.Param_Boolean paramBoolean;

                paramBoolean = new global::Grasshopper.Kernel.Parameters.Param_Boolean() { Name = "run_", NickName = "run_", Description = "Run", Access = GH_ParamAccess.item };
                paramBoolean.SetPersistentData(false);
                result.Add(new GH_SAMParam(paramBoolean, ParamVisibility.Binding));

                paramBoolean = new global::Grasshopper.Kernel.Parameters.Param_Boolean() { Name = "tryCellComplexByCells_", NickName = "tryCellComplexByCells_", Description = "Try CellComplex By Cells", Access = GH_ParamAccess.item };
                paramBoolean.SetPersistentData(true);
                result.Add(new GH_SAMParam(paramBoolean, ParamVisibility.Voluntary));

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
                result.Add(new GH_SAMParam(new global::Grasshopper.Kernel.Parameters.Param_GenericObject() { Name = "shells", NickName = "shells", Description = "SAM Geometry Shells", Access = GH_ParamAccess.list }, ParamVisibility.Binding));
                result.Add(new GH_SAMParam(new global::Grasshopper.Kernel.Parameters.Param_GenericObject() { Name = "topologies", NickName = "topologies", Description = "Topologies", Access = GH_ParamAccess.list }, ParamVisibility.Voluntary));
                result.Add(new GH_SAMParam(new global::Grasshopper.Kernel.Parameters.Param_Boolean() { Name = "sucessfull", NickName = "sucessfull", Description = "Sucessfull", Access = GH_ParamAccess.item }, ParamVisibility.Binding));
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
            int index_Sucessfull = Params.IndexOfOutputParam("sucessfull");
            if (index_Sucessfull != -1)
                dataAccess.SetData(index_Sucessfull, false);

            int index;
            index = Params.IndexOfInputParam("run_");
            bool run = false;
            if (index != -1)
                dataAccess.GetData(index, ref run);

            if (!run)
                return;

            index = Params.IndexOfInputParam("face3Ds_");

            List<GH_ObjectWrapper> objectWrappers = new List<GH_ObjectWrapper>();
            if (index == -1 || !dataAccess.GetDataList(index, objectWrappers) || objectWrappers == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            List<Face3D> face3Ds = new List<Face3D>();
            foreach (GH_ObjectWrapper objectWrapper in objectWrappers)
            {
                if (Query.TryGetSAMGeometries(objectWrapper, out List<Face3D> face3Ds_Temp) && face3Ds_Temp != null)
                    face3Ds.AddRange(face3Ds_Temp);
            }

            //index = Params.IndexOfInputParam("tryCellComplexByCells_");
            //bool tryCellComplexByCells = true;
            //if (index != -1)
            //    dataAccess.GetData(index, ref tryCellComplexByCells);

            index = Params.IndexOfInputParam("tolerance_");
            double tolerance = Core.Tolerance.Distance;
            if (index != -1)
                dataAccess.GetData(index, ref tolerance);


            bool tryCellComplexByCells = true;
            index = Params.IndexOfInputParam("tryCellComplexByCells_");
            if (index != -1)
            {
                dataAccess.GetData(index, ref tryCellComplexByCells);
            }

            List<Shell> shells = Geometry.Topologic.Create.Shells(face3Ds, out List<global::Topologic.Topology> topologies, tryCellComplexByCells, tolerance);

            index = Params.IndexOfOutputParam("shells");
            if (index != -1)
                dataAccess.SetDataList(index, shells);

            index = Params.IndexOfOutputParam("topologies");
            if (index != -1)
                dataAccess.SetDataList(index, topologies);

            if (index_Sucessfull != -1)
                dataAccess.SetData(index_Sucessfull, shells != null);
        }
    }
}
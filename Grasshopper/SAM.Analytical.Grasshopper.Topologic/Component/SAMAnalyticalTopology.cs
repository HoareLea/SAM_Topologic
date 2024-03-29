﻿using Grasshopper.Kernel;
using SAM.Analytical.Grasshopper.Topologic.Properties;
using SAM.Core;
using SAM.Core.Grasshopper;
using System;
using System.Collections.Generic;

namespace SAM.Analytical.Grasshopper.Topologic
{
    public class SAMAnalyticalTopology : GH_SAMComponent
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("d3dc979f-71b4-444d-8b75-9c29d1f7e769");

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
        public SAMAnalyticalTopology()
          : base("SAMAnalytical.Topology", "SAMAnalytical.Topology",
              "Convert SAM Analytical To Topologic Geometry ie. SAM Panel to Topology Face",
              "SAM", "Topologic")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager inputParamManager)
        {
            inputParamManager.AddParameter(new GooJSAMObjectParam<SAMObject>(), "_SAMAnalytical", "_SAMAnalytical", "SAM Analytical Object", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager outputParamManager)
        {
            outputParamManager.AddGenericParameter("Topology", "Topology", "Topologic Geometry", GH_ParamAccess.list);
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
            if (!dataAccess.GetData(0, ref sAMObject))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            if (sAMObject is Panel)
            {
                dataAccess.SetDataList(0, new List<global::Topologic.Face> { Analytical.Topologic.Convert.ToTopologic((Panel)sAMObject) });
                return;
            }
            else if(sAMObject is AdjacencyCluster)
            {
                dataAccess.SetDataList(0, null);

                AdjacencyCluster adjacencyCluster = sAMObject as AdjacencyCluster;
                List<Geometry.Spatial.Shell> shells = adjacencyCluster.GetShells();
                if(shells != null)
                {
                    dataAccess.SetDataList(0, shells.ConvertAll(x => Geometry.Topologic.Convert.ToTopologic_Cell(x)));
                }

                return;
            }
            else if (sAMObject is AnalyticalModel)
            {
                dataAccess.SetDataList(0, null);

                AdjacencyCluster adjacencyCluster = (sAMObject as AnalyticalModel).AdjacencyCluster;
                List<Geometry.Spatial.Shell> shells = adjacencyCluster.GetShells();
                if (shells != null)
                {
                    dataAccess.SetDataList(0, shells.ConvertAll(x => Geometry.Topologic.Convert.ToTopologic_Cell(x)));
                }

                return;
            }

            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
        }
    }
}
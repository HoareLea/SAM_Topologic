using SAM.Geometry.Spatial;
using System;
using System.Collections.Generic;
using System.Linq;
using Topologic;


namespace SAM.Geometry.Topologic
{
    public static partial class Query
    {
        public static bool TrySplit(this IEnumerable<Spatial.Shell> shells_In, out List<Spatial.Shell> shells_Out, out List<Topology> topologies, bool tryCellComplexByCells = true, double tolerance = Core.Tolerance.Distance)
        {
            shells_Out = null;
            topologies = null;

            if (shells_In == null)
                return false;

            List<global::Topologic.Face> faces = new List<global::Topologic.Face>();
            foreach(Spatial.Shell shell in shells_In)
            {
                List<Face3D> face3Ds = shell?.Face3Ds;
                if (face3Ds == null || face3Ds.Count == 0)
                    continue;
                
                foreach(Face3D face3D in face3Ds)
                {
                    global::Topologic.Face face = Convert.ToTopologic(face3D);
                }
            }

            topologies = new List<Topology>();
            List<Cell> cells = null;
            if (tryCellComplexByCells)
            {
                try
                {
                    Cluster cluster = Cluster.ByTopologies(faces as IList<Topology>);
                    Topology topology = cluster.SelfMerge();
                    if (topology.Cells != null && topology.Cells.Count != 0)
                    {
                        cells = topology.Cells?.ToList();
                        CellComplex cellComplex = null;
                        try
                        {
                            cellComplex = CellComplex.ByCells(cells);
                        }
                        catch (Exception exception)
                        {

                        }

                        if (cellComplex != null && cellComplex.Cells != null && cellComplex.Cells.Count != 0)
                        {
                            topologies.Add(cellComplex);

                            cells = cellComplex.Cells?.ToList();
                        }
                        else
                        {
                            topologies.Add(topology);
                        }
                    }

                }
                catch (Exception exception)
                {
                    cells = null;
                }
            }

            if (cells == null)
            {
                try
                {
                    CellComplex cellComplex = CellComplex.ByFaces(faces, tolerance);
                    topologies.Add(cellComplex);
                    cells = cellComplex.Cells?.ToList();
                }
                catch (Exception exception)
                {
                    cells = null;
                }
            }

            if (cells == null || cells.Count == 0)
                return false;

            shells_Out = cells.ToSAM();
            return shells_Out != null;
        }
    }
}
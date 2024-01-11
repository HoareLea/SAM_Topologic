using SAM.Core;
using SAM.Geometry.Object.Spatial;
using SAM.Geometry.Spatial;
using System;
using System.Collections.Generic;
using System.Linq;
using Topologic;

namespace SAM.Geometry.Topologic
{
    public static partial class Create
    {
        public static List<Spatial.Shell> Shells(IEnumerable<Face3D> face3Ds, out List<Topology> topologies, bool tryCellComplexByCells = true, double tolerance = Tolerance.Distance)
        {
            topologies = null;
            
            if (face3Ds == null || face3Ds.Count() == 0)
            {
                return null;
            }

            List<global::Topologic.Face> faces = new List<global::Topologic.Face>();
            foreach(Face3D face3D in face3Ds)
            {
                global::Topologic.Face face = Convert.ToTopologic(face3D);
                if (face == null)
                {
                    continue;
                }

                faces.Add(face);
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
                            cellComplex = null;
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

            return cells?.ToSAM();
        }

        public static List<Spatial.Shell> Shells<T>(IEnumerable<T> face3DObjects, out List<Topology> topologies, bool tryCellComplexByCells = true, double tolerance = Tolerance.Distance) where T :IFace3DObject
        {
            topologies = null;

            if (face3DObjects == null)
            {
                return null;
            }

            return Shells(face3DObjects.ToList().ConvertAll(x => x?.Face3D), out topologies, tryCellComplexByCells, tolerance);
        }
    }
}
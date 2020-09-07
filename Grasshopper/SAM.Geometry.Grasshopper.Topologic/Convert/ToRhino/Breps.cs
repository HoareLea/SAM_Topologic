using Rhino.Geometry;
using System.Collections.Generic;
using System.Linq;
using Topologic;

namespace SAM.Geometry.Grasshopper.Topologic
{
    public static partial class Convert
    {
        public static List<Brep> ToRhino_Breps(Topology topology, double tolerance = Core.Tolerance.Distance)
        {
            IList<global::Topologic.Face> faces = topology.Faces;
            List<Brep> ghBrepSurfaces = new List<Brep>();
            foreach (global::Topologic.Face face in faces)
            {
                Brep ghBrepSurface = ToRhino(face, tolerance);
                ghBrepSurfaces.Add(ghBrepSurface);
            }

            if (ghBrepSurfaces.Count == 0)
            {
                return null;
            }

            Brep[] ghJoinedBreps = Brep.JoinBreps(ghBrepSurfaces, 0.1);
            if (ghJoinedBreps == null)
                return null;

            return ghJoinedBreps.ToList();
        }

        public static List<Brep> ToRhino_Breps(CellComplex cellComplex, double tolerance)
        {
            IList<Cell> cells = cellComplex.Cells;
            List<Brep> ghBreps = new List<Brep>();
            foreach (Cell cell in cells)
            {
                List<Brep> ghBrep = ToRhino_Breps(cell, tolerance);

                ghBreps.AddRange(ghBrep);
            }
            return ghBreps;
        }
    }
}
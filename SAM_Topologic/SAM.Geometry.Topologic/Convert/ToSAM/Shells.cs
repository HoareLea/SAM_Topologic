using System.Collections.Generic;
using Topologic;

namespace SAM.Geometry.Topologic
{
    public static partial class Convert
    {
        public static List<Spatial.Shell> ToSAM(this CellComplex cellComplex)
        {
            return ToSAM(cellComplex?.Cells);
        }

        public static List<Spatial.Shell> ToSAM(this IEnumerable<Cell> cells)
        {
            if (cells == null)
                return null;

            List<Spatial.Shell> result = new List<Spatial.Shell>();
            foreach (Cell cell in cells)
            {
                Spatial.Shell shell = cell.ToSAM();
                if (shell == null)
                    continue;

                result.Add(shell);
            }
            return result;
        }
    }
}
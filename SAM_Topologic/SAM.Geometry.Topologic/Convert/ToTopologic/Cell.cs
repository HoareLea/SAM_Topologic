using SAM.Geometry.Spatial;
using System.Collections.Generic;
using Topologic;

namespace SAM.Geometry.Topologic
{
    public static partial class Convert
    {
        public static Cell ToTopologic_Cell(this Spatial.Shell shell, double tolerance = Core.Tolerance.Distance)
        {
            List<Face3D> face3Ds = shell?.Face3Ds;
            if (face3Ds == null || face3Ds.Count == 0)
                return null;

            Cell result = null;
            try
            {
                result = Cell.ByFaces(face3Ds.ConvertAll(x => x.ToTopologic()), tolerance);
            }
            catch
            {
                result = null;
            }

            if (result != null)
                return result;

            global::Topologic.Shell shell_Topologic = shell.ToTopologic(tolerance);
            if (shell_Topologic == null)
                return result;

            try
            {
                result = Cell.ByShell(shell_Topologic);
            }
            catch
            {
                result = null;
            }

            return result;
        }
    }
}
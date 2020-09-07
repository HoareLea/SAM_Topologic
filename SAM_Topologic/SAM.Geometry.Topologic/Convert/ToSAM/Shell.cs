using SAM.Geometry.Spatial;
using System.Collections.Generic;
using Topologic;

namespace SAM.Geometry.Topologic
{
    public static partial class Convert
    {
        //due to issue with plane calculation we rounding
        public static Spatial.Shell ToSAM(this Cell cell)
        {
            if (cell == null)
                return null;

            IList<global::Topologic.Face> faces = cell.Faces;
            if (faces == null || faces.Count == 0)
                return null;

            List<Face3D> face3Ds = new List<Face3D>();
            foreach(global::Topologic.Face face in faces)
            {
                Face3D face3D = face.ToSAM();
                if (face3D == null)
                    continue;

                face3Ds.Add(face3D);
            }

            return new Spatial.Shell(face3Ds);
        }
    }
}
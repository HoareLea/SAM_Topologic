using SAM.Core;
using SAM.Geometry.Spatial;
using SAM.Geometry.Topologic;
using System.Collections.Generic;
using Topologic;

namespace SAM.Analytical.Topologic
{
    public static partial class Create
    {
        public static List<Panel> Panels(this List<Face> faces, PanelType panelType, Construction construction = null, double minArea = Tolerance.MacroDistance)
        {
            if (faces == null)
                return null;

            List<ISAMGeometry3D> faces_SAM = new List<ISAMGeometry3D>();
            foreach(Face face in faces)
            {
                Face3D face3D = face.ToSAM();
                if (face3D == null)
                    continue;

                faces_SAM.Add(face3D);
            }

            return Analytical.Create.Panels(faces_SAM, panelType, construction, minArea);
        }
    }
}
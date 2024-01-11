using Rhino.Geometry;
using System;

namespace SAM.Geometry.Grasshopper.Topologic
{
    public static partial class Convert
    {
        public static Surface ToRhino(global::Topologic.Face face)
        {
            System.Object faceGeometry = face.BasicGeometry;

            // 1. Compute the base surface Based on https://developer.rhino3d.com/api/RhinoCommon/html/M_Rhino_Geometry_NurbsSurface_Create.htm
            global::Topologic.NurbsSurface nurbsSurface = faceGeometry as global::Topologic.NurbsSurface;
            if (nurbsSurface != null)
            {
                return ToRhino_NurbsSurface(nurbsSurface);
            }

            global::Topologic.PlanarSurface planarSurface = faceGeometry as global::Topologic.PlanarSurface;
            if (planarSurface != null)
            {
                PlaneSurface planeSurface = ToRhino(planarSurface, face);
                return planeSurface;
            }

            throw new Exception("An invalid surface is created.");
        }
    }
}
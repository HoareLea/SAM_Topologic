using System.Collections.Generic;

namespace SAM.Analytical.Topologic
{
    public static partial class Query
    {
        public static Panel SimilarPanel(this Geometry.Spatial.Face3D face3D, Dictionary<Panel, Geometry.Spatial.Face3D> panelsDictionary, double distanceTolerance = Core.Tolerance.MacroDistance, double coplanarTolerance = Core.Tolerance.MacroDistance)
        {
            if (face3D == null || panelsDictionary == null)
                return null;

            Geometry.Spatial.Plane plane = face3D.GetPlane();
            double area = face3D.GetArea();

            Geometry.Planar.IClosed2D closed2D_1 = Geometry.Spatial.Query.Convert(plane, face3D.GetExternalEdge3D());
            Geometry.Planar.Point2D point2D_Internal = closed2D_1.GetInternalPoint2D();

            double areaDifferece_Min = double.MaxValue;
            Panel result = null;
            foreach (KeyValuePair<Panel, Geometry.Spatial.Face3D> keyValuePair in panelsDictionary)
            {
                if (keyValuePair.Value == null)
                    continue;

                double areaDifference = System.Math.Abs(keyValuePair.Value.GetArea() - area);

                if (areaDifferece_Min < areaDifference)
                    continue;

                Geometry.Spatial.Face3D face3D_Temp = keyValuePair.Value;
                Geometry.Spatial.Plane plane_Temp = face3D_Temp.GetPlane();

                if (!plane.Coplanar(plane_Temp, coplanarTolerance))
                    continue;

                Geometry.Spatial.Point3D point3D_Origin = plane_Temp.Origin;
                Geometry.Spatial.Point3D point3D_Project = Geometry.Spatial.Query.Project(plane, point3D_Origin);
                if (point3D_Origin.Distance(point3D_Project) > distanceTolerance)
                    continue;

                Geometry.Planar.IClosed2D closed2D_2 = Geometry.Spatial.Query.Convert(plane, face3D_Temp.GetExternalEdge3D());
                if (closed2D_2.Inside(point2D_Internal))
                {
                    result = keyValuePair.Key;
                    areaDifferece_Min = areaDifference;
                }
            }

            return result;
        }
    }
}
using SAM.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SAM.Analytical.Topologic
{
    public static partial class Query
    {
        public static List<Panel> SimilarPanels(this Geometry.Spatial.Face3D face3D, Dictionary<Panel, Geometry.Spatial.Face3D> panelsDictionary, double distanceTolerance = Tolerance.MacroDistance, double coplanarTolerance = Tolerance.MacroDistance)
        {
            if (face3D == null || panelsDictionary == null)
                return null;

            Geometry.Spatial.Plane plane = face3D.GetPlane();

            Geometry.Planar.IClosed2D closed2D_1 = Geometry.Spatial.Query.Convert(plane, face3D.GetExternalEdge3D());
            Geometry.Planar.Point2D point2D_Internal = closed2D_1.GetInternalPoint2D();

            double area = face3D.GetArea();

            List<Tuple<Panel, double>> tuples = new List<Tuple<Panel, double>>();
            foreach (KeyValuePair<Panel, Geometry.Spatial.Face3D> keyValuePair in panelsDictionary)
            {
                if (keyValuePair.Value == null)
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
                    tuples.Add(new Tuple<Panel, double>(keyValuePair.Key, Math.Abs(keyValuePair.Value.GetArea() - area)));
            }

            if (tuples == null || tuples.Count == 0)
                return null;

            if (tuples.Count == 1)
                return new List<Panel>() { tuples.First().Item1 };

            tuples.Sort((x, y) => x.Item2.CompareTo(y.Item2));

            double areatolerance = distanceTolerance * 10; 
            List<Panel> result = tuples.FindAll(x => x.Item2 < areatolerance).ConvertAll(x => x.Item1);
            if (result != null && result.Count > 0)
                return result;

            return new List<Panel>() { tuples.First().Item1 };
        }
    }
}
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using Topologic;

namespace SAM.Geometry.Grasshopper.Topologic
{
    public static partial class Convert
    {
        public static Curve ToRhino(this global::Topologic.NurbsCurve nurbsCurve)
        {
            // Based on https://developer.rhino3d.com/api/RhinoCommon/html/P_Rhino_Geometry_NurbsCurve_Knots.htm
            bool isRational = nurbsCurve.IsRational;
            int degree = nurbsCurve.Degree;
            List<Vertex> controlVertices = nurbsCurve.ControlVertices;
            List<Point3d> ghControlPoints = new List<Point3d>();

            global::Rhino.Geometry.NurbsCurve ghNurbsCurve = new global::Rhino.Geometry.NurbsCurve(3, isRational, degree + 1, controlVertices.Count);

            int i = 0;
            foreach (Vertex controlVertex in controlVertices)
            {
                Point3d ghControlPoint = ToRhino(controlVertex);
                ghControlPoints.Add(ghControlPoint);
                ghNurbsCurve.Points.SetPoint(i, ghControlPoint);
                ++i;
            }

            List<double> knots = nurbsCurve.Knots;
            knots = knots.GetRange(1, knots.Count - 2);
            i = 0;
            foreach (double knot in knots)
            {
                ghNurbsCurve.Knots[i] = knot;
                ++i;
            }

            double t0 = nurbsCurve.FirstParameter;
            double t1 = nurbsCurve.LastParameter;

            Curve ghTrimmedNurbsCurve = ghNurbsCurve.Trim(t0, t1);

            String log = "";
            if (ghTrimmedNurbsCurve.IsValidWithLog(out log))
            {
                return ghTrimmedNurbsCurve;
            }

            throw new Exception("A valid curve cannot be created from this Edge.");
        }
    }
}
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

using Topologic;

namespace SAM.Geometry.Grasshopper.Topologic
{
    public static partial class Convert
    {
        public static List<Curve> ToRhino(this Wire wire)
        {
            List<Edge> edges = wire.Edges;
            List<Curve> ghOriginalCurves = new List<Curve>();
            foreach (Edge edge in edges)
            {
                Curve ghCurve = ToRhino(edge);
                ghOriginalCurves.Add(ghCurve);
            }

            if (ghOriginalCurves.Count == 0)
            {
                return null;
            }

            List<Curve> ghFinalCurves = Curve.JoinCurves(ghOriginalCurves).ToList();

            List<Curve> ghFinalCurvesAsObjects = new List<Curve>();
            foreach (Curve ghFinalCurve in ghFinalCurves)
            {
                String log = "";
                if (!ghFinalCurve.IsValidWithLog(out log))
                {
                    throw new Exception(log);
                }
                ghFinalCurvesAsObjects.Add(ghFinalCurve);
            }

            return ghFinalCurvesAsObjects;
        }
    }
}
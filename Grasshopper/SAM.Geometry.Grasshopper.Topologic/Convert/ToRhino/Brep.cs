using Rhino.Geometry;
using System;
using System.Collections.Generic;
using Topologic;

namespace SAM.Geometry.Grasshopper.Topologic
{
    public static partial class Convert
    {
        public static Brep ToRhino(global::Topologic.Face face, double tolerance = Core.Tolerance.Distance)
        {
            Rhino.Geometry.Surface ghSurface = ToRhino(face);

            double width = 0.0, height = 0.0;
            bool canGetSurfaceSize = ghSurface.GetSurfaceSize(out width, out height);
            if (!canGetSurfaceSize)
            {
                throw new Exception("Fails to get the surface size.");
            }
            double maxSize = Math.Max(width, height);
            double maxSizeAndMargin = maxSize + 2;
            ghSurface = ghSurface.Extend(IsoStatus.North, maxSizeAndMargin, true);
            ghSurface = ghSurface.Extend(IsoStatus.South, maxSizeAndMargin, true);
            ghSurface = ghSurface.Extend(IsoStatus.West, maxSizeAndMargin, true);
            ghSurface = ghSurface.Extend(IsoStatus.East, maxSizeAndMargin, true);

            List<GeometryBase> ghGeometryBases = new List<GeometryBase>();

            IList<Edge> outerEdges = face.ExternalBoundary.Edges;
            List<Curve> ghCurves = new List<Curve>();
            foreach (Edge edge in outerEdges)
            {
                Curve ghCurve3D = ToRhino(edge);
                ghGeometryBases.Add(ghCurve3D);
                ghCurves.Add(ghCurve3D);
            }

            Brep ghBrep2 = Brep.CreatePatch(
                ghGeometryBases,
                ghSurface,
                20,
                20,
                true,
                true,
                tolerance,
                100.0,
                1,
                new Boolean[] { true, true, true, true },
                tolerance);

            BrepFace ghSurfaceAsBrepFace = ghSurface as BrepFace;

            if (ghBrep2 == null)
            {
                return null;
            }

            IList<Wire> internalBoundaries = face.InternalBoundaries;
            if (internalBoundaries.Count == 0)
            {
                return ghBrep2;
            }

            BrepFace ghBrepFace = ghBrep2.Faces[0];

            List<Curve> ghInternalCurves = new List<Curve>();
            foreach (Wire internalBoundary in internalBoundaries)
            {
                List<Curve> ghCurvesFromWireAsObjects = ToRhino(internalBoundary);
                foreach (Curve ghCurveFromWireAsObject in ghCurvesFromWireAsObjects)
                {
                    Curve ghCurveFromWire = ghCurveFromWireAsObject as Curve;
                    if (ghCurveFromWire != null)
                    {
                        Curve[] ghPulledCurveFromWire = ghCurveFromWire.PullToBrepFace(ghBrepFace, tolerance);
                        ghInternalCurves.AddRange(ghPulledCurveFromWire);
                    }
                }
            }

            Brep ghBrep3 = ghBrepFace.Split(ghInternalCurves, tolerance);
            return ghBrep3.Faces.ExtractFace(0);
        }
    }
}
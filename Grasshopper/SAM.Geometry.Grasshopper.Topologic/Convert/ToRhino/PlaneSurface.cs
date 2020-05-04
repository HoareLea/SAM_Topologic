using Rhino.Geometry;
using System;
using System.Collections.Generic;
using Topologic;

namespace SAM.Geometry.Grasshopper.Topologic
{
    public static partial class Convert
    {
        public static PlaneSurface ToRhino(PlanarSurface planarSurface, global::Topologic.Face face)
        {
            // From Topologic
            List<double> coefficients = planarSurface.Coefficients;
            double a = coefficients[0];
            double b = coefficients[1];
            double c = coefficients[2];
            double d = coefficients[3];
            Vertex faceCenterOfMass = face.CenterOfMass;
            Point3d ghFaceCenterOfMass = ToRhino(faceCenterOfMass);

            Rhino.Geometry.Plane ghPlane = new Rhino.Geometry.Plane(a, b, c, d);

            double occtXMin = planarSurface.XMin;
            double occtXMax = planarSurface.XMax;
            double occtAbsDeltaX = Math.Abs(occtXMax - occtXMin);
            double occtHalfDeltaX = occtAbsDeltaX / 2.0;

            double occtYMin = planarSurface.YMin;
            double occtYMax = planarSurface.YMax;
            double occtAbsDeltaY = Math.Abs(occtYMax - occtYMin);
            double occtHalfDeltaY = occtAbsDeltaY / 2.0;

            double ghXMin = occtXMin;// - occtHalfDeltaX - safetyMarginX;
            double ghXMax = occtXMax;// - occtHalfDeltaX + safetyMarginX;
            double ghYMin = occtYMin;// - occtHalfDeltaY - safetyMarginY;
            double ghYMax = occtYMax;// - occtHalfDeltaY + safetyMarginY;

            Interval xExtents = new Interval(
                ghXMin,
                ghXMax);
            Interval yExtents = new Interval(
                ghYMin,
                ghYMax);

            PlaneSurface ghPlaneSurface = new PlaneSurface(ghPlane, xExtents, yExtents);
            Point3d ghCentroid = Rhino.Geometry.AreaMassProperties.Compute(ghPlaneSurface).Centroid;
            Vector3d ghTranslationVector = ghFaceCenterOfMass - ghCentroid;
            ghPlaneSurface.Translate(ghTranslationVector);
            if (!ghPlaneSurface.IsValid)
            {
                throw new Exception("A valid surface cannot be created from this Face.");
            }

            return ghPlaneSurface;
        }
    }
}
using Rhino.Geometry;
using Rhino.Geometry.Collections;
using System.Collections.Generic;
using System.Linq;
using Topologic;

namespace SAM.Geometry.Grasshopper.Topologic
{
    public static partial class Convert
    {
        public static global::Topologic.Face ToTopologic(this BrepFace brepFace)
        {
            Rhino.Geometry.Surface ghSurface = brepFace?.UnderlyingSurface();
            if (ghSurface == null)
                return null;

            global::Topologic.Face untrimmedFace = ghSurface.ToTopologic();

            BrepLoop ghOuterLoop = brepFace.OuterLoop;
            Wire outerWire = null;
            BrepLoopList ghLoops = brepFace.Loops;
            List<Wire> innerWires = new List<Wire>();
            foreach (BrepLoop ghLoop in ghLoops)
            {
                BrepTrimList ghTrims = ghLoop.Trims;
                List<Edge> trimmingEdges = new List<Edge>();
                foreach (BrepTrim ghTrim in ghTrims)
                {
                    BrepEdge ghEdge = ghTrim.Edge;
                    if (ghEdge == null)
                    {
                        continue;
                        //throw new Exception("An invalid Rhino edge is encountered.");
                    }

                    Topology topology = ghEdge.DuplicateCurve().ToTopologic();

                    // Edge or Wire?
                    Edge trimmingEdge = topology as Edge;
                    if (trimmingEdge != null)
                    {
                        trimmingEdges.Add(trimmingEdge);
                    }

                    Wire partialTrimmingWire = topology as Wire;
                    if (partialTrimmingWire != null)
                    {
                        IList<Edge> partialTrimmingEdges = partialTrimmingWire.Edges;
                        trimmingEdges.AddRange(partialTrimmingEdges);
                    }
                }

                Wire trimmingWire = Wire.ByEdges(trimmingEdges);
                if (ghLoop == ghOuterLoop)
                {
                    outerWire = trimmingWire;
                }
                else
                {
                    innerWires.Add(trimmingWire);
                }
            }

            global::Topologic.Face outerTrimmedFace = global::Topologic.Utilities.FaceUtility.TrimByWire(untrimmedFace, outerWire, true);
            global::Topologic.Face finalFace = outerTrimmedFace.AddInternalBoundaries(innerWires);

            return finalFace;
        }

        public static global::Topologic.Face ToTopologic(this Rhino.Geometry.Surface surface)
        {
            if (surface == null)
                return null;
            
            SumSurface sumSurface = surface as SumSurface;
            if (sumSurface != null)
            {
                return sumSurface.ToTopologic();
            }

            RevSurface revSurface = surface as RevSurface;
            if (revSurface != null)
            {
                return revSurface.ToTopologic();
            }

            PlaneSurface planeSurface = surface as PlaneSurface;
            if (planeSurface != null)
            {
                return planeSurface.ToTopologic();
            }

            Rhino.Geometry.Extrusion ghExtrusion = surface as Rhino.Geometry.Extrusion;
            if (ghExtrusion != null)
            {
                return ghExtrusion.ToTopologic();
            }

            Rhino.Geometry.NurbsSurface ghNurbsSurface = surface as Rhino.Geometry.NurbsSurface;
            if (ghNurbsSurface != null)
            {
                return ghNurbsSurface.ToTopologic();
            }


            return null;
        }

        public static global::Topologic.Face ToTopologic(this SumSurface sumSurface)
        {
            Rhino.Geometry.NurbsSurface ghNurbsSurface = sumSurface?.ToNurbsSurface();
            return ghNurbsSurface?.ToTopologic();
        }

        public static global::Topologic.Face ToTopologic(this Rhino.Geometry.NurbsSurface nurbsSurface)
        {
            if (nurbsSurface == null)
                return null;
            
            int uDegree = nurbsSurface.Degree(0);
            int vDegree = nurbsSurface.Degree(1);
            bool isUPeriodic = nurbsSurface.IsPeriodic(0);
            bool isVPeriodic = nurbsSurface.IsPeriodic(1);
            bool isRational = nurbsSurface.IsRational;
            NurbsSurfaceKnotList ghUKnots = nurbsSurface.KnotsU;
            List<double> uKnots = ghUKnots.ToList();

            NurbsSurfaceKnotList ghVKnots = nurbsSurface.KnotsV;
            List<double> vKnots = ghVKnots.ToList();

            // OCCT-compatible
            uKnots.Insert(0, uKnots[0]);
            uKnots.Add(uKnots.Last());
            vKnots.Insert(0, vKnots[0]);
            vKnots.Add(vKnots.Last());

            NurbsSurfacePointList ghControlPoints = nurbsSurface.Points;
            List<IList<Vertex>> controlPoints = new List<IList<Vertex>>();
            List<IList<double>> weights = new List<IList<double>>();
            for (int i = 0; i < ghControlPoints.CountU; ++i)
            {
                List<Vertex> controlPoints1D = new List<Vertex>();
                List<double> weights1D = new List<double>();
                for (int j = 0; j < ghControlPoints.CountV; ++j)
                {
                    ControlPoint ghControlPoint = ghControlPoints.GetControlPoint(i, j);
                    controlPoints1D.Add(ghControlPoint.Location.ToTopologic());
                    weights1D.Add(ghControlPoint.Weight);
                }
                controlPoints.Add(controlPoints1D);
                weights.Add(weights1D);
            }

            return global::Topologic.Face.ByNurbsParameters(controlPoints, weights, uKnots, vKnots, isRational, isUPeriodic, isVPeriodic, uDegree, vDegree);
        }

        public static global::Topologic.Face ToTopologic(this RevSurface revSurface)
        {
            Rhino.Geometry.NurbsSurface ghNurbsSurface = revSurface?.ToNurbsSurface();
            return ghNurbsSurface?.ToTopologic();
        }

        public static global::Topologic.Face ToTopologic(this PlaneSurface planeSurface)
        {
            Rhino.Geometry.NurbsSurface ghNurbsSurface = planeSurface?.ToNurbsSurface();
            return ghNurbsSurface?.ToTopologic();
        }

        public static global::Topologic.Face ToTopologic(Rhino.Geometry.Extrusion extrusion)
        {
            Rhino.Geometry.NurbsSurface ghNurbsSurface = extrusion?.ToNurbsSurface();
            return ghNurbsSurface?.ToTopologic();
        }
    }
}
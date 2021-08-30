using Rhino.Geometry;
using Rhino.Geometry.Collections;
using System.Collections.Generic;
using System.Linq;
using Topologic;

namespace SAM.Geometry.Grasshopper.Topologic
{
    public static partial class Convert
    {
        public static Edge ToTopologic(this Rhino.Geometry.Line line)
        {
            if (line == null)
            {
                return null;
            }

            Vertex vertex_1 = line.From.ToTopologic();
            if (vertex_1 == null)
            {
                return null;
            }

            Vertex vertex_2 = line.To.ToTopologic();
            if (vertex_2 == null)
            {
                return null;
            }
                
            if(vertex_1 == vertex_2)
            {
                return null;
            }

            if(vertex_1.Coordinates[0] == vertex_2.Coordinates[0] && 
                vertex_1.Coordinates[1] == vertex_2.Coordinates[1] && 
                vertex_1.Coordinates[2] == vertex_2.Coordinates[2])
            {
                return null;
            }

            return Edge.ByStartVertexEndVertex(vertex_1, vertex_2);
        }

        public static Edge ToTopologic(this Rhino.Geometry.NurbsCurve nurbsCurve)
        {
            if (nurbsCurve == null)
                return null;
            
            int degree = nurbsCurve.Degree;
            bool isPeriodic = nurbsCurve.IsPeriodic;
            bool isRational = nurbsCurve.IsRational;
            List<double> knots = nurbsCurve.Knots.ToList();

            knots.Insert(0, knots[0]);
            knots.Add(knots.Last());

            NurbsCurvePointList ghControlPoints = nurbsCurve.Points;
            List<Vertex> controlPoints = new List<Vertex>();
            List<double> weights = new List<double>();
            for (int i = 0; i < ghControlPoints.Count; ++i)
            {
                controlPoints.Add(ghControlPoints[i].Location.ToTopologic());
                weights.Add(ghControlPoints[i].Weight);
            }

            return Edge.ByNurbsParameters(controlPoints, weights, knots, isRational, isPeriodic, degree);
        }

        public static Edge ToTopologic(this ArcCurve arcCurve)
        {
            if (arcCurve == null)
                return null;
            
            Rhino.Geometry.NurbsCurve nurbsCurve = arcCurve.ToNurbsCurve();
            return ToTopologic(nurbsCurve);
        }

        public static Edge ToTopologic(this BrepEdge brepEdge)
        {
            if (brepEdge == null)
                return null;
            
            Rhino.Geometry.NurbsCurve nurbsCurve = brepEdge.ToNurbsCurve();
            return ToTopologic(nurbsCurve);
        }
    }
}
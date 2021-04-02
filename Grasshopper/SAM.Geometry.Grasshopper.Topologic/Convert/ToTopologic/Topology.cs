using Rhino.Geometry;
using Rhino.Geometry.Collections;
using System.Collections.Generic;
using Topologic;

namespace SAM.Geometry.Grasshopper.Topologic
{
    public static partial class Convert
    {
        public static Topology ToTopologic(this Box box, double tolerance = Core.Tolerance.Distance)
        {
            return box.ToBrep()?.ToTopologic(tolerance);
        }

        public static Topology ToTopologic(this Brep brep, double tolerance = Core.Tolerance.Distance)
        {
            if (brep == null)
                return null;
            
            BrepFaceList ghBrepFaces = brep.Faces;
            List<global::Topologic.Face> faces = new List<global::Topologic.Face>();
            foreach (BrepFace ghBrepFace in ghBrepFaces)
            {
                global::Topologic.Face face = ghBrepFace.ToTopologic();
                faces.Add(face);
            }

            if (faces.Count == 0)
            {
                return null;
            }
            else if (faces.Count == 1)
            {
                return faces[0];
            }
            else
            {

                global::Topologic.Shell shell = global::Topologic.Shell.ByFaces(faces, tolerance);
                if (brep.IsSolid)
                {
                    Cell cell = Cell.ByShell(shell);
                    return cell;
                }

                return shell;
            }
        }

        public static Topology ToTopologic(this Curve curve)
        {
            if (curve == null)
                return null;
            
            LineCurve lineCurve = curve as LineCurve;
            if (lineCurve != null)
            {
                return lineCurve.Line.ToTopologic();
            }

            Rhino.Geometry.NurbsCurve nurbsCurve = curve as Rhino.Geometry.NurbsCurve;
            if (nurbsCurve != null)
            {
                return nurbsCurve.ToTopologic();
            }

            ArcCurve arcCurve = curve as ArcCurve;
            if (arcCurve != null)
            {
                return arcCurve.ToTopologic();
            }

            BrepEdge brepEdge = curve as BrepEdge;
            if (brepEdge != null)
            {
                return brepEdge.ToTopologic();
            }

            PolylineCurve ghPolylineCurve = curve as PolylineCurve;
            if (ghPolylineCurve != null)
            {
                return ghPolylineCurve.ToTopologic();
            }

            PolyCurve ghPolyCurve = curve as PolyCurve;
            if (ghPolyCurve != null)
            {
                return ghPolyCurve.ToTopologic();
            }

            return null;
        }

        public static Topology ToTopologic(this Mesh mesh)
        {
            if (mesh == null)
                return null;
            
            MeshVertexList ghMeshVertices = mesh.Vertices;
            int ghMeshVertexCount = ghMeshVertices.Count;
            MeshFaceList ghMeshFaces = mesh.Faces;
            int ghMeshFaceCount = ghMeshFaces.Count;

            List<Vertex> vertices = new List<Vertex>();
            for (int i = 0; i < ghMeshVertexCount; ++i)
            {
                Vertex vertex = ghMeshVertices[i].ToTopologic();
                vertices.Add(vertex);
            }

            List<IList<int>> indices2D = new List<IList<int>>();
            for (int i = 0; i < ghMeshFaceCount; ++i)
            {
                MeshFace ghMeshFace = ghMeshFaces[i];
                List<int> indices1D = new List<int>();
                indices1D.Add(ghMeshFace.A);
                indices1D.Add(ghMeshFace.B);
                indices1D.Add(ghMeshFace.C);
                if (ghMeshFace.IsQuad)
                {
                    indices1D.Add(ghMeshFace.D);
                }
                indices1D.Add(ghMeshFace.A);

                indices2D.Add(indices1D);
            }

            IList<Topology> topologies = Topology.ByVerticesIndices(vertices, indices2D);

            Cluster cluster = Cluster.ByTopologies(topologies);
            Topology topology = cluster.SelfMerge();
            return topology;
        }
    }
}
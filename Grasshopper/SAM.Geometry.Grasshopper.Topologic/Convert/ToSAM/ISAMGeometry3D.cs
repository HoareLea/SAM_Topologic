using Topologic;

namespace SAM.Geometry.Grasshopper.Topologic
{
    public static partial class Convert
    {
        public static Spatial.ISAMGeometry3D ToSAM(this Topology topology)
        {
            Vertex vertex = topology as Vertex;
            if (vertex != null)
                return Geometry.Topologic.Convert.ToSAM(vertex);

            Edge edge = topology as Edge;
            if (edge != null)
                return Geometry.Topologic.Convert.ToSAM(edge);

            Wire wire = topology as Wire;
            if (wire != null)
            {
                if(wire.IsClosed)
                    return Geometry.Topologic.Convert.ToSAM_Polygon3D(wire);
                else
                    return Geometry.Topologic.Convert.ToSAM(wire);
            }
                

            global::Topologic.Face face = topology as global::Topologic.Face;
            if (face != null)
                return Geometry.Topologic.Convert.ToSAM(face);

            return null;
        }
    }
}
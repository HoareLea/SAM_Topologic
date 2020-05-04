namespace SAM_TopologicGeometryDynamo
{
    public static class ISAMGeometry3D
    {
        public static SAM.Geometry.Spatial.ISAMGeometry3D ToSAMGeometry(Topologic.Topology topology)
        {
            return SAM.Geometry.Topologic.Convert.ToSAM(topology as dynamic);
        }
    }
}
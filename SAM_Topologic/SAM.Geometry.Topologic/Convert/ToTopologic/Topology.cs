using SAM.Geometry.Spatial;

namespace SAM.Geometry.Topologic
{
    public static partial class Convert
    {
        public static global::Topologic.Topology ToTopologic(this ISAMGeometry3D sAMGeometry)
        {
            if(sAMGeometry is Shell)
            {
                global::Topologic.Cell cell = ((Shell)sAMGeometry).ToTopologic_Cell();
                if (cell != null)
                    return cell;

                return ((Shell)sAMGeometry).ToTopologic();
            }
            
            return ToTopologic(sAMGeometry as dynamic);
        }
    }
}
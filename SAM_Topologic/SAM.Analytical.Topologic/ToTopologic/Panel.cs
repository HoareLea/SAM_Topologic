using Topologic;

namespace SAM.Analytical.Topologic
{
    public static partial class Convert
    {
        public static object ToTopologic(this Panel panel)
        {
            return Face.ByWire(Geometry.Topologic.Convert.ToTopologic(panel.Polygon));
        }
    }
}

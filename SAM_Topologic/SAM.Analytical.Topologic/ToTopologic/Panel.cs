using Topologic;

namespace SAM.Analytical.Topologic
{
    public static partial class Convert
    {
        public static Face ToTopologic(this Panel panel)
        {
            Wire wire = Geometry.Topologic.Convert.ToTopologic(panel.GetClosedPlanar3D() as Geometry.Spatial.ICurvable3D);
            if (wire == null)
                return null;

            return Face.ByWire(wire);

        }
    }
}

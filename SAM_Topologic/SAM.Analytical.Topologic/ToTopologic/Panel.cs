using Topologic;

namespace SAM.Analytical.Topologic
{
    public static partial class Convert
    {
        public static Face ToTopologic(this Panel panel)
        {
            SAM.Geometry.Spatial.PolycurveLoop3D polycurveLoop3D = panel.ToPolycurveLoop();
            if (polycurveLoop3D == null)
                return null;

            Wire wire = Geometry.Topologic.Convert.ToTopologic((Geometry.Spatial.ICurvable3D)polycurveLoop3D);
            if (wire == null)
                return null;

            return Face.ByWire(wire);

        }
    }
}

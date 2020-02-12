using System.Collections.Generic;
using Topologic;

namespace SAM.Analytical.Topologic
{
    public static partial class Convert
    {
        public static Face ToTopologic(this Panel panel)
        {
            if (panel == null)
                return null;

            Geometry.Spatial.Face3D face3D = panel.GetFace3D();
            if (face3D == null)
                return null;


            Wire externalWire = Geometry.Topologic.Convert.ToTopologic(face3D.GetExternalEdge() as Geometry.Spatial.ICurvable3D);
            if (externalWire == null)
                return null;

            List<Wire> internalWires = new List<Wire>();

            List<Geometry.Spatial.IClosedPlanar3D> internalClosedPlanar3Ds = face3D.GetInternalEdges();
            if (internalClosedPlanar3Ds != null && internalClosedPlanar3Ds.Count > 0)
            {
                foreach(Geometry.Spatial.IClosedPlanar3D closedPlanar3D in internalClosedPlanar3Ds)
                {
                    Wire internalWire = Geometry.Topologic.Convert.ToTopologic(closedPlanar3D as Geometry.Spatial.ICurvable3D);
                    if (internalWire != null)
                        internalWires.Add(internalWire);
                }
            }

            return Face.ByExternalInternalBoundaries(externalWire, internalWires);

        }
    }
}

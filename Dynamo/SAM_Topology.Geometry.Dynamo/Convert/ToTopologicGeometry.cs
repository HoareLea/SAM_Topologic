using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAM_Topologic.Geometry.Dynamo
{
    public static partial class Convert
    {
        public static Autodesk.DesignScript.Geometry.Geometry ToTopologicGeometry(object geometry)
        {
            return Autodesk.DesignScript.Geometry.Point.ByCoordinates(100,100);
        }
    }
}

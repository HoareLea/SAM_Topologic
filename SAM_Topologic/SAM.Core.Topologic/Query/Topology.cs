﻿
using Topologic;

namespace SAM.Core.Topologic
{
    public static partial class Query
    {
        public static Topology Topology(this SAMObject sAMObject)
        {
            if (sAMObject == null)
                return null;

            ParameterSet parameterSet = sAMObject.GetParameterSet(typeof(Topology).Assembly);
            if (parameterSet == null)
                return null;

            string text = parameterSet.ToString("Topology");
            if (string.IsNullOrEmpty(text))
                return null;

            string filePath = System.IO.Path.GetTempFileName();
            System.IO.File.WriteAllText(filePath, text);

            Topology result = global::Topologic.Topology.ByImportedBRep(filePath);
            System.IO.File.Delete(filePath);

            return result;
        }
    }
}
using System.Linq;
using UnityEngine;
using VDS.RDF.Query;

public static class Utils {
    public static float AnimSpeedBasedOnTime => Time.deltaTime * 25;

    public static void Log(object source, string prefix, string message) {
        Debug.Log($"{source.GetType()} > {prefix} : {message}");
    }

    public static void Log(object source, string method) {
        Debug.Log($"{source.GetType()} > {method}");
    }

    public static string GetEntityID(SparqlResult result) {
        string r = result.Value("value").ToString().Split('/').Last();
        return r;
    }

    public static string GetLabel(SparqlResult result) {
        string r = result.Value("valueLabel").ToString();
        r = r.Replace("@en", "");
        return r;
    }
}

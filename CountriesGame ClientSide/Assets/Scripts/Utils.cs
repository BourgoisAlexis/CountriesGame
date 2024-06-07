using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
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

    public static Vector2 SizeToParent(this RawImage image, float padding = 0) {
        var parent = image.transform.parent.GetComponent<RectTransform>();
        var imageTransform = image.GetComponent<RectTransform>();
        if (!parent) { return imageTransform.sizeDelta; } //if we don't have a parent, just return our current width;
        padding = 1 - padding;
        float w = 0, h = 0;
        float ratio = image.texture.width / (float)image.texture.height;
        var bounds = new Rect(0, 0, parent.rect.width, parent.rect.height);
        if (Mathf.RoundToInt(imageTransform.eulerAngles.z) % 180 == 90) {
            //Invert the bounds if the image is rotated
            bounds.size = new Vector2(bounds.height, bounds.width);
        }
        //Size by height first
        h = bounds.height * padding;
        w = h * ratio;
        if (w > bounds.width * padding) { //If it doesn't fit, fallback to width;
            w = bounds.width * padding;
            h = w / ratio;
        }
        imageTransform.sizeDelta = new Vector2(w, h);
        return imageTransform.sizeDelta;
    }

    public static bool ActiveForTasks(this GameObject obj) {
        bool active = obj != null && obj.activeInHierarchy;

        if (!active)
            Log(obj, "ActiveForTasks", $"an object is inactive or null ({obj.name})");

        return active;
    }
}

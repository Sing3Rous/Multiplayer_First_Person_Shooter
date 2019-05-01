using UnityEngine;

public class Util
{
    public static void SetLayerRecursively (GameObject obj, int layer)
    {
        if (obj == null)
        {
            return;
        }

        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            if (child == null)
            {
                continue;
            }

            SetLayerRecursively(child.gameObject, layer);
        }
    }
}

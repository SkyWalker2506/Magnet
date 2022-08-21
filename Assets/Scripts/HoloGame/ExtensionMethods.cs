using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class ExtensionMethods
{
    public static void ResetTransformation(this Transform trans)
    {
        trans.position = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = new Vector3(1, 1, 1);
    }

    public static T GetRandom<T>(this List<T> list)
    {
        return list[Random.Range(0,list.Count)];
    }

    public static List<T> GetOnlyFromChildren<T>(this GameObject parent)
    {
        var children = parent.GetComponentsInChildren<T>().ToList();
        var parObj = parent.GetComponent<T>();
        if (parObj!=null)
            children.Remove(parObj);
        return children;
    }
    public static List<Transform> GetChildrenTR(this GameObject parent)
    {
        return parent.GetOnlyFromChildren<Transform>();
    }
    public static List<GameObject> GetChildrenGO(this GameObject parent)
    {
        var children = new List<GameObject>();
        parent.GetChildrenTR().ForEach(tr => children.Add(tr.gameObject));
        return children;
    }

    public static Color RandomColor(this Color color)
    {
       return new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
    }
}
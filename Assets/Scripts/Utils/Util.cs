using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Util : MonoBehaviour
{
    public static GameObject FindChild(GameObject go, string name) 
    {
        if (go == null || string.IsNullOrEmpty(name))
            return null;

        foreach (Transform child in go.transform)
        {
            if (child.name == name)
                return child.gameObject;
        }

        return null;
    }

    public static T GetorAddComponent<T>(GameObject go) where T : Component
    {
        var component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();

        return component;
    }
}

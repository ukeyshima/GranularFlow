using System.Collections.Generic;
using UnityEngine;

namespace GranularFlow.Tools
{
    public static class InterfaceUtility
    {
        public static T FindObjectOfInterface<T>() where T : class
        {
            foreach ( var component in GameObject.FindObjectsOfType<Component>() )
            {
                if (component is T t) return t;
            }
            return null;
        }

        public static List<T> FindObjectsOfInterfaceAll<T>() where T : class
        {
            List<T> tList = new List<T>();
            Component[] components = GameObject.FindObjectsOfType<Component>();
            foreach (var component in components)
            {
                if (component is T t) tList.Add(t);
            }
            return tList;
        }
    }

}
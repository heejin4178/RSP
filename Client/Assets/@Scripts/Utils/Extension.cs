using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utils
{
    public static class Extension
    {
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            return Util.GetOrAddComponent<T>(go);
        }

        // public static void AddUIEvent(this GameObject go, Action<PointerEventData> action,
        //     Define.UIEvent type = Define.UIEvent.Click)
        // {
        //     UI_Base.BindEvent(go, action, type);
        // }
        
        public static bool IsValid(this BaseController bc)
        {
            return bc != null && bc.isActiveAndEnabled;
        }

        public static bool IsValid(this GameObject go)
        {
            return go != null && go.activeSelf;
        }
    }
}
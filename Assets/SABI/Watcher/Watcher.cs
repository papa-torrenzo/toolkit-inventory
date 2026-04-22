namespace SABI
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Object = UnityEngine.Object;

    public static class Watcher
    {
        static Dictionary<string, Func<string>> watchList_string = new();
        static Dictionary<string, Func<Object>> watchList_object = new();
        static Dictionary<string, Func<bool>> watchList_boolean = new();

        public static Dictionary<string, Func<string>> GetWatchList_String() => watchList_string;

        public static Dictionary<string, Func<Object>> GetWatchList_Object() => watchList_object;

        public static Dictionary<string, Func<bool>> GetWatchList_Bool() => watchList_boolean;

        public static readonly HashSet<UnityEngine.Object> attributeSubscribers = new();

        public static void WatchString(string name, Func<string> value)
        {
            if (watchList_string == null)
                watchList_string = new();
            watchList_string[name] = value;
        }

        public static void WatchObject(string name, Func<Object> value)
        {
            if (watchList_object == null)
                watchList_object = new();
            watchList_object[name] = value;
        }

        public static void WatchBoolean(string name, Func<bool> value)
        {
            if (watchList_boolean == null)
                watchList_boolean = new();
            watchList_boolean[name] = value;
        }

        public static void WatchString(Func<string> value)
        {
            WatchString($"NoName[{watchList_string.Count}]", value);
        }

        public static bool IsAnyWatcherAvailable()
        {
            if (watchList_string == null)
                watchList_string = new();
            if (watchList_object == null)
                watchList_object = new();
            if (watchList_boolean == null)
                watchList_boolean = new();
            return watchList_object.Keys.Count > 0
                || watchList_string.Keys.Count > 0
                || watchList_boolean.Count > 0;
        }

        public static void ClearAll()
        {
            watchList_string = new();
            watchList_object = new();
            watchList_boolean = new();
        }
    }
}

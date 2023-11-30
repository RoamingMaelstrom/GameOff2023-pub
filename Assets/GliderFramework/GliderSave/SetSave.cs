using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GliderSave
{
    public static class SetSave
    {
        public static bool Set(SaveObject saveObject, float newValue) => saveObject.Set(newValue);
        public static bool Set(SaveObject saveObject, int newValue) => saveObject.Set(newValue);
        public static bool Set(SaveObject saveObject, string newValue) => saveObject.Set(newValue);

        public static void OverrideSet(SaveObject saveObject, float newValue) => saveObject.OverrideSet(newValue);
        public static void OverrideSet(SaveObject saveObject, int newValue) => saveObject.OverrideSet(newValue);
        public static void OverrideSet(SaveObject saveObject, string newValue) => saveObject.OverrideSet(newValue);
    }
}

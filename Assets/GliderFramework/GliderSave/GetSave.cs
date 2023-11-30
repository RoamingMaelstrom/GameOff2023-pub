using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GliderSave
{
    public static class GetSave
    {
        public static int IntValue(SaveObject saveObject)
        {
            return PlayerPrefs.GetInt(saveObject.GetPrefsName(), 0);
        }

        public static float FloatValue(SaveObject saveObject)
        {
            return PlayerPrefs.GetFloat(saveObject.GetPrefsName(), 0);
        }

        public static string StringValue(SaveObject saveObject)
        {
            return PlayerPrefs.GetString(saveObject.GetPrefsName(), "");
        }

        public static bool Exists(SaveObject saveObject)
        {
            switch (saveObject.saveType)
            {
                case SaveType.INT: return IntSaveExists(saveObject.GetPrefsName());
                case SaveType.FLOAT: return FloatSaveExists(saveObject.GetPrefsName());
                case SaveType.STRING: return StringSaveExists(saveObject.GetPrefsName());
                default: return false;
            }
        }

        private static bool FloatSaveExists(string name) => PlayerPrefs.GetFloat(name,  -123456.654321f) ==  -123456.654321f ? false : true;
        private static bool IntSaveExists(string name) => PlayerPrefs.GetInt(name,  -123456789) ==  -123456789 ? false : true;
        private static bool StringSaveExists(string name) => PlayerPrefs.GetString(name,  "__NOT_EXIST_XTj@") ==  "__NOT_EXIST_XTj@" ? false : true;

    }
}

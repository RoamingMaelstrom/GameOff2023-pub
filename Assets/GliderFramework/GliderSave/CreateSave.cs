using System;
using UnityEngine;

namespace GliderSave
{
    public static class CreateSave
    {

        public static void Create(SaveObject saveObject)
        {
            string savePrefName = saveObject.GetPrefsName();

            switch (saveObject.saveType)
            {
                case SaveType.INT: CreateIntSave(saveObject, savePrefName); break;
                case SaveType.FLOAT: CreateFloatSave(saveObject, savePrefName); break;
                case SaveType.STRING: CreateStringSave(saveObject, savePrefName); break;
                default: Debug.Log(string.Format("Could not create save of type {0}", saveObject.saveType.ToString())); break;
            }
        }

        private static void CreateIntSave(SaveObject saveObject, string savePrefName)
        {
            if (!IntSaveExists(savePrefName)) PlayerPrefs.SetInt(savePrefName, saveObject.intDefaultValue);
        }

        private static void CreateFloatSave(SaveObject saveObject, string savePrefName)
        {
            if (!FloatSaveExists(savePrefName)) PlayerPrefs.SetFloat(savePrefName, saveObject.floatDefaultValue);
        }

        private static void CreateStringSave(SaveObject saveObject, string savePrefName)
        {
            if (!StringSaveExists(savePrefName)) PlayerPrefs.SetString(savePrefName, saveObject.stringDefaultValue);
        }

        private static bool FloatSaveExists(string name) => PlayerPrefs.GetFloat(name,  -123456.654321f) ==  -123456.654321f ? false : true;
        private static bool IntSaveExists(string name) => PlayerPrefs.GetInt(name,  -123456789) ==  -123456789 ? false : true;
        private static bool StringSaveExists(string name) => PlayerPrefs.GetString(name,  "__NOT_EXIST_XTj@") ==  "__NOT_EXIST_XTj@" ? false : true;
    }
}



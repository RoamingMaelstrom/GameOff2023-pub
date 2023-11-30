using UnityEngine;
using UnityEditor;
using GliderSave;

[CustomEditor(typeof(SaveObject)), CanEditMultipleObjects]
public class SaveObjectPropertyDrawer : Editor {

    // Define Property Fields
    public SerializedProperty
        saveName_Prop,
        saveType_Prop,

        intDefaultType_Prop,
        floatDefaultType_Prop,
        stringDefaultType_Prop,

        rangeLower_Prop,
        rangeUpper_Prop,

        saveUpdateRuleNumeric_Prop,
        saveUpdateRuleString_Prop;
    
    void OnEnable () 
    {
        // Setup the SerializedProperties Property Fields
        saveName_Prop = serializedObject.FindProperty("saveName");
        saveType_Prop = serializedObject.FindProperty("saveType");

        intDefaultType_Prop = serializedObject.FindProperty("intDefaultValue");
        floatDefaultType_Prop = serializedObject.FindProperty("floatDefaultValue");
        stringDefaultType_Prop = serializedObject.FindProperty("stringDefaultValue");

        rangeLower_Prop = serializedObject.FindProperty("rangeLower");
        rangeUpper_Prop = serializedObject.FindProperty("rangeUpper");

        saveUpdateRuleNumeric_Prop = serializedObject.FindProperty("saveUpdateRuleNumeric");
        saveUpdateRuleString_Prop = serializedObject.FindProperty("saveUpdateRuleString");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.PropertyField(saveName_Prop);
        EditorGUILayout.PropertyField(saveType_Prop);
        
        
        SaveType saveType = (SaveType)saveType_Prop.enumValueIndex;

        switch (saveType)
        {
            case SaveType.INT: 
                EditorGUILayout.PropertyField(intDefaultType_Prop, new GUIContent("Default Value"));
                EditorGUILayout.PropertyField(saveUpdateRuleNumeric_Prop, new GUIContent("Update Rule"));
                break;
            case SaveType.FLOAT: 
                EditorGUILayout.PropertyField(floatDefaultType_Prop, new GUIContent("Default Value"));
                EditorGUILayout.PropertyField(saveUpdateRuleNumeric_Prop, new GUIContent("Update Rule"));
                break;
            default:                 
                EditorGUILayout.PropertyField(stringDefaultType_Prop, new GUIContent("Default Value"));
                EditorGUILayout.PropertyField(saveUpdateRuleString_Prop, new GUIContent("Update Rule"));
                break;
        }


        if (saveType < SaveType.STRING && (SaveUpdateRuleNumeric)saveUpdateRuleNumeric_Prop.enumValueIndex == SaveUpdateRuleNumeric.IN_RANGE)
        {
            EditorGUILayout.PropertyField(rangeLower_Prop, new GUIContent("Lower Bound"));
            EditorGUILayout.PropertyField(rangeUpper_Prop, new GUIContent("Upper Bound"));
        }
        

        if (GUILayout.Button("Reset To Default Value")) 
        {
            SaveObject saveObject = (SaveObject)target;
            saveObject.ResetToDefault();
            Debug.Log(string.Format("\"{0}\" reset to default value.", saveObject.saveName));
        }

        if (GUILayout.Button("Delete Saved Entry")) 
        {
            SaveObject saveObject = (SaveObject)target;
            ((SaveObject)target).Delete();
            Debug.Log(string.Format("\"{0}\" PlayerPrefs entry deleted.", saveObject.saveName));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
